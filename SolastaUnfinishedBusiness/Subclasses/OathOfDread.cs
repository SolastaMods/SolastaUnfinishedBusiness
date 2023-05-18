using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
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
            .SetGuiPresentation(Name, Category.Subclass, "Feature/&DomainSpellsDescription")
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Bane, ShieldOfFaith),
                BuildSpellGroup(5, HoldPerson, SeeInvisibility),
                BuildSpellGroup(9, Fear, Haste),
                BuildSpellGroup(13, GuardianOfFaith, PhantasmalKiller),
                BuildSpellGroup(17, DominatePerson, HoldMonster))
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        // Mark of Submission

        const string MARK_OF_SUBMISSION = "MarkOfTheSubmission";

        var conditionMarkOfTheSubmission = ConditionDefinitionBuilder
            .Create($"Condition{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation(Category.Condition, ConditionMarkedByBrandingSmite)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .AddToDB();

        var combatAffinityMarkOfTheSubmission = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation($"Condition{Name}{MARK_OF_SUBMISSION}", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetHasCondition, conditionMarkOfTheSubmission)
            .AddToDB();

        var powerMarkOfTheSubmission = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation($"FeatureSet{Name}{MARK_OF_SUBMISSION}", Category.Feature,
                Sprites.GetSprite(MARK_OF_SUBMISSION, Resources.PowerMarkOfTheSubmission, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 2, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(BestowCurseOnAttackRoll)
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
            .Create($"FeatureSet{Name}{MARK_OF_SUBMISSION}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(combatAffinityMarkOfTheSubmission, powerMarkOfTheSubmission)
            .AddToDB();

        // Dreadful Presence

        const string DREADFUL_PRESENCE = "DreadfulPresence";

        var powerDreadfulPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{DREADFUL_PRESENCE}")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite(DREADFUL_PRESENCE, Resources.PowerDreadfulPresence, 256, 128))
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

        const string AURA_DOMINATION = "AuraOfDomination";

        var featureAuraOfDomination = FeatureDefinitionBuilder
            .Create($"Feature{Name}{AURA_DOMINATION}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionAuraOfDomination = ConditionDefinitionBuilder
            .Create($"Condition{Name}{AURA_DOMINATION}")
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
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}{AURA_DOMINATION}")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite(AURA_DOMINATION, Resources.PowerAuraOfDomination, 256, 128))
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

        const string HARROWING_CRUSADE = "HarrowingCrusade";

        var featureHarrowingCrusade = FeatureDefinitionBuilder
            .Create($"Feature{Name}{HARROWING_CRUSADE}")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ReactToAttackOnMeOrMeFinishedHarrowingCrusade(conditionMarkOfTheSubmission))
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
                featureHarrowingCrusade)
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
        private readonly ConditionDefinition _conditionAuraOfDomination;

        public CharacterTurnStartListenerAuraOfDomination(ConditionDefinition conditionAuraOfDomination)
        {
            _conditionAuraOfDomination = conditionAuraOfDomination;
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
                x.ConditionDefinition == _conditionAuraOfDomination);

            if (rulesetCondition == null)
            {
                return;
            }

            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var locationCharacterAttacker = gameLocationCharacterService.PartyCharacters
                .FirstOrDefault(x => x.Guid == rulesetCondition.SourceGuid);

            if (locationCharacterAttacker == null)
            {
                return;
            }

            var rulesetAttacker = locationCharacterAttacker.RulesetCharacter;

            rulesetDefender.InflictCondition(
                CustomConditionsContext.StopMovement.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Paladin);
            var totalDamage = classLevel / 2;
            var damageForm = new DamageForm
            {
                DamageType = DamageTypePsychic, DieType = DieType.D1, DiceNumber = 0, BonusDamage = totalDamage
            };

            EffectHelpers.StartVisualEffect(locationCharacterAttacker, locationCharacter, DreadfulOmen);
            RulesetActor.InflictDamage(
                totalDamage,
                damageForm,
                DamageTypePsychic,
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
    // Harrowing Crusade
    //

    private class ReactToAttackOnMeOrMeFinishedHarrowingCrusade : IReactToAttackOnMeOrAllyFinished
    {
        private readonly ConditionDefinition _conditionMarkOfTheSubmission;

        public ReactToAttackOnMeOrMeFinishedHarrowingCrusade(ConditionDefinition conditionMarkOfTheSubmission)
        {
            _conditionMarkOfTheSubmission = conditionMarkOfTheSubmission;
        }

        public IEnumerator HandleReactToAttackOnAllyFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            GameLocationCharacter ally,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.HasConditionOfType(ConditionDefinitions.ConditionFrightened) &&
                !rulesetAttacker.HasConditionOfType(ConditionDefinitions.ConditionFrightenedFear) &&
                !rulesetAttacker.HasConditionOfType(_conditionMarkOfTheSubmission))
            {
                yield break;
            }

            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (!me.CanReact() ||
                me == ally ||
                rulesetEnemy == null ||
                rulesetEnemy.IsDeadOrDying)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = me.GetFirstRangedModeThatCanAttack(attacker);

                if (retaliationMode == null)
                {
                    yield break;
                }
            }

            // do I need to check this as well?
            if (!battle.IsWithinBattleRange(me, attacker))
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.StringParameter = ally.Name;
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("HarrowingCrusade", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);
        }
    }
}
