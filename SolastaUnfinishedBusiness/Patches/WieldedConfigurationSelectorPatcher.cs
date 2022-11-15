using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

public static class WieldedConfigurationSelectorPatcher
{
    [HarmonyPatch(typeof(WieldedConfigurationSelector), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: do not show warning sign over bows for Zen Archer monks
            var baseIsMonkWeapon =
                typeof(WeaponDescription).GetMethod(nameof(WeaponDescription.IsMonkWeaponOrUnarmed));

            var customIsMonkWeapon =
                typeof(Bind_Patch).GetMethod(nameof(IsMonkWeapon), BindingFlags.Static | BindingFlags.NonPublic);

            return instructions.ReplaceCalls(baseIsMonkWeapon,
                "WieldedConfigurationSelector.Bind",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, customIsMonkWeapon));
        }

        private static bool IsMonkWeapon(WeaponDescription description, GuiCharacter guiCharacter)
        {
            return WayOfTheDistantHand.IsMonkWeapon(guiCharacter.RulesetCharacter, description);
        }
    }
}
