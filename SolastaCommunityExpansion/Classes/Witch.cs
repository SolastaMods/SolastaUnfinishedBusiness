using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Features;
using SolastaCommunityExpansion.Helpers;
using SolastaCommunityExpansion.Subclasses.Witch;
using UnityEngine;
using static CharacterClassDefinition;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch
{
    internal class Witch : AbstractClass
    {

        internal override CharacterClassDefinition GetClass()
        {
            if (Class == null)
            {
                Class = BuildAndAddClass();
            }
            return Class;
        }
        public static readonly Guid WITCH_BASE_GUID = new Guid("ea7715dd-00cb-45a3-a8c4-458d0639d72c");
        public CharacterClassDefinition Class;
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

        internal override void BuildClassStats(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard);
            classBuilder.SetPictogram(DatabaseHelper.CharacterClassDefinitions.Sorcerer.ClassPictogramReference);
            classBuilder.SetBattleAI(DatabaseHelper.CharacterClassDefinitions.Sorcerer.DefaultBattleDecisions);
            classBuilder.SetHitDice(RuleDefinitions.DieType.D8);
            classBuilder.SetIngredientGatheringOdds(DatabaseHelper.CharacterClassDefinitions.Sorcerer.IngredientGatheringOdds);

            classBuilder.SetAbilityScorePriorities(
                    AttributeDefinitions.Charisma,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Strength);

            classBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.PowerfulCantrip);

            classBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.HerbalismKitType);
            classBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.PoisonersKitType);

            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Arcana);
            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Deception);
            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Insight);
            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Intimidation);
            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Persuasion);
            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Nature);
            classBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Religion);
        }

        internal override void BuildEquipment(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ScholarPack, EquipmentDefinitions.OptionStarterPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionArcaneFocusChoice, 1),
                });

            classBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.SorcererArmor, EquipmentDefinitions.OptionArmor, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeapon, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });
        }

        internal override void BuildProficiencies(CharacterClassDefinitionBuilder classBuilder)
        {
            FeatureDefinitionProficiencyArmor = new FeatureDefinitionProficiencyBuilder(
                    "ProficiencyWitchArmor",
                    GuidHelper.Create(WITCH_BASE_GUID, "ProficiencyWitchArmor").ToString(),
                    RuleDefinitions.ProficiencyType.Armor,
                    new List<string>() { EquipmentDefinitions.LightArmorCategory },
                    new GuiPresentationBuilder(
                            "Class/&WitchArmorProficiencyDescription",
                            "Class/&WitchArmorProficiencyTitle")
                            .Build())
                    .AddToDB();

            FeatureDefinitionProficiencyWeapon = new FeatureDefinitionProficiencyBuilder(
                    "ProficiencyWitchWeapon",
                    GuidHelper.Create(WITCH_BASE_GUID, "ProficiencyWitchWeapon").ToString(),
                    RuleDefinitions.ProficiencyType.Weapon,
                    new List<string>() { EquipmentDefinitions.SimpleWeaponCategory },
                    new GuiPresentationBuilder(
                            "Class/&WitchWeaponProficiencyDescription",
                            "Class/&WitchWeaponProficiencyTitle")
                            .Build())
                    .AddToDB();

            FeatureDefinitionProficiencySavingThrow = new FeatureDefinitionProficiencyBuilder(
                    "ProficiencyWitchSavinghrow",
                    GuidHelper.Create(WITCH_BASE_GUID, "ProficiencyWitchSavingthrow").ToString(),
                    RuleDefinitions.ProficiencyType.SavingThrow,
                    new List<string>() { AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom },
                    new GuiPresentationBuilder(
                            "Class/&WitchSavingthrowProficiencyDescription",
                            "Class/&WitchSavingthrowProficiencyTitle")
                            .Build())
                    .AddToDB();

            FeatureDefinitionPointPoolSkills = new FeatureDefinitionPointPoolBuilder(
                    "ProficiencyWitchSkill",
                    GuidHelper.Create(WITCH_BASE_GUID, "ProficiencyWitchSkill").ToString(),
                    HeroDefinitions.PointsPoolType.Skill,
                    2,
                    new GuiPresentationBuilder(
                            "Class/&WitchSkillProficiencyDescription",
                            "Class/&WitchSkillProficiencyTitle")
                            .Build())
                    .RestrictChoices(new List<string>() {
                            SkillDefinitions.Arcana,
                            SkillDefinitions.Deception,
                            SkillDefinitions.Insight,
                            SkillDefinitions.Intimidation,
                            SkillDefinitions.Persuasion,
                            SkillDefinitions.Nature,
                            SkillDefinitions.Religion})
                    .AddToDB();

            FeatureDefinitionPointPoolTools = new FeatureDefinitionPointPoolBuilder(
                    "ProficiencyWitchTool",
                    GuidHelper.Create(WITCH_BASE_GUID, "ProficiencyWitchTool").ToString(),
                    HeroDefinitions.PointsPoolType.Tool,
                    1,
                    new GuiPresentationBuilder(
                            "Class/&WitchToolProficiencyDescription",
                            "Class/&WitchToolProficiencyTitle")
                            .Build())
                    .RestrictChoices(new List<string>() {
                            DatabaseHelper.ToolTypeDefinitions.HerbalismKitType.Name,
                            DatabaseHelper.ToolTypeDefinitions.PoisonersKitType.Name})
                    .AddToDB();
        }

        private static void BuildSpells()
        {
            // Keeping all spells listed here for ease to edit and see the big picture
            var classSpellList = SpellListBuilder.createSpellList(
                    "WitchSpellList",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchSpellList").ToString(),
                    "",
                    new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.AcidSplash,
//?                                                                                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                        DatabaseHelper.SpellDefinitions.ChillTouch,
                        DatabaseHelper.SpellDefinitions.DancingLights,
//?                                                                                    DatabaseHelper.SpellDefinitions.Dazzle,
                        //EldritchOrbSpellBuilder.EldritchOrbSpell,
//                                                                                    DatabaseHelper.SpellDefinitions.FireBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.Guidance,
//                                                                                    DatabaseHelper.SpellDefinitions.Light,
                        //MinorLifestealSpellBuilder.MinorLifestealSpell,
//                                                                                    DatabaseHelper.SpellDefinitions.PoisonSpray,
                        DatabaseHelper.SpellDefinitions.ProduceFlame,
//                                                                                    DatabaseHelper.SpellDefinitions.RayOfFrost,
                        DatabaseHelper.SpellDefinitions.Resistance,
//                                                                                    DatabaseHelper.SpellDefinitions.SacredFlame,
//?                                                                                    DatabaseHelper.SpellDefinitions.ShadowArmor,
//?                                                                                    DatabaseHelper.SpellDefinitions.ShadowDagger,
//                                                                                    DatabaseHelper.SpellDefinitions.Shillelagh,
//?                                                                                    DatabaseHelper.SpellDefinitions.Shine,
//                                                                                    DatabaseHelper.SpellDefinitions.ShockingGrasp,
                        DatabaseHelper.SpellDefinitions.SpareTheDying,
//?                                                                                    DatabaseHelper.SpellDefinitions.Sparkle,
                        DatabaseHelper.SpellDefinitions.TrueStrike
//                                                                                    DatabaseHelper.SpellDefinitions.VenomousSpike
                    },
                    new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.AnimalFriendship,
                        DatabaseHelper.SpellDefinitions.Bane,
//                                                                                    DatabaseHelper.SpellDefinitions.Bless,
//                                                                                    DatabaseHelper.SpellDefinitions.BurningHands,
                        DatabaseHelper.SpellDefinitions.CharmPerson,
//                                                                                    DatabaseHelper.SpellDefinitions.ColorSpray,
//                                                                                    DatabaseHelper.SpellDefinitions.Command,
                        DatabaseHelper.SpellDefinitions.ComprehendLanguages,
//                                                                                    DatabaseHelper.SpellDefinitions.CureWounds,
//                                                                                    DatabaseHelper.SpellDefinitions.DetectEvilAndGood,
                        DatabaseHelper.SpellDefinitions.DetectMagic,
//                                                                                    DatabaseHelper.SpellDefinitions.DetectPoisonAndDisease,
//                                                                                    DatabaseHelper.SpellDefinitions.DivineFavor,
//                                                                                    DatabaseHelper.SpellDefinitions.Entangle,
                        DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                        DatabaseHelper.SpellDefinitions.FaerieFire,
//                                                                                    DatabaseHelper.SpellDefinitions.FalseLife,
//                                                                                    DatabaseHelper.SpellDefinitions.FeatherFall,
//                                                                                    DatabaseHelper.SpellDefinitions.FogCloud,
//                                                                                    DatabaseHelper.SpellDefinitions.Goodberry,
//                                                                                    DatabaseHelper.SpellDefinitions.Grease,
//                                                                                    DatabaseHelper.SpellDefinitions.GuidingBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.HealingWord,
//                                                                                    DatabaseHelper.SpellDefinitions.Heroism,
                        DatabaseHelper.SpellDefinitions.HideousLaughter,
//                                                                                    DatabaseHelper.SpellDefinitions.HuntersMark,
//                                                                                    DatabaseHelper.SpellDefinitions.Identify,
//                                                                                    DatabaseHelper.SpellDefinitions.InflictWounds,
//                                                                                    DatabaseHelper.SpellDefinitions.Jump,
//                                                                                    DatabaseHelper.SpellDefinitions.Longstrider,
//                                                                                    DatabaseHelper.SpellDefinitions.MageArmor,
//                                                                                    DatabaseHelper.SpellDefinitions.MagicMissile,
                        DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
//                                                                                    DatabaseHelper.SpellDefinitions.Shield,
//                                                                                    DatabaseHelper.SpellDefinitions.ShieldOfFaith,
                        DatabaseHelper.SpellDefinitions.Sleep,
                        DatabaseHelper.SpellDefinitions.Thunderwave
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.AcidArrow,
//                                                                                    DatabaseHelper.SpellDefinitions.Aid,
//                                                                                    DatabaseHelper.SpellDefinitions.Barkskin,
                        DatabaseHelper.SpellDefinitions.Blindness,
//                                                                                    DatabaseHelper.SpellDefinitions.Blur,
//                                                                                    DatabaseHelper.SpellDefinitions.BrandingSmite,
                        DatabaseHelper.SpellDefinitions.CalmEmotions,
//?                                                                                    DatabaseHelper.SpellDefinitions.ConjureGoblinoids,
                        DatabaseHelper.SpellDefinitions.Darkness,
                        DatabaseHelper.SpellDefinitions.Darkvision,
//                                                                                    DatabaseHelper.SpellDefinitions.EnhanceAbility,
//                                                                                    DatabaseHelper.SpellDefinitions.FindTraps,
//                                                                                    DatabaseHelper.SpellDefinitions.FlameBlade,
//                                                                                    DatabaseHelper.SpellDefinitions.FlamingSphere,
//                                                                                    DatabaseHelper.SpellDefinitions.HeatMetal,
                        DatabaseHelper.SpellDefinitions.HoldPerson,
                        DatabaseHelper.SpellDefinitions.Invisibility,
                        DatabaseHelper.SpellDefinitions.Knock,
//                                                                                    DatabaseHelper.SpellDefinitions.LesserRestoration,
                        DatabaseHelper.SpellDefinitions.Levitate,
//                                                                                    DatabaseHelper.SpellDefinitions.MagicWeapon,
//                                                                                    DatabaseHelper.SpellDefinitions.MirrorImage,
                        DatabaseHelper.SpellDefinitions.MistyStep,
//                                                                                    DatabaseHelper.SpellDefinitions.MoonBeam,
//                                                                                    DatabaseHelper.SpellDefinitions.PassWithoutTrace,
                        //PetalStormSpellBuilder.PetalStormSpell,
//                                                                                    DatabaseHelper.SpellDefinitions.PrayerOfHealing,
//                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison,
                        //ProtectThresholdSpellBuilder.ProtectThresholdSpell,
                        DatabaseHelper.SpellDefinitions.RayOfEnfeeblement,
//                                                                                    DatabaseHelper.SpellDefinitions.ScorchingRay,
                        DatabaseHelper.SpellDefinitions.SeeInvisibility,
                        DatabaseHelper.SpellDefinitions.Shatter,
//                                                                                    DatabaseHelper.SpellDefinitions.Silence,
                        DatabaseHelper.SpellDefinitions.SpiderClimb
//                                                                                    DatabaseHelper.SpellDefinitions.SpikeGrowth,
//                                                                                    DatabaseHelper.SpellDefinitions.SpiritualWeapon,
//                                                                                    DatabaseHelper.SpellDefinitions.WardingBond
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.AnimateDead,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,
//                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,                                                                                    DatabaseHelper.SpellDefinitions.BeaconOfHope,
                        DatabaseHelper.SpellDefinitions.BestowCurse,
//                                                                                    DatabaseHelper.SpellDefinitions.CallLightning,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureAnimals,
                        DatabaseHelper.SpellDefinitions.Counterspell,
//                                                                                    DatabaseHelper.SpellDefinitions.CreateFood,
//                                                                                    DatabaseHelper.SpellDefinitions.Daylight,
                        DatabaseHelper.SpellDefinitions.DispelMagic,
                        DatabaseHelper.SpellDefinitions.Fear,
//                                                                                    DatabaseHelper.SpellDefinitions.Fireball,
                        DatabaseHelper.SpellDefinitions.Fly,
//                                                                                    DatabaseHelper.SpellDefinitions.Haste,
                        DatabaseHelper.SpellDefinitions.HypnoticPattern,
//                                                                                    DatabaseHelper.SpellDefinitions.LightningBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.MassHealingWord,
//                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromEnergy,
                        DatabaseHelper.SpellDefinitions.RemoveCurse,
//                                                                                    DatabaseHelper.SpellDefinitions.Revivify,
//                                                                                    DatabaseHelper.SpellDefinitions.SleetStorm,
                        DatabaseHelper.SpellDefinitions.Slow,
//                                                                                    DatabaseHelper.SpellDefinitions.SpiritGuardians,
                        DatabaseHelper.SpellDefinitions.StinkingCloud,
                        DatabaseHelper.SpellDefinitions.Tongues
//                                                                                    DatabaseHelper.SpellDefinitions.VampiricTouch,
//                                                                                    DatabaseHelper.SpellDefinitions.WaterBreathing,
//                                                                                    DatabaseHelper.SpellDefinitions.WaterWalk,
//                                                                                    DatabaseHelper.SpellDefinitions.WindWall
                    },
                    new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Banishment,
                        DatabaseHelper.SpellDefinitions.BlackTentacles,
