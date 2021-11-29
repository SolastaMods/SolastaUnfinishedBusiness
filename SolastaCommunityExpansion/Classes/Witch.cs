using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Classes.Witch
{
    internal class Witch : AbstractClass
    {
        private static Guid ClassNamespace = new Guid("ea7715dd-00cb-45a3-a8c4-458d0639d72c");
        private readonly CharacterClassDefinition Class;
        internal override CharacterClassDefinition GetClass()
        {
            return Class;
        }

        internal Witch()
        {
            // Make Witch Class
            CharacterClassDefinitionBuilder witch = new CharacterClassDefinitionBuilder("Witch", GuidHelper.Create(ClassNamespace, "Witch").ToString());

//            FeatureDefinitionSubclassChoiceBuilder witchFeatureDefinitionSubclassChoiceBuilder = new FeatureDefinitionSubclassChoiceBuilder("SubclassChoiceWitchCovens", GuidHelper.Create(ClassSubclassChoice, "SubclassChoiceWitchCovens").ToString());

            var subclassChoicesGuiPresentation = new GuiPresentation();
            subclassChoicesGuiPresentation.Title = "Subclass/&WitchSubclassPathTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&WitchSubclassPathDescription";
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

            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Arcana);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Deception);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Insight);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Intimidation);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Persuasion);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Nature);
            witch.AddSkillPreference(DatabaseHelper.SkillDefinitions.Religion);
            
            witch.AddEquipmentRow(  new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Bolt, EquipmentDefinitions.OptionWeapon, 20),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Quarterstaff, EquipmentDefinitions.OptionWeapon, 1),
                                    }
            );
            witch.AddEquipmentRow(  new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ScholarPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    }
            );
            witch.AddEquipmentRow(  new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.EnchantingTool, EquipmentDefinitions.OptionTool, 1),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                         EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.HerbalismKit, EquipmentDefinitions.OptionTool, 1),
                                    }
            );
            witch.AddEquipmentRow(  new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionFocus, 1),
                                    }
            );

            witch.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.SorcererArmor, EquipmentDefinitions.OptionArmor, 1),
            });

            FeatureDefinitionProficiency savingThrowProficiencies = new FeatureDefinitionProficiencyBuilder("WitchSavingthrowProficiency", 
                                                                                                            GuidHelper.Create(ClassNamespace, "WitchSavingthrowProficiency").ToString(),
                                                                                                            RuleDefinitions.ProficiencyType.SavingThrow,
                                                                                                            new List<string>() { AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom },
                                                                                                            new GuiPresentationBuilder(
                                                                                                            "Class/&WitchSavingthrowProficiencyDescription",
                                                                                                            "Class/&WitchSavingthrowProficiencyTitle").Build())
                                                                                                            .AddToDB();

            FeatureDefinitionPointPool skillProficiencies = new FeatureDefinitionPointPoolBuilder(  "WitchSkillProficiency", 
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

            FeatureDefinitionProficiency weaponProficiencies = new FeatureDefinitionProficiencyBuilder( "WitchWeaponProficiency", 
                                                                                                        GuidHelper.Create(ClassNamespace, "WitchWeaponProficiency").ToString(),
                                                                                                        RuleDefinitions.ProficiencyType.Weapon,
                                                                                                        new List<string>() { EquipmentDefinitions.SimpleWeaponCategory },
                                                                                                        new GuiPresentationBuilder(
                                                                                                        "Class/&WitchWeaponProficiencyDescription",
                                                                                                        "Class/&WitchWeaponProficiencyTitle").Build())
                                                                                                        .AddToDB();

            FeatureDefinitionProficiency armorProficiencies = new FeatureDefinitionProficiencyBuilder(  "WitchArmorProficiency", 
                                                                                                        GuidHelper.Create(ClassNamespace, "WitchArmorProficiency").ToString(),
                                                                                                        RuleDefinitions.ProficiencyType.Armor,
                                                                                                        new List<string>() { EquipmentDefinitions.LightArmorCategory },
                                                                                                        new GuiPresentationBuilder(
                                                                                                        "Class/&WitchArmorProficiencyDescription",
                                                                                                        "Class/&WitchArmorProficiencyTitle").Build())
                                                                                                        .AddToDB();

/*            FeatureDefinitionProficiency toolProficiencies = new FeatureDefinitionProficiencyBuilder(  "WitchToolProficiency", 
                                                                                                        GuidHelper.Create(ClassNamespace, "WitchToolProficiency").ToString(),
                                                                                                        RuleDefinitions.ProficiencyType.Tool,
                                                                                                        new List<string>() { ToolDefinitions.ArtisanToolType },
                                                                                                        new GuiPresentationBuilder(
                                                                                                        "Class/&WitchToolProficiencyDescription",
                                                                                                        "Class/&WitchToolProficiencyTitle").Build())
                                                                                                        .AddToDB();
*/

            // Keeping all spells listed here for ease to edit and see the big picture
            SpellListDefinition spellList = SpellListBuilder.createSpellList(  "SpellListClassWitch", 
                                                                                GuidHelper.Create(ClassNamespace, "SpellListClassWitch").ToString(),
                                                                                "",
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.AcidSplash,
//?                                                                                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                                                                                    DatabaseHelper.SpellDefinitions.ChillTouch,
                                                                                    DatabaseHelper.SpellDefinitions.DancingLights,
//?                                                                                    DatabaseHelper.SpellDefinitions.Dazzle,
//                                                                                    DatabaseHelper.SpellDefinitions.FireBolt,
//                                                                                    DatabaseHelper.SpellDefinitions.Guidance,
//                                                                                    DatabaseHelper.SpellDefinitions.Light,
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
//                                                                                    DatabaseHelper.SpellDefinitions.PrayerOfHealing,
//                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison,
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

            CastSpellBuilder spellCasting = new CastSpellBuilder(   "CastSpellClassWitch", 
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
                                                                    .SetGuiPresentation( new GuiPresentationBuilder(
                                                                        "Class/&WitchSpellcastingDescription",
                                                                        "Class/&WitchSpellcastingTitle").Build());

/*
            var summoner_spellcasting = Helpers.SpellcastingBuilder.createSpontaneousSpellcasting("SummonerClassSpellcasting",
                                                                                              "f720edaf-92c4-43e3-8228-c48c0b411234",
                                                                                              "Feature/&SummonerClassSpellcastingTitle",
                                                                                              "Feature/&SummonerClassSpellcastingDescription",
                                                                                              summoner_spelllist,
                                                                                              Helpers.Stats.Charisma,
                                                                                              DatabaseHelper.FeatureDefinitionCastSpells.CastSpellWizard.KnownCantrips,
                                                                                              new List<int> { 4,  5,  6,  7,  8,  9, 10, 11, 12, 12,
                                                                                                             13, 13, 14, 14, 15, 15, 16, 16, 16, 16},
                                                                                              DatabaseHelper.FeatureDefinitionCastSpells.CastSpellWizard.SlotsPerLevels
                                                                                              );
            
*/

            GuiPresentationBuilder witchPresentation = new GuiPresentationBuilder(
                "Class/&WitchDescription",
                "Class/&WitchTitle");
            witchPresentation.SetSpriteReference(DatabaseHelper.CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference);
            witch.SetGuiPresentation(witchPresentation.Build());


            witch.AddFeatureAtLevel(savingThrowProficiencies, 1);
            witch.AddFeatureAtLevel(skillProficiencies, 1);
            witch.AddFeatureAtLevel(weaponProficiencies, 1);
            witch.AddFeatureAtLevel(armorProficiencies, 1);
//            witch.AddFeatureAtLevel(toolProficiencies, 1);
            witch.AddFeatureAtLevel(spellCasting.AddToDB(), 1);
//            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(ritual_spellcasting, 1));

            // TODO: Do a dropdown like the Druid of the Land land type or summon type for the Druid of Spirits (?)
//            witch.AddFeatureAtLevel(WitchsCurse,1);
            // TODO: Do some powers like the Tinkerer infusions, dropdown like infusions too
            // powers should last 1 round by default
            // how to do 2nd concentration mechanic???
//            witch.AddFeatureAtLevel(Maledictions, 1);
            // This should actually be a bit refined, i.e. give the Find Familiar spell, and have a longer casting time
            // Beckon Familiar is actually a Malediction, i.e. instant cast familiar
            witch.AddFeatureAtLevel(BeckonFamiliar(), 2);
            // TODO: unsure about this one...
            // Needs to refresh an existing Malediction, preventing the saving throw again I presume
//            witch.AddFeatureAtLevel(Cackle,2);

            witch.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4);

            // TODO: Maledictions should now apply a debuff for disadvantage on saving throw like Force Of Law
//            witch.AddFeatureAtLevel(InsidiousSpell,5);
            // TODO: Simply buff the familiar accordingly, i.e. offer more forms, and if that is too hard, 
            // apply proficiency bonus on hit, or
            // extra attack to the familiar
//            witch.AddFeatureAtLevel(ImprovedFamiliar,7);

            witch.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8);

            // Maybe change this... not sure what to do... is there an OnDeath event or something?
//            witch.AddFeatureAtLevel(DyingCurse,9);
            // TODO: Another set of Tinkerer infusions
//            witch.AddFeatureAtLevel(GreaterMalediction,11);

            witch.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12);

            // TODO: Another set of Tinkerer infusions
