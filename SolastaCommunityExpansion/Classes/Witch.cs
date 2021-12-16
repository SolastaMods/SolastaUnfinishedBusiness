using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Helpers;

namespace SolastaCommunityExpansion.Classes.Witch
{
    internal class Witch : AbstractClass
    {
        private static Guid ClassNamespace = new Guid("ea7715dd-00cb-45a3-a8c4-458d0639d72c");
        private readonly CharacterClassDefinition Class;
        private readonly FeatureDefinitionPower familiarHelpAction;
        internal override CharacterClassDefinition GetClass()
        {
            return Class;
        }

        internal Witch()
        {
            // Make Witch Class
            CharacterClassDefinitionBuilder witch = new CharacterClassDefinitionBuilder("Witch", GuidHelper.Create(ClassNamespace, "Witch").ToString());

            //            FeatureDefinitionSubclassChoiceBuilder witchFeatureDefinitionSubclassChoiceBuilder = new FeatureDefinitionSubclassChoiceBuilder("SubclassChoiceWitchCovens", GuidHelper.Create(ClassSubclassChoice, "SubclassChoiceWitchCovens").ToString());

            var subclassChoicesGuiPresentation = new GuiPresentation
            {
                Title = "Subclass/&WitchSubclassPathTitle",
                Description = "Subclass/&WitchSubclassPathDescription"
            };
            witch.BuildSubclassChoice(3, "Coven", false, "SubclassChoiceWitchCovens", subclassChoicesGuiPresentation, GuidHelper.Create(ClassNamespace, "SubclassChoiceWitchCovens").ToString());

            var sorcerer = DatabaseHelper.CharacterClassDefinitions.Sorcerer;
            witch.SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard);
            witch.SetPictogram(sorcerer.ClassPictogramReference);
            witch.SetBattleAI(sorcerer.DefaultBattleDecisions);
            witch.SetHitDice(RuleDefinitions.DieType.D8);
            witch.SetIngredientGatheringOdds(sorcerer.IngredientGatheringOdds);

            witch.SetAbilityScorePriorities(AttributeDefinitions.Charisma,
                                            AttributeDefinitions.Constitution,
                                            AttributeDefinitions.Dexterity,
                                            AttributeDefinitions.Wisdom,
                                            AttributeDefinitions.Intelligence,
                                            AttributeDefinitions.Strength);

            witch.AddFeatPreference(DatabaseHelper.FeatDefinitions.PowerfulCantrip);

