using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerSurvivalist : AbstractSubclass
{
    internal const string Name = "RangerSurvivalist";

    internal RangerSurvivalist()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Entangle),
                BuildSpellGroup(5, SpellsContext.Web),
                BuildSpellGroup(9, SleetStorm),
                BuildSpellGroup(13, GreaterInvisibility),
                BuildSpellGroup(17, HoldMonster))
            .AddToDB();

        // Wandering Outcast

        var proficiencyWanderingOutcast = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}WanderingOutcast")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Survival)
            .AddToDB();

        // Disabling Strike

        var additionalDamageDisablingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DisablingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.Negates,
                AttributeDefinitions.Dexterity)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = ConditionDefinitions.ConditionHindered_By_Frost,
                Operation = ConditionOperationDescription.ConditionOperation.Add,
                hasSavingThrow = true,
                canSaveToCancel = true,
                saveOccurence = TurnOccurenceType.EndOfTurn
            })
            .AddToDB();

        //
        // LEVEL 07
        //

        // Analytical Mind

        var abilityCheckAnalyticalMind = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheck{Name}AnalyticalMind")
            .SetGuiPresentation($"FeatureSet{Name}AnalyticalMind", Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Survival))
            .AddToDB();

        var featureSetAnalyticalMind = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AnalyticalMind")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(abilityCheckAnalyticalMind, Executioner.FeatureFightingStyleExecutioner)
            .AddToDB();

        //
        // LEVEL 11
        //

        // Improved Disabling Strike


        var attributeModifierImprovedDisablingStrike = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ImprovedDisablingStrike")
            .SetGuiPresentation($"Condition{Name}ImprovedDisablingStrike", Category.Condition)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, -2)
            .AddToDB();

        var conditionImprovedDisablingStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImprovedDisablingStrike")
            .SetGuiPresentation(Category.Condition)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddFeatures(attributeModifierImprovedDisablingStrike)
            .AddToDB();

        var additionalDamageImprovedDisablingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}ImprovedDisablingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.Negates,
                AttributeDefinitions.Dexterity)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = ConditionDefinitions.ConditionHindered_By_Frost,
                Operation = ConditionOperationDescription.ConditionOperation.Add,
                hasSavingThrow = true,
                canSaveToCancel = true,
                saveOccurence = TurnOccurenceType.EndOfTurn
            })
            .SetCustomSubFeatures(
                new CustomBehaviorImprovedDisablingStrikeDisablingStrike(conditionImprovedDisablingStrike))
            .AddToDB();

        //
        // LEVEL 15
        //

        // Unmatched Experience

        var savingThrowAffinityUnmatchedExperience = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}UnmatchedExperience")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.ProficiencyBonusOrPlus1, true, AttributeDefinitions.Wisdom)
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerSurvivalist, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                additionalDamageDisablingStrike,
                proficiencyWanderingOutcast)
            .AddFeaturesAtLevel(7,
                featureSetAnalyticalMind)
            .AddFeaturesAtLevel(11,
                additionalDamageImprovedDisablingStrike)
            .AddFeaturesAtLevel(15,
                savingThrowAffinityUnmatchedExperience,
                FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorImprovedDisablingStrikeDisablingStrike :
        IFeatureDefinitionCustomCode, IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionDefinition;

        public CustomBehaviorImprovedDisablingStrikeDisablingStrike(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome is not RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null || rulesetDefender.IsDeadOrDying)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetDefender.guid,
                _conditionDefinition,
                _conditionDefinition.DurationType,
                _conditionDefinition.DurationParameter,
                _conditionDefinition.TurnOccurence,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name);

            rulesetDefender.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x.Name == $"AdditionalDamage{Name}DisablingStrike");
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // Empty
        }
    }
}
