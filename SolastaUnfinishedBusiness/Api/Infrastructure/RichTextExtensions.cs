// borrowed shamelessly and enhanced from Bag of Tricks https://www.nexusmods.com/pathfinderkingmaker/mods/26, which is under the MIT License

using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

internal static class RichText
{
    [NotNull]
    internal static string Size(this string s, int size)
    {
        return _ = $"<size={size}>{s}</size>";
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
    internal static string Pink(this string s)
    {
        return s.Color("#FFA0A0E0");
    }

    [NotNull]
    internal static string Green(this string s)
    {
        return s.Color("#00ff00ff");
    }

    [NotNull]
    internal static string Blue(this string s)
    {
        return s.Color("blue");
    }

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

    [NotNull]
    internal static string Yellow(this string s)
    {
        return s.Color("yellow");
    }

    [NotNull]
    internal static string Orange(this string s)
    {
        return s.Color("orange");
    }
}
