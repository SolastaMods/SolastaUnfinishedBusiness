using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class WieldedConfigurationSelectorPatcher
{
    [HarmonyPatch(typeof(WieldedConfigurationSelector), nameof(WieldedConfigurationSelector.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: do not show warning sign over specialized monk weapons
            var baseIsMonkWeapon =
                typeof(WeaponDescription).GetMethod(nameof(WeaponDescription.IsMonkWeaponOrUnarmed));

            var customIsMonkWeapon =
                typeof(Bind_Patch).GetMethod(nameof(IsMonkWeaponOrUnarmed),
                    BindingFlags.Static | BindingFlags.NonPublic);

            return instructions.ReplaceCalls(baseIsMonkWeapon,
                "WieldedConfigurationSelector.Bind",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, customIsMonkWeapon));
        }

        private static bool IsMonkWeaponOrUnarmed(WeaponDescription description, GuiCharacter guiCharacter)
        {
            return guiCharacter.RulesetCharacter.IsMonkWeaponOrUnarmed(description);
        }

        [UsedImplicitly]
        public static void Postfix(WieldedConfigurationSelector __instance,
            GuiCharacter guiCharacter,
            RulesetWieldedConfiguration configuration)
        {
            var hero = guiCharacter.RulesetCharacterHero;

            AddHandXbowWarning(__instance.mainHandWarning, configuration.MainHandSlot, hero, configuration);
            AddHandXbowWarning(__instance.offHandWarning, configuration.OffHandSlot, hero, configuration);
        }

        private static void AddHandXbowWarning(
            Component warning,
            RulesetInventorySlot slot,
            RulesetCharacterHero hero,
            RulesetWieldedConfiguration configuration)
        {
            if (!warning
                || warning.gameObject.activeSelf
                || !CustomItemsContext.IsHandCrossbowUseInvalid(slot.equipedItem, hero,
                    configuration.MainHandSlot.EquipedItem, configuration.OffHandSlot.EquipedItem))
            {
                return;
            }

            warning.gameObject.SetActive(true);
            warning.GetComponent<GuiTooltip>().Content = "Tooltip/&NoFreeHandToLoadAmmoDescription";
        }
    }
}
