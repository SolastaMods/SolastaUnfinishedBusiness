using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal static class GLBM
{
    // ReSharper disable once InconsistentNaming
    private static int ComputeSavingThrowDC(GameLocationCharacter glc, IAdditionalDamageProvider provider)
    {
        var character = glc.RulesetCharacter;

        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (provider.DcComputation)
        {
            case RuleDefinitions.EffectDifficultyClassComputation.FixedValue:
                return provider.SavingThrowDC;

            case RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature:
            {
                //BUGFIX: original game code considers first repertoire
                return character.SpellRepertoires
                    .Select(x => x.SaveDC)
                    .Max();
            }
            case RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency:
            {
                //BUGFIX: original game code considers first repertoire
                return character.SpellRepertoires
                    .Select(x => RuleDefinitions.ComputeAbilityScoreBasedDC(
                        character.TryGetAttributeValue(x.SpellCastingFeature.SpellcastingAbility),
                        character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus)))
                    .Max();
            }
            case RuleDefinitions.EffectDifficultyClassComputation.Ki:
                return RuleDefinitions.ComputeAbilityScoreBasedDC(
                    character.TryGetAttributeValue(AttributeDefinitions.Wisdom),
                    character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus));

            case RuleDefinitions.EffectDifficultyClassComputation.BreathWeapon:
                return RuleDefinitions.ComputeAbilityScoreBasedDC(
                    character.TryGetAttributeValue(AttributeDefinitions.Constitution),
                    character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus));

            case RuleDefinitions.EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency:
                return RuleDefinitions.ComputeAbilityScoreBasedDC(
                    character.TryGetAttributeValue(provider.SavingThrowDCAbilityModifier),
                    character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus));

            default:
                return 10;
        }
    }

    internal static RuleDefinitions.RollOutcome GetAttackResult(int rawRoll, int modifier, RulesetCharacter defender)
    {
        if (rawRoll == RuleDefinitions.DiceMaxValue[(int)RuleDefinitions.DieType.D20])
        {
            return RuleDefinitions.RollOutcome.CriticalSuccess;
        }

        if (rawRoll == RuleDefinitions.DiceMinValue[(int)RuleDefinitions.DieType.D20])
        {
            return RuleDefinitions.RollOutcome.CriticalFailure;
        }

        var defenderArmorClass = defender.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
        return rawRoll + modifier >= defenderArmorClass
            ? RuleDefinitions.RollOutcome.Success
            : RuleDefinitions.RollOutcome.Failure;
    }

    /**
     * This method is almost completely original game source provided by TA (1.4.8)
     * All changes made by CE mod should be clearly marked for easy future updates
     */
    public static void ComputeAndNotifyAdditionalDamage(
        GameLocationBattleManager instance,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        IAdditionalDamageProvider provider,
        List<EffectForm> actualEffectForms,
        CharacterActionParams reactionParams,
        RulesetAttackMode attackMode,
        bool criticalHit)
    {
        var additionalDamageForm = DamageForm.Get();
        var featureDefinition = provider as FeatureDefinition;

        /*
         * ######################################
         * [CE] EDIT START
         * Support for wild-shaped characters
         */

        if (!featureDefinition)
        {
            return;
        }

        //[CE] Store original RulesetCharacterHero for future use
        var hero = attacker.RulesetCharacter.GetOriginalHero();

        /*
         * Support for wild-shaped characters
         * [CE] EDIT END
         * ######################################
         */

        // What is the method to determine the amount of damage?
        if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.Die)
        {
            var diceNumber = provider.DamageDiceNumber;

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
                if (classDefinition)
                {
                    var classLevel = hero.ClassesAndLevels[classDefinition];
                    diceNumber = provider.GetDiceOfRank(classLevel);
                }
            }
            /*
             * ######################################
             * [CE] EDIT START
             * Support for `CharacterLevel` damage progression
             */
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            else if ((ExtraAdditionalDamageAdvancement)provider.DamageAdvancement ==
                     ExtraAdditionalDamageAdvancement.CharacterLevel)
            {
                if (hero != null)
                {
                    var characterLevel = hero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                    diceNumber = provider.GetDiceOfRank(characterLevel);
                }
            }
            /*
             * Support for `CharacterLevel` damage progression
             * [CE] EDIT END
             * ######################################
             */
            /*
             * ######################################
             * [CE] EDIT START
             * Support for `ConditionAmount` damage progression
             */
            else if ((ExtraAdditionalDamageAdvancement)provider.DamageAdvancement ==
                     ExtraAdditionalDamageAdvancement.ConditionAmount)
            {
                var condition =
                    attacker.RulesetCharacter.FindFirstConditionHoldingFeature((FeatureDefinition)provider);
                if (condition != null)
                {
                    diceNumber = provider.GetDiceOfRank(condition.Amount);
                }
            }
            /*
             * Support for `ConditionAmount` damage progression
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
                    var condition =
                        attacker.RulesetCharacter.FindFirstConditionHoldingFeature((FeatureDefinition)provider);
                    if (condition != null)
                    {
                        diceNumber = provider.GetDiceOfRank(condition.EffectLevel);
                    }
                }
            }

            // Some specific families may receive more dice (example paladin smiting undead/fiends)
            if (defender.RulesetCharacter != null && provider.FamiliesWithAdditionalDice.Count > 0 &&
                provider.FamiliesWithAdditionalDice.Contains(defender.RulesetCharacter.CharacterFamily))
            {
                diceNumber += provider.FamiliesDiceNumber;
            }

            additionalDamageForm.DieType = provider.DamageDieType;
            additionalDamageForm.DiceNumber = diceNumber;
        }
        /*
         * Support for ExtraAdditionalDamageValueDetermination.AttributeModifier
         */
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        else if ((ExtraAdditionalDamageValueDetermination)provider.DamageValueDetermination ==
                 ExtraAdditionalDamageValueDetermination.CustomModifier)
        {
            var customModifierProvider = featureDefinition.GetFirstSubFeatureOfType<CustomModifierProvider>();

            additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = customModifierProvider?.Invoke(hero) ?? 0;
        }
        /*
         * ######################################
         * [CE] EDIT START
         * Support for ExtraAdditionalDamageValueDetermination.FlatWithProgress
         */
        else if ((ExtraAdditionalDamageValueDetermination)provider.DamageValueDetermination ==
                 ExtraAdditionalDamageValueDetermination.FlatWithProgression)
        {
            var bonus = provider.FlatBonus;

            if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
            {
                // Find the character class which triggered this
                var classDefinition = hero!.FindClassHoldingFeature(featureDefinition);

                if (classDefinition)
                {
                    var classLevel = hero.ClassesAndLevels[classDefinition];
                    bonus += provider.GetDiceOfRank(classLevel);
                }
            }
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            else if ((ExtraAdditionalDamageAdvancement)provider.DamageAdvancement ==
                     ExtraAdditionalDamageAdvancement.CharacterLevel)
            {
                if (hero != null)
                {
                    var characterLevel = hero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                    bonus += provider.GetDiceOfRank(characterLevel);
                }
            }
            else if ((ExtraAdditionalDamageAdvancement)provider.DamageAdvancement ==
                     ExtraAdditionalDamageAdvancement.ConditionAmount)
            {
                var condition =
                    attacker.RulesetCharacter.FindFirstConditionHoldingFeature((FeatureDefinition)provider);
                if (condition != null)
                {
                    bonus += provider.GetDiceOfRank(condition.Amount);
                }
            }
            else if (provider.DamageAdvancement == RuleDefinitions.AdditionalDamageAdvancement.SlotLevel)
            {
                if (reactionParams != null)
                {
                    bonus += provider.GetDiceOfRank(reactionParams.IntParameter);
                }
                else
                {
                    var condition =
                        attacker.RulesetCharacter.FindFirstConditionHoldingFeature((FeatureDefinition)provider);
                    if (condition != null)
                    {
                        bonus += provider.GetDiceOfRank(condition.EffectLevel);
                    }
                }
            }

            additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = bonus;
        }
        /*
         * Support for ExtraAdditionalDamageValueDetermination.FlatWithProgress
         * [CE] EDIT END
         * ######################################
         */
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
                 provider.DamageValueDetermination is
                     RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus
                     or RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus or RuleDefinitions
                         .AdditionalDamageValueDetermination
                         .ProficiencyBonusAndSpellcastingBonus
                     or RuleDefinitions.AdditionalDamageValueDetermination.RageDamage
                     or RuleDefinitions.AdditionalDamageValueDetermination.FlatBonus)
        {
            additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = 0;

            if (provider.DamageValueDetermination is RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonus
                or RuleDefinitions.AdditionalDamageValueDetermination
                    .ProficiencyBonusAndSpellcastingBonus)
            {
                /*
                 * ######################################
                 * [CE] EDIT START
                 * Support for wild-shaped characters
                 */

                //Commented out original check
                // additionalDamageForm.BonusDamage += (attacker.RulesetCharacter as RulesetCharacterHero).GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;

                //use previously saved original RulesetCharacterHero
                additionalDamageForm.BonusDamage +=
                    hero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                /*
                 * Support for wild-shaped characters
                 * [CE] EDIT END
                 * ######################################
                 */
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (provider.DamageValueDetermination is RuleDefinitions.AdditionalDamageValueDetermination
                    .SpellcastingBonus or RuleDefinitions.AdditionalDamageValueDetermination
                    .ProficiencyBonusAndSpellcastingBonus)
            {
                // Look for the Spell Repertoire
                var spellBonus = 0;
                foreach (var spellRepertoire in hero.SpellRepertoires)
                {
                    spellBonus = AttributeDefinitions.ComputeAbilityScoreModifier(hero
                        .GetAttribute(spellRepertoire.SpellCastingAbility).CurrentValue);

                    // Stop if this is a class repertoire
                    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
                        FeatureDefinitionCastSpell.CastingOrigin.Class)
                    {
                        break;
                    }
                }

                //TODO: make this a custom feature where we can grab a forced casting attribute
                //for now there are only 3 cases that fall here so hard-coded
                /*
                 * ######################################
                 * [CE] EDIT START
                 * Support 3 exceptional cases here
                 */
                if (featureDefinition == AdditionalDamageInvocationAgonizingBlast ||
                    featureDefinition == AdditionalDamageLifedrinker)
                {
                    spellBonus = AttributeDefinitions.ComputeAbilityScoreModifier(hero
                        .TryGetAttributeValue(AttributeDefinitions.Charisma));
                }
                else if (featureDefinition == AdditionalDamageTraditionShockArcanistArcaneFury)
                {
                    spellBonus = AttributeDefinitions.ComputeAbilityScoreModifier(hero
                        .TryGetAttributeValue(AttributeDefinitions.Intelligence));
                }

                /*
                 * Support 3 exceptional cases here
                 * [CE] EDIT END
                 * ######################################
                 */
                additionalDamageForm.BonusDamage += spellBonus;
            }

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.RageDamage)
            {
                additionalDamageForm.BonusDamage =
                    hero.TryGetAttributeValue(AttributeDefinitions.RageDamage);
            }

            if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.FlatBonus)
            {
                additionalDamageForm.BonusDamage += provider.FlatBonus;
            }
        }
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.ProficiencyBonusOfSource)
        {
            // Try to find the condition granting the provider
            var holdingCondition =
                attacker.RulesetCharacter.FindFirstConditionHoldingFeature((FeatureDefinition)provider);
            if (holdingCondition != null &&
                RulesetEntity.TryGetEntity(holdingCondition.SourceGuid, out RulesetCharacter sourceCharacter))
            {
                additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
                additionalDamageForm.DiceNumber = 0;
                additionalDamageForm.BonusDamage =
                    sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            }
        }
        /*
         * ######################################
         * [CE] EDIT START
         * Support for wild-shaped characters
         */

        //Commented out original check
        // else if (provider.DamageValueDetermination == RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && attacker.RulesetCharacter is RulesetCharacterHero && defender.RulesetCharacter is RulesetCharacterMonster)

        // [CE] use previously saved hero variable to check if attacker is actually a  hero, this allows for wild-shaped characters to count
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.TargetKnowledgeLevel && hero != null &&
                 defender.RulesetCharacter is RulesetCharacterMonster)
            /*
             * Support for wild-shaped characters
             * [CE] EDIT END
             * ######################################
             */
        {
            additionalDamageForm.DieType = RuleDefinitions.DieType.D1;
            additionalDamageForm.DiceNumber = 0;
            additionalDamageForm.BonusDamage = ServiceRepository.GetService<IGameLoreService>()
                .GetCreatureKnowledgeLevel(defender.RulesetCharacter).AdditionalDamage;
        }
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.BrutalCriticalDice)
        {
            var useVersatileDamage = attackMode is { UseVersatileDamage: true };
            var damageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
            additionalDamageForm.DieType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;
            additionalDamageForm.DiceNumber =
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.BrutalCriticalDice);
            additionalDamageForm.BonusDamage = 0;
        }
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
        {
            var useVersatileDamage = attackMode is { UseVersatileDamage: true };
            var damageForm = EffectForm.GetFirstDamageForm(actualEffectForms);
            additionalDamageForm.DieType = useVersatileDamage ? damageForm.VersatileDieType : damageForm.DieType;
            /*
             * ######################################
             * [CE] EDIT START
             * Support for accounting for all damage dice on savage critical
             * Base game uses only 1 dice, which makes Greatsword weak in this case
             */
            //Commented out original code
            //additionalDamageForm.DiceNumber = 1;
            additionalDamageForm.DiceNumber =
                Main.Settings.AccountForAllDiceOnSavageAttack ? damageForm.DiceNumber : 1;
            /*
             * Support for accounting for all damage dice on savage critical
             * [CE] EDIT END
             * ######################################
             */
            additionalDamageForm.BonusDamage = 0;
        }
        else if (provider.DamageValueDetermination ==
                 RuleDefinitions.AdditionalDamageValueDetermination.HalfAbilityScoreBonus)
        {
            if (attackMode != null)
            {
                var abilityScore = attackMode.AbilityScore;
                var halfUp = Mathf.CeilToInt(0.5f *
                                             AttributeDefinitions.ComputeAbilityScoreModifier(
                                                 attacker.RulesetCharacter.TryGetAttributeValue(abilityScore)));
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
#pragma warning disable CA1854
        if (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
#pragma warning restore CA1854
        {
            attacker.UsedSpecialFeatures[featureDefinition.Name]++;
        }
        else
        {
            attacker.UsedSpecialFeatures[featureDefinition.Name] = 1;
        }

        /*
         * ######################################
         * [CE] EDIT START
         * Support for IModifyAdditionalDamageForm
         */
        var originalDamageType = additionalDamageForm.DamageType;

        if (provider is FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage)
        {
            foreach (var modifyAdditionalDamageForm in hero.GetSubFeaturesByType<IModifyAdditionalDamage>())
            {
                modifyAdditionalDamageForm.ModifyAdditionalDamage(
                    attacker, defender, attackMode, featureDefinitionAdditionalDamage, actualEffectForms,
                    ref additionalDamageForm);
            }
        }

        var newDamageType = additionalDamageForm.DamageType;
        /*
         * Support for IModifyAdditionalDamageForm
         * [CE] EDIT END
         * ######################################
         */
        if (additionalDamageForm.DiceNumber > 0 || additionalDamageForm.BonusDamage > 0)
        {
            // Add the new damage form
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (provider.AdditionalDamageType)
            {
                case RuleDefinitions.AdditionalDamageType.SameAsBaseDamage:
                    additionalDamageForm.DamageType = EffectForm.GetFirstDamageForm(actualEffectForms).DamageType;
                    break;

                case RuleDefinitions.AdditionalDamageType.Specific:
                    additionalDamageForm.DamageType = provider.SpecificDamageType;
                    break;

                case RuleDefinitions.AdditionalDamageType.AncestryDamageType:
                    attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(
                        FeatureDefinitionAncestry.FeaturesToBrowse);

                    // Pick the first matching one
                    foreach (var definitionAncestry in FeatureDefinitionAncestry.FeaturesToBrowse
                                 .Select(definition => definition as FeatureDefinitionAncestry)
                                 .Where(definitionAncestry =>
                                     definitionAncestry &&
                                     definitionAncestry.Type == provider.AncestryTypeForDamageType &&
                                     !string.IsNullOrEmpty(definitionAncestry.DamageType)))
                    {
                        additionalDamageForm.DamageType = definitionAncestry.DamageType;
                    }

                    if (string.IsNullOrEmpty(additionalDamageForm.DamageType))
                    {
                        Trace.LogError("Couldn't find relevant ancestry/damage type for " + provider.Name +
                                       " (attacker: " + attacker.Name + ")");
                    }

                    break;
            }

            /*
             * ######################################
             * [CE] EDIT START
             * Support for IModifyAdditionalDamageForm
             */
            if (originalDamageType != newDamageType)
            {
                additionalDamageForm.DamageType = newDamageType;
            }
            /*
             * Support for IModifyAdditionalDamageForm
             * [CE] EDIT END
             * ######################################
             */

            // For ancestry damage, add to the existing / matching damage, instead of add a new effect form
            if (provider.AdditionalDamageType == RuleDefinitions.AdditionalDamageType.AncestryDamageType
                && provider.DamageValueDetermination ==
                RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus)
            {
                foreach (var effectForm in actualEffectForms
                             .Where(effectForm =>
                                 effectForm.FormType == EffectForm.EffectFormType.Damage &&
                                 effectForm.DamageForm.DamageType == additionalDamageForm.DamageType))
                {
                    effectForm.DamageForm.BonusDamage += additionalDamageForm.BonusDamage;
                }
            }
            else
            {
                // Add a new effect form
                var newEffectForm = EffectForm.GetFromDamageForm(additionalDamageForm);

                // Specific saving throw?
                if (provider.HasSavingThrow)
                {
                    // This additional damage will override the saving throw for the whole attack
                    newEffectForm.SavingThrowAffinity = provider.DamageSaveAffinity;
                    // var implementationService =
                    //     ServiceRepository.GetService<IRulesetImplementationService>();
                    // ReSharper disable once InconsistentNaming
                    var saveDC = ComputeSavingThrowDC(attacker, provider);
                    newEffectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility,
                        saveDC, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                }

                actualEffectForms.Add(newEffectForm);
            }

            // Notify observers
            if (attacker.RulesetCharacter.AdditionalDamageGenerated != null)
            {
                // We want to include doubling the dice for a critical hit
                var diceNumber = additionalDamageForm.DiceNumber;
                if (additionalDamageForm.DieType != RuleDefinitions.DieType.D1 && criticalHit &&
                    !additionalDamageForm.IgnoreCriticalDoubleDice)
                {
                    diceNumber *= 2;
                }

                // Handle bardic inspiration die override
                if (additionalDamageForm.OverrideWithBardicInspirationDie &&
                    attacker.RulesetCharacter.GetBardicInspirationDieValue() != RuleDefinitions.DieType.D1)
                {
                    additionalDamageForm.DieType = attacker.RulesetCharacter.GetBardicInspirationDieValue();
                }

                attacker.RulesetCharacter.AdditionalDamageGenerated.Invoke(attacker.RulesetCharacter,
                    defender.RulesetActor, additionalDamageForm.DieType, diceNumber,
                    additionalDamageForm.BonusDamage, provider.NotificationTag);
            }
        }

        // Do I need to perform condition operations?
        if (provider.ConditionOperations.Count > 0)
        {
            foreach (var conditionOperation in provider.ConditionOperations)
            {
                var newEffectForm = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        ConditionDefinition = conditionOperation.ConditionDefinition,
                        Operation = conditionOperation.Operation ==
                                    ConditionOperationDescription.ConditionOperation.Add
                            ? ConditionForm.ConditionOperation.Add
                            : ConditionForm.ConditionOperation.Remove
                    },
                    CanSaveToCancel = conditionOperation.CanSaveToCancel,
                    SaveOccurence = conditionOperation.SaveOccurence
                };

                if (conditionOperation.Operation == ConditionOperationDescription.ConditionOperation.Add &&
                    provider.HasSavingThrow)
                {
                    // This additional damage will override the saving throw for the whole attack
                    newEffectForm.SavingThrowAffinity = conditionOperation.SaveAffinity;
                    // var implementationService =
                    //     ServiceRepository.GetService<IRulesetImplementationService>();
                    // ReSharper disable once InconsistentNaming
                    var saveDC = ComputeSavingThrowDC(attacker, provider);
                    newEffectForm.OverrideSavingThrowInfo = new OverrideSavingThrowInfo(provider.SavingThrowAbility,
                        saveDC, provider.Name, RuleDefinitions.FeatureSourceType.ExplicitFeature);
                }

                actualEffectForms.Add(newEffectForm);
            }
        }

        /*
         * ######################################
         * [CE] EDIT START
         * Support for additional effects
         */

        var additionalForms = featureDefinition
            .GetFirstSubFeatureOfType<AdditionalEffectFormOnDamageHandler>()
            ?.Invoke(attacker, defender, provider);
        if (additionalForms != null)
        {
            actualEffectForms.AddRange(additionalForms);
        }

        /*
         * Support for for additional effects
         * [CE] EDIT END
         * ######################################
         */

        // Do I need to add a light source?
        if (provider.AddLightSource && defender.RulesetCharacter is { PersonalLightSource: null })
        {
            var lightSourceForm = provider.LightSourceForm;

            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            float brightRange = lightSourceForm.BrightRange;
            var dimRange = brightRange + lightSourceForm.DimAdditionalRange;
            defender.RulesetCharacter.PersonalLightSource = new RulesetLightSource(
                lightSourceForm.Color,
                brightRange,
                dimRange,
                lightSourceForm.GraphicsPrefabAssetGUID,
                lightSourceForm.LightSourceType,
                featureDefinition.Name,
                defender.RulesetCharacter.Guid);
            defender.RulesetCharacter.PersonalLightSource.Register(true);

            visibilityService.AddCharacterLightSource(defender, defender.RulesetCharacter.PersonalLightSource);

            var holdingCondition =
                attacker.RulesetCharacter.FindFirstConditionHoldingFeature(provider as FeatureDefinition);
            if (holdingCondition != null)
            {
                var effect = attacker.RulesetCharacter.FindEffectTrackingCondition(holdingCondition);
                effect.TrackLightSource(defender.RulesetCharacter, defender.Guid, string.Empty,
                    defender.RulesetCharacter.PersonalLightSource);
            }
        }

        //CHANGE: replaced `this` with `instance`
        instance.AdditionalDamageProviderActivated?.Invoke(attacker, defender, provider);
    }


    /**
     * This method is almost completely original game source provided by TA (1.4.8)
     * All changes made by CE mod should be clearly marked for easy future updates
     * This is for both physical and magical attacks
     */
    public static IEnumerator HandleAdditionalDamageOnCharacterAttackHitConfirmed(
        GameLocationBattleManager instance,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode,
        bool rangedAttack,
        RuleDefinitions.AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        RulesetEffect rulesetEffect,
        bool criticalHit,
        bool firstTarget)
    {
        instance.triggeredAdditionalDamageTags.Clear();
        attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(
            instance.featuresToBrowseReaction);

        /*
         * ######################################
         * [CE] EDIT START
         * Supports for extra damage from elemental infusions on armorer
         */

        if (InnovationArmor.IsBuiltInWeapon(attackMode, null, null))
        {
            var torsoSlot =
                attacker.RulesetCharacter.CharacterInventory.InventorySlotsByType[
                    DatabaseHelper.SlotTypeDefinitions.TorsoSlot.Name];

            if (torsoSlot is { Count: > 0 })
            {
                var additionalDamages = torsoSlot[0].EquipedItem.DynamicItemProperties
                    .Select(x => x.FeatureDefinition)
                    .OfType<IAdditionalDamageProvider>()
                    .OfType<FeatureDefinition>();

                instance.featuresToBrowseReaction.AddRange(additionalDamages);
            }
        }

        /*
         * Supports for extra damage from elemental infusions on armorer
         * [CE] EDIT END
         * ######################################
         */

        // Add item properties?
        if (attacker.RulesetCharacter.CharacterInventory != null)
        {
            if (attackMode?.SourceObject is RulesetItem weapon)
            {
                weapon.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(
                    instance.featuresToBrowseItem, attacker.Name);
                instance.featuresToBrowseReaction.AddRange(instance.featuresToBrowseItem);
                instance.featuresToBrowseItem.Clear();
            }
        }

        /*
         * ######################################
         * [CE] EDIT START
         * Support for extra types of Smite (like eldritch smite)
         */

        // store ruleset service for further use
        var rulesetImplementation = ServiceRepository.GetService<IRulesetImplementationService>();

        /*
         * Support for extra types of Smite (like eldritch smite)
         * [CE] EDIT END
         * ######################################
         */

        foreach (var featureDefinition in instance.featuresToBrowseReaction)
        {
            var provider = featureDefinition as IAdditionalDamageProvider;
            var additionalDamage = provider as FeatureDefinitionAdditionalDamage;

            // Some additional damage only work with attack modes (Hunter's Mark)
            if (provider.AttackModeOnly && attackMode == null)
            {
                continue;
            }

            // Some additional damage works on enemies only
            if ((provider.TargetSide == RuleDefinitions.Side.Enemy &&
                 !attacker.RulesetCharacter.IsOppositeSide(defender.RulesetCharacter.Side))
                || (provider.TargetSide == RuleDefinitions.Side.Ally &&
                    attacker.RulesetCharacter.IsOppositeSide(defender.RulesetCharacter.Side)))
            {
                continue;
            }

            // Trigger method
            var validTrigger = false;
            var validUses = true;
            if (provider.LimitedUsage != RuleDefinitions.FeatureLimitedUsage.None)
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (provider.LimitedUsage)
                {
                    case RuleDefinitions.FeatureLimitedUsage.OnceInMyTurn
                        when attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name) ||
                             (instance.Battle != null && instance.Battle.ActiveContender != attacker):
                    case RuleDefinitions.FeatureLimitedUsage.OncePerTurn
                        when attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name):
                        validUses = false;
                        break;

                    default:
                    {
                        if (attacker.UsedSpecialFeatures.Count > 0)
                        {
                            // Check if there is not already a used feature with the same tag (special sneak attack for Rogue Hoodlum / COTM-18228)
                            foreach (var kvp in attacker.UsedSpecialFeatures)
                            {
                                // ReSharper disable once InvertIf
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

                        break;
                    }
                }
            }

            if (additionalDamage
                && additionalDamage.OtherSimilarAdditionalDamages is { Count: > 0 }
                && attacker.UsedSpecialFeatures.Count > 0)
            {
                // Check if there is not already a used feature of the same "family"
                foreach (var kvp in attacker.UsedSpecialFeatures)
                {
                    // ReSharper disable once InvertIf
                    if (DatabaseRepository.GetDatabase<FeatureDefinitionAdditionalDamage>()
                        .TryGetElement(kvp.Key, out var previousFeature))
                    {
                        if (additionalDamage.OtherSimilarAdditionalDamages.Contains(previousFeature))
                        {
                            validUses = false;
                        }
                    }
                }
            }

            ItemDefinition itemDefinition = null;

            if (attackMode != null)
            {
                DatabaseHelper.TryGetDefinition(attackMode.SourceDefinition.Name, out itemDefinition);
            }

            CharacterActionParams reactionParams = null;

            /*
             * ######################################
             * [CE] EDIT START
             * Support for extra types of Smite (like eldritch smite)
             */
            var validProperty = true;

            if ((attackMode != null && validUses &&
                 provider.RequiredProperty != RuleDefinitions.RestrictedContextRequiredProperty.None)
                //[CE] ignore other restrictions and check context if we have custom validator for it
                || featureDefinition.HasSubFeatureOfType<IRestrictedContextProvider>())
            {
                validProperty = rulesetImplementation.IsValidContextForRestrictedContextProvider(provider,
                    attacker.RulesetCharacter, itemDefinition, rangedAttack, attackMode, rulesetEffect);
            }

            //[CE] try checking triggers only if context is valid, to prevent SpendSpellSlot showing popup on incorrect context
            if (validUses && validProperty)
                //commented-out original code
                // if (validUses)
                /*
                 * Support for extra types of Smite (like eldritch smite)
                 * [CE] EDIT END
                 * ######################################
                 */
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (provider.TriggerCondition)
                {
                    // Typical for Sneak Attack
                    case RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly
                        when attackMode != null
                             || (rulesetEffect != null
                                 && provider.RequiredProperty == RuleDefinitions.RestrictedContextRequiredProperty
                                     .SpellWithAttackRoll):
                    {
                        if (advantageType == RuleDefinitions.AdvantageType.Advantage ||
                            (advantageType != RuleDefinitions.AdvantageType.Disadvantage &&
                             instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side,
                                 attacker)))
                        {
                            validTrigger = true;
                        }

                        break;
                    }
                    /*
                     * ######################################
                     * [CE] EDIT START
                     * Support for extra types of Smite (like eldritch smite)
                     */

                    // [CE] remove melee check, so that other types of smites can be made
                    case RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot:

                        // commented-out original code
                        // case RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot
                        //     when attackModifier != null
                        //          && attackModifier.Proximity == RuleDefinitions.AttackProximity.Melee:

                        /*
                         * Support for extra types of Smite (like eldritch smite)
                         * [CE] EDIT END
                         * ######################################
                         */
                    {
                        //TODO: implement wild-shape, MC and warlock spell slot tweaks 
                        // This is used for Divine Smite
                        // Look for the spellcasting feature holding the smite
                        var hero = attacker.RulesetCharacter.GetOriginalHero();

                        // This is used to only offer divine smites on critical hits
                        var isDivineSmite = featureDefinition is FeatureDefinitionAdditionalDamage
                        {
                            NotificationTag: "DivineSmite"
                        };

                        // One DnD only allow smites as bonus action
                        if (Main.Settings.EnablePaladinSmite2024 &&
                            isDivineSmite &&
                            !ValidatorsCharacter.HasAvailableBonusAction(attacker.RulesetCharacter))
                        {
                            break;
                        }

                        if (!criticalHit &&
                            Main.Settings.AddPaladinSmiteToggle &&
                            isDivineSmite &&
                            !hero.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle))
                        {
                            break;
                        }

                        var classDefinition = hero.FindClassHoldingFeature(featureDefinition);

                        RulesetSpellRepertoire selectedSpellRepertoire = null;

                        foreach (var spellRepertoire in hero.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellCastingClass != classDefinition)
                            {
                                continue;
                            }

                            var atLeastOneSpellSlotAvailable = false;

                            for (var spellLevel = 1;
                                 spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel;
                                 spellLevel++)
                            {
                                spellRepertoire.GetSlotsNumber(spellLevel, out var remaining,
                                    out var dummy);

                                // handle EldritchSmite case that can only consume pact slots
                                if (featureDefinition is FeatureDefinitionAdditionalDamage
                                    {
                                        NotificationTag: InvocationsBuilders.EldritchSmiteTag
                                    })
                                {
                                    var pactMagicMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
                                    var pactMagicUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);

                                    remaining = pactMagicMaxSlots - pactMagicUsedSlots;
                                }

                                if (remaining <= 0)
                                {
                                    continue;
                                }

                                selectedSpellRepertoire = spellRepertoire;
                                atLeastOneSpellSlotAvailable = true;

                                break;
                            }

                            if (!atLeastOneSpellSlotAvailable)
                            {
                                continue;
                            }

                            reactionParams =
                                new CharacterActionParams(attacker, ActionDefinitions.Id.SpendSpellSlot);
                            reactionParams.ActionModifiers.Add(new ActionModifier());

                            yield return instance.PrepareAndReactWithSpellUsingSpellSlot(attacker,
                                selectedSpellRepertoire, provider.NotificationTag, reactionParams);

                            validTrigger = reactionParams.ReactionValidated;

                            // One DnD only allow smites as bonus action
                            if (Main.Settings.EnablePaladinSmite2024 && isDivineSmite && validTrigger)
                            {
                                attacker.SpendActionType(ActionDefinitions.ActionType.Bonus);
                            }

                            /*
                             * ######################################
                             * [CE] EDIT START
                             * Support for Oath of Thunder level 20 feature
                             */

                            //TODO: convert this to a proper interface to change number of smite dice
                            if (validTrigger && isDivineSmite &&
                                hero.GetSubclassLevel(
                                    DatabaseHelper.CharacterClassDefinitions.Paladin,
                                    OathOfDemonHunter.Name) == 20)
                            {
                                reactionParams.intParameter++;
                            }
                            /*
                             * Support for Oath of Thunder level 20 feature
                             * [CE] EDIT END
                             * ######################################
                             */
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe:
                    {
                        if (defender.RulesetActor.HasConditionOfTypeAndSource(
                                provider.RequiredTargetCondition,
                                attacker.Guid))
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition:
                    {
                        if (defender == null)
                        {
                            break;
                        }

                        if (!provider.RequiredTargetCondition)
                        {
                            Trace.LogError(
                                "Provider trigger condition is TargetHasCondition, but no condition given");
                            break;
                        }

                        if (defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition:
                    {
                        if (defender == null)
                        {
                            break;
                        }

                        if (!provider.RequiredTargetCondition)
                        {
                            Trace.LogError(
                                "Provider trigger condition is TargetDoesNotHaveCondition, but no condition given");
                            break;
                        }

                        if (!defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition
                                .Name))
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    /*
                     * ######################################
                     * [CE] EDIT START
                     * Support for extra types of trigger conditions
                     */
                    case (RuleDefinitions.AdditionalDamageTriggerCondition)
                        ExtraAdditionalDamageTriggerCondition.SourceIsSneakingAttack:
                    {
                        var isVanillaSneakAttack =
                            (attackMode != null
                             || (rulesetEffect != null
                                 && provider.RequiredProperty == RuleDefinitions.RestrictedContextRequiredProperty
                                     .SpellWithAttackRoll)) &&
                            (advantageType == RuleDefinitions.AdvantageType.Advantage ||
                             (advantageType != RuleDefinitions.AdvantageType.Disadvantage &&
                              instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side,
                                  attacker)));

                        validTrigger =
                            isVanillaSneakAttack ||
                            RoguishDuelist.TargetIsDuelingWithRoguishDuelist(attacker, defender, advantageType) ||
                            RoguishUmbralStalker.SourceOrTargetAreNotBright(attacker, defender, advantageType);
                        break;
                    }

                    case (RuleDefinitions.AdditionalDamageTriggerCondition)
                        ExtraAdditionalDamageTriggerCondition.FlurryOfBlows:
                    {
                        validTrigger =
                            attackMode != null &&
                            attackMode.AttackTags.Contains(TagsDefinitions.FlurryOfBlows);
                        break;
                    }

                    case (RuleDefinitions.AdditionalDamageTriggerCondition)
                        ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou:
                    {
                        validTrigger = RoguishDuelist
                            .TargetIsDuelingWithRoguishDuelist(attacker, defender, advantageType);
                        break;
                    }

                    case (RuleDefinitions.AdditionalDamageTriggerCondition)
                        ExtraAdditionalDamageTriggerCondition.TargetIsWithin10Ft:
                    {
                        validTrigger = attacker.IsWithinRange(defender, 2);
                        break;
                    }

                    case (RuleDefinitions.AdditionalDamageTriggerCondition)
                        ExtraAdditionalDamageTriggerCondition.SourceOrTargetAreNotBright:
                    {
                        validTrigger = RoguishUmbralStalker
                            .SourceOrTargetAreNotBright(attacker, defender, advantageType);
                        break;
                    }
                    /*
                     * Support for extra types of trigger conditions
                     * [CE] EDIT END
                     * ######################################
                     */

                    case RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded:
                    {
                        if (defender?.RulesetCharacter != null &&
                            defender.RulesetCharacter.CurrentHitPoints <
                            defender.RulesetCharacter.GetAttribute(AttributeDefinitions.HitPoints)
                                .CurrentValue)
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasSenseType:
                    {
                        if (defender?.RulesetCharacter != null &&
                            defender.RulesetCharacter.HasSenseType(provider.RequiredTargetSenseType))
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCreatureTag:
                    {
                        if (defender?.RulesetCharacter != null &&
                            defender.RulesetCharacter.HasTag(provider.RequiredTargetCreatureTag))
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.RangeAttackFromHigherGround
                        when attackMode != null:
                    {
                        if (defender == null)
                        {
                            break;
                        }

                        if (attacker.LocationPosition.y > defender.LocationPosition.y)
                        {
                            if (itemDefinition && itemDefinition.IsWeapon)
                            {
                                var weaponTypeDefinition =
                                    DatabaseHelper.GetDefinition<WeaponTypeDefinition>(itemDefinition
                                        .WeaponDescription
                                        .WeaponType);

                                if (weaponTypeDefinition.WeaponProximity ==
                                    RuleDefinitions.AttackProximity.Range)
                                {
                                    validTrigger = true;
                                }
                            }
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily:
                    {
                        if (defender?.RulesetCharacter != null &&
                            defender.RulesetCharacter.CharacterFamily ==
                            provider.RequiredCharacterFamily.Name)
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit:
                        validTrigger = criticalHit;
                        break;
                    case RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage
                        when (firstTarget || !provider.FirstTargetOnly) &&
                             rulesetEffect is RulesetEffectSpell spell &&
                             spell.SpellDefinition.SchoolOfMagic ==
                             RuleDefinitions.SchoolEvocation:
                    case RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage
                        when (firstTarget || !provider.FirstTargetOnly) &&
                             rulesetEffect is RulesetEffectPower power &&
                             power.PowerDefinition.SurrogateToSpell &&
                             power.PowerDefinition.SurrogateToSpell.SchoolOfMagic ==
                             RuleDefinitions.SchoolEvocation:
                    case RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry
                        when (firstTarget || !provider.FirstTargetOnly) &&
                             rulesetEffect is RulesetEffectSpell &&
                             attacker.RulesetCharacter.HasAncestryMatchingDamageType(
                                 provider.RequiredAncestryType,
                                 actualEffectForms):
                        validTrigger = true;
                        break;

                    case RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamagesTarget
                        when (firstTarget || !provider.FirstTargetOnly) &&
                             rulesetEffect is RulesetEffectSpell spell:
                    {
                        // This check is for Warlock / invocation / agonizing blast
                        if (!provider.RequiredSpecificSpell || provider.RequiredSpecificSpell ==
                            spell.SpellDefinition)
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.NotWearingHeavyArmor:
                    {
                        if (attacker.RulesetCharacter != null &&
                            !attacker.RulesetCharacter.IsWearingHeavyArmor())
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive:
                        validTrigger = true;
                        break;

                    case RuleDefinitions.AdditionalDamageTriggerCondition.RagingAndTargetIsSpellcaster
                        when defender?.RulesetCharacter != null:
                    {
                        if (attacker.RulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionRaging) &&
                            defender.RulesetCharacter.SpellRepertoires.Count > 0)
                        {
                            validTrigger = true;
                        }

                        break;
                    }

                    case RuleDefinitions.AdditionalDamageTriggerCondition.Raging:
                    {
                        if (attacker.RulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionRaging))
                        {
                            validTrigger = true;
                        }

                        break;
                    }
                }
            }

            /*
             * ######################################
             * [CE] EDIT START
             * Support for extra types of Smite (like eldritch smite)
             */

            //Commented-out original code. Actual check moved up, to make sure Reaction popups (like SpendSpellSlot) won't be shown if context is not valid.
            // // Check required properties for physical attacks if needed
            // IRulesetImplementationService implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            //
            // bool validProperty = true;
            // if (attackMode != null && validTrigger && provider.RequiredProperty != RuleDefinitions.RestrictedContextRequiredProperty.None)
            // {
            //     validProperty = implementationService.IsValidContextForRestrictedContextProvider(provider, attacker.RulesetCharacter, itemDefinition, rangedAttack, attackMode, rulesetEffect);
            // }

            /*
             * Support for extra types of Smite (like eldritch smite)
             * [CE] EDIT END
             * ######################################
             */

            // ReSharper disable once InvertIf
            if (validTrigger && validProperty)
            {
                instance.ComputeAndNotifyAdditionalDamage(attacker, defender, provider, actualEffectForms,
                    reactionParams, attackMode, criticalHit);
                instance.triggeredAdditionalDamageTags.Add(provider.NotificationTag);
            }
        }

        /*
         * ######################################
         * [CE] EDIT START
         * Support for `CustomAdditionalDamage`
         */

        foreach (var feature in attacker.RulesetCharacter
                     .GetSubFeaturesByType<CustomAdditionalDamage>())
        {
            var validUses = true;
            var additionalDamage = feature.Provider as FeatureDefinitionAdditionalDamage;
            var provider = feature.Provider;

            if (provider.LimitedUsage != RuleDefinitions.FeatureLimitedUsage.None)
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (provider.LimitedUsage)
                {
                    case RuleDefinitions.FeatureLimitedUsage.OnceInMyTurn
                        when attacker.UsedSpecialFeatures.ContainsKey(additionalDamage.Name) ||
                             (instance.Battle != null && instance.Battle.ActiveContender != attacker):
                    case RuleDefinitions.FeatureLimitedUsage.OncePerTurn
                        when attacker.UsedSpecialFeatures.ContainsKey(additionalDamage.Name):
                        validUses = false;
                        break;

                    default:
                    {
                        if (attacker.UsedSpecialFeatures.Count > 0)
                        {
                            // Check if there is not already a used feature with the same tag (special sneak attack for Rogue Hoodlum / COTM-18228)
                            foreach (var kvp in attacker.UsedSpecialFeatures)
                            {
                                // ReSharper disable once InvertIf
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

                        break;
                    }
                }
            }

            var validProperty = rulesetImplementation.IsValidContextForRestrictedContextProvider(
                provider, attacker.RulesetCharacter, attackMode?.SourceDefinition as ItemDefinition,
                rangedAttack,
                attackMode, rulesetEffect);

            if (!validUses || !validProperty || !feature.IsValid(
                    instance, attacker, defender, attackModifier, attackMode, rangedAttack,
                    advantageType,
                    actualEffectForms, rulesetEffect, criticalHit, firstTarget, out var reactionParams))
            {
                continue;
            }

            instance.ComputeAndNotifyAdditionalDamage(
                attacker, defender, feature.Provider, actualEffectForms, reactionParams, attackMode,
                criticalHit);
            instance.triggeredAdditionalDamageTags.Add(feature.Provider.NotificationTag);
        }

        /*
         * Support for for `CustomAdditionalDamage`
         * [CE] EDIT END
         * ######################################
         */
    }
}
