// borrowed shamelessly and enhanced from Bag of Tricks https://www.nexusmods.com/pathfinderkingmaker/mods/26, which is under the MIT License

using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

public static class RichText
{
    [NotNull]
    public static string Size(this string s, int size)
    {
        return _ = $"<size={size}>{s}</size>";
    }

    [NotNull]
    public static string Bold(this string s)
    {
        return _ = $"<b>{s}</b>";
    }

    [NotNull]
    public static string Italic(this string s)
    {
        return _ = $"<i>{s}</i>";
    }

    [NotNull]
    public static string Color(this string s, string color)
    {
        return _ = $"<color={color}>{s}</color>";
    }

    [NotNull]
    public static string Khaki(this string s)
    {
        return s.Color("#F0DAA0");
    }

    [NotNull]
    public static string White(this string s)
    {
        return s.Color("white");
    }

    [NotNull]
    public static string Grey(this string s)
    {
        return s.Color("#A0A0A0FF");
    }

    [NotNull]
    public static string MedGrey(this string s)
    {
        return s.Color("#A8A8A8ff");
    }

    [NotNull]
    public static string Red(this string s)
    {
        return s.Color("#C04040E0");
    }

    [NotNull]
    public static string Pink(this string s)
    {
        return s.Color("#FFA0A0E0");
    }

    [NotNull]
    public static string Green(this string s)
    {
        return s.Color("#00ff00ff");
    }

    [NotNull]
    public static string Blue(this string s)
    {
        return s.Color("blue");
    }

    [NotNull]
    public static string Cyan(this string s)
    {
        return s.Color("cyan");
    }

    [NotNull]
    public static string Magenta(this string s)
    {
        return s.Color("magenta");
    }

    [NotNull]
    public static string Yellow(this string s)
    {
        return s.Color("yellow");
    }

    [NotNull]
    public static string Orange(this string s)
    {
        return s.Color("orange");
    }
}
