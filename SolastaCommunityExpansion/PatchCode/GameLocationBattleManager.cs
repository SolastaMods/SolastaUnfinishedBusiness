using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.PatchCode;

internal static class GameLocationBattleManagerTweaks
{
    /**
     * This method is almost completely original game source provided by TA
     * All changes made by CE mod should be clearly marked to easy future updates
     */
     public static void ComputeAndNotifyAdditionalDamage(GameLocationBattleManager instance, GameLocationCharacter attacker, GameLocationCharacter defender, IAdditionalDamageProvider provider, List<EffectForm> actualEffectForms, CharacterActionParams reactionParams, RulesetAttackMode attackMode, bool criticalHit)
    {
        DamageForm additionalDamageForm = DamageForm.Get();
        FeatureDefinition featureDefinition = provider as FeatureDefinition;
        
        /*
         * ######################################
         * [CE] EDIT START
         * Support for wild-shaped characters
         */
                
        //[CE] Store original RulesetCharacterHero for future use
        var hero = attacker.RulesetCharacter as RulesetCharacterHero ??
                   attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

        /*
         * Support for wild-shaped characters
         * [CE] EDIT END
         * ######################################
         */

        // What is the method to determine the amount of damage?
        if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.Die)
        {
            int diceNumber = provider.DamageDiceNumber;

            if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
            {
                // Find the character class which triggered this
                /*
                 * ######################################
                 * [CE] EDIT START
                 * Support for wild-shaped characters
                 */
                
                // [CE] comment-out this local variable, so that one declared above, which accounts for wild-shape, is used
                // RulesetCharacterHero hero = attacker.RulesetCharacter as RulesetCharacterHero;
                
                // [CE] commented-out original code
                //CharacterClassDefinition classDefinition = hero.FindClassHoldingFeature(featureDefinition);
                
                // Use null-coalescing operator to ward against possible `NullReferenceException`
                var classDefinition = hero?.FindClassHoldingFeature(featureDefinition);
                
                /*
                 * Support for wild-shaped characters
                 * [CE] EDIT END
                 * ######################################
                 */
                if (classDefinition != null)
                {
                    int classLevel = hero.ClassesAndLevels[classDefinition];
                    diceNumber = provider.GetDiceOfRank(classLevel);
                }
            }
            /*
             * ######################################
             * [CE] EDIT START
             * Support for `CharacterLevel` damage progression
             */
            else if ((ExtraAdditionalDamageAdvancement) provider.DamageAdvancement ==
                     ExtraAdditionalDamageAdvancement.CharacterLevel)
            {
                var rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero ??
                                       attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

                if (rulesetCharacter != null)
                {
                    var characterLevel =
                        rulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
                    diceNumber = provider.GetDiceOfRank(characterLevel);
                }
            }
            /*
             * Support for `CharacterLevel` damage progression
             * [CE] EDIT END
             * ######################################
             */
            else if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.SlotLevel)
            {
                if (reactionParams != null)
                {
                    diceNumber = provider.GetDiceOfRank(reactionParams.IntParameter);
                }
                else
                {
                    RulesetCondition condition = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
                    if (condition != null)
                    {
                        diceNumber = provider.GetDiceOfRank(condition.EffectLevel);
                    }
                }
            }

            // Some specific families may receive more dice (example paladin smiting undead/fiends)
            if (defender.RulesetCharacter != null && provider.FamiliesWithAdditionalDice.Count > 0 && provider.FamiliesWithAdditionalDice.Contains(defender.RulesetCharacter.CharacterFamily))
            {
                diceNumber += provider.FamiliesDiceNumber;
            }

