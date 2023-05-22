using System.Collections;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfAltruism : AbstractSubclass
{
    private const string Name = "OathOfAltruism";
    internal const string DefensiveStrike = $"Feature{Name}DefensiveStrike";

    internal OathOfAltruism()
    {
        var autoPreparedSpellsAltruism = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("Subclass/&OathOfAltruismTitle", "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, HealingWord, ShieldOfFaith),
                BuildSpellGroup(5, CalmEmotions, HoldPerson),
                BuildSpellGroup(9, Counterspell, HypnoticPattern),
                BuildSpellGroup(13, DominateBeast, GuardianOfFaith),
                BuildSpellGroup(17, HoldMonster, WallOfForce)
            )
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var featureSpiritualShielding = FeatureDefinitionBuilder
            .Create($"Feature{Name}SpiritualShielding")
            .SetGuiPresentation(Category.Feature, ShieldOfFaith)
            .SetCustomSubFeatures(BlockAttacks.SpiritualShieldingMarker)
            .AddToDB();

        var featureDefensiveStrike = FeatureDefinitionBuilder
            .Create(DefensiveStrike)
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDecisiveStrike)
            .SetCustomSubFeatures(DefensiveStrikeMarker.Mark)
            .AddToDB();

        var featureAuraOfTheGuardian = FeatureDefinitionBuilder
            .Create("FeatureAuraOfTheGuardian")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianConditionMarker)
            .AddToDB();

        var conditionAuraOfTheGuardian = ConditionDefinitionBuilder
            .Create($"Condition{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Condition, ConditionShielded)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(featureAuraOfTheGuardian)
            .AddToDB();

        var powerAuraOfTheGuardian = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Feature, GuardianOfFaith)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianUserMarker)
            .AddToDB();

        powerAuraOfTheGuardian.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraOfTheGuardian, ConditionForm.ConditionOperation.Add)
            .Build();

        var powerTakeThePain = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TakeThePain")
            .SetGuiPresentation(Category.Feature, BeaconOfHope)
            .SetUsesFixed(RuleDefinitions.ActivationTime.BonusAction, RuleDefinitions.RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 5,
                        RuleDefinitions.TargetType.IndividualsUnique)
                    .Build())
            .SetCustomSubFeatures(new AfterActionFinishedTakeThePain())
            .AddToDB();

        var effectPowerAuraOfTheGuardian18 = new EffectDescription();

        effectPowerAuraOfTheGuardian18.Copy(powerAuraOfTheGuardian.EffectDescription);
        effectPowerAuraOfTheGuardian18.targetParameter = 13;

        var powerAuraOfTheGuardian18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraOfTheGuardian, $"Power{Name}AuraOfTheGuardian18")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(effectPowerAuraOfTheGuardian18)
            .SetOverriddenPower(powerAuraOfTheGuardian)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianUserMarker)
            .AddToDB();

        var magicAffinityExaltedProtector = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExaltedProtector")
            .SetGuiPresentation($"Power{Name}ExaltedProtector", Category.Feature)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage)
            .AddToDB();

        var conditionExaltedProtector = ConditionDefinitionBuilder
            .Create($"Condition{Name}ExaltedProtector")
            .SetGuiPresentation(Category.Condition, ConditionBlessed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(ConditionBlessed.Features)
            .AddToDB();

        conditionExaltedProtector.Features.Add(magicAffinityExaltedProtector);
        conditionExaltedProtector.Features.Add(
            FeatureDefinitionDeathSavingThrowAffinitys.DeathSavingThrowAffinityBeaconOfHope);

        var powerExaltedProtector = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}ExaltedProtector")
            .SetUsesFixed(RuleDefinitions.ActivationTime.Permanent)
            .SetGuiPresentation(Category.Feature, GuardianOfFaith)
            .AddToDB();

        powerExaltedProtector.EffectDescription.targetParameter = 13;
        powerExaltedProtector.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionExaltedProtector, ConditionForm.ConditionOperation.Add)
            .Build();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("OathOfAltruism", Resources.OathOfAltruism, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsAltruism,
                featureDefensiveStrike,
                featureSpiritualShielding)
            .AddFeaturesAtLevel(7, powerAuraOfTheGuardian)
            .AddFeaturesAtLevel(15, powerTakeThePain)
            .AddFeaturesAtLevel(18, powerAuraOfTheGuardian18)
            .AddFeaturesAtLevel(20, powerExaltedProtector)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class AfterActionFinishedTakeThePain : IActionFinished
    {
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action.ActionType != ActionDefinitions.ActionType.Bonus)
            {
                yield break;
            }

            if (action.ActingCharacter == null)
            {
                yield break;
            }

            var self = action.ActingCharacter;

            if (action is not CharacterActionUsePower characterActionUsePower ||
                !characterActionUsePower.activePower.PowerDefinition.Name.StartsWith($"Power{Name}TakeThePain"))
            {
                yield break;
            }

            foreach (var character in action.ActionParams.targetCharacters)
            {
                var targetHitPoints = character.RulesetCharacter.currentHitPoints;
                var casterHitPoints = self.RulesetCharacter.currentHitPoints;

                if (casterHitPoints <= targetHitPoints)
                {
                    continue;
                }

                character.RulesetCharacter.ForceSetHealth(casterHitPoints, false);
                self.RulesetCharacter.ForceSetHealth(targetHitPoints, false);

                var profBonus = AttributeDefinitions.ComputeProficiencyBonus(self.RulesetCharacter
                    .TryGetAttributeValue(AttributeDefinitions.CharacterLevel));

                var myCharismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(self.RulesetCharacter
                    .TryGetAttributeValue(AttributeDefinitions.Charisma));

                self.RulesetCharacter.ReceiveTemporaryHitPoints((profBonus * 2) + myCharismaModifier,
                    RuleDefinitions.DurationType.UntilAnyRest, 0, RuleDefinitions.TurnOccurenceType.StartOfTurn,
                    self.RulesetCharacter.guid);
            }
        }
    }
}
