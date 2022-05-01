using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
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

            DamageForm damageForm = DamageForm.Get();
            FeatureDefinition featureDefinition = provider as FeatureDefinition;

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.Die)
            {
                int num = provider.DamageDiceNumber;

                if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
                {
                    // game code doesn't consider heroes in wildshape form
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
            // game code doesn't consider heroes in wildshape form
            //else if (attacker.RulesetCharacter is RulesetCharacterHero && (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)))
            else if ((attacker.RulesetCharacter is RulesetCharacterHero || attacker.RulesetCharacter.OriginalFormCharacter is RulesetCharacterHero) && (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)))
            {
                damageForm.DieType = RuleDefinitions.DieType.D1;
                damageForm.DiceNumber = 0;
                damageForm.BonusDamage = 0;

                if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
                {
                    // game code doesn't consider heroes in wildshape form
                    //damageForm.BonusDamage += (attacker.RulesetCharacter as RulesetCharacterHero).GetAttribute("ProficiencyBonus").CurrentValue;
                    damageForm.BonusDamage += attacker.RulesetCharacter.GetAttribute("ProficiencyBonus").CurrentValue;
                }

                if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
                {
                    int num = 0;

                    // under a MC scenario best I can do is use the max spell bonus modifier across all caster classes
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
            // game code doesn't consider heroes in wildshape form
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

            // for some reason only Die value determination increments feature uses
            // this fix increments for all other types otherwise additional damage features that use other types
            // (like Elemental Forms of Elementalist Warlock use PB as dmage bonus) will trigger on each hit
            // regardless of usage limit setting

            if (Main.Settings.BugFixCorrectlyCalculateDamageOnMultipleHits
                && provider.DamageValueDetermination != RuleDefinitions.AdditionalDamageValueDetermination.Die)
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
}
