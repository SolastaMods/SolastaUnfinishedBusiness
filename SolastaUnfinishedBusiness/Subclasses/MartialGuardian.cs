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
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialGuardian : AbstractSubclass
{
    private const string Name = "MartialGuardian";
    private const string ConditionVigilanceName = $"Condition{Name}Vigilance";

    public MartialGuardian()
    {
        // LEVEL 03

        // Compelling Strike

        var actionAffinityCompellingStrike = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, $"ActionAffinity{Name}CompellingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.CompellingStrikeToggle)
            .AddCustomSubFeatures(new AttackBeforeHitConfirmedOnEnemyCompellingStrike())
            .AddToDB();

        // Stalwart Front (Sentinel FS)

        var proficiencySentinel = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Sentinel")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.FightingStyle, "Sentinel")
            .AddToDB();

        //
        // LEVEL 07
        //

        // Unyielding

        var savingThrowAffinityUnyielding = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Unyielding")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SavingThrow, AttributeDefinitions.Wisdom)
            .AddCustomSubFeatures(new ModifyCoverTypeUnyielding())
            .AddToDB();

        //
        // LEVEL 10
        //

        // Grand Challenge

        var powerGrandChallenge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GrandChallenge")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerShoutOfProvocation", Resources.PowerShoutOfProvocation, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.PerceivingWithinDistance, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution)
                    .SetParticleEffectParameters(PowerMartialCommanderInvigoratingShout)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(CustomConditionsContext.Taunted, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // LEVEL 15
        //

        // Vigilance

        _ = ConditionDefinitionBuilder
            .Create(ConditionVigilanceName)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var perceptionAffinityVigilance = FeatureDefinitionPerceptionAffinityBuilder
            .Create($"PerceptionAffinity{Name}Vigilance")
            .SetGuiPresentation(Category.Feature)
            .CannotBeSurprised()
            .AddToDB();

        //
        // LEVEL 18
        //

        // Impervious Protector

        var conditionImperviousProtector = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImperviousProtector")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                DamageAffinityBludgeoningResistance, DamageAffinityPiercingResistance, DamageAffinitySlashingResistance)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var featureImperviousProtector = FeatureDefinitionBuilder
            .Create($"Feature{Name}ImperviousProtector")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new CustomBehaviorImperviousProtector(conditionImperviousProtector, powerGrandChallenge))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, FightingStyleDefinitions.Protection)
            .AddFeaturesAtLevel(3, actionAffinityCompellingStrike, proficiencySentinel)
            .AddFeaturesAtLevel(7, savingThrowAffinityUnyielding)
            .AddFeaturesAtLevel(10, powerGrandChallenge)
            .AddFeaturesAtLevel(15, perceptionAffinityVigilance)
            .AddFeaturesAtLevel(18, featureImperviousProtector)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void HandleVigilance(RulesetCharacter rulesetCharacter)
    {
        if (rulesetCharacter.GetSubclassLevel(Fighter, Name) > 0)
        {
            return;
        }

        foreach (var guardian in Gui.Battle.AllContenders
                     .Where(x => x.RulesetCharacter.GetSubclassLevel(Fighter, Name) > 0))
        {
            var rulesetGuardian = guardian.RulesetCharacter;

            if (guardian.CanReact() || rulesetGuardian.HasConditionOfType(ConditionVigilanceName))
            {
                continue;
            }

            guardian.RefundActionUse(ActionDefinitions.ActionType.Reaction);
            guardian.ActionRefunded?.Invoke(guardian, ActionDefinitions.ActionType.Reaction);

            rulesetGuardian.InflictCondition(
                ConditionVigilanceName,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetGuardian.Guid,
                rulesetGuardian.CurrentFaction.Name,
                1,
                ConditionVigilanceName,
                0,
                0,
                0);
        }
    }

    //
    // Compelling Strike
    //

    private sealed class AttackBeforeHitConfirmedOnEnemyCompellingStrike : IAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker!.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.CompellingStrikeToggle))
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode))
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.InflictCondition(
                CustomConditionsContext.Taunted.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                // we need this to check for the condition range later on
                $"ActionAffinity{Name}CompellingStrike",
                1,
                0,
                0);
        }
    }

    //
    // Unyielding
    //

    private sealed class ModifyCoverTypeUnyielding : IModifyCoverType
    {
        public void ModifyCoverType(
            GameLocationCharacter attacker,
            int3 attackerPosition,
            GameLocationCharacter defender,
            int3 defenderPosition,
            ActionModifier attackModifier,
            ref CoverType bestCoverType,
            bool ignoreCoverFromCharacters)
        {
            if (bestCoverType < CoverType.Half &&
                ValidatorsCharacter.HasHeavyArmor(defender.RulesetCharacter) &&
                !defender.RulesetCharacter.IsIncapacitated)
            {
                bestCoverType = CoverType.Half;
            }
        }
    }

    //
    // Impervious Protector
    //

    private sealed class CustomBehaviorImperviousProtector :
        ICharacterBattleStartedListener, IAttackBeforeHitConfirmedOnMe
    {
        private const string Line = "Feedback/&ActivateRepaysLine";
        private readonly ConditionDefinition _conditionImperviousProtector;
        private readonly FeatureDefinitionPower _powerGrandChallenge;

        public CustomBehaviorImperviousProtector(
            ConditionDefinition conditionImperviousProtector,
            FeatureDefinitionPower powerGrandChallenge)
        {
            _conditionImperviousProtector = conditionImperviousProtector;
            _powerGrandChallenge = powerGrandChallenge;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (attackMode.Magical || !ValidatorsCharacter.HasHeavyArmor(rulesetDefender))
            {
                yield break;
            }

            rulesetDefender.InflictCondition(
                _conditionImperviousProtector.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                _conditionImperviousProtector.Name,
                0,
                0,
                0);
        }

        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var rulesetUsablePower = UsablePowersProvider.Get(_powerGrandChallenge, rulesetCharacter);

            if (rulesetUsablePower.MaxUses == rulesetUsablePower.RemainingUses)
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedPower(_powerGrandChallenge, Line);
            rulesetCharacter.RepayPowerUse(rulesetUsablePower);
        }
    }
}
