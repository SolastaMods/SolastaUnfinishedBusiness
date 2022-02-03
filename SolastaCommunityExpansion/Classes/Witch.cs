using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Level20;
using SolastaCommunityExpansion.Subclasses.Witch;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static CharacterClassDefinition;
using static FeatureDefinitionCastSpell;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes
{
    internal static class Witch
    {
        public static readonly Guid WITCH_BASE_GUID = new("ea7715dd-00cb-45a3-a8c4-458d0639d72c");

        public static readonly CharacterClassDefinition Instance = BuildAndAddClass();
        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; private set; }
        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; private set; }
        public static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; private set; }
        public static FeatureDefinitionPointPool FeatureDefinitionPointPoolSkills { get; private set; }
        public static FeatureDefinitionPointPool FeatureDefinitionPointPoolTools { get; private set; }
        public static FeatureDefinitionCastSpell FeatureDefinitionCastSpellWitch { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetRitualCasting { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetWitchCurses { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetMaledictions { get; private set; }
        public static FeatureDefinitionPower FeatureDefinitionPowerCackle { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetWitchFamiliar { get; private set; }

        const string ClassCategory = "Class";

        private static void BuildClassStats(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard);
            classBuilder.SetPictogram(CharacterClassDefinitions.Sorcerer.ClassPictogramReference);
            classBuilder.SetBattleAI(CharacterClassDefinitions.Sorcerer.DefaultBattleDecisions);
            classBuilder.SetHitDice(RuleDefinitions.DieType.D8);
            classBuilder.SetIngredientGatheringOdds(CharacterClassDefinitions.Sorcerer.IngredientGatheringOdds);

            classBuilder.SetAbilityScorePriorities(
                    AttributeDefinitions.Charisma,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Strength);

            classBuilder.AddFeatPreference(FeatDefinitions.PowerfulCantrip);

            classBuilder.AddToolPreferences(
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType);

            classBuilder.AddSkillPreferences(
                DatabaseHelper.SkillDefinitions.Arcana,
                DatabaseHelper.SkillDefinitions.Deception,
                DatabaseHelper.SkillDefinitions.Insight,
                DatabaseHelper.SkillDefinitions.Intimidation,
                DatabaseHelper.SkillDefinitions.Persuasion,
                DatabaseHelper.SkillDefinitions.Nature,
                DatabaseHelper.SkillDefinitions.Religion);
        }

        private static void BuildEquipment(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.ScholarPack, EquipmentDefinitions.OptionStarterPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1),
                },
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionArcaneFocusChoice, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                EquipmentOptionsBuilder.Option(ItemDefinitions.SorcererArmor, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });
        }

        private static void BuildProficiencies()
        {
            FeatureDefinitionProficiencyArmor = new FeatureDefinitionProficiencyBuilder(
                        "ProficiencyWitchArmor", WITCH_BASE_GUID, RuleDefinitions.ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory)
                    .SetGuiPresentationGenerate("WitchArmorProficiency", "Class")
                    .AddToDB();

            FeatureDefinitionProficiencyWeapon = new FeatureDefinitionProficiencyBuilder(
                        "ProficiencyWitchWeapon", WITCH_BASE_GUID, RuleDefinitions.ProficiencyType.Weapon, EquipmentDefinitions.SimpleWeaponCategory)
                    .SetGuiPresentationGenerate("WitchWeaponProficiency", "Class")
                    .AddToDB();

            FeatureDefinitionProficiencySavingThrow = new FeatureDefinitionProficiencyBuilder(
                        "ProficiencyWitchSavingthrow", WITCH_BASE_GUID, RuleDefinitions.ProficiencyType.SavingThrow,
                         AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom).
                     SetGuiPresentationGenerate("WitchSavingthrowProficiency", "Class")
                    .AddToDB();

            FeatureDefinitionPointPoolSkills = new FeatureDefinitionPointPoolBuilder(
                        "PointPoolWitchSkillPoints", WITCH_BASE_GUID, HeroDefinitions.PointsPoolType.Skill, 2)
                    .SetGuiPresentationGenerate("WitchSkillProficiency", "Class")
                    .RestrictChoices(
                        SkillDefinitions.Arcana,
                        SkillDefinitions.Deception,
                        SkillDefinitions.Insight,
                        SkillDefinitions.Intimidation,
                        SkillDefinitions.Persuasion,
                        SkillDefinitions.Nature,
                        SkillDefinitions.Religion)
                    .AddToDB();

            FeatureDefinitionPointPoolTools = new FeatureDefinitionPointPoolBuilder(
                        "ProficiencyWitchTool", WITCH_BASE_GUID, HeroDefinitions.PointsPoolType.Tool, 1)
                    .SetGuiPresentationGenerate("WitchToolProficiency", "Class")
                    .RestrictChoices(
                        ToolTypeDefinitions.HerbalismKitType.Name,
                        ToolTypeDefinitions.PoisonersKitType.Name)
                    .AddToDB();
        }

        private static void BuildSpells()
        {
            // Keeping all spells listed here for ease to edit and see the big picture
            var classSpellList = SpellListDefinitionBuilder.CreateSpellList(
                    "WitchSpellList",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchSpellList").ToString(),
                    "",
                    SpellListDefinitions.SpellListWizard,
                    new List<SpellDefinition>
                    {
                    SpellDefinitions.AcidSplash,
//?                                                                                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                    SpellDefinitions.ChillTouch,
                    SpellDefinitions.DancingLights,
//?                                                                                    DatabaseHelper.SpellDefinitions.Dazzle,
//                                                                                    DatabaseHelper.SpellDefinitions.FireBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.Guidance,
//                                                                                    DatabaseHelper.SpellDefinitions.Light,
//                                                                                    DatabaseHelper.SpellDefinitions.PoisonSpray,
                    SpellDefinitions.ProduceFlame,
//                                                                                    DatabaseHelper.SpellDefinitions.RayOfFrost,
                    SpellDefinitions.Resistance,
//                                                                                    DatabaseHelper.SpellDefinitions.SacredFlame,
//?                                                                                    DatabaseHelper.SpellDefinitions.ShadowArmor,
//?                                                                                    DatabaseHelper.SpellDefinitions.ShadowDagger,
//                                                                                    DatabaseHelper.SpellDefinitions.Shillelagh,
//?                                                                                    DatabaseHelper.SpellDefinitions.Shine,
//                                                                                    DatabaseHelper.SpellDefinitions.ShockingGrasp,
                    SpellDefinitions.SpareTheDying,
//?                                                                                    DatabaseHelper.SpellDefinitions.Sparkle,
                    SpellDefinitions.TrueStrike
                        //                                                                                    DatabaseHelper.SpellDefinitions.VenomousSpike
                    },
                    new List<SpellDefinition>
                    {
                    SpellDefinitions.AnimalFriendship,
                    SpellDefinitions.Bane,
//                                                                                    DatabaseHelper.SpellDefinitions.Bless,
//                                                                                    DatabaseHelper.SpellDefinitions.BurningHands,
                    SpellDefinitions.CharmPerson,
//                                                                                    DatabaseHelper.SpellDefinitions.ColorSpray,
//                                                                                    DatabaseHelper.SpellDefinitions.Command,
                    SpellDefinitions.ComprehendLanguages,
//                                                                                    DatabaseHelper.SpellDefinitions.CureWounds,
//                                                                                    DatabaseHelper.SpellDefinitions.DetectEvilAndGood,
                    SpellDefinitions.DetectMagic,
//                                                                                    DatabaseHelper.SpellDefinitions.DetectPoisonAndDisease,
//                                                                                    DatabaseHelper.SpellDefinitions.DivineFavor,
//                                                                                    DatabaseHelper.SpellDefinitions.Entangle,
                    SpellDefinitions.ExpeditiousRetreat,
                    SpellDefinitions.FaerieFire,
//                                                                                    DatabaseHelper.SpellDefinitions.FalseLife,
//                                                                                    DatabaseHelper.SpellDefinitions.FeatherFall,
//                                                                                    DatabaseHelper.SpellDefinitions.FogCloud,
//                                                                                    DatabaseHelper.SpellDefinitions.Goodberry,
//                                                                                    DatabaseHelper.SpellDefinitions.Grease,
//                                                                                    DatabaseHelper.SpellDefinitions.GuidingBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.HealingWord,
//                                                                                    DatabaseHelper.SpellDefinitions.Heroism,
                    SpellDefinitions.HideousLaughter,
//                                                                                    DatabaseHelper.SpellDefinitions.HuntersMark,
//                                                                                    DatabaseHelper.SpellDefinitions.Identify,
//                                                                                    DatabaseHelper.SpellDefinitions.InflictWounds,
//                                                                                    DatabaseHelper.SpellDefinitions.Jump,
//                                                                                    DatabaseHelper.SpellDefinitions.Longstrider,
//                                                                                    DatabaseHelper.SpellDefinitions.MageArmor,
//                                                                                    DatabaseHelper.SpellDefinitions.MagicMissile,
                    SpellDefinitions.ProtectionFromEvilGood,
//                                                                                    DatabaseHelper.SpellDefinitions.Shield,
//                                                                                    DatabaseHelper.SpellDefinitions.ShieldOfFaith,
                    SpellDefinitions.Sleep,
                    SpellDefinitions.Thunderwave
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.AcidArrow,
//                                                                                    DatabaseHelper.SpellDefinitions.Aid,
//                                                                                    DatabaseHelper.SpellDefinitions.Barkskin,
                    SpellDefinitions.Blindness,
//                                                                                    DatabaseHelper.SpellDefinitions.Blur,
//                                                                                    DatabaseHelper.SpellDefinitions.BrandingSmite,
                    SpellDefinitions.CalmEmotions,
//?                                                                                    DatabaseHelper.SpellDefinitions.ConjureGoblinoids,
                    SpellDefinitions.Darkness,
                    SpellDefinitions.Darkvision,
//                                                                                    DatabaseHelper.SpellDefinitions.EnhanceAbility,
//                                                                                    DatabaseHelper.SpellDefinitions.FindTraps,
//                                                                                    DatabaseHelper.SpellDefinitions.FlameBlade,
//                                                                                    DatabaseHelper.SpellDefinitions.FlamingSphere,
//                                                                                    DatabaseHelper.SpellDefinitions.HeatMetal,
                    SpellDefinitions.HoldPerson,
                    SpellDefinitions.Invisibility,
                    SpellDefinitions.Knock,
//                                                                                    DatabaseHelper.SpellDefinitions.LesserRestoration,
                    SpellDefinitions.Levitate,
//                                                                                    DatabaseHelper.SpellDefinitions.MagicWeapon,
//                                                                                    DatabaseHelper.SpellDefinitions.MirrorImage,
                    SpellDefinitions.MistyStep,
//                                                                                    DatabaseHelper.SpellDefinitions.MoonBeam,
//                                                                                    DatabaseHelper.SpellDefinitions.PassWithoutTrace,
//                                                                                    DatabaseHelper.SpellDefinitions.PrayerOfHealing,
//                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison,
                    SpellDefinitions.RayOfEnfeeblement,
//                                                                                    DatabaseHelper.SpellDefinitions.ScorchingRay,
                    SpellDefinitions.SeeInvisibility,
                    SpellDefinitions.Shatter,
//                                                                                    DatabaseHelper.SpellDefinitions.Silence,
                    SpellDefinitions.SpiderClimb
                        //                                                                                    DatabaseHelper.SpellDefinitions.SpikeGrowth,
                        //                                                                                    DatabaseHelper.SpellDefinitions.SpiritualWeapon,
                        //                                                                                    DatabaseHelper.SpellDefinitions.WardingBond
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.AnimateDead,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,
//                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,
                    SpellDefinitions.BestowCurse,
//                                                                                    DatabaseHelper.SpellDefinitions.CallLightning,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureAnimals,
                    SpellDefinitions.Counterspell,
//                                                                                    DatabaseHelper.SpellDefinitions.CreateFood,
//                                                                                    DatabaseHelper.SpellDefinitions.Daylight,
                    SpellDefinitions.DispelMagic,
                    SpellDefinitions.Fear,
//                                                                                    DatabaseHelper.SpellDefinitions.Fireball,
                    SpellDefinitions.Fly,
//                                                                                    DatabaseHelper.SpellDefinitions.Haste,
                    SpellDefinitions.HypnoticPattern,
//                                                                                    DatabaseHelper.SpellDefinitions.LightningBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.MassHealingWord,
//                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromEnergy,
                    SpellDefinitions.RemoveCurse,
//                                                                                    DatabaseHelper.SpellDefinitions.Revivify,
//                                                                                    DatabaseHelper.SpellDefinitions.SleetStorm,
                    SpellDefinitions.Slow,
//                                                                                    DatabaseHelper.SpellDefinitions.SpiritGuardians,
                    SpellDefinitions.StinkingCloud,
                    SpellDefinitions.Tongues
                        //                                                                                    DatabaseHelper.SpellDefinitions.VampiricTouch,
                        //                                                                                    DatabaseHelper.SpellDefinitions.WaterBreathing,
                        //                                                                                    DatabaseHelper.SpellDefinitions.WaterWalk,
                        //                                                                                    DatabaseHelper.SpellDefinitions.WindWall
                    },
                    new List<SpellDefinition>
                    {
                    SpellDefinitions.Banishment,
                    SpellDefinitions.BlackTentacles,
//                                                                                    DatabaseHelper.SpellDefinitions.Blight,
                    SpellDefinitions.Confusion,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureMinorElementals,
//                                                                                    DatabaseHelper.SpellDefinitions.DeathWard,
                    SpellDefinitions.DimensionDoor,
                    SpellDefinitions.DominateBeast,
//                                                                                    DatabaseHelper.SpellDefinitions.FireShield,
//                                                                                    DatabaseHelper.SpellDefinitions.FreedomOfMovement,
//                                                                                    DatabaseHelper.SpellDefinitions.GiantInsect,
                    SpellDefinitions.GreaterInvisibility,
//                                                                                    DatabaseHelper.SpellDefinitions.GuardianOfFaith,
//                                                                                    DatabaseHelper.SpellDefinitions.IceStorm,
//?                                                                                    DatabaseHelper.SpellDefinitions.IdentifyCreatures,
                    SpellDefinitions.PhantasmalKiller
                        //                                                                                    DatabaseHelper.SpellDefinitions.Stoneskin,
                        //                                                                                    DatabaseHelper.SpellDefinitions.WallOfFire
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.CloudKill,
//                                                                                    DatabaseHelper.SpellDefinitions.ConeOfCold,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureElemental,
                    SpellDefinitions.Contagion,
                    SpellDefinitions.DispelEvilAndGood,
                    SpellDefinitions.DominatePerson,
//                                                                                    DatabaseHelper.SpellDefinitions.FlameStrike,
//                                                                                    DatabaseHelper.SpellDefinitions.GreaterRestoration,
                    SpellDefinitions.HoldMonster,
                        //                                                                                    DatabaseHelper.SpellDefinitions.InsectPlague,
                        //                                                                                    DatabaseHelper.SpellDefinitions.MassCureWounds,
                        //?                                                                                    DatabaseHelper.SpellDefinitions.MindTwist,
                        //                                                                                    DatabaseHelper.SpellDefinitions.RaiseDead,
                        //                                                                                    DatabaseHelper.SpellDefinitions.WallOfForce
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.BladeBarrier,
//                                                                                    DatabaseHelper.SpellDefinitions.ChainLightning,
//                                                                                    DatabaseHelper.SpellDefinitions.CircleOfDeath,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureFey,
//                                                                                    DatabaseHelper.SpellDefinitions.Disintegrate,
                    SpellDefinitions.Eyebite,
//                                                                                    DatabaseHelper.SpellDefinitions.FreezingSphere,
//                                                                                    DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability,
//                                                                                    DatabaseHelper.SpellDefinitions.Harm,
//                                                                                    DatabaseHelper.SpellDefinitions.Heal,
//                                                                                    DatabaseHelper.SpellDefinitions.HeroesFeast,
//                                                                                    DatabaseHelper.SpellDefinitions.Sunbeam,
                    SpellDefinitions.TrueSeeing
                        //                                                                                    DatabaseHelper.SpellDefinitions.WallOfThorns
                        //                                                                                },
                        //                                                                                new List<SpellDefinition>
                        //                                                                                {
                        //                                                                                    DatabaseHelper.SpellDefinitions.Resurrection
                    });

            // How to check if additional spells are enabled? For now, I check if it exists in the DB
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("EldritchOrb", out SpellDefinition eldritchOrb))
            {
                classSpellList.SpellsByLevel[eldritchOrb.SpellLevel].Spells.Add(eldritchOrb);
            }
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("Frenzy", out SpellDefinition frenzy))
            {
                classSpellList.SpellsByLevel[frenzy.SpellLevel].Spells.Add(frenzy);
            }
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("MinorLifesteal", out SpellDefinition minorLifesteal))
            {
                classSpellList.SpellsByLevel[minorLifesteal.SpellLevel].Spells.Add(minorLifesteal);
            }
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("PetalStorm", out SpellDefinition petalStorm))
            {
                classSpellList.SpellsByLevel[petalStorm.SpellLevel].Spells.Add(petalStorm);
            }
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("ProtectThreshold", out SpellDefinition protectThreshold))
            {
                classSpellList.SpellsByLevel[protectThreshold.SpellLevel].Spells.Add(protectThreshold);
            }

            // Build our spellCast object containing previously created spell list
            var classSpellCast = new FeatureDefinitionCastSpellBuilder(
                    "CastSpellWitch",
                    GuidHelper.Create(WITCH_BASE_GUID, "CastSpellWitch").ToString());

            classSpellCast.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Class/&WitchSpellcastingTitle",
                            "Class/&WitchSpellcastingDescription")
                            .Build());
            classSpellCast.SetKnownCantrips(new List<int>{
                4, 4, 4, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6});
            classSpellCast.SetKnownSpells(new List<int>{
                2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15});

            List<SlotsByLevelDuplet> witchCastingSlots = new List<SlotsByLevelDuplet>{
                new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 01 },
                new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0,0,0,0,0}, Level = 02 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0,0,0,0,0}, Level = 03 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0,0,0,0,0}, Level = 04 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0,0,0,0,0}, Level = 05 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0,0,0,0,0}, Level = 06 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0,0,0,0,0}, Level = 07 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0,0,0,0,0}, Level = 08 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0,0,0,0,0}, Level = 09 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0,0,0,0,0}, Level = 10 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 11 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 12 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 13 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 14 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 15 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 16 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,1,0}, Level = 17 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,1,1,1,1,0}, Level = 18 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,1,1,1,0}, Level = 19 },
                new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,2,1,1,0}, Level = 20 },};

            classSpellCast.SetSlotsPerLevel(witchCastingSlots);
            classSpellCast.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            classSpellCast.SetSpellCastingOrigin(CastingOrigin.Class);
            classSpellCast.SetSpellCastingAbility(AttributeDefinitions.Charisma);
            classSpellCast.SetSpellCastingLevel(9);
            classSpellCast.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection);
            classSpellCast.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            classSpellCast.SetSpellList(classSpellList);

            FeatureDefinitionCastSpellWitch = classSpellCast.AddToDB();

            // Waiting for addition of the interface to change replaced spells. Until then, assign directly. (??)
            FeatureDefinitionCastSpellWitch.ReplacedSpells.SetRange(SpellsHelper.FullCasterReplacedSpells);
        }

        private static void BuildRitualCasting()
        {
            FeatureDefinitionFeatureSetRitualCasting = new FeatureDefinitionFeatureSetBuilder(
                    FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetRitualCasting",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetRitualCasting").ToString(),
                    new GuiPresentationBuilder(
                        "Class/&WitchFeatureSetRitualCastingTitle",
                        "Class/&WitchFeatureSetRitualCastingDescription").Build())
                    .ClearFeatures()
                    .AddFeature(new FeatureDefinitionMagicAffinityBuilder(
                            "WitchRitualCastingMagicAffinity",
                            GuidHelper.Create(WITCH_BASE_GUID, "WitchRitualCastingMagicAffinity").ToString(),
                            new GuiPresentationBuilder(
                                    "Class/&WitchRitualCastingMagicAffinityTitle",
                                    "Class/&WitchRitualCastingMagicAffinityDescription").Build())
                            .SetRitualCasting((RuleDefinitions.RitualCasting)ExtraRitualCasting.Known).AddToDB())
                    .AddFeature(FeatureDefinitionActionAffinitys.ActionAffinityWizardRitualCasting)
                    .AddToDB();
        }

        private static void BuildWitchCurses()
        {
            // Legend: 
            // +: implemented 
            // -: will not implement
            // ?: maybe? not sure how

            //+ Burned: Produce Flame + resist fire
            //- Drowned: Can breathe water, swimming speed is walking speed
            //? Feral: Survival Skill + Natural Armor 12 AC + Dex (no armor no shield)
            //? Hideous: Intimidation skill + at initiative, scare 1 humanoid with wisdom save or frightened until end of turn
            //? Hollow: When you or familiar have killing blow, gain witch level + CHA mod in temp hp
            //? Infested: immune to disease + familiar can be swarm of rats at 2nd level + swarm of insects at lvl 7
            //+ Loveless: immune to charm
            //? Possessed: learn an additional witch spell of level you have spell slots at 1,4,8,12 
            //? Starving: no need to eat (?), immune to poison
            //+ Visions: Add CHA to initiative, on top of DEX
            //- Whispers: can communicate telepathically 30 feet

            var burnedFireRes = new FeatureDefinitionDamageAffinityBuilder(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                "WitchBurnedFireResistance", WITCH_BASE_GUID, ClassCategory);

            var burnedProduceFlame = new FeatureDefinitionBonusCantripsBuilder(
                FeatureDefinitionBonusCantripss.BonusCantripsDomainElementaFire,
                "WitchBurnedProduceFlame", WITCH_BASE_GUID, ClassCategory)
                    .ClearBonusCantrips()
                    .AddBonusCantrip(SpellDefinitions.ProduceFlame);

            var burnedCurse = new FeatureDefinitionFeatureSetBuilder(
                FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                "WitchFeatureSetBurnedCurse", WITCH_BASE_GUID, ClassCategory)
                    .ClearFeatures()
                    .AddFeature(burnedFireRes.AddToDB())
                    .AddFeature(burnedProduceFlame.AddToDB())
                    .AddToDB();

            var lovelessCharmImmunity = new FeatureDefinitionConditionAffinityBuilder(
                FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                "WitchLovelessCharmImmunity", WITCH_BASE_GUID, ClassCategory);

            var lovelessCurse = new FeatureDefinitionFeatureSetBuilder(
                FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                "WitchFeatureSetLovelessCurse", WITCH_BASE_GUID, ClassCategory)
                    .ClearFeatures()
                    .AddFeature(lovelessCharmImmunity.AddToDB())
                    .AddToDB();

            // NOTE: I have no idea how to apply a Charisma bonus, so setting the initiative bonus to 3. It seems like only the "Additive" operation works
            var visionsInitiative = new FeatureDefinitionAttributeModifierBuilder(
                    "WitchVisionsInitiative",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchVisionsInitiative").ToString(),
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.Initiative,
                    3,
                    new GuiPresentationBuilder(
                            "Class/&WitchVisionsInitiativeTitle",
                            "Class/&WitchVisionsInitiativeDescription").Build())
                    .SetModifierAbilityScore(AttributeDefinitions.Charisma);

            var visionsCurse = new FeatureDefinitionFeatureSetBuilder(
                    FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetVisionsCurse",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetVisionsCurse").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetVisionsCurseTitle",
                            "Class/&WitchFeatureSetVisionsCurseDescription").Build())
                    .ClearFeatures()
                    .AddFeature(visionsInitiative.AddToDB())
                    .AddToDB();

            FeatureDefinitionFeatureSetWitchCurses = new FeatureDefinitionFeatureSetBuilder(
                    FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetWitchCurse",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetWitchCurse").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetWitchCurseTitle",
                            "Class/&WitchFeatureSetWitchCurseDescription").Build())
                    .ClearFeatures()
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                    .SetUniqueChoices(true)
                    .AddFeature(burnedCurse)
                    .AddFeature(lovelessCurse)
                    .AddFeature(visionsCurse)
                    .AddToDB();
        }

        private static void BuildMaledictions()
        {
            // Maledictions are actions unless mentioned otherwise
            // If a Malediction calls for an attack roll or saving throw, it uses your spell attack bonus or spell save DC, unless mentioned otherwise. 
            // All Maledictions require verbal or somatic components
            // If a Malediction lasts for a duration, i.e. until end of next turn (unless mentioned otherwise), you concentrate on it as you would a spell.
            // You can concentrate on a Malediction and a spell at the same time, and you make only one Constitution saving throw to maintain your concentration on both.

            // Maledictions progression is as follows:
            // lvl 1:  2
            // lvl 2:  3
            // lvl 5:  4
            // lvl 9:  5
            // lvl 13: 6
            // lvl 17: 7

            // Legend: 
            // +: implemented 

            //+ Abate: 60 feet CHA save, no reactions on fail
            //+ Apathy: 60 feet CHA save, calm emotions effect on fail
            // Beckon Familiar: summon familiar as an action, cooldown of 1 minute (how to implement cooldowns??)
            // Bleeding: 60 feet CON save, applies a Hex-like effect of 1d4 extra damage on any damage on fail
            //+ Charm: 60 feet WIS save, Charm Person/Monster effect on fail
            // Dire Familiar: for 1 minute, Familiar gains double witch level in hp and CHA mod bonus on dmg rolls 
            //                can cast other maledictions while active -> This should be a power or lvl 7 Improved Familiar feature?
            // Disorient: 60 feet CON save, -1d6 on attack rolls on fail
            // Doomward: 60 feet friendly creature, Death Ward effect, cannot target that creature again until short or long rest (i.e. add a Doomward fatigue debuff?)
            // Duplicity: single mirror image effect, odd roll the iamge image abosrbs the hit and disappears
            //+ Evil Eye: 60 feet WIS save, frightened on fail
            // Fortune: 60 feet friendly creature other than you gets adv. on saving throws
            // Go Unseen: you and familiar become invisible, 1 minute cooldown (how to implement cooldowns??)
            // Hobble: 60 feet STR save, speed reduced TO 10 feet (not BY 10 feet) on fail. Flying creatures fall
            // Knowing: too complicated... basically, you get one level of Creature lore/encyclopedia
            //? Mire: 30 feet radius centered on where you cast becomes difficult terrain, you have Land Stride effect
            // Misfortune: 60 feet, 1 creature, if it rolls a 20 on a d20, it becomes a 1. No saving throw
            //+ Obfuscate: 20 feet radius Fog Cloud centered on you
            // Peacebond: no weapons in 30 feet radius, STR check to free -> incapacitated if weapon?
            //+ Pox: 5 feet CON save, poisoned on fail
            //+ Ruin: 60 feet CON save, -3 to AC (minimum of 10)
            // Scurry: don't bother...
            // Shriek: BONUS action, Large or less creatures in a 5 foot radius around you are pushed 5 feet, no saving throw, instant duration
            // Slumber: 60 feet WIS save, unconscious if fail. Undead, charm immune, and creatures with current HP > 5x witch level are immune
            // Slur: 60 feet CHA save, on fail, if tries to cast spell with verbal component, fails if rolls odd on d20
            // Tremors: 10 feet radius centered on you, DEX save, creatures on ground become prone if fail, instant duration
            // Ward: 60 feet 1 creature other than you, reduce damage taken by 3 for every hit

            // NEED TO MAKE IT LESS SHOCKING GRASPY
            var abateEffectDescription = new EffectDescription();
            abateEffectDescription.Copy(SpellDefinitions.ShockingGrasp.EffectDescription);
            abateEffectDescription.EffectForms.RemoveAt(0);
            abateEffectDescription.SetDurationParameter(1);
            abateEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            abateEffectDescription.SetHasSavingThrow(true);
            abateEffectDescription.SetRangeParameter(12);
            abateEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            abateEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Charisma);
            abateEffectDescription.SetTargetParameter(1);
            abateEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            var abate = new FeatureDefinitionPowerBuilder(
                    "WitchMaledictionAbate",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionAbate").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    abateEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionAbateTitle",
                            "Class/&WitchMaledictionAbateDescription").Build()
                            .SetSpriteReference(SpellDefinitions.ShockingGrasp.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            var apathyEffectDescription = new EffectDescription();
            apathyEffectDescription.Copy(SpellDefinitions.CalmEmotionsOnEnemy.EffectDescription);
            apathyEffectDescription.SetDurationParameter(1);
            apathyEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            apathyEffectDescription.SetHasSavingThrow(true);
            apathyEffectDescription.SetRangeParameter(12);
            apathyEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            apathyEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Charisma);
            apathyEffectDescription.SetTargetParameter(1);
            apathyEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            var apathy = new FeatureDefinitionPowerBuilder("WitchMaledictionApathy",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionApathy").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    apathyEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionApathyTitle",
                            "Class/&WitchMaledictionApathyDescription").Build()
                            .SetSpriteReference(SpellDefinitions.CalmEmotions.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            var charmEffectDescription = new EffectDescription();
            charmEffectDescription.Copy(SpellDefinitions.CharmPerson.EffectDescription);
            charmEffectDescription.SetDurationParameter(1);
            charmEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            charmEffectDescription.SetHasSavingThrow(true);
            charmEffectDescription.SetRangeParameter(12);
            charmEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            charmEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
            charmEffectDescription.SetTargetParameter(1);
            charmEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            var charm = new FeatureDefinitionPowerBuilder(
                    "WitchMaledictionCharm",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionCharm").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    charmEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionCharmTitle",
                            "Class/&WitchMaledictionCharmDescription").Build()
                            .SetSpriteReference(SpellDefinitions.CharmPerson.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            var evileyeEffectDescription = new EffectDescription();
            evileyeEffectDescription.Copy(SpellDefinitions.Fear.EffectDescription);
            evileyeEffectDescription.SetDurationParameter(1);
            evileyeEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            evileyeEffectDescription.SetHasSavingThrow(true);
            evileyeEffectDescription.SetRangeParameter(12);
            evileyeEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            evileyeEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
            evileyeEffectDescription.SetTargetParameter(1);
            evileyeEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            var evileye = new FeatureDefinitionPowerBuilder(
                    "WitchMaledictionEvilEye",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionEvilEye").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    evileyeEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionEvilEyeTitle",
                            "Class/&WitchMaledictionEvilEyeDescription").Build()
                            .SetSpriteReference(SpellDefinitions.Fear.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            var obfuscateEffectDescription = new EffectDescription();
            obfuscateEffectDescription.Copy(SpellDefinitions.FogCloud.EffectDescription);
            obfuscateEffectDescription.SetCanBePlacedOnCharacter(true);
            obfuscateEffectDescription.SetDurationParameter(1);
            obfuscateEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            obfuscateEffectDescription.SetRangeParameter(0);
            obfuscateEffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);

            var obfuscate = new FeatureDefinitionPowerBuilder(
                    "WitchMaledictionObfuscate",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionObfuscate").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    obfuscateEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionObfuscateTitle",
                            "Class/&WitchMaledictionObfuscateDescription").Build()
                            .SetSpriteReference(SpellDefinitions.FogCloud.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            EffectForm poxEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition
            };
            ConditionForm poxConditionForm = new ConditionForm();
            poxEffectForm.SetConditionForm(poxConditionForm);
            poxEffectForm.ConditionForm.SetConditionDefinition(ConditionDefinitions.ConditionPoisoned);

            var poxEffectDescription = new EffectDescription();
            poxEffectDescription.Copy(SpellDefinitions.PoisonSpray.EffectDescription);
            poxEffectDescription.SetDurationParameter(1);
            poxEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            poxEffectDescription.SetHasSavingThrow(true);
            poxEffectDescription.SetRangeParameter(1);
            poxEffectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
            poxEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Constitution);
            poxEffectDescription.SetTargetParameter(1);
            poxEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            poxEffectDescription.EffectForms.Clear();
            poxEffectDescription.EffectForms.Add(poxEffectForm);

            var pox = new FeatureDefinitionPowerBuilder(
                    "WitchMaledictionPox",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionPox").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    poxEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionPoxTitle",
                            "Class/&WitchMaledictionPoxDescription").Build()
                            .SetSpriteReference(SpellDefinitions.PoisonSpray.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            EffectForm ruinEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition
            };
            ConditionForm ruinConditionForm = new ConditionForm();
            ruinEffectForm.SetConditionForm(ruinConditionForm);
            ruinEffectForm.SetCreatedByCharacter(true);

            var ruinConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                    ConditionDefinitions.ConditionAcidArrowed,
                    "ConditionRuined",
                    GuidHelper.Create(WITCH_BASE_GUID, "ConditionRuined").ToString(),
                    new GuiPresentationBuilder(
                            "Condition/&RuinedTitle",
                            "Condition/&RuinedDescription")
                            .SetSpriteReference(ConditionDefinitions.ConditionAcidArrowed.GuiPresentation.SpriteReference)
                            .Build())
                    .AddToDB();
            ruinConditionDefinition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
            ruinConditionDefinition.SetDurationParameter(1);
            ruinConditionDefinition.SetDurationType(RuleDefinitions.DurationType.Round);
            ruinConditionDefinition.RecurrentEffectForms.Clear();
            ruinConditionDefinition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            ruinConditionDefinition.Features.Clear();
            ruinConditionDefinition.Features.Add(new FeatureDefinitionAttributeModifierBuilder(
                    "Ruined",
                    GuidHelper.Create(WITCH_BASE_GUID, "Ruined").ToString(),
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    "ArmorClass",
                    -3,
                    new GuiPresentationBuilder(
                            "Modifier/&RuinedTitle",
                            "Modifier/&RuinedDescription")
                            .SetSpriteReference(ConditionDefinitions.ConditionAcidArrowed.GuiPresentation.SpriteReference)
                            .Build())
                    .AddToDB());

            ruinEffectForm.ConditionForm.SetConditionDefinition(ruinConditionDefinition);

            var ruinEffectDescription = new EffectDescription();
            ruinEffectDescription.Copy(SpellDefinitions.AcidArrow.EffectDescription);
            ruinEffectDescription.SetDurationParameter(1);
            ruinEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            ruinEffectDescription.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            ruinEffectDescription.SetHasSavingThrow(true);
            ruinEffectDescription.SetRangeParameter(12);
            ruinEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            ruinEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Constitution);
            ruinEffectDescription.SetTargetParameter(1);
            ruinEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            ruinEffectDescription.EffectForms.Clear();
            ruinEffectDescription.EffectForms.Add(ruinEffectForm);

            var ruin = new FeatureDefinitionPowerBuilder(
                    "WitchMaledictionRuin",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchMaledictionRuin").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    ruinEffectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchMaledictionRuinTitle",
                            "Class/&WitchMaledictionRuinDescription").Build()
                            .SetSpriteReference(SpellDefinitions.AcidSplash.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            FeatureDefinitionFeatureSetMaledictions = new FeatureDefinitionFeatureSetBuilder(
                    FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetMaledictions",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetMaledictions").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetMaledictionsTitle",
                            "Class/&WitchFeatureSetMaledictionsDescription").Build())
                    .ClearFeatures()
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
                    .SetUniqueChoices(true)
                    .AddFeature(abate)
                    .AddFeature(apathy)
                    .AddFeature(charm)
                    .AddFeature(evileye)
                    .AddFeature(obfuscate)
                    .AddFeature(pox)
                    .AddFeature(ruin)
                    .AddToDB();
        }

        private static void BuildCackle()
        {
            // At 2nd level, you can use your bonus action to cackle. 
            // The duration of your Malediction extends by 1 round for each creature affected within 60 feet of you. 
            // Not all witches laugh maniacally when they cackle, but all cackles require a verbal component, as a spell. 
            // These range from mundane curses and insults, to the murmuring of dead languages and speaking backwards.

            var effectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition
            };
            effectForm.SetCreatedByCharacter(true);

            ConditionForm conditionForm = new ConditionForm();
            conditionForm.SetConditionDefinition(ConditionDefinitions.ConditionDeafened);
            effectForm.SetConditionForm(conditionForm);

            //Add to our new effect
            var effectDescription = new EffectDescription();
            effectDescription.Copy(SpellDefinitions.HideousLaughter.EffectDescription);
            effectDescription.SetDurationParameter(1);
            effectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            effectDescription.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effectDescription.SetHasSavingThrow(false);
            effectDescription.SetRangeType(RuleDefinitions.RangeType.Self);
            // Target by tag?
            //            effectDescription.SetTargetFilteringTag(RuleDefinitions.TargetFilteringTag.CursedByMalediction);
            effectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
            effectDescription.SetTargetParameter(12);
            effectDescription.EffectForms.Clear();
            // Can we add a Condition dynamically? i.e. can we detect what kind of condition a creature has, and then add that condition here?
            // And/or can we add a new "CackleCondition" which gets evaluated at end of turn and would search for any 
            // RuleDefinitions.TargetFilteringTag.CursedByMalediction condition on the creature and reapply the condition?
            effectDescription.EffectForms.Add(effectForm);

            FeatureDefinitionPowerCackle = new FeatureDefinitionPowerBuilder(
                    "WitchCacklePower",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchCacklePower").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.BonusAction,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    true,
                    AttributeDefinitions.Charisma,
                    effectDescription,
                    new GuiPresentationBuilder(
                            "Class/&WitchCacklePowerTitle",
                            "Class/&WitchCacklePowerDescription").Build()
                            .SetSpriteReference(SpellDefinitions.HideousLaughter.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();
        }

        private static CharacterClassDefinition BuildAndAddClass()
        {
            var classGuiPresentation = new GuiPresentationBuilder(
                    "Class/&WitchTitle",
                    "Class/&WitchDescription")
                    .SetSpriteReference(CharacterClassDefinitions.Sorcerer.GuiPresentation.SpriteReference)
                    .Build();

            var classBuilder = new CharacterClassDefinitionBuilder(
                    "ClassWitch",
                    GuidHelper.Create(WITCH_BASE_GUID, "ClassWitch").ToString())
                    .SetGuiPresentation(classGuiPresentation);

            BuildClassStats(classBuilder);
            BuildEquipment(classBuilder);
            BuildProficiencies();
            BuildSpells();
            BuildRitualCasting();
            BuildWitchCurses();
            BuildMaledictions();
            BuildCackle();

            var witch = classBuilder.AddToDB();

            BuildWitchFamiliar();
            BuildSubclasses();
            BuildProgression();

            return witch;

            void BuildWitchFamiliar()
            {
                var witchFamiliarAttackIteration = new MonsterAttackIteration(MonsterAttackDefinitions.Attack_EagleMatriarch_Talons, 1);
                // We remove the inherent bonus as we will be using the Witch's spell attack bonus
                witchFamiliarAttackIteration.MonsterAttackDefinition.SetToHitBonus(0);
                witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
                witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D1);
                witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(0);

                var witchFamiliarMonsterBuilder = new MonsterDefinitionBuilder(
                        MonsterDefinitions.Eagle_Matriarch,
                        "WitchOwl",
                        GuidHelper.Create(WITCH_BASE_GUID, "WitchOwl").ToString(),
                        "Owl",
                        "Owl")
                        .ClearFeatures()
                        .AddFeatures(
                            FeatureDefinitionSenses.SenseNormalVision,
                            FeatureDefinitionSenses.SenseDarkvision24,
                            FeatureDefinitionMoveModes.MoveModeMove2,
                            FeatureDefinitionMoveModes.MoveModeFly12,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing,
                            FeatureDefinitionCombatAffinitys.CombatAffinityFlyby,
                            FeatureDefinitionMovementAffinitys.MovementAffinityNoClimb,
                            FeatureDefinitionMovementAffinitys.MovementAffinityNoSpecialMoves,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
                        .ClearAttackIterations()
                        .AddAttackIterations(new List<MonsterAttackIteration>{
                        witchFamiliarAttackIteration})
                        .ClearSkillScores()
                        .AddSkillScores(
                            (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                            (DatabaseHelper.SkillDefinitions.Stealth.Name, 3))
                        .SetArmorClass(11)
                        .SetAbilityScores(3, 13, 8, 2, 12, 7)
                        .SetHitDiceNumber(1)
                        .SetHitDiceType(RuleDefinitions.DieType.D4)
                        .SetHitPointsBonus(-1)
                        .SetStandardHitPoints(1)
                        .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
                        .SetAlignment(AlignmentDefinitions.Neutral.Name)
                        .SetCharacterFamily(CharacterFamilyDefinitions.Fey.name)
                        .SetChallengeRating(0)
                        .SetDroppedLootDefinition(null)
                        .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions)
                        .SetFullyControlledWhenAllied(true)
                        .SetDefaultFaction("Party")
                        .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

                if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out FeatureDefinition help))
                {
                    witchFamiliarMonsterBuilder.AddFeatures(new List<FeatureDefinition> { help });
                }

                var witchFamiliarMonster = witchFamiliarMonsterBuilder.AddToDB();
                witchFamiliarMonster.CreatureTags.Add("WitchFamiliar");

                var spellBuilder = new SpellDefinitionBuilder(
                        SpellDefinitions.Fireball,
                        "WitchFamiliar",
                        GuidHelper.Create(WITCH_BASE_GUID, "WitchFamiliar").ToString());

                spellBuilder.SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration);
                spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.None);
                spellBuilder.SetSomaticComponent(true);
                spellBuilder.SetVerboseComponent(true);
                spellBuilder.SetSpellLevel(1);
                spellBuilder.SetCastingTime(RuleDefinitions.ActivationTime.Hours1);
                // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
                spellBuilder.SetRitualCasting(RuleDefinitions.ActivationTime.Hours1);
                spellBuilder.SetGuiPresentation(
                        new GuiPresentationBuilder(
                                "Spell/&WitchFamiliarTitle",
                                "Spell/&WitchFamiliarDescription").Build()
                                .SetSpriteReference(SpellDefinitions.AnimalFriendship.GuiPresentation.SpriteReference));

                var spell = spellBuilder.AddToDB();

                spell.SetUniqueInstance(true);

                spell.EffectDescription.Copy(SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
                spell.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                spell.EffectDescription.SetRangeParameter(2);
                spell.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Permanent);
                spell.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                spell.EffectDescription.EffectForms.Clear();

                var summonForm = new SummonForm();
                summonForm.SetMonsterDefinitionName(witchFamiliarMonster.name);
                summonForm.SetDecisionPackage(null);

                var effectForm = new EffectForm();
                effectForm.SetFormType(EffectForm.EffectFormType.Summon);
                effectForm.SetCreatedByCharacter(true);
                effectForm.SetSummonForm(summonForm);

                spell.EffectDescription.EffectForms.Add(effectForm);

                var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                        "WitchFamiliarAutoPreparedSpell",
                        GuidHelper.Create(WITCH_BASE_GUID, "WitchFamiliarAutoPreparedSpell").ToString(),
                        new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>{
                        FeatureDefinitionAutoPreparedSpellsBuilder.BuildAutoPreparedSpellGroup(
                                2,
                                new List<SpellDefinition>{spell})},
                        new GuiPresentationBuilder(
                                "Class/&WitchFamiliarPowerTitle",
                                "Class/&WitchFamiliarPowerDescription").Build()
                                .SetSpriteReference(SpellDefinitions.AnimalFriendship.GuiPresentation.SpriteReference))
                        .SetCharacterClass(witch)
                        .SetAutoTag("Witch")
                        .AddToDB();

                var summoningAffinity = new FeatureDefinitionSummoningAffinityBuilder(
                        FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond,
                        "SummoningAffinityWitchFamiliar",
                        GuidHelper.Create(WITCH_BASE_GUID, "SummoningAffinityWitchFamiliar").ToString())
                        .AddToDB();

                summoningAffinity.SetRequiredMonsterTag("WitchFamiliar");
                summoningAffinity.EffectForms.Clear();

                var blank = GuiPresentationBuilder.NoContent;

                var acConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                        ConditionDefinitions.ConditionKindredSpiritBondAC, "ConditionWitchFamiliarAC", WITCH_BASE_GUID, blank)
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                    .AddToDB();

                var stConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                        ConditionDefinitions.ConditionKindredSpiritBondSavingThrows, "ConditionWitchFamiliarST", WITCH_BASE_GUID, blank)
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                    .AddToDB();

                var damageConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                        ConditionDefinitions.ConditionKindredSpiritBondMeleeDamage, "ConditionWitchFamiliarDamage", WITCH_BASE_GUID, blank)
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                    .AddToDB();

                var hitConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                        ConditionDefinitions.ConditionKindredSpiritBondMeleeAttack, "ConditionWitchFamiliarHit", WITCH_BASE_GUID, blank)
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceSpellAttack)
                    .AddToDB();

                var hpConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(
                        ConditionDefinitions.ConditionKindredSpiritBondHP, "ConditionWitchFamiliarHP", WITCH_BASE_GUID, blank)
                    .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
                    .AddToDB();

                hpConditionDefinition.SetAllowMultipleInstances(true);

                // Find a better place to put this in?
                hpConditionDefinition.SetAdditionalDamageType("ClassWitch");

                summoningAffinity.AddedConditions.SetRange(
                    acConditionDefinition, stConditionDefinition, damageConditionDefinition,
                    hitConditionDefinition, hpConditionDefinition, hpConditionDefinition);

                FeatureDefinitionFeatureSetWitchFamiliar = new FeatureDefinitionFeatureSetBuilder(
                        FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                        "FeatureSetWitchFamiliar",
                        GuidHelper.Create(WITCH_BASE_GUID, "FeatureSetWitchFamiliar").ToString(),
                        new GuiPresentationBuilder(
                                "Class/&WitchFamiliarPowerTitle",
                                "Class/&WitchFamiliarPowerDescription").Build())
                        .ClearFeatures()
                        .AddFeature(preparedSpells)
                        .AddFeature(summoningAffinity)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(true)
                        .AddToDB();
            }

            void BuildSubclasses()
            {
                var subClassChoices = classBuilder.BuildSubclassChoice(
                        3,
                        "Coven",
                        false,
                        "SubclassChoiceWitchCovens",
                        new GuiPresentationBuilder(
                                "Subclass/&WitchSubclassPathTitle",
                                "Subclass/&WitchSubclassPathDescription")
                                .Build(),
                        GuidHelper.Create(WITCH_BASE_GUID, "SubclassChoiceWitchCovens").ToString());

                //            subClassChoices.Subclasses.Add(new BloodWitch().GetSubclass(classDef).name);
                subClassChoices.Subclasses.Add(new GreenWitch().GetSubclass(witch).name);
                //            subClassChoices.Subclasses.Add(new PurpleWitch().GetSubclass(classDef).name);
                subClassChoices.Subclasses.Add(new RedWitch().GetSubclass(witch).name);
                subClassChoices.Subclasses.Add(new WhiteWitch().GetSubclass(witch).name);
            }

            void BuildProgression()
            {
                if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out FeatureDefinition help))
                {
                    classBuilder.AddFeatureAtLevel(help, 1);
                }

                classBuilder.AddFeaturesAtLevel(1,
                    FeatureDefinitionProficiencyArmor,
                    FeatureDefinitionProficiencyWeapon,
                    FeatureDefinitionProficiencySavingThrow,
                    FeatureDefinitionPointPoolSkills,
                    FeatureDefinitionPointPoolTools,
                    FeatureDefinitionCastSpellWitch,
                    FeatureDefinitionFeatureSetRitualCasting,
                    FeatureDefinitionFeatureSetWitchCurses,
                    FeatureDefinitionFeatureSetMaledictions,
                    FeatureDefinitionFeatureSetMaledictions);

                classBuilder.AddFeaturesAtLevel(2,
                    FeatureDefinitionPowerCackle,
                    FeatureDefinitionFeatureSetWitchFamiliar,
                    FeatureDefinitionFeatureSetMaledictions);

                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 5);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 9);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 13);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 17);
                classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19);

                // TODO: Maledictions should now apply a debuff for disadvantage on saving throw like Force Of Law
                //            witch.AddFeatureAtLevel(InsidiousSpell,5);

                // TODO: Simply buff the familiar accordingly, i.e. offer more forms, and if that is too hard, 
                // apply proficiency bonus on hit, or
                // extra attack to the familiar
                //            witch.AddFeatureAtLevel(ImprovedFamiliar,7);

                // Maybe change this... not sure what to do... is there an OnDeath event or something?
                //            witch.AddFeatureAtLevel(DyingCurse,9);

                // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
                //            witch.AddFeatureAtLevel(GreaterMalediction,11);

                // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
                //            witch.AddFeatureAtLevel(GreaterMalediction,13);

                // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
                //            witch.AddFeatureAtLevel(GreaterMalediction,15);

                // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
                //            witch.AddFeatureAtLevel(GreaterMalediction,18);
                // TODO: Another drop down list like Circle of the Land Druid
                //            witch.AddFeatureAtLevel(AbsoluteMalediction,20);
            }
        }
    }
}
