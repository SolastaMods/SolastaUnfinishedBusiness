//
// TODO: looking for contributors to finish this class
//

#if false
using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warden.Subclasses;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaModApi.Infrastructure;
using static CharacterClassDefinition;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Classes.Warden
{
    internal static class Warden
    {
        public static readonly Guid WARDEN_BASE_GUID = new("ab6ac3a4-c800-4560-a46b-6fb826ed5773");

        public static readonly CharacterClassDefinition Instance = BuildAndAddClass();
        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; private set; }
        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; private set; }
        public static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; private set; }
        public static FeatureDefinitionPointPool FeatureDefinitionPointPoolSkills { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetSentinelStand { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerWardenGrasp { get; private set; }
        public static FeatureDefinitionFightingStyleChoice FeatureDefinitionFightingStyleChoiceWarden { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerWardenMark { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerWardenResolve { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerFontOfLife { get; private set; }
        public static FeatureDefinitionAttributeModifier FeatureDefinitionAttributeModifierExtraAttack { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetSentinelStep { get; private set; }
        public static FeatureDefinitionDamageAffinity FeatureDefinitionDamageAffinityUndying { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerInterrupt { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetSentinelSoul { get; private set; }

        private static void BuildClassStats(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Fighter);
            classBuilder.SetPictogram(CharacterClassDefinitions.Fighter.ClassPictogramReference);
            classBuilder.SetBattleAI(CharacterClassDefinitions.Fighter.DefaultBattleDecisions);
            classBuilder.SetHitDice(RuleDefinitions.DieType.D10);
            classBuilder.SetIngredientGatheringOdds(CharacterClassDefinitions.Fighter.IngredientGatheringOdds);

            classBuilder.SetAbilityScorePriorities(
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Charisma);

            classBuilder.AddFeatPreference(FeatDefinitions.Enduring_Body);

            classBuilder.AddToolPreferences(ToolTypeDefinitions.ArtisanToolSmithToolsType);

            classBuilder.AddSkillPreferences(
                DatabaseHelper.SkillDefinitions.AnimalHandling,
                DatabaseHelper.SkillDefinitions.Athletics,
                DatabaseHelper.SkillDefinitions.Nature,
                DatabaseHelper.SkillDefinitions.Perception,
                DatabaseHelper.SkillDefinitions.Survival);
        }

        private static void BuildEquipment(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Shield, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Longsword, EquipmentDefinitions.OptionWeaponMartialChoice, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.ChainShirt, EquipmentDefinitions.OptionArmor, 1),
                },
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Spear, EquipmentDefinitions.OptionWeapon, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Handaxe, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Handaxe, EquipmentDefinitions.OptionWeapon, 1),
                },
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.ExplorerPack, EquipmentDefinitions.OptionStarterPack, 1),
                });
        }

        private static void BuildProficiencies()
        {
            FeatureDefinitionProficiencyArmor = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenArmor", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenArmorProficiency", Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor,
                    EquipmentDefinitions.LightArmorCategory,
                    EquipmentDefinitions.MediumArmorCategory,
                    EquipmentDefinitions.ShieldCategory)
                .AddToDB();

            FeatureDefinitionProficiencyWeapon = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenWeapon", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenWeaponProficiency", Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                    EquipmentDefinitions.SimpleWeaponCategory,
                    EquipmentDefinitions.MartialWeaponCategory)
                .AddToDB();

            FeatureDefinitionProficiencySavingThrow = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenSavingthrow", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenSavingthrowProficiency", Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Strength, AttributeDefinitions.Constitution)
                .AddToDB();

            FeatureDefinitionPointPoolSkills = FeatureDefinitionPointPoolBuilder
                .Create("PointPoolWardenSkillPoints", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenSkillProficiency", Category.Class)
                .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
                .RestrictChoices(
                    SkillDefinitions.AnimalHandling,
                    SkillDefinitions.Athletics,
                    SkillDefinitions.Nature,
                    SkillDefinitions.Perception,
                    SkillDefinitions.Survival)
                .AddToDB();
        }

        private static void BuildSentinelStand()
        {

            var heavyArmorProficiency = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenHeavyArmor", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenHeavyArmorProficiency", Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
                .AddToDB();

            // Like Witch Vision Curse, this would need to depend on the character's CON bonus. Setting to 3 for now
            var primalToughnessFlat = FeatureDefinitionAttributeModifierBuilder
                .Create("WardenPrimalToughnessFlat", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPoints, 3)
                .AddToDB();

            var primalToughnessPerLevel = FeatureDefinitionAttributeModifierBuilder
                .Create("WardenPrimalToughnessPerLevel", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
                .AddToDB();

            var primalToughness = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetRangerHunterMultiAttackChoice, "WardenPrimalToughness", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetFeatureSet(primalToughnessFlat, primalToughnessPerLevel)
                .AddToDB();

            var stalwartDexterity = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenSavingThrowDexterity", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Dexterity)
                .AddToDB();

            var stalwartIntelligence = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenSavingThrowIntelligence", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Intelligence)
                .AddToDB();

            var stalwartWisdom = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenSavingThrowWisdom", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Wisdom)
                .AddToDB();

            var stalwartCharisma = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenSavingThrowCharisma", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma)
                .AddToDB();

            FeatureDefinitionFeatureSetSentinelStand = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetRangerHunterMultiAttackChoice, "WardenFeatureSetSentinelStand", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class)
                .SetFeatureSet(heavyArmorProficiency, primalToughness, stalwartDexterity, stalwartIntelligence, stalwartWisdom, stalwartCharisma)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .AddToDB();
        }

        private static void BuildWardenGrasp()
        {
            var wardenGraspMovementAffinity = FeatureDefinitionMovementAffinityBuilder
                .Create(FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained, "MovementWardenGrasp", WARDEN_BASE_GUID)
                .SetGuiPresentation("MovementWardenGrasp", Category.Modifier)
                .AddToDB();

            var wardenGraspConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionHeavilyEncumbered, "ConditionWardenGrasp", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenGrasp", Category.Condition, ConditionDefinitions.ConditionRestrained.GuiPresentation.SpriteReference)
                .AddToDB()
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetDurationParameter(1)
            .SetDurationType(RuleDefinitions.DurationType.Round)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            wardenGraspConditionDefinition.RecurrentEffectForms.Clear();
            wardenGraspConditionDefinition.Features.Clear();
            wardenGraspConditionDefinition.Features.Add(wardenGraspMovementAffinity);

            var wardenGraspConditionForm = new ConditionForm()
                .SetConditionDefinition(wardenGraspConditionDefinition);

            var wardenGraspEffectForm = new EffectForm()
                .SetFormType(EffectForm.EffectFormType.Condition)
                .SetCreatedByCharacter(true)
                .SetConditionForm(wardenGraspConditionForm);

            var wardenGraspEffectDescription = new EffectDescription();
            wardenGraspEffectDescription.Copy(SpellDefinitions.Entangle.EffectDescription);
            wardenGraspEffectDescription
                .SetCanBePlacedOnCharacter(true)
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetHasSavingThrow(false)
                .SetRangeType(RuleDefinitions.RangeType.Self)
                .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.No)
                .SetTargetExcludeCaster(false)
                .SetTargetParameter(3)
                .SetTargetSide(RuleDefinitions.Side.All)
                .SetTargetType(RuleDefinitions.TargetType.Cube);
            wardenGraspEffectDescription.EffectParticleParameters.SetZoneParticleReference(null);
            wardenGraspEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Large);
            wardenGraspEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Medium);
            wardenGraspEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Small);
            wardenGraspEffectDescription.RestrictedCharacterSizes.Add(RuleDefinitions.CreatureSize.Tiny);
            wardenGraspEffectDescription.EffectForms.Clear();
            wardenGraspEffectDescription.EffectForms.Add(wardenGraspEffectForm);

            FeatureDefinitionPowerWardenGrasp = FeatureDefinitionPowerBuilder
                .Create("WardenGrasp", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class, SpellDefinitions.Entangle.GuiPresentation.SpriteReference)
                .SetActivation(RuleDefinitions.ActivationTime.BonusAction, 0)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffectDescription(wardenGraspEffectDescription)
                .AddToDB();

        }

        private static void BuildFightingStyle()
        {
            FeatureDefinitionFightingStyleChoiceWarden = FeatureDefinitionFightingStyleChoiceBuilder
                .Create(FeatureDefinitionFightingStyleChoices.FightingStyleFighter, "FightingStyleWarden", WARDEN_BASE_GUID)
                .SetGuiPresentation("FightingStyleWarden", Category.Class)
                .AddToDB();

            FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Clear();
            FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Add(DatabaseHelper.FightingStyleDefinitions.GreatWeapon.Name);
            FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Add(DatabaseHelper.FightingStyleDefinitions.Protection.Name);
            if (DatabaseRepository.GetDatabase<FightingStyleDefinition>().TryGetElement("Crippling", out FightingStyleDefinition crippling))
            {
                FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Add(crippling.Name);
            }
            if (DatabaseRepository.GetDatabase<FightingStyleDefinition>().TryGetElement("TitanFighting", out FightingStyleDefinition titanFighting))
            {
                FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Add(titanFighting.Name);
            }
        }

        private static void BuildWardenMark()
        {
        }


        private class WardenResolveEffectForm : CustomEffectForm
        {
            public static ConditionDefinition conditionWardenResolve { get; private set; }
            public WardenResolveEffectForm() : base()
            {
                if (conditionWardenResolve == null)
                {
                    var wardenResolveConditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionRaging, "ConditionWardenResolve", WARDEN_BASE_GUID)
                        .SetGuiPresentation("WardenResolve", Category.Condition, ConditionDefinitions.ConditionRaging.GuiPresentation.SpriteReference)
                        .AddToDB()
                    .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                    .SetDurationParameter(1)
                    .SetDurationType(RuleDefinitions.DurationType.Instantaneous)
                    .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
                    wardenResolveConditionDefinition.RecurrentEffectForms.Clear();
                    wardenResolveConditionDefinition.Features.Clear();
                    wardenResolveConditionDefinition.Features.Add(FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
                    wardenResolveConditionDefinition.Features.Add(FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance);
                    wardenResolveConditionDefinition.Features.Add(FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance);

                    conditionWardenResolve = wardenResolveConditionDefinition;
                }
            }
            public override void ApplyForm(
                RulesetImplementationDefinitions.ApplyFormsParams formsParams, 
                bool retargeting,
                bool proxyOnly,
                bool forceSelfConditionOnly,
                RuleDefinitions.EffectApplication effectApplication = RuleDefinitions.EffectApplication.All,
                List<EffectFormFilter> filters = null)
            {
                if (formsParams.sourceCharacter.CurrentHitPoints <= formsParams.sourceCharacter.MissingHitPoints)
                {
                    // Apply our resistances
                    ApplyCondition(formsParams, conditionWardenResolve, RuleDefinitions.DurationType.Round, 1);
                }
            }
            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap) { }
            private static void ApplyCondition(RulesetImplementationDefinitions.ApplyFormsParams formsParams, ConditionDefinition condition, RuleDefinitions.DurationType durationType, int durationParam)
            {
                // Prepare params for inflicting conditions
                ulong sourceGuid = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.Guid : 0L;
                string sourceFaction =
 formsParams.sourceCharacter != null ? formsParams.sourceCharacter.CurrentFaction.Name : string.Empty;
                string effectDefinitionName = string.Empty;

                if (formsParams.attackMode != null)
                {
                    effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
                }
                else if (formsParams.activeEffect != null)
                {
                    effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
                }

                int sourceAbilityBonus =
 (formsParams.activeEffect?.ComputeSourceAbilityBonus(formsParams.sourceCharacter)) ?? 0;

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.StartOfTurn, AttributeDefinitions.TagEffect, sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }

        private static void BuildWardenResolve()
        {
            var wardenResolveEffectDescription = new EffectDescription();
            wardenResolveEffectDescription.Copy(SpellDefinitions.ShieldOfFaith.EffectDescription);
            wardenResolveEffectDescription
                .SetCanBePlacedOnCharacter(true)
                .SetDurationType(RuleDefinitions.DurationType.Permanent)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetRangeType(RuleDefinitions.RangeType.Self)
                .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnActivation | RuleDefinitions.RecurrentEffect.OnTurnEnd | RuleDefinitions.RecurrentEffect.OnTurnStart)
                .SetTargetType(RuleDefinitions.TargetType.Self);
            wardenResolveEffectDescription.EffectForms.Clear();
            wardenResolveEffectDescription.EffectForms.Add(new WardenResolveEffectForm());

            FeatureDefinitionPowerWardenResolve = FeatureDefinitionPowerBuilder
                .Create("WardenResolve", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class, SpellDefinitions.ShieldOfFaith.GuiPresentation.SpriteReference)
                .SetActivation(RuleDefinitions.ActivationTime.PermanentUnlessIncapacitated, 0)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffectDescription(wardenResolveEffectDescription)
                .AddToDB();
        }

        private static void BuildFontOfLife()
        {

            List<ConditionDefinition> healedConditions = new List<ConditionDefinition>();
            healedConditions.Add(ConditionDefinitions.ConditionBlinded);
            healedConditions.Add(ConditionDefinitions.ConditionCharmed);
            healedConditions.Add(ConditionDefinitions.ConditionDeafened);
            healedConditions.Add(ConditionDefinitions.ConditionDiseased);
            healedConditions.Add(ConditionDefinitions.ConditionFrightened);
            healedConditions.Add(ConditionDefinitions.ConditionParalyzed);
            healedConditions.Add(ConditionDefinitions.ConditionPoisoned);

            var fontOfLifeConditionForm = new ConditionForm()
                .SetConditionDefinition(ConditionDefinitions.ConditionParalyzed)
                .SetOperation(ConditionForm.ConditionOperation.RemoveDetrimentalRandom);
            fontOfLifeConditionForm.ConditionsList.AddRange(healedConditions);

            var fontOfLifeEffectForm = new EffectForm()
                .SetCreatedByCharacter(true)
                .SetFormType(EffectForm.EffectFormType.Condition)
                .SetConditionForm(fontOfLifeConditionForm);

            var fontOfLifeEffectDescription = new EffectDescription();
            fontOfLifeEffectDescription.Copy(SpellDefinitions.LesserRestoration.EffectDescription);
            fontOfLifeEffectDescription
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetHasSavingThrow(false)
                .SetRangeParameter(1)
                .SetRangeType(RuleDefinitions.RangeType.Self)
                .SetSavingThrowAbility(AttributeDefinitions.Constitution)
                .SetTargetParameter(1)
                .SetTargetType(RuleDefinitions.TargetType.Self);
            fontOfLifeEffectDescription.EffectForms.Clear();
            fontOfLifeEffectDescription.EffectForms.Add(fontOfLifeEffectForm);

            FeatureDefinitionPowerFontOfLife = FeatureDefinitionPowerBuilder
                .Create("FontOfLife", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class, SpellDefinitions.LesserRestoration.GuiPresentation.SpriteReference)
                .SetActivation(RuleDefinitions.ActivationTime.Action, 1)
                .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
                .SetUsesFixed(1)
                .SetEffectDescription(fontOfLifeEffectDescription)
                .AddToDB();

        }

        private static void BuildExtraAttack()
        {
            FeatureDefinitionAttributeModifierExtraAttack = FeatureDefinitionAttributeModifierBuilder
                .Create(FeatureDefinitionAttributeModifiers.AttributeModifierRangerExtraAttack, "WardenExtraAttack", WARDEN_BASE_GUID)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.AttacksNumber, 1)
                .AddToDB();
        }

        private static void BuildSentinelStep()
        {

            var earthstrength = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetRangerHunterMultiAttackChoice, "WardenEarthstrength", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(FeatureDefinitionEquipmentAffinitys.EquipmentAffinityFeatHauler, FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityDwarvenPlateResistShove)
                .AddToDB();
            /*
                        var thunderingChargeSpeed = FeatureDefinitionBuilder<FeatureDefinitionMovementAffinity>
                            .Create(FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots, "WardenThunderingChargeSpeed", WARDEN_BASE_GUID)
                            .SetGuiPresentation(Category.Modifier)
                            .AddToDB()
                        .SetBaseSpeedAdditiveModifier(6);

                        var thunderingChargeAdvantage = FeatureDefinitionBuilder<FeatureDefinitionCombatAffinity>
                            .Create(FeatureDefinitionCombatAffinitys.CombatAffinityReckless, "WardenThunderingChargeAdvantage", WARDEN_BASE_GUID)
                            .SetGuiPresentation(Category.Modifier)
                            .AddToDB();

                        var test = FeatureDefinitionConditionalPowerBuilder
                            .Create("test", WARDEN_BASE_GUID)
                            .
            */
            /*
                        var thunderingChargeAdvantage = new ConditionDefinitionBuilder<ConditionDefinition>(
                            ConditionDefinitions.ConditionReckless, "ConditionWardenThunderingChargeAdvantage", WARDEN_BASE_GUID)
                                .SetGuiPresentation("WardenThunderingChargeAdvantage", Category.Condition, ConditionDefinitions.ConditionReckless.GuiPresentation.SpriteReference)
                                .AddToDB()
                            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                            .SetDurationParameter(1)
                            .SetDurationType(RuleDefinitions.DurationType.Round)
                            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                            .SetSpecialInterruptions(RuleDefinitions.ConditionInterruption.Attacks);*/

            var thunderingCharge = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetRangerHunterMultiAttackChoice, "WardenThunderingCharge", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Feature)
                //                .SetFeatures(thunderingChargeSpeed, thunderingChargeAdvantage)
                .AddToDB();

            var wildbloodInsight = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup();
            wildbloodInsight.abilityCheckModifierDiceNumber = 5;
            wildbloodInsight.abilityCheckModifierDieType = RuleDefinitions.DieType.D1;
            wildbloodInsight.abilityScoreName = AttributeDefinitions.Intelligence;
            wildbloodInsight.proficiencyName = SkillDefinitions.Investigation;

            var wildbloodPerception = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup();
            wildbloodPerception.abilityCheckModifierDiceNumber = 5;
            wildbloodPerception.abilityCheckModifierDieType = RuleDefinitions.DieType.D1;
            wildbloodPerception.abilityScoreName = AttributeDefinitions.Wisdom;
            wildbloodPerception.proficiencyName = SkillDefinitions.Perception;

            var wildbloodSkillBonus = FeatureDefinitionAbilityCheckAffinityBuilder
                .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinitySpoonOfDiscord, "WardenWildbloodSkillBonus", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Modifier)
                .AddToDB()
            .SetAffinityGroups(wildbloodInsight, wildbloodPerception);

            var wildblood = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetRangerHunterMultiAttackChoice, "WardenWildblood", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(FeatureDefinitionCampAffinitys.CampAffinityDomainOblivionPeacefulRest, wildbloodSkillBonus)
                .AddToDB();

            FeatureDefinitionFeatureSetSentinelStep = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetRangerHunterMultiAttackChoice, "WardenFeatureSetSentinelStep", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(earthstrength, thunderingCharge, wildblood)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .AddToDB();

        }

        private static void BuildUndying()
        {

            FeatureDefinitionDamageAffinityUndying = FeatureDefinitionDamageAffinityBuilder
                .Create(FeatureDefinitionDamageAffinitys.DamageAffinityHalfOrcRelentlessEndurance, "WardenUndying", WARDEN_BASE_GUID)
                .AddToDB();

        }

        private static void BuildInterrupt()
        {

            // Until I understand how to apply a condition on an enemy as part of a reaction power, I will put this as a +2 AC boost
            var wardenInterruptAttributeModifier = FeatureDefinitionAttributeModifierBuilder
                .Create("ModifierWardenInterrupt", WARDEN_BASE_GUID)
                //                .SetGuiPresentation(Category.Modifier, ConditionDefinitions.ConditionSlowed.GuiPresentation.SpriteReference)
                .SetGuiPresentation(Category.Modifier, ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
                .AddToDB();

            var wardenInterruptConditionDefinition = ConditionDefinitionBuilder
                //                .Create(ConditionDefinitions.ConditionSlowed, "ConditionWardenInterrupt", WARDEN_BASE_GUID)
                .Create("ConditionWardenInterrupt", WARDEN_BASE_GUID)
                //                .SetGuiPresentation("WardenInterrupt", Category.Condition, ConditionDefinitions.ConditionSlowed.GuiPresentation.SpriteReference)
                .SetGuiPresentation("WardenInterrupt", Category.Condition, ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference)
                .AddToDB()
            //            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetDurationParameter(0)
            .SetDurationType(RuleDefinitions.DurationType.Round)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn);
            wardenInterruptConditionDefinition.RecurrentEffectForms.Clear();
            wardenInterruptConditionDefinition.Features.Clear();
            wardenInterruptConditionDefinition.Features.Add(wardenInterruptAttributeModifier);

            var wardenInterruptConditionForm = new ConditionForm()
                .SetConditionDefinition(wardenInterruptConditionDefinition);

            var wardenInterruptEffectForm = new EffectForm()
                .SetFormType(EffectForm.EffectFormType.Condition)
                .SetCreatedByCharacter(true)
                .SetConditionForm(wardenInterruptConditionForm);

            var wardenInterruptEffectDescription = new EffectDescription();
            wardenInterruptEffectDescription.Copy(SpellDefinitions.Longstrider.EffectDescription);
            wardenInterruptEffectDescription
                .SetDurationParameter(0)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                //                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetHasSavingThrow(false)
                .SetRangeParameter(1)
                .SetRangeType(RuleDefinitions.RangeType.MeleeHit)
                .SetTargetParameter(1)
                .SetTargetType(RuleDefinitions.TargetType.Individuals);
            wardenInterruptEffectDescription.EffectForms.Clear();
            wardenInterruptEffectDescription.EffectForms.Add(wardenInterruptEffectForm);

            FeatureDefinitionPowerInterrupt = FeatureDefinitionPowerBuilder
                .Create("WardenInterrupt", WARDEN_BASE_GUID)
                //                .SetGuiPresentation(Category.Class, SpellDefinitions.Slow.GuiPresentation.SpriteReference)
                .SetGuiPresentation(Category.Class)
                .SetActivation(RuleDefinitions.ActivationTime.Reaction, 0)
                .SetReaction(RuleDefinitions.ReactionTriggerContext.HitByMelee, string.Empty)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffectDescription(wardenInterruptEffectDescription)
                .AddToDB();

        }

        private static void BuildSentinelSoul()
        {

            var agelessGuardianDexSavingThrowAdvantage = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WardenAgelessGuardianDexSavingThrowAdvantage", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Modifier)
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true, AttributeDefinitions.Dexterity)
                .AddToDB();

            var agelessGuardian = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenAgelessGuardian", WARDEN_BASE_GUID)
                .SetFeatureSet(
                    FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityDiseaseImmunity,
                    agelessGuardianDexSavingThrowAdvantage)
                .AddToDB();

            var eyesOfTheMountainTremorsense = FeatureDefinitionSenseBuilder
                .Create(FeatureDefinitionSenses.SenseTremorsense16, "WardenEyesOfTheMountainTremorsense", WARDEN_BASE_GUID)
                .AddToDB()
                .SetSenseRange(3)
                .SetStealthBreakerRange(3);

            var eyesOfTheMountainSeeInvisible = FeatureDefinitionSenseBuilder
                .Create(FeatureDefinitionSenses.SenseSeeInvisible12, "WardenEyesOfTheMountainSeeInvisible", WARDEN_BASE_GUID)
                .AddToDB()
                .SetSenseRange(6)
                .SetStealthBreakerRange(6);

            var eyesOfTheMountainConSavingThrowAdvantage = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WardenEyesOfTheMountainConSavingThrowAdvantage", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Modifier)
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true, AttributeDefinitions.Constitution)
                .AddToDB();

            var eyesOfTheMountain = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenEyesOfTheMountain", WARDEN_BASE_GUID)
                .SetFeatureSet(eyesOfTheMountainTremorsense, eyesOfTheMountainSeeInvisible, eyesOfTheMountainConSavingThrowAdvantage)
                .AddToDB();

            var impenetrableMindWisSavingThrowAdvantage = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WardenImpenetrableMindWisSavingThrowAdvantage", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Modifier)
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true, AttributeDefinitions.Wisdom)
                .AddToDB();

            var impenetrableMind = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenImpenetrableMind", WARDEN_BASE_GUID)
                .SetFeatureSet(
                    FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                    impenetrableMindWisSavingThrowAdvantage)
                .AddToDB();

            FeatureDefinitionFeatureSetSentinelSoul = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenFeatureSetSentinelSoul", WARDEN_BASE_GUID)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .SetFeatureSet(agelessGuardian, eyesOfTheMountain, impenetrableMind)
                .AddToDB();

        }

        private static CharacterClassDefinition BuildAndAddClass()
        {

            var classBuilder = CharacterClassDefinitionBuilder
                .Create("ClassWarden", WARDEN_BASE_GUID)
                .SetGuiPresentation("Warden", Category.Class, Fighter.GuiPresentation.SpriteReference);

            BuildClassStats(classBuilder);
            BuildEquipment(classBuilder);
            BuildProficiencies();
            BuildSentinelStand();
            BuildWardenGrasp();
            BuildFightingStyle();
            //            BuildWardenMark();
            BuildWardenResolve();
            BuildFontOfLife();
            BuildExtraAttack();
            BuildSentinelStep();
            BuildUndying();
            BuildInterrupt();
            BuildSentinelSoul();

            var warden = classBuilder.AddToDB();

            BuildSubclasses();
            BuildProgression();

            return warden;

            void BuildSubclasses()
            {
                var subclassChoices = FeatureDefinitionSubclassChoiceBuilder
                    .Create("SubclassChoiceWardenCalls", WARDEN_BASE_GUID)
                    .SetGuiPresentation("WardenSubclassPath", Category.Subclass)
                    .SetSubclassSuffix("Call")
                    .SetFilterByDeity(false)
                    .SetSubclasses(
                        GreyWatchman.GetSubclass(warden),
                        StoneheartDefender.GetSubclass(warden))
                    .AddToDB();

                classBuilder.AddFeatureAtLevel(3, subclassChoices);
            }

            void BuildProgression()
            {
                if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out FeatureDefinition help))
                {
                    classBuilder.AddFeatureAtLevel(1, help);
                }

                classBuilder
                    .AddFeaturesAtLevel(1,
                        FeatureDefinitionProficiencyArmor,
                        FeatureDefinitionProficiencyWeapon,
                        FeatureDefinitionProficiencySavingThrow,
                        FeatureDefinitionPointPoolSkills,
                        FeatureDefinitionFeatureSetSentinelStand,
                        FeatureDefinitionPowerWardenGrasp)
                    .AddFeaturesAtLevel(2,
                        FeatureDefinitionFightingStyleChoiceWarden)//,
                                                                   //                        FeatureDefinitionPowerWardenMark)
                    .AddFeatureAtLevel(3, FeatureDefinitionPowerWardenResolve)
                    .AddFeaturesAtLevel(4,
                        FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                        FeatureDefinitionPowerFontOfLife)
                    .AddFeatureAtLevel(5, FeatureDefinitionAttributeModifierExtraAttack)
                    //                    .AddFeatureAtLevel(7, FeatureDefinitionFeatureSetSentinelStep)
                    .AddFeatureAtLevel(8, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
                    .AddFeatureAtLevel(9, FeatureDefinitionDamageAffinityUndying)
                    .AddFeatureAtLevel(10, FeatureDefinitionPowerInterrupt)
                    .AddFeatureAtLevel(12, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
                    .AddFeatureAtLevel(16, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
                    .AddFeatureAtLevel(18, FeatureDefinitionFeatureSetSentinelSoul)
                    .AddFeatureAtLevel(19, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            }
        }
    }
}
#endif
