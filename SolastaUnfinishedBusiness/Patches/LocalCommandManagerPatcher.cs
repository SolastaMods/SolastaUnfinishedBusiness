using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class LocalCommandManagerPatcher
{
    [HarmonyPatch(typeof(LocalCommandManager), nameof(LocalCommandManager.SwitchWeaponConfiguration))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SwitchWeaponConfiguration_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var method = typeof(GameLocationCharacter).GetMethod(nameof(GameLocationCharacter.SpendActionType));
            var custom = new Action<GameLocationCharacter, ActionDefinitions.ActionType>(Check).Method;

            return instructions.ReplaceCalls(method, "LocalCommandManager.SwitchWeaponConfiguration",
                new CodeInstruction(OpCodes.Call, custom));
        }

        private static void Check(GameLocationCharacter character, ActionDefinitions.ActionType type)
        {
            if (!character.RulesetCharacter.HasSubFeatureOfType<FreeWeaponSwitching>())
            {
                character.SpendActionType(type);
            }
        }
    }
}
