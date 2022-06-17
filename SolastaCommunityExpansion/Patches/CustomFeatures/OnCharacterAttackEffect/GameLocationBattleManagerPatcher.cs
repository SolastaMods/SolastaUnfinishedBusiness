using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.OnCharacterAttackEffect;

[HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
internal static class GameLocationBattleManager_ComputeAndNotifyAdditionalDamage
{
    internal static bool Prefix(
        GameLocationBattleManager __instance,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        IAdditionalDamageProvider provider,
        List<EffectForm> actualEffectForms,
        CharacterActionParams reactionParams,
        RulesetAttackMode attackMode,
        bool criticalHit)
    {
        //
        // GAME CODE FROM HERE
        //

        var damageForm = DamageForm.Get();
        var featureDefinition = provider as FeatureDefinition;

        if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.Die)
        {
            var num = provider.DamageDiceNumber;

            if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
            {
                // game code doesn't consider heroes in wildshape form
                //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                var rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero ??
                                       attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;
                var classHoldingFeature = rulesetCharacter.FindClassHoldingFeature(featureDefinition);

                if (classHoldingFeature != null)
                {
                    var classesAndLevel = rulesetCharacter.ClassesAndLevels[classHoldingFeature];
                    num = provider.GetDiceOfRank(classesAndLevel);
                }
            }
            else if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.SlotLevel)
            {
                if (reactionParams != null)
                {
                    num = provider.GetDiceOfRank(reactionParams.IntParameter);
                }
                else
                {
                    var conditionHoldingFeature =
                        attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
                    if (conditionHoldingFeature != null)
                    {
                        num = provider.GetDiceOfRank(conditionHoldingFeature.EffectLevel);
                    }
                }
            }

            if (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
            {
                attacker.UsedSpecialFeatures[featureDefinition.Name]++;
            }
            else
            {
                attacker.UsedSpecialFeatures[featureDefinition.Name] = 1;
            }

            if (defender.RulesetCharacter != null && provider.FamiliesWithAdditionalDice.Count > 0 &&
                provider.FamiliesWithAdditionalDice.Contains(defender.RulesetCharacter.CharacterFamily))
            {
                num += provider.FamiliesDiceNumber;
            }

            damageForm.DieType = provider.DamageDieType;
            damageForm.DiceNumber = num;
        }
        // game code doesn't consider heroes in wildshape form
        //else if (attacker.RulesetCharacter is RulesetCharacterHero && (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)))
        else if
            ((attacker.RulesetCharacter is RulesetCharacterHero ||
              attacker.RulesetCharacter.OriginalFormCharacter is RulesetCharacterHero) &&
             (provider.DamageValueDetermination ==
              RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus ||
              provider.DamageValueDetermination ==
              RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus ||
              provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination
                  .ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination ==
              RuleDefinitions.AdditionalDamageValueDetermination.RageDamage))
        {
            damageForm.DieType = RuleDefinitions.DieType.D1;
            damageForm.DiceNumber = 0;
            damageForm.BonusDamage = 0;

            if (provider.DamageValueDetermination ==
                RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus ||
                provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination
                    .ProficiencyBonusAndSpellcastingBonus)
            {
                // game code doesn't consider heroes in wildshape form
                //damageForm.BonusDamage += (attacker.RulesetCharacter as RulesetCharacterHero).GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
                damageForm.BonusDamage += attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            }

            if (provider.DamageValueDetermination ==
                RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus ||
                provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination
                    .ProficiencyBonusAndSpellcastingBonus)
            {
                var num = 0;

                // use the correct spell repertoire for calculating spell bonus (MC scenario)
                //
                //foreach (RulesetSpellRepertoire spellRepertoire in attacker.RulesetCharacter.SpellRepertoires)
                //{
                //    num = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue);
                //    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                //        break;
                //}
                if (Global.CastedSpellRepertoire != null)
                {
                    num = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter
                        .GetAttribute(Global.CastedSpellRepertoire.SpellCastingAbility).CurrentValue);
                }
                // this scenario should not happen but who knows under MP ;-)
                else
                {
                    foreach (var spellRepertoire in attacker.RulesetCharacter.SpellRepertoires)
                    {
                        num = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter
                            .GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue);
                        if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
                            FeatureDefinitionCastSpell.CastingOrigin.Class)
                        {
                            break;
                        }
                    }
                }

                damageForm.BonusDamage += num;
            }

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)
            {
                damageForm.BonusDamage = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.RageDamage);
            }
        }
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusOfSource)
        {
            var conditionHoldingFeature =
                attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);

            if (conditionHoldingFeature != null && RulesetEntity.TryGetEntity(conditionHoldingFeature.SourceGuid,
                    out RulesetCharacter entity))
            {
                damageForm.DieType = RuleDefinitions.DieType.D1;
                damageForm.DiceNumber = 0;
                damageForm.BonusDamage = entity.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            }
        }
        // game code doesn't consider heroes in wildshape form
        //else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && attacker.RulesetCharacter is RulesetCharacterHero && defender.RulesetCharacter is RulesetCharacterMonster)
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel &&
                 (attacker.RulesetCharacter is RulesetCharacterHero ||
                  attacker.RulesetCharacter.OriginalFormCharacter is RulesetCharacterHero) &&
                 defender.RulesetCharacter is RulesetCharacterMonster)
        {
            damageForm.DieType = RuleDefinitions.DieType.D1;
            damageForm.DiceNumber = 0;
            damageForm.BonusDamage = ServiceRepository.GetService<IGameLoreService>()
                .GetCreatureKnowledgeLevel(defender.RulesetCharacter).AdditionalDamage;
        }
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.BrutalCriticalDice)
        {
            var flag = attackMode != null && attackMode.UseVersatileDamage;
            var firstDamageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
            damageForm.DieType = flag ? firstDamageForm.VersatileDieType : firstDamageForm.DieType;
            damageForm.DiceNumber = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.BrutalCriticalDice);
            damageForm.BonusDamage = 0;
        }
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
        {
            var flag = attackMode != null && attackMode.UseVersatileDamage;
            var firstDamageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
            damageForm.DieType = flag ? firstDamageForm.VersatileDieType : firstDamageForm.DieType;
            damageForm.DiceNumber = 1;
            damageForm.BonusDamage = 0;
        }
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.None)
        {
            damageForm.DiceNumber = 0;
            damageForm.BonusDamage = 0;
        }

        damageForm.IgnoreCriticalDoubleDice = provider.IgnoreCriticalDoubleDice;
        damageForm.IgnoreSpellAdvancementDamageDice = true;

        if (damageForm.DiceNumber > 0 || damageForm.BonusDamage > 0)
        {
            switch (provider.AdditionalDamageType)
            {
                case RuleDefinitions.AdditionalDamageType.SameAsBaseDamage:
                    damageForm.DamageType = EffectForm.GetFirstDamageForm(actualEffectForms).DamageType;
                    break;

                case RuleDefinitions.AdditionalDamageType.Specific:
                    damageForm.DamageType = provider.SpecificDamageType;
                    break;

                case RuleDefinitions.AdditionalDamageType.AncestryDamageType:
                    attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(
                        FeatureDefinitionAncestry.FeaturesToBrowse);
                    if (FeatureDefinitionAncestry.FeaturesToBrowse.Count > 0)
                    {
                        damageForm.DamageType =
                            (FeatureDefinitionAncestry.FeaturesToBrowse[0] as FeatureDefinitionAncestry).DamageType;
                    }

                    break;
            }

            var fromDamageForm = EffectForm.GetFromDamageForm(damageForm);

            if (provider.HasSavingThrow)
            {
                fromDamageForm.SavingThrowAffinity = provider.DamageSaveAffinity;
                var savingThrowDc = ServiceRepository.GetService<IRulesetImplementationService>()
                    .ComputeSavingThrowDC(attacker.RulesetCharacter, provider);
                fromDamageForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility,
                    savingThrowDc, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
            }

            actualEffectForms.Add(fromDamageForm);

            if (attacker.RulesetCharacter.AdditionalDamageGenerated != null)
            {
                var diceNumber = damageForm.DiceNumber;

                if ((damageForm.DieType != 0) & criticalHit && !damageForm.IgnoreCriticalDoubleDice)
                {
                    diceNumber *= 2;
                }

                attacker.RulesetCharacter.AdditionalDamageGenerated(attacker.RulesetCharacter,
                    defender.RulesetActor, damageForm.DieType, diceNumber, damageForm.BonusDamage,
                    provider.NotificationTag);
            }
        }

        if (provider.ConditionOperations.Count > 0)
        {
            foreach (var conditionOperation in provider.ConditionOperations)
            {
                var effectForm = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition, ConditionForm = new ConditionForm()
                };

                effectForm.ConditionForm.ConditionDefinition = conditionOperation.ConditionDefinition;
                effectForm.ConditionForm.Operation =
                    conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add
                        ? ConditionForm.ConditionOperation.Add
                        : ConditionForm.ConditionOperation.Remove;
                effectForm.CanSaveToCancel = conditionOperation.CanSaveToCancel;
                effectForm.SaveOccurence = conditionOperation.SaveOccurence;

                if (conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add &&
                    provider.HasSavingThrow)
                {
                    effectForm.SavingThrowAffinity = conditionOperation.SaveAffinity;
                    var savingThrowDc = ServiceRepository.GetService<IRulesetImplementationService>()
                        .ComputeSavingThrowDC(attacker.RulesetCharacter, provider);
                    effectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility,
                        savingThrowDc, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                }

                actualEffectForms.Add(effectForm);
            }
        }

        if (provider.AddLightSource && defender.RulesetCharacter != null &&
            defender.RulesetCharacter.PersonalLightSource == null)
        {
            var lightSourceForm = provider.LightSourceForm;
            var service = ServiceRepository.GetService<IGameLocationVisibilityService>();
            float brightRange = lightSourceForm.BrightRange;
            var dimRangeCells = brightRange + lightSourceForm.DimAdditionalRange;
            defender.RulesetCharacter.PersonalLightSource = new RulesetLightSource(lightSourceForm.Color,
                brightRange, dimRangeCells, lightSourceForm.GraphicsPrefabAssetGUID,
                lightSourceForm.LightSourceType, featureDefinition.Name, defender.RulesetCharacter.Guid);
            defender.RulesetCharacter.PersonalLightSource.Register(true);
            var character = defender;
            var personalLightSource = defender.RulesetCharacter.PersonalLightSource;
            service.AddCharacterLightSource(character, personalLightSource);
            var conditionHoldingFeature =
                attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
            if (conditionHoldingFeature != null)
            {
                attacker.RulesetCharacter.FindEffectTrackingCondition(conditionHoldingFeature).TrackLightSource(
                    defender.RulesetCharacter, defender.Guid, string.Empty,
                    defender.RulesetCharacter.PersonalLightSource);
            }
        }

        // for some reason only Die value determination increments feature uses
        // this fix increments for all other types otherwise additional damage features that use other types
        // (like Elemental Forms of Elementalist Warlock use PB as damage bonus) will trigger on each hit
        // regardless of usage limit setting

        if (provider.DamageValueDetermination != RuleDefinitions.AdditionalDamageValueDetermination.Die)
        {
            if (attacker.UsedSpecialFeatures.ContainsKey(provider.Name))
            {
                attacker.UsedSpecialFeatures[provider.Name]++;
            }
            else
            {
                attacker.UsedSpecialFeatures[provider.Name] = 1;
            }
        }

        //
        // GAME CODE
        //

        __instance.AdditionalDamageProviderActivated?.Invoke(attacker, defender, provider);

        return false;
    }
}

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
        bool firstTarget)
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
                feature.BeforeOnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack,
                    advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
            }
        }

        var digitsToTrim = GameLocationBattleManager.digitsToTrim;

        //
        // original game code from here
        //

        // Process this only when targeting a hero or monster
        if (defender != null && defender.RulesetActor != null &&
            (defender.RulesetActor is RulesetCharacterMonster || defender.RulesetActor is RulesetCharacterHero))
        {
            // Can I add additional damage?
            __instance.triggeredAdditionalDamageTags.Clear();
            attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(
                __instance.featuresToBrowseReaction);

            // Add item properties?
            if (attacker.RulesetCharacter.CharacterInventory != null)
            {
                if (attackMode != null && attackMode.SourceObject != null &&
                    attackMode.SourceObject is RulesetItem weapon)
                {
                    weapon.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(__instance.featuresToBrowseItem);
                    __instance.featuresToBrowseReaction.AddRange(__instance.featuresToBrowseItem);
                    __instance.featuresToBrowseItem.Clear();
                }
            }

            foreach (var featureDefinition in __instance.featuresToBrowseReaction)
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
                    if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OnceInMyturn &&
                        (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name) ||
                         (__instance.Battle != null && __instance.Battle.ActiveContender != attacker)))
                    {
                        validUses = false;
                    }
                    else if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OncePerTurn &&
                             attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
                    {
                        validUses = false;
                    }
                    else if (attacker.UsedSpecialFeatures.Count > 0)
                    {
                        // Check if there is not already a used feature with the same tag (special sneak attack for Rogue Hoodlum / COTM-18228
                        foreach (var kvp in attacker.UsedSpecialFeatures)
                        {
                            if (DatabaseRepository.GetDatabase<FeatureDefinitionAdditionalDamage>()
                                .TryGetElement(kvp.Key, out var previousFeature))
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
                    if (provider.TriggerCondition ==
                        RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly &&
                        attackMode != null)
                    {
                        if (advantageType == RuleDefinitions.AdvantageType.Advantage ||
                            (advantageType != RuleDefinitions.AdvantageType.Disadvantage &&
                             __instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side,
                                 attacker)))
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
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot)
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
                                for (var spellLevel = 1;
                                     spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel;
                                     spellLevel++)
                                {
                                    spellRepertoire.GetSlotsNumber(spellLevel, out var remaining, out var max);
                                    // handle EldritchSmite case that can only consume pact slots
                                    //
                                    // patch here
                                    //
                                    if (featureDefinition is FeatureDefinitionAdditionalDamage
                                            featureDefinitionAdditionalDamage
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
                                    reactionParams = new CharacterActionParams(attacker,
                                        ActionDefinitions.Id.SpendSpellSlot);
                                    reactionParams.IntParameter = 1;
                                    reactionParams.StringParameter = provider.NotificationTag;
                                    reactionParams.SpellRepertoire = selectedSpellRepertoire;
                                    var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                                    var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                                    actionService.ReactToSpendSpellSlot(reactionParams);

                                    yield return __instance.WaitForReactions(attacker, actionService,
                                        previousReactionCount);

                                    validTrigger = reactionParams.ReactionValidated;
                                }
                            }
                        }
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition
                                 .TargetHasConditionCreatedByMe)
                    {
                        if (defender.RulesetActor.HasConditionOfTypeAndSource(provider.RequiredTargetCondition,
                                attacker.Guid))
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition)
                    {
                        if (defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
                    {
                        if (!defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded)
                    {
                        if (defender.RulesetCharacter != null && defender.RulesetCharacter.CurrentHitPoints <
                            defender.RulesetCharacter.GetAttribute(AttributeDefinitions.HitPoints).CurrentValue)
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasSenseType)
                    {
                        if (defender.RulesetCharacter != null &&
                            defender.RulesetCharacter.HasSenseType(provider.RequiredTargetSenseType))
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCreatureTag)
                    {
                        if (defender.RulesetCharacter != null &&
                            defender.RulesetCharacter.HasTag(provider.RequiredTargetCreatureTag))
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.RangeAttackFromHigherGround &&
                             attackMode != null)
                    {
                        if (attacker.LocationPosition.y > defender.LocationPosition.y)
                        {
                            var itemDefinition = DatabaseRepository.GetDatabase<ItemDefinition>()
                                .GetElement(attackMode.SourceDefinition.Name, true);
                            if (itemDefinition != null
                                && itemDefinition.IsWeapon)
                            {
                                var weaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>()
                                    .GetElement(itemDefinition.WeaponDescription.WeaponType);
                                if (weaponTypeDefinition.WeaponProximity == RuleDefinitions.AttackProximity.Range)
                                {
                                    validTrigger = true;
                                }
                            }
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily)
                    {
                        if (defender.RulesetCharacter != null && defender.RulesetCharacter.CharacterFamily ==
                            provider.RequiredCharacterFamily.Name)
                        {
                            validTrigger = true;
                        }
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                    {
                        validTrigger = criticalHit;
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage
                             && firstTarget
                             && rulesetEffect is RulesetEffectSpell
                             && (rulesetEffect as RulesetEffectSpell).SpellDefinition.SchoolOfMagic ==
                             RuleDefinitions.SchoolEvocation)
                    {
                        validTrigger = true;
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage
                             && firstTarget
                             && rulesetEffect is RulesetEffectPower
                             && (rulesetEffect as RulesetEffectPower).PowerDefinition.SurrogateToSpell != null
                             && (rulesetEffect as RulesetEffectPower).PowerDefinition.SurrogateToSpell
                             .SchoolOfMagic == RuleDefinitions.SchoolEvocation)
                    {
                        validTrigger = true;
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition
                                 .SpellDamageMatchesSourceAncestry
                             && firstTarget
                             && rulesetEffect is RulesetEffectSpell
                             && attacker.RulesetCharacter.HasAncestryMatchingDamageType(actualEffectForms))
                    {
                        validTrigger = true;
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamagesTarget
                             && firstTarget
                             && rulesetEffect is RulesetEffectSpell)
                    {
                        validTrigger = true;
                    }
                    else if (provider.TriggerCondition ==
                             RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                    {
                        validTrigger = true;
                    }
                    else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition
                                 .RagingAndTargetIsSpellcaster && defender.RulesetCharacter != null)
                    {
                        if (attacker.RulesetCharacter.HasConditionOfType(RuleDefinitions.ConditionRaging) &&
                            defender.RulesetCharacter.SpellRepertoires.Count > 0)
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
                            if (!melee && ShieldStrikeContext.IsShield(attackMode.SourceDefinition as ItemDefinition))
                            {
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
                    __instance.ComputeAndNotifyAdditionalDamage(attacker, defender, provider, actualEffectForms,
                        reactionParams, attackMode, criticalHit);

                    __instance.triggeredAdditionalDamageTags.Add(provider.NotificationTag);
                }
            }

            // Can the attacker trigger a power on performing a hit? Example use: Decisive Strike of the Battle domain
            if (attacker.RulesetCharacter.UsablePowers.Count > 0)
            {
                foreach (var usablePower in attacker.RulesetCharacter.UsablePowers)
                {
                    var validator = usablePower.PowerDefinition
                        .GetFirstSubFeatureOfType<IReactionAttackModeRestriction>();
                    if (validator != null && !validator.ValidReactionMode(attackMode, rangedAttack, attacker,
                            defender))
                    {
                        continue;
                    }

                    if (!attacker.RulesetCharacter.IsPowerOverriden(usablePower)
                        && attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0
                        && ((usablePower.PowerDefinition.ActivationTime ==
                                RuleDefinitions.ActivationTime.OnAttackHit && attackMode != null)
                            || (usablePower.PowerDefinition.ActivationTime ==
                                RuleDefinitions.ActivationTime.OnAttackHitWithBow && attackMode != null &&
                                attacker.RulesetCharacter.IsWieldingBow())))
                    {
                        var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                        reactionParams.StringParameter = usablePower.PowerDefinition.Name;

                        // Remove trailing numbers for advanced powers
                        if (usablePower.PowerDefinition.OverriddenPower != null)
                        {
                            //reactionParams.StringParameter = reactionParams.StringParameter.Trim(GameLocationBattleManager.digitsToTrim);
                            reactionParams.StringParameter = reactionParams.StringParameter.Trim(digitsToTrim);
                        }

                        var rulesetImplementationService =
                            ServiceRepository.GetService<IRulesetImplementationService>();
                        reactionParams.RulesetEffect =
                            rulesetImplementationService.InstantiateEffectPower(attacker.RulesetCharacter,
                                usablePower, false);
                        reactionParams.TargetCharacters.Add(defender);
                        reactionParams.ActionModifiers.Add(new ActionModifier());
                        reactionParams.IsReactionEffect = true;

                        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                        actionService.ReactToSpendPower(reactionParams);

                        yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                    }
                    else if (attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0
                             && (usablePower.PowerDefinition.ActivationTime ==
                                 RuleDefinitions.ActivationTime.OnAttackSpellHitAutomatic
                                 || (usablePower.PowerDefinition.ActivationTime ==
                                     RuleDefinitions.ActivationTime.OnSneakAttackHit &&
                                     __instance.triggeredAdditionalDamageTags.Contains(TagsDefinitions
                                         .AdditionalDamageSneakAttackTag))))
                    {
                        // This case is for the Rogue Hoodlum
                        var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                        actionParams.StringParameter = usablePower.PowerDefinition.Name;

                        var rulesetImplementationService =
                            ServiceRepository.GetService<IRulesetImplementationService>();
                        actionParams.RulesetEffect =
                            rulesetImplementationService.InstantiateEffectPower(attacker.RulesetCharacter,
                                usablePower, false);
                        actionParams.TargetCharacters.Add(defender);

                        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                        actionService.ExecuteAction(actionParams, null, true);
                    }
                }
            }

            // Can I reduce the damage quantity of this attackMode?
            if (attackMode != null && attackMode.Ranged &&
                defender.GetActionStatus(ActionDefinitions.Id.DeflectMissile, ActionDefinitions.ActionScope.Battle,
                    ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available)
            {
                var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.DeflectMissile);
                reactionParams.ActionModifiers.Add(attackModifier);
                reactionParams.TargetCharacters.Add(attacker);
                var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                actionService.ReactToDeflectMissile(reactionParams);

                yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
            }

            // Can I modify the damage?
            // Basic verifications
            if (defender.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) ==
                ActionDefinitions.ActionStatus.Available)
            {
                // Does it have the proper action?
                if (defender.GetActionStatus(ActionDefinitions.Id.UncannyDodge,
                        ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) ==
                    ActionDefinitions.ActionStatus.Available
                    && defender.PerceivedFoes.Contains(attacker))
                {
                    var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UncannyDodge);
                    reactionParams.ActionModifiers.Add(attackModifier);
                    reactionParams.TargetCharacters.Add(attacker);
                    var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                    var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                    actionService.ReactToUncannyDodge(reactionParams);

                    yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                }

                if (defender.GetActionStatus(ActionDefinitions.Id.LeafScales, ActionDefinitions.ActionScope.Battle,
                        ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available
                    && defender.PerceivedFoes.Contains(attacker)
                    && rangedAttack)
                {
                    var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.LeafScales);
                    reactionParams.ActionModifiers.Add(attackModifier);
                    reactionParams.TargetCharacters.Add(attacker);
                    var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                    var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                    actionService.ReactToLeafScales(reactionParams);

                    yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                }
            }

            // Can the defender retaliate when damaged? This is for Remorhaz & Fireshield
            if (defender.RulesetCharacter != null)
            {
                defender.RulesetCharacter.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(
                    __instance.featuresToBrowseReaction);
                foreach (IDamageAffinityProvider provider in __instance.featuresToBrowseReaction)
                {
                    if (provider.RetaliateWhenHit && attackMode != null)
                    {
                        // Does the range match
                        if ((attackMode.Ranged &&
                             provider.RetaliateProximity == RuleDefinitions.AttackProximity.Range) ||
                            (!attackMode.Ranged &&
                             provider.RetaliateProximity == RuleDefinitions.AttackProximity.Melee))
                        {
                            // In the range distance?
                            if (__instance.IsWithinXCells(attacker, defender, provider.RetaliateRangeCells))
                            {
                                // Just check the power is valid, no need to check the defender ability to use the power as this is automatic
                                if (provider.RetaliatePower != null)
                                {
                                    // Notify the console
                                    defender.RulesetCharacter.DamageRetaliated?.Invoke(defender.RulesetCharacter,
                                        attacker.RulesetCharacter, provider);

                                    // Build the params
                                    var retaliateParams = new CharacterActionParams(defender,
                                        ActionDefinitions.Id.SpendPower, attacker);
                                    var dummyUsablePower =
                                        new RulesetUsablePower(provider.RetaliatePower, null, null);

                                    // Build the active effect
                                    var rulesetImplementationService =
                                        ServiceRepository.GetService<IRulesetImplementationService>();
                                    retaliateParams.RulesetEffect =
                                        rulesetImplementationService.InstantiateEffectPower(
                                            defender.RulesetCharacter, dummyUsablePower, false);
                                    retaliateParams.StringParameter = provider.RetaliatePower.Name;
                                    retaliateParams.IsReactionEffect = true;

                                    // Start the action
                                    var gameLocationActionService =
                                        ServiceRepository.GetService<IGameLocationActionService>();
                                    gameLocationActionService.ExecuteInstantSingleAction(retaliateParams);
                                }
                            }
                        }
                    }
                }

                yield return __instance.HandleReactionToDamage(attacker, defender, attackModifier,
                    actualEffectForms, attackMode);
            }
        }

        //
        // Custom Behavior after damage calculation
        //

        if (rulesetCharacter != null)
        {
            foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackDamageEffect>())
            {
                feature.AfterOnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack,
                    advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
            }
        }

        yield return CustomReactionsContext.TryReactingToDamageWithSpell(attacker, defender, attackModifier,
            attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);

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
                feature.BeforeOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect,
                    actualEffectForms, firstTarget, criticalHit);
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
                feature.AfterOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect,
                    actualEffectForms, firstTarget, criticalHit);
            }
        }

        Global.CriticalHit = false;
    }
}
