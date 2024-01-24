using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationEnvironmentManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationEnvironmentManager),
        nameof(GameLocationEnvironmentManager.UpdateAffectedCharactersOfGlobalEffects))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateAffectedCharactersOfGlobalEffects_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for marker to make recurring effect that happen on turn start happened only on caster's turn

            var validate = new Func<
                RulesetEffect,
                RecurrentEffect,
                ulong
            >(ValidateApplicationOfEffect).Method;

            var method = typeof(RulesetEffect).GetProperty(nameof(RulesetEffect.SourceGuid))!.GetGetMethod();

            return instructions.ReplaceCode(instruction => instruction.Calls(method),
                -1, "GameLocationEnvironmentManagerPatcher.UpdateAffectedCharactersOfGlobalEffects",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, validate));
        }

        private static ulong ValidateApplicationOfEffect(RulesetEffect effect, RecurrentEffect context)
        {
            var guid = effect.SourceGuid;

            if (context != RecurrentEffect.OnTurnStart)
            {
                return guid;
            }

            if (!effect.SourceDefinition.HasSubFeatureOfType<RestrictRecurrentEffectsOnSelfTurnOnly>())
            {
                return guid;
            }

            var active = Gui.Battle?.activeContender?.RulesetCharacter?.Guid ?? 0;
            if (guid > 0 && guid != active)
            {
                return 0;
            }

            return guid;
        }
    }

    //BUGFIX: removes LOG and TRACE message
    [HarmonyPatch(typeof(GameLocationEnvironmentManager),
        nameof(GameLocationEnvironmentManager.RegisterGlobalActiveEffect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RegisterGlobalActiveEffect_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string)], null);
            var logExceptionMethod = typeof(Trace).GetMethod("LogException", BindingFlags.Public | BindingFlags.Static);

            return instructions
                .ReplaceCalls(
                    logErrorMethod, "GameLocationEnvironmentManager.RegisterGlobalActiveEffect.LogError",
                    new CodeInstruction(OpCodes.Pop))
                .ReplaceCalls(
                    logExceptionMethod, "GameLocationEnvironmentManager.RegisterGlobalActiveEffect.LogException",
                    new CodeInstruction(OpCodes.Pop));
        }
    }
}