//                                                                                    DatabaseHelper.SpellDefinitions.Blight,
                        DatabaseHelper.SpellDefinitions.Confusion,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureMinorElementals,
//                                                                                    DatabaseHelper.SpellDefinitions.DeathWard,
                        DatabaseHelper.SpellDefinitions.DimensionDoor,
                        DatabaseHelper.SpellDefinitions.DominateBeast,
//                                                                                    DatabaseHelper.SpellDefinitions.FireShield,
//                                                                                    DatabaseHelper.SpellDefinitions.FreedomOfMovement,
//                                                                                    DatabaseHelper.SpellDefinitions.GiantInsect,
                        DatabaseHelper.SpellDefinitions.GreaterInvisibility,
//                                                                                    DatabaseHelper.SpellDefinitions.GuardianOfFaith,
//                                                                                    DatabaseHelper.SpellDefinitions.IceStorm,
//?                                                                                    DatabaseHelper.SpellDefinitions.IdentifyCreatures,
                        DatabaseHelper.SpellDefinitions.PhantasmalKiller
//                                                                                    DatabaseHelper.SpellDefinitions.Stoneskin,
//                                                                                    DatabaseHelper.SpellDefinitions.WallOfFire
                    },
                    new List<SpellDefinition>
                    {
//                                                                                    DatabaseHelper.SpellDefinitions.CloudKill,
//                                                                                    DatabaseHelper.SpellDefinitions.ConeOfCold,
//                                                                                    DatabaseHelper.SpellDefinitions.ConjureElemental,
                        DatabaseHelper.SpellDefinitions.Contagion,
                        DatabaseHelper.SpellDefinitions.DispelEvilAndGood,
                        DatabaseHelper.SpellDefinitions.DominatePerson,
//                                                                                    DatabaseHelper.SpellDefinitions.FlameStrike,
//                                                                                    DatabaseHelper.SpellDefinitions.GreaterRestoration,
                        DatabaseHelper.SpellDefinitions.HoldMonster,
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
                        DatabaseHelper.SpellDefinitions.Eyebite,
//                                                                                    DatabaseHelper.SpellDefinitions.FreezingSphere,
                        //FrenzySpellBuilder.FrenzySpell,
//                                                                                    DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability,
//                                                                                    DatabaseHelper.SpellDefinitions.Harm,
//                                                                                    DatabaseHelper.SpellDefinitions.Heal,
//                                                                                    DatabaseHelper.SpellDefinitions.HeroesFeast,
//                                                                                    DatabaseHelper.SpellDefinitions.Sunbeam,
                        DatabaseHelper.SpellDefinitions.TrueSeeing
//                                                                                    DatabaseHelper.SpellDefinitions.WallOfThorns
//                                                                                },
//                                                                                new List<SpellDefinition>
//                                                                                {
//                                                                                    DatabaseHelper.SpellDefinitions.Resurrection
                    });

            // How to check if additional spells are enabled? For now, I check if it exists in the DB
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("EldritchOrb", out SpellDefinition eldritchOrb)){
                    classSpellList.SpellsByLevel[eldritchOrb.SpellLevel].Spells.Add(eldritchOrb);}
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("Frenzy", out SpellDefinition frenzy)){
                    classSpellList.SpellsByLevel[frenzy.SpellLevel].Spells.Add(frenzy);}
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("MinorLifesteal", out SpellDefinition minorLifesteal)){
                    classSpellList.SpellsByLevel[minorLifesteal.SpellLevel].Spells.Add(minorLifesteal);}
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("PetalStorm", out SpellDefinition petalStorm)){
                    classSpellList.SpellsByLevel[petalStorm.SpellLevel].Spells.Add(petalStorm);}
            if (DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement("ProtectThreshold", out SpellDefinition protectThreshold)){
                    classSpellList.SpellsByLevel[protectThreshold.SpellLevel].Spells.Add(protectThreshold);}

            // Build our spellCast object containing previously created spell list
            var classSpellCast = new CastSpellBuilder(
                    "WitchSpellCast", 
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchSpellCast").ToString());

            classSpellCast.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Class/&WitchSpellcastingDescription",
                            "Class/&WitchSpellcastingTitle")
                            .Build());
            classSpellCast.SetKnownCantrips(4, 1, CastSpellBuilder.CasterProgression.FULL_CASTER);
            classSpellCast.SetKnownSpells(2, 1, CastSpellBuilder.CasterProgression.FULL_CASTER);
            classSpellCast.SetSlotsPerLevel(1, CastSpellBuilder.CasterProgression.FULL_CASTER);
            classSpellCast.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            classSpellCast.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class);
            classSpellCast.SetSpellCastingAbility(AttributeDefinitions.Charisma);
            classSpellCast.SetSpellCastingLevel(9);
            classSpellCast.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection);
            classSpellCast.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            classSpellCast.SetSpellList(classSpellList);

            FeatureDefinitionCastSpellWitch = classSpellCast.AddToDB();
        }

        private static void BuildRitualCasting()
        {

            FeatureDefinitionFeatureSetRitualCasting = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetRitualCasting",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetRitualCasting").ToString(),
                    new GuiPresentationBuilder(
                        "Class/&WitchFeatureSetRitualCastingDescription",
                        "Class/&WitchFeatureSetRitualCastingTitle").Build())
                    .ClearFeatures()
                    .AddFeature(new FeatureDefinitionMagicAffinityBuilder(
                            "WitchRitualCastingMagicAffinity",
                            GuidHelper.Create(WITCH_BASE_GUID, "WitchRitualCastingMagicAffinity").ToString(),
                            new GuiPresentationBuilder(
                                    "Class/&WitchRitualCastingMagicAffinityDescription",
                                    "Class/&WitchRitualCastingMagicAffinityTitle").Build())
                            .SetRitualCasting((RuleDefinitions.RitualCasting)ExtraRitualCasting.Known).AddToDB())
                    .AddFeature(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityWizardRitualCasting)
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
                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                    "WitchBurnedFireResistance",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchBurnedFireResistance").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchBurnedFireResistanceDescription",
                            "Class/&WitchBurnedFireResistanceTitle").Build());

            var burnedProduceFlame = new FeatureDefinitionBonusCantripsBuilder(
                    DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainElementaFire,
                    "WitchBurnedProduceFlame",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchBurnedProduceFlame").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchBurnedProduceFlameDescription",
                            "Class/&WitchBurnedProduceFlameTitle").Build())
                    .ClearBonusCantrips()
                    .AddBonusCantrip(DatabaseHelper.SpellDefinitions.ProduceFlame);

            var burnedCurse = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetBurnedCurse",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetBurnedCurse").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetBurnedCurseDescription",
                            "Class/&WitchFeatureSetBurnedCurseTitle").Build())
                    .ClearFeatures()
                    .AddFeature(burnedFireRes.AddToDB())
                    .AddFeature(burnedProduceFlame.AddToDB())
                    .AddToDB();

            var lovelessCharmImmunity = new FeatureDefinitionConditionAffinityBuilder(
                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                    "WitchLovelessCharmImmunity",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchLovelessCharmImmunity").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchLovelessCharmImmunityDescription",
                            "Class/&WitchLovelessCharmImmunityTitle").Build());

            var lovelessCurse = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetLovelessCurse",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetLovelessCurse").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetLovelessCurseDescription",
                            "Class/&WitchFeatureSetLovelessCurseTitle").Build())
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
                            "Class/&WitchVisionsInitiativeDescription",
                            "Class/&WitchVisionsInitiativeTitle").Build())
                    .SetModifierAbilityScore(AttributeDefinitions.Charisma);

            var visionsCurse = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetVisionsCurse",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetVisionsCurse").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetVisionsCurseDescription",
                            "Class/&WitchFeatureSetVisionsCurseTitle").Build())
                    .ClearFeatures()
                    .AddFeature(visionsInitiative.AddToDB())
                    .AddToDB();

            FeatureDefinitionFeatureSetWitchCurses = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                    "WitchFeatureSetWitchCurse",
                    GuidHelper.Create(WITCH_BASE_GUID, "WitchFeatureSetWitchCurse").ToString(),
                    new GuiPresentationBuilder(
                            "Class/&WitchFeatureSetWitchCurseDescription",
                            "Class/&WitchFeatureSetWitchCurseTitle").Build())
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
            conditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionDeafened);
            effectForm.SetConditionForm(conditionForm);

            //Add to our new effect
            var effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.HideousLaughter.EffectDescription);
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
                            "Class/&WitchCacklePowerDescription",
                            "Class/&WitchCacklePowerTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.HideousLaughter.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

        }

        private static void BuildWitchFamiliar()
        {

        }

        internal override void BuildSubclasses(CharacterClassDefinitionBuilder classBuilder)
        {

            var subClassChoices = classBuilder.BuildSubclassChoice(
                    3,
                    "Coven", 
                    false, 
                    "SubclassChoiceWitchCovens", 
                    new GuiPresentationBuilder(
                            "Subclass/&WitchSubclassPathDescription",
                            "Subclass/&WitchSubclassPathTitle")
                            .Build(),
                    GuidHelper.Create(WITCH_BASE_GUID, "SubclassChoiceWitchCovens").ToString());

//            subClassChoices.Subclasses.Add(new BloodWitch().GetSubclass().name);
            subClassChoices.Subclasses.Add(new GreenWitch().GetSubclass(Class).name);
//            subClassChoices.Subclasses.Add(new PurpleWitch().GetSubclass().name);
        }

        internal override void BuildProgression(CharacterClassDefinitionBuilder classBuilder)
        {
            classBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencyArmor, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencyWeapon, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencySavingThrow, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionPointPoolSkills, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionPointPoolTools, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionCastSpellWitch, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetRitualCasting, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetWitchCurses, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 1);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionPowerCackle, 2);
