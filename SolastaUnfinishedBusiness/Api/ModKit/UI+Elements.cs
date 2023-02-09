// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    private const string CheckGlyphOn = "<color=green><b>o</b></color>";
    private const string CheckGlyphOff = "<color=#101011><b>o</b></color>";
    private const string DisclosureGlyphOn = "<color=#C0C0C0FF><b>v</b></color>"; // ▼▲
    private const string DisclosureGlyphOff = "<color=#C0C0C0FF><b>></b></color>"; // ▶▲
    private const string DisclosureGlyphEmpty = " <color=#B8B8B8FF> </color> ";

    // Basic UI Elements (box, div, etc.)
    internal static void Div(float indent = 0, float height = 0, float width = 0)
    {
        Div(FillColor, indent, height, width);
    }
}
