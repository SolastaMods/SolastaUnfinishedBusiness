using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

[UsedImplicitly]
public sealed class OathOfDread : AbstractSubclass
{
    private const string Name = "OathOfDread";

    private static readonly ConditionDefinition ConditionAspectOfDreadEnemy = ConditionDefinitionBuilder
        .Create($"Condition{Name}AspectOfDreadEnemy")
        .SetGuiPresentation($"Condition{Name}AspectOfDread", Category.Condition)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .SetFeatures(
            FeatureDefinitionSavingThrowAffinityBuilder
                .Create($"SavingThrowAffinity{Name}AspectOfDreadEnemy")
                .SetGuiPresentation($"Condition{Name}AspectOfDread", Category.Condition, Gui.NoLocalization)
                .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Charisma)
                .AddToDB())
        .AddToDB();

    public OathOfDread()
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

        featureAuraOfDomination.AddCustomSubFeatures(
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
            .AddCustomSubFeatures(
                new ReactToAttackOnMeOrMeFinishedHarrowingCrusade(conditionMarkOfTheSubmission))
            .AddToDB();

        //
        // LEVEL 18
        //

        // Improved Aura of Domination

        var powerAuraOfDomination18 = FeatureDefinitionPowerBuilder
            .Create(powerAuraOfDomination, $"Power{Name}AuraOfDomination18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerAuraOfDomination)
            .AddToDB();

        powerAuraOfDomination18.EffectDescription.targetParameter = 13;

        //
        // LEVEL 20
        //

        // Aspect of Dread

        var additionalDamageAspectOfDread = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}AspectOfDread")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("AspectOfDread")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .AddToDB();

        var featureAspectOfDread = FeatureDefinitionBuilder
            .Create($"Feature{Name}AspectOfDread")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var featureSetAspectOfDread = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AspectOfDreadDamageResistance")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var conditionAspectOfDread = ConditionDefinitionBuilder
            .Create($"Condition{Name}AspectOfDread")
            .SetGuiPresentation(Category.Condition, ConditionPactChainImp)
            .SetPossessive()
            .AddFeatures(additionalDamageAspectOfDread, featureAspectOfDread, featureSetAspectOfDread)
            .AddToDB();

        foreach (var damage in DatabaseRepository.GetDatabase<DamageDefinition>())
        {
            var title = Gui.Localize($"Rules/&{damage.Name}Title");
            var damageAffinityAspectOfDread = FeatureDefinitionDamageAffinityBuilder
                .Create($"DamageAffinity{Name}AspectOfDread{damage.Name}")
                .SetGuiPresentation(title, Gui.Format("Feature/&DamageResistanceFormat", title))
                .SetDamageType(damage.Name)
                .SetDamageAffinityType(DamageAffinityType.Resistance)
                .AddToDB();

            featureSetAspectOfDread.FeatureSet.Add(damageAffinityAspectOfDread);
        }

        var powerAspectOfDread = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AspectOfDread")
            .SetGuiPresentation(Category.Feature, Sprites
                .GetSprite("PowerArdentHate", Resources.PowerArdentHate, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PowerFighterActionSurge)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionAspectOfDread, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
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
            .AddFeaturesAtLevel(18,
                powerAuraOfDomination18)
            .AddFeaturesAtLevel(20,
                powerAspectOfDread)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Paladin;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Aspect of Dread
    //

    internal static void OnRollSavingThrowAspectOfDread(
        RulesetCharacter caster,
        RulesetActor target,
        BaseDefinition sourceDefinition)
    {
        if (sourceDefinition is not ItemDefinition && // for smite spells but can bleed
            sourceDefinition is not FeatureDefinitionAdditionalDamage && // for smite spells but can bleed
            sourceDefinition is not SpellDefinition { castingTime: ActivationTime.Action } &&
            sourceDefinition is not FeatureDefinitionPower { RechargeRate: RechargeRate.ChannelDivinity })
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var gameLocationCaster = GameLocationCharacter.GetFromActor(caster);
        var gameLocationTarget = GameLocationCharacter.GetFromActor(target);

        if (gameLocationCaster == null ||
            gameLocationTarget == null ||
            gameLocationBattleService == null ||
            !gameLocationBattleService.IsWithinXCells(gameLocationCaster, gameLocationTarget, 2))
        {
            return;
        }

        if (!caster.HasAnyConditionOfType($"Condition{Name}AspectOfDread"))
        {
            return;
        }

        target.InflictCondition(
            ConditionAspectOfDreadEnemy.Name,
            ConditionAspectOfDreadEnemy.DurationType,
            ConditionAspectOfDreadEnemy.DurationParameter,
            ConditionAspectOfDreadEnemy.TurnOccurence,
            AttributeDefinitions.TagCombat,
            caster.guid,
            caster.CurrentFaction.Name,
            1,
            null,
            0,
            0,
            0);
    }

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
            var rulesetCondition = rulesetDefender.AllConditions.FirstOrDefault(x =>
                x.ConditionDefinition == _conditionAuraOfDomination);

            if (rulesetCondition == null)
            {
                return;
            }

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
            var hasFrightenedFromSource = rulesetDefender.AllConditions.Any(x =>
                x.SourceGuid == rulesetAttacker.Guid &&
                (x.ConditionDefinition == ConditionDefinitions.ConditionFrightened ||
                 x.ConditionDefinition.IsSubtypeOf(RuleDefinitions.ConditionFrightened)));

            if (!hasFrightenedFromSource)
            {
                return;
            }

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


            var locationCharacterAttacker = GameLocationCharacter.GetFromActor(rulesetAttacker);

            if (locationCharacterAttacker != null)
            {
                EffectHelpers.StartVisualEffect(locationCharacterAttacker, locationCharacter, DreadfulOmen);
            }

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

    private class ReactToAttackOnMeOrMeFinishedHarrowingCrusade : IPhysicalAttackFinishedOnMeOrAlly
    {
        private readonly ConditionDefinition _conditionMarkOfTheSubmission;

        public ReactToAttackOnMeOrMeFinishedHarrowingCrusade(ConditionDefinition conditionMarkOfTheSubmission)
        {
            _conditionMarkOfTheSubmission = conditionMarkOfTheSubmission;
        }

        public IEnumerator OnPhysicalAttackFinishedOnMeOrAlly(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter me,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var hasFrightened = rulesetAttacker.AllConditions.Any(x =>
                x.ConditionDefinition == ConditionDefinitions.ConditionFrightened ||
                x.ConditionDefinition.IsSubtypeOf(RuleDefinitions.ConditionFrightened));

            if (!hasFrightened && !rulesetAttacker.HasConditionOfType(_conditionMarkOfTheSubmission))
            {
                yield break;
            }

            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
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
            if (!gameLocationBattleService.IsWithinBattleRange(me, attacker))
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.StringParameter = defender.Name;
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("HarrowingCrusade", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);
        }
    }
}
