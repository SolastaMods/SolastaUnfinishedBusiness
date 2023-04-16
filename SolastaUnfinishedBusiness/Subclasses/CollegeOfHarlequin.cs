using System.Collections;
using System.Linq;
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

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfHarlequin : AbstractSubclass
{
    private const string CombatInspirationCondition = "ConditionCollegeOfHarlequinFightingAbilityEnhanced";

    internal CollegeOfHarlequin()
    {
        var conditionTerrified = ConditionDefinitionBuilder
            .Create("ConditionTerrifiedByHarlequinPerformance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFrightenedFear)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(ConditionDefinitions.ConditionFrightened.Features.ToArray())
            .AddFeatures(ConditionDefinitions.ConditionPatronHiveWeakeningPheromones.Features.ToArray())
            .AddToDB();

        var powerTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfHarlequinTerrificPerformance")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                //actual targeting is happening in sub-feature, this is for proper tooltip
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
                .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
                .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(EffectFormBuilder.Create()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetConditionForm(conditionTerrified, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        var powerTerrificPerformanceImproved = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfHarlequinTerrificPerformanceImproved")
            .SetGuiPresentation(Category.Feature)
            .SetOverriddenPower(powerTerrificPerformance)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
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
            new TerrificPerformance(powerTerrificPerformance, powerTerrificPerformanceImproved),
            PowerVisibilityModifier.Hidden
        );

        const string FeatureName = "PowerCollegeOfHarlequinCombatInspiration";
        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create(FeatureName)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(CombatInspirationCondition))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                .UseQuickAnimations()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create(CombatInspirationCondition)
                            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
                            .AddFeatures(
                                FeatureDefinitionMovementAffinityBuilder
                                    .Create("MovementAffinityCollegeOfHarlequinCombatInspirationMovementEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetCustomSubFeatures(new AddConditionAmountToSpeedModifier())
                                    .AddToDB(),
                                FeatureDefinitionAttackModifierBuilder
                                    .Create("AttackModifierCollegeOfHarlequinCombatInspirationAttackEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetAttackRollModifier(method: AttackModifierMethod.SourceConditionAmount)
                                    .AddToDB())
                            .SetCustomSubFeatures(new ConditionCombatInspired(FeatureName))
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        var proficiencyCollegeOfHarlequinMartialWeapon = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCollegeOfHarlequinMartialWeapon")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var proficiencyCollegeOfHarlequinFightingStyle = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyCollegeOfHarlequinFightingStyle")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.FightingStyle, FightingStyleDefinitions.TwoWeapon.Name)
            .AddToDB();

        var powerImprovedCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfHarlequinImprovedCombatInspiration")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionCollegeOfHarlequinRegainBardicInspirationOnKill")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetCustomSubFeatures(new ConditionRegainBardicInspirationDieOnKill())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                .Build())
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfHarlequin")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("CollegeOfHarlequin", Resources.CollegeOfHarlequin, 256))
            .AddFeaturesAtLevel(3,
                powerCombatInspiration,
                powerTerrificPerformance,
                proficiencyCollegeOfHarlequinMartialWeapon,
                CommonBuilders.MagicAffinityCasterFightingCombatMagic,
                proficiencyCollegeOfHarlequinFightingStyle)
            .AddFeaturesAtLevel(6,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                powerImprovedCombatInspiration)
            .AddFeaturesAtLevel(14, powerTerrificPerformanceImproved)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ConditionCombatInspired : ICustomConditionFeature
    {
        private const string Line = "Feedback/&BardicInspirationUsedToBoostCombatAbility";
        private readonly string feature;

        public ConditionCombatInspired(string feature)
        {
            this.feature = feature;
        }

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
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

            GameConsoleHelper.LogCharacterActivatesAbility(hero, title, Line, tooltipContent: description,
                indent: true,
                extra: new[]
                {
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                });

            rulesetCondition.amount = dieRoll;
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class ConditionRegainBardicInspirationDieOnKill : ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
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

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class TerrificPerformance : ITargetReducedToZeroHp
    {
        private readonly FeatureDefinitionPower power14;
        private readonly FeatureDefinitionPower power6;

        public TerrificPerformance(FeatureDefinitionPower power6, FeatureDefinitionPower power14)
        {
            this.power6 = power6;
            this.power14 = power14;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            // activeEffect != null means a magical attack
            if (attackMode == null || activeEffect != null)
            {
                yield break;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var battle = battleService?.Battle;

            if (battle == null)
            {
                yield break;
            }

            var power = power6;
            var level = attacker.RulesetCharacter.GetClassLevel(BardClass);
            if (level >= 14)
            {
                power = power14;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var effectPower = new RulesetEffectPower(rulesetAttacker, UsablePowersProvider.Get(power, rulesetAttacker));

            GameConsoleHelper.LogCharacterUsedPower(rulesetAttacker, power);

            foreach (var enemy in battle.AllContenders
                         .Where(unit => attacker.IsOppositeSide(unit.Side))
                         .Where(enemy => battleService.IsWithinXCells(attacker, enemy, 3)))
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }
}
