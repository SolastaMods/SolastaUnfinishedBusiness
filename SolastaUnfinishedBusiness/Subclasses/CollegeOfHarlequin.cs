using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
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
        .Create("PowerTerrificPerformance")
        .SetGuiPresentation("TerrificPerformance", Category.Feature)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 1)
            .SetParticleEffectParameters(SpellDefinitions.Fear.effectDescription.effectParticleParameters)
            .SetEffectForms(
                new EffectForm()
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm()
                    {
                        ConditionDefinition = ConditionDefinitions.ConditionFrightened,
                        operation = ConditionForm.ConditionOperation.Add
                    }
                },
                new EffectForm()
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm()
                    {
                        ConditionDefinition =
                            ConditionDefinitions.ConditionPatronHiveWeakeningPheromones,
                        operation = ConditionForm.ConditionOperation.Add
                    }
                }
            )
            .Build()
        )
        .AddToDB();
    
    internal CollegeOfHarlequin()
    {
        var powerCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerCombatInspiration")
            .SetGuiPresentation("CombatInspiration", Category.Feature,
                SpellDefinitions.MagicWeapon.GuiPresentation.spriteReference)
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
                        .Create("ConditionFightingAbilityEnhanced")
                        .SetGuiPresentation("ConditionFightingAbilityEnhanced", Category.Condition,
                            ConditionDefinitions.ConditionHeraldOfBattle.GuiPresentation.SpriteReference)
                        .AddFeatures(FeatureDefinitionAttributeModifierBuilder
                                .Create("CombatInspirationArmorClassEnhancement")
                                .SetGuiPresentation("CombatInspirationArmorClassEnhancement", Category.Feature)
                                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                    AttributeDefinitions.ArmorClass)
                                .AddToDB(),
                            FeatureDefinitionMovementAffinityBuilder
                                .Create("CombatInspirationMovementEnhancement")
                                .SetGuiPresentation("CombatInspirationMovementEnhancement", Category.Feature)
                                .SetCustomSubFeatures(new UseBardicDieRollForSpeedModifier())
                                .AddToDB(),
                            FeatureDefinitionAttackModifierBuilder
                                .Create("CombatInspirationAttackEnhancement")
                                .SetGuiPresentation("CombatInspirationAttackEnhancement", Category.Feature)
                                .SetCustomSubFeatures(new AddBardicDieRollToAttackAndDamage())
                                .AddToDB()
                        )
                        .SetCustomSubFeatures(new ConditionCombatInspired())
                        .SetAmountOrigin(ExtraOriginOfAmount.SourceRollBardicDie)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build()
            )
            .AddToDB();

        var featureTerrificPerformance = FeatureDefinitionBuilder
            .Create("TargetReducedToZeroHpPowerTerrificPerformance")
            .SetGuiPresentation("TerrificPerformance", Category.Feature)
            .SetCustomSubFeatures(new TargetReducedToZeroHpTerrificPerformance())
            .AddToDB();

        var powerImprovedCombatInspiration = FeatureDefinitionPowerBuilder
            .Create("PowerImprovedCombatInspiration")
            .SetGuiPresentation("ImproveCombatInspiration", Category.Feature)
            //.SetShowCasting(false)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionRegainBardicInspirationOnKill")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetCustomSubFeatures(new ConditionRegainBardicInspirationDieOnKill())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBardGiveBardicInspiration.EffectDescription
                    .effectParticleParameters)
                .Build()
            )
            .SetUsesFixed(ActivationTime.OnReduceCreatureToZeroHPAuto)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfHarlequin")
            .SetOrUpdateGuiPresentation("CollegeOfHarlequin", Category.Subclass,
                CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3, powerCombatInspiration, featureTerrificPerformance,
                FeatureDefinitionProficiencys.ProficiencyFighterWeapon,
                FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic)
            .AddFeaturesAtLevel(6,
                FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack,
                powerImprovedCombatInspiration)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    private sealed class ConditionCombatInspired : ICustomConditionFeature
    {
        private const string CombatInspiredEnhancedArmorClass = "CombatInspiredEnhancedArmorClass";

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            Main.Log("DEBUG: CombatInspirationEffectForm Apply Form", true);

            if (target is not RulesetCharacterHero hero ||
                hero.GetBardicInspirationDieValue() == DieType.D1)
            {
                Main.Log("DEBUG: CombatInspirationEffectForm Apply Form Return", true);
                return;
            }

            var dieType = hero.GetBardicInspirationDieValue();
            var dieRoll = RollDie(dieType, AdvantageType.None, out int _, out int _);
            dieRoll = 30;
            
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
            else
            {
                Main.Log("Skip adding amount", true);
            }
            /*var armorClassAttribute = target.attributes[AttributeDefinitions.ArmorClass];
            RulesetAttributeModifier rulesetAttributeModifier =
                RulesetAttributeModifier.BuildAttributeModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, dieRoll,
                    CombatInspiredEnhancedArmorClass);

            armorClassAttribute.AddModifier(rulesetAttributeModifier);
            TrendInfo item = new TrendInfo(dieRoll, FeatureSourceType.Condition, rulesetCondition.Name, null, rulesetAttributeModifier);
            item.additionalDetails = "Combat Inspiration";
            item.additive = true;
            armorClassAttribute.ValueTrends.Add(item);*/
            
            // this is set for movement modifier
            Global.SetBardicRoll(target.guid, dieRoll);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            Global.RemoveBardicRoll(target.guid);
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
        public IEnumerator HandleCharacterReducedToZeroHp(GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            Main.Log("DEBUG HandleCharacterReduceToZero", true);
            if (Global.CurrentAction is not CharacterActionAttack)
            {
                Main.Log("DEBUG HandleCharacterReduceToZero 1", true);
                yield break;
            }

            var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

            if (battle == null)
            {
                Main.Log("DEBUG HandleCharacterReduceToZero 2", true);
                yield break;
            }

            var proficiencyBonus = attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var charisma = attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.Charisma).CurrentValue;
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = new RulesetUsablePower(PowerTerrificPerformance, null, null)
            {
                SaveDC = 8 + proficiencyBonus + charisma
            };

            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);
            
            foreach (var enemy in battle.EnemyContenders
                         .Where(enemy => attacker.RulesetActor.DistanceTo(enemy.RulesetActor) <= 12))
            {
                if (enemy == downedCreature)
                {
                    Main.Log("Skip down enemy", true);
                    continue;
                }
                Main.Log("DEBUG HandleCharacterReduceToZero 3", true);
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

            var dieRoll = Global.GetBardicRoll(character.guid);

            attackMode.ToHitBonus += dieRoll;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(dieRoll,
                FeatureSourceType.Condition, "Combat Inspiration", null));

            damage.BonusDamage += dieRoll;
            damage.DamageBonusTrends.Add(new TrendInfo(dieRoll,
                FeatureSourceType.Condition, "Combat Inspiration", null));
        }
    }
}
