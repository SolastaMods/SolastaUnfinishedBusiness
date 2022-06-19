using SolastaCommunityExpansion.Builders;
using UnityEngine;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MorphotypeElementDefinitions;

namespace SolastaCommunityExpansion.Races;

internal static class Morphotypes
{
    internal static void Load()
    {
        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin1", "d26c8ce0-884f-4abd-90fd-dc961802c48a")
            .SetMainColor(BodyDecorationColor_Default_00.MainColor)
            .SetSortOrder(48)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin2", "d26c8ce0-884f-4abd-90fd-dc961802c48b")
            .SetMainColor(new Color {r = 0.129411765f, g = 0.188235294f, b = 0.239215686f, a = 1.0f})
            .SetSortOrder(49)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin3", "d26c8ce0-884f-4abd-90fd-dc961802c48c")
            .SetMainColor(new Color {r = 0.188235294f, g = 0.258823529f, b = 0.317647059f, a = 1.0f})
            .SetSortOrder(50)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin4", "d26c8ce0-884f-4abd-90fd-dc961802c48d")
            .SetMainColor(new Color {r = 0.266666667f, g = 0.360784314f, b = 0.439215687f, a = 1.0f})
            .SetSortOrder(51)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin5", "d26c8ce0-884f-4abd-90fd-dc961802c48e")
            .SetMainColor(new Color {r = 0.164705882f, g = 0.184313725f, b = 0.239215686f, a = 1.0f})
            .SetSortOrder(52)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin6", "d26c8ce0-884f-4abd-90fd-dc961802c48f")
            .SetMainColor(new Color {r = 0.054901961f, g = 0.007843137f, b = 0.015686274f, a = 1.0f})
            .SetSortOrder(53)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair1", "7dd1a932-69d3-4d51-b9c6-eccaaed90007")
            .SetMainColor(FaceAndSkin_Neutral.MainColor)
            .SetSortOrder(48)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair2", "7dd1a932-69d3-4d51-b9c6-eccaaed90008")
            .SetMainColor(new Color {r = 0.945098039f, g = 0.952941176f, b = 0.980392157f, a = 1.0f})
            .SetSortOrder(49)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair3", "7dd1a932-69d3-4d51-b9c6-eccaaed90009")
            .SetMainColor(new Color {r = 0.890196078f, g = 0.933333333f, b = 0.976470588f, a = 1.0f})
            .SetSortOrder(50)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair4", "7dd1a932-69d3-4d51-b9c6-eccaaed90010")
            .SetMainColor(new Color {r = 0.870588235f, g = 0.894117647f, b = 0.925490196f, a = 1.0f})
            .SetSortOrder(51)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair5", "7dd1a932-69d3-4d51-b9c6-eccaaed90011")
            .SetMainColor(new Color {r = 0.968627451f, g = 0.929411765f, b = 0.937254902f, a = 1.0f})
            .SetSortOrder(52)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair6", "7dd1a932-69d3-4d51-b9c6-eccaaed90012")
            .SetMainColor(new Color {r = 0.996078431f, g = 0.913725490f, b = 0.913725490f, a = 1.0f})
            .SetSortOrder(53)
            .AddToDB();
    }
}
