using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
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
        //
        // LEVEL 03
        //

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
                    .AddCustomSubFeatures(new AddConditionAmountToSpeedModifier())
                    .AddToDB())
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedCombatInspired())
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

        //
        // LEVEL 06
        //

        // Finale

        var conditionFinale = ConditionDefinitionBuilder
            .Create($"Condition{Name}Finale")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedFinale())
            .AddToDB();

        var powerFinale = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Finale")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionFinale))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                    .Build())
            .AddToDB();

        //
        // Level 14
        //

        // Improved Terrific Performance

        var powerImprovedTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedTerrificPerformance")
            .SetUsesFixed(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerTerrificPerformance)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    //actual targeting is happening in sub-feature, this is for proper tooltip
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.Create()
                            .SetDamageForm(DamageTypePsychic, 2, DieType.D8, overrideWithBardicInspirationDie: true)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Fear)
                    .Build())
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        powerTerrificPerformance.AddCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
            new OnReducedToZeroHpByMeTerrificPerformance(powerTerrificPerformance, powerTerrificPerformance));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfHarlequin, 256))
            .AddFeaturesAtLevel(3,
                magicAffinityMacabreInstruments,
                proficiencyCollegeOfHarlequinFightingStyle,
                powerCombatInspiration,
                powerTerrificPerformance)
            .AddFeaturesAtLevel(6, AttributeModifierCasterFightingExtraAttack, powerFinale)
            .AddFeaturesAtLevel(14, powerImprovedTerrificPerformance)
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

    private sealed class OnConditionAddedOrRemovedCombatInspired : IOnConditionAddedOrRemoved
    {
        private const string Line = "Feedback/&CollegeOfThespianBardicInspiration";
        private const string Feature = $"Power{Name}CombatInspiration";

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var dieType = target.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.None, out var r1, out var r2);
            var title = GuiPresentationBuilder.CreateTitleKey(Feature, Category.Feature);
            var description = GuiPresentationBuilder.CreateDescriptionKey(Feature, Category.Feature);

            target.ShowDieRoll(dieType, r1, r2, advantage: AdvantageType.None, title: title);
            target.LogCharacterActivatesAbility(title, Line, tooltipContent: description,
                indent: true,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                ]);

            rulesetCondition.amount = dieRoll;
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Finale
    //

    private sealed class OnConditionAddedOrRemovedFinale : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterHero hero)
            {
                return;
            }

            if (hero.usedBardicInspiration > 0)
            {
                hero.usedBardicInspiration -= 1;
            }
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Terrific Performance
    //

    private sealed class OnReducedToZeroHpByMeTerrificPerformance(
        FeatureDefinitionPower powerTerrificPerformance,
        FeatureDefinitionPower powerImprovedTerrificPerformance)
        : IOnReducedToZeroHpByMe
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
                .GetContenders(attacker, isWithinXCells: 3)
                .Where(x => x.CanPerceiveTarget(attacker))
                .ToList();

            if (targets.Empty())
            {
                yield break;
            }

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Bard);
            var power = classLevel < 14 ? powerTerrificPerformance : powerImprovedTerrificPerformance;
            var usablePower = UsablePowersProvider.Get(power, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    //CHECK: no need for AddAsActivePowerToSource
                    .InstantiateEffectPower(rulesetAttacker, usablePower, false),
                targetCharacters = targets
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<ICommandService>()?.ExecuteAction(actionParams, null, true);
        }
    }
}
