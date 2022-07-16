using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Witch.Subclasses;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Level20;
using SolastaCommunityExpansion.Models;
using static FeatureDefinitionAttributeModifier;
using static FeatureDefinitionCastSpell;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Builders.EquipmentOptionsBuilder;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Spells.BazouSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch;

internal static class Witch
{
    private static readonly Guid WitchBaseGuid = new("ea7715dd-00cb-45a3-a8c4-458d0639d72c");

    public static readonly CharacterClassDefinition Instance = BuildAndAddClass();

    private static SpellListDefinition _witchSpellList;
    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; set; }
    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; set; }
    private static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; set; }
    private static FeatureDefinitionPointPool FeatureDefinitionPointPoolSkills { get; set; }
    private static FeatureDefinitionPointPool FeatureDefinitionPointPoolTools { get; set; }
    private static FeatureDefinitionCastSpell FeatureDefinitionCastSpellWitch { get; set; }
    private static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetRitualCasting { get; set; }
    private static FeatureDefinitionFeatureSetCustom FeatureDefinitionFeatureSetWitchCurses { get; set; }
    private static FeatureDefinitionFeatureSetCustom FeatureDefinitionFeatureSetMaledictions { get; set; }
    private static FeatureDefinitionPower FeatureDefinitionPowerCackle { get; set; }
    private static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetWitchFamiliar { get; set; }

    internal static SpellListDefinition WitchSpellList => _witchSpellList ??= SpellListDefinitionBuilder
        .Create(SpellListDefinitions.SpellListWizard, "WitchSpellList", WitchBaseGuid)
        .SetGuiPresentationNoContent()
        .ClearSpells()
        .SetSpellsAtLevel(0,
            AcidSplash, ChillTouch, DancingLights, EldritchOrb, ProduceFlame,
            MinorLifesteal, Resistance, SpareTheDying, TrueStrike)
        .SetSpellsAtLevel(1,
            AnimalFriendship, Bane, CharmPerson, ComprehendLanguages,
            DetectMagic, ExpeditiousRetreat, FaerieFire, FindFamiliar, HideousLaughter,
            ProtectionFromEvilGood, Sleep, Thunderwave)
        .SetSpellsAtLevel(2,
            Blindness, CalmEmotions, Darkness, Darkvision,
            HoldPerson, Invisibility, Knock, Levitate,
            MistyStep, PetalStorm, ProtectThreshold, RayOfEnfeeblement,
            SeeInvisibility, Shatter, SpiderClimb)
        .SetSpellsAtLevel(3,
            BestowCurse, Counterspell, DispelMagic, Fear,
            Fly, HypnoticPattern, RemoveCurse, Slow,
            StinkingCloud, Tongues)
        .SetSpellsAtLevel(4,
            Banishment, BlackTentacles, Confusion, DimensionDoor,
            DominateBeast, GreaterInvisibility, PhantasmalKiller)
        .SetSpellsAtLevel(5,
            Contagion, DispelEvilAndGood, DominatePerson, HoldMonster)
        .SetSpellsAtLevel(6,
            Eyebite, Frenzy, TrueSeeing)
        .FinalizeSpells()
        .AddToDB();

    private static void BuildClassStats([NotNull] CharacterClassDefinitionBuilder classBuilder)
    {
        classBuilder
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard)
            .SetPictogram(Sorcerer.ClassPictogramReference)
            .SetBattleAI(Sorcerer.DefaultBattleDecisions)
            .SetHitDice(DieType.D8)
            .SetIngredientGatheringOdds(Sorcerer.IngredientGatheringOdds)
            .SetAbilityScorePriorities(
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Strength)
            .AddFeatPreference(FeatDefinitions.PowerfulCantrip)
            .AddToolPreferences(
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType)
            .AddSkillPreferences(
                DatabaseHelper.SkillDefinitions.Arcana,
                DatabaseHelper.SkillDefinitions.Deception,
                DatabaseHelper.SkillDefinitions.Insight,
                DatabaseHelper.SkillDefinitions.Intimidation,
                DatabaseHelper.SkillDefinitions.Persuasion,
                DatabaseHelper.SkillDefinitions.Nature,
                DatabaseHelper.SkillDefinitions.Religion);
    }

    private static void BuildEquipment([NotNull] CharacterClassDefinitionBuilder classBuilder)
    {
        classBuilder
            .AddEquipmentRow(
                Column(
                    Option(ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                    Option(ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1)),
                Column(
                    Option(ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1)))
            .AddEquipmentRow(
                Column(Option(ItemDefinitions.ScholarPack, EquipmentDefinitions.OptionStarterPack, 1)),
                Column(Option(ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1)))
            .AddEquipmentRow(
                Column(Option(ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1)),
                Column(Option(ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionArcaneFocusChoice, 1)))
            .AddEquipmentRow(
                Option(ItemDefinitions.SorcererArmor, EquipmentDefinitions.OptionArmor, 1),
                Option(ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                Option(ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeapon, 1),
                Option(ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1));
    }

    private static void BuildProficiencies()
    {
        FeatureDefinitionProficiencyArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWitchArmor", WitchBaseGuid)
            .SetGuiPresentation("WitchArmorProficiency", Category.Class)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory)
            .AddToDB();

        FeatureDefinitionProficiencyWeapon = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWitchWeapon", WitchBaseGuid)
            .SetGuiPresentation("WitchWeaponProficiency", Category.Class)
            .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.SimpleWeaponCategory)
            .AddToDB();

        FeatureDefinitionProficiencySavingThrow = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWitchSavingthrow", WitchBaseGuid)
            .SetGuiPresentation("WitchSavingthrowProficiency", Category.Class)
            .SetProficiencies(ProficiencyType.SavingThrow, AttributeDefinitions.Charisma,
                AttributeDefinitions.Wisdom)
            .AddToDB();

        FeatureDefinitionPointPoolSkills = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolWitchSkillPoints", WitchBaseGuid)
            .SetGuiPresentation("WitchSkillProficiency", Category.Class)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
            .RestrictChoices(
                SkillDefinitions.Arcana,
                SkillDefinitions.Deception,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Nature,
                SkillDefinitions.Religion)
            .AddToDB();

        FeatureDefinitionPointPoolTools = FeatureDefinitionPointPoolBuilder
            .Create("ProficiencyWitchTool", WitchBaseGuid)
            .SetGuiPresentation("WitchToolProficiency", Category.Class)
            .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
            .RestrictChoices(
                ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();
    }

    private static void BuildSpells()
    {
        // Build our spellCast object containing previously created spell list
        var witchCastingSlots = new List<SlotsByLevelDuplet>
        {
            new()
            {
                Slots = new List<int>
                {
                    2,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 01
            },
            new()
            {
                Slots = new List<int>
                {
                    3,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 02
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    2,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 03
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 04
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    2,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 05
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 06
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    1,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 07
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    2,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 08
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    1,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 09
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    0,
                    0,
                    0,
                    0,
                    0
                },
                Level = 10
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    0,
                    0,
                    0,
                    0
                },
                Level = 11
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    0,
                    0,
                    0,
                    0
                },
                Level = 12
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    1,
                    0,
                    0,
                    0
                },
                Level = 13
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    1,
                    0,
                    0,
                    0
                },
                Level = 14
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    1,
                    1,
                    0,
                    0
                },
                Level = 15
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    1,
                    1,
                    0,
                    0
                },
                Level = 16
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    2,
                    1,
                    1,
                    1,
                    1,
                    0
                },
                Level = 17
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    3,
                    1,
                    1,
                    1,
                    1,
                    0
                },
                Level = 18
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    3,
                    2,
                    1,
                    1,
                    1,
                    0
                },
                Level = 19
            },
            new()
            {
                Slots = new List<int>
                {
                    4,
                    3,
                    3,
                    3,
                    3,
                    2,
                    2,
                    1,
                    1,
                    0
                },
                Level = 20
            }
        };

        FeatureDefinitionCastSpellWitch = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellWitch", WitchBaseGuid)
            .SetGuiPresentation("WitchSpellcasting", Category.Class)
            .SetKnownCantrips(4, 4, 4, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6)
            .SetKnownSpells(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15)
            .SetSlotsPerLevel(witchCastingSlots)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSpellCastingOrigin(CastingOrigin.Class)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellCastingLevel(9)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSpellList(WitchSpellList)
            .SetReplacedSpells(SpellsHelper.FullCasterReplacedSpells)
            .AddToDB();
    }

    private static void BuildRitualCasting()
    {
        var witchRitualCastingMagicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("WitchRitualCastingMagicAffinity", WitchBaseGuid)
            .SetGuiPresentation(Category.Class)
            .SetRitualCasting((RitualCasting)ExtraRitualCasting.Known)
            .AddToDB();

        FeatureDefinitionFeatureSetRitualCasting = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WitchFeatureSetRitualCasting",
                WitchBaseGuid)
            .SetGuiPresentation(Category.Class)
            .SetFeatureSet(witchRitualCastingMagicAffinity,
                FeatureDefinitionActionAffinitys.ActionAffinityWizardRitualCasting)
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

        var burnedFireRes = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityFireResistance, "WitchBurnedFireResistance", WitchBaseGuid)
            .SetGuiPresentation(Category.Class)
            .AddToDB();

        var burnedProduceFlame = FeatureDefinitionBonusCantripsBuilder
            .Create(FeatureDefinitionBonusCantripss.BonusCantripsDomainElementaFire, "WitchBurnedProduceFlame",
                WitchBaseGuid)
            .SetGuiPresentation(Category.Class)
            .SetBonusCantrips(ProduceFlame)
            .AddToDB();

        var burnedCurse = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WitchFeatureSetBurnedCurse",
                WitchBaseGuid)
            .SetGuiPresentation(Category.Class, ProduceFlame.GuiPresentation.SpriteReference)
            .SetFeatureSet(burnedFireRes, burnedProduceFlame)
            .AddToDB();

        var lovelessCharmImmunity = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                "WitchLovelessCharmImmunity", WitchBaseGuid)
            .SetGuiPresentation(Category.Class)
            .AddToDB();

        var lovelessCurse = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WitchFeatureSetLovelessCurse",
                WitchBaseGuid)
            .SetGuiPresentation(Category.Class, CharmPerson.GuiPresentation.SpriteReference)
            .SetFeatureSet(lovelessCharmImmunity)
            .AddToDB();

        // NOTE: I have no idea how to apply a Charisma bonus, so setting the initiative bonus to 3. It seems like only the "Additive" operation works
        var visionsInitiative = FeatureDefinitionAttributeModifierBuilder
            .Create("WitchVisionsInitiative", WitchBaseGuid)
            .SetGuiPresentation(Category.Class)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Initiative, 3)
            .SetModifierAbilityScore(AttributeDefinitions.Charisma)
            .AddToDB();


        var visionsCurse = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetWizardRitualCasting, "WitchFeatureSetVisionsCurse",
                WitchBaseGuid)
            .SetGuiPresentation(Category.Class, Blindness.GuiPresentation.SpriteReference)
            .SetFeatureSet(visionsInitiative)
            .AddToDB();

        var cursesFeatures = new FeatureDefinition[] {burnedCurse, lovelessCurse, visionsCurse};

        FeatureDefinitionFeatureSetWitchCurses = FeatureDefinitionFeatureSetCustomBuilder
            .Create("WitchCurseChoice", WitchBaseGuid)
            .SetGuiPresentation(Category.Feature)
            .SetRequireClassLevels(true)
            .SetLevelFeatures(1, cursesFeatures)
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
        //+ Disorient: 60 feet CON save, -1d6 on attack rolls on fail
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

        // Abate
        var abateConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionShocked, "ConditionAbate", WitchBaseGuid)
            .SetGuiPresentation("Abate", Category.Condition, ConditionShocked.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .AddToDB();

        var abateConditionForm = new ConditionForm {conditionDefinition = abateConditionDefinition};

        var abateEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = abateConditionForm
        };

        var abateEffectDescription = ShockingGrasp.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Distance, 12)
            .SetSavingThrowAbility(AttributeDefinitions.Charisma)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(abateEffectForm);

        var abate = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionAbate", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, ShockingGrasp.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(abateEffectDescription)
            .AddToDB();

        // Apathy
        var apathyConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionCalmedByCalmEmotionsEnemy, "ConditionApathy", WitchBaseGuid)
            .SetGuiPresentation("Apathy", Category.Condition,
                ConditionCalmedByCalmEmotionsEnemy.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .AddToDB();

        var apathyConditionForm = new ConditionForm {conditionDefinition = apathyConditionDefinition};

        var apathyEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = apathyConditionForm
        };

        var apathyEffectDescription = CalmEmotionsOnEnemy.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Distance, 12)
            .SetSavingThrowAbility(AttributeDefinitions.Charisma)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(apathyEffectForm);

        var apathy = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionApathy", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, CalmEmotions.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(apathyEffectDescription)
            .AddToDB();

        // Charm
        var charmConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionCharmed, "ConditionCharm", WitchBaseGuid)
            .SetGuiPresentation("Charm", Category.Condition,
                ConditionDefinitions.ConditionCharmed.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .AddToDB();

        var charmConditionForm = new ConditionForm {conditionDefinition = charmConditionDefinition};

        var charmEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = charmConditionForm
        };

        var charmEffectDescription = CharmPerson.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Distance, 12)
            .SetSavingThrowAbility(AttributeDefinitions.Charisma)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(charmEffectForm);

        var charm = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionCharm", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, CharmPerson.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(charmEffectDescription)
            .AddToDB();

        // Disorient
        var disorientCombatAffinity = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinityBaned, "CombatAffinityDisorient",
                WitchBaseGuid)
            .SetGuiPresentation("Disorient", Category.Modifier)
            .SetMyAttackModifierDieType(DieType.D6)
            .AddToDB();

        var disorientConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionBaned, "ConditionDisorient", WitchBaseGuid)
            .SetGuiPresentation("Disorient", Category.Condition, ConditionBaned.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .SetFeatures(disorientCombatAffinity)
            .AddToDB();

        var disorientConditionForm = new ConditionForm {conditionDefinition = disorientConditionDefinition};

        var disorientEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = disorientConditionForm
        };

        var disorientEffectDescription = Bane.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Distance, 12)
            .SetSavingThrowAbility(AttributeDefinitions.Constitution)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(disorientEffectForm);

        var disorient = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionDisorient", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, Bane.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(disorientEffectDescription)
            .AddToDB();

        // Evil Eye
        var evileyeConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFrightenedFear, "ConditionEvilEye", WitchBaseGuid)
            .SetGuiPresentation("EvilEye", Category.Condition,
                ConditionDefinitions.ConditionFrightenedFear.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .ClearRecurrentEffectForms()
            .AddToDB();

        var evileyeConditionForm = new ConditionForm {conditionDefinition = evileyeConditionDefinition};

        var evileyeEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = evileyeConditionForm
        };

        var evileyeEffectDescription = Fear.EffectDescription.Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Distance, 12)
            .SetSavingThrowAbility(AttributeDefinitions.Wisdom)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(evileyeEffectForm);

        var evileye = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionEvilEye", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, Fear.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(evileyeEffectDescription)
            .AddToDB();

        // Obfuscate
        var obfuscateEffectDescription = FogCloud.EffectDescription
            .Copy()
            .SetCanBePlacedOnCharacter(true)
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetRange(RangeType.Self);

        var obfuscate = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionObfuscate", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, FogCloud.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(obfuscateEffectDescription)
            .AddToDB();

        // Pox
        var poxConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionPoisoned, "ConditionPox", WitchBaseGuid)
            .SetGuiPresentation("Pox", Category.Condition,
                ConditionDefinitions.ConditionPoisoned.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .ClearRecurrentEffectForms()
            .AddToDB();

        var poxConditionForm = new ConditionForm {conditionDefinition = poxConditionDefinition};

        var poxEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = poxConditionForm
        };

        var poxEffectDescription = PoisonSpray.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Touch, 1)
            .SetSavingThrowAbility(AttributeDefinitions.Constitution)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(poxEffectForm);

        var pox = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionPox", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, PoisonSpray.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(poxEffectDescription)
            .AddToDB();

        // Ruin
        var ruinAttributeModifier = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierRuin", WitchBaseGuid)
            .SetGuiPresentation(Category.Modifier)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, -3)
            .AddToDB();

        var ruinConditionDefinition = ConditionDefinitionBuilder
            .Create(ConditionAcidArrowed, "ConditionRuin", WitchBaseGuid)
            .SetGuiPresentation("Ruin", Category.Condition, ConditionAcidArrowed.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .AddConditionTags("Malediction")
            .ClearRecurrentEffectForms()
            .SetFeatures(ruinAttributeModifier)
            .AddToDB();

        var ruinConditionForm = new ConditionForm {conditionDefinition = ruinConditionDefinition};

        var ruinEffectForm = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            createdByCharacter = true,
            conditionForm = ruinConditionForm
        };

        var ruinEffectDescription = AcidArrow.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(true)
            .SetRange(RangeType.Distance, 12)
            .SetSavingThrowAbility(AttributeDefinitions.Constitution)
            .SetTargetParameter(1)
            .SetTargetType(TargetType.Individuals)
            .SetEffectForms(ruinEffectForm);

        var ruin = FeatureDefinitionPowerBuilder
            .Create("WitchMaledictionRuin", WitchBaseGuid)
            .SetActivation(ActivationTime.Action, 0)
            .SetGuiPresentation(Category.Class, AcidArrow.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(ruinEffectDescription)
            .AddToDB();

        var maledictionsFeatures =
            new FeatureDefinition[] {abate, apathy, charm, disorient, evileye, obfuscate, pox, ruin};

        FeatureDefinitionFeatureSetMaledictions = FeatureDefinitionFeatureSetCustomBuilder
            .Create("WitchMaledictionChoice", WitchBaseGuid)
            .SetGuiPresentation(Category.Feature)
            .SetRequireClassLevels(true)
            .SetLevelFeatures(1, maledictionsFeatures)
            .AddToDB();
    }

    private static void BuildCackle()
    {
        var effectDescription = HideousLaughter.EffectDescription
            .Copy()
            .SetDuration(DurationType.Round, 1)
            .SetEndOfEffect(TurnOccurenceType.EndOfTurn)
            .SetHasSavingThrow(false)
            .SetRange(RangeType.Self)
            .SetTargetType(TargetType.Sphere)
            .SetTargetParameter(12)
            .ClearRestrictedCreatureFamilies()
            .SetEffectForms(new CackleEffectForm());

        FeatureDefinitionPowerCackle = FeatureDefinitionPowerBuilder
            .Create("WitchCacklePower", WitchBaseGuid)
            .SetActivation(ActivationTime.BonusAction, 0)
            .SetGuiPresentation(Category.Class, HideousLaughter.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            .SetEffectDescription(effectDescription)
            .AddToDB();
    }

    private static CharacterClassDefinition BuildAndAddClass()
    {
        var classBuilder = CharacterClassDefinitionBuilder
            .Create("ClassWitch", WitchBaseGuid)
            .SetGuiPresentation("Witch", Category.Class, Sorcerer.GuiPresentation.SpriteReference);

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
            var witchAttackDefinition = MonsterAttackDefinitionBuilder
                .Create(MonsterAttackDefinitions.Attack_EagleMatriarch_Talons, "AttackWitchOwlTalons",
                    WitchBaseGuid).AddToDB();
            var witchFamiliarAttackIteration = new MonsterAttackIteration(witchAttackDefinition, 1)
            {
                MonsterAttackDefinition =
                {
                    // We remove the inherent bonus as we will be using the Witch's spell attack bonus
                    toHitBonus = 0
                }
            };

            var damageForm = witchFamiliarAttackIteration.MonsterAttackDefinition.EffectDescription.EffectForms[0]
                .DamageForm;

            damageForm.diceNumber = 1;
            damageForm.dieType = DieType.D1;
            damageForm.bonusDamage = 0;

            var witchFamiliarMonsterBuilder = MonsterDefinitionBuilder
                .Create(Eagle_Matriarch, "WitchOwl", WitchBaseGuid)
                .SetGuiPresentation("WitchOwlFamiliar", Category.Monster,
                    Eagle_Matriarch.GuiPresentation.SpriteReference)
                .SetFeatures(
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
                .SetAttackIterations(witchFamiliarAttackIteration)
                .SetSkillScores(
                    (DatabaseHelper.SkillDefinitions.Perception.Name, 3),
                    (DatabaseHelper.SkillDefinitions.Stealth.Name, 3))
                .SetArmorClass(11)
                .SetAbilityScores(3, 13, 8, 2, 12, 7)
                .SetHitDice(DieType.D4, 1)
                .SetHitPointsBonus(-1)
                .SetStandardHitPoints(1)
                .SetSizeDefinition(CharacterSizeDefinitions.Tiny)
                .SetAlignment(AlignmentDefinitions.Neutral.Name)
                .SetCharacterFamily(CharacterFamilyDefinitions.Fey.name)
                .SetChallengeRating(0)
                .SetDroppedLootDefinition(null)
                .SetDefaultBattleDecisionPackage(DecisionPackageDefinitions
                    .DefaultSupportCasterWithBackupAttacksDecisions)
                .SetFullyControlledWhenAllied(true)
                .SetDefaultFaction("Party")
                .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

            if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out var help))
            {
                witchFamiliarMonsterBuilder.AddFeatures(help);
            }

            var witchFamiliarMonster = witchFamiliarMonsterBuilder.AddToDB();
            witchFamiliarMonster.CreatureTags.Add("WitchFamiliar");

            var spell = SpellDefinitionBuilder
                .Create(Fireball, "WitchFamiliar", WitchBaseGuid)
                .SetGuiPresentation(Category.Spell, AnimalFriendship.GuiPresentation.SpriteReference)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
                .SetMaterialComponent(MaterialComponentType.None)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSpellLevel(1)
                .SetCastingTime(ActivationTime.Hours1)
                // BUG: Unable to have 70 minutes ritual casting time... if set to 10 minutes, it really only takes 10 minutes, instead of 70
                .SetRitualCasting(ActivationTime.Hours1)
                .SetUniqueInstance()
                .SetEffectDescription(
                    ConjureAnimalsOneBeast.EffectDescription.Copy()
                        .SetRange(RangeType.Distance, 2)
                        .SetDurationType(DurationType.Permanent)
                        .SetTargetSide(Side.Ally)
                )
                .AddToDB();

            var summonForm = new SummonForm {monsterDefinitionName = witchFamiliarMonster.name, decisionPackage = null};

            var effectForm = new EffectForm
            {
                formType = EffectForm.EffectFormType.Summon, createdByCharacter = true, summonForm = summonForm
            };

            spell.EffectDescription.SetEffectForms(effectForm);
            GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, spell);

            var preparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create("WitchFamiliarAutoPreparedSpell", WitchBaseGuid)
                .SetGuiPresentation("WitchFamiliarPower", Category.Class,
                    AnimalFriendship.GuiPresentation.SpriteReference)
                .SetPreparedSpellGroups(BuildSpellGroup(2, spell))
                .SetCastingClass(witch)
                .SetAutoTag("Witch")
                .AddToDB();

            var acConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondAC, "ConditionWitchFamiliarAC", WitchBaseGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var stConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondSavingThrows, "ConditionWitchFamiliarST", WitchBaseGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var damageConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondMeleeDamage, "ConditionWitchFamiliarDamage", WitchBaseGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
                .AddToDB();

            var hitConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondMeleeAttack, "ConditionWitchFamiliarHit", WitchBaseGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceSpellAttack)
                .AddToDB();

            var hpConditionDefinition = ConditionDefinitionBuilder
                .Create(ConditionKindredSpiritBondHP, "ConditionWitchFamiliarHP", WitchBaseGuid)
                .SetGuiPresentationNoContent()
                .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
                .SetAllowMultipleInstances(true)
                .AddToDB();

            // Find a better place to put this in?
            hpConditionDefinition.additionalDamageType = "ClassWitch";

            var summoningAffinity = FeatureDefinitionSummoningAffinityBuilder
                .Create(FeatureDefinitionSummoningAffinitys.SummoningAffinityKindredSpiritBond,
                    "SummoningAffinityWitchFamiliar", WitchBaseGuid)
                .ClearEffectForms()
                .SetRequiredMonsterTag("WitchFamiliar")
                .SetAddedConditions(
                    acConditionDefinition, stConditionDefinition, damageConditionDefinition,
                    hitConditionDefinition, hpConditionDefinition, hpConditionDefinition)
                .AddToDB();

            FeatureDefinitionFeatureSetWitchFamiliar = FeatureDefinitionFeatureSetBuilder
                .Create(FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, "FeatureSetWitchFamiliar",
                    WitchBaseGuid)
                .SetGuiPresentation("WitchFamiliarPower", Category.Class)
                .SetFeatureSet(preparedSpells, summoningAffinity)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(true)
                .AddToDB();
        }

        void BuildSubclasses()
        {
            var subclassChoices = FeatureDefinitionSubclassChoiceBuilder
                .Create("SubclassChoiceWitchCovens", WitchBaseGuid)
                .SetGuiPresentation("WitchSubclassPath", Category.Subclass)
                .SetSubclassSuffix("Coven")
                .SetFilterByDeity(false)
                .SetSubclasses(
                    GreenWitch.GetSubclass(witch),
                    RedWitch.GetSubclass(witch),
                    WhiteWitch.GetSubclass(witch))
                .AddToDB();

            classBuilder.AddFeatureAtLevel(3, subclassChoices);
        }

        void BuildProgression()
        {
            if (DatabaseRepository.GetDatabase<FeatureDefinition>().TryGetElement("HelpAction", out var help))
            {
                classBuilder.AddFeatureAtLevel(1, help);
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
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSetMaledictions);

            classBuilder.AddFeaturesAtLevel(3,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(4,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            classBuilder.AddFeaturesAtLevel(5,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSetMaledictions);

            classBuilder.AddFeaturesAtLevel(6,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(7,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(8,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            classBuilder.AddFeaturesAtLevel(9,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSetMaledictions);

            classBuilder.AddFeaturesAtLevel(10,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(11,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(12,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            classBuilder.AddFeaturesAtLevel(13,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSetMaledictions);

            classBuilder.AddFeaturesAtLevel(14,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(15,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(16,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            classBuilder.AddFeaturesAtLevel(17,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSetMaledictions);

            classBuilder.AddFeaturesAtLevel(18,
                WitchMaledictionReplacer);

            classBuilder.AddFeaturesAtLevel(19,
                WitchMaledictionReplacer,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            classBuilder.AddFeaturesAtLevel(20,
                WitchMaledictionReplacer);

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

    private sealed class CackleEffectForm : CustomEffectForm
    {
        public override void ApplyForm(
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly,
            EffectApplication effectApplication = EffectApplication.All,
            [CanBeNull] List<EffectFormFilter> filters = null)
        {
            var conditions = formsParams.targetCharacter.AllConditions;

            var activeMaledictions = conditions
                .Where(i => i.ConditionDefinition.ConditionTags.Contains("Malediction")).ToList();

            foreach (var malediction in activeMaledictions)
            {
                // Remove the condition in order to refresh it
                formsParams.targetCharacter.RemoveCondition(malediction);
                // Refresh the condition
                ApplyCondition(formsParams, malediction.ConditionDefinition, DurationType.Round, 1);
            }
        }

        public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
            // Nothing
        }

        private static void ApplyCondition(RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            [NotNull] ConditionDefinition condition, DurationType durationType, int durationParam)
        {
            // Prepare params for inflicting conditions
            var sourceGuid = formsParams.sourceCharacter?.Guid ?? 0L;
            var sourceFaction = formsParams.sourceCharacter != null
                ? formsParams.sourceCharacter.CurrentFaction.Name
                : string.Empty;
            var effectDefinitionName = string.Empty;

            if (formsParams.attackMode != null)
            {
                effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
            }
            else if (formsParams.activeEffect != null)
            {
                effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
            }

            var sourceAbilityBonus =
                formsParams.activeEffect?.ComputeSourceAbilityBonus(formsParams.sourceCharacter) ?? 0;

            formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam,
                TurnOccurenceType.EndOfTurn, AttributeDefinitions.TagEffect, sourceGuid, sourceFaction,
                formsParams.effectLevel,
                effectDefinitionName, 0, sourceAbilityBonus);
        }
    }

    #region WitchMaledictionReplacer

    private static FeatureDefinitionFeatureSetReplaceCustom _witchMaledictionReplacer;

    private static FeatureDefinitionFeatureSetReplaceCustom WitchMaledictionReplacer =>
        _witchMaledictionReplacer ??= FeatureDefinitionFeatureSetReplaceCustomBuilder
            .Create("ClassWitchMaledictionReplace", WitchBaseGuid)
            .SetGuiPresentation(Category.Feature)
            .SetReplacedFeatureSet(FeatureDefinitionFeatureSetMaledictions)
            .AddToDB();

    #endregion
}
