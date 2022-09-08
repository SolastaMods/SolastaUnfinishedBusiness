/*
 
Feature written by ElAntonius for the Ranger
- requires ModHelpers extensions
- left code here to check how difficult to rewrite without this dependency

*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using SolastaModApi;
//using SolastaCommunityExpansion.Api.Extensions;
//using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;
//using static SolastaModApi.DatabaseHelper.ConditionDefinitions;

//using SolastaModHelpers;
//using SolastaModHelpers.Helpers;
//using SolastaModHelpers.NewFeatureDefinitions;

//namespace SolastaCommunityExpansion.Models.Features
//{
//    internal class FeatureSetRangerFoeSlayerBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
//    {
//        const string FeatureSetRangerFoeSlayerName = "ZSFeatureSetRangerFoeSlayer";
//        const string FeatureSetRangerFoeSlayerGuid = "11001D8E-12FE-436E-AF95-D9644B7EEF1D";

//        protected FeatureSetRangerFoeSlayerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetShadowTamerDarkSlayer, name, guid)
//        {
//            var foe_slayer_avail_condition = buildConditionFoeSlayerAvailable();
//            var foe_slayer_used_condition = buildConditionFoeSlayerUsed();
//            var foe_slayer_avail = buildFeatureFoeSlayerAvailable(foe_slayer_avail_condition);
//            var foe_slayer_damage = buildPowerFoeSlayerDamage(foe_slayer_avail_condition, foe_slayer_used_condition);
//            var foe_slayer_attack = buildPowerFoeSlayerAttack(foe_slayer_avail_condition, foe_slayer_used_condition);
//            var foe_slayer_used = buildFeatureFoeSlayerUsed
//            (
//                foe_slayer_used_condition,
//                new List<FeatureDefinitionPower>()
//                {
//                    foe_slayer_damage,
//                    foe_slayer_attack
//                }
//            );

//            Definition.GuiPresentation.Title = "Feature/&RangerFeatureSetFoeSlayerTitle";
//            Definition.GuiPresentation.Description = "Feature/&RangerFeatureSetFoeSlayerDescription";

//            Definition.FeatureSet.Clear();
//            Definition.FeatureSet.Add(foe_slayer_avail);
//            Definition.FeatureSet.Add(foe_slayer_damage);
//            Definition.FeatureSet.Add(foe_slayer_attack);
//            Definition.FeatureSet.Add(foe_slayer_used);

//            Definition.enumerateInDescription = false;
//            Definition.mode = FeatureDefinitionFeatureSet.FeatureSetMode.Union;
//        }

//        private static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
//            => new FeatureSetRangerFoeSlayerBuilder(name, guid).AddToDB();

//        public static readonly FeatureDefinitionFeatureSet FeatureSetRangerFoeSlayer
//            = CreateAndAddToDB(FeatureSetRangerFoeSlayerName, FeatureSetRangerFoeSlayerGuid);

//        private static ConditionDefinition buildConditionFoeSlayerAvailable()
//        {
//            var condition = ConditionBuilder.createConditionWithInterruptions
//            (
//                "ZSConditionRangerFoeSlayerAvailable",
//                "B9B7D2DB-365C-4FA9-A3FB-0E901B4783F7",
//                Common.common_no_title,
//                Common.common_no_title,
//                null,
//                ConditionMagicalWeapon,
//                new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks }
//            );

//            condition.SetSilentWhenAdded(true);
//            condition.SetSilentWhenRemoved(true);

//            condition.GuiPresentation.SetHidden(true);

//            return condition;
//        }

//        private static ConditionDefinition buildConditionFoeSlayerUsed()
//        {
//            var condition = ConditionBuilder.createCondition
//            (
//                "ZSConditionRangerFoeSlayerUsed",
//                "4F748DD5-14CA-4986-A871-8D459357F1C8",
//                Common.common_no_title,
//                Common.common_no_title,
//                null,
//                ConditionMagicalWeapon
//            );

//            condition.SetSilentWhenAdded(true);
//            condition.SetSilentWhenRemoved(true);

//            condition.GuiPresentation.SetHidden(true);

//            return condition;
//        }

//        private static FeatureDefinitionFoeSlayerAttackingFavored buildFeatureFoeSlayerAvailable(ConditionDefinition condition)
//        {
//            return FeatureBuilder<FeatureDefinitionFoeSlayerAttackingFavored>.createFeature
//            (
//                "ZSFeatureRangerFoeSlayerAvailable",
//                "AE51E473-0711-46C7-B6D6-F671A17243F0",
//                Common.common_no_title,
//                Common.common_no_title,
//                Common.common_no_icon,
//                a =>
//                {
//                    a.condition = condition;
//                    a.durationType = RuleDefinitions.DurationType.Turn;
//                    a.durationValue = 1;
//                    a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
//                }
//            );
//        }

//        private static ApplyConditionOnPowerUseToSelf buildFeatureFoeSlayerUsed(ConditionDefinition condition, List<FeatureDefinitionPower> foe_slayer_powers)
//        {
//            return FeatureBuilder<ApplyConditionOnPowerUseToSelf>.createFeature
//            (
//                "ZSFeatureRangerFoeSlayerUsed",
//                "A00B2D1D-A8E2-4A04-B104-BA2BC682E947",
//                Common.common_no_title,
//                Common.common_no_title,
//                Common.common_no_icon,
//                a =>
//                {
//                    a.condition = condition;
//                    a.durationType = RuleDefinitions.DurationType.Turn;
//                    a.durationValue = 1;
//                    a.powers = foe_slayer_powers;
//                }
//            );
//        }

//        private static PowerWithRestrictions buildPowerFoeSlayerDamage(ConditionDefinition avail_condition, ConditionDefinition restrict_condition)
//        {
//            // This looks stupendously weird, but if I set Bonus Damage to 0 and use 0 dice, the game
//            // hangs for 10-15 seconds on foe slayer damage. So the compensate, the Foe Slayer
//            // Damage calculation is 1d1 + Wisdom Mod - 1, which lets the game "roll" a dice and
//            // prevents a hang. If a future update of the game "fixes" this, then it can easily
//            // be set to BonusDamage = 0 and DiceNumber = 0.
//            var damage_form = new DamageForm();
//            damage_form.SetBonusDamage(-1);
//            damage_form.SetDamageType("DamagePiercing");
//            damage_form.SetDiceNumber(1);
//            damage_form.SetDieType(RuleDefinitions.DieType.D1);
//            damage_form.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);

//            var effect_form = new EffectForm();
//            effect_form.SetFormType(EffectForm.EffectFormType.Damage);
//            effect_form.SetDamageForm(damage_form);
//            effect_form.SetApplyAbilityBonus(true);

//            var effect_description = new EffectDescription();
//            effect_description.SetCanBePlacedOnCharacter(true);
//            effect_description.SetCreatedByCharacter(true);
//            effect_description.SetRangeType(RuleDefinitions.RangeType.RangeHit);
//            effect_description.SetTargetSide(RuleDefinitions.Side.Enemy);
//            effect_description.SetTargetType(RuleDefinitions.TargetType.Individuals);
//            effect_description.SetDurationParameter(1);
//            effect_description.SetDurationType(RuleDefinitions.DurationType.Turn);
//            effect_description.EffectForms.Clear();
//            effect_description.EffectForms.Add(effect_form);

//            var power = GenericPowerBuilder<PowerWithRestrictions>.createPower
//            (
//                "ZSPowerRangerFoeSlayerDamage",
//                "172AA353-C0A1-4648-B9F3-5ACA0B814D5B",
//                "Feature/&RangerFeatureSetFoeSlayerTitle",
//                Common.common_no_title,
//                PowerDomainBattleDecisiveStrike.GuiPresentation.SpriteReference,
//                effect_description,
//                RuleDefinitions.ActivationTime.OnAttackHit,
//                2,
//                RuleDefinitions.UsesDetermination.Fixed,
//                RuleDefinitions.RechargeRate.AtWill,
//                Stats.Wisdom,
//                Stats.Wisdom,
//                1,
//                false
//            );

//            power.SetShortTitleOverride("Foe Slayer");

//            power.restrictions = new List<IRestriction>()
//            {
//                new HasConditionRestriction(avail_condition),
//                new NoConditionRestriction(restrict_condition)
//            };

//            power.checkReaction = true;

//            return power;
//        }

//        private static PowerWithRestrictions buildPowerFoeSlayerAttack(ConditionDefinition avail_condition, ConditionDefinition restrict_condition)
//        {
//            var attack_wis = FeatureBuilder<FeatureDefinitionFoeSlayerAddWisdomAttack>.createFeature
//            (
//                "ZSAttackModifierRangerFoeSlayerAttack",
//                "1074AA14-1BDF-444F-B1BC-BF168F4133E2",
//                Common.common_no_title,
//                Common.common_no_title,
//                null
//            );

//            var condition = ConditionBuilder.createConditionWithInterruptions
//            (
//                "ZSConditionRangerFoeSlayerAttack",
//                "B6A524C2-01C0-4B83-9E1F-8B02FE13A990",
//                "Feature/&RangerFeatureSetFoeSlayerTitle",
//                Common.common_no_title,
//                null,
//                ConditionMagicalWeapon,
//                new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
//                attack_wis
//            );

//            condition.SetSilentWhenAdded(true);
//            condition.SetSilentWhenRemoved(true);
//            condition.GuiPresentation.SetHidden(true);
//            condition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
//            condition.SetDurationParameter(1);
//            condition.SetDurationType(RuleDefinitions.DurationType.Turn);

//            var condition_form = new ConditionForm();
//            condition_form.SetApplyToSelf(true);
//            condition_form.SetOperation(ConditionForm.ConditionOperation.Add);
//            condition_form.SetConditionDefinition(condition);

//            var effect_form = new EffectForm();
//            effect_form.SetFormType(EffectForm.EffectFormType.Condition);
//            effect_form.SetConditionForm(condition_form);
//            effect_form.SetApplyAbilityBonus(true);

//            var effect_description = new EffectDescription();
//            effect_description.SetCanBePlacedOnCharacter(true);
//            effect_description.SetCreatedByCharacter(true);
//            effect_description.SetRangeType(RuleDefinitions.RangeType.Self);
//            effect_description.SetTargetSide(RuleDefinitions.Side.Ally);
//            effect_description.SetTargetType(RuleDefinitions.TargetType.Self);
//            effect_description.SetDurationParameter(1);
//            effect_description.SetDurationType(RuleDefinitions.DurationType.Turn);
//            effect_description.EffectForms.Clear();
//            effect_description.EffectForms.Add(effect_form);

//            var power = GenericPowerBuilder<FeatureDefinitionFoeSlayerPowerOnAttack>.createPower
//            (
//                "ZSPowerRangerFoeSlayerAttack",
//                "493B92B2-FAF3-46A7-84F8-A753D4F2A1AF",
//                Common.common_no_title,
//                Common.common_no_title,
//                PowerDomainBattleDecisiveStrike.GuiPresentation.SpriteReference,
//                effect_description,
//                RuleDefinitions.ActivationTime.Reaction,
//                2,
//                RuleDefinitions.UsesDetermination.Fixed,
//                RuleDefinitions.RechargeRate.AtWill,
//                Stats.Wisdom,
//                Stats.Wisdom,
//                1,
//                false
//            );

//            power.SetShortTitleOverride("Foe Slayer");

//            power.restrictions = new List<IRestriction>()
//            {
//                new HasConditionRestriction(avail_condition),
//                new NoConditionRestriction(restrict_condition)
//            };

//            power.checkReaction = true;

//            return power;
//        }
//    }

//    public class FeatureDefinitionFoeSlayerAttackingFavored : FeatureDefinition, IInitiatorApplyEffectOnAttack
//    {
//        public ConditionDefinition condition;
//        public int durationValue;
//        public RuleDefinitions.DurationType durationType;
//        public RuleDefinitions.TurnOccurenceType turnOccurence;

//        public void processAttackInitiator(GameLocationCharacter attacker, GameLocationCharacter defender, ActionModifier attack_modifier, RulesetAttackMode attack_mode)
//        {
//            var favored_enemies = new List<string>();

//            // Get all favored enemies
//            attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAdditionalDamage>(attacker.RulesetCharacter.FeaturesToBrowse);

//            favored_enemies.Clear();

//            foreach (FeatureDefinition feature in attacker.RulesetCharacter.FeaturesToBrowse)
//            {
//                var damage_feature = feature as FeatureDefinitionAdditionalDamage;

//                if (damage_feature.Name.Contains("AdditionalDamageRangerFavoredEnemy"))
//                {
//                    favored_enemies.Add(damage_feature.RequiredCharacterFamily.Name);
//                }
//            }

//            // Apply a condition if we're attacking a favored enemy
//            if (favored_enemies.Contains(defender.RulesetCharacter.CharacterFamily))
//            {
//                RulesetCondition active_condition = RulesetCondition.CreateActiveCondition(attacker.RulesetCharacter.Guid,
//                                                                                           condition, RuleDefinitions.DurationType.Turn, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn,
//                                                                                           attacker.RulesetCharacter.Guid,
//                                                                                           attacker.RulesetCharacter.CurrentFaction.Name);
//                attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, active_condition, true);
//            }
//        }
//    }

//    public class FeatureDefinitionFoeSlayerPowerOnAttack : PowerWithRestrictions, IReactionPowerOnAttackAttempt
//    {
//        bool IReactionPowerOnAttackAttempt.canBeUsedOnAttackAttempt(GameLocationCharacter caster, GameLocationCharacter attacker, GameLocationCharacter defender, ActionModifier attack_modifier, RulesetAttackMode attack_mode)
//        {
//            var prerolled_data = AttackRollsData.getPrerolledData(attacker);

//            if (caster != attacker)
//            {
//                return false;
//            }

//            if ( prerolled_data.outcome != RuleDefinitions.RollOutcome.Failure )
//            {
//                return false;
//            }

//            var effect = this.EffectDescription;
//            if (effect == null)
//            {
//                return false;
//            }

//            if (attack_mode == null)
//            {
//                return false;
//            }

//            return true;
//        }
//    }

//    public class FeatureDefinitionFoeSlayerAddWisdomAttack : FeatureDefinitionAffinity, ICombatAffinityProvider,  IAffinityProvider, IInitiativeModifier
//    {
//        private bool autoCritical = false;
//        private bool criticalHitImmunity = false;
//        private bool ignoreCover = false;
//        private RuleDefinitions.SituationalContext situationalContext = RuleDefinitions.SituationalContext.None;
//        private ConditionDefinition requiredTargetCondition = null;

//        public bool AutoCritical => this.autoCritical;
//        public bool CriticalHitImmunity => this.criticalHitImmunity;
//        public bool IgnoreCover => this.ignoreCover;
//        public RuleDefinitions.SituationalContext SituationalContext => this.situationalContext;
//        public ConditionDefinition RequiredTargetCondition => this.requiredTargetCondition;

//        public void RefreshInitiativeBonus(ref int advantageValue)
//        {
//            return;
//        }

//        public bool IsImmuneToOpportunityAttack(RulesetCharacter myself, RulesetCharacter attacker) => false;

//        public RuleDefinitions.AdvantageType GetAdvantageOnOpportunityAttackOnMe(
//          RulesetCharacter myself,
//          RulesetCharacter attacker)
//        {
//            return RuleDefinitions.AdvantageType.None;
//        }

//        public void ComputeAttackModifier(RulesetCharacter myself, RulesetCharacter defender, RulesetAttackMode attackMode, ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin)
//        {
//            if (myself == null || defender == null || attackModifier == null)
//            {
//                Trace.LogWarning("Invalid parameters in ComputeAttackModifier");
//            }
//            else
//            {
//                var num = AttributeDefinitions.ComputeAbilityScoreModifier(myself.GetAttribute(Stats.Wisdom).CurrentValue);
//                attackModifier.AttackRollModifier += num;
//                attackModifier.AttacktoHitTrends.Add(new RuleDefinitions.TrendInfo(num, featureOrigin.sourceType, featureOrigin.sourceName, (object)null));
//            }
//        }

//        public void ComputeDefenseModifier(RulesetCharacter myself, RulesetCharacter attacker, int sustainedAttacks, bool defenderAlreadyAttackedByAttackerThisTurn, ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin)
//        {
//            return;
//        }
//    }
//}