            witch.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.HerbalismKitType);
            witch.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.PoisonersKitType);

            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Arcana);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Deception);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Insight);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Intimidation);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Persuasion);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Nature);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Religion);

            witch.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Bolt, EquipmentDefinitions.OptionWeapon, 20),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Quarterstaff, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                                    }
            );
            witch.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionArcaneFocusChoice, 1),
                                    }
            );
            witch.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ScholarPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    }
            );

            witch.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.SorcererArmor, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Club, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeapon, 1),
            });

            FeatureDefinitionProficiency savingThrowProficiencies = new FeatureDefinitionProficiencyBuilder("WitchSavingthrowProficiency",
                                                                                                            GuidHelper.Create(ClassNamespace, "WitchSavingthrowProficiency").ToString(),
                                                                                                            RuleDefinitions.ProficiencyType.SavingThrow,
                                                                                                            new List<string>() { AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom },
                                                                                                            new GuiPresentationBuilder(
                                                                                                            "Class/&WitchSavingthrowProficiencyDescription",
                                                                                                            "Class/&WitchSavingthrowProficiencyTitle").Build())
                                                                                                            .AddToDB();

            FeatureDefinitionPointPool skillProficiencies = new FeatureDefinitionPointPoolBuilder("WitchSkillProficiency",
                                                                                                    GuidHelper.Create(ClassNamespace, "WitchSkillProficiency").ToString(),
                                                                                                    HeroDefinitions.PointsPoolType.Skill,
                                                                                                    2,
                                                                                                    new GuiPresentationBuilder(
                                                                                                        "Class/&WitchSkillProficiencyDescription",
                                                                                                        "Class/&WitchSkillProficiencyTitle").Build())
                                                                                                    .RestrictChoices(new List<string>() {
                                                                                                        SkillDefinitions.Arcana,
                                                                                                        SkillDefinitions.Deception,
                                                                                                        SkillDefinitions.Insight,
                                                                                                        SkillDefinitions.Intimidation,
                                                                                                        SkillDefinitions.Persuasion,
                                                                                                        SkillDefinitions.Nature,
                                                                                                        SkillDefinitions.Religion})
                                                                                                    .AddToDB();

            FeatureDefinitionProficiency weaponProficiencies = new FeatureDefinitionProficiencyBuilder("WitchWeaponProficiency",
                                                                                                        GuidHelper.Create(ClassNamespace, "WitchWeaponProficiency").ToString(),
                                                                                                        RuleDefinitions.ProficiencyType.Weapon,
                                                                                                        new List<string>() { EquipmentDefinitions.SimpleWeaponCategory },
                                                                                                        new GuiPresentationBuilder(
                                                                                                        "Class/&WitchWeaponProficiencyDescription",
                                                                                                        "Class/&WitchWeaponProficiencyTitle").Build())
                                                                                                        .AddToDB();

            FeatureDefinitionProficiency armorProficiencies = new FeatureDefinitionProficiencyBuilder("WitchArmorProficiency",
                                                                                                        GuidHelper.Create(ClassNamespace, "WitchArmorProficiency").ToString(),
                                                                                                        RuleDefinitions.ProficiencyType.Armor,
                                                                                                        new List<string>() { EquipmentDefinitions.LightArmorCategory },
                                                                                                        new GuiPresentationBuilder(
                                                                                                        "Class/&WitchArmorProficiencyDescription",
                                                                                                        "Class/&WitchArmorProficiencyTitle").Build())
                                                                                                        .AddToDB();

            FeatureDefinitionPointPool toolProficiencies = new FeatureDefinitionPointPoolBuilder("WitchToolProficiency",
                                                                                                    GuidHelper.Create(ClassNamespace, "WitchToolProficiency").ToString(),
                                                                                                    HeroDefinitions.PointsPoolType.Tool,
                                                                                                    1,
                                                                                                    new GuiPresentationBuilder(
                                                                                                        "Class/&WitchToolProficiencyDescription",
                                                                                                        "Class/&WitchToolProficiencyTitle").Build())
                                                                                                    .RestrictChoices(new List<string>() {
                                                                                                        DatabaseHelper.ToolTypeDefinitions.HerbalismKitType.Name,
                                                                                                        DatabaseHelper.ToolTypeDefinitions.PoisonersKitType.Name})
                                                                                                    .AddToDB();

            // Keeping all spells listed here for ease to edit and see the big picture
            SpellListDefinition spellList = SpellListBuilder.createSpellList("WitchSpellList",
                                                                                GuidHelper.Create(ClassNamespace, "WitchSpellList").ToString(),
                                                                                "",
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.AcidSplash,
//?                                                                                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                                                                                    DatabaseHelper.SpellDefinitions.ChillTouch,
                                                                                    DatabaseHelper.SpellDefinitions.DancingLights,
//?                                                                                    DatabaseHelper.SpellDefinitions.Dazzle,
                                                                                    EldritchOrbSpellBuilder.EldritchOrbSpell,
//                                                                                    DatabaseHelper.SpellDefinitions.FireBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.Guidance,
//                                                                                    DatabaseHelper.SpellDefinitions.Light,
                                                                                    MinorLifestealSpellBuilder.MinorLifestealSpell,
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
                                                                                    PetalStormSpellBuilder.PetalStormSpell,
//                                                                                    DatabaseHelper.SpellDefinitions.PrayerOfHealing,
//                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison,
                                                                                    ProtectThresholdSpellBuilder.ProtectThresholdSpell,
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
                                                                                    FrenzySpellBuilder.FrenzySpell,
//                                                                                    DatabaseHelper.SpellDefinitions.GlobeOfInvulnerability,
//                                                                                    DatabaseHelper.SpellDefinitions.Harm,
//                                                                                    DatabaseHelper.SpellDefinitions.Heal,
//                                                                                    DatabaseHelper.SpellDefinitions.HeroesFeast,
//                                                                                    DatabaseHelper.SpellDefinitions.Sunbeam,
                                                                                    DatabaseHelper.SpellDefinitions.TrueSeeing
//                                                                                    DatabaseHelper.SpellDefinitions.WallOfThorns
/*                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.Resurrection
*/                                                                                });

            FeatureDefinitionFeatureSet ritualCasting = new FeatureDefinitionFeatureSetBuilder(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                                                                                                "WitchFeatureSetRitualCasting",
                                                                                                GuidHelper.Create(ClassNamespace, "WitchFeatureSetRitualCasting").ToString(),
                                                                                                new GuiPresentationBuilder(
                                                                                                    "Class/&WitchFeatureSetRitualCastingDescription",
                                                                                                    "Class/&WitchFeatureSetRitualCastingTitle").Build())
                                                                                                .SetFeature(new FeatureDefinitionMagicAffinityBuilder("WitchRitualCastingMagicAffinity",
                                                                                                                                                        GuidHelper.Create(ClassNamespace, "WitchRitualCastingMagicAffinity").ToString(),
                                                                                                                                                        new GuiPresentationBuilder(
                                                                                                                                                            "Class/&WitchRitualCastingMagicAffinityDescription",
                                                                                                                                                            "Class/&WitchRitualCastingMagicAffinityTitle").Build())
                                                                                                                                                        .SetRitualCasting((RuleDefinitions.RitualCasting)ExtraRitualCasting.Known).AddToDB())
                                                                                                .AddToDB();

            CastSpellBuilder spellCasting = new CastSpellBuilder("CastSpellClassWitch",
                                                                    GuidHelper.Create(ClassNamespace, "CastSpellClassWitch").ToString())
                                                                    .SetSpellList(spellList)
                                                                    .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
                                                                    .SetSpellCastingAbility(AttributeDefinitions.Charisma)
                                                                    .SetSpellCastingLevel(9)
                                                                    .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
                                                                    .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
                                                                    .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
                                                                    .SetKnownCantrips(4, 1, CastSpellBuilder.CasterProgression.FULL_CASTER)
                                                                    .SetKnownSpells(2, 1, CastSpellBuilder.CasterProgression.FULL_CASTER)
                                                                    .SetSlotsPerLevel(1, CastSpellBuilder.CasterProgression.FULL_CASTER)
                                                                    .SetGuiPresentation(new GuiPresentationBuilder(
                                                                        "Class/&WitchSpellcastingDescription",
                                                                        "Class/&WitchSpellcastingTitle").Build());

            GuiPresentationBuilder witchPresentation = new GuiPresentationBuilder(
                "Class/&WitchDescription",
                "Class/&WitchTitle");
            witchPresentation.SetSpriteReference(DatabaseHelper.CharacterClassDefinitions.Sorcerer.GuiPresentation.SpriteReference);
            witch.SetGuiPresentation(witchPresentation.Build());

            witch.AddFeatureAtLevel(savingThrowProficiencies, 1);
            witch.AddFeatureAtLevel(skillProficiencies, 1);
            witch.AddFeatureAtLevel(weaponProficiencies, 1);
            witch.AddFeatureAtLevel(armorProficiencies, 1);
            witch.AddFeatureAtLevel(toolProficiencies, 1);

            witch.AddFeatureAtLevel(spellCasting.AddToDB(), 1);
            witch.AddFeatureAtLevel(ritualCasting, 1);

            FeatureDefinitionFeatureSet maledictions = Maledictions();

            witch.AddFeatureAtLevel(maledictions, 1);
            witch.AddFeatureAtLevel(maledictions, 1);

            // BUG: this will not work if applied at level 1, since not all data structures are built before choosing features.
            // This seems like a limitation of the way character creation is handled :(
            // Workaround is to apply this at level 2
            witch.AddFeatureAtLevel(WitchCurse(), 2);

            witch.AddFeatureAtLevel(maledictions, 2);

            // This should actually be a bit refined, i.e. give the Find Familiar spell, and have a longer casting time/ritual tag
            // Beckon Familiar is actually a Malediction, i.e. instant cast familiar
