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
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialGuardian : AbstractSubclass
{
    private const string Name = "MartialGuardian";
    private const string FullName = $"Martial{Name}";
    private const string ConditionVigilanceName = $"Condition{Name}Vigilance";

    public MartialGuardian()
    {
        // LEVEL 03

        // Taunted Conditions

        var conditionTauntedSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}TauntedSelf")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var combatAffinityGrandChallengeAlly = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}Taunted")
            .SetGuiPresentation($"Condition{Name}Taunted", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .SetSituationalContext(
                (SituationalContext)ExtraSituationalContext.TargetDoesNotHaveCondition, conditionTauntedSelf)
            .AddToDB();

        var conditionTaunted = ConditionDefinitionBuilder
            .Create($"Condition{Name}Taunted")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConfused)
            .AddFeatures(combatAffinityGrandChallengeAlly)
            .AddToDB();

        // Compelling Strike

        var actionAffinityCompellingStrike = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, $"ActionAffinity{Name}CompellingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.CompellingStrikeToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasShield),
                new AttackBeforeHitConfirmedOnEnemyCompellingStrike(
                    conditionTauntedSelf,
                    conditionTaunted))
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

        var savingThrowAffinityUnyielding = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}Unyielding")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, false, AttributeDefinitions.Wisdom)
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
                    .ExcludeCaster()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 7)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution)
                    .SetParticleEffectParameters(SpellDefinitions.Confusion)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionTaunted, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionTauntedSelf,
                            ConditionForm.ConditionOperation.Add, true, true))
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
            .Create(FullName)
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

    //
    // Vigilance
    //

    internal static void HandleVigilance(GameLocationCharacter __instance)
    {
        if (__instance.RulesetCharacter.GetSubclassLevel(Fighter, FullName) > 0)
        {
            return;
        }

        foreach (var guardian in Gui.Battle.PlayerContenders
                     .Where(x => x.RulesetCharacter.GetSubclassLevel(Fighter, FullName) > 0))
        {
            var rulesetGuardian = guardian.RulesetCharacter;

            if (guardian.CanReact() || rulesetGuardian.HasConditionOfType(ConditionVigilanceName))
            {
                continue;
            }

            guardian.RefundActionUse(ActionDefinitions.ActionType.Reaction);
            rulesetGuardian.InflictCondition(
                ConditionVigilanceName,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
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
        private readonly ConditionDefinition _conditionDefinitionEnemy;
        private readonly ConditionDefinition _conditionDefinitionSelf;

        public AttackBeforeHitConfirmedOnEnemyCompellingStrike(
            ConditionDefinition conditionDefinitionSelf,
            ConditionDefinition conditionDefinitionEnemy)
        {
            _conditionDefinitionSelf = conditionDefinitionSelf;
            _conditionDefinitionEnemy = conditionDefinitionEnemy;
        }

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
                _conditionDefinitionSelf.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionDefinitionSelf.Name,
                0,
                0,
                0);

            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.InflictCondition(
                _conditionDefinitionEnemy.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionDefinitionEnemy.Name,
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
            if (bestCoverType < CoverType.Half)
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
        private readonly FeatureDefinitionPower _powerImperviousProtector;

        public CustomBehaviorImperviousProtector(
            ConditionDefinition conditionImperviousProtector,
            FeatureDefinitionPower powerImperviousProtector)
        {
            _conditionImperviousProtector = conditionImperviousProtector;
            _powerImperviousProtector = powerImperviousProtector;
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

            var rulesetUsablePower = UsablePowersProvider.Get(_powerImperviousProtector, rulesetCharacter);

            if (rulesetCharacter.GetRemainingUsesOfPower(rulesetUsablePower) > 0)
            {
                return;
            }

            rulesetCharacter.LogCharacterUsedPower(_powerImperviousProtector, Line);
            rulesetUsablePower.RepayUse();
        }
    }
}