//            witch.AddFeatureAtLevel(GreaterMalediction,13);
            // TODO: Another set of Tinkerer infusions
//            witch.AddFeatureAtLevel(GreaterMalediction,15);
            // TODO: Another set of Tinkerer infusions
//            witch.AddFeatureAtLevel(GreaterMalediction,18);
            // TODO: Another drop down list like Circle of the Land Druid
//            witch.AddFeatureAtLevel(AbsoluteMalediction,20);

            // add class to db
            Class = witch.AddToDB();
        }

        FeatureDefinitionPower BeckonFamiliar()
        {
            var effectForm = new EffectForm();
            effectForm.FormType = EffectForm.EffectFormType.Summon;
            effectForm.SetCreatedByCharacter(true);

            SummonForm summonForm = new SummonForm();
            effectForm.SetSummonForm(summonForm);

            MonsterDefinition monsterDefinition = new MonsterDefinition();
            monsterDefinition = FamiliarMonsterBuilder.AddToMonsterList();

            effectForm.SummonForm.SetMonsterDefinitionName(monsterDefinition.Name);
            effectForm.SummonForm.SetDecisionPackage(null);

            //Add to our new effect
            var effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
            effectDescription.DurationType = RuleDefinitions.DurationType.UntilLongRest;
            effectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            effectDescription.SetRangeParameter(24);
            effectDescription.EffectForms.Clear();
            effectDescription.EffectForms.Add(effectForm);

            FeatureDefinitionPower beckonFamiliar = new FeatureDefinitionPowerBuilder(  "ClassWitchBeckonFamiliarPower",
                                                                                        GuidHelper.Create(ClassNamespace, "ClassWitchBeckonFamiliarPower").ToString(),
                                                                                        1,
                                                                                        RuleDefinitions.UsesDetermination.Fixed,
                                                                                        AttributeDefinitions.Charisma,
                                                                                        RuleDefinitions.ActivationTime.Action,
                                                                                        0,
                                                                                        RuleDefinitions.RechargeRate.AtWill,
                                                                                        false,
                                                                                        true,
                                                                                        AttributeDefinitions.Charisma,
                                                                                        effectDescription,
                                                                                        new GuiPresentationBuilder(
                                                                                            "Class/&ClassWitchBeckonFamiliarPowerDescription",
                                                                                            "Class/&ClassWitchBeckonFamiliarPowerTitle").Build(),
                                                                                        true)
                                                                                        .AddToDB();

            return beckonFamiliar;
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

                FeatureDefinitionPower helpAction = new FeatureDefinitionPowerBuilder(  "FamiliarHelpAction",
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

                Definition.Features.Add(helpAction);

/*                FeatureDefinitionCastSpell featureDefinitionCastSpell = new FeatureDefinitionCastSpell();
                featureDefinitionCastSpell = FamiliarCastSpellBuilder.AddToFeatureDefinitionCastSpellList();
                Definition.Features.Remove(DatabaseHelper.FeatureDefinitionCastSpells.CastSpellCubeOfLight);
                Definition.Features.Add(featureDefinitionCastSpell);
*/
            }

            public static MonsterDefinition CreateAndAddToDB(string name, string guid)
                => new FamiliarMonsterBuilder(name, guid).AddToDB();

            public static MonsterDefinition FamiliarMonster = CreateAndAddToDB(FamiliarMonsterName, FamiliarMonsterNameGuid);

            public static MonsterDefinition AddToMonsterList()
            {
                var FamiliarMonster = FamiliarMonsterBuilder.FamiliarMonster;//Instantiating it adds to the DB
                return FamiliarMonster;
            }

        }




        internal class FamiliarSpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
        {
            const string FamiliarSpellListName = "FamiliarSpellList";
            const string FamiliarSpellListNameGuid = "0a3e6a7d-4628-4189-b91d-d7146d991234";

            protected FamiliarSpellListBuilder(string name, string guid) : base(DatabaseHelper.SpellListDefinitions.SpellListCubeOfLight, name, guid)
            {
                Definition.SpellsByLevel.Clear();
                Definition.SetHasCantrips(true);

                // 0
                SpellListDefinition.SpellsByLevelDuplet spellsByLevelDuplet0 = new SpellListDefinition.SpellsByLevelDuplet();
                List<SpellDefinition> spellList0 = new List<SpellDefinition>();
                spellsByLevelDuplet0.Spells = spellList0;
                spellsByLevelDuplet0.Spells.Add(DatabaseHelper.SpellDefinitions.Dazzle);
                spellsByLevelDuplet0.Spells.Add(DatabaseHelper.SpellDefinitions.Guidance);

                SpellDefinition nudgeFate = new SpellDefinition();
                nudgeFate = NudgeFateSpellBuilder.AddToSpellList();
                spellsByLevelDuplet0.Spells.Add(nudgeFate);

                spellsByLevelDuplet0.Spells.Add(DatabaseHelper.SpellDefinitions.Resistance);
                spellsByLevelDuplet0.Spells.Add(DatabaseHelper.SpellDefinitions.TrueStrike);

                // 1
                SpellListDefinition.SpellsByLevelDuplet spellsByLevelDuplet1 = new SpellListDefinition.SpellsByLevelDuplet();
                spellsByLevelDuplet1.Level = 1;
                List<SpellDefinition> spellList1 = new List<SpellDefinition>();
                spellsByLevelDuplet1.Spells = spellList1;
                spellsByLevelDuplet1.Spells.Add(DatabaseHelper.SpellDefinitions.Bane);
                spellsByLevelDuplet1.Spells.Add(DatabaseHelper.SpellDefinitions.Bless);
                spellsByLevelDuplet1.Spells.Add(DatabaseHelper.SpellDefinitions.FaerieFire);
                spellsByLevelDuplet1.Spells.Add(DatabaseHelper.SpellDefinitions.ShieldOfFaith);
                // 2
                SpellListDefinition.SpellsByLevelDuplet spellsByLevelDuplet2 = new SpellListDefinition.SpellsByLevelDuplet();
                spellsByLevelDuplet2.Level = 2;
                List<SpellDefinition> spellList2 = new List<SpellDefinition>();
                spellsByLevelDuplet2.Spells = spellList2;
                spellsByLevelDuplet2.Spells.Add(DatabaseHelper.SpellDefinitions.EnhanceAbility);
                spellsByLevelDuplet2.Spells.Add(DatabaseHelper.SpellDefinitions.LesserRestoration);
                spellsByLevelDuplet2.Spells.Add(DatabaseHelper.SpellDefinitions.MagicWeapon);
                spellsByLevelDuplet2.Spells.Add(DatabaseHelper.SpellDefinitions.RayOfEnfeeblement);
                spellsByLevelDuplet2.Spells.Add(DatabaseHelper.SpellDefinitions.WardingBond);
                // 3
                SpellListDefinition.SpellsByLevelDuplet spellsByLevelDuplet3 = new SpellListDefinition.SpellsByLevelDuplet();
                spellsByLevelDuplet3.Level = 3;
                List<SpellDefinition> spellList3 = new List<SpellDefinition>();
                spellsByLevelDuplet3.Spells = spellList3;
                spellsByLevelDuplet3.Spells.Add(DatabaseHelper.SpellDefinitions.BeaconOfHope);
                spellsByLevelDuplet3.Spells.Add(DatabaseHelper.SpellDefinitions.BestowCurse);
    //            spellsByLevelDuplet3.Spells.Add(DatabaseHelper.SpellDefinitions.Counterspell);
                spellsByLevelDuplet3.Spells.Add(DatabaseHelper.SpellDefinitions.DispelMagic);

                // 4
                SpellListDefinition.SpellsByLevelDuplet spellsByLevelDuplet4 = new SpellListDefinition.SpellsByLevelDuplet();
                spellsByLevelDuplet4.Level = 4;
                List<SpellDefinition> spellList4 = new List<SpellDefinition>();
                spellsByLevelDuplet4.Spells = spellList4;
                spellsByLevelDuplet4.Spells.Add(DatabaseHelper.SpellDefinitions.DeathWard);
                spellsByLevelDuplet4.Spells.Add(DatabaseHelper.SpellDefinitions.DimensionDoor);
                spellsByLevelDuplet4.Spells.Add(DatabaseHelper.SpellDefinitions.IdentifyCreatures);

                // put this as 4th level spell until multitarget can be done, and proper saving throw logic applies
                SpellDefinition timeBlink = new SpellDefinition();
                timeBlink = TimeBlinkSpellBuilder.AddToSpellList();
                spellsByLevelDuplet4.Spells.Add(timeBlink);


                // 5
                SpellListDefinition.SpellsByLevelDuplet spellsByLevelDuplet5 = new SpellListDefinition.SpellsByLevelDuplet();
                spellsByLevelDuplet5.Level = 5;
                List<SpellDefinition> spellList5 = new List<SpellDefinition>();
                spellsByLevelDuplet5.Spells = spellList5;
                spellsByLevelDuplet5.Spells.Add(DatabaseHelper.SpellDefinitions.DispelEvilAndGood);
                spellsByLevelDuplet5.Spells.Add(DatabaseHelper.SpellDefinitions.GreaterRestoration);



                Definition.SpellsByLevel.Add(spellsByLevelDuplet0);
                Definition.SpellsByLevel.Add(spellsByLevelDuplet1);
                Definition.SpellsByLevel.Add(spellsByLevelDuplet2);
                Definition.SpellsByLevel.Add(spellsByLevelDuplet3);
                Definition.SpellsByLevel.Add(spellsByLevelDuplet4);
                Definition.SpellsByLevel.Add(spellsByLevelDuplet5);

            }

            public static SpellListDefinition CreateAndAddToDB(string name, string guid)
                => new FamiliarSpellListBuilder(name, guid).AddToDB();

            public static SpellListDefinition FamiliarSpellList = CreateAndAddToDB(FamiliarSpellListName, FamiliarSpellListNameGuid);

            public static SpellListDefinition AddToSpellListDefinitionList()
            {
                var FamiliarSpellList = FamiliarSpellListBuilder.FamiliarSpellList;//Instantiating it adds to the DB
                return FamiliarSpellList;
            }
        }

        internal class NudgeFateSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "NudgeFateSpell";
            const string SpellNameGuid = "88f1fb27-66af-49c6-b038-a38142b21234";

            protected NudgeFateSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.ShadowDagger, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&NudgeFateTitle";
                Definition.GuiPresentation.Description = "Spell/&NudgeFateDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.TrueStrike.GuiPresentation.SpriteReference);

                Definition.SetSchoolOfMagic("SchoolDivination");
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(false);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);

                EffectDescription effectDescription = Definition.EffectDescription;
                effectDescription.DurationType = RuleDefinitions.DurationType.Round;
                effectDescription.SetRangeParameter(12);
                effectDescription.HasSavingThrow = true;
                effectDescription.SetHalfDamageOnAMiss(false);
                effectDescription.EffectParticleParameters.Clear();
                effectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);

                EffectForm mainEffectForm = effectDescription.EffectForms[0];
                mainEffectForm.FormType = EffectForm.EffectFormType.Condition;
                mainEffectForm.SetCreatedByCharacter(true);
                mainEffectForm.HasSavingThrow = true;
                mainEffectForm.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

                ConditionDefinition conditionDefinition = new ConditionDefinition();
                conditionDefinition = NudgeFateConditionBuilder.AddToConditionList();

                ConditionForm conditionForm = new ConditionForm();
                conditionForm.ConditionDefinition = conditionDefinition;
                conditionForm.SetConditionDefinitionName(conditionDefinition.Name);

                mainEffectForm.ConditionForm = conditionForm;

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new NudgeFateSpellBuilder(name, guid).AddToDB();

            public static SpellDefinition NudgeFateSpell = CreateAndAddToDB(SpellName, SpellNameGuid);

            public static SpellDefinition AddToSpellList()
            {
                var NudgeFateSpell = NudgeFateSpellBuilder.NudgeFateSpell;//Instantiating it adds to the DB
                return NudgeFateSpell;
            }
        }

        internal class NudgeFateConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string ConditionName = "NudgeFateCondition";
            const string ConditionNameGuid = "1231fb27-66af-49c6-b038-a38142b21234";

            protected NudgeFateConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionTargetedByGuidingBolt, name, guid)
            {
                Definition.GuiPresentation.Title = "Condition/&NudgeFateTitle";
                Definition.GuiPresentation.Description = "Condition/&NudgeFateDescription";

                Definition.SpecialInterruptions.Clear();
                Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Attacks);
                Definition.SetTerminateWhenRemoved(true);
                Definition.Features.Clear();

                //            FeatureDefinitionCombatAffinity featureDefinitionCombatAffinity = new FeatureDefinitionCombatAffinity();
                FeatureDefinitionCombatAffinity featureDefinitionCombatAffinity = new FeatureDefinitionCombatAffinity();

                featureDefinitionCombatAffinity = NudgeFateFeatureDefinitionCombatAffinityBuilder.AddToFeatureDefinitionCombatAffinityList();

                Definition.Features.Add(featureDefinitionCombatAffinity);
            }

            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new NudgeFateConditionBuilder(name, guid).AddToDB();

            public static ConditionDefinition NudgeFateCondition = CreateAndAddToDB(ConditionName, ConditionNameGuid);

            public static ConditionDefinition AddToConditionList()
            {
                var NudgeFateCondition = NudgeFateConditionBuilder.NudgeFateCondition;//Instantiating it adds to the DB
                return NudgeFateCondition;
            }
        }

        internal class NudgeFateFeatureDefinitionCombatAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionCombatAffinity>
        {
            const string FeatureDefinitionCombatAffinityName = "NudgeFateFeatureDefinitionCombatAffinity";
            const string FeatureDefinitionCombatAffinityNameGuid = "123aab27-66af-49c6-b038-a38142b21234";

            protected NudgeFateFeatureDefinitionCombatAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityPoisoned, name, guid)
            {
                Definition.GuiPresentation.Title = "FeatureDefinitionCombatAffinity/&NudgeFateTitle";
                Definition.GuiPresentation.Description = "FeatureDefinitionCombatAffinity/&NudgeFateDescription";

                Definition.SetMyAttackAdvantage(RuleDefinitions.AdvantageType.Disadvantage);
            }

            public static FeatureDefinitionCombatAffinity CreateAndAddToDB(string name, string guid)
                => new NudgeFateFeatureDefinitionCombatAffinityBuilder(name, guid).AddToDB();

            public static FeatureDefinitionCombatAffinity NudgeFateFeatureDefinitionCombatAffinity = CreateAndAddToDB(FeatureDefinitionCombatAffinityName, FeatureDefinitionCombatAffinityNameGuid);

            public static FeatureDefinitionCombatAffinity AddToFeatureDefinitionCombatAffinityList()
            {
                var NudgeFateFeatureDefinitionCombatAffinity = NudgeFateFeatureDefinitionCombatAffinityBuilder.NudgeFateFeatureDefinitionCombatAffinity;//Instantiating it adds to the DB
                return NudgeFateFeatureDefinitionCombatAffinity;
            }
        }


        internal class TimeBlinkSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
        {
            const string SpellName = "TimeBlinkSpell";
            const string SpellNameGuid = "88f1fb27-66af-49c6-b038-a38142b31234";

            protected TimeBlinkSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.Counterspell, name, guid)
            {
                Definition.GuiPresentation.Title = "Spell/&TimeBlinkTitle";
                Definition.GuiPresentation.Description = "Spell/&TimeBlinkDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Banishment.GuiPresentation.SpriteReference);

                Definition.SetSchoolOfMagic("SchoolTransmutation");
                Definition.SetSpellLevel(4);
                Definition.SetVerboseComponent(true);
                Definition.SetSomaticComponent(false);
                Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
                Definition.SetRequiresConcentration(false);

                EffectDescription effectDescription = Definition.EffectDescription;
                effectDescription.DurationType = RuleDefinitions.DurationType.Round;
                effectDescription.SetRangeParameter(24);
                effectDescription.HasSavingThrow = true;
                effectDescription.SetHalfDamageOnAMiss(false);
                effectDescription.EffectParticleParameters.Clear();
                effectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.Banishment.EffectDescription.EffectParticleParameters);
                effectDescription.SavingThrowAbility = "Wisdom";
                effectDescription.EffectAdvancement.SetAdditionalTargetsPerIncrement(1);

                // counter form
                EffectForm counterEffectForm = effectDescription.EffectForms[0];
                counterEffectForm.FormType = EffectForm.EffectFormType.Counter;
                counterEffectForm.SetCreatedByCharacter(true);
                counterEffectForm.HasSavingThrow = true;
                counterEffectForm.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
                // Until there is a way to actually put a saving throw upon casting a counterspell, make this act like Counterspell, i.e. give a chance to fail
    //            counterEffectForm.CounterForm.SetAutomaticSpellLevel(9);

                // banish form
                ConditionDefinition banishConditionDefinition = new ConditionDefinition();
                banishConditionDefinition = TimeBlinkConditionBuilder.AddToConditionList();

                ConditionForm banishConditionForm = new ConditionForm();
                banishConditionForm.ConditionDefinition = banishConditionDefinition;
                banishConditionForm.SetConditionDefinitionName(banishConditionDefinition.Name);

                EffectForm banishEffectForm = new EffectForm();
                banishEffectForm.FormType = EffectForm.EffectFormType.Condition;
                banishEffectForm.SetCreatedByCharacter(true);
                banishEffectForm.HasSavingThrow = true;
                banishEffectForm.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

                banishEffectForm.ConditionForm = banishConditionForm;
                effectDescription.EffectForms.Add(banishEffectForm);

            }

            public static SpellDefinition CreateAndAddToDB(string name, string guid)
                => new TimeBlinkSpellBuilder(name, guid).AddToDB();

            public static SpellDefinition TimeBlinkSpell = CreateAndAddToDB(SpellName, SpellNameGuid);

            public static SpellDefinition AddToSpellList()
            {
                var TimeBlinkSpell = TimeBlinkSpellBuilder.TimeBlinkSpell;//Instantiating it adds to the DB
                return TimeBlinkSpell;
            }
        }

        internal class TimeBlinkConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string ConditionName = "TimeBlinkCondition";
            const string ConditionNameGuid = "1231fb27-66af-49c6-b038-a38142b31234";

            protected TimeBlinkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionBanished, name, guid)
            {
                Definition.GuiPresentation.Title = "Condition/&TimeBlinkTitle";
                Definition.GuiPresentation.Description = "Condition/&TimeBlinkDescription";

                Definition.SetPermanentlyRemovedIfExtraPlanar(false);

            }

            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new TimeBlinkConditionBuilder(name, guid).AddToDB();

            public static ConditionDefinition TimeBlinkCondition = CreateAndAddToDB(ConditionName, ConditionNameGuid);

            public static ConditionDefinition AddToConditionList()
            {
                var TimeBlinkCondition = TimeBlinkConditionBuilder.TimeBlinkCondition;//Instantiating it adds to the DB
                return TimeBlinkCondition;
            }
        }




    }
}