//            classBuilder.AddFeatureAtLevel(WitchFamiliar, 2);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 2);
            classBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 5);
            classBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 9);
            classBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 13);
            classBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16);
            classBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetMaledictions, 17);
            classBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19);

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

        public CharacterClassDefinition BuildAndAddClass()
        {
            var classGuiPresentation = new GuiPresentationBuilder(
                    "Class/&WitchDescription",
                    "Class/&WitchTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterClassDefinitions.Sorcerer.GuiPresentation.SpriteReference)
                    .Build();

            var classBuilder = new CharacterClassDefinitionBuilder(
                    "Witch", 
                    GuidHelper.Create(WITCH_BASE_GUID, "Witch").ToString())
                    .SetGuiPresentation(classGuiPresentation);

            BuildClassStats(classBuilder);
            BuildEquipment(classBuilder);
            BuildProficiencies(classBuilder);
            BuildSpells();
            BuildRitualCasting();
            BuildWitchCurses();
            BuildMaledictions();
            BuildCackle();
            BuildWitchFamiliar();

            Class = classBuilder.AddToDB();

            // I have not found another way to do it like this when trying to build
            // skills or powers that require a reference to the ClassDefinition
            BuildSubclasses(classBuilder);
            BuildProgression(classBuilder);

            return Class;
        }

    }
}
