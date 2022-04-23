using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_OnActivate
    {
        internal static bool Prefix(
            SubspellSelectionModal __instance,
            int index,
            RulesetSpellRepertoire ___spellRepertoire,
            SpellDefinition ___masterSpell,
            SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged,
            int ___slotLevel,
            UsableDeviceFunctionBox.DeviceFunctionEngagedHandler ___deviceFunctionEngaged
            )
        {
            var masterPower = PowerBundleContext.GetPower(___masterSpell);

            if (masterPower == null)
            {
                return true;
            }

            if (___spellCastEngaged != null)
            {
                ___spellCastEngaged(___spellRepertoire, ___spellRepertoire.KnownSpells[index], ___slotLevel);
            }
            else
            {
                ___deviceFunctionEngaged?.Invoke(
                    __instance.GetField<GuiCharacter>("guiCharacter"),
                    __instance.GetField<RulesetItemDevice>("rulesetItemDevice"),
                    __instance.GetField<RulesetDeviceFunction>("rulesetDeviceFunction"),
                    0, index
                );
            }

            __instance.Hide();
            return false;
        }
    }

    [HarmonyPatch(typeof(SubspellItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellItem_Bind
    {
        internal static bool Prefix(
            SubspellItem __instance, 
            RulesetCharacter caster,
            SpellDefinition spellDefinition, 
            int index, 
            SubspellItem.OnActivateHandler onActivate,
            GuiLabel ___spellTitle,
            GuiTooltip ___tooltip
            )
        {
            var power = PowerBundleContext.GetPower(spellDefinition);

            if (power == null)
            {
                return true;
            }

            __instance.SetField("index", index);

            GuiPowerDefinition guiPowerDefinition = ServiceRepository.GetService<IGuiWrapperService>().GetGuiPowerDefinition(power.Name);
            ___spellTitle.Text = guiPowerDefinition.Title;

            //add info about remaining spell slots if powers consume them
            // var usablePower = caster.GetPowerFromDefinition(power);
            // if (usablePower != null && power.rechargeRate == RuleDefinitions.RechargeRate.SpellSlot)
            // {
            //     var power_info = Helpers.Accessors.getNumberOfSpellsFromRepertoireOfSpecificSlotLevelAndFeature(power.costPerUse, caster, power.spellcastingFeature);
            //     instance.spellTitle.Text += $"   [{power_info.remains}/{power_info.total}]";
            // }

            ___tooltip.TooltipClass = guiPowerDefinition.TooltipClass;
            ___tooltip.Content = power.GuiPresentation.Description;
            ___tooltip.DataProvider = guiPowerDefinition;
            ___tooltip.Context = caster;

            __instance.SetField("onActivate", onActivate);

            return false;
        }
    }

    [HarmonyPatch(typeof(UsablePowerBox), "OnActivateCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UsablePowerBox_OnActivateCb
    {
        internal static bool Prefix(
            UsablePowerBox __instance,
            RulesetUsablePower ___usablePower,
            UsablePowerBox.PowerEngagedHandler ___powerEngaged,
            RulesetCharacter ___activator)
        {
            var masterPower = ___usablePower.PowerDefinition;

            if (PowerBundleContext.GetBundle(masterPower) == null)
            {
                return true;
            }

            if (___powerEngaged == null)
            {
                return true;
            }

            var masterSpell = PowerBundleContext.GetSpell(masterPower);
            var repertoire = new RulesetSpellRepertoire();
            var subspellSelectionModalScreen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
            var handler = new SpellsByLevelBox.SpellCastEngagedHandler(
                (spellRepertoire, spell, slotLevel) => PowerEngagedHandler(__instance, spell));

            repertoire.KnownSpells.AddRange(masterSpell.SubspellsList);

            subspellSelectionModalScreen.Bind(masterSpell, ___activator, repertoire, handler, 0, __instance.RectTransform);
            subspellSelectionModalScreen.Show();

            return false;
        }

        private static void PowerEngagedHandler(UsablePowerBox box, SpellDefinition spell)
        {
            var power = PowerBundleContext.GetPower(spell);
            var engagedHandler = box.GetField<UsablePowerBox.PowerEngagedHandler>("powerEngaged");
            var activator = box.GetField<RulesetCharacter>("activator");

            engagedHandler(UsablePowersProvider.Get(power, activator));
        }
    }

    [HarmonyPatch(typeof(AfterRestActionItem), "OnExecuteCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AfterRestActionItem_OnExecuteCb
    {
        internal static bool Prefix(
            AfterRestActionItem __instance, 
            bool ___executing)
        {
            if (___executing)
            { 
                return true; 
            }

            var activity = __instance.RestActivityDefinition;

            if (activity.Functor == "UseCustomRestPower" && activity.StringParameter != null)
            {
                var masterPower = PowerBundleContext.GetPower(activity.StringParameter);

                if (masterPower)
                {
                    var masterSpell = PowerBundleContext.GetSpell(masterPower);
                    var repertoire = new RulesetSpellRepertoire();
                    var subspellSelectionModalScreen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
                    var handler = new SpellsByLevelBox.SpellCastEngagedHandler((spellRepertoire, spell, slotLevel) =>
                        PowerEngagedHandler(__instance, spell));

                    repertoire.KnownSpells.AddRange(masterSpell.SubspellsList);

                    subspellSelectionModalScreen.Bind(masterSpell, __instance.Hero, repertoire, handler, 0, __instance.RectTransform);
                    subspellSelectionModalScreen.Show();

                    return false;
                }
            }

            return true;
        }

        private static void PowerEngagedHandler(AfterRestActionItem item, SpellDefinition spell)
        {
            item.GetField<Button>("button").interactable = false;

            var power = PowerBundleContext.GetPower(spell).Name;

            ServiceRepository.GetService<IGameRestingService>().ExecuteAsync(ExecuteAsync(item, power), power);
        }

        private static IEnumerator ExecuteAsync(AfterRestActionItem item, string powerName)
        {
            item.SetField("executing", true);

            var parameters = new FunctorParametersDescription {RestingHero = item.Hero, StringParameter = powerName};
            var gameRestingService = ServiceRepository.GetService<IGameRestingService>();

            yield return ServiceRepository.GetService<IFunctorService>()
                .ExecuteFunctorAsync(item.RestActivityDefinition.Functor, parameters, gameRestingService);

            yield return null;

            var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            if (gameLocationActionService != null && gameLocationCharacterService != null)
            {
                bool needsToWait;

                do
                {
                    needsToWait = false;
                    foreach (var partyCharacter in gameLocationCharacterService.PartyCharacters)
                    {
                        if (gameLocationActionService.IsCharacterActing(partyCharacter))
                        {
                            needsToWait = true;
                            break;
                        }
                    }

                    if (needsToWait)
                    {
                        yield return null;
                    }

                } while (needsToWait);
            }

            item.AfterRestActionTaken?.Invoke();
            item.SetField("executing", false);

            var button = item.GetField<Button>("button");

            if (button != null)
            {
                button.interactable = true;
            }
        }
    }
}
