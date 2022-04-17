using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace SolastaMulticlass.Patches.Wildshape
{
    internal static class GameLocationBattleManagerPatcher
    {
        // fixes additional damage calculation under wildshape (i.e.: rage, etc.)
        [HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
        internal static class GameLocationBattleManagerComputeAndNotifyAdditionalDamage
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
                if (!attacker.RulesetCharacter.IsSubstitute)
                {
                    return true;
                }

                //
                // GAME CODE FROM HERE
                //

                DamageForm damageForm = DamageForm.Get();
                FeatureDefinition featureDefinition = provider as FeatureDefinition;

                if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.Die)
                {
                    int num = provider.DamageDiceNumber;

                    if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
                    {
                        //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                        RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero ?? attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;
                        CharacterClassDefinition classHoldingFeature = rulesetCharacter.FindClassHoldingFeature(featureDefinition);

                        if (classHoldingFeature != null)
                        {
                            int classesAndLevel = rulesetCharacter.ClassesAndLevels[classHoldingFeature];
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
                            RulesetCondition conditionHoldingFeature = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
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

                    if (defender.RulesetCharacter != null && provider.FamiliesWithAdditionalDice.Count > 0 && provider.FamiliesWithAdditionalDice.Contains(defender.RulesetCharacter.CharacterFamily))
                    {
                        num += provider.FamiliesDiceNumber;
                    }

                    damageForm.DieType = provider.DamageDieType;
                    damageForm.DiceNumber = num;
                }
                //else if (attacker.RulesetCharacter is RulesetCharacterHero && (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)))
                else if ((attacker.RulesetCharacter is RulesetCharacterHero || attacker.RulesetCharacter.OriginalFormCharacter is RulesetCharacterHero) && (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)))
                {
                    damageForm.DieType = RuleDefinitions.DieType.D1;
                    damageForm.DiceNumber = 0;
                    damageForm.BonusDamage = 0;

                    if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
                    {
                        //damageForm.BonusDamage += (attacker.RulesetCharacter as RulesetCharacterHero).GetAttribute("ProficiencyBonus").CurrentValue;
                        damageForm.BonusDamage += attacker.RulesetCharacter.GetAttribute("ProficiencyBonus").CurrentValue;
                    }

                    if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
                    {
                        int num = 0;

                        //foreach (RulesetSpellRepertoire spellRepertoire in attacker.RulesetCharacter.SpellRepertoires)
                        foreach (RulesetSpellRepertoire spellRepertoire in attacker.RulesetCharacter.SpellRepertoires
                            .Where(x => x.SpellCastingFeature.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Class))
                        {
                            //num = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue);
                            //if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                            //    break;
                            num = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue), num);
                        }

                        damageForm.BonusDamage += num;
                    }

                    if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)
                    {
                        damageForm.BonusDamage = attacker.RulesetCharacter.TryGetAttributeValue("RageDamage");
                    }
                }
                else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusOfSource)
                {
                    RulesetCondition conditionHoldingFeature = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);

                    if (conditionHoldingFeature != null && RulesetEntity.TryGetEntity(conditionHoldingFeature.SourceGuid, out RulesetCharacter entity))
                    {
                        damageForm.DieType = RuleDefinitions.DieType.D1;
                        damageForm.DiceNumber = 0;
                        damageForm.BonusDamage = entity.TryGetAttributeValue("ProficiencyBonus");
                    }
                }
                //else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && attacker.RulesetCharacter is RulesetCharacterHero && defender.RulesetCharacter is RulesetCharacterMonster)
                else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && (attacker.RulesetCharacter is RulesetCharacterHero || attacker.RulesetCharacter.OriginalFormCharacter is RulesetCharacterHero) && defender.RulesetCharacter is RulesetCharacterMonster)
                {
                    damageForm.DieType = RuleDefinitions.DieType.D1;
                    damageForm.DiceNumber = 0;
                    damageForm.BonusDamage = ServiceRepository.GetService<IGameLoreService>().GetCreatureKnowledgeLevel(defender.RulesetCharacter).AdditionalDamage;
                }
                else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.BrutalCriticalDice)
                {
                    bool flag = attackMode != null && attackMode.UseVersatileDamage;
                    DamageForm firstDamageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
                    damageForm.DieType = flag ? firstDamageForm.VersatileDieType : firstDamageForm.DieType;
                    damageForm.DiceNumber = attacker.RulesetCharacter.TryGetAttributeValue("BrutalCriticalDice");
                    damageForm.BonusDamage = 0;
                }
                else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
                {
                    bool flag = attackMode != null && attackMode.UseVersatileDamage;
                    DamageForm firstDamageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
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
                            attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(FeatureDefinitionAncestry.FeaturesToBrowse);
                            if (FeatureDefinitionAncestry.FeaturesToBrowse.Count > 0)
                            {
                                damageForm.DamageType = (FeatureDefinitionAncestry.FeaturesToBrowse[0] as FeatureDefinitionAncestry).DamageType;
                            }

                            break;
                    }

                    EffectForm fromDamageForm = EffectForm.GetFromDamageForm(damageForm);

                    if (provider.HasSavingThrow)
                    {
                        fromDamageForm.SavingThrowAffinity = provider.DamageSaveAffinity;
                        int savingThrowDc = ServiceRepository.GetService<IRulesetImplementationService>().ComputeSavingThrowDC(attacker.RulesetCharacter, provider);
                        fromDamageForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility, savingThrowDc, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                    }

                    actualEffectForms.Add(fromDamageForm);

                    if (attacker.RulesetCharacter.AdditionalDamageGenerated != null)
                    {
                        int diceNumber = damageForm.DiceNumber;

                        if ((uint)damageForm.DieType > 0U & criticalHit && !damageForm.IgnoreCriticalDoubleDice)
                        {
                            diceNumber *= 2;
                        }

                        attacker.RulesetCharacter.AdditionalDamageGenerated(attacker.RulesetCharacter, defender.RulesetActor, damageForm.DieType, diceNumber, damageForm.BonusDamage, provider.NotificationTag);
                    }
                }

                if (provider.ConditionOperations.Count > 0)
                {
                    foreach (ConditionOperationDescription conditionOperation in provider.ConditionOperations)
                    {
                        EffectForm effectForm = new EffectForm()
                        {
                            FormType = EffectForm.EffectFormType.Condition,
                            ConditionForm = new ConditionForm()
                        };

                        effectForm.ConditionForm.ConditionDefinition = conditionOperation.ConditionDefinition;
                        effectForm.ConditionForm.Operation = conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add ? ConditionForm.ConditionOperation.Add : ConditionForm.ConditionOperation.Remove;
                        effectForm.CanSaveToCancel = conditionOperation.CanSaveToCancel;
                        effectForm.SaveOccurence = conditionOperation.SaveOccurence;

                        if (conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add && provider.HasSavingThrow)
                        {
                            effectForm.SavingThrowAffinity = conditionOperation.SaveAffinity;
                            int savingThrowDc = ServiceRepository.GetService<IRulesetImplementationService>().ComputeSavingThrowDC(attacker.RulesetCharacter, provider);
                            effectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility, savingThrowDc, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                        }

                        actualEffectForms.Add(effectForm);
                    }
                }
                if (provider.AddLightSource && defender.RulesetCharacter != null && defender.RulesetCharacter.PersonalLightSource == null)
                {
                    LightSourceForm lightSourceForm = provider.LightSourceForm;
                    IGameLocationVisibilityService service = ServiceRepository.GetService<IGameLocationVisibilityService>();
                    float brightRange = (float)lightSourceForm.BrightRange;
                    float dimRangeCells = brightRange + (float)lightSourceForm.DimAdditionalRange;
                    defender.RulesetCharacter.PersonalLightSource = new RulesetLightSource(lightSourceForm.Color, brightRange, dimRangeCells, lightSourceForm.GraphicsPrefabAssetGUID, lightSourceForm.LightSourceType, featureDefinition.Name, defender.RulesetCharacter.Guid);
                    defender.RulesetCharacter.PersonalLightSource.Register(true);
                    GameLocationCharacter character = defender;
                    RulesetLightSource personalLightSource = defender.RulesetCharacter.PersonalLightSource;
                    service.AddCharacterLightSource(character, personalLightSource);
                    RulesetCondition conditionHoldingFeature = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
                    if (conditionHoldingFeature != null)
                    {
                        attacker.RulesetCharacter.FindEffectTrackingCondition(conditionHoldingFeature).TrackLightSource(defender.RulesetCharacter, defender.Guid, string.Empty, defender.RulesetCharacter.PersonalLightSource);
                    }
                }

                __instance.AdditionalDamageProviderActivated?.Invoke(attacker, defender, provider);

                return false;
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
                if (!attacker.RulesetCharacter.IsSubstitute)
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

                    foreach (FeatureDefinition featureDefinition in ___featuresToBrowseReaction)
                    {
                        IAdditionalDamageProvider provider = featureDefinition as IAdditionalDamageProvider;

                        // Some additional damage only work with attack modes (Hunter's Mark)
                        if (provider.AttackModeOnly && attackMode == null)
                        {
                            continue;
                        }

                        // Trigger method
                        bool validTrigger = false;
                        bool validUses = true;
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
                                foreach (KeyValuePair<string, int> kvp in attacker.UsedSpecialFeatures)
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
                            // Typical for Sneak Attack
                            if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly && attackMode != null)
                            {
                                if (advantageType == RuleDefinitions.AdvantageType.Advantage || (advantageType != RuleDefinitions.AdvantageType.Disadvantage && __instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker)))
                                {
                                    validTrigger = true;
                                }
                            }
                            else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot && attackModifier != null && attackModifier.Proximity == RuleDefinitions.AttackProximity.Melee)
                            {
                                // This is used for Divine Smite
                                // Look for the spellcasting feature holding the smite

                                //
                                // patch here
                                //
                                //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                                RulesetCharacterHero hero = attacker.RulesetCharacter as RulesetCharacterHero
                                    ?? attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;
                                CharacterClassDefinition classDefinition = hero.FindClassHoldingFeature(featureDefinition);
                                RulesetSpellRepertoire selectedSpellRepertoire = null;
                                foreach (RulesetSpellRepertoire spellRepertoire in hero.SpellRepertoires)
                                {
                                    if (spellRepertoire.SpellCastingClass == classDefinition)
                                    {
                                        bool atLeastOneSpellSlotAvailable = false;
                                        for (int spellLevel = 1; spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel; spellLevel++)
                                        {
                                            spellRepertoire.GetSlotsNumber(spellLevel, out var remaining, out var max);
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
                                            IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();

                                            int previousReactionCount = actionService.PendingReactionRequestGroups.Count;
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
                                    ItemDefinition itemDefinition = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                    if (itemDefinition != null
                                        && itemDefinition.IsWeapon)
                                    {
                                        WeaponTypeDefinition weaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(itemDefinition.WeaponDescription.WeaponType);
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

                        // Check required properties if needed
                        bool validProperty = true;
                        if (validTrigger && provider.RequiredProperty != RuleDefinitions.AdditionalDamageRequiredProperty.None && attackMode != null)
                        {
                            bool finesse = false;
                            bool melee = false;
                            bool range = false;
                            ItemDefinition itemDefinition = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                            if (itemDefinition != null
                                && itemDefinition.IsWeapon)
                            {
                                WeaponTypeDefinition weaponTypeDefinition = DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(itemDefinition.WeaponDescription.WeaponType);
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
                        foreach (RulesetUsablePower usablePower in attacker.RulesetCharacter.UsablePowers)
                        {
                            if (!attacker.RulesetCharacter.IsPowerOverriden(usablePower)
                                && attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0
                                && ((usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHit && attackMode != null)
                                    || (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHitWithBow && attackMode != null && attacker.RulesetCharacter.IsWieldingBow())))
                            {
                                CharacterActionParams reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                                reactionParams.StringParameter = usablePower.PowerDefinition.Name;

                                // Remove trailing numbers for advanced powers
                                if (usablePower.PowerDefinition.OverriddenPower != null)
                                {
                                    //reactionParams.StringParameter = reactionParams.StringParameter.Trim(GameLocationBattleManager.digitsToTrim);
                                    reactionParams.StringParameter = reactionParams.StringParameter.Trim(digitsToTrim);
                                }

                                IRulesetImplementationService rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                                reactionParams.RulesetEffect = rulesetImplementationService.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                                reactionParams.TargetCharacters.Add(defender);
                                reactionParams.IsReactionEffect = true;

                                IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();
                                int previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                                actionService.ReactToSpendPower(reactionParams);

                                //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                                yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });
                            }
                            else if (attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0
                                && ((usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackSpellHitAutomatic)
                                    || (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnSneakAttackHit && ___triggeredAdditionalDamageTags.Contains(TagsDefinitions.AdditionalDamageSneakAttackTag))))
                            {
                                // This case is for the Rogue Hoodlum
                                CharacterActionParams actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower);
                                actionParams.StringParameter = usablePower.PowerDefinition.Name;

                                IRulesetImplementationService rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                                actionParams.RulesetEffect = rulesetImplementationService.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                                actionParams.TargetCharacters.Add(defender);

                                IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();
                                actionService.ExecuteAction(actionParams, null, true);
                            }
                        }
                    }

                    // Can I reduce the damage quantity of this attackMode?
                    if (attackMode != null && attackMode.Ranged && defender.GetActionStatus(ActionDefinitions.Id.DeflectMissile, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available)
                    {
                        CharacterActionParams reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.DeflectMissile);
                        reactionParams.ActionModifiers.Add(attackModifier);
                        reactionParams.TargetCharacters.Add(attacker);
                        IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();

                        int previousReactionCount = actionService.PendingReactionRequestGroups.Count;
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
                            CharacterActionParams reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UncannyDodge);
                            reactionParams.ActionModifiers.Add(attackModifier);
                            reactionParams.TargetCharacters.Add(attacker);
                            IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();

                            int previousReactionCount = actionService.PendingReactionRequestGroups.Count;
                            actionService.ReactToUncannyDodge(reactionParams);

                            //yield return __instance.WaitForReactions(attacker, actionService, previousReactionCount);
                            yield return waitForReactionsMethod.Invoke(__instance, new object[] { attacker, actionService, previousReactionCount });
                        }

                        if (defender.GetActionStatus(ActionDefinitions.Id.LeafScales, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available
                            && defender.PerceivedFoes.Contains(attacker)
                            && rangedAttack)
                        {
                            CharacterActionParams reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.LeafScales);
                            reactionParams.ActionModifiers.Add(attackModifier);
                            reactionParams.TargetCharacters.Add(attacker);
                            IGameLocationActionService actionService = ServiceRepository.GetService<IGameLocationActionService>();

                            int previousReactionCount = actionService.PendingReactionRequestGroups.Count;
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
                                            CharacterActionParams retaliateParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower, attacker);
                                            RulesetUsablePower dummyUsablePower = new RulesetUsablePower(provider.RetaliatePower, null, null);

                                            // Build the active effect
                                            IRulesetImplementationService rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                                            retaliateParams.RulesetEffect = rulesetImplementationService.InstantiateEffectPower(defender.RulesetCharacter, dummyUsablePower, false);
                                            retaliateParams.StringParameter = provider.RetaliatePower.Name;
                                            retaliateParams.IsReactionEffect = true;

                                            // Start the action
                                            IGameLocationActionService gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
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
            }
        }
    }
}
