using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RaceSelectionSlotPatcher
{
    [HarmonyPatch(typeof(RaceSelectionSlot), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] RaceSelectionSlot __instance,
            [NotNull] CharacterRaceDefinition raceDefinition,
            int selectedSubRace)
        {
            //PATCH: Fixes the display if user enables/disables races during level up
            var gameObject = __instance.subraceCountLabel.gameObject;
            var count = raceDefinition.SubRaces.Count(x => !x.GuiPresentation.Hidden);

            if (gameObject.activeSelf)
            {
                __instance.subraceCountLabel.Text = Gui.Format(
                    "Stage/&RaceSubraceCountDescription",
                    (selectedSubRace + 1).ToString(),
                    count.ToString());
            }
        }
    }
}
