using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PCRE.Dfa;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class DocumentationTests
{
    private static Dictionary<string, XElement>? _members;

    [Test]
    public void should_have_correct_version_number_in_nuget_readme()
    {
        var readmeText = File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(DocumentationTests).Assembly.Location)!, "NuGetReadme.md"));
        var match = Regex.Match(readmeText, @"\*\*v(?<libVersion>[\d.]+(?:-\w+)?)\*\* is based on PCRE2 \*\*v(?<pcreVersion>[\d.]+(?:-RC\d+)?)\*\*", RegexOptions.CultureInvariant);

        var assemblyVersion = typeof(PcreRegex).Assembly.GetName().Version!.ToString();

        match.Success.ShouldBeTrue();
        NormalizeVersion(match.Groups["libVersion"].Value).ShouldBe(NormalizeVersion(assemblyVersion));
        NormalizeVersion(match.Groups["pcreVersion"].Value).ShouldBe(NormalizeVersion(PcreBuildInfo.Version.Split(' ')[0]));

        static string NormalizeVersion(string version)
        {
            version = Regex.Replace(version, @"-\w+$", string.Empty);
            version = Regex.Replace(version, @"(?:\.0)+$", string.Empty);
            return version;
        }
    }

    [Test]
    public void should_have_correct_pcre_version_badge_in_readme()
    {
        var readmeText = File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(DocumentationTests).Assembly.Location)!, "README.md"));
        var match = Regex.Match(readmeText, @"https://img\.shields\.io/badge/pcre2-v(?<pcreVersion>[\d.]+(?:--RC\d+)?)-blue\.svg", RegexOptions.CultureInvariant);

        match.Success.ShouldBeTrue();
        NormalizeVersion(match.Groups["pcreVersion"].Value.Replace("--", "-")).ShouldBe(NormalizeVersion(PcreBuildInfo.Version.Split(' ')[0]));

        static string NormalizeVersion(string version)
            => Regex.Replace(version, @"(?:\.0)+$", string.Empty);
    }

    [Test]
    [TestCaseSource(nameof(GetDocumentedMembers))]
    public void should_have_valid_documentation(XElement member)
    {
        if (member.Element("inheritdoc") != null)
        {
            member.Elements("inheritdoc").ShouldHaveSingleItem();
            member.Elements().Where(i => i.Name.LocalName is not ("param" or "inheritdoc")).ShouldBeEmpty();
            return;
        }

        member.Elements("summary").ShouldHaveSingleItem();
        member.Elements("remarks").Count().ShouldBeLessThanOrEqualTo(1);
    }

    [Test]
    [TestCaseSource(nameof(GetMatchMethods))]
    public void should_have_expected_remarks(MethodInfo method)
    {
        var doc = GetMembers()[GetMethodDocId(method)];

        AssertContainsIf(method.IsStatic, "using a static matching method");
        AssertContainsIf(method.DeclaringType == typeof(PcreDfaRegex) && method.Name == nameof(PcreDfaRegex.Match), "The returned result represents all matches starting at the same index.");
        AssertContainsIf(method.DeclaringType == typeof(PcreDfaRegex) && method.Name == nameof(PcreDfaRegex.Matches), "The returned result is a sequence of match results starting at different points in the subject string.");

        AssertContainsIfParam(p => p.Name is "startIndex", "Passing a non-zero");
        AssertContainsIfParam(p => p.Name is "callout" or "onCallout", "(?C<arg>)");
        AssertContainsIfParam(p => p.Name is "replacement" && (p.ParameterType == typeof(string) || p.ParameterType == typeof(ReadOnlySpan<char>)), "supported placeholders");

        AssertXmlContainsIf(method.DeclaringType == typeof(PcreRegex) && method.Name == nameof(PcreRegex.Replace), """<seealso cref="M:PCRE.PcreRegex.Substitute(System.String,System.String)" />""");
        AssertXmlContainsIf(method.DeclaringType == typeof(PcreRegex) && method.Name == nameof(PcreRegex.Substitute), """<seealso cref="M:PCRE.PcreRegex.Replace(System.String,System.String)" />""");

        return;

        void AssertContainsIf(bool condition, string expectedString)
        {
            if (condition)
                doc.Value.ShouldContain(expectedString);
            else
                doc.Value.ShouldNotContain(expectedString);
        }

        void AssertContainsIfParam(Func<ParameterInfo, bool> param, string expectedString)
            => AssertContainsIf(method.GetParameters().Any(param), expectedString);

        void AssertXmlContainsIf(bool condition, string expectedString)
        {
            if (condition)
                doc.ToString().ShouldContain(expectedString);
            else
                doc.ToString().ShouldNotContain(expectedString);
        }
    }

    private static Dictionary<string, XElement> GetMembers()
    {
        if (_members != null)
            return _members;

        var xmlFilePath = Path.ChangeExtension(typeof(PcreRegex).Assembly.Location, ".xml");
        var members = XDocument.Load(xmlFilePath).Root!.Element("members")!.Elements("member");
        var membersDict = members.ToDictionary(i => i.Attribute("name")!.Value, i => i);

        _members ??= membersDict;
        return _members;
    }

    private static IEnumerable<ITestCaseData> GetDocumentedMembers()
        => GetMembers().Select(pair => new TestCaseData(pair.Value).SetCategory("Documentation").SetName(pair.Key));

    private static IEnumerable<ITestCaseData> GetMatchMethods()
        => typeof(PcreRegex).GetMethods()
                            .Concat(typeof(PcreDfaRegex).GetMethods())
                            .Concat(typeof(PcreMatchBuffer).GetMethods())
                            .Where(m => m.Name is nameof(PcreRegex.Match)
                                       or nameof(PcreRegex.IsMatch)
                                       or nameof(PcreRegex.Matches)
                                       or nameof(PcreRegex.Split)
                                       or nameof(PcreRegex.Replace)
                                       or nameof(PcreRegex.Substitute))
                            .Select(m => new TestCaseData(m).SetName("Remarks: " + GetMethodDocId(m)));

    private static string GetMethodDocId(MethodInfo method)
    {
        var sb = new StringBuilder();

        sb.Append("M:");
        AppendDocTypeName(sb, method.DeclaringType!);
        sb.Append('.').Append(method.Name.Replace('.', '#'));

        var parameters = method.GetParameters();
        if (parameters.Length != 0)
        {
            sb.Append('(');

            foreach (var parameter in parameters)
            {
                AppendDocTypeName(sb, parameter.ParameterType);
                sb.Append(',');
            }

            sb[sb.Length - 1] = ')';
        }

        if (method.IsSpecialName && method.Name == "op_Implicit")
        {
            sb.Append('~');
            AppendDocTypeName(sb, method.ReturnType);
        }

        return sb.ToString();
    }

    private static void AppendDocTypeName(StringBuilder sb, Type type)
    {
        if (type.Namespace != null)
            sb.Append(type.Namespace).Append('.');

        var elemType = type.GetElementType() ?? type;
        var elemTypeName = elemType.Name.Replace('+', '.');
        var maxIndex = elemTypeName.IndexOf('`');

        if (maxIndex < 0)
            sb.Append(elemTypeName);
        else
            sb.Append(elemTypeName, 0, maxIndex);

        if (type.IsByRef)
            sb.Append('@');
        else if (type.IsPointer)
            sb.Append('*');

        if (type.IsGenericType)
        {
            sb.Append('{');

            foreach (var genericArg in type.GenericTypeArguments)
            {
                AppendDocTypeName(sb, genericArg);
                sb.Append(',');
            }

            sb[sb.Length - 1] = '}';
        }
    }
}
