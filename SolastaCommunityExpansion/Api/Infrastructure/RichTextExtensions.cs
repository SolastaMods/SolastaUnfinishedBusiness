// borrowed shamelessly and enhanced from Bag of Tricks https://www.nexusmods.com/pathfinderkingmaker/mods/26, which is under the MIT License

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class RichText
{
    public static string Size(this string s, int size)
    {
        return _ = $"<size={size}>{s}</size>";
    }

    public static string Bold(this string s)
    {
        return _ = $"<b>{s}</b>";
    }

    public static string Italic(this string s)
    {
        return _ = $"<i>{s}</i>";
    }

    public static string Color(this string s, string color)
    {
        return _ = $"<color={color}>{s}</color>";
    }

    public static string White(this string s)
    {
        return s.Color("white");
    }

    public static string Grey(this string s)
    {
        return s.Color("#A0A0A0FF");
    }

    public static string MedGrey(this string s)
    {
        return s.Color("#A8A8A8ff");
    }

    public static string Red(this string s)
    {
        return s.Color("#C04040E0");
    }

    public static string Pink(this string s)
    {
        return s.Color("#FFA0A0E0");
    }

    public static string Green(this string s)
    {
        return s.Color("#00ff00ff");
    }

    public static string Blue(this string s)
    {
        return s.Color("blue");
    }

    public static string Cyan(this string s)
    {
        return s.Color("cyan");
    }

    public static string Magenta(this string s)
    {
        return s.Color("magenta");
    }

    public static string Yellow(this string s)
    {
        return s.Color("yellow");
    }

    public static string Orange(this string s)
    {
        return s.Color("orange");
    }
}
