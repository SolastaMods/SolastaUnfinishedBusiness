using System;
using System.Text.RegularExpressions;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

internal static class StringExtensions
{
    internal static string SplitCamelCase(this string str)
    {
        return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }

    internal static string LazyManStripXml(this string str)
    {
        return str
            .Replace("<color=#D89555>", string.Empty)
            .Replace("<color=#F0DAA0>", string.Empty)
            .Replace("<color=#ADD8E6>", string.Empty)
            .Replace("<#57BCF4>", "\r\n\t")
            .Replace("<#B5D3DE>", string.Empty)
            .Replace("</color>", string.Empty)
            .Replace("<b>", string.Empty)
            .Replace("<i>", string.Empty)
            .Replace("</b>", string.Empty)
            .Replace("</i>", string.Empty);
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
