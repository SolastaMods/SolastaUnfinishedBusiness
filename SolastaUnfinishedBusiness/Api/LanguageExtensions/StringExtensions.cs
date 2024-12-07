using System;
using System.Text.RegularExpressions;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

internal static class StringExtensions
{
    internal static string ReplaceFirst(this string str, string term, string replace)
    {
        var position = str.IndexOf(term, StringComparison.Ordinal);

        if (position < 0)
        {
            return str;
        }

        str = str.Substring(0, position) + replace + str.Substring(position + term.Length);

        return str;
    }

    internal static string SplitCamelCase(this string str)
    {
        return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }

    private static readonly Regex RemoveXmlTags = new(@"<[bci/].*?>|<#B5D3DE>", RegexOptions.Compiled);

    internal static string StripXmlTags(this string str)
    {
        return RemoveXmlTags.Replace(str.Replace("<#57BCF4>", "\r\n\t"), string.Empty);
    }

    internal static bool Matches(this string source, string other)
    {
        if (source == null || other == null)
        {
            return false;
        }

        return source.IndexOf(other, 0, StringComparison.InvariantCultureIgnoreCase) != -1;
    }

    internal static string MarkedSubstring(this string source, string other)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(other))
        {
            return source;
        }

        var index = source.IndexOf(other, StringComparison.InvariantCultureIgnoreCase);

        if (index == -1)
        {
            return source;
        }

        var substr = source.Substring(index, other.Length);

        source = source.Replace(substr, substr.Cyan()).Bold();

        return source;
    }

#if false
    private static string Repeat(this string s, int n)
    {
        if (n < 0 || s == null || s.Length == 0)
        {
            return s;
        }

        return new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }

    internal static string Indent(this string s, int n)
    {
        return "    ".Repeat(n) + s;
    }
#endif
}
