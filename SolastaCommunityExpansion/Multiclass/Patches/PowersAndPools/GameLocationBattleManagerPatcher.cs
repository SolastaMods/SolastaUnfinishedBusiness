using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class GameLocationBattleManagerPatcher
    {
        //
        // TODO: WIP
        //

        // fixes additional damage calculation under wildshape (i.e.: rage, etc.)
        //[HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
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
                bool rangedAttack,
                RuleDefinitions.AdvantageType advantageType,
                List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect,
                bool criticalHit,
                bool firstTarget,
                List<string> ___triggeredAdditionalDamageTags,
                List<FeatureDefinition> ___featuresToBrowseReaction,
                List<FeatureDefinition> ___featuresToBrowseItem)
            {
                if (!Main.Settings.EnableMulticlass || !attacker.RulesetCharacter.IsSubstitute)
                {
                    while (values.MoveNext())
                    {
                        yield return values.Current;
                    }

                    yield break;
                }

                var gameLocationBattleManagerType = typeof(GameLocationBattleManager);
                var digitsToTrim = (char[])gameLocationBattleManagerType.GetField("digitsToTrim", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var computeAndNotifyAdditionalDamageMethod = gameLocationBattleManagerType.GetMethod("ComputeAndNotifyAdditionalDamage", BindingFlags.NonPublic | BindingFlags.Instance);
                var handleReactionToDamageMethod = gameLocationBattleManagerType.GetMethod("HandleReactionToDamage", BindingFlags.NonPublic | BindingFlags.Instance);
                var waitForReactionsMethod = gameLocationBattleManagerType.GetMethod("WaitForReactions", BindingFlags.NonPublic | BindingFlags.Instance);

                //
                // original game code from here with 2 patches (unfortunately cannot use transpilers on coroutines)
                //
                if (defender != null && defender.RulesetActor != null && (defender.RulesetActor is RulesetCharacterMonster || defender.RulesetActor is RulesetCharacterHero))
                {
                    ___triggeredAdditionalDamageTags.Clear();
                    attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(___featuresToBrowseReaction);
                    if (attacker.RulesetCharacter.CharacterInventory != null && attackMode != null && attackMode.SourceObject != null && attackMode.SourceObject is RulesetItem sourceObject)
                    {
                        sourceObject.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(___featuresToBrowseItem);
                        ___featuresToBrowseReaction.AddRange(___featuresToBrowseItem);
                        ___featuresToBrowseItem.Clear();
                    }
                    foreach (FeatureDefinition featureDefinition in ___featuresToBrowseReaction)
                    {
                        IAdditionalDamageProvider provider = featureDefinition as IAdditionalDamageProvider;
                        if (!provider.AttackModeOnly || attackMode != null)
                        {
                            bool flag1 = false;
                            bool flag2 = true;
                            if (provider.LimitedUsage != RuleDefinitions.FeatureLimitedUsage.None)
                            {
                                if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OnceInMyturn && (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name) || __instance.Battle != null && __instance.Battle.ActiveContender != attacker))
                                    flag2 = false;
                                else if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OncePerTurn && attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
                                    flag2 = false;
                                else if (attacker.UsedSpecialFeatures.Count > 0)
                                {
                                    foreach (KeyValuePair<string, int> usedSpecialFeature in attacker.UsedSpecialFeatures)
                                    {
                                        FeatureDefinitionAdditionalDamage result = (FeatureDefinitionAdditionalDamage)null;
                                        if (DatabaseRepository.GetDatabase<FeatureDefinitionAdditionalDamage>().TryGetElement(usedSpecialFeature.Key, out result) && result.NotificationTag == provider.NotificationTag)
                                            flag2 = false;
                                    }
                                }
                            }
                            CharacterActionParams reactionParams = null;
                            if (flag2)
                            {
                                if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly && attackMode != null)
                                {
                                    if (advantageType == RuleDefinitions.AdvantageType.Advantage || advantageType != RuleDefinitions.AdvantageType.Disadvantage && __instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker))
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot && attackModifier != null && attackModifier.Proximity == RuleDefinitions.AttackProximity.Melee)
                                {
                                    //
                                    // patch here to allow spell slot dependent powers to work under wildshape
                                    //

                                    //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                                    RulesetCharacterHero rulesetCharacter = Models.WildshapeContext.GetHero(attacker.RulesetCharacter) as RulesetCharacterHero;
                                    CharacterClassDefinition classDefinition = rulesetCharacter.FindClassHoldingFeature(featureDefinition);
                                    RulesetSpellRepertoire selectedSpellRepertoire = (RulesetSpellRepertoire)null;
                                    foreach (RulesetSpellRepertoire spellRepertoire in rulesetCharacter.SpellRepertoires)
                                    {
                                        if (spellRepertoire.SpellCastingClass == classDefinition)
                                        {
                                            bool flag3 = false;
                                            for (int spellLevel = 1; spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel; ++spellLevel)
                                            {
                                                spellRepertoire.GetSlotsNumber(spellLevel, out int remaining, out int max);
                                                if (remaining > 0)
                                                {
                                                    selectedSpellRepertoire = spellRepertoire;
                                                    flag3 = true;
                                                    break;
                                                }
                                            }
                                            if (flag3)
                                            {
                                                reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendSpellSlot);
                                                reactionParams.IntParameter = 1;
                                                reactionParams.StringParameter = provider.NotificationTag;
                                                reactionParams.SpellRepertoire = selectedSpellRepertoire;
                                                IGameLocationActionService service = ServiceRepository.GetService<IGameLocationActionService>();
                                                int count = service.PendingReactionRequestGroups.Count;
                                                service.ReactToSpendSpellSlot(reactionParams);
                                                //yield return (object)__instance.WaitForReactions(attacker, service, count);
                                                yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, service, count });
                                                flag1 = reactionParams.ReactionValidated;
                                            }
                                        }
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
                                {
                                    if (defender.RulesetActor.HasConditionOfTypeAndSource(provider.RequiredTargetCondition, attacker.Guid))
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition)
                                {
                                    if (defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
                                {
                                    if (!defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.CurrentHitPoints < defender.RulesetCharacter.GetAttribute("HitPoints").CurrentValue)
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasSenseType)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasSenseType(provider.RequiredTargetSenseType))
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCreatureTag)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasTag(provider.RequiredTargetCreatureTag))
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RangeAttackFromHigherGround && attackMode != null)
                                {
                                    if (attacker.LocationPosition.y > defender.LocationPosition.y)
                                    {
                                        ItemDefinition element = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                        if (element != null && element.IsWeapon && DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(element.WeaponDescription.WeaponType).WeaponProximity == RuleDefinitions.AttackProximity.Range)
                                            flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.CharacterFamily == provider.RequiredCharacterFamily.Name)
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                                    flag1 = criticalHit;
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage & firstTarget && rulesetEffect is RulesetEffectSpell && (rulesetEffect as RulesetEffectSpell).SpellDefinition.SchoolOfMagic == "SchoolEvocation")
                                    flag1 = true;
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage & firstTarget && rulesetEffect is RulesetEffectPower && (rulesetEffect as RulesetEffectPower).PowerDefinition.SurrogateToSpell != null && (rulesetEffect as RulesetEffectPower).PowerDefinition.SurrogateToSpell.SchoolOfMagic == "SchoolEvocation")
                                    flag1 = true;
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry & firstTarget && rulesetEffect is RulesetEffectSpell && attacker.RulesetCharacter.HasAncestryMatchingDamageType(actualEffectForms))
                                    flag1 = true;
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamagesTarget & firstTarget && rulesetEffect is RulesetEffectSpell)
                                    flag1 = true;
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                                    flag1 = true;
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RagingAndTargetIsSpellcaster && defender.RulesetCharacter != null)
                                {
                                    if (attacker.RulesetCharacter.HasConditionOfType("ConditionRaging") && defender.RulesetCharacter.SpellRepertoires.Count > 0)
                                        flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.Raging && attacker.RulesetCharacter.HasConditionOfType("ConditionRaging"))
                                    flag1 = true;
                            }
                            bool flag4 = true;
                            if (flag1 && provider.RequiredProperty != RuleDefinitions.AdditionalDamageRequiredProperty.None && attackMode != null)
                            {
                                bool flag5 = false;
                                bool flag6 = false;
                                bool flag7 = false;

                                //
                                // patch here to always recognize wildshape attack as a non finess one
                                //

                                //ItemDefinition element = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                //if (element != null && element.IsWeapon)
                                {
                                    //if (DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(element.WeaponDescription.WeaponType).WeaponProximity == RuleDefinitions.AttackProximity.Melee && !rangedAttack)
                                    if (!rangedAttack)
                                    {
                                        flag6 = true;
                                        //if (element.WeaponDescription.WeaponTags.Contains("Finesse"))
                                        //    flag5 = true;
                                    }
                                    else
                                        flag7 = true;
                                }
                                if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.FinesseOrRangeWeapon)
                                {
                                    if (!flag5 && !flag7)
                                        flag4 = false;
                                }
                                else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.RangeWeapon)
                                {
                                    if (!flag7)
                                        flag4 = false;
                                }
                                else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
                                {
                                    if (!flag6)
                                        flag4 = false;
                                }
                                else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.MeleeStrengthWeapon)
                                {
                                    if (!flag6 || attackMode.AbilityScore != "Strength")
                                        flag4 = false;
                                }
                                else
                                    Trace.LogAssertion(string.Format("RequiredProperty {0} not implemented for {1}.", (object)provider.RequiredProperty, (object)provider.TriggerCondition));
                            }
                            if (flag1 & flag4)
                            {
                                //__instance.ComputeAndNotifyAdditionalDamage(attacker, defender, provider, actualEffectForms, reactionParams, attackMode, criticalHit);
                                computeAndNotifyAdditionalDamageMethod.Invoke(__instance, new object[] { attacker, defender, provider, actualEffectForms, reactionParams, attackMode, criticalHit });
                                ___triggeredAdditionalDamageTags.Add(provider.NotificationTag);
                            }
                            provider = null;
                            reactionParams = null;
                        }
                    }
                    if (attacker.RulesetCharacter.UsablePowers.Count > 0)
                    {
                        foreach (RulesetUsablePower usablePower in attacker.RulesetCharacter.UsablePowers)
                        {
                            if (!attacker.RulesetCharacter.IsPowerOverriden(usablePower) && attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0 && (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHit && attackMode != null || usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHitWithBow && attackMode != null && attacker.RulesetCharacter.IsWieldingBow()))
                            {
                                CharacterActionParams reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                                reactionParams.StringParameter = usablePower.PowerDefinition.Name;
                                if (usablePower.PowerDefinition.OverriddenPower != null)
                                    reactionParams.StringParameter = reactionParams.StringParameter.Trim(digitsToTrim);
                                IRulesetImplementationService service1 = ServiceRepository.GetService<IRulesetImplementationService>();
                                reactionParams.RulesetEffect = (RulesetEffect)service1.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                                reactionParams.TargetCharacters.Add(defender);
                                reactionParams.IsReactionEffect = true;
                                IGameLocationActionService service2 = ServiceRepository.GetService<IGameLocationActionService>();
                                int count = service2.PendingReactionRequestGroups.Count;
                                service2.ReactToSpendPower(reactionParams);
                                //yield return (object)__instance.WaitForReactions(attacker, service2, count);
                                yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, service2, count });
                            }
                            else if (attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0 && (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackSpellHitAutomatic || usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnSneakAttackHit && ___triggeredAdditionalDamageTags.Contains("SneakAttack")))
                            {
                                CharacterActionParams actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                                actionParams.StringParameter = usablePower.PowerDefinition.Name;
                                IRulesetImplementationService service = ServiceRepository.GetService<IRulesetImplementationService>();
                                actionParams.RulesetEffect = service.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                                actionParams.TargetCharacters.Add(defender);
                                ServiceRepository.GetService<IGameLocationActionService>().ExecuteAction(actionParams, null, true);
                            }
                        }
                    }
                    if (attackMode != null && attackMode.Ranged && defender.GetActionStatus(ActionDefinitions.Id.DeflectMissile, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available)
                    {
                        CharacterActionParams reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.DeflectMissile);
                        reactionParams.ActionModifiers.Add(attackModifier);
                        reactionParams.TargetCharacters.Add(attacker);
                        IGameLocationActionService service = ServiceRepository.GetService<IGameLocationActionService>();
                        int count = service.PendingReactionRequestGroups.Count;
                        service.ReactToDeflectMissile(reactionParams);
                        //yield return (object)__instance.WaitForReactions(attacker, service, count);
                        yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, service, count });
                    }
                    if (defender.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) == ActionDefinitions.ActionStatus.Available)
                    {
                        if (defender.GetActionStatus(ActionDefinitions.Id.UncannyDodge, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available && defender.PerceivedFoes.Contains(attacker))
                        {
                            CharacterActionParams reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UncannyDodge);
                            reactionParams.ActionModifiers.Add(attackModifier);
                            reactionParams.TargetCharacters.Add(attacker);
                            IGameLocationActionService service = ServiceRepository.GetService<IGameLocationActionService>();
                            int count = service.PendingReactionRequestGroups.Count;
                            service.ReactToUncannyDodge(reactionParams);
                            //yield return (object)__instance.WaitForReactions(attacker, service, count);
                            yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, service, count });
                        }
                        if (((defender.GetActionStatus(ActionDefinitions.Id.LeafScales, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) != ActionDefinitions.ActionStatus.Available ? 0 : (defender.PerceivedFoes.Contains(attacker) ? 1 : 0)) & (rangedAttack ? 1 : 0)) != 0)
                        {
                            CharacterActionParams reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.LeafScales);
                            reactionParams.ActionModifiers.Add(attackModifier);
                            reactionParams.TargetCharacters.Add(attacker);
                            IGameLocationActionService service = ServiceRepository.GetService<IGameLocationActionService>();
                            int count = service.PendingReactionRequestGroups.Count;
                            service.ReactToLeafScales(reactionParams);
                            //yield return (object)__instance.WaitForReactions(attacker, service, count);
                            yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, service, count });
                        }
                    }
                    if (defender.RulesetCharacter != null)
                    {
                        defender.RulesetCharacter.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(___featuresToBrowseReaction);
                        foreach (IDamageAffinityProvider damageAffinityProvider in ___featuresToBrowseReaction)
                        {
                            if (damageAffinityProvider.RetaliateWhenHit && attackMode != null && (attackMode.Ranged && damageAffinityProvider.RetaliateProximity == RuleDefinitions.AttackProximity.Range || !attackMode.Ranged && damageAffinityProvider.RetaliateProximity == RuleDefinitions.AttackProximity.Melee) && __instance.IsWithinXCells(attacker, defender, damageAffinityProvider.RetaliateRangeCells) && damageAffinityProvider.RetaliatePower != null)
                            {
                                RulesetCharacter.DamageRetaliatedHandler damageRetaliated = defender.RulesetCharacter.DamageRetaliated;
                                if (damageRetaliated != null)
                                    damageRetaliated(defender.RulesetCharacter, attacker.RulesetCharacter, damageAffinityProvider);
                                CharacterActionParams actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower, attacker);
                                RulesetUsablePower usablePower = new RulesetUsablePower(damageAffinityProvider.RetaliatePower, null, null);
                                IRulesetImplementationService service = ServiceRepository.GetService<IRulesetImplementationService>();
                                actionParams.RulesetEffect = service.InstantiateEffectPower(defender.RulesetCharacter, usablePower, false);
                                actionParams.StringParameter = damageAffinityProvider.RetaliatePower.Name;
                                actionParams.IsReactionEffect = true;
                                ServiceRepository.GetService<IGameLocationActionService>().ExecuteInstantSingleAction(actionParams);
                            }
                        }
                        //yield return (object)__instance.HandleReactionToDamage(attacker, defender, attackModifier, actualEffectForms, attackMode);
                        yield return handleReactionToDamageMethod.Invoke(__instance, new object[] { attacker, defender, attackModifier, actualEffectForms, attackMode });
                    }
                }

            }
        }
    }
}
