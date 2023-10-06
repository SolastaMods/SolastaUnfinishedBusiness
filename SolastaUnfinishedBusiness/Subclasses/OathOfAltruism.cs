using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
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

        var featureSpiritualShielding = FeatureDefinitionBuilder
            .Create($"Feature{Name}SpiritualShielding")
            .SetGuiPresentation(Category.Feature, ShieldOfFaith)
            .AddCustomSubFeatures(new SpiritualShieldingBlockAttack())
            .AddToDB();

        var featureDefensiveStrike = FeatureDefinitionBuilder
            .Create(DefensiveStrike)
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDecisiveStrike)
            .AddCustomSubFeatures(DefensiveStrikeMarker.Mark)
            .AddToDB();

        var conditionAuraOfTheGuardian = ConditionDefinitionBuilder
            .Create($"Condition{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Condition, ConditionShielded)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianConditionMarker)
            .AddToDB();

        var powerAuraOfTheGuardian = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Feature, GuardianOfFaith)
            .AddCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianUserMarker)
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
            .AddCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianUserMarker)
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
                featureSpiritualShielding)
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

    private class SpiritualShieldingBlockAttack : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
            GameLocationBattleManager battleManager,
            GameLocationCharacter me,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            ActionModifier attackModifier,
            int attackRoll)
        {
            if (rulesetEffect != null
                && rulesetEffect.EffectDescription.RangeType != RangeType.Touch
                && rulesetEffect.EffectDescription.RangeType != RangeType.MeleeHit)
            {
                yield break;
            }

            var unitCharacter = me.RulesetCharacter;

            if (me == defender)
            {
                yield break;
            }

            //Is this unit able to react (not paralyzed, prone etc.)?
            if (!me.CanReact(true))
            {
                yield break;
            }

            //Can this unit see defender?
            if (!me.PerceivedAllies.Contains(defender))
            {
                yield break;
            }

            //Does this unit has enough Channel Divinity uses left?
            var maxUses = unitCharacter.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);

            if (unitCharacter.UsedChannelDivinity >= maxUses)
            {
                yield break;
            }

            //Is defender already shielded?
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.HasConditionOfType(ConditionShielded))
            {
                yield break;
            }

            var totalAttack = attackRoll
                              + (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0)
                              + attackModifier.AttackRollModifier;

            //Can shielding prevent hit?
            if (!rulesetDefender.CanMagicEffectPreventHit(Shield, totalAttack))
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;

            var actionParams = new CharacterActionParams(me, (Id)ExtraActionId.DoNothingReaction)
            {
                StringParameter = "CustomReactionSpiritualShieldingDescription"
                    .Formatted(Category.Reaction, defender.Name, attacker.Name)
            };

            RequestCustomReaction("SpiritualShielding", actionParams);

            yield return battleManager.WaitForReactions(me, actionService, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            //Spend resources
            unitCharacter.UsedChannelDivinity++;

            rulesetDefender.InflictCondition(
                ConditionShielded.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                unitCharacter.guid,
                unitCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }

        private static void RequestCustomReaction(string type, CharacterActionParams actionParams)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionManager == null)
            {
                return;
            }

            var reactionRequest = new ReactionRequestCustom(type, actionParams)
            {
                Resource = ReactionResourceChannelDivinity.Instance
            };

            actionManager.AddInterruptRequest(reactionRequest);
        }
    }
}
