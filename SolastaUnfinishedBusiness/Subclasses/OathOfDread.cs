using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfDread : AbstractSubclass
{
    private const string Name = "OathOfDread";

    internal OathOfDread()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("Subclass/&OathOfDreadTitle", "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, Bane, ShieldOfFaith),
                BuildSpellGroup(5, HoldPerson, SeeInvisibility),
                BuildSpellGroup(9, Fear, Haste),
                BuildSpellGroup(13, GuardianOfFaith, PhantasmalKiller),
                BuildSpellGroup(17, DominatePerson, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        // Mark of Submission

        var conditionMarkOfTheSubmission = ConditionDefinitionBuilder
            .Create($"Condition{Name}MarkOfTheSubmission")
            .SetGuiPresentation(Category.Condition, ConditionMarkedByBrandingSmite)
            .SetConditionType(ConditionType.Detrimental)
            .SetConditionParticleReference(ConditionMarkedByBrandingSmite.conditionParticleReference)
            .SetPossessive()
            .AddToDB();

        var combatAffinityMarkOfTheSubmission = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}MarkOfTheSubmission")
            .SetGuiPresentation($"Condition{Name}MarkOfTheSubmission", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetHasCondition, conditionMarkOfTheSubmission)
            .AddToDB();

        var powerMarkOfTheSubmission = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MarkOfTheSubmission")
            .SetGuiPresentation($"FeatureSet{Name}MarkOfTheSubmission", Category.Feature,
                Sprites.GetSprite("PowerMarkOfTheSubmission", Resources.PowerMarkOfTheSubmission, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 2, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionMarkOfTheSubmission,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetMarkOfTheSubmission = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MarkOfTheSubmission")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(combatAffinityMarkOfTheSubmission, powerMarkOfTheSubmission)
            .AddToDB();

        // Dreadful Presence

        var powerDreadfulPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DreadfulPresence")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDreadfulPresence", Resources.PowerDreadfulPresence, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(PowerFighterActionSurge.EffectDescription.effectParticleParameters)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 07

        // Aura of Domination

        var featureAuraOfDomination = FeatureDefinitionBuilder
            .Create($"Feature{Name}AuraOfDomination")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionAuraOfDomination = ConditionDefinitionBuilder
            .Create($"Condition{Name}AuraOfDomination")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(featureAuraOfDomination)
            .SetConditionParticleReference(ConditionBaned.conditionParticleReference)
            .AddToDB();

        featureAuraOfDomination.SetCustomSubFeatures(
            new CharacterTurnStartListenerAuraOfDomination(conditionAuraOfDomination));

        var powerAuraOfDomination = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}AuraOfDomination")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerAuraOfDomination", Resources.PowerAuraOfDomination, 256, 128))
            .AddToDB();

        // keep it simple and ensure it'll follow any changes from Aura of Protection
        powerAuraOfDomination.EffectDescription.targetSide = Side.Enemy;
        powerAuraOfDomination.EffectDescription.targetExcludeCaster = true;
        powerAuraOfDomination.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraOfDomination, ConditionForm.ConditionOperation.Add)
            .Build();

        // LEVEL 15

        // Harrowing Crusade

        var powerHarrowingCrusade = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HarrowingCrusade")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ReactionTriggerContext.HitByMelee)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.OathOfDread, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                featureSetMarkOfTheSubmission,
                powerDreadfulPresence)
            .AddFeaturesAtLevel(7,
                powerAuraOfDomination)
            .AddFeaturesAtLevel(15,
                powerHarrowingCrusade)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Aura of Domination
    //

    private sealed class CharacterTurnStartListenerAuraOfDomination : ICharacterTurnStartListener
    {
        private readonly ConditionDefinition _conditionDefinition;

        public CharacterTurnStartListenerAuraOfDomination(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetDefender = locationCharacter.RulesetCharacter;

            if (!rulesetDefender.HasConditionOfType(ConditionDefinitions.ConditionFrightened) &&
                !rulesetDefender.HasConditionOfType(ConditionDefinitions.ConditionFrightenedFear))
            {
                return;
            }

            var rulesetCondition = rulesetDefender.AllConditions.FirstOrDefault(x =>
                x.ConditionDefinition == _conditionDefinition);

            if (rulesetCondition == null)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var locationCharacterAttacker =
                gameLocationCharacterService.PartyCharacters.FirstOrDefault(x => x.Guid == rulesetCondition.SourceGuid);

            if (locationCharacterAttacker == null)
            {
                return;
            }

            var rulesetAttacker = locationCharacterAttacker.RulesetCharacter;
            var newCondition = RulesetCondition.CreateActiveCondition(
                rulesetDefender.Guid,
                ConditionDefinitions.ConditionRestrained,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                rulesetAttacker.Guid,
                rulesetAttacker.BaseFaction.Name);

            rulesetDefender.AddConditionOfCategory(AttributeDefinitions.TagCombat, newCondition);

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Paladin);
            var totalDamage = classLevel / 2;
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeNecrotic, DieType = DieType.D1, DiceNumber = 0, BonusDamage = totalDamage
            };

            RulesetActor.InflictDamage(
                totalDamage,
                damageForm,
                DamageTypeNecrotic,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                rulesetDefender,
                false,
                rulesetAttacker.Guid,
                false,
                new List<string>(),
                new RollInfo(DieType.D1, new List<int>(), totalDamage),
                true,
                out _);
        }
    }

    //
    //
    //

#if false
    private class TargetReducedToZeroHpMomentum : ITargetReducedToZeroHp
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _featureDefinition;

        public TargetReducedToZeroHpMomentum(
            FeatureDefinition featureDefinition,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinition = featureDefinition;
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (attacker.CurrentActionRankByType[ActionDefinitions.ActionType.Reaction] > 0)
            {
                yield break;
            }

            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService == null)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var reactionParams =
                new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "Reaction/&CustomReactionMomentumDescription"
                };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("Momentum", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            GameConsoleHelper.LogCharacterUsedFeature(rulesetAttacker, _featureDefinition);

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetAttacker.Guid,
                _conditionDefinition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name);

            rulesetAttacker.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }
#endif
}
