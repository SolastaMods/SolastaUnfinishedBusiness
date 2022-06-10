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
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
            }
        }

        if (Main.Settings.UnlockGlowingEyeColors)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.EyeColor))
            {
                morphotype.playerSelectable = true;
            }
        }

        if (Main.Settings.UnlockEyeStyles)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.Eye))
            {
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
            }
        }

        if (Main.Settings.UnlockAllNpcFaces)
        {
            FaceAndSkin_Defiler.playerSelectable = true;
            FaceAndSkin_Neutral.playerSelectable = true;

            HalfElf.RacePresentation.FemaleFaceShapeOptions.Add("FaceShape_NPC_Princess");
            HalfElf.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_HalfElf_NPC_Bartender");
            Human.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_NPC_TavernGuy");
            Human.RacePresentation.MaleFaceShapeOptions.Add("FaceShape_NPC_TomWorker");

            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.FaceShape &&
                         x != FaceShape_NPC_Aksha))
            {
                morphotype.playerSelectable = true;
            }
        }

        if (Main.Settings.UnlockMarkAndTatoosForAllCharacters)
        {
            foreach (var morphotype in dbMorphotypeElementDefinition.Where(x =>
                         x.Category == MorphotypeElementDefinition.ElementCategory.BodyDecoration))
            {
                morphotype.subClassFilterMask = GraphicsDefinitions.MorphotypeSubclassFilterTag.All;
            }
        }

        if (Main.Settings.AllowUnmarkedSorcerers)
        {
            SorcerousDraconicBloodline.morphotypeSubclassFilterTag = (GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
            SorcerousManaPainter.morphotypeSubclassFilterTag = (GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
            SorcerousChildRift.morphotypeSubclassFilterTag = (GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
            SorcerousHauntedSoul.morphotypeSubclassFilterTag = (GraphicsDefinitions.MorphotypeSubclassFilterTag
                .Default);
        }
    }
}
