using System.Linq;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.MorphotypeElementDefinitions;

namespace SolastaCommunityExpansion.Models;

internal static class FaceUnlockContext
{
    internal static void Load()
    {
        var dbMorphotypeElementDefinition = DatabaseRepository.GetDatabase<MorphotypeElementDefinition>();

        if (Main.Settings.UnlockGlowingColorsForAllMarksAndTatoos)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(
                         x => x.Category == MorphotypeElementDefinition.ElementCategory.BodyDecorationColor &&
                              x.SubclassFilterMask == GraphicsDefinitions.MorphotypeSubclassFilterTag
                                  .SorcererManaPainter))
            {
                morphotype.SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All);
            }
        }

        if (Main.Settings.UnlockGlowingEyeColors)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.EyeColor))
            {
                morphotype.SetPlayerSelectable(true);
            }
        }

        if (Main.Settings.UnlockEyeStyles)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.Eye))
            {
                morphotype.SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All);
            }
        }

        if (Main.Settings.UnlockAllNpcFaces)
        {
            FaceAndSkin_Defiler.SetPlayerSelectable(true);
            FaceAndSkin_Neutral.SetPlayerSelectable(true);

            HalfElf.RacePresentation.FemaleFaceShapeOptions.Add("FaceShape_NPC_Princess");
            HalfElf.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_HalfElf_NPC_Bartender");
            Human.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_NPC_TavernGuy");
            Human.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_NPC_TomWorker");

            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.FaceShape &&
                         x != FaceShape_NPC_Aksha))
            {
                morphotype.SetPlayerSelectable(true);
            }
        }

        if (Main.Settings.UnlockMarkAndTatoosForAllCharacters)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.BodyDecoration))
            {
                morphotype.SetSubClassFilterMask(GraphicsDefinitions.MorphotypeSubclassFilterTag.All);
            }
        }

        if (Main.Settings.AllowUnmarkedSorcerers)
        {
            SorcerousDraconicBloodline.SetMorphotypeSubclassFilterTag(GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
            SorcerousManaPainter.SetMorphotypeSubclassFilterTag(GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
            SorcerousChildRift.SetMorphotypeSubclassFilterTag(GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
            SorcerousHauntedSoul.SetMorphotypeSubclassFilterTag(GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
        }
    }
}
