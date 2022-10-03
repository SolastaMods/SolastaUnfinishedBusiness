// this file is manually maintained
// whenever you use a definition in DEBUG mode copy it over here
// goal is to increase mod boot time and reduce total payload

namespace SolastaUnfinishedBusiness.Api;

internal static partial class DatabaseHelper
{
    internal static class ArmorCategoryDefinitions
    {
        internal static ArmorCategoryDefinition HeavyArmorCategory { get; } =
            GetDefinition<ArmorCategoryDefinition>("HeavyArmorCategory");

        internal static ArmorCategoryDefinition LightArmorCategory { get; } =
            GetDefinition<ArmorCategoryDefinition>("LightArmorCategory");

        internal static ArmorCategoryDefinition MediumArmorCategory { get; } =
            GetDefinition<ArmorCategoryDefinition>("MediumArmorCategory");

        internal static ArmorCategoryDefinition NoArmorCategory { get; } =
            GetDefinition<ArmorCategoryDefinition>("NoArmorCategory");

        internal static ArmorCategoryDefinition ShieldCategory { get; } =
            GetDefinition<ArmorCategoryDefinition>("ShieldCategory");
    }

    internal static class BestiaryStatsDefinitions
    {
    }

    internal static class BestiaryTableDefinitions
    {
    }

    internal static class CharacterBackgroundDefinitions
    {
        internal static CharacterBackgroundDefinition Academic { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Academic");

        internal static CharacterBackgroundDefinition Academic_Aristocrat_QA_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Academic_Aristocrat_QA_Background");

        internal static CharacterBackgroundDefinition Acolyte { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Acolyte");

        internal static CharacterBackgroundDefinition Acolyte_Lawkeeper_QA_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Acolyte_Lawkeeper_QA_Background");

        internal static CharacterBackgroundDefinition Aescetic_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Aescetic_Background");

        internal static CharacterBackgroundDefinition Aristocrat { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Aristocrat");

        internal static CharacterBackgroundDefinition Artist_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Artist_Background");

        internal static CharacterBackgroundDefinition KS_Garrad_Miles { get; } =
            GetDefinition<CharacterBackgroundDefinition>("KS_Garrad_Miles");

        internal static CharacterBackgroundDefinition KS_Ruad_Swifthand { get; } =
            GetDefinition<CharacterBackgroundDefinition>("KS_Ruad_Swifthand");

        internal static CharacterBackgroundDefinition KS_VigdisKaikonnen { get; } =
            GetDefinition<CharacterBackgroundDefinition>("KS_VigdisKaikonnen");

        internal static CharacterBackgroundDefinition KS_VioletGoodcheer { get; } =
            GetDefinition<CharacterBackgroundDefinition>("KS_VioletGoodcheer");

        internal static CharacterBackgroundDefinition Lawkeeper { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Lawkeeper");

        internal static CharacterBackgroundDefinition Lowlife { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Lowlife");

        internal static CharacterBackgroundDefinition Lowlife_Sellsword_QA_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Lowlife_Sellsword_QA_Background");

        internal static CharacterBackgroundDefinition Occultist_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Occultist_Background");

        internal static CharacterBackgroundDefinition Philosopher { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Philosopher");

        internal static CharacterBackgroundDefinition SellSword { get; } =
            GetDefinition<CharacterBackgroundDefinition>("SellSword");

        internal static CharacterBackgroundDefinition Spy { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Spy");

        internal static CharacterBackgroundDefinition Spy_Philosopher_QA_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Spy_Philosopher_QA_Background");

        internal static CharacterBackgroundDefinition Wanderer { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Wanderer");
    }

    internal static class CharacterClassDefinitions
    {
        internal static CharacterClassDefinition Barbarian { get; } =
            GetDefinition<CharacterClassDefinition>("Barbarian");

        internal static CharacterClassDefinition Bard { get; } = GetDefinition<CharacterClassDefinition>("Bard");
        internal static CharacterClassDefinition Cleric { get; } = GetDefinition<CharacterClassDefinition>("Cleric");
        internal static CharacterClassDefinition Druid { get; } = GetDefinition<CharacterClassDefinition>("Druid");
        internal static CharacterClassDefinition Fighter { get; } = GetDefinition<CharacterClassDefinition>("Fighter");
        internal static CharacterClassDefinition Monk { get; } = GetDefinition<CharacterClassDefinition>("Monk");
        internal static CharacterClassDefinition Paladin { get; } = GetDefinition<CharacterClassDefinition>("Paladin");
        internal static CharacterClassDefinition Ranger { get; } = GetDefinition<CharacterClassDefinition>("Ranger");
        internal static CharacterClassDefinition Rogue { get; } = GetDefinition<CharacterClassDefinition>("Rogue");

        internal static CharacterClassDefinition Sorcerer { get; } =
            GetDefinition<CharacterClassDefinition>("Sorcerer");

        internal static CharacterClassDefinition Warlock { get; } = GetDefinition<CharacterClassDefinition>("Warlock");
        internal static CharacterClassDefinition Wizard { get; } = GetDefinition<CharacterClassDefinition>("Wizard");
    }

    internal static class CharacterFamilyDefinitions
    {
        internal static CharacterFamilyDefinition Aberration { get; } =
            GetDefinition<CharacterFamilyDefinition>("Aberration");

        internal static CharacterFamilyDefinition Beast { get; } = GetDefinition<CharacterFamilyDefinition>("Beast");

        internal static CharacterFamilyDefinition Celestial { get; } =
            GetDefinition<CharacterFamilyDefinition>("Celestial");

        internal static CharacterFamilyDefinition Construct { get; } =
            GetDefinition<CharacterFamilyDefinition>("Construct");

        internal static CharacterFamilyDefinition Dragon { get; } = GetDefinition<CharacterFamilyDefinition>("Dragon");

        internal static CharacterFamilyDefinition Elemental { get; } =
            GetDefinition<CharacterFamilyDefinition>("Elemental");

        internal static CharacterFamilyDefinition Fey { get; } = GetDefinition<CharacterFamilyDefinition>("Fey");
        internal static CharacterFamilyDefinition Fiend { get; } = GetDefinition<CharacterFamilyDefinition>("Fiend");
        internal static CharacterFamilyDefinition Giant { get; } = GetDefinition<CharacterFamilyDefinition>("Giant");

        internal static CharacterFamilyDefinition Giant_Rugan { get; } =
            GetDefinition<CharacterFamilyDefinition>("Giant_Rugan");

        internal static CharacterFamilyDefinition Humanoid { get; } =
            GetDefinition<CharacterFamilyDefinition>("Humanoid");

        internal static CharacterFamilyDefinition Monstrosity { get; } =
            GetDefinition<CharacterFamilyDefinition>("Monstrosity");

        internal static CharacterFamilyDefinition Ooze { get; } = GetDefinition<CharacterFamilyDefinition>("Ooze");
        internal static CharacterFamilyDefinition Plant { get; } = GetDefinition<CharacterFamilyDefinition>("Plant");
        internal static CharacterFamilyDefinition Undead { get; } = GetDefinition<CharacterFamilyDefinition>("Undead");
    }

    internal static class CharacterRaceDefinitions
    {
        internal static CharacterRaceDefinition Dragonborn { get; } =
            GetDefinition<CharacterRaceDefinition>("Dragonborn");

        internal static CharacterRaceDefinition Dwarf { get; } = GetDefinition<CharacterRaceDefinition>("Dwarf");

        internal static CharacterRaceDefinition DwarfHill { get; } =
            GetDefinition<CharacterRaceDefinition>("DwarfHill");

        internal static CharacterRaceDefinition DwarfSnow { get; } =
            GetDefinition<CharacterRaceDefinition>("DwarfSnow");

        internal static CharacterRaceDefinition Elf { get; } = GetDefinition<CharacterRaceDefinition>("Elf");
        internal static CharacterRaceDefinition ElfHigh { get; } = GetDefinition<CharacterRaceDefinition>("ElfHigh");

        internal static CharacterRaceDefinition ElfSylvan { get; } =
            GetDefinition<CharacterRaceDefinition>("ElfSylvan");

        internal static CharacterRaceDefinition Gnome { get; } = GetDefinition<CharacterRaceDefinition>("Gnome");

        internal static CharacterRaceDefinition GnomeRock { get; } =
            GetDefinition<CharacterRaceDefinition>("GnomeRock");

        internal static CharacterRaceDefinition GnomeShadow { get; } =
            GetDefinition<CharacterRaceDefinition>("GnomeShadow");

        internal static CharacterRaceDefinition HalfElf { get; } = GetDefinition<CharacterRaceDefinition>("HalfElf");
        internal static CharacterRaceDefinition Halfling { get; } = GetDefinition<CharacterRaceDefinition>("Halfling");

        internal static CharacterRaceDefinition HalflingIsland { get; } =
            GetDefinition<CharacterRaceDefinition>("HalflingIsland");

        internal static CharacterRaceDefinition HalflingMarsh { get; } =
            GetDefinition<CharacterRaceDefinition>("HalflingMarsh");

        internal static CharacterRaceDefinition HalfOrc { get; } = GetDefinition<CharacterRaceDefinition>("HalfOrc");
        internal static CharacterRaceDefinition Human { get; } = GetDefinition<CharacterRaceDefinition>("Human");
        internal static CharacterRaceDefinition Tiefling { get; } = GetDefinition<CharacterRaceDefinition>("Tiefling");
    }

    internal static class CharacterSizeDefinitions
    {
        internal static CharacterSizeDefinition DragonSize { get; } =
            GetDefinition<CharacterSizeDefinition>("DragonSize");

        internal static CharacterSizeDefinition Gargantuan { get; } =
            GetDefinition<CharacterSizeDefinition>("Gargantuan");

        internal static CharacterSizeDefinition Huge { get; } = GetDefinition<CharacterSizeDefinition>("Huge");
        internal static CharacterSizeDefinition Large { get; } = GetDefinition<CharacterSizeDefinition>("Large");

        internal static CharacterSizeDefinition LargeFlat { get; } =
            GetDefinition<CharacterSizeDefinition>("LargeFlat");

        internal static CharacterSizeDefinition Medium { get; } = GetDefinition<CharacterSizeDefinition>("Medium");
        internal static CharacterSizeDefinition Small { get; } = GetDefinition<CharacterSizeDefinition>("Small");

        internal static CharacterSizeDefinition SpiderQueenSize { get; } =
            GetDefinition<CharacterSizeDefinition>("SpiderQueenSize");

        internal static CharacterSizeDefinition Tiny { get; } = GetDefinition<CharacterSizeDefinition>("Tiny");
    }

    internal static class CharacterSubclassDefinitions
    {
        internal static CharacterSubclassDefinition CircleBalance { get; } =
            GetDefinition<CharacterSubclassDefinition>("CircleBalance");

        internal static CharacterSubclassDefinition CircleKindred { get; } =
            GetDefinition<CharacterSubclassDefinition>("CircleKindred");

        internal static CharacterSubclassDefinition CircleLand { get; } =
            GetDefinition<CharacterSubclassDefinition>("CircleLand");

        internal static CharacterSubclassDefinition CircleWinds { get; } =
            GetDefinition<CharacterSubclassDefinition>("CircleWinds");

        internal static CharacterSubclassDefinition CollegeOfHeroism { get; } =
            GetDefinition<CharacterSubclassDefinition>("CollegeOfHeroism");

        internal static CharacterSubclassDefinition CollegeOfHope { get; } =
            GetDefinition<CharacterSubclassDefinition>("CollegeOfHope");

        internal static CharacterSubclassDefinition CollegeOfLore { get; } =
            GetDefinition<CharacterSubclassDefinition>("CollegeOfLore");

        internal static CharacterSubclassDefinition CollegeOfTraditions { get; } =
            GetDefinition<CharacterSubclassDefinition>("CollegeOfTraditions");

        internal static CharacterSubclassDefinition DomainBattle { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainBattle");

        internal static CharacterSubclassDefinition DomainElementalCold { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainElementalCold");

        internal static CharacterSubclassDefinition DomainElementalFire { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainElementalFire");

        internal static CharacterSubclassDefinition DomainElementalLighting { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainElementalLighting");

        internal static CharacterSubclassDefinition DomainInsight { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainInsight");

        internal static CharacterSubclassDefinition DomainLaw { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainLaw");

        internal static CharacterSubclassDefinition DomainLife { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainLife");

        internal static CharacterSubclassDefinition DomainMischief { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainMischief");

        internal static CharacterSubclassDefinition DomainOblivion { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainOblivion");

        internal static CharacterSubclassDefinition DomainSun { get; } =
            GetDefinition<CharacterSubclassDefinition>("DomainSun");

        internal static CharacterSubclassDefinition MartialChampion { get; } =
            GetDefinition<CharacterSubclassDefinition>("MartialChampion");

        internal static CharacterSubclassDefinition MartialCommander { get; } =
            GetDefinition<CharacterSubclassDefinition>("MartialCommander");

        internal static CharacterSubclassDefinition MartialMountaineer { get; } =
            GetDefinition<CharacterSubclassDefinition>("MartialMountaineer");

        internal static CharacterSubclassDefinition MartialSpellblade { get; } =
            GetDefinition<CharacterSubclassDefinition>("MartialSpellblade");

        internal static CharacterSubclassDefinition OathOfDevotion { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfDevotion");

        internal static CharacterSubclassDefinition OathOfJugement { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfJugement");

        internal static CharacterSubclassDefinition OathOfTheMotherland { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfTheMotherland");

        internal static CharacterSubclassDefinition OathOfTirmar { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfTirmar");

        internal static CharacterSubclassDefinition PathBerserker { get; } =
            GetDefinition<CharacterSubclassDefinition>("PathBerserker");

        internal static CharacterSubclassDefinition PathClaw { get; } =
            GetDefinition<CharacterSubclassDefinition>("PathClaw");

        internal static CharacterSubclassDefinition PathMagebane { get; } =
            GetDefinition<CharacterSubclassDefinition>("PathMagebane");

        internal static CharacterSubclassDefinition PathStone { get; } =
            GetDefinition<CharacterSubclassDefinition>("PathStone");

        internal static CharacterSubclassDefinition PatronFiend { get; } =
            GetDefinition<CharacterSubclassDefinition>("PatronFiend");

        internal static CharacterSubclassDefinition PatronHive { get; } =
            GetDefinition<CharacterSubclassDefinition>("PatronHive");

        internal static CharacterSubclassDefinition PatronTimekeeper { get; } =
            GetDefinition<CharacterSubclassDefinition>("PatronTimekeeper");

        internal static CharacterSubclassDefinition PatronTree { get; } =
            GetDefinition<CharacterSubclassDefinition>("PatronTree");

        internal static CharacterSubclassDefinition RangerHunter { get; } =
            GetDefinition<CharacterSubclassDefinition>("RangerHunter");

        internal static CharacterSubclassDefinition RangerMarksman { get; } =
            GetDefinition<CharacterSubclassDefinition>("RangerMarksman");

        internal static CharacterSubclassDefinition RangerShadowTamer { get; } =
            GetDefinition<CharacterSubclassDefinition>("RangerShadowTamer");

        internal static CharacterSubclassDefinition RangerSwiftBlade { get; } =
            GetDefinition<CharacterSubclassDefinition>("RangerSwiftBlade");

        internal static CharacterSubclassDefinition RoguishDarkweaver { get; } =
            GetDefinition<CharacterSubclassDefinition>("RoguishDarkweaver");

        internal static CharacterSubclassDefinition RoguishHoodlum { get; } =
            GetDefinition<CharacterSubclassDefinition>("RoguishHoodlum");

        internal static CharacterSubclassDefinition RoguishShadowCaster { get; } =
            GetDefinition<CharacterSubclassDefinition>("RoguishShadowCaster");

        internal static CharacterSubclassDefinition RoguishThief { get; } =
            GetDefinition<CharacterSubclassDefinition>("RoguishThief");

        internal static CharacterSubclassDefinition SorcerousChildRift { get; } =
            GetDefinition<CharacterSubclassDefinition>("SorcerousChildRift");

        internal static CharacterSubclassDefinition SorcerousDraconicBloodline { get; } =
            GetDefinition<CharacterSubclassDefinition>("SorcerousDraconicBloodline");

        internal static CharacterSubclassDefinition SorcerousHauntedSoul { get; } =
            GetDefinition<CharacterSubclassDefinition>("SorcerousHauntedSoul");

        internal static CharacterSubclassDefinition SorcerousManaPainter { get; } =
            GetDefinition<CharacterSubclassDefinition>("SorcerousManaPainter");

        internal static CharacterSubclassDefinition TraditionCourtMage { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionCourtMage");

        internal static CharacterSubclassDefinition TraditionFreedom { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionFreedom");

        internal static CharacterSubclassDefinition TraditionGreenmage { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionGreenmage");

        internal static CharacterSubclassDefinition TraditionLight { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionLight");

        internal static CharacterSubclassDefinition TraditionLoremaster { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionLoremaster");

        internal static CharacterSubclassDefinition TraditionOpenHand { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionOpenHand");

        internal static CharacterSubclassDefinition TraditionShockArcanist { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionShockArcanist");

        internal static CharacterSubclassDefinition TraditionSurvival { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionSurvival");
    }


    internal static class ConsoleTableDefinitions
    {
    }

    internal static class CreditsTableDefinitions
    {
    }

    internal static class DailyLogDefinitions
    {
    }

    internal static class DamageDefinitions
    {
        internal static DamageDefinition DamageAcid { get; } = GetDefinition<DamageDefinition>("DamageAcid");

        internal static DamageDefinition DamageBludgeoning { get; } =
            GetDefinition<DamageDefinition>("DamageBludgeoning");

        internal static DamageDefinition DamageCold { get; } = GetDefinition<DamageDefinition>("DamageCold");
        internal static DamageDefinition DamageFire { get; } = GetDefinition<DamageDefinition>("DamageFire");
        internal static DamageDefinition DamageForce { get; } = GetDefinition<DamageDefinition>("DamageForce");
        internal static DamageDefinition DamageLightning { get; } = GetDefinition<DamageDefinition>("DamageLightning");
        internal static DamageDefinition DamageNecrotic { get; } = GetDefinition<DamageDefinition>("DamageNecrotic");
        internal static DamageDefinition DamagePiercing { get; } = GetDefinition<DamageDefinition>("DamagePiercing");
        internal static DamageDefinition DamagePoison { get; } = GetDefinition<DamageDefinition>("DamagePoison");
        internal static DamageDefinition DamagePsychic { get; } = GetDefinition<DamageDefinition>("DamagePsychic");
        internal static DamageDefinition DamageRadiant { get; } = GetDefinition<DamageDefinition>("DamageRadiant");
        internal static DamageDefinition DamageSlashing { get; } = GetDefinition<DamageDefinition>("DamageSlashing");
        internal static DamageDefinition DamageThunder { get; } = GetDefinition<DamageDefinition>("DamageThunder");
    }

    internal static class DocumentTableDefinitions
    {
    }

    internal static class FactionStatusDefinitions
    {
        internal static FactionStatusDefinition Alliance { get; } = GetDefinition<FactionStatusDefinition>("Alliance");

        internal static FactionStatusDefinition Animosity { get; } =
            GetDefinition<FactionStatusDefinition>("Animosity");

        internal static FactionStatusDefinition Brotherhood { get; } =
            GetDefinition<FactionStatusDefinition>("Brotherhood");

        internal static FactionStatusDefinition Hatred { get; } = GetDefinition<FactionStatusDefinition>("Hatred");

        internal static FactionStatusDefinition Indifference { get; } =
            GetDefinition<FactionStatusDefinition>("Indifference");

        internal static FactionStatusDefinition LivingLegend { get; } =
            GetDefinition<FactionStatusDefinition>("LivingLegend");

        internal static FactionStatusDefinition Sympathy { get; } = GetDefinition<FactionStatusDefinition>("Sympathy");
    }

    internal static class FeatDefinitions
    {
        internal static FeatDefinition Ambidextrous { get; } = GetDefinition<FeatDefinition>("Ambidextrous");
        internal static FeatDefinition ArcaneAppraiser { get; } = GetDefinition<FeatDefinition>("ArcaneAppraiser");
        internal static FeatDefinition ArmorMaster { get; } = GetDefinition<FeatDefinition>("ArmorMaster");
        internal static FeatDefinition BadlandsMarauder { get; } = GetDefinition<FeatDefinition>("BadlandsMarauder");

        internal static FeatDefinition BlessingOfTheElements { get; } =
            GetDefinition<FeatDefinition>("BlessingOfTheElements");

        internal static FeatDefinition BurningTouch { get; } = GetDefinition<FeatDefinition>("BurningTouch");
        internal static FeatDefinition CloakAndDagger { get; } = GetDefinition<FeatDefinition>("CloakAndDagger");
        internal static FeatDefinition Creed_Of_Arun { get; } = GetDefinition<FeatDefinition>("Creed_Of_Arun");
        internal static FeatDefinition Creed_Of_Einar { get; } = GetDefinition<FeatDefinition>("Creed_Of_Einar");
        internal static FeatDefinition Creed_Of_Maraike { get; } = GetDefinition<FeatDefinition>("Creed_Of_Maraike");
        internal static FeatDefinition Creed_Of_Misaye { get; } = GetDefinition<FeatDefinition>("Creed_Of_Misaye");
        internal static FeatDefinition Creed_Of_Pakri { get; } = GetDefinition<FeatDefinition>("Creed_Of_Pakri");
        internal static FeatDefinition Creed_Of_Solasta { get; } = GetDefinition<FeatDefinition>("Creed_Of_Solasta");
        internal static FeatDefinition DauntingPush { get; } = GetDefinition<FeatDefinition>("DauntingPush");

        internal static FeatDefinition DiscretionOfTheCoedymwarth { get; } =
            GetDefinition<FeatDefinition>("DiscretionOfTheCoedymwarth");

        internal static FeatDefinition DistractingGambit { get; } = GetDefinition<FeatDefinition>("DistractingGambit");
        internal static FeatDefinition EagerForBattle { get; } = GetDefinition<FeatDefinition>("EagerForBattle");
        internal static FeatDefinition ElectrifyingTouch { get; } = GetDefinition<FeatDefinition>("ElectrifyingTouch");
        internal static FeatDefinition Enduring_Body { get; } = GetDefinition<FeatDefinition>("Enduring_Body");

        internal static FeatDefinition FlawlessConcentration { get; } =
            GetDefinition<FeatDefinition>("FlawlessConcentration");

        internal static FeatDefinition FocusedSleeper { get; } = GetDefinition<FeatDefinition>("FocusedSleeper");
        internal static FeatDefinition FollowUpStrike { get; } = GetDefinition<FeatDefinition>("FollowUpStrike");

        internal static FeatDefinition ForestallingStrength { get; } =
            GetDefinition<FeatDefinition>("ForestallingStrength");

        internal static FeatDefinition ForestRunner { get; } = GetDefinition<FeatDefinition>("ForestRunner");
        internal static FeatDefinition HardToKill { get; } = GetDefinition<FeatDefinition>("HardToKill");
        internal static FeatDefinition Hauler { get; } = GetDefinition<FeatDefinition>("Hauler");
        internal static FeatDefinition IcyTouch { get; } = GetDefinition<FeatDefinition>("IcyTouch");
        internal static FeatDefinition InitiateAlchemist { get; } = GetDefinition<FeatDefinition>("InitiateAlchemist");
        internal static FeatDefinition InitiateEnchanter { get; } = GetDefinition<FeatDefinition>("InitiateEnchanter");
        internal static FeatDefinition Lockbreaker { get; } = GetDefinition<FeatDefinition>("Lockbreaker");
        internal static FeatDefinition Manipulator { get; } = GetDefinition<FeatDefinition>("Manipulator");
        internal static FeatDefinition MasterAlchemist { get; } = GetDefinition<FeatDefinition>("MasterAlchemist");
        internal static FeatDefinition MasterEnchanter { get; } = GetDefinition<FeatDefinition>("MasterEnchanter");
        internal static FeatDefinition MeltingTouch { get; } = GetDefinition<FeatDefinition>("MeltingTouch");
        internal static FeatDefinition Mender { get; } = GetDefinition<FeatDefinition>("Mender");

        internal static FeatDefinition MightOfTheIronLegion { get; } =
            GetDefinition<FeatDefinition>("MightOfTheIronLegion");

        internal static FeatDefinition MightyBlow { get; } = GetDefinition<FeatDefinition>("MightyBlow");
        internal static FeatDefinition PowerfulCantrip { get; } = GetDefinition<FeatDefinition>("PowerfulCantrip");
        internal static FeatDefinition RaiseShield { get; } = GetDefinition<FeatDefinition>("RaiseShield");
        internal static FeatDefinition ReadyOrNot { get; } = GetDefinition<FeatDefinition>("ReadyOrNot");
        internal static FeatDefinition Robust { get; } = GetDefinition<FeatDefinition>("Robust");
        internal static FeatDefinition RushToBattle { get; } = GetDefinition<FeatDefinition>("RushToBattle");

        internal static FeatDefinition SturdinessOfTheTundra { get; } =
            GetDefinition<FeatDefinition>("SturdinessOfTheTundra");

        internal static FeatDefinition TakeAim { get; } = GetDefinition<FeatDefinition>("TakeAim");
        internal static FeatDefinition ToxicTouch { get; } = GetDefinition<FeatDefinition>("ToxicTouch");
        internal static FeatDefinition TripAttack { get; } = GetDefinition<FeatDefinition>("TripAttack");
        internal static FeatDefinition TwinBlade { get; } = GetDefinition<FeatDefinition>("TwinBlade");
        internal static FeatDefinition UncannyAccuracy { get; } = GetDefinition<FeatDefinition>("UncannyAccuracy");
    }

    internal static class FeatureDefinitionAbilityCheckAffinitys
    {
        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinity_ConditionChilled { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinity_ConditionChilled");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinity_ConditionFrozen { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinity_ConditionFrozen");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinity_PalaceOfIce_LairEffect { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinity_PalaceOfIce_LairEffect");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityAmuletOfPureSouls { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityAmuletOfPureSouls");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityArmor_Of_The_Forest { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityArmor_Of_The_Forest");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityArmor_Of_The_Oak { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityArmor_Of_The_Oak");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityArmorOfTheVagrant { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityArmorOfTheVagrant");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBardJackOfAllTrades { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBardJackOfAllTrades");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBeltOfDwarvenkind { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBeltOfDwarvenkind");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBestowCurseCharisma { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBestowCurseCharisma");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBestowCurseConstitution { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBestowCurseConstitution");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBestowCurseDexterity { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBestowCurseDexterity");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBestowCurseIntelligence { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBestowCurseIntelligence");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBestowCurseStrength { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBestowCurseStrength");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBestowCurseWisdom { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBestowCurseWisdom");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityBootsOfElvenkind { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityBootsOfElvenkind");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityChampionRemarkableAthlete { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityChampionRemarkableAthlete");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityCloakOfElvenkind { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityCloakOfElvenkind");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityCloakOfTheAncientKing { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityCloakOfTheAncientKing");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityCollegeLoreCuttingWords { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityCollegeLoreCuttingWords");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionBackFromTheDead1 { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionBackFromTheDead1");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionBackFromTheDead2 { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionBackFromTheDead2");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionBackFromTheDead3 { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionBackFromTheDead3");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionBackFromTheDead4 { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionBackFromTheDead4");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionBearsEndurance { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionBearsEndurance");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionBullsStrength { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionBullsStrength");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionCatsGrace { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionCatsGrace");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionEaglesSplendor { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionEaglesSplendor");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionExhausted { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionExhausted");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionFoxsCunning { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionFoxsCunning");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionOwlsWisdom { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionOwlsWisdom");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionPheromoned { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionPheromoned");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionRaging { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionRaging");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityContagionBlindingSickness { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityContagionBlindingSickness");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityContagionFilthFever { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityContagionFilthFever");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityContagionFleshRot { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityContagionFleshRot");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityContagionMindfire { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityContagionMindfire");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityContagionSeizure { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityContagionSeizure");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityContagionSlimyDoom { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityContagionSlimyDoom");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityCrownOfTheMagister { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityCrownOfTheMagister");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityDomainInsightDivineEye { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityDomainInsightDivineEye");

        internal static FeatureDefinitionAbilityCheckAffinity
            AbilityCheckAffinityDomainInsightDivineIntuition { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityDomainInsightDivineIntuition");

        internal static FeatureDefinitionAbilityCheckAffinity
            AbilityCheckAffinityDomainInsightInspiredDiplomat { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityDomainInsightInspiredDiplomat");

        internal static FeatureDefinitionAbilityCheckAffinity
            AbilityCheckAffinityDomainLawCommandingPresenceIntimidationAdvantage { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>(
                "AbilityCheckAffinityDomainLawCommandingPresenceIntimidationAdvantage");

        internal static FeatureDefinitionAbilityCheckAffinity
            AbilityCheckAffinityDomainLawUnyieldingEnforcerShove { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>(
                "AbilityCheckAffinityDomainLawUnyieldingEnforcerShove");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityDwarfSnowCamouflage { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityDwarfSnowCamouflage");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityDwarvenPlateResistShove { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityDwarvenPlateResistShove");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityEyebiteSickened { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityEyebiteSickened");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityFeatLockbreaker { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityFeatLockbreaker");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityFiendDarkOnesOwnLuck { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityFiendDarkOnesOwnLuck");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityFrightened { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityFrightened");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityGreenmageArmor { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityGreenmageArmor");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityGuided { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityGuided");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityHeatMetal { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityHeatMetal");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityHuntersMark { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityHuntersMark");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityIslandHalflingAcrobatics { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityIslandHalflingAcrobatics");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityKeenHearing { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityKeenHearing");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityKeenSight { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityKeenSight");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityKeenSmell { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityKeenSmell");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityKindredSpiritBond { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityKindredSpiritBond");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityLoremasterKeenMindSkills { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityLoremasterKeenMindSkills");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityMarshHalflingCamouflage { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityMarshHalflingCamouflage");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityOathOfTirmarGoldenSpeech { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityOathOfTirmarGoldenSpeech");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityOneRing { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityOneRing");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityOrcBerserkerShove { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityOrcBerserkerShove");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityPassWithoutTrace { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityPassWithoutTrace");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityPerceptionBonus { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityPerceptionBonus");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityPoisoned { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityPoisoned");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityRangerHideInPlainSight { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityRangerHideInPlainSight");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityRingOfTheAmbassador { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityRingOfTheAmbassador");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinitySaboteurStrengthBonus { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinitySaboteurStrengthBonus");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityScimitarOfTheAnfarels { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityScimitarOfTheAnfarels");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityShadowTamerKnowDarkness { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityShadowTamerKnowDarkness");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityShieldOfCohhCharisma { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityShieldOfCohhCharisma");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinitySpoonOfDiscord { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinitySpoonOfDiscord");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityStaffOfMetisIntelligence { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityStaffOfMetisIntelligence");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityStealthDisadvantage { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityStealthDisadvantage");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityStoneOfGoodLuck { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityStoneOfGoodLuck");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityThiefSupremeSneak { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityThiefSupremeSneak");
    }

    internal static class FeatureDefinitionActionAffinitys
    {
        internal static FeatureDefinitionActionAffinity ActionAffinityAggressive { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityAggressive");

        internal static FeatureDefinitionActionAffinity ActionAffinityBarbarianRage { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityBarbarianRage");

        internal static FeatureDefinitionActionAffinity ActionAffinityBarbarianRecklessAttack { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityBarbarianRecklessAttack");

        internal static FeatureDefinitionActionAffinity ActionAffinityBardicInspiration { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityBardicInspiration");

        internal static FeatureDefinitionActionAffinity ActionAffinityBardRitualCasting { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityBardRitualCasting");

        internal static FeatureDefinitionActionAffinity ActionAffinityBlackTentacles { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityBlackTentacles");

        internal static FeatureDefinitionActionAffinity ActionAffinityClericRitualCasting { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityClericRitualCasting");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionBanished { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionBanished");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionCalmed { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionCalmed");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionCharmed { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionCharmed");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionCharmedByAnimalFriendship { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionCharmedByAnimalFriendship");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionConfused { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionConfused");

        internal static FeatureDefinitionActionAffinity
            ActionAffinityConditionCursedByBestowCurseOnActionTurn { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionCursedByBestowCurseOnActionTurn");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionDazzled { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionDazzled");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionDispellingEvilAndGood { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionDispellingEvilAndGood");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionFrightenedFear { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionFrightenedFear");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionHopeless { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionHopeless");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionIncapacitated { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionIncapacitated");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionInsane { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionInsane");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionLethargic { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionLethargic");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionOnFire { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionOnFire");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionOpenHandTechniqueDazzled { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionOpenHandTechniqueDazzled");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionRaging { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionRaging");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionRestrained { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionRestrained");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionRetchingReeling { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionRetchingReeling");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionShocked { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionShocked");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionSlowed { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionSlowed");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionSunbeam { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionSunbeam");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionSurprised { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionSurprised");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionTurned { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionTurned");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionVampiricTouch { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionVampiricTouch");

        internal static FeatureDefinitionActionAffinity ActionAffinityConjuredItemLink { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConjuredItemLink");

        internal static FeatureDefinitionActionAffinity ActionAffinityDruidRitualCasting { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityDruidRitualCasting");

        internal static FeatureDefinitionActionAffinity ActionAffinityExpeditiousRetreat { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityExpeditiousRetreat");

        internal static FeatureDefinitionActionAffinity ActionAffinityEyebiteAsleep { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityEyebiteAsleep");

        internal static FeatureDefinitionActionAffinity ActionAffinityEyebitePanicked { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityEyebitePanicked");

        internal static FeatureDefinitionActionAffinity ActionAffinityEyebiteSickened { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityEyebiteSickened");

        internal static FeatureDefinitionActionAffinity ActionAffinityFeatRushToBattle { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityFeatRushToBattle");

        internal static FeatureDefinitionActionAffinity ActionAffinityFeatTakeAim { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityFeatTakeAim");

        internal static FeatureDefinitionActionAffinity ActionAffinityFightingStyleProtection { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityFightingStyleProtection");

        internal static FeatureDefinitionActionAffinity ActionAffinityGlovesMissileSnaring { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityGlovesMissileSnaring");

        internal static FeatureDefinitionActionAffinity ActionAffinityGrappled { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityGrappled");

        internal static FeatureDefinitionActionAffinity ActionAffinityHunterGiantKiller { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityHunterGiantKiller");

        internal static FeatureDefinitionActionAffinity ActionAffinityInvocationBookAncientSecrets { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityInvocationBookAncientSecrets");

        internal static FeatureDefinitionActionAffinity ActionAffinityInvocationOneWithShadowsTurnInvisible { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityInvocationOneWithShadowsTurnInvisible");

        internal static FeatureDefinitionActionAffinity ActionAffinityKindredSpiritRage { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityKindredSpiritRage");

        internal static FeatureDefinitionActionAffinity ActionAffinityKindredSpiritRally { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityKindredSpiritRally");

        internal static FeatureDefinitionActionAffinity ActionAffinityLevitate { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityLevitate");

        internal static FeatureDefinitionActionAffinity ActionAffinityMagebaneSpellCrusher { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMagebaneSpellCrusher");

        internal static FeatureDefinitionActionAffinity ActionAffinityMarksmanReactionShot { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMarksmanReactionShot");

        internal static FeatureDefinitionActionAffinity ActionAffinityMarksmanStepback { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMarksmanStepback");

        internal static FeatureDefinitionActionAffinity ActionAffinityMartialCommanderCoordinatedDefense { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMartialCommanderCoordinatedDefense");

        internal static FeatureDefinitionActionAffinity ActionAffinityMartialMountainerShieldBash { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMartialMountainerShieldBash");

        internal static FeatureDefinitionActionAffinity ActionAffinityMonkDeflectMissiles { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMonkDeflectMissiles");

        internal static FeatureDefinitionActionAffinity ActionAffinityMonkFlurryOfBlows { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMonkFlurryOfBlows");

        internal static FeatureDefinitionActionAffinity ActionAffinityMonkStunningStrikeToggle { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMonkStunningStrikeToggle");

        internal static FeatureDefinitionActionAffinity ActionAffinityMountaineerShieldCharge { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMountaineerShieldCharge");

        internal static FeatureDefinitionActionAffinity ActionAffinityNimbleEscape { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityNimbleEscape");

        internal static FeatureDefinitionActionAffinity ActionAffinityRangerHunterVolley { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRangerHunterVolley");

        internal static FeatureDefinitionActionAffinity ActionAffinityRangerHunterWhirlwindAttack { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRangerHunterWhirlwindAttack");

        internal static FeatureDefinitionActionAffinity ActionAffinityRangerMarksmanFastAim { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRangerMarksmanFastAim");

        internal static FeatureDefinitionActionAffinity ActionAffinityRangerVanish { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRangerVanish");

        internal static FeatureDefinitionActionAffinity ActionAffinityReactive { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityReactive");

        internal static FeatureDefinitionActionAffinity ActionAffinityReapplyEffect { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityReapplyEffect");

        internal static FeatureDefinitionActionAffinity ActionAffinityRetaliation { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRetaliation");

        internal static FeatureDefinitionActionAffinity ActionAffinityRogueCunningAction { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRogueCunningAction");

        internal static FeatureDefinitionActionAffinity ActionAffinityRoguishDarkweaverShadowy { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRoguishDarkweaverShadowy");

        internal static FeatureDefinitionActionAffinity ActionAffinityShadowTamerSwiftRetaliation { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityShadowTamerSwiftRetaliation");

        internal static FeatureDefinitionActionAffinity ActionAffinitySharedPain { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySharedPain");

        internal static FeatureDefinitionActionAffinity ActionAffinitySlowFall { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySlowFall");

        internal static FeatureDefinitionActionAffinity ActionAffinitySongOfHope { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySongOfHope");

        internal static FeatureDefinitionActionAffinity ActionAffinitySorcererDraconicDragonWings { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySorcererDraconicDragonWings");

        internal static FeatureDefinitionActionAffinity ActionAffinitySorcererMetamagicToggle { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySorcererMetamagicToggle");

        internal static FeatureDefinitionActionAffinity ActionAffinitySubstituteForm { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySubstituteForm");

        internal static FeatureDefinitionActionAffinity ActionAffinityThiefFastHands { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityThiefFastHands");

        internal static FeatureDefinitionActionAffinity ActionAffinityTraditionFreedomSwiftSteps { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityTraditionFreedomSwiftSteps");

        internal static FeatureDefinitionActionAffinity ActionAffinityTraditionFreedomSwirlingDance { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityTraditionFreedomSwirlingDance");

        internal static FeatureDefinitionActionAffinity ActionAffinityTraditionFreedomUnendingStrikes { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityTraditionFreedomUnendingStrikes");

        internal static FeatureDefinitionActionAffinity ActionAffinityTraditionGreenMageLeafScales { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityTraditionGreenMageLeafScales");

        internal static FeatureDefinitionActionAffinity ActionAffinityTraditionOpenHandOpenHandTechnique { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityTraditionOpenHandOpenHandTechnique");

        internal static FeatureDefinitionActionAffinity ActionAffinityUncannyDodge { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityUncannyDodge");

        internal static FeatureDefinitionActionAffinity ActionAffinityWildShapeRevertShape { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityWildShapeRevertShape");

        internal static FeatureDefinitionActionAffinity ActionAffinityWizardRitualCasting { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityWizardRitualCasting");
    }

    internal static class FeatureDefinitionAdditionalActions
    {
        internal static FeatureDefinitionAdditionalAction AdditionalActionCunning { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionCunning");

        internal static FeatureDefinitionAdditionalAction AdditionalActionCunningFastHands { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionCunningFastHands");

        internal static FeatureDefinitionAdditionalAction AdditionalActionExpeditiousRetreat { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionExpeditiousRetreat");

        internal static FeatureDefinitionAdditionalAction AdditionalActionHasted { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionHasted");

        internal static FeatureDefinitionAdditionalAction AdditionalActionHunterHordeBreaker { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionHunterHordeBreaker");

        internal static FeatureDefinitionAdditionalAction AdditionalActionSurgedMain { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionSurgedMain");
    }

    internal static class FeatureDefinitionAdditionalDamages
    {
        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Daliat_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Daliat_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_DLC1_Mask_Spy_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_DLC1_Mask_Spy_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Dlc3_Shadowcaster_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Dlc3_Shadowcaster_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_DLC3_SneakAttack_Misouk { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_DLC3_SneakAttack_Misouk");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_GoblinCutthroat_Finisher { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_GoblinCutthroat_Finisher");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_GoblinCutthroat_Predator { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_GoblinCutthroat_Predator");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_OrcGrimblade_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_OrcGrimblade_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Sorr_Akkath_Assassin_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Sorr-Akkath_Assassin_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Sorr_Akkath_Harasser_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Sorr-Akkath_Harasser_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Sorr_Akkath_Saboteur_SneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Sorr-Akkath_Saboteur_SneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Cheater { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Cheater");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Cheater2 { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Cheater2");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Dragonblade { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Dragonblade");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Feybane { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Feybane");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Feybane2 { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Feybane2");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Guardian { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Guardian");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Sunstar { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Sunstar");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Sunstar2 { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Sunstar2");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Truth { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Truth");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Truth2 { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Truth2");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Unity { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Unity");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamage_Weapon_Unity2 { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamage_Weapon_Unity2");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageBarbarianBrutalCritical { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageBarbarianBrutalCritical");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageBarbarianClawFrightfulStrike { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageBarbarianClawFrightfulStrike");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageBarbarianStoneLastStand { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageBarbarianStoneLastStand");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageBracersOfArchery { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageBracersOfArchery");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageBrandingSmite { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageBrandingSmite");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageCircleBalanceColdEmbrace { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageCircleBalanceColdEmbrace");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageConditionRaging { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageConditionRaging");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageCursedByBestowCurseDamage { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageCursedByBestowCurseDamage");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDarkweaverPoisonous { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDarkweaverPoisonous");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDemonGrease_CONSpellTaint { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDemonGrease_CONSpellTaint");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDemonGrease_DawnBreak { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDemonGrease_DawnBreak");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDemonGrease_DMGSpellTaint { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDemonGrease_DMGSpellTaint");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDemonGrease_FiendSlaying { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDemonGrease_FiendSlaying");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDemonGrease_NightHunt { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDemonGrease_NightHunt");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDemonGrease_PseudoLife { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDemonGrease_PseudoLife");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDivineFavor { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDivineFavor");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDomainLifeDivineStrike { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDomainLifeDivineStrike");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDomainMischiefDivineStrike { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDomainMischiefDivineStrike");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDomainOblivionStrikeOblivion { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDomainOblivionStrikeOblivion");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDoomblade { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDoomblade");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageFeatBurningTouch { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageFeatBurningTouch");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageFeatElectrifyingTouch { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageFeatElectrifyingTouch");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageFeatIcyTouch { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageFeatIcyTouch");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageFeatMeltingTouch { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageFeatMeltingTouch");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageFeatToxicTouch { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageFeatToxicTouch");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageHalfOrcSavageAttacks { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageHalfOrcSavageAttacks");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageHunterColossusSlayer { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageHunterColossusSlayer");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageHuntersMark { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageHuntersMark");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageInvocationAgonizingBlast { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageInvocationAgonizingBlast");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageLifedrinker { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageLifedrinker");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageMalediction { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageMalediction");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageMightyBlow { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageMightyBlow");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageOathOfJugementAuraOfRighteousnessMagic { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageOathOfJugementAuraOfRighteousnessMagic");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageOathOfJugementAuraOfRighteousnessWeapon { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageOathOfJugementAuraOfRighteousnessWeapon");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageOathOfJugementWeightOfJustice { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageOathOfJugementWeightOfJustice");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageOathOfTirmarSmiteTheHiddenDarkvision { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageOathOfTirmarSmiteTheHiddenDarkvision");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageOathOfTirmarSmiteTheHiddenShapeChanger { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageOathOfTirmarSmiteTheHiddenShapeChanger");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageOathOfTirmarSmiteTheHiddenSuperiorDarkvision { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>(
                "AdditionalDamageOathOfTirmarSmiteTheHiddenSuperiorDarkvision");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePaladinDivineSmite { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePaladinDivineSmite");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePaladinImprovedDivineSmite { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePaladinImprovedDivineSmite");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePathClawDragonsBlessing { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePathClawDragonsBlessing");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePathMagebaneEnemyMagic { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePathMagebaneEnemyMagic");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePatronHiveWeakeningPheromones { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePatronHiveWeakeningPheromones");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePatronTimekeeperCurseOfTime { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePatronTimekeeperCurseOfTime");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_ArivadsKiss { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_ArivadsKiss");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_ArunsLight { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_ArunsLight");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_Basic { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_Basic");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_BrimstoneFang { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_BrimstoneFang");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_DarkStab { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_DarkStab");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_DeepPain { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_DeepPain");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_GhoulsCaress { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_GhoulsCaress");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_MaraikesTorpor { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_MaraikesTorpor");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_QueenSpidersBlood { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_QueenSpidersBlood");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_TheBurden { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_TheBurden");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_TheLongNight { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_TheLongNight");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_TigerFang { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_TigerFang");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyAberration { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyAberration");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyBeast { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyBeast");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyCelestial { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyCelestial");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyConstruct { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyConstruct");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyDragon { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyDragon");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyElemental { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyElemental");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyFey { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyFey");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyFiend { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyFiend");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyGiant { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyGiant");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyMonstrosity { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyMonstrosity");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyOoze { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyOoze");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyPlant { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyPlant");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyUndead { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyUndead");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerSwiftBladeBattleFocus { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerSwiftBladeBattleFocus");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRogueSneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRogueSneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRoguishDarkweaverPredator { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRoguishDarkweaverPredator");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRoguishHoodlumNonFinesseSneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRoguishHoodlumNonFinesseSneakAttack");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRoguishShadowcasterShadowCasting { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRoguishShadowcasterShadowCasting");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageShadowTamerDarkSlayerDmgHyperSensitive { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageShadowTamerDarkSlayerDmgHyperSensitive");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageShadowTamerDarkSlayerDmgSuperiorDarkvision { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>(
                "AdditionalDamageShadowTamerDarkSlayerDmgSuperiorDarkvision");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageSorcererDraconicElementalAffinity { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageSorcererDraconicElementalAffinity");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageTraditionLightRadiantStrikesLuminousKi { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageTraditionLightRadiantStrikesLuminousKi");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageTraditionLightRadiantStrikesShine { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageTraditionLightRadiantStrikesShine");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageTraditionShockArcanistArcaneFury { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageTraditionShockArcanistArcaneFury");
    }

    internal static class FeatureDefinitionAncestrys
    {
        internal static FeatureDefinitionAncestry AncestryDragonbornDraconicBlack { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestryDragonbornDraconicBlack");

        internal static FeatureDefinitionAncestry AncestryDragonbornDraconicBlue { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestryDragonbornDraconicBlue");

        internal static FeatureDefinitionAncestry AncestryDragonbornDraconicGold { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestryDragonbornDraconicGold");

        internal static FeatureDefinitionAncestry AncestryDragonbornDraconicGreen { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestryDragonbornDraconicGreen");

        internal static FeatureDefinitionAncestry AncestryDragonbornDraconicSilver { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestryDragonbornDraconicSilver");

        internal static FeatureDefinitionAncestry AncestrySorcererDraconicBlack { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestrySorcererDraconicBlack");

        internal static FeatureDefinitionAncestry AncestrySorcererDraconicBlue { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestrySorcererDraconicBlue");

        internal static FeatureDefinitionAncestry AncestrySorcererDraconicGold { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestrySorcererDraconicGold");

        internal static FeatureDefinitionAncestry AncestrySorcererDraconicGreen { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestrySorcererDraconicGreen");

        internal static FeatureDefinitionAncestry AncestrySorcererDraconicSilver { get; } =
            GetDefinition<FeatureDefinitionAncestry>("AncestrySorcererDraconicSilver");
    }

    internal static class FeatureDefinitionAttackModifiers
    {
        internal static FeatureDefinitionAttackModifier AttackModifier_ApostleOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_ApostleOfDarkness_Darkness");

        internal static FeatureDefinitionAttackModifier AttackModifier_ApostleOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_ApostleOfDarkness_DimLight");

        internal static FeatureDefinitionAttackModifier AttackModifier_ChildOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_ChildOfDarkness_Darkness");

        internal static FeatureDefinitionAttackModifier AttackModifier_ChildOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_ChildOfDarkness_DimLight");

        internal static FeatureDefinitionAttackModifier AttackModifier_PalaceOfIce_LairEffect { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_PalaceOfIce_LairEffect");

        internal static FeatureDefinitionAttackModifier AttackModifier_ProphetOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_ProphetOfDarkness_Darkness");

        internal static FeatureDefinitionAttackModifier AttackModifier_ProphetOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifier_ProphetOfDarkness_DimLight");

        internal static FeatureDefinitionAttackModifier AttackModifierBerserkerFrenzy { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierBerserkerFrenzy");

        internal static FeatureDefinitionAttackModifier AttackModifierDemonGreaseNightHunt { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierDemonGreaseNightHunt");

        internal static FeatureDefinitionAttackModifier AttackModifierFeatAmbidextrous { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFeatAmbidextrous");

        internal static FeatureDefinitionAttackModifier AttackModifierFightingStyleArchery { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFightingStyleArchery");

        internal static FeatureDefinitionAttackModifier AttackModifierFightingStyleDueling { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFightingStyleDueling");

        internal static FeatureDefinitionAttackModifier AttackModifierFightingStyleTwoWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFightingStyleTwoWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierFlameBlade2 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFlameBlade2");

        internal static FeatureDefinitionAttackModifier AttackModifierFlameBlade4 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFlameBlade4");

        internal static FeatureDefinitionAttackModifier AttackModifierFlameBlade6 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFlameBlade6");

        internal static FeatureDefinitionAttackModifier AttackModifierFlameBlade8 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFlameBlade8");

        internal static FeatureDefinitionAttackModifier AttackModifierFollowUpStrike { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFollowUpStrike");

        internal static FeatureDefinitionAttackModifier AttackModifierHeraldOfBattle { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierHeraldOfBattle");

        internal static FeatureDefinitionAttackModifier AttackModifierKindredSpiritBondMeleeAttack { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierKindredSpiritBondMeleeAttack");

        internal static FeatureDefinitionAttackModifier AttackModifierKindredSpiritBondMeleeDamage { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierKindredSpiritBondMeleeDamage");

        internal static FeatureDefinitionAttackModifier AttackModifierKindredSpiritMagicSpiritMagicAttack { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierKindredSpiritMagicSpiritMagicAttack");

        internal static FeatureDefinitionAttackModifier
            AttackModifierKindredSpiritRageAttackDamageProficiency { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierKindredSpiritRageAttackDamageProficiency");

        internal static FeatureDefinitionAttackModifier AttackModifierKindredSpiritRageAttackExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierKindredSpiritRageAttackExtraAttack");

        internal static FeatureDefinitionAttackModifier AttackModifierMagicWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMagicWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierMagicWeapon2 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMagicWeapon2");

        internal static FeatureDefinitionAttackModifier AttackModifierMagicWeapon3 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMagicWeapon3");

        internal static FeatureDefinitionAttackModifier AttackModifierMartialSpellBladeMagicWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMartialSpellBladeMagicWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkDeflectMissile { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkDeflectMissile");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus");

        internal static FeatureDefinitionAttackModifier
            AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkKiEmpoweredStrikes { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkKiEmpoweredStrikes");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkMartialArtsImprovedDamage { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkMartialArtsImprovedDamage");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkMartialArtsUnarmedStrikeBonus { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkMartialArtsUnarmedStrikeBonus");

        internal static FeatureDefinitionAttackModifier AttackModifierOathOfDevotionSacredWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierOathOfDevotionSacredWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierOathOfTirmarSoraksBane { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierOathOfTirmarSoraksBane");

        internal static FeatureDefinitionAttackModifier AttackModifierOilOfSharpnessPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierOilOfSharpness+3");

        internal static FeatureDefinitionAttackModifier AttackModifierPactBladeMagicalWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierPactBladeMagicalWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierScimitarOfSpeed { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierScimitarOfSpeed");

        internal static FeatureDefinitionAttackModifier AttackModifierShillelagh { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierShillelagh");

        internal static FeatureDefinitionAttackModifier AttackModifierTraditionSurvivalUnmovingStrength { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierTraditionSurvivalUnmovingStrength");

        internal static FeatureDefinitionAttackModifier AttackModifierWeaponPlus1 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierWeapon+1");

        internal static FeatureDefinitionAttackModifier AttackModifierWeaponPlus1AT { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierWeapon+1AT");

        internal static FeatureDefinitionAttackModifier AttackModifierWeaponPlus2 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierWeapon+2");

        internal static FeatureDefinitionAttackModifier AttackModifierWeaponPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierWeapon+3");
    }

    internal static class FeatureDefinitionAttributeModifiers
    {
        internal static FeatureDefinitionAttributeModifier AttributeModifier_BootsOfFirstStrike_Initiative { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifier_BootsOfFirstStrike_Initiative");

        internal static FeatureDefinitionAttributeModifier AttributeModifierACDemonGreaseDawnBreak_ARM { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierACDemonGreaseDawnBreak_ARM");

        internal static FeatureDefinitionAttributeModifier AttributeModifierACDemonGreaseNightHunt_ARM { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierACDemonGreaseNightHunt_ARM");

        internal static FeatureDefinitionAttributeModifier AttributeModifierACDemonGreaseNightHunt_WPN { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierACDemonGreaseNightHunt_WPN");

        internal static FeatureDefinitionAttributeModifier AttributeModifierACFeatRushToBattle { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierACFeatRushToBattle");

        internal static FeatureDefinitionAttributeModifier AttributeModifierAided { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierAided");

        internal static FeatureDefinitionAttributeModifier AttributeModifierAmuletOfHealth { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierAmuletOfHealth");

        internal static FeatureDefinitionAttributeModifier AttributeModifierArmor_Arcane_Shieldstaff { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_Arcane_Shieldstaff");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force13Plus1 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force13+1");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force14Plus1 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force14+1");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force14Plus2 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force14+2");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force15Plus1 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force15+1");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force15Plus2 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force15+2");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force16Plus2 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force16+2");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force16Plus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force16+3");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force18Plus2 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force18+2");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierArmor_ChildOfDarkness_Force18Plus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor_ChildOfDarkness_Force18+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierArmorPlus1 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor+1");

        internal static FeatureDefinitionAttributeModifier AttributeModifierArmorPlus2 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor+2");

        internal static FeatureDefinitionAttributeModifier AttributeModifierArmorPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmor+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierArmorConditionSlowed { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierArmorConditionSlowed");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianBrutalCriticalAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianBrutalCriticalAdd");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianBrutalCriticalBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianBrutalCriticalBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianRageDamageAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianRageDamageAdd");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianRageDamageBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianRageDamageBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianRagePointsAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianRagePointsAdd");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianRagePointsBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianRagePointsBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianUnarmoredDefense { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianUnarmoredDefense");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBardicInspirationDieD10 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBardicInspirationDieD10");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBardicInspirationDieD12 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBardicInspirationDieD12");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBardicInspirationDieD6 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBardicInspirationDieD6");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBardicInspirationDieD8 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBardicInspirationDieD8");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBardicInspirationNumber { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBardicInspirationNumber");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarkskin { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarkskin");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBeltOfDwarvenkind { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBeltOfDwarvenkind");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBeltOfTheBarbarianKing { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBeltOfTheBarbarianKing");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBracersDefense { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBracersDefense");

        internal static FeatureDefinitionAttributeModifier AttributeModifierClericChannelDivinity { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierClericChannelDivinity");

        internal static FeatureDefinitionAttributeModifier AttributeModifierClericChannelDivinityAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierClericChannelDivinityAdd");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCloakOfTheAncientKing { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCloakOfTheAncientKing");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCloakOfTheDandy { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCloakOfTheDandy");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCogOfCohh { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCogOfCohh");

        internal static FeatureDefinitionAttributeModifier AttributeModifierConditionCloakAndDagger { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierConditionCloakAndDagger");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCreed_Of_Arun { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCreed_Of_Arun");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCreed_Of_Einar { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCreed_Of_Einar");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCreed_Of_Maraike { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCreed_Of_Maraike");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCreed_Of_Misaye { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCreed_Of_Misaye");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCreed_Of_Pakri { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCreed_Of_Pakri");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCreed_Of_Solasta { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCreed_Of_Solasta");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_DaggerPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>(
                "AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_Dagger+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierCursedByMummyRot { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierCursedByMummyRot");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDaggerSoulHuntingWisLower { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDaggerSoulHuntingWisLower");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDazzled { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDazzled");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDiscretionOfTheCoedymwarth { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDiscretionOfTheCoedymwarth");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierDLC3_Dwarven_Weapon_BattleaxePlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Battleaxe+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDLC3_Dwarven_Weapon_DaggerPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Dagger+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDLC3_Dwarven_Weapon_GreataxePlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Greataxe+3");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierDLC3_Dwarven_Weapon_LongswordPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Longsword+3");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierDLC3_Dwarven_Weapon_QuarterstaffPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Quarterstaff+3");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierDLC3_Dwarven_Weapon_ShortswordPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Shortsword+3");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierDLC3_Dwarven_Weapon_WarhammerPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDLC3_Dwarven_Weapon_Warhammer+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDomainBattleExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDomainBattleExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDoomBlade { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDoomBlade");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDragonbornAbilityScoreIncreaseCha { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDragonbornAbilityScoreIncreaseCha");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDragonbornAbilityScoreIncreaseStr { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDragonbornAbilityScoreIncreaseStr");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDruidCircleWindsUnfettered { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDruidCircleWindsUnfettered");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfBread { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfBread");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfHillAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfHillAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfHillToughness { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfHillToughness");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfSnowAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfSnowAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierEarlyBirdInitiative { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierEarlyBirdInitiative");

        internal static FeatureDefinitionAttributeModifier AttributeModifierElfAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierElfAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierElfHighAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierElfHighAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierElfSylvanAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierElfSylvanAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierEnduringBody_HitPoints { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierEnduringBody_HitPoints");

        internal static FeatureDefinitionAttributeModifier AttributeModifierEnduringBodyConstitution { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierEnduringBodyConstitution");

        internal static FeatureDefinitionAttributeModifier AttributeModifierEnhancedAbilityEndurance { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierEnhancedAbilityEndurance");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatAlert { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatAlert");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatAmbidextrous { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatAmbidextrous");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatArmorMaster { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatArmorMaster");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatBadlandsMarauder { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatBadlandsMarauder");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatDistractingGambit { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatDistractingGambit");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatEagerForBattle { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatEagerForBattle");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatFocusedSleeper { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatFocusedSleeper");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatForestRunner { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatForestRunner");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatHardToKill { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatHardToKill");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatHauler { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatHauler");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatRobust { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatRobust");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeatUncannyAccuracy { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeatUncannyAccuracy");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeeblemindedCharisma { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeeblemindedCharisma");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeeblemindedIntelligence { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeeblemindedIntelligence");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFeeblemindedWisdom { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFeeblemindedWisdom");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFighterExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFighterExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFighterIndomitable { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFighterIndomitable");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFighterIndomitableAdd1 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFighterIndomitableAdd1");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFightingStyleDefense { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFightingStyleDefense");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantCloud { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantCloud");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantFire { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantFire");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantFirePermanent { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantFirePermanent");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantFrost { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantFrost");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantHill { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantHill");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantHillPermanent { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantHillPermanent");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantMorningStarOfPowerPermanent { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantMorningStarOfPowerPermanent");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantStoneFrostPermanent { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantStoneFrostPermanent");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGiantStrengthHillPermanent { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGiantStrengthHillPermanent");

        internal static FeatureDefinitionAttributeModifier AttributeModifierGnomeRockAbilityScoreIncreaseCon { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGnomeRockAbilityScoreIncreaseCon");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierGnomeShadowAbilityScoreIncreaseDex { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierGnomeShadowAbilityScoreIncreaseDex");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalfElfAbilityScoreIncreaseCha { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalfElfAbilityScoreIncreaseCha");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalflingAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalflingAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierHalflingIslandAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalflingIslandAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalflingMarshAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalflingMarshAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalfOrcAbilityScoreIncreaseCon { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalfOrcAbilityScoreIncreaseCon");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalfOrcAbilityScoreIncreaseStr { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalfOrcAbilityScoreIncreaseStr");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHasted { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHasted");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHeadbandOfIntellect { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHeadbandOfIntellect");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHeraldOfBattle { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHeraldOfBattle");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHeroesFeast { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHeroesFeast");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHumanAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHumanAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierInvocationVoiceChainMaster { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierInvocationVoiceChainMaster");

        internal static FeatureDefinitionAttributeModifier AttributeModifierKindredSpiritBear { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierKindredSpiritBear");

        internal static FeatureDefinitionAttributeModifier AttributeModifierKindredSpiritBondAC { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierKindredSpiritBondAC");

        internal static FeatureDefinitionAttributeModifier AttributeModifierKindredSpiritBondHP { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierKindredSpiritBondHP");

        internal static FeatureDefinitionAttributeModifier AttributeModifierKindredSpiritEagle { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierKindredSpiritEagle");

        internal static FeatureDefinitionAttributeModifier AttributeModifierKindredSpiritRageAC { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierKindredSpiritRageAC");

        internal static FeatureDefinitionAttributeModifier AttributeModifierKindredSpiritWolf { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierKindredSpiritWolf");

        internal static FeatureDefinitionAttributeModifier AttributeModifierLifeDrained { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierLifeDrained");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMageArmor { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMageArmor");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMarksmanExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMarksmanExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMartialChampionImprovedCritical { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMartialChampionImprovedCritical");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMartialChampionSuperiorCritical { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMartialChampionSuperiorCritical");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMartialMountainerTunnelFighter { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMartialMountainerTunnelFighter");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMightOfTheIronLegion { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMightOfTheIronLegion");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMonkExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMonkExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMonkKiPointsBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMonkKiPointsBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMonkKiPointsMultiplier { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMonkKiPointsMultiplier");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMonkUnarmoredDefense { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMonkUnarmoredDefense");

        internal static FeatureDefinitionAttributeModifier AttributeModifierNecklaceOfGroundingAC { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierNecklaceOfGroundingAC");

        internal static FeatureDefinitionAttributeModifier AttributeModifierNecklaceOfGroundingKi { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierNecklaceOfGroundingKi");

        internal static FeatureDefinitionAttributeModifier AttributeModifierOgrePowerPermanent { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierOgrePowerPermanent");

        internal static FeatureDefinitionAttributeModifier AttributeModifierOneRing { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierOneRing");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPaladinChannelDivinity { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPaladinChannelDivinity");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPaladinExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPaladinExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPaladinHealingPoolBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPaladinHealingPoolBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPaladinHealingPoolMultiplier { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPaladinHealingPoolMultiplier");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPathClawDragonScalesAC { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPathClawDragonScalesAC");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPatronTreeOneWithTheTree { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPatronTreeOneWithTheTree");

        internal static FeatureDefinitionAttributeModifier AttributeModifierRangerExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierRangerExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierShielded { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierShielded");

        internal static FeatureDefinitionAttributeModifier AttributeModifierShieldedByFaith { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierShieldedByFaith");

        internal static FeatureDefinitionAttributeModifier AttributeModifierSorcererDraconicResilienceAC { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSorcererDraconicResilienceAC");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierSorcererDraconicResilienceHitPoints { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSorcererDraconicResilienceHitPoints");

        internal static FeatureDefinitionAttributeModifier AttributeModifierSorcererSorceryPointsBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSorcererSorceryPointsBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierSorcererSorceryPointsMultiplier { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSorcererSorceryPointsMultiplier");

        internal static FeatureDefinitionAttributeModifier AttributeModifierStoneRockSolid { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierStoneRockSolid");

        internal static FeatureDefinitionAttributeModifier AttributeModifierStuddedArmorOfLeadership { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierStuddedArmorOfLeadership");

        internal static FeatureDefinitionAttributeModifier AttributeModifierSturdinessOfTheTundra { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSturdinessOfTheTundra");

        internal static FeatureDefinitionAttributeModifier AttributeModifierSwiftBladeBladeDance { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSwiftBladeBladeDance");

        internal static FeatureDefinitionAttributeModifier AttributeModifierThirstingBladeExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierThirstingBladeExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTieflingAbilityScoreIncreaseCha { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTieflingAbilityScoreIncreaseCha");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTieflingAbilityScoreIncreaseInt { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTieflingAbilityScoreIncreaseInt");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTomeOfAllThings_CHA { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTomeOfAllThings_CHA");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTomeOfAllThings_CON { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTomeOfAllThings_CON");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTomeOfAllThings_DEX { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTomeOfAllThings_DEX");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTomeOfAllThings_INT { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTomeOfAllThings_INT");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTomeOfAllThings_STR { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTomeOfAllThings_STR");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTomeOfAllThings_WIS { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTomeOfAllThings_WIS");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTraditionSurvivalDefensiveStance { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTraditionSurvivalDefensiveStance");

        internal static FeatureDefinitionAttributeModifier AttributeModifierWardedByWardingBond { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierWardedByWardingBond");

        internal static FeatureDefinitionAttributeModifier AttributeModifierWarlockPactChainInitial { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierWarlockPactChainInitial");

        internal static FeatureDefinitionAttributeModifier AttributeModiierFeatForestallingStrength { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModiierFeatForestallingStrength");
    }

    internal static class FeatureDefinitionAutoPreparedSpellss
    {
        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleBalance { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleBalance");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandArctic { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandArctic");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandCoast { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandCoast");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandDesert { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandDesert");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandForest { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandForest");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandGrassland { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandGrassland");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandMountain { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandMountain");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCircleLandSwamp { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCircleLandSwamp");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsCollegeHope { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsCollegeHope");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainBattle { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainBattle");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainElemental { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainElemental");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainInsight { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainInsight");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainLaw { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainLaw");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainLife { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainLife");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainMischief { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainMischief");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainOblivion { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainOblivion");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsDomainSun { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsDomainSun");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsOathOfDevotion { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsOathOfDevotion");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsOathOfJugement { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsOathOfJugement");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsOathOfMotherland { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsOathOfMotherland");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsOathOfTirmar { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsOathOfTirmar");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsSorcereDraconicBloodline { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsSorcereDraconicBloodline");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsSorcererChildRift { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsSorcererChildRift");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsSorcererHauntedSoul { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsSorcererHauntedSoul");

        internal static FeatureDefinitionAutoPreparedSpells AutoPreparedSpellsSorcererManaPainter { get; } =
            GetDefinition<FeatureDefinitionAutoPreparedSpells>("AutoPreparedSpellsSorcererManaPainter");
    }

    internal static class FeatureDefinitionBonusCantripss
    {
        internal static FeatureDefinitionBonusCantrips BonusCantripsDomainElementaCold { get; } =
            GetDefinition<FeatureDefinitionBonusCantrips>("BonusCantripsDomainElementaCold");

        internal static FeatureDefinitionBonusCantrips BonusCantripsDomainElementaFire { get; } =
            GetDefinition<FeatureDefinitionBonusCantrips>("BonusCantripsDomainElementaFire");

        internal static FeatureDefinitionBonusCantrips BonusCantripsDomainElementaLightning { get; } =
            GetDefinition<FeatureDefinitionBonusCantrips>("BonusCantripsDomainElementaLightning");

        internal static FeatureDefinitionBonusCantrips BonusCantripsDomainOblivion { get; } =
            GetDefinition<FeatureDefinitionBonusCantrips>("BonusCantripsDomainOblivion");

        internal static FeatureDefinitionBonusCantrips BonusCantripsDomainSun { get; } =
            GetDefinition<FeatureDefinitionBonusCantrips>("BonusCantripsDomainSun");

        internal static FeatureDefinitionBonusCantrips BonusCantripsTiefling { get; } =
            GetDefinition<FeatureDefinitionBonusCantrips>("BonusCantripsTiefling");
    }

    internal static class FeatureDefinitionCampAffinitys
    {
        internal static FeatureDefinitionCampAffinity CampAffinityBardFontOfInspiration { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityBardFontOfInspiration");

        internal static FeatureDefinitionCampAffinity CampAffinityDomainOblivionPeacefulRest { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityDomainOblivionPeacefulRest");

        internal static FeatureDefinitionCampAffinity CampAffinityElfTrance { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityElfTrance");

        internal static FeatureDefinitionCampAffinity CampAffinityFeatFocusedSleeper { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityFeatFocusedSleeper");

        internal static FeatureDefinitionCampAffinity CampAffinityMonkTimelessBody { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityMonkTimelessBody");
    }

    internal static class FeatureDefinitionCastSpells
    {
        internal static FeatureDefinitionCastSpell CastSpell_DLC1_Cafrain { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC1_Cafrain");

        internal static FeatureDefinitionCastSpell CastSpell_DLC1_Dominion_Arcanist { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC1_Dominion_Arcanist");

        internal static FeatureDefinitionCastSpell CastSpell_DLC1_Forge_Druid { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC1_Forge_Druid");

        internal static FeatureDefinitionCastSpell CastSpell_DLC1_ManaScientist { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC1_ManaScientist");

        internal static FeatureDefinitionCastSpell CastSpell_DLC1_Mask_Cleric { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC1_Mask_Cleric");

        internal static FeatureDefinitionCastSpell CastSpell_DLC3_Gallivan_Druid { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC3_Gallivan_Druid");

        internal static FeatureDefinitionCastSpell CastSpell_DLC3_Gallivan_RogueShadowCaster { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC3_Gallivan_RogueShadowCaster");

        internal static FeatureDefinitionCastSpell CastSpell_DLC3_Kratshar { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC3_Kratshar");

        internal static FeatureDefinitionCastSpell CastSpell_DLC3_Misouk { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC3_Misouk");

        internal static FeatureDefinitionCastSpell CastSpell_DLC3_Vigdis { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_DLC3_Vigdis");

        internal static FeatureDefinitionCastSpell CastSpell_Swamp_Hag { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpell_Swamp_Hag");

        internal static FeatureDefinitionCastSpell CastSpellAcolyte { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellAcolyte");

        internal static FeatureDefinitionCastSpell CastSpellAdria { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellAdria");

        internal static FeatureDefinitionCastSpell CastSpellArrok { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellArrok");

        internal static FeatureDefinitionCastSpell CastSpellBard { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellBard");

        internal static FeatureDefinitionCastSpell CastSpellBerylStonebeard { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellBerylStonebeard");

        internal static FeatureDefinitionCastSpell CastSpellBerylStonebeard_DLC3 { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellBerylStonebeard_DLC3");

        internal static FeatureDefinitionCastSpell CastSpellCleric { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellCleric");

        internal static FeatureDefinitionCastSpell CastSpellCubeOfLight { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellCubeOfLight");

        internal static FeatureDefinitionCastSpell CastSpellCultFanatic { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellCultFanatic");

        internal static FeatureDefinitionCastSpell CastSpellDivineAvatar_Cleric { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDivineAvatar_Cleric");

        internal static FeatureDefinitionCastSpell CastSpellDivineAvatar_Paladin { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDivineAvatar_Paladin");

        internal static FeatureDefinitionCastSpell CastSpellDivineAvatar_Wizard { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDivineAvatar_Wizard");

        internal static FeatureDefinitionCastSpell CastSpellDLC_Orenetis { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDLC_Orenetis");

        internal static FeatureDefinitionCastSpell CastSpellDLC_Rose { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDLC_Rose");

        internal static FeatureDefinitionCastSpell CastSpellDLC_Vando { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDLC_Vando");

        internal static FeatureDefinitionCastSpell CastSpellDruid { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDruid");

        internal static FeatureDefinitionCastSpell CastSpellDryad { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDryad");

        internal static FeatureDefinitionCastSpell CastSpellDryad_Queen { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDryad_Queen");

        internal static FeatureDefinitionCastSpell CastSpellDryad_Water { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDryad_Water");

        internal static FeatureDefinitionCastSpell CastSpellElfHigh { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellElfHigh");

        internal static FeatureDefinitionCastSpell CastSpellGalar { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellGalar");

        internal static FeatureDefinitionCastSpell CastSpellGenericSorcerer { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellGenericSorcerer");

        internal static FeatureDefinitionCastSpell CastSpellGlabrezu { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellGlabrezu");

        internal static FeatureDefinitionCastSpell CastSpellGnomeShadow { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellGnomeShadow");

        internal static FeatureDefinitionCastSpell CastSpellGoblinShaman { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellGoblinShaman");

        internal static FeatureDefinitionCastSpell CastSpellHeliaFairblade { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellHeliaFairblade");

        internal static FeatureDefinitionCastSpell CastSpellHighPriest { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellHighPriest");

        internal static FeatureDefinitionCastSpell CastSpellHyeronimus { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellHyeronimus");

        internal static FeatureDefinitionCastSpell CastSpellKebra { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellKebra");

        internal static FeatureDefinitionCastSpell CastSpellKythaela_Elf { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellKythaela_Elf");

        internal static FeatureDefinitionCastSpell CastSpellKythaela_Full { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellKythaela_Full");

        internal static FeatureDefinitionCastSpell CastSpellMage { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellMage");

        internal static FeatureDefinitionCastSpell CastSpellMardracht { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellMardracht");

        internal static FeatureDefinitionCastSpell CastSpellMartialSpellBlade { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellMartialSpellBlade");

        internal static FeatureDefinitionCastSpell CastSpellMummyLord { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellMummyLord");

        internal static FeatureDefinitionCastSpell CastSpellNecromancer { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellNecromancer");

        internal static FeatureDefinitionCastSpell CastSpellNecromancer_BoneKeep { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellNecromancer_BoneKeep");

        internal static FeatureDefinitionCastSpell CastSpellOrcShaman { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellOrcShaman");

        internal static FeatureDefinitionCastSpell CastSpellPaladin { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellPaladin");

        internal static FeatureDefinitionCastSpell CastSpellPriest { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellPriest");

        internal static FeatureDefinitionCastSpell CastSpellRanger { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellRanger");

        internal static FeatureDefinitionCastSpell CastSpellReya { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellReya");

        internal static FeatureDefinitionCastSpell CastSpellShadowcaster { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellShadowcaster");

        internal static FeatureDefinitionCastSpell CastSpellShockArcanist { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellShockArcanist");

        internal static FeatureDefinitionCastSpell CastSpellSkeleton_Knight { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSkeleton_Knight");

        internal static FeatureDefinitionCastSpell CastSpellSkeletonSorcerer { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSkeletonSorcerer");

        internal static FeatureDefinitionCastSpell CastSpellSorcerer { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSorcerer");

        internal static FeatureDefinitionCastSpell CastSpellSorr_Akkath_Acolyte_of_Sorr_Tarr { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSorr-Akkath_Acolyte_of_Sorr-Tarr");

        internal static FeatureDefinitionCastSpell CastSpellSorr_Akkath_Archpriest_of_Sorr_Tarr { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSorr-Akkath_Archpriest_of_Sorr-Tarr");

        internal static FeatureDefinitionCastSpell CastSpellSorr_Akkath_Priest_of_Sorr_Tarr { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSorr-Akkath_Priest_of_Sorr-Tarr");

        internal static FeatureDefinitionCastSpell CastSpellTiefling { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellTiefling");

        internal static FeatureDefinitionCastSpell CastSpellTraditionLight { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellTraditionLight");

        internal static FeatureDefinitionCastSpell CastSpellWarlock { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellWarlock");

        internal static FeatureDefinitionCastSpell CastSpellWizard { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellWizard");
    }

    internal static class FeatureDefinitionCharacterPresentations
    {
        internal static FeatureDefinitionCharacterPresentation CharacterPresentationBeltOfDwarvenKind { get; } =
            GetDefinition<FeatureDefinitionCharacterPresentation>("CharacterPresentationBeltOfDwarvenKind");
    }

    internal static class FeatureDefinitionCombatAffinitys
    {
        internal static FeatureDefinitionCombatAffinity CombatAffinityAdamantinePlateArmor { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityAdamantinePlateArmor");

        internal static FeatureDefinitionCombatAffinity CombatAffinityAttackAdvantageDemonGreaseTrueStrike { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityAttackAdvantageDemonGreaseTrueStrike");

        internal static FeatureDefinitionCombatAffinity CombatAffinityAttackDisadvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityAttackDisadvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBackfromTheDead1 { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBackfromTheDead1");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBackfromTheDead2 { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBackfromTheDead2");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBackfromTheDead3 { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBackfromTheDead3");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBackfromTheDead4 { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBackfromTheDead4");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBalanceOfPower { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBalanceOfPower");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBalanceOfPowerVulnerability { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBalanceOfPowerVulnerability");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBaned { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBaned");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBarbarianFeralInstinct { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBarbarianFeralInstinct");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBlessed { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBlessed");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBlessingSorrtarr { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBlessingSorrtarr");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBlinded { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBlinded");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBlurred { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBlurred");

        internal static FeatureDefinitionCombatAffinity CombatAffinityChilledByTouch { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityChilledByTouch");

        internal static FeatureDefinitionCombatAffinity CombatAffinityCloakOfDisplacement { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityCloakOfDisplacement");

        internal static FeatureDefinitionCombatAffinity CombatAffinityCollegeLoreCuttingWords { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityCollegeLoreCuttingWords");

        internal static FeatureDefinitionCombatAffinity CombatAffinityConditionHolyAura { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityConditionHolyAura");

        internal static FeatureDefinitionCombatAffinity CombatAffinityContagionFilthFever { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityContagionFilthFever");

        internal static FeatureDefinitionCombatAffinity CombatAffinityContagionSeizure { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityContagionSeizure");

        internal static FeatureDefinitionCombatAffinity CombatAffinityCursedByBestowCurseOnAttackRoll { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityCursedByBestowCurseOnAttackRoll");

        internal static FeatureDefinitionCombatAffinity CombatAffinityDemonicInfluence { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityDemonicInfluence");

        internal static FeatureDefinitionCombatAffinity CombatAffinityDisengaging { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityDisengaging");

        internal static FeatureDefinitionCombatAffinity CombatAffinityDispellingEvil { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityDispellingEvil");

        internal static FeatureDefinitionCombatAffinity CombatAffinityDodging { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityDodging");

        internal static FeatureDefinitionCombatAffinity CombatAffinityEagerForBattle { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityEagerForBattle");

        internal static FeatureDefinitionCombatAffinity CombatAffinityEldritchSpear { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityEldritchSpear");

        internal static FeatureDefinitionCombatAffinity CombatAffinityEnfeebled { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityEnfeebled");

        internal static FeatureDefinitionCombatAffinity CombatAffinityEyebiteSickened { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityEyebiteSickened");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFeatReadyOrNot { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFeatReadyOrNot");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFeatTakeAim { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFeatTakeAim");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFeatTripAttack { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFeatTripAttack");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFeatUncannyAccuracy { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFeatUncannyAccuracy");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFlyby { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFlyby");

        internal static FeatureDefinitionCombatAffinity CombatAffinityForeknowledge { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityForeknowledge");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFrightened { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFrightened");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHeatMetal { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHeatMetal");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHeavilyEncumbered { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHeavilyEncumbered");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHeavilyObscured { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHeavilyObscured");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHeavilyObscuredSelf { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHeavilyObscuredSelf");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHitByThunderingVoice { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHitByThunderingVoice");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHunterEscapeTheHorde { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHunterEscapeTheHorde");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHunterMultiattackDefense { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHunterMultiattackDefense");

        internal static FeatureDefinitionCombatAffinity CombatAffinityIncreasedReliance { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityIncreasedReliance");

        internal static FeatureDefinitionCombatAffinity CombatAffinityInvisible { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityInvisible");

        internal static FeatureDefinitionCombatAffinity CombatAffinityInvisibleStalker { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityInvisibleStalker");

        internal static FeatureDefinitionCombatAffinity CombatAffinityMartialMountaineerPackStriker { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityMartialMountaineerPackStriker");

        internal static FeatureDefinitionCombatAffinity CombatAffinityPackTactics { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityPackTactics");

        internal static FeatureDefinitionCombatAffinity CombatAffinityParalyzedAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityParalyzedAdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityParalyzedAutoCrit { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityParalyzedAutoCrit");

        internal static FeatureDefinitionCombatAffinity CombatAffinityPatronTreeOneWithTheTree { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityPatronTreeOneWithTheTree");

        internal static FeatureDefinitionCombatAffinity CombatAffinityPoisoned { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityPoisoned");

        internal static FeatureDefinitionCombatAffinity CombatAffinityProneAttackerAway { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityProneAttackerAway");

        internal static FeatureDefinitionCombatAffinity CombatAffinityProneAttackerContact { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityProneAttackerContact");

        internal static FeatureDefinitionCombatAffinity CombatAffinityProneDefender { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityProneDefender");

        internal static FeatureDefinitionCombatAffinity CombatAffinityProtectedByMagicCircle { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityProtectedByMagicCircle");

        internal static FeatureDefinitionCombatAffinity CombatAffinityProtectedFromEvil { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityProtectedFromEvil");

        internal static FeatureDefinitionCombatAffinity CombatAffinityReckless { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityReckless");

        internal static FeatureDefinitionCombatAffinity CombatAffinityRecklessVulnerability { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityRecklessVulnerability");

        internal static FeatureDefinitionCombatAffinity CombatAffinityRestrained { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityRestrained");

        internal static FeatureDefinitionCombatAffinity CombatAffinityRoguishHoodlumMenaced { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityRoguishHoodlumMenaced");

        internal static FeatureDefinitionCombatAffinity CombatAffinityRousingShout { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityRousingShout");

        internal static FeatureDefinitionCombatAffinity CombatAffinitySensitiveToLight { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinitySensitiveToLight");

        internal static FeatureDefinitionCombatAffinity CombatAffinitySensitiveToLightImmunitySorakSaboteur { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinitySensitiveToLightImmunitySorakSaboteur");

        internal static FeatureDefinitionCombatAffinity CombatAffinityShadowMurder { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityShadowMurder");

        internal static FeatureDefinitionCombatAffinity CombatAffinityShadowTamerDarkSlayerHitHyperSensitive { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityShadowTamerDarkSlayerHitHyperSensitive");

        internal static FeatureDefinitionCombatAffinity CombatAffinityShadowTamerDarkSlayerHitLightSensitive { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityShadowTamerDarkSlayerHitLightSensitive");

        internal static FeatureDefinitionCombatAffinity CombatAffinitySorcererChildRiftDeflection { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinitySorcererChildRiftDeflection");

        internal static FeatureDefinitionCombatAffinity CombatAffinityStealthy { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStealthy");

        internal static FeatureDefinitionCombatAffinity CombatAffinityStepback { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStepback");

        internal static FeatureDefinitionCombatAffinity CombatAffinityStrikeOfChaosAttackAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStrikeOfChaosAttackAdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityStrikeOfChaosAttackDisdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStrikeOfChaosAttackDisdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityStunnedAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStunnedAdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityTargetedByGuidingBolt { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityTargetedByGuidingBolt");

        internal static FeatureDefinitionCombatAffinity CombatAffinityTargetedByGuidingWinds { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityTargetedByGuidingWinds");

        internal static FeatureDefinitionCombatAffinity CombatAffinityTraditionFreedomSwiftStepsAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityTraditionFreedomSwiftStepsAdvantage");

        internal static FeatureDefinitionCombatAffinity
            CombatAffinityTraditionSurvivalDefensiveStanceAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityTraditionSurvivalDefensiveStanceAdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityTrueStrike { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityTrueStrike");

        internal static FeatureDefinitionCombatAffinity CombatAffinityUnconscious { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityUnconscious");

        internal static FeatureDefinitionCombatAffinity CombatAffinityUnconsciousAutoCrit { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityUnconsciousAutoCrit");

        internal static FeatureDefinitionCombatAffinity CombatAffinityVeil { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityVeil");

        internal static FeatureDefinitionCombatAffinity CombatAffinityWandOfWarMageCover { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityWandOfWarMageCover");
    }

    internal static class FeatureDefinitionConditionAffinitys
    {
        internal static FeatureDefinitionConditionAffinity ConditionAffinityAlreadyHitByDirtyFighting { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityAlreadyHitByDirtyFighting");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityBanishedByMazeImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityBanishedByMazeImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityBlindnessImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityBlindnessImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCalmEmotionCharmedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCalmEmotionCharmedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCalmEmotionFrightenedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCalmEmotionFrightenedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCharmedAdvantage { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCharmedAdvantage");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCharmedAdvantageHypnoticPattern { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCharmedAdvantageHypnoticPattern");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCharmImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCharmImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCharmImmunityHypnoticPattern { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCharmImmunityHypnoticPattern");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandLandsStride { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandLandsStride");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandNaturesWardCharmed { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandNaturesWardCharmed");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandNaturesWardDiseased { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandNaturesWardDiseased");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandNaturesWardFrightened { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandNaturesWardFrightened");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandNaturesWardPoisoned { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandNaturesWardPoisoned");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCloakOfArachnida { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCloakOfArachnida");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityDemonicInfluenceImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityDemonicInfluenceImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityDiseaseImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityDiseaseImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityDiseaseImmunity_PeriaptOfHealth { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityDiseaseImmunity_PeriaptOfHealth");

        internal static FeatureDefinitionConditionAffinity
            ConditionAffinityDruidCircleLandNaturesSanctuaryImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>(
                "ConditionAffinityDruidCircleLandNaturesSanctuaryImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityElfFeyAncestryCharm { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityElfFeyAncestryCharm");

        internal static FeatureDefinitionConditionAffinity
            ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>(
                "ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityElfFeyAncestrySleep { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityElfFeyAncestrySleep");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityExhaustionImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityExhaustionImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFreedomOfMovementParalyzed { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFreedomOfMovementParalyzed");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFreedomOfMovementRestrained { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFreedomOfMovementRestrained");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedAdvantage { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedAdvantage");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedByDragonImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedByDragonImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedFearImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedFearImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedHeroism { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedHeroism");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedMummyImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedMummyImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrozen { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrozen");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrozenImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrozenImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityGrappledImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityGrappledImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityHalflingBrave { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityHalflingBrave");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityHezrouStenchImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityHezrouStenchImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityHinderedByFrostImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityHinderedByFrostImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityHunterSteelWill { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityHunterSteelWill");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityInvocationDevilsSight { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityInvocationDevilsSight");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityLightSensitivityImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityLightSensitivityImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityMindControlledImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMindControlledImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityMindDominatedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMindDominatedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityMindlessRageCharm { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMindlessRageCharm");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityMindlessRageFrightened { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMindlessRageFrightened");

        internal static FeatureDefinitionConditionAffinity
            ConditionAffinityMonkTimelessBodyLifeDrainedAdvantage { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMonkTimelessBodyLifeDrainedAdvantage");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityParalyzedGhoulImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityParalyzedGhoulImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityParalyzedmmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityParalyzedmmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityParalyzedMummyImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityParalyzedMummyImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityPatronTreeBlessingOfTheTree { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityPatronTreeBlessingOfTheTree");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityPetrifiedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityPetrifiedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityPoisonImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityPoisonImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityProneImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityProneImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityProtectedFromEvilCharmImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityProtectedFromEvilCharmImmunity");

        internal static FeatureDefinitionConditionAffinity
            ConditionAffinityProtectedFromEvilFrightenedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityProtectedFromEvilFrightenedImmunity");

        internal static FeatureDefinitionConditionAffinity
            ConditionAffinityProtectedFromEvilPossessedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityProtectedFromEvilPossessedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityResistanceToShine { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityResistanceToShine");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityRestrainedmmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityRestrainedmmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityUnconsciousImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityUnconsciousImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityVeilImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityVeilImmunity");
    }

    internal static class FeatureDefinitionCraftingAffinitys
    {
        internal static FeatureDefinitionCraftingAffinity CraftingAffinityFeatMasterAlchemist { get; } =
            GetDefinition<FeatureDefinitionCraftingAffinity>("CraftingAffinityFeatMasterAlchemist");

        internal static FeatureDefinitionCraftingAffinity CraftingAffinityFeatMasterEnchanter { get; } =
            GetDefinition<FeatureDefinitionCraftingAffinity>("CraftingAffinityFeatMasterEnchanter");

        internal static FeatureDefinitionCraftingAffinity CraftingAffinityGnomeRockTinker { get; } =
            GetDefinition<FeatureDefinitionCraftingAffinity>("CraftingAffinityGnomeRockTinker");

        internal static FeatureDefinitionCraftingAffinity CraftingAffinityLoremasterKeenMindCrafting { get; } =
            GetDefinition<FeatureDefinitionCraftingAffinity>("CraftingAffinityLoremasterKeenMindCrafting");

        internal static FeatureDefinitionCraftingAffinity CraftingAffinityPeriaptOfTheMasterEnchanter { get; } =
            GetDefinition<FeatureDefinitionCraftingAffinity>("CraftingAffinityPeriaptOfTheMasterEnchanter");
    }

    internal static class FeatureDefinitionCriticalCharacters
    {
        internal static FeatureDefinitionCriticalCharacter CriticalCharacter { get; } =
            GetDefinition<FeatureDefinitionCriticalCharacter>("CriticalCharacter");
    }

    internal static class FeatureDefinitionDamageAffinitys
    {
        internal static FeatureDefinitionDamageAffinity DamageAffinityAcidImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityAcidImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityAcidImmunityClayGolem { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityAcidImmunityClayGolem");

        internal static FeatureDefinitionDamageAffinity DamageAffinityAcidResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityAcidResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBarbarianRelentlessRage { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBarbarianRelentlessRage");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningImmunity_except_magical { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningImmunity_except_magical");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningResistanceByWardingBond { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningResistanceByWardingBond");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningResistanceExceptSilver { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningResistanceExceptSilver");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityCircleLandNaturesWardPoison { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityCircleLandNaturesWardPoison");

        internal static FeatureDefinitionDamageAffinity DamageAffinityColdImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityColdImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityColdResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityColdResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityColdVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityColdVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionHolyAura { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionHolyAura");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionRagingBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionRagingBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionRagingPiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionRagingPiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionRagingSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionRagingSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotAcid { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotAcid");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotCold { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotCold");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotFire { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotFire");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotForce { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotForce");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotLightning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotLightning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotNecrotic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotNecrotic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotPiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotPiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotPoison { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotPoison");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotPsychic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotPsychic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotRadiant { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotRadiant");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityContagionFleshRotThunder { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityContagionFleshRotThunder");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDeathWarded { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDeathWarded");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleAcid { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleAcid");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleCold { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleCold");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleFire { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleFire");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleForce { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleForce");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleLightning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleLightning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleNecrotic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleNecrotic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInviciblePiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInviciblePiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInviciblePoison { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInviciblePoison");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInviciblePsychic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInviciblePsychic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleRadiant { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleRadiant");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDebugInvicibleThunder { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDebugInvicibleThunder");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDiscipleColdImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDiscipleColdImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDiscipleFireImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDiscipleFireImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDiscipleLightningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDiscipleLightningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDomainElementaColdPrimalHarmony { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDomainElementaColdPrimalHarmony");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDomainElementaFirePrimalHarmony { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDomainElementaFirePrimalHarmony");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDomainElementaLightningPrimalHarmony { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDomainElementaLightningPrimalHarmony");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDragonbornDamageResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDragonbornDamageResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDwarfResilience { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDwarfResilience");

        internal static FeatureDefinitionDamageAffinity DamageAffinityDwarfResilience_BeltOfDwarvenKind { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityDwarfResilience_BeltOfDwarvenKind");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFeatBlessingOfTheElementsCold { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFeatBlessingOfTheElementsCold");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFeatBlessingOfTheElementsFire { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFeatBlessingOfTheElementsFire");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFeatBlessingOfTheElementsLightning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFeatBlessingOfTheElementsLightning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceAcid { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceAcid");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceCold { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceCold");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceFire { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceFire");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceForce { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceForce");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceLightning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceLightning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceNecrotic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceNecrotic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResiliencePiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResiliencePiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResiliencePoison { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResiliencePoison");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResiliencePsychic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResiliencePsychic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceRadiant { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceRadiant");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFiendishResilienceThunder { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFiendishResilienceThunder");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireImmunityIronGolem { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireImmunityIronGolem");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireImmunityRemorhaz { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireImmunityRemorhaz");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireImmunityYoungRemorhaz { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireImmunityYoungRemorhaz");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireShieldCold { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireShieldCold");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireShieldWarm { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireShieldWarm");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityForceDamageResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityForceDamageResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityForceImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityForceImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityHalfOrcRelentlessEndurance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityHalfOrcRelentlessEndurance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityLightningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityLightningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityLightningResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityLightningResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityMummyLord_BludgeoningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityMummyLord_BludgeoningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityMummyLord_PiercingImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityMummyLord_PiercingImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityMummyLord_SlashingImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityMummyLord_SlashingImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityNecroticImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityNecroticImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityNecroticResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityNecroticResistance");

        internal static FeatureDefinitionDamageAffinity
            DamageAffinityOathOfMotherlandHeartOfLavaBludgeoningReduction { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>(
                "DamageAffinityOathOfMotherlandHeartOfLavaBludgeoningReduction");

        internal static FeatureDefinitionDamageAffinity
            DamageAffinityOathOfMotherlandHeartOfLavaPiercingReduction { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>(
                "DamageAffinityOathOfMotherlandHeartOfLavaPiercingReduction");

        internal static FeatureDefinitionDamageAffinity
            DamageAffinityOathOfMotherlandHeartOfLavaSlashingReduction { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>(
                "DamageAffinityOathOfMotherlandHeartOfLavaSlashingReduction");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPathClawDragonScales { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPathClawDragonScales");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPatronTreeBlessingOfTheTreeNecrotic { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPatronTreeBlessingOfTheTreeNecrotic");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPatronTreeBlessingOfTheTreePoison { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPatronTreeBlessingOfTheTreePoison");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPatronTreePiercingBranch { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPatronTreePiercingBranch");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPatronTreePiercingBranchOneWithTheTree { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPatronTreePiercingBranchOneWithTheTree");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingImmunity_except_magical { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingImmunity_except_magical");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingResistanceArmorOfDeflection { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingResistanceArmorOfDeflection");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingResistanceByWardingBond { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingResistanceByWardingBond");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingResistanceExceptSilver { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingResistanceExceptSilver");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPoisonAdvantage { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPoisonAdvantage");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPoisonImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPoisonImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPoisonResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPoisonResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPoisonVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPoisonVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPsychicImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPsychicImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPsychicResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPsychicResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityRadiantImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityRadiantImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityRadiantResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityRadiantResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityRadiantVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityRadiantVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingImmunity_except_magical { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingImmunity_except_magical");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingResistanceByWardingBond { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingResistanceByWardingBond");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingResistanceExceptSilver { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingResistanceExceptSilver");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySorakAbomination_SlashingResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySorakAbomination_SlashingResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySorcererDraconicElementalResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySorcererDraconicElementalResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityStoneskinBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityStoneskinBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityStoneskinPiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityStoneskinPiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityStoneskinSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityStoneskinSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityThunderImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityThunderImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityThunderResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityThunderResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityThunderVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityThunderVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityWerewolf_BludgeoningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityWerewolf_BludgeoningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityWerewolf_PiercingImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityWerewolf_PiercingImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityWerewolf_SlashingImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityWerewolf_SlashingImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityWightLord_NecroticImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityWightLord_NecroticImmunity");
    }

    internal static class FeatureDefinitionDeathSavingThrowAffinitys
    {
        internal static FeatureDefinitionDeathSavingThrowAffinity DeathSavingThrowAffinityBeaconOfHope { get; } =
            GetDefinition<FeatureDefinitionDeathSavingThrowAffinity>("DeathSavingThrowAffinityBeaconOfHope");

        internal static FeatureDefinitionDeathSavingThrowAffinity DeathSavingThrowAffinityFeatHardToKill { get; } =
            GetDefinition<FeatureDefinitionDeathSavingThrowAffinity>("DeathSavingThrowAffinityFeatHardToKill");

        internal static FeatureDefinitionDeathSavingThrowAffinity DeathSavingThrowAffinityGateKeeper { get; } =
            GetDefinition<FeatureDefinitionDeathSavingThrowAffinity>("DeathSavingThrowAffinityGateKeeper");
    }

    internal static class FeatureDefinitionDieRollModifiers
    {
        internal static FeatureDefinitionDieRollModifier DieRollModifierBardHeroismBolsterMorale { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierBardHeroismBolsterMorale");

        internal static FeatureDefinitionDieRollModifier DieRollModifierBardTraditionAuraOfPreeminence { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierBardTraditionAuraOfPreeminence");

        internal static FeatureDefinitionDieRollModifier DieRollModifierCaerLemMonsterInitiativeFirst { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierCaerLemMonsterInitiativeFirst");

        internal static FeatureDefinitionDieRollModifier DieRollModifierConditionBardicInspiration { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierConditionBardicInspiration");

        internal static FeatureDefinitionDieRollModifier DieRollModifierConditionManacalonsPerfection { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierConditionManacalonsPerfection");

        internal static FeatureDefinitionDieRollModifier DieRollModifierEmpoweredSpell { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierEmpoweredSpell");

        internal static FeatureDefinitionDieRollModifier DieRollModifierFightingStyleGreatWeapon { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierFightingStyleGreatWeapon");

        internal static FeatureDefinitionDieRollModifier DieRollModifierHalfingLucky { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierHalfingLucky");

        internal static FeatureDefinitionDieRollModifier DieRollModifierHyeronimusAttackRollCritical { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierHyeronimusAttackRollCritical");

        internal static FeatureDefinitionDieRollModifier DieRollModifierPlayerInitiativeFirstHyeronimus { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierPlayerInitiativeFirstHyeronimus");

        internal static FeatureDefinitionDieRollModifier DieRollModifierRogueReliableTalent { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierRogueReliableTalent");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTraditionShockArcanistArcaneWarfare { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTraditionShockArcanistArcaneWarfare");

        internal static FeatureDefinitionDieRollModifier
            DieRollModifierTraditionShockArcanistGreaterArcaneShock { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTraditionShockArcanistGreaterArcaneShock");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialLiamAttackRollDisadvantage { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialLiamAttackRollDisadvantage");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialPlayerAbilityCheckAdvantage { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialPlayerAbilityCheckAdvantage");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialPlayerAttackRollAdvantage { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialPlayerAttackRollAdvantage");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialPlayerAttackRollCritical { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialPlayerAttackRollCritical");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialPlayerInitiativeFirst { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialPlayerInitiativeFirst");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialWolfAbilityCheckPenalty { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialWolfAbilityCheckPenalty");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialWolfAlphaAttack { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialWolfAlphaAttack");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialWolfAlphaDamage { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialWolfAlphaDamage");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialWolfDamagePenalty { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialWolfDamagePenalty");

        internal static FeatureDefinitionDieRollModifier DieRollModifierTutorialWolfInitiativeLast { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierTutorialWolfInitiativeLast");

        internal static FeatureDefinitionDieRollModifier
            DieRollModifierTutorialWolfSavingThrowAndAttackRollPenalty { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>(
                "DieRollModifierTutorialWolfSavingThrowAndAttackRollPenalty");
    }

    internal static class FeatureDefinitionEquipmentAffinitys
    {
        internal static FeatureDefinitionEquipmentAffinity EquipmentAffinityBullsStrength { get; } =
            GetDefinition<FeatureDefinitionEquipmentAffinity>("EquipmentAffinityBullsStrength");

        internal static FeatureDefinitionEquipmentAffinity EquipmentAffinityFeatHauler { get; } =
            GetDefinition<FeatureDefinitionEquipmentAffinity>("EquipmentAffinityFeatHauler");
    }

    internal static class FeatureDefinitionFactionAffinitys
    {
        internal static FeatureDefinitionFactionAffinity FactionAffinityAntiquarians { get; } =
            GetDefinition<FeatureDefinitionFactionAffinity>("FactionAffinityAntiquarians");

        internal static FeatureDefinitionFactionAffinity FactionAffinityTowerOfKnowledge { get; } =
            GetDefinition<FeatureDefinitionFactionAffinity>("FactionAffinityTowerOfKnowledge");
    }

    internal static class FeatureDefinitionFactionChanges
    {
        internal static FeatureDefinitionFactionChange FactionChangeConditionCharmedByAnimalFriendship { get; } =
            GetDefinition<FeatureDefinitionFactionChange>("FactionChangeConditionCharmedByAnimalFriendship");

        internal static FeatureDefinitionFactionChange FactionChangeConditionCharmedByCaster { get; } =
            GetDefinition<FeatureDefinitionFactionChange>("FactionChangeConditionCharmedByCaster");

        internal static FeatureDefinitionFactionChange FactionChangeConditionMindControlledByCaster { get; } =
            GetDefinition<FeatureDefinitionFactionChange>("FactionChangeConditionMindControlledByCaster");

        internal static FeatureDefinitionFactionChange FactionChangeConditionMindDominatedByCaster { get; } =
            GetDefinition<FeatureDefinitionFactionChange>("FactionChangeConditionMindDominatedByCaster");

        internal static FeatureDefinitionFactionChange FactionChangeDebugSpawnGuest { get; } =
            GetDefinition<FeatureDefinitionFactionChange>("FactionChangeDebugSpawnGuest");

        internal static FeatureDefinitionFactionChange FactionChangeWildShape { get; } =
            GetDefinition<FeatureDefinitionFactionChange>("FactionChangeWildShape");
    }

    internal static class FeatureDefinitionFeatureSets
    {
        internal static FeatureDefinitionFeatureSet AdditionalDamageRangerFavoredEnemyChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("AdditionalDamageRangerFavoredEnemyChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetAbilityScoreChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetAbilityScoreChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetAllLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetAllLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetAllLanguagesButCode { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetAllLanguagesButCode");

        internal static FeatureDefinitionFeatureSet FeatureSetBarbarianBrutalCritical { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBarbarianBrutalCritical");

        internal static FeatureDefinitionFeatureSet FeatureSetBarbarianRage { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBarbarianRage");

        internal static FeatureDefinitionFeatureSet FeatureSetBardFontOfInspiration_Obsolete { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBardFontOfInspiration_Obsolete");

        internal static FeatureDefinitionFeatureSet FeatureSetBardicInspiration { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBardicInspiration");

        internal static FeatureDefinitionFeatureSet FeatureSetBardRitualCasting { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBardRitualCasting");

        internal static FeatureDefinitionFeatureSet FeatureSetChampionRemarkableAthlete { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetChampionRemarkableAthlete");

        internal static FeatureDefinitionFeatureSet FeatureSetCircleLandCircleSpells { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetCircleLandCircleSpells");

        internal static FeatureDefinitionFeatureSet FeatureSetCircleLandLandsStride { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetCircleLandLandsStride");

        internal static FeatureDefinitionFeatureSet FeatureSetCircleLandNaturesWard { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetCircleLandNaturesWard");

        internal static FeatureDefinitionFeatureSet FeatureSetClericRitualCasting { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetClericRitualCasting");

        internal static FeatureDefinitionFeatureSet FeatureSetCommanderCoordinatedDefense { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetCommanderCoordinatedDefense");

        internal static FeatureDefinitionFeatureSet FeatureSetDomainInsightDivineLore { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDomainInsightDivineLore");

        internal static FeatureDefinitionFeatureSet FeatureSetDomainLawCommandingPresence { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDomainLawCommandingPresence");

        internal static FeatureDefinitionFeatureSet FeatureSetDomainLawUnyieldingEnforcer { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDomainLawUnyieldingEnforcer");

        internal static FeatureDefinitionFeatureSet FeatureSetDragonbornAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDragonbornAbilityScoreIncrease");

        internal static FeatureDefinitionFeatureSet FeatureSetDragonbornBreathWeapon { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDragonbornBreathWeapon");

        internal static FeatureDefinitionFeatureSet FeatureSetDragonbornDraconicChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDragonbornDraconicChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetDruidRitualCasting { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDruidRitualCasting");

        internal static FeatureDefinitionFeatureSet FeatureSetElfFeyAncestry { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetElfFeyAncestry");

        internal static FeatureDefinitionFeatureSet FeatureSetElfHighLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetElfHighLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetElfSylvanLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetElfSylvanLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetGnomeRockTinker { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetGnomeRockTinker");

        internal static FeatureDefinitionFeatureSet FeatureSetGreenmageWardenOfTheForest { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetGreenmageWardenOfTheForest");

        internal static FeatureDefinitionFeatureSet FeatureSetHalfElfAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHalfElfAbilityScoreIncrease");

        internal static FeatureDefinitionFeatureSet FeatureSetHalfElfLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHalfElfLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetHalflingIslandLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHalflingIslandLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetHalflingMarshLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHalflingMarshLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetHalfOrcAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHalfOrcAbilityScoreIncrease");

        internal static FeatureDefinitionFeatureSet FeatureSetHumanAbilityScoreChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHumanAbilityScoreChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetHumanAbilityScoreIncreaseAdvanced { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHumanAbilityScoreIncreaseAdvanced");

        internal static FeatureDefinitionFeatureSet FeatureSetHumanLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHumanLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetHunterDefensiveTactics { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHunterDefensiveTactics");

        internal static FeatureDefinitionFeatureSet FeatureSetHunterHuntersPrey { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHunterHuntersPrey");

        internal static FeatureDefinitionFeatureSet FeatureSetInvocationBookAncientSecrets { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetInvocationBookAncientSecrets");

        internal static FeatureDefinitionFeatureSet FeatureSetInvocationDevilsSight { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetInvocationDevilsSight");

        internal static FeatureDefinitionFeatureSet FeatureSetInvocationWitchSight { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetInvocationWitchSight");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritBond { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritBond");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritCall { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritCall");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritMagicalSpirit { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritMagicalSpirit");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritRage { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritRage");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritSharedPain { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritSharedPain");

        internal static FeatureDefinitionFeatureSet FeatureSetLoreMasterArcaneLore { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetLoreMasterArcaneLore");

        internal static FeatureDefinitionFeatureSet FeatureSetLoreMasterArcaneProfessor { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetLoreMasterArcaneProfessor");

        internal static FeatureDefinitionFeatureSet FeatureSetLoremasterKeenMind { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetLoremasterKeenMind");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkDeflectMissiles { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkDeflectMissiles");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkDiamondSoul { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkDiamondSoul");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkFlurryOfBlows { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkFlurryOfBlows");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkPurityOfBody { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkPurityOfBody");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkStepOfTheWind { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkStepOfTheWind");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkStillnessOfMind { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkStillnessOfMind");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkTimelessBody { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkTimelessBody");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkTongueSunMoon { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkTongueSunMoon");

        internal static FeatureDefinitionFeatureSet FeatureSetOathOfMotherlandHeartOfLava { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetOathOfMotherlandHeartOfLava");

        internal static FeatureDefinitionFeatureSet FeatureSetPactBlade { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactBlade");

        internal static FeatureDefinitionFeatureSet FeatureSetPactChain { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactChain");

        internal static FeatureDefinitionFeatureSet FeatureSetPactSelection { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactSelection");

        internal static FeatureDefinitionFeatureSet FeatureSetPactTome { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactTome");

        internal static FeatureDefinitionFeatureSet FeatureSetPathClawDragonAncestry { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPathClawDragonAncestry");

        internal static FeatureDefinitionFeatureSet FeatureSetPathClawDragonScales { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPathClawDragonScales");

        internal static FeatureDefinitionFeatureSet FeatureSetPatronFiendFiendishResilience { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPatronFiendFiendishResilience");

        internal static FeatureDefinitionFeatureSet FeatureSetPatronTreeBlessingOfTheTree { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPatronTreeBlessingOfTheTree");

        internal static FeatureDefinitionFeatureSet FeatureSetPatronTreeOneWithTheTree { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPatronTreeOneWithTheTree");

        internal static FeatureDefinitionFeatureSet FeatureSetRangerHunterMultiAttackChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetRangerHunterMultiAttackChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetRangerHunterSuperiorHuntersDefenseChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetRangerHunterSuperiorHuntersDefenseChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetRoguishHoodlumTheRightTools { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetRoguishHoodlumTheRightTools");

        internal static FeatureDefinitionFeatureSet FeatureSetShadowTamerDarkSlayer { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetShadowTamerDarkSlayer");

        internal static FeatureDefinitionFeatureSet FeatureSetShadowTamerKnowDarkness { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetShadowTamerKnowDarkness");

        internal static FeatureDefinitionFeatureSet FeatureSetShadowy { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetShadowy");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererChildRiftRiftwalk { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererChildRiftRiftwalk");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererDraconicChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererDraconicChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererDraconicDragonWings { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererDraconicDragonWings");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererDraconicResilience { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererDraconicResilience");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererFlexibleCasting { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererFlexibleCasting");

        internal static FeatureDefinitionFeatureSet FeatureSetSpellCrusher { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSpellCrusher");

        internal static FeatureDefinitionFeatureSet FeatureSetTieflingHellishResistance { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTieflingHellishResistance");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionCourtMageAlwaysPrepared { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionCourtMageAlwaysPrepared");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionFreedomSwiftSteps { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionFreedomSwiftSteps");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionFreedomUnendingStrikes { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionFreedomUnendingStrikes");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionLightRadiantStrikes { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionLightRadiantStrikes");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionOpenHandOpenHandTechnique { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionOpenHandOpenHandTechnique");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionSurvivalDefensiveStance { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionSurvivalDefensiveStance");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionSurvivalUnbreakableBody { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionSurvivalUnbreakableBody");

        internal static FeatureDefinitionFeatureSet FeatureSetUndeadFeatures { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetUndeadFeatures");

        internal static FeatureDefinitionFeatureSet FeatureSetWindUnfettered { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetWindUnfettered");

        internal static FeatureDefinitionFeatureSet FeatureSetWizardRitualCasting { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetWizardRitualCasting");

        internal static FeatureDefinitionFeatureSet TerrainTypeAffinityRangerNaturalExplorerChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("TerrainTypeAffinityRangerNaturalExplorerChoice");
    }

    internal static class FeatureDefinitionFightingStyleChoices
    {
        internal static FeatureDefinitionFightingStyleChoice FightingStyleChampionAdditional { get; } =
            GetDefinition<FeatureDefinitionFightingStyleChoice>("FightingStyleChampionAdditional");

        internal static FeatureDefinitionFightingStyleChoice FightingStyleFighter { get; } =
            GetDefinition<FeatureDefinitionFightingStyleChoice>("FightingStyleFighter");

        internal static FeatureDefinitionFightingStyleChoice FightingStylePaladin { get; } =
            GetDefinition<FeatureDefinitionFightingStyleChoice>("FightingStylePaladin");

        internal static FeatureDefinitionFightingStyleChoice FightingStyleRanger { get; } =
            GetDefinition<FeatureDefinitionFightingStyleChoice>("FightingStyleRanger");
    }

    internal static class FeatureDefinitionHealingModifiers
    {
        internal static FeatureDefinitionHealingModifier HealingModifierBeaconOfHope { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierBeaconOfHope");

        internal static FeatureDefinitionHealingModifier HealingModifierChilledByTouch { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierChilledByTouch");

        internal static FeatureDefinitionHealingModifier HealingModifierCircleBalanceGiftOfLife { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierCircleBalanceGiftOfLife");

        internal static FeatureDefinitionHealingModifier HealingModifierDomainLifeBlessedHealer { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierDomainLifeBlessedHealer");

        internal static FeatureDefinitionHealingModifier HealingModifierDomainLifeDiscipleOfLife { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierDomainLifeDiscipleOfLife");

        internal static FeatureDefinitionHealingModifier HealingModifierFeatMender { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierFeatMender");

        internal static FeatureDefinitionHealingModifier HealingModifierFeatRobust { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierFeatRobust");

        internal static FeatureDefinitionHealingModifier HealingModifierKindredSpiritBond { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierKindredSpiritBond");

        internal static FeatureDefinitionHealingModifier HealingModifierKindredSpiritMagicalSpirit { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierKindredSpiritMagicalSpirit");

        internal static FeatureDefinitionHealingModifier HealingModifierNegativeEnergy { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierNegativeEnergy");
    }

    internal static class FeatureDefinitionLightAffinitys
    {
        internal static FeatureDefinitionLightAffinity LightAffinity_ApostleOfDarkness { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_ApostleOfDarkness");

        internal static FeatureDefinitionLightAffinity LightAffinity_ApostleOfDarkness_Assassin { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_ApostleOfDarkness_Assassin");

        internal static FeatureDefinitionLightAffinity LightAffinity_ChildOfDarkness { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_ChildOfDarkness");

        internal static FeatureDefinitionLightAffinity LightAffinity_ChildOfDarknessSaboteur { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_ChildOfDarknessSaboteur");

        internal static FeatureDefinitionLightAffinity LightAffinity_Defiler_Darkness { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_Defiler_Darkness");

        internal static FeatureDefinitionLightAffinity LightAffinity_DemonGeneral { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_DemonGeneral");

        internal static FeatureDefinitionLightAffinity LightAffinity_ProphetOfDarkness { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_ProphetOfDarkness");

        internal static FeatureDefinitionLightAffinity LightAffinity_ProphetOfDarkness_Shikkath { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinity_ProphetOfDarkness_Shikkath");

        internal static FeatureDefinitionLightAffinity LightAffinityHypersensitivity { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinityHypersensitivity");

        internal static FeatureDefinitionLightAffinity LightAffinityInvocationOneWithShadows { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinityInvocationOneWithShadows");

        internal static FeatureDefinitionLightAffinity LightAffinityLightSensitivity { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinityLightSensitivity");

        internal static FeatureDefinitionLightAffinity LightAffinityLightSensitivityImmunitySaboteur { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinityLightSensitivityImmunitySaboteur");
    }

    internal static class FeatureDefinitionLightSources
    {
        internal static FeatureDefinitionLightSource LightSourceFireElemental { get; } =
            GetDefinition<FeatureDefinitionLightSource>("LightSourceFireElemental");
    }

    internal static class FeatureDefinitionMagicAffinitys
    {
        internal static FeatureDefinitionMagicAffinity MagicAffinityAdditionalSpellSlot1 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAdditionalSpellSlot1");

        internal static FeatureDefinitionMagicAffinity MagicAffinityAdditionalSpellSlot2 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAdditionalSpellSlot2");

        internal static FeatureDefinitionMagicAffinity MagicAffinityAdditionalSpellSlot3 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAdditionalSpellSlot3");

        internal static FeatureDefinitionMagicAffinity MagicAffinityAdditionalSpellSlot4 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAdditionalSpellSlot4");

        internal static FeatureDefinitionMagicAffinity MagicAffinityAdditionalSpellSlot5 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAdditionalSpellSlot5");

        internal static FeatureDefinitionMagicAffinity MagicAffinityAnnoyedByBee { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAnnoyedByBee");

        internal static FeatureDefinitionMagicAffinity MagicAffinityArcaneAppraiser { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityArcaneAppraiser");

        internal static FeatureDefinitionMagicAffinity MagicAffinityBardRitualCasting { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityBardRitualCasting");

        internal static FeatureDefinitionMagicAffinity MagicAffinityBattleMagic { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityBattleMagic");

        internal static FeatureDefinitionMagicAffinity MagicAffinityCantUseMagic { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityCantUseMagic");

        internal static FeatureDefinitionMagicAffinity MagicAffinityChainsCarceriImmunity { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityChainsCarceriImmunity");

        internal static FeatureDefinitionMagicAffinity MagicAffinityChitinousBoonAdditionalSpellSlot { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityChitinousBoonAdditionalSpellSlot");

        internal static FeatureDefinitionMagicAffinity MagicAffinityCircleBalanceSurvivalOfTheWisest { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityCircleBalanceSurvivalOfTheWisest");

        internal static FeatureDefinitionMagicAffinity MagicAffinityCircleLandLandsStride { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityCircleLandLandsStride");

        internal static FeatureDefinitionMagicAffinity MagicAffinityClericRitualCasting { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityClericRitualCasting");

        internal static FeatureDefinitionMagicAffinity MagicAffinityCollegeLorePeerlessSkill { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityCollegeLorePeerlessSkill");

        internal static FeatureDefinitionMagicAffinity MagicAffinityCombatCasting { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityCombatCasting");

        internal static FeatureDefinitionMagicAffinity MagicAffinityConditionBranded { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityConditionBranded");

        internal static FeatureDefinitionMagicAffinity MagicAffinityConditionImmuneToShine { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityConditionImmuneToShine");

        internal static FeatureDefinitionMagicAffinity MagicAffinityConditionRaging { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityConditionRaging");

        internal static FeatureDefinitionMagicAffinity MagicAffinityConditionShielded { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityConditionShielded");

        internal static FeatureDefinitionMagicAffinity MagicAffinityConditionSlowed { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityConditionSlowed");

        internal static FeatureDefinitionMagicAffinity MagicAffinityCourtMageCounterspellMastery { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityCourtMageCounterspellMastery");

        internal static FeatureDefinitionMagicAffinity MagicAffinityDemonGreaseNightHunt { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityDemonGreaseNightHunt");

        internal static FeatureDefinitionMagicAffinity MagicAffinityDistracted { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityDistracted");

        internal static FeatureDefinitionMagicAffinity MagicAffinityDomainLawForceOfLawImposeDisadvantage { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityDomainLawForceOfLawImposeDisadvantage");

        internal static FeatureDefinitionMagicAffinity MagicAffinityDomainSunHolyRadiance { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityDomainSunHolyRadiance");

        internal static FeatureDefinitionMagicAffinity MagicAffinityDruidRitualCasting { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityDruidRitualCasting");

        internal static FeatureDefinitionMagicAffinity MagicAffinityEvocationPowerfulCantrip { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityEvocationPowerfulCantrip");

        internal static FeatureDefinitionMagicAffinity MagicAffinityFeatFlawlessConcentration { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityFeatFlawlessConcentration");

        internal static FeatureDefinitionMagicAffinity MagicAffinityFeatMasterAlchemist { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityFeatMasterAlchemist");

        internal static FeatureDefinitionMagicAffinity MagicAffinityFeatPowerfulCantrip { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityFeatPowerfulCantrip");

        internal static FeatureDefinitionMagicAffinity MagicAffinityGreenmageGreenMagicList { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityGreenmageGreenMagicList");

        internal static FeatureDefinitionMagicAffinity MagicAffinityInvocationBookAncientSecrets { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityInvocationBookAncientSecrets");

        internal static FeatureDefinitionMagicAffinity MagicAffinityInvocationWitchSight { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityInvocationWitchSight");

        internal static FeatureDefinitionMagicAffinity MagicAffinityLoreMasterArcaneLore { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityLoreMasterArcaneLore");

        internal static FeatureDefinitionMagicAffinity MagicAffinityLoreMasterArcaneProfessor { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityLoreMasterArcaneProfessor");

        internal static FeatureDefinitionMagicAffinity MagicAffinityLoremasterKeenMindScribing { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityLoremasterKeenMindScribing");

        internal static FeatureDefinitionMagicAffinity MagicAffinityLoreMasterSpellAcademic { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityLoreMasterSpellAcademic");

        internal static FeatureDefinitionMagicAffinity MagicAffinityPatronFiendExpandedSpells { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityPatronFiendExpandedSpells");

        internal static FeatureDefinitionMagicAffinity MagicAffinityPatronHiveExpandedSpells { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityPatronHiveExpandedSpells");

        internal static FeatureDefinitionMagicAffinity MagicAffinityPatronTimekeeperExpandedSpells { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityPatronTimekeeperExpandedSpells");

        internal static FeatureDefinitionMagicAffinity MagicAffinityPatronTreeExpandedSpells { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityPatronTreeExpandedSpells");

        internal static FeatureDefinitionMagicAffinity MagicAffinityShadowRetribution { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityShadowRetribution");

        internal static FeatureDefinitionMagicAffinity MagicAffinityShockArcanistArcaneWarfare { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityShockArcanistArcaneWarfare");

        internal static FeatureDefinitionMagicAffinity MagicAffinitySilenced { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinitySilenced");

        internal static FeatureDefinitionMagicAffinity MagicAffinitySilencedDemonGreaseTrueStrike { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinitySilencedDemonGreaseTrueStrike");

        internal static FeatureDefinitionMagicAffinity MagicAffinitySorcererChildRiftMagic { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinitySorcererChildRiftMagic");

        internal static FeatureDefinitionMagicAffinity MagicAffinitySorcererManaPainterManaBalance { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinitySorcererManaPainterManaBalance");

        internal static FeatureDefinitionMagicAffinity MagicAffinitySpellBladeIntoTheFray { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinitySpellBladeIntoTheFray");

        internal static FeatureDefinitionMagicAffinity MagicAffinityStunningStrikeBracers { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityStunningStrikeBracers");

        internal static FeatureDefinitionMagicAffinity MagicAffinityUseMagicalItem { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityUseMagicalItem");

        internal static FeatureDefinitionMagicAffinity MagicAffinityWandOfWarMagePlus1 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityWandOfWarMage+1");

        internal static FeatureDefinitionMagicAffinity MagicAffinityWandOfWarMagePlus2 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityWandOfWarMage+2");

        internal static FeatureDefinitionMagicAffinity MagicAffinityWizardRitualCasting { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityWizardRitualCasting");
    }

    internal static class FeatureDefinitionMovementAffinitys
    {
        internal static FeatureDefinitionMovementAffinity MovementAffinity_PalaceOfIce_LairEffect { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinity_PalaceOfIce_LairEffect");

        internal static FeatureDefinitionMovementAffinity MovementAffinityBarbarianFastMovement { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityBarbarianFastMovement");

        internal static FeatureDefinitionMovementAffinity MovementAffinityBootsOfStriding { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityBootsOfStriding");

        internal static FeatureDefinitionMovementAffinity MovementAffinityCarriedByWind { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityCarriedByWind");

        internal static FeatureDefinitionMovementAffinity MovementAffinityCatsGrace { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityCatsGrace");

        internal static FeatureDefinitionMovementAffinity MovementAffinityChampionRemarkableAthlete { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityChampionRemarkableAthlete");

        internal static FeatureDefinitionMovementAffinity MovementAffinityCircleLandLandsStride { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityCircleLandLandsStride");

        internal static FeatureDefinitionMovementAffinity MovementAffinityCloakOfArachnida { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityCloakOfArachnida");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionChilled { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionChilled");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionDashing { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionDashing");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionDashingAdditional { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionDashingAdditional");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionDashingBonus { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionDashingBonus");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionDashingExpeditious { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionDashingExpeditious");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionEncumbered { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionEncumbered");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionFlyingAdaptive { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionFlyingAdaptive");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionFrozen { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionFrozen");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionHasted { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionHasted");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionHeavilyEncumbered { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionHeavilyEncumbered");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionHindered { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionHindered");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionLethargic { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionLethargic");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionLevitate { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionLevitate");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionRestrained { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionRestrained");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionSlowed { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionSlowed");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionSpellbladeArcaneEscape { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionSpellbladeArcaneEscape");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionSurprised { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionSurprised");

        internal static FeatureDefinitionMovementAffinity MovementAffinityDarkweaverSpiderOnWall { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityDarkweaverSpiderOnWall");

        internal static FeatureDefinitionMovementAffinity MovementAffinityDruidCircleWindsUnfettered { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityDruidCircleWindsUnfettered");

        internal static FeatureDefinitionMovementAffinity MovementAffinityFeatDauntingPush { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityFeatDauntingPush");

        internal static FeatureDefinitionMovementAffinity MovementAffinityFeatForestRunner { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityFeatForestRunner");

        internal static FeatureDefinitionMovementAffinity MovementAffinityFeatRushToBattle { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityFeatRushToBattle");

        internal static FeatureDefinitionMovementAffinity MovementAffinityFreedomOfMovement { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityFreedomOfMovement");

        internal static FeatureDefinitionMovementAffinity MovementAffinityHeavyArmorImmunity { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityHeavyArmorImmunity");

        internal static FeatureDefinitionMovementAffinity MovementAffinityHeavyArmorOverload { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityHeavyArmorOverload");

        internal static FeatureDefinitionMovementAffinity MovementAffinityJump { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityJump");

        internal static FeatureDefinitionMovementAffinity MovementAffinityKindredSpiritSpider { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityKindredSpiritSpider");

        internal static FeatureDefinitionMovementAffinity MovementAffinityKindredSpiritViper { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityKindredSpiritViper");

        internal static FeatureDefinitionMovementAffinity MovementAffinityLongstrider { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityLongstrider");

        internal static FeatureDefinitionMovementAffinity MovementAffinityMonkUnarmoredMovement { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityMonkUnarmoredMovement");

        internal static FeatureDefinitionMovementAffinity MovementAffinityMonkUnarmoredMovementImproved { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityMonkUnarmoredMovementImproved");

        internal static FeatureDefinitionMovementAffinity MovementAffinityNoClimb { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityNoClimb");

        internal static FeatureDefinitionMovementAffinity MovementAffinityNoSpecialMoves { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityNoSpecialMoves");

        internal static FeatureDefinitionMovementAffinity MovementAffinityNoVault { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityNoVault");

        internal static FeatureDefinitionMovementAffinity MovementAffinityPactChainSprite { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityPactChainSprite");

        internal static FeatureDefinitionMovementAffinity MovementAffinityRangerSwiftBladeQuickStep { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityRangerSwiftBladeQuickStep");

        internal static FeatureDefinitionMovementAffinity MovementAffinitySixLeaguesBoots { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinitySixLeaguesBoots");

        internal static FeatureDefinitionMovementAffinity MovementAffinitySpiderClimb { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinitySpiderClimb");

        internal static FeatureDefinitionMovementAffinity MovementAffinitySpiritGuardians { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinitySpiritGuardians");

        internal static FeatureDefinitionMovementAffinity MovementAffinityThiefSecondStory { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityThiefSecondStory");

        internal static FeatureDefinitionMovementAffinity MovementAffinityTraditionShockArcanistArcaneShocked { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityTraditionShockArcanistArcaneShocked");

        internal static FeatureDefinitionMovementAffinity MovementAffinityTutorialSafetyWolves { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityTutorialSafetyWolves");
    }

    internal static class FeatureDefinitionMoveModes
    {
        internal static FeatureDefinitionMoveMode MoveModeBurrow10 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeBurrow10");

        internal static FeatureDefinitionMoveMode MoveModeBurrow16 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeBurrow16");

        internal static FeatureDefinitionMoveMode MoveModeBurrow8 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeBurrow8");

        internal static FeatureDefinitionMoveMode MoveModeClimb6 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeClimb6");

        internal static FeatureDefinitionMoveMode MoveModeElfSylvanMoveSpeed { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeElfSylvanMoveSpeed");

        internal static FeatureDefinitionMoveMode MoveModeFly10 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly10");

        internal static FeatureDefinitionMoveMode MoveModeFly12 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly12");

        internal static FeatureDefinitionMoveMode MoveModeFly18 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly18");

        internal static FeatureDefinitionMoveMode MoveModeFly2 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly2");

        internal static FeatureDefinitionMoveMode MoveModeFly4 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly4");

        internal static FeatureDefinitionMoveMode MoveModeFly6 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly6");

        internal static FeatureDefinitionMoveMode MoveModeFly8 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly8");

        internal static FeatureDefinitionMoveMode MoveModeMove10 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove10");

        internal static FeatureDefinitionMoveMode MoveModeMove12 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove12");

        internal static FeatureDefinitionMoveMode MoveModeMove2 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove2");

        internal static FeatureDefinitionMoveMode MoveModeMove4 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove4");

        internal static FeatureDefinitionMoveMode MoveModeMove5 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove5");

        internal static FeatureDefinitionMoveMode MoveModeMove6 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove6");

        internal static FeatureDefinitionMoveMode MoveModeMove7 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove7");

        internal static FeatureDefinitionMoveMode MoveModeMove8 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove8");
    }

    internal static class FeatureDefinitionMoveThroughEnemyModifiers
    {
        internal static FeatureDefinitionMoveThroughEnemyModifier MoveThroughEnemyModifierHalflingNimbleness { get; } =
            GetDefinition<FeatureDefinitionMoveThroughEnemyModifier>("MoveThroughEnemyModifierHalflingNimbleness");
    }

    internal static class FeatureDefinitionPerceptionAffinitys
    {
        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionBlinded { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionBlinded");

        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionDetectedAsEvilGood { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionDetectedAsEvilGood");

        internal static FeatureDefinitionPerceptionAffinity
            PerceptionAffinityConditionDetectedAsPoisonedOrDiseased { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>(
                "PerceptionAffinityConditionDetectedAsPoisonedOrDiseased");

        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionDivinelyRevealed { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionDivinelyRevealed");

        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionHighlighted { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionHighlighted");

        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionInvisible { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionInvisible");
    }

    internal static class FeatureDefinitionPointPools
    {
        internal static FeatureDefinitionPointPool PointPoolAbilityScoreImprovement { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolAbilityScoreImprovement");

        internal static FeatureDefinitionPointPool PointPoolBackgroundLanguageChoice_one { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBackgroundLanguageChoice_one");

        internal static FeatureDefinitionPointPool PointPoolBackgroundLanguageChoice_two { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBackgroundLanguageChoice_two");

        internal static FeatureDefinitionPointPool PointPoolBarbarianrSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBarbarianrSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolBardExpertiseLevel10 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardExpertiseLevel10");

        internal static FeatureDefinitionPointPool PointPoolBardExpertiseLevel3 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardExpertiseLevel3");

        internal static FeatureDefinitionPointPool PointPoolBardMagicalSecrets10 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardMagicalSecrets10");

        internal static FeatureDefinitionPointPool PointPoolBardMagicalSecrets14 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardMagicalSecrets14");

        internal static FeatureDefinitionPointPool PointPoolBardSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolBonusFeat { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBonusFeat");

        internal static FeatureDefinitionPointPool PointPoolCircleLandBonusCantrip { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolCircleLandBonusCantrip");

        internal static FeatureDefinitionPointPool PointPoolClericSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolClericSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolCollegeLoreAdditionalMagicalSecrets { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolCollegeLoreAdditionalMagicalSecrets");

        internal static FeatureDefinitionPointPool PointPoolCollegeLoreBonusSkills { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolCollegeLoreBonusSkills");

        internal static FeatureDefinitionPointPool PointPoolDruidSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolDruidSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolElfHighLanguageChoice { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolElfHighLanguageChoice");

        internal static FeatureDefinitionPointPool PointPoolFighterSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolFighterSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolHalfElfAbilityScoreIncreasePool { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHalfElfAbilityScoreIncreasePool");

        internal static FeatureDefinitionPointPool PointPoolHalfElfLanguageChoice { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHalfElfLanguageChoice");

        internal static FeatureDefinitionPointPool PointPoolHalfElfSkillPool { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHalfElfSkillPool");

        internal static FeatureDefinitionPointPool PointPoolHalflingIslandLanguageChoice { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHalflingIslandLanguageChoice");

        internal static FeatureDefinitionPointPool PointPoolHumanAbilityScorePool2 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHumanAbilityScorePool2");

        internal static FeatureDefinitionPointPool PointPoolHumanLanguageChoice { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHumanLanguageChoice");

        internal static FeatureDefinitionPointPool PointPoolHumanSkillPool { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHumanSkillPool");

        internal static FeatureDefinitionPointPool PointPoolInvocationBookAncientSecrets { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolInvocationBookAncientSecrets");

        internal static FeatureDefinitionPointPool PointPoolLoreMasterArcaneLore { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolLoreMasterArcaneLore");

        internal static FeatureDefinitionPointPool PointPoolLoreMasterArcaneProfessor { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolLoreMasterArcaneProfessor");

        internal static FeatureDefinitionPointPool PointPoolMonkSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolMonkSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolPactTome { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolPactTome");

        internal static FeatureDefinitionPointPool PointPoolPaladinSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolPaladinSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolRangerSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolRangerSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolRogueExpertise { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolRogueExpertise");

        internal static FeatureDefinitionPointPool PointPoolRogueSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolRogueSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolSorcererAdditionalMetamagic { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolSorcererAdditionalMetamagic");

        internal static FeatureDefinitionPointPool PointPoolSorcererMetamagic { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolSorcererMetamagic");

        internal static FeatureDefinitionPointPool PointPoolSorcererSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolSorcererSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation12 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation12");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation15 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation15");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation2 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation2");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation5 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation5");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation7 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation7");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation9 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation9");

        internal static FeatureDefinitionPointPool PointPoolWarlockMysticArcanum6 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockMysticArcanum6");

        internal static FeatureDefinitionPointPool PointPoolWarlockMysticArcanum7 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockMysticArcanum7");

        internal static FeatureDefinitionPointPool PointPoolWarlockMysticArcanum8 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockMysticArcanum8");

        internal static FeatureDefinitionPointPool PointPoolWarlockSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolWizardSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWizardSkillPoints");

        internal static FeatureDefinitionPointPool ProficiencyMarksmanToolChoice { get; } =
            GetDefinition<FeatureDefinitionPointPool>("ProficiencyMarksmanToolChoice");
    }

    internal static class FeatureDefinitionPowers
    {
        internal static FeatureDefinitionPower Power_HornOfBlasting { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_HornOfBlasting");

        internal static FeatureDefinitionPower Power_Legendary_AncientRemorhaz_Feed { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Legendary_AncientRemorhaz_Feed");

        internal static FeatureDefinitionPower Power_Legendary_AncientRemorhaz_SonicBlast { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Legendary_AncientRemorhaz_SonicBlast");

        internal static FeatureDefinitionPower Power_Legendary_MummyLord_BlasphemousWords { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Legendary_MummyLord_BlasphemousWords");

        internal static FeatureDefinitionPower Power_Legendary_MummyLord_BlindingDust { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Legendary_MummyLord_BlindingDust");

        internal static FeatureDefinitionPower Power_Legendary_MummyLord_ChannelNegativeEnergy { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Legendary_MummyLord_ChannelNegativeEnergy");

        internal static FeatureDefinitionPower Power_Legendary_MummyLord_Whirlwind { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Legendary_MummyLord_Whirlwind");

        internal static FeatureDefinitionPower Power_Mummy_DreadfulGlare { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Mummy_DreadfulGlare");

        internal static FeatureDefinitionPower Power_MummyLord_DreadfulGlare { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_MummyLord_DreadfulGlare");

        internal static FeatureDefinitionPower PowerAncientRemorhazSwallow { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerAncientRemorhazSwallow");

        internal static FeatureDefinitionPower PowerArrokAuraOfFire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerArrokAuraOfFire");

        internal static FeatureDefinitionPower PowerBarbarianPersistentRageStart { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBarbarianPersistentRageStart");

        internal static FeatureDefinitionPower PowerBarbarianRageStart { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBarbarianRageStart");

        internal static FeatureDefinitionPower PowerBarbarianRageStop { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBarbarianRageStop");

        internal static FeatureDefinitionPower PowerBardCountercharm { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardCountercharm");

        internal static FeatureDefinitionPower PowerBardGiveBardicInspiration { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardGiveBardicInspiration");

        internal static FeatureDefinitionPower PowerBardHeroismAtRoadsEnd { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHeroismAtRoadsEnd");

        internal static FeatureDefinitionPower PowerBardHeroismBolsterMorale { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHeroismBolsterMorale");

        internal static FeatureDefinitionPower PowerBardHeroismHeroicTale { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHeroismHeroicTale");

        internal static FeatureDefinitionPower PowerBardHeroismThunderingVoice { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHeroismThunderingVoice");

        internal static FeatureDefinitionPower PowerBardHopeSingSongOfHope { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeSingSongOfHope");

        internal static FeatureDefinitionPower PowerBardHopeStartSongOfHope { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeStartSongOfHope");

        internal static FeatureDefinitionPower PowerBardHopeWordsOfHope13 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeWordsOfHope13");

        internal static FeatureDefinitionPower PowerBardHopeWordsOfHope17 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeWordsOfHope17");

        internal static FeatureDefinitionPower PowerBardHopeWordsOfHope6 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeWordsOfHope6");

        internal static FeatureDefinitionPower PowerBardHopeWordsOfHope9 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeWordsOfHope9");

        internal static FeatureDefinitionPower PowerBardTraditionAncientTradition { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardTraditionAncientTradition");

        internal static FeatureDefinitionPower PowerBardTraditionManacalonsPerfection { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardTraditionManacalonsPerfection");

        internal static FeatureDefinitionPower PowerBardTraditionVerbalOnslaught { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardTraditionVerbalOnslaught");

        internal static FeatureDefinitionPower PowerBerserkerFrenzy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBerserkerFrenzy");

        internal static FeatureDefinitionPower PowerBerserkerIntimidatingPresence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBerserkerIntimidatingPresence");

        internal static FeatureDefinitionPower PowerBerserkerMindlessRage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBerserkerMindlessRage");

        internal static FeatureDefinitionPower PowerBreath_GlabrezuGeneral { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBreath_GlabrezuGeneral");

        internal static FeatureDefinitionPower PowerBulette_Snow_Leap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBulette_Snow_Leap");

        internal static FeatureDefinitionPower PowerBuletteLeap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBuletteLeap");

        internal static FeatureDefinitionPower PowerCallLightning { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerCallLightning");

        internal static FeatureDefinitionPower PowerCircleLandNaturalRecovery { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerCircleLandNaturalRecovery");

        internal static FeatureDefinitionPower PowerClayGolemHaste { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClayGolemHaste");

        internal static FeatureDefinitionPower PowerClericDivineInterventionCleric { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericDivineInterventionCleric");

        internal static FeatureDefinitionPower PowerClericDivineInterventionPaladin { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericDivineInterventionPaladin");

        internal static FeatureDefinitionPower PowerClericDivineInterventionWizard { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericDivineInterventionWizard");

        internal static FeatureDefinitionPower PowerClericTurnUndead { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead");

        internal static FeatureDefinitionPower PowerClericTurnUndead11 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead11");

        internal static FeatureDefinitionPower PowerClericTurnUndead14 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead14");

        internal static FeatureDefinitionPower PowerClericTurnUndead5 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead5");

        internal static FeatureDefinitionPower PowerClericTurnUndead8 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead8");

        internal static FeatureDefinitionPower PowerCollegeLoreCuttingWords { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerCollegeLoreCuttingWords");

        internal static FeatureDefinitionPower PowerCouatlConstrict { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerCouatlConstrict");

        internal static FeatureDefinitionPower PowerDefilerDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDefilerDarkness");

        internal static FeatureDefinitionPower PowerDefilerEatFriends { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDefilerEatFriends");

        internal static FeatureDefinitionPower PowerDefilerMistyFormEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDefilerMistyFormEscape");

        internal static FeatureDefinitionPower PowerDelayedBlastFireballDetonate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDelayedBlastFireballDetonate");

        internal static FeatureDefinitionPower PowerDispelEvilBreakEnchantment { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDispelEvilBreakEnchantment");

        internal static FeatureDefinitionPower PowerDispelEvilDismissal { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDispelEvilDismissal");

        internal static FeatureDefinitionPower PowerDomainBattleDecisiveStrike { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDecisiveStrike");

        internal static FeatureDefinitionPower PowerDomainBattleDecisiveStrike11 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDecisiveStrike11");

        internal static FeatureDefinitionPower PowerDomainBattleDecisiveStrike14 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDecisiveStrike14");

        internal static FeatureDefinitionPower PowerDomainBattleDecisiveStrike5 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDecisiveStrike5");

        internal static FeatureDefinitionPower PowerDomainBattleDecisiveStrike8 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDecisiveStrike8");

        internal static FeatureDefinitionPower PowerDomainBattleDivineWrath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDivineWrath");

        internal static FeatureDefinitionPower PowerDomainBattleHeraldOfBattle { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleHeraldOfBattle");

        internal static FeatureDefinitionPower PowerDomainElementalDiscipleOfTheElementsCold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalDiscipleOfTheElementsCold");

        internal static FeatureDefinitionPower PowerDomainElementalDiscipleOfTheElementsFire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalDiscipleOfTheElementsFire");

        internal static FeatureDefinitionPower PowerDomainElementalDiscipleOfTheElementsLightning { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalDiscipleOfTheElementsLightning");

        internal static FeatureDefinitionPower PowerDomainElementalFireBurst { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalFireBurst");

        internal static FeatureDefinitionPower PowerDomainElementalHeraldOfTheElementsCold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalHeraldOfTheElementsCold");

        internal static FeatureDefinitionPower PowerDomainElementalHeraldOfTheElementsFire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalHeraldOfTheElementsFire");

        internal static FeatureDefinitionPower PowerDomainElementalHeraldOfTheElementsThunder { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalHeraldOfTheElementsThunder");

        internal static FeatureDefinitionPower PowerDomainElementalIceLance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalIceLance");

        internal static FeatureDefinitionPower PowerDomainElementalLightningBlade { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalLightningBlade");

        internal static FeatureDefinitionPower PowerDomainInsightDivineLoreIdentification { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainInsightDivineLoreIdentification");

        internal static FeatureDefinitionPower PowerDomainInsightForeknowledge { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainInsightForeknowledge");

        internal static FeatureDefinitionPower PowerDomainLawAnathema { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawAnathema");

        internal static FeatureDefinitionPower PowerDomainLawAnathemaImprovement { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawAnathemaImprovement");

        internal static FeatureDefinitionPower PowerDomainLawForceOfLaw { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawForceOfLaw");

        internal static FeatureDefinitionPower PowerDomainLawHolyRetribution { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawHolyRetribution");

        internal static FeatureDefinitionPower PowerDomainLawWordOfLaw { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawWordOfLaw");

        internal static FeatureDefinitionPower PowerDomainLifePreserveLife { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLifePreserveLife");

        internal static FeatureDefinitionPower PowerDomainMischiefElusiveTarget { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefElusiveTarget");

        internal static FeatureDefinitionPower PowerDomainMischiefStrikeOfChaos { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefStrikeOfChaos");

        internal static FeatureDefinitionPower PowerDomainMischiefStrikeOfChaos11 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefStrikeOfChaos11");

        internal static FeatureDefinitionPower PowerDomainMischiefStrikeOfChaos14 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefStrikeOfChaos14");

        internal static FeatureDefinitionPower PowerDomainMischiefStrikeOfChaos5 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefStrikeOfChaos5");

        internal static FeatureDefinitionPower PowerDomainMischiefStrikeOfChaos8 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefStrikeOfChaos8");

        internal static FeatureDefinitionPower PowerDomainOblivionGateKeeper { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainOblivionGateKeeper");

        internal static FeatureDefinitionPower PowerDomainOblivionHeraldOfPain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainOblivionHeraldOfPain");

        internal static FeatureDefinitionPower PowerDomainOblivionMarkOfFate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainOblivionMarkOfFate");

        internal static FeatureDefinitionPower PowerDomainOblivionMarkOfFateImprovement { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainOblivionMarkOfFateImprovement");

        internal static FeatureDefinitionPower PowerDomainSunHeraldOfTheSun { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainSunHeraldOfTheSun");

        internal static FeatureDefinitionPower PowerDomainSunIndomitableLight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainSunIndomitableLight");

        internal static FeatureDefinitionPower PowerDomainSunSoothingHand { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainSunSoothingHand");

        internal static FeatureDefinitionPower PowerDragonbornBreathWeaponBlack { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonbornBreathWeaponBlack");

        internal static FeatureDefinitionPower PowerDragonbornBreathWeaponBlue { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonbornBreathWeaponBlue");

        internal static FeatureDefinitionPower PowerDragonbornBreathWeaponGold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonbornBreathWeaponGold");

        internal static FeatureDefinitionPower PowerDragonbornBreathWeaponGreen { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonbornBreathWeaponGreen");

        internal static FeatureDefinitionPower PowerDragonbornBreathWeaponSilver { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonbornBreathWeaponSilver");

        internal static FeatureDefinitionPower PowerDragonBreath_Acid { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Acid");

        internal static FeatureDefinitionPower PowerDragonBreath_Acid_Spectral_DLC3 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Acid_Spectral_DLC3");

        internal static FeatureDefinitionPower PowerDragonBreath_Cold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Cold");

        internal static FeatureDefinitionPower PowerDragonBreath_Fire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Fire");

        internal static FeatureDefinitionPower PowerDragonBreath_Poison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Poison");

        internal static FeatureDefinitionPower PowerDragonBreath_YoungBlack_Acid { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_YoungBlack_Acid");

        internal static FeatureDefinitionPower PowerDragonBreath_YoungGold_Fire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_YoungGold_Fire");

        internal static FeatureDefinitionPower PowerDragonBreath_YoungGreen_Poison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_YoungGreen_Poison");

        internal static FeatureDefinitionPower PowerDragonFrightfulPresence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonFrightfulPresence");

        internal static FeatureDefinitionPower PowerDragonFrightfulPresence_Spectral_DLC3 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonFrightfulPresence_Spectral_DLC3");

        internal static FeatureDefinitionPower PowerDragonWingAttack { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonWingAttack");

        internal static FeatureDefinitionPower PowerDragonWingAttack_Spectral_DLC3 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonWingAttack_Spectral_DLC3");

        internal static FeatureDefinitionPower PowerDruidCircleBalanceBalanceOfPower { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDruidCircleBalanceBalanceOfPower");

        internal static FeatureDefinitionPower PowerDruidCircleLandNaturesSanctuary { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDruidCircleLandNaturesSanctuary");

        internal static FeatureDefinitionPower PowerDruidWildShape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDruidWildShape");

        internal static FeatureDefinitionPower PowerEyebiteAsleep { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerEyebiteAsleep");

        internal static FeatureDefinitionPower PowerEyebitePanicked { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerEyebitePanicked");

        internal static FeatureDefinitionPower PowerEyebiteSickened { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerEyebiteSickened");

        internal static FeatureDefinitionPower PowerFeatBlessingOfTheElements { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFeatBlessingOfTheElements");

        internal static FeatureDefinitionPower PowerFeatCloakAndDagger { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFeatCloakAndDagger");

        internal static FeatureDefinitionPower PowerFeatDauntingPush { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFeatDauntingPush");

        internal static FeatureDefinitionPower PowerFeatDistractingGambit { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFeatDistractingGambit");

        internal static FeatureDefinitionPower PowerFeatRaiseShield { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFeatRaiseShield");

        internal static FeatureDefinitionPower PowerFeatTwinBlade { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFeatTwinBlade");

        internal static FeatureDefinitionPower PowerFiendishResilienceAcid { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceAcid");

        internal static FeatureDefinitionPower PowerFiendishResilienceBludgeoning { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceBludgeoning");

        internal static FeatureDefinitionPower PowerFiendishResilienceCold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceCold");

        internal static FeatureDefinitionPower PowerFiendishResilienceFire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceFire");

        internal static FeatureDefinitionPower PowerFiendishResilienceForce { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceForce");

        internal static FeatureDefinitionPower PowerFiendishResilienceLightning { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceLightning");

        internal static FeatureDefinitionPower PowerFiendishResilienceNecrotic { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceNecrotic");

        internal static FeatureDefinitionPower PowerFiendishResiliencePiercing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResiliencePiercing");

        internal static FeatureDefinitionPower PowerFiendishResiliencePoison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResiliencePoison");

        internal static FeatureDefinitionPower PowerFiendishResiliencePsychic { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResiliencePsychic");

        internal static FeatureDefinitionPower PowerFiendishResilienceRadiant { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceRadiant");

        internal static FeatureDefinitionPower PowerFiendishResilienceSlashing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceSlashing");

        internal static FeatureDefinitionPower PowerFiendishResilienceThunder { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFiendishResilienceThunder");

        internal static FeatureDefinitionPower PowerFighterActionSurge { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterActionSurge");

        internal static FeatureDefinitionPower PowerFighterSecondWind { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind");

        internal static FeatureDefinitionPower PowerFighterSecondWind_DLC1_5 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_DLC1_5");

        internal static FeatureDefinitionPower PowerFighterSecondWind_DLC1_9 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_DLC1_9");

        internal static FeatureDefinitionPower PowerFighterSecondWind_DLC1_Ellaria { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_DLC1_Ellaria");

        internal static FeatureDefinitionPower PowerFighterSecondWind_DLC1_NPC1 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_DLC1_NPC1");

        internal static FeatureDefinitionPower PowerFighterSecondWind_DLC3_Ashdown { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_DLC3_Ashdown");

        internal static FeatureDefinitionPower PowerFighterSecondWind_Dominion_Soldier { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_Dominion_Soldier");

        internal static FeatureDefinitionPower PowerFighterSecondWind_Robar { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_Robar");

        internal static FeatureDefinitionPower PowerFighterSecondWind_Veteran { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_Veteran");

        internal static FeatureDefinitionPower PowerFighterSecondWind_Warlord { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind_Warlord");

        internal static FeatureDefinitionPower PowerFireOspreyBlast { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFireOspreyBlast");

        internal static FeatureDefinitionPower PowerFireShieldColdRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFireShieldColdRetaliate");

        internal static FeatureDefinitionPower PowerFireShieldWarmRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFireShieldWarmRetaliate");

        internal static FeatureDefinitionPower PowerFluteOfRespiteSoothingNotes { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFluteOfRespiteSoothingNotes");

        internal static FeatureDefinitionPower PowerFunctionAlchemistFire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionAlchemistFire");

        internal static FeatureDefinitionPower PowerFunctionAntitoxin { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionAntitoxin");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_DawnBreak_ARM { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_DawnBreak_ARM");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_DawnBreak_WPN { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_DawnBreak_WPN");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_FiendSlaying_ARM { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_FiendSlaying_ARM");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_FiendSlaying_WPN { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_FiendSlaying_WPN");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_NightHunt_ARM { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_NightHunt_ARM");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_NightHunt_WPN { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_NightHunt_WPN");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_PseudoLife_ARM { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_PseudoLife_ARM");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_PseudoLife_WPN { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_PseudoLife_WPN");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_SpellTaint_ARM { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_SpellTaint_ARM");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_SpellTaint_WPN { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_SpellTaint_WPN");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_TrueStrike_ARM { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_TrueStrike_ARM");

        internal static FeatureDefinitionPower PowerFunctionApplyDemonGrease_TrueStrike_WPN { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyDemonGrease_TrueStrike_WPN");

        internal static FeatureDefinitionPower PowerFunctionApplyOilOfSharpness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyOilOfSharpness");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_ArivadsKiss { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_ArivadsKiss");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_ArunsLight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_ArunsLight");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_Basic { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_Basic");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_BrimstoneFang { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_BrimstoneFang");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_DarkStab { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_DarkStab");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_DeepPain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_DeepPain");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_GhoulsCaress { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_GhoulsCaress");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_MaraikesTorpor { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_MaraikesTorpor");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_SpiderQueensBlood { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_SpiderQueensBlood");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_TheBurden { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_TheBurden");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_TheLongNight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_TheLongNight");

        internal static FeatureDefinitionPower PowerFunctionApplyPoison_TigerFang { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionApplyPoison_TigerFang");

        internal static FeatureDefinitionPower PowerFunctionBeltOfRegeneration { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionBeltOfRegeneration");

        internal static FeatureDefinitionPower PowerFunctionBootsWinged { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionBootsWinged");

        internal static FeatureDefinitionPower PowerFunctionDustDisappearance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionDustDisappearance");

        internal static FeatureDefinitionPower PowerFunctionEndlessQuiver { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionEndlessQuiver");

        internal static FeatureDefinitionPower PowerFunctionGemOfSeeing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionGemOfSeeing");

        internal static FeatureDefinitionPower PowerFunctionGoodberryHealing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionGoodberryHealing");

        internal static FeatureDefinitionPower PowerFunctionGoodberryHealingOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionGoodberryHealingOther");

        internal static FeatureDefinitionPower PowerFunctionLavaBlast { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionLavaBlast");

        internal static FeatureDefinitionPower PowerFunctionManualBodilyWealth { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionManualBodilyWealth");

        internal static FeatureDefinitionPower PowerFunctionManualGainfulExercise { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionManualGainfulExercise");

        internal static FeatureDefinitionPower PowerFunctionManualQuicknessOfAction { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionManualQuicknessOfAction");

        internal static FeatureDefinitionPower PowerFunctionPotionOfClimbing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfClimbing");

        internal static FeatureDefinitionPower PowerFunctionPotionOfComprehendLanguages { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfComprehendLanguages");

        internal static FeatureDefinitionPower PowerFunctionPotionOfFly { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfFly");

        internal static FeatureDefinitionPower PowerFunctionPotionOfGiantStrengthCloud { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfGiantStrengthCloud");

        internal static FeatureDefinitionPower PowerFunctionPotionOfGiantStrengthFire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfGiantStrengthFire");

        internal static FeatureDefinitionPower PowerFunctionPotionOfGiantStrengthFrost { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfGiantStrengthFrost");

        internal static FeatureDefinitionPower PowerFunctionPotionOfGiantStrengthHill { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfGiantStrengthHill");

        internal static FeatureDefinitionPower PowerFunctionPotionOfGreaterHealing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfGreaterHealing");

        internal static FeatureDefinitionPower PowerFunctionPotionOfGreaterHealingOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfGreaterHealingOther");

        internal static FeatureDefinitionPower PowerFunctionPotionOfHealing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfHealing");

        internal static FeatureDefinitionPower PowerFunctionPotionOfHealingOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfHealingOther");

        internal static FeatureDefinitionPower PowerFunctionPotionOfHeroism { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfHeroism");

        internal static FeatureDefinitionPower PowerFunctionPotionOfInvisibility { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfInvisibility");

        internal static FeatureDefinitionPower PowerFunctionPotionOfSpeed { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfSpeed");

        internal static FeatureDefinitionPower PowerFunctionPotionOfSuperiorHealing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfSuperiorHealing");

        internal static FeatureDefinitionPower PowerFunctionPotionOfSuperiorHealingOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionOfSuperiorHealingOther");

        internal static FeatureDefinitionPower PowerFunctionPotionRemedy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionPotionRemedy");

        internal static FeatureDefinitionPower PowerFunctionRemedyOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionRemedyOther");

        internal static FeatureDefinitionPower PowerFunctionRestorativeOintment { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionRestorativeOintment");

        internal static FeatureDefinitionPower PowerFunctionRestorativeOintmentOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionRestorativeOintmentOther");

        internal static FeatureDefinitionPower PowerFunctionTestDisease { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionTestDisease");

        internal static FeatureDefinitionPower PowerFunctionTomeOfLeadershipAndInfluence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionTomeOfLeadershipAndInfluence");

        internal static FeatureDefinitionPower PowerFunctionTomeOfQuickThought { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionTomeOfQuickThought");

        internal static FeatureDefinitionPower PowerFunctionTomeOfUnderstanding { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionTomeOfUnderstanding");

        internal static FeatureDefinitionPower PowerFunctionWandFearCommand { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionWandFearCommand");

        internal static FeatureDefinitionPower PowerFunctionWandFearCone { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionWandFearCone");

        internal static FeatureDefinitionPower PowerGhast { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerGhast");

        internal static FeatureDefinitionPower PowerGlabrezuGeneralShadowEscape_at_will { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerGlabrezuGeneralShadowEscape_at_will");

        internal static FeatureDefinitionPower PowerGreen_Hag_Invisibility { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerGreen_Hag_Invisibility");

        internal static FeatureDefinitionPower PowerGrimBladeShadowEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerGrimBladeShadowEscape");

        internal static FeatureDefinitionPower PowerHezrouEatFriends { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerHezrouEatFriends");

        internal static FeatureDefinitionPower PowerHezrouPoisonBolt { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerHezrouPoisonBolt");

        internal static FeatureDefinitionPower PowerHezrouStench { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerHezrouStench");

        internal static FeatureDefinitionPower PowerHezrouStench_Small { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerHezrouStench_Small");

        internal static FeatureDefinitionPower PowerHolyAuraRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerHolyAuraRetaliate");

        internal static FeatureDefinitionPower PowerIceElementalBlizzard { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerIceElementalBlizzard");

        internal static FeatureDefinitionPower PowerIceGolemBreath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerIceGolemBreath");

        internal static FeatureDefinitionPower PowerIncubus_Drain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerIncubus_Drain");

        internal static FeatureDefinitionPower PowerIncubusDemonicInfluence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerIncubusDemonicInfluence");

        internal static FeatureDefinitionPower PowerInvocationOneWithShadowsTurnInvisivle { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerInvocationOneWithShadowsTurnInvisivle");

        internal static FeatureDefinitionPower PowerInvocationRepellingBlast { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerInvocationRepellingBlast");

        internal static FeatureDefinitionPower PowerIronGolemBreath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerIronGolemBreath");

        internal static FeatureDefinitionPower PowerKindredSpiritApe { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritApe");

        internal static FeatureDefinitionPower PowerKindredSpiritBear { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritBear");

        internal static FeatureDefinitionPower PowerKindredSpiritEagle { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritEagle");

        internal static FeatureDefinitionPower PowerKindredSpiritRage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritRage");

        internal static FeatureDefinitionPower PowerKindredSpiritRally { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritRally");

        internal static FeatureDefinitionPower PowerKindredSpiritRallyTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritRallyTeleport");

        internal static FeatureDefinitionPower PowerKindredSpiritSpider { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritSpider");

        internal static FeatureDefinitionPower PowerKindredSpiritTiger { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritTiger");

        internal static FeatureDefinitionPower PowerKindredSpiritViper { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritViper");

        internal static FeatureDefinitionPower PowerKindredSpiritWolf { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritWolf");

        internal static FeatureDefinitionPower PowerKutkartalIncreasedReliance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKutkartalIncreasedReliance");

        internal static FeatureDefinitionPower PowerLaetharMistyFormEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerLaetharMistyFormEscape");

        internal static FeatureDefinitionPower PowerLaetharParalyzingGaze { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerLaetharParalyzingGaze");

        internal static FeatureDefinitionPower PowerMagebaneSpellCrusher { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMagebaneSpellCrusher");

        internal static FeatureDefinitionPower PowerMagebaneWarcry { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMagebaneWarcry");

        internal static FeatureDefinitionPower PowerMarilithParry { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMarilithParry");

        internal static FeatureDefinitionPower PowerMarilithTail { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMarilithTail");

        internal static FeatureDefinitionPower PowerMarksmanRecycler { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMarksmanRecycler");

        internal static FeatureDefinitionPower PowerMartialCommanderCoordinatedDefense { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialCommanderCoordinatedDefense");

        internal static FeatureDefinitionPower PowerMartialCommanderInvigoratingShout { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialCommanderInvigoratingShout");

        internal static FeatureDefinitionPower PowerMartialCommanderLeadByExample { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialCommanderLeadByExample");

        internal static FeatureDefinitionPower PowerMartialCommanderRousingShout { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialCommanderRousingShout");

        internal static FeatureDefinitionPower PowerMartialSpellbladeArcaneEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialSpellbladeArcaneEscape");

        internal static FeatureDefinitionPower PowerMelekTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMelekTeleport");

        internal static FeatureDefinitionPower PowerMonkFlurryOfBlows { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkFlurryOfBlows");

        internal static FeatureDefinitionPower PowerMonkItemPenitentBelt { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkItemPenitentBelt");

        internal static FeatureDefinitionPower PowerMonkMartialArts { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkMartialArts");

        internal static FeatureDefinitionPower PowerMonkPatientDefense { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkPatientDefense");

        internal static FeatureDefinitionPower PowerMonkPatientDefenseSurvival3 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkPatientDefenseSurvival3");

        internal static FeatureDefinitionPower PowerMonkPatientDefenseSurvival6 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkPatientDefenseSurvival6");

        internal static FeatureDefinitionPower PowerMonkReturnMissile { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkReturnMissile");

        internal static FeatureDefinitionPower PowerMonkStepOfTheWindDash { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkStepOfTheWindDash");

        internal static FeatureDefinitionPower PowerMonkStepOftheWindDisengage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkStepOftheWindDisengage");

        internal static FeatureDefinitionPower PowerMonkStunningStrike { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkStunningStrike");

        internal static FeatureDefinitionPower PowerMountaineerCloseQuarters { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMountaineerCloseQuarters");

        internal static FeatureDefinitionPower PowerMutantApeSlam { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMutantApeSlam");

        internal static FeatureDefinitionPower PowerMutantBuletteLeap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMutantBuletteLeap");

        internal static FeatureDefinitionPower PowerOathOfDevotionAuraDevotion { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfDevotionAuraDevotion");

        internal static FeatureDefinitionPower PowerOathOfDevotionPurityOfSpirit { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfDevotionPurityOfSpirit");

        internal static FeatureDefinitionPower PowerOathOfDevotionSacredWeapon { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfDevotionSacredWeapon");

        internal static FeatureDefinitionPower PowerOathOfDevotionTurnUnholy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfDevotionTurnUnholy");

        internal static FeatureDefinitionPower PowerOathOfJugementAuraRightenousness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementAuraRightenousness");

        internal static FeatureDefinitionPower PowerOathOfJugementPurgeCorruption { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementPurgeCorruption");

        internal static FeatureDefinitionPower PowerOathOfJugementRetribution { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementRetribution");

        internal static FeatureDefinitionPower PowerOathOfJugementWeightOfJustice { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementWeightOfJustice");

        internal static FeatureDefinitionPower PowerOathOfMotherlandFieryPresence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfMotherlandFieryPresence");

        internal static FeatureDefinitionPower PowerOathOfMotherlandFieryWrath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfMotherlandFieryWrath");

        internal static FeatureDefinitionPower PowerOathOfMotherlandVolcanicAura { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfMotherlandVolcanicAura");

        internal static FeatureDefinitionPower PowerOathOfTirmarAuraTruth { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfTirmarAuraTruth");

        internal static FeatureDefinitionPower PowerOathOfTirmarGoldenSpeech { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfTirmarGoldenSpeech");

        internal static FeatureDefinitionPower PowerOathOfTirmarSmiteTheHidden { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfTirmarSmiteTheHidden");

        internal static FeatureDefinitionPower PowerOathOfTirmarSoraksBane { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfTirmarSoraksBane");

        internal static FeatureDefinitionPower PowerPactChainImp { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPactChainImp");

        internal static FeatureDefinitionPower PowerPactChainPseudodragon { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPactChainPseudodragon");

        internal static FeatureDefinitionPower PowerPactChainQuasit { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPactChainQuasit");

        internal static FeatureDefinitionPower PowerPactChainSprite { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPactChainSprite");

        internal static FeatureDefinitionPower PowerPaladinAuraOfCourage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinAuraOfCourage");

        internal static FeatureDefinitionPower PowerPaladinAuraOfProtection { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinAuraOfProtection");

        internal static FeatureDefinitionPower PowerPaladinCleansingTouch { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinCleansingTouch");

        internal static FeatureDefinitionPower PowerPaladinCureDisease { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinCureDisease");

        internal static FeatureDefinitionPower PowerPaladinDivineSense { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinDivineSense");

        internal static FeatureDefinitionPower PowerPaladinHyeronimus { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinHyeronimus");

        internal static FeatureDefinitionPower PowerPaladinLayOnHands { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinLayOnHands");

        internal static FeatureDefinitionPower PowerPaladinNeutralizePoison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinNeutralizePoison");

        internal static FeatureDefinitionPower PowerPathClawDraconicWrath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPathClawDraconicWrath");

        internal static FeatureDefinitionPower PowerPatronFiendDarkOnesBlessing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronFiendDarkOnesBlessing");

        internal static FeatureDefinitionPower PowerPatronFiendDarkOnesOwnLuck { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronFiendDarkOnesOwnLuck");

        internal static FeatureDefinitionPower PowerPatronFiendHurlThroughHell { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronFiendHurlThroughHell");

        internal static FeatureDefinitionPower PowerPatronHiveMagicCounter { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronHiveMagicCounter");

        internal static FeatureDefinitionPower PowerPatronHiveReactiveCarapace { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronHiveReactiveCarapace");

        internal static FeatureDefinitionPower PowerPatronTimekeeperAccelerate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTimekeeperAccelerate");

        internal static FeatureDefinitionPower PowerPatronTimekeeperTimeShift { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTimekeeperTimeShift");

        internal static FeatureDefinitionPower PowerPatronTimekeeperTimeWarp { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTimekeeperTimeWarp");

        internal static FeatureDefinitionPower PowerPatronTreeExplosiveGrowth { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTreeExplosiveGrowth");

        internal static FeatureDefinitionPower PowerPatronTreePiercingBranch { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTreePiercingBranch");

        internal static FeatureDefinitionPower PowerPatronTreePiercingBranchOneWithTheTree { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTreePiercingBranchOneWithTheTree");

        internal static FeatureDefinitionPower PowerPeaksAbominationLeap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPeaksAbominationLeap");

        internal static FeatureDefinitionPower PowerPeaksAbominationrBreath_Cold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPeaksAbominationrBreath_Cold");

        internal static FeatureDefinitionPower PowerPeaksTerrorBreath_Cold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPeaksTerrorBreath_Cold");

        internal static FeatureDefinitionPower PowerPhaseIncubusTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPhaseIncubusTeleport");

        internal static FeatureDefinitionPower PowerPhaseMarilithTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPhaseMarilithTeleport");

        internal static FeatureDefinitionPower PowerPhaseSpiderTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPhaseSpiderTeleport");

        internal static FeatureDefinitionPower PowerRangerHideInPlainSight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerHideInPlainSight");

        internal static FeatureDefinitionPower PowerRangerHunterStandAgainstTheTide { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerHunterStandAgainstTheTide");

        internal static FeatureDefinitionPower PowerRangerPrimevalAwareness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerPrimevalAwareness");

        internal static FeatureDefinitionPower PowerRangerSwiftBladeBattleFocus { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerSwiftBladeBattleFocus");

        internal static FeatureDefinitionPower PowerRangerSwiftBladeUncatchable { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerSwiftBladeUncatchable");

        internal static FeatureDefinitionPower PowerReckless { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerReckless");

        internal static FeatureDefinitionPower PowerRemorhazRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRemorhazRetaliate");

        internal static FeatureDefinitionPower PowerRemorhazSwallow { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRemorhazSwallow");

        internal static FeatureDefinitionPower PowerRoguishDarkweaverShadowy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRoguishDarkweaverShadowy");

        internal static FeatureDefinitionPower PowerRoguishHoodlumDirtyFighting { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRoguishHoodlumDirtyFighting");

        internal static FeatureDefinitionPower PowerRoguishHoodlumMenacing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRoguishHoodlumMenacing");

        internal static FeatureDefinitionPower PowerShadowcasterShadowDodge { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerShadowcasterShadowDodge");

        internal static FeatureDefinitionPower PowerShadowTamerRopeGrapple { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerShadowTamerRopeGrapple");

        internal static FeatureDefinitionPower PowerShamblingMoundEngulf { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerShamblingMoundEngulf");

        internal static FeatureDefinitionPower PowerSorakAssassinShadowMurder { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakAssassinShadowMurder");

        internal static FeatureDefinitionPower PowerSorakCallOfTheNight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakCallOfTheNight");

        internal static FeatureDefinitionPower PowerSorakCallOfTheNight_Shikkath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakCallOfTheNight_Shikkath");

        internal static FeatureDefinitionPower PowerSorakDoomLaughter { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakDoomLaughter");

        internal static FeatureDefinitionPower PowerSorakDreadLaughter { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakDreadLaughter");

        internal static FeatureDefinitionPower PowerSorakDummy_ApostleDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakDummy_ApostleDarkness");

        internal static FeatureDefinitionPower PowerSorakDummy_ChildOfDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakDummy_ChildOfDarkness");

        internal static FeatureDefinitionPower PowerSorakDummy_ProphetDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakDummy_ProphetDarkness");

        internal static FeatureDefinitionPower PowerSorakSaboteurTranceOfSorrtarr { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakSaboteurTranceOfSorrtarr");

        internal static FeatureDefinitionPower PowerSorakShadowEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakShadowEscape");

        internal static FeatureDefinitionPower PowerSorakShadowEscape_at_will { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakShadowEscape_at_will");

        internal static FeatureDefinitionPower PowerSorakShriek { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakShriek");

        internal static FeatureDefinitionPower PowerSorakWordOfDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakWordOfDarkness");

        internal static FeatureDefinitionPower PowerSorcererChildRiftDeflection { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftDeflection");

        internal static FeatureDefinitionPower PowerSorcererChildRiftOffering { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftOffering");

        internal static FeatureDefinitionPower PowerSorcererChildRiftRiftwalk { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftRiftwalk");

        internal static FeatureDefinitionPower PowerSorcererChildRiftRiftwalkLandingDamage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftRiftwalkLandingDamage");

        internal static FeatureDefinitionPower PowerSorcererCreateSorceryPoints { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererCreateSorceryPoints");

        internal static FeatureDefinitionPower PowerSorcererCreateSpellSlot { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererCreateSpellSlot");

        internal static FeatureDefinitionPower PowerSorcererDraconicDragonWingsDismiss { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererDraconicDragonWingsDismiss");

        internal static FeatureDefinitionPower PowerSorcererDraconicDragonWingsSprout { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererDraconicDragonWingsSprout");

        internal static FeatureDefinitionPower PowerSorcererDraconicElementalResistance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererDraconicElementalResistance");

        internal static FeatureDefinitionPower PowerSorcererHauntedSoulRechargeVengefulSpirits { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererHauntedSoulRechargeVengefulSpirits");

        internal static FeatureDefinitionPower PowerSorcererHauntedSoulSoulDrain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererHauntedSoulSoulDrain");

        internal static FeatureDefinitionPower PowerSorcererHauntedSoulSpiritVisage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererHauntedSoulSpiritVisage");

        internal static FeatureDefinitionPower PowerSorcererHauntedSoulVengefulSpirits { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererHauntedSoulVengefulSpirits");

        internal static FeatureDefinitionPower PowerSorcererManaPainterDrain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererManaPainterDrain");

        internal static FeatureDefinitionPower PowerSorcererManaPainterTap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererManaPainterTap");

        internal static FeatureDefinitionPower PowerSpecialEmtanHolySymbol { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSpecialEmtanHolySymbol");

        internal static FeatureDefinitionPower PowerSpellBladeSpellTyrant { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSpellBladeSpellTyrant");

        internal static FeatureDefinitionPower PowerSpiderQueenEatFriends { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSpiderQueenEatFriends");

        internal static FeatureDefinitionPower PowerSpiderQueenPoisonCloud { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSpiderQueenPoisonCloud");

        internal static FeatureDefinitionPower PowerStaffOfMetis { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerStaffOfMetis");

        internal static FeatureDefinitionPower PowerStoneGolemSlow { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerStoneGolemSlow");

        internal static FeatureDefinitionPower PowerStoneResilience { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerStoneResilience");

        internal static FeatureDefinitionPower PowerStormGiantLightningBolt { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerStormGiantLightningBolt");

        internal static FeatureDefinitionPower PowerSunbeam { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSunbeam");

        internal static FeatureDefinitionPower PowerSymbolOfDeath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfDeath");

        internal static FeatureDefinitionPower PowerSymbolOfFear { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfFear");

        internal static FeatureDefinitionPower PowerSymbolOfHopelessness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfHopelessness");

        internal static FeatureDefinitionPower PowerSymbolOfInsanity { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfInsanity");

        internal static FeatureDefinitionPower PowerSymbolOfPain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfPain");

        internal static FeatureDefinitionPower PowerSymbolOfSleep { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfSleep");

        internal static FeatureDefinitionPower PowerSymbolOfStun { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfStun");

        internal static FeatureDefinitionPower PowerTraditionCourtMageExpandedSpellShield { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionCourtMageExpandedSpellShield");

        internal static FeatureDefinitionPower PowerTraditionCourtMageImprovedSpellShield { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionCourtMageImprovedSpellShield");

        internal static FeatureDefinitionPower PowerTraditionCourtMageSpellShield { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionCourtMageSpellShield");

        internal static FeatureDefinitionPower PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement");

        internal static FeatureDefinitionPower PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement");

        internal static FeatureDefinitionPower PowerTraditionFreedomSwirlingDance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionFreedomSwirlingDance");

        internal static FeatureDefinitionPower PowerTraditionGreenmageMagicalArrow { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionGreenmageMagicalArrow");

        internal static FeatureDefinitionPower PowerTraditionGreenmageWeakeningEntanglingShot { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionGreenmageWeakeningEntanglingShot");

        internal static FeatureDefinitionPower PowerTraditionLightBlindingFlash { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionLightBlindingFlash");

        internal static FeatureDefinitionPower PowerTraditionLightLuminousKi { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionLightLuminousKi");

        internal static FeatureDefinitionPower PowerTraditionOpenHandOpenHandTechniqueDazzle { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandOpenHandTechniqueDazzle");

        internal static FeatureDefinitionPower PowerTraditionOpenHandOpenHandTechniqueKnockProne { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandOpenHandTechniqueKnockProne");

        internal static FeatureDefinitionPower PowerTraditionOpenHandOpenHandTechniquePushAway { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandOpenHandTechniquePushAway");

        internal static FeatureDefinitionPower PowerTraditionOpenHandTranquility { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandTranquility");

        internal static FeatureDefinitionPower PowerTraditionOpenHandWholenessOfBody { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandWholenessOfBody");

        internal static FeatureDefinitionPower PowerTraditionShockArcanistArcaneFury { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionShockArcanistArcaneFury");

        internal static FeatureDefinitionPower PowerTraditionShockArcanistArcaneShock { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionShockArcanistArcaneShock");

        internal static FeatureDefinitionPower PowerTraditionShockArcanistGreaterArcaneShock { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionShockArcanistGreaterArcaneShock");

        internal static FeatureDefinitionPower PowerTraditionSurvivalUnbreakableBody { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionSurvivalUnbreakableBody");

        internal static FeatureDefinitionPower PowerTsharNecroticBlast { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTsharNecroticBlast");

        internal static FeatureDefinitionPower PowerVampiricTouch { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerVampiricTouch");

        internal static FeatureDefinitionPower PowerVrockScreech { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerVrockScreech");

        internal static FeatureDefinitionPower PowerVrockSpores { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerVrockSpores");

        internal static FeatureDefinitionPower PowerWight_DrainLife { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWight_DrainLife");

        internal static FeatureDefinitionPower PowerWightLord_CircleOfDeath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWightLord_CircleOfDeath");

        internal static FeatureDefinitionPower PowerWightLordRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWightLordRetaliate");

        internal static FeatureDefinitionPower PowerWindCarriedByWind { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWindCarriedByWind");

        internal static FeatureDefinitionPower PowerWindGuidingWinds { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWindGuidingWinds");

        internal static FeatureDefinitionPower PowerWindShelteringBreeze { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWindShelteringBreeze");

        internal static FeatureDefinitionPower PowerWinterWolfBreath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWinterWolfBreath");

        internal static FeatureDefinitionPower PowerWizardArcaneRecovery { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWizardArcaneRecovery");

        internal static FeatureDefinitionPower PowerYoungRemorhazRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerYoungRemorhazRetaliate");
    }

    internal static class FeatureDefinitionProficiencys
    {
        internal static FeatureDefinitionProficiency ProficienctSpySkillsTool { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficienctSpySkillsTool");

        internal static FeatureDefinitionProficiency ProficiencyAcademicSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAcademicSkills");

        internal static FeatureDefinitionProficiency ProficiencyAcademicSkillsTool { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAcademicSkillsTool");

        internal static FeatureDefinitionProficiency ProficiencyAcolyteSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAcolyteSkills");

        internal static FeatureDefinitionProficiency ProficiencyAcolyteToolsSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAcolyteToolsSkills");

        internal static FeatureDefinitionProficiency ProficiencyAesceticSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAesceticSkills");

        internal static FeatureDefinitionProficiency ProficiencyAesceticToolsSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAesceticToolsSkills");

        internal static FeatureDefinitionProficiency ProficiencyAllLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAllLanguages");

        internal static FeatureDefinitionProficiency ProficiencyAllLanguagesButCode { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAllLanguagesButCode");

        internal static FeatureDefinitionProficiency ProficiencyAmbassador { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAmbassador");

        internal static FeatureDefinitionProficiency ProficiencyAristocratSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAristocratSkills");

        internal static FeatureDefinitionProficiency ProficiencyArmor_Of_The_Forest { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyArmor_Of_The_Forest");

        internal static FeatureDefinitionProficiency ProficiencyArmor_Of_The_Forest2 { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyArmor_Of_The_Forest2");

        internal static FeatureDefinitionProficiency ProficiencyArmor_Of_The_Oak { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyArmor_Of_The_Oak");

        internal static FeatureDefinitionProficiency ProficiencyArtistSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyArtistSkills");

        internal static FeatureDefinitionProficiency ProficiencyBarbarianArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBarbarianArmor");

        internal static FeatureDefinitionProficiency ProficiencyBarbarianSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBarbarianSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyBarbarianWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBarbarianWeapon");

        internal static FeatureDefinitionProficiency ProficiencyBardArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBardArmor");

        internal static FeatureDefinitionProficiency ProficiencyBardSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBardSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyBardTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBardTools");

        internal static FeatureDefinitionProficiency ProficiencyBardWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBardWeapon");

        internal static FeatureDefinitionProficiency ProficiencyBeguilingInfluence { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBeguilingInfluence");

        internal static FeatureDefinitionProficiency ProficiencyBeltOfDwarvenKind { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBeltOfDwarvenKind");

        internal static FeatureDefinitionProficiency ProficiencyBracersOfArchery { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBracersOfArchery");

        internal static FeatureDefinitionProficiency ProficiencyClericArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyClericArmor");

        internal static FeatureDefinitionProficiency ProficiencyClericSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyClericSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyClericTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyClericTools");

        internal static FeatureDefinitionProficiency ProficiencyClericWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyClericWeapon");

        internal static FeatureDefinitionProficiency ProficiencyCommanderSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyCommanderSkills");

        internal static FeatureDefinitionProficiency ProficiencyDiscretionOfTheCoedymwarth { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDiscretionOfTheCoedymwarth");

        internal static FeatureDefinitionProficiency ProficiencyDiscretionOfTheCoedymwarthArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDiscretionOfTheCoedymwarthArmor");

        internal static FeatureDefinitionProficiency ProficiencyDomainBattleWepaon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDomainBattleWepaon");

        internal static FeatureDefinitionProficiency ProficiencyDomainInsightDivineLore { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDomainInsightDivineLore");

        internal static FeatureDefinitionProficiency ProficiencyDomainLawCommandingPresenceIntimidation { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDomainLawCommandingPresenceIntimidation");

        internal static FeatureDefinitionProficiency ProficiencyDomainLifeArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDomainLifeArmor");

        internal static FeatureDefinitionProficiency ProficiencyDomainMischiefTrickster { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDomainMischiefTrickster");

        internal static FeatureDefinitionProficiency ProficiencyDragonbornLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDragonbornLanguages");

        internal static FeatureDefinitionProficiency ProficiencyDruidArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidArmor");

        internal static FeatureDefinitionProficiency ProficiencyDruidLanguage { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidLanguage");

        internal static FeatureDefinitionProficiency ProficiencyDruidSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyDruidWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidWeapon");

        internal static FeatureDefinitionProficiency ProficiencyDwarfLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDwarfLanguages");

        internal static FeatureDefinitionProficiency ProficiencyDwarfSnowWeaponTraining { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDwarfSnowWeaponTraining");

        internal static FeatureDefinitionProficiency ProficiencyDwarfWeaponTraining { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDwarfWeaponTraining");

        internal static FeatureDefinitionProficiency ProficiencyElfKeenSenses { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyElfKeenSenses");

        internal static FeatureDefinitionProficiency ProficiencyElfStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyElfStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyElfSylvanStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyElfSylvanStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyElfSylvanSurvival { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyElfSylvanSurvival");

        internal static FeatureDefinitionProficiency ProficiencyElfWeaponTraining { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyElfWeaponTraining");

        internal static FeatureDefinitionProficiency ProficiencyFeatInitiateAlchemistSkill { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatInitiateAlchemistSkill");

        internal static FeatureDefinitionProficiency ProficiencyFeatInitiateAlchemistTool1 { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatInitiateAlchemistTool1");

        internal static FeatureDefinitionProficiency ProficiencyFeatInitiateAlchemistTool2 { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatInitiateAlchemistTool2");

        internal static FeatureDefinitionProficiency ProficiencyFeatInitiateEnchanterSkill { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatInitiateEnchanterSkill");

        internal static FeatureDefinitionProficiency ProficiencyFeatInitiateEnchanterTool { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatInitiateEnchanterTool");

        internal static FeatureDefinitionProficiency ProficiencyFeatLockbreaker { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatLockbreaker");

        internal static FeatureDefinitionProficiency ProficiencyFeatManipulatorSkillOrExpertise { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatManipulatorSkillOrExpertise");

        internal static FeatureDefinitionProficiency ProficiencyFighterArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFighterArmor");

        internal static FeatureDefinitionProficiency ProficiencyFighterSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFighterSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyFighterWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFighterWeapon");

        internal static FeatureDefinitionProficiency ProficiencyGnomeRockArtificersLore { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyGnomeRockArtificersLore");

        internal static FeatureDefinitionProficiency ProficiencyGnomeShadowStealthy { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyGnomeShadowStealthy");

        internal static FeatureDefinitionProficiency ProficiencyGreenmageWardenOfTheForestArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyGreenmageWardenOfTheForestArmor");

        internal static FeatureDefinitionProficiency ProficiencyGreenmageWardenOfTheForestBow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyGreenmageWardenOfTheForestBow");

        internal static FeatureDefinitionProficiency ProficiencyGreenmageWardenOfTheForestStyle { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyGreenmageWardenOfTheForestStyle");

        internal static FeatureDefinitionProficiency ProficiencyHalfElfStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyHalfElfStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyHalflingStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyHalflingStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyHalfOrcMenacing { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyHalfOrcMenacing");

        internal static FeatureDefinitionProficiency ProficiencyHalfOrcStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyHalfOrcStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyHumanStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyHumanStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyKindredSpiritApe { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyKindredSpiritApe");

        internal static FeatureDefinitionProficiency ProficiencyLawkeeperSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLawkeeperSkills");

        internal static FeatureDefinitionProficiency ProficiencyLawkeeperSkillsWeapons { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLawkeeperSkillsWeapons");

        internal static FeatureDefinitionProficiency ProficiencyLowlifeSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLowlifeSkills");

        internal static FeatureDefinitionProficiency ProficiencyLowLifeSkillsTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLowLifeSkillsTools");

        internal static FeatureDefinitionProficiency ProficiencyMightOfTheIronLegion { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMightOfTheIronLegion");

        internal static FeatureDefinitionProficiency ProficiencyMightOfTheIronLegionArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMightOfTheIronLegionArmor");

        internal static FeatureDefinitionProficiency ProficiencyMonkArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkArmor");

        internal static FeatureDefinitionProficiency ProficiencyMonkDiamondSoulSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkDiamondSoulSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyMonkSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyMonkTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkTools");

        internal static FeatureDefinitionProficiency ProficiencyMonkWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkWeapon");

        internal static FeatureDefinitionProficiency ProficiencyOathOfTirmarBonusLanguage { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyOathOfTirmarBonusLanguage");

        internal static FeatureDefinitionProficiency ProficiencyOccultistSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyOccultistSkills");

        internal static FeatureDefinitionProficiency ProficiencyOccultistToolsSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyOccultistToolsSkills");

        internal static FeatureDefinitionProficiency ProficiencyPactBladeMartialWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPactBladeMartialWeapon");

        internal static FeatureDefinitionProficiency ProficiencyPaladinArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPaladinArmor");

        internal static FeatureDefinitionProficiency ProficiencyPaladinSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPaladinSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyPaladinWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPaladinWeapon");

        internal static FeatureDefinitionProficiency ProficiencyPeriaptOfTheMasterEnchanterSkillsTool { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPeriaptOfTheMasterEnchanterSkillsTool");

        internal static FeatureDefinitionProficiency ProficiencyPhilosopherSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPhilosopherSkills");

        internal static FeatureDefinitionProficiency ProficiencyPhilosopherTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPhilosopherTools");

        internal static FeatureDefinitionProficiency ProficiencyRaiseShield { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRaiseShield");

        internal static FeatureDefinitionProficiency ProficiencyRangerArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRangerArmor");

        internal static FeatureDefinitionProficiency ProficiencyRangerSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRangerSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyRangerWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRangerWeapon");

        internal static FeatureDefinitionProficiency ProficiencyRogueArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueArmor");

        internal static FeatureDefinitionProficiency ProficiencyRogueSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyRogueSlipperyMind { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueSlipperyMind");

        internal static FeatureDefinitionProficiency ProficiencyRogueTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueTools");

        internal static FeatureDefinitionProficiency ProficiencyRogueWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueWeapon");

        internal static FeatureDefinitionProficiency ProficiencyRoguishDarkweaver { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRoguishDarkweaver");

        internal static FeatureDefinitionProficiency ProficiencyRoguishHoodlumMeanMug { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRoguishHoodlumMeanMug");

        internal static FeatureDefinitionProficiency ProficiencyRoguishHoodlumTheRightToolsArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRoguishHoodlumTheRightToolsArmor");

        internal static FeatureDefinitionProficiency ProficiencyRoguishHoodlumTheRightToolsWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRoguishHoodlumTheRightToolsWeapon");

        internal static FeatureDefinitionProficiency ProficiencySellSwordSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySellSwordSkills");

        internal static FeatureDefinitionProficiency ProficiencySellSwordSkillsArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySellSwordSkillsArmor");

        internal static FeatureDefinitionProficiency ProficiencyShockArcanistWarcasting { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyShockArcanistWarcasting");

        internal static FeatureDefinitionProficiency ProficiencySmithTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySmithTools");

        internal static FeatureDefinitionProficiency ProficiencySorcererDraconicKnowledge { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySorcererDraconicKnowledge");

        internal static FeatureDefinitionProficiency ProficiencySorcererSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySorcererSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencySorcererTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySorcererTools");

        internal static FeatureDefinitionProficiency ProficiencySorcererWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySorcererWeapon");

        internal static FeatureDefinitionProficiency ProficiencySpyLanguage { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySpyLanguage");

        internal static FeatureDefinitionProficiency ProficiencySpySkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySpySkills");

        internal static FeatureDefinitionProficiency ProficiencySturdinessOfTheTundra { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySturdinessOfTheTundra");

        internal static FeatureDefinitionProficiency ProficiencySturdinessOfTheTundraArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySturdinessOfTheTundraArmor");

        internal static FeatureDefinitionProficiency ProficiencyTraditionCourtMageAlwaysPreparedProtection { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyTraditionCourtMageAlwaysPreparedProtection");

        internal static FeatureDefinitionProficiency ProficiencyTraditionCourtMageAlwaysPreparedShield { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyTraditionCourtMageAlwaysPreparedShield");

        internal static FeatureDefinitionProficiency ProficiencyWandererSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWandererSkills");

        internal static FeatureDefinitionProficiency ProficiencyWandererTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWandererTools");

        internal static FeatureDefinitionProficiency ProficiencyWarlockArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWarlockArmor");

        internal static FeatureDefinitionProficiency ProficiencyWarlockSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWarlockSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyWarlockWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWarlockWeapon");

        internal static FeatureDefinitionProficiency ProficiencyWizardSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWizardSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyWizardTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWizardTools");

        internal static FeatureDefinitionProficiency ProficiencyWizardWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWizardWeapon");
    }

    internal static class FeatureDefinitionRegenerations
    {
        internal static FeatureDefinitionRegeneration Regeneration_Ancient_Remorhaz { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("Regeneration_Ancient_Remorhaz");

        internal static FeatureDefinitionRegeneration Regeneration_DemonGeneral { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("Regeneration_DemonGeneral");

        internal static FeatureDefinitionRegeneration Regeneration_Sessroth { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("Regeneration_Sessroth");

        internal static FeatureDefinitionRegeneration RegenerationApostleOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationApostleOfDarkness_Darkness");

        internal static FeatureDefinitionRegeneration RegenerationApostleOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationApostleOfDarkness_DimLight");

        internal static FeatureDefinitionRegeneration RegenerationBeltOfRegeneration { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationBeltOfRegeneration");

        internal static FeatureDefinitionRegeneration RegenerationChildOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationChildOfDarkness_Darkness");

        internal static FeatureDefinitionRegeneration RegenerationChildOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationChildOfDarkness_DimLight");

        internal static FeatureDefinitionRegeneration RegenerationDefiler { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationDefiler");

        internal static FeatureDefinitionRegeneration RegenerationMutantApe { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationMutantApe");

        internal static FeatureDefinitionRegeneration RegenerationMutantBear { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationMutantBear");

        internal static FeatureDefinitionRegeneration RegenerationMutantMinotaur { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationMutantMinotaur");

        internal static FeatureDefinitionRegeneration RegenerationOni { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationOni");

        internal static FeatureDefinitionRegeneration RegenerationProphetOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationProphetOfDarkness_Darkness");

        internal static FeatureDefinitionRegeneration RegenerationProphetOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationProphetOfDarkness_DimLight");

        internal static FeatureDefinitionRegeneration RegenerationRegenerateSpell { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationRegenerateSpell");

        internal static FeatureDefinitionRegeneration RegenerationRing { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationRing");

        internal static FeatureDefinitionRegeneration RegenerationTroll { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationTroll");

        internal static FeatureDefinitionRegeneration RegenerationTroll_Mutant { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationTroll_Mutant");

        internal static FeatureDefinitionRegeneration RegenerationVampire { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationVampire");
    }

    internal static class FeatureDefinitionRestHealingModifiers
    {
        internal static FeatureDefinitionRestHealingModifier RestHealingModifierBardHealingBallad { get; } =
            GetDefinition<FeatureDefinitionRestHealingModifier>("RestHealingModifierBardHealingBallad");

        internal static FeatureDefinitionRestHealingModifier RestHealingModifierBardItemRespite { get; } =
            GetDefinition<FeatureDefinitionRestHealingModifier>("RestHealingModifierBardItemRespite");

        internal static FeatureDefinitionRestHealingModifier RestHealingModifierBardSongOfRest { get; } =
            GetDefinition<FeatureDefinitionRestHealingModifier>("RestHealingModifierBardSongOfRest");
    }

    internal static class FeatureDefinitionSavingThrowAffinitys
    {
        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinity_PalaceOfIce_LairEffect { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinity_PalaceOfIce_LairEffect");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityAdvantageToAll { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityAdvantageToAll");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityAntitoxin { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityAntitoxin");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityApostleOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityApostleOfDarkness_Darkness");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityApostleOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityApostleOfDarkness_DimLight");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityArmorOfSurvival { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityArmorOfSurvival");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityAuraOfProtection { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityAuraOfProtection");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityBarbarianDangerSense { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityBarbarianDangerSense");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityChildOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityChildOfDarkness_Darkness");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityChildOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityChildOfDarkness_DimLight");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCloakOfProtection { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCloakOfProtection");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCloakOfProtectionPlusOne { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCloakOfProtectionPlusOne");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBackFromTheDead1 { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBackFromTheDead1");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBackFromTheDead2 { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBackFromTheDead2");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBackFromTheDead3 { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBackFromTheDead3");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBackFromTheDead4 { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBackFromTheDead4");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBaned { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBaned");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBeaconOfHope { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBeaconOfHope");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBestowCurseCharisma { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBestowCurseCharisma");

        internal static FeatureDefinitionSavingThrowAffinity
            SavingThrowAffinityConditionBestowCurseConstitution { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBestowCurseConstitution");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBestowCurseDexterity { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBestowCurseDexterity");

        internal static FeatureDefinitionSavingThrowAffinity
            SavingThrowAffinityConditionBestowCurseIntelligence { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBestowCurseIntelligence");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBestowCurseStrength { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBestowCurseStrength");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBestowCurseWisdom { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBestowCurseWisdom");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBlessed { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBlessed");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBlessingSorrtarr { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBlessingSorrtarr");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBlinded { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBlinded");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionChilled { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionChilled");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionDodging { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionDodging");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionFrozen { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionFrozen");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionHasted { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionHasted");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionHeroesFeast { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionHeroesFeast");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionHolyAura { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionHolyAura");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionParalyzed { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionParalyzed");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionRaging { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionRaging");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionResisting { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionResisting");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionRestrained { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionRestrained");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionSlowed { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionSlowed");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionStunned { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionStunned");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionUnconscious { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionUnconscious");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionWardedByWardingBond { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionWardedByWardingBond");

        internal static FeatureDefinitionSavingThrowAffinity
            SavingThrowAffinityConditionWeakeningEntanglingShot { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionWeakeningEntanglingShot");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityContagionBlindingSickness { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityContagionBlindingSickness");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityContagionFilthFever { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityContagionFilthFever");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityContagionMindfire { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityContagionMindfire");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityContagionSeizure { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityContagionSeizure");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityContagionSlimyDoom { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityContagionSlimyDoom");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfArun { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfArun");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfEinar { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfEinar");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfMaraike { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfMaraike");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfMisaye { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfMisaye");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfPakri { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfPakri");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfSolasta { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfSolasta");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCrownOfTheMagister { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCrownOfTheMagister");

        internal static FeatureDefinitionSavingThrowAffinity
            SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>(
                "SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityDomainMischiefBorrowLuck { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityDomainMischiefBorrowLuck");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityDomainMischiefBorrrowedLuck { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityDomainMischiefBorrrowedLuck");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityDwarfBread { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityDwarfBread");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityDwarvenPlate { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityDwarvenPlate");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityFiendDarkOnesOwnLuck { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityFiendDarkOnesOwnLuck");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGarbOfTheLightbringer { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGarbOfTheLightbringer");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemAbjuration { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemAbjuration");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemConjuration { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemConjuration");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemDivination { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemDivination");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemEnchantment { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemEnchantment");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemEvocation { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemEvocation");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemIllusion { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemIllusion");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemNecromancy { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemNecromancy");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemTransmutation { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemTransmutation");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGnomeCunning { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGnomeCunning");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityHeraldOfBattle { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityHeraldOfBattle");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityKindredSpiritBond { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityKindredSpiritBond");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityMagebaneRejectMagic { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityMagebaneRejectMagic");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityManaPainterAbsorption { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityManaPainterAbsorption");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityMonkDiamondSoul { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityMonkDiamondSoul");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityMonkEvasion { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityMonkEvasion");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityPatronHiveAntimagicChitin { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityPatronHiveAntimagicChitin");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityPatronHiveWeakeningPheromones { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityPatronHiveWeakeningPheromones");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityProphetOfDarkness_Darkness { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityProphetOfDarkness_Darkness");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityProphetOfDarkness_DimLight { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityProphetOfDarkness_DimLight");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityRangerHunterEvasion { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityRangerHunterEvasion");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityRingOfProtectionPlusOne { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityRingOfProtectionPlusOne");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityRingOfProtectionPlusTwo { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityRingOfProtectionPlusTwo");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityRingOfTheLordInquisitor { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityRingOfTheLordInquisitor");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityRogueEvasion { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityRogueEvasion");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySensitiveToLight { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySensitiveToLight");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityShadowTamerAtHomeInTheDark { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityShadowTamerAtHomeInTheDark");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityShadowTamerTunnelWisdom { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityShadowTamerTunnelWisdom");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityShapechangerMoonbeam { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityShapechangerMoonbeam");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityShelteringBreeze { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityShelteringBreeze");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySigilRingAbjurationPlusOne { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySigilRingAbjurationPlusOne");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySnowDwarfEndurance { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySnowDwarfEndurance");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySorcererChildRiftDeflection { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySorcererChildRiftDeflection");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySpellResistance { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySpellResistance");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySpellVulnerability { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySpellVulnerability");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityStoneOfGoodLuck { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityStoneOfGoodLuck");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityStoneStrengthWithin { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityStoneStrengthWithin");
    }

    internal static class FeatureDefinitionSchoolSavants
    {
        internal static FeatureDefinitionSchoolSavant SchoolSavantEvocation { get; } =
            GetDefinition<FeatureDefinitionSchoolSavant>("SchoolSavantEvocation");
    }

    internal static class FeatureDefinitionSenses
    {
        internal static FeatureDefinitionSense SenseBlindSight12 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseBlindSight12");

        internal static FeatureDefinitionSense SenseBlindSight16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseBlindSight16");

        internal static FeatureDefinitionSense SenseBlindSight2 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseBlindSight2");

        internal static FeatureDefinitionSense SenseBlindSight6 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseBlindSight6");

        internal static FeatureDefinitionSense SenseDarkvision { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvision");

        internal static FeatureDefinitionSense SenseDarkvision12 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvision12");

        internal static FeatureDefinitionSense SenseDarkvision24 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvision24");

        internal static FeatureDefinitionSense SenseDarkvisionFull { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvisionFull");

        internal static FeatureDefinitionSense SenseNormalVision { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseNormalVision");

        internal static FeatureDefinitionSense SenseRogueBlindsense { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseRogueBlindsense");

        internal static FeatureDefinitionSense SenseSeeInvisible12 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSeeInvisible12");

        internal static FeatureDefinitionSense SenseSeeInvisible16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSeeInvisible16");

        internal static FeatureDefinitionSense SenseSeeInvisible24 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSeeInvisible24");

        internal static FeatureDefinitionSense SenseSuperiorDarkvision { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSuperiorDarkvision");

        internal static FeatureDefinitionSense SenseTremorsense16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseTremorsense16");

        internal static FeatureDefinitionSense SenseTruesight16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseTruesight16");

        internal static FeatureDefinitionSense SenseTruesight24 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseTruesight24");
    }

    internal static class FeatureDefinitionSocialAffinitys
    {
        internal static FeatureDefinitionSocialAffinity SocialAffinityShelterOfTheFaithful { get; } =
            GetDefinition<FeatureDefinitionSocialAffinity>("SocialAffinityShelterOfTheFaithful");
    }

    internal static class FeatureDefinitionSubclassChoices
    {
        internal static FeatureDefinitionSubclassChoice SubclassChoiceBarbarianPrimalPath { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceBarbarianPrimalPath");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceBardColleges { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceBardColleges");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceClericDivineDomains { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceClericDivineDomains");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceDruidCircle { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceDruidCircle");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceFighterMartialArchetypes { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceFighterMartialArchetypes");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceMonkMonasticTraditions { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceMonkMonasticTraditions");

        internal static FeatureDefinitionSubclassChoice SubclassChoicePaladinSacredOaths { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoicePaladinSacredOaths");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceRangerArchetypes { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceRangerArchetypes");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceRogueRoguishArchetypes { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceRogueRoguishArchetypes");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceSorcerousOrigin { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceSorcerousOrigin");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceWarlockOtherworldlyPatrons { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceWarlockOtherworldlyPatrons");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceWizardArcaneTraditions { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceWizardArcaneTraditions");
    }

    internal static class FeatureDefinitionSummoningAffinitys
    {
        internal static FeatureDefinitionSummoningAffinity SummoningAffinityKindredSpiritBond { get; } =
            GetDefinition<FeatureDefinitionSummoningAffinity>("SummoningAffinityKindredSpiritBond");

        internal static FeatureDefinitionSummoningAffinity SummoningAffinityKindredSpiritMagicalSpirit { get; } =
            GetDefinition<FeatureDefinitionSummoningAffinity>("SummoningAffinityKindredSpiritMagicalSpirit");

        internal static FeatureDefinitionSummoningAffinity SummoningAffinityKindredSpiritSharedPain { get; } =
            GetDefinition<FeatureDefinitionSummoningAffinity>("SummoningAffinityKindredSpiritSharedPain");
    }

    internal static class FeatureDefinitionTerrainTypeAffinitys
    {
        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerArctic { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerArctic");

        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerCoast { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerCoast");

        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerDesert { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerDesert");

        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerForest { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerForest");

        internal static FeatureDefinitionTerrainTypeAffinity
            TerrainTypeAffinityRangerNaturalExplorerGrassland { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerGrassland");

        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerMountain { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerMountain");

        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerSwamp { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerSwamp");
    }

    internal static class InvocationDefinitions
    {
        internal static InvocationDefinition AgonizingBlast { get; } =
            GetDefinition<InvocationDefinition>("AgonizingBlast");

        internal static InvocationDefinition ArmorOfShadows { get; } =
            GetDefinition<InvocationDefinition>("ArmorOfShadows");

        internal static InvocationDefinition AscendantStep { get; } =
            GetDefinition<InvocationDefinition>("AscendantStep");

        internal static InvocationDefinition BeguilingInfluence { get; } =
            GetDefinition<InvocationDefinition>("BeguilingInfluence");

        internal static InvocationDefinition BewitchingWhispers { get; } =
            GetDefinition<InvocationDefinition>("BewitchingWhispers");

        internal static InvocationDefinition BookAncientSecrets { get; } =
            GetDefinition<InvocationDefinition>("BookAncientSecrets");

        internal static InvocationDefinition ChainsCarceri { get; } =
            GetDefinition<InvocationDefinition>("ChainsCarceri");

        internal static InvocationDefinition DevilsSight { get; } = GetDefinition<InvocationDefinition>("DevilsSight");

        internal static InvocationDefinition DreadfulWord { get; } =
            GetDefinition<InvocationDefinition>("DreadfulWord");

        internal static InvocationDefinition EldritchSight { get; } =
            GetDefinition<InvocationDefinition>("EldritchSight");

        internal static InvocationDefinition EldritchSpear { get; } =
            GetDefinition<InvocationDefinition>("EldritchSpear");

        internal static InvocationDefinition EyesRuneKeeper { get; } =
            GetDefinition<InvocationDefinition>("EyesRuneKeeper");

        internal static InvocationDefinition FiendishVigor { get; } =
            GetDefinition<InvocationDefinition>("FiendishVigor");

        internal static InvocationDefinition Lifedrinker { get; } = GetDefinition<InvocationDefinition>("Lifedrinker");

        internal static InvocationDefinition MinionsChaos { get; } =
            GetDefinition<InvocationDefinition>("MinionsChaos");

        internal static InvocationDefinition MireMind { get; } = GetDefinition<InvocationDefinition>("MireMind");

        internal static InvocationDefinition OneWithShadows { get; } =
            GetDefinition<InvocationDefinition>("OneWithShadows");

        internal static InvocationDefinition OtherworldlyLeap { get; } =
            GetDefinition<InvocationDefinition>("OtherworldlyLeap");

        internal static InvocationDefinition RepellingBlast { get; } =
            GetDefinition<InvocationDefinition>("RepellingBlast");

        internal static InvocationDefinition SignIllOmen { get; } = GetDefinition<InvocationDefinition>("SignIllOmen");

        internal static InvocationDefinition ThiefFiveFates { get; } =
            GetDefinition<InvocationDefinition>("ThiefFiveFates");

        internal static InvocationDefinition ThirstingBlade { get; } =
            GetDefinition<InvocationDefinition>("ThirstingBlade");

        internal static InvocationDefinition VoiceChainMaster { get; } =
            GetDefinition<InvocationDefinition>("VoiceChainMaster");

        internal static InvocationDefinition WitchSight { get; } = GetDefinition<InvocationDefinition>("WitchSight");
    }

    internal static class ItemDefinitions
    {
        internal static ItemDefinition _1_Gold_Coin { get; } = GetDefinition<ItemDefinition>("1_Gold_Coin");
        internal static ItemDefinition _100_Gold_Coins { get; } = GetDefinition<ItemDefinition>("100_Gold_Coins");
        internal static ItemDefinition _100_GP_Emerald { get; } = GetDefinition<ItemDefinition>("100_GP_Emerald");
        internal static ItemDefinition _100_GP_Pearl { get; } = GetDefinition<ItemDefinition>("100_GP_Pearl");
        internal static ItemDefinition _1000_GP_Diamond { get; } = GetDefinition<ItemDefinition>("1000_GP_Diamond");
        internal static ItemDefinition _10000_Gold_Coins { get; } = GetDefinition<ItemDefinition>("10000_Gold_Coins");
        internal static ItemDefinition _10D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("10D6_Copper_Coins");
        internal static ItemDefinition _10D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("10D6_Gold_Coins");
        internal static ItemDefinition _10D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("10D6_Silver_Coins");
        internal static ItemDefinition _12D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("12D6_Copper_Coins");
        internal static ItemDefinition _1D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("1D6_Copper_Coins");
        internal static ItemDefinition _1D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("1D6_Gold_Coins");
        internal static ItemDefinition _1D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("1D6_Silver_Coins");
        internal static ItemDefinition _20_GP_Amethyst { get; } = GetDefinition<ItemDefinition>("20_GP_Amethyst");
        internal static ItemDefinition _200_Gold_Coins { get; } = GetDefinition<ItemDefinition>("200_Gold_Coins");
        internal static ItemDefinition _20D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("20D6_Gold_Coins");
        internal static ItemDefinition _20D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("20D6_Silver_Coins");
        internal static ItemDefinition _250_Gold_Coins { get; } = GetDefinition<ItemDefinition>("250_Gold_Coins");
        internal static ItemDefinition _300_GP_Opal { get; } = GetDefinition<ItemDefinition>("300_GP_Opal");
        internal static ItemDefinition _30D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("30D6_Gold_Coins");
        internal static ItemDefinition _30D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("30D6_Silver_Coins");
        internal static ItemDefinition _40D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("40D6_Gold_Coins");
        internal static ItemDefinition _40D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("40D6_Silver_Coins");
        internal static ItemDefinition _50_Gold_Coins { get; } = GetDefinition<ItemDefinition>("50_Gold_Coins");
        internal static ItemDefinition _50_GP_Sapphire { get; } = GetDefinition<ItemDefinition>("50_GP_Sapphire");

        internal static ItemDefinition _500_GP_Black_Pearl_Powder { get; } =
            GetDefinition<ItemDefinition>("500_GP_Black_Pearl_Powder");

        internal static ItemDefinition _500_GP_Ruby { get; } = GetDefinition<ItemDefinition>("500_GP_Ruby");
        internal static ItemDefinition _5D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("5D6_Gold_Coins");
        internal static ItemDefinition _5D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("5D6_Silver_Coins");
        internal static ItemDefinition _60D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("60D6_Gold_Coins");
        internal static ItemDefinition _60D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("60D6_Silver_Coins");
        internal static ItemDefinition _6D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("6D6_Copper_Coins");
        internal static ItemDefinition _8D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("8D6_Copper_Coins");

        internal static ItemDefinition ABJURATION_Adrasteia_Letter_Document { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_Adrasteia_Letter_Document");

        internal static ItemDefinition Abjuration_Baron_Cell_Key { get; } =
            GetDefinition<ItemDefinition>("Abjuration_Baron_Cell_Key");

        internal static ItemDefinition ABJURATION_Basement_LightFiller_Document { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_Basement_LightFiller_Document");

        internal static ItemDefinition ABJURATION_EmtanHolySymbol_Imbued_Quest { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_EmtanHolySymbol_Imbued_Quest");

        internal static ItemDefinition ABJURATION_EmtanHolySymbol_Quest { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_EmtanHolySymbol_Quest");

        internal static ItemDefinition ABJURATION_EmtanHolySymbol_Relic { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_EmtanHolySymbol_Relic");

        internal static ItemDefinition ABJURATION_EmtansPackage_Quest { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_EmtansPackage_Quest");

        internal static ItemDefinition ABJURATION_Estalla_Letter_Document { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_Estalla_Letter_Document");

        internal static ItemDefinition ABJURATION_MastersmithLoreDocument { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_MastersmithLoreDocument");

        internal static ItemDefinition ABJURATION_MonasteryJournal_quest { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_MonasteryJournal_quest");

        internal static ItemDefinition ABJURATION_MonasteryJournal_relic { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_MonasteryJournal_relic");

        internal static ItemDefinition ABJURATION_Razan_Letter_Document { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_Razan_Letter_Document");

        internal static ItemDefinition ABJURATION_SecretSpellBook2 { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_SecretSpellBook2");

        internal static ItemDefinition ABJURATION_TOWER_ElvenWars { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_ElvenWars");

        internal static ItemDefinition ABJURATION_TOWER_List { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_List");

        internal static ItemDefinition ABJURATION_TOWER_Manifest { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_Manifest");

        internal static ItemDefinition ABJURATION_TOWER_Order { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_Order");

        internal static ItemDefinition ABJURATION_TOWER_Poem { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_Poem");

        internal static ItemDefinition ABJURATION_ValeOfRemembrance { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_ValeOfRemembrance");

        internal static ItemDefinition AdamantinePlateArmor { get; } =
            GetDefinition<ItemDefinition>("AdamantinePlateArmor");

        internal static ItemDefinition AlchemistFire { get; } = GetDefinition<ItemDefinition>("AlchemistFire");
        internal static ItemDefinition AmuletOfHealth { get; } = GetDefinition<ItemDefinition>("AmuletOfHealth");
        internal static ItemDefinition AmuletOfPureSouls { get; } = GetDefinition<ItemDefinition>("AmuletOfPureSouls");
        internal static ItemDefinition Angbi_Bones_01 { get; } = GetDefinition<ItemDefinition>("Angbi_Bones_01");
        internal static ItemDefinition Angbi_Bones_02 { get; } = GetDefinition<ItemDefinition>("Angbi_Bones_02");
        internal static ItemDefinition Angbi_Bones_03 { get; } = GetDefinition<ItemDefinition>("Angbi_Bones_03");
        internal static ItemDefinition ArcaneFocusOrb { get; } = GetDefinition<ItemDefinition>("ArcaneFocusOrb");
        internal static ItemDefinition ArcaneFocusWand { get; } = GetDefinition<ItemDefinition>("ArcaneFocusWand");
        internal static ItemDefinition ArcaneShieldstaff { get; } = GetDefinition<ItemDefinition>("ArcaneShieldstaff");

        internal static ItemDefinition ARISTOCRAT_AdriasJournal { get; } =
            GetDefinition<ItemDefinition>("ARISTOCRAT_AdriasJournal");

        internal static ItemDefinition Arrow { get; } = GetDefinition<ItemDefinition>("Arrow");

        internal static ItemDefinition Arrow_Alchemy_Corrosive { get; } =
            GetDefinition<ItemDefinition>("Arrow_Alchemy_Corrosive");

        internal static ItemDefinition Arrow_Alchemy_Flaming { get; } =
            GetDefinition<ItemDefinition>("Arrow_Alchemy_Flaming");

        internal static ItemDefinition Arrow_Alchemy_Flash { get; } =
            GetDefinition<ItemDefinition>("Arrow_Alchemy_Flash");

        internal static ItemDefinition Arrow_Poisoned_ArivadsKiss { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_ArivadsKiss");

        internal static ItemDefinition Arrow_Poisoned_ArunsLight { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_ArunsLight");

        internal static ItemDefinition Arrow_Poisoned_BasicPoison { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_BasicPoison");

        internal static ItemDefinition Arrow_Poisoned_BrimstoneFang { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_BrimstoneFang");

        internal static ItemDefinition Arrow_Poisoned_DarkStab { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_DarkStab");

        internal static ItemDefinition Arrow_Poisoned_DeepPain { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_DeepPain");

        internal static ItemDefinition Arrow_Poisoned_GhoulsCaress { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_GhoulsCaress");

        internal static ItemDefinition Arrow_Poisoned_MaraikesTorpor { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_MaraikesTorpor");

        internal static ItemDefinition Arrow_Poisoned_SpiderQueenBlood { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_SpiderQueenBlood");

        internal static ItemDefinition Arrow_Poisoned_TheBurden { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_TheBurden");

        internal static ItemDefinition Arrow_Poisoned_TheLongNight { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_TheLongNight");

        internal static ItemDefinition Arrow_Poisoned_TigerFang { get; } =
            GetDefinition<ItemDefinition>("Arrow_Poisoned_TigerFang");

        internal static ItemDefinition ArrowPlus1 { get; } = GetDefinition<ItemDefinition>("Arrow+1");
        internal static ItemDefinition ArrowPlus2 { get; } = GetDefinition<ItemDefinition>("Arrow+2");

        internal static ItemDefinition Art_Item_25_GP_BronzeStatuette { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_BronzeStatuette");

        internal static ItemDefinition Art_Item_25_GP_EngraveBoneDice { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_EngraveBoneDice");

        internal static ItemDefinition Art_Item_25_GP_GoldBracelet { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_GoldBracelet");

        internal static ItemDefinition Art_Item_25_GP_GoldLocket { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_GoldLocket");

        internal static ItemDefinition Art_Item_25_GP_Mirror { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_Mirror");

        internal static ItemDefinition Art_Item_25_GP_SilkScarf { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_SilkScarf");

        internal static ItemDefinition Art_Item_25_GP_SilverBrooch { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_SilverBrooch");

        internal static ItemDefinition Art_Item_25_GP_SilverChalice { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_SilverChalice");

        internal static ItemDefinition Art_Item_25_GP_SilverComb { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_SilverComb");

        internal static ItemDefinition Art_Item_25_GP_VelvetMask { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_VelvetMask");

        internal static ItemDefinition Art_Item_50_GP_AnimalFigurine { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_AnimalFigurine");

        internal static ItemDefinition Art_Item_50_GP_BronzeCrown { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_BronzeCrown");

        internal static ItemDefinition Art_Item_50_GP_ChaliceGem { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_ChaliceGem");

        internal static ItemDefinition Art_Item_50_GP_DragonStatuette { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_DragonStatuette");

        internal static ItemDefinition Art_Item_50_GP_GemBracelet { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_GemBracelet");

        internal static ItemDefinition Art_Item_50_GP_GemRing { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_GemRing");

        internal static ItemDefinition Art_Item_50_GP_GemstonePendant { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_GemstonePendant");

        internal static ItemDefinition Art_Item_50_GP_ImperialFigurine { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_ImperialFigurine");

        internal static ItemDefinition Art_Item_50_GP_IvoryStatuette { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_IvoryStatuette");

        internal static ItemDefinition Art_Item_50_GP_JadePendant { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_JadePendant");

        internal static ItemDefinition Art_Item_50_GP_SilkRobe { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_SilkRobe");

        internal static ItemDefinition ArtisanToolSmithTools { get; } =
            GetDefinition<ItemDefinition>("ArtisanToolSmithTools");

        internal static ItemDefinition ArwinMertonSword { get; } = GetDefinition<ItemDefinition>("ArwinMertonSword");

        internal static ItemDefinition Backer_Document_01 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_01");

        internal static ItemDefinition Backer_Document_02 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_02");

        internal static ItemDefinition Backer_Document_03 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_03");

        internal static ItemDefinition Backer_Document_04 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_04");

        internal static ItemDefinition Backer_Document_05 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_05");

        internal static ItemDefinition Backer_Document_06 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_06");

        internal static ItemDefinition Backer_Document_07 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_07");

        internal static ItemDefinition Backer_Document_08 { get; } =
            GetDefinition<ItemDefinition>("Backer_Document_08");

        internal static ItemDefinition Background_Academic_Notebook { get; } =
            GetDefinition<ItemDefinition>("Background_Academic_Notebook");

        internal static ItemDefinition Background_Acolyte_PrayersBook { get; } =
            GetDefinition<ItemDefinition>("Background_Acolyte_PrayersBook");

        internal static ItemDefinition Background_Aristocrat_Ring { get; } =
            GetDefinition<ItemDefinition>("Background_Aristocrat_Ring");

        internal static ItemDefinition Background_Artist_Contract { get; } =
            GetDefinition<ItemDefinition>("Background_Artist_Contract");

        internal static ItemDefinition Background_Lawkeeper_Badge { get; } =
            GetDefinition<ItemDefinition>("Background_Lawkeeper_Badge");

        internal static ItemDefinition Background_Occultist_Notebook { get; } =
            GetDefinition<ItemDefinition>("Background_Occultist_Notebook");

        internal static ItemDefinition Background_Sellsword_Ribbon { get; } =
            GetDefinition<ItemDefinition>("Background_Sellsword_Ribbon");

        internal static ItemDefinition Backpack { get; } = GetDefinition<ItemDefinition>("Backpack");

        internal static ItemDefinition Backpack_Bag_Of_Holding { get; } =
            GetDefinition<ItemDefinition>("Backpack_Bag_Of_Holding");

        internal static ItemDefinition Backpack_Handy_Haversack { get; } =
            GetDefinition<ItemDefinition>("Backpack_Handy_Haversack");

        internal static ItemDefinition BallOfLightning { get; } = GetDefinition<ItemDefinition>("BallOfLightning");
        internal static ItemDefinition BarbarianClothes { get; } = GetDefinition<ItemDefinition>("BarbarianClothes");
        internal static ItemDefinition Bard_Armor { get; } = GetDefinition<ItemDefinition>("Bard_Armor");
        internal static ItemDefinition Battleaxe { get; } = GetDefinition<ItemDefinition>("Battleaxe");
        internal static ItemDefinition BattleaxePlus1 { get; } = GetDefinition<ItemDefinition>("Battleaxe+1");
        internal static ItemDefinition BeltOfDwarvenKind { get; } = GetDefinition<ItemDefinition>("BeltOfDwarvenKind");
        internal static ItemDefinition BeltOfGiantFire { get; } = GetDefinition<ItemDefinition>("BeltOfGiantFire");

        internal static ItemDefinition BeltOfGiantHillStrength { get; } =
            GetDefinition<ItemDefinition>("BeltOfGiantHillStrength");

        internal static ItemDefinition BeltOfGiantStone { get; } = GetDefinition<ItemDefinition>("BeltOfGiantStone");

        internal static ItemDefinition BeltOfRegeneration { get; } =
            GetDefinition<ItemDefinition>("BeltOfRegeneration");

        internal static ItemDefinition BeltOfTheBarbarianKing { get; } =
            GetDefinition<ItemDefinition>("BeltOfTheBarbarianKing");

        internal static ItemDefinition Berry_Ration { get; } = GetDefinition<ItemDefinition>("Berry_Ration");
        internal static ItemDefinition Bolt { get; } = GetDefinition<ItemDefinition>("Bolt");

        internal static ItemDefinition Bolt_Alchemy_Corrosive { get; } =
            GetDefinition<ItemDefinition>("Bolt_Alchemy_Corrosive");

        internal static ItemDefinition Bolt_Alchemy_Flaming { get; } =
            GetDefinition<ItemDefinition>("Bolt_Alchemy_Flaming");

        internal static ItemDefinition Bolt_Alchemy_Flash { get; } =
            GetDefinition<ItemDefinition>("Bolt_Alchemy_Flash");

        internal static ItemDefinition Bolt_Poisoned_ArivadsKiss { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_ArivadsKiss");

        internal static ItemDefinition Bolt_Poisoned_ArunsLight { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_ArunsLight");

        internal static ItemDefinition Bolt_Poisoned_BasicPoison { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_BasicPoison");

        internal static ItemDefinition Bolt_Poisoned_BrimstoneFang { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_BrimstoneFang");

        internal static ItemDefinition Bolt_Poisoned_DarkStab { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_DarkStab");

        internal static ItemDefinition Bolt_Poisoned_DeepPain { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_DeepPain");

        internal static ItemDefinition Bolt_Poisoned_GhoulsCaress { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_GhoulsCaress");

        internal static ItemDefinition Bolt_Poisoned_MaraikesTorpor { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_MaraikesTorpor");

        internal static ItemDefinition Bolt_Poisoned_SpiderQueenBlood { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_SpiderQueenBlood");

        internal static ItemDefinition Bolt_Poisoned_TheBurden { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_TheBurden");

        internal static ItemDefinition Bolt_Poisoned_TheLongNight { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_TheLongNight");

        internal static ItemDefinition Bolt_Poisoned_TigerFang { get; } =
            GetDefinition<ItemDefinition>("Bolt_Poisoned_TigerFang");

        internal static ItemDefinition BoltPlus1 { get; } = GetDefinition<ItemDefinition>("Bolt+1");

        internal static ItemDefinition BONEKEEP_AdventurerJournal01 { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_AdventurerJournal01");

        internal static ItemDefinition BONEKEEP_AdventurerJournal02 { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_AdventurerJournal02");

        internal static ItemDefinition BONEKEEP_AkshasJournal { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_AkshasJournal");

        internal static ItemDefinition BONEKEEP_AngbiJournal { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_AngbiJournal");

        internal static ItemDefinition BONEKEEP_LizzariaJournal { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_LizzariaJournal");

        internal static ItemDefinition BONEKEEP_MagicRune { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_MagicRune");

        internal static ItemDefinition BONEKEEP_MardrachtJournal { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_MardrachtJournal");

        internal static ItemDefinition BONEKEEP_SecretSpellBook5 { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_SecretSpellBook5");

        internal static ItemDefinition BONEKEEP_Solar_Rune_Plaque { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_Solar_Rune_Plaque");

        internal static ItemDefinition Bongo { get; } = GetDefinition<ItemDefinition>("Bongo");
        internal static ItemDefinition Boots_SixLeagues { get; } = GetDefinition<ItemDefinition>("Boots_SixLeagues");
        internal static ItemDefinition BootsLevitation { get; } = GetDefinition<ItemDefinition>("BootsLevitation");
        internal static ItemDefinition BootsOfElvenKind { get; } = GetDefinition<ItemDefinition>("BootsOfElvenKind");

        internal static ItemDefinition BootsOfFireWalking { get; } =
            GetDefinition<ItemDefinition>("BootsOfFireWalking");

        internal static ItemDefinition BootsOfFirstStrike { get; } =
            GetDefinition<ItemDefinition>("BootsOfFirstStrike");

        internal static ItemDefinition BootsOfStridingAndSpringing { get; } =
            GetDefinition<ItemDefinition>("BootsOfStridingAndSpringing");

        internal static ItemDefinition BootsOfTheWinterland { get; } =
            GetDefinition<ItemDefinition>("BootsOfTheWinterland");

        internal static ItemDefinition BootsWinged { get; } = GetDefinition<ItemDefinition>("BootsWinged");

        internal static ItemDefinition Bracers_Of_Archery { get; } =
            GetDefinition<ItemDefinition>("Bracers_Of_Archery");

        internal static ItemDefinition Bracers_Of_Defense { get; } =
            GetDefinition<ItemDefinition>("Bracers_Of_Defense");

        internal static ItemDefinition Bracers_Of_Storms { get; } = GetDefinition<ItemDefinition>("Bracers_Of_Storms");

        internal static ItemDefinition Bracers_Of_StunningStrike { get; } =
            GetDefinition<ItemDefinition>("Bracers_Of_StunningStrike");

        internal static ItemDefinition BracersOfSparkles { get; } = GetDefinition<ItemDefinition>("BracersOfSparkles");
        internal static ItemDefinition Breastplate { get; } = GetDefinition<ItemDefinition>("Breastplate");
        internal static ItemDefinition BreastplatePlus1 { get; } = GetDefinition<ItemDefinition>("Breastplate+1");
        internal static ItemDefinition Brightwall { get; } = GetDefinition<ItemDefinition>("Brightwall");
        internal static ItemDefinition BroochOfShielding { get; } = GetDefinition<ItemDefinition>("BroochOfShielding");
        internal static ItemDefinition BurglarPack { get; } = GetDefinition<ItemDefinition>("BurglarPack");

        internal static ItemDefinition CAERLEM_Daliat_Document { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_Daliat_Document");

        internal static ItemDefinition CaerLem_Gate_Plaque { get; } =
            GetDefinition<ItemDefinition>("CaerLem_Gate_Plaque");

        internal static ItemDefinition CAERLEM_GoblinStudy { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_GoblinStudy");

        internal static ItemDefinition CAERLEM_Inquisitor_Document { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_Inquisitor_Document");

        internal static ItemDefinition CAERLEM_Notebook_Henrik { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_Notebook_Henrik");

        internal static ItemDefinition CAERLEM_TirmarianHolySymbol { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_TirmarianHolySymbol");

        internal static ItemDefinition Candle { get; } = GetDefinition<ItemDefinition>("Candle");
        internal static ItemDefinition CaveIllnessDrug { get; } = GetDefinition<ItemDefinition>("CaveIllnessDrug");
        internal static ItemDefinition ChainMail { get; } = GetDefinition<ItemDefinition>("ChainMail");
        internal static ItemDefinition ChainmailPlus1 { get; } = GetDefinition<ItemDefinition>("Chainmail+1");
        internal static ItemDefinition ChainShirt { get; } = GetDefinition<ItemDefinition>("ChainShirt");

        internal static ItemDefinition ChargedBlueSapphire { get; } =
            GetDefinition<ItemDefinition>("ChargedBlueSapphire");

        internal static ItemDefinition ChargedRedCorundum { get; } =
            GetDefinition<ItemDefinition>("ChargedRedCorundum");

        internal static ItemDefinition ChargedYellowDiamond { get; } =
            GetDefinition<ItemDefinition>("ChargedYellowDiamond");

        internal static ItemDefinition ChimeOfOpening { get; } = GetDefinition<ItemDefinition>("ChimeOfOpening");
        internal static ItemDefinition ChitinousBoon { get; } = GetDefinition<ItemDefinition>("ChitinousBoon");
        internal static ItemDefinition CircletOfBlasting { get; } = GetDefinition<ItemDefinition>("CircletOfBlasting");
        internal static ItemDefinition CloakA { get; } = GetDefinition<ItemDefinition>("CloakA");
        internal static ItemDefinition CloakOfArachnida { get; } = GetDefinition<ItemDefinition>("CloakOfArachnida");
        internal static ItemDefinition CloakOfBat { get; } = GetDefinition<ItemDefinition>("CloakOfBat");

        internal static ItemDefinition CloakOfDisplacement { get; } =
            GetDefinition<ItemDefinition>("CloakOfDisplacement");

        internal static ItemDefinition CloakOfElvenkind { get; } = GetDefinition<ItemDefinition>("CloakOfElvenkind");
        internal static ItemDefinition CloakOfProtection { get; } = GetDefinition<ItemDefinition>("CloakOfProtection");

        internal static ItemDefinition CloakOfTheAncientKing { get; } =
            GetDefinition<ItemDefinition>("CloakOfTheAncientKing");

        internal static ItemDefinition CloakOfTheDandy { get; } = GetDefinition<ItemDefinition>("CloakOfTheDandy");
        internal static ItemDefinition CloakOfUbiquity { get; } = GetDefinition<ItemDefinition>("CloakOfUbiquity");
        internal static ItemDefinition ClothesCommon { get; } = GetDefinition<ItemDefinition>("ClothesCommon");

        internal static ItemDefinition ClothesCommon_Tattoo { get; } =
            GetDefinition<ItemDefinition>("ClothesCommon_Tattoo");

        internal static ItemDefinition ClothesCommon_Valley { get; } =
            GetDefinition<ItemDefinition>("ClothesCommon_Valley");

        internal static ItemDefinition ClothesDefiler { get; } = GetDefinition<ItemDefinition>("ClothesDefiler");
        internal static ItemDefinition ClothesNoble { get; } = GetDefinition<ItemDefinition>("ClothesNoble");

        internal static ItemDefinition ClothesNoble_Valley { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley");

        internal static ItemDefinition ClothesNoble_Valley_Cherry { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Cherry");

        internal static ItemDefinition ClothesNoble_Valley_Green { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Green");

        internal static ItemDefinition ClothesNoble_Valley_Orange { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Orange");

        internal static ItemDefinition ClothesNoble_Valley_Pink { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Pink");

        internal static ItemDefinition ClothesNoble_Valley_Purple { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Purple");

        internal static ItemDefinition ClothesNoble_Valley_Red { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Red");

        internal static ItemDefinition ClothesNoble_Valley_Silver { get; } =
            GetDefinition<ItemDefinition>("ClothesNoble_Valley_Silver");

        internal static ItemDefinition ClothesSage { get; } = GetDefinition<ItemDefinition>("ClothesSage");

        internal static ItemDefinition ClothesScavenger_A { get; } =
            GetDefinition<ItemDefinition>("ClothesScavenger_A");

        internal static ItemDefinition ClothesScavenger_B { get; } =
            GetDefinition<ItemDefinition>("ClothesScavenger_B");

        internal static ItemDefinition ClothesTabard { get; } = GetDefinition<ItemDefinition>("ClothesTabard");

        internal static ItemDefinition ClothesTabardAlternate { get; } =
            GetDefinition<ItemDefinition>("ClothesTabardAlternate");

        internal static ItemDefinition ClothesTabardCouncil { get; } =
            GetDefinition<ItemDefinition>("ClothesTabardCouncil");

        internal static ItemDefinition ClothesWizard { get; } = GetDefinition<ItemDefinition>("ClothesWizard");
        internal static ItemDefinition ClothesWizard_B { get; } = GetDefinition<ItemDefinition>("ClothesWizard_B");
        internal static ItemDefinition Club { get; } = GetDefinition<ItemDefinition>("Club");
        internal static ItemDefinition ComponentPouch { get; } = GetDefinition<ItemDefinition>("ComponentPouch");

        internal static ItemDefinition ComponentPouch_ArcaneAmulet { get; } =
            GetDefinition<ItemDefinition>("ComponentPouch_ArcaneAmulet");

        internal static ItemDefinition ComponentPouch_Belt { get; } =
            GetDefinition<ItemDefinition>("ComponentPouch_Belt");

        internal static ItemDefinition ComponentPouch_Bracers { get; } =
            GetDefinition<ItemDefinition>("ComponentPouch_Bracers");

        internal static ItemDefinition ComponentPouch_Circlet { get; } =
            GetDefinition<ItemDefinition>("ComponentPouch_Circlet");

        internal static ItemDefinition Conch { get; } = GetDefinition<ItemDefinition>("Conch");

        internal static ItemDefinition CONJURATION_SecretSpellBook3 { get; } =
            GetDefinition<ItemDefinition>("CONJURATION_SecretSpellBook3");

        internal static ItemDefinition Coparann_Mines_Document_01 { get; } =
            GetDefinition<ItemDefinition>("Coparann_Mines_Document_01");

        internal static ItemDefinition Coparann_Mines_HideoutKey { get; } =
            GetDefinition<ItemDefinition>("Coparann_Mines_HideoutKey");

        internal static ItemDefinition Coparann_Mines_WagonLever { get; } =
            GetDefinition<ItemDefinition>("Coparann_Mines_WagonLever");

        internal static ItemDefinition CraftingManual_Alchemy_Corrosive_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Alchemy_Corrosive_Arrows");

        internal static ItemDefinition CraftingManual_Alchemy_Corrosive_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Alchemy_Corrosive_Bolts");

        internal static ItemDefinition CraftingManual_Alchemy_Flaming_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Alchemy_Flaming_Arrows");

        internal static ItemDefinition CraftingManual_Alchemy_Flaming_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Alchemy_Flaming_Bolts");

        internal static ItemDefinition CraftingManual_Alchemy_Flash_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Alchemy_Flash_Arrows");

        internal static ItemDefinition CraftingManual_Alchemy_Flash_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Alchemy_Flash_Bolts");

        internal static ItemDefinition CraftingManual_BasicPoison_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_BasicPoison_Arrows");

        internal static ItemDefinition CraftingManual_BasicPoison_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_BasicPoison_Bolts");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_AmuletOfPureSouls { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_AmuletOfPureSouls");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_ArmorOfTheForest { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_ArmorOfTheForest");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BallOfLightning { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BallOfLightning");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BeltOfRegeneration { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BeltOfRegeneration");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BeltOfTheBarbarianKing { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BeltOfTheBarbarianKing");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BootsOfFireWalking { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BootsOfFireWalking");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BootsOfFirstStrike { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BootsOfFirstStrike");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BootsOfTheWinterland { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BootsOfTheWinterland");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_BracersOfStorms { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_BracersOfStorms");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_CloakOfTheAncientKing { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_CloakOfTheAncientKing");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_CloakOfTheDandy { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_CloakOfTheDandy");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_CrossbowOfAccuracy { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_CrossbowOfAccuracy");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_CrossbowSouldrinker { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_CrossbowSouldrinker");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_HeavyCrossbowOfAccuracy { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_HeavyCrossbowOfAccuracy");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_HeavyCrossbowWhiteburn { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_HeavyCrossbowWhiteburn");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_HideArmorOfTheVagrant { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_HideArmorOfTheVagrant");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_HideArmorOfWilderness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_HideArmorOfWilderness");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_MaulOfSmashing { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_MaulOfSmashing");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_MaulOfTheDestroyer { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_MaulOfTheDestroyer");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_PendantOfTheHealer { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_PendantOfTheHealer");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_PeriaptOfTheMasterEnchanter { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_PeriaptOfTheMasterEnchanter");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_RingOfTheAmbassador { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_RingOfTheAmbassador");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_ScimitarOfAcuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_ScimitarOfAcuteness");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_ScimitarOfTheAnfarels { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_ScimitarOfTheAnfarels");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_SpearDoomSpear { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_SpearDoomSpear");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_SpearOfAcuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_SpearOfAcuteness");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_StuddedOfLeadership { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_StuddedOfLeadership");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_StuddedOfSurvival { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_StuddedOfSurvival");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_VestmentOfThePrimalOak { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_VestmentOfThePrimalOak");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_WandOfBlight { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_WandOfBlight");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_WandOfThorns { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_WandOfThorns");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_WandOfWinter { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_WandOfWinter");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_WarhammerOfAcuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_WarhammerOfAcuteness");

        internal static ItemDefinition CraftingManual_DLC1_Enchant_WarhammerStormbinder { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC1_Enchant_WarhammerStormbinder");

        internal static ItemDefinition CraftingManual_DLC2_5_Enchant_GauntletOfAcuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC2.5_Enchant_GauntletOfAcuteness");

        internal static ItemDefinition CraftingManual_DLC2_5_Enchant_GauntletOfSharpness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC2.5_Enchant_GauntletOfSharpness");

        internal static ItemDefinition CraftingManual_DLC2_5_Enchant_WandOfWarMagePlus1 { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC2.5_Enchant_WandOfWarMage+1");

        internal static ItemDefinition CraftingManual_DLC2_5_Enchant_WandOfWarMagePlus2 { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC2.5_Enchant_WandOfWarMage+2");

        internal static ItemDefinition CraftingManual_DLC2_5_Item_RestorativeOintment { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC2.5_Item_RestorativeOintment");

        internal static ItemDefinition CraftingManual_DLC3_Item_CaveIllnessDrug { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_DLC3_Item_CaveIllnessDrug");

        internal static ItemDefinition CraftingManual_Enchant_BattleAxe_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_BattleAxe_of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_BattleAxe_of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_BattleAxe_of_Sharpness");

        internal static ItemDefinition CraftingManual_Enchant_BattleAxe_Punisher { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_BattleAxe_Punisher");

        internal static ItemDefinition CraftingManual_Enchant_Breastplate_Of_Deflection { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Breastplate_Of_Deflection");

        internal static ItemDefinition CraftingManual_Enchant_Breastplate_Of_Sturdiness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Breastplate_Of_Sturdiness");

        internal static ItemDefinition CraftingManual_Enchant_Chainmail_Of_Robustness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Chainmail_Of_Robustness");

        internal static ItemDefinition CraftingManual_Enchant_Chainmail_Of_Sturdiness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Chainmail_Of_Sturdiness");

        internal static ItemDefinition CraftingManual_Enchant_Dagger_Frostburn { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Dagger_Frostburn");

        internal static ItemDefinition CraftingManual_Enchant_Dagger_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Dagger_Of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Dagger_Of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Dagger_Of_Sharpness");

        internal static ItemDefinition CraftingManual_Enchant_Dagger_Souldrinker { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Dagger_Souldrinker");

        internal static ItemDefinition CraftingManual_Enchant_EmpressGarb { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_EmpressGarb");

        internal static ItemDefinition CraftingManual_Enchant_Greataxe_Of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Greataxe_Of_Sharpness");

        internal static ItemDefinition CraftingManual_Enchant_Greataxe_Stormblade { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Greataxe_Stormblade");

        internal static ItemDefinition CraftingManual_Enchant_Greatsword_Doomblade { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Greatsword_Doomblade");

        internal static ItemDefinition CraftingManual_Enchant_Greatsword_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Greatsword_Lightbringer");

        internal static ItemDefinition CraftingManual_Enchant_Greatsword_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Greatsword_Of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Halfplate_Of_Robustness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Halfplate_Of_Robustness");

        internal static ItemDefinition CraftingManual_Enchant_Halfplate_Of_Sturdiness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Halfplate_Of_Sturdiness");

        internal static ItemDefinition CraftingManual_Enchant_Leather_Armor_Of_FlameDancing { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Leather_Armor_Of_FlameDancing");

        internal static ItemDefinition CraftingManual_Enchant_Leather_Armor_Of_Robustness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Leather_Armor_Of_Robustness");

        internal static ItemDefinition CraftingManual_Enchant_Leather_Armor_Of_Sturdiness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Leather_Armor_Of_Sturdiness");

        internal static ItemDefinition CraftingManual_Enchant_Leather_Armor_Of_Survival { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Leather_Armor_Of_Survival");

        internal static ItemDefinition CraftingManual_Enchant_Longbow_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longbow_Lightbringer");

        internal static ItemDefinition CraftingManual_Enchant_Longbow_Of_Accuracy { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longbow_Of_Accuracy");

        internal static ItemDefinition CraftingManual_Enchant_Longbow_Stormbow { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longbow_Stormbow");

        internal static ItemDefinition CraftingManual_Enchant_Longsword_Dragonblade { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longsword_Dragonblade");

        internal static ItemDefinition CraftingManual_Enchant_Longsword_Frostburn { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longsword_Frostburn");

        internal static ItemDefinition CraftingManual_Enchant_Longsword_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longsword_of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Longsword_Stormblade { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longsword_Stormblade");

        internal static ItemDefinition CraftingManual_Enchant_Longsword_Warden { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longsword_Warden");

        internal static ItemDefinition CraftingManual_Enchant_Mace_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Mace_Of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Mace_Of_Smashing { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Mace_Of_Smashing");

        internal static ItemDefinition CraftingManual_Enchant_Morningstar_Bearclaw { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Morningstar_Bearclaw");

        internal static ItemDefinition CraftingManual_Enchant_Morningstar_Of_Power { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Morningstar_Of_Power");

        internal static ItemDefinition CraftingManual_Enchant_Plate_Of_Robustness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Plate_Of_Robustness");

        internal static ItemDefinition CraftingManual_Enchant_Plate_Of_Sturdiness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Plate_Of_Sturdiness");

        internal static ItemDefinition CraftingManual_Enchant_Rapier_BlackAdder { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Rapier_BlackAdder");

        internal static ItemDefinition CraftingManual_Enchant_Rapier_Doomblade { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Rapier_Doomblade");

        internal static ItemDefinition CraftingManual_Enchant_Rapier_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Rapier_of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Scale_Of_IceDancing { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Scale_Of_IceDancing");

        internal static ItemDefinition CraftingManual_Enchant_Scale_Of_Robustness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Scale_Of_Robustness");

        internal static ItemDefinition CraftingManual_Enchant_Scale_Of_Sturdiness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Scale_Of_Sturdiness");

        internal static ItemDefinition CraftingManual_Enchant_Scimitar_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Scimitar_of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Shortbow_Medusa { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortbow_Medusa");

        internal static ItemDefinition CraftingManual_Enchant_Shortbow_Of_Accuracy { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortbow_Of_Accuracy");

        internal static ItemDefinition CraftingManual_Enchant_Shortbow_Of_Sharpshooting { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortbow_Of_Sharpshooting");

        internal static ItemDefinition CraftingManual_Enchant_Shortsword_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortsword_Lightbringer");

        internal static ItemDefinition CraftingManual_Enchant_Shortsword_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortsword_Of_Acuteness");

        internal static ItemDefinition CraftingManual_Enchant_Shortsword_Of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortsword_Of_Sharpness");

        internal static ItemDefinition CraftingManual_Enchant_Shortsword_Sovereign { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortsword_Sovereign");

        internal static ItemDefinition CraftingManual_Enchant_Shortsword_Whiteburn { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Shortsword_Whiteburn");

        internal static ItemDefinition CraftingManual_Poison_ArivadsKiss { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_ArivadsKiss");

        internal static ItemDefinition CraftingManual_Poison_ArivadsKiss_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_ArivadsKiss_Bolts");

        internal static ItemDefinition CraftingManual_Poison_ArivadsKissArrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_ArivadsKissArrows");

        internal static ItemDefinition CraftingManual_Poison_ArunsLight { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_ArunsLight");

        internal static ItemDefinition CraftingManual_Poison_ArunsLight_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_ArunsLight_Arrows");

        internal static ItemDefinition CraftingManual_Poison_ArunsLight_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_ArunsLight_Bolts");

        internal static ItemDefinition CraftingManual_Poison_BrimstoneFang { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_BrimstoneFang");

        internal static ItemDefinition CraftingManual_Poison_BrimstoneFang_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_BrimstoneFang_Arrows");

        internal static ItemDefinition CraftingManual_Poison_BrimstoneFang_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_BrimstoneFang_Bolts");

        internal static ItemDefinition CraftingManual_Poison_DarkStab { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_DarkStab");

        internal static ItemDefinition CraftingManual_Poison_DarkStab_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_DarkStab_Arrows");

        internal static ItemDefinition CraftingManual_Poison_DarkStab_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_DarkStab_Bolts");

        internal static ItemDefinition CraftingManual_Poison_DeepPain { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_DeepPain");

        internal static ItemDefinition CraftingManual_Poison_DeepPain_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_DeepPain_Arrows");

        internal static ItemDefinition CraftingManual_Poison_DeepPain_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_DeepPain_Bolts");

        internal static ItemDefinition CraftingManual_Poison_GhoulsCaress { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_GhoulsCaress");

        internal static ItemDefinition CraftingManual_Poison_GhoulsCaress_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_GhoulsCaress_Arrows");

        internal static ItemDefinition CraftingManual_Poison_GhoulsCaress_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_GhoulsCaress_Bolts");

        internal static ItemDefinition CraftingManual_Poison_MaraikesTorpor { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_MaraikesTorpor");

        internal static ItemDefinition CraftingManual_Poison_MaraikesTorpor_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_MaraikesTorpor_Arrows");

        internal static ItemDefinition CraftingManual_Poison_MaraikesTorpor_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_MaraikesTorpor_Bolts");

        internal static ItemDefinition CraftingManual_Poison_SpiderQueensBlood { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_SpiderQueensBlood");

        internal static ItemDefinition CraftingManual_Poison_SpiderQueensBlood_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_SpiderQueensBlood_Arrows");

        internal static ItemDefinition CraftingManual_Poison_SpiderQueensBlood_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_SpiderQueensBlood_Bolts");

        internal static ItemDefinition CraftingManual_Poison_TheBurden { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TheBurden");

        internal static ItemDefinition CraftingManual_Poison_TheBurden_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TheBurden_Arrows");

        internal static ItemDefinition CraftingManual_Poison_TheBurden_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TheBurden_Bolts");

        internal static ItemDefinition CraftingManual_Poison_TheLongNight { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TheLongNight");

        internal static ItemDefinition CraftingManual_Poison_TheLongNight_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TheLongNight_Arrows");

        internal static ItemDefinition CraftingManual_Poison_TheLongNight_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TheLongNight_Bolts");

        internal static ItemDefinition CraftingManual_Poison_TigerFang { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TigerFang");

        internal static ItemDefinition CraftingManual_Poison_TigerFang_Arrows { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TigerFang_Arrows");

        internal static ItemDefinition CraftingManual_Poison_TigerFang_Bolts { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Poison_TigerFang_Bolts");

        internal static ItemDefinition CraftingManualBasicPoison { get; } =
            GetDefinition<ItemDefinition>("CraftingManualBasicPoison");

        internal static ItemDefinition CraftingManualPotionOfGreaterHealing { get; } =
            GetDefinition<ItemDefinition>("CraftingManualPotionOfGreaterHealing");

        internal static ItemDefinition CraftingManualPotionOfHealing { get; } =
            GetDefinition<ItemDefinition>("CraftingManualPotionOfHealing");

        internal static ItemDefinition CraftingManualPotionOfHeroism { get; } =
            GetDefinition<ItemDefinition>("CraftingManualPotionOfHeroism");

        internal static ItemDefinition CraftingManualPotionOfSuperiorHealing { get; } =
            GetDefinition<ItemDefinition>("CraftingManualPotionOfSuperiorHealing");

        internal static ItemDefinition CraftingManualRemedy { get; } =
            GetDefinition<ItemDefinition>("CraftingManualRemedy");

        internal static ItemDefinition CraftingManualScrollOfAcidArrow { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfAcidArrow");

        internal static ItemDefinition CraftingManualScrollOfBane { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBane");

        internal static ItemDefinition CraftingManualScrollOfBanishment { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBanishment");

        internal static ItemDefinition CraftingManualScrollOfBarkskin { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBarkskin");

        internal static ItemDefinition CraftingManualScrollOfBlackTentacles { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBlackTentacles");

        internal static ItemDefinition CraftingManualScrollOfBless { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBless");

        internal static ItemDefinition CraftingManualScrollOfBlight { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBlight");

        internal static ItemDefinition CraftingManualScrollOfBlindness { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBlindness");

        internal static ItemDefinition CraftingManualScrollOfBlur { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBlur");

        internal static ItemDefinition CraftingManualScrollOfBurningHands { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfBurningHands");

        internal static ItemDefinition CraftingManualScrollOfCharmPerson { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfCharmPerson");

        internal static ItemDefinition CraftingManualScrollOfColorSpray { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfColorSpray");

        internal static ItemDefinition CraftingManualScrollOfCommand { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfCommand");

        internal static ItemDefinition CraftingManualScrollOfComprehendLanguages { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfComprehendLanguages");

        internal static ItemDefinition CraftingManualScrollOfConfusion { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfConfusion");

        internal static ItemDefinition CraftingManualScrollOfConjureAnimals { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfConjureAnimals");

        internal static ItemDefinition CraftingManualScrollOfConjureMinorElemental { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfConjureMinorElemental");

        internal static ItemDefinition CraftingManualScrollOfCounterSpell { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfCounterSpell");

        internal static ItemDefinition CraftingManualScrollOfCureWounds { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfCureWounds");

        internal static ItemDefinition CraftingManualScrollOfDarkness { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDarkness");

        internal static ItemDefinition CraftingManualScrollOfDarkvision { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDarkvision");

        internal static ItemDefinition CraftingManualScrollOfDaylight { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDaylight");

        internal static ItemDefinition CraftingManualScrollOfDeathWard { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDeathWard");

        internal static ItemDefinition CraftingManualScrollOfDetectEvilAndGood { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDetectEvilAndGood");

        internal static ItemDefinition CraftingManualScrollOfDetectMagic { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDetectMagic");

        internal static ItemDefinition CraftingManualScrollOfDetectPoisonAndDisease { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDetectPoisonAndDisease");

        internal static ItemDefinition CraftingManualScrollOfDimensionDoor { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfDimensionDoor");

        internal static ItemDefinition CraftingManualScrollOfEntangle { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfEntangle");

        internal static ItemDefinition CraftingManualScrollOfExpeditiousRetreat { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfExpeditiousRetreat");

        internal static ItemDefinition CraftingManualScrollOfFaerieFire { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFaerieFire");

        internal static ItemDefinition CraftingManualScrollOfFear { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFear");

        internal static ItemDefinition CraftingManualScrollOfFindTraps { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFindTraps");

        internal static ItemDefinition CraftingManualScrollOfFireball { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFireball");

        internal static ItemDefinition CraftingManualScrollOfFireShield { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFireShield");

        internal static ItemDefinition CraftingManualScrollOfFlameBlade { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFlameBlade");

        internal static ItemDefinition CraftingManualScrollOfFlamingSphere { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFlamingSphere");

        internal static ItemDefinition CraftingManualScrollOfFly { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFly");

        internal static ItemDefinition CraftingManualScrollOfFogCloud { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFogCloud");

        internal static ItemDefinition CraftingManualScrollOfFreedomOfMovement { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfFreedomOfMovement");

        internal static ItemDefinition CraftingManualScrollOfGrease { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfGrease");

        internal static ItemDefinition CraftingManualScrollOfGreaterInvisibility { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfGreaterInvisibility");

        internal static ItemDefinition CraftingManualScrollOfGuardianOfFaith { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfGuardianOfFaith");

        internal static ItemDefinition CraftingManualScrollOfGuidingBolt { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfGuidingBolt");

        internal static ItemDefinition CraftingManualScrollOfHaste { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfHaste");

        internal static ItemDefinition CraftingManualScrollOfHoldPerson { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfHoldPerson");

        internal static ItemDefinition CraftingManualScrollOfHypnoticPattern { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfHypnoticPattern");

        internal static ItemDefinition CraftingManualScrollOfIceStorm { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfIceStorm");

        internal static ItemDefinition CraftingManualScrollOfInflictWounds { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfInflictWounds");

        internal static ItemDefinition CraftingManualScrollOfInvisibility { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfInvisibility");

        internal static ItemDefinition CraftingManualScrollOfJump { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfJump");

        internal static ItemDefinition CraftingManualScrollOfKnock { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfKnock");

        internal static ItemDefinition CraftingManualScrollOfLesserRestoration { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfLesserRestoration");

        internal static ItemDefinition CraftingManualScrollOfLevitate { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfLevitate");

        internal static ItemDefinition CraftingManualScrollOfLightningBolt { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfLightningBolt");

        internal static ItemDefinition CraftingManualScrollOfMageArmor { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfMageArmor");

        internal static ItemDefinition CraftingManualScrollOfMagicMissile { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfMagicMissile");

        internal static ItemDefinition CraftingManualScrollOfMagicWeapon { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfMagicWeapon");

        internal static ItemDefinition CraftingManualScrollOfMassHealingWord { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfMassHealingWord");

        internal static ItemDefinition CraftingManualScrollOfMirrorImage { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfMirrorImage");

        internal static ItemDefinition CraftingManualScrollOfMistyStep { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfMistyStep");

        internal static ItemDefinition CraftingManualScrollOfPassWithoutTrace { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfPassWithoutTrace");

        internal static ItemDefinition CraftingManualScrollOfPhantasmalKiller { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfPhantasmalKiller");

        internal static ItemDefinition CraftingManualScrollOfProtectionFromEnergy { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfProtectionFromEnergy");

        internal static ItemDefinition CraftingManualScrollOfProtectionFromEvilAndGood { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfProtectionFromEvilAndGood");

        internal static ItemDefinition CraftingManualScrollOfRayOfEnfeeblement { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfRayOfEnfeeblement");

        internal static ItemDefinition CraftingManualScrollOfRemoveCurse { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfRemoveCurse");

        internal static ItemDefinition CraftingManualScrollOfRevivify { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfRevivify");

        internal static ItemDefinition CraftingManualScrollOfScorchingRay { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfScorchingRay");

        internal static ItemDefinition CraftingManualScrollOfSeeInvisibility { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSeeInvisibility");

        internal static ItemDefinition CraftingManualScrollOfShatter { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfShatter");

        internal static ItemDefinition CraftingManualScrollOfShield { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfShield");

        internal static ItemDefinition CraftingManualScrollOfShieldOfFaith { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfShieldOfFaith");

        internal static ItemDefinition CraftingManualScrollOfSilence { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSilence");

        internal static ItemDefinition CraftingManualScrollOfSleep { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSleep");

        internal static ItemDefinition CraftingManualScrollOfSleetStorm { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSleetStorm");

        internal static ItemDefinition CraftingManualScrollOfSlow { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSlow");

        internal static ItemDefinition CraftingManualScrollOfSpiderClimb { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSpiderClimb");

        internal static ItemDefinition CraftingManualScrollOfSpiritGuardians { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSpiritGuardians");

        internal static ItemDefinition CraftingManualScrollOfSpiritualWeapon { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfSpiritualWeapon");

        internal static ItemDefinition CraftingManualScrollOfStinkingCloud { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfStinkingCloud");

        internal static ItemDefinition CraftingManualScrollOfStoneSkin { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfStoneSkin");

        internal static ItemDefinition CraftingManualScrollOfThunderwave { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfThunderwave");

        internal static ItemDefinition CraftingManualScrollOfTongues { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfTongues");

        internal static ItemDefinition CraftingManualScrollOfVampiricTouch { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfVampiricTouch");

        internal static ItemDefinition CraftingManualScrollOfWallOfFire { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfWallOfFire");

        internal static ItemDefinition CraftingManualScrollOfWardingBond { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfWardingBond");

        internal static ItemDefinition CraftingStarterPack { get; } =
            GetDefinition<ItemDefinition>("CraftingStarterPack");

        internal static ItemDefinition CrownOfTheMagister { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister");

        internal static ItemDefinition CrownOfTheMagister01 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister01");

        internal static ItemDefinition CrownOfTheMagister02 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister02");

        internal static ItemDefinition CrownOfTheMagister03 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister03");

        internal static ItemDefinition CrownOfTheMagister04 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister04");

        internal static ItemDefinition CrownOfTheMagister05 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister05");

        internal static ItemDefinition CrownOfTheMagister06 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister06");

        internal static ItemDefinition CrownOfTheMagister07 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister07");

        internal static ItemDefinition CrownOfTheMagister08 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister08");

        internal static ItemDefinition CrownOfTheMagister09 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister09");

        internal static ItemDefinition CrownOfTheMagister10 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister10");

        internal static ItemDefinition CrownOfTheMagister11 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister11");

        internal static ItemDefinition CrownOfTheMagister12 { get; } =
            GetDefinition<ItemDefinition>("CrownOfTheMagister12");

        internal static ItemDefinition CYFLEN_MertonsJournal { get; } =
            GetDefinition<ItemDefinition>("CYFLEN_MertonsJournal");

        internal static ItemDefinition Dagger { get; } = GetDefinition<ItemDefinition>("Dagger");

        internal static ItemDefinition Dagger_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("Dagger_Duel_Autoequip");

        internal static ItemDefinition Dagger_Duel_AutoequipOffhand { get; } =
            GetDefinition<ItemDefinition>("Dagger_Duel_AutoequipOffhand");

        internal static ItemDefinition DaggerPlus1 { get; } = GetDefinition<ItemDefinition>("Dagger+1");
        internal static ItemDefinition DaggerPlus2 { get; } = GetDefinition<ItemDefinition>("Dagger+2");
        internal static ItemDefinition Dart { get; } = GetDefinition<ItemDefinition>("Dart");
        internal static ItemDefinition DiplomatPack { get; } = GetDefinition<ItemDefinition>("DiplomatPack");
        internal static ItemDefinition DivineBlade { get; } = GetDefinition<ItemDefinition>("DivineBlade");

        internal static ItemDefinition DLC_Equipment_City_ScepterOfRedeemerControl { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_City_ScepterOfRedeemerControl");

        internal static ItemDefinition DLC_Equipment_Complex_Elevator_Prism { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Elevator_Prism");

        internal static ItemDefinition DLC_Equipment_Complex_Indoor_Book { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Indoor_Book");

        internal static ItemDefinition DLC_Equipment_Complex_Indoor_Potion { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Indoor_Potion");

        internal static ItemDefinition DLC_Equipment_Complex_Main_IronKey { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Main_IronKey");

        internal static ItemDefinition DLC_Equipment_Complex_MajorGate_Fragment { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_MajorGate_Fragment");

        internal static ItemDefinition DLC_Equipment_Complex_ManaGenerator_Fragment { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_ManaGenerator_Fragment");

        internal static ItemDefinition DLC_Equipment_Complex_Tunnels_Gate_Key { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Tunnels_Gate_Key");

        internal static ItemDefinition DLC_Equipment_Complex_Tunnels_Gate_Key_02 { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Tunnels_Gate_Key_02");

        internal static ItemDefinition DLC_Equipment_Complex_Tunnels_Gate_Key_03 { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Tunnels_Gate_Key_03");

        internal static ItemDefinition DLC_Equipment_Complex_Tunnels_Gate_Key_04 { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_Complex_Tunnels_Gate_Key_04");

        internal static ItemDefinition DLC_Equipment_MysteriousPalace_BanditsJournal { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_MysteriousPalace_BanditsJournal");

        internal static ItemDefinition DLC_Equipment_MysteriousPalace_HenchmenJournal { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_MysteriousPalace_HenchmenJournal");

        internal static ItemDefinition DLC_Equipment_MysteriousPalace_Loot { get; } =
            GetDefinition<ItemDefinition>("DLC_Equipment_MysteriousPalace_Loot");

        internal static ItemDefinition DLC1_Equipment_Document_AnfarelLetter { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_AnfarelLetter");

        internal static ItemDefinition DLC1_Equipment_Document_AnfarelRitual { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_AnfarelRitual");

        internal static ItemDefinition DLC1_Equipment_Document_Complex_HybridReport1 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_Complex_HybridReport1");

        internal static ItemDefinition DLC1_Equipment_Document_Complex_HybridReport2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_Complex_HybridReport2");

        internal static ItemDefinition DLC1_Equipment_Document_Complex_HybridReport3 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_Complex_HybridReport3");

        internal static ItemDefinition DLC1_Equipment_Document_Complex_OrenetisStudy { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_Complex_OrenetisStudy");

        internal static ItemDefinition DLC1_Equipment_Document_Complex_SitineroReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_Complex_SitineroReport");

        internal static ItemDefinition DLC1_Equipment_Document_DominionOrdersFood { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionOrdersFood");

        internal static ItemDefinition DLC1_Equipment_Document_DominionOrdersToImprison { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionOrdersToImprison");

        internal static ItemDefinition DLC1_Equipment_Document_DominionSpyLetter1 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionSpyLetter1");

        internal static ItemDefinition DLC1_Equipment_Document_DominionSpyLetter2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionSpyLetter2");

        internal static ItemDefinition DLC1_Equipment_Document_DominionWarningToAnfarel1 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionWarningToAnfarel1");

        internal static ItemDefinition DLC1_Equipment_Document_DominionWarningToAnfarel2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionWarningToAnfarel2");

        internal static ItemDefinition DLC1_Equipment_Document_DominionWarningToAnfarel3 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_DominionWarningToAnfarel3");

        internal static ItemDefinition DLC1_Equipment_Document_EventAdvertisement { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_EventAdvertisement");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeCaptureGiantReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeCaptureGiantReport");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeCaptureGiantReportOrders { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeCaptureGiantReportOrders");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeDryadReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeDryadReport");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeOrcBaseReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeOrcBaseReport");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeRebelMeetingReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeRebelMeetingReport");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeTrollsSightingReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeTrollsSightingReport");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeUndeadSightingReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeUndeadSightingReport");

        internal static ItemDefinition DLC1_Equipment_Document_ForgeWanderingElfReport { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_ForgeWanderingElfReport");

        internal static ItemDefinition DLC1_Equipment_Document_PreManacalon1 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_PreManacalon1");

        internal static ItemDefinition DLC1_Equipment_Document_PreManacalon2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_PreManacalon2");

        internal static ItemDefinition DLC1_Equipment_Document_PreManacalon3 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_PreManacalon3");

        internal static ItemDefinition DLC1_Equipment_Document_RebelInstructions { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelInstructions");

        internal static ItemDefinition DLC1_Equipment_Document_RebelLetterToFamily1 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelLetterToFamily1");

        internal static ItemDefinition DLC1_Equipment_Document_RebelLetterToFamily2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelLetterToFamily2");

        internal static ItemDefinition DLC1_Equipment_Document_RebelMemories { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelMemories");

        internal static ItemDefinition DLC1_Equipment_Document_RebelOrders1 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelOrders1");

        internal static ItemDefinition DLC1_Equipment_Document_RebelOrders2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelOrders2");

        internal static ItemDefinition DLC1_Equipment_Document_RebelOrders3 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelOrders3");

        internal static ItemDefinition DLC1_Equipment_Document_RebelOrders4 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelOrders4");

        internal static ItemDefinition DLC1_Equipment_Document_RebelReportAboutForge { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelReportAboutForge");

        internal static ItemDefinition DLC1_Equipment_Document_RebelReportAboutMask { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_RebelReportAboutMask");

        internal static ItemDefinition DLC1_Equipment_Document_SongBook { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_SongBook");

        internal static ItemDefinition DLC1_Equipment_Document_WitchHunterAnswer { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Document_WitchHunterAnswer");

        internal static ItemDefinition DLC1_Equipment_PaganRitual { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_PaganRitual");

        internal static ItemDefinition DLC1_Equipment_Quest_Albino_Minotaur_Head { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Albino_Minotaur_Head");

        internal static ItemDefinition DLC1_Equipment_Quest_BuriedCity_Jail_Key { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_BuriedCity_Jail_Key");

        internal static ItemDefinition DLC1_Equipment_Quest_BuriedCity_Jail_Message { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_BuriedCity_Jail_Message");

        internal static ItemDefinition DLC1_Equipment_Quest_BuriedCity_LabKey { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_BuriedCity_LabKey");

        internal static ItemDefinition DLC1_Equipment_Quest_BuriedCity_Rebel_Palace_Key { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_BuriedCity_Rebel_Palace_Key");

        internal static ItemDefinition DLC1_Equipment_Quest_City_Finaliel_Lab_Key { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_City_Finaliel_Lab_Key");

        internal static ItemDefinition DLC1_Equipment_Quest_City_People_HQ_Key { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_City_People_HQ_Key");

        internal static ItemDefinition DLC1_Equipment_Quest_Complex_03_Note { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Complex_03_Note");

        internal static ItemDefinition DLC1_Equipment_Quest_Forge_01_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Forge_01_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Forge_01_Receipt { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Forge_01_Receipt");

        internal static ItemDefinition DLC1_Equipment_Quest_Forge_02_Ingredient { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Forge_02_Ingredient");

        internal static ItemDefinition DLC1_Equipment_Quest_Forge_04_Ingredient { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Forge_04_Ingredient");

        internal static ItemDefinition DLC1_Equipment_Quest_Forge_04_Letter { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Forge_04_Letter");

        internal static ItemDefinition DLC1_Equipment_Quest_Giant_01_Feather_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Giant_01_Feather_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Giant_01_Pelt_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Giant_01_Pelt_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Giant_02_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Giant_02_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Giant_02_Thread_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Giant_02_Thread_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Giant_Ape_Head { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Giant_Ape_Head");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Antique { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Antique");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Antique02 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Antique02");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Antique03 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Antique03");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Antique04 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Antique04");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Antique05 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Antique05");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Nest_Key { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Nest_Key");

        internal static ItemDefinition DLC1_Equipment_Quest_Marches_Pelt { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Marches_Pelt");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_01_Amulet { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_01_Amulet");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_01_Note { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_01_Note");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_03_Letter { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_03_Letter");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_04_Guard_Report { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_04_Guard_Report");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_04_Guard_Report_Document { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_04_Guard_Report_Document");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_05_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_05_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_06_Mask_Banner { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_06_Mask_Banner");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_06_Rebellion_Insignia { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_06_Rebellion_Insignia");

        internal static ItemDefinition DLC1_Equipment_Quest_Mask_07_Incriminating_Documents { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mask_07_Incriminating_Documents");

        internal static ItemDefinition DLC1_Equipment_Quest_Mutant_Bulette_Head { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Mutant_Bulette_Head");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_02_Ingredient { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_02_Ingredient");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_02_List { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_02_List");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_02_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_02_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_03_Ingredient_2 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_03_Ingredient_2");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_03_Ingredient_3 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_03_Ingredient_3");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_04_Notes { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_04_Notes");

        internal static ItemDefinition DLC1_Equipment_Quest_Orenetis_08_Rebel_Access { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Orenetis_08_Rebel_Access");

        internal static ItemDefinition DLC1_Equipment_Quest_People_02_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_People_02_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_Rebellion_01_Pickpocketable { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Rebellion_01_Pickpocketable");

        internal static ItemDefinition DLC1_Equipment_Quest_Rebellion_02_Package { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Rebellion_02_Package");

        internal static ItemDefinition DLC1_Equipment_Quest_ShamblingMoundHead { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_ShamblingMoundHead");

        internal static ItemDefinition DLC1_Equipment_Quest_Smart_Giant_Head { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Smart_Giant_Head");

        internal static ItemDefinition DLC1_Equipment_Quest_Swamp_LoveLetter { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Swamp_LoveLetter");

        internal static ItemDefinition DLC1_Equipment_Quest_Swamp_PrisonKey { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Swamp_PrisonKey");

        internal static ItemDefinition DLC1_Equipment_Quest_Swamp_Queen_Dryad_Head { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Swamp_Queen_Dryad_Head");

        internal static ItemDefinition DLC1_Equipment_Quest_Swamp_RuganKey { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Swamp_RuganKey");

        internal static ItemDefinition DLC1_Equipment_Quest_Valley_Beetle_Key_01 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Valley_Beetle_Key_01");

        internal static ItemDefinition DLC1_Equipment_Quest_Valley_Beetle_Key_02 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Valley_Beetle_Key_02");

        internal static ItemDefinition DLC1_Equipment_Quest_Valley_Beetle_Key_03 { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Valley_Beetle_Key_03");

        internal static ItemDefinition DLC1_Equipment_Quest_Vip_Green_Book { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_Quest_Vip_Green_Book");

        internal static ItemDefinition DLC1_Equipment_TrollHuntersNotebook { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_TrollHuntersNotebook");

        internal static ItemDefinition DLC1_Equipment_WitchHunterletter { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_WitchHunterletter");

        internal static ItemDefinition DLC1_Equipment_WitchLedger { get; } =
            GetDefinition<ItemDefinition>("DLC1_Equipment_WitchLedger");

        internal static ItemDefinition DLC1_Ingredient_GiantCrow_Feather { get; } =
            GetDefinition<ItemDefinition>("DLC1_Ingredient_GiantCrow_Feather");

        internal static ItemDefinition DLC1_Ingredient_Sewing_Thread_Ball { get; } =
            GetDefinition<ItemDefinition>("DLC1_Ingredient_Sewing_Thread_Ball");

        internal static ItemDefinition DLC1_Item_Charmer_Ring { get; } =
            GetDefinition<ItemDefinition>("DLC1_Item_Charmer_Ring");

        internal static ItemDefinition DLC1_Item_Stolen_Officer_NonQuest { get; } =
            GetDefinition<ItemDefinition>("DLC1_Item_Stolen_Officer_NonQuest");

        internal static ItemDefinition DLC1_Item_Stolen_Officer_Ring { get; } =
            GetDefinition<ItemDefinition>("DLC1_Item_Stolen_Officer_Ring");

        internal static ItemDefinition DLC3_DemonGrease_DawnBreak { get; } =
            GetDefinition<ItemDefinition>("DLC3_DemonGrease_DawnBreak");

        internal static ItemDefinition DLC3_DemonGrease_FiendSlaying { get; } =
            GetDefinition<ItemDefinition>("DLC3_DemonGrease_FiendSlaying");

        internal static ItemDefinition DLC3_DemonGrease_NightHunt { get; } =
            GetDefinition<ItemDefinition>("DLC3_DemonGrease_NightHunt");

        internal static ItemDefinition DLC3_DemonGrease_PseudoLife { get; } =
            GetDefinition<ItemDefinition>("DLC3_DemonGrease_PseudoLife");

        internal static ItemDefinition DLC3_DemonGrease_SpellTaint { get; } =
            GetDefinition<ItemDefinition>("DLC3_DemonGrease_SpellTaint");

        internal static ItemDefinition DLC3_DemonGrease_TrueStrike { get; } =
            GetDefinition<ItemDefinition>("DLC3_DemonGrease_TrueStrike");

        internal static ItemDefinition DLC3_Demonic_Dagger_Of_Soul_Hunting { get; } =
            GetDefinition<ItemDefinition>("DLC3_Demonic_Dagger_Of_Soul_Hunting");

        internal static ItemDefinition DLC3_Dwarven_Weapon_BattleaxePlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Battleaxe+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_DaggerPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Dagger+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_GreataxePlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Greataxe+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_HeavyCrossbowPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_HeavyCrossbow+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_LightCrossbowPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_LightCrossbow+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_LongswordPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Longsword+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_QuarterstaffPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Quarterstaff+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_ShortswordPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Shortsword+3");

        internal static ItemDefinition DLC3_Dwarven_Weapon_WarhammerPlus3 { get; } =
            GetDefinition<ItemDefinition>("DLC3_Dwarven_Weapon_Warhammer+3");

        internal static ItemDefinition DLC3_ElvenSettlementHighDistrict_Key { get; } =
            GetDefinition<ItemDefinition>("DLC3_ElvenSettlementHighDistrict_Key");

        internal static ItemDefinition DLC3_ElvenSettlementRuins_Document { get; } =
            GetDefinition<ItemDefinition>("DLC3_ElvenSettlementRuins_Document");

        internal static ItemDefinition DLC3_ElvenSettlementRuins_Key { get; } =
            GetDefinition<ItemDefinition>("DLC3_ElvenSettlementRuins_Key");

        internal static ItemDefinition DLC3_Equipment_Kaupaa_Necropolis_Skeleton_Key { get; } =
            GetDefinition<ItemDefinition>("DLC3_Equipment_Kaupaa_Necropolis_Skeleton_Key");

        internal static ItemDefinition DLC3_Equipment_WhiteCity_Marduk_Ring { get; } =
            GetDefinition<ItemDefinition>("DLC3_Equipment_WhiteCity_Marduk_Ring");

        internal static ItemDefinition DLC3_GarradsCastle_ItemClue1 { get; } =
            GetDefinition<ItemDefinition>("DLC3_GarradsCastle_ItemClue1");

        internal static ItemDefinition DLC3_Kaupaa_Kaikonnen_Amulet_WoodBox { get; } =
            GetDefinition<ItemDefinition>("DLC3_Kaupaa_Kaikonnen_Amulet_WoodBox");

        internal static ItemDefinition DLC3_LeralynRing { get; } = GetDefinition<ItemDefinition>("DLC3_LeralynRing");

        internal static ItemDefinition DLC3_Magic_Morningstar_Guardian_Complete { get; } =
            GetDefinition<ItemDefinition>("DLC3_Magic_Morningstar_Guardian_Complete");

        internal static ItemDefinition DLC3_Magic_Shortsword_Sovereign { get; } =
            GetDefinition<ItemDefinition>("DLC3_Magic_Shortsword_Sovereign");

        internal static ItemDefinition DLC3_Telema_CustomGateInstructions { get; } =
            GetDefinition<ItemDefinition>("DLC3_Telema_CustomGateInstructions");

        internal static ItemDefinition DLC3_Telema_CustomGateManual { get; } =
            GetDefinition<ItemDefinition>("DLC3_Telema_CustomGateManual");

        internal static ItemDefinition DLC3_Telema_HectorsDiary { get; } =
            GetDefinition<ItemDefinition>("DLC3_Telema_HectorsDiary");

        internal static ItemDefinition DLC3_Telema_PyramidKey { get; } =
            GetDefinition<ItemDefinition>("DLC3_Telema_PyramidKey");

        internal static ItemDefinition DLC3_Telema_RoundKey { get; } =
            GetDefinition<ItemDefinition>("DLC3_Telema_RoundKey");

        internal static ItemDefinition DLC3_Telema_SessrothReport { get; } =
            GetDefinition<ItemDefinition>("DLC3_Telema_SessrothReport");

        internal static ItemDefinition DLC3_Undermountain_BrigandsJournal { get; } =
            GetDefinition<ItemDefinition>("DLC3_Undermountain_BrigandsJournal");

        internal static ItemDefinition DLC3_Undermountain_HeatGenerator_Instructions { get; } =
            GetDefinition<ItemDefinition>("DLC3_Undermountain_HeatGenerator_Instructions");

        internal static ItemDefinition DLC3_WhiteCity_FrozenQuest_Document_Lore_Dwarf_Note { get; } =
            GetDefinition<ItemDefinition>("DLC3_WhiteCity_FrozenQuest_Document_Lore_Dwarf_Note");

        internal static ItemDefinition DLC3_WhiteCity_Giant_Clan_Trophy { get; } =
            GetDefinition<ItemDefinition>("DLC3_WhiteCity_Giant_Clan_Trophy");

        internal static ItemDefinition DLC3_WhiteCity_Unusual_Rock_Fragment { get; } =
            GetDefinition<ItemDefinition>("DLC3_WhiteCity_Unusual_Rock_Fragment");

        internal static ItemDefinition Document_HalmanSummer_Conspiration_Docs { get; } =
            GetDefinition<ItemDefinition>("Document_HalmanSummer_Conspiration_Docs");

        internal static ItemDefinition Document_HalmanSummer_Letter { get; } =
            GetDefinition<ItemDefinition>("Document_HalmanSummer_Letter");

        internal static ItemDefinition DruidicFocus { get; } = GetDefinition<ItemDefinition>("DruidicFocus");
        internal static ItemDefinition Drum { get; } = GetDefinition<ItemDefinition>("Drum");
        internal static ItemDefinition Dryad_Spine { get; } = GetDefinition<ItemDefinition>("Dryad_Spine");
        internal static ItemDefinition Dulcimer { get; } = GetDefinition<ItemDefinition>("Dulcimer");
        internal static ItemDefinition DungeoneerPack { get; } = GetDefinition<ItemDefinition>("DungeoneerPack");

        internal static ItemDefinition DustOfDisappearance { get; } =
            GetDefinition<ItemDefinition>("DustOfDisappearance");

        internal static ItemDefinition DustOfDisappearance_Pre_Identified_NotForDM { get; } =
            GetDefinition<ItemDefinition>("DustOfDisappearance_Pre_Identified_NotForDM");

        internal static ItemDefinition DwarfBread { get; } = GetDefinition<ItemDefinition>("DwarfBread");
        internal static ItemDefinition Dwarven_Plate { get; } = GetDefinition<ItemDefinition>("Dwarven_Plate");
        internal static ItemDefinition DwarvenThrower { get; } = GetDefinition<ItemDefinition>("DwarvenThrower");
        internal static ItemDefinition DwarvenWarDrums { get; } = GetDefinition<ItemDefinition>("DwarvenWarDrums");
        internal static ItemDefinition EarlyBird { get; } = GetDefinition<ItemDefinition>("EarlyBird");
        internal static ItemDefinition EfficientQuiver { get; } = GetDefinition<ItemDefinition>("EfficientQuiver");
        internal static ItemDefinition ElvenChain { get; } = GetDefinition<ItemDefinition>("ElvenChain");
        internal static ItemDefinition ElvenThinbladePlus1 { get; } = GetDefinition<ItemDefinition>("ElvenThinblade+1");

        internal static ItemDefinition Enchanted_Battleaxe_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Battleaxe_Of_Acuteness");

        internal static ItemDefinition Enchanted_Battleaxe_Of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Battleaxe_Of_Sharpness");

        internal static ItemDefinition Enchanted_Battleaxe_Punisher { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Battleaxe_Punisher");

        internal static ItemDefinition Enchanted_BreastplateOfDeflection { get; } =
            GetDefinition<ItemDefinition>("Enchanted_BreastplateOfDeflection");

        internal static ItemDefinition Enchanted_BreastplateOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_BreastplateOfSturdiness");

        internal static ItemDefinition Enchanted_ChainmailOfRobustness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ChainmailOfRobustness");

        internal static ItemDefinition Enchanted_ChainmailOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ChainmailOfSturdiness");

        internal static ItemDefinition Enchanted_ChainShirt_Empress_war_garb { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ChainShirt_Empress_war_garb");

        internal static ItemDefinition Enchanted_Dagger_EldritchWounds { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Dagger_EldritchWounds");

        internal static ItemDefinition Enchanted_Dagger_Frostburn { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Dagger_Frostburn");

        internal static ItemDefinition Enchanted_Dagger_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Dagger_of_Acuteness");

        internal static ItemDefinition Enchanted_Dagger_of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Dagger_of_Sharpness");

        internal static ItemDefinition Enchanted_Dagger_Souldrinker { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Dagger_Souldrinker");

        internal static ItemDefinition Enchanted_Druid_Armor_Of_The_Forest { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Druid_Armor_Of_The_Forest");

        internal static ItemDefinition Enchanted_Druid_Armor_Of_The_Primal_Oak { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Druid_Armor_Of_The_Primal_Oak");

        internal static ItemDefinition Enchanted_Gauntlet_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Gauntlet_Of_Acuteness");

        internal static ItemDefinition Enchanted_Gauntlet_Of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Gauntlet_Of_Sharpness");

        internal static ItemDefinition Enchanted_Greataxe_Of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Greataxe_Of_Sharpness");

        internal static ItemDefinition Enchanted_Greataxe_Stormblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Greataxe_Stormblade");

        internal static ItemDefinition Enchanted_Greatsword_Doomblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Greatsword_Doomblade");

        internal static ItemDefinition Enchanted_Greatsword_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Greatsword_Lightbringer");

        internal static ItemDefinition Enchanted_Greatsword_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Greatsword_of_Acuteness");

        internal static ItemDefinition Enchanted_HalfPlateOfRobustness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HalfPlateOfRobustness");

        internal static ItemDefinition Enchanted_HalfPlateOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HalfPlateOfSturdiness");

        internal static ItemDefinition Enchanted_HeavyCrossbow_of_Accuracy { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HeavyCrossbow_of_Accuracy");

        internal static ItemDefinition Enchanted_HeavyCrossbow_Whiteburn { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HeavyCrossbow_Whiteburn");

        internal static ItemDefinition Enchanted_HideArmor_Of_The_Vagrant { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HideArmor_Of_The_Vagrant");

        internal static ItemDefinition Enchanted_HideArmor_Of_Wilderness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HideArmor_Of_Wilderness");

        internal static ItemDefinition Enchanted_LeatherArmorOfFlameDancing { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LeatherArmorOfFlameDancing");

        internal static ItemDefinition Enchanted_LeatherArmorOfRobustness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LeatherArmorOfRobustness");

        internal static ItemDefinition Enchanted_LeatherArmorOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LeatherArmorOfSturdiness");

        internal static ItemDefinition Enchanted_LeatherArmorOfSurvival { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LeatherArmorOfSurvival");

        internal static ItemDefinition Enchanted_LightCrossbow_of_Accuracy { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LightCrossbow_of_Accuracy");

        internal static ItemDefinition Enchanted_LightCrossbow_Souldrinker { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LightCrossbow_Souldrinker");

        internal static ItemDefinition Enchanted_Longbow_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longbow_Lightbringer");

        internal static ItemDefinition Enchanted_Longbow_Of_Accurary { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longbow_Of_Accurary");

        internal static ItemDefinition Enchanted_Longbow_Stormbow { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longbow_Stormbow");

        internal static ItemDefinition Enchanted_Longsword_Dragonblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_Dragonblade");

        internal static ItemDefinition Enchanted_Longsword_Frostburn { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_Frostburn");

        internal static ItemDefinition Enchanted_Longsword_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_of_Acuteness");

        internal static ItemDefinition Enchanted_Longsword_Stormblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_Stormblade");

        internal static ItemDefinition Enchanted_Longsword_Warden { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_Warden");

        internal static ItemDefinition Enchanted_Mace_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Mace_Of_Acuteness");

        internal static ItemDefinition Enchanted_Mace_Of_Smashing { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Mace_Of_Smashing");

        internal static ItemDefinition Enchanted_Maul_Dragonclaw { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Maul_Dragonclaw");

        internal static ItemDefinition Enchanted_Maul_of_Smashing { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Maul_of_Smashing");

        internal static ItemDefinition Enchanted_Maul_Of_The_Destroyer { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Maul_Of_The_Destroyer");

        internal static ItemDefinition Enchanted_Morningstar_Bearclaw { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Morningstar_Bearclaw");

        internal static ItemDefinition Enchanted_Morningstar_Of_Power { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Morningstar_Of_Power");

        internal static ItemDefinition Enchanted_PlateOfRobustness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_PlateOfRobustness");

        internal static ItemDefinition Enchanted_PlateOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_PlateOfSturdiness");

        internal static ItemDefinition Enchanted_Rapier_Blackadder { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Blackadder");

        internal static ItemDefinition Enchanted_Rapier_Doomblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Doomblade");

        internal static ItemDefinition Enchanted_Rapier_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Of_Acuteness");

        internal static ItemDefinition Enchanted_Rapier_Of_Harmony { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Of_Harmony");

        internal static ItemDefinition Enchanted_ScaleMailOfIceDancing { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ScaleMailOfIceDancing");

        internal static ItemDefinition Enchanted_ScaleMailOfRobustness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ScaleMailOfRobustness");

        internal static ItemDefinition Enchanted_ScaleMailOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ScaleMailOfSturdiness");

        internal static ItemDefinition Enchanted_Scimitar_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Scimitar_Of_Acuteness");

        internal static ItemDefinition Enchanted_Scimitar_Of_Speed { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Scimitar_Of_Speed");

        internal static ItemDefinition Enchanted_ScimitarOfTheAnfarels { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ScimitarOfTheAnfarels");

        internal static ItemDefinition Enchanted_Shortbow_Medusa { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortbow_Medusa");

        internal static ItemDefinition Enchanted_Shortbow_Of_Accuracy { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortbow_Of_Accuracy");

        internal static ItemDefinition Enchanted_Shortbow_Of_Sharpshooting { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortbow_Of_Sharpshooting");

        internal static ItemDefinition Enchanted_Shortsword_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_Lightbringer");

        internal static ItemDefinition Enchanted_Shortsword_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_Of_Acuteness");

        internal static ItemDefinition Enchanted_Shortsword_of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_of_Sharpness");

        internal static ItemDefinition Enchanted_Shortsword_Whiteburn { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_Whiteburn");

        internal static ItemDefinition Enchanted_SpearDoomSpear { get; } =
            GetDefinition<ItemDefinition>("Enchanted_SpearDoomSpear");

        internal static ItemDefinition Enchanted_SpearOfAcuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_SpearOfAcuteness");

        internal static ItemDefinition Enchanted_StuddedLeatherOfLeadership { get; } =
            GetDefinition<ItemDefinition>("Enchanted_StuddedLeatherOfLeadership");

        internal static ItemDefinition Enchanted_StuddedLeatherOfSurvival { get; } =
            GetDefinition<ItemDefinition>("Enchanted_StuddedLeatherOfSurvival");

        internal static ItemDefinition Enchanted_Warhammer_of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Warhammer_of_Acuteness");

        internal static ItemDefinition Enchanted_Warhammer_Stormbinder { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Warhammer_Stormbinder");

        internal static ItemDefinition EnchantingTool { get; } = GetDefinition<ItemDefinition>("EnchantingTool");
        internal static ItemDefinition EndlessQuiver { get; } = GetDefinition<ItemDefinition>("EndlessQuiver");
        internal static ItemDefinition EntertainerPack { get; } = GetDefinition<ItemDefinition>("EntertainerPack");
        internal static ItemDefinition EverlastingTorch { get; } = GetDefinition<ItemDefinition>("EverlastingTorch");

        internal static ItemDefinition Evocation_Cradle_Key { get; } =
            GetDefinition<ItemDefinition>("Evocation_Cradle_Key");

        internal static ItemDefinition EVOCATION_SecretSpellBook4 { get; } =
            GetDefinition<ItemDefinition>("EVOCATION_SecretSpellBook4");

        internal static ItemDefinition ExplorerPack { get; } = GetDefinition<ItemDefinition>("ExplorerPack");
        internal static ItemDefinition FireArrow { get; } = GetDefinition<ItemDefinition>("FireArrow");
        internal static ItemDefinition FlameBlade { get; } = GetDefinition<ItemDefinition>("FlameBlade");
        internal static ItemDefinition Flute { get; } = GetDefinition<ItemDefinition>("Flute");
        internal static ItemDefinition FluteOfRespite { get; } = GetDefinition<ItemDefinition>("FluteOfRespite");
        internal static ItemDefinition Food_Ration { get; } = GetDefinition<ItemDefinition>("Food_Ration");

        internal static ItemDefinition Food_Ration_Created { get; } =
            GetDefinition<ItemDefinition>("Food_Ration_Created");

        internal static ItemDefinition Food_Ration_Foraged { get; } =
            GetDefinition<ItemDefinition>("Food_Ration_Foraged");

        internal static ItemDefinition GauntletsOfOgrePower { get; } =
            GetDefinition<ItemDefinition>("GauntletsOfOgrePower");

        internal static ItemDefinition GemOfSeeing { get; } = GetDefinition<ItemDefinition>("GemOfSeeing");
        internal static ItemDefinition Giant_Greataxe { get; } = GetDefinition<ItemDefinition>("Giant_Greataxe");
        internal static ItemDefinition Giant_Rock { get; } = GetDefinition<ItemDefinition>("Giant_Rock");

        internal static ItemDefinition GlovesOfMissileSnaring { get; } =
            GetDefinition<ItemDefinition>("GlovesOfMissileSnaring");

        internal static ItemDefinition Greataxe { get; } = GetDefinition<ItemDefinition>("Greataxe");

        internal static ItemDefinition Greataxe_Orc_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("Greataxe_Orc_Duel_Autoequip");

        internal static ItemDefinition GreataxePlus1 { get; } = GetDefinition<ItemDefinition>("Greataxe+1");
        internal static ItemDefinition GreataxePlus2 { get; } = GetDefinition<ItemDefinition>("Greataxe+2");
        internal static ItemDefinition Greatsword { get; } = GetDefinition<ItemDefinition>("Greatsword");
        internal static ItemDefinition GreatswordPlus1 { get; } = GetDefinition<ItemDefinition>("Greatsword+1");
        internal static ItemDefinition GreenmageArmor { get; } = GetDefinition<ItemDefinition>("GreenmageArmor");
        internal static ItemDefinition HalfPlate { get; } = GetDefinition<ItemDefinition>("HalfPlate");
        internal static ItemDefinition HalfPlatePlus1 { get; } = GetDefinition<ItemDefinition>("HalfPlate+1");

        internal static ItemDefinition HalfPlatePlus1_Aksha { get; } =
            GetDefinition<ItemDefinition>("HalfPlate+1_Aksha");

        internal static ItemDefinition Handaxe { get; } = GetDefinition<ItemDefinition>("Handaxe");
        internal static ItemDefinition HandaxePlus1 { get; } = GetDefinition<ItemDefinition>("Handaxe+1");

        internal static ItemDefinition HeadbandOfIntellect { get; } =
            GetDefinition<ItemDefinition>("HeadbandOfIntellect");

        internal static ItemDefinition HeavyCrossbow { get; } = GetDefinition<ItemDefinition>("HeavyCrossbow");
        internal static ItemDefinition HeavyCrossbowPlus1 { get; } = GetDefinition<ItemDefinition>("HeavyCrossbow+1");
        internal static ItemDefinition HeavyCrossbowPlus2 { get; } = GetDefinition<ItemDefinition>("HeavyCrossbow+2");

        internal static ItemDefinition HelmOfComprehendingLanguages { get; } =
            GetDefinition<ItemDefinition>("HelmOfComprehendingLanguages");

        internal static ItemDefinition HerbalismKit { get; } = GetDefinition<ItemDefinition>("HerbalismKit");
        internal static ItemDefinition HideArmor { get; } = GetDefinition<ItemDefinition>("HideArmor");

        internal static ItemDefinition HideArmor_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("HideArmor_Duel_Autoequip");

        internal static ItemDefinition HideArmor_plus_one { get; } =
            GetDefinition<ItemDefinition>("HideArmor_plus_one");

        internal static ItemDefinition HideArmor_plus_two { get; } =
            GetDefinition<ItemDefinition>("HideArmor_plus_two");

        internal static ItemDefinition HolySymbolAmulet { get; } = GetDefinition<ItemDefinition>("HolySymbolAmulet");
        internal static ItemDefinition HolySymbolBelt { get; } = GetDefinition<ItemDefinition>("HolySymbolBelt");
        internal static ItemDefinition HolySymbolCape { get; } = GetDefinition<ItemDefinition>("HolySymbolCape");
        internal static ItemDefinition HolySymbolCrown { get; } = GetDefinition<ItemDefinition>("HolySymbolCrown");
        internal static ItemDefinition Horn { get; } = GetDefinition<ItemDefinition>("Horn");
        internal static ItemDefinition HornOfBlasting { get; } = GetDefinition<ItemDefinition>("HornOfBlasting");

        internal static ItemDefinition Ingredient_AbyssMoss { get; } =
            GetDefinition<ItemDefinition>("Ingredient_AbyssMoss");

        internal static ItemDefinition Ingredient_Acid { get; } = GetDefinition<ItemDefinition>("Ingredient_Acid");

        internal static ItemDefinition Ingredient_AngryViolet { get; } =
            GetDefinition<ItemDefinition>("Ingredient_AngryViolet");

        internal static ItemDefinition Ingredient_Badlands_Ape_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Badlands_Ape_Pelt");

        internal static ItemDefinition Ingredient_BadlandsBear_Meat { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BadlandsBear_Meat");

        internal static ItemDefinition Ingredient_BadlandsBear_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BadlandsBear_Pelt");

        internal static ItemDefinition Ingredient_BadlandsDryadBark { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BadlandsDryadBark");

        internal static ItemDefinition Ingredient_BadlandsSpiderVenomGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BadlandsSpiderVenomGland");

        internal static ItemDefinition Ingredient_BlackBear_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BlackBear_Pelt");

        internal static ItemDefinition Ingredient_BloodDaffodil { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BloodDaffodil");

        internal static ItemDefinition Ingredient_BrimstoneViperPoisonGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BrimstoneViperPoisonGland");

        internal static ItemDefinition Ingredient_BrimstoneViperScales { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BrimstoneViperScales");

        internal static ItemDefinition Ingredient_BrownBear_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BrownBear_Pelt");

        internal static ItemDefinition Ingredient_Bulette_Teeth { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Bulette_Teeth");

        internal static ItemDefinition Ingredient_CrimsonSpiderVenomGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_CrimsonSpiderVenomGland");

        internal static ItemDefinition Ingredient_DeepRootLichen { get; } =
            GetDefinition<ItemDefinition>("Ingredient_DeepRootLichen");

        internal static ItemDefinition Ingredient_DeepSpiderVenomGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_DeepSpiderVenomGland");

        internal static ItemDefinition Ingredient_DireWolf_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_DireWolf_Pelt");

        internal static ItemDefinition Ingredient_Dragonrose { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Dragonrose");

        internal static ItemDefinition Ingredient_DwarvenPrimrose { get; } =
            GetDefinition<ItemDefinition>("Ingredient_DwarvenPrimrose");

        internal static ItemDefinition Ingredient_Emperorseye { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Emperorseye");

        internal static ItemDefinition Ingredient_Enchant_Blood_Gem { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Blood_Gem");

        internal static ItemDefinition Ingredient_Enchant_Blood_Of_Solasta { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Blood_Of_Solasta");

        internal static ItemDefinition Ingredient_Enchant_Cloud_Diamond { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Cloud_Diamond");

        internal static ItemDefinition Ingredient_Enchant_Crystal_Of_Winter { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Crystal_Of_Winter");

        internal static ItemDefinition Ingredient_Enchant_Demonic_Essence { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Demonic_Essence");

        internal static ItemDefinition Ingredient_Enchant_Diamond_Of_Elai { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Diamond_Of_Elai");

        internal static ItemDefinition Ingredient_Enchant_Doom_Gem { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Doom_Gem");

        internal static ItemDefinition Ingredient_Enchant_Heartstone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Heartstone");

        internal static ItemDefinition Ingredient_Enchant_LifeStone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_LifeStone");

        internal static ItemDefinition Ingredient_Enchant_Medusa_Coral { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Medusa_Coral");

        internal static ItemDefinition Ingredient_Enchant_MithralStone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_MithralStone");

        internal static ItemDefinition Ingredient_Enchant_Oil_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Oil_Of_Acuteness");

        internal static ItemDefinition Ingredient_Enchant_PrimordialCrystal { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_PrimordialCrystal");

        internal static ItemDefinition Ingredient_Enchant_PurpleAmber { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_PurpleAmber");

        internal static ItemDefinition Ingredient_Enchant_RiverEmerald { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_RiverEmerald");

        internal static ItemDefinition Ingredient_Enchant_Shard_Of_Fire { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Shard_Of_Fire");

        internal static ItemDefinition Ingredient_Enchant_Shard_Of_Ice { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Shard_Of_Ice");

        internal static ItemDefinition Ingredient_Enchant_Shard_Of_Ice_Minor { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Shard_Of_Ice_Minor");

        internal static ItemDefinition Ingredient_Enchant_Slavestone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Slavestone");

        internal static ItemDefinition Ingredient_Enchant_Soul_Gem { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Soul_Gem");

        internal static ItemDefinition Ingredient_Enchant_SpiderQueen_Venom { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_SpiderQueen_Venom");

        internal static ItemDefinition Ingredient_Enchant_Stardust { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Stardust");

        internal static ItemDefinition Ingredient_Enchant_SwampOpal { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_SwampOpal");

        internal static ItemDefinition Ingredient_Enchant_TearOfTirmar { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_TearOfTirmar");

        internal static ItemDefinition Ingredient_Enchant_TrollHeart { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_TrollHeart");

        internal static ItemDefinition Ingredient_FireSpiderVenomGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_FireSpiderVenomGland");

        internal static ItemDefinition Ingredient_GallivanAmaranth { get; } =
            GetDefinition<ItemDefinition>("Ingredient_GallivanAmaranth");

        internal static ItemDefinition Ingredient_Giant_Ape_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Giant_Ape_Pelt");

        internal static ItemDefinition Ingredient_GiantBeetleElytron { get; } =
            GetDefinition<ItemDefinition>("Ingredient_GiantBeetleElytron");

        internal static ItemDefinition Ingredient_GoblinHairFungus { get; } =
            GetDefinition<ItemDefinition>("Ingredient_GoblinHairFungus");

        internal static ItemDefinition Ingredient_Gorilla_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Gorilla_Pelt");

        internal static ItemDefinition Ingredient_LilyOfTheBadlands { get; } =
            GetDefinition<ItemDefinition>("Ingredient_LilyOfTheBadlands");

        internal static ItemDefinition Ingredient_Magnesium { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Magnesium");

        internal static ItemDefinition Ingredient_ManacalonOrchid { get; } =
            GetDefinition<ItemDefinition>("Ingredient_ManacalonOrchid");

        internal static ItemDefinition Ingredient_Moonflower { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Moonflower");

        internal static ItemDefinition Ingredient_Mutant_DireWolf_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Mutant_DireWolf_Pelt");

        internal static ItemDefinition Ingredient_PolarBear_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_PolarBear_Pelt");

        internal static ItemDefinition Ingredient_PrimordialDragonstone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_PrimordialDragonstone");

        internal static ItemDefinition Ingredient_PrimordialLavaStones { get; } =
            GetDefinition<ItemDefinition>("Ingredient_PrimordialLavaStones");

        internal static ItemDefinition Ingredient_Projectile_Parts { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Projectile_Parts");

        internal static ItemDefinition Ingredient_QueenDryadBark { get; } =
            GetDefinition<ItemDefinition>("Ingredient_QueenDryadBark");

        internal static ItemDefinition Ingredient_QueenIvy { get; } =
            GetDefinition<ItemDefinition>("Ingredient_QueenIvy");

        internal static ItemDefinition Ingredient_RefinedOil { get; } =
            GetDefinition<ItemDefinition>("Ingredient_RefinedOil");

        internal static ItemDefinition Ingredient_Serpentinite { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Serpentinite");

        internal static ItemDefinition Ingredient_Skarn { get; } = GetDefinition<ItemDefinition>("Ingredient_Skarn");

        internal static ItemDefinition Ingredient_Sorak_Poison_Spine { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Sorak_Poison_Spine");

        internal static ItemDefinition Ingredient_Sovereign_Stone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Sovereign_Stone");

        internal static ItemDefinition Ingredient_SpiderQueenVenomGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_SpiderQueenVenomGland");

        internal static ItemDefinition Ingredient_StormHeather { get; } =
            GetDefinition<ItemDefinition>("Ingredient_StormHeather");

        internal static ItemDefinition Ingredient_TigerDrake_Fang { get; } =
            GetDefinition<ItemDefinition>("Ingredient_TigerDrake_Fang");

        internal static ItemDefinition Ingredient_TigerDrakeScales { get; } =
            GetDefinition<ItemDefinition>("Ingredient_TigerDrakeScales");

        internal static ItemDefinition Ingredient_TrollAcanthus { get; } =
            GetDefinition<ItemDefinition>("Ingredient_TrollAcanthus");

        internal static ItemDefinition Ingredient_TrollsTongue { get; } =
            GetDefinition<ItemDefinition>("Ingredient_TrollsTongue");

        internal static ItemDefinition Ingredient_WinterWolf_Pelt { get; } =
            GetDefinition<ItemDefinition>("Ingredient_WinterWolf_Pelt");

        internal static ItemDefinition Javelin { get; } = GetDefinition<ItemDefinition>("Javelin");
        internal static ItemDefinition JavelinPlus1 { get; } = GetDefinition<ItemDefinition>("Javelin+1");
        internal static ItemDefinition LavaBlast { get; } = GetDefinition<ItemDefinition>("LavaBlast");
        internal static ItemDefinition Leather { get; } = GetDefinition<ItemDefinition>("Leather");
        internal static ItemDefinition LeatherArmorPlus1 { get; } = GetDefinition<ItemDefinition>("LeatherArmor+1");
        internal static ItemDefinition LeatherArmorPlus2 { get; } = GetDefinition<ItemDefinition>("LeatherArmor+2");
        internal static ItemDefinition LeatherDruid { get; } = GetDefinition<ItemDefinition>("LeatherDruid");

        internal static ItemDefinition LEGENDARY_Document { get; } =
            GetDefinition<ItemDefinition>("LEGENDARY_Document");

        internal static ItemDefinition LightCrossbow { get; } = GetDefinition<ItemDefinition>("LightCrossbow");
        internal static ItemDefinition LightCrossbowPlus1 { get; } = GetDefinition<ItemDefinition>("LightCrossbow+1");
        internal static ItemDefinition LightCrossbowPlus2 { get; } = GetDefinition<ItemDefinition>("LightCrossbow+2");
        internal static ItemDefinition LoadedDice { get; } = GetDefinition<ItemDefinition>("LoadedDice");
        internal static ItemDefinition Longbow { get; } = GetDefinition<ItemDefinition>("Longbow");
        internal static ItemDefinition LongbowPlus1 { get; } = GetDefinition<ItemDefinition>("Longbow+1");
        internal static ItemDefinition LongbowPlus2 { get; } = GetDefinition<ItemDefinition>("Longbow+2");
        internal static ItemDefinition Longsword { get; } = GetDefinition<ItemDefinition>("Longsword");
        internal static ItemDefinition LongswordPlus1 { get; } = GetDefinition<ItemDefinition>("Longsword+1");
        internal static ItemDefinition LongswordPlus2 { get; } = GetDefinition<ItemDefinition>("Longsword+2");

        internal static ItemDefinition Lowlife_Quest_Document_Thief_Note { get; } =
            GetDefinition<ItemDefinition>("Lowlife_Quest_Document_Thief_Note");

        internal static ItemDefinition Lowlife_Quest_MagicalSword { get; } =
            GetDefinition<ItemDefinition>("Lowlife_Quest_MagicalSword");

        internal static ItemDefinition Lute { get; } = GetDefinition<ItemDefinition>("Lute");
        internal static ItemDefinition Mace { get; } = GetDefinition<ItemDefinition>("Mace");
        internal static ItemDefinition MacePlus1 { get; } = GetDefinition<ItemDefinition>("Mace+1");
        internal static ItemDefinition MacePlus2 { get; } = GetDefinition<ItemDefinition>("Mace+2");

        internal static ItemDefinition Magic_Dagger_Cheater { get; } =
            GetDefinition<ItemDefinition>("Magic_Dagger_Cheater");

        internal static ItemDefinition Magic_Dagger_Feybane { get; } =
            GetDefinition<ItemDefinition>("Magic_Dagger_Feybane");

        internal static ItemDefinition Magic_Longsword_Unity { get; } =
            GetDefinition<ItemDefinition>("Magic_Longsword_Unity");

        internal static ItemDefinition Magic_Mace_Sunstar { get; } =
            GetDefinition<ItemDefinition>("Magic_Mace_Sunstar");

        internal static ItemDefinition Magic_Shortbow_Truth { get; } =
            GetDefinition<ItemDefinition>("Magic_Shortbow_Truth");

        internal static ItemDefinition Magister_Dragon_Tooth { get; } =
            GetDefinition<ItemDefinition>("Magister_Dragon_Tooth");

        internal static ItemDefinition MantleOfSpellResistance { get; } =
            GetDefinition<ItemDefinition>("MantleOfSpellResistance");

        internal static ItemDefinition Manual_Of_Bodily_Health { get; } =
            GetDefinition<ItemDefinition>("Manual_Of_Bodily_Health");

        internal static ItemDefinition Manual_Of_Gainful_Exercise { get; } =
            GetDefinition<ItemDefinition>("Manual_Of_Gainful_Exercise");

        internal static ItemDefinition Manual_Of_Quickness_of_Action { get; } =
            GetDefinition<ItemDefinition>("Manual_Of_Quickness_of_Action");

        internal static ItemDefinition Mark_Of_The_Crown { get; } = GetDefinition<ItemDefinition>("Mark_Of_The_Crown");
        internal static ItemDefinition Maul { get; } = GetDefinition<ItemDefinition>("Maul");
        internal static ItemDefinition MaulPlus1 { get; } = GetDefinition<ItemDefinition>("Maul+1");
        internal static ItemDefinition MaulPlus2 { get; } = GetDefinition<ItemDefinition>("Maul+2");
        internal static ItemDefinition MonkArmor { get; } = GetDefinition<ItemDefinition>("MonkArmor");
        internal static ItemDefinition MonkGauntletPlus1 { get; } = GetDefinition<ItemDefinition>("MonkGauntlet+1");
        internal static ItemDefinition MonkGauntletPlus2 { get; } = GetDefinition<ItemDefinition>("MonkGauntlet+2");
        internal static ItemDefinition Morningstar { get; } = GetDefinition<ItemDefinition>("Morningstar");
        internal static ItemDefinition MorningstarPlus1 { get; } = GetDefinition<ItemDefinition>("Morningstar+1");
        internal static ItemDefinition MorningstarPlus2 { get; } = GetDefinition<ItemDefinition>("Morningstar+2");

        internal static ItemDefinition NecklaceOfFireballs { get; } =
            GetDefinition<ItemDefinition>("NecklaceOfFireballs");

        internal static ItemDefinition NecklaceOfGrounding { get; } =
            GetDefinition<ItemDefinition>("NecklaceOfGrounding");

        internal static ItemDefinition OBSOLETE_WandOfMagicMissile { get; } =
            GetDefinition<ItemDefinition>("OBSOLETE-WandOfMagicMissile");

        internal static ItemDefinition OfOrcsAndMen { get; } = GetDefinition<ItemDefinition>("OfOrcsAndMen");
        internal static ItemDefinition Ogre_Javelin { get; } = GetDefinition<ItemDefinition>("Ogre_Javelin");
        internal static ItemDefinition Ogre_Mace { get; } = GetDefinition<ItemDefinition>("Ogre_Mace");
        internal static ItemDefinition OilOfSharpness { get; } = GetDefinition<ItemDefinition>("OilOfSharpness");
        internal static ItemDefinition OneRing { get; } = GetDefinition<ItemDefinition>("OneRing");
        internal static ItemDefinition Orc_Arrow { get; } = GetDefinition<ItemDefinition>("Orc_Arrow");
        internal static ItemDefinition Orc_Greataxe { get; } = GetDefinition<ItemDefinition>("Orc_Greataxe");
        internal static ItemDefinition Orc_Javelin { get; } = GetDefinition<ItemDefinition>("Orc_Javelin");
        internal static ItemDefinition Orc_Mace { get; } = GetDefinition<ItemDefinition>("Orc_Mace");
        internal static ItemDefinition Orc_Shortbow { get; } = GetDefinition<ItemDefinition>("Orc_Shortbow");

        internal static ItemDefinition OrcGrimblade_IceDagger { get; } =
            GetDefinition<ItemDefinition>("OrcGrimblade_IceDagger");

        internal static ItemDefinition PaddedLeather { get; } = GetDefinition<ItemDefinition>("PaddedLeather");

        internal static ItemDefinition PendantOfTheHealer { get; } =
            GetDefinition<ItemDefinition>("PendantOfTheHealer");

        internal static ItemDefinition PenitentBelt { get; } = GetDefinition<ItemDefinition>("PenitentBelt");
        internal static ItemDefinition PeriaptOfHealth { get; } = GetDefinition<ItemDefinition>("PeriaptOfHealth");

        internal static ItemDefinition PeriaptOfProofAgainstPoison { get; } =
            GetDefinition<ItemDefinition>("PeriaptOfProofAgainstPoison");

        internal static ItemDefinition PeriaptOfTheMasterEnchanter { get; } =
            GetDefinition<ItemDefinition>("PeriaptOfTheMasterEnchanter");

        internal static ItemDefinition PickupQuest_Item_Aquila { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_Aquila");

        internal static ItemDefinition PickupQuest_Item_ChroniclesOfTheInquisition { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_ChroniclesOfTheInquisition");

        internal static ItemDefinition PickupQuest_Item_CrownNotes { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_CrownNotes");

        internal static ItemDefinition PickupQuest_Item_Cypher { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_Cypher");

        internal static ItemDefinition PickupQuest_Item_DanantarLetter { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_DanantarLetter");

        internal static ItemDefinition PickupQuest_Item_FirstLegionInsignia { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_FirstLegionInsignia");

        internal static ItemDefinition PickupQuest_Item_GhostsOfTheEmpire_Key { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_GhostsOfTheEmpire_Key");

        internal static ItemDefinition PickupQuest_Item_HiddenBlade { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_HiddenBlade");

        internal static ItemDefinition PickupQuest_Item_LavaForest { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_LavaForest");

        internal static ItemDefinition PickupQuest_Item_MagisterAmulet { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_MagisterAmulet");

        internal static ItemDefinition PickupQuest_Item_PaintedHide { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_PaintedHide");

        internal static ItemDefinition PickupQuest_Item_SylvanRebelJournal { get; } =
            GetDefinition<ItemDefinition>("PickupQuest_Item_SylvanRebelJournal");

        internal static ItemDefinition PipesOfHaunting { get; } = GetDefinition<ItemDefinition>("PipesOfHaunting");
        internal static ItemDefinition Plate { get; } = GetDefinition<ItemDefinition>("Plate");
        internal static ItemDefinition PlatePlus1 { get; } = GetDefinition<ItemDefinition>("Plate+1");

        internal static ItemDefinition Poison_ArivadsKiss { get; } =
            GetDefinition<ItemDefinition>("Poison_ArivadsKiss");

        internal static ItemDefinition Poison_ArunsLight { get; } = GetDefinition<ItemDefinition>("Poison_ArunsLight");
        internal static ItemDefinition Poison_Basic { get; } = GetDefinition<ItemDefinition>("Poison_Basic");

        internal static ItemDefinition Poison_BrimstoneFang { get; } =
            GetDefinition<ItemDefinition>("Poison_BrimstoneFang");

        internal static ItemDefinition Poison_DarkStab { get; } = GetDefinition<ItemDefinition>("Poison_DarkStab");

        internal static ItemDefinition Poison_Deep_Spider { get; } =
            GetDefinition<ItemDefinition>("Poison_Deep_Spider");

        internal static ItemDefinition Poison_DeepPain { get; } = GetDefinition<ItemDefinition>("Poison_DeepPain");

        internal static ItemDefinition Poison_GhoulsCaress { get; } =
            GetDefinition<ItemDefinition>("Poison_GhoulsCaress");

        internal static ItemDefinition Poison_MaraikesTorpor { get; } =
            GetDefinition<ItemDefinition>("Poison_MaraikesTorpor");

        internal static ItemDefinition Poison_Spider { get; } = GetDefinition<ItemDefinition>("Poison_Spider");

        internal static ItemDefinition Poison_SpiderQueensBlood { get; } =
            GetDefinition<ItemDefinition>("Poison_SpiderQueensBlood");

        internal static ItemDefinition Poison_TheBurden { get; } = GetDefinition<ItemDefinition>("Poison_TheBurden");

        internal static ItemDefinition Poison_TheLongNight { get; } =
            GetDefinition<ItemDefinition>("Poison_TheLongNight");

        internal static ItemDefinition Poison_TigerFang { get; } = GetDefinition<ItemDefinition>("Poison_TigerFang");
        internal static ItemDefinition PoisonersKit { get; } = GetDefinition<ItemDefinition>("PoisonersKit");
        internal static ItemDefinition Potion_Antitoxin { get; } = GetDefinition<ItemDefinition>("Potion_Antitoxin");
        internal static ItemDefinition PotionOfClimbing { get; } = GetDefinition<ItemDefinition>("PotionOfClimbing");

        internal static ItemDefinition PotionOfComprehendLanguages { get; } =
            GetDefinition<ItemDefinition>("PotionOfComprehendLanguages");

        internal static ItemDefinition PotionOfFlying { get; } = GetDefinition<ItemDefinition>("PotionOfFlying");

        internal static ItemDefinition PotionOfGiantStrengthCloud { get; } =
            GetDefinition<ItemDefinition>("PotionOfGiantStrengthCloud");

        internal static ItemDefinition PotionOfGiantStrengthFire { get; } =
            GetDefinition<ItemDefinition>("PotionOfGiantStrengthFire");

        internal static ItemDefinition PotionOfGiantStrengthFrost { get; } =
            GetDefinition<ItemDefinition>("PotionOfGiantStrengthFrost");

        internal static ItemDefinition PotionOfGiantStrengthHill { get; } =
            GetDefinition<ItemDefinition>("PotionOfGiantStrengthHill");

        internal static ItemDefinition PotionOfGreaterHealing { get; } =
            GetDefinition<ItemDefinition>("PotionOfGreaterHealing");

        internal static ItemDefinition PotionOfHealing { get; } = GetDefinition<ItemDefinition>("PotionOfHealing");
        internal static ItemDefinition PotionOfHeroism { get; } = GetDefinition<ItemDefinition>("PotionOfHeroism");

        internal static ItemDefinition PotionOfInvisibility { get; } =
            GetDefinition<ItemDefinition>("PotionOfInvisibility");

        internal static ItemDefinition PotionOfInvulnerability { get; } =
            GetDefinition<ItemDefinition>("PotionOfInvulnerability");

        internal static ItemDefinition PotionOfSpeed { get; } = GetDefinition<ItemDefinition>("PotionOfSpeed");

        internal static ItemDefinition PotionOfSuperiorHealing { get; } =
            GetDefinition<ItemDefinition>("PotionOfSuperiorHealing");

        internal static ItemDefinition PotionRemedy { get; } = GetDefinition<ItemDefinition>("PotionRemedy");
        internal static ItemDefinition PriestPack { get; } = GetDefinition<ItemDefinition>("PriestPack");
        internal static ItemDefinition Primed_Battleaxe { get; } = GetDefinition<ItemDefinition>("Primed Battleaxe");

        internal static ItemDefinition Primed_Breastplate { get; } =
            GetDefinition<ItemDefinition>("Primed Breastplate");

        internal static ItemDefinition Primed_ChainMail { get; } = GetDefinition<ItemDefinition>("Primed ChainMail");
        internal static ItemDefinition Primed_ChainShirt { get; } = GetDefinition<ItemDefinition>("Primed ChainShirt");
        internal static ItemDefinition Primed_Dagger { get; } = GetDefinition<ItemDefinition>("Primed Dagger");
        internal static ItemDefinition Primed_Gauntlet { get; } = GetDefinition<ItemDefinition>("Primed Gauntlet");
        internal static ItemDefinition Primed_Greataxe { get; } = GetDefinition<ItemDefinition>("Primed Greataxe");
        internal static ItemDefinition Primed_Greatsword { get; } = GetDefinition<ItemDefinition>("Primed Greatsword");
        internal static ItemDefinition Primed_HalfPlate { get; } = GetDefinition<ItemDefinition>("Primed HalfPlate");

        internal static ItemDefinition Primed_Leather_Armor { get; } =
            GetDefinition<ItemDefinition>("Primed Leather Armor");

        internal static ItemDefinition Primed_Longbow { get; } = GetDefinition<ItemDefinition>("Primed Longbow");
        internal static ItemDefinition Primed_Mace { get; } = GetDefinition<ItemDefinition>("Primed Mace");

        internal static ItemDefinition Primed_Morningstar { get; } =
            GetDefinition<ItemDefinition>("Primed Morningstar");

        internal static ItemDefinition Primed_Plate { get; } = GetDefinition<ItemDefinition>("Primed Plate");
        internal static ItemDefinition Primed_Rapier { get; } = GetDefinition<ItemDefinition>("Primed Rapier");
        internal static ItemDefinition Primed_ScaleMail { get; } = GetDefinition<ItemDefinition>("Primed ScaleMail");
        internal static ItemDefinition Primed_Scimitar { get; } = GetDefinition<ItemDefinition>("Primed Scimitar");
        internal static ItemDefinition Primed_Shortbow { get; } = GetDefinition<ItemDefinition>("Primed Shortbow");
        internal static ItemDefinition Primed_Shortsword { get; } = GetDefinition<ItemDefinition>("Primed Shortsword");

        internal static ItemDefinition Primed_Shortsword_DLC_Sovereign { get; } =
            GetDefinition<ItemDefinition>("Primed Shortsword_DLC_Sovereign");

        internal static ItemDefinition Primed_HeavyCrossbow { get; } =
            GetDefinition<ItemDefinition>("Primed_HeavyCrossbow");

        internal static ItemDefinition Primed_HideArmor { get; } = GetDefinition<ItemDefinition>("Primed_HideArmor");

        internal static ItemDefinition Primed_LeatherDruid { get; } =
            GetDefinition<ItemDefinition>("Primed_LeatherDruid");

        internal static ItemDefinition Primed_LightCrossbow { get; } =
            GetDefinition<ItemDefinition>("Primed_LightCrossbow");

        internal static ItemDefinition Primed_Longsword { get; } = GetDefinition<ItemDefinition>("Primed_Longsword");
        internal static ItemDefinition Primed_Maul { get; } = GetDefinition<ItemDefinition>("Primed_Maul");
        internal static ItemDefinition Primed_Spear { get; } = GetDefinition<ItemDefinition>("Primed_Spear");

        internal static ItemDefinition Primed_StuddedLeather { get; } =
            GetDefinition<ItemDefinition>("Primed_StuddedLeather");

        internal static ItemDefinition Primed_Warhammer { get; } = GetDefinition<ItemDefinition>("Primed_Warhammer");
        internal static ItemDefinition PrimedRing { get; } = GetDefinition<ItemDefinition>("PrimedRing");
        internal static ItemDefinition PrimedScepter { get; } = GetDefinition<ItemDefinition>("PrimedScepter");
        internal static ItemDefinition PrimedWand { get; } = GetDefinition<ItemDefinition>("PrimedWand");
        internal static ItemDefinition ProducedFlame { get; } = GetDefinition<ItemDefinition>("ProducedFlame");
        internal static ItemDefinition Quarterstaff { get; } = GetDefinition<ItemDefinition>("Quarterstaff");
        internal static ItemDefinition QuarterstaffPlus1 { get; } = GetDefinition<ItemDefinition>("Quarterstaff+1");
        internal static ItemDefinition QuarterstaffPlus2 { get; } = GetDefinition<ItemDefinition>("Quarterstaff+2");
        internal static ItemDefinition Rapier { get; } = GetDefinition<ItemDefinition>("Rapier");
        internal static ItemDefinition RapierPlus1 { get; } = GetDefinition<ItemDefinition>("Rapier+1");
        internal static ItemDefinition RapierPlus2 { get; } = GetDefinition<ItemDefinition>("Rapier+2");

        internal static ItemDefinition RestorativeOintment { get; } =
            GetDefinition<ItemDefinition>("RestorativeOintment");

        internal static ItemDefinition RingDarkvision { get; } = GetDefinition<ItemDefinition>("RingDarkvision");

        internal static ItemDefinition RingDetectInvisible { get; } =
            GetDefinition<ItemDefinition>("RingDetectInvisible");

        internal static ItemDefinition RingFeatherFalling { get; } =
            GetDefinition<ItemDefinition>("RingFeatherFalling");

        internal static ItemDefinition Ringmail { get; } = GetDefinition<ItemDefinition>("Ringmail");

        internal static ItemDefinition RingMightyStrength { get; } =
            GetDefinition<ItemDefinition>("RingMightyStrength");

        internal static ItemDefinition RingOfColdResistance { get; } =
            GetDefinition<ItemDefinition>("RingOfColdResistance");

        internal static ItemDefinition RingOfFireResistance { get; } =
            GetDefinition<ItemDefinition>("RingOfFireResistance");

        internal static ItemDefinition RingOfLanguages { get; } = GetDefinition<ItemDefinition>("RingOfLanguages");

        internal static ItemDefinition RingOfNecroticResistance { get; } =
            GetDefinition<ItemDefinition>("RingOfNecroticResistance");

        internal static ItemDefinition RingOfPoisonResistance { get; } =
            GetDefinition<ItemDefinition>("RingOfPoisonResistance");

        internal static ItemDefinition RingOfTheAmbassador { get; } =
            GetDefinition<ItemDefinition>("RingOfTheAmbassador");

        internal static ItemDefinition RingOfTheLightbringers { get; } =
            GetDefinition<ItemDefinition>("RingOfTheLightbringers");

        internal static ItemDefinition RingOfTheLordInquisitor { get; } =
            GetDefinition<ItemDefinition>("RingOfTheLordInquisitor");

        internal static ItemDefinition RingProtectionPlus1 { get; } = GetDefinition<ItemDefinition>("RingProtection+1");
        internal static ItemDefinition RingRegeneration { get; } = GetDefinition<ItemDefinition>("RingRegeneration");

        internal static ItemDefinition Rodric_Hideout_Note { get; } =
            GetDefinition<ItemDefinition>("Rodric_Hideout_Note");

        internal static ItemDefinition ScaleMail { get; } = GetDefinition<ItemDefinition>("ScaleMail");

        internal static ItemDefinition ScaleMail_VigdisOnly { get; } =
            GetDefinition<ItemDefinition>("ScaleMail_VigdisOnly");

        internal static ItemDefinition ScaleMailPlus1 { get; } = GetDefinition<ItemDefinition>("ScaleMail+1");
        internal static ItemDefinition ScaleMailCleric { get; } = GetDefinition<ItemDefinition>("ScaleMailCleric");
        internal static ItemDefinition ScholarPack { get; } = GetDefinition<ItemDefinition>("ScholarPack");
        internal static ItemDefinition Scimitar { get; } = GetDefinition<ItemDefinition>("Scimitar");

        internal static ItemDefinition Scimitar_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("Scimitar_Duel_Autoequip");

        internal static ItemDefinition ScimitarPlus1 { get; } = GetDefinition<ItemDefinition>("Scimitar+1");
        internal static ItemDefinition ScimitarOfSpeed { get; } = GetDefinition<ItemDefinition>("ScimitarOfSpeed");
        internal static ItemDefinition ScrollAcidArrow { get; } = GetDefinition<ItemDefinition>("ScrollAcidArrow");

        internal static ItemDefinition ScrollAnimalFriendship { get; } =
            GetDefinition<ItemDefinition>("ScrollAnimalFriendship");

        internal static ItemDefinition ScrollBane { get; } = GetDefinition<ItemDefinition>("ScrollBane");
        internal static ItemDefinition ScrollBanishment { get; } = GetDefinition<ItemDefinition>("ScrollBanishment");
        internal static ItemDefinition ScrollBarkskin { get; } = GetDefinition<ItemDefinition>("ScrollBarkskin");

        internal static ItemDefinition ScrollBlackTentacles { get; } =
            GetDefinition<ItemDefinition>("ScrollBlackTentacles");

        internal static ItemDefinition ScrollBladeBarrier { get; } =
            GetDefinition<ItemDefinition>("ScrollBladeBarrier");

        internal static ItemDefinition ScrollBless { get; } = GetDefinition<ItemDefinition>("ScrollBless");
        internal static ItemDefinition ScrollBlight { get; } = GetDefinition<ItemDefinition>("ScrollBlight");
        internal static ItemDefinition ScrollBlindness { get; } = GetDefinition<ItemDefinition>("ScrollBlindness");
        internal static ItemDefinition ScrollBlur { get; } = GetDefinition<ItemDefinition>("ScrollBlur");

        internal static ItemDefinition ScrollBurningHands { get; } =
            GetDefinition<ItemDefinition>("ScrollBurningHands");

        internal static ItemDefinition ScrollChainLightning { get; } =
            GetDefinition<ItemDefinition>("ScrollChainLightning");

        internal static ItemDefinition ScrollCharmPerson { get; } = GetDefinition<ItemDefinition>("ScrollCharmPerson");

        internal static ItemDefinition ScrollCircleOfDeath { get; } =
            GetDefinition<ItemDefinition>("ScrollCircleOfDeath");

        internal static ItemDefinition ScrollCloudKill { get; } = GetDefinition<ItemDefinition>("ScrollCloudKill");
        internal static ItemDefinition ScrollColorSpray { get; } = GetDefinition<ItemDefinition>("ScrollColorSpray");
        internal static ItemDefinition ScrollCommand { get; } = GetDefinition<ItemDefinition>("ScrollCommand");

        internal static ItemDefinition ScrollComprehendLanguages { get; } =
            GetDefinition<ItemDefinition>("ScrollComprehendLanguages");

        internal static ItemDefinition ScrollConeOfCold { get; } = GetDefinition<ItemDefinition>("ScrollConeOfCold");
        internal static ItemDefinition ScrollConfusion { get; } = GetDefinition<ItemDefinition>("ScrollConfusion");

        internal static ItemDefinition ScrollConjureAnimals { get; } =
            GetDefinition<ItemDefinition>("ScrollConjureAnimals");

        internal static ItemDefinition ScrollConjureElemental { get; } =
            GetDefinition<ItemDefinition>("ScrollConjureElemental");

        internal static ItemDefinition ScrollConjureFey { get; } = GetDefinition<ItemDefinition>("ScrollConjureFey");

        internal static ItemDefinition ScrollConjureMinorElementals { get; } =
            GetDefinition<ItemDefinition>("ScrollConjureMinorElementals");

        internal static ItemDefinition ScrollContagion { get; } = GetDefinition<ItemDefinition>("ScrollContagion");

        internal static ItemDefinition ScrollCounterspell { get; } =
            GetDefinition<ItemDefinition>("ScrollCounterspell");

        internal static ItemDefinition ScrollCureWounds { get; } = GetDefinition<ItemDefinition>("ScrollCureWounds");
        internal static ItemDefinition ScrollDarkness { get; } = GetDefinition<ItemDefinition>("ScrollDarkness");
        internal static ItemDefinition ScrollDarkvision { get; } = GetDefinition<ItemDefinition>("ScrollDarkvision");
        internal static ItemDefinition ScrollDaylight { get; } = GetDefinition<ItemDefinition>("ScrollDaylight");
        internal static ItemDefinition ScrollDeathward { get; } = GetDefinition<ItemDefinition>("ScrollDeathward");

        internal static ItemDefinition ScrollDetectEvilandGood { get; } =
            GetDefinition<ItemDefinition>("ScrollDetectEvilandGood");

        internal static ItemDefinition ScrollDetectMagic { get; } = GetDefinition<ItemDefinition>("ScrollDetectMagic");

        internal static ItemDefinition ScrollDetectPoisonAndDisease { get; } =
            GetDefinition<ItemDefinition>("ScrollDetectPoisonAndDisease");

        internal static ItemDefinition ScrollDimensionDoor { get; } =
            GetDefinition<ItemDefinition>("ScrollDimensionDoor");

        internal static ItemDefinition ScrollDisintegrate { get; } =
            GetDefinition<ItemDefinition>("ScrollDisintegrate");

        internal static ItemDefinition ScrollDispelEvilAndGood { get; } =
            GetDefinition<ItemDefinition>("ScrollDispelEvilAndGood");

        internal static ItemDefinition ScrollDispelMagic { get; } = GetDefinition<ItemDefinition>("ScrollDispelMagic");

        internal static ItemDefinition ScrollDominatePerson { get; } =
            GetDefinition<ItemDefinition>("ScrollDominatePerson");

        internal static ItemDefinition ScrollEntangle { get; } = GetDefinition<ItemDefinition>("ScrollEntangle");

        internal static ItemDefinition ScrollExpeditiousRetreat { get; } =
            GetDefinition<ItemDefinition>("ScrollExpeditiousRetreat");

        internal static ItemDefinition ScrollEyeBite_Asleep { get; } =
            GetDefinition<ItemDefinition>("ScrollEyeBite_Asleep");

        internal static ItemDefinition ScrollEyeBite_Panicked { get; } =
            GetDefinition<ItemDefinition>("ScrollEyeBite_Panicked");

        internal static ItemDefinition ScrollEyeBite_Sickened { get; } =
            GetDefinition<ItemDefinition>("ScrollEyeBite_Sickened");

        internal static ItemDefinition ScrollFaerieFire { get; } = GetDefinition<ItemDefinition>("ScrollFaerieFire");
        internal static ItemDefinition ScrollFear { get; } = GetDefinition<ItemDefinition>("ScrollFear");
        internal static ItemDefinition ScrollFeatherFall { get; } = GetDefinition<ItemDefinition>("ScrollFeatherFall");
        internal static ItemDefinition ScrollFindTraps { get; } = GetDefinition<ItemDefinition>("ScrollFindTraps");
        internal static ItemDefinition ScrollFireball { get; } = GetDefinition<ItemDefinition>("ScrollFireball");
        internal static ItemDefinition ScrollFireShield { get; } = GetDefinition<ItemDefinition>("ScrollFireShield");
        internal static ItemDefinition ScrollFlameblade { get; } = GetDefinition<ItemDefinition>("ScrollFlameblade");
        internal static ItemDefinition ScrollFlameStrike { get; } = GetDefinition<ItemDefinition>("ScrollFlameStrike");

        internal static ItemDefinition ScrollFlamingSphere { get; } =
            GetDefinition<ItemDefinition>("ScrollFlamingSphere");

        internal static ItemDefinition ScrollFly { get; } = GetDefinition<ItemDefinition>("ScrollFly");
        internal static ItemDefinition ScrollFogCloud { get; } = GetDefinition<ItemDefinition>("ScrollFogCloud");

        internal static ItemDefinition ScrollFreedomOfMovement { get; } =
            GetDefinition<ItemDefinition>("ScrollFreedomOfMovement");

        internal static ItemDefinition ScrollFreezingSphere { get; } =
            GetDefinition<ItemDefinition>("ScrollFreezingSphere");

        internal static ItemDefinition ScrollGiantInsect { get; } = GetDefinition<ItemDefinition>("ScrollGiantInsect");

        internal static ItemDefinition ScrollGlobeOfInvulnerability { get; } =
            GetDefinition<ItemDefinition>("ScrollGlobeOfInvulnerability");

        internal static ItemDefinition ScrollGrease { get; } = GetDefinition<ItemDefinition>("ScrollGrease");

        internal static ItemDefinition ScrollGreaterInvisibility { get; } =
            GetDefinition<ItemDefinition>("ScrollGreaterInvisibility");

        internal static ItemDefinition ScrollGreaterRestoration { get; } =
            GetDefinition<ItemDefinition>("ScrollGreaterRestoration");

        internal static ItemDefinition ScrollGuardianOfFaith { get; } =
            GetDefinition<ItemDefinition>("ScrollGuardianOfFaith");

        internal static ItemDefinition ScrollGuidingBolt { get; } = GetDefinition<ItemDefinition>("ScrollGuidingBolt");
        internal static ItemDefinition ScrollHarm { get; } = GetDefinition<ItemDefinition>("ScrollHarm");
        internal static ItemDefinition ScrollHaste { get; } = GetDefinition<ItemDefinition>("ScrollHaste");
        internal static ItemDefinition ScrollHeal { get; } = GetDefinition<ItemDefinition>("ScrollHeal");
        internal static ItemDefinition ScrollHeroesFeast { get; } = GetDefinition<ItemDefinition>("ScrollHeroesFeast");

        internal static ItemDefinition ScrollHideousLaughter { get; } =
            GetDefinition<ItemDefinition>("ScrollHideousLaughter");

        internal static ItemDefinition ScrollHoldMonster { get; } = GetDefinition<ItemDefinition>("ScrollHoldMonster");
        internal static ItemDefinition ScrollHoldPerson { get; } = GetDefinition<ItemDefinition>("ScrollHoldPerson");

        internal static ItemDefinition ScrollHypnoticPattern { get; } =
            GetDefinition<ItemDefinition>("ScrollHypnoticPattern");

        internal static ItemDefinition ScrollIceStorm { get; } = GetDefinition<ItemDefinition>("ScrollIceStorm");
        internal static ItemDefinition ScrollIdentify { get; } = GetDefinition<ItemDefinition>("ScrollIdentify");

        internal static ItemDefinition ScrollInflictWounds { get; } =
            GetDefinition<ItemDefinition>("ScrollInflictWounds");

        internal static ItemDefinition ScrollInsectPlague { get; } =
            GetDefinition<ItemDefinition>("ScrollInsectPlague");

        internal static ItemDefinition ScrollInvisibility { get; } =
            GetDefinition<ItemDefinition>("ScrollInvisibility");

        internal static ItemDefinition ScrollJump { get; } = GetDefinition<ItemDefinition>("ScrollJump");
        internal static ItemDefinition ScrollKit { get; } = GetDefinition<ItemDefinition>("ScrollKit");
        internal static ItemDefinition ScrollKnock { get; } = GetDefinition<ItemDefinition>("ScrollKnock");

        internal static ItemDefinition ScrollLesserRestoration { get; } =
            GetDefinition<ItemDefinition>("ScrollLesserRestoration");

        internal static ItemDefinition ScrollLevitate { get; } = GetDefinition<ItemDefinition>("ScrollLevitate");

        internal static ItemDefinition ScrollLightningBolt { get; } =
            GetDefinition<ItemDefinition>("ScrollLightningBolt");

        internal static ItemDefinition ScrollMageArmor { get; } = GetDefinition<ItemDefinition>("ScrollMageArmor");

        internal static ItemDefinition ScrollMagicMissile { get; } =
            GetDefinition<ItemDefinition>("ScrollMagicMissile");

        internal static ItemDefinition ScrollMagicWeapon { get; } = GetDefinition<ItemDefinition>("ScrollMagicWeapon");

        internal static ItemDefinition ScrollMassCureWounds { get; } =
            GetDefinition<ItemDefinition>("ScrollMassCureWounds");

        internal static ItemDefinition ScrollMassHealingWord { get; } =
            GetDefinition<ItemDefinition>("ScrollMassHealingWord");

        internal static ItemDefinition ScrollMindTwist { get; } = GetDefinition<ItemDefinition>("ScrollMindTwist");
        internal static ItemDefinition ScrollMirrorImage { get; } = GetDefinition<ItemDefinition>("ScrollMirrorImage");
        internal static ItemDefinition ScrollMistyStep { get; } = GetDefinition<ItemDefinition>("ScrollMistyStep");

        internal static ItemDefinition ScrollPassWithoutTrace { get; } =
            GetDefinition<ItemDefinition>("ScrollPassWithoutTrace");

        internal static ItemDefinition ScrollPhantasmalKiller { get; } =
            GetDefinition<ItemDefinition>("ScrollPhantasmalKiller");

        internal static ItemDefinition ScrollProtectionFromEnergy { get; } =
            GetDefinition<ItemDefinition>("ScrollProtectionFromEnergy");

        internal static ItemDefinition ScrollProtectionFromEvilandGood { get; } =
            GetDefinition<ItemDefinition>("ScrollProtectionFromEvilandGood");

        internal static ItemDefinition ScrollRaiseDead { get; } = GetDefinition<ItemDefinition>("ScrollRaiseDead");

        internal static ItemDefinition ScrollRayOfEnfeeblement { get; } =
            GetDefinition<ItemDefinition>("ScrollRayOfEnfeeblement");

        internal static ItemDefinition ScrollRemoveCurse { get; } = GetDefinition<ItemDefinition>("ScrollRemoveCurse");

        internal static ItemDefinition ScrollResurrection { get; } =
            GetDefinition<ItemDefinition>("ScrollResurrection");

        internal static ItemDefinition ScrollRevivify { get; } = GetDefinition<ItemDefinition>("ScrollRevivify");

        internal static ItemDefinition ScrollScorchingRay { get; } =
            GetDefinition<ItemDefinition>("ScrollScorchingRay");

        internal static ItemDefinition ScrollSeeInvisibility { get; } =
            GetDefinition<ItemDefinition>("ScrollSeeInvisibility");

        internal static ItemDefinition ScrollShatter { get; } = GetDefinition<ItemDefinition>("ScrollShatter");
        internal static ItemDefinition ScrollShield { get; } = GetDefinition<ItemDefinition>("ScrollShield");

        internal static ItemDefinition ScrollShieldOfFaith { get; } =
            GetDefinition<ItemDefinition>("ScrollShieldOfFaith");

        internal static ItemDefinition ScrollSilence { get; } = GetDefinition<ItemDefinition>("ScrollSilence");
        internal static ItemDefinition ScrollSleep { get; } = GetDefinition<ItemDefinition>("ScrollSleep");
        internal static ItemDefinition ScrollSleetStorm { get; } = GetDefinition<ItemDefinition>("ScrollSleetStorm");
        internal static ItemDefinition ScrollSlow { get; } = GetDefinition<ItemDefinition>("ScrollSlow");
        internal static ItemDefinition ScrollSpiderClimb { get; } = GetDefinition<ItemDefinition>("ScrollSpiderClimb");

        internal static ItemDefinition ScrollSpiritGuardians { get; } =
            GetDefinition<ItemDefinition>("ScrollSpiritGuardians");

        internal static ItemDefinition ScrollSpiritualWeapon { get; } =
            GetDefinition<ItemDefinition>("ScrollSpiritualWeapon");

        internal static ItemDefinition ScrollStinkingCloud { get; } =
            GetDefinition<ItemDefinition>("ScrollStinkingCloud");

        internal static ItemDefinition ScrollStoneskin { get; } = GetDefinition<ItemDefinition>("ScrollStoneskin");
        internal static ItemDefinition ScrollSunbeam { get; } = GetDefinition<ItemDefinition>("ScrollSunbeam");
        internal static ItemDefinition ScrollThunderWave { get; } = GetDefinition<ItemDefinition>("ScrollThunderWave");
        internal static ItemDefinition ScrollTongues { get; } = GetDefinition<ItemDefinition>("ScrollTongues");
        internal static ItemDefinition ScrollTrueSeeing { get; } = GetDefinition<ItemDefinition>("ScrollTrueSeeing");

        internal static ItemDefinition ScrollVampiricTouch { get; } =
            GetDefinition<ItemDefinition>("ScrollVampiricTouch");

        internal static ItemDefinition ScrollWallOfFire { get; } = GetDefinition<ItemDefinition>("ScrollWallOfFire");

        internal static ItemDefinition ScrollWallOfThorns { get; } =
            GetDefinition<ItemDefinition>("ScrollWallOfThorns");

        internal static ItemDefinition ScrollWardingBond { get; } = GetDefinition<ItemDefinition>("ScrollWardingBond");

        internal static ItemDefinition Sellsword_Quest_Document_01 { get; } =
            GetDefinition<ItemDefinition>("Sellsword_Quest_Document_01");

        internal static ItemDefinition Sellsword_Quest_Document_02 { get; } =
            GetDefinition<ItemDefinition>("Sellsword_Quest_Document_02");

        internal static ItemDefinition Sellsword_Quest_Document_03 { get; } =
            GetDefinition<ItemDefinition>("Sellsword_Quest_Document_03");

        internal static ItemDefinition Sellsword_Quest_Document_04 { get; } =
            GetDefinition<ItemDefinition>("Sellsword_Quest_Document_04");

        internal static ItemDefinition Sellsword_Quest_House_Key { get; } =
            GetDefinition<ItemDefinition>("Sellsword_Quest_House_Key");

        internal static ItemDefinition Shawm { get; } = GetDefinition<ItemDefinition>("Shawm");
        internal static ItemDefinition Shield { get; } = GetDefinition<ItemDefinition>("Shield");

        internal static ItemDefinition Shield_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("Shield_Duel_Autoequip");

        internal static ItemDefinition Shield_of_Cohh { get; } = GetDefinition<ItemDefinition>("Shield_of_Cohh");
        internal static ItemDefinition Shield_Wooden { get; } = GetDefinition<ItemDefinition>("Shield_Wooden");
        internal static ItemDefinition ShieldPlus1 { get; } = GetDefinition<ItemDefinition>("Shield+1");
        internal static ItemDefinition ShieldPlus2 { get; } = GetDefinition<ItemDefinition>("Shield+2");
        internal static ItemDefinition ShieldPlus3 { get; } = GetDefinition<ItemDefinition>("Shield+3");
        internal static ItemDefinition Shortbow { get; } = GetDefinition<ItemDefinition>("Shortbow");
        internal static ItemDefinition ShortbowPlus1 { get; } = GetDefinition<ItemDefinition>("Shortbow+1");
        internal static ItemDefinition ShortbowPlus2 { get; } = GetDefinition<ItemDefinition>("Shortbow+2");
        internal static ItemDefinition Shortsword { get; } = GetDefinition<ItemDefinition>("Shortsword");

        internal static ItemDefinition Shortsword_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("Shortsword_Duel_Autoequip");

        internal static ItemDefinition ShortswordPlus1 { get; } = GetDefinition<ItemDefinition>("Shortsword+1");
        internal static ItemDefinition ShortswordPlus2 { get; } = GetDefinition<ItemDefinition>("Shortsword+2");

        internal static ItemDefinition Sigil_Ring_Abjuration { get; } =
            GetDefinition<ItemDefinition>("Sigil_Ring_Abjuration");

        internal static ItemDefinition Sigil_Ring_Necromancy { get; } =
            GetDefinition<ItemDefinition>("Sigil_Ring_Necromancy");

        internal static ItemDefinition SkeletonMarksman_Necro_Arrow { get; } =
            GetDefinition<ItemDefinition>("SkeletonMarksman_Necro_Arrow");

        internal static ItemDefinition SlippersOfSpiderClimbing { get; } =
            GetDefinition<ItemDefinition>("SlippersOfSpiderClimbing");

        internal static ItemDefinition Sorak_Head { get; } = GetDefinition<ItemDefinition>("Sorak_Head");

        internal static ItemDefinition Sorak_PoisonedSpine { get; } =
            GetDefinition<ItemDefinition>("Sorak_PoisonedSpine");

        internal static ItemDefinition SorakShriekerBlade { get; } =
            GetDefinition<ItemDefinition>("SorakShriekerBlade");

        internal static ItemDefinition SorcererArmor { get; } = GetDefinition<ItemDefinition>("SorcererArmor");
        internal static ItemDefinition Spear { get; } = GetDefinition<ItemDefinition>("Spear");
        internal static ItemDefinition SpearPlus1 { get; } = GetDefinition<ItemDefinition>("Spear+1");
        internal static ItemDefinition SpearPlus2 { get; } = GetDefinition<ItemDefinition>("Spear+2");
        internal static ItemDefinition Spellbook { get; } = GetDefinition<ItemDefinition>("Spellbook");

        internal static ItemDefinition SpiderCrimson_AcidSpit { get; } =
            GetDefinition<ItemDefinition>("SpiderCrimson_AcidSpit");

        internal static ItemDefinition Spit { get; } = GetDefinition<ItemDefinition>("Spit");
        internal static ItemDefinition SplintArmor { get; } = GetDefinition<ItemDefinition>("SplintArmor");
        internal static ItemDefinition SpoonOfDiscord { get; } = GetDefinition<ItemDefinition>("SpoonOfDiscord");

        internal static ItemDefinition Spy_Quest_Document_01 { get; } =
            GetDefinition<ItemDefinition>("Spy_Quest_Document_01");

        internal static ItemDefinition StaffOfFire { get; } = GetDefinition<ItemDefinition>("StaffOfFire");
        internal static ItemDefinition StaffOfHealing { get; } = GetDefinition<ItemDefinition>("StaffOfHealing");
        internal static ItemDefinition StaffOfMetis { get; } = GetDefinition<ItemDefinition>("StaffOfMetis");

        internal static ItemDefinition StaffOfSwarmingInsects { get; } =
            GetDefinition<ItemDefinition>("StaffOfSwarmingInsects");

        internal static ItemDefinition StandardBrassKey { get; } = GetDefinition<ItemDefinition>("StandardBrassKey");
        internal static ItemDefinition StandardDragonKey { get; } = GetDefinition<ItemDefinition>("StandardDragonKey");
        internal static ItemDefinition StandardGoldKey { get; } = GetDefinition<ItemDefinition>("StandardGoldKey");
        internal static ItemDefinition StandardIronKey { get; } = GetDefinition<ItemDefinition>("StandardIronKey");
        internal static ItemDefinition StandardSilverKey { get; } = GetDefinition<ItemDefinition>("StandardSilverKey");

        internal static ItemDefinition StartingWealth_10GP { get; } =
            GetDefinition<ItemDefinition>("StartingWealth_10GP");

        internal static ItemDefinition StartingWealth_15GP { get; } =
            GetDefinition<ItemDefinition>("StartingWealth_15GP");

        internal static ItemDefinition StartingWealth_25GP { get; } =
            GetDefinition<ItemDefinition>("StartingWealth_25GP");

        internal static ItemDefinition StartingWealth_80GP { get; } =
            GetDefinition<ItemDefinition>("StartingWealth_80GP");

        internal static ItemDefinition StoneOfGoodLuck { get; } = GetDefinition<ItemDefinition>("StoneOfGoodLuck");
        internal static ItemDefinition StuddedLeather { get; } = GetDefinition<ItemDefinition>("StuddedLeather");

        internal static ItemDefinition StuddedLeather_Duel_Autoequip { get; } =
            GetDefinition<ItemDefinition>("StuddedLeather_Duel_Autoequip");

        internal static ItemDefinition StuddedLeather_plus_one { get; } =
            GetDefinition<ItemDefinition>("StuddedLeather_plus_one");

        internal static ItemDefinition StuddedLeather_plus_two { get; } =
            GetDefinition<ItemDefinition>("StuddedLeather_plus_two");

        internal static ItemDefinition Telema_Confiscation_letter { get; } =
            GetDefinition<ItemDefinition>("Telema_Confiscation_letter");

        internal static ItemDefinition Telema_Golden_Key { get; } = GetDefinition<ItemDefinition>("Telema_Golden_Key");
        internal static ItemDefinition Telema_Notebook { get; } = GetDefinition<ItemDefinition>("Telema_Notebook");
        internal static ItemDefinition Telema_Orc_Plaque { get; } = GetDefinition<ItemDefinition>("Telema_Orc_Plaque");

        internal static ItemDefinition Telema_Testament_document { get; } =
            GetDefinition<ItemDefinition>("Telema_Testament_document");

        internal static ItemDefinition TemplateDM_Document_Book01 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book01");

        internal static ItemDefinition TemplateDM_Document_Book02 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book02");

        internal static ItemDefinition TemplateDM_Document_Book03 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book03");

        internal static ItemDefinition TemplateDM_Document_Book04 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book04");

        internal static ItemDefinition TemplateDM_Document_Book05 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book05");

        internal static ItemDefinition TemplateDM_Document_Book06 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book06");

        internal static ItemDefinition TemplateDM_Document_Book07 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Book07");

        internal static ItemDefinition TemplateDM_Document_Letter { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Letter");

        internal static ItemDefinition TemplateDM_Document_Letter02 { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Letter02");

        internal static ItemDefinition TemplateDM_Document_Notes { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Notes");

        internal static ItemDefinition TemplateDM_Document_RolledLetter { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_RolledLetter");

        internal static ItemDefinition TemplateDM_Document_Scroll { get; } =
            GetDefinition<ItemDefinition>("TemplateDM_Document_Scroll");

        internal static ItemDefinition Temple_Of_Einar_Key { get; } =
            GetDefinition<ItemDefinition>("Temple_Of_Einar_Key");

        internal static ItemDefinition TheAmendment { get; } = GetDefinition<ItemDefinition>("TheAmendment");
        internal static ItemDefinition ThievesTool { get; } = GetDefinition<ItemDefinition>("ThievesTool");

        internal static ItemDefinition Tirmarian_Greataxe { get; } =
            GetDefinition<ItemDefinition>("Tirmarian_Greataxe");

        internal static ItemDefinition Tome_Of_LeadershipAndInfluence { get; } =
            GetDefinition<ItemDefinition>("Tome_Of_LeadershipAndInfluence");

        internal static ItemDefinition Tome_Of_QuickThought { get; } =
            GetDefinition<ItemDefinition>("Tome_Of_QuickThought");

        internal static ItemDefinition Tome_Of_QuickThought_DLC_Hasdrubal { get; } =
            GetDefinition<ItemDefinition>("Tome_Of_QuickThought_DLC_Hasdrubal");

        internal static ItemDefinition Tome_Of_Understanding { get; } =
            GetDefinition<ItemDefinition>("Tome_Of_Understanding");

        internal static ItemDefinition TomeOfAllThings { get; } = GetDefinition<ItemDefinition>("TomeOfAllThings");
        internal static ItemDefinition Torch { get; } = GetDefinition<ItemDefinition>("Torch");

        internal static ItemDefinition TOWEROFMAGIC_ArcheologicalNotes01 { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_ArcheologicalNotes01");

        internal static ItemDefinition TOWEROFMAGIC_ArcheologicalNotes02 { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_ArcheologicalNotes02");

        internal static ItemDefinition TOWEROFMAGIC_ArcheologicalNotes03 { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_ArcheologicalNotes03");

        internal static ItemDefinition TOWEROFMAGIC_LoreDocument { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_LoreDocument");

        internal static ItemDefinition TOWEROFMAGIC_SecretSpellBook { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_SecretSpellBook");

        internal static ItemDefinition TOWEROFMAGIC_SorakOrders { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_SorakOrders");

        internal static ItemDefinition TOWEROFMAGIC_WorkerLetter { get; } =
            GetDefinition<ItemDefinition>("TOWEROFMAGIC_WorkerLetter");

        internal static ItemDefinition Tutorial_01_Arrow { get; } = GetDefinition<ItemDefinition>("Tutorial_01_Arrow");

        internal static ItemDefinition Tutorial_01_Backpack { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_Backpack");

        internal static ItemDefinition Tutorial_01_Candle { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_Candle");

        internal static ItemDefinition Tutorial_01_Dagger { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_Dagger");

        internal static ItemDefinition Tutorial_01_Leather_Armor { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_Leather_Armor");

        internal static ItemDefinition Tutorial_01_Rapier { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_Rapier");

        internal static ItemDefinition Tutorial_01_Shortbow { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_Shortbow");

        internal static ItemDefinition Tutorial_01_ThievesTool { get; } =
            GetDefinition<ItemDefinition>("Tutorial_01_ThievesTool");

        internal static ItemDefinition Tutorial_03_PouchOfHealing { get; } =
            GetDefinition<ItemDefinition>("Tutorial_03_PouchOfHealing");

        internal static ItemDefinition Tutorial_03_Torch { get; } = GetDefinition<ItemDefinition>("Tutorial_03_Torch");

        internal static ItemDefinition Tutorial_04_Golden_Key { get; } =
            GetDefinition<ItemDefinition>("Tutorial_04_Golden_Key");

        internal static ItemDefinition Tutorial_04_Longsword { get; } =
            GetDefinition<ItemDefinition>("Tutorial_04_Longsword");

        internal static ItemDefinition Tutorial_04_Quest_Ruby { get; } =
            GetDefinition<ItemDefinition>("Tutorial_04_Quest_Ruby");

        internal static ItemDefinition UnarmedStrikeBase { get; } = GetDefinition<ItemDefinition>("UnarmedStrikeBase");
        internal static ItemDefinition WandFear { get; } = GetDefinition<ItemDefinition>("WandFear");
        internal static ItemDefinition WandMagicMissile { get; } = GetDefinition<ItemDefinition>("WandMagicMissile");
        internal static ItemDefinition WandOfBlight { get; } = GetDefinition<ItemDefinition>("WandOfBlight");
        internal static ItemDefinition WandOfIdentify { get; } = GetDefinition<ItemDefinition>("WandOfIdentify");

        internal static ItemDefinition WandOfLightningBolts { get; } =
            GetDefinition<ItemDefinition>("WandOfLightningBolts");

        internal static ItemDefinition WandOfMagicDetection { get; } =
            GetDefinition<ItemDefinition>("WandOfMagicDetection");

        internal static ItemDefinition WandOfThorns { get; } = GetDefinition<ItemDefinition>("WandOfThorns");
        internal static ItemDefinition WandOfWarMagePlus1 { get; } = GetDefinition<ItemDefinition>("WandOfWarMage+1");
        internal static ItemDefinition WandOfWarMagePlus2 { get; } = GetDefinition<ItemDefinition>("WandOfWarMage+2");
        internal static ItemDefinition WandOfWinter { get; } = GetDefinition<ItemDefinition>("WandOfWinter");
        internal static ItemDefinition WandTestDisease { get; } = GetDefinition<ItemDefinition>("WandTestDisease");
        internal static ItemDefinition Warhammer { get; } = GetDefinition<ItemDefinition>("Warhammer");
        internal static ItemDefinition WarhammerPlus1 { get; } = GetDefinition<ItemDefinition>("Warhammer+1");
        internal static ItemDefinition WarhammerPlus2 { get; } = GetDefinition<ItemDefinition>("Warhammer+2");
        internal static ItemDefinition Warlock_Armor { get; } = GetDefinition<ItemDefinition>("Warlock_Armor");
        internal static ItemDefinition Web { get; } = GetDefinition<ItemDefinition>("Web");

        internal static ItemDefinition WizardClothes_Alternate { get; } =
            GetDefinition<ItemDefinition>("WizardClothes_Alternate");
    }

    internal static class ItemFlagDefinitions
    {
        internal static ItemFlagDefinition ItemFlag_Clue_Exonerating { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlag_Clue_Exonerating");

        internal static ItemFlagDefinition ItemFlag_Clue_Incriminating { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlag_Clue_Incriminating");

        internal static ItemFlagDefinition ItemFlag_Corrosive { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlag_Corrosive");

        internal static ItemFlagDefinition ItemFlag_Flaming { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlag_Flaming");

        internal static ItemFlagDefinition ItemFlag_Flash { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlag_Flash");

        internal static ItemFlagDefinition ItemFlagDLC3_Dwarven_Weapon { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagDLC3_Dwarven_Weapon");

        internal static ItemFlagDefinition ItemFlagIngredient_Component { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagIngredient_Component");

        internal static ItemFlagDefinition ItemFlagIngredient_Enchant { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagIngredient_Enchant");

        internal static ItemFlagDefinition ItemFlagPoison_1D4 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_1D4");

        internal static ItemFlagDefinition ItemFlagPoison_1D6 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_1D6");

        internal static ItemFlagDefinition ItemFlagPoison_1D8 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_1D8");

        internal static ItemFlagDefinition ItemFlagPoison_2D4 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_2D4");

        internal static ItemFlagDefinition ItemFlagPoison_2D8 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_2D8");

        internal static ItemFlagDefinition ItemFlagPoison_3D6 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_3D6");

        internal static ItemFlagDefinition ItemFlagPoison_3D8 { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_3D8");

        internal static ItemFlagDefinition ItemFlagPoison_Blinding { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_Blinding");

        internal static ItemFlagDefinition ItemFlagPoison_Condition_Poisoned { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_Condition_Poisoned");

        internal static ItemFlagDefinition ItemFlagPoison_Overtime { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_Overtime");

        internal static ItemFlagDefinition ItemFlagPoison_Paralyzing { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_Paralyzing");

        internal static ItemFlagDefinition ItemFlagPoison_Restrained { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPoison_Restrained");

        internal static ItemFlagDefinition ItemFlagPrimed { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPrimed");

        internal static ItemFlagDefinition ItemFlagQuest { get; } = GetDefinition<ItemFlagDefinition>("ItemFlagQuest");
    }

    internal static class KnowledgeLevelDefinitions
    {
        internal static KnowledgeLevelDefinition Known3 { get; } = GetDefinition<KnowledgeLevelDefinition>("Known3");

        internal static KnowledgeLevelDefinition Mastered4 { get; } =
            GetDefinition<KnowledgeLevelDefinition>("Mastered4");

        internal static KnowledgeLevelDefinition Observed1 { get; } =
            GetDefinition<KnowledgeLevelDefinition>("Observed1");

        internal static KnowledgeLevelDefinition Studied2 { get; } =
            GetDefinition<KnowledgeLevelDefinition>("Studied2");

        internal static KnowledgeLevelDefinition Unknown0 { get; } =
            GetDefinition<KnowledgeLevelDefinition>("Unknown0");
    }

    internal static class MerchantCategoryDefinitions
    {
        internal static MerchantCategoryDefinition Adventuring { get; } =
            GetDefinition<MerchantCategoryDefinition>("Adventuring");

        internal static MerchantCategoryDefinition All { get; } = GetDefinition<MerchantCategoryDefinition>("All");
        internal static MerchantCategoryDefinition Armor { get; } = GetDefinition<MerchantCategoryDefinition>("Armor");

        internal static MerchantCategoryDefinition Crafting { get; } =
            GetDefinition<MerchantCategoryDefinition>("Crafting");

        internal static MerchantCategoryDefinition Document { get; } =
            GetDefinition<MerchantCategoryDefinition>("Document");

        internal static MerchantCategoryDefinition Ingredient { get; } =
            GetDefinition<MerchantCategoryDefinition>("Ingredient");

        internal static MerchantCategoryDefinition MagicDevice { get; } =
            GetDefinition<MerchantCategoryDefinition>("MagicDevice");

        internal static MerchantCategoryDefinition Weapon { get; } =
            GetDefinition<MerchantCategoryDefinition>("Weapon");
    }

    internal static class MerchantDefinitions
    {
        internal static MerchantDefinition CHANGE_ME_Store_NPC_Merchant_ADV_NPC_Telerien { get; } =
            GetDefinition<MerchantDefinition>("CHANGE_ME_Store_NPC_Merchant_ADV_NPC_Telerien");

        internal static MerchantDefinition DLC1_Valley_NPC_Store_Merchant_Telerien_Purevoice { get; } =
            GetDefinition<MerchantDefinition>("DLC1_Valley_NPC_Store_Merchant_Telerien_Purevoice");

        internal static MerchantDefinition Store_Merchant_Annie_Bagmordah_Scavengers_Cyflen_HQ { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Annie_Bagmordah_Scavengers_Cyflen_HQ");

        internal static MerchantDefinition Store_Merchant_Antiquarians_Halman_Summer { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Antiquarians_Halman_Summer");

        internal static MerchantDefinition Store_Merchant_Arcaneum_Heddlon_Surespell { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Arcaneum_Heddlon_Surespell");

        internal static MerchantDefinition Store_Merchant_Atima_Bladeburn { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Atima_Bladeburn");

        internal static MerchantDefinition Store_Merchant_Caer_Lem_Outpost { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Caer_Lem_Outpost");

        internal static MerchantDefinition Store_Merchant_Circe { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Circe");

        internal static MerchantDefinition Store_Merchant_CircleOfDanantar_Joriel_Foxeye { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_CircleOfDanantar_Joriel_Foxeye");

        internal static MerchantDefinition Store_Merchant_Cyflen_Priest_Arun { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Cyflen_Priest_Arun");

        internal static MerchantDefinition Store_Merchant_Cyflen_Priest_Maraike { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Cyflen_Priest_Maraike");

        internal static MerchantDefinition Store_Merchant_Cyflen_Priest_Pakri { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Cyflen_Priest_Pakri");

        internal static MerchantDefinition Store_Merchant_DLC_Armorer_Gail_Hunt { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC_Armorer_Gail_Hunt");

        internal static MerchantDefinition Store_Merchant_DLC1_Hanno { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC1_Hanno");

        internal static MerchantDefinition Store_Merchant_DLC1_Henrik { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC1_Henrik");

        internal static MerchantDefinition Store_Merchant_DLC1_Renno { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC1_Renno");

        internal static MerchantDefinition Store_Merchant_DLC1_Reya { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC1_Reya");

        internal static MerchantDefinition Store_Merchant_DLC1_Yasmin { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC1_Yasmin");

        internal static MerchantDefinition Store_Merchant_DLC3_EINAREUM_NPC_General { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_EINAREUM_NPC_General");

        internal static MerchantDefinition Store_Merchant_DLC3_EINAREUM_NPC_Ingredients { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_EINAREUM_NPC_Ingredients");

        internal static MerchantDefinition Store_Merchant_DLC3_EINAREUM_NPC_Weapons { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_EINAREUM_NPC_Weapons");

        internal static MerchantDefinition Store_Merchant_DLC3_ElfShopOwner { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_ElfShopOwner");

        internal static MerchantDefinition Store_Merchant_DLC3_Grimhild { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_Grimhild");

        internal static MerchantDefinition Store_Merchant_DLC3_KAUPAA_NPC_ClanHouse { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_KAUPAA_NPC_ClanHouse");

        internal static MerchantDefinition Store_Merchant_DLC3_KAUPAA_NPC_General { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_KAUPAA_NPC_General");

        internal static MerchantDefinition Store_Merchant_DLC3_KAUPAA_NPC_Ingredients { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_KAUPAA_NPC_Ingredients");

        internal static MerchantDefinition Store_Merchant_DLC3_KAUPAA_NPC_Scavenger { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_KAUPAA_NPC_Scavenger");

        internal static MerchantDefinition Store_Merchant_DLC3_KAUPAA_NPC_Weapons { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_KAUPAA_NPC_Weapons");

        internal static MerchantDefinition Store_Merchant_DLC3_Lena { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_Lena");

        internal static MerchantDefinition Store_Merchant_DLC3_WhiteCity_NPC_General { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_WhiteCity_NPC_General");

        internal static MerchantDefinition Store_Merchant_DLC3_WhiteCity_NPC_Ingredients { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_WhiteCity_NPC_Ingredients");

        internal static MerchantDefinition Store_Merchant_DLC3_WhiteCity_NPC_Scavenger { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_WhiteCity_NPC_Scavenger");

        internal static MerchantDefinition Store_Merchant_DLC3_WhiteCity_NPC_Weapons { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DLC3_WhiteCity_NPC_Weapons");

        internal static MerchantDefinition Store_Merchant_DM_Armorer { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DM_Armorer");

        internal static MerchantDefinition Store_Merchant_DM_Crafter { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DM_Crafter");

        internal static MerchantDefinition Store_Merchant_DM_GeneralStore { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DM_GeneralStore");

        internal static MerchantDefinition Store_Merchant_DM_MagicalItems { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DM_MagicalItems");

        internal static MerchantDefinition Store_Merchant_DM_Weaponsmith { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_DM_Weaponsmith");

        internal static MerchantDefinition Store_Merchant_Einar_Dalon_Lark { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Einar_Dalon_Lark");

        internal static MerchantDefinition Store_Merchant_Galar_Goldentongue { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Galar_Goldentongue");

        internal static MerchantDefinition Store_Merchant_Giant { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Giant");

        internal static MerchantDefinition Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore");

        internal static MerchantDefinition Store_Merchant_Hugo_Requer_Cyflen_Potions { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Hugo_Requer_Cyflen_Potions");

        internal static MerchantDefinition Store_Merchant_Karel_Martel_Cylfen_Bartender { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Karel_Martel_Cylfen_Bartender");

        internal static MerchantDefinition Store_Merchant_Maddy_new { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Maddy_new");

        internal static MerchantDefinition Store_Merchant_Orc_BladeFang { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Orc_BladeFang");

        internal static MerchantDefinition Store_Merchant_Orc_BloodSpear { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Orc_BloodSpear");

        internal static MerchantDefinition Store_Merchant_Orc_SandRaven { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Orc_SandRaven");

        internal static MerchantDefinition Store_Merchant_TowerOfKnowledge { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_TowerOfKnowledge");

        internal static MerchantDefinition Store_Merchant_TowerOfKnowledge_Maddy_Greenisle { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_TowerOfKnowledge_Maddy_Greenisle");

        internal static MerchantDefinition Store_Merchant_Wanderer_Telema_Weaponsmith { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Wanderer_Telema_Weaponsmith");

        internal static MerchantDefinition Store_Merchant_Wilf_Warmhearth { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Wilf_Warmhearth");
    }

    internal static class MetamagicOptionDefinitions
    {
        internal static MetamagicOptionDefinition MetamagicCarefullSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicCarefullSpell");

        internal static MetamagicOptionDefinition MetamagicDistantSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicDistantSpell");

        internal static MetamagicOptionDefinition MetamagicEmpoweredSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicEmpoweredSpell");

        internal static MetamagicOptionDefinition MetamagicExtendedSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicExtendedSpell");

        internal static MetamagicOptionDefinition MetamagicHeightenedSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicHeightenedSpell");

        internal static MetamagicOptionDefinition MetamagicQuickenedSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicQuickenedSpell");

        internal static MetamagicOptionDefinition MetamagicTwinnedSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicTwinnedSpell");
    }

    internal static class NarrativeTreeDefinitions
    {
    }

    internal static class PropBlueprints
    {
    }

    internal static class QuestTreeDefinitions
    {
    }

    internal static class RecipeDefinitions
    {
        internal static RecipeDefinition Recipe_Antitoxin { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Antitoxin");

        internal static RecipeDefinition Recipe_Enchantment_AmuletOfPureSouls { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_AmuletOfPureSouls");

        internal static RecipeDefinition Recipe_Enchantment_ArmorOfTheForest { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ArmorOfTheForest");

        internal static RecipeDefinition Recipe_Enchantment_ArmorOfThePrimalOak { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ArmorOfThePrimalOak");

        internal static RecipeDefinition Recipe_Enchantment_BallOfLightning { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BallOfLightning");

        internal static RecipeDefinition Recipe_Enchantment_BattleaxeOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BattleaxeOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_BattleaxeOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BattleaxeOfSharpness");

        internal static RecipeDefinition Recipe_Enchantment_BattleaxePunisher { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BattleaxePunisher");

        internal static RecipeDefinition Recipe_Enchantment_BeltOfRegeneration { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BeltOfRegeneration");

        internal static RecipeDefinition Recipe_Enchantment_BeltOfTheBarbarianKing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BeltOfTheBarbarianKing");

        internal static RecipeDefinition Recipe_Enchantment_BootsOfFireWalking { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BootsOfFireWalking");

        internal static RecipeDefinition Recipe_Enchantment_BootsOfFirstStrike { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BootsOfFirstStrike");

        internal static RecipeDefinition Recipe_Enchantment_BootsOfTheWinterland { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BootsOfTheWinterland");

        internal static RecipeDefinition Recipe_Enchantment_BracersOfStorms { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BracersOfStorms");

        internal static RecipeDefinition Recipe_Enchantment_BreastplateOfDeflection { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BreastplateOfDeflection");

        internal static RecipeDefinition Recipe_Enchantment_BreastplateOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BreastplateOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_CaveIllnessDrug { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_CaveIllnessDrug");

        internal static RecipeDefinition Recipe_Enchantment_ChainmailOfRobustness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ChainmailOfRobustness");

        internal static RecipeDefinition Recipe_Enchantment_ChainmailOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ChainmailOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_CloakOfTheAncientKing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_CloakOfTheAncientKing");

        internal static RecipeDefinition Recipe_Enchantment_CloakOfTheDandy { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_CloakOfTheDandy");

        internal static RecipeDefinition Recipe_Enchantment_CrossbowOfAccuracy { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_CrossbowOfAccuracy");

        internal static RecipeDefinition Recipe_Enchantment_CrossbowSouldrinker { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_CrossbowSouldrinker");

        internal static RecipeDefinition Recipe_Enchantment_DaggerFrostburn { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerFrostburn");

        internal static RecipeDefinition Recipe_Enchantment_DaggerOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_DaggerOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerOfSharpness");

        internal static RecipeDefinition Recipe_Enchantment_DaggerSouldrinker { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerSouldrinker");

        internal static RecipeDefinition Recipe_Enchantment_EmpressWarGarb { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_EmpressWarGarb");

        internal static RecipeDefinition Recipe_Enchantment_GauntletOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GauntletOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_GauntletOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GauntletOfSharpness");

        internal static RecipeDefinition Recipe_Enchantment_GreataxeOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GreataxeOfSharpness");

        internal static RecipeDefinition Recipe_Enchantment_GreataxeStormblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GreataxeStormblade");

        internal static RecipeDefinition Recipe_Enchantment_GreatswordLightbringer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GreatswordLightbringer");

        internal static RecipeDefinition Recipe_Enchantment_GreatwordDoomblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GreatwordDoomblade");

        internal static RecipeDefinition Recipe_Enchantment_GreatwordOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GreatwordOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_HalfplateOfRobustness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HalfplateOfRobustness");

        internal static RecipeDefinition Recipe_Enchantment_HalfplateOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HalfplateOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_HeavyCrossbowOfAccuracy { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HeavyCrossbowOfAccuracy");

        internal static RecipeDefinition Recipe_Enchantment_HeavyCrossbowWhiteburn { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HeavyCrossbowWhiteburn");

        internal static RecipeDefinition Recipe_Enchantment_HideArmorOfTheVagrant { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HideArmorOfTheVagrant");

        internal static RecipeDefinition Recipe_Enchantment_HideArmorOfWilderness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HideArmorOfWilderness");

        internal static RecipeDefinition Recipe_Enchantment_LeatherArmorOfFlameDancing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LeatherArmorOfFlameDancing");

        internal static RecipeDefinition Recipe_Enchantment_LeatherArmorOfRobustness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LeatherArmorOfRobustness");

        internal static RecipeDefinition Recipe_Enchantment_LeatherArmorOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LeatherArmorOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_LeatherArmorOfSurvival { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LeatherArmorOfSurvival");

        internal static RecipeDefinition Recipe_Enchantment_LongbowLightbringer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongbowLightbringer");

        internal static RecipeDefinition Recipe_Enchantment_LongbowOfAcurracy { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongbowOfAcurracy");

        internal static RecipeDefinition Recipe_Enchantment_LongsbowStormbow { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongsbowStormbow");

        internal static RecipeDefinition Recipe_Enchantment_LongswordDragonblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordDragonblade");

        internal static RecipeDefinition Recipe_Enchantment_LongswordFrostburn { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordFrostburn");

        internal static RecipeDefinition Recipe_Enchantment_LongswordOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_LongswordStormblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordStormblade");

        internal static RecipeDefinition Recipe_Enchantment_LongswordWarden { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordWarden");

        internal static RecipeDefinition Recipe_Enchantment_MaceOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MaceOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_MaceOfSmashing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MaceOfSmashing");

        internal static RecipeDefinition Recipe_Enchantment_MaulOfSmashing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MaulOfSmashing");

        internal static RecipeDefinition Recipe_Enchantment_MaulOfTheDestroyer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MaulOfTheDestroyer");

        internal static RecipeDefinition Recipe_Enchantment_MorningstarBearclaw { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MorningstarBearclaw");

        internal static RecipeDefinition Recipe_Enchantment_MorningstarOfPower { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MorningstarOfPower");

        internal static RecipeDefinition Recipe_Enchantment_PendantOfTheHealer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_PendantOfTheHealer");

        internal static RecipeDefinition Recipe_Enchantment_PeriaptOfTheMasterEnchanter { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_PeriaptOfTheMasterEnchanter");

        internal static RecipeDefinition Recipe_Enchantment_PlateOfRobustness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_PlateOfRobustness");

        internal static RecipeDefinition Recipe_Enchantment_PlateOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_PlateOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_RapierBlackAdder { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RapierBlackAdder");

        internal static RecipeDefinition Recipe_Enchantment_RapierDoomblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RapierDoomblade");

        internal static RecipeDefinition Recipe_Enchantment_RapierOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RapierOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_RingAmbassador { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RingAmbassador");

        internal static RecipeDefinition Recipe_Enchantment_ScaleMailOfIceDancing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScaleMailOfIceDancing");

        internal static RecipeDefinition Recipe_Enchantment_ScaleMailOfRobustness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScaleMailOfRobustness");

        internal static RecipeDefinition Recipe_Enchantment_ScaleMailOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScaleMailOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_ScepterOfRedeemerControl { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScepterOfRedeemerControl");

        internal static RecipeDefinition Recipe_Enchantment_ScimitarOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScimitarOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_ScimitarOfTheAnfarels { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScimitarOfTheAnfarels");

        internal static RecipeDefinition Recipe_Enchantment_ShortbowMedusa { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortbowMedusa");

        internal static RecipeDefinition Recipe_Enchantment_ShortbowOfAcurracy { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortbowOfAcurracy");

        internal static RecipeDefinition Recipe_Enchantment_ShortbowOfSharpshooting { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortbowOfSharpshooting");

        internal static RecipeDefinition Recipe_Enchantment_Shortsword_Sovereign { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_Shortsword_Sovereign");

        internal static RecipeDefinition Recipe_Enchantment_ShortswordLightbringer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortswordLightbringer");

        internal static RecipeDefinition Recipe_Enchantment_ShortswordWhiteburn { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortswordWhiteburn");

        internal static RecipeDefinition Recipe_Enchantment_ShortwordOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortwordOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_ShortwordOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortwordOfSharpness");

        internal static RecipeDefinition Recipe_Enchantment_SpearDoomSpear { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_SpearDoomSpear");

        internal static RecipeDefinition Recipe_Enchantment_SpearOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_SpearOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_StuddedArmorOfLeadership { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_StuddedArmorOfLeadership");

        internal static RecipeDefinition Recipe_Enchantment_StuddedArmorOfSurvival { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_StuddedArmorOfSurvival");

        internal static RecipeDefinition Recipe_Enchantment_WandOfBlight { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WandOfBlight");

        internal static RecipeDefinition Recipe_Enchantment_WandOfThorns { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WandOfThorns");

        internal static RecipeDefinition Recipe_Enchantment_WandOfWarMagePlus1 { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WandOfWarMage+1");

        internal static RecipeDefinition Recipe_Enchantment_WandOfWarMagePlus2 { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WandOfWarMage+2");

        internal static RecipeDefinition Recipe_Enchantment_WandOfWinter { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WandOfWinter");

        internal static RecipeDefinition Recipe_Enchantment_WarhammerOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WarhammerOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_WarhammerStormbinder { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_WarhammerStormbinder");

        internal static RecipeDefinition RecipeBasic_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipeBasic_Arrows");

        internal static RecipeDefinition RecipeBasic_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipeBasic_Bolts");

        internal static RecipeDefinition Recipe_DLC2_5_RestorativeOintment { get; } =
            GetDefinition<RecipeDefinition>("Recipe-DLC2.5-RestorativeOintment");

        internal static RecipeDefinition RecipeHealingRemedy { get; } =
            GetDefinition<RecipeDefinition>("RecipeHealingRemedy");

        internal static RecipeDefinition RecipePoison_ArivadsKiss { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_ArivadsKiss");

        internal static RecipeDefinition RecipePoison_ArivadsKiss_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_ArivadsKiss_Arrows");

        internal static RecipeDefinition RecipePoison_ArivadsKiss_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_ArivadsKiss_Bolts");

        internal static RecipeDefinition RecipePoison_Arrows_Alchemy_Corrosive { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Arrows_Alchemy_Corrosive");

        internal static RecipeDefinition RecipePoison_Arrows_Alchemy_Flaming { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Arrows_Alchemy_Flaming");

        internal static RecipeDefinition RecipePoison_Arrows_Alchemy_Flash { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Arrows_Alchemy_Flash");

        internal static RecipeDefinition RecipePoison_ArunsLight { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_ArunsLight");

        internal static RecipeDefinition RecipePoison_ArunsLight_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_ArunsLight_Arrows");

        internal static RecipeDefinition RecipePoison_ArunsLight_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_ArunsLight_Bolts");

        internal static RecipeDefinition RecipePoison_Basic { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Basic");

        internal static RecipeDefinition RecipePoison_BasicPoison_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_BasicPoison_Arrows");

        internal static RecipeDefinition RecipePoison_BasicPoison_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_BasicPoison_Bolts");

        internal static RecipeDefinition RecipePoison_Bolts_Alchemy_Corrosive { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Bolts_Alchemy_Corrosive");

        internal static RecipeDefinition RecipePoison_Bolts_Alchemy_Flaming { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Bolts_Alchemy_Flaming");

        internal static RecipeDefinition RecipePoison_Bolts_Alchemy_Flash { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_Bolts_Alchemy_Flash");

        internal static RecipeDefinition RecipePoison_BrimstoneFang { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_BrimstoneFang");

        internal static RecipeDefinition RecipePoison_BrimstoneFang_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_BrimstoneFang_Arrows");

        internal static RecipeDefinition RecipePoison_BrimstoneFang_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_BrimstoneFang_Bolts");

        internal static RecipeDefinition RecipePoison_DarkStab { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_DarkStab");

        internal static RecipeDefinition RecipePoison_DarkStab_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_DarkStab_Arrows");

        internal static RecipeDefinition RecipePoison_DarkStab_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_DarkStab_Bolts");

        internal static RecipeDefinition RecipePoison_DeepPain { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_DeepPain");

        internal static RecipeDefinition RecipePoison_DeepPain_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_DeepPain_Arrows");

        internal static RecipeDefinition RecipePoison_DeepPain_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_DeepPain_Bolts");

        internal static RecipeDefinition RecipePoison_GhoulsCaress { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_GhoulsCaress");

        internal static RecipeDefinition RecipePoison_GhoulsCaress_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_GhoulsCaress_Arrows");

        internal static RecipeDefinition RecipePoison_GhoulsCaress_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_GhoulsCaress_Bolts");

        internal static RecipeDefinition RecipePoison_MaraikesTorpor { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_MaraikesTorpor");

        internal static RecipeDefinition RecipePoison_MaraikesTorpor_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_MaraikesTorpor_Arrows");

        internal static RecipeDefinition RecipePoison_MaraikesTorpor_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_MaraikesTorpor_Bolts");

        internal static RecipeDefinition RecipePoison_SpiderQueensBlood { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_SpiderQueensBlood");

        internal static RecipeDefinition RecipePoison_SpiderQueensBlood_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_SpiderQueensBlood_Arrows");

        internal static RecipeDefinition RecipePoison_SpiderQueensBlood_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_SpiderQueensBlood_Bolts");

        internal static RecipeDefinition RecipePoison_TheBurden { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TheBurden");

        internal static RecipeDefinition RecipePoison_TheBurden_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TheBurden_Arrows");

        internal static RecipeDefinition RecipePoison_TheBurden_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TheBurden_Bolts");

        internal static RecipeDefinition RecipePoison_TheLongNight { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TheLongNight");

        internal static RecipeDefinition RecipePoison_TheLongNight_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TheLongNight_Arrows");

        internal static RecipeDefinition RecipePoison_TheLongNight_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TheLongNight_Bolts");

        internal static RecipeDefinition RecipePoison_TigerFang { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TigerFang");

        internal static RecipeDefinition RecipePoison_TigerFang_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TigerFang_Arrows");

        internal static RecipeDefinition RecipePoison_TigerFang_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipePoison_TigerFang_Bolts");

        internal static RecipeDefinition RecipePotionOfClimbing { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfClimbing");

        internal static RecipeDefinition RecipePotionOfGiantStrengthCloud { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfGiantStrengthCloud");

        internal static RecipeDefinition RecipePotionOfGiantStrengthFire { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfGiantStrengthFire");

        internal static RecipeDefinition RecipePotionOfGiantStrengthFrost { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfGiantStrengthFrost");

        internal static RecipeDefinition RecipePotionOfGiantStrengthHill { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfGiantStrengthHill");

        internal static RecipeDefinition RecipePotionOfGreaterHealing { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfGreaterHealing");

        internal static RecipeDefinition RecipePotionOfHealing { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfHealing");

        internal static RecipeDefinition RecipePotionOfHeroism { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfHeroism");

        internal static RecipeDefinition RecipePotionOfInvisibility { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfInvisibility");

        internal static RecipeDefinition RecipePotionOfSpeed { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfSpeed");

        internal static RecipeDefinition RecipePotionOfSuperiorHealing { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfSuperiorHealing");

        internal static RecipeDefinition RecipePotionOfWaterBreathing { get; } =
            GetDefinition<RecipeDefinition>("RecipePotionOfWaterBreathing");

        internal static RecipeDefinition RecipeScroll_L1_CharmPerson { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_CharmPerson");

        internal static RecipeDefinition RecipeScroll_L1_ColorSpray { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_ColorSpray");

        internal static RecipeDefinition RecipeScroll_L1_Command { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_Command");

        internal static RecipeDefinition RecipeScroll_L1_ComprehendLanguages { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_ComprehendLanguages");

        internal static RecipeDefinition RecipeScroll_L1_DetectEvilAndGood { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_DetectEvilAndGood");

        internal static RecipeDefinition RecipeScroll_L1_DetectMagic { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_DetectMagic");

        internal static RecipeDefinition RecipeScroll_L1_DetectPoisonAndDisease { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_DetectPoisonAndDisease");

        internal static RecipeDefinition RecipeScroll_L1_Entangle { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_Entangle");

        internal static RecipeDefinition RecipeScroll_L1_ExpeditiousRetreat { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_ExpeditiousRetreat");

        internal static RecipeDefinition RecipeScroll_L1_FaerieFire { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_FaerieFire");

        internal static RecipeDefinition RecipeScroll_L1_FeatherFall { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_FeatherFall");

        internal static RecipeDefinition RecipeScroll_L1_Grease { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_Grease");

        internal static RecipeDefinition RecipeScroll_L1_GuidingBolt { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_GuidingBolt");

        internal static RecipeDefinition RecipeScroll_L1_InflictWounds { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_InflictWounds");

        internal static RecipeDefinition RecipeScroll_L1_OfBane { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfBane");

        internal static RecipeDefinition RecipeScroll_L1_OfBless { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfBless");

        internal static RecipeDefinition RecipeScroll_L1_OfBurningHands { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfBurningHands");

        internal static RecipeDefinition RecipeScroll_L1_OfCureWounds { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfCureWounds");

        internal static RecipeDefinition RecipeScroll_L1_OfFogCloud { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfFogCloud");

        internal static RecipeDefinition RecipeScroll_L1_OfJump { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfJump");

        internal static RecipeDefinition RecipeScroll_L1_OfMageArmor { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfMageArmor");

        internal static RecipeDefinition RecipeScroll_L1_OfMagicMissile { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfMagicMissile");

        internal static RecipeDefinition RecipeScroll_L1_OfSleep { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfSleep");

        internal static RecipeDefinition RecipeScroll_L1_OfThunderwave { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_OfThunderwave");

        internal static RecipeDefinition RecipeScroll_L1_ProtectionFromEvilandGood { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_ProtectionFromEvilandGood");

        internal static RecipeDefinition RecipeScroll_L1_Shield { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_Shield");

        internal static RecipeDefinition RecipeScroll_L1_ShieldOfFaith { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L1_ShieldOfFaith");

        internal static RecipeDefinition RecipeScroll_L2_OfAcidArrow { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfAcidArrow");

        internal static RecipeDefinition RecipeScroll_L2_OfBarkskin { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfBarkskin");

        internal static RecipeDefinition RecipeScroll_L2_OfBlindness { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfBlindness");

        internal static RecipeDefinition RecipeScroll_L2_OfBlur { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfBlur");

        internal static RecipeDefinition RecipeScroll_L2_OfDarkness { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfDarkness");

        internal static RecipeDefinition RecipeScroll_L2_OfDarkvision { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfDarkvision");

        internal static RecipeDefinition RecipeScroll_L2_OfFindTraps { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfFindTraps");

        internal static RecipeDefinition RecipeScroll_L2_OfFlameblade { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfFlameblade");

        internal static RecipeDefinition RecipeScroll_L2_OfFlamingSphere { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfFlamingSphere");

        internal static RecipeDefinition RecipeScroll_L2_OfHoldPerson { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfHoldPerson");

        internal static RecipeDefinition RecipeScroll_L2_OfInvisibility { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfInvisibility");

        internal static RecipeDefinition RecipeScroll_L2_OfKnock { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfKnock");

        internal static RecipeDefinition RecipeScroll_L2_OfLesserRestoration { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfLesserRestoration");

        internal static RecipeDefinition RecipeScroll_L2_OfLevitate { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfLevitate");

        internal static RecipeDefinition RecipeScroll_L2_OfMagicWeapon { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfMagicWeapon");

        internal static RecipeDefinition RecipeScroll_L2_OfMirrorImage { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfMirrorImage");

        internal static RecipeDefinition RecipeScroll_L2_OfMistyStep { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfMistyStep");

        internal static RecipeDefinition RecipeScroll_L2_OfPassWithoutTrace { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfPassWithoutTrace");

        internal static RecipeDefinition RecipeScroll_L2_OfRayOfEnfeeblement { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfRayOfEnfeeblement");

        internal static RecipeDefinition RecipeScroll_L2_OfScorchingRay { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfScorchingRay");

        internal static RecipeDefinition RecipeScroll_L2_OfSeeInvisibility { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfSeeInvisibility");

        internal static RecipeDefinition RecipeScroll_L2_OfShatter { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfShatter");

        internal static RecipeDefinition RecipeScroll_L2_OfSilence { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfSilence");

        internal static RecipeDefinition RecipeScroll_L2_OfSpiderClimb { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfSpiderClimb");

        internal static RecipeDefinition RecipeScroll_L2_OfSpiritualWeapon { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfSpiritualWeapon");

        internal static RecipeDefinition RecipeScroll_L2_OfWardingBonds { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L2_OfWardingBonds");

        internal static RecipeDefinition RecipeScroll_L3_OfConjureAnimals { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfConjureAnimals");

        internal static RecipeDefinition RecipeScroll_L3_OfCounterspell { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfCounterspell");

        internal static RecipeDefinition RecipeScroll_L3_OfDaylight { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfDaylight");

        internal static RecipeDefinition RecipeScroll_L3_OfFear { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfFear");

        internal static RecipeDefinition RecipeScroll_L3_OfFireball { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfFireball");

        internal static RecipeDefinition RecipeScroll_L3_OfFly { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfFly");

        internal static RecipeDefinition RecipeScroll_L3_OfHaste { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfHaste");

        internal static RecipeDefinition RecipeScroll_L3_OfHypnoticPattern { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfHypnoticPattern");

        internal static RecipeDefinition RecipeScroll_L3_OfLightningBolt { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfLightningBolt");

        internal static RecipeDefinition RecipeScroll_L3_OfMassHealingWord { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfMassHealingWord");

        internal static RecipeDefinition RecipeScroll_L3_OfProtectionFromEnergy { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfProtectionFromEnergy");

        internal static RecipeDefinition RecipeScroll_L3_OfRemoveCurse { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfRemoveCurse");

        internal static RecipeDefinition RecipeScroll_L3_OfRevivify { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfRevivify");

        internal static RecipeDefinition RecipeScroll_L3_OfSleetStorm { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfSleetStorm");

        internal static RecipeDefinition RecipeScroll_L3_OfSlow { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfSlow");

        internal static RecipeDefinition RecipeScroll_L3_OfSpiritGuardians { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfSpiritGuardians");

        internal static RecipeDefinition RecipeScroll_L3_OfStinkingCloud { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfStinkingCloud");

        internal static RecipeDefinition RecipeScroll_L3_OfTongues { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfTongues");

        internal static RecipeDefinition RecipeScroll_L3_OfVampiricTouch { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L3_OfVampiricTouch");

        internal static RecipeDefinition RecipeScroll_L4__Banishement { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_ Banishement");

        internal static RecipeDefinition RecipeScroll_L4__Blight { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_ Blight");

        internal static RecipeDefinition RecipeScroll_L4__Confusion { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_ Confusion");

        internal static RecipeDefinition RecipeScroll_L4__ConjureMinorElementals { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_ ConjureMinorElementals");

        internal static RecipeDefinition RecipeScroll_L4__Deathward { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_ Deathward");

        internal static RecipeDefinition RecipeScroll_L4_BlackTentacles { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_BlackTentacles");

        internal static RecipeDefinition RecipeScroll_L4_DimensionDoor { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_DimensionDoor");

        internal static RecipeDefinition RecipeScroll_L4_FireShield { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_FireShield");

        internal static RecipeDefinition RecipeScroll_L4_FreedomOfMovement { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_FreedomOfMovement");

        internal static RecipeDefinition RecipeScroll_L4_GreaterInvisibility { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_GreaterInvisibility");

        internal static RecipeDefinition RecipeScroll_L4_GuardianOFFaith { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_GuardianOFFaith");

        internal static RecipeDefinition RecipeScroll_L4_IceStorm { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_IceStorm");

        internal static RecipeDefinition RecipeScroll_L4_PhantasmalKiller { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_PhantasmalKiller");

        internal static RecipeDefinition RecipeScroll_L4_StoneSkin { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_StoneSkin");

        internal static RecipeDefinition RecipeScroll_L4_WallOfFire { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L4_WallOfFire");

        internal static RecipeDefinition RecipeScroll_L5_CloudKill { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_CloudKill");

        internal static RecipeDefinition RecipeScroll_L5_ConeOfCold { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_ConeOfCold");

        internal static RecipeDefinition RecipeScroll_L5_ConjureElemental { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_ConjureElemental");

        internal static RecipeDefinition RecipeScroll_L5_Contagion { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_Contagion");

        internal static RecipeDefinition RecipeScroll_L5_DispelEvilAndGood { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_DispelEvilAndGood");

        internal static RecipeDefinition RecipeScroll_L5_DominatePerson { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_DominatePerson");

        internal static RecipeDefinition RecipeScroll_L5_FlameStrike { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_FlameStrike");

        internal static RecipeDefinition RecipeScroll_L5_GreaterRestoration { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_GreaterRestoration");

        internal static RecipeDefinition RecipeScroll_L5_HoldMonster { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_HoldMonster");

        internal static RecipeDefinition RecipeScroll_L5_InsectPlague { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_InsectPlague");

        internal static RecipeDefinition RecipeScroll_L5_MassCureWounds { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_MassCureWounds");

        internal static RecipeDefinition RecipeScroll_L5_MindTwist { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_MindTwist");

        internal static RecipeDefinition RecipeScroll_L5_ScrollRaiseDead { get; } =
            GetDefinition<RecipeDefinition>("RecipeScroll_L5_ScrollRaiseDead");
    }

    internal static class RoomBlueprints
    {
    }

    internal static class SchoolOfMagicDefinitions
    {
        internal static SchoolOfMagicDefinition SchoolAbjuration { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolAbjuration");

        internal static SchoolOfMagicDefinition SchoolConjuration { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolConjuration");

        internal static SchoolOfMagicDefinition SchoolDivination { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolDivination");

        internal static SchoolOfMagicDefinition SchoolEnchantment { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolEnchantment");

        internal static SchoolOfMagicDefinition SchoolEvocation { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolEvocation");

        internal static SchoolOfMagicDefinition SchoolIllusion { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolIllusion");

        internal static SchoolOfMagicDefinition SchoolNecromancy { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolNecromancy");

        internal static SchoolOfMagicDefinition SchoolTransmutation { get; } =
            GetDefinition<SchoolOfMagicDefinition>("SchoolTransmutation");
    }

    internal static class SkillDefinitions
    {
        internal static SkillDefinition Acrobatics { get; } = GetDefinition<SkillDefinition>("Acrobatics");
        internal static SkillDefinition AnimalHandling { get; } = GetDefinition<SkillDefinition>("AnimalHandling");
        internal static SkillDefinition Arcana { get; } = GetDefinition<SkillDefinition>("Arcana");
        internal static SkillDefinition Athletics { get; } = GetDefinition<SkillDefinition>("Athletics");
        internal static SkillDefinition Deception { get; } = GetDefinition<SkillDefinition>("Deception");
        internal static SkillDefinition History { get; } = GetDefinition<SkillDefinition>("History");
        internal static SkillDefinition Insight { get; } = GetDefinition<SkillDefinition>("Insight");
        internal static SkillDefinition Intimidation { get; } = GetDefinition<SkillDefinition>("Intimidation");
        internal static SkillDefinition Investigation { get; } = GetDefinition<SkillDefinition>("Investigation");
        internal static SkillDefinition Medecine { get; } = GetDefinition<SkillDefinition>("Medecine");
        internal static SkillDefinition Nature { get; } = GetDefinition<SkillDefinition>("Nature");
        internal static SkillDefinition Perception { get; } = GetDefinition<SkillDefinition>("Perception");
        internal static SkillDefinition Performance { get; } = GetDefinition<SkillDefinition>("Performance");
        internal static SkillDefinition Persuasion { get; } = GetDefinition<SkillDefinition>("Persuasion");
        internal static SkillDefinition Religion { get; } = GetDefinition<SkillDefinition>("Religion");
        internal static SkillDefinition SleightOfHand { get; } = GetDefinition<SkillDefinition>("SleightOfHand");
        internal static SkillDefinition Stealth { get; } = GetDefinition<SkillDefinition>("Stealth");
        internal static SkillDefinition Survival { get; } = GetDefinition<SkillDefinition>("Survival");
    }

    internal static class SlotTypeDefinitions
    {
        internal static SlotTypeDefinition AmmunitionSlot { get; } =
            GetDefinition<SlotTypeDefinition>("AmmunitionSlot");

        internal static SlotTypeDefinition BackSlot { get; } = GetDefinition<SlotTypeDefinition>("BackSlot");
        internal static SlotTypeDefinition BeltSlot { get; } = GetDefinition<SlotTypeDefinition>("BeltSlot");
        internal static SlotTypeDefinition ContainerSlot { get; } = GetDefinition<SlotTypeDefinition>("ContainerSlot");
        internal static SlotTypeDefinition FeetSlot { get; } = GetDefinition<SlotTypeDefinition>("FeetSlot");
        internal static SlotTypeDefinition FingerSlot { get; } = GetDefinition<SlotTypeDefinition>("FingerSlot");
        internal static SlotTypeDefinition GlovesSlot { get; } = GetDefinition<SlotTypeDefinition>("GlovesSlot");
        internal static SlotTypeDefinition HeadSlot { get; } = GetDefinition<SlotTypeDefinition>("HeadSlot");
        internal static SlotTypeDefinition MainHandSlot { get; } = GetDefinition<SlotTypeDefinition>("MainHandSlot");
        internal static SlotTypeDefinition NeckSlot { get; } = GetDefinition<SlotTypeDefinition>("NeckSlot");
        internal static SlotTypeDefinition OffHandSlot { get; } = GetDefinition<SlotTypeDefinition>("OffHandSlot");
        internal static SlotTypeDefinition ShouldersSlot { get; } = GetDefinition<SlotTypeDefinition>("ShouldersSlot");
        internal static SlotTypeDefinition TabardSlot { get; } = GetDefinition<SlotTypeDefinition>("TabardSlot");

        internal static SlotTypeDefinition TorchHolderSlot { get; } =
            GetDefinition<SlotTypeDefinition>("TorchHolderSlot");

        internal static SlotTypeDefinition TorsoSlot { get; } = GetDefinition<SlotTypeDefinition>("TorsoSlot");
        internal static SlotTypeDefinition UtilitySlot { get; } = GetDefinition<SlotTypeDefinition>("UtilitySlot");
        internal static SlotTypeDefinition WristsSlot { get; } = GetDefinition<SlotTypeDefinition>("WristsSlot");
    }

    internal static class SoundbanksDefinitions
    {
        internal static SoundbanksDefinition SoundbanksDefinition { get; } =
            GetDefinition<SoundbanksDefinition>("SoundbanksDefinition");
    }

    internal static class SpellDefinitions
    {
        internal static SpellDefinition AcidArrow { get; } = GetDefinition<SpellDefinition>("AcidArrow");
        internal static SpellDefinition AcidSplash { get; } = GetDefinition<SpellDefinition>("AcidSplash");
        internal static SpellDefinition Aid { get; } = GetDefinition<SpellDefinition>("Aid");
        internal static SpellDefinition AnimalFriendship { get; } = GetDefinition<SpellDefinition>("AnimalFriendship");
        internal static SpellDefinition AnimalShapes { get; } = GetDefinition<SpellDefinition>("AnimalShapes");
        internal static SpellDefinition AnimateDead { get; } = GetDefinition<SpellDefinition>("AnimateDead");
        internal static SpellDefinition AnnoyingBee { get; } = GetDefinition<SpellDefinition>("AnnoyingBee");
        internal static SpellDefinition ArcaneSword { get; } = GetDefinition<SpellDefinition>("ArcaneSword");
        internal static SpellDefinition Bane { get; } = GetDefinition<SpellDefinition>("Bane");
        internal static SpellDefinition Banishment { get; } = GetDefinition<SpellDefinition>("Banishment");
        internal static SpellDefinition Barkskin { get; } = GetDefinition<SpellDefinition>("Barkskin");
        internal static SpellDefinition BeaconOfHope { get; } = GetDefinition<SpellDefinition>("BeaconOfHope");
        internal static SpellDefinition BestowCurse { get; } = GetDefinition<SpellDefinition>("BestowCurse");

        internal static SpellDefinition BestowCurseAbilityCharisma { get; } =
            GetDefinition<SpellDefinition>("BestowCurseAbilityCharisma");

        internal static SpellDefinition BestowCurseAbilityConstitution { get; } =
            GetDefinition<SpellDefinition>("BestowCurseAbilityConstitution");

        internal static SpellDefinition BestowCurseAbilityDexterity { get; } =
            GetDefinition<SpellDefinition>("BestowCurseAbilityDexterity");

        internal static SpellDefinition BestowCurseAbilityIntelligence { get; } =
            GetDefinition<SpellDefinition>("BestowCurseAbilityIntelligence");

        internal static SpellDefinition BestowCurseAbilityStrength { get; } =
            GetDefinition<SpellDefinition>("BestowCurseAbilityStrength");

        internal static SpellDefinition BestowCurseAbilityWisdom { get; } =
            GetDefinition<SpellDefinition>("BestowCurseAbilityWisdom");

        internal static SpellDefinition BestowCurseOnActionTurn { get; } =
            GetDefinition<SpellDefinition>("BestowCurseOnActionTurn");

        internal static SpellDefinition BestowCurseOnAttackRoll { get; } =
            GetDefinition<SpellDefinition>("BestowCurseOnAttackRoll");

        internal static SpellDefinition BestowCurseOnDamageAddNecrotic { get; } =
            GetDefinition<SpellDefinition>("BestowCurseOnDamageAddNecrotic");

        internal static SpellDefinition BlackTentacles { get; } = GetDefinition<SpellDefinition>("BlackTentacles");
        internal static SpellDefinition BladeBarrier { get; } = GetDefinition<SpellDefinition>("BladeBarrier");

        internal static SpellDefinition BladeBarrierWallLine { get; } =
            GetDefinition<SpellDefinition>("BladeBarrierWallLine");

        internal static SpellDefinition BladeBarrierWallRing { get; } =
            GetDefinition<SpellDefinition>("BladeBarrierWallRing");

        internal static SpellDefinition Bless { get; } = GetDefinition<SpellDefinition>("Bless");
        internal static SpellDefinition Blight { get; } = GetDefinition<SpellDefinition>("Blight");
        internal static SpellDefinition Blindness { get; } = GetDefinition<SpellDefinition>("Blindness");
        internal static SpellDefinition Blur { get; } = GetDefinition<SpellDefinition>("Blur");
        internal static SpellDefinition BrandingSmite { get; } = GetDefinition<SpellDefinition>("BrandingSmite");
        internal static SpellDefinition BurningHands { get; } = GetDefinition<SpellDefinition>("BurningHands");
        internal static SpellDefinition BurningHands_B { get; } = GetDefinition<SpellDefinition>("BurningHands_B");
        internal static SpellDefinition CallLightning { get; } = GetDefinition<SpellDefinition>("CallLightning");
        internal static SpellDefinition CalmEmotions { get; } = GetDefinition<SpellDefinition>("CalmEmotions");

        internal static SpellDefinition CalmEmotionsOnAlly { get; } =
            GetDefinition<SpellDefinition>("CalmEmotionsOnAlly");

        internal static SpellDefinition CalmEmotionsOnEnemy { get; } =
            GetDefinition<SpellDefinition>("CalmEmotionsOnEnemy");

        internal static SpellDefinition ChainLightning { get; } = GetDefinition<SpellDefinition>("ChainLightning");
        internal static SpellDefinition CharmPerson { get; } = GetDefinition<SpellDefinition>("CharmPerson");
        internal static SpellDefinition ChillTouch { get; } = GetDefinition<SpellDefinition>("ChillTouch");
        internal static SpellDefinition CircleOfDeath { get; } = GetDefinition<SpellDefinition>("CircleOfDeath");
        internal static SpellDefinition CloudKill { get; } = GetDefinition<SpellDefinition>("CloudKill");
        internal static SpellDefinition ColorSpray { get; } = GetDefinition<SpellDefinition>("ColorSpray");
        internal static SpellDefinition Command { get; } = GetDefinition<SpellDefinition>("Command");
        internal static SpellDefinition CommandApproach { get; } = GetDefinition<SpellDefinition>("CommandApproach");
        internal static SpellDefinition CommandDrop { get; } = GetDefinition<SpellDefinition>("CommandDrop");
        internal static SpellDefinition CommandFlee { get; } = GetDefinition<SpellDefinition>("CommandFlee");
        internal static SpellDefinition CommandGrovel { get; } = GetDefinition<SpellDefinition>("CommandGrovel");
        internal static SpellDefinition CommandHalt { get; } = GetDefinition<SpellDefinition>("CommandHalt");

        internal static SpellDefinition ComprehendLanguages { get; } =
            GetDefinition<SpellDefinition>("ComprehendLanguages");

        internal static SpellDefinition ConeOfCold { get; } = GetDefinition<SpellDefinition>("ConeOfCold");
        internal static SpellDefinition Confusion { get; } = GetDefinition<SpellDefinition>("Confusion");
        internal static SpellDefinition ConjureAnimals { get; } = GetDefinition<SpellDefinition>("ConjureAnimals");

        internal static SpellDefinition ConjureAnimalsFourBeasts { get; } =
            GetDefinition<SpellDefinition>("ConjureAnimalsFourBeasts");

        internal static SpellDefinition ConjureAnimalsOneBeast { get; } =
            GetDefinition<SpellDefinition>("ConjureAnimalsOneBeast");

        internal static SpellDefinition ConjureAnimalsTwoBeasts { get; } =
            GetDefinition<SpellDefinition>("ConjureAnimalsTwoBeasts");

        internal static SpellDefinition ConjureCelestial { get; } = GetDefinition<SpellDefinition>("ConjureCelestial");

        internal static SpellDefinition ConjureCelestialCouatl { get; } =
            GetDefinition<SpellDefinition>("ConjureCelestialCouatl");

        internal static SpellDefinition ConjureCelestialKutkartal { get; } =
            GetDefinition<SpellDefinition>("ConjureCelestialKutkartal");

        internal static SpellDefinition ConjureCelestialMelek { get; } =
            GetDefinition<SpellDefinition>("ConjureCelestialMelek");

        internal static SpellDefinition ConjureElemental { get; } = GetDefinition<SpellDefinition>("ConjureElemental");

        internal static SpellDefinition ConjureElementalAir { get; } =
            GetDefinition<SpellDefinition>("ConjureElementalAir");

        internal static SpellDefinition ConjureElementalEarth { get; } =
            GetDefinition<SpellDefinition>("ConjureElementalEarth");

        internal static SpellDefinition ConjureElementalFire { get; } =
            GetDefinition<SpellDefinition>("ConjureElementalFire");

        internal static SpellDefinition ConjureFey { get; } = GetDefinition<SpellDefinition>("ConjureFey");
        internal static SpellDefinition ConjureFey_Ape { get; } = GetDefinition<SpellDefinition>("ConjureFey_Ape");
        internal static SpellDefinition ConjureFey_Bear { get; } = GetDefinition<SpellDefinition>("ConjureFey_Bear");
        internal static SpellDefinition ConjureFey_Dryad { get; } = GetDefinition<SpellDefinition>("ConjureFey_Dryad");
        internal static SpellDefinition ConjureFey_Eagle { get; } = GetDefinition<SpellDefinition>("ConjureFey_Eagle");

        internal static SpellDefinition ConjureFey_GreenHag { get; } =
            GetDefinition<SpellDefinition>("ConjureFey_GreenHag");

        internal static SpellDefinition ConjureFey_Wolf { get; } = GetDefinition<SpellDefinition>("ConjureFey_Wolf");

        internal static SpellDefinition ConjureGoblinoids { get; } =
            GetDefinition<SpellDefinition>("ConjureGoblinoids");

        internal static SpellDefinition ConjureMinorElementals { get; } =
            GetDefinition<SpellDefinition>("ConjureMinorElementals");

        internal static SpellDefinition ConjureMinorElementalsFour { get; } =
            GetDefinition<SpellDefinition>("ConjureMinorElementalsFour");

        internal static SpellDefinition ConjureMinorElementalsOne { get; } =
            GetDefinition<SpellDefinition>("ConjureMinorElementalsOne");

        internal static SpellDefinition ConjureMinorElementalsOne_b { get; } =
            GetDefinition<SpellDefinition>("ConjureMinorElementalsOne_b");

        internal static SpellDefinition ConjureMinorElementalsTwo { get; } =
            GetDefinition<SpellDefinition>("ConjureMinorElementalsTwo");

        internal static SpellDefinition Contagion { get; } = GetDefinition<SpellDefinition>("Contagion");

        internal static SpellDefinition ContagionBlindingSickness { get; } =
            GetDefinition<SpellDefinition>("ContagionBlindingSickness");

        internal static SpellDefinition ContagionFilthFever { get; } =
            GetDefinition<SpellDefinition>("ContagionFilthFever");

        internal static SpellDefinition ContagionFleshRot { get; } =
            GetDefinition<SpellDefinition>("ContagionFleshRot");

        internal static SpellDefinition ContagionMindfire { get; } =
            GetDefinition<SpellDefinition>("ContagionMindfire");

        internal static SpellDefinition ContagionSeizure { get; } = GetDefinition<SpellDefinition>("ContagionSeizure");

        internal static SpellDefinition ContagionSlimyDoom { get; } =
            GetDefinition<SpellDefinition>("ContagionSlimyDoom");

        internal static SpellDefinition Counterspell { get; } = GetDefinition<SpellDefinition>("Counterspell");
        internal static SpellDefinition CreateFood { get; } = GetDefinition<SpellDefinition>("CreateFood");
        internal static SpellDefinition CureWounds { get; } = GetDefinition<SpellDefinition>("CureWounds");
        internal static SpellDefinition DancingLights { get; } = GetDefinition<SpellDefinition>("DancingLights");
        internal static SpellDefinition Darkness { get; } = GetDefinition<SpellDefinition>("Darkness");
        internal static SpellDefinition Darkvision { get; } = GetDefinition<SpellDefinition>("Darkvision");
        internal static SpellDefinition Daylight { get; } = GetDefinition<SpellDefinition>("Daylight");
        internal static SpellDefinition Dazzle { get; } = GetDefinition<SpellDefinition>("Dazzle");
        internal static SpellDefinition DeathWard { get; } = GetDefinition<SpellDefinition>("DeathWard");

        internal static SpellDefinition DelayedBlastFireball { get; } =
            GetDefinition<SpellDefinition>("DelayedBlastFireball");

        internal static SpellDefinition DetectEvilAndGood { get; } =
            GetDefinition<SpellDefinition>("DetectEvilAndGood");

        internal static SpellDefinition DetectMagic { get; } = GetDefinition<SpellDefinition>("DetectMagic");

        internal static SpellDefinition DetectPoisonAndDisease { get; } =
            GetDefinition<SpellDefinition>("DetectPoisonAndDisease");

        internal static SpellDefinition DimensionDoor { get; } = GetDefinition<SpellDefinition>("DimensionDoor");
        internal static SpellDefinition Disintegrate { get; } = GetDefinition<SpellDefinition>("Disintegrate");

        internal static SpellDefinition DispelEvilAndGood { get; } =
            GetDefinition<SpellDefinition>("DispelEvilAndGood");

        internal static SpellDefinition DispelMagic { get; } = GetDefinition<SpellDefinition>("DispelMagic");
        internal static SpellDefinition DivineBlade { get; } = GetDefinition<SpellDefinition>("DivineBlade");
        internal static SpellDefinition DivineFavor { get; } = GetDefinition<SpellDefinition>("DivineFavor");
        internal static SpellDefinition DivineWord { get; } = GetDefinition<SpellDefinition>("DivineWord");
        internal static SpellDefinition DominateBeast { get; } = GetDefinition<SpellDefinition>("DominateBeast");
        internal static SpellDefinition DominateMonster { get; } = GetDefinition<SpellDefinition>("DominateMonster");
        internal static SpellDefinition DominatePerson { get; } = GetDefinition<SpellDefinition>("DominatePerson");
        internal static SpellDefinition DreadfulOmen { get; } = GetDefinition<SpellDefinition>("DreadfulOmen");
        internal static SpellDefinition Earthquake { get; } = GetDefinition<SpellDefinition>("Earthquake");
        internal static SpellDefinition EldritchBlast { get; } = GetDefinition<SpellDefinition>("EldritchBlast");
        internal static SpellDefinition EnhanceAbility { get; } = GetDefinition<SpellDefinition>("EnhanceAbility");

        internal static SpellDefinition EnhanceAbilityBearsEndurance { get; } =
            GetDefinition<SpellDefinition>("EnhanceAbilityBearsEndurance");

        internal static SpellDefinition EnhanceAbilityBullsStrength { get; } =
            GetDefinition<SpellDefinition>("EnhanceAbilityBullsStrength");

        internal static SpellDefinition EnhanceAbilityCatsGrace { get; } =
            GetDefinition<SpellDefinition>("EnhanceAbilityCatsGrace");

        internal static SpellDefinition EnhanceAbilityEaglesSplendor { get; } =
            GetDefinition<SpellDefinition>("EnhanceAbilityEaglesSplendor");

        internal static SpellDefinition EnhanceAbilityFoxsCunning { get; } =
            GetDefinition<SpellDefinition>("EnhanceAbilityFoxsCunning");

        internal static SpellDefinition EnhanceAbilityOwlsWisdom { get; } =
            GetDefinition<SpellDefinition>("EnhanceAbilityOwlsWisdom");

        internal static SpellDefinition Entangle { get; } = GetDefinition<SpellDefinition>("Entangle");

        internal static SpellDefinition ExpeditiousRetreat { get; } =
            GetDefinition<SpellDefinition>("ExpeditiousRetreat");

        internal static SpellDefinition Eyebite { get; } = GetDefinition<SpellDefinition>("Eyebite");
        internal static SpellDefinition EyebiteAsleep { get; } = GetDefinition<SpellDefinition>("EyebiteAsleep");
        internal static SpellDefinition EyebitePanicked { get; } = GetDefinition<SpellDefinition>("EyebitePanicked");
        internal static SpellDefinition EyebiteSickened { get; } = GetDefinition<SpellDefinition>("EyebiteSickened");
        internal static SpellDefinition FaerieFire { get; } = GetDefinition<SpellDefinition>("FaerieFire");
        internal static SpellDefinition FalseLife { get; } = GetDefinition<SpellDefinition>("FalseLife");
        internal static SpellDefinition Fear { get; } = GetDefinition<SpellDefinition>("Fear");
        internal static SpellDefinition FeatherFall { get; } = GetDefinition<SpellDefinition>("FeatherFall");
        internal static SpellDefinition Feeblemind { get; } = GetDefinition<SpellDefinition>("Feeblemind");
        internal static SpellDefinition FindTraps { get; } = GetDefinition<SpellDefinition>("FindTraps");
        internal static SpellDefinition FingerOfDeath { get; } = GetDefinition<SpellDefinition>("FingerOfDeath");
        internal static SpellDefinition Fireball { get; } = GetDefinition<SpellDefinition>("Fireball");
        internal static SpellDefinition FireBolt { get; } = GetDefinition<SpellDefinition>("FireBolt");
        internal static SpellDefinition FireShield { get; } = GetDefinition<SpellDefinition>("FireShield");
        internal static SpellDefinition FireShieldCold { get; } = GetDefinition<SpellDefinition>("FireShieldCold");
        internal static SpellDefinition FireShieldWarm { get; } = GetDefinition<SpellDefinition>("FireShieldWarm");
        internal static SpellDefinition FireStorm { get; } = GetDefinition<SpellDefinition>("FireStorm");
        internal static SpellDefinition FlameBlade { get; } = GetDefinition<SpellDefinition>("FlameBlade");
        internal static SpellDefinition FlameStrike { get; } = GetDefinition<SpellDefinition>("FlameStrike");
        internal static SpellDefinition FlamingSphere { get; } = GetDefinition<SpellDefinition>("FlamingSphere");
        internal static SpellDefinition Fly { get; } = GetDefinition<SpellDefinition>("Fly");
        internal static SpellDefinition FogCloud { get; } = GetDefinition<SpellDefinition>("FogCloud");

        internal static SpellDefinition FreedomOfMovement { get; } =
            GetDefinition<SpellDefinition>("FreedomOfMovement");

        internal static SpellDefinition FreezingSphere { get; } = GetDefinition<SpellDefinition>("FreezingSphere");
        internal static SpellDefinition GiantInsect { get; } = GetDefinition<SpellDefinition>("GiantInsect");

        internal static SpellDefinition GlobeOfInvulnerability { get; } =
            GetDefinition<SpellDefinition>("GlobeOfInvulnerability");

        internal static SpellDefinition Goodberry { get; } = GetDefinition<SpellDefinition>("Goodberry");
        internal static SpellDefinition GravitySlam { get; } = GetDefinition<SpellDefinition>("GravitySlam");
        internal static SpellDefinition Grease { get; } = GetDefinition<SpellDefinition>("Grease");

        internal static SpellDefinition GreaterInvisibility { get; } =
            GetDefinition<SpellDefinition>("GreaterInvisibility");

        internal static SpellDefinition GreaterRestoration { get; } =
            GetDefinition<SpellDefinition>("GreaterRestoration");

        internal static SpellDefinition GuardianOfFaith { get; } = GetDefinition<SpellDefinition>("GuardianOfFaith");
        internal static SpellDefinition Guidance { get; } = GetDefinition<SpellDefinition>("Guidance");
        internal static SpellDefinition GuidingBolt { get; } = GetDefinition<SpellDefinition>("GuidingBolt");
        internal static SpellDefinition Harm { get; } = GetDefinition<SpellDefinition>("Harm");
        internal static SpellDefinition Haste { get; } = GetDefinition<SpellDefinition>("Haste");
        internal static SpellDefinition Heal { get; } = GetDefinition<SpellDefinition>("Heal");
        internal static SpellDefinition HealingWord { get; } = GetDefinition<SpellDefinition>("HealingWord");
        internal static SpellDefinition HeatMetal { get; } = GetDefinition<SpellDefinition>("HeatMetal");
        internal static SpellDefinition HellishRebuke { get; } = GetDefinition<SpellDefinition>("HellishRebuke");

        internal static SpellDefinition HellishRebukeTiefling { get; } =
            GetDefinition<SpellDefinition>("HellishRebukeTiefling");

        internal static SpellDefinition HeroesFeast { get; } = GetDefinition<SpellDefinition>("HeroesFeast");
        internal static SpellDefinition Heroism { get; } = GetDefinition<SpellDefinition>("Heroism");
        internal static SpellDefinition HideousLaughter { get; } = GetDefinition<SpellDefinition>("HideousLaughter");
        internal static SpellDefinition Hilarity { get; } = GetDefinition<SpellDefinition>("Hilarity");
        internal static SpellDefinition HoldMonster { get; } = GetDefinition<SpellDefinition>("HoldMonster");

        internal static SpellDefinition HoldMonsterInvocationChainsCarceri { get; } =
            GetDefinition<SpellDefinition>("HoldMonsterInvocationChainsCarceri");

        internal static SpellDefinition HoldPerson { get; } = GetDefinition<SpellDefinition>("HoldPerson");
        internal static SpellDefinition HolyAura { get; } = GetDefinition<SpellDefinition>("HolyAura");
        internal static SpellDefinition HuntersMark { get; } = GetDefinition<SpellDefinition>("HuntersMark");
        internal static SpellDefinition HypnoticPattern { get; } = GetDefinition<SpellDefinition>("HypnoticPattern");
        internal static SpellDefinition IceStorm { get; } = GetDefinition<SpellDefinition>("IceStorm");
        internal static SpellDefinition Identify { get; } = GetDefinition<SpellDefinition>("Identify");

        internal static SpellDefinition IdentifyCreatures { get; } =
            GetDefinition<SpellDefinition>("IdentifyCreatures");

        internal static SpellDefinition IncendiaryCloud { get; } = GetDefinition<SpellDefinition>("IncendiaryCloud");
        internal static SpellDefinition InflictWounds { get; } = GetDefinition<SpellDefinition>("InflictWounds");
        internal static SpellDefinition InsectPlague { get; } = GetDefinition<SpellDefinition>("InsectPlague");
        internal static SpellDefinition Invisibility { get; } = GetDefinition<SpellDefinition>("Invisibility");
        internal static SpellDefinition Jump { get; } = GetDefinition<SpellDefinition>("Jump");

        internal static SpellDefinition JumpOtherworldlyLeap { get; } =
            GetDefinition<SpellDefinition>("JumpOtherworldlyLeap");

        internal static SpellDefinition Knock { get; } = GetDefinition<SpellDefinition>("Knock");

        internal static SpellDefinition LesserRestoration { get; } =
            GetDefinition<SpellDefinition>("LesserRestoration");

        internal static SpellDefinition Levitate { get; } = GetDefinition<SpellDefinition>("Levitate");
        internal static SpellDefinition LevitateBoots { get; } = GetDefinition<SpellDefinition>("LevitateBoots");
        internal static SpellDefinition Light { get; } = GetDefinition<SpellDefinition>("Light");

        internal static SpellDefinition Light_Monk_NoFocus { get; } =
            GetDefinition<SpellDefinition>("Light_Monk_NoFocus");

        internal static SpellDefinition LightningBolt { get; } = GetDefinition<SpellDefinition>("LightningBolt");
        internal static SpellDefinition Longstrider { get; } = GetDefinition<SpellDefinition>("Longstrider");
        internal static SpellDefinition MageArmor { get; } = GetDefinition<SpellDefinition>("MageArmor");

        internal static SpellDefinition MageArmorInvocationArmorShadows { get; } =
            GetDefinition<SpellDefinition>("MageArmorInvocationArmorShadows");

        internal static SpellDefinition MagicMissile { get; } = GetDefinition<SpellDefinition>("MagicMissile");
        internal static SpellDefinition MagicWeapon { get; } = GetDefinition<SpellDefinition>("MagicWeapon");
        internal static SpellDefinition Malediction { get; } = GetDefinition<SpellDefinition>("Malediction");
        internal static SpellDefinition MassCureWounds { get; } = GetDefinition<SpellDefinition>("MassCureWounds");
        internal static SpellDefinition MassHealingWord { get; } = GetDefinition<SpellDefinition>("MassHealingWord");
        internal static SpellDefinition Maze { get; } = GetDefinition<SpellDefinition>("Maze");
        internal static SpellDefinition MindTwist { get; } = GetDefinition<SpellDefinition>("MindTwist");
        internal static SpellDefinition MirrorImage { get; } = GetDefinition<SpellDefinition>("MirrorImage");
        internal static SpellDefinition MistyStep { get; } = GetDefinition<SpellDefinition>("MistyStep");
        internal static SpellDefinition MoonBeam { get; } = GetDefinition<SpellDefinition>("MoonBeam");
        internal static SpellDefinition PassWithoutTrace { get; } = GetDefinition<SpellDefinition>("PassWithoutTrace");
        internal static SpellDefinition PhantasmalKiller { get; } = GetDefinition<SpellDefinition>("PhantasmalKiller");
        internal static SpellDefinition PoisonSpray { get; } = GetDefinition<SpellDefinition>("PoisonSpray");
        internal static SpellDefinition PowerWordStun { get; } = GetDefinition<SpellDefinition>("PowerWordStun");
        internal static SpellDefinition PrayerOfHealing { get; } = GetDefinition<SpellDefinition>("PrayerOfHealing");
        internal static SpellDefinition PrismaticSpray { get; } = GetDefinition<SpellDefinition>("PrismaticSpray");
        internal static SpellDefinition ProduceFlame { get; } = GetDefinition<SpellDefinition>("ProduceFlame");
        internal static SpellDefinition ProduceFlameHold { get; } = GetDefinition<SpellDefinition>("ProduceFlameHold");
        internal static SpellDefinition ProduceFlameHurl { get; } = GetDefinition<SpellDefinition>("ProduceFlameHurl");

        internal static SpellDefinition ProtectionFromEnergy { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEnergy");

        internal static SpellDefinition ProtectionFromEnergyAcid { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEnergyAcid");

        internal static SpellDefinition ProtectionFromEnergyCold { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEnergyCold");

        internal static SpellDefinition ProtectionFromEnergyFire { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEnergyFire");

        internal static SpellDefinition ProtectionFromEnergyLightning { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEnergyLightning");

        internal static SpellDefinition ProtectionFromEnergyThunder { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEnergyThunder");

        internal static SpellDefinition ProtectionFromEvilGood { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromEvilGood");

        internal static SpellDefinition ProtectionFromPoison { get; } =
            GetDefinition<SpellDefinition>("ProtectionFromPoison");

        internal static SpellDefinition RaiseDead { get; } = GetDefinition<SpellDefinition>("RaiseDead");

        internal static SpellDefinition RayOfEnfeeblement { get; } =
            GetDefinition<SpellDefinition>("RayOfEnfeeblement");

        internal static SpellDefinition RayOfFrost { get; } = GetDefinition<SpellDefinition>("RayOfFrost");
        internal static SpellDefinition Regenerate { get; } = GetDefinition<SpellDefinition>("Regenerate");
        internal static SpellDefinition RemoveCurse { get; } = GetDefinition<SpellDefinition>("RemoveCurse");
        internal static SpellDefinition Resistance { get; } = GetDefinition<SpellDefinition>("Resistance");
        internal static SpellDefinition Resurrection { get; } = GetDefinition<SpellDefinition>("Resurrection");
        internal static SpellDefinition Revivify { get; } = GetDefinition<SpellDefinition>("Revivify");
        internal static SpellDefinition SacredFlame { get; } = GetDefinition<SpellDefinition>("SacredFlame");
        internal static SpellDefinition SacredFlame_B { get; } = GetDefinition<SpellDefinition>("SacredFlame_B");
        internal static SpellDefinition ScorchingRay { get; } = GetDefinition<SpellDefinition>("ScorchingRay");
        internal static SpellDefinition SeeInvisibility { get; } = GetDefinition<SpellDefinition>("SeeInvisibility");
        internal static SpellDefinition ShadowArmor { get; } = GetDefinition<SpellDefinition>("ShadowArmor");
        internal static SpellDefinition ShadowDagger { get; } = GetDefinition<SpellDefinition>("ShadowDagger");
        internal static SpellDefinition Shatter { get; } = GetDefinition<SpellDefinition>("Shatter");
        internal static SpellDefinition Shield { get; } = GetDefinition<SpellDefinition>("Shield");
        internal static SpellDefinition ShieldOfFaith { get; } = GetDefinition<SpellDefinition>("ShieldOfFaith");
        internal static SpellDefinition Shillelagh { get; } = GetDefinition<SpellDefinition>("Shillelagh");
        internal static SpellDefinition Shine { get; } = GetDefinition<SpellDefinition>("Shine");
        internal static SpellDefinition ShockingGrasp { get; } = GetDefinition<SpellDefinition>("ShockingGrasp");
        internal static SpellDefinition Silence { get; } = GetDefinition<SpellDefinition>("Silence");
        internal static SpellDefinition Sleep { get; } = GetDefinition<SpellDefinition>("Sleep");
        internal static SpellDefinition SleetStorm { get; } = GetDefinition<SpellDefinition>("SleetStorm");
        internal static SpellDefinition Slow { get; } = GetDefinition<SpellDefinition>("Slow");
        internal static SpellDefinition SpareTheDying { get; } = GetDefinition<SpellDefinition>("SpareTheDying");
        internal static SpellDefinition Sparkle { get; } = GetDefinition<SpellDefinition>("Sparkle");
        internal static SpellDefinition SpellWard { get; } = GetDefinition<SpellDefinition>("SpellWard");
        internal static SpellDefinition SpiderClimb { get; } = GetDefinition<SpellDefinition>("SpiderClimb");
        internal static SpellDefinition SpikeGrowth { get; } = GetDefinition<SpellDefinition>("SpikeGrowth");
        internal static SpellDefinition SpiritGuardians { get; } = GetDefinition<SpellDefinition>("SpiritGuardians");
        internal static SpellDefinition SpiritualWeapon { get; } = GetDefinition<SpellDefinition>("SpiritualWeapon");
        internal static SpellDefinition StinkingCloud { get; } = GetDefinition<SpellDefinition>("StinkingCloud");
        internal static SpellDefinition Stoneskin { get; } = GetDefinition<SpellDefinition>("Stoneskin");
        internal static SpellDefinition Sunbeam { get; } = GetDefinition<SpellDefinition>("Sunbeam");
        internal static SpellDefinition Sunburst { get; } = GetDefinition<SpellDefinition>("Sunburst");
        internal static SpellDefinition Symbol { get; } = GetDefinition<SpellDefinition>("Symbol");
        internal static SpellDefinition SymbolOfDeath { get; } = GetDefinition<SpellDefinition>("SymbolOfDeath");
        internal static SpellDefinition SymbolOfFear { get; } = GetDefinition<SpellDefinition>("SymbolOfFear");

        internal static SpellDefinition SymbolOfHopelessness { get; } =
            GetDefinition<SpellDefinition>("SymbolOfHopelessness");

        internal static SpellDefinition SymbolOfInsanity { get; } = GetDefinition<SpellDefinition>("SymbolOfInsanity");
        internal static SpellDefinition SymbolOfPain { get; } = GetDefinition<SpellDefinition>("SymbolOfPain");
        internal static SpellDefinition SymbolOfSleep { get; } = GetDefinition<SpellDefinition>("SymbolOfSleep");
        internal static SpellDefinition SymbolOfStun { get; } = GetDefinition<SpellDefinition>("SymbolOfStun");
        internal static SpellDefinition Thunderstorm { get; } = GetDefinition<SpellDefinition>("Thunderstorm");
        internal static SpellDefinition Thunderwave { get; } = GetDefinition<SpellDefinition>("Thunderwave");
        internal static SpellDefinition Tongues { get; } = GetDefinition<SpellDefinition>("Tongues");
        internal static SpellDefinition TrueSeeing { get; } = GetDefinition<SpellDefinition>("TrueSeeing");
        internal static SpellDefinition TrueStrike { get; } = GetDefinition<SpellDefinition>("TrueStrike");
        internal static SpellDefinition VampiricTouch { get; } = GetDefinition<SpellDefinition>("VampiricTouch");
        internal static SpellDefinition VenomousSpike { get; } = GetDefinition<SpellDefinition>("VenomousSpike");
        internal static SpellDefinition ViciousMockery { get; } = GetDefinition<SpellDefinition>("ViciousMockery");
        internal static SpellDefinition WallOfFire { get; } = GetDefinition<SpellDefinition>("WallOfFire");
        internal static SpellDefinition WallOfFireLine { get; } = GetDefinition<SpellDefinition>("WallOfFireLine");

        internal static SpellDefinition WallOfFireRing_Inner { get; } =
            GetDefinition<SpellDefinition>("WallOfFireRing_Inner");

        internal static SpellDefinition WallOfFireRing_Outer { get; } =
            GetDefinition<SpellDefinition>("WallOfFireRing_Outer");

        internal static SpellDefinition WallOfForce { get; } = GetDefinition<SpellDefinition>("WallOfForce");
        internal static SpellDefinition WallOfThorns { get; } = GetDefinition<SpellDefinition>("WallOfThorns");

        internal static SpellDefinition WallOfThornsWallLine { get; } =
            GetDefinition<SpellDefinition>("WallOfThornsWallLine");

        internal static SpellDefinition WallOfThornsWallRing { get; } =
            GetDefinition<SpellDefinition>("WallOfThornsWallRing");

        internal static SpellDefinition WardingBond { get; } = GetDefinition<SpellDefinition>("WardingBond");
        internal static SpellDefinition WaterBreathing { get; } = GetDefinition<SpellDefinition>("WaterBreathing");
        internal static SpellDefinition WaterWalk { get; } = GetDefinition<SpellDefinition>("WaterWalk");
        internal static SpellDefinition WindWall { get; } = GetDefinition<SpellDefinition>("WindWall");
    }

    internal static class SpellListDefinitions
    {
        internal static SpellListDefinition SpellList_DLC1_Cafrain { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC1_Cafrain");

        internal static SpellListDefinition SpellList_DLC1_Dominion_Arcanist { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC1_Dominion_Arcanist");

        internal static SpellListDefinition SpellList_DLC1_Forge_Druid { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC1_Forge_Druid");

        internal static SpellListDefinition SpellList_DLC1_ManaScientist { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC1_ManaScientist");

        internal static SpellListDefinition SpellList_DLC3_Gallivan_Druid { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC3_Gallivan_Druid");

        internal static SpellListDefinition SpellList_DLC3_Gallivan_RogueShadowCaster { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC3_Gallivan_RogueShadowCaster");

        internal static SpellListDefinition SpellList_DLC3_Kratshar { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC3_Kratshar");

        internal static SpellListDefinition SpellList_DLC3_Misouk { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC3_Misouk");

        internal static SpellListDefinition SpellList_DLC3_Vigdis { get; } =
            GetDefinition<SpellListDefinition>("SpellList_DLC3_Vigdis");

        internal static SpellListDefinition SpellListAcolyte { get; } =
            GetDefinition<SpellListDefinition>("SpellListAcolyte");

        internal static SpellListDefinition SpellListAdria { get; } =
            GetDefinition<SpellListDefinition>("SpellListAdria");

        internal static SpellListDefinition SpellListAllCantrips { get; } =
            GetDefinition<SpellListDefinition>("SpellListAllCantrips");

        internal static SpellListDefinition SpellListAllSpells { get; } =
            GetDefinition<SpellListDefinition>("SpellListAllSpells");

        internal static SpellListDefinition SpellListArrok { get; } =
            GetDefinition<SpellListDefinition>("SpellListArrok");

        internal static SpellListDefinition SpellListBard { get; } =
            GetDefinition<SpellListDefinition>("SpellListBard");

        internal static SpellListDefinition SpellListBerylStonebeard { get; } =
            GetDefinition<SpellListDefinition>("SpellListBerylStonebeard");

        internal static SpellListDefinition SpellListBerylStonebeard_DLC3 { get; } =
            GetDefinition<SpellListDefinition>("SpellListBerylStonebeard_DLC3");

        internal static SpellListDefinition SpellListCleric { get; } =
            GetDefinition<SpellListDefinition>("SpellListCleric");

        internal static SpellListDefinition SpellListCubeOfLight { get; } =
            GetDefinition<SpellListDefinition>("SpellListCubeOfLight");

        internal static SpellListDefinition SpellListCultFanatic { get; } =
            GetDefinition<SpellListDefinition>("SpellListCultFanatic");

        internal static SpellListDefinition SpellListDivine_Avatar_Cleric { get; } =
            GetDefinition<SpellListDefinition>("SpellListDivine_Avatar_Cleric");

        internal static SpellListDefinition SpellListDivine_Avatar_Paladin { get; } =
            GetDefinition<SpellListDefinition>("SpellListDivine_Avatar_Paladin");

        internal static SpellListDefinition SpellListDivine_Avatar_Wizard { get; } =
            GetDefinition<SpellListDefinition>("SpellListDivine_Avatar_Wizard");

        internal static SpellListDefinition SpellListDLC1_Rose { get; } =
            GetDefinition<SpellListDefinition>("SpellListDLC1_Rose");

        internal static SpellListDefinition SpellListDLC1_Vando { get; } =
            GetDefinition<SpellListDefinition>("SpellListDLC1_Vando");

        internal static SpellListDefinition SpellListDruid { get; } =
            GetDefinition<SpellListDefinition>("SpellListDruid");

        internal static SpellListDefinition SpellListDryad { get; } =
            GetDefinition<SpellListDefinition>("SpellListDryad");

        internal static SpellListDefinition SpellListDryad_Queen { get; } =
            GetDefinition<SpellListDefinition>("SpellListDryad_Queen");

        internal static SpellListDefinition SpellListDryad_Water { get; } =
            GetDefinition<SpellListDefinition>("SpellListDryad_Water");

        internal static SpellListDefinition SpellListGalar { get; } =
            GetDefinition<SpellListDefinition>("SpellListGalar");

        internal static SpellListDefinition SpellListGenericSorcerer { get; } =
            GetDefinition<SpellListDefinition>("SpellListGenericSorcerer");

        internal static SpellListDefinition SpellListGlabrezu { get; } =
            GetDefinition<SpellListDefinition>("SpellListGlabrezu");

        internal static SpellListDefinition SpellListGnomeShadow { get; } =
            GetDefinition<SpellListDefinition>("SpellListGnomeShadow");

        internal static SpellListDefinition SpellListGoblinShaman { get; } =
            GetDefinition<SpellListDefinition>("SpellListGoblinShaman");

        internal static SpellListDefinition SpellListHeliaFairblade { get; } =
            GetDefinition<SpellListDefinition>("SpellListHeliaFairblade");

        internal static SpellListDefinition SpellListHighPriest { get; } =
            GetDefinition<SpellListDefinition>("SpellListHighPriest");

        internal static SpellListDefinition SpellListHyeronimus { get; } =
            GetDefinition<SpellListDefinition>("SpellListHyeronimus");

        internal static SpellListDefinition SpellListKebra { get; } =
            GetDefinition<SpellListDefinition>("SpellListKebra");

        internal static SpellListDefinition SpellListKythaela_Cantrips { get; } =
            GetDefinition<SpellListDefinition>("SpellListKythaela_Cantrips");

        internal static SpellListDefinition SpellListKythaela_Full { get; } =
            GetDefinition<SpellListDefinition>("SpellListKythaela_Full");

        internal static SpellListDefinition SpellListMage { get; } =
            GetDefinition<SpellListDefinition>("SpellListMage");

        internal static SpellListDefinition SpellListMardracht { get; } =
            GetDefinition<SpellListDefinition>("SpellListMardracht");

        internal static SpellListDefinition SpellListMonkTraditionLight { get; } =
            GetDefinition<SpellListDefinition>("SpellListMonkTraditionLight");

        internal static SpellListDefinition SpellListMummyLord { get; } =
            GetDefinition<SpellListDefinition>("SpellListMummyLord");

        internal static SpellListDefinition SpellListNecromancer { get; } =
            GetDefinition<SpellListDefinition>("SpellListNecromancer");

        internal static SpellListDefinition SpellListNecromancer_BoneKeep { get; } =
            GetDefinition<SpellListDefinition>("SpellListNecromancer_BoneKeep");

        internal static SpellListDefinition SpellListOrcShaman { get; } =
            GetDefinition<SpellListDefinition>("SpellListOrcShaman");

        internal static SpellListDefinition SpellListPaladin { get; } =
            GetDefinition<SpellListDefinition>("SpellListPaladin");

        internal static SpellListDefinition SpellListPatronFiend { get; } =
            GetDefinition<SpellListDefinition>("SpellListPatronFiend");

        internal static SpellListDefinition SpellListPatronHive { get; } =
            GetDefinition<SpellListDefinition>("SpellListPatronHive");

        internal static SpellListDefinition SpellListPatronTimekeeper { get; } =
            GetDefinition<SpellListDefinition>("SpellListPatronTimekeeper");

        internal static SpellListDefinition SpellListPatronTree { get; } =
            GetDefinition<SpellListDefinition>("SpellListPatronTree");

        internal static SpellListDefinition SpellListPriest { get; } =
            GetDefinition<SpellListDefinition>("SpellListPriest");

        internal static SpellListDefinition SpellListRanger { get; } =
            GetDefinition<SpellListDefinition>("SpellListRanger");

        internal static SpellListDefinition SpellListReya { get; } =
            GetDefinition<SpellListDefinition>("SpellListReya");

        internal static SpellListDefinition SpellListShockArcanist { get; } =
            GetDefinition<SpellListDefinition>("SpellListShockArcanist");

        internal static SpellListDefinition SpellListShockOrenetis { get; } =
            GetDefinition<SpellListDefinition>("SpellListShockOrenetis");

        internal static SpellListDefinition SpellListSkeletonKnight { get; } =
            GetDefinition<SpellListDefinition>("SpellListSkeletonKnight");

        internal static SpellListDefinition SpellListSkeletonSorcerer { get; } =
            GetDefinition<SpellListDefinition>("SpellListSkeletonSorcerer");

        internal static SpellListDefinition SpellListSorcerer { get; } =
            GetDefinition<SpellListDefinition>("SpellListSorcerer");

        internal static SpellListDefinition SpellListSorr_Akkath_Acolyte_of_Sorr_Tarr { get; } =
            GetDefinition<SpellListDefinition>("SpellListSorr-Akkath_Acolyte_of_Sorr-Tarr");

        internal static SpellListDefinition SpellListSorr_Akkath_Archpriest_of_Sorr_Tarr { get; } =
            GetDefinition<SpellListDefinition>("SpellListSorr-Akkath_Archpriest_of_Sorr-Tarr");

        internal static SpellListDefinition SpellListSorr_Akkath_Priest_of_Sorr_Tarr { get; } =
            GetDefinition<SpellListDefinition>("SpellListSorr-Akkath_Priest_of_Sorr-Tarr");

        internal static SpellListDefinition SpellListSwamp_Hag { get; } =
            GetDefinition<SpellListDefinition>("SpellListSwamp_Hag");

        internal static SpellListDefinition SpellListTiefling { get; } =
            GetDefinition<SpellListDefinition>("SpellListTiefling");

        internal static SpellListDefinition SpellListWarlock { get; } =
            GetDefinition<SpellListDefinition>("SpellListWarlock");

        internal static SpellListDefinition SpellListWizard { get; } =
            GetDefinition<SpellListDefinition>("SpellListWizard");

        internal static SpellListDefinition SpellListWizardGreenmage { get; } =
            GetDefinition<SpellListDefinition>("SpellListWizardGreenmage");
    }

    internal static class SubtitleTableDefinitions
    {
    }

    internal static class ToolTypeDefinitions
    {
        internal static ToolTypeDefinition ArtisanToolSmithToolsType { get; } =
            GetDefinition<ToolTypeDefinition>("ArtisanToolSmithToolsType");

        internal static ToolTypeDefinition DisguiseKitType { get; } =
            GetDefinition<ToolTypeDefinition>("DisguiseKitType");

        internal static ToolTypeDefinition EnchantingToolType { get; } =
            GetDefinition<ToolTypeDefinition>("EnchantingToolType");

        internal static ToolTypeDefinition GamingSetDiceType { get; } =
            GetDefinition<ToolTypeDefinition>("GamingSetDiceType");

        internal static ToolTypeDefinition HerbalismKitType { get; } =
            GetDefinition<ToolTypeDefinition>("HerbalismKitType");

        internal static ToolTypeDefinition MusicalInstrumentLyreType { get; } =
            GetDefinition<ToolTypeDefinition>("MusicalInstrumentLyreType");

        internal static ToolTypeDefinition PoisonersKitType { get; } =
            GetDefinition<ToolTypeDefinition>("PoisonersKitType");

        internal static ToolTypeDefinition ScrollKitType { get; } = GetDefinition<ToolTypeDefinition>("ScrollKitType");

        internal static ToolTypeDefinition ThievesToolsType { get; } =
            GetDefinition<ToolTypeDefinition>("ThievesToolsType");
    }

    internal static class TravelJournalDefinitions
    {
    }

    internal static class TutorialTableDefinitions
    {
    }

    internal static class TutorialTocDefinitions
    {
    }

    internal static class WeaponCategoryDefinitions
    {
        internal static WeaponCategoryDefinition MartialWeaponCategory { get; } =
            GetDefinition<WeaponCategoryDefinition>("MartialWeaponCategory");

        internal static WeaponCategoryDefinition SimpleWeaponCategory { get; } =
            GetDefinition<WeaponCategoryDefinition>("SimpleWeaponCategory");
    }

    internal static class WeaponTypeDefinitions
    {
        internal static WeaponTypeDefinition BattleaxeType { get; } =
            GetDefinition<WeaponTypeDefinition>("BattleaxeType");

        internal static WeaponTypeDefinition ClubType { get; } = GetDefinition<WeaponTypeDefinition>("ClubType");
        internal static WeaponTypeDefinition DaggerType { get; } = GetDefinition<WeaponTypeDefinition>("DaggerType");
        internal static WeaponTypeDefinition DartType { get; } = GetDefinition<WeaponTypeDefinition>("DartType");

        internal static WeaponTypeDefinition GreataxeType { get; } =
            GetDefinition<WeaponTypeDefinition>("GreataxeType");

        internal static WeaponTypeDefinition GreatswordType { get; } =
            GetDefinition<WeaponTypeDefinition>("GreatswordType");

        internal static WeaponTypeDefinition HandaxeType { get; } = GetDefinition<WeaponTypeDefinition>("HandaxeType");

        internal static WeaponTypeDefinition HeavyCrossbowType { get; } =
            GetDefinition<WeaponTypeDefinition>("HeavyCrossbowType");

        internal static WeaponTypeDefinition JavelinType { get; } = GetDefinition<WeaponTypeDefinition>("JavelinType");

        internal static WeaponTypeDefinition LightCrossbowType { get; } =
            GetDefinition<WeaponTypeDefinition>("LightCrossbowType");

        internal static WeaponTypeDefinition LongbowType { get; } = GetDefinition<WeaponTypeDefinition>("LongbowType");

        internal static WeaponTypeDefinition LongswordType { get; } =
            GetDefinition<WeaponTypeDefinition>("LongswordType");

        internal static WeaponTypeDefinition MaceType { get; } = GetDefinition<WeaponTypeDefinition>("MaceType");
        internal static WeaponTypeDefinition MaulType { get; } = GetDefinition<WeaponTypeDefinition>("MaulType");

        internal static WeaponTypeDefinition MorningstarType { get; } =
            GetDefinition<WeaponTypeDefinition>("MorningstarType");

        internal static WeaponTypeDefinition QuarterstaffType { get; } =
            GetDefinition<WeaponTypeDefinition>("QuarterstaffType");

        internal static WeaponTypeDefinition RapierType { get; } = GetDefinition<WeaponTypeDefinition>("RapierType");

        internal static WeaponTypeDefinition ScimitarType { get; } =
            GetDefinition<WeaponTypeDefinition>("ScimitarType");

        internal static WeaponTypeDefinition ShortbowType { get; } =
            GetDefinition<WeaponTypeDefinition>("ShortbowType");

        internal static WeaponTypeDefinition ShortswordType { get; } =
            GetDefinition<WeaponTypeDefinition>("ShortswordType");

        internal static WeaponTypeDefinition SpearType { get; } = GetDefinition<WeaponTypeDefinition>("SpearType");

        internal static WeaponTypeDefinition UnarmedStrikeType { get; } =
            GetDefinition<WeaponTypeDefinition>("UnarmedStrikeType");

        internal static WeaponTypeDefinition WarhammerType { get; } =
            GetDefinition<WeaponTypeDefinition>("WarhammerType");
    }
}
