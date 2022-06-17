#if false
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static EquipmentDefinitions;
using static SolastaCommunityExpansion.Builders.EquipmentOptionsBuilder;

namespace SolastaCommunityExpansion.Classes.Magus;

public static class Magus
{
    public static CharacterClassDefinition ClassMagus { get; private set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyTool { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; set; }

    private static FeatureDefinitionPointPool FeatureDefinitionSkillPoints { get; set; }

    private static FeatureDefinitionCastSpell FeatureDefinitionClassMagusCastSpell { get; set; }

    public static FeatureDefinitionPower ArcaneFocus { get; private set; }

    private static void BuildEquipment(CharacterClassDefinitionBuilder classMagusBuilder)
    {
        classMagusBuilder
            .AddEquipmentRow(
                Column(
                    Option(DatabaseHelper.ItemDefinitions.Longbow, OptionWeaponMartialRangedChoice, 1),
                    Option(DatabaseHelper.ItemDefinitions.Arrow, OptionAmmoPack, 1)
                ),
                Column(Option(DatabaseHelper.ItemDefinitions.Rapier, OptionWeaponMartialMeleeChoice, 1)))
            .AddEquipmentRow(
                Column(Option(DatabaseHelper.ItemDefinitions.ScholarPack, OptionStarterPack, 1)),
                Column(Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, OptionStarterPack, 1)))
            .AddEquipmentRow(
                Column(
                    Option(DatabaseHelper.ItemDefinitions.Leather, OptionArmor, 1),
                    Option(DatabaseHelper.ItemDefinitions.ComponentPouch, OptionFocus, 1),
                    Option(DatabaseHelper.ItemDefinitions.Shortsword, OptionWeaponMartialMeleeChoice, 2)
                ));
    }

    private static void BuildProficiencies()
    {
        static FeatureDefinitionProficiency BuildProficiency(string name, RuleDefinitions.ProficiencyType type,
            params string[] proficiencies)
        {
            return FeatureDefinitionProficiencyBuilder
                .Create(name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(type, proficiencies)
                .AddToDB();
        }

        FeatureDefinitionProficiencyArmor =
            BuildProficiency("ClassMagusArmorProficiency", RuleDefinitions.ProficiencyType.Armor,
                LightArmorCategory);

        FeatureDefinitionProficiencyWeapon =
            BuildProficiency("ClassMagusWeaponProficiency", RuleDefinitions.ProficiencyType.Weapon,
                MartialWeaponCategory);

        FeatureDefinitionProficiencyTool =
            BuildProficiency("ClassMagusToolsProficiency", RuleDefinitions.ProficiencyType.Tool,
                DatabaseHelper.ToolTypeDefinitions.EnchantingToolType.Name,
                DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType.Name);

        FeatureDefinitionProficiencySavingThrow =
            BuildProficiency("ClassMagusSavingThrowProficiency", RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Dexterity, AttributeDefinitions.Wisdom);

        FeatureDefinitionSkillPoints = FeatureDefinitionPointPoolBuilder
            .Create("ClassMagusSkillProficiency", DefinitionBuilder.CENamespaceGuid)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 4)
            .OnlyUniqueChoices()
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.Athletics,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Arcana,
                SkillDefinitions.Deception,
                SkillDefinitions.History,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Perception,
                SkillDefinitions.Nature,
                SkillDefinitions.Religion)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private static void BuildSpells()
    {
        FeatureDefinitionClassMagusCastSpell = FeatureDefinitionCastSpellBuilder
            .Create("ClassMagusCastSpell", DefinitionBuilder.CENamespaceGuid)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetGuiPresentation("ClassMagusSpellcasting", Category.Class)
            .SetSpellCastingLevel(9)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel)
            .SetKnownCantrips(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            .SetKnownSpells(0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19)
            .SetSlotsPerLevel(2, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetSpellList(MagusSpells.MagusSpellList)
            .AddToDB();
    }

    private static void BuildArcaneArt()
    {
        ArcaneFocus = FeatureDefinitionPowerBuilder
            .Create("ClassMagusArcaneFocus", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Power)
            .SetFixedUsesPerRecharge(20)
            .SetActivationTime(RuleDefinitions.ActivationTime.OnAttackHit)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetUsesFixed(1)
            .AddToDB();

        var rupture = BuildRupture(ArcaneFocus);

        // lurking death
        // broken courage
        // menace present
        // mind spike
        // exile
        // immunity disruption: fire, ice, acid, thunder, lighting, necro
        // soul drinker: regain an arcana focus

        PowerBundleContext.RegisterPowerBundle(ArcaneFocus, true, rupture);
    }

    private static FeatureDefinitionPower BuildRupture(FeatureDefinitionPower sharedPool)
    {
        var condition = ConditionDefinitionBuilder
            .Create("ClassMagusConditionRupture", DefinitionBuilder.CENamespaceGuid)
            .Configure(RuleDefinitions.DurationType.Round, 1, true)
            .SetSpecialDuration(true)
            .AddToDB();

        condition.durationParameterDie = RuleDefinitions.DieType.D4;
        condition.turnOccurence = (RuleDefinitions.TurnOccurenceType)ExtraTurnOccurenceType.OnMoveEnd;
        condition.RecurrentEffectForms.Add(new EffectForm
        {
            formType = EffectForm.EffectFormType.Damage,
            damageForm = new DamageForm
            {
                damageType = RuleDefinitions.DamageTypePsychic,
                diceNumber = 2,
                dieType = RuleDefinitions.DieType.D8
            }
        });

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMagusArcaneArtRupture", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, "ClassMagusArcaneArtRupture")
            .SetSharedPool(sharedPool)
            .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Distance, 24,
                        RuleDefinitions.TargetType.Individuals)
                    .SetSavingThrowData(
                        true,
                        false,
                        AttributeDefinitions.Constitution,
                        false,
                        RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature,
                        AttributeDefinitions.Intelligence)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                            .SetConditionForm(condition, ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .Build())
            .AddToDB();
    }

