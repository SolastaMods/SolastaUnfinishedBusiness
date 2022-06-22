using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.LevelUp;

[HarmonyPatch(typeof(RaceSelectionSlot), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RaceSelectionSlot_Refresh
{
    internal static void Postfix(
        RaceSelectionSlot __instance,
        CharacterRaceDefinition raceDefinition,
        int selectedSubRace)
    {
        var gameObject = __instance.subraceCountLabel.gameObject;
        var count = raceDefinition.SubRaces
            .Count(x => !x.GuiPresentation.Hidden);

        if (gameObject.activeSelf)
        {
            __instance.subraceCountLabel.Text = Gui.Format("Stage/&RaceSubraceCountDescription",
                (selectedSubRace + 1).ToString(), count.ToString());
        }
    }
}
