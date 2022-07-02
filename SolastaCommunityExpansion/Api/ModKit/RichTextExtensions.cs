using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ModKit;

public static class StringExtensions
{
    public static bool Matches(string source, string other)
    {
        if (source == null || other == null)
        {
            return false;
        }

        return source.IndexOf(other, 0, StringComparison.InvariantCultureIgnoreCase) != -1;
    }

    public static string MarkedSubstring(this string source, string other)
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

    private static string Repeat(this string s, int n)
    {
        if (n < 0 || s == null || s.Length == 0)
        {
            return s;
        }

        return new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }

    public static string Indent(this string s, int n)
    {
        return "    ".Repeat(n) + s;
    }
}

public static class RichTextExtensions
{
    // https://docs.unity3d.com/Manual/StyledText.html
    public static string Bold(this string str)
    {
        return $"<b>{str}</b>";
    }

    public static string Color(this string str, string rrggbbaa)
    {
        return $"<color=#{rrggbbaa}>{str}</color>";
    }

    private static string color(this string s, string color)
    {
        return _ = $"<color={color}>{s}</color>";
    }

    public static string White(this string s)
    {
        return _ = s.color("white");
    }

    public static string Grey(this string s)
    {
        return _ = s.color("#A0A0A0FF");
    }

    public static string Red(this string s)
    {
        return _ = s.color("#C04040E0");
    }

    public static string Pink(this string s)
    {
        return _ = s.color("#FFA0A0E0");
    }

    public static string Green(this string s)
    {
        return _ = s.color("#00ff00ff");
    }

    public static string Blue(this string s)
    {
        return _ = s.color("blue");
    }

    public static string Cyan(this string s)
    {
        return _ = s.color("cyan");
    }

    public static string Magenta(this string s)
    {
        return _ = s.color("magenta");
    }

    public static string Yellow(this string s)
    {
        return _ = s.color("yellow");
    }

    public static string Orange(this string s)
    {
        return _ = s.color("orange");
    }

    public static string Italic(this string str)
    {
        return $"<i>{str}</i>";
    }

    public static string ToSentence(this string str)
    {
        return Regex.Replace(str, @"((?<=\p{Ll})\p{Lu})|\p{Lu}(?=\p{Ll})", " $0").TrimStart();
    }
}
