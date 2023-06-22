#if false
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RangerSkyWarrior : AbstractSubclass
{
    private const string Name = "RangerSkyWarrior";

    internal RangerSkyWarrior()
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
                BuildSpellGroup(2, SpellsContext.AirBlast),
                BuildSpellGroup(5, SpellsContext.MirrorImage),
                BuildSpellGroup(9, Fly),
                BuildSpellGroup(13, PhantasmalKiller),
                BuildSpellGroup(17, MindTwist))
            .AddToDB();

        // Gift of The Wind

        var conditionGiftOfTheWindAttacked = ConditionDefinitionBuilder
            .Create($"Condition{Name}GiftOfTheWindAttacked")
            .SetGuiPresentationNoContent(true)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var movementAffinityGiftOfTheWind = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}GiftOfTheWind")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(2)
            .AddToDB();

        var combatAffinityGiftOfTheWind = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}GiftOfTheWind")
            .SetGuiPresentationNoContent(true)
            .SetAttackOfOpportunityImmunity(true)
            .SetSituationalContext(SituationalContext.TargetHasCondition, conditionGiftOfTheWindAttacked)
            .AddToDB();

        var conditionGiftOfTheWind = ConditionDefinitionBuilder
            .Create($"Condition{Name}GiftOfTheWind")
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(movementAffinityGiftOfTheWind, combatAffinityGiftOfTheWind)
            .AddToDB();
        
        // Disabling Strike

        var conditionDisablingStrike = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionHindered_By_Frost, $"Condition{Name}DisablingStrike")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionHindered)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Minute, 1)
            .AddToDB();

        var additionalDamageDisablingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DisablingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.Negates,
                AttributeDefinitions.Dexterity)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    ConditionDefinition = conditionDisablingStrike,
                    Operation = ConditionOperationDescription.ConditionOperation.Add,
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveOccurence = TurnOccurenceType.EndOfTurn,
                    saveAffinity = EffectSavingThrowType.Negates
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

        FeatureSetAnalyticalMind.FeatureSet.Add(abilityCheckAnalyticalMind);

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
            .Create(conditionDisablingStrike, $"Condition{Name}ImprovedDisablingStrike")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionHindered)
            .AddFeatures(attributeModifierImprovedDisablingStrike)
            .AddToDB();

        var additionalDamageImprovedDisablingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}ImprovedDisablingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetSavingThrowData(EffectDifficultyClassComputation.SpellCastingFeature, EffectSavingThrowType.Negates,
                AttributeDefinitions.Dexterity)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    ConditionDefinition = conditionImprovedDisablingStrike,
                    Operation = ConditionOperationDescription.ConditionOperation.Add,
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveOccurence = TurnOccurenceType.EndOfTurn,
                    saveAffinity = EffectSavingThrowType.Negates
                })
            .SetCustomSubFeatures(new CustomCodeImprovedDisablingStrike(additionalDamageDisablingStrike))
            .AddToDB();

        //
        // LEVEL 15
        //

        // Unmatched Experience

        var savingThrowAffinityUnmatchedExperience = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}UnmatchedExperience")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.ProficiencyBonusOrPlus1, false, AttributeDefinitions.Wisdom)
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
                FeatureSetAnalyticalMind)
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

    internal static void LateLoad()
    {
        FeatureSetAnalyticalMind.FeatureSet.Add(
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatExecutioner"));
    }

    private sealed class CustomCodeImprovedDisablingStrike : IFeatureDefinitionCustomCode
    {
        private readonly FeatureDefinitionAdditionalDamage _additionalDamageDisablingStrike;

        public CustomCodeImprovedDisablingStrike(FeatureDefinitionAdditionalDamage additionalDamageDisablingStrike)
        {
            _additionalDamageDisablingStrike = additionalDamageDisablingStrike;
        }

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x == _additionalDamageDisablingStrike);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // Empty
        }
    }
}
#endif
