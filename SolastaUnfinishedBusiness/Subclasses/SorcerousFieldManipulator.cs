using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class SorcerousFieldManipulator : AbstractSubclass
{
    internal SorcerousFieldManipulator()
    {
        var autoPreparedSpellsFieldManipulator = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsFieldManipulator")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Origin")
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Sleep)
            .AddPreparedSpellGroup(3, Invisibility)
            .AddPreparedSpellGroup(5, Counterspell)
            .AddPreparedSpellGroup(7, Banishment)
            .AddPreparedSpellGroup(9, HoldMonster)
            .AddPreparedSpellGroup(11, GlobeOfInvulnerability)
            .AddToDB();

        var magicAffinityFieldManipulatorControlHeightened = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityFieldManipulatorControlHeightened")
            .SetGuiPresentation(Category.Feature)
            .SetWarList(1,
                Banishment, // abjuration 4
                Counterspell, // abjuration 3
                GlobeOfInvulnerability, // abjuration 6
                HoldMonster, // Enchantment 5
                Invisibility, // illusion 2
                Sleep) // enchantment 1
            .AddToDB();

        var savingThrowAffinityFieldManipulatorDC = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinityFieldManipulatorDC")
            .SetGuiPresentation("MagicAffinityFieldManipulatorDC", Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, true,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var conditionFieldManipulatorDC = ConditionDefinitionBuilder
            .Create("ConditionFieldManipulatorDC")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(savingThrowAffinityFieldManipulatorDC)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var magicAffinityFieldManipulatorDc = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityFieldManipulatorDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 2)
            .SetCustomSubFeatures(new FieldManipulatorMagicalAttackFinished(conditionFieldManipulatorDC))
            .AddToDB();

        var proficiencyFieldManipulatorMentalSavingThrows = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFieldManipulatorMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.SavingThrow,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        var powerFieldManipulatorDominatePerson = FeatureDefinitionPowerBuilder
            .Create("PowerFieldManipulatorDominatePerson")
            .SetGuiPresentation(Category.Feature, DominatePerson)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(DominatePerson.EffectDescription)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("FieldManipulator")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("SorcererFieldManipulator", Resources.SorcererFieldManipulator, 256))
            .AddFeaturesAtLevel(1,
                magicAffinityFieldManipulatorControlHeightened,
                autoPreparedSpellsFieldManipulator)
            .AddFeaturesAtLevel(6,
                magicAffinityFieldManipulatorDc)
            .AddFeaturesAtLevel(14,
                proficiencyFieldManipulatorMentalSavingThrows)
            .AddFeaturesAtLevel(18,
                powerFieldManipulatorDominatePerson)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class FieldManipulatorMagicalAttackFinished : IMagicalAttackFinished
    {
        private readonly ConditionDefinition _conditionDefinition;

        public FieldManipulatorMagicalAttackFinished(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator BeforeOnMagicalAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null)
            {
                yield break;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetDefender.Guid,
                _conditionDefinition,
                _conditionDefinition.durationType,
                _conditionDefinition.durationParameter,
                _conditionDefinition.turnOccurence,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name);

            rulesetDefender.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }

        public IEnumerator OnMagicalAttackFinished(GameLocationCharacter attacker, GameLocationCharacter defender,
            ActionModifier magicModifier, RulesetEffect rulesetEffect, List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield break;
        }
    }
}
