using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    //prevent some effects from being removed when entering new location
    [HarmonyPatch(typeof(GameLocationManager), "StopCharacterEffectsIfRelevant")]
    internal static class GameLocationManager_StopCharacterEffectsIfRelevant
    {
        internal static void Prefix(GameLocationManager __instance, bool willEnterChainedLocation)
        {
            if (willEnterChainedLocation) { return; }

            //remove summoned monsters upon entering new locations, since the game somehow removes corresponding summoned (and all other conditions)
            //from them and thus they are no longer linked to the caster
            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            var gameLocationCharacters = characterService.GuestCharacters.ToArray();
            foreach (var locationCharacter in gameLocationCharacters)
            {
                var rulesetCharacter = locationCharacter.RulesetCharacter;

                var isSummon = rulesetCharacter.ConditionsByCategory.Any(c => c.Value.Any(cc =>
                    cc.ConditionDefinition == DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature));

                var isKindredSpirit = rulesetCharacter is RulesetCharacterMonster monster
                                       && monster.MonsterDefinition.CreatureTags.Contains("KindredSpirit");

                if (isSummon && !isKindredSpirit)
                {
                    characterService.DestroyCharacterBody(locationCharacter);
                }
            }
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();

            if (!Main.Settings.BugFixHeroKeepConditionsAcrossLocations)
            {
                return code;
            }

            var removeEffects = code.FindIndex(x =>
                x.opcode == OpCodes.Callvirt && x.operand.ToString().Contains("Terminate"));
            var maybeTerminate = new Action<RulesetEffect, bool, bool>(MaybeTerminate).Method;

            code[removeEffects] = new CodeInstruction(OpCodes.Call, maybeTerminate);
            code.Insert(removeEffects, new CodeInstruction(OpCodes.Ldarg_1));

            return code;
        }

        internal static void MaybeTerminate(RulesetEffect effect, bool self, bool willEnterChainedLocation)
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
}
