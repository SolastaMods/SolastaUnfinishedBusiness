using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;

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
            //PATCH: Support for marker to make reoccuring effect that happen on turn start happened only on caster's turn

            var validate = new Func<
                RulesetEffect,
                RuleDefinitions.RecurrentEffect,
                ulong
            >(ValidateApplicationOfEffect).Method;

            var method = typeof(RulesetEffect).GetProperty(nameof(RulesetEffect.SourceGuid))!.GetGetMethod();

            return instructions.ReplaceCode(instruction => instruction.Calls(method),
                -1, "GameLocationEnvironmentManagerPatcher.UpdateAffectedCharactersOfGlobalEffects",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, validate));
        }

        private static ulong ValidateApplicationOfEffect(RulesetEffect effect, RuleDefinitions.RecurrentEffect context)
        {
            var guid = effect.SourceGuid;

            if (context != RuleDefinitions.RecurrentEffect.OnTurnStart)
            {
                return guid;
            }

            if (!effect.SourceDefinition.HasSubFeatureOfType<RecurrenceOnlyOnSelfTurn>())
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
}
