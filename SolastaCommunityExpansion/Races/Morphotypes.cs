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

        //FaceUnlock
        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CEGlowingEyeColor_00", "0736115f-5f15-4a01-abd1-7fa069b6278a")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(40)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CEGlowingEyeColor_01", "0736115f-5f15-4a01-abd1-7fa069b6278b")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(41)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_02, "CEGlowingEyeColor_02", "0736115f-5f15-4a01-abd1-7fa069b6278c")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(42)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CEGlowingEyeColor_03", "0736115f-5f15-4a01-abd1-7fa069b6278d")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(43)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_04, "CEGlowingEyeColor_04", "0736115f-5f15-4a01-abd1-7fa069b6278e")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(44)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_05, "CEGlowingEyeColor_05", "0736115f-5f15-4a01-abd1-7fa069b6278f")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(45)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CEGlowingEyeColor_06", "0736115f-5f15-4a01-abd1-7fa069b62780")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(46)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_07, "CEGlowingEyeColor_07", "0736115f-5f15-4a01-abd1-7fa069b62782")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(47)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_08, "CEGlowingEyeColor_08", "0736115f-5f15-4a01-abd1-7fa069b62783")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(48)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_09, "CEGlowingEyeColor_09", "0736115f-5f15-4a01-abd1-7fa069b62784")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(49)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CEGlowingEyeColor_10", "0736115f-5f15-4a01-abd1-7fa069b62785")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(50)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_11, "CEGlowingEyeColor_11", "0736115f-5f15-4a01-abd1-7fa069b62786")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(51)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CEGlowingEyeColor_12", "0736115f-5f15-4a01-abd1-7fa069b62787")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(52)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_13, "CEGlowingEyeColor_13", "0736115f-5f15-4a01-abd1-7fa069b62788")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(53)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_14, "CEGlowingEyeColor_14", "0736115f-5f15-4a01-abd1-7fa069b62789")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(54)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_15, "CEGlowingEyeColor_15", "aaa4520d-c7c8-4d8c-b1e5-add88efe7eca")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(55)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CEGlowingEyeColor_16", "aaa4520d-c7c8-4d8c-b1e5-add88efe7ecb")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new UnityEngine.Color()
            {
                r = BodyDecorationColor_SorcererManaPainter_00.MainColor.r * 4,
                g = BodyDecorationColor_SorcererManaPainter_00.MainColor.g * 4,
                b = BodyDecorationColor_SorcererManaPainter_00.MainColor.b * 4,
                a = 1.0f,
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.Default)
            .SetSortOrder(56)
            .AddToDB()
            .GuiPresentation.color = new UnityEngine.Color()
            {
                r = BodyDecorationColor_SorcererManaPainter_00.GuiPresentation.Color.r * 4,
                g = BodyDecorationColor_SorcererManaPainter_00.GuiPresentation.Color.g * 4,
                b = BodyDecorationColor_SorcererManaPainter_00.GuiPresentation.Color.b * 4,
                a = 1.0f,
            };
        
    }
}
