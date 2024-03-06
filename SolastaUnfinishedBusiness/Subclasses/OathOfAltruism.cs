using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class OathOfAltruism : AbstractSubclass
{
    private const string Name = "OathOfAltruism";
    internal const string DefensiveStrike = $"Feature{Name}DefensiveStrike";

    public OathOfAltruism()
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
                BuildSpellGroup(17, HoldMonster, WallOfForce))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var conditionSpiritualShielding = ConditionDefinitionBuilder
            .Create(ConditionShielded, $"Condition{Name}SpiritualShielding")
            .AddToDB();

        // kept name for backward compatibility
        var powerSpiritualShielding = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}SpiritualShielding")
            .SetGuiPresentation(Category.Feature, ShieldOfFaith)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionSpiritualShielding))
                    .Build())
            .AddToDB();

        powerSpiritualShielding.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new AttackBeforeHitPossibleOnMeOrAllySpiritualShielding(powerSpiritualShielding));

        var featureDefensiveStrike = FeatureDefinitionBuilder
            .Create(DefensiveStrike)
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDecisiveStrike)
            .AddCustomSubFeatures(DefensiveStrikeMarker.Mark)
            .AddToDB();

        var conditionAuraOfTheGuardian = ConditionDefinitionBuilder
            .Create($"Condition{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Condition, ConditionShielded)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(GuardianAura.AuraGuardianConditionMarker)
            .AddToDB();

        var powerAuraOfTheGuardian = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Feature, GuardianOfFaith)
            .AddCustomSubFeatures(GuardianAura.AuraGuardianUserMarker)
            .AddToDB();

        powerAuraOfTheGuardian.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraOfTheGuardian, ConditionForm.ConditionOperation.Add)
            .Build();

        var powerTakeThePain = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TakeThePain")
            .SetGuiPresentation(Category.Feature, BeaconOfHope)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 5,
                        TargetType.IndividualsUnique)
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeTakeThePain())
            .AddToDB();

        var powerAuraOfTheGuardian18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraOfTheGuardian, $"Power{Name}AuraOfTheGuardian18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerAuraOfTheGuardian)
            .AddCustomSubFeatures(GuardianAura.AuraGuardianUserMarker)
            .AddToDB();

        powerAuraOfTheGuardian18.EffectDescription.targetParameter = 13;

        var magicAffinityExaltedProtector = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExaltedProtector")
            .SetGuiPresentation($"Power{Name}ExaltedProtector", Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
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
            .SetUsesFixed(ActivationTime.Permanent)
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
                powerSpiritualShielding)
            .AddFeaturesAtLevel(7, powerAuraOfTheGuardian)
            .AddFeaturesAtLevel(15, powerTakeThePain)
            .AddFeaturesAtLevel(18, powerAuraOfTheGuardian18)
            .AddFeaturesAtLevel(20, powerExaltedProtector)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Paladin;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class MagicEffectFinishedByMeTakeThePain : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var self = action.ActingCharacter;

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
                    DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn,
                    self.RulesetCharacter.guid);
            }

            yield break;
        }
    }

    private class AttackBeforeHitPossibleOnMeOrAllySpiritualShielding(FeatureDefinitionPower powerSpiritualShielding)
        : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            if (rulesetEffect != null &&
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (helper == defender ||
                !helper.CanReact(true) ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingPowerUses(powerSpiritualShielding) == 0)
            {
                yield break;
            }

            var armorClass = defender.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
            var totalAttack =
                attackRoll +
                (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0) +
                actionModifier.AttackRollModifier;

            if (armorClass > totalAttack)
            {
                yield break;
            }

            const int SHIELD_BONUS = 5;

            if (armorClass + SHIELD_BONUS <= totalAttack)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerSpiritualShielding, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, Id.PowerReaction)
                {
                    StringParameter = "SpiritualShielding",
                    StringParameter2 = "UseSpiritualShieldingDescription".Formatted(
                        Category.Reaction, attacker.Name, defender.Name),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionManager, count);
        }
    }
}
