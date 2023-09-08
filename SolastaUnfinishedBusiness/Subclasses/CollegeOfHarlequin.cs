using System.Collections;
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
            .Create("ConditionTerrifiedByHarlequinPerformance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightenedFear)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(ConditionDefinitions.ConditionFrightened.Features.ToArray())
            .AddFeatures(ConditionDefinitions.ConditionPatronHiveWeakeningPheromones.Features.ToArray())
            .SetParentCondition(ConditionDefinitions.ConditionFrightened)
            .AddToDB();

        var powerTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TerrificPerformance")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    //actual targeting is happening in sub-feature, this is for proper tooltip
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionTerrified, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var powerTerrificPerformanceImproved = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TerrificPerformanceImproved")
            .SetGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerTerrificPerformance)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    //actual targeting is happening in sub-feature, this is for proper tooltip
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
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
                    .Build())
            .AddToDB();

        powerTerrificPerformance.SetCustomSubFeatures(
            PowerVisibilityModifier.Hidden,
            new TerrificPerformance(powerTerrificPerformance, powerTerrificPerformanceImproved)
        );

        const string PowerCombatInspirationName = $"Power{Name}CombatInspiration";

        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create(PowerCombatInspirationName)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(CombatInspirationCondition))
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
                                    .AddFeatures(
                                        FeatureDefinitionMovementAffinityBuilder
                                            .Create($"MovementAffinity{Name}CombatInspirationMovementEnhancement")
                                            .SetGuiPresentation(Category.Feature)
                                            .SetCustomSubFeatures(new AddConditionAmountToSpeedModifier())
                                            .AddToDB(),
                                        FeatureDefinitionAttackModifierBuilder
                                            .Create($"AttackModifier{Name}CombatInspirationAttackEnhancement")
                                            .SetGuiPresentation(Category.Feature)
                                            .SetAttackRollModifier(method: AttackModifierMethod.SourceConditionAmount)
                                            .AddToDB())
                                    .SetCustomSubFeatures(new ConditionCombatInspired(PowerCombatInspirationName))
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
                            .SetConditionForm(ConditionDefinitionBuilder
                                    .Create($"Condition{Name}RegainBardicInspirationOnKill")
                                    .SetGuiPresentationNoContent(true)
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .SetCustomSubFeatures(new ConditionRegainBardicInspirationDieOnKill())
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

    private sealed class ConditionCombatInspired : ICustomConditionFeature
    {
        private const string Line = "Feedback/&BardicInspirationUsedToBoostCombatAbility";
        private readonly string _feature;

        public ConditionCombatInspired(string feature)
        {
            _feature = feature;
        }

        public void OnApplyCondition(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterHero hero || hero.GetBardicInspirationDieValue() == DieType.D1)
            {
                return;
            }

            var dieType = hero.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.Advantage, out var r1, out var r2);

            var title = GuiPresentationBuilder.CreateTitleKey(_feature, Category.Feature);
            var description = GuiPresentationBuilder.CreateDescriptionKey(_feature, Category.Feature);

            hero.ShowDieRoll(dieType, r1, r2, advantage: AdvantageType.Advantage, title: title);

            hero.LogCharacterActivatesAbility(title, Line, tooltipContent: description,
                indent: true,
                extra: new[]
                {
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                });

            rulesetCondition.amount = dieRoll;
        }

        public void OnRemoveCondition(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class ConditionRegainBardicInspirationDieOnKill : ICustomConditionFeature
    {
        public void OnApplyCondition(RulesetCharacter target, RulesetCondition rulesetCondition)
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

        public void OnRemoveCondition(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class TerrificPerformance : IOnTargetReducedToZeroHp
    {
        private readonly FeatureDefinitionPower _power14;
        private readonly FeatureDefinitionPower _power6;

        public TerrificPerformance(FeatureDefinitionPower power6, FeatureDefinitionPower power14)
        {
            _power6 = power6;
            _power14 = power14;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
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

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var battle = battleService?.Battle;

            if (battle == null)
            {
                yield break;
            }

            var level = attacker.RulesetCharacter.GetClassLevel(BardClass);
            var power = level >= 14 ? _power14 : _power6;

            var usablePower = UsablePowersProvider.Get(power, rulesetAttacker);
            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();

            rulesetAttacker.LogCharacterUsedPower(power);

            foreach (var enemy in battle.AllContenders
                         .Where(unit => attacker.IsOppositeSide(unit.Side))
                         .Where(enemy => battleService.IsWithinXCells(attacker, enemy, 3))
                         .ToList())
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }
}
