using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class SorcerousPsion : AbstractSubclass
{
    private const string Name = "SorcerousPsion";

    public SorcerousPsion()
    {
        // LEVEL 01

        var autoPreparedSpellsPsion = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Origin")
            .SetSpellcastingClass(CharacterClassDefinitions.Sorcerer)
            .AddPreparedSpellGroup(1, Shield)
            .AddPreparedSpellGroup(3, SpellsContext.PsychicWhip)
            .AddPreparedSpellGroup(5, SpellsContext.PulseWave)
            .AddPreparedSpellGroup(7, Confusion)
            .AddPreparedSpellGroup(9, MindTwist)
            .AddToDB();

        // Psionic Mind

        var bonusCantripsPsionicMind = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}PsionicMind")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypePsychic)
            .AddToDB();

        // Psychokinesis

        var powerPsychokinesisFixed = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsychokinesisFixed")
            .SetGuiPresentation($"FeatureSet{Name}Psychokinesis", Category.Feature, PowerMonkStepOfTheWindDash)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        powerPsychokinesisFixed.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character =>
                PowerProvider.Get(powerPsychokinesisFixed, character).RemainingUses > 0
                || character.GetClassLevel(CharacterClassDefinitions.Sorcerer) < 2));

        var powerPsychokinesisFixedDrag = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsychokinesisFixedDrag")
            .SetGuiPresentation($"Power{Name}PsychokinesisDrag", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, powerPsychokinesisFixed)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(true, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .Build())
            .AddToDB();

        var powerPsychokinesisFixedPush = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsychokinesisFixedPush")
            .SetGuiPresentation($"Power{Name}PsychokinesisPush", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, powerPsychokinesisFixed)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(true, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .Build())
            .AddToDB();

        var powerPsychokinesisPoints = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsychokinesisPoints")
            .SetGuiPresentation($"FeatureSet{Name}Psychokinesis", Category.Feature, PowerMonkStepOfTheWindDash)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 1, 0)
            .AddToDB();

        var powerPsychokinesisPointsDrag = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsychokinesisPointsDrag")
            .SetGuiPresentation($"Power{Name}PsychokinesisDrag", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, powerPsychokinesisPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(true, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .Build())
            .AddToDB();

        var powerPsychokinesisPointsPush = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsychokinesisPointsPush")
            .SetGuiPresentation($"Power{Name}PsychokinesisPush", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.BonusAction, powerPsychokinesisPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(true, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .Build())
            .AddToDB();

        powerPsychokinesisPoints.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character =>
                PowerProvider.Get(powerPsychokinesisFixed, character).RemainingUses == 0
                && character.GetClassLevel(CharacterClassDefinitions.Sorcerer) >= 2));

        PowerBundle.RegisterPowerBundle(powerPsychokinesisFixed, true,
            powerPsychokinesisFixedPush, powerPsychokinesisFixedDrag);

        PowerBundle.RegisterPowerBundle(powerPsychokinesisPoints, true,
            powerPsychokinesisPointsPush, powerPsychokinesisPointsDrag);

        var featureSetPsychokinesis = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Psychokinesis")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerPsychokinesisFixed,
                powerPsychokinesisFixedDrag,
                powerPsychokinesisFixedPush,
                powerPsychokinesisPoints,
                powerPsychokinesisPointsDrag,
                powerPsychokinesisPointsPush)
            .AddToDB();

        // LEVEL 06

        // Mind Sculpt

        var actionAffinityMindSculpt = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, $"ActionAffinity{Name}MindSculpt")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.MindSculptToggle)
            .AddCustomSubFeatures(new CustomBehaviorMindSculpt())
            .AddToDB();

        // LEVEL 14

        // Mind over Matter

        var powerMindOverMatter = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MindOverMatter")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.LongRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainSunHeraldOfTheSun)
                    .Build())
            .AddToDB();

        powerMindOverMatter.AddCustomSubFeatures(new OnReducedToZeroHpByEnemyMindOverMatter(powerMindOverMatter));

        // LEVEL 18

        // Supreme Will

        var powerSupremeWill = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SupremeWill")
            .SetGuiPresentation($"FeatureSet{Name}SupremeWill", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        var actionAffinitySupremeWill = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, $"ActionAffinity{Name}SupremeWill")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.SupremeWillToggle)
            .AddCustomSubFeatures(
                new CustomBehaviorSupremeWill(powerSupremeWill),
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerSupremeWill)))
            .AddToDB();

        var featureSetSupremeWill = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SupremeWill")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinitySupremeWill, powerSupremeWill)
            .AddToDB();

        // 
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheElements, 256))
            .AddFeaturesAtLevel(1, autoPreparedSpellsPsion, bonusCantripsPsionicMind, featureSetPsychokinesis)
            .AddFeaturesAtLevel(6, actionAffinityMindSculpt)
            .AddFeaturesAtLevel(14, powerMindOverMatter)
            .AddFeaturesAtLevel(18, featureSetSupremeWill)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Sorcerer;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceSorcerousOrigin;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Mind Sculpt
    //

    private sealed class CustomBehaviorMindSculpt : IMagicalAttackBeforeHitConfirmedOnEnemy, IActionFinishedByMe
    {
        private bool _hasDamageChanged;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionCastSpell)
            {
                yield break;
            }

            if (!_hasDamageChanged)
            {
                yield break;
            }

            _hasDamageChanged = false;

            var character = action.ActingCharacter.RulesetCharacter;

            character.SpendSorceryPoints(1);
            character.SorceryPointsAltered?.Invoke(character, character.RemainingSorceryPoints);
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            _hasDamageChanged = false;

            var character = attacker.RulesetCharacter;

            if (rulesetEffect is RulesetEffectSpell rulesetEffectSpell
                && rulesetEffectSpell.EffectDescription.HasDamageForm()
                && character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MindSculptToggle)
                && character.RemainingSorceryPoints > 0)
            {
                foreach (var effectForm in actualEffectForms
                             .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
                {
                    _hasDamageChanged = _hasDamageChanged || effectForm.DamageForm.DamageType != DamageTypePsychic;
                    effectForm.DamageForm.DamageType = DamageTypePsychic;
                }

                _hasDamageChanged = true;
            }

            if (!firstTarget)
            {
                yield break;
            }

            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                character.TryGetAttributeValue(AttributeDefinitions.Charisma));

            foreach (var effectForm in actualEffectForms
                         .Where(x =>
                             x.FormType == EffectForm.EffectFormType.Damage
                             && x.DamageForm.DamageType == DamageTypePsychic))
            {
                effectForm.DamageForm.BonusDamage = charismaModifier;
            }
        }
    }

    //
    // Mind over Matter
    //

    private sealed class OnReducedToZeroHpByEnemyMindOverMatter(FeatureDefinitionPower powerMindOverMatter)
        : IOnReducedToZeroHpByEnemy
    {
        public IEnumerator HandleReducedToZeroHpByEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter source,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetCharacter = source.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.CanUsePower(powerMindOverMatter))
            {
                yield break;
            }


            var reactionParams = new CharacterActionParams(source, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = "Reaction/&CustomReactionMindOverMatterDescription"
            };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("MindOverMatter", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService
                .WaitForReactions(attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var tempHitPoints = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Sorcerer) * 3;

            rulesetCharacter.StabilizeAndGainHitPoints(1);
            rulesetCharacter.ReceiveTemporaryHitPoints(
                tempHitPoints, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);

            var actionParams = new CharacterActionParams(source, ActionDefinitions.Id.SpendPower);
            var usablePower = PowerProvider.Get(powerMindOverMatter, rulesetCharacter);
            var targets = gameLocationBattleService.Battle.GetContenders(source, isWithinXCells: 2);
            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            actionParams.RulesetEffect = implementationManagerService
                //CHECK: no need for AddAsActivePowerToSource
                .MyInstantiateEffectPower(rulesetCharacter, usablePower, false);
            actionParams.TargetCharacters.SetRange(targets);

            EffectHelpers.StartVisualEffect(
                source, source, PowerPatronFiendDarkOnesBlessing, EffectHelpers.EffectType.Effect);

            foreach (var target in targets)
            {
                EffectHelpers.StartVisualEffect(
                    source, target, PowerDomainSunHeraldOfTheSun, EffectHelpers.EffectType.Effect);
            }

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, false);
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(new CharacterActionParams(source, ActionDefinitions.Id.StandUp), null, true);
        }
    }

    //
    // Supreme Will
    //

    private sealed class CustomBehaviorSupremeWill(FeatureDefinitionPower powerSupremeWill)
        : IModifyConcentrationRequirement, IActionFinishedByMe
    {
        private bool _hasConcentrationChanged;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionCastSpell actionCastSpell)
            {
                yield break;
            }

            if (!_hasConcentrationChanged)
            {
                yield break;
            }

            _hasConcentrationChanged = false;

            var character = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerSupremeWill, character);

            character.UsePower(usablePower);
            character.SpendSorceryPoints(2 * actionCastSpell.ActiveSpell.EffectLevel);
            character.SorceryPointsAltered?.Invoke(character, character.RemainingSorceryPoints);
        }

        public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell)
        {
            if (!rulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.SupremeWillToggle))
            {
                return rulesetEffectSpell.SpellDefinition.RequiresConcentration;
            }

            if (!rulesetCharacter.CanUsePower(powerSupremeWill))
            {
                return rulesetEffectSpell.SpellDefinition.RequiresConcentration;
            }

            if (!rulesetEffectSpell.SpellDefinition.RequiresConcentration)
            {
                return rulesetEffectSpell.SpellDefinition.RequiresConcentration;
            }

            var requiredPoints = rulesetEffectSpell.EffectLevel * 2;

            _hasConcentrationChanged = rulesetCharacter.RemainingSorceryPoints >= requiredPoints;

            return !_hasConcentrationChanged;
        }
    }
}
