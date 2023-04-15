using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialRoyalKnight : AbstractSubclass
{
    internal MartialRoyalKnight()
    {
        const string Name = "RoyalKnight";

        var abilityCheckAffinityRoyalEnvoy = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}RoyalEnvoy")
            .SetGuiPresentationNoContent()
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.HalfProficiencyWhenNotProficient,
                DieType.D1,
                0,
                (AttributeDefinitions.Charisma, null))
            .AddToDB();

        var featureSetRoyalEnvoy = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}RoyalEnvoy")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                abilityCheckAffinityRoyalEnvoy,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfSolasta)
            .AddToDB();

        var powerRallyingCry = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RallyingCry")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerRallyingCry", Resources.PowerRallyingCry, 256, 128))
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.ShortRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(PowerDomainLifePreserveLife.EffectDescription)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetHealingForm(
                            HealingComputation.Dice,
                            0,
                            DieType.D1,
                            4,
                            false,
                            HealingCap.MaximumHitPoints,
                            EffectForm.LevelApplianceType.MultiplyBonus)
                        .Build())
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly, TargetFilteringTag.No, 5, DieType.D8)
                .Build())
            .SetOverriddenPower(PowerFighterSecondWind)
            .AddToDB();

        var additionalActionInspiringSurge = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}InspiringSurge")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .AddToDB();

        var conditionInspiringSurge = ConditionDefinitionBuilder
            .Create($"Condition{Name}InspiringSurge")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalActionInspiringSurge)
            .AddToDB();

        var powerInspiringSurge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}InspiringSurge")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionInspiringSurge, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var conditionProtection = ConditionDefinitionBuilder
            .Create($"Condition{Name}Protection")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}Protection")
                    .SetGuiPresentation($"Power{Name}Protection", Category.Feature,
                        GuiPresentationBuilder.NoContentTitle)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
                    .SetCustomSubFeatures(ValidatorsCharacter.HasLessThan25PercentHealth)
                    .AddToDB())
            .AddToDB();

        var powerProtection = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Protection")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bless)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 13)
                .SetDurationData(DurationType.Permanent)
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionProtection, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .SetShowCasting(false)
            .SetCustomSubFeatures(new CharacterTurnStartListenerProtection())
            .AddToDB();

        var savingThrowAffinitySpiritedSurge = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}SpiritedSurge")
            .SetGuiPresentationNoContent(true)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, true,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var combatAffinitySpiritedSurge = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}SpiritedSurge")
            .SetGuiPresentation($"Power{Name}SpiritedSurge", Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var abilityCheckAffinitySpiritedSurge = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}SpiritedSurge")
            .SetGuiPresentation($"Power{Name}SpiritedSurge", Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var conditionSpiritedSurge = ConditionDefinitionBuilder
            .Create($"Condition{Name}SpiritedSurge")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                abilityCheckAffinitySpiritedSurge,
                additionalActionInspiringSurge,
                combatAffinitySpiritedSurge,
                savingThrowAffinitySpiritedSurge)
            .AddToDB();

        var powerSpiritedSurge = FeatureDefinitionPowerBuilder
            .Create(powerInspiringSurge, $"Power{Name}SpiritedSurge")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionSpiritedSurge, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerInspiringSurge)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Martial{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialRoyalKnight, 256))
            .AddFeaturesAtLevel(3, powerRallyingCry)
            .AddFeaturesAtLevel(7, featureSetRoyalEnvoy)
            .AddFeaturesAtLevel(10, powerInspiringSurge)
            .AddFeaturesAtLevel(15, powerProtection)
            .AddFeaturesAtLevel(18, powerSpiritedSurge)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CharacterTurnStartListenerProtection : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            var healingAmount =
                2 * locationCharacter.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            foreach (var rulesetCharacter in battle.PlayerContenders.Select(x => x.RulesetCharacter))
            {
                if (rulesetCharacter.TemporaryHitPoints <= healingAmount)
                {
                    rulesetCharacter.ReceiveTemporaryHitPoints(healingAmount, DurationType.Minute, 1,
                        TurnOccurenceType.EndOfTurn, locationCharacter.Guid);
                }
            }
        }
    }
}
