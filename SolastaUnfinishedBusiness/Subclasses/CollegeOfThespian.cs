using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfThespian : AbstractSubclass
{
    private const string Name = "CollegeOfThespian";

    public CollegeOfThespian()
    {
        // LEVEL 03

        // Macabre Instruments

        var magicAffinityMacabreInstruments = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}MacabreInstruments")
            .SetGuiPresentation(Category.Feature)
            .SetHandsFullCastingModifiers(true, false, true)
            .AddToDB();

        // Two-Weapon Fighting Style

        var proficiencyCollegeOfHarlequinFightingStyle = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}FightingStyle")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.FightingStyle, "TwoWeapon")
            .AddToDB();

        // Combat Inspiration

        var conditionCombatInspirationCombat = ConditionDefinitionBuilder
            .Create($"Condition{Name}CombatInspirationCombat")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetPossessive()
            .AddFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}CombatInspiration")
                    .SetGuiPresentation($"Condition{Name}CombatInspirationCombat", Category.Condition,
                        Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var conditionCombatInspirationMovement = ConditionDefinitionBuilder
            .Create($"Condition{Name}CombatInspirationMovement")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
            .SetPossessive()
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}CombatInspiration")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(new ModifyMovementSpeedAdditionCombatInspiration())
                    .AddToDB())
            .AddToDB();

        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CombatInspiration")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionCombatInspirationCombat),
                        EffectFormBuilder.ConditionForm(conditionCombatInspirationMovement))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                    .UseQuickAnimations()
                    .Build())
            .AddCustomSubFeatures(
                ValidatorsValidatePowerUse.HasNoneOfConditions(conditionCombatInspirationMovement.Name))
            .AddToDB();

        conditionCombatInspirationMovement.AddCustomSubFeatures(
            new OnConditionAddedOrRemovedCombatInspired(powerCombatInspiration));

        // Terrific Performance

        var powerTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TerrificPerformance")
            .SetUsesFixed(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    //actual targeting is happening in sub-feature, this is for proper tooltip
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Fear)
                    .Build())
            .AddToDB();

        // LEVEL 06

        // Finale

        var powerFinale = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Finale")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeFinale())
            .AddToDB();

        // LEVEL 14

        // Improved Terrific Performance

        var powerImprovedTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedTerrificPerformance")
            .SetUsesFixed(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    //actual targeting is happening in sub-feature, this is for proper tooltip
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(
                            DamageTypePsychic, 2, DieType.D8, overrideWithBardicInspirationDie: true),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Fear)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetOverriddenPower(powerTerrificPerformance)
            .AddToDB();

        powerTerrificPerformance.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByMeTerrificPerformance(powerTerrificPerformance, powerImprovedTerrificPerformance));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfHarlequin, 256))
            .AddFeaturesAtLevel(3,
                magicAffinityMacabreInstruments,
                proficiencyCollegeOfHarlequinFightingStyle,
                powerCombatInspiration,
                powerTerrificPerformance)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                powerFinale)
            .AddFeaturesAtLevel(14,
                powerImprovedTerrificPerformance)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Combat Inspired
    //

    private sealed class OnConditionAddedOrRemovedCombatInspired(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerCombatInspiration) : IOnConditionAddedOrRemoved
    {
        private const string Line = "Feedback/&CollegeOfThespianBardicInspiration";

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var dieType = target.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.None, out var r1, out var r2);
            var title = powerCombatInspiration.GuiPresentation.Title;
            var description = powerCombatInspiration.GuiPresentation.Description;

            rulesetCondition.amount = dieRoll;

            target.ShowDieRoll(dieType, r1, r2, advantage: AdvantageType.None, title: title);
            target.LogCharacterActivatesAbility(
                title, Line, tooltipContent: description, indent: true,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                ]);
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Finale
    //

    private sealed class PowerOrSpellFinishedByMeFinale : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var hero = action.ActingCharacter.RulesetCharacter.GetOriginalHero();

            if (hero == null ||
                hero.usedBardicInspiration == 0)
            {
                yield break;
            }

            hero.usedBardicInspiration -= 1;
        }
    }

    //
    // Terrific Performance
    //

    private sealed class OnReducedToZeroHpByMeTerrificPerformance(
        FeatureDefinitionPower powerTerrificPerformance,
        FeatureDefinitionPower powerImprovedTerrificPerformance) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var targets = Gui.Battle
                .GetContenders(attacker, hasToPerceivePerceiver: true, withinRange: 3);

            if (targets.Count == 0)
            {
                yield break;
            }

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Bard);
            var power = classLevel < 14 ? powerTerrificPerformance : powerImprovedTerrificPerformance;
            var usablePower = PowerProvider.Get(power, rulesetAttacker);

            attacker.MyExecuteAction(ActionDefinitions.Id.PowerNoCost, usablePower, targets);
        }
    }

    private sealed class ModifyMovementSpeedAdditionCombatInspiration : IModifyMovementSpeedAddition
    {
        public int ModifySpeedAddition(RulesetCharacter character, IMovementAffinityProvider provider)
        {
            return character.FindFirstConditionHoldingFeature(provider as FeatureDefinition)?.Amount ?? 0;
        }
    }
}
