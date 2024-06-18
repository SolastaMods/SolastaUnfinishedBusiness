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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
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
                BuildSpellGroup(17, HoldMonster, MassCureWounds))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var conditionSpiritualShielding = ConditionDefinitionBuilder
            .Create($"Condition{Name}SpiritualShielding")
            .SetGuiPresentation(Category.Condition, ConditionShielded)
            .SetPossessive()
            .SetFeatures(
                MagicAffinityConditionShielded,
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}SpiritualShielding")
                    .SetGuiPresentation($"Condition{Name}SpiritualShielding", Category.Condition, Global.Empty)
                    .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceAbilityBonus, AttributeDefinitions.Charisma)
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

    private static int GetChaModifier(
        // ReSharper disable once SuggestBaseTypeForParameter
        RulesetCharacter rulesetCharacter)
    {
        var charisma = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma);
        var chaMod = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);

        return chaMod;
    }

    private sealed class MagicEffectFinishedByMeTakeThePain : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            foreach (var character in action.ActionParams.targetCharacters)
            {
                var rulesetTarget = character.RulesetCharacter;
                var targetHitPoints = rulesetTarget.currentHitPoints;
                var casterHitPoints = rulesetCharacter.currentHitPoints;

                if (casterHitPoints <= targetHitPoints)
                {
                    continue;
                }

                rulesetTarget.ForceSetHealth(casterHitPoints, false);
                rulesetCharacter.ForceSetHealth(targetHitPoints, false);

                var profBonus = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                var chaMod = GetChaModifier(rulesetCharacter);
                var tempHitPoints = (profBonus * 2) + chaMod;

                rulesetCharacter.ReceiveTemporaryHitPoints(
                    tempHitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.guid);
            }

            yield break;
        }
    }

    private class AttackBeforeHitPossibleOnMeOrAllySpiritualShielding(FeatureDefinitionPower powerSpiritualShielding)
        : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                helper == defender ||
                helper.IsOppositeSide(defender.Side) ||
                !helper.CanReact(true) ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingPowerUses(powerSpiritualShielding) == 0)
            {
                yield break;
            }

            var armorClass = defender.RulesetActor.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
            var attackRoll = action.AttackRoll;
            var totalAttack =
                attackRoll +
                (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0) +
                actionModifier.AttackRollModifier;

            if (armorClass > totalAttack)
            {
                yield break;
            }

            var chaMod = GetChaModifier(rulesetHelper);

            if (armorClass + chaMod <= totalAttack)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerSpiritualShielding, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, Id.PowerReaction)
                {
                    StringParameter = "SpiritualShielding",
                    StringParameter2 = "UseSpiritualShieldingDescription".Formatted(
                        Category.Reaction, attacker.Name, defender.Name, chaMod.ToString()),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, actionService, count);
        }
    }
}
