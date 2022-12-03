using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static FeatureDefinitionAttributeModifier.AttributeModifierOperation;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfHarlequin : AbstractSubclass
{
    private const string CombatInspirationCondition = "ConditionCollegeOfHarlequinFightingAbilityEnhanced";

    internal CollegeOfHarlequin()
    {
        var conditionTerrifiedByHarlequinPerformance = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFrightened, "ConditionTerrifiedByHarlequinPerformance")
            .SetGuiPresentation("ConditionTerrifiedByHarlequinPerformance", Category.Condition,
                ConditionDefinitions.ConditionFrightened)
            .AddFeatures(ConditionDefinitions.ConditionPatronHiveWeakeningPheromones.Features.ToArray())
            .AddToDB();

        var powerTerrificPerformance = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfHarlequinTerrificPerformance")
            .SetGuiPresentation(Category.Feature)
            .SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                //actual targeting is happening in sub-feature, this is for proper tooltip
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
                .SetHasSavingThrow(AttributeDefinitions.Wisdom, EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(conditionTerrifiedByHarlequinPerformance,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        powerTerrificPerformance.SetCustomSubFeatures(
            new TerrificPerformance(powerTerrificPerformance),
            PowerVisibilityModifier.Hidden
        );
        
        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfHarlequinCombatInspiration")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoCondition(CombatInspirationCondition))
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
                                FeatureDefinitionBuilder
                                    .Create("AttributeModifierCollegeOfHarlequinCombatInspirationExtraAttack")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetCustomSubFeatures(new CombatInspiredExtraAttack())
                                    .AddToDB(),
                                FeatureDefinitionAttributeModifierBuilder
                                    .Create("AttributeModifierCollegeOfHarlequinCombatInspirationArmorClassEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetModifier(AddConditionAmount, AttributeDefinitions.ArmorClass)
                                    .AddToDB(),
                                FeatureDefinitionMovementAffinityBuilder
                                    .Create("MovementAffinityCollegeOfHarlequinCombatInspirationMovementEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetCustomSubFeatures(new AddConditionAmountToSpeedModifier())
                                    .AddToDB(),
                                FeatureDefinitionAttackModifierBuilder
                                    .Create("AttackModifierCollegeOfHarlequinCombatInspirationAttackEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetAttackRollModifier(method: AttackModifierMethod.SourceConditionAmount)
                                    .SetDamageRollModifier(method: AttackModifierMethod.SourceConditionAmount)
                                    .AddToDB())
                            .SetCustomSubFeatures(new ConditionCombatInspired())
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

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfHarlequin")
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3,
                powerCombatInspiration,
                proficiencyCollegeOfHarlequinMartialWeapon,
                CommonBuilders.MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                powerTerrificPerformance)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    private sealed class ConditionCombatInspired : ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not RulesetCharacterHero hero || hero.GetBardicInspirationDieValue() == DieType.D1)
            {
                return;
            }

            var dieType = hero.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.Advantage, out var _, out var _);

            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry("Feedback/&BardicInspirationUsedToBoostCombatAbility",
                console.consoleTableDefinition) { indent = true };

            console.AddCharacterEntry(target, entry);
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, Gui.FormatDieTitle(dieType));
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString());
            console.AddEntry(entry);

            rulesetCondition.amount = dieRoll;
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    private sealed class TerrificPerformance : ITargetReducedToZeroHp
    {
        private readonly FeatureDefinitionPower power;

        public TerrificPerformance(FeatureDefinitionPower power)
        {
            this.power = power;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
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

    private sealed class CombatInspiredExtraAttack : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (attackMode != null)
            {
                attackMode.AttacksNumber += 1;   
            }
        }
    }
}
