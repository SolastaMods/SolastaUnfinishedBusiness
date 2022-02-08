using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warden.Subclasses;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static CharacterClassDefinition;
using static SolastaModApi.DatabaseHelper;
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
        public static FeatureDefinitionCombatAffinity FeatureDefinitionDamageAffinityWardenResolve { get; private set; }
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
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPoints, 3)
                .AddToDB();

            var primalToughnessPerLevel = FeatureDefinitionAttributeModifierBuilder
                .Create("WardenPrimalToughnessPerLevel", WARDEN_BASE_GUID)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
                .AddToDB();

            var primalToughness = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenPrimalToughness", WARDEN_BASE_GUID)
                .SetFeatures(primalToughnessFlat, primalToughnessPerLevel)
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
                .SetGuiPresentation("WardenWisdomSavingThrowProficiency", Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Wisdom)
                .AddToDB();

            var stalwartCharisma = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyWardenSavingThrowCharisma", WARDEN_BASE_GUID)
                .SetGuiPresentation("WardenCharismaSavingThrowProficiency", Category.Class)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma)
                .AddToDB();

            FeatureDefinitionFeatureSetSentinelStand = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenFeatureSetSentinelStand", WARDEN_BASE_GUID)
                .SetFeatures(heavyArmorProficiency, primalToughness, stalwartDexterity, stalwartIntelligence, stalwartWisdom, stalwartCharisma)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .AddToDB();
        }

        private static void BuildWardenGrasp()
        {

            var wardenGraspMovementAffinity = new FeatureDefinitionBuilder<FeatureDefinitionMovementAffinity>(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained, "MovementWardenGrasp", WARDEN_BASE_GUID)
                    .SetGuiPresentation("MovementWardenGrasp", Category.Modifier)
                    .AddToDB();

            var wardenGraspConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                ConditionDefinitions.ConditionHeavilyEncumbered, "ConditionWardenGrasp", WARDEN_BASE_GUID)
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
                .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffect(wardenGraspEffectDescription)
                .AddToDB();

        }

        private static void BuildFightingStyle()
        {

            FeatureDefinitionFightingStyleChoiceWarden = new FeatureDefinitionBuilder<FeatureDefinitionFightingStyleChoice>(
                FeatureDefinitionFightingStyleChoices.FightingStyleFighter, "FightingStyleWarden", WARDEN_BASE_GUID)
                    .SetGuiPresentation("FightingStyleWarden", Category.Class)
                    .AddToDB();

            FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Clear();

            // I'm not sure why but this here will crash? 
            FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Add(DatabaseHelper.FightingStyleDefinitions.GreatWeapon.ToString());
            FeatureDefinitionFightingStyleChoiceWarden.FightingStyles.Add(DatabaseHelper.FightingStyleDefinitions.Protection.ToString());

        }

        private static void BuildWardenMark()
        {
        }

        private static void BuildWardenResolve()
        {
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
                .SetRecharge(RuleDefinitions.RechargeRate.ShortRest)
                .SetUsesFixed(1)
                .SetEffect(fontOfLifeEffectDescription)
                .AddToDB();

        }

        private static void BuildExtraAttack()
        {
            FeatureDefinitionAttributeModifierExtraAttack = FeatureDefinitionAttributeModifierBuilder
                .Create("WardenExtraAttack", WARDEN_BASE_GUID)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.AttacksNumber, 1)
                .AddToDB();
        }

        private static void BuildSentinelStep()
        {

            var earthstrength = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenEarthstrength", WARDEN_BASE_GUID)
//                .SetFeatures(abc)
                .AddToDB();

            var thunderingCharge = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenThunderingCharge", WARDEN_BASE_GUID)
//                .SetFeatures(abc)
                .AddToDB();

            var wildblood = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenWildblood", WARDEN_BASE_GUID)
//                .SetFeatures(abc)
                .AddToDB();

            FeatureDefinitionFeatureSetSentinelStep = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenFeatureSetSentinelStep", WARDEN_BASE_GUID)
                .SetFeatures(earthstrength, thunderingCharge, wildblood)
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

            var wardenInterruptAttributeModifier = FeatureDefinitionAttributeModifierBuilder
                .Create("ModifierWardenInterrupt", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Modifier, ConditionDefinitions.ConditionSlowed.GuiPresentation.SpriteReference)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.AttacksNumber, -1)
                .AddToDB();

            var wardenInterruptConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                ConditionDefinitions.ConditionSlowed, "ConditionWardenInterrupt", WARDEN_BASE_GUID)
                    .SetGuiPresentation("WardenInterrupt", Category.Condition, ConditionDefinitions.ConditionSlowed.GuiPresentation.SpriteReference)
                    .AddToDB()
                .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
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
            wardenInterruptEffectDescription.Copy(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription);
            wardenInterruptEffectDescription
                .SetDurationParameter(1)
                .SetDurationType(RuleDefinitions.DurationType.Round)
                .SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetHasSavingThrow(false)
                .SetRangeParameter(1)
                .SetRangeType(RuleDefinitions.RangeType.MeleeHit)
                .SetTargetParameter(1)
                .SetTargetType(RuleDefinitions.TargetType.Individuals);
            wardenInterruptEffectDescription.EffectForms.Clear();
            wardenInterruptEffectDescription.EffectForms.Add(wardenInterruptEffectForm);

            FeatureDefinitionPowerInterrupt = FeatureDefinitionPowerBuilder
                .Create("WardenInterrupt", WARDEN_BASE_GUID)
                .SetGuiPresentation(Category.Class, SpellDefinitions.Slow.GuiPresentation.SpriteReference)
                .SetActivation(RuleDefinitions.ActivationTime.Reaction, 0)
                .SetReaction(RuleDefinitions.ReactionTriggerContext.HitByMelee, string.Empty)
                .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
                .SetUsesFixed(1)
                .SetEffect(wardenInterruptEffectDescription)
                .AddToDB();

        }

        private static void BuildSentinelSoul()
        {

            // SavingThrow builder is a bit more complex, keeping legacy for now
            var agelessGuardianDexSavingThrowAdvantage = new FeatureDefinitionSavingThrowAffinityBuilder(
                    "WardenAgelessGuardianDexSavingThrowAdvantage",
                    GuidHelper.Create(WARDEN_BASE_GUID, "WardenAgelessGuardianDexSavingThrowAdvantage").ToString(),
                    new List<string>{ AttributeDefinitions.Dexterity },
                    RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                    true,
                    new GuiPresentationBuilder(
                            "Class/&WardenAgelessGuardianDexSavingThrowAdvantageDescription",
                            "Class/&WardenAgelessGuardianDexSavingThrowAdvantageTitle").Build())
                    .AddToDB();

            var agelessGuardian = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenAgelessGuardian", WARDEN_BASE_GUID)
                .SetFeatures(
                    FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity, 
                    FeatureDefinitionConditionAffinitys.ConditionAffinityDiseaseImmunity, 
                    agelessGuardianDexSavingThrowAdvantage)
                .AddToDB();

            var eyesOfTheMountainTremorsense = new FeatureDefinitionBuilder<FeatureDefinitionSense>(
                FeatureDefinitionSenses.SenseTremorsense16, "WardenEyesOfTheMountainTremorsense", WARDEN_BASE_GUID)
                .AddToDB()
                .SetSenseRange(3)
                .SetStealthBreakerRange(3);

            var eyesOfTheMountainSeeInvisible = new FeatureDefinitionBuilder<FeatureDefinitionSense>(
                FeatureDefinitionSenses.SenseSeeInvisible12, "WardenEyesOfTheMountainSeeInvisible", WARDEN_BASE_GUID)
                .AddToDB()
                .SetSenseRange(6)
                .SetStealthBreakerRange(6);

            // SavingThrow builder is a bit more complex, keeping legacy for now
            var eyesOfTheMountainConSavingThrowAdvantage = new FeatureDefinitionSavingThrowAffinityBuilder(
                    "WardenEyesOfTheMountainConSavingThrowAdvantage",
                    GuidHelper.Create(WARDEN_BASE_GUID, "WardenEyesOfTheMountainConSavingThrowAdvantage").ToString(),
                    new List<string>{ AttributeDefinitions.Constitution },
                    RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                    true,
                    new GuiPresentationBuilder(
                            "Class/&WardenEyesOfTheMountainConSavingThrowAdvantageDescription",
                            "Class/&WardenEyesOfTheMountainConSavingThrowAdvantageTitle").Build())
                    .AddToDB();

            var eyesOfTheMountain = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenEyesOfTheMountain", WARDEN_BASE_GUID)
                .SetFeatures(eyesOfTheMountainTremorsense, eyesOfTheMountainSeeInvisible, eyesOfTheMountainConSavingThrowAdvantage)
                .AddToDB();

            // SavingThrow builder is a bit more complex, keeping legacy for now
            var impenetrableMindWisSavingThrowAdvantage = new FeatureDefinitionSavingThrowAffinityBuilder(
                    "WardenImpenetrableMindWisSavingThrowAdvantage",
                    GuidHelper.Create(WARDEN_BASE_GUID, "WardenImpenetrableMindWisSavingThrowAdvantage").ToString(),
                    new List<string>{ AttributeDefinitions.Wisdom },
                    RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                    true,
                    new GuiPresentationBuilder(
                            "Class/&WardenImpenetrableMindWisSavingThrowAdvantageDescription",
                            "Class/&WardenImpenetrableMindWisSavingThrowAdvantageTitle").Build())
                    .AddToDB();

            var impenetrableMind = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenImpenetrableMind", WARDEN_BASE_GUID)
                .SetFeatures(
                    FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                    FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                    impenetrableMindWisSavingThrowAdvantage)
                .AddToDB();

            FeatureDefinitionFeatureSetSentinelSoul = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WardenFeatureSetSentinelSoul", WARDEN_BASE_GUID)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                .SetUniqueChoices(true)
                .SetFeatures(agelessGuardian, eyesOfTheMountain, impenetrableMind)
                .AddToDB();

        }

        private static CharacterClassDefinition BuildAndAddClass()
        {

            var classBuilder = new CharacterClassDefinitionBuilder("ClassWarden", WARDEN_BASE_GUID)
                .SetGuiPresentation("Warden", Category.Class, CharacterClassDefinitions.Fighter.GuiPresentation.SpriteReference);

            BuildClassStats(classBuilder);
            BuildEquipment(classBuilder);
            BuildProficiencies();
            BuildSentinelStand();
            BuildWardenGrasp();
//            BuildFightingStyle();
//            BuildWardenMark();
//            BuildWardenResolve();
            BuildFontOfLife();
            BuildExtraAttack();
//            BuildSentinelStep();
            BuildUndying();
//            BuildInterrupt();
            BuildSentinelSoul();

            var warden = classBuilder.AddToDB();

            BuildSubclasses();
            BuildProgression();

            return warden;

            void BuildSubclasses()
            {
                var subClassChoices = classBuilder.BuildSubclassChoice(
                    3,
                    "ChampionCall",
                    false,
                    "SubclassChoiceWardenChampionCalls",
                    new GuiPresentationBuilder(
                            "Subclass/&WardenSubclassPathDescription",
                            "Subclass/&WardenSubclassPathTitle")
                            .Build(),
                    GuidHelper.Create(WARDEN_BASE_GUID, "SubclassChoiceWardenChampionCalls").ToString());

                subClassChoices.Subclasses.Add(new GreyWatchman().GetSubclass(warden).name);
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
//                    .AddFeaturesAtLevel(2,
//                        FeatureDefinitionFightingStyleChoiceWarden,
//                        FeatureDefinitionPowerWardenMark)
//                    .AddFeatureAtLevel(3, FeatureDefinitionDamageAffinityWardenResolve)
                    .AddFeaturesAtLevel(4,
                        FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                        FeatureDefinitionPowerFontOfLife)
                    .AddFeatureAtLevel(5, FeatureDefinitionAttributeModifierExtraAttack)
//                    .AddFeatureAtLevel(7, FeatureDefinitionFeatureSetSentinelStep)
                    .AddFeatureAtLevel(8, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
                    .AddFeatureAtLevel(9, FeatureDefinitionDamageAffinityUndying)
//                    .AddFeatureAtLevel(10, FeatureDefinitionPowerInterrupt)
                    .AddFeatureAtLevel(12, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
                    .AddFeatureAtLevel(16, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)
                    .AddFeatureAtLevel(18, FeatureDefinitionFeatureSetSentinelSoul)
                    .AddFeatureAtLevel(19, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            }
        }
    }
}
