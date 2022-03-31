using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;

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
                if (!Main.Settings.EnableMulticlass || !attacker.RulesetCharacter.IsSubstitute)
                {
                    return true;
                }

                var hero = WildshapeContext.GetHero(attacker.RulesetCharacter) as RulesetCharacterHero;

                //
                // original game code from here
                //

                DamageForm damageForm = DamageForm.Get();
                FeatureDefinition featureDefinition = provider as FeatureDefinition;
                if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.Die)
                {
                    int num = provider.DamageDiceNumber;
                    if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
                    {

                        //
                        // patch here to allow FindClassHoldingFeature to work
                        //

                        //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                        RulesetCharacterHero rulesetCharacter = hero;

                        // end patch

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
                                num = provider.GetDiceOfRank(conditionHoldingFeature.EffectLevel);
                        }
                    }
                    if (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
                        attacker.UsedSpecialFeatures[featureDefinition.Name]++;
                    else
                        attacker.UsedSpecialFeatures[featureDefinition.Name] = 1;
                    if (defender.RulesetCharacter != null && provider.FamiliesWithAdditionalDice.Count > 0 && provider.FamiliesWithAdditionalDice.Contains(defender.RulesetCharacter.CharacterFamily))
                        num += provider.FamiliesDiceNumber;
                    damageForm.DieType = provider.DamageDieType;
                    damageForm.DiceNumber = num;
                }

                //
                // patch here as at this point it is confirmed to be a substitute
                //

                //else if (attacker.RulesetCharacter is RulesetCharacterHero && (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage))
                else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)
                
                // end patch
                
                {
                    damageForm.DieType = RuleDefinitions.DieType.D1;
                    damageForm.DiceNumber = 0;
                    damageForm.BonusDamage = 0;
                    if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
                        damageForm.BonusDamage += attacker.RulesetCharacter.GetAttribute("ProficiencyBonus").CurrentValue;
                    if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
                    {
                        int num = 0;
                        foreach (RulesetSpellRepertoire spellRepertoire in attacker.RulesetCharacter.SpellRepertoires)
                        {
                            num = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue);
                            if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                                break;
                        }
                        damageForm.BonusDamage += num;
                    }
                    if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)
                        damageForm.BonusDamage = attacker.RulesetCharacter.TryGetAttributeValue("RageDamage");
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

                //
                // patch here as at this point it is confirmed to be a substitute
                //

                //else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && attacker.RulesetCharacter is RulesetCharacterHero && defender.RulesetCharacter is RulesetCharacterMonster)
                else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && attacker.RulesetCharacter is RulesetCharacterHero&& defender.RulesetCharacter is RulesetCharacterMonster)
               
                // end patch

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
                                break;
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
                        if ((uint)damageForm.DieType > 0U & criticalHit)
                            diceNumber *= 2;
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
                    float brightRange = lightSourceForm.BrightRange;
                    float dimRangeCells = brightRange + lightSourceForm.DimAdditionalRange;
                    defender.RulesetCharacter.PersonalLightSource = new RulesetLightSource(lightSourceForm.Color, brightRange, dimRangeCells, lightSourceForm.GraphicsPrefabAssetGUID, lightSourceForm.LightSourceType, featureDefinition.Name, defender.RulesetCharacter.Guid);
                    defender.RulesetCharacter.PersonalLightSource.Register(true);
                    service.AddCharacterLightSource(defender, defender.RulesetCharacter.PersonalLightSource);
                    RulesetCondition conditionHoldingFeature = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
                    if (conditionHoldingFeature != null)
                        attacker.RulesetCharacter.FindEffectTrackingCondition(conditionHoldingFeature).TrackLightSource(defender.RulesetCharacter, defender.Guid, string.Empty, defender.RulesetCharacter.PersonalLightSource);
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
                var gameLocationBattleManagerType = typeof(GameLocationBattleManager);
                var digitsToTrim = (char[])gameLocationBattleManagerType.GetField("digitsToTrim", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var computeAndNotifyAdditionalDamageMethod = gameLocationBattleManagerType.GetMethod("ComputeAndNotifyAdditionalDamage", BindingFlags.NonPublic | BindingFlags.Instance);
                var handleReactionToDamageMethod = gameLocationBattleManagerType.GetMethod("HandleReactionToDamage", BindingFlags.NonPublic | BindingFlags.Instance);
                var waitForReactionsMethod = gameLocationBattleManagerType.GetMethod("WaitForReactions", BindingFlags.NonPublic | BindingFlags.Instance);

                //
                // original game code from here
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
                                    // patch here to allow FindClassHoldingFeature to work
                                    //

                                    //RulesetCharacterHero rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                                    RulesetCharacterHero rulesetCharacter = WildshapeContext.GetHero(attacker.RulesetCharacter) as RulesetCharacterHero;

                                    // end patch

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
                                bool flag5 = false; // finesse
                                bool flag6 = false; // melee
                                bool flag7 = false; // range

                                ItemDefinition element = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                if (element != null && element.IsWeapon)
                                {
                                    if (DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(element.WeaponDescription.WeaponType).WeaponProximity == RuleDefinitions.AttackProximity.Melee && !rangedAttack)
                                    {
                                        flag6 = true;
                                        if (element.WeaponDescription.WeaponTags.Contains("Finesse"))
                                            flag5 = true;
                                    }
                                    else
                                        flag7 = true;
                                }

                                //
                                // patch here to always recognize wildshape attack as a melee non finess one
                                //

                                else if (attacker.RulesetCharacter.IsSubstitute)
                                {
                                    flag6 = true;
                                }

                                // end patch 

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
