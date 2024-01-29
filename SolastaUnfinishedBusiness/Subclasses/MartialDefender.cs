using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialDefender : AbstractSubclass
{
    private const string Name = "MartialDefender";

    public MartialDefender()
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
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .AddCustomSubFeatures(new ValidateDefinitionApplication(ValidatorsCharacter.HasShield))
            .AddToDB();

        // Shout of Provocation

        var combatAffinityShoutOfProvocationAlly = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ShoutOfProvocationAlly")
            .SetGuiPresentation($"Condition{Name}ShoutOfProvocation", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .SetSituationalContext(ExtraSituationalContext.TargetIsNotEffectSource)
            .AddToDB();

        var combatAffinityShoutOfProvocationSelf = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ShoutOfProvocationSelf")
            .SetGuiPresentation($"Condition{Name}ShoutOfProvocation", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetIsEffectSource)
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
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 10

        // Aegis Assault

        var powerAegisAssault = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AegisAssault")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.MotionForm(MotionForm.MotionType.FallProne))
                    .Build())
            .AddToDB();

        // LEVEL 15

        // Brutal Aegis

        var featureBrutalAegis = FeatureDefinitionBuilder
            .Create($"Feature{Name}BrutalAegis")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new UpgradeWeaponDice(
                (_, damage) => (damage.diceNumber, DieType.D6, DieType.D6), ValidatorsWeapon.IsShield))
            .AddToDB();

        // LEVEL 18

        // Aegis Paragon

        var additionalActionAegisParagon = FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}AegisParagon")
            .SetGuiPresentation($"Feature{Name}AegisParagon", Category.Feature)
            .SetActionType(ActionDefinitions.ActionType.Bonus)
            .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
            .SetMaxAttacksNumber(-1)
            .AddCustomSubFeatures(ValidateAdditionalActionAttack.Shield)
            .AddToDB();

        var conditionAegisParagon = ConditionDefinitionBuilder
            .Create($"Condition{Name}AegisParagon")
            .SetGuiPresentation($"Feature{Name}AegisParagon", Category.Feature)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddFeatures(additionalActionAegisParagon)
            .AddToDB();

        var featureAegisParagon = FeatureDefinitionBuilder
            .Create($"Feature{Name}AegisParagon")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures()
            .AddToDB();

        // BEHAVIORS

        powerAegisAssault.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new PhysicalAttackFinishedByMeAegisAssault(powerAegisAssault, conditionAegisParagon));

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
                powerAegisAssault)
            .AddFeaturesAtLevel(15,
                featureBrutalAegis)
            .AddFeaturesAtLevel(18,
                attributeModifierShieldMastery,
                featureAegisParagon)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Aegis Assault / Aegis Paragon
    //

    private sealed class PhysicalAttackFinishedByMeAegisAssault(
        FeatureDefinitionPower powerAegisAssault,
        ConditionDefinition conditionAegisParagon)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            //
            // Validators
            //

            if (attackRollOutcome != RollOutcome.Success && attackRollOutcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsShield(attackerAttackMode?.SourceDefinition as ItemDefinition))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            //
            // Aegis Paragon
            //

            var levels = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Fighter);

            if (levels >= 18 && attacker.OncePerTurnIsValid("AegisParagon"))
            {
                attacker.UsedSpecialFeatures.TryAdd("AegisParagon", 1);

                rulesetAttacker.InflictCondition(
                    conditionAegisParagon.Name,
                    conditionAegisParagon.DurationType,
                    conditionAegisParagon.DurationParameter,
                    conditionAegisParagon.turnOccurence,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionAegisParagon.Name,
                    0,
                    0,
                    0);
            }

            //
            // Aegis Assault
            //

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerAegisAssault, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            actionService.ExecuteAction(actionParams, null, true);
        }
    }
}
