using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    internal static class GameLocationManagerPatcher
    {
        //prevent some effects from being removed when entering new location
        [HarmonyPatch(typeof(GameLocationManager), "StopCharacterEffectsIfRelevant")]
        class GameLocationManager_StopCharacterEffectsIfRelevant
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var removeEffects = codes.FindIndex(x =>
                    x.opcode == OpCodes.Callvirt && x.operand.ToString().Contains("Terminate"));
                var maybeTerminate = new Action<RulesetEffect, bool, bool>(MaybeTerminate).Method;
                codes[removeEffects] = new CodeInstruction(OpCodes.Call, maybeTerminate);
                codes.Insert(removeEffects, new CodeInstruction(OpCodes.Ldarg_1));

                return codes.AsEnumerable();
            }

            static void MaybeTerminate(RulesetEffect effect, bool self, bool willEnterChainedLocation)
            {
                var baseDefinition = effect.SourceDefinition;
                if (baseDefinition != null)
                {
                    var skip = baseDefinition.GetFirstSubFeatureOfType<ISKipEffectRemovalOnLocationChange>();
                    if (skip != null && skip.Skip(willEnterChainedLocation))
                    {
                        return;
                    }
                }

                effect.Terminate(self);
            }
        }
    }
}
