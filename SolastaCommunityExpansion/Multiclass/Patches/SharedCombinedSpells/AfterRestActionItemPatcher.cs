using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells
{
    internal static class AfterRestActionItemPatcher
    {
        // gets a context on which power triggered spell slots recovery
        [HarmonyPatch(typeof(AfterRestActionItem), "OnExecuteCb")]
        internal static class RulesetImplementationManagerApplySpellSlotsForm
        {
            internal static void Prefix(AfterRestActionItem __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                RulesetImplementationManagerPatcher.RestActivityInvokerClass = __instance.RestActivityDefinition.Name switch
                {
                    "PowerAlchemistSpellBonusRecovery" => Models.IntegrationContext.TinkererClass,
                    "PowerWizardArcaneRecovery" => Wizard,
                    "PowerCircleLandNaturalRecovery" => Druid,
                    _ => null,
                };
            }
        }
    }
}
