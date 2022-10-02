// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    internal const string CheckGlyphOn = "<color=green><b>x</b></color>";
    internal const string CheckGlyphOff = "<color=red>o</color>"; // #A0A0A0E0
    internal const string CheckGlyphEmpty = " <color=#B8B8B8FF> </color> ";
    internal const string DisclosureGlyphOn = "<color=#C0C0C0FF><b>v</b></color>"; // ▼▲∧⋀
    internal const string DisclosureGlyphOff = "<color=#C0C0C0FF><b>></b></color>"; // ▶▲∨⋁
    internal const string DisclosureGlyphEmpty = " <color=#B8B8B8FF> </color> ";

    // Basic UI Elements (box, div, etc.)
    internal static void Div(float indent = 0, float height = 0, float width = 0)
    {
        Div(FillColor, indent, height, width);
    }
}
