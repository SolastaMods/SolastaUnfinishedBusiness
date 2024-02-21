#if false
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
            .AddCustomSubFeatures(HasModifiedUses.Marker, IsModifyPowerPool.Marker, ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerUseModifierPsionicInitiate = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}PsionicInitiate")
            .SetGuiPresentationNoContent(true)
            .SetFixedValue(powerPsionicInitiate, 1)
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

        var additionalDamageForcePoweredStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}ForcePoweredStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("ForcePoweredStrike")
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetAttackModeOnly()
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeForce)
            .AddCustomSubFeatures(new ModifyAdditionalDamageFormForcePoweredStrike(powerPsionicInitiate))
            .AddToDB();

        // Kinetic Barrier

        var conditionKineticBarrier = ConditionDefinitionBuilder
            .Create($"Condition{Name}KineticBarrier")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}KineticBarrier")
                    .SetGuiPresentation($"Condition{Name}KineticBarrier", Category.Condition)
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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceDrive))
                    .Build())
            .AddToDB();

        var powerForceDrive = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ForceDrive")
            .SetGuiPresentation(Category.Feature, forceDriveSprite)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceDrive))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(c => !c.CanUsePower(powerForceDriveOncePerShort)))
            .AddToDB();

        // Psionic Initiate

        var featureSetPsionicInitiate = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicInitiate")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerPsionicInitiate,
                actionAffinityForcePoweredStrikeToggle,
                additionalDamageForcePoweredStrike,
                powerKineticBarrier,
                powerForceDrive,
                powerForceDriveOncePerShort)
            .AddToDB();

        // LEVEL 07

        // Psionic Adept
        // Your psychokinetic abilities grow in strength. Your Force-Powered Strike can impose a Strength saving throw upon the creature struck by it (DC = 8 + proficiency + Intelligence modifier). If the creature fails it, you can either knock the creature back 15 feet or knock it prone.

        // Psionic Propulsion
        // You can expend 1 Force Point and use your bonus action to gain flying speed equal to twice your walking speed and not provoke opportunity attacks until the end of your turn. You can use this feature once per short rest without expending a Force Point.


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
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite("Telekinesis", Resources.Telekinesis, 128))
            .SetSharedPool(ActivationTime.Action, powerPsionicInitiate)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionKineticBarrier))
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeTelekineticGrasp())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardGravityMage, 256))
            .AddFeaturesAtLevel(3, featureSetPsionicInitiate)
            .AddFeaturesAtLevel(6, powerUseModifierPsionicInitiate)
            .AddFeaturesAtLevel(7)
            .AddFeaturesAtLevel(9, powerUseModifierPsionicInitiate)
            .AddFeaturesAtLevel(10, featureForceOfWill)
            .AddFeaturesAtLevel(12, powerUseModifierPsionicInitiate)
            .AddFeaturesAtLevel(15, powerUseModifierPsionicInitiate, powerForceBulwark)
            .AddFeaturesAtLevel(18, powerUseModifierPsionicInitiate, powerTelekineticGrasp)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static int GetIntModifier(
        // ReSharper disable once SuggestBaseTypeForParameter
        RulesetCharacter rulesetCharacter)
    {
        var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
        var intMod = AttributeDefinitions.ComputeAbilityScoreModifier(intelligence);

        return intMod;
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

    private sealed class ModifyAdditionalDamageFormForcePoweredStrike(
        FeatureDefinitionPower powerPsionicInitiate) : IModifyAdditionalDamageForm
    {
        public DamageForm AdditionalDamageForm(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            DamageForm damageForm)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled(ForcePoweredStrikeToggle) ||
                !rulesetAttacker.CanUsePower(powerPsionicInitiate))
            {
                damageForm.DiceNumber = 0;

                return damageForm;
            }

            var usablePower = PowerProvider.Get(powerPsionicInitiate, rulesetAttacker);
            var dieType = GetForcePoweredStrikeSize(rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);
            damageForm.DieType = dieType;

            return damageForm;
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
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (rulesetEffect != null &&
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (!helper.CanReact() ||
                !helper.CanPerceiveTarget(defender) ||
                !rulesetHelper.CanUsePower(powerKineticBarrier))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;
            var armorClass = rulesetDefender.RefreshArmorClass(true).CurrentValue;
            var intMod = GetIntModifier(rulesetHelper);

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
                    StringParameter =
                        Gui.Format("Reaction/&UseKineticBarrierDescription", defender.Name, attacker.Name),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(helper, gameLocationActionManager, count);
        }
    }

    private sealed class OnConditionAddedOrRemovedKineticBarrier : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var intMod = GetIntModifier(target);

            rulesetCondition.Amount = intMod;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Force Drive
    //

    private sealed class ModifyWeaponModifyAttackModeForceDrive : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            attackMode.AddAttackTagAsNeeded(TagsDefinitions.WeaponTagThrown);
            attackMode.thrown = true;
            attackMode.closeRange += 6;
            attackMode.maxRange += 6;
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
#endif
