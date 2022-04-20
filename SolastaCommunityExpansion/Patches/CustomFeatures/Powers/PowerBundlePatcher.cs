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
        internal static bool Prefix(SubspellSelectionModal __instance, int index,
            RulesetSpellRepertoire ___spellRepertoire, SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged)
        {
            var masterSpell = __instance.GetField<SpellDefinition>("masterSpell");

            var masterPower = PowerBundleContext.GetPower(masterSpell);


            if (masterPower == null)
            {
                return true;
            }

            if (___spellCastEngaged != null)
            {
                var slotLevel = __instance.GetField<int>("slotLevel");
                ___spellCastEngaged(___spellRepertoire, ___spellRepertoire.KnownSpells[index],
                    slotLevel);
            }
            else
            {
                UsableDeviceFunctionBox.DeviceFunctionEngagedHandler deviceFunctionEngaged =
                    __instance.GetField<UsableDeviceFunctionBox.DeviceFunctionEngagedHandler>("deviceFunctionEngaged");
                if (deviceFunctionEngaged != null)
                    deviceFunctionEngaged(
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
        internal static bool Prefix(SubspellItem __instance, RulesetCharacter caster,
            SpellDefinition spellDefinition, int index, SubspellItem.OnActivateHandler onActivate)
        {
            var power = PowerBundleContext.GetPower(spellDefinition);

            if (power == null)
            {
                return true;
            }

            __instance.SetField("index", index);

            GuiPowerDefinition guiPowerDefinition =
                ServiceRepository.GetService<IGuiWrapperService>().GetGuiPowerDefinition(power.Name);
            __instance.GetField<GuiLabel>("spellTitle").Text = guiPowerDefinition.Title;

            //add info about remaining spell slots if powers consume them
            // var usablePower = caster.GetPowerFromDefinition(power);
            // if (usablePower != null && power.rechargeRate == RuleDefinitions.RechargeRate.SpellSlot)
            // {
            //     var power_info = Helpers.Accessors.getNumberOfSpellsFromRepertoireOfSpecificSlotLevelAndFeature(power.costPerUse, caster, power.spellcastingFeature);
            //     instance.spellTitle.Text += $"   [{power_info.remains}/{power_info.total}]";
            // }
            var tooltip = __instance.GetField<GuiTooltip>("tooltip");
            tooltip.TooltipClass = guiPowerDefinition.TooltipClass;
            tooltip.Content = power.GuiPresentation.Description;
            tooltip.DataProvider = guiPowerDefinition;
            tooltip.Context = caster;
            __instance.SetField("onActivate", onActivate);
            return false;
        }
    }

    [HarmonyPatch(typeof(UsablePowerBox), "OnActivateCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UsablePowerBox_OnActivateCb
    {
        internal static bool Prefix(UsablePowerBox __instance)
        {
            var masterPower = __instance.GetField<RulesetUsablePower>("usablePower").PowerDefinition;

            if (PowerBundleContext.GetBundle(masterPower) == null)
            {
                return true;
            }

            if (__instance.GetField<UsablePowerBox.PowerEngagedHandler>("powerEngaged") == null)
            {
                return true;
            }

            var masterSpell = PowerBundleContext.GetSpell(masterPower);

            SubspellSelectionModal screen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
            var handler = new SpellsByLevelBox.SpellCastEngagedHandler((spellRepertoire, spell, slotLevel) =>
                PowerEngagedHandler(__instance, spell));
            var repertoir = new RulesetSpellRepertoire();
            repertoir.KnownSpells.AddRange(masterSpell.SubspellsList);

            screen.Bind(masterSpell, __instance.GetField<RulesetCharacter>("activator"), repertoir, handler, 0,
                __instance.RectTransform);
            screen.Show();

            return false;
        }

        static void PowerEngagedHandler(UsablePowerBox box, SpellDefinition spell)
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
        internal static bool Prefix(AfterRestActionItem __instance, bool ___executing, Button ___button)
        {
            if (___executing) { return true; }

            var activity = __instance.RestActivityDefinition;

            if (activity.Functor == "UseCustomRestPower" && activity.StringParameter != null)
            {
                var masterPower = PowerBundleContext.GetPower(activity.StringParameter);
                if (masterPower)
                {
                    var masterSpell = PowerBundleContext.GetSpell(masterPower);

                    SubspellSelectionModal screen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
                    var handler = new SpellsByLevelBox.SpellCastEngagedHandler((spellRepertoire, spell, slotLevel) =>
                        PowerEngagedHandler(__instance, spell));
                    var repertoir = new RulesetSpellRepertoire();
                    repertoir.KnownSpells.AddRange(masterSpell.SubspellsList);

                    screen.Bind(masterSpell, __instance.Hero, repertoir, handler, 0, __instance.RectTransform);
                    screen.Show();

                    return false;
                }
            }

            return true;
        }

        static void PowerEngagedHandler(AfterRestActionItem item, SpellDefinition spell)
        {
            item.GetField<Button>("button").interactable = false;

            var power = PowerBundleContext.GetPower(spell).Name;

            ServiceRepository.GetService<IGameRestingService>().ExecuteAsync(ExecuteAsync(item, power), power);
        }

        private static IEnumerator ExecuteAsync(AfterRestActionItem item, string powerName)
        {
            item.SetField("executing", true);
            var parameters = new FunctorParametersDescription {RestingHero = item.Hero, StringParameter = powerName};
            IGameRestingService service = ServiceRepository.GetService<IGameRestingService>();
            yield return ServiceRepository.GetService<IFunctorService>()
                .ExecuteFunctorAsync(item.RestActivityDefinition.Functor, parameters, service);

            yield return null;
            IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();
            IGameLocationCharacterService characterService =
                ServiceRepository.GetService<IGameLocationCharacterService>();
            if (actionService != null && characterService != null)
            {
                bool needsToWait = false;
                do
                {
                    needsToWait = false;
                    foreach (GameLocationCharacter partyCharacter in characterService.PartyCharacters)
                    {
                        if (actionService.IsCharacterActing(partyCharacter))
                        {
                            needsToWait = true;
                            break;
                        }
                    }

                    if (needsToWait)
                        yield return null;
                } while (needsToWait);
            }

            AfterRestActionItem.AfterRestActionTakenHandler afterRestActionTaken = item.AfterRestActionTaken;
            if (afterRestActionTaken != null)
                afterRestActionTaken();
            item.SetField("executing", false);
            Button button = item.GetField<Button>("button");
            if (button != null)
                button.interactable = true;
        }
    }
}
