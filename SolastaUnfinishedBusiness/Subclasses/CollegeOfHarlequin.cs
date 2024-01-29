using System.Collections;
using System.Linq;
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
public sealed class CollegeOfHarlequin : AbstractSubclass
{
    private const string Name = "CollegeOfHarlequin";
    private const string CombatInspirationCondition = "ConditionCollegeOfHarlequinFightingAbilityEnhanced";

    public CollegeOfHarlequin()
    {
        var conditionTerrified = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFrightened, "ConditionTerrifiedByHarlequinPerformance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightenedFear)
            .AddFeatures(ConditionDefinitions.ConditionPatronHiveWeakeningPheromones.Features.ToArray())
            .SetParentCondition(ConditionDefinitions.ConditionFrightened)
            .AddToDB();

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
                            .SetConditionForm(conditionTerrified, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Fear)
                    .Build())
            .AddToDB();

        var powerTerrificPerformanceImproved = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TerrificPerformanceImproved")
            .SetUsesFixed(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerTerrificPerformance)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    //actual targeting is happening in sub-feature, this is for proper tooltip
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.Create()
                            .SetDamageForm(DamageTypePsychic, 2, DieType.D8, 0, HealFromInflictedDamage.Never, true)
                            .Build(),
                        EffectFormBuilder.Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionTerrified, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Fear)
                    .Build())
            .AddToDB();

        powerTerrificPerformance.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByMeTerrificPerformance(powerTerrificPerformance, powerTerrificPerformanceImproved)
        );

        const string PowerCombatInspirationName = $"Power{Name}CombatInspiration";

        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create(PowerCombatInspirationName)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(CombatInspirationCondition))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                    .UseQuickAnimations()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(CombatInspirationCondition)
                                    .SetGuiPresentation(Category.Condition,
                                        ConditionDefinitions.ConditionHeraldOfBattle)
                                    .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
                                    .AddFeatures(
                                        FeatureDefinitionMovementAffinityBuilder
                                            .Create($"MovementAffinity{Name}CombatInspirationMovementEnhancement")
                                            .SetGuiPresentation(Category.Feature)
                                            .AddCustomSubFeatures(new ModifyMovementSpeedAdditionCombatInspiration())
                                            .AddToDB(),
                                        FeatureDefinitionAttackModifierBuilder
                                            .Create($"AttackModifier{Name}CombatInspirationAttackEnhancement")
                                            .SetGuiPresentation(Category.Feature)
                                            .SetAttackRollModifier(method: AttackModifierMethod.SourceConditionAmount)
                                            .AddToDB())
                                    .AddCustomSubFeatures(new ConditionCombatInspired(PowerCombatInspirationName))
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var proficiencyCollegeOfHarlequinMartialWeapon = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}MartialWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var proficiencyCollegeOfHarlequinFightingStyle = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}FightingStyle")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.FightingStyle, "TwoWeapon")
            .AddToDB();

        var powerImprovedCombatInspiration = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedCombatInspiration")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{Name}RegainBardicInspirationOnKill")
                                    .SetGuiPresentationNoContent(true)
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .AddCustomSubFeatures(new ConditionRegainBardicInspirationDieOnKill())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfHarlequin, 256))
            .AddFeaturesAtLevel(3,
                MagicAffinityCasterFightingCombatMagic,
                powerCombatInspiration,
                powerTerrificPerformance,
                proficiencyCollegeOfHarlequinMartialWeapon,
                proficiencyCollegeOfHarlequinFightingStyle)
            .AddFeaturesAtLevel(6, PowerCasterFightingWarMagic, powerImprovedCombatInspiration)
            .AddFeaturesAtLevel(14, powerTerrificPerformanceImproved)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ConditionCombatInspired(string feature) : IOnConditionAddedOrRemoved
    {
        private const string Line = "Feedback/&BardicInspirationUsedToBoostCombatAbility";

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterHero hero || hero.GetBardicInspirationDieValue() == DieType.D1)
            {
                return;
            }

            var dieType = hero.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.Advantage, out var r1, out var r2);

            var title = GuiPresentationBuilder.CreateTitleKey(feature, Category.Feature);
            var description = GuiPresentationBuilder.CreateDescriptionKey(feature, Category.Feature);

            hero.ShowDieRoll(dieType, r1, r2, advantage: AdvantageType.Advantage, title: title);

            hero.LogCharacterActivatesAbility(title, Line, tooltipContent: description,
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

    private sealed class ConditionRegainBardicInspirationDieOnKill : IOnConditionAddedOrRemoved
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

    private sealed class OnReducedToZeroHpByMeTerrificPerformance(
        FeatureDefinitionPower power6,
        FeatureDefinitionPower power14)
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

            // activeEffect != null means a magical attack
            if (attackMode == null || activeEffect != null)
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

            if (targets.Count == 0)
            {
                yield break;
            }

            var level = attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Bard);
            var power = level >= 14 ? power14 : power6;

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(power, rulesetAttacker);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
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
