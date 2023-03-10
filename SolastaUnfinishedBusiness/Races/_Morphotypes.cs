using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MorphotypeElementDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class Morphotypes
{
    internal static void Load()
    {
        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin1")
            .SetMainColor(BodyDecorationColor_Default_00.MainColor)
            .SetSortOrder(48)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin2")
            .SetMainColor(new Color { r = 0.129411765f, g = 0.188235294f, b = 0.239215686f, a = 1.0f })
            .SetSortOrder(49)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin3")
            .SetMainColor(new Color { r = 0.188235294f, g = 0.258823529f, b = 0.317647059f, a = 1.0f })
            .SetSortOrder(50)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin4")
            .SetMainColor(new Color { r = 0.266666667f, g = 0.360784314f, b = 0.439215687f, a = 1.0f })
            .SetSortOrder(51)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin5")
            .SetMainColor(new Color { r = 0.164705882f, g = 0.184313725f, b = 0.239215686f, a = 1.0f })
            .SetSortOrder(52)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "DarkelfSkin6")
            .SetMainColor(new Color { r = 0.054901961f, g = 0.007843137f, b = 0.015686274f, a = 1.0f })
            .SetSortOrder(53)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair1")
            .SetMainColor(FaceAndSkin_Neutral.MainColor)
            .SetSortOrder(48)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair2")
            .SetMainColor(new Color { r = 0.945098039f, g = 0.952941176f, b = 0.980392157f, a = 1.0f })
            .SetSortOrder(49)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair3")
            .SetMainColor(new Color { r = 0.890196078f, g = 0.933333333f, b = 0.976470588f, a = 1.0f })
            .SetSortOrder(50)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair4")
            .SetMainColor(new Color { r = 0.870588235f, g = 0.894117647f, b = 0.925490196f, a = 1.0f })
            .SetSortOrder(51)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair5")
            .SetMainColor(new Color { r = 0.968627451f, g = 0.929411765f, b = 0.937254902f, a = 1.0f })
            .SetSortOrder(52)
            .AddToDB();

        _ = MorphotypeElementDefinitionBuilder
            .Create(HairColorSilver, "DarkelfHair6")
            .SetMainColor(new Color { r = 0.996078431f, g = 0.913725490f, b = 0.913725490f, a = 1.0f })
            .SetSortOrder(53)
            .AddToDB();

        /*testing low alpha values - gives a smooth "cartoony" and less reflective look
        //gives a smooth "cartoony" and less reflective look
        _ = MorphotypeElementDefinitionBuilder
            .Create(FaceAndSkin_01, "TestSkin1")
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
            .Create(FaceAndSkin_01, "TestSkin2")
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

    internal static void CreateBrightEyes([NotNull] List<MorphotypeElementDefinition> brightEyes)
    {
        var brightEyes00 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CEBrightEyeColor00")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(40)
            .AddToDB();

        var brightEyes01 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CEBrightEyeColor01")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(41)
            .AddToDB();

        var brightEyes02 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_02, "CEBrightEyeColor02")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(42)
            .AddToDB();

        var brightEyes03 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CEBrightEyeColor03")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(43)
            .AddToDB();

        var brightEyes04 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_04, "CEBrightEyeColor04")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(44)
            .AddToDB();

        var brightEyes05 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_05, "CEBrightEyeColor05")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(45)
            .AddToDB();

        var brightEyes06 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CEBrightEyeColor06")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(46)
            .AddToDB();

        var brightEyes07 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_07, "CEBrightEyeColor07")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(47)
            .AddToDB();

        var brightEyes08 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_08, "CEBrightEyeColor08")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(48)
            .AddToDB();

        var brightEyes09 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_09, "CEBrightEyeColor09")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(49)
            .AddToDB();

        var brightEyes10 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CEBrightEyeColor10")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(50)
            .AddToDB();

        var brightEyes11 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_11, "CEBrightEyeColor11")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(51)
            .AddToDB();

        var brightEyes12 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CEBrightEyeColor12")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(52)
            .AddToDB();

        var brightEyes13 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_13, "CEBrightEyeColor13")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(53)
            .AddToDB();

        var brightEyes14 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_14, "CEBrightEyeColor14")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(54)
            .AddToDB();

        var brightEyes15 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_15, "CEBrightEyeColor15")
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

    internal static void CreateGlowingEyes([NotNull] List<MorphotypeElementDefinition> glowingEyes)
    {
        var glowingEyes00 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CEGlowingEyeColor00")
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
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CEGlowingEyeColor01")
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
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CEGlowingEyeColor02")
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
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CEGlowingEyeColor03")
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
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CEGlowingEyeColor04")
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
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CEGlowingEyeColor05")
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
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CEGlowingEyeColor06")
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
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CEGlowingEyeColor07")
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
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CEGlowingEyeColor08")
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
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CEGlowingEyeColor09")
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
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CEGlowingEyeColor10")
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
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CEGlowingEyeColor11")
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
            .Create(BodyDecorationColor_SorcererManaPainter_00, "CEGlowingEyeColor12")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_00.MainColor.r * 240,
                g = BodyDecorationColor_SorcererManaPainter_00.MainColor.g * 240,
                b = BodyDecorationColor_SorcererManaPainter_00.MainColor.b * 240,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(68)
            .AddToDB();

        var glowingEyes13 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_01, "CEGlowingEyeColor13")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_01.MainColor.r * 240,
                g = BodyDecorationColor_SorcererManaPainter_01.MainColor.g * 240,
                b = BodyDecorationColor_SorcererManaPainter_01.MainColor.b * 240,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(69)
            .AddToDB();

        var glowingEyes14 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_03, "CEGlowingEyeColor14")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_03.MainColor.r * 240,
                g = BodyDecorationColor_SorcererManaPainter_03.MainColor.g * 240,
                b = BodyDecorationColor_SorcererManaPainter_03.MainColor.b * 240,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(70)
            .AddToDB();

        var glowingEyes15 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_06, "CEGlowingEyeColor15")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_06.MainColor.r * 240,
                g = BodyDecorationColor_SorcererManaPainter_06.MainColor.g * 240,
                b = BodyDecorationColor_SorcererManaPainter_06.MainColor.b * 240,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(71)
            .AddToDB();

        var glowingEyes16 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_10, "CEGlowingEyeColor16")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_10.MainColor.r * 240,
                g = BodyDecorationColor_SorcererManaPainter_10.MainColor.g * 240,
                b = BodyDecorationColor_SorcererManaPainter_10.MainColor.b * 240,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(72)
            .AddToDB();

        var glowingEyes17 = MorphotypeElementDefinitionBuilder
            .Create(BodyDecorationColor_SorcererManaPainter_12, "CEGlowingEyeColor17")
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = BodyDecorationColor_SorcererManaPainter_12.MainColor.r * 240,
                g = BodyDecorationColor_SorcererManaPainter_12.MainColor.g * 240,
                b = BodyDecorationColor_SorcererManaPainter_12.MainColor.b * 240,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(73)
            .AddToDB();

        var glowingEyes18 = MorphotypeElementDefinitionBuilder
            .Create(EyeColorDefiler, "CEGlowingEyeColor18")
            // don't use SetGuiPresentationNoContentHere as we need a different object for sort order
            .SetGuiPresentation(GuiPresentationBuilder.NoContentTitle, GuiPresentationBuilder.NoContentTitle)
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = EyeColorDefiler.MainColor.r,
                g = EyeColorDefiler.MainColor.g,
                b = EyeColorDefiler.MainColor.b,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(74)
            .AddToDB();

        glowingEyes18.playerSelectable = true;

        var glowingEyes19 = MorphotypeElementDefinitionBuilder
            .Create(EyeColorInfiltrator, "CEGlowingEyeColor19")
            // don't use SetGuiPresentationNoContentHere as we need a different object for sort order
            .SetGuiPresentation(GuiPresentationBuilder.NoContentTitle, GuiPresentationBuilder.NoContentTitle)
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = EyeColorInfiltrator.MainColor.r,
                g = EyeColorInfiltrator.MainColor.g,
                b = EyeColorInfiltrator.MainColor.b,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(75)
            .AddToDB();

        glowingEyes19.playerSelectable = true;

        var glowingEyes20 = MorphotypeElementDefinitionBuilder
            .Create(EyeColorNecromancer, "CEGlowingEyeColor20")
            // don't use SetGuiPresentationNoContentHere as we need a different object for sort order
            .SetGuiPresentation(GuiPresentationBuilder.NoContentTitle, GuiPresentationBuilder.NoContentTitle)
            .SetCategory(MorphotypeElementDefinition.ElementCategory.EyeColor)
            .SetMainColor(new Color
            {
                r = EyeColorNecromancer.MainColor.r,
                g = EyeColorNecromancer.MainColor.g,
                b = EyeColorNecromancer.MainColor.b,
                a = 1.0f
            })
            .SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All)
            .SetSortOrder(76)
            .AddToDB();

        glowingEyes20.playerSelectable = true;

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
            glowingEyes17,
            glowingEyes18,
            glowingEyes19,
            glowingEyes20);
    }
}