//            witch.AddFeatureAtLevel(Familiar(), 2);

            // TODO: unsure about this one...
            // Needs to refresh an existing Malediction, preventing the saving throw again I presume
            witch.AddFeatureAtLevel(Cackle(),2);

            witch.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4);

            // TODO: Maledictions should now apply a debuff for disadvantage on saving throw like Force Of Law
            //            witch.AddFeatureAtLevel(InsidiousSpell,5);

            witch.AddFeatureAtLevel(maledictions, 5);

            // TODO: Simply buff the familiar accordingly, i.e. offer more forms, and if that is too hard, 
            // apply proficiency bonus on hit, or
            // extra attack to the familiar
            //            witch.AddFeatureAtLevel(ImprovedFamiliar,7);

            witch.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8);

            // Maybe change this... not sure what to do... is there an OnDeath event or something?
            //            witch.AddFeatureAtLevel(DyingCurse,9);

            witch.AddFeatureAtLevel(maledictions, 9);

            // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
            //            witch.AddFeatureAtLevel(GreaterMalediction,11);

            witch.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12);

            // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
            //            witch.AddFeatureAtLevel(GreaterMalediction,13);

            witch.AddFeatureAtLevel(maledictions, 13);

            // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
            //            witch.AddFeatureAtLevel(GreaterMalediction,15);

            witch.AddFeatureAtLevel(maledictions, 17);

            // TODO: Another set of Maledictions, but stronger, and again follow the Tinkerer infusions pattern
            //            witch.AddFeatureAtLevel(GreaterMalediction,18);
            // TODO: Another drop down list like Circle of the Land Druid
            //            witch.AddFeatureAtLevel(AbsoluteMalediction,20);

            // add class to db
            Class = witch.AddToDB();

            // How to add an auto-prepared spell if the class has not been instanciated? the caster definition doesn't exist yet
            familiarHelpAction = FamiliarHelpAction();
            witch.AddFeatureAtLevel(Familiar(familiarHelpAction), 2);
        }

        private static FeatureDefinitionPower FamiliarHelpAction()
        {

            EffectDescription helpActionEffectDescription = new EffectDescription();
            helpActionEffectDescription.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription);
            helpActionEffectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
            helpActionEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            helpActionEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            helpActionEffectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetDescription("Power/&FamiliarHelpActionDescription");
            helpActionEffectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetTitle("Power/&FamiliarHelpActionTitle");

            FeatureDefinitionPower helpAction = new FeatureDefinitionPowerBuilder("FamiliarHelpAction",
                                                                                    GuidHelper.Create(ClassNamespace, "FamiliarHelpAction").ToString(),
                                                                                    1,
                                                                                    RuleDefinitions.UsesDetermination.Fixed,
                                                                                    AttributeDefinitions.Charisma,
                                                                                    RuleDefinitions.ActivationTime.Action,
                                                                                    0,
                                                                                    RuleDefinitions.RechargeRate.AtWill,
                                                                                    false,
                                                                                    false,
                                                                                    AttributeDefinitions.Charisma,
                                                                                    helpActionEffectDescription,
                                                                                    new GuiPresentationBuilder(
                                                                                        "Power/&FamiliarHelpActionDescription",
                                                                                        "Power/&FamiliarHelpActionTitle").Build(),
                                                                                    true)
                                                                                    .AddToDB();

            return helpAction;

        }
        private static FeatureDefinitionFeatureSet Maledictions()
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

            EffectDescription abateEffectDescription = new EffectDescription();
            abateEffectDescription.Copy(DatabaseHelper.SpellDefinitions.ShockingGrasp.EffectDescription);
            abateEffectDescription.EffectForms.RemoveAt(0);
            abateEffectDescription.SetDurationParameter(1);
            abateEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            abateEffectDescription.SetHasSavingThrow(true);
            abateEffectDescription.SetRangeParameter(12);
            abateEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            abateEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Charisma);
            abateEffectDescription.SetTargetParameter(1);
            abateEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            FeatureDefinitionPower abate = new FeatureDefinitionPowerBuilder("WitchMaledictionAbate",
                                                                                GuidHelper.Create(ClassNamespace, "WitchMaledictionAbate").ToString(),
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
                                                                                    "Class/&WitchMaledictionAbateDescription",
                                                                                    "Class/&WitchMaledictionAbateTitle").Build(),
                                                                                true)
                                                                                .AddToDB();

            EffectDescription apathyEffectDescription = new EffectDescription();
            apathyEffectDescription.Copy(DatabaseHelper.SpellDefinitions.CalmEmotionsOnEnemy.EffectDescription);
            apathyEffectDescription.SetDurationParameter(1);
            apathyEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            apathyEffectDescription.SetHasSavingThrow(true);
            apathyEffectDescription.SetRangeParameter(12);
            apathyEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            apathyEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Charisma);
            apathyEffectDescription.SetTargetParameter(1);
            apathyEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            FeatureDefinitionPower apathy = new FeatureDefinitionPowerBuilder("WitchMaledictionApathy",
                                                                                GuidHelper.Create(ClassNamespace, "WitchMaledictionApathy").ToString(),
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
                                                                                    "Class/&WitchMaledictionApathyDescription",
                                                                                    "Class/&WitchMaledictionApathyTitle").Build(),
                                                                                true)
                                                                                .AddToDB();

            EffectDescription charmEffectDescription = new EffectDescription();
            charmEffectDescription.Copy(DatabaseHelper.SpellDefinitions.CharmPerson.EffectDescription);
            charmEffectDescription.SetDurationParameter(1);
            charmEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            charmEffectDescription.SetHasSavingThrow(true);
            charmEffectDescription.SetRangeParameter(12);
            charmEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            charmEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
            charmEffectDescription.SetTargetParameter(1);
            charmEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            FeatureDefinitionPower charm = new FeatureDefinitionPowerBuilder("WitchMaledictionCharm",
                                                                                GuidHelper.Create(ClassNamespace, "WitchMaledictionCharm").ToString(),
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
                                                                                    "Class/&WitchMaledictionCharmDescription",
                                                                                    "Class/&WitchMaledictionCharmTitle").Build(),
                                                                                true)
                                                                                .AddToDB();

            EffectDescription evileyeEffectDescription = new EffectDescription();
            evileyeEffectDescription.Copy(DatabaseHelper.SpellDefinitions.Fear.EffectDescription);
            evileyeEffectDescription.SetDurationParameter(1);
            evileyeEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            evileyeEffectDescription.SetHasSavingThrow(true);
            evileyeEffectDescription.SetRangeParameter(12);
            evileyeEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            evileyeEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
            evileyeEffectDescription.SetTargetParameter(1);
            evileyeEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            FeatureDefinitionPower evileye = new FeatureDefinitionPowerBuilder("WitchMaledictionEvilEye",
                                                                                GuidHelper.Create(ClassNamespace, "WitchMaledictionEvilEye").ToString(),
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
                                                                                    "Class/&WitchMaledictionEvilEyeDescription",
                                                                                    "Class/&WitchMaledictionEvilEyeTitle").Build(),
                                                                                true)
                                                                                .AddToDB();

            EffectDescription obfuscateEffectDescription = new EffectDescription();
            obfuscateEffectDescription.Copy(DatabaseHelper.SpellDefinitions.FogCloud.EffectDescription);
            obfuscateEffectDescription.SetCanBePlacedOnCharacter(true);
            obfuscateEffectDescription.SetDurationParameter(1);
            obfuscateEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            obfuscateEffectDescription.SetRangeParameter(0);
            obfuscateEffectDescription.SetRangeType(RuleDefinitions.RangeType.Self);

            FeatureDefinitionPower obfuscate = new FeatureDefinitionPowerBuilder("WitchMaledictionObfuscate",
                                                                                    GuidHelper.Create(ClassNamespace, "WitchMaledictionObfuscate").ToString(),
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
                                                                                        "Class/&WitchMaledictionObfuscateDescription",
                                                                                        "Class/&WitchMaledictionObfuscateTitle").Build(),
                                                                                    true)
                                                                                    .AddToDB();

            EffectForm poxEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition
            };
            ConditionForm poxConditionForm = new ConditionForm();
            poxEffectForm.SetConditionForm(poxConditionForm);
            poxEffectForm.ConditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionPoisoned);
            EffectDescription poxEffectDescription = new EffectDescription();
            poxEffectDescription.Copy(DatabaseHelper.SpellDefinitions.PoisonSpray.EffectDescription);
            poxEffectDescription.SetDurationParameter(1);
            poxEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            poxEffectDescription.SetHasSavingThrow(true);
            poxEffectDescription.SetRangeParameter(1);
            poxEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            poxEffectDescription.SetSavingThrowAbility(AttributeDefinitions.Constitution);
            poxEffectDescription.SetTargetParameter(1);
            poxEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            poxEffectDescription.EffectForms.Clear();
            poxEffectDescription.EffectForms.Add(poxEffectForm);

            FeatureDefinitionPower pox = new FeatureDefinitionPowerBuilder("WitchMaledictionPox",
                                                                            GuidHelper.Create(ClassNamespace, "WitchMaledictionPox").ToString(),
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
                                                                                "Class/&WitchMaledictionPoxDescription",
                                                                                "Class/&WitchMaledictionPoxTitle").Build(),
                                                                            true)
                                                                            .AddToDB();

            EffectForm ruinEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition
            };
            ConditionForm ruinConditionForm = new ConditionForm();
            ruinEffectForm.SetConditionForm(ruinConditionForm);
            ruinEffectForm.SetCreatedByCharacter(true);
            ConditionDefinition ruinConditionDefinition = new ConditionDefinitionBuilder<ConditionDefinition>(DatabaseHelper.ConditionDefinitions.ConditionAcidArrowed,
                                                                                                                "ConditionRuined",
                                                                                                                GuidHelper.Create(ClassNamespace, "ConditionRuined").ToString(),
                                                                                                                new GuiPresentationBuilder(
                                                                                                                    "Condition/&RuinedDescription",
                                                                                                                    "Condition/&RuinedTitle")
                                                                                                                    .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAcidArrowed.GuiPresentation.SpriteReference)
                                                                                                                    .Build())
                                                                                                                .AddToDB();
            ruinConditionDefinition.SetConditionType(RuleDefinitions.ConditionType.Detrimental);
            ruinConditionDefinition.SetDurationParameter(1);
            ruinConditionDefinition.SetDurationType(RuleDefinitions.DurationType.Round);
            ruinConditionDefinition.RecurrentEffectForms.Clear();
            ruinConditionDefinition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            ruinConditionDefinition.Features.Clear();
            ruinConditionDefinition.Features.Add(new FeatureDefinitionAttributeModifierBuilder("Ruined",
                                                                                                GuidHelper.Create(ClassNamespace, "Ruined").ToString(),
                                                                                                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                                                                                "ArmorClass",
                                                                                                -3,
                                                                                                new GuiPresentationBuilder(
                                                                                                    "Modifier/&RuinedDescription",
                                                                                                    "Modifier/&RuinedTitle")
                                                                                                    .SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionAcidArrowed.GuiPresentation.SpriteReference)
                                                                                                    .Build())
                                                                                                .AddToDB());
            ruinEffectForm.ConditionForm.SetConditionDefinition(ruinConditionDefinition);
            EffectDescription ruinEffectDescription = new EffectDescription();
            ruinEffectDescription.Copy(DatabaseHelper.SpellDefinitions.AcidArrow.EffectDescription);
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

            FeatureDefinitionPower ruin = new FeatureDefinitionPowerBuilder("WitchMaledictionRuin",
                                                                            GuidHelper.Create(ClassNamespace, "WitchMaledictionRuin").ToString(),
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
                                                                                "Class/&WitchMaledictionRuinDescription",
                                                                                "Class/&WitchMaledictionRuinTitle").Build(),
                                                                            true)
                                                                            .AddToDB();

            FeatureDefinitionFeatureSet maledictions = new FeatureDefinitionFeatureSetBuilder(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                                                                                                "WitchFeatureSetMaledictions",
                                                                                                GuidHelper.Create(ClassNamespace, "WitchFeatureSetMaledictions").ToString(),
                                                                                                new GuiPresentationBuilder(
                                                                                                    "Class/&WitchFeatureSetMaledictionsDescription",
                                                                                                    "Class/&WitchFeatureSetMaledictionsTitle").Build())
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

            return maledictions;
        }

        private static FeatureDefinitionFeatureSet WitchCurse()
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

            FeatureDefinitionDamageAffinityBuilder burnedFireRes = new FeatureDefinitionDamageAffinityBuilder(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                                                                                                                "WitchBurnedFireResistance",
                                                                                                                GuidHelper.Create(ClassNamespace, "WitchBurnedFireResistance").ToString(),
                                                                                                                new GuiPresentationBuilder(
                                                                                                                    "Class/&WitchBurnedFireResistanceDescription",
                                                                                                                    "Class/&WitchBurnedFireResistanceTitle").Build());

            FeatureDefinitionBonusCantripsBuilder burnedProduceFlame = new FeatureDefinitionBonusCantripsBuilder(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainElementaFire,
                                                                                                                "WitchBurnedProduceFlame",
                                                                                                                GuidHelper.Create(ClassNamespace, "WitchBurnedProduceFlame").ToString(),
                                                                                                                new GuiPresentationBuilder(
                                                                                                                    "Class/&WitchBurnedProduceFlameDescription",
                                                                                                                    "Class/&WitchBurnedProduceFlameTitle").Build())
                                                                                                                .ClearBonusCantrips()
                                                                                                                .AddBonusCantrip(DatabaseHelper.SpellDefinitions.ProduceFlame);

            FeatureDefinitionFeatureSet burnedCurse = new FeatureDefinitionFeatureSetBuilder(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                                                                                                "WitchFeatureSetBurnedCurse",
                                                                                                GuidHelper.Create(ClassNamespace, "WitchFeatureSetBurnedCurse").ToString(),
                                                                                                new GuiPresentationBuilder(
                                                                                                    "Class/&WitchFeatureSetBurnedCurseDescription",
                                                                                                    "Class/&WitchFeatureSetBurnedCurseTitle").Build())
                                                                                                .ClearFeatures()
                                                                                                .AddFeature(burnedFireRes.AddToDB())
                                                                                                .AddFeature(burnedProduceFlame.AddToDB())
                                                                                                .AddToDB();

            FeatureDefinitionConditionAffinityBuilder lovelessCharmImmunity = new FeatureDefinitionConditionAffinityBuilder(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                                                                                                                            "WitchLovelessCharmImmunity",
                                                                                                                            GuidHelper.Create(ClassNamespace, "WitchLovelessCharmImmunity").ToString(),
                                                                                                                            new GuiPresentationBuilder(
                                                                                                                                "Class/&WitchLovelessCharmImmunityDescription",
                                                                                                                                "Class/&WitchLovelessCharmImmunityTitle").Build());

            FeatureDefinitionFeatureSet lovelessCurse = new FeatureDefinitionFeatureSetBuilder(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                                                                                                "WitchFeatureSetLovelessCurse",
                                                                                                GuidHelper.Create(ClassNamespace, "WitchFeatureSetLovelessCurse").ToString(),
                                                                                                new GuiPresentationBuilder(
                                                                                                    "Class/&WitchFeatureSetLovelessCurseDescription",
                                                                                                    "Class/&WitchFeatureSetLovelessCurseTitle").Build())
                                                                                                .ClearFeatures()
                                                                                                .AddFeature(lovelessCharmImmunity.AddToDB())
                                                                                                .AddToDB();

            // NOTE: I have no idea how to apply a Charisma bonus, so setting the initiative bonus to 3. It seems like only the "Additive" operation works
            FeatureDefinitionAttributeModifierBuilder visionsInitiative = new FeatureDefinitionAttributeModifierBuilder("WitchVisionsInitiative",
                                                                                                                            GuidHelper.Create(ClassNamespace, "WitchVisionsInitiative").ToString(),
                                                                                                                            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                                                                                                            AttributeDefinitions.Initiative,
                                                                                                                            3,
                                                                                                                            new GuiPresentationBuilder(
                                                                                                                                "Class/&WitchVisionsInitiativeDescription",
                                                                                                                                "Class/&WitchVisionsInitiativeTitle").Build())
                                                                                                                            .SetModifierAbilityScore(AttributeDefinitions.Charisma);

            FeatureDefinitionFeatureSet visionsCurse = new FeatureDefinitionFeatureSetBuilder(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                                                                                                "WitchFeatureSetVisionsCurse",
                                                                                                GuidHelper.Create(ClassNamespace, "WitchFeatureSetVisionsCurse").ToString(),
                                                                                                new GuiPresentationBuilder(
                                                                                                    "Class/&WitchFeatureSetVisionsCurseDescription",
                                                                                                    "Class/&WitchFeatureSetVisionsCurseTitle").Build())
                                                                                                .ClearFeatures()
                                                                                                .AddFeature(visionsInitiative.AddToDB())
                                                                                                .AddToDB();

            FeatureDefinitionFeatureSet witchCurse = new FeatureDefinitionFeatureSetBuilder(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting,
                                                                                                "WitchFeatureSetWitchCurse",
                                                                                                GuidHelper.Create(ClassNamespace, "WitchFeatureSetWitchCurse").ToString(),
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

            return witchCurse;
        }

        private static FeatureDefinition Cackle()
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

            FeatureDefinitionPower cackle = new FeatureDefinitionPowerBuilder("WitchCacklePower",
                                                                                GuidHelper.Create(ClassNamespace, "WitchCacklePower").ToString(),
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
                                                                                    "Class/&WitchCacklePowerTitle").Build(),
                                                                                true)
                                                                                .AddToDB();

            return cackle;
        }

        private FeatureDefinitionAutoPreparedSpells Familiar(FeatureDefinitionPower helpAction)
        {

            // This should be the real place where the empowered version of find familiar gets learned. However,
            // how to add an auto-prepared spell if the class has not been instanciated? the caster definition doesn't exist yet
            // i.e. this doesn't work:                      .SetCharacterClass(this.Class)

            MonsterAttackIteration witchFamiliarAttackIteration = new MonsterAttackIteration(DatabaseHelper.MonsterAttackDefinitions.Attack_EagleMatriarch_Talons, 1);
            witchFamiliarAttackIteration.MonsterAttackDefinition.SetToHitBonus(3);
            witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
            witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D1);
            witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(0);

            MonsterDefinition witchFamiliarMonster = new WitchFamiliarMonsterBuilder(DatabaseHelper.MonsterDefinitions.Eagle_Matriarch,
                                                                                        "Owl",
                                                                                        GuidHelper.Create(ClassNamespace, "Owl").ToString(),
                                                                                        DatabaseHelper.MonsterDefinitions.Eagle_Matriarch.GuiPresentation)
                                                                                        .ClearFeatures()
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision24)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenSight)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityKeenHearing)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityFlyby)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityNoClimb)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityNoSpecialMoves)
                                                                                        .AddFeature(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
                                                                                        .AddFeature(helpAction)
                                                                                        .ClearAttackIterations()
                                                                                        .AddAttackIterations(witchFamiliarAttackIteration)
                                                                                        .ClearSkills()
                                                                                        .AddSkills(new MonsterSkillProficiency(DatabaseHelper.SkillDefinitions.Perception.Name, 3))
                                                                                        .AddSkills(new MonsterSkillProficiency(DatabaseHelper.SkillDefinitions.Stealth.Name, 3))
                                                                                        .SetArmorClass(11)
                                                                                        .SetAttributes(new int[] {3,13,8,2,12,7})
                                                                                        .SetHP(1,RuleDefinitions.DieType.D4,-1,1)
                                                                                        .SetSize(DatabaseHelper.CharacterSizeDefinitions.Tiny)
                                                                                        .SetAlignment(DatabaseHelper.AlignmentDefinitions.Unaligned.Name)
                                                                                        .SetChallengeRating(0)
                                                                                        .SetDroppedLoot(null)
                                                                                        .AddToDB();

            SpellDefinition witchFamiliarSpell = new WitchFamiliarSpellBuilder("WitchFamiliar",
                                                                                GuidHelper.Create(ClassNamespace, "WitchFamiliar").ToString())
                                                                                .ClearFamiliar()
                                                                                .AddFamiliar(witchFamiliarMonster)
                                                                                .AddToDB();

            FeatureDefinitionAutoPreparedSpells witchFamiliarAutoPreparedSpell = new FeatureDefinitionAutoPreparedSpellsBuilder("WitchFamiliar",
                                                                                                                                GuidHelper.Create(ClassNamespace, "WitchFamiliar").ToString(),
                                                                                                                                new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>{
                                                                                                                                    FeatureDefinitionAutoPreparedSpellsBuilder.BuildAutoPreparedSpellGroup(2,
                                                                                                                                                                                                            new List<SpellDefinition>{
                                                                                                                                                                                                                witchFamiliarSpell
                                                                                                                                                                                                            })
                                                                                                                                },
                                                                                                                                new GuiPresentationBuilder(
                                                                                                                                    "Spell/&WitchFamiliarDescription",
                                                                                                                                    "Spell/&WitchFamiliarTitle").Build())
                                                                                                                                .SetCharacterClass(this.Class)
                                                                                                                                .SetAutoTag("Witch")
                                                                                                                                .AddToDB();

            return witchFamiliarAutoPreparedSpell;
        }

        internal class WitchFamiliarMonsterBuilder : BaseDefinitionBuilder<MonsterDefinition>
        {

            public WitchFamiliarMonsterBuilder(MonsterDefinition toCopy, string name, string guid, GuiPresentation guiPresentation) : base(toCopy, name, guid)
            {
                Definition.SetGuiPresentation(guiPresentation);

                Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
                Definition.SetFullyControlledWhenAllied(true);
                Definition.SetDefaultFaction("Party");
            }
            public WitchFamiliarMonsterBuilder ClearFeatures()
            {
                Definition.Features.Clear();
                return this;
            }

            public WitchFamiliarMonsterBuilder AddFeature(FeatureDefinition feature)
            {
                Definition.Features.Add(feature);
                return this;
            }

            public WitchFamiliarMonsterBuilder ClearAttackIterations()
            {
                Definition.AttackIterations.Clear();
                return this;
            }

            public WitchFamiliarMonsterBuilder AddAttackIterations(MonsterAttackIteration monsterAttack)
            {
                Definition.AttackIterations.Add(monsterAttack);
                return this;
            }

            public WitchFamiliarMonsterBuilder ClearSkills()
            {
                Definition.SkillScores.Clear();
                return this;
            }

            public WitchFamiliarMonsterBuilder AddSkills(MonsterSkillProficiency monsterSkill)
            {
                Definition.SkillScores.Add(monsterSkill);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetDroppedLoot(LootPackDefinition loot)
            {
                Definition.SetDroppedLootDefinition(loot);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetAttributes(int[] attributes)
            {
                Definition.SetAbilityScores(attributes);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetArmorClass(int armorClass)
            {
                Definition.SetArmorClass(armorClass);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetSize(CharacterSizeDefinition size)
            {
                Definition.SetSizeDefinition(size);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetAlignment(string alignment)
            {
                Definition.SetAlignment(alignment);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetChallengeRating(int cr)
            {
                Definition.SetChallengeRating(cr);
                return this;
            }

            public WitchFamiliarMonsterBuilder SetHP(int hitDice, RuleDefinitions.DieType dieType, int hitPointBonus, int standardHitPoints)
            {
                Definition.SetHitDice(hitDice);
                Definition.SetHitDiceType(dieType);
                Definition.SetHitPointsBonus(hitPointBonus);
                Definition.SetStandardHitPoints(standardHitPoints);
                return this;
            }
        }

        internal class WitchFamiliarSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            public WitchFamiliarSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.Fireball, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&WitchFamiliarTitle";
                Definition.GuiPresentation.Description = "Spell/&WitchFamiliarDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.ConjureAnimals.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(1);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolConjuration);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
                Definition.SetCastingTime(RuleDefinitions.ActivationTime.Hours1);
                Definition.SetRitual(true);
                Definition.SetUniqueInstance(true);

                // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, isntead of 70
                Definition.SetRitualCastingTime(RuleDefinitions.ActivationTime.Hours1);
                Definition.SetRequiresConcentration(false);

                Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(2);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Permanent);
                Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                
            }

            public WitchFamiliarSpellBuilder ClearFamiliar()
            {
                Definition.EffectDescription.EffectForms.Clear();
                return this;
            }

            public WitchFamiliarSpellBuilder AddFamiliar(MonsterDefinition familiar)
            {
                var summonForm = new SummonForm();
                summonForm.SetMonsterDefinitionName(familiar.name);
                summonForm.SetDecisionPackage(null);

                var effectForm = new EffectForm();
                effectForm.SetFormType(EffectForm.EffectFormType.Summon);
                effectForm.SetCreatedByCharacter(true);
                effectForm.SetSummonForm(summonForm);

                Definition.EffectDescription.EffectForms.Add(effectForm);

                return this;
            }

        }

        internal class EldritchOrbSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "EldritchOrbSpell";
            const string SpellNameGuid = "141901ce-79b6-484d-a8ff-f7c6be7c4538";

            protected EldritchOrbSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.Fireball, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&EldritchOrbTitle";
                Definition.GuiPresentation.Description = "Spell/&EldritchOrbDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Shine.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(0);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolEvocation);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
                Definition.SetRitual(false);
                Definition.SetRequiresConcentration(false);

                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(12);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
                Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
                Definition.EffectDescription.SetTargetParameter(1);
                Definition.EffectDescription.SetHasSavingThrow(false);
                Definition.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Dexterity);
                Definition.EffectDescription.SetCanBeDispersed(true);
                Definition.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
                Definition.EffectDescription.EffectAdvancement.SetIncrementMultiplier(5);
                Definition.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.CasterLevelTable);

                Definition.EffectDescription.EffectForms[0].SetHasSavingThrow(false);
                Definition.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(1);
                Definition.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D8);
                Definition.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeForce);
                Definition.EffectDescription.EffectForms[0].SetLevelMultiplier(1);
                Definition.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
                Definition.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

                EffectForm effectForm = new EffectForm();
                effectForm.Copy(Definition.EffectDescription.EffectForms[0]);
                effectForm.SetHasSavingThrow(true);
                effectForm.SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
                effectForm.DamageForm.SetDieType(RuleDefinitions.DieType.D4);
                Definition.EffectDescription.EffectForms.Add(effectForm);

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new EldritchOrbSpellBuilder(name, guid).AddToDB();

            public static readonly SpellDefinition EldritchOrbSpell = CreateAndAddToDB(SpellName, SpellNameGuid);
        }


        internal class MinorLifestealSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "MinorLifestealSpell";
            const string SpellNameGuid = "35240973-9bd0-466c-a835-32e4915a27ec";

            protected MinorLifestealSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.VampiricTouch, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&MinorLifestealTitle";
                Definition.GuiPresentation.Description = "Spell/&MinorLifestealDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.VampiricTouch.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(0);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolNecromancy);
                Definition.SetVerboseComponent(false);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
                Definition.SetRitual(false);
                Definition.SetRequiresConcentration(false);

                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(12);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
                Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
                Definition.EffectDescription.SetHasSavingThrow(true);
                Definition.EffectDescription.SetHalfDamageOnAMiss(false);
                Definition.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Constitution);
                Definition.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
                Definition.EffectDescription.EffectAdvancement.SetIncrementMultiplier(5);
                Definition.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.CasterLevelTable);

                Definition.EffectDescription.EffectForms[1].SetHasSavingThrow(true);
                Definition.EffectDescription.EffectForms[1].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetDiceNumber(1);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetDamageType(RuleDefinitions.DamageTypeNecrotic);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Full);
                Definition.EffectDescription.EffectForms[1].SetLevelMultiplier(1);
                Definition.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
                Definition.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new MinorLifestealSpellBuilder(name, guid).AddToDB();

            public static readonly SpellDefinition MinorLifestealSpell = CreateAndAddToDB(SpellName, SpellNameGuid);
        }

        internal class PetalStormSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "PetalStormSpell";
            const string SpellNameGuid = "9b11d003-e3ae-4cf0-b31c-697d386841ec";

            protected PetalStormSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.InsectPlague, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&PetalStormTitle";
                Definition.GuiPresentation.Description = "Spell/&PetalStormDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.WindWall.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(2);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolConjuration);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Mundane);
                Definition.SetRitual(false);
                Definition.SetRequiresConcentration(true);

                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(12);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
                Definition.EffectDescription.SetDurationParameter(1);
                Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cube);
                Definition.EffectDescription.SetTargetParameter(3);
                Definition.EffectDescription.SetHasSavingThrow(true);
                Definition.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Strength);
                Definition.EffectDescription.SetRecurrentEffect((RuleDefinitions.RecurrentEffect)20);
                Definition.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(2);
                Definition.EffectDescription.EffectAdvancement.SetIncrementMultiplier(1);
                Definition.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);
                Definition.EffectDescription.EffectForms[0].SetHasSavingThrow(true);
                Definition.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
                Definition.EffectDescription.EffectForms[0].DamageForm.SetDiceNumber(3);
                Definition.EffectDescription.EffectForms[0].DamageForm.SetDieType(RuleDefinitions.DieType.D4);
                Definition.EffectDescription.EffectForms[0].DamageForm.SetDamageType(RuleDefinitions.DamageTypeSlashing);
                Definition.EffectDescription.EffectForms[0].SetLevelMultiplier(1);
                Definition.EffectDescription.EffectForms[0].AlterationForm.SetMaximumIncrease(2);
                Definition.EffectDescription.EffectForms[0].AlterationForm.SetValueIncrease(2);

                EffectProxyDefinitionBuilder proxyDefinitionBuilder = new EffectProxyDefinitionBuilder(DatabaseHelper.EffectProxyDefinitions.ProxyInsectPlague,
                                                                                                        "ProxyPetalStorm",
                                                                                                        GuidHelper.Create(ClassNamespace, "ProxyPetalStorm").ToString());
                EffectProxyDefinition proxyDefinition = proxyDefinitionBuilder.AddToDB();
                proxyDefinition.SetActionId(ActionDefinitions.Id.ProxyFlamingSphere);
                proxyDefinition.AdditionalFeatures.Add(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6);
                proxyDefinition.SetGuiPresentation(new GuiPresentationBuilder("PetalStormDescription",
                                                                                "PetalStormTitle").Build());
                proxyDefinition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.WindWall.GuiPresentation.SpriteReference);
                proxyDefinition.SetAttackMethod(RuleDefinitions.ProxyAttackMethod.ReproduceDamageForms);
                proxyDefinition.SetCanMove(true);
                proxyDefinition.SetCanMoveOnCharacters(true);
                proxyDefinition.SetHasPortrait(true);
                proxyDefinition.SetIsEmptyPresentation(false);
                proxyDefinition.SetPortraitSpriteReference(DatabaseHelper.SpellDefinitions.WindWall.GuiPresentation.SpriteReference);

                Definition.EffectDescription.EffectForms[2].SummonForm.SetEffectProxyDefinitionName("ProxyPetalStorm");

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new PetalStormSpellBuilder(name, guid).AddToDB();

            public static readonly SpellDefinition PetalStormSpell = CreateAndAddToDB(SpellName, SpellNameGuid);
        }

        internal class ProtectThresholdSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "ProtectThresholdSpell";
            const string SpellNameGuid = "ff7330cd-e1a2-4ac0-b0b9-3786b8ec29e7";

            protected ProtectThresholdSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.SpikeGrowth, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&ProtectThresholdTitle";
                Definition.GuiPresentation.Description = "Spell/&ProtectThresholdDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Bane.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(2);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolAbjuration);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Mundane);
                Definition.SetRitual(true);
                Definition.SetRitualCastingTime(RuleDefinitions.ActivationTime.Minute10);
                Definition.SetRequiresConcentration(false);

                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(1);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
                Definition.EffectDescription.SetDurationParameter(10);
                Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
                Definition.EffectDescription.SetTargetParameter(0);
                Definition.EffectDescription.SetHasSavingThrow(true);
                Definition.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
                Definition.EffectDescription.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnEnter);
                Definition.EffectDescription.EffectAdvancement.SetAdditionalDicePerIncrement(1);
                Definition.EffectDescription.EffectAdvancement.SetIncrementMultiplier(1);
                Definition.EffectDescription.EffectAdvancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);
                Definition.EffectDescription.EffectForms[1].SetHasSavingThrow(true);
                Definition.EffectDescription.EffectForms[1].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.HalfDamage);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetDiceNumber(4);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetDieType(RuleDefinitions.DieType.D6);
                Definition.EffectDescription.EffectForms[1].DamageForm.SetDamageType(RuleDefinitions.DamageTypePsychic);
                Definition.EffectDescription.EffectForms[1].SetLevelMultiplier(1);
                Definition.EffectDescription.EffectForms[1].AlterationForm.SetMaximumIncrease(2);
                Definition.EffectDescription.EffectForms[1].AlterationForm.SetValueIncrease(2);

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new ProtectThresholdSpellBuilder(name, guid).AddToDB();

            public static readonly SpellDefinition ProtectThresholdSpell = CreateAndAddToDB(SpellName, SpellNameGuid);
        }

        internal class FrenzySpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "FrenzySpell";
            const string SpellNameGuid = "c5ba7337-a1e1-4a74-8117-906c63c6af65";

            protected FrenzySpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.Confusion, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&FrenzyTitle";
                Definition.GuiPresentation.Description = "Spell/&FrenzyDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Confusion.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(6);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolEnchantement);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Mundane);
                Definition.SetRitual(false);
                Definition.SetRequiresConcentration(true);

                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(24);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Minute);
                Definition.EffectDescription.SetDurationParameter(1);
                Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Sphere);
                Definition.EffectDescription.SetTargetParameter(4);
                Definition.EffectDescription.SetHasSavingThrow(true);
                Definition.EffectDescription.SetSavingThrowAbility(AttributeDefinitions.Wisdom);
                Definition.EffectDescription.EffectForms[0].SetHasSavingThrow(true);
                Definition.EffectDescription.EffectForms[0].SetSavingThrowAffinity(RuleDefinitions.EffectSavingThrowType.Negates);
                Definition.EffectDescription.EffectForms[0].ConditionForm.SetConditionDefinition(DatabaseHelper.ConditionDefinitions.ConditionConfusedAttack);

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new FrenzySpellBuilder(name, guid).AddToDB();

            public static readonly SpellDefinition FrenzySpell = CreateAndAddToDB(SpellName, SpellNameGuid);
        }














        internal class FindFamiliarSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "FindFamiliar";
            const string SpellNameGuid = "c20743cd-a0f3-4cee-a653-a8caaa40cc37";

            protected FindFamiliarSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.Fireball, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&FindFamiliarTitle";
                Definition.GuiPresentation.Description = "Spell/&FindFamiliarDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Shine.GuiPresentation.SpriteReference);

                Definition.SetSpellLevel(1);
                Definition.SetSchoolOfMagic(RuleDefinitions.SchoolConjuration);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(true);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Specific);
                Definition.SetSpecificMaterialComponentConsumed(true);
                Definition.SetSpecificMaterialComponentCostGp(10);
                Definition.SetCastingTime(RuleDefinitions.ActivationTime.Hours1);
                Definition.SetRitual(true);
                Definition.SetUniqueInstance(true);

                // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, isntead of 70
                Definition.SetRitualCastingTime(RuleDefinitions.ActivationTime.Hours1);
                Definition.SetRequiresConcentration(false);

                var effectForm = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Summon
                };
                effectForm.SetCreatedByCharacter(true);

                SummonForm summonForm = new SummonForm();
                effectForm.SetSummonForm(summonForm);

                var monsterDefinition = FamiliarMonsterBuilder.FamiliarMonster;

                effectForm.SummonForm.SetMonsterDefinitionName(monsterDefinition.Name);
                effectForm.SummonForm.SetDecisionPackage(null);

                Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
                Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
                Definition.EffectDescription.SetRangeParameter(2);
                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.Permanent);
                //                Definition.EffectDescription.SetDurationType(RuleDefinitions.DurationType.UntilLongRest);
                Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                Definition.EffectDescription.EffectForms.Clear();
                Definition.EffectDescription.EffectForms.Add(effectForm);

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new FindFamiliarSpellBuilder(name, guid).AddToDB();

            public static readonly SpellDefinition FindFamiliarSpell = CreateAndAddToDB(SpellName, SpellNameGuid);
        }

        internal class FamiliarMonsterBuilder : BaseDefinitionBuilder<MonsterDefinition>
        {
            const string FamiliarMonsterName = "Familiar";
            const string FamiliarMonsterNameGuid = "0a3e6a7d-4628-4189-b91d-d7146d771234";

            protected FamiliarMonsterBuilder(string name, string guid) : base(DatabaseHelper.MonsterDefinitions.Flying_Snake, name, guid)
            {
                Definition.GuiPresentation.Title = "Familiar";
                Definition.GuiPresentation.Description = "Fate has granted you a portion of its power through this construct.";

                Definition.SetAlignment("neutral");
                Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
                Definition.SetFullyControlledWhenAllied(true);
                Definition.SetDefaultFaction("Party");
                Definition.AttackIterations.Clear();

                EffectDescription helpActionEffectDescription = new EffectDescription();
                helpActionEffectDescription.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription);
                helpActionEffectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
                helpActionEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
                helpActionEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
                helpActionEffectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetDescription("Power/&FamiliarHelpActionDescription");
                helpActionEffectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetTitle("Power/&FamiliarHelpActionTitle");
/*
                FeatureDefinitionPower helpAction = new FeatureDefinitionPowerBuilder("FamiliarHelpAction",
                                                                                        GuidHelper.Create(ClassNamespace, "FamiliarHelpAction").ToString(),
                                                                                        1,
                                                                                        RuleDefinitions.UsesDetermination.Fixed,
                                                                                        AttributeDefinitions.Charisma,
                                                                                        RuleDefinitions.ActivationTime.Action,
                                                                                        0,
                                                                                        RuleDefinitions.RechargeRate.AtWill,
                                                                                        false,
                                                                                        false,
                                                                                        AttributeDefinitions.Charisma,
                                                                                        helpActionEffectDescription,
                                                                                        new GuiPresentationBuilder(
                                                                                            "Power/&FamiliarHelpActionDescription",
                                                                                            "Power/&FamiliarHelpActionTitle").Build(),
                                                                                        true)
                                                                                        .AddToDB();

                Definition.Features.Add(helpAction);*/
            }

            public static MonsterDefinition CreateAndAddToDB(string name, string guid)
                => new FamiliarMonsterBuilder(name, guid).AddToDB();

            public static readonly MonsterDefinition FamiliarMonster = CreateAndAddToDB(FamiliarMonsterName, FamiliarMonsterNameGuid);
        }


    }
}