            additionalDamageForm.DieType = provider.DamageDieType;
            additionalDamageForm.DiceNumber = diceNumber;
        }

        /*
        * ######################################
        * [CE] EDIT START
        * Support for wild-shaped characters
        */
        
        //Commented out original check
        //else if (attacker.RulesetCharacter is RulesetCharacterHero &&
        
        //check previously saved hero variable to allow wild-shaped heroes to count for these bonuses
        else if (hero != null &&
        
        /*
         * Support for wild-shaped characters
         * [CE] EDIT END
         * ######################################
         */
            (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus
            || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus
            || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus
            || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage))
        {
            additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = 0;

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
            {
                /*
                 * ######################################
                 * [CE] EDIT START
                 * Support for wild-shaped characters
                 */
                
                //Commented out original check
                // additionalDamageForm.BonusDamage += (attacker.RulesetCharacter as RulesetCharacterHero).GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

                //use previously saved original RulesetCharacterHero
                additionalDamageForm.BonusDamage += hero.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

                /*
                 * Support for wild-shaped characters
                 * [CE] EDIT END
                 * ######################################
                 */
            }

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus || provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusAndSpellcastingBonus)
            {
                // Look for the Spell Repertoire
                int spellBonus = 0;
                foreach (RulesetSpellRepertoire spellRepertoire in attacker.RulesetCharacter.SpellRepertoires)
                {
                    spellBonus = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue);

                    // Stop if this is a class repertoire
                    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                    {
                        break;
                    }
                }

                additionalDamageForm.BonusDamage += spellBonus;
            }

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)
            {
                additionalDamageForm.BonusDamage = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.RageDamage);
            }
        }
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusOfSource)
        {
            // Try to find the condition granting the provider
            RulesetCondition holdingCondition = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
            RulesetCharacter sourceCharacter = null;
            if (holdingCondition != null && RulesetEntity.TryGetEntity<RulesetCharacter>(holdingCondition.SourceGuid, out sourceCharacter))
            {
                additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
                additionalDamageForm.DiceNumber = 0;
                additionalDamageForm.BonusDamage = sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            }
        }
        /*
         * ######################################
         * [CE] EDIT START
         * Support for wild-shaped characters
         */
                
        //Commented out original check
        // else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && attacker.RulesetCharacter is RulesetCharacterHero && defender.RulesetCharacter is RulesetCharacterMonster)
        
        // [CE] use previously saved hero variable to check if attacker is actually a  hero, this allows for wild-shaped charaters to count
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && hero != null && defender.RulesetCharacter is RulesetCharacterMonster)
            
        /*
         * Support for wild-shaped characters
         * [CE] EDIT END
         * ######################################
         */
        {
            additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = ServiceRepository.GetService<IGameLoreService>().GetCreatureKnowledgeLevel(defender.RulesetCharacter).AdditionalDamage;
        }
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.BrutalCriticalDice)
        {
            bool useVersatileDamage = attackMode != null && attackMode.UseVersatileDamage;
            DamageForm damageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
            additionalDamageForm.DieType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;
            additionalDamageForm.DiceNumber = attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.BrutalCriticalDice);
            additionalDamageForm.BonusDamage = 0;
        }
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
        {
            bool useVersatileDamage = attackMode != null && attackMode.UseVersatileDamage;
            DamageForm damageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
            additionalDamageForm.DieType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;
            additionalDamageForm.DiceNumber = 1;
            additionalDamageForm.BonusDamage = 0;
        }
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.HalfAbilityScoreBonus)
        {
            if (attackMode != null)
            {
                string abilityScore = attackMode.AbilityScore;
                int halfUp = Mathf.CeilToInt(0.5f * (float)AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.TryGetAttributeValue(abilityScore)));
                if (halfUp > 0)
                {
                    additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
                    additionalDamageForm.DiceNumber = 0;
                    additionalDamageForm.BonusDamage = halfUp;
                }
            }
        }
        else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.None)
        {
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = 0;
        }

        additionalDamageForm.IgnoreCriticalDoubleDice = provider.IgnoreCriticalDoubleDice;
        additionalDamageForm.IgnoreSpellAdvancementDamageDice = true;

        // Account the use
        if (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
        {
            attacker.UsedSpecialFeatures[featureDefinition.Name]++;
        }
        else
        {
            attacker.UsedSpecialFeatures[featureDefinition.Name] = 1;
        }

        if (additionalDamageForm.DiceNumber > 0 || additionalDamageForm.BonusDamage > 0)
        {
            // Add the new damage form
            switch (provider.AdditionalDamageType)
            {
                case RuleDefinitions.AdditionalDamageType.SameAsBaseDamage:
                    additionalDamageForm.DamageType = EffectForm.GetFirstDamageForm(actualEffectForms).DamageType;
                    break;

                case RuleDefinitions.AdditionalDamageType.Specific:
                    additionalDamageForm.DamageType = provider.SpecificDamageType;
                    break;

                case RuleDefinitions.AdditionalDamageType.AncestryDamageType:
                    attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(FeatureDefinitionAncestry.FeaturesToBrowse, null);

                    // Pick the first one
                    if (FeatureDefinitionAncestry.FeaturesToBrowse.Count > 0)
                    {
                        additionalDamageForm.DamageType = (FeatureDefinitionAncestry.FeaturesToBrowse[0] as FeatureDefinitionAncestry).DamageType;
                    }

                    break;
            }

            // For ancestry damage, add to the existing / matching damage, instead of add a new effect form
            if (provider.AdditionalDamageType == RuleDefinitions.AdditionalDamageType.AncestryDamageType
                && provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus)
            {
                foreach (EffectForm effectForm in actualEffectForms)
                {
                    if (effectForm.FormType == EffectForm.EffectFormType.Damage && effectForm.DamageForm.DamageType == additionalDamageForm.DamageType)
                    {
                        effectForm.DamageForm.BonusDamage += additionalDamageForm.BonusDamage;
                    }
                }
            }
            else
            {
                // Add a new effect form
                EffectForm newEffectForm = EffectForm.GetFromDamageForm(additionalDamageForm);

                // Specific saving throw?
                if (provider.HasSavingThrow)
                {
                    // This additional damage will override the saving throw for the whole attack
                    newEffectForm.SavingThrowAffinity = provider.DamageSaveAffinity;
                    IRulesetImplementationService rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                    int saveDC = rulesetImplementationService.ComputeSavingThrowDC(attacker.RulesetCharacter, provider);
                    newEffectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility, saveDC, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                }

                actualEffectForms.Add(newEffectForm);
            }

            // Notify observers
            if (attacker.RulesetCharacter.AdditionalDamageGenerated != null)
            {
                // We want to include doubling the dice for a critical hit
                int diceNumber = additionalDamageForm.DiceNumber;
                if (additionalDamageForm.DieType != RuleDefinitions.DieType.D1 && criticalHit && !additionalDamageForm.IgnoreCriticalDoubleDice)
                {
                    diceNumber *= 2;
                }

                // Handle bardic inspiration die override
                if (additionalDamageForm.OverrideWithBardicInspirationDie && attacker.RulesetCharacter.GetBardicInspirationDieValue() != RuleDefinitions.DieType.D1)
                {
                    additionalDamageForm.DieType = attacker.RulesetCharacter.GetBardicInspirationDieValue();
                }

                attacker.RulesetCharacter.AdditionalDamageGenerated.Invoke(attacker.RulesetCharacter, defender.RulesetActor, additionalDamageForm.DieType, diceNumber, additionalDamageForm.BonusDamage, provider.NotificationTag);
            }
        }

        // Do I need to perform condition operations?
        if (provider.ConditionOperations.Count > 0)
        {
            foreach (ConditionOperationDescription conditionOperation in provider.ConditionOperations)
            {
                EffectForm newEffectForm = new EffectForm();
                newEffectForm.FormType = EffectForm.EffectFormType.Condition;
                newEffectForm.ConditionForm = new ConditionForm();
                newEffectForm.ConditionForm.ConditionDefinition = conditionOperation.ConditionDefinition;
                newEffectForm.ConditionForm.Operation = conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add ? ConditionForm.ConditionOperation.Add : ConditionForm.ConditionOperation.Remove;
                newEffectForm.CanSaveToCancel = conditionOperation.CanSaveToCancel;
                newEffectForm.SaveOccurence = conditionOperation.SaveOccurence;

                if (conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add && provider.HasSavingThrow)
                {
                    // This additional damage will override the saving throw for the whole attack
                    newEffectForm.SavingThrowAffinity = conditionOperation.SaveAffinity;
                    IRulesetImplementationService rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                    int saveDC = rulesetImplementationService.ComputeSavingThrowDC(attacker.RulesetCharacter, provider);
                    newEffectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility, saveDC, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                }

                actualEffectForms.Add(newEffectForm);
            }
        }

        // Do I need to add a light source?
        if (provider.AddLightSource && defender.RulesetCharacter != null && defender.RulesetCharacter.PersonalLightSource == null)
        {
            LightSourceForm lightSourceForm = provider.LightSourceForm;

            IGameLocationVisibilityService visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            float brightRange = (float)lightSourceForm.BrightRange;
            float dimRange = brightRange + (float)lightSourceForm.DimAdditionalRange;
            defender.RulesetCharacter.PersonalLightSource = new RulesetLightSource(
                lightSourceForm.Color,
                brightRange,
                dimRange,
                lightSourceForm.GraphicsPrefabAssetGUID,
                lightSourceForm.LightSourceType,
                featureDefinition.Name,
                targetGuid: defender.RulesetCharacter.Guid);
            defender.RulesetCharacter.PersonalLightSource.Register(true);

            visibilityService.AddCharacterLightSource(defender, defender.RulesetCharacter.PersonalLightSource);

            RulesetCondition holdingCondition = attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
            if (holdingCondition != null)
            {
                RulesetEffect effect = attacker.RulesetCharacter.FindEffectTrackingCondition(holdingCondition);
                effect.TrackLightSource(defender.RulesetCharacter, defender.Guid, string.Empty, defender.RulesetCharacter.PersonalLightSource);
            }
        }

        //CHANGE: replaced `this` with `instance`
        instance.AdditionalDamageProviderActivated?.Invoke(attacker, defender, provider);
    }
}