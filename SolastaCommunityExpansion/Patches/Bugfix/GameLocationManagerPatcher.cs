using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.Bugfix;

//prevent some effects from being removed when entering new location
[HarmonyPatch(typeof(GameLocationManager), "StopCharacterEffectsIfRelevant")]
internal static class GameLocationManager_StopCharacterEffectsIfRelevant
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        //
        // BUGFIX: hero keep conditions across locations
        //

        var code = instructions.ToList();

        var removeEffects = code.FindIndex(x =>
            x.opcode == OpCodes.Callvirt && x.operand.ToString().Contains("Terminate"));
        var maybeTerminate = new Action<RulesetEffect, bool, bool>(MaybeTerminate).Method;

        code[removeEffects] = new CodeInstruction(OpCodes.Call, maybeTerminate);
        code.Insert(removeEffects, new CodeInstruction(OpCodes.Ldarg_1));

        return code;
    }

    private static void MaybeTerminate(RulesetEffect effect, bool self, bool willEnterChainedLocation)
    {
        var baseDefinition = effect.SourceDefinition;

        if (baseDefinition != null)
        {
            var skip = baseDefinition.GetFirstSubFeatureOfType<ISKipEffectRemovalOnLocationChange>();

            if (skip != null && skip.Skip(willEnterChainedLocation))
            {
                return;
            }

            var effectDescription = effect.EffectDescription;
            if (willEnterChainedLocation
                && RuleDefinitions.MatchesMagicType(effectDescription, RuleDefinitions.MagicType.SummonsCreature))
            {
                return;
            }
        }

        effect.Terminate(self);
    }
}
