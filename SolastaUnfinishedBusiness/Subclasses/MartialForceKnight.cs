using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialForceKnight : AbstractSubclass
{
    private const string Name = "MartialForceKnight";

    private const ActionDefinitions.Id ForcePoweredStrikeToggle =
        (ActionDefinitions.Id)ExtraActionId.ForcePoweredStrikeToggle;

    public MartialForceKnight()
    {
        // LEVEL 03

        // Psionic Initiate

        var powerPsionicInitiate = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsionicInitiate")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 3)
            .AddCustomSubFeatures(
                HasModifiedUses.Marker,
                IsModifyPowerPool.Marker,
                ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Force-Powered Strike

        _ = ActionDefinitionBuilder
            .Create(MetamagicToggle, "ForcePoweredStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ForcePoweredStrikeToggle)
            .AddCustomSubFeatures(new ActionItemDiceBoxForcePoweredStrike(powerPsionicInitiate))
            .AddToDB();

        var actionAffinityForcePoweredStrikeToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityForcePoweredStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ForcePoweredStrikeToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerPsionicInitiate)))
            .AddToDB();

        var powerForcePoweredStrike = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ForcePoweredStrike")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicInitiate)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeForce, 1, DieType.D6))
                    .Build())
            .AddToDB();

        powerForcePoweredStrike.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorForcePoweredStrike(powerForcePoweredStrike));

        // Kinetic Barrier

        var conditionKineticBarrier = ConditionDefinitionBuilder
            .Create($"Condition{Name}KineticBarrier")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}KineticBarrier")
                    .SetGuiPresentation($"Power{Name}KineticBarrier", Category.Feature)
                    .SetModifier(
                        FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddConditionAmount,
                        AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedKineticBarrier())
            .SetFixedAmount(1)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerKineticBarrier = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}KineticBarrier")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.Reaction, powerPsionicInitiate)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionKineticBarrier))
                    .Build())
            .AddToDB();

        powerKineticBarrier.AddCustomSubFeatures(
            new AttackBeforeHitPossibleOnMeOrAllyKineticBarrier(powerKineticBarrier));

        // Force Drive

        var forceDriveSprite = Sprites.GetSprite("PowerForceDrive", Resources.PowerForceDrive, 256, 128);

        var conditionForceDrive = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForceDrive")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeroism)
            .SetPossessive()
            .AddCustomSubFeatures(new ModifyWeaponModifyAttackModeForceDrive())
            .AddToDB();

        var powerForceDriveOncePerShort = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForceDriveOncePerShort")
            .SetGuiPresentation($"Power{Name}ForceDrive", Category.Feature, forceDriveSprite)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceDrive))
                    .Build())
            .AddToDB();

        powerForceDriveOncePerShort.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerForceDriveOncePerShort) > 0));

        var powerForceDrive = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ForceDrive")
            .SetGuiPresentation(Category.Feature, forceDriveSprite)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicInitiate)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceDrive))
                    .Build())
            .AddToDB();

        powerForceDrive.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerForceDriveOncePerShort) == 0));

        // Psionic Initiate

        var featureSetPsionicInitiate = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicInitiate")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerPsionicInitiate,
                powerForcePoweredStrike, actionAffinityForcePoweredStrikeToggle,
                powerKineticBarrier,
                powerForceDrive, powerForceDriveOncePerShort)
            .AddToDB();

        // LEVEL 07

        // Psionic Adept

        var powerPsionicAdept = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsionicAdept")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerPsionicAdeptPush = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsionicAdeptPush")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicAdept)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerPsionicAdeptProne = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsionicAdeptProne")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicAdept)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerPsionicAdept, true,
            powerPsionicAdeptProne, powerPsionicAdeptPush);

        powerForcePoweredStrike.AddCustomSubFeatures(new MagicEffectFinishedByMePsionicAdept(powerPsionicAdept));

        var featureSetPsionicAdept = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicAdept")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPsionicInitiate, powerPsionicAdeptProne, powerPsionicAdeptPush)
            .AddToDB();

        // Psionic Propulsion

        var psionicPropulsionSprite =
            Sprites.GetSprite("PowerPsionicPropulsion", Resources.PowerPsionicPropulsion, 256, 128);

        for (var i = 2; i <= 18; i += 2)
        {
            if (!DatabaseRepository.GetDatabase<FeatureDefinitionMoveMode>()
                    .TryGetElement($"MoveModeFly{i}", out var moveMode))
            {
                continue;
            }

            var conditionPsionicPropulsion = ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionFlying, $"Condition{Name}PsionicPropulsion{i}")
                .SetOrUpdateGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature)
                .SetPossessive()
                .SetParentCondition(ConditionDefinitions.ConditionFlying)
                .AddFeatures(moveMode)
                .AddToDB();

            conditionPsionicPropulsion.GuiPresentation.description = Gui.NoLocalization;

            // there is indeed a typo on tag
            // ReSharper disable once StringLiteralTypo
            conditionPsionicPropulsion.ConditionTags.Add("Verticality");
        }

        var powerPsionicPropulsionOncePerShort = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsionicPropulsionOncePerShort")
            .SetGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature, psionicPropulsionSprite)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // only a placeholder
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlying))
                    .Build())
            .AddToDB();

        powerPsionicPropulsionOncePerShort.AddCustomSubFeatures(
            new ModifyEffectDescriptionPsionicPropulsion(powerPsionicPropulsionOncePerShort),
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerPsionicPropulsionOncePerShort) > 0));

        var powerPsionicPropulsion = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsionicPropulsion")
            .SetGuiPresentation(Category.Feature, psionicPropulsionSprite)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlying))
                    .Build())
            .AddToDB();

        powerPsionicPropulsion.AddCustomSubFeatures(
            new ModifyEffectDescriptionPsionicPropulsion(powerPsionicPropulsion),
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerPsionicPropulsionOncePerShort) == 0));

        var featureSetPsionicPropulsion = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicPropulsion")
            .SetGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature)
            .AddFeatureSet(powerPsionicPropulsion, powerPsionicPropulsionOncePerShort)
            .AddToDB();

        // LEVEL 10

        // Force of Will

        var featureForceOfWill = FeatureDefinitionBuilder
            .Create($"Feature{Name}ForceOfWill")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureForceOfWill.AddCustomSubFeatures(new ForceOfWill(featureForceOfWill));

        // LEVEL 15

        // Force Bulwark

        var conditionForceBulwark = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForceBulwark")
            .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}ForceBulwark")
                    .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature)
                    .SetPermanentCover(CoverType.Half)
                    .AddToDB())
            .AddToDB();

        conditionForceBulwark.GuiPresentation.description = Gui.NoLocalization;

        var powerForceBulwark = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ForceBulwark")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerForceBulwark", Resources.PowerForceBulwark, 128))
            .SetSharedPool(ActivationTime.BonusAction, powerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceBulwark))
                    .Build())
            .AddToDB();

        powerForceBulwark.AddCustomSubFeatures(new ModifyEffectDescriptionForceBulwark(powerForceBulwark));

        // LEVEL 18

        // Telekinetic Grasp

        var powerTelekineticGrasp = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}TelekineticGrasp")
            .SetGuiPresentation(Category.Feature, SpellsContext.Telekinesis)
            .SetSharedPool(ActivationTime.Action, powerPsionicInitiate)
            .SetShowCasting(false)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeTelekineticGrasp())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardGravityMage, 256))
            .AddFeaturesAtLevel(3, featureSetPsionicInitiate)
            .AddFeaturesAtLevel(6, BuildPowerModifier(powerPsionicInitiate, 6))
            .AddFeaturesAtLevel(7, featureSetPsionicAdept, featureSetPsionicPropulsion)
            .AddFeaturesAtLevel(9, BuildPowerModifier(powerPsionicInitiate, 9))
            .AddFeaturesAtLevel(10, featureForceOfWill)
            .AddFeaturesAtLevel(12, BuildPowerModifier(powerPsionicInitiate, 12))
            .AddFeaturesAtLevel(15, BuildPowerModifier(powerPsionicInitiate, 15), powerForceBulwark)
            .AddFeaturesAtLevel(18, BuildPowerModifier(powerPsionicInitiate, 18), powerTelekineticGrasp)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPowerUseModifier BuildPowerModifier(
        FeatureDefinitionPower powerPsionicInitiate, int level)
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}PsionicInitiate{level:00}")
            .SetGuiPresentationNoContent(true)
            .SetFixedValue(powerPsionicInitiate, 1)
            .AddToDB();
    }

    private static int GetIntModifier(
        // ReSharper disable once SuggestBaseTypeForParameter
        RulesetCharacter rulesetCharacter)
    {
        var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
        var intMod = AttributeDefinitions.ComputeAbilityScoreModifier(intelligence);

        return Math.Max(1, intMod);
    }

    private static DieType GetForcePoweredStrikeSize(RulesetCharacter character)
    {
        var level = character.GetSubclassLevel(CharacterClassDefinitions.Fighter, Name);

        return level switch
        {
            >= 17 => DieType.D12,
            >= 11 => DieType.D10,
            >= 5 => DieType.D8,
            _ => DieType.D6
        };
    }

    //
    // Force Powered Strike
    //

    private sealed class CustomBehaviorForcePoweredStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerForcePoweredStrike) : IModifyEffectDescription, IPhysicalAttackFinishedByMe
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerForcePoweredStrike;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var dieType = GetForcePoweredStrikeSize(character);
            var intMod = GetIntModifier(character);
            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            damageForm.BonusDamage = intMod;
            damageForm.DieType = dieType;

            return effectDescription;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionManager == null)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.OnceInMyTurnIsValid(powerForcePoweredStrike.Name) ||
                !rulesetAttacker.IsToggleEnabled(ForcePoweredStrikeToggle))
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(powerForcePoweredStrike.Name, 0);

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerForcePoweredStrike, rulesetAttacker);
            var actionParams =
                new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
                {
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    private sealed class ActionItemDiceBoxForcePoweredStrike(FeatureDefinitionPower powerPsionicInitiate)
        : IActionItemDiceBox
    {
        public (DieType type, int number, string format) GetDiceInfo(RulesetCharacter character)
        {
            return (GetForcePoweredStrikeSize(character), character.GetRemainingPowerUses(powerPsionicInitiate),
                "Screen/&ForcePoweredStrikeDieDescription");
        }
    }

    //
    // Kinetic Barrier
    //

    private class AttackBeforeHitPossibleOnMeOrAllyKineticBarrier(FeatureDefinitionPower powerKineticBarrier)
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
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionManager == null)
            {
                yield break;
            }

            if (rulesetEffect != null &&
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;
            var intMod = GetIntModifier(rulesetHelper);

            if (!helper.CanReact() ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingPowerUses(powerKineticBarrier) == 0)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;
            var armorClass = rulesetDefender.RefreshArmorClass(true).CurrentValue;

            var totalAttack =
                attackRoll +
                actionModifier.AttackRollModifier +
                (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0);
            var requiredACAddition = totalAttack - armorClass + 1;

            // if other actions already blocked it or if intMod isn't enough
            if (requiredACAddition <= 0 || requiredACAddition > intMod)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerKineticBarrier, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "KineticBarrier",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(helper, actionManager, count);
        }
    }

    private sealed class OnConditionAddedOrRemovedKineticBarrier : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var caster = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (caster == null)
            {
                return;
            }

            var intMod = GetIntModifier(caster);

            rulesetCondition.Amount = intMod;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Psionic Adept
    //

    private sealed class MagicEffectFinishedByMePsionicAdept(FeatureDefinitionPower powerPsionicInitiate) :
        IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionManager == null ||
                battleManager is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var levels = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Fighter);

            if (levels < 7)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerPsionicInitiate, rulesetCharacter);
            var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                StringParameter = "PsionicAdept",
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { action.ActionParams.TargetCharacters[0] }
            };

            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(actingCharacter, actionManager, count);
        }
    }

    //
    // Force Drive
    //

    private sealed class ModifyWeaponModifyAttackModeForceDrive : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            attackMode.reachRange += 6;
        }
    }

    //
    // Psionic Propulsion
    //

    private sealed class ModifyEffectDescriptionPsionicPropulsion(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerPsionicPropulsion) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerPsionicPropulsion;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null)
            {
                return effectDescription;
            }

            var flyMoves = Math.Min(glc.MaxTacticalMoves, 9) * 2;

            if (DatabaseRepository.GetDatabase<ConditionDefinition>()
                .TryGetElement($"Condition{Name}PsionicPropulsion{flyMoves}", out var condition))
            {
                return effectDescription;
            }

            effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = condition;

            return effectDescription;
        }
    }

    //
    // Force of Will
    //

    private sealed class ForceOfWill(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureForceOfWill)
        : ICharacterTurnStartListener, IRollSavingThrowInitiated
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
            var intMod = AttributeDefinitions.ComputeAbilityScoreModifier(intelligence);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                intMod, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            var intelligence = defender.TryGetAttributeValue(AttributeDefinitions.Intelligence);

            if (abilityScoreName == AttributeDefinitions.Wisdom)
            {
                var wisdom = defender.TryGetAttributeValue(AttributeDefinitions.Wisdom);

                if (intelligence > wisdom)
                {
                    abilityScoreName = AttributeDefinitions.Intelligence;

                    rollModifier += AttributeDefinitions.ComputeAbilityScoreModifier(intelligence) -
                                    AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);

                    defender.LogCharacterUsedFeature(featureForceOfWill);
                }
            }

            // ReSharper disable once InvertIf
            if (abilityScoreName == AttributeDefinitions.Charisma)
            {
                var charisma = defender.TryGetAttributeValue(AttributeDefinitions.Charisma);

                // ReSharper disable once InvertIf
                if (intelligence > charisma)
                {
                    abilityScoreName = AttributeDefinitions.Intelligence;

                    rollModifier += AttributeDefinitions.ComputeAbilityScoreModifier(intelligence) -
                                    AttributeDefinitions.ComputeAbilityScoreModifier(charisma);

                    defender.LogCharacterUsedFeature(featureForceOfWill);
                }
            }
        }
    }

    //
    // Force Bulwark
    //

    private sealed class ModifyEffectDescriptionForceBulwark(
        BaseDefinition powerForceBulwark) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerForceBulwark;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var intMod = GetIntModifier(character);

            effectDescription.targetParameter = intMod;

            return effectDescription;
        }
    }

    //
    // Telekinetic Grasp
    //

    private sealed class MagicEffectFinishedByMeTelekineticGrasp : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(rulesetCharacter, null, SpellsContext.Telekinesis, 5, false);

            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.CastNoCost];
            actionParams.RulesetEffect = effectSpell;

            actionService.ExecuteAction(actionParams, null, true);
        }
    }
}
