using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle
{
    [HarmonyPatch(typeof(SubspellItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellItem_Bind
    {
        internal static bool Prefix(
            SubspellItem __instance,
            RulesetCharacter caster,
            SpellDefinition spellDefinition,
            int index,
            SubspellItem.OnActivateHandler onActivate)
        {
            if (!Main.Settings.EnablePowersBundlePatch)
            {
                return true;
            }

            var power = PowerBundleContext.GetPower(spellDefinition);

            if (power == null)
            {
                return true;
            }

            __instance.index = index;

            var guiPowerDefinition =
                ServiceRepository.GetService<IGuiWrapperService>().GetGuiPowerDefinition(power.Name);
            __instance.spellTitle.Text = guiPowerDefinition.Title;

            //add info about remaining spell slots if powers consume them
            // var usablePower = caster.GetPowerFromDefinition(power);
            // if (usablePower != null && power.rechargeRate == RuleDefinitions.RechargeRate.SpellSlot)
            // {
            //     var power_info = Helpers.Accessors.getNumberOfSpellsFromRepertoireOfSpecificSlotLevelAndFeature(power.costPerUse, caster, power.spellcastingFeature);
            //     instance.spellTitle.Text += $"   [{power_info.remains}/{power_info.total}]";
            // }

            __instance.tooltip.TooltipClass = guiPowerDefinition.TooltipClass;
            __instance.tooltip.Content = power.GuiPresentation.Description;
            __instance.tooltip.DataProvider = guiPowerDefinition;
            __instance.tooltip.Context = caster;

            __instance.onActivate = onActivate;

            return false;
        }
    }
}
