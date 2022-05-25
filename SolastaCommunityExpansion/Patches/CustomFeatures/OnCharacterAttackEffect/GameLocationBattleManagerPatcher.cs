using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.OnCharacterAttackEffect
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttack
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            Main.Logger.Log("HandleCharacterAttack");

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.GetSubFeaturesByType<IOnAttackEffect>())
                {
                    feature.BeforeOnAttack(attacker, defender, attackModifier, attackerAttackMode);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.GetSubFeaturesByType<IOnAttackEffect>())
                {
                    feature.AfterOnAttack(attacker, defender, attackModifier, attackerAttackMode);
                }
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackHit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackHit
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged)
        {
            Main.Logger.Log("HandleCharacterAttackHit");

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
                {
                    feature.BeforeOnAttackHit(attacker, defender, attackModifier, attackRoll, successDelta, ranged);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
                {
                    feature.AfterOnAttackHit(attacker, defender, attackModifier, attackRoll, successDelta, ranged);
                }
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackDamage
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack, RuleDefinitions.AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            List<string> ___triggeredAdditionalDamageTags,
            List<FeatureDefinition> ___featuresToBrowseReaction,
            List<FeatureDefinition> ___featuresToBrowseItem)
        {
            Main.Logger.Log("HandleCharacterAttackDamage");

            var rulesetCharacter = attacker.RulesetCharacter;

            Global.CriticalHit = criticalHit;

            //
            // custom behavior before damage
            //
            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackDamageEffect>())
                {
                    feature.BeforeOnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
                }
            }

            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var gameLocationBattleManagerType = typeof(GameLocationBattleManager);
            var digitsToTrim = (char[])gameLocationBattleManagerType.GetField("digitsToTrim", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var computeAndNotifyAdditionalDamageMethod = gameLocationBattleManagerType.GetMethod("ComputeAndNotifyAdditionalDamage", BindingFlags.NonPublic | BindingFlags.Instance);
            var handleReactionToDamageMethod = gameLocationBattleManagerType.GetMethod("HandleReactionToDamage", BindingFlags.NonPublic | BindingFlags.Instance);
            var waitForReactionsMethod = gameLocationBattleManagerType.GetMethod("WaitForReactions", BindingFlags.NonPublic | BindingFlags.Instance);

            //
            // original game code from here
            //

            // Process this only when targeting a hero or monster
            if (defender != null && defender.RulesetActor != null && (defender.RulesetActor is RulesetCharacterMonster || defender.RulesetActor is RulesetCharacterHero))
            {
                // Can I add additional damage?
                ___triggeredAdditionalDamageTags.Clear();
                attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(___featuresToBrowseReaction);

                // Add item properties?
                if (attacker.RulesetCharacter.CharacterInventory != null)
                {
                    if (attackMode != null && attackMode.SourceObject != null && attackMode.SourceObject is RulesetItem weapon)
                    {
                        weapon.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(___featuresToBrowseItem);
                        ___featuresToBrowseReaction.AddRange(___featuresToBrowseItem);
                        ___featuresToBrowseItem.Clear();
                    }
                }

                foreach (var featureDefinition in ___featuresToBrowseReaction)
                {
                    var provider = featureDefinition as IAdditionalDamageProvider;

                    // Some additional damage only work with attack modes (Hunter's Mark)
                    if (provider.AttackModeOnly && attackMode == null)
                    {
                        continue;
                    }

                    // Trigger method
                    var validTrigger = false;
                    var validUses = true;
                    var validProperty = true;
                    if (provider.LimitedUsage != RuleDefinitions.FeatureLimitedUsage.None)
                    {
                        if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OnceInMyturn && (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name) || (__instance.Battle != null && __instance.Battle.ActiveContender != attacker)))
                        {
                            validUses = false;
                        }
                        else if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OncePerTurn && attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
                        {
                            validUses = false;
                        }
                        else if (attacker.UsedSpecialFeatures.Count > 0)
                        {
                            // Check if there is not already a used feature with the same tag (special sneak attack for Rogue Hoodlum / COTM-18228
                            foreach (var kvp in attacker.UsedSpecialFeatures)
                            {
                                if (DatabaseRepository.GetDatabase<FeatureDefinitionAdditionalDamage>().TryGetElement(kvp.Key, out var previousFeature))
                                {
                                    if (previousFeature.NotificationTag == provider.NotificationTag)
                                    {
                                        validUses = false;
                                    }
                                }
                            }
                        }
                    }

                    CharacterActionParams reactionParams = null;
                    if (validUses)
                    {
                        // Check required properties if needed
                        validProperty = ValidateProperty();
                    }

                    if (validUses && validProperty)
                    {
                        // Typical for Sneak Attack
                        if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly && attackMode != null)
                        {
                            if (advantageType == RuleDefinitions.AdvantageType.Advantage || (advantageType != RuleDefinitions.AdvantageType.Disadvantage && __instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker)))
                            {
                                validTrigger = true;
                            }
                        }
                        // This is used to ignore Divine Smite
                        //
                        // patch here
                        //
                        // melee only is now controlled via properties
                        //
                        //else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot && attackModifier != null && attackModifier.Proximity == RuleDefinitions.AttackProximity.Melee)
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot)
                        {
                            // This is used to allow Divine Smite under Wildshape
                            // Look for the spellcasting feature holding the smite
                            //
                            // patch here
                            //
                            //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                            var hero = attacker.RulesetCharacter as RulesetCharacterHero
                                ?? attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;
                            var classDefinition = hero.FindClassHoldingFeature(featureDefinition);
                            RulesetSpellRepertoire selectedSpellRepertoire = null;
                            foreach (var spellRepertoire in hero.SpellRepertoires)
                            {
                                if (spellRepertoire.SpellCastingClass == classDefinition)
                                {
                                    var atLeastOneSpellSlotAvailable = false;
                                    for (var spellLevel = 1; spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel; spellLevel++)
                                    {
                                        spellRepertoire.GetSlotsNumber(spellLevel, out var remaining, out var max);
                                        // handle EldritchSmite case that can only consume pact slots
                                        //
                                        // patch here
                                        //
                                        if (featureDefinition is FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage
                                            && featureDefinitionAdditionalDamage.NotificationTag == "EldritchSmite")
                                        {
                                            var pactMagicMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
                                            var pactMagicUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);

                                            remaining = pactMagicMaxSlots - pactMagicUsedSlots;
                                        }
                                        // end patch

                                        if (remaining > 0)
                                        {
                                            selectedSpellRepertoire = spellRepertoire;
                                            atLeastOneSpellSlotAvailable = true;
                                            break;
                                        }
                                    }

                                    if (atLeastOneSpellSlotAvailable)
                                    {
                                        reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendSpellSlot);
                                        reactionParams.IntParameter = 1;
                                        reactionParams.StringParameter = provider.NotificationTag;
                                        reactionParams.SpellRepertoire = selectedSpellRepertoire;
                                        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                                        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                                        actionService.ReactToSpendSpellSlot(reactionParams);

                                        //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                                        yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });

                                        validTrigger = reactionParams.ReactionValidated;
                                    }
                                }
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
                        {
                            if (defender.RulesetActor.HasConditionOfTypeAndSource(provider.RequiredTargetCondition, attacker.Guid))
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition)
                        {
                            if (defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
                        {
                            if (!defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded)
                        {
                            if (defender.RulesetCharacter != null && defender.RulesetCharacter.CurrentHitPoints < defender.RulesetCharacter.GetAttribute(AttributeDefinitions.HitPoints).CurrentValue)
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasSenseType)
                        {
                            if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasSenseType(provider.RequiredTargetSenseType))
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCreatureTag)
                        {
                            if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasTag(provider.RequiredTargetCreatureTag))
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RangeAttackFromHigherGround && attackMode != null)
                        {
                            if (attacker.LocationPosition.y > defender.LocationPosition.y)
                            {
                                var itemDefinition = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                if (itemDefinition != null
                                    && itemDefinition.IsWeapon)
                                {
                                    var weaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(itemDefinition.WeaponDescription.WeaponType);
                                    if (weaponTypeDefinition.WeaponProximity == RuleDefinitions.AttackProximity.Range)
                                    {
                                        validTrigger = true;
                                    }
                                }
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily)
                        {
                            if (defender.RulesetCharacter != null && defender.RulesetCharacter.CharacterFamily == provider.RequiredCharacterFamily.Name)
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                        {
                            validTrigger = criticalHit;
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage
                            && firstTarget
                            && rulesetEffect is RulesetEffectSpell
                            && (rulesetEffect as RulesetEffectSpell).SpellDefinition.SchoolOfMagic == RuleDefinitions.SchoolEvocation)
                        {
                            validTrigger = true;
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage
                            && firstTarget
                            && rulesetEffect is RulesetEffectPower
                            && (rulesetEffect as RulesetEffectPower).PowerDefinition.SurrogateToSpell != null
                            && (rulesetEffect as RulesetEffectPower).PowerDefinition.SurrogateToSpell.SchoolOfMagic == RuleDefinitions.SchoolEvocation)
                        {
                            validTrigger = true;
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry
                            && firstTarget
                            && rulesetEffect is RulesetEffectSpell
                            && attacker.RulesetCharacter.HasAncestryMatchingDamageType(actualEffectForms))
                        {
                            validTrigger = true;
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamagesTarget
                            && firstTarget
                            && rulesetEffect is RulesetEffectSpell)
                        {
                            validTrigger = true;
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                        {
                            validTrigger = true;
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RagingAndTargetIsSpellcaster && defender.RulesetCharacter != null)
                        {
                            if (attacker.RulesetCharacter.HasConditionOfType(RuleDefinitions.ConditionRaging) && defender.RulesetCharacter.SpellRepertoires.Count > 0)
                            {
                                validTrigger = true;
                            }
                        }
                        else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.Raging)
                        {
                            if (attacker.RulesetCharacter.HasConditionOfType(RuleDefinitions.ConditionRaging))
                            {
                                validTrigger = true;
                            }
                        }
                    }

                    //Wrapped in no-formatting to simplify merges/changes from TA ------ START --------
                    // @formatter:off
                    bool ValidateProperty()
                    {
                        // ReSharper disable once VariableHidesOuterVariable
                        // Check required properties if needed
                        var validProperty = true;
                        if (/*validTrigger &&*/ provider.RequiredProperty != RuleDefinitions.AdditionalDamageRequiredProperty.None && attackMode != null)
                        {
                            var finesse = false;
                            var melee = false;
                            var range = false;
                            var itemDefinition = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                            if (itemDefinition != null
                                && itemDefinition.IsWeapon)
                            {
                                var weaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(itemDefinition.WeaponDescription.WeaponType);
                                if (weaponTypeDefinition.WeaponProximity == RuleDefinitions.AttackProximity.Melee && !rangedAttack)
                                {
                                    melee = true;

                                    if (itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagFinesse))
                                    {
                                        finesse = true;
                                    }
                                }
                                else
                                {
                                    range = true;
                                }
                            }
                            else if (attacker.RulesetCharacter.IsSubstitute)
                            {
                                melee = true;
                            }
                            //CUSTOM CODE ---- START
                            //Count shield bashing as melee
                            if (!melee && ShieldStrikeContext.IsShield(attackMode.SourceDefinition as ItemDefinition)) {
                                melee = true;
                            }
                            //CUSTOM CODE ---- END

                            if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.FinesseOrRangeWeapon)
                            {
                                if (!finesse && !range)
                                {
                                    validProperty = false;
                                }
                            }
                            else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.RangeWeapon)
                            {
                                if (!range)
                                {
                                    validProperty = false;
                                }
                            }
                            else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
                            {
                                if (!melee)
                                {
                                    validProperty = false;
                                }
                            }
                            else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.MeleeStrengthWeapon)
                            {
                                if (!melee || attackMode.AbilityScore != AttributeDefinitions.Strength)
                                {
                                    validProperty = false;
                                }
                            }
                            else
                            {
                                Trace.LogAssertion($"RequiredProperty {provider.RequiredProperty} not implemented for {provider.TriggerCondition}.");
                            }
                        }

                        return validProperty;
                    }
                    // @formatter:on
                    //Wrapped in no-formatting to simplify merges/changes from TA ------ END --------
                    if (validTrigger && validProperty)
                    {
                        //__instance.ComputeAndNotifyAdditionalDamage(attacker, defender, provider, actualEffectForms, reactionParams, attackMode, criticalHit);
                        computeAndNotifyAdditionalDamageMethod.Invoke(__instance, new object[] { attacker, defender, provider, actualEffectForms, reactionParams, attackMode, criticalHit });

                        ___triggeredAdditionalDamageTags.Add(provider.NotificationTag);
                    }
                }

                // Can the attacker trigger a power on performing a hit? Example use: Decisive Strike of the Battle domain
                if (attacker.RulesetCharacter.UsablePowers.Count > 0)
                {
                    foreach (var usablePower in attacker.RulesetCharacter.UsablePowers)
                    {
                        var validator = usablePower.PowerDefinition.GetFirstSubFeatureOfType<IReactionAttackModeRestriction>();
                        if (validator != null && !validator.ValidReactionMode(attackMode, attacker.RulesetCharacter, defender.RulesetCharacter))
                        {
                            continue;
                        }

                        if (!attacker.RulesetCharacter.IsPowerOverriden(usablePower)
                            && attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0
                            && ((usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHit && attackMode != null)
                                || (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHitWithBow && attackMode != null && attacker.RulesetCharacter.IsWieldingBow())))
                        {
                            var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                            reactionParams.StringParameter = usablePower.PowerDefinition.Name;

                            // Remove trailing numbers for advanced powers
                            if (usablePower.PowerDefinition.OverriddenPower != null)
                            {
                                //reactionParams.StringParameter = reactionParams.StringParameter.Trim(GameLocationBattleManager.digitsToTrim);
                                reactionParams.StringParameter = reactionParams.StringParameter.Trim(digitsToTrim);
                            }

                            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                            reactionParams.RulesetEffect = rulesetImplementationService.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                            reactionParams.TargetCharacters.Add(defender);
                            reactionParams.ActionModifiers.Add(new ActionModifier());
                            reactionParams.IsReactionEffect = true;

                            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                            var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                            actionService.ReactToSpendPower(reactionParams);

                            //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                            yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });
                        }
                        else if (attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0
                            && ((usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackSpellHitAutomatic)
                                || (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnSneakAttackHit && ___triggeredAdditionalDamageTags.Contains(TagsDefinitions.AdditionalDamageSneakAttackTag))))
                        {
                            // This case is for the Rogue Hoodlum
                            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                            actionParams.StringParameter = usablePower.PowerDefinition.Name;

                            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                            actionParams.RulesetEffect = rulesetImplementationService.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                            actionParams.TargetCharacters.Add(defender);

                            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                            actionService.ExecuteAction(actionParams, null, true);
                        }
                    }
                }

                // Can I reduce the damage quantity of this attackMode?
                if (attackMode != null && attackMode.Ranged && defender.GetActionStatus(ActionDefinitions.Id.DeflectMissile, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available)
                {
                    var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.DeflectMissile);
                    reactionParams.ActionModifiers.Add(attackModifier);
                    reactionParams.TargetCharacters.Add(attacker);
                    var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                    var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                    actionService.ReactToDeflectMissile(reactionParams);

                    //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                    yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });
                }

                // Can I modify the damage?
                // Basic verifications
                if (defender.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) == ActionDefinitions.ActionStatus.Available)
                {
                    // Does it have the proper action?
                    if (defender.GetActionStatus(ActionDefinitions.Id.UncannyDodge, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available
                        && defender.PerceivedFoes.Contains(attacker))
                    {
                        var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UncannyDodge);
                        reactionParams.ActionModifiers.Add(attackModifier);
                        reactionParams.TargetCharacters.Add(attacker);
                        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                        actionService.ReactToUncannyDodge(reactionParams);

                        //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                        yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });
                    }

                    if (defender.GetActionStatus(ActionDefinitions.Id.LeafScales, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available
                        && defender.PerceivedFoes.Contains(attacker)
                        && rangedAttack)
                    {
                        var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.LeafScales);
                        reactionParams.ActionModifiers.Add(attackModifier);
                        reactionParams.TargetCharacters.Add(attacker);
                        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                        actionService.ReactToLeafScales(reactionParams);

                        //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                        yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });
                    }
                }

                // Can the defender retaliate when damaged? This is for Remorhaz & Fireshield
                if (defender.RulesetCharacter != null)
                {
                    defender.RulesetCharacter.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(___featuresToBrowseReaction);
                    foreach (IDamageAffinityProvider provider in ___featuresToBrowseReaction)
                    {
                        if (provider.RetaliateWhenHit && attackMode != null)
                        {
                            // Does the range match
                            if ((attackMode.Ranged && provider.RetaliateProximity == RuleDefinitions.AttackProximity.Range) || (!attackMode.Ranged && provider.RetaliateProximity == RuleDefinitions.AttackProximity.Melee))
                            {
                                // In the range distance?
                                if (__instance.IsWithinXCells(attacker, defender, provider.RetaliateRangeCells))
                                {
                                    // Just check the power is valid, no need to check the defender ability to use the power as this is automatic
                                    if (provider.RetaliatePower != null)
                                    {
                                        // Notify the console
                                        defender.RulesetCharacter.DamageRetaliated?.Invoke(defender.RulesetCharacter, attacker.RulesetCharacter, provider);

                                        // Build the params
                                        var retaliateParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower, attacker);
                                        var dummyUsablePower = new RulesetUsablePower(provider.RetaliatePower, null, null);

                                        // Build the active effect
                                        var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                                        retaliateParams.RulesetEffect = rulesetImplementationService.InstantiateEffectPower(defender.RulesetCharacter, dummyUsablePower, false);
                                        retaliateParams.StringParameter = provider.RetaliatePower.Name;
                                        retaliateParams.IsReactionEffect = true;

                                        // Start the action
                                        var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
                                        gameLocationActionService.ExecuteInstantSingleAction(retaliateParams);
                                    }
                                }
                            }
                        }
                    }

                    //yield return __instance.HandleReactionToDamage(attacker, defender, attackModifier, actualEffectForms, attackMode);
                    yield return handleReactionToDamageMethod.Invoke(__instance, new object[] { attacker, defender, attackModifier, actualEffectForms, attackMode });
                }
            }

            //
            // Custom Behavior after damage calculation
            //

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackDamageEffect>())
                {
                    feature.AfterOnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
                }
            }

            yield return CustomReactionsContext.TryReactingToDamageWithSpell(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);

            Global.CriticalHit = false;
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMagicalAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterMagicalAttackDamage
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            Main.Logger.Log("HandleCharacterMagicalAttackDamage");

            Global.CriticalHit = criticalHit;

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnMagicalAttackDamageEffect>())
                {
                    feature.BeforeOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect, actualEffectForms, firstTarget, criticalHit);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnMagicalAttackDamageEffect>())
                {
                    feature.AfterOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect, actualEffectForms, firstTarget, criticalHit);
                }
            }

            Global.CriticalHit = false;
        }
    }
}
