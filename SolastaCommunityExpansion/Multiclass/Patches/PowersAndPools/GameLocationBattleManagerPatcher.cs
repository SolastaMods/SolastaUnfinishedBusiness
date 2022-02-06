using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class GameLocationBattleManagerPatcher
    {
        // fixes rage damage calculation under wildshape
        [HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
        internal static class GameLocationBattleManagerComputeAndNotifyAdditionalDamage
        {
            public static bool IsRulesetCharacterHeroOrSubstitute(RulesetCharacter rulesetCharacter)
            {
                return rulesetCharacter is RulesetCharacterHero || rulesetCharacter.IsSubstitute;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var found = 0;
                var isRulesetCharacterHeroOrSubstituteMethod = typeof(GameLocationBattleManagerComputeAndNotifyAdditionalDamage).GetMethod("IsRulesetCharacterHeroOrSubstitute");

                foreach (var instruction in instructions)
                {
                    // at this point RulesetCharacter is on stack so I call IsRulesetCharacterHeroOrSubstitute to determine if it is a hero or a substitute
                    if (instruction.opcode == OpCodes.Isinst && instruction.operand.ToString() == "RulesetCharacterHero" && ++found == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Call, isRulesetCharacterHeroOrSubstituteMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // allows some class feature damages to be correctly calculated under wildshape
        [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
        internal static class GameLocationBattleManagerHandleCharacterAttackDamage
        {
            internal static System.Collections.IEnumerator Postfix(
                System.Collections.IEnumerator values,
                GameLocationBattleManager __instance,
                GameLocationCharacter attacker,
                GameLocationCharacter defender,
                ActionModifier attackModifier,
                RulesetAttackMode attackMode,
                RuleDefinitions.AdvantageType advantageType,
                List<EffectForm> actualEffectForms,
                bool criticalHit)
            {
                // wildshape is a very particular case... cannot afford to process the enumerator with a monster as attacker. it produces a null exception later on at ModHelpers
                if (!Main.Settings.EnableMulticlass || !(attacker.RulesetCharacter is RulesetCharacterMonster monster && monster.IsSubstitute))
                {
                    while (values.MoveNext())
                    {
                        yield return values.Current;
                    };

                    yield break;
                }

                if (!(Gui.GameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == attacker.Name)?.RulesetCharacter is RulesetCharacterHero rulesetCharacterHero))
                {
                    yield break;
                }

                if (attackMode == null || !(defender?.RulesetActor is RulesetCharacterMonster || defender?.RulesetActor is RulesetCharacterHero))
                {
                    yield break;
                }

                var featuresToBrowseReaction = __instance.GetField<GameLocationBattleManager, List<FeatureDefinition>>("featuresToBrowseReaction");
                var gameLocationBattleManagerType = typeof(GameLocationBattleManager);
                var waitForReactionsMethod = gameLocationBattleManagerType.GetMethod("WaitForReactions", BindingFlags.NonPublic | BindingFlags.Instance);
                var computeAndNotifyAdditionalDamageMethod = gameLocationBattleManagerType.GetMethod("ComputeAndNotifyAdditionalDamage", BindingFlags.NonPublic | BindingFlags.Instance);

                attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(featuresToBrowseReaction);

                foreach (var featureDefinition in featuresToBrowseReaction.Where(x => !(x as IAdditionalDamageProvider).AttackModeOnly))
                {
                    var hasAditionalDamage = false;
                    var provider = featureDefinition as IAdditionalDamageProvider;
                    CharacterActionParams reactionParams = null;

                    // bellow code is pretty much official's TA code
                    // only had a chance to test Divine Smite and Rage damage integrations with wildshape. not sure if all other IFs will ever trigger
                    if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
                    {
                        if (advantageType == RuleDefinitions.AdvantageType.Advantage || advantageType != RuleDefinitions.AdvantageType.Disadvantage && __instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker))
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot && attackModifier != null && attackModifier.Proximity == RuleDefinitions.AttackProximity.Melee)
                    {
                        var classDefinition = rulesetCharacterHero.FindClassHoldingFeature(featureDefinition);

                        foreach (var spellRepertoire in rulesetCharacterHero.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellCastingClass == classDefinition)
                            {
                                RulesetSpellRepertoire selectedSpellRepertoire = null;

                                for (var spellLevel = 1; spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel; ++spellLevel)
                                {
                                    spellRepertoire.GetSlotsNumber(spellLevel, out var remaining, out var max);

                                    if (remaining > 0)
                                    {
                                        selectedSpellRepertoire = spellRepertoire;

                                        break;
                                    }
                                }

                                if (selectedSpellRepertoire != null)
                                {
                                    reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendSpellSlot)
                                    {
                                        IntParameter = 1,
                                        StringParameter = provider.NotificationTag,
                                        SpellRepertoire = selectedSpellRepertoire
                                    };

                                    var service = ServiceRepository.GetService<IGameLocationActionService>();
                                    var count = service.PendingReactionRequestGroups.Count;

                                    service.ReactToSpendSpellSlot(reactionParams);

                                    yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, service, count });

                                    hasAditionalDamage = reactionParams.ReactionValidated;
                                }
                            }
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
                    {
                        if (defender.RulesetActor.HasConditionOfTypeAndSource(provider.RequiredTargetCondition, attacker.Guid))
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition)
                    {
                        if (defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
                    {
                        if (!defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded)
                    {
                        if (defender.RulesetCharacter != null && defender.RulesetCharacter.CurrentHitPoints < defender.RulesetCharacter.GetAttribute("HitPoints").CurrentValue)
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasSenseType)
                    {
                        if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasSenseType(provider.RequiredTargetSenseType))
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCreatureTag)
                    {
                        if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasTag(provider.RequiredTargetCreatureTag))
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily)
                    {
                        if (defender.RulesetCharacter != null && defender.RulesetCharacter.CharacterFamily == provider.RequiredCharacterFamily.Name)
                        {
                            hasAditionalDamage = true;
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                    {
                        hasAditionalDamage = criticalHit;
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                    {
                        hasAditionalDamage = true;
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RagingAndTargetIsSpellcaster)
                    {
                        if (defender.RulesetCharacter != null && (attacker.RulesetCharacter.HasConditionOfType("ConditionRaging") && defender.RulesetCharacter.SpellRepertoires.Count > 0))
                        {
                            hasAditionalDamage = true;
                        }
                    }

                    if (hasAditionalDamage)
                    {
                        computeAndNotifyAdditionalDamageMethod.Invoke(__instance, new object[] { attacker, defender, provider, actualEffectForms, reactionParams, attackMode });
                    }
                }
            }
        }
    }
}
