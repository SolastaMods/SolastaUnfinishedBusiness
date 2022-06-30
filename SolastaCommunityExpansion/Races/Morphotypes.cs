using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
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

        /*testing low alpha values - gives a smooth "cartoony" and less reflective look
        //gives a smooth "cartoony" and less reflective look
        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "TestSkin1", "032a90eb-39f4-425d-bf61-57910fce6a81")
            .SetMainColor(new Color()
            {
                r = BodyDecorationColor_Default_00.MainColor.r,
                g = BodyDecorationColor_Default_00.MainColor.g,
                b = BodyDecorationColor_Default_00.MainColor.b,
                a = 0.2f,
            })
            .SetSortOrder(86)
            .AddToDB();

        //looks decidedly more pink
        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "TestSkin2", "032a90eb-39f4-425d-bf61-57910fce6a82")
            .SetMainColor(new Color()
            {
                r = FaceAndSkin_01.MainColor.r,
                g = FaceAndSkin_01.MainColor.g,
                b = FaceAndSkin_01.MainColor.b,
                a = 0.2f,
            })
            .SetSortOrder(87)
            .AddToDB();
        */
    }

    internal static void CreateBrightEyes(List<MorphotypeElementDefinition> brightEyes)
    {
        var brightEyes00 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CE_BrightEyeColor_00",
                "0736115f-5f15-4a01-abd1-7fa069b6278a")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(40)
            .AddToDB();

        var brightEyes01 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CE_BrightEyeColor_01",
                "0736115f-5f15-4a01-abd1-7fa069b6278b")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(41)
            .AddToDB();

        var brightEyes02 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_02, "CE_BrightEyeColor_02",
                "0736115f-5f15-4a01-abd1-7fa069b6278c")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(42)
            .AddToDB();

        var brightEyes03 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CE_BrightEyeColor_03",
                "0736115f-5f15-4a01-abd1-7fa069b6278d")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(43)
            .AddToDB();

        var brightEyes04 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_04, "CE_BrightEyeColor_04",
                "0736115f-5f15-4a01-abd1-7fa069b6278e")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(44)
            .AddToDB();

        var brightEyes05 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_05, "CE_BrightEyeColor_05",
                "0736115f-5f15-4a01-abd1-7fa069b6278f")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(45)
            .AddToDB();

        var brightEyes06 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CE_BrightEyeColor_06",
                "0736115f-5f15-4a01-abd1-7fa069b62780")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(46)
            .AddToDB();

        var brightEyes07 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_07, "CE_BrightEyeColor_07",
                "0736115f-5f15-4a01-abd1-7fa069b62782")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(47)
            .AddToDB();

        var brightEyes08 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_08, "CE_BrightEyeColor_08",
                "0736115f-5f15-4a01-abd1-7fa069b62783")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(48)
            .AddToDB();

        var brightEyes09 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_09, "CE_BrightEyeColor_09",
                "0736115f-5f15-4a01-abd1-7fa069b62784")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(49)
            .AddToDB();

        var brightEyes10 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CE_BrightEyeColor_10",
                "0736115f-5f15-4a01-abd1-7fa069b62785")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(50)
            .AddToDB();

        var brightEyes11 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_11, "CE_BrightEyeColor_11",
                "0736115f-5f15-4a01-abd1-7fa069b62786")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(51)
            .AddToDB();

        var brightEyes12 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CE_BrightEyeColor_12",
                "0736115f-5f15-4a01-abd1-7fa069b62787")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(52)
            .AddToDB();

        var brightEyes13 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_13, "CE_BrightEyeColor_13",
                "0736115f-5f15-4a01-abd1-7fa069b62788")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(53)
            .AddToDB();

        var brightEyes14 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_14, "CE_BrightEyeColor_14",
                "0736115f-5f15-4a01-abd1-7fa069b62789")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(54)
            .AddToDB();

        var brightEyes15 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_15, "CE_BrightEyeColor_15",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7eca")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(55)
            .AddToDB();

        brightEyes.AddRange(
            brightEyes00,
            brightEyes01,
            brightEyes02,
            brightEyes03,
            brightEyes04,
            brightEyes05,
            brightEyes06,
            brightEyes07,
            brightEyes08,
            brightEyes09,
            brightEyes10,
            brightEyes11,
            brightEyes12,
            brightEyes13,
            brightEyes14,
            brightEyes15);
    }

    internal static void CreateGlowingEyes(List<MorphotypeElementDefinition> glowingEyes)
    {
        var glowingEyes00 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CE_GlowingEyeColor_00",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ecb")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_00.MainColor.r * 20,
                g = BodyDecorationColor_SorcererManaPainter_00.MainColor.g * 20,
                b = BodyDecorationColor_SorcererManaPainter_00.MainColor.b * 20,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(56)
            .AddToDB();

        var glowingEyes01 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CE_GlowingEyeColor_01",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ecc")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_01.MainColor.r * 20,
                g = BodyDecorationColor_SorcererManaPainter_01.MainColor.g * 20,
                b = BodyDecorationColor_SorcererManaPainter_01.MainColor.b * 20,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(57)
            .AddToDB();

        var glowingEyes02 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CE_GlowingEyeColor_02",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ecd")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_03.MainColor.r * 20,
                g = BodyDecorationColor_SorcererManaPainter_03.MainColor.g * 20,
                b = BodyDecorationColor_SorcererManaPainter_03.MainColor.b * 20,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(58)
            .AddToDB();

        var glowingEyes03 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CE_GlowingEyeColor_03",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ece")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_06.MainColor.r * 20,
                g = BodyDecorationColor_SorcererManaPainter_06.MainColor.g * 20,
                b = BodyDecorationColor_SorcererManaPainter_06.MainColor.b * 20,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(59)
            .AddToDB();

        var glowingEyes04 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CE_GlowingEyeColor_04",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ecf")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_10.MainColor.r * 20,
                g = BodyDecorationColor_SorcererManaPainter_10.MainColor.g * 20,
                b = BodyDecorationColor_SorcererManaPainter_10.MainColor.b * 20,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(60)
            .AddToDB();

        var glowingEyes05 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CE_GlowingEyeColor_05",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec0")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_12.MainColor.r * 20,
                g = BodyDecorationColor_SorcererManaPainter_12.MainColor.g * 20,
                b = BodyDecorationColor_SorcererManaPainter_12.MainColor.b * 20,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(61)
            .AddToDB();

        var glowingEyes06 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CE_GlowingEyeColor_06",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec1")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_00.MainColor.r * 80,
                g = BodyDecorationColor_SorcererManaPainter_00.MainColor.g * 80,
                b = BodyDecorationColor_SorcererManaPainter_00.MainColor.b * 80,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(62)
            .AddToDB();

        var glowingEyes07 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CE_GlowingEyeColor_07",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec2")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_01.MainColor.r * 80,
                g = BodyDecorationColor_SorcererManaPainter_01.MainColor.g * 80,
                b = BodyDecorationColor_SorcererManaPainter_01.MainColor.b * 80,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(63)
            .AddToDB();

        var glowingEyes08 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CE_GlowingEyeColor_08",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec3")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_03.MainColor.r * 80,
                g = BodyDecorationColor_SorcererManaPainter_03.MainColor.g * 80,
                b = BodyDecorationColor_SorcererManaPainter_03.MainColor.b * 80,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(64)
            .AddToDB();

        var glowingEyes09 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CE_GlowingEyeColor_09",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec4")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_06.MainColor.r * 80,
                g = BodyDecorationColor_SorcererManaPainter_06.MainColor.g * 80,
                b = BodyDecorationColor_SorcererManaPainter_06.MainColor.b * 80,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(65)
            .AddToDB();

        var glowingEyes10 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CE_GlowingEyeColor_10",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec5")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_10.MainColor.r * 80,
                g = BodyDecorationColor_SorcererManaPainter_10.MainColor.g * 80,
                b = BodyDecorationColor_SorcererManaPainter_10.MainColor.b * 80,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(66)
            .AddToDB();

        var glowingEyes11 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CE_GlowingEyeColor_11",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec6")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_12.MainColor.r * 80,
                g = BodyDecorationColor_SorcererManaPainter_12.MainColor.g * 80,
                b = BodyDecorationColor_SorcererManaPainter_12.MainColor.b * 80,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(67)
            .AddToDB();

        var glowingEyes12 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CE_GlowingEyeColor_12",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec7")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_00.MainColor.r * 160,
                g = BodyDecorationColor_SorcererManaPainter_00.MainColor.g * 160,
                b = BodyDecorationColor_SorcererManaPainter_00.MainColor.b * 160,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(68)
            .AddToDB();

        var glowingEyes13 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CE_GlowingEyeColor_13",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec8")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_01.MainColor.r * 160,
                g = BodyDecorationColor_SorcererManaPainter_01.MainColor.g * 160,
                b = BodyDecorationColor_SorcererManaPainter_01.MainColor.b * 160,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(69)
            .AddToDB();

        var glowingEyes14 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CE_GlowingEyeColor_14",
                "aaa4520d-c7c8-4d8c-b1e5-add88efe7ec9")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_03.MainColor.r * 160,
                g = BodyDecorationColor_SorcererManaPainter_03.MainColor.g * 160,
                b = BodyDecorationColor_SorcererManaPainter_03.MainColor.b * 160,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(70)
            .AddToDB();

        var glowingEyes15 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CE_GlowingEyeColor_15",
                "5e7fe208-0c36-4a2f-9ea7-51e2de103360")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_06.MainColor.r * 160,
                g = BodyDecorationColor_SorcererManaPainter_06.MainColor.g * 160,
                b = BodyDecorationColor_SorcererManaPainter_06.MainColor.b * 160,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(71)
            .AddToDB();

        var glowingEyes16 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CE_GlowingEyeColor_16",
                "5e7fe208-0c36-4a2f-9ea7-51e2de103361")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_10.MainColor.r * 160,
                g = BodyDecorationColor_SorcererManaPainter_10.MainColor.g * 160,
                b = BodyDecorationColor_SorcererManaPainter_10.MainColor.b * 160,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(72)
            .AddToDB();

        var glowingEyes17 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CE_GlowingEyeColor_17",
                "5e7fe208-0c36-4a2f-9ea7-51e2de103362")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_12.MainColor.r * 160,
                g = BodyDecorationColor_SorcererManaPainter_12.MainColor.g * 160,
                b = BodyDecorationColor_SorcererManaPainter_12.MainColor.b * 160,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(73)
            .AddToDB();

        glowingEyes.AddRange(
            glowingEyes00,
            glowingEyes01,
            glowingEyes02,
            glowingEyes03,
            glowingEyes04,
            glowingEyes05,
            glowingEyes06,
            glowingEyes07,
            glowingEyes08,
            glowingEyes09,
            glowingEyes10,
            glowingEyes11,
            glowingEyes12,
            glowingEyes13,
            glowingEyes14,
            glowingEyes15,
            glowingEyes16,
            glowingEyes17);
    }
}