    private static void BuildProgression(CharacterClassDefinitionBuilder classMagusBuilder)
    {
        // No subclass for now
        /*var subclassChoices = FeatureDefinitionSubclassChoiceBuilder
            .Create("ClassWarlockSubclassChoice", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassWarlockPatron", Category.Subclass)
            .SetSubclassSuffix("Patron")
            .SetFilterByDeity(false)
            .SetSubclasses(
                AHWarlockSubclassSoulBladePact.Build(),
                DHWarlockSubclassAncientForestPatron.Build(),
                DHWarlockSubclassElementalPatron.Build(),
                DHWarlockSubclassMoonLitPatron.Build()
            )
            .AddToDB();*/

        classMagusBuilder
            .AddFeaturesAtLevel(1,
                FeatureDefinitionProficiencySavingThrow,
                FeatureDefinitionProficiencyArmor,
                FeatureDefinitionProficiencyWeapon,
                FeatureDefinitionProficiencyTool,
                FeatureDefinitionSkillPoints
            )
            .AddFeaturesAtLevel(1,
                FeatureDefinitionClassMagusCastSpell,
                ArcaneFocus);
    }

    internal static CharacterClassDefinition BuildMagusClass()
    {
        var warlockSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Magus", Resources.Warlock, 1024, 576);

        var classMagusBuilder = CharacterClassDefinitionBuilder
            .Create("ClassMagus", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, warlockSpriteReference, 1)
            .AddFeatPreferences(DatabaseHelper.FeatDefinitions.PowerfulCantrip,
                DatabaseHelper.FeatDefinitions.FlawlessConcentration,
                DatabaseHelper.FeatDefinitions.Robust)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Violence, 3)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Self_Preservation, 3)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Normal, 3)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpSpellcaster, 5)
            .AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpExplorer, 1)
            .AddSkillPreferences(
                DatabaseHelper.SkillDefinitions.Acrobatics,
                DatabaseHelper.SkillDefinitions.Athletics,
                DatabaseHelper.SkillDefinitions.Persuasion,
                DatabaseHelper.SkillDefinitions.Arcana,
                DatabaseHelper.SkillDefinitions.Deception,
                DatabaseHelper.SkillDefinitions.History,
                DatabaseHelper.SkillDefinitions.Intimidation,
                DatabaseHelper.SkillDefinitions.Perception,
                DatabaseHelper.SkillDefinitions.Nature,
                DatabaseHelper.SkillDefinitions.Religion)
            .AddToolPreferences(
                DatabaseHelper.ToolTypeDefinitions.EnchantingToolType,
                DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType)
            .SetAbilityScorePriorities(
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Charisma)
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Paladin)
            .SetBattleAI(DatabaseHelper.DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetHitDice(RuleDefinitions.DieType.D8)
            .SetIngredientGatheringOdds(DatabaseHelper.CharacterClassDefinitions.Sorcerer.IngredientGatheringOdds)
            .SetPictogram(DatabaseHelper.CharacterClassDefinitions.Wizard.ClassPictogramReference);

        BuildEquipment(classMagusBuilder);
        BuildProficiencies();
        BuildSpells();
        BuildArcaneArt();
        BuildProgression(classMagusBuilder);

        ClassMagus = classMagusBuilder.AddToDB();

        var itemlist = new List<ItemDefinition>
        {
            DatabaseHelper.ItemDefinitions.WandOfLightningBolts,
            DatabaseHelper.ItemDefinitions.StaffOfFire,
            DatabaseHelper.ItemDefinitions.ArcaneShieldstaff,
            DatabaseHelper.ItemDefinitions.WizardClothes_Alternate
        };

        foreach (var item in itemlist)
        {
            item.RequiredAttunementClasses.Add(ClassMagus);
        }

        return ClassMagus;
    }
}
#endif
