using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
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
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.BardicInspiration)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration.EffectDescription
                    .EffectParticleParameters)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                            .Create("ConditionCollegeOfHarlequinFightingAbilityEnhanced")
                            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
                            .AddFeatures(FeatureDefinitionAttributeModifierBuilder
                                    .Create("AttributeModifierCollegeOfHarlequinCombatInspirationArmorClassEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                        AttributeDefinitions.ArmorClass)
                                    .AddToDB(),
                                FeatureDefinitionMovementAffinityBuilder
                                    .Create("MovementAffinityCollegeOfHarlequinCombatInspirationMovementEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetCustomSubFeatures(new UseBardicDieRollForSpeedModifier())
                                    .AddToDB(),
                                FeatureDefinitionAttackModifierBuilder
                                    .Create("AttackModifierCollegeOfHarlequinCombatInspirationAttackEnhancement")
                                    .SetGuiPresentation(Category.Feature)
                                    .SetCustomSubFeatures(new AddBardicDieRollToAttackAndDamage())
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
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder
                    .Create()
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

    private static Dictionary<ulong, int> BardicDieRollPerCharacter { get; } = new();

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    internal static int GetBardicRoll(ulong sourceGuid)
    {
        BardicDieRollPerCharacter.TryGetValue(sourceGuid, out var roll);

        return roll;
    }

    private static void SetBardicRoll(ulong sourceGuid, int roll)
    {
        BardicDieRollPerCharacter.AddOrReplace(sourceGuid, roll);
    }

    private static void RemoveBardicRoll(ulong sourceGuid)
    {
        BardicDieRollPerCharacter.Remove(sourceGuid);
    }

    private sealed class ConditionCombatInspired : ICustomConditionFeature
    {
        private const string CombatInspiredEnhancedArmorClass = "CombatInspiredEnhancedArmorClass";

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
                console.consoleTableDefinition);

            console.AddCharacterEntry(target, entry);
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, Gui.FormatDieTitle(dieType));
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString());
            console.AddEntry(entry);

            // for armor class
            var feature = rulesetCondition.ConditionDefinition.Features[0] as FeatureDefinitionAttributeModifier;

            if (feature != null)
            {
                feature.modifierValue = dieRoll;
            }

            // this is set for movement modifier
            SetBardicRoll(target.guid, dieRoll);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            RemoveBardicRoll(target.guid);

            var armorClassAttribute = target.attributes[AttributeDefinitions.ArmorClass];

            armorClassAttribute.RemoveModifiersByTags(CombatInspiredEnhancedArmorClass);
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

    internal sealed class UseBardicDieRollForSpeedModifier
    {
    }

    private sealed class TargetReducedToZeroHpTerrificPerformance : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            if (Global.CurrentAction is not CharacterActionAttack)
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

    private sealed class AddBardicDieRollToAttackAndDamage : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var dieRoll = GetBardicRoll(character.guid);

            attackMode.ToHitBonus += dieRoll;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(dieRoll,
                FeatureSourceType.Condition, "Combat Inspiration", null));

            damage.BonusDamage += dieRoll;
            damage.DamageBonusTrends.Add(new TrendInfo(dieRoll,
                FeatureSourceType.Condition, "Combat Inspiration", null));
        }
    }
}
