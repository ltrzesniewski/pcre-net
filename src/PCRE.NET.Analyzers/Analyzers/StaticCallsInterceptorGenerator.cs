using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace PCRE.Analyzers;

[Generator(LanguageNames.CSharp)]
public sealed partial class StaticCallsInterceptorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var invocations = context.SyntaxProvider
                                 .CreateSyntaxProvider(SyntaxPredicate, SyntaxTransform);

        var isEnabled = context.ParseOptionsProvider
                               .Select(static (parseOptions, _) => parseOptions.Features.TryGetValue("InterceptorsNamespaces", out var value) ? value : string.Empty)
                               .Select(static (namespaces, _) => namespaces.Split(';').Any(static i => i.Trim() == "PCRE.Generated"));

        var languageVersion = context.CompilationProvider
                                     .Select(static (compilation, _) => ((CSharpCompilation)compilation).LanguageVersion);

        var pipeline = invocations.Combine(isEnabled)
                                  .SelectMany(static (pair, _) =>
                                  {
                                      var (invocation, isEnabled) = pair;
                                      return isEnabled && invocation is not null
                                          ? ImmutableArray.Create(invocation)
                                          : ImmutableArray<InvocationModel>.Empty;
                                  })
                                  .Collect()
                                  .Combine(languageVersion);

        context.RegisterImplementationSourceOutput(
            pipeline,
            static (context, pair) =>
            {
                var (invocations, languageVersion) = pair;

                if (invocations.Length == 0)
                    return;

                context.AddSource(
                    "StaticCallsInterceptor.g.cs",
                    Generate(invocations, languageVersion)
                );
            }
        );
    }

    private static bool SyntaxPredicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (!node.IsKind(SyntaxKind.InvocationExpression))
            return false;

        var methodName = (node as InvocationExpressionSyntax)?.Expression switch
        {
            MemberAccessExpressionSyntax i => i.Name.Identifier.ValueText,
            IdentifierNameSyntax i         => i.Identifier.ValueText,
            _                              => null
        };

        return methodName is "Match" or "IsMatch" or "Matches" or "Replace" or "Split" or "Substitute";
    }

    private static InvocationModel? SyntaxTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is InvocationExpressionSyntax invocationSyntax
            && context.SemanticModel.GetOperation(invocationSyntax, cancellationToken) is IInvocationOperation
            {
                TargetMethod:
                {
                    IsStatic: true,
                    ContainingType.Name: "PcreRegex"
                } method
            } invocation
            && invocation.GetArgument("pattern")?.Value.ConstantValue is { HasValue: true, Value: string pattern }
            && invocation.GetArgument("options") is var optionsArg and (null or { Value.ConstantValue.HasValue: true })
            && context.SemanticModel.GetInterceptableLocation(invocationSyntax, cancellationToken) is { } location
           )
        {
            return new InvocationModel(
                method,
                pattern,
                optionsArg?.Value.ConstantValue.Value as long? ?? 0,
                method.Name is "Replace" ? invocation.GetArgument("replacement")?.Value.ConstantValue.Value as string : null,
                location
            );
        }

        return null;
    }

    private static string Generate(ImmutableArray<InvocationModel> invocations, LanguageVersion languageVersion)
    {
        // ReSharper disable once InvocationIsSkipped
        OnGenerate();

        var writer = new CodeWriter();
        writer.AppendInterceptorHeader(languageVersion);

        using (writer.WriteBlock("namespace PCRE.Generated"))
        using (writer.AppendEmbeddedAttributeLine().WriteBlock($"{languageVersion.GeneratedTypeModifier} static class StaticCallsInterceptor"))
            GenerateInterceptors(writer, invocations);

        return writer.ToString();
    }

    private static void GenerateInterceptors(CodeWriter writer, ImmutableArray<InvocationModel> invocations)
    {
        var replacementPatterns = new ReplacementPatternSet();
        var regexCounter = 0;

        foreach (var invocationGroup in invocations.GroupBy(i => (i.Pattern, i.Options)))
        {
            var (pattern, options) = invocationGroup.Key;

            writer.AppendLine(
                $"""
                private static global::PCRE.PcreRegex? _regex{regexCounter};
                private static global::PCRE.PcreRegex Regex{regexCounter} => _regex{regexCounter} ??= new global::PCRE.PcreRegex(
                    {SymbolDisplay.FormatLiteral(pattern, true)},
                    (global::PCRE.PcreOptions){SymbolDisplay.FormatPrimitive(options, false, true)}
                );

                """
            );

            var callCounter = 0;

            foreach (var interceptedMethodGroup in invocationGroup.GroupBy(i => i.Method, SymbolEqualityComparer.Default))
            {
                var method = (IMethodSymbol)interceptedMethodGroup.Key!;

                foreach (var invocationsGroup in interceptedMethodGroup.GroupBy(i => i.ReplacementPattern))
                {
                    var replacementPattern = invocationsGroup.Key;

                    foreach (var interceptedMethod in invocationsGroup)
                        writer.AppendInterceptsLocationAttributeLine(interceptedMethod.Location);

                    writer.Append(
                        $"""
                        public static {method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Regex{regexCounter}_Call{callCounter}_{method.ToDisplayString(GeneratorHelpers.ParametersFormat)}
                            => Regex{regexCounter}.{method.Name}(
                        """
                    );

                    foreach (var param in method.Parameters)
                    {
                        if (param.Name is "pattern" or "options")
                            continue;

                        if (param.Name is "replacement" && method.Name is "Replace" && replacementPatterns.GetOrAdd(replacementPattern, out _) is { } replacement)
                        {
                            writer.Append($"replacementFunc: {replacement.GetReplacementFuncCall()}, ");
                            continue;
                        }

                        writer.Append($"{param.Name}: {param.Name}, ");
                    }

                    writer.TrimComma()
                          .AppendLine(");")
                          .AppendLine();

                    ++callCounter;
                }
            }

            ++regexCounter;
        }

        replacementPatterns.AppendFields(writer);
        replacementPatterns.AppendHelpers(writer);
    }

    [SuppressMessage("ReSharper", "PartialMethodWithSinglePart")]
    static partial void OnGenerate();

    private sealed record InvocationModel(
        IMethodSymbol Method,
        string Pattern,
        long Options,
        string? ReplacementPattern,
        InterceptableLocation Location
    );
}
