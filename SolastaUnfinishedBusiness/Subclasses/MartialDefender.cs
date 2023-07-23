using System.Collections;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ArmorTypeDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialDefender : AbstractSubclass
{
    private const string Name = "MartialDefender";

    internal MartialDefender()
    {
        // LEVEL 03

        // Protection Fighting Training

        var proficiencyProtection = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Protection")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.FightingStyle, "Protection")
            .AddToDB();

        // Shield Expert Training

        var proficiencyShieldExpert = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}ShieldExpert")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.FightingStyle, "ShieldExpert")
            .AddToDB();

        // Shield Mastery

        var attributeModifierShieldMastery = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ShieldMastery")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .SetSituationalContext(ExtraSituationalContext.HasShieldInHands)
            .AddToDB();

        // LEVEL 07

        // Aegis Finesse

        var actionAffinityAegisFinesse = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}AegisFinesse")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .SetCustomSubFeatures(new ValidatorsDefinitionApplication(ValidatorsCharacter.HasShield))
            .AddToDB();

        // Shout of Provocation

        var conditionShoutOfProvocationSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}ShoutOfProvocationSelf")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var combatAffinityShoutOfProvocationAlly = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ShoutOfProvocationAlly")
            .SetGuiPresentation($"Condition{Name}ShoutOfProvocation", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .SetSituationalContext(
                (SituationalContext)ExtraSituationalContext.TargetDoesNotHaveCondition,
                conditionShoutOfProvocationSelf)
            .AddToDB();

        var combatAffinityShoutOfProvocationSelf = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ShoutOfProvocationSelf")
            .SetGuiPresentation($"Condition{Name}ShoutOfProvocation", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetHasCondition, conditionShoutOfProvocationSelf)
            .AddToDB();

        var conditionShoutOfProvocationEnemy = ConditionDefinitionBuilder
            .Create($"Condition{Name}ShoutOfProvocation")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConfused)
            .SetPossessive()
            .AddFeatures(combatAffinityShoutOfProvocationAlly, combatAffinityShoutOfProvocationSelf)
            .AddToDB();

        var powerShoutOfProvocation = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShoutOfProvocation")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerShoutOfProvocation", Resources.PowerShoutOfProvocation, 128))
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .ExcludeCaster()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 7)
                    .SetSavingThrowData(false, AttributeDefinitions.Charisma, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution,
                        8)
                    .SetParticleEffectParameters(SpellDefinitions.Confusion)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionShoutOfProvocationEnemy, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionShoutOfProvocationSelf,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .Build())
            .AddToDB();

        // LEVEL 10

        // Aegis Assault

        var powerShieldAttackExpert = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AegisAssault")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 0, TargetType.Individuals)
                    .SetEffectForms(EffectFormBuilder.MotionForm(MotionForm.MotionType.FallProne))
                    .Build())
            .AddToDB();

        powerShieldAttackExpert.SetCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
            new PhysicalAttackFinishedByMeAegisAssault(powerShieldAttackExpert));

        // LEVEL 15

        // Reactive Aegis

        var featureReactiveAegis = FeatureDefinitionBuilder
            .Create($"Feature{Name}ReactiveAegis")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ActionFinishedByMeReactiveAegis())
            .AddToDB();

        // LEVEL 18

        // Aegis Paragon

        var featureAegisParagon = FeatureDefinitionBuilder
            .Create($"Feature{Name}AegisParagon")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new AddBonusShieldAttack(ValidatorsCharacter.HasUsedWeaponType(ShieldType)))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Martial{Name}")
            .SetGuiPresentation(Category.Subclass, FightingStyleDefinitions.Protection)
            .AddFeaturesAtLevel(3,
                attributeModifierShieldMastery,
                proficiencyProtection,
                proficiencyShieldExpert)
            .AddFeaturesAtLevel(7,
                actionAffinityAegisFinesse,
                powerShoutOfProvocation)
            .AddFeaturesAtLevel(10,
                attributeModifierShieldMastery,
                powerShieldAttackExpert)
            .AddFeaturesAtLevel(15,
                featureReactiveAegis)
            .AddFeaturesAtLevel(18,
                attributeModifierShieldMastery,
                featureAegisParagon)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Aegis Assault
    //

    private sealed class PhysicalAttackFinishedByMeAegisAssault : IPhysicalAttackFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerAegisAssault;

        public PhysicalAttackFinishedByMeAegisAssault(FeatureDefinitionPower powerAegisAssault)
        {
            _powerAegisAssault = powerAegisAssault;
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome != RollOutcome.Success && attackRollOutcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsShield(attackerAttackMode?.SourceDefinition as ItemDefinition))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(_powerAegisAssault, rulesetAttacker);

            ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource()
                .ApplyEffectOnCharacter(rulesetDefender, true, defender.LocationPosition);
        }
    }

    //
    // Reactive Aegis
    //

    private sealed class ActionFinishedByMeReactiveAegis : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction.ActionType != ActionDefinitions.ActionType.Reaction)
            {
                yield break;
            }

            var attackMode = characterAction.actionParams.attackMode;

            if (!ValidatorsWeapon.IsShield(attackMode.SourceDefinition as ItemDefinition))
            {
            }
        }
    }
}
