using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using PCRE.NET.Analyzers.Support;

namespace PCRE.NET.Analyzers;

[Generator]
public sealed partial class StaticMatchInterceptorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider
                            .CreateSyntaxProvider(SyntaxPredicate, SyntaxTransform)
                            .WhereNotNullAndInterceptorsEnabled(context)
                            .Collect();

        context.RegisterImplementationSourceOutput(
            source,
            static (context, invocations) =>
            {
                if (invocations.Length == 0)
                    return;

                context.AddSource(
                    "StaticMatchInterceptor.g.cs",
                    Generate(invocations)
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
                location
            );
        }

        return null;
    }

    private static string Generate(ImmutableArray<InvocationModel> invocations)
    {
        // ReSharper disable once InvocationIsSkipped
        OnGenerate();

        var writer = new CodeWriter();
        writer.AppendInterceptorHeader();

        writer.AppendLine(
            """
            namespace PCRE.Generated
            {
                file static class StaticMatchInterceptor
                {
            """
        );

        GenerateInterceptors(writer, invocations);

        writer.AppendLine(
            """
                }
            }
            """
        );

        return writer.ToString();
    }

    private static void GenerateInterceptors(CodeWriter writer, ImmutableArray<InvocationModel> invocations)
    {
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

                foreach (var interceptedMethod in interceptedMethodGroup)
                    writer.Append("        ").AppendInterceptsLocationAttribute(interceptedMethod.Location);

                writer.AppendLine(
                    $"""
                            public static {method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Regex{regexCounter}_Call{callCounter}_{method.ToDisplayString(GeneratorHelpers.ParametersFormat)}
                                => Regex{regexCounter}.{method.Name}({method.Parameters.Where(p => p.Name is not ("pattern" or "options")).Select(p => $"{p.Name}: {p.Name}").Join(", ")});

                    """
                );

                ++callCounter;
            }

            ++regexCounter;
        }
    }

    [SuppressMessage("ReSharper", "PartialMethodWithSinglePart")]
    static partial void OnGenerate();

    private sealed record InvocationModel(
        IMethodSymbol Method,
        string Pattern,
        long Options,
        InterceptableLocation Location
    );
}
