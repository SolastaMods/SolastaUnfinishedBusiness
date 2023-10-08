using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
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
                UsablePowersProvider.Get(powerPsychokinesisFixed, character).RemainingUses > 0
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
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 2, 0)
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
                UsablePowersProvider.Get(powerPsychokinesisFixed, character).RemainingUses == 0
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
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.MindSculptToggle)
            .AddCustomSubFeatures(new ModifyEffectDescriptionMindSculpt())
            .AddToDB();

        var additionalDamageMindSculpt = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}MindSculpt")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("MindSculpt")
            .SetDamageValueDetermination(ExtraAdditionalDamageValueDetermination.CustomModifier)
            .SetSpecificDamageType(DamageTypePsychic)
            .AddCustomSubFeatures(
                new CustomModifierProvider(x => x.TryGetAttributeValue(AttributeDefinitions.Charisma)),
                new ValidateContextInsteadOfRestrictedProperty(
                    (_, _, _, _, _, _, rulesetEffect) =>
                        (OperationType.Set,
                            rulesetEffect != null
                            && rulesetEffect.EffectDescription.EffectForms.Any(x =>
                                x.FormType == EffectForm.EffectFormType.Damage
                                && x.DamageForm.DamageType == DamageTypePsychic))))
            .AddToDB();

        additionalDamageMindSculpt.firstTargetOnly = true;

        var featureSetMindSculpt = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MindSculpt")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(actionAffinityMindSculpt, additionalDamageMindSculpt)
            .AddToDB();

        // LEVEL 14

        // Mind over Matter

        var powerMindOverMatter = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MindOverMatter")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.LongRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerMindOverMatter.AddCustomSubFeatures(new OnReducedToZeroHpByEnemyMindOverMatter());

        // LEVEL 18

        // Supreme Will

        var actionAffinitySupremeWill = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, $"ActionAffinity{Name}SupremeWill")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.SupremeWillToggle)
            .AddCustomSubFeatures(new ModifyConcentrationRequirementSupremeWill())
            .AddToDB();

        var powerSupremeWill = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SupremeWill")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
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
            .AddFeaturesAtLevel(6, featureSetMindSculpt)
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

    private sealed class ModifyEffectDescriptionMindSculpt : IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition is SpellDefinition spellDefinition
                   && spellDefinition.EffectDescription.HasDamageForm()
                   && character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MindSculptToggle)
                   && character.RemainingSorceryPoints > 0;
        }

        // currently broken because of our effect descriptions cache
        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            foreach (var effectForm in effectDescription.EffectForms
                         .Where(x => x.FormType == EffectForm.EffectFormType.Damage))
            {
                effectForm.DamageForm.DamageType = DamageTypePsychic;
            }

            return effectDescription;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var character = action.ActingCharacter.RulesetCharacter;

            if (baseDefinition is SpellDefinition spellDefinition
                && spellDefinition.EffectDescription.HasDamageForm()
                && character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MindSculptToggle))
            {
                character.SpendSorceryPoints(1);
            }

            yield break;
        }
    }

    //
    // Mind over Matter
    //

    private sealed class OnReducedToZeroHpByEnemyMindOverMatter : IOnReducedToZeroHpByEnemy
    {
        public IEnumerator HandleReducedToZeroHpByEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter source,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetCharacter = source.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(source, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("MindOverMatter", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Sorcerer);

            rulesetCharacter.StabilizeAndGainHitPoints(1);
            rulesetCharacter.ReceiveTemporaryHitPoints(
                3 * classLevel, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
            rulesetCharacter.InflictCondition(
                ConditionDodging,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                source.Guid,
                string.Empty,
                1,
                null,
                0,
                0,
                0);

            EffectHelpers.StartVisualEffect(
                source, source, PowerPatronFiendDarkOnesBlessing, EffectHelpers.EffectType.Effect);

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(new CharacterActionParams(source, ActionDefinitions.Id.StandUp), null, false);
        }
    }

    //
    // Supreme Will
    //

    private sealed class ModifyConcentrationRequirementSupremeWill :
        IModifyConcentrationRequirement, IMagicEffectFinishedByMe
    {
        public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell)
        {
            if (!rulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.SupremeWillToggle))
            {
                return true;
            }

            var requiredPoints = rulesetEffectSpell.SpellDefinition.SpellLevel * 2;

            return rulesetCharacter.RemainingSorceryPoints < requiredPoints;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var character = action.ActingCharacter.RulesetCharacter;

            if (baseDefinition is SpellDefinition { RequiresConcentration: true } spellDefinition
                && character.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.SupremeWillToggle))
            {
                character.SpendSorceryPoints(2 * spellDefinition.SpellLevel);
            }

            yield break;
        }
    }
}
