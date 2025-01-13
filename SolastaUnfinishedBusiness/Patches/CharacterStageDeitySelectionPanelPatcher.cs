using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageDeitySelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), nameof(CharacterStageDeitySelectionPanel.UpdateRelevance))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateRelevance_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterStageDeitySelectionPanel __instance)
        {
            //PATCH: updates this panel relevance (MULTICLASS)
            if (LevelUpHelper.IsLevelingUp(__instance.currentHero))
            {
                __instance.isRelevant = LevelUpHelper.RequiresDeity(__instance.currentHero);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), nameof(CharacterStageDeitySelectionPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterStageDeitySelectionPanel __instance)
        {
            if (!Main.Settings.EnableClericToLearnDomainAtLevel3 ||
                __instance.selectedDeity < 0 ||
                LevelUpHelper.GetSelectedClass(__instance.currentHero) != Cleric)
            {
                return;
            }

            var deity = __instance.compatibleDeities[__instance.selectedDeity];
            var alignment = DatabaseHelper.GetDefinition<AlignmentDefinition>(deity.Alignment).FormatTitle();
            var domains = Gui.Localize("Screen/&DomainsTitle");
            var label = $"{alignment}\n\n<b><color=#B5D3DE>{domains}</color></b>\n";
            var finalText = deity.subclasses
                .Select(DatabaseHelper.GetDefinition<CharacterSubclassDefinition>)
                .Aggregate(label,
                    (current, subClass) =>
                        current +
                        $"<i><color=#B5F3FE>{subClass.FormatTitle()}</color></i>\n{subClass.FormatDescription()}\n\n");

            __instance.selectedDeityAlignment.Text = finalText;
        }
    }
}
