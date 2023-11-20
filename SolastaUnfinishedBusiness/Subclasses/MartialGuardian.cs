using System.Collections;
using System.Collections.Generic;
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

    public MartialGuardian()
    {
        // LEVEL 03

        // Compelling Strike

        var actionAffinityCompellingStrike = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, $"ActionAffinity{Name}CompellingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.CompellingStrikeToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasShield),
                new AttackBeforeHitConfirmedOnEnemyCompellingStrike())
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
                            .Build(),
                        EffectFormBuilder.ConditionForm(CustomConditionsContext.Taunter,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .Build())
            .AddToDB();

        //
        // LEVEL 15
        //

        // Vigilance

        var perceptionAffinityVigilance = FeatureDefinitionPerceptionAffinityBuilder
            .Create($"PerceptionAffinity{Name}Vigilance")
            .SetGuiPresentation(Category.Feature)
            .CannotBeSurprised()
            .AddToDB();

        var featureSetVigilance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Vigilance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(ActionAffinityReactive, perceptionAffinityVigilance)
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
            .AddFeaturesAtLevel(15, featureSetVigilance)
            .AddFeaturesAtLevel(18, featureImperviousProtector)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.CompellingStrikeToggle))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                CustomConditionsContext.Taunter.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                CustomConditionsContext.Taunter.Name,
                0,
                0,
                0);

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
                CustomConditionsContext.Taunted.Name,
                0,
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
                AttributeDefinitions.TagCombat,
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
