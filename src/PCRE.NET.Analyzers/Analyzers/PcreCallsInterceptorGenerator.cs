using System.Collections.Generic;
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
public sealed class PcreCallsInterceptorGenerator : IIncrementalGenerator
{
    private const string _generatedNamespace = "PCRE.Generated";

    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var invocations = context.SyntaxProvider
                                 .CreateSyntaxProvider(SyntaxPredicate, SyntaxTransform);

        var isEnabled = context.ParseOptionsProvider
                               .Select(static (parseOptions, _) => IsEnabled(parseOptions));

        var pipeline = invocations.Combine(isEnabled)
                                  .SelectMany(static (pair, _) =>
                                  {
                                      var (invocation, isEnabled) = pair;
                                      return isEnabled && invocation is not null
                                          ? ImmutableArray.Create(invocation)
                                          : ImmutableArray<InvocationModel>.Empty;
                                  })
                                  .Collect();

        context.RegisterImplementationSourceOutput(
            pipeline,
            static (context, invocations) =>
            {
                if (invocations.Length == 0)
                    return;

                context.AddSource(
                    "PcreCallsInterceptor.g.cs",
                    new Generator().Generate(invocations)
                );
            }
        );
    }

    private static bool IsEnabled(ParseOptions parseOptions)
    {
        // The generator requires file-local type support, which was added in C# 11.
        if (parseOptions is not CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp11 })
            return false;

        if (!parseOptions.Features.TryGetValue("InterceptorsNamespaces", out var namespaces))
            return false;

        return namespaces.Split(';')
                         .Select(i => i.Trim())
                         .Contains(_generatedNamespace);
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
                    ContainingType:
                    {
                        Name: "PcreRegex",
                        ContainingNamespace: { Name: "PCRE", ContainingNamespace.IsGlobalNamespace: true }
                    }
                } method
            } invocation
            && context.SemanticModel.GetInterceptableLocation(invocationSyntax, cancellationToken) is { } location)
        {
            if (method.IsStatic)
            {
                if (invocation.GetArgument("pattern")?.Value.ConstantValue is { HasValue: true, Value: string pattern }
                    && invocation.GetArgument("options") is var optionsArg and (null or { Value.ConstantValue.HasValue: true }))
                {
                    return new StaticInvocationModel(
                        new PcreMethod(method),
                        location,
                        pattern,
                        optionsArg?.Value.ConstantValue.Value as long? ?? 0,
                        method.Name is "Replace" ? invocation.GetArgument("replacement")?.Value.ConstantValue.Value as string : null
                    );
                }
            }
            else
            {
                if (method.Name is "Replace"
                    && invocation.GetArgument("replacement")?.Value.ConstantValue is { HasValue: true, Value: string replacement })
                {
                    return new InstanceReplaceInvocationModel(
                        new PcreMethod(method),
                        location,
                        replacement
                    );
                }
            }
        }

        return null;
    }

    private class Generator
    {
        private readonly CodeWriter _writer = new();

        public string Generate(ImmutableArray<InvocationModel> invocations)
        {
            _writer.Clear();
            AppendHeader();

            using (_writer.WriteBlock($"namespace {_generatedNamespace}"))
            using (_writer.WriteBlock("file static class PcreCallsInterceptor"))
            {
                GenerateInterceptors(invocations);
            }

            return _writer.ToString();
        }

        private void AppendHeader()
        {
            _writer.AppendLine(
                """
                // <auto-generated>
                //   This file was generated by PCRE.NET to provide additional optimizations.
                //   If it causes issues, you can disable its generation by adding the
                //   following property to a <PropertyGroup /> in the csproj file:
                //   <PcreNetInterceptors>false</PcreNetInterceptors>
                // </auto-generated>

                #nullable enable

                namespace System.Runtime.CompilerServices
                {
                    [global::System.Diagnostics.Conditional("PCRE_GENERATED")]
                    [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
                    file sealed class InterceptsLocationAttribute : global::System.Attribute
                    {
                        public InterceptsLocationAttribute(int version, string data)
                        { }
                    }
                }

                """
            );
        }

        private void AppendInterceptsLocationAttributeLine(InterceptableLocation location)
        {
            _writer.Append(location.GetInterceptsLocationAttributeSyntax())
#if DEBUG
                   .Append(" // ").Append(location.GetDisplayLocation())
#endif
                   .AppendLine();
        }

        private void GenerateInterceptors(ImmutableArray<InvocationModel> invocations)
        {
            var replacementPatterns = new ReplacementPatternSet();

            GenerateStaticCalls(
                replacementPatterns,
                invocations.OfType<StaticInvocationModel>()
            );

            GenerateInstanceReplaceCalls(
                replacementPatterns,
                invocations.OfType<InstanceReplaceInvocationModel>()
            );

            replacementPatterns.AppendFields(_writer);
            replacementPatterns.AppendHelpers(_writer);
        }

        private void GenerateStaticCalls(ReplacementPatternSet replacementPatterns, IEnumerable<StaticInvocationModel> invocations)
        {
            var regexCounter = 0;

            foreach (var regexGroup in invocations.GroupBy(i => (i.Pattern, i.Options)))
            {
                var (pattern, options) = regexGroup.Key;

                _writer.AppendLine(
                    $"""
                    private static global::PCRE.PcreRegex? _regex{regexCounter};
                    private static global::PCRE.PcreRegex Regex{regexCounter} => _regex{regexCounter} ??= new global::PCRE.PcreRegex(
                        {SymbolDisplay.FormatLiteral(pattern, true)},
                        (global::PCRE.PcreOptions){SymbolDisplay.FormatPrimitive(options, false, true)}
                    );

                    """
                );

                var callCounter = 0;

                foreach (var methodGroup in regexGroup.GroupBy(i => i.Method))
                {
                    var method = methodGroup.Key;

                    foreach (var replacementPatternGroup in methodGroup.GroupBy(i => i.ReplacementPattern))
                    {
                        var replacementPattern = replacementPatternGroup.Key;

                        foreach (var interceptedMethod in replacementPatternGroup)
                            AppendInterceptsLocationAttributeLine(interceptedMethod.Location);

                        _writer.Append(
                            $"""
                            public static {method.ReturnType} Regex{regexCounter}_Call{callCounter}_{method.ParametersSignature}
                                => Regex{regexCounter}.{method.Name}(
                            """
                        );

                        foreach (var paramName in method.GetParameterNames())
                        {
                            if (paramName is "pattern" or "options")
                                continue;

                            if (paramName is "replacement" && method.Name is "Replace" && replacementPatterns.GetOrAdd(replacementPattern) is { } replacement)
                            {
                                _writer.Append($"replacementFunc: {replacement.GetReplacementFuncCall()}, ");
                                continue;
                            }

                            _writer.Append($"{paramName}: {paramName}, ");
                        }

                        _writer.TrimComma()
                               .AppendLine(");")
                               .AppendLine();

                        ++callCounter;
                    }
                }

                ++regexCounter;
            }
        }

        private void GenerateInstanceReplaceCalls(ReplacementPatternSet replacementPatterns, IEnumerable<InstanceReplaceInvocationModel> invocations)
        {
            foreach (var replacementPatternGroup in invocations.GroupBy(i => i.ReplacementPattern))
            {
                if (replacementPatterns.GetOrAdd(replacementPatternGroup.Key) is not { } replacement)
                    continue;

                var callCounter = 0;

                foreach (var methodGroup in replacementPatternGroup.GroupBy(i => i.Method))
                {
                    var method = methodGroup.Key;

                    var paramsSignature = method.ParametersSignature;

                    const string prefix = "Replace(";
                    if (!paramsSignature.StartsWith(prefix))
                        continue; // Shouldn't happen

                    paramsSignature = $"(this global::PCRE.PcreRegex regex, {paramsSignature.Substring(prefix.Length)}";

                    foreach (var interceptedMethod in methodGroup)
                        AppendInterceptsLocationAttributeLine(interceptedMethod.Location);

                    _writer.Append(
                        $"""
                        public static {method.ReturnType} Replace{replacement.PatternId}_Call{callCounter}{paramsSignature}
                            => regex.{method.Name}(
                        """
                    );

                    foreach (var paramName in method.GetParameterNames())
                    {
                        if (paramName is "replacement")
                        {
                            _writer.Append($"replacementFunc: {replacement.GetReplacementFuncCall()}, ");
                            continue;
                        }

                        _writer.Append($"{paramName}: {paramName}, ");
                    }

                    _writer.TrimComma()
                           .AppendLine(");")
                           .AppendLine();

                    ++callCounter;
                }
            }
        }
    }

    private readonly record struct PcreMethod(
        string Name,
        string ReturnType,
        string ParametersSignature,
        string ParameterNames
    )
    {
        private static readonly SymbolDisplayFormat _parametersSignatureFormat
            = new(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                memberOptions: SymbolDisplayMemberOptions.IncludeParameters,
                parameterOptions: SymbolDisplayParameterOptions.IncludeName
                                  | SymbolDisplayParameterOptions.IncludeType
                                  | SymbolDisplayParameterOptions.IncludeModifiers,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
                                      | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                                      | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
            );

        public PcreMethod(IMethodSymbol method)
            : this(
                method.Name,
                method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                method.ToDisplayString(_parametersSignatureFormat),
                string.Join(",", method.Parameters.Select(i => i.Name))
            )
        { }

        public string[] GetParameterNames()
            => ParameterNames.Split(',');
    }

    private abstract record InvocationModel(
        PcreMethod Method,
        InterceptableLocation Location
    );

    private sealed record StaticInvocationModel(
        PcreMethod Method,
        InterceptableLocation Location,
        string Pattern,
        long Options,
        string? ReplacementPattern
    ) : InvocationModel(Method, Location);

    private sealed record InstanceReplaceInvocationModel(
        PcreMethod Method,
        InterceptableLocation Location,
        string ReplacementPattern
    ) : InvocationModel(Method, Location);
}
