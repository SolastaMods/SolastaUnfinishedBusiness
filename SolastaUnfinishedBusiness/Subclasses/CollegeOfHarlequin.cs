using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
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
    private static readonly FeatureDefinitionPower PowerTerrificPerformance = FeatureDefinitionPowerBuilder
        .Create("PowerCollegeOfHarlequinTerrificPerformance")
        .SetGuiPresentation(Category.Feature)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 1)
            .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
            .SetEffectForms(
                new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        ConditionDefinition = ConditionDefinitions.ConditionFrightened,
                        operation = ConditionForm.ConditionOperation.Add
                    }
                },
                new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        ConditionDefinition =
                            ConditionDefinitions.ConditionPatronHiveWeakeningPheromones,
                        operation = ConditionForm.ConditionOperation.Add
                    }
                })
            .Build())
        .AddToDB();

    internal CollegeOfHarlequin()
    {
        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfHarlequinCombatInspiration")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            //TODO: hide outside of combat or when already affected by Combat Inspiration
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionCollegeOfHarlequinFightingAbilityEnhanced")
                            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
                            .AddFeatures(
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

        var featureTerrificPerformance = FeatureDefinitionBuilder
            .Create("TargetReducedToZeroHpPowerCollegeOfHarlequinTerrificPerformance")
            .SetGuiPresentation("PowerCollegeOfHarlequinTerrificPerformance", Category.Feature)
            .SetCustomSubFeatures(new TargetReducedToZeroHpTerrificPerformance())
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
                .SetParticleEffectParameters(
                    FeatureDefinitionPowers.PowerBardGiveBardicInspiration.EffectDescription.effectParticleParameters)
                .Build())
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfHarlequin")
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3,
                powerCombatInspiration,
                featureTerrificPerformance,
                proficiencyCollegeOfHarlequinMartialWeapon,
                CommonBuilders.MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6,
                CommonBuilders.AttributeModifierCasterFightingExtraAttack,
                powerImprovedCombatInspiration)
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
                console.consoleTableDefinition) {indent = true};

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

    private sealed class TargetReducedToZeroHpTerrificPerformance : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            if (attackMode == null || activeEffect != null)
            {
                yield break;
            }

            var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

            if (battle == null)
            {
                yield break;
            }

            var proficiencyBonus =
                attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var charisma = attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.Charisma).CurrentValue;
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = new RulesetUsablePower(PowerTerrificPerformance, null, null)
            {
                SaveDC = 8 + proficiencyBonus + charisma
            };

            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);

            foreach (var enemy in battle.EnemyContenders
                         .Where(enemy => attacker.RulesetActor.DistanceTo(enemy.RulesetActor) <= 3))
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }
}
