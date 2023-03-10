using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class RichText
{
    public enum Rgba : uint
    {
        Blue = 0x8080ffff,
        Brown = 0xC09050ff,
        Crimson = 0x7b0340ff,
        Cyan = 0x00ffffff,
        Darkblue = 0x0000a0ff,
        Darkgrey = 0x808080ff,
        Darkred = 0xa0333bff,
        Green = 0x40C040ff,
        Gold = 0xED9B1Aff,
        Lightblue = 0xd8e6ff,
        Lightgrey = 0xE8E8E8ff,
        Lime = 0x40ff40ff,
        Magenta = 0xff40ffff,
        Maroon = 0xFF6060ff,
        MedRed = 0xd03333ff,
        Navy = 0x3b5681ff,
        Olive = 0xb0b000ff,
        Orange = 0xffa500ff,
        Darkorange = 0xb1521fff,
        Pink = 0xf03399ff,
        Purple = 0xC060F0ff,
        Red = 0xFF4040ff,
        Black = 0x000000ff,
        MedGrey = 0xA8A8A8ff,
        Grey = 0xC0C0C0ff,
        Silver = 0xD0D0D0ff,
        Teal = 0x80f0c0ff,
        Yellow = 0xffff00ff,
        White = 0xffffffff
    }

    [NotNull]
    internal static string Bold(this string s)
    {
        return _ = $"<b>{s}</b>";
    }

    [NotNull]
    internal static string Italic(this string s)
    {
        return _ = $"<i>{s}</i>";
    }

    [NotNull]
    internal static string Color(this string s, string color)
    {
        return _ = $"<color={color}>{s}</color>";
    }

    [NotNull]
    internal static string Khaki(this string s)
    {
        return s.Color("#F0DAA0");
    }

    [NotNull]
    internal static string White(this string s)
    {
        return s.Color("white");
    }

    [NotNull]
    internal static string Grey(this string s)
    {
        return s.Color("#A0A0A0FF");
    }

    [NotNull]
    internal static string MedGrey(this string s)
    {
        return s.Color("#A8A8A8ff");
    }

    [NotNull]
    internal static string Red(this string s)
    {
        return s.Color("#C04040E0");
    }

    [NotNull]
    internal static string Green(this string s)
    {
        return s.Color("#00ff00ff");
    }

#if false
    [NotNull]
    internal static string Blue(this string s)
    {
        return s.Color("blue");
    }
#endif

    [NotNull]
    internal static string Cyan(this string s)
    {
        return s.Color("cyan");
    }

    [NotNull]
    internal static string Magenta(this string s)
    {
        return s.Color("magenta");
    }

#if false
    [NotNull]
    internal static string Yellow(this string s)
    {
        return s.Color("yellow");
    }
#endif

    [NotNull]
    internal static string Orange(this string s)
    {
        return s.Color("orange");
    }

    public static string WarningLargeRedFormat(this string s)
    {
        return _ = s.Red().Size(16).Bold();
    }

    public static string SizePercent(this string s, int percent)
    {
        return _ = $"<size={percent}%>{s}</size>";
    }

    public static string Localized(this string s, Category category = Category.UI)
    {
        return Gui.Localize($"{category}/&{s}");
    }

    public static string Formatted(this string s, Category category, params object[] args)
    {
        return Gui.Format($"{category}/&{s}", args.Select(a => $"{a}").ToArray());
    }

    public static string ToHtmlString(this Rgba color)
    {
        return $"{color:X}";
    }

    public static string Color(this string str, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{str}</color>";
    }

    public static string Color(this string str, Rgba color)
    {
        return $"<color=#{color:X}>{str}</color>";
    }

    public static string Pink(this string s)
    {
        return _ = s.Color("#FFA0A0E0");
    }

    public static string Blue(this string s)
    {
        return _ = s.Color("blue");
    }

    public static string Yellow(this string s)
    {
        return _ = s.Color("yellow");
    }

    public static string ToSentence(this string str)
    {
        return Regex.Replace(str, @"((?<=\p{Ll})\p{Lu})|\p{Lu}(?=\p{Ll})", " $0").TrimStart();
        //return string.Concat(str.Select(c => char.IsUpper(c) ? " " + c : c.ToString())).TrimStart(' ');
    }

    private static string Size(this string str, int size)
    {
        return $"<size={size}>{str}</size>";
    }
}

public static class ColorUtils
{
    public static Color Color(this RichText.Rgba rga, float adjust = 0)
    {
        var red = ((long)rga >> 24) / 256f;
        var green = (0xFF & ((long)rga >> 16)) / 256f;
        var blue = (0xFF & ((long)rga >> 8)) / 256f;
        var alpha = (0xFF & (long)rga) / 256f;
        var color = new Color(red, green, blue, alpha);

        switch (adjust)
        {
            case < 0:
                color = UnityEngine.Color.Lerp(color, UnityEngine.Color.black, -adjust);
                break;
            case > 0:
                color = UnityEngine.Color.Lerp(color, UnityEngine.Color.white, adjust);
                break;
        }

        return color;
    }
}
