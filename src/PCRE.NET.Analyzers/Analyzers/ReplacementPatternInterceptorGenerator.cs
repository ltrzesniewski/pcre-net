using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace PCRE.Analyzers;

[Generator]
public partial class ReplacementPatternInterceptorGenerator : IIncrementalGenerator
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
                    "ReplacementPatternInterceptor.g.cs",
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

        return methodName is "Replace";
    }

    private static InvocationModel? SyntaxTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is InvocationExpressionSyntax invocationSyntax
            && context.SemanticModel.GetOperation(invocationSyntax, cancellationToken) is IInvocationOperation
            {
                TargetMethod:
                {
                    IsStatic: false,
                    ContainingType.Name: "PcreRegex"
                } method
            } invocation
            && invocation.GetArgument("replacement")?.Value.ConstantValue is { HasValue: true, Value: string replacement }
            && context.SemanticModel.GetInterceptableLocation(invocationSyntax, cancellationToken) is { } location
           )
        {
            return new InvocationModel(
                method,
                replacement,
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
        using (writer.AppendEmbeddedAttributeLine().WriteBlock($"{languageVersion.GeneratedTypeModifier} static class ReplacementPatternInterceptor"))
            GenerateInterceptors(writer, invocations);

        return writer.ToString();
    }

    private static void GenerateInterceptors(CodeWriter writer, ImmutableArray<InvocationModel> invocations)
    {
        var replacementPatterns = new ReplacementPatternSet();

        foreach (var replacementGroup in invocations.GroupBy(i => i.Replacement))
        {
            if (replacementPatterns.GetOrAdd(replacementGroup.Key, out var added) is not { } pattern)
                continue;

            if (added)
                pattern.AppendField(writer);

            var callCounter = 0;

            foreach (var interceptedMethodGroup in replacementGroup.GroupBy(i => i.Method, SymbolEqualityComparer.Default))
            {
                var method = (IMethodSymbol)interceptedMethodGroup.Key!;

                var paramsSignature = method.ToDisplayString(GeneratorHelpers.ParametersFormat);

                const string prefix = "Replace(";
                if (!paramsSignature.StartsWith(prefix))
                    continue; // Shouldn't happen

                paramsSignature = $"(this global::PCRE.PcreRegex regex, {paramsSignature.Substring(prefix.Length)}";

                foreach (var interceptedMethod in interceptedMethodGroup)
                    writer.AppendInterceptsLocationAttributeLine(interceptedMethod.Location);

                var lambdaCall = pattern.GetReplacementFuncCall();

                writer.AppendLine(
                    $"""
                    public static {method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Replace{pattern.PatternId}_Call{callCounter}{paramsSignature}
                        => regex.{method.Name}({method.Parameters.Where(p => p.Name is not "replacement").Select(p => $"{p.Name}: {p.Name}").Join(", ")}, replacementFunc: {lambdaCall});

                    """
                );

                ++callCounter;
            }
        }

        replacementPatterns.AppendHelpers(writer);
    }

    [SuppressMessage("ReSharper", "PartialMethodWithSinglePart")]
    static partial void OnGenerate();

    private sealed record InvocationModel(
        IMethodSymbol Method,
        string Replacement,
        InterceptableLocation Location
    );
}
