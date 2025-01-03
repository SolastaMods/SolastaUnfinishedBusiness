using TA.AI;

// manually generated on 06/05/2023
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace SolastaUnfinishedBusiness.Api;

internal static partial class DatabaseHelper
{
    internal static class FeatureDefinitionMoveThroughEnemyModifiers
    {
        internal static FeatureDefinitionMoveThroughEnemyModifier MoveThroughEnemyModifierHalflingNimbleness { get; } =
            GetDefinition<FeatureDefinitionMoveThroughEnemyModifier>("MoveThroughEnemyModifierHalflingNimbleness");
    }

    internal static class FeatureDefinitionPerceptionAffinitys
    {
        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionBlinded { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionBlinded");

        internal static FeatureDefinitionPerceptionAffinity PerceptionAffinityConditionInvisible { get; } =
            GetDefinition<FeatureDefinitionPerceptionAffinity>("PerceptionAffinityConditionInvisible");
    }

    internal static class ActionDefinitions
    {
        internal static ActionDefinition ActionSurge { get; } = GetDefinition<ActionDefinition>("ActionSurge");
        internal static ActionDefinition AttackFree { get; } = GetDefinition<ActionDefinition>("AttackFree");
        internal static ActionDefinition CastMain { get; } = GetDefinition<ActionDefinition>("CastMain");
        internal static ActionDefinition CastBonus { get; } = GetDefinition<ActionDefinition>("CastBonus");
        internal static ActionDefinition CastInvocation { get; } = GetDefinition<ActionDefinition>("CastInvocation");
        internal static ActionDefinition CastNoCost { get; } = GetDefinition<ActionDefinition>("CastNoCost");
        internal static ActionDefinition DashBonus { get; } = GetDefinition<ActionDefinition>("DashBonus");
        internal static ActionDefinition DropProne { get; } = GetDefinition<ActionDefinition>("DropProne");
        internal static ActionDefinition ReapplyEffect { get; } = GetDefinition<ActionDefinition>("ReapplyEffect");
        internal static ActionDefinition SpiritRage { get; } = GetDefinition<ActionDefinition>("SpiritRage");

        internal static ActionDefinition GrantBardicInspiration { get; } =
            GetDefinition<ActionDefinition>("GrantBardicInspiration");

        internal static ActionDefinition MetamagicToggle { get; } = GetDefinition<ActionDefinition>("MetamagicToggle");

        internal static ActionDefinition ProxyFlamingSphere { get; } =
            GetDefinition<ActionDefinition>("ProxyFlamingSphere");

        internal static ActionDefinition ProxySpiritualWeapon { get; } =
            GetDefinition<ActionDefinition>("ProxySpiritualWeapon");

        internal static ActionDefinition ProxySpiritualWeaponFree { get; } =
            GetDefinition<ActionDefinition>("ProxySpiritualWeaponFree");

        internal static ActionDefinition Pushed { get; } = GetDefinition<ActionDefinition>("Pushed");
        internal static ActionDefinition RageStart { get; } = GetDefinition<ActionDefinition>("RageStart");
        internal static ActionDefinition RecklessAttack { get; } = GetDefinition<ActionDefinition>("RecklessAttack");

        internal static ActionDefinition StunningStrikeToggle { get; } =
            GetDefinition<ActionDefinition>("StunningStrikeToggle");

        internal static ActionDefinition UseBardicInspiration { get; } =
            GetDefinition<ActionDefinition>("UseBardicInspiration");

        internal static ActionDefinition UseIndomitableResistance { get; } =
            GetDefinition<ActionDefinition>("UseIndomitableResistance");

        internal static ActionDefinition Volley { get; } = GetDefinition<ActionDefinition>("Volley");

        internal static ActionDefinition WhirlwindAttack { get; } = GetDefinition<ActionDefinition>("WhirlwindAttack");
        internal static ActionDefinition WildShape { get; } = GetDefinition<ActionDefinition>("WildShape");
    }

    internal static class CharacterBackgroundDefinitions
    {
        internal static CharacterBackgroundDefinition Academic { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Academic");

        internal static CharacterBackgroundDefinition Acolyte { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Acolyte");

        internal static CharacterBackgroundDefinition Aescetic_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Aescetic_Background");

        internal static CharacterBackgroundDefinition Aristocrat { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Aristocrat");

        internal static CharacterBackgroundDefinition Artist_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Artist_Background");

        internal static CharacterBackgroundDefinition Lawkeeper { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Lawkeeper");

        internal static CharacterBackgroundDefinition Lowlife { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Lowlife");

        internal static CharacterBackgroundDefinition Occultist_Background { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Occultist_Background");

        internal static CharacterBackgroundDefinition Philosopher { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Philosopher");

        internal static CharacterBackgroundDefinition SellSword { get; } =
            GetDefinition<CharacterBackgroundDefinition>("SellSword");

        internal static CharacterBackgroundDefinition Spy { get; } =
            GetDefinition<CharacterBackgroundDefinition>("Spy");

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
        internal static CharacterFamilyDefinition Construct { get; } =
            GetDefinition<CharacterFamilyDefinition>("Construct");

        internal static CharacterFamilyDefinition Dragon { get; } =
            GetDefinition<CharacterFamilyDefinition>("Dragon");

        internal static CharacterFamilyDefinition Elemental { get; } =
            GetDefinition<CharacterFamilyDefinition>("Elemental");

        internal static CharacterFamilyDefinition Fey { get; } =
            GetDefinition<CharacterFamilyDefinition>("Fey");

        internal static CharacterFamilyDefinition Fiend { get; } =
            GetDefinition<CharacterFamilyDefinition>("Fiend");

        internal static CharacterFamilyDefinition Giant { get; } =
            GetDefinition<CharacterFamilyDefinition>("Giant");

        internal static CharacterFamilyDefinition Humanoid { get; } =
            GetDefinition<CharacterFamilyDefinition>("Humanoid");

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

        internal static CharacterRaceDefinition HalfOrc { get; } = GetDefinition<CharacterRaceDefinition>("HalfOrc");
        internal static CharacterRaceDefinition HalfElf { get; } = GetDefinition<CharacterRaceDefinition>("HalfElf");
        internal static CharacterRaceDefinition Halfling { get; } = GetDefinition<CharacterRaceDefinition>("Halfling");
        internal static CharacterRaceDefinition Human { get; } = GetDefinition<CharacterRaceDefinition>("Human");
        internal static CharacterRaceDefinition Gnome { get; } = GetDefinition<CharacterRaceDefinition>("Gnome");
        internal static CharacterRaceDefinition Tiefling { get; } = GetDefinition<CharacterRaceDefinition>("Tiefling");
    }

    internal static class CharacterSizeDefinitions
    {
        internal static CharacterSizeDefinition DragonSize { get; } =
            GetDefinition<CharacterSizeDefinition>("DragonSize");

        internal static CharacterSizeDefinition Gargantuan { get; } =
            GetDefinition<CharacterSizeDefinition>("Gargantuan");

        internal static CharacterSizeDefinition Huge { get; } = GetDefinition<CharacterSizeDefinition>("Huge");
        internal static CharacterSizeDefinition Medium { get; } = GetDefinition<CharacterSizeDefinition>("Medium");

        internal static CharacterSizeDefinition SpiderQueenSize { get; } =
            GetDefinition<CharacterSizeDefinition>("SpiderQueenSize");

        internal static CharacterSizeDefinition Small { get; } = GetDefinition<CharacterSizeDefinition>("Small");
        internal static CharacterSizeDefinition Tiny { get; } = GetDefinition<CharacterSizeDefinition>("Tiny");
    }

    internal static class CharacterSubclassDefinitions
    {
        internal static CharacterSubclassDefinition CircleKindred { get; } =
            GetDefinition<CharacterSubclassDefinition>("CircleKindred");

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

        internal static CharacterSubclassDefinition PathClaw { get; } =
            GetDefinition<CharacterSubclassDefinition>("PathClaw");

        internal static CharacterSubclassDefinition OathOfDevotion { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfDevotion");

        internal static CharacterSubclassDefinition OathOfJugement { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfJugement");

        internal static CharacterSubclassDefinition OathOfTheMotherland { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfTheMotherland");

        internal static CharacterSubclassDefinition OathOfTirmar { get; } =
            GetDefinition<CharacterSubclassDefinition>("OathOfTirmar");

        internal static CharacterSubclassDefinition RangerMarksman { get; } =
            GetDefinition<CharacterSubclassDefinition>("RangerMarksman");

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

        internal static CharacterSubclassDefinition TraditionFreedom { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionFreedom");

        internal static CharacterSubclassDefinition TraditionLight { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionLight");

        internal static CharacterSubclassDefinition TraditionOpenHand { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionOpenHand");

        internal static CharacterSubclassDefinition TraditionSurvival { get; } =
            GetDefinition<CharacterSubclassDefinition>("TraditionSurvival");
    }

    internal static class ConditionDefinitions
    {
        internal static ConditionDefinition ConditionTraditionSurvivalDefensiveStance { get; } =
            GetDefinition<ConditionDefinition>("ConditionTraditionSurvivalDefensiveStance");

        internal static ConditionDefinition ConditionTraditionSurvivalUnbreakableBodyPatientDefenseImproved { get; } =
            GetDefinition<ConditionDefinition>("ConditionTraditionSurvivalUnbreakableBodyPatientDefenseImproved");

        internal static ConditionDefinition ConditionDodgingPatientDefense { get; } =
            GetDefinition<ConditionDefinition>("ConditionDodgingPatientDefense");

        internal static ConditionDefinition ConditionBerserkerFrenzy { get; } =
            GetDefinition<ConditionDefinition>("ConditionBerserkerFrenzy");

        internal static ConditionDefinition ConditionDisengagingStepOfTheWind { get; } =
            GetDefinition<ConditionDefinition>("ConditionDisengagingStepOfTheWind");

        internal static ConditionDefinition ConditionBerserkerMindlessRage { get; } =
            GetDefinition<ConditionDefinition>("ConditionBerserkerMindlessRage");

        internal static ConditionDefinition ConditionRagingNormal { get; } =
            GetDefinition<ConditionDefinition>("ConditionRagingNormal");

        internal static ConditionDefinition ConditionRagingPersistent { get; } =
            GetDefinition<ConditionDefinition>("ConditionRagingPersistent");

        internal static ConditionDefinition ConditionStoneskin { get; } =
            GetDefinition<ConditionDefinition>("ConditionStoneskin");

        internal static ConditionDefinition ConditionBardicInspiration { get; } =
            GetDefinition<ConditionDefinition>("ConditionBardicInspiration");

        internal static ConditionDefinition ConditionCharmedByHypnoticPattern { get; } =
            GetDefinition<ConditionDefinition>("ConditionCharmedByHypnoticPattern");

        internal static ConditionDefinition ConditionHeavilyEncumbered { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeavilyEncumbered");

        internal static ConditionDefinition ConditionDashing { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashing");

        internal static ConditionDefinition ConditionDashingAdditional { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingAdditional");

        internal static ConditionDefinition ConditionDashingAdditionalSwiftBlade { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingAdditionalSwiftBlade");

        internal static ConditionDefinition ConditionDashingBonus { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingBonus");

        internal static ConditionDefinition ConditionDashingBonusAdditional { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingBonusAdditional");

        internal static ConditionDefinition ConditionDashingBonusStepOfTheWind { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingBonusStepOfTheWind");

        internal static ConditionDefinition ConditionDashingBonusSwiftBlade { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingBonusSwiftBlade");

        internal static ConditionDefinition ConditionDashingBonusSwiftSteps { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingBonusSwiftSteps");

        internal static ConditionDefinition ConditionDashingExpeditiousRetreat { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingExpeditiousRetreat");

        internal static ConditionDefinition ConditionDashingExpeditiousRetreatSwiftBlade { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingExpeditiousRetreatSwiftBlade");

        internal static ConditionDefinition ConditionDashingSwiftBlade { get; } =
            GetDefinition<ConditionDefinition>("ConditionDashingSwiftBlade");

        internal static ConditionDefinition ConditionMonkFlurryOfBlowsUnarmedStrikeBonus { get; } =
            GetDefinition<ConditionDefinition>("ConditionMonkFlurryOfBlowsUnarmedStrikeBonus");

        internal static ConditionDefinition ConditionDomainMischiefBorrowedLuck { get; } =
            GetDefinition<ConditionDefinition>("ConditionDomainMischiefBorrowedLuck");

        internal static ConditionDefinition ConditionSurged { get; } =
            GetDefinition<ConditionDefinition>("ConditionSurged");

        internal static ConditionDefinition ConditionFeebleMinded { get; } =
            GetDefinition<ConditionDefinition>("ConditionFeebleMinded");

        internal static ConditionDefinition ConditionMonkSlowFall { get; } =
            GetDefinition<ConditionDefinition>("ConditionMonkSlowFall");

        internal static ConditionDefinition ConditionHolyAura { get; } =
            GetDefinition<ConditionDefinition>("ConditionHolyAura");

        internal static ConditionDefinition ConditionAcidArrowed { get; } =
            GetDefinition<ConditionDefinition>("ConditionAcidArrowed");

        internal static ConditionDefinition ConditionAcidSpit { get; } =
            GetDefinition<ConditionDefinition>("ConditionAcidSpit");

        internal static ConditionDefinition ConditionAided { get; } =
            GetDefinition<ConditionDefinition>("ConditionAided");

        internal static ConditionDefinition ConditionAsleep { get; } =
            GetDefinition<ConditionDefinition>("ConditionAsleep");

        internal static ConditionDefinition ConditionAuraOfProtection { get; } =
            GetDefinition<ConditionDefinition>("ConditionAuraOfProtection");

        internal static ConditionDefinition ConditionBaned { get; } =
            GetDefinition<ConditionDefinition>("ConditionBaned");

        internal static ConditionDefinition ConditionBanished { get; } =
            GetDefinition<ConditionDefinition>("ConditionBanished");

        internal static ConditionDefinition ConditionBarkskin { get; } =
            GetDefinition<ConditionDefinition>("ConditionBarkskin");

        internal static ConditionDefinition ConditionBearsEndurance { get; } =
            GetDefinition<ConditionDefinition>("ConditionBearsEndurance");

        internal static ConditionDefinition ConditionBleeding { get; } =
            GetDefinition<ConditionDefinition>("ConditionBleeding");

        internal static ConditionDefinition ConditionBlessed { get; } =
            GetDefinition<ConditionDefinition>("ConditionBlessed");

        internal static ConditionDefinition ConditionBlinded { get; } =
            GetDefinition<ConditionDefinition>("ConditionBlinded");

        internal static ConditionDefinition ConditionBlindedEndOfNextTurn { get; } =
            GetDefinition<ConditionDefinition>("ConditionBlindedEndOfNextTurn");

        internal static ConditionDefinition ConditionBlurred { get; } =
            GetDefinition<ConditionDefinition>("ConditionBlurred");

        internal static ConditionDefinition ConditionBranded { get; } =
            GetDefinition<ConditionDefinition>("ConditionBranded");

        internal static ConditionDefinition ConditionBrandingSmite { get; } =
            GetDefinition<ConditionDefinition>("ConditionBrandingSmite");

        internal static ConditionDefinition ConditionBullsStrength { get; } =
            GetDefinition<ConditionDefinition>("ConditionBullsStrength");

        internal static ConditionDefinition ConditionCalmedByCalmEmotionsAlly { get; } =
            GetDefinition<ConditionDefinition>("ConditionCalmedByCalmEmotionsAlly");

        internal static ConditionDefinition ConditionCarriedByWind { get; } =
            GetDefinition<ConditionDefinition>("ConditionCarriedByWind");

        internal static ConditionDefinition ConditionCharmed { get; } =
            GetDefinition<ConditionDefinition>("ConditionCharmed");

        internal static ConditionDefinition ConditionChildOfDarkness_DimLight { get; } =
            GetDefinition<ConditionDefinition>("ConditionChildOfDarkness_DimLight");

        internal static ConditionDefinition ConditionChilled { get; } =
            GetDefinition<ConditionDefinition>("ConditionChilled");

        internal static ConditionDefinition ConditionChilledByTouch { get; } =
            GetDefinition<ConditionDefinition>("ConditionChilledByTouch");

        internal static ConditionDefinition ConditionConfused { get; } =
            GetDefinition<ConditionDefinition>("ConditionConfused");

        internal static ConditionDefinition ConditionConjuredCreature { get; } =
            GetDefinition<ConditionDefinition>("ConditionConjuredCreature");

        internal static ConditionDefinition ConditionCursed { get; } =
            GetDefinition<ConditionDefinition>("ConditionCursed");

        internal static ConditionDefinition ConditionCursedByBestowCurseAttackRoll { get; } =
            GetDefinition<ConditionDefinition>("ConditionCursedByBestowCurseAttackRoll");

        internal static ConditionDefinition ConditionDarkness { get; } =
            GetDefinition<ConditionDefinition>("ConditionDarkness");

        internal static ConditionDefinition ConditionDarkvision { get; } =
            GetDefinition<ConditionDefinition>("ConditionDarkvision");

        internal static ConditionDefinition ConditionDazzled { get; } =
            GetDefinition<ConditionDefinition>("ConditionDazzled");

        internal static ConditionDefinition ConditionDead { get; } =
            GetDefinition<ConditionDefinition>("ConditionDead");

        internal static ConditionDefinition ConditionDeafened { get; } =
            GetDefinition<ConditionDefinition>("ConditionDeafened");

        internal static ConditionDefinition ConditionDisengaging { get; } =
            GetDefinition<ConditionDefinition>("ConditionDisengaging");

        internal static ConditionDefinition ConditionDispellingEvilAndGood { get; } =
            GetDefinition<ConditionDefinition>("ConditionDispellingEvilAndGood");

        internal static ConditionDefinition ConditionDiseased { get; } =
            GetDefinition<ConditionDefinition>("ConditionDiseased");

        internal static ConditionDefinition ConditionDistracted { get; } =
            GetDefinition<ConditionDefinition>("ConditionDistracted");

        internal static ConditionDefinition ConditionDivineFavor { get; } =
            GetDefinition<ConditionDefinition>("ConditionDivineFavor");

        internal static ConditionDefinition ConditionDodging { get; } =
            GetDefinition<ConditionDefinition>("ConditionDodging");

        internal static ConditionDefinition ConditionDoomLaughter { get; } =
            GetDefinition<ConditionDefinition>("ConditionDoomLaughter");

        internal static ConditionDefinition ConditionEncumbered { get; } =
            GetDefinition<ConditionDefinition>("ConditionEncumbered");

        internal static ConditionDefinition ConditionEnfeebled { get; } =
            GetDefinition<ConditionDefinition>("ConditionEnfeebled");

        internal static ConditionDefinition ConditionExhausted { get; } =
            GetDefinition<ConditionDefinition>("ConditionExhausted");

        internal static ConditionDefinition ConditionEyebitePanicked { get; } =
            GetDefinition<ConditionDefinition>("ConditionEyebitePanicked");

        internal static ConditionDefinition ConditionEyebiteSickened { get; } =
            GetDefinition<ConditionDefinition>("ConditionEyebiteSickened");

        internal static ConditionDefinition ConditionFeatTakeAim { get; } =
            GetDefinition<ConditionDefinition>("ConditionFeatTakeAim");

        internal static ConditionDefinition ConditionFlying { get; } =
            GetDefinition<ConditionDefinition>("ConditionFlying");

        internal static ConditionDefinition ConditionFlyingAdaptive { get; } =
            GetDefinition<ConditionDefinition>("ConditionFlyingAdaptive");

        internal static ConditionDefinition ConditionFreedomOfMovement { get; } =
            GetDefinition<ConditionDefinition>("ConditionFreedomOfMovement");

        internal static ConditionDefinition ConditionFrightened { get; } =
            GetDefinition<ConditionDefinition>("ConditionFrightened");

        internal static ConditionDefinition ConditionFrightenedFear { get; } =
            GetDefinition<ConditionDefinition>("ConditionFrightenedFear");

        internal static ConditionDefinition ConditionFrightenedPhantasmalKiller { get; } =
            GetDefinition<ConditionDefinition>("ConditionFrightenedPhantasmalKiller");

        internal static ConditionDefinition ConditionGuided { get; } =
            GetDefinition<ConditionDefinition>("ConditionGuided");

        internal static ConditionDefinition ConditionAuraOfCourage { get; } =
            GetDefinition<ConditionDefinition>("ConditionAuraOfCourage");

        internal static ConditionDefinition ConditionHasted { get; } =
            GetDefinition<ConditionDefinition>("ConditionHasted");

        internal static ConditionDefinition ConditionHeatMetal { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeatMetal");

        internal static ConditionDefinition ConditionHeavilyObscured { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeavilyObscured");

        internal static ConditionDefinition ConditionHeraldOfBattle { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeraldOfBattle");

        internal static ConditionDefinition ConditionHeroism { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeroism");

        internal static ConditionDefinition ConditionHighlighted { get; } =
            GetDefinition<ConditionDefinition>("ConditionHighlighted");

        internal static ConditionDefinition ConditionHindered { get; } =
            GetDefinition<ConditionDefinition>("ConditionHindered");

        internal static ConditionDefinition ConditionHindered_By_Frost { get; } =
            GetDefinition<ConditionDefinition>("ConditionHindered_By_Frost");

        internal static ConditionDefinition ConditionHopeless { get; } =
            GetDefinition<ConditionDefinition>("ConditionHopeless");

        internal static ConditionDefinition ConditionIncapacitated { get; } =
            GetDefinition<ConditionDefinition>("ConditionIncapacitated");

        internal static ConditionDefinition ConditionInsane { get; } =
            GetDefinition<ConditionDefinition>("ConditionInsane");

        internal static ConditionDefinition ConditionInStinkingCloud { get; } =
            GetDefinition<ConditionDefinition>("ConditionInStinkingCloud");

        internal static ConditionDefinition ConditionInvisible { get; } =
            GetDefinition<ConditionDefinition>("ConditionInvisible");

        internal static ConditionDefinition ConditionInvisibleBase { get; } =
            GetDefinition<ConditionDefinition>("ConditionInvisibleBase");

        internal static ConditionDefinition ConditionInvisibleGreater { get; } =
            GetDefinition<ConditionDefinition>("ConditionInvisibleGreater");

        internal static ConditionDefinition ConditionJump { get; } =
            GetDefinition<ConditionDefinition>("ConditionJump");

        internal static ConditionDefinition ConditionLeadByExampleMarked { get; } =
            GetDefinition<ConditionDefinition>("ConditionLeadByExampleMarked");

        internal static ConditionDefinition ConditionLethargic { get; } =
            GetDefinition<ConditionDefinition>("ConditionLethargic");

        internal static ConditionDefinition ConditionLevitate { get; } =
            GetDefinition<ConditionDefinition>("ConditionLevitate");

        internal static ConditionDefinition ConditionLifeDrained { get; } =
            GetDefinition<ConditionDefinition>("ConditionLifeDrained");

        internal static ConditionDefinition ConditionLightSensitive { get; } =
            GetDefinition<ConditionDefinition>("ConditionLightSensitive");

        internal static ConditionDefinition ConditionLightSensitiveSorakSaboteur { get; } =
            GetDefinition<ConditionDefinition>("ConditionLightSensitiveSorakSaboteur");

        internal static ConditionDefinition ConditionLongstrider { get; } =
            GetDefinition<ConditionDefinition>("ConditionLongstrider");

        internal static ConditionDefinition ConditionLuminousKi { get; } =
            GetDefinition<ConditionDefinition>("ConditionLuminousKi");

        internal static ConditionDefinition ConditionMagicallyArmored { get; } =
            GetDefinition<ConditionDefinition>("ConditionMagicallyArmored");

        internal static ConditionDefinition ConditionMalediction { get; } =
            GetDefinition<ConditionDefinition>("ConditionMalediction");

        internal static ConditionDefinition ConditionMarkedByBrandingSmite { get; } =
            GetDefinition<ConditionDefinition>("ConditionMarkedByBrandingSmite");

        internal static ConditionDefinition ConditionMarkedByHunter { get; } =
            GetDefinition<ConditionDefinition>("ConditionMarkedByHunter");

        internal static ConditionDefinition ConditionMindControlledByCaster { get; } =
            GetDefinition<ConditionDefinition>("ConditionMindControlledByCaster");

        internal static ConditionDefinition ConditionMindDominatedByCaster { get; } =
            GetDefinition<ConditionDefinition>("ConditionMindDominatedByCaster");

        internal static ConditionDefinition Condition_MummyLord_ChannelNegativeEnergy { get; } =
            GetDefinition<ConditionDefinition>("Condition_MummyLord_ChannelNegativeEnergy");

        internal static ConditionDefinition ConditionOnAcidPilgrim { get; } =
            GetDefinition<ConditionDefinition>("ConditionOnAcidPilgrim");

        internal static ConditionDefinition ConditionOnFire { get; } =
            GetDefinition<ConditionDefinition>("ConditionOnFire");

        internal static ConditionDefinition ConditionOnFire1D4 { get; } =
            GetDefinition<ConditionDefinition>("ConditionOnFire1D4");

        internal static ConditionDefinition ConditionPactChainImp { get; } =
            GetDefinition<ConditionDefinition>("ConditionPactChainImp");

        internal static ConditionDefinition ConditionPactChainPseudodragon { get; } =
            GetDefinition<ConditionDefinition>("ConditionPactChainPseudodragon");

        internal static ConditionDefinition ConditionPactChainQuasit { get; } =
            GetDefinition<ConditionDefinition>("ConditionPactChainQuasit");

        internal static ConditionDefinition ConditionPactChainSprite { get; } =
            GetDefinition<ConditionDefinition>("ConditionPactChainSprite");

        internal static ConditionDefinition ConditionPainful { get; } =
            GetDefinition<ConditionDefinition>("ConditionPainful");

        internal static ConditionDefinition ConditionPatronTimekeeperCurseOfTime { get; } =
            GetDefinition<ConditionDefinition>("ConditionPatronTimekeeperCurseOfTime");

        internal static ConditionDefinition ConditionParalyzed { get; } =
            GetDefinition<ConditionDefinition>("ConditionParalyzed");

        internal static ConditionDefinition ConditionPassWithoutTrace { get; } =
            GetDefinition<ConditionDefinition>("ConditionPassWithoutTrace");

        internal static ConditionDefinition ConditionPatronHiveWeakeningPheromones { get; } =
            GetDefinition<ConditionDefinition>("ConditionPatronHiveWeakeningPheromones");

        internal static ConditionDefinition ConditionPheromoned { get; } =
            GetDefinition<ConditionDefinition>("ConditionPheromoned");

        internal static ConditionDefinition ConditionPoisoned { get; } =
            GetDefinition<ConditionDefinition>("ConditionPoisoned");

        internal static ConditionDefinition ConditionPossessed { get; } =
            GetDefinition<ConditionDefinition>("ConditionPossessed");

        internal static ConditionDefinition ConditionProne { get; } =
            GetDefinition<ConditionDefinition>("ConditionProne");

        internal static ConditionDefinition ConditionProtectedFromEnergyLightning { get; } =
            GetDefinition<ConditionDefinition>("ConditionProtectedFromEnergyLightning");

        internal static ConditionDefinition ConditionProtectedFromPoison { get; } =
            GetDefinition<ConditionDefinition>("ConditionProtectedFromPoison");

        internal static ConditionDefinition ConditionProtectedInsideMagicCircle { get; } =
            GetDefinition<ConditionDefinition>("ConditionProtectedInsideMagicCircle");

        internal static ConditionDefinition ConditionRaging { get; } =
            GetDefinition<ConditionDefinition>("ConditionRaging");

        internal static ConditionDefinition ConditionRangerHideInPlainSight { get; } =
            GetDefinition<ConditionDefinition>("ConditionRangerHideInPlainSight");

        internal static ConditionDefinition ConditionReckless { get; } =
            GetDefinition<ConditionDefinition>("ConditionReckless");

        internal static ConditionDefinition ConditionRecklessVulnerability { get; } =
            GetDefinition<ConditionDefinition>("ConditionRecklessVulnerability");

        internal static ConditionDefinition ConditionRestrained { get; } =
            GetDefinition<ConditionDefinition>("ConditionRestrained");

        internal static ConditionDefinition ConditionRestrainedByEntangle { get; } =
            GetDefinition<ConditionDefinition>("ConditionRestrainedByEntangle");

        internal static ConditionDefinition ConditionRestrainedByMagicalArrow { get; } =
            GetDefinition<ConditionDefinition>("ConditionRestrainedByMagicalArrow");

        internal static ConditionDefinition ConditionRestrictedInsideMagicCircle { get; } =
            GetDefinition<ConditionDefinition>("ConditionRestrictedInsideMagicCircle");

        internal static ConditionDefinition ConditionRevealedByDetectGoodOrEvil { get; } =
            GetDefinition<ConditionDefinition>("ConditionRevealedByDetectGoodOrEvil");

        internal static ConditionDefinition ConditionRousingShout { get; } =
            GetDefinition<ConditionDefinition>("ConditionRousingShout");

        internal static ConditionDefinition ConditionSeeInvisibility { get; } =
            GetDefinition<ConditionDefinition>("ConditionSeeInvisibility");

        internal static ConditionDefinition ConditionShielded { get; } =
            GetDefinition<ConditionDefinition>("ConditionShielded");

        internal static ConditionDefinition ConditionShine { get; } =
            GetDefinition<ConditionDefinition>("ConditionShine");

        internal static ConditionDefinition ConditionShocked { get; } =
            GetDefinition<ConditionDefinition>("ConditionShocked");

        internal static ConditionDefinition ConditionSleetStorm { get; } =
            GetDefinition<ConditionDefinition>("ConditionSleetStorm");

        internal static ConditionDefinition ConditionSlowed { get; } =
            GetDefinition<ConditionDefinition>("ConditionSlowed");

        internal static ConditionDefinition ConditionSorcererChildRiftDeflection { get; } =
            GetDefinition<ConditionDefinition>("ConditionSorcererChildRiftDeflection");

        internal static ConditionDefinition ConditionSpiritGuardians { get; } =
            GetDefinition<ConditionDefinition>("ConditionSpiritGuardians");

        internal static ConditionDefinition ConditionSpiritGuardiansSelf { get; } =
            GetDefinition<ConditionDefinition>("ConditionSpiritGuardiansSelf");

        internal static ConditionDefinition ConditionStealthy { get; } =
            GetDefinition<ConditionDefinition>("ConditionStealthy");

        internal static ConditionDefinition ConditionStoneResilience { get; } =
            GetDefinition<ConditionDefinition>("ConditionStoneResilience");

        internal static ConditionDefinition ConditionStrikeOfChaosAttackAdvantage { get; } =
            GetDefinition<ConditionDefinition>("ConditionStrikeOfChaosAttackAdvantage");

        internal static ConditionDefinition ConditionStrikeOfChaosAttackDisadvantage { get; } =
            GetDefinition<ConditionDefinition>("ConditionStrikeOfChaosAttackDisadvantage");

        internal static ConditionDefinition ConditionStunned { get; } =
            GetDefinition<ConditionDefinition>("ConditionStunned");

        internal static ConditionDefinition ConditionStunned_MonkStunningStrike { get; } =
            GetDefinition<ConditionDefinition>("ConditionStunned_MonkStunningStrike");

        internal static ConditionDefinition ConditionSunbeam { get; } =
            GetDefinition<ConditionDefinition>("ConditionSunbeam");

        internal static ConditionDefinition ConditionSurprised { get; } =
            GetDefinition<ConditionDefinition>("ConditionSurprised");

        internal static ConditionDefinition ConditionTargetedByGuidingBolt { get; } =
            GetDefinition<ConditionDefinition>("ConditionTargetedByGuidingBolt");

        internal static ConditionDefinition ConditionTraditionSurvivalUnbreakableBody { get; } =
            GetDefinition<ConditionDefinition>("ConditionTraditionSurvivalUnbreakableBody");

        internal static ConditionDefinition ConditionTruesight { get; } =
            GetDefinition<ConditionDefinition>("ConditionTruesight");

        internal static ConditionDefinition ConditionTrueStrike { get; } =
            GetDefinition<ConditionDefinition>("ConditionTrueStrike");

        internal static ConditionDefinition ConditionTurned { get; } =
            GetDefinition<ConditionDefinition>("ConditionTurned");

        internal static ConditionDefinition ConditionVeil { get; } =
            GetDefinition<ConditionDefinition>("ConditionVeil");

        internal static ConditionDefinition ConditionUnderDemonicInfluence { get; } =
            GetDefinition<ConditionDefinition>("ConditionUnderDemonicInfluence");

        internal static ConditionDefinition ConditionWardedByWardingBond { get; } =
            GetDefinition<ConditionDefinition>("ConditionWardedByWardingBond");

        internal static ConditionDefinition ConditionWildShapeSubstituteForm { get; } =
            GetDefinition<ConditionDefinition>("ConditionWildShapeSubstituteForm");
    }

    internal static class DeityDefinitions
    {
        internal static DeityDefinition Einar { get; } = GetDefinition<DeityDefinition>("Einar");
        internal static DeityDefinition Maraike { get; } = GetDefinition<DeityDefinition>("Maraike");
        internal static DeityDefinition Pakri { get; } = GetDefinition<DeityDefinition>("Pakri");
    }

    internal static class EffectProxyDefinitions
    {
        internal static EffectProxyDefinition ProxyWallOfFire_Line { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyWallOfFire_Line");

        internal static EffectProxyDefinition ProxyIndomitableLight { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyIndomitableLight");

        internal static EffectProxyDefinition ProxyDancingLights { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyDancingLights");

        internal static EffectProxyDefinition ProxyArcaneSword { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyArcaneSword");

        internal static EffectProxyDefinition ProxyDaylight { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyDaylight");

        internal static EffectProxyDefinition ProxyEntangle { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyEntangle");

        internal static EffectProxyDefinition ProxyGrease { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyGrease");

        internal static EffectProxyDefinition ProxyGuardianOfFaith { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyGuardianOfFaith");

        internal static EffectProxyDefinition ProxyInsectPlague { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyInsectPlague");

        internal static EffectProxyDefinition ProxySpikeGrowth { get; } =
            GetDefinition<EffectProxyDefinition>("ProxySpikeGrowth");

        internal static EffectProxyDefinition ProxyStinkingCloud { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyStinkingCloud");

        internal static EffectProxyDefinition ProxyCloudKill { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyCloudKill");

        internal static EffectProxyDefinition ProxyFogCloud { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyFogCloud");

        internal static EffectProxyDefinition ProxyIncendiaryCloud { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyIncendiaryCloud");

        internal static EffectProxyDefinition ProxySleetStorm { get; } =
            GetDefinition<EffectProxyDefinition>("ProxySleetStorm");

        internal static EffectProxyDefinition ProxyDarkness { get; } =
            GetDefinition<EffectProxyDefinition>("ProxyDarkness");
    }

    internal static class FactionDefinitions
    {
        internal static FactionDefinition HostileMonsters { get; } =
            GetDefinition<FactionDefinition>("HostileMonsters");

        internal static FactionDefinition Party { get; } = GetDefinition<FactionDefinition>("Party");
    }

    internal static class FactionStatusDefinitions
    {
        internal static FactionStatusDefinition Alliance { get; } = GetDefinition<FactionStatusDefinition>("Alliance");

        internal static FactionStatusDefinition Brotherhood { get; } =
            GetDefinition<FactionStatusDefinition>("Brotherhood");

        internal static FactionStatusDefinition Indifference { get; } =
            GetDefinition<FactionStatusDefinition>("Indifference");

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
        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityConditionRaging { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityConditionRaging");

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

        internal static FeatureDefinitionAbilityCheckAffinity
            AbilityCheckAffinityDomainLawUnyieldingEnforcerShove { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>(
                "AbilityCheckAffinityDomainLawUnyieldingEnforcerShove");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityFeatLockbreaker { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityFeatLockbreaker");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityIslandHalflingAcrobatics { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityIslandHalflingAcrobatics");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityKeenSight { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityKeenSight");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityKeenHearing { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityKeenHearing");

        internal static FeatureDefinitionAbilityCheckAffinity AbilityCheckAffinityStealthDisadvantage { get; } =
            GetDefinition<FeatureDefinitionAbilityCheckAffinity>("AbilityCheckAffinityStealthDisadvantage");
    }

    internal static class FeatureDefinitionActionAffinitys
    {
        internal static FeatureDefinitionActionAffinity ActionAffinityMonkDeflectMissiles { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMonkDeflectMissiles");

        internal static FeatureDefinitionActionAffinity ActionAffinityRangerVanish { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRangerVanish");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionSurprised { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionSurprised");

        internal static FeatureDefinitionActionAffinity ActionAffinityMartialCommanderCoordinatedDefense { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMartialCommanderCoordinatedDefense");

        internal static FeatureDefinitionActionAffinity ActionAffinityAggressive { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityAggressive");

        internal static FeatureDefinitionActionAffinity ActionAffinityBarbarianRecklessAttack { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityBarbarianRecklessAttack");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionRestrained { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionRestrained");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionRetchingReeling { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionRetchingReeling");

        internal static FeatureDefinitionActionAffinity ActionAffinityConditionShocked { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityConditionShocked");

        internal static FeatureDefinitionActionAffinity ActionAffinityGrappled { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityGrappled");

        internal static FeatureDefinitionActionAffinity ActionAffinityFightingStyleProtection { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityFightingStyleProtection");

        internal static FeatureDefinitionActionAffinity ActionAffinityMountaineerShieldCharge { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityMountaineerShieldCharge");

        internal static FeatureDefinitionActionAffinity ActionAffinityRetaliation { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRetaliation");

        internal static FeatureDefinitionActionAffinity ActionAffinityRogueCunningAction { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityRogueCunningAction");

        internal static FeatureDefinitionActionAffinity ActionAffinitySorcererMetamagicToggle { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinitySorcererMetamagicToggle");

        internal static FeatureDefinitionActionAffinity ActionAffinityUncannyDodge { get; } =
            GetDefinition<FeatureDefinitionActionAffinity>("ActionAffinityUncannyDodge");
    }

    internal static class FeatureDefinitionAdditionalActions
    {
        internal static FeatureDefinitionAdditionalAction AdditionalActionHasted { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionHasted");

        internal static FeatureDefinitionAdditionalAction AdditionalActionHunterHordeBreaker { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionHunterHordeBreaker");

        internal static FeatureDefinitionAdditionalAction AdditionalActionSurgedMain { get; } =
            GetDefinition<FeatureDefinitionAdditionalAction>("AdditionalActionSurgedMain");
    }

    internal static class FeatureDefinitionAdditionalDamages
    {
        internal static FeatureDefinitionAdditionalDamage AdditionalDamageConditionRaging { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageConditionRaging");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageBrandingSmite { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageBrandingSmite");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDivineFavor { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDivineFavor");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDomainLifeDivineStrike { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDomainLifeDivineStrike");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageDomainMischiefDivineStrike { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageDomainMischiefDivineStrike");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageHalfOrcSavageAttacks { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageHalfOrcSavageAttacks");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageHuntersMark { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageHuntersMark");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageInvocationAgonizingBlast { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageInvocationAgonizingBlast");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageLifedrinker { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageLifedrinker");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePaladinDivineSmite { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePaladinDivineSmite");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePaladinImprovedDivineSmite { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePaladinImprovedDivineSmite");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamagePoison_GhoulsCaress { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamagePoison_GhoulsCaress");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerFavoredEnemyElemental { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerFavoredEnemyElemental");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRangerSwiftBladeBattleFocus { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRangerSwiftBladeBattleFocus");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageRogueSneakAttack { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageRogueSneakAttack");

        internal static FeatureDefinitionAdditionalDamage
            AdditionalDamageTraditionLightRadiantStrikesLuminousKi { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageTraditionLightRadiantStrikesLuminousKi");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageTraditionLightRadiantStrikesShine { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageTraditionLightRadiantStrikesShine");

        internal static FeatureDefinitionAdditionalDamage AdditionalDamageTraditionShockArcanistArcaneFury { get; } =
            GetDefinition<FeatureDefinitionAdditionalDamage>("AdditionalDamageTraditionShockArcanistArcaneFury");
    }

    internal static class FeatureDefinitionAttackModifiers
    {
        internal static FeatureDefinitionAttackModifier AttackModifierFeatAmbidextrous { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFeatAmbidextrous");

        internal static FeatureDefinitionAttackModifier AttackModifierFightingStyleArchery { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFightingStyleArchery");

        internal static FeatureDefinitionAttackModifier AttackModifierFightingStyleTwoWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFightingStyleTwoWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierFlameBlade2 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierFlameBlade2");

        internal static FeatureDefinitionAttackModifier AttackModifierMagicWeapon { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMagicWeapon");

        internal static FeatureDefinitionAttackModifier AttackModifierMagicWeapon2 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMagicWeapon2");

        internal static FeatureDefinitionAttackModifier AttackModifierMagicWeapon3 { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMagicWeapon3");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus");

        internal static FeatureDefinitionAttackModifier
            AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkMartialArtsImprovedDamage { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkMartialArtsImprovedDamage");

        internal static FeatureDefinitionAttackModifier AttackModifierMonkMartialArtsUnarmedStrikeBonus { get; } =
            GetDefinition<FeatureDefinitionAttackModifier>("AttackModifierMonkMartialArtsUnarmedStrikeBonus");

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
        internal static FeatureDefinitionAttributeModifier AttributeModifierTraditionSurvivalDefensiveStance { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTraditionSurvivalDefensiveStance");

        internal static FeatureDefinitionAttributeModifier AttributeModifierClericChannelDivinity { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierClericChannelDivinity");

        internal static FeatureDefinitionAttributeModifier AttributeModifierPaladinChannelDivinity { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierPaladinChannelDivinity");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianBrutalCriticalAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianBrutalCriticalAdd");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarbarianRagePointsAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarbarianRagePointsAdd");

        internal static FeatureDefinitionAttributeModifier AttributeModifierBarkskin { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierBarkskin");

        internal static FeatureDefinitionAttributeModifier AttributeModifierClericChannelDivinityAdd { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierClericChannelDivinityAdd");

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

        internal static FeatureDefinitionAttributeModifier AttributeModifierDazzled { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDazzled");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDragonbornAbilityScoreIncreaseCha { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDragonbornAbilityScoreIncreaseCha");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDragonbornAbilityScoreIncreaseStr { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDragonbornAbilityScoreIncreaseStr");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfHillAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfHillAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierDwarfAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierDwarfAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier
            AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_DaggerPlus3 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>(
                "AttributeModifierCriticalThresholdDLC3_Dwarven_Weapon_Dagger+3");

        internal static FeatureDefinitionAttributeModifier AttributeModifierElfAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierElfAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierElfHighAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierElfHighAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFighterExtraAttack { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFighterExtraAttack");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFighterIndomitable { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFighterIndomitable");

        internal static FeatureDefinitionAttributeModifier AttributeModifierFighterIndomitableAdd1 { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierFighterIndomitableAdd1");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalflingAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalflingAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHalfOrcAbilityScoreIncreaseCon { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHalfOrcAbilityScoreIncreaseCon");

        internal static FeatureDefinitionAttributeModifier AttributeModifierHumanAbilityScoreIncrease { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierHumanAbilityScoreIncrease");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMartialChampionImprovedCritical { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMartialChampionImprovedCritical");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMartialChampionSuperiorCritical { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMartialChampionSuperiorCritical");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMartialMountainerTunnelFighter { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMartialMountainerTunnelFighter");

        internal static FeatureDefinitionAttributeModifier AttributeModifierMonkUnarmoredDefense { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierMonkUnarmoredDefense");

        internal static FeatureDefinitionAttributeModifier AttributeModifierSorcererSorceryPointsBase { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierSorcererSorceryPointsBase");

        internal static FeatureDefinitionAttributeModifier AttributeModifierTieflingAbilityScoreIncreaseCha { get; } =
            GetDefinition<FeatureDefinitionAttributeModifier>("AttributeModifierTieflingAbilityScoreIncreaseCha");
    }

    internal static class FeatureDefinitionCampAffinitys
    {
        internal static FeatureDefinitionCampAffinity CampAffinityDomainOblivionPeacefulRest { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityDomainOblivionPeacefulRest");

        internal static FeatureDefinitionCampAffinity CampAffinityElfTrance { get; } =
            GetDefinition<FeatureDefinitionCampAffinity>("CampAffinityElfTrance");
    }

    internal static class FeatureDefinitionLightAffinitys
    {
        internal static FeatureDefinitionLightAffinity LightAffinityInvocationOneWithShadows { get; } =
            GetDefinition<FeatureDefinitionLightAffinity>("LightAffinityInvocationOneWithShadows");
    }

    internal static class FeatureDefinitionCastSpells
    {
        internal static FeatureDefinitionCastSpell CastSpellBard { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellBard");

        internal static FeatureDefinitionCastSpell CastSpellCleric { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellCleric");

        internal static FeatureDefinitionCastSpell CastSpellDruid { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellDruid");

        internal static FeatureDefinitionCastSpell CastSpellElfHigh { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellElfHigh");

        internal static FeatureDefinitionCastSpell CastSpellGnomeShadow { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellGnomeShadow");

        internal static FeatureDefinitionCastSpell CastSpellMartialSpellBlade { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellMartialSpellBlade");

        internal static FeatureDefinitionCastSpell CastSpellPaladin { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellPaladin");

        internal static FeatureDefinitionCastSpell CastSpellRanger { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellRanger");

        internal static FeatureDefinitionCastSpell CastSpellShadowcaster { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellShadowcaster");

        internal static FeatureDefinitionCastSpell CastSpellSorcerer { get; } =
            GetDefinition<FeatureDefinitionCastSpell>("CastSpellSorcerer");

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
        internal static FeatureDefinitionCombatAffinity CombatAffinityStunnedAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStunnedAdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityAdamantinePlateArmor { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityAdamantinePlateArmor");

        internal static FeatureDefinitionCombatAffinity CombatAffinityBlinded { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityBlinded");

        internal static FeatureDefinitionCombatAffinity CombatAffinityDisengaging { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityDisengaging");

        internal static FeatureDefinitionCombatAffinity CombatAffinityEagerForBattle { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityEagerForBattle");

        internal static FeatureDefinitionCombatAffinity CombatAffinityEnfeebled { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityEnfeebled");

        internal static FeatureDefinitionCombatAffinity CombatAffinityForeknowledge { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityForeknowledge");

        internal static FeatureDefinitionCombatAffinity CombatAffinityFlyby { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityFlyby");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHeavilyObscured { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHeavilyObscured");

        internal static FeatureDefinitionCombatAffinity CombatAffinityHeavilyObscuredSelf { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityHeavilyObscuredSelf");

        internal static FeatureDefinitionCombatAffinity CombatAffinityInvisible { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityInvisible");

        internal static FeatureDefinitionCombatAffinity CombatAffinityInvisibleStalker { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityInvisibleStalker");

        internal static FeatureDefinitionCombatAffinity CombatAffinityPackTactics { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityPackTactics");

        internal static FeatureDefinitionCombatAffinity CombatAffinityParalyzedAdvantage { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityParalyzedAdvantage");

        internal static FeatureDefinitionCombatAffinity CombatAffinityPoisoned { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityPoisoned");

        internal static FeatureDefinitionCombatAffinity CombatAffinityProtectedFromEvil { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityProtectedFromEvil");

        internal static FeatureDefinitionCombatAffinity CombatAffinityReckless { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityReckless");

        internal static FeatureDefinitionCombatAffinity CombatAffinitySensitiveToLight { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinitySensitiveToLight");

        internal static FeatureDefinitionCombatAffinity CombatAffinityStealthy { get; } =
            GetDefinition<FeatureDefinitionCombatAffinity>("CombatAffinityStealthy");
    }

    internal static class FeatureDefinitionConditionAffinitys
    {
        internal static FeatureDefinitionConditionAffinity ConditionAffinityBlindnessImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityBlindnessImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCalmEmotionCharmedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCalmEmotionCharmedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCharmImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCharmImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCharmImmunityHypnoticPattern { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCharmImmunityHypnoticPattern");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandNaturesWardCharmed { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandNaturesWardCharmed");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityCircleLandNaturesWardFrightened { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityCircleLandNaturesWardFrightened");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityDemonicInfluenceImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityDemonicInfluenceImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityDiseaseImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityDiseaseImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityElfFeyAncestryCharm { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityElfFeyAncestryCharm");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityElfFeyAncestrySleep { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityElfFeyAncestrySleep");

        internal static FeatureDefinitionConditionAffinity
            ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>(
                "ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityExhaustionImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityExhaustionImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityFrightenedFearImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityFrightenedFearImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityHalflingBrave { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityHalflingBrave");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityInvocationDevilsSight { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityInvocationDevilsSight");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityMindControlledImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMindControlledImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityMindDominatedImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityMindDominatedImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityPoisonImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityPoisonImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityProneImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityProneImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityRestrainedmmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityRestrainedmmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityVeilImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityVeilImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityWeatherChilledImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityWeatherChilledImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityWeatherChilledInsteadOfFrozenImmunity
        {
            get;
        } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityWeatherChilledInsteadOfFrozenImmunity");

        internal static FeatureDefinitionConditionAffinity ConditionAffinityWeatherFrozenImmunity { get; } =
            GetDefinition<FeatureDefinitionConditionAffinity>("ConditionAffinityWeatherFrozenImmunity");
    }

    internal static class FeatureDefinitionDamageAffinitys
    {
        internal static FeatureDefinitionDamageAffinity DamageAffinityBarbarianRelentlessRage { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBarbarianRelentlessRage");

        internal static FeatureDefinitionDamageAffinity DamageAffinityStoneskinBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityStoneskinBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityStoneskinPiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityStoneskinPiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityStoneskinSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityStoneskinSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPatronTreePiercingBranch { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPatronTreePiercingBranch");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPatronTreePiercingBranchOneWithTheTree { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPatronTreePiercingBranchOneWithTheTree");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionRagingBludgeoning { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionRagingBludgeoning");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionRagingPiercing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionRagingPiercing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityConditionRagingSlashing { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityConditionRagingSlashing");

        internal static FeatureDefinitionDamageAffinity DamageAffinityAcidResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityAcidResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityBludgeoningResistanceTrue { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityBludgeoningResistanceTrue");

        internal static FeatureDefinitionDamageAffinity DamageAffinityColdImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityColdImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireVulnerability");

        internal static FeatureDefinitionDamageAffinity DamageAffinityColdResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityColdResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityFireResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityFireResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityForceDamageResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityForceDamageResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityHalfOrcRelentlessEndurance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityHalfOrcRelentlessEndurance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityLightningImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityLightningImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityLightningResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityLightningResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityNecroticImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityNecroticImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityNecroticResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityNecroticResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingResistanceTrue { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingResistanceTrue");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPoisonImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPoisonImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPoisonResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPoisonResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPsychicImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPsychicImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPsychicResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPsychicResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityRadiantImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityRadiantImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityRadiantResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityRadiantResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinitySlashingResistanceTrue { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinitySlashingResistanceTrue");

        internal static FeatureDefinitionDamageAffinity DamageAffinityThunderImmunity { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityThunderImmunity");

        internal static FeatureDefinitionDamageAffinity DamageAffinityThunderResistance { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityThunderResistance");

        internal static FeatureDefinitionDamageAffinity DamageAffinityPiercingVulnerability { get; } =
            GetDefinition<FeatureDefinitionDamageAffinity>("DamageAffinityPiercingVulnerability");
    }

    internal static class FeatureDefinitionEquipmentAffinitys
    {
        internal static FeatureDefinitionEquipmentAffinity EquipmentAffinityFeatHauler { get; } =
            GetDefinition<FeatureDefinitionEquipmentAffinity>("EquipmentAffinityFeatHauler");
    }

    internal static class FeatureDefinitionFeatureSets
    {
        internal static FeatureDefinitionFeatureSet FeatureSetMonkTimelessBody { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkTimelessBody");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkPurityOfBody { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkPurityOfBody");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkTongueSunMoon { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkTongueSunMoon");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkStillnessOfMind { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkStillnessOfMind");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkDeflectMissiles { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkDeflectMissiles");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionSurvivalUnbreakableBody { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionSurvivalUnbreakableBody");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionSurvivalDefensiveStance { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionSurvivalDefensiveStance");

        internal static FeatureDefinitionFeatureSet FeatureSetPactSelection { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactSelection");

        internal static FeatureDefinitionFeatureSet FeatureSetClericRitualCasting { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetClericRitualCasting");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererChildRiftRiftwalk { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererChildRiftRiftwalk");

        internal static FeatureDefinitionFeatureSet AdditionalDamageRangerFavoredEnemyChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("AdditionalDamageRangerFavoredEnemyChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetAbilityScoreChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetAbilityScoreChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetBarbarianBrutalCritical { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBarbarianBrutalCritical");

        internal static FeatureDefinitionFeatureSet FeatureSetBarbarianRage { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetBarbarianRage");

        internal static FeatureDefinitionFeatureSet FeatureSetDragonbornBreathWeapon { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetDragonbornBreathWeapon");

        internal static FeatureDefinitionFeatureSet FeatureSetInvocationDevilsSight { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetInvocationDevilsSight");

        internal static FeatureDefinitionFeatureSet FeatureSetElfFeyAncestry { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetElfFeyAncestry");

        internal static FeatureDefinitionFeatureSet FeatureSetElfHighLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetElfHighLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetGreenmageWardenOfTheForest { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetGreenmageWardenOfTheForest");

        internal static FeatureDefinitionFeatureSet FeatureSetHumanLanguages { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetHumanLanguages");

        internal static FeatureDefinitionFeatureSet FeatureSetKindredSpiritChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetKindredSpiritChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkFlurryOfBlows { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkFlurryOfBlows");

        internal static FeatureDefinitionFeatureSet FeatureSetMonkStepOfTheWind { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetMonkStepOfTheWind");

        internal static FeatureDefinitionFeatureSet FeatureSetPactBlade { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactBlade");

        internal static FeatureDefinitionFeatureSet FeatureSetPactChain { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactChain");

        internal static FeatureDefinitionFeatureSet FeatureSetPactTome { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPactTome");

        internal static FeatureDefinitionFeatureSet FeatureSetPathClawDragonAncestry { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetPathClawDragonAncestry");

        internal static FeatureDefinitionFeatureSet FeatureSetSorcererDraconicChoice { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetSorcererDraconicChoice");

        internal static FeatureDefinitionFeatureSet FeatureSetTieflingHellishResistance { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTieflingHellishResistance");

        internal static FeatureDefinitionFeatureSet FeatureSetTraditionLightRadiantStrikes { get; } =
            GetDefinition<FeatureDefinitionFeatureSet>("FeatureSetTraditionLightRadiantStrikes");

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

        internal static FeatureDefinitionHealingModifier HealingModifierKindredSpiritBond { get; } =
            GetDefinition<FeatureDefinitionHealingModifier>("HealingModifierKindredSpiritBond");
    }

    internal static class FeatureDefinitionMagicAffinitys
    {
        internal static FeatureDefinitionMagicAffinity MagicAffinityBattleMagic { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityBattleMagic");

        internal static FeatureDefinitionMagicAffinity MagicAffinityChitinousBoonAdditionalSpellSlot { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityChitinousBoonAdditionalSpellSlot");

        internal static FeatureDefinitionMagicAffinity MagicAffinityConditionShielded { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityConditionShielded");

        internal static FeatureDefinitionMagicAffinity MagicAffinitySpellBladeIntoTheFray { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinitySpellBladeIntoTheFray");

        internal static FeatureDefinitionMagicAffinity MagicAffinityAdditionalSpellSlot1 { get; } =
            GetDefinition<FeatureDefinitionMagicAffinity>("MagicAffinityAdditionalSpellSlot1");
    }

    internal static class FeatureDefinitionMovementAffinitys
    {
        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionSurprised { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionSurprised");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionDashing { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionDashing");

        internal static FeatureDefinitionMovementAffinity MovementAffinityBarbarianFastMovement { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityBarbarianFastMovement");

        internal static FeatureDefinitionMovementAffinity MovementAffinityCarriedByWind { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityCarriedByWind");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionSlowed { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionSlowed");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionFlyingAdaptive { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionFlyingAdaptive");

        internal static FeatureDefinitionMovementAffinity MovementAffinityConditionRestrained { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityConditionRestrained");

        internal static FeatureDefinitionMovementAffinity MovementAffinityFreedomOfMovement { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityFreedomOfMovement");

        internal static FeatureDefinitionMovementAffinity MovementAffinityMonkUnarmoredMovementImproved { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityMonkUnarmoredMovementImproved");

        internal static FeatureDefinitionMovementAffinity MovementAffinityNoClimb { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityNoClimb");

        internal static FeatureDefinitionMovementAffinity MovementAffinityNoSpecialMoves { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinityNoSpecialMoves");

        internal static FeatureDefinitionMovementAffinity MovementAffinitySpiderClimb { get; } =
            GetDefinition<FeatureDefinitionMovementAffinity>("MovementAffinitySpiderClimb");
    }

    internal static class FeatureDefinitionMoveModes
    {
        internal static FeatureDefinitionMoveMode MoveModeFly2 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly2");

        internal static FeatureDefinitionMoveMode MoveModeFly4 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly4");

        internal static FeatureDefinitionMoveMode MoveModeFly6 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly6");

        internal static FeatureDefinitionMoveMode MoveModeFly8 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly8");

        internal static FeatureDefinitionMoveMode MoveModeFly12 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeFly12");

        internal static FeatureDefinitionMoveMode MoveModeMove2 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove2");

        internal static FeatureDefinitionMoveMode MoveModeMove4 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove4");

        internal static FeatureDefinitionMoveMode MoveModeMove6 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove6");

        internal static FeatureDefinitionMoveMode MoveModeMove7 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove7");

        internal static FeatureDefinitionMoveMode MoveModeMove8 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove8");

        internal static FeatureDefinitionMoveMode MoveModeMove12 { get; } =
            GetDefinition<FeatureDefinitionMoveMode>("MoveModeMove12");
    }

    internal static class FeatureDefinitionPointPools
    {
        internal static FeatureDefinitionPointPool PointPoolBardExpertiseLevel3 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardExpertiseLevel3");

        internal static FeatureDefinitionPointPool PointPoolBardExpertiseLevel10 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardExpertiseLevel10");

        internal static FeatureDefinitionPointPool PointPoolAbilityScoreImprovement { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolAbilityScoreImprovement");

        internal static FeatureDefinitionPointPool PointPoolBarbarianrSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBarbarianrSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolBardMagicalSecrets10 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardMagicalSecrets10");

        internal static FeatureDefinitionPointPool PointPoolBardMagicalSecrets14 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardMagicalSecrets14");

        internal static FeatureDefinitionPointPool PointPoolBardSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBardSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolBonusFeat { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBonusFeat");

        internal static FeatureDefinitionPointPool PointPoolClericSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolClericSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolDruidSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolDruidSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolFighterSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolFighterSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolHalfElfSkillPool { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHalfElfSkillPool");

        internal static FeatureDefinitionPointPool PointPoolHumanSkillPool { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolHumanSkillPool");

        internal static FeatureDefinitionPointPool PointPoolBackgroundLanguageChoice_one { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBackgroundLanguageChoice_one");

        internal static FeatureDefinitionPointPool PointPoolBackgroundLanguageChoice_two { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolBackgroundLanguageChoice_two");

        internal static FeatureDefinitionPointPool PointPoolMonkSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolMonkSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolPaladinSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolPaladinSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolRangerSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolRangerSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolRogueSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolRogueSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolSorcererMetamagic { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolSorcererMetamagic");

        internal static FeatureDefinitionPointPool PointPoolSorcererAdditionalMetamagic { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolSorcererAdditionalMetamagic");

        internal static FeatureDefinitionPointPool PointPoolSorcererSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolSorcererSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolWarlockSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolWizardSkillPoints { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWizardSkillPoints");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation2 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation2");

        internal static FeatureDefinitionPointPool PointPoolWarlockInvocation5 { get; } =
            GetDefinition<FeatureDefinitionPointPool>("PointPoolWarlockInvocation5");
    }

    internal static class FeatureDefinitionPowers
    {
        internal static FeatureDefinitionPower PowerMonkReturnMissile { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkReturnMissile");

        internal static FeatureDefinitionPower PowerDefilerEatFriends { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDefilerEatFriends");

        internal static FeatureDefinitionPower PowerBarbarianPersistentRageStart { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBarbarianPersistentRageStart");

        internal static FeatureDefinitionPower PowerBardCountercharm { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardCountercharm");

        internal static FeatureDefinitionPower PowerTraditionShockArcanistGreaterArcaneShock { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionShockArcanistGreaterArcaneShock");

        internal static FeatureDefinitionPower PowerBerserkerFrenzy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBerserkerFrenzy");

        internal static FeatureDefinitionPower PowerWindGuidingWinds { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWindGuidingWinds");

        internal static FeatureDefinitionPower PowerBardTraditionVerbalOnslaught { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardTraditionVerbalOnslaught");

        internal static FeatureDefinitionPower PowerIncubus_Drain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerIncubus_Drain");

        internal static FeatureDefinitionPower PowerSessrothTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSessrothTeleport");

        internal static FeatureDefinitionPower PowerSymbolOfHopelessness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSymbolOfHopelessness");

        internal static FeatureDefinitionPower PowerBulette_Snow_Leap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBulette_Snow_Leap");

        internal static FeatureDefinitionPower PowerDomainSunIndomitableLight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainSunIndomitableLight");

        internal static FeatureDefinitionPower PowerSorakWordOfDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakWordOfDarkness");

        internal static FeatureDefinitionPower PowerMagebaneWarcry { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMagebaneWarcry");

        internal static FeatureDefinitionPower PowerOathOfMotherlandFieryWrath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfMotherlandFieryWrath");

        internal static FeatureDefinitionPower PowerPaladinCleansingTouch { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinCleansingTouch");

        internal static FeatureDefinitionPower PowerGreen_Hag_Invisibility { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerGreen_Hag_Invisibility");

        internal static FeatureDefinitionPower PowerPatronTimekeeperAccelerate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTimekeeperAccelerate");

        internal static FeatureDefinitionPower PowerMartialSpellbladeArcaneEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialSpellbladeArcaneEscape");

        internal static FeatureDefinitionPower PowerBarbarianRageStart { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBarbarianRageStart");

        internal static FeatureDefinitionPower PowerBardTraditionManacalonsPerfection { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardTraditionManacalonsPerfection");

        internal static FeatureDefinitionPower PowerBardGiveBardicInspiration { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardGiveBardicInspiration");

        internal static FeatureDefinitionPower PowerBardHeroismAtRoadsEnd { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHeroismAtRoadsEnd");

        internal static FeatureDefinitionPower PowerBardHopeSingSongOfHope { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBardHopeSingSongOfHope");

        internal static FeatureDefinitionPower PowerBerserkerIntimidatingPresence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBerserkerIntimidatingPresence");

        internal static FeatureDefinitionPower PowerBerserkerMindlessRage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerBerserkerMindlessRage");

        internal static FeatureDefinitionPower PowerCircleLandNaturalRecovery { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerCircleLandNaturalRecovery");

        internal static FeatureDefinitionPower PowerClericDivineInterventionCleric { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericDivineInterventionCleric");

        internal static FeatureDefinitionPower PowerClericDivineInterventionPaladin { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericDivineInterventionPaladin");

        internal static FeatureDefinitionPower PowerClericDivineInterventionWizard { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericDivineInterventionWizard");

        internal static FeatureDefinitionPower PowerClericTurnUndead { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead");

        internal static FeatureDefinitionPower PowerClericTurnUndead5 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead5");

        internal static FeatureDefinitionPower PowerClericTurnUndead11 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead11");

        internal static FeatureDefinitionPower PowerClericTurnUndead14 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerClericTurnUndead14");

        internal static FeatureDefinitionPower PowerCollegeLoreCuttingWords { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerCollegeLoreCuttingWords");

        internal static FeatureDefinitionPower PowerDefilerDarkness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDefilerDarkness");

        internal static FeatureDefinitionPower PowerDruidCircleBalanceBalanceOfPower { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDruidCircleBalanceBalanceOfPower");

        internal static FeatureDefinitionPower PowerPhaseMarilithTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPhaseMarilithTeleport");

        internal static FeatureDefinitionPower PowerDomainBattleDecisiveStrike { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleDecisiveStrike");

        internal static FeatureDefinitionPower PowerPatronFiendDarkOnesBlessing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronFiendDarkOnesBlessing");

        internal static FeatureDefinitionPower PowerDomainBattleHeraldOfBattle { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainBattleHeraldOfBattle");

        internal static FeatureDefinitionPower PowerDomainElementalFireBurst { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalFireBurst");

        internal static FeatureDefinitionPower PowerDomainElementalHeraldOfTheElementsCold { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalHeraldOfTheElementsCold");

        internal static FeatureDefinitionPower PowerDomainElementalHeraldOfTheElementsThunder { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalHeraldOfTheElementsThunder");

        internal static FeatureDefinitionPower PowerDomainElementalIceLance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalIceLance");

        internal static FeatureDefinitionPower PowerDomainElementalLightningBlade { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainElementalLightningBlade");

        internal static FeatureDefinitionPower PowerDomainInsightForeknowledge { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainInsightForeknowledge");

        internal static FeatureDefinitionPower PowerDomainLawForceOfLaw { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawForceOfLaw");

        internal static FeatureDefinitionPower PowerDomainLawHolyRetribution { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawHolyRetribution");

        internal static FeatureDefinitionPower PowerDomainLawWordOfLaw { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLawWordOfLaw");

        internal static FeatureDefinitionPower PowerDomainLifePreserveLife { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainLifePreserveLife");

        internal static FeatureDefinitionPower PowerDomainMischiefStrikeOfChaos14 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainMischiefStrikeOfChaos14");

        internal static FeatureDefinitionPower PowerDomainOblivionMarkOfFate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainOblivionMarkOfFate");

        internal static FeatureDefinitionPower PowerDomainSunHeraldOfTheSun { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDomainSunHeraldOfTheSun");

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

        internal static FeatureDefinitionPower PowerDragonBreath_Fire { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Fire");

        internal static FeatureDefinitionPower PowerDragonBreath_Poison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_Poison");

        internal static FeatureDefinitionPower PowerDragonBreath_YoungGreen_Poison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonBreath_YoungGreen_Poison");

        internal static FeatureDefinitionPower PowerDragonFrightfulPresence { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDragonFrightfulPresence");

        internal static FeatureDefinitionPower PowerKindredSpiritRage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKindredSpiritRage");

        internal static FeatureDefinitionPower PowerDruidWildShape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerDruidWildShape");

        internal static FeatureDefinitionPower PowerFighterActionSurge { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterActionSurge");

        internal static FeatureDefinitionPower PowerFighterSecondWind { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFighterSecondWind");

        internal static FeatureDefinitionPower PowerFunctionGoodberryHealing { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionGoodberryHealing");

        internal static FeatureDefinitionPower PowerFunctionGoodberryHealingOther { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionGoodberryHealingOther");

        internal static FeatureDefinitionPower PowerFunctionWandFearCommand { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionWandFearCommand");

        internal static FeatureDefinitionPower PowerFunctionWandFearCone { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerFunctionWandFearCone");

        internal static FeatureDefinitionPower Power_HornOfBlasting { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_HornOfBlasting");

        internal static FeatureDefinitionPower PowerInvocationRepellingBlast { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerInvocationRepellingBlast");

        internal static FeatureDefinitionPower PowerKnightLeadership { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerKnightLeadership");

        internal static FeatureDefinitionPower PowerMagebaneSpellCrusher { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMagebaneSpellCrusher");

        internal static FeatureDefinitionPower PowerMarksmanRecycler { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMarksmanRecycler");

        internal static FeatureDefinitionPower PowerMartialCommanderInvigoratingShout { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialCommanderInvigoratingShout");

        internal static FeatureDefinitionPower PowerMartialCommanderRousingShout { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMartialCommanderRousingShout");

        internal static FeatureDefinitionPower PowerMelekTeleport { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMelekTeleport");

        internal static FeatureDefinitionPower PowerMonkFlurryOfBlows { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkFlurryOfBlows");

        internal static FeatureDefinitionPower PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement");

        internal static FeatureDefinitionPower PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement");

        internal static FeatureDefinitionPower PowerMonkMartialArts { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkMartialArts");

        internal static FeatureDefinitionPower PowerMonkPatientDefense { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkPatientDefense");

        internal static FeatureDefinitionPower PowerMonkPatientDefenseSurvival3 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkPatientDefenseSurvival3");

        internal static FeatureDefinitionPower PowerMonkPatientDefenseSurvival6 { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkPatientDefenseSurvival6");

        internal static FeatureDefinitionPower PowerMonkStepOfTheWindDash { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkStepOfTheWindDash");

        internal static FeatureDefinitionPower PowerMonkStepOftheWindDisengage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkStepOftheWindDisengage");

        internal static FeatureDefinitionPower PowerMonkStunningStrike { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerMonkStunningStrike");

        internal static FeatureDefinitionPower Power_Mummy_DreadfulGlare { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_Mummy_DreadfulGlare");

        internal static FeatureDefinitionPower Power_MummyLord_DreadfulGlare { get; } =
            GetDefinition<FeatureDefinitionPower>("Power_MummyLord_DreadfulGlare");

        internal static FeatureDefinitionPower PowerOathOfDevotionAuraDevotion { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfDevotionAuraDevotion");

        internal static FeatureDefinitionPower PowerOathOfDevotionTurnUnholy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfDevotionTurnUnholy");

        internal static FeatureDefinitionPower PowerOathOfJugementAuraRightenousness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementAuraRightenousness");

        internal static FeatureDefinitionPower PowerOathOfJugementPurgeCorruption { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementPurgeCorruption");

        internal static FeatureDefinitionPower PowerOathOfJugementWeightOfJustice { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfJugementWeightOfJustice");

        internal static FeatureDefinitionPower PowerOathOfMotherlandVolcanicAura { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfMotherlandVolcanicAura");

        internal static FeatureDefinitionPower PowerOathOfTirmarAuraTruth { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfTirmarAuraTruth");

        internal static FeatureDefinitionPower PowerOathOfTirmarGoldenSpeech { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerOathOfTirmarGoldenSpeech");

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

        internal static FeatureDefinitionPower PowerPaladinCureDisease { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinCureDisease");

        internal static FeatureDefinitionPower PowerPaladinLayOnHands { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinLayOnHands");

        internal static FeatureDefinitionPower PowerPaladinNeutralizePoison { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPaladinNeutralizePoison");

        internal static FeatureDefinitionPower PowerPatronFiendDarkOnesOwnLuck { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronFiendDarkOnesOwnLuck");

        internal static FeatureDefinitionPower PowerPatronHiveReactiveCarapace { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronHiveReactiveCarapace");

        internal static FeatureDefinitionPower PowerPatronTimekeeperTimeShift { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTimekeeperTimeShift");

        internal static FeatureDefinitionPower PowerPatronTreeExplosiveGrowth { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerPatronTreeExplosiveGrowth");

        internal static FeatureDefinitionPower PowerRangerHideInPlainSight { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerHideInPlainSight");

        internal static FeatureDefinitionPower PowerRangerPrimevalAwareness { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerPrimevalAwareness");

        internal static FeatureDefinitionPower PowerRangerSwiftBladeBattleFocus { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRangerSwiftBladeBattleFocus");

        internal static FeatureDefinitionPower PowerRoguishHoodlumDirtyFighting { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRoguishHoodlumDirtyFighting");

        internal static FeatureDefinitionPower PowerRoguishDarkweaverShadowy { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerRoguishDarkweaverShadowy");

        internal static FeatureDefinitionPower PowerSessrothBreath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSessrothBreath");

        internal static FeatureDefinitionPower PowerSorakAssassinShadowMurder { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakAssassinShadowMurder");

        internal static FeatureDefinitionPower PowerShadowcasterShadowDodge { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerShadowcasterShadowDodge");

        internal static FeatureDefinitionPower PowerShadowTamerRopeGrapple { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerShadowTamerRopeGrapple");

        internal static FeatureDefinitionPower PowerSorakDreadLaughter { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakDreadLaughter");

        internal static FeatureDefinitionPower PowerSorakShadowEscape { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorakShadowEscape");

        internal static FeatureDefinitionPower PowerSorcererChildRiftOffering { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftOffering");

        internal static FeatureDefinitionPower PowerSorcererCreateSpellSlot { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererCreateSpellSlot");

        internal static FeatureDefinitionPower PowerSorcererDraconicDragonWingsSprout { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererDraconicDragonWingsSprout");

        internal static FeatureDefinitionPower PowerSorcererDraconicElementalResistance { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererDraconicElementalResistance");

        internal static FeatureDefinitionPower PowerSorcererHauntedSoulSpiritVisage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererHauntedSoulSpiritVisage");

        internal static FeatureDefinitionPower PowerSorcererHauntedSoulVengefulSpirits { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererHauntedSoulVengefulSpirits");

        internal static FeatureDefinitionPower PowerTraditionCourtMageExpandedSpellShield { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionCourtMageExpandedSpellShield");

        internal static FeatureDefinitionPower PowerSorcererManaPainterDrain { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererManaPainterDrain");

        internal static FeatureDefinitionPower PowerSorcererManaPainterTap { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererManaPainterTap");

        internal static FeatureDefinitionPower PowerSorcererChildRiftDeflection { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftDeflection");

        internal static FeatureDefinitionPower PowerSorcererChildRiftRiftwalk { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftRiftwalk");

        internal static FeatureDefinitionPower PowerSorcererChildRiftRiftwalkLandingDamage { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSorcererChildRiftRiftwalkLandingDamage");

        internal static FeatureDefinitionPower PowerSpellBladeSpellTyrant { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSpellBladeSpellTyrant");

        internal static FeatureDefinitionPower PowerSpiderQueenPoisonCloud { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerSpiderQueenPoisonCloud");

        internal static FeatureDefinitionPower PowerTraditionCourtMageSpellShield { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionCourtMageSpellShield");

        internal static FeatureDefinitionPower PowerTraditionLightBlindingFlash { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionLightBlindingFlash");

        internal static FeatureDefinitionPower PowerTraditionLightLuminousKi { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionLightLuminousKi");

        internal static FeatureDefinitionPower PowerTraditionOpenHandTranquility { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandTranquility");

        internal static FeatureDefinitionPower PowerTraditionOpenHandWholenessOfBody { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionOpenHandWholenessOfBody");

        internal static FeatureDefinitionPower PowerTraditionShockArcanistArcaneFury { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionShockArcanistArcaneFury");

        internal static FeatureDefinitionPower PowerTraditionSurvivalUnbreakableBody { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerTraditionSurvivalUnbreakableBody");

        internal static FeatureDefinitionPower PowerVrockSpores { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerVrockSpores");

        internal static FeatureDefinitionPower PowerWightLord_CircleOfDeath { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWightLord_CircleOfDeath");

        internal static FeatureDefinitionPower PowerWightLordRetaliate { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWightLordRetaliate");

        internal static FeatureDefinitionPower PowerWindShelteringBreeze { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWindShelteringBreeze");

        internal static FeatureDefinitionPower PowerWizardArcaneRecovery { get; } =
            GetDefinition<FeatureDefinitionPower>("PowerWizardArcaneRecovery");
    }

    internal static class FeatureDefinitionProficiencys
    {
        internal static FeatureDefinitionProficiency ProficiencyAllLanguagesButCode { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAllLanguagesButCode");

        internal static FeatureDefinitionProficiency ProficiencyMonkWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkWeapon");

        internal static FeatureDefinitionProficiency ProficiencyRogueSlipperyMind { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueSlipperyMind");

        internal static FeatureDefinitionProficiency ProficiencyElfWeaponTraining { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyElfWeaponTraining");

        internal static FeatureDefinitionProficiency ProficiencyHumanStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyHumanStaticLanguages");

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

        internal static FeatureDefinitionProficiency ProficiencyAristocratSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyAristocratSkills");

        internal static FeatureDefinitionProficiency ProficiencyArtistSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyArtistSkills");

        internal static FeatureDefinitionProficiency ProficiencyBarbarianArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBarbarianArmor");

        internal static FeatureDefinitionProficiency ProficiencyBarbarianSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBarbarianSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyBardSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBardSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyBardWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyBardWeapon");

        internal static FeatureDefinitionProficiency ProficiencyClericSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyClericSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyClericWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyClericWeapon");

        internal static FeatureDefinitionProficiency ProficiencyDruidArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidArmor");

        internal static FeatureDefinitionProficiency ProficiencyDruidSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyDruidWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDruidWeapon");

        internal static FeatureDefinitionProficiency ProficiencyDwarfLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyDwarfLanguages");

        internal static FeatureDefinitionProficiency ProficiencyFeatLockbreaker { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatLockbreaker");

        internal static FeatureDefinitionProficiency ProficiencyFighterArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFighterArmor");

        internal static FeatureDefinitionProficiency ProficiencyFighterSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFighterSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyFighterWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFighterWeapon");

        internal static FeatureDefinitionProficiency ProficiencyLawkeeperSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLawkeeperSkills");

        internal static FeatureDefinitionProficiency ProficiencyLowlifeSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLowlifeSkills");

        internal static FeatureDefinitionProficiency ProficiencyLowLifeSkillsTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyLowLifeSkillsTools");

        internal static FeatureDefinitionProficiency ProficiencyMonkSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyMonkSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyOccultistSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyOccultistSkills");

        internal static FeatureDefinitionProficiency ProficiencyOccultistToolsSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyOccultistToolsSkills");

        internal static FeatureDefinitionProficiency ProficiencyPaladinArmor { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPaladinArmor");

        internal static FeatureDefinitionProficiency ProficiencyPaladinSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPaladinSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyPhilosopherSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPhilosopherSkills");

        internal static FeatureDefinitionProficiency ProficiencyPhilosopherTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyPhilosopherTools");

        internal static FeatureDefinitionProficiency ProficiencyRangerSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRangerSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyRogueSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyRogueWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyRogueWeapon");

        internal static FeatureDefinitionProficiency ProficiencySellSwordSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySellSwordSkills");

        internal static FeatureDefinitionProficiency ProficiencySmithTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySmithTools");

        internal static FeatureDefinitionProficiency ProficiencySorcererSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySorcererSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencySorcererWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySorcererWeapon");

        internal static FeatureDefinitionProficiency ProficiencySpySkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencySpySkills");

        internal static FeatureDefinitionProficiency ProficiencyTieflingStaticLanguages { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyTieflingStaticLanguages");

        internal static FeatureDefinitionProficiency ProficiencyWandererSkills { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWandererSkills");

        internal static FeatureDefinitionProficiency ProficiencyWandererTools { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWandererTools");

        internal static FeatureDefinitionProficiency ProficiencyWarlockSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWarlockSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyWizardSavingThrow { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWizardSavingThrow");

        internal static FeatureDefinitionProficiency ProficiencyWizardWeapon { get; } =
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyWizardWeapon");
    }

    internal static class FeatureDefinitionDieRollModifiers
    {
        internal static FeatureDefinitionDieRollModifier DieRollModifierFightingStyleGreatWeapon { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierFightingStyleGreatWeapon");

        internal static FeatureDefinitionDieRollModifier DieRollModifierHalfingLucky { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierHalfingLucky");

        internal static FeatureDefinitionDieRollModifier DieRollModifierRogueReliableTalent { get; } =
            GetDefinition<FeatureDefinitionDieRollModifier>("DieRollModifierRogueReliableTalent");
    }

    internal static class FeatureDefinitionRegenerations
    {
        internal static FeatureDefinitionRegeneration RegenerationRing { get; } =
            GetDefinition<FeatureDefinitionRegeneration>("RegenerationRing");
    }

    internal static class FeatureDefinitionRestHealingModifiers
    {
        internal static FeatureDefinitionRestHealingModifier RestHealingModifierBardHealingBallad { get; } =
            GetDefinition<FeatureDefinitionRestHealingModifier>("RestHealingModifierBardHealingBallad");

        internal static FeatureDefinitionRestHealingModifier RestHealingModifierBardSongOfRest { get; } =
            GetDefinition<FeatureDefinitionRestHealingModifier>("RestHealingModifierBardSongOfRest");
    }

    internal static class FeatureDefinitionSavingThrowAffinitys
    {
        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityAdvantageToAll { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityAdvantageToAll");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinitySpellResistance { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinitySpellResistance");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityAntitoxin { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityAntitoxin");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBaned { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBaned");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionBlinded { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionBlinded");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionHasted { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionHasted");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityConditionRaging { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityConditionRaging");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfArun { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfArun");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfMaraike { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfMaraike");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfMisaye { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfMisaye");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfPakri { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfPakri");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityCreedOfSolasta { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityCreedOfSolasta");

        internal static FeatureDefinitionSavingThrowAffinity
            SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>(
                "SavingThrowAffinityDomainLawUnyieldingEnforcerMotionForm");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityDwarvenPlate { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityDwarvenPlate");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityGemIllusion { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityGemIllusion");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityManaPainterAbsorption { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityManaPainterAbsorption");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityPatronHiveWeakeningPheromones { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityPatronHiveWeakeningPheromones");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityRogueEvasion { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityRogueEvasion");

        internal static FeatureDefinitionSavingThrowAffinity SavingThrowAffinityShelteringBreeze { get; } =
            GetDefinition<FeatureDefinitionSavingThrowAffinity>("SavingThrowAffinityShelteringBreeze");
    }

    internal static class FeatureDefinitionSenses
    {
        internal static FeatureDefinitionSense SenseRogueBlindsense { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseRogueBlindsense");

        internal static FeatureDefinitionSense SenseBlindSight6 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseBlindSight6");

        internal static FeatureDefinitionSense SenseBlindSight16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseBlindSight16");

        internal static FeatureDefinitionSense SenseDarkvision { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvision");

        internal static FeatureDefinitionSense SenseDarkvision12 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvision12");

        internal static FeatureDefinitionSense SenseDarkvision24 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseDarkvision24");

        internal static FeatureDefinitionSense SenseNormalVision { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseNormalVision");

        internal static FeatureDefinitionSense SenseSeeInvisible12 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSeeInvisible12");

        internal static FeatureDefinitionSense SenseSeeInvisible16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSeeInvisible16");

        internal static FeatureDefinitionSense SenseSuperiorDarkvision { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseSuperiorDarkvision");

        internal static FeatureDefinitionSense SenseTremorsense16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseTremorsense16");

        internal static FeatureDefinitionSense SenseTruesight16 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseTruesight16");

        internal static FeatureDefinitionSense SenseTruesight24 { get; } =
            GetDefinition<FeatureDefinitionSense>("SenseTruesight24");
    }

    internal static class FeatureDefinitionDeathSavingThrowAffinitys
    {
        internal static FeatureDefinitionDeathSavingThrowAffinity DeathSavingThrowAffinityBeaconOfHope { get; } =
            GetDefinition<FeatureDefinitionDeathSavingThrowAffinity>("DeathSavingThrowAffinityBeaconOfHope");
    }

    internal static class FeatureDefinitionSubclassChoices
    {
        internal static FeatureDefinitionSubclassChoice SubclassChoiceBarbarianPrimalPath { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceBarbarianPrimalPath");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceBardColleges { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceBardColleges");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceDruidCircle { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceDruidCircle");

        internal static FeatureDefinitionSubclassChoice SubclassChoiceClericDivineDomains { get; } =
            GetDefinition<FeatureDefinitionSubclassChoice>("SubclassChoiceClericDivineDomains");

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
        internal static FeatureDefinitionSummoningAffinity SummoningAffinityKindredSpiritMagicalSpirit { get; } =
            GetDefinition<FeatureDefinitionSummoningAffinity>("SummoningAffinityKindredSpiritMagicalSpirit");
    }

    internal static class FeatureDefinitionTerrainTypeAffinitys
    {
        internal static FeatureDefinitionTerrainTypeAffinity TerrainTypeAffinityRangerNaturalExplorerMountain { get; } =
            GetDefinition<FeatureDefinitionTerrainTypeAffinity>("TerrainTypeAffinityRangerNaturalExplorerMountain");
    }

    internal static class FightingStyleDefinitions
    {
        internal static FightingStyleDefinition GreatWeapon { get; } =
            GetDefinition<FightingStyleDefinition>("GreatWeapon");

        internal static FightingStyleDefinition TwoWeapon { get; } =
            GetDefinition<FightingStyleDefinition>("TwoWeapon");
    }

    internal static class FormationDefinitions
    {
        internal static FormationDefinition Column2 { get; } = GetDefinition<FormationDefinition>("Column2");
    }

    internal static class GadgetBlueprints
    {
        internal static GadgetBlueprint Exit { get; } = GetDefinition<GadgetBlueprint>("Exit");
        internal static GadgetBlueprint ExitMultiple { get; } = GetDefinition<GadgetBlueprint>("ExitMultiple");

        internal static GadgetBlueprint TeleporterIndividual { get; } =
            GetDefinition<GadgetBlueprint>("TeleporterIndividual");

        internal static GadgetBlueprint TeleporterParty { get; } = GetDefinition<GadgetBlueprint>("TeleporterParty");
        internal static GadgetBlueprint VirtualExit { get; } = GetDefinition<GadgetBlueprint>("VirtualExit");

        internal static GadgetBlueprint VirtualExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExitMultiple");
    }

    internal static class GadgetDefinitions
    {
        internal static GadgetDefinition Activator { get; } = GetDefinition<GadgetDefinition>("Activator");
    }

    internal static class InvocationDefinitions
    {
        internal static InvocationDefinition OtherworldlyLeap { get; } =
            GetDefinition<InvocationDefinition>("OtherworldlyLeap");

        internal static InvocationDefinition ArmorOfShadows { get; } =
            GetDefinition<InvocationDefinition>("ArmorOfShadows");

        internal static InvocationDefinition DevilsSight { get; } =
            GetDefinition<InvocationDefinition>("DevilsSight");

        internal static InvocationDefinition EldritchSpear { get; } =
            GetDefinition<InvocationDefinition>("EldritchSpear");

        internal static InvocationDefinition OneWithShadows { get; } =
            GetDefinition<InvocationDefinition>("OneWithShadows");

        internal static InvocationDefinition RepellingBlast { get; } =
            GetDefinition<InvocationDefinition>("RepellingBlast");

        internal static InvocationDefinition ThirstingBlade { get; } =
            GetDefinition<InvocationDefinition>("ThirstingBlade");

        internal static InvocationDefinition SignIllOmen { get; } =
            GetDefinition<InvocationDefinition>("SignIllOmen");

        internal static InvocationDefinition VoiceChainMaster { get; } =
            GetDefinition<InvocationDefinition>("VoiceChainMaster");
    }

    internal static class ItemDefinitions
    {
        internal static ItemDefinition StaffOfFire { get; } =
            GetDefinition<ItemDefinition>("StaffOfFire");

        internal static ItemDefinition RingFeatherFalling { get; } =
            GetDefinition<ItemDefinition>("RingFeatherFalling");

        internal static ItemDefinition _1D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("1D6_Silver_Coins");
        internal static ItemDefinition _1D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("1D6_Copper_Coins");
        internal static ItemDefinition _1D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("1D6_Gold_Coins");
        internal static ItemDefinition _5D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("5D6_Silver_Coins");
        internal static ItemDefinition _5D6_Gold_Coins { get; } = GetDefinition<ItemDefinition>("5D6_Gold_Coins");
        internal static ItemDefinition _20D6_Silver_Coins { get; } = GetDefinition<ItemDefinition>("20D6_Silver_Coins");
        internal static ItemDefinition _6D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("6D6_Copper_Coins");
        internal static ItemDefinition _10D6_Copper_Coins { get; } = GetDefinition<ItemDefinition>("10D6_Copper_Coins");

        internal static ItemDefinition StartingWealth_10GP { get; } =
            GetDefinition<ItemDefinition>("StartingWealth_10GP");

        internal static ItemDefinition _100_GP_Emerald { get; } = GetDefinition<ItemDefinition>("100_GP_Emerald");
        internal static ItemDefinition _100_GP_Pearl { get; } = GetDefinition<ItemDefinition>("100_GP_Pearl");
        internal static ItemDefinition _1000_GP_Diamond { get; } = GetDefinition<ItemDefinition>("1000_GP_Diamond");
        internal static ItemDefinition _20_GP_Amethyst { get; } = GetDefinition<ItemDefinition>("20_GP_Amethyst");
        internal static ItemDefinition _300_GP_Opal { get; } = GetDefinition<ItemDefinition>("300_GP_Opal");
        internal static ItemDefinition _50_GP_Sapphire { get; } = GetDefinition<ItemDefinition>("50_GP_Sapphire");
        internal static ItemDefinition _500_GP_Ruby { get; } = GetDefinition<ItemDefinition>("500_GP_Ruby");

        internal static ItemDefinition ABJURATION_MastersmithLoreDocument { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_MastersmithLoreDocument");

        internal static ItemDefinition ABJURATION_TOWER_ElvenWars { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_ElvenWars");

        internal static ItemDefinition ABJURATION_TOWER_Manifest { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_Manifest");

        internal static ItemDefinition ABJURATION_TOWER_Poem { get; } =
            GetDefinition<ItemDefinition>("ABJURATION_TOWER_Poem");

        internal static ItemDefinition AmuletOfHealth { get; } = GetDefinition<ItemDefinition>("AmuletOfHealth");
        internal static ItemDefinition ArcaneShieldstaff { get; } = GetDefinition<ItemDefinition>("ArcaneShieldstaff");

        internal static ItemDefinition Art_Item_25_GP_EngraveBoneDice { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_EngraveBoneDice");

        internal static ItemDefinition Art_Item_25_GP_SilverChalice { get; } =
            GetDefinition<ItemDefinition>("Art_Item_25_GP_SilverChalice");

        internal static ItemDefinition Art_Item_50_GP_JadePendant { get; } =
            GetDefinition<ItemDefinition>("Art_Item_50_GP_JadePendant");

        internal static ItemDefinition ArtisanToolSmithTools { get; } =
            GetDefinition<ItemDefinition>("ArtisanToolSmithTools");

        internal static ItemDefinition Backpack_Bag_Of_Holding { get; } =
            GetDefinition<ItemDefinition>("Backpack_Bag_Of_Holding");

        internal static ItemDefinition Backpack_Handy_Haversack { get; } =
            GetDefinition<ItemDefinition>("Backpack_Handy_Haversack");

        internal static ItemDefinition BarbarianClothes { get; } = GetDefinition<ItemDefinition>("BarbarianClothes");
        internal static ItemDefinition Bard_Armor { get; } = GetDefinition<ItemDefinition>("Bard_Armor");
        internal static ItemDefinition Battleaxe { get; } = GetDefinition<ItemDefinition>("Battleaxe");
        internal static ItemDefinition BattleaxePlus2 { get; } = GetDefinition<ItemDefinition>("Battleaxe+2");
        internal static ItemDefinition BattleaxePlus3 { get; } = GetDefinition<ItemDefinition>("Battleaxe+3");

        internal static ItemDefinition BeltOfGiantHillStrength { get; } =
            GetDefinition<ItemDefinition>("BeltOfGiantHillStrength");

        internal static ItemDefinition BeltOfRegeneration { get; } =
            GetDefinition<ItemDefinition>("BeltOfRegeneration");

        internal static ItemDefinition Berry_Ration { get; } = GetDefinition<ItemDefinition>("Berry_Ration");
        internal static ItemDefinition Bolt { get; } = GetDefinition<ItemDefinition>("Bolt");

        internal static ItemDefinition Bolt_Alchemy_Corrosive { get; } =
            GetDefinition<ItemDefinition>("Bolt_Alchemy_Corrosive");

        internal static ItemDefinition Bolt_Alchemy_Flaming { get; } =
            GetDefinition<ItemDefinition>("Bolt_Alchemy_Flaming");

        internal static ItemDefinition Bolt_Alchemy_Flash { get; } =
            GetDefinition<ItemDefinition>("Bolt_Alchemy_Flash");

        internal static ItemDefinition BONEKEEP_AkshasJournal { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_AkshasJournal");

        internal static ItemDefinition BONEKEEP_MagicRune { get; } =
            GetDefinition<ItemDefinition>("BONEKEEP_MagicRune");

        internal static ItemDefinition BootsLevitation { get; } = GetDefinition<ItemDefinition>("BootsLevitation");
        internal static ItemDefinition BootsOfElvenKind { get; } = GetDefinition<ItemDefinition>("BootsOfElvenKind");

        internal static ItemDefinition BootsOfStridingAndSpringing { get; } =
            GetDefinition<ItemDefinition>("BootsOfStridingAndSpringing");

        internal static ItemDefinition BootsWinged { get; } = GetDefinition<ItemDefinition>("BootsWinged");

        internal static ItemDefinition Bracers_Of_Archery { get; } =
            GetDefinition<ItemDefinition>("Bracers_Of_Archery");

        internal static ItemDefinition Bracers_Of_Defense { get; } =
            GetDefinition<ItemDefinition>("Bracers_Of_Defense");

        internal static ItemDefinition Breastplate { get; } = GetDefinition<ItemDefinition>("Breastplate");
        internal static ItemDefinition BreastplatePlus1 { get; } = GetDefinition<ItemDefinition>("Breastplate+1");
        internal static ItemDefinition BreastplatePlus2 { get; } = GetDefinition<ItemDefinition>("Breastplate+2");
        internal static ItemDefinition BroochOfShielding { get; } = GetDefinition<ItemDefinition>("BroochOfShielding");

        internal static ItemDefinition CAERLEM_Daliat_Document { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_Daliat_Document");

        internal static ItemDefinition CaerLem_Gate_Plaque { get; } =
            GetDefinition<ItemDefinition>("CaerLem_Gate_Plaque");

        internal static ItemDefinition CAERLEM_Inquisitor_Document { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_Inquisitor_Document");

        internal static ItemDefinition CAERLEM_TirmarianHolySymbol { get; } =
            GetDefinition<ItemDefinition>("CAERLEM_TirmarianHolySymbol");

        internal static ItemDefinition ChainMail { get; } = GetDefinition<ItemDefinition>("ChainMail");
        internal static ItemDefinition ChainmailPlus2 { get; } = GetDefinition<ItemDefinition>("Chainmail+2");
        internal static ItemDefinition ChainShirt { get; } = GetDefinition<ItemDefinition>("ChainShirt");
        internal static ItemDefinition ChainShirtPlus2 { get; } = GetDefinition<ItemDefinition>("ChainShirt+2");

        internal static ItemDefinition CloakOfArachnida { get; } = GetDefinition<ItemDefinition>("CloakOfArachnida");
        internal static ItemDefinition CloakOfElvenkind { get; } = GetDefinition<ItemDefinition>("CloakOfElvenkind");
        internal static ItemDefinition CloakOfProtection { get; } = GetDefinition<ItemDefinition>("CloakOfProtection");
        internal static ItemDefinition ClothesCommon { get; } = GetDefinition<ItemDefinition>("ClothesCommon");

        internal static ItemDefinition ClothesCommon_Tattoo { get; } =
            GetDefinition<ItemDefinition>("ClothesCommon_Tattoo");

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

        internal static ItemDefinition ClothesScavenger_A { get; } =
            GetDefinition<ItemDefinition>("ClothesScavenger_A");

        internal static ItemDefinition ClothesScavenger_B { get; } =
            GetDefinition<ItemDefinition>("ClothesScavenger_B");

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

        internal static ItemDefinition CraftingManual_Enchant_Longsword_Warden { get; } =
            GetDefinition<ItemDefinition>("CraftingManual_Enchant_Longsword_Warden");

        internal static ItemDefinition CraftingManualRemedy { get; } =
            GetDefinition<ItemDefinition>("CraftingManualRemedy");

        internal static ItemDefinition CraftingManualScrollOfVampiricTouch { get; } =
            GetDefinition<ItemDefinition>("CraftingManualScrollOfVampiricTouch");

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

        internal static ItemDefinition Dagger { get; } = GetDefinition<ItemDefinition>("Dagger");
        internal static ItemDefinition DaggerPlus2 { get; } = GetDefinition<ItemDefinition>("Dagger+2");
        internal static ItemDefinition Dart { get; } = GetDefinition<ItemDefinition>("Dart");
        internal static ItemDefinition DivineBladeWeapon { get; } = GetDefinition<ItemDefinition>("DivineBladeWeapon");
        internal static ItemDefinition DruidicFocus { get; } = GetDefinition<ItemDefinition>("DruidicFocus");
        internal static ItemDefinition DungeoneerPack { get; } = GetDefinition<ItemDefinition>("DungeoneerPack");
        internal static ItemDefinition ElvenChain { get; } = GetDefinition<ItemDefinition>("ElvenChain");

        internal static ItemDefinition Enchanted_Battleaxe_Punisher { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Battleaxe_Punisher");

        internal static ItemDefinition Enchanted_BreastplateOfDeflection { get; } =
            GetDefinition<ItemDefinition>("Enchanted_BreastplateOfDeflection");

        internal static ItemDefinition Enchanted_ChainShirt_Empress_war_garb { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ChainShirt_Empress_war_garb");

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

        internal static ItemDefinition Enchanted_Greatsword_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Greatsword_Lightbringer");

        internal static ItemDefinition Enchanted_HalfPlateOfRobustness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HalfPlateOfRobustness");

        internal static ItemDefinition Enchanted_HalfPlateOfSturdiness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_HalfPlateOfSturdiness");

        internal static ItemDefinition Enchanted_LeatherArmorOfFlameDancing { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LeatherArmorOfFlameDancing");

        internal static ItemDefinition Enchanted_LeatherArmorOfSurvival { get; } =
            GetDefinition<ItemDefinition>("Enchanted_LeatherArmorOfSurvival");

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

        internal static ItemDefinition Enchanted_Longsword_Stormblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_Stormblade");

        internal static ItemDefinition Enchanted_Longsword_Warden { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Longsword_Warden");

        internal static ItemDefinition Enchanted_Mace_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Mace_Of_Acuteness");

        internal static ItemDefinition Enchanted_Morningstar_Bearclaw { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Morningstar_Bearclaw");

        internal static ItemDefinition Enchanted_Morningstar_Of_Power { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Morningstar_Of_Power");

        internal static ItemDefinition Enchanted_Rapier_Blackadder { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Blackadder");

        internal static ItemDefinition Enchanted_Rapier_Doomblade { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Doomblade");

        internal static ItemDefinition Enchanted_Rapier_Of_Acuteness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Rapier_Of_Acuteness");

        internal static ItemDefinition Enchanted_ScaleMailOfIceDancing { get; } =
            GetDefinition<ItemDefinition>("Enchanted_ScaleMailOfIceDancing");

        internal static ItemDefinition Enchanted_Shortbow_Medusa { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortbow_Medusa");

        internal static ItemDefinition Enchanted_Shortbow_Of_Sharpshooting { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortbow_Of_Sharpshooting");

        internal static ItemDefinition Enchanted_Shortsword_Lightbringer { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_Lightbringer");

        internal static ItemDefinition Enchanted_Shortsword_of_Sharpness { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_of_Sharpness");

        internal static ItemDefinition Enchanted_Shortsword_Whiteburn { get; } =
            GetDefinition<ItemDefinition>("Enchanted_Shortsword_Whiteburn");

        internal static ItemDefinition ExplorerPack { get; } = GetDefinition<ItemDefinition>("ExplorerPack");
        internal static ItemDefinition FlameBlade { get; } = GetDefinition<ItemDefinition>("FlameBlade");
        internal static ItemDefinition Flute { get; } = GetDefinition<ItemDefinition>("Flute");
        internal static ItemDefinition Food_Ration { get; } = GetDefinition<ItemDefinition>("Food_Ration");

        internal static ItemDefinition Food_Ration_Foraged { get; } =
            GetDefinition<ItemDefinition>("Food_Ration_Foraged");

        internal static ItemDefinition GauntletsOfOgrePower { get; } =
            GetDefinition<ItemDefinition>("GauntletsOfOgrePower");

        internal static ItemDefinition GemOfSeeing { get; } = GetDefinition<ItemDefinition>("GemOfSeeing");

        internal static ItemDefinition GlovesOfMissileSnaring { get; } =
            GetDefinition<ItemDefinition>("GlovesOfMissileSnaring");

        internal static ItemDefinition Greataxe { get; } = GetDefinition<ItemDefinition>("Greataxe");
        internal static ItemDefinition GreataxePlus1 { get; } = GetDefinition<ItemDefinition>("Greataxe+1");
        internal static ItemDefinition GreataxePlus2 { get; } = GetDefinition<ItemDefinition>("Greataxe+2");
        internal static ItemDefinition Greatsword { get; } = GetDefinition<ItemDefinition>("Greatsword");
        internal static ItemDefinition GreatswordPlus2 { get; } = GetDefinition<ItemDefinition>("Greatsword+2");
        internal static ItemDefinition GreenmageArmor { get; } = GetDefinition<ItemDefinition>("GreenmageArmor");
        internal static ItemDefinition HalfPlate { get; } = GetDefinition<ItemDefinition>("HalfPlate");
        internal static ItemDefinition HalfPlatePlus2 { get; } = GetDefinition<ItemDefinition>("HalfPlate+2");
        internal static ItemDefinition Handaxe { get; } = GetDefinition<ItemDefinition>("Handaxe");
        internal static ItemDefinition HandaxePlus2 { get; } = GetDefinition<ItemDefinition>("Handaxe+2");

        internal static ItemDefinition HeadbandOfIntellect { get; } =
            GetDefinition<ItemDefinition>("HeadbandOfIntellect");

        internal static ItemDefinition HeavyCrossbow { get; } = GetDefinition<ItemDefinition>("HeavyCrossbow");
        internal static ItemDefinition HeavyCrossbowPlus2 { get; } = GetDefinition<ItemDefinition>("HeavyCrossbow+2");

        internal static ItemDefinition HelmOfComprehendingLanguages { get; } =
            GetDefinition<ItemDefinition>("HelmOfComprehendingLanguages");

        internal static ItemDefinition HerbalismKit { get; } = GetDefinition<ItemDefinition>("HerbalismKit");
        internal static ItemDefinition HideArmor { get; } = GetDefinition<ItemDefinition>("HideArmor");

        internal static ItemDefinition HideArmor_plus_two { get; } =
            GetDefinition<ItemDefinition>("HideArmor_plus_two");

        internal static ItemDefinition HolySymbolAmulet { get; } = GetDefinition<ItemDefinition>("HolySymbolAmulet");
        internal static ItemDefinition HornOfBlasting { get; } = GetDefinition<ItemDefinition>("HornOfBlasting");

        internal static ItemDefinition Ingredient_AbyssMoss { get; } =
            GetDefinition<ItemDefinition>("Ingredient_AbyssMoss");

        internal static ItemDefinition Ingredient_Acid { get; } = GetDefinition<ItemDefinition>("Ingredient_Acid");

        internal static ItemDefinition Ingredient_AngryViolet { get; } =
            GetDefinition<ItemDefinition>("Ingredient_AngryViolet");

        internal static ItemDefinition Ingredient_BadlandsSpiderVenomGland { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BadlandsSpiderVenomGland");

        internal static ItemDefinition Ingredient_BloodDaffodil { get; } =
            GetDefinition<ItemDefinition>("Ingredient_BloodDaffodil");

        internal static ItemDefinition Ingredient_Enchant_Blood_Gem { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Blood_Gem");

        internal static ItemDefinition Ingredient_Enchant_Blood_Of_Solasta { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Blood_Of_Solasta");

        internal static ItemDefinition Ingredient_Enchant_Cloud_Diamond { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Cloud_Diamond");

        internal static ItemDefinition Ingredient_Enchant_Crystal_Of_Winter { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Crystal_Of_Winter");

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

        internal static ItemDefinition Ingredient_Enchant_PurpleAmber { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_PurpleAmber");

        internal static ItemDefinition Ingredient_Enchant_Shard_Of_Fire { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Shard_Of_Fire");

        internal static ItemDefinition Ingredient_Enchant_Shard_Of_Ice { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Shard_Of_Ice");

        internal static ItemDefinition Ingredient_Enchant_Slavestone { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Slavestone");

        internal static ItemDefinition Ingredient_Enchant_Soul_Gem { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Soul_Gem");

        internal static ItemDefinition Ingredient_Enchant_SpiderQueen_Venom { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_SpiderQueen_Venom");

        internal static ItemDefinition Ingredient_Enchant_Stardust { get; } =
            GetDefinition<ItemDefinition>("Ingredient_Enchant_Stardust");

        internal static ItemDefinition Ingredient_LilyOfTheBadlands { get; } =
            GetDefinition<ItemDefinition>("Ingredient_LilyOfTheBadlands");

        internal static ItemDefinition Ingredient_ManacalonOrchid { get; } =
            GetDefinition<ItemDefinition>("Ingredient_ManacalonOrchid");

        internal static ItemDefinition Ingredient_PrimordialLavaStones { get; } =
            GetDefinition<ItemDefinition>("Ingredient_PrimordialLavaStones");

        internal static ItemDefinition Ingredient_RefinedOil { get; } =
            GetDefinition<ItemDefinition>("Ingredient_RefinedOil");

        internal static ItemDefinition Ingredient_Skarn { get; } = GetDefinition<ItemDefinition>("Ingredient_Skarn");
        internal static ItemDefinition Javelin { get; } = GetDefinition<ItemDefinition>("Javelin");
        internal static ItemDefinition JavelinPlus2 { get; } = GetDefinition<ItemDefinition>("Javelin+2");
        internal static ItemDefinition Leather { get; } = GetDefinition<ItemDefinition>("Leather");
        internal static ItemDefinition LeatherArmorPlus2 { get; } = GetDefinition<ItemDefinition>("LeatherArmor+2");
        internal static ItemDefinition LeatherDruid { get; } = GetDefinition<ItemDefinition>("LeatherDruid");
        internal static ItemDefinition LightCrossbow { get; } = GetDefinition<ItemDefinition>("LightCrossbow");
        internal static ItemDefinition LightCrossbowPlus2 { get; } = GetDefinition<ItemDefinition>("LightCrossbow+2");
        internal static ItemDefinition Longbow { get; } = GetDefinition<ItemDefinition>("Longbow");
        internal static ItemDefinition LongbowPlus1 { get; } = GetDefinition<ItemDefinition>("Longbow+1");
        internal static ItemDefinition LongbowPlus2 { get; } = GetDefinition<ItemDefinition>("Longbow+2");
        internal static ItemDefinition Longsword { get; } = GetDefinition<ItemDefinition>("Longsword");
        internal static ItemDefinition LongswordPlus2 { get; } = GetDefinition<ItemDefinition>("Longsword+2");
        internal static ItemDefinition Mace { get; } = GetDefinition<ItemDefinition>("Mace");
        internal static ItemDefinition MacePlus2 { get; } = GetDefinition<ItemDefinition>("Mace+2");
        internal static ItemDefinition Maul { get; } = GetDefinition<ItemDefinition>("Maul");
        internal static ItemDefinition MaulPlus2 { get; } = GetDefinition<ItemDefinition>("Maul+2");
        internal static ItemDefinition MonkArmor { get; } = GetDefinition<ItemDefinition>("MonkArmor");
        internal static ItemDefinition Morningstar { get; } = GetDefinition<ItemDefinition>("Morningstar");
        internal static ItemDefinition PaddedLeather { get; } = GetDefinition<ItemDefinition>("PaddedLeather");
        internal static ItemDefinition PipesOfHaunting { get; } = GetDefinition<ItemDefinition>("PipesOfHaunting");
        internal static ItemDefinition Plate { get; } = GetDefinition<ItemDefinition>("Plate");
        internal static ItemDefinition PlatePlus2 { get; } = GetDefinition<ItemDefinition>("Plate+2");
        internal static ItemDefinition Poison_Basic { get; } = GetDefinition<ItemDefinition>("Poison_Basic");
        internal static ItemDefinition PotionOfClimbing { get; } = GetDefinition<ItemDefinition>("PotionOfClimbing");
        internal static ItemDefinition PotionOfHealing { get; } = GetDefinition<ItemDefinition>("PotionOfHealing");
        internal static ItemDefinition PotionOfHeroism { get; } = GetDefinition<ItemDefinition>("PotionOfHeroism");

        internal static ItemDefinition PotionOfInvisibility { get; } =
            GetDefinition<ItemDefinition>("PotionOfInvisibility");

        internal static ItemDefinition PotionOfSpeed { get; } = GetDefinition<ItemDefinition>("PotionOfSpeed");
        internal static ItemDefinition PotionRemedy { get; } = GetDefinition<ItemDefinition>("PotionRemedy");
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
        internal static ItemDefinition Primed_Handaxe { get; } = GetDefinition<ItemDefinition>("Primed_Handaxe");

        internal static ItemDefinition Primed_HeavyCrossbow { get; } =
            GetDefinition<ItemDefinition>("Primed_HeavyCrossbow");

        internal static ItemDefinition Primed_HideArmor { get; } = GetDefinition<ItemDefinition>("Primed_HideArmor");
        internal static ItemDefinition Primed_Javelin { get; } = GetDefinition<ItemDefinition>("Primed_Javelin");

        internal static ItemDefinition Primed_Leather_Armor { get; } =
            GetDefinition<ItemDefinition>("Primed Leather Armor");

        internal static ItemDefinition Primed_LeatherDruid { get; } =
            GetDefinition<ItemDefinition>("Primed_LeatherDruid");

        internal static ItemDefinition Primed_LightCrossbow { get; } =
            GetDefinition<ItemDefinition>("Primed_LightCrossbow");

        internal static ItemDefinition Primed_Longbow { get; } = GetDefinition<ItemDefinition>("Primed Longbow");
        internal static ItemDefinition Primed_Longsword { get; } = GetDefinition<ItemDefinition>("Primed_Longsword");
        internal static ItemDefinition Primed_Mace { get; } = GetDefinition<ItemDefinition>("Primed Mace");
        internal static ItemDefinition Primed_Maul { get; } = GetDefinition<ItemDefinition>("Primed_Maul");

        internal static ItemDefinition Primed_Morningstar { get; } =
            GetDefinition<ItemDefinition>("Primed Morningstar");

        internal static ItemDefinition Primed_Plate { get; } = GetDefinition<ItemDefinition>("Primed Plate");
        internal static ItemDefinition Primed_Rapier { get; } = GetDefinition<ItemDefinition>("Primed Rapier");

        internal static ItemDefinition Primed_Quarterstaff { get; } =
            GetDefinition<ItemDefinition>("Primed_Quarterstaff");

        internal static ItemDefinition Primed_ScaleMail { get; } = GetDefinition<ItemDefinition>("Primed ScaleMail");
        internal static ItemDefinition Primed_Scimitar { get; } = GetDefinition<ItemDefinition>("Primed Scimitar");
        internal static ItemDefinition Primed_Shield { get; } = GetDefinition<ItemDefinition>("Primed Shield");
        internal static ItemDefinition Primed_Shortbow { get; } = GetDefinition<ItemDefinition>("Primed Shortbow");
        internal static ItemDefinition Primed_Shortsword { get; } = GetDefinition<ItemDefinition>("Primed Shortsword");
        internal static ItemDefinition Primed_Spear { get; } = GetDefinition<ItemDefinition>("Primed_Spear");

        internal static ItemDefinition Primed_StuddedLeather { get; } =
            GetDefinition<ItemDefinition>("Primed_StuddedLeather");

        internal static ItemDefinition Primed_Warhammer { get; } = GetDefinition<ItemDefinition>("Primed_Warhammer");
        internal static ItemDefinition ProducedFlame { get; } = GetDefinition<ItemDefinition>("ProducedFlame");
        internal static ItemDefinition Quarterstaff { get; } = GetDefinition<ItemDefinition>("Quarterstaff");
        internal static ItemDefinition QuarterstaffPlus1 { get; } = GetDefinition<ItemDefinition>("Quarterstaff+1");
        internal static ItemDefinition QuarterstaffPlus2 { get; } = GetDefinition<ItemDefinition>("Quarterstaff+2");
        internal static ItemDefinition Rapier { get; } = GetDefinition<ItemDefinition>("Rapier");
        internal static ItemDefinition RapierPlus2 { get; } = GetDefinition<ItemDefinition>("Rapier+2");
        internal static ItemDefinition RingDarkvision { get; } = GetDefinition<ItemDefinition>("RingDarkvision");

        internal static ItemDefinition RingDetectInvisible { get; } =
            GetDefinition<ItemDefinition>("RingDetectInvisible");

        internal static ItemDefinition RingProtectionPlus1 { get; } = GetDefinition<ItemDefinition>("RingProtection+1");
        internal static ItemDefinition ScaleMail { get; } = GetDefinition<ItemDefinition>("ScaleMail");
        internal static ItemDefinition ScaleMailPlus2 { get; } = GetDefinition<ItemDefinition>("ScaleMail+2");
        internal static ItemDefinition Scimitar { get; } = GetDefinition<ItemDefinition>("Scimitar");
        internal static ItemDefinition ScimitarPlus2 { get; } = GetDefinition<ItemDefinition>("Scimitar+2");
        internal static ItemDefinition ScrollFly { get; } = GetDefinition<ItemDefinition>("ScrollFly");
        internal static ItemDefinition Shield { get; } = GetDefinition<ItemDefinition>("Shield");
        internal static ItemDefinition Shield_Wooden { get; } = GetDefinition<ItemDefinition>("Shield_Wooden");
        internal static ItemDefinition ShieldPlus1 { get; } = GetDefinition<ItemDefinition>("Shield+1");
        internal static ItemDefinition ShieldPlus2 { get; } = GetDefinition<ItemDefinition>("Shield+2");
        internal static ItemDefinition Shortbow { get; } = GetDefinition<ItemDefinition>("Shortbow");
        internal static ItemDefinition ShortbowPlus2 { get; } = GetDefinition<ItemDefinition>("Shortbow+2");

        internal static ItemDefinition Shortsword { get; } = GetDefinition<ItemDefinition>("Shortsword");

        internal static ItemDefinition SlippersOfSpiderClimbing { get; } =
            GetDefinition<ItemDefinition>("SlippersOfSpiderClimbing");

        internal static ItemDefinition SorcererArmor { get; } = GetDefinition<ItemDefinition>("SorcererArmor");
        internal static ItemDefinition Spear { get; } = GetDefinition<ItemDefinition>("Spear");
        internal static ItemDefinition SpearPlus2 { get; } = GetDefinition<ItemDefinition>("Spear+2");
        internal static ItemDefinition Spellbook { get; } = GetDefinition<ItemDefinition>("Spellbook");
        internal static ItemDefinition StaffOfHealing { get; } = GetDefinition<ItemDefinition>("StaffOfHealing");
        internal static ItemDefinition StuddedLeather { get; } = GetDefinition<ItemDefinition>("StuddedLeather");

        internal static ItemDefinition StuddedLeather_plus_one { get; } =
            GetDefinition<ItemDefinition>("StuddedLeather_plus_one");

        internal static ItemDefinition StuddedLeather_plus_two { get; } =
            GetDefinition<ItemDefinition>("StuddedLeather_plus_two");

        internal static ItemDefinition Torch { get; } = GetDefinition<ItemDefinition>("Torch");
        internal static ItemDefinition UnarmedStrikeBase { get; } = GetDefinition<ItemDefinition>("UnarmedStrikeBase");
        internal static ItemDefinition WandMagicMissile { get; } = GetDefinition<ItemDefinition>("WandMagicMissile");
        internal static ItemDefinition WandOfIdentify { get; } = GetDefinition<ItemDefinition>("WandOfIdentify");

        internal static ItemDefinition WandOfMagicDetection { get; } =
            GetDefinition<ItemDefinition>("WandOfMagicDetection");

        internal static ItemDefinition WandOfWarMagePlus1 { get; } = GetDefinition<ItemDefinition>("WandOfWarMage+1");
        internal static ItemDefinition Warhammer { get; } = GetDefinition<ItemDefinition>("Warhammer");
        internal static ItemDefinition WarhammerPlus2 { get; } = GetDefinition<ItemDefinition>("Warhammer+2");
        internal static ItemDefinition Warlock_Armor { get; } = GetDefinition<ItemDefinition>("Warlock_Armor");

        internal static ItemDefinition WizardClothes_Alternate { get; } =
            GetDefinition<ItemDefinition>("WizardClothes_Alternate");
    }

    internal static class ItemFlagDefinitions
    {
        internal static ItemFlagDefinition ItemFlagPrimed { get; } =
            GetDefinition<ItemFlagDefinition>("ItemFlagPrimed");
    }

    internal static class MerchantCategoryDefinitions
    {
        internal static MerchantCategoryDefinition All { get; } = GetDefinition<MerchantCategoryDefinition>("All");

        internal static MerchantCategoryDefinition Crafting { get; } =
            GetDefinition<MerchantCategoryDefinition>("Crafting");

        internal static MerchantCategoryDefinition MagicDevice { get; } =
            GetDefinition<MerchantCategoryDefinition>("MagicDevice");

        internal static MerchantCategoryDefinition Weapon { get; } =
            GetDefinition<MerchantCategoryDefinition>("Weapon");
    }

    internal static class MerchantDefinitions
    {
        internal static MerchantDefinition Store_Merchant_Antiquarians_Halman_Summer { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Antiquarians_Halman_Summer");

        internal static MerchantDefinition Store_Merchant_Arcaneum_Heddlon_Surespell { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Arcaneum_Heddlon_Surespell");

        internal static MerchantDefinition Store_Merchant_CircleOfDanantar_Joriel_Foxeye { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_CircleOfDanantar_Joriel_Foxeye");

        internal static MerchantDefinition Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore");

        internal static MerchantDefinition Store_Merchant_Hugo_Requer_Cyflen_Potions { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_Hugo_Requer_Cyflen_Potions");

        internal static MerchantDefinition Store_Merchant_TowerOfKnowledge_Maddy_Greenisle { get; } =
            GetDefinition<MerchantDefinition>("Store_Merchant_TowerOfKnowledge_Maddy_Greenisle");
    }

    internal static class MetamagicOptionDefinitions
    {
        internal static MetamagicOptionDefinition MetamagicQuickenedSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicQuickenedSpell");

        internal static MetamagicOptionDefinition MetamagicSubtleSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicSubtleSpell");

        internal static MetamagicOptionDefinition MetamagicTwinnedSpell { get; } =
            GetDefinition<MetamagicOptionDefinition>("MetamagicTwinnedSpell");
    }

    internal static class MonsterAttackDefinitions
    {
        internal static MonsterAttackDefinition Attack_Minotaur_Elite_Charged_Gore { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Minotaur_Elite_Charged_Gore");

        internal static MonsterAttackDefinition Attack_Minotaur_Elite_Gore { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Minotaur_Elite_Gore");

        internal static MonsterAttackDefinition Attack_Minotaur_Elite_Greataxe { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Minotaur_Elite_Greataxe");

        internal static MonsterAttackDefinition Attack_Spiderling_Crimson_Bite { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Spiderling_Crimson_Bite");

        internal static MonsterAttackDefinition Attack_Wildshape_Ape_Toss_Rock { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Wildshape_Ape_Toss_Rock");

        internal static MonsterAttackDefinition Attack_Wildshape_BrownBear_Bite { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Wildshape_BrownBear_Bite");

        internal static MonsterAttackDefinition Attack_Wildshape_BrownBear_Claw { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Wildshape_BrownBear_Claw");

        internal static MonsterAttackDefinition Attack_Wildshape_GiantEagle_Talons { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Wildshape_GiantEagle_Talons");

        internal static MonsterAttackDefinition Attack_Wildshape_Wolf_Bite { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Wildshape_Wolf_Bite");
    }

    internal static class MonsterDefinitions
    {
        internal static MonsterDefinition ShamblingMound_MonsterDefinition { get; } =
            GetDefinition<MonsterDefinition>("ShamblingMound_MonsterDefinition");

        internal static MonsterDefinition ShamblingMound_MonsterDefinition_POI_ONLY { get; } =
            GetDefinition<MonsterDefinition>("ShamblingMound_MonsterDefinition_POI_ONLY");

        internal static MonsterDefinition Wraith { get; } =
            GetDefinition<MonsterDefinition>("Wraith");

        internal static MonsterDefinition SpectralDragon_01 { get; } =
            GetDefinition<MonsterDefinition>("SpectralDragon_01");

        internal static MonsterDefinition SpectralDragon_02 { get; } =
            GetDefinition<MonsterDefinition>("SpectralDragon_02");

        internal static MonsterDefinition SpectralDragon_03 { get; } =
            GetDefinition<MonsterDefinition>("SpectralDragon_03");

        internal static MonsterDefinition SpectralDragon_Magister { get; } =
            GetDefinition<MonsterDefinition>("SpectralDragon_Magister");

        internal static MonsterDefinition MinotaurSpectral { get; } =
            GetDefinition<MonsterDefinition>("MinotaurSpectral");

        internal static MonsterDefinition Redeemer_Zealot { get; } =
            GetDefinition<MonsterDefinition>("Redeemer_Zealot");

        internal static MonsterDefinition Ghost_Emtan { get; } = GetDefinition<MonsterDefinition>("Ghost_Emtan");
        internal static MonsterDefinition Ghost_Wolf { get; } = GetDefinition<MonsterDefinition>("Ghost_Wolf");

        internal static MonsterDefinition Ghost_Dwarf_Guardian { get; } =
            GetDefinition<MonsterDefinition>("Ghost_Dwarf_Guardian");

        internal static MonsterDefinition Bone_Keep_Adventurer_Ghost { get; } =
            GetDefinition<MonsterDefinition>("Bone_Keep_Adventurer_Ghost");

        internal static MonsterDefinition DLC3_ElvenClans_Leralyn { get; } =
            GetDefinition<MonsterDefinition>("DLC3_ElvenClans_Leralyn");

        internal static MonsterDefinition Air_Elemental { get; } = GetDefinition<MonsterDefinition>("Air_Elemental");

        internal static MonsterDefinition AlphaWolf { get; } = GetDefinition<MonsterDefinition>("AlphaWolf");

        internal static MonsterDefinition Ape_MonsterDefinition { get; } =
            GetDefinition<MonsterDefinition>("Ape_MonsterDefinition");

        internal static MonsterDefinition BlackBear { get; } =
            GetDefinition<MonsterDefinition>("BlackBear");

        internal static MonsterDefinition BlackDragon_MasterOfNecromancy { get; } =
            GetDefinition<MonsterDefinition>("BlackDragon_MasterOfNecromancy");

        internal static MonsterDefinition BrownBear { get; } = GetDefinition<MonsterDefinition>("BrownBear");

        internal static MonsterDefinition ConjuredEightBeast_Wolf { get; } =
            GetDefinition<MonsterDefinition>("ConjuredEightBeast_Wolf");

        internal static MonsterDefinition ConjuredFourBeast_BadlandsSpider { get; } =
            GetDefinition<MonsterDefinition>("ConjuredFourBeast_BadlandsSpider");

        internal static MonsterDefinition ConjuredOneBeastTiger_Drake { get; } =
            GetDefinition<MonsterDefinition>("ConjuredOneBeastTiger_Drake");

        internal static MonsterDefinition ConjuredTwoBeast_Direwolf { get; } =
            GetDefinition<MonsterDefinition>("ConjuredTwoBeast_Direwolf");

        internal static MonsterDefinition CrimsonSpider { get; } = GetDefinition<MonsterDefinition>("CrimsonSpider");
        internal static MonsterDefinition CubeOfLight { get; } = GetDefinition<MonsterDefinition>("CubeOfLight");
        internal static MonsterDefinition Divine_Avatar { get; } = GetDefinition<MonsterDefinition>("Divine_Avatar");

        internal static MonsterDefinition Eagle_Matriarch { get; } =
            GetDefinition<MonsterDefinition>("Eagle_Matriarch");

        internal static MonsterDefinition Earth_Elemental { get; } =
            GetDefinition<MonsterDefinition>("Earth_Elemental");

        internal static MonsterDefinition Emperor_Laethar { get; } =
            GetDefinition<MonsterDefinition>("Emperor_Laethar");

        internal static MonsterDefinition FeyBear { get; } = GetDefinition<MonsterDefinition>("FeyBear");
        internal static MonsterDefinition FeyDriad { get; } = GetDefinition<MonsterDefinition>("FeyDriad");
        internal static MonsterDefinition FeyGiant_Eagle { get; } = GetDefinition<MonsterDefinition>("FeyGiant_Eagle");
        internal static MonsterDefinition FeyGiantApe { get; } = GetDefinition<MonsterDefinition>("FeyGiantApe");
        internal static MonsterDefinition FeyWolf { get; } = GetDefinition<MonsterDefinition>("FeyWolf");
        internal static MonsterDefinition Fire_Elemental { get; } = GetDefinition<MonsterDefinition>("Fire_Elemental");
        internal static MonsterDefinition Fire_Jester { get; } = GetDefinition<MonsterDefinition>("Fire_Jester");
        internal static MonsterDefinition Fire_Osprey { get; } = GetDefinition<MonsterDefinition>("Fire_Osprey");
        internal static MonsterDefinition Fire_Spider { get; } = GetDefinition<MonsterDefinition>("Fire_Spider");
        internal static MonsterDefinition Ghost { get; } = GetDefinition<MonsterDefinition>("Ghost");
        internal static MonsterDefinition Ghoul { get; } = GetDefinition<MonsterDefinition>("Ghoul");
        internal static MonsterDefinition Giant_Ape { get; } = GetDefinition<MonsterDefinition>("Giant_Ape");

        internal static MonsterDefinition Giant_Eagle { get; } = GetDefinition<MonsterDefinition>("Giant_Eagle");

        internal static MonsterDefinition GoldDragon_AerElai { get; } =
            GetDefinition<MonsterDefinition>("GoldDragon_AerElai");

        internal static MonsterDefinition Golem_Iron { get; } = GetDefinition<MonsterDefinition>("Golem_Iron");
        internal static MonsterDefinition Green_Hag { get; } = GetDefinition<MonsterDefinition>("Green_Hag");

        internal static MonsterDefinition GreenDragon_MasterOfConjuration { get; } =
            GetDefinition<MonsterDefinition>("GreenDragon_MasterOfConjuration");

        internal static MonsterDefinition Hezrou_MonsterDefinition { get; } =
            GetDefinition<MonsterDefinition>("Hezrou_MonsterDefinition");

        internal static MonsterDefinition Ice_Elemental { get; } = GetDefinition<MonsterDefinition>("Ice_Elemental");

        internal static MonsterDefinition InvisibleStalker { get; } =
            GetDefinition<MonsterDefinition>("InvisibleStalker");

        internal static MonsterDefinition KindredSpiritBear { get; } =
            GetDefinition<MonsterDefinition>("KindredSpiritBear");

        internal static MonsterDefinition KindredSpiritEagle { get; } =
            GetDefinition<MonsterDefinition>("KindredSpiritEagle");

        internal static MonsterDefinition KindredSpiritWolf { get; } =
            GetDefinition<MonsterDefinition>("KindredSpiritWolf");

        internal static MonsterDefinition Marilith_MonsterDefinition { get; } =
            GetDefinition<MonsterDefinition>("Marilith_MonsterDefinition");

        internal static MonsterDefinition MinotaurElite { get; } = GetDefinition<MonsterDefinition>("MinotaurElite");
        internal static MonsterDefinition PhaseSpider { get; } = GetDefinition<MonsterDefinition>("PhaseSpider");
        internal static MonsterDefinition Remorhaz { get; } = GetDefinition<MonsterDefinition>("Remorhaz");
        internal static MonsterDefinition SkarnGhoul { get; } = GetDefinition<MonsterDefinition>("SkarnGhoul");
        internal static MonsterDefinition Skeleton { get; } = GetDefinition<MonsterDefinition>("Skeleton");

        internal static MonsterDefinition Skeleton_Archer { get; } =
            GetDefinition<MonsterDefinition>("Skeleton_Archer");

        internal static MonsterDefinition Skeleton_Enforcer { get; } =
            GetDefinition<MonsterDefinition>("Skeleton_Enforcer");

        internal static MonsterDefinition Skeleton_Knight { get; } =
            GetDefinition<MonsterDefinition>("Skeleton_Knight");

        internal static MonsterDefinition Skeleton_Marksman { get; } =
            GetDefinition<MonsterDefinition>("Skeleton_Marksman");

        internal static MonsterDefinition Sorr_Akkath_Shikkath { get; } =
            GetDefinition<MonsterDefinition>("Sorr-Akkath_Shikkath");

        internal static MonsterDefinition Sorr_Akkath_Tshar_Boss { get; } =
            GetDefinition<MonsterDefinition>("Sorr-Akkath_Tshar_Boss");

        internal static MonsterDefinition SpectralSpider { get; } = GetDefinition<MonsterDefinition>("SpectralSpider");
        internal static MonsterDefinition Spider_Queen { get; } = GetDefinition<MonsterDefinition>("Spider_Queen");

        internal static MonsterDefinition Wight { get; } = GetDefinition<MonsterDefinition>("Wight");
        internal static MonsterDefinition WightLord { get; } = GetDefinition<MonsterDefinition>("WightLord");
        internal static MonsterDefinition WildShapeApe { get; } = GetDefinition<MonsterDefinition>("WildShapeApe");

        internal static MonsterDefinition WildShapeBadlandsSpider { get; } =
            GetDefinition<MonsterDefinition>("WildShapeBadlandsSpider");

        internal static MonsterDefinition WildshapeBlackBear { get; } =
            GetDefinition<MonsterDefinition>("WildshapeBlackBear");

        internal static MonsterDefinition WildShapeBrownBear { get; } =
            GetDefinition<MonsterDefinition>("WildShapeBrownBear");

        internal static MonsterDefinition WildshapeDeepSpider { get; } =
            GetDefinition<MonsterDefinition>("WildshapeDeepSpider");

        internal static MonsterDefinition WildshapeDirewolf { get; } =
            GetDefinition<MonsterDefinition>("WildshapeDirewolf");

        internal static MonsterDefinition WildShapeGiant_Eagle { get; } =
            GetDefinition<MonsterDefinition>("WildShapeGiant_Eagle");

        internal static MonsterDefinition WildshapeTiger_Drake { get; } =
            GetDefinition<MonsterDefinition>("WildshapeTiger_Drake");

        internal static MonsterDefinition WildShapeTundraTiger { get; } =
            GetDefinition<MonsterDefinition>("WildShapeTundraTiger");

        internal static MonsterDefinition WindSnake { get; } = GetDefinition<MonsterDefinition>("WindSnake");
        internal static MonsterDefinition Wolf { get; } = GetDefinition<MonsterDefinition>("Wolf");
    }

    internal static class MorphotypeElementDefinitions
    {
        internal static MorphotypeElementDefinition BeardShape_None { get; } =
            GetDefinition<MorphotypeElementDefinition>("BeardShape_None");

        internal static MorphotypeElementDefinition BodyDecorationColor_Default_00 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_Default_00");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_00 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_00");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_01 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_01");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_02 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_02");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_03 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_03");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_04 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_04");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_05 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_05");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_06 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_06");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_07 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_07");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_08 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_08");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_09 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_09");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_10 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_10");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_11 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_11");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_12 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_12");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_13 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_13");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_14 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_14");

        internal static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_15 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_15");

        internal static MorphotypeElementDefinition EyeColor_001 { get; } =
            GetDefinition<MorphotypeElementDefinition>("EyeColor_001");

        internal static MorphotypeElementDefinition EyeColorDefiler { get; } =
            GetDefinition<MorphotypeElementDefinition>("EyeColorDefiler");

        internal static MorphotypeElementDefinition EyeColorInfiltrator { get; } =
            GetDefinition<MorphotypeElementDefinition>("EyeColorInfiltrator");

        internal static MorphotypeElementDefinition EyeColorNecromancer { get; } =
            GetDefinition<MorphotypeElementDefinition>("EyeColorNecromancer");

        internal static MorphotypeElementDefinition FaceAndSkin_01 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_01");

        internal static MorphotypeElementDefinition FaceAndSkin_12 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_12");

        internal static MorphotypeElementDefinition FaceAndSkin_13 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_13");

        internal static MorphotypeElementDefinition FaceAndSkin_14 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_14");

        internal static MorphotypeElementDefinition FaceAndSkin_15 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_15");

        internal static MorphotypeElementDefinition FaceAndSkin_16 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_16");

        internal static MorphotypeElementDefinition FaceAndSkin_17 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_17");

        internal static MorphotypeElementDefinition FaceAndSkin_18 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_18");

        internal static MorphotypeElementDefinition FaceAndSkin_Neutral { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_Neutral");

        internal static MorphotypeElementDefinition FaceShape_NPC_Aksha { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceShape_NPC_Aksha");

        internal static MorphotypeElementDefinition HairColorSilver { get; } =
            GetDefinition<MorphotypeElementDefinition>("HairColorSilver");
    }

    internal static class RecipeDefinitions
    {
        internal static RecipeDefinition Recipe_Enchantment_BattleaxePunisher { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BattleaxePunisher");

        internal static RecipeDefinition Recipe_Enchantment_BreastplateOfDeflection { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_BreastplateOfDeflection");

        internal static RecipeDefinition Recipe_Enchantment_DaggerFrostburn { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerFrostburn");

        internal static RecipeDefinition Recipe_Enchantment_DaggerOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_DaggerOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerOfSharpness");

        internal static RecipeDefinition Recipe_Enchantment_DaggerSouldrinker { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_DaggerSouldrinker");

        internal static RecipeDefinition Recipe_Enchantment_GreatswordLightbringer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_GreatswordLightbringer");

        internal static RecipeDefinition Recipe_Enchantment_HalfplateOfRobustness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HalfplateOfRobustness");

        internal static RecipeDefinition Recipe_Enchantment_HalfplateOfSturdiness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_HalfplateOfSturdiness");

        internal static RecipeDefinition Recipe_Enchantment_LeatherArmorOfFlameDancing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LeatherArmorOfFlameDancing");

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

        internal static RecipeDefinition Recipe_Enchantment_LongswordStormblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordStormblade");

        internal static RecipeDefinition Recipe_Enchantment_LongswordWarden { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_LongswordWarden");

        internal static RecipeDefinition Recipe_Enchantment_MaceOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MaceOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_MorningstarBearclaw { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MorningstarBearclaw");

        internal static RecipeDefinition Recipe_Enchantment_MorningstarOfPower { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_MorningstarOfPower");

        internal static RecipeDefinition Recipe_Enchantment_RapierBlackAdder { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RapierBlackAdder");

        internal static RecipeDefinition Recipe_Enchantment_RapierDoomblade { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RapierDoomblade");

        internal static RecipeDefinition Recipe_Enchantment_RapierOfAcuteness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_RapierOfAcuteness");

        internal static RecipeDefinition Recipe_Enchantment_ScaleMailOfIceDancing { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ScaleMailOfIceDancing");

        internal static RecipeDefinition Recipe_Enchantment_ShortbowMedusa { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortbowMedusa");

        internal static RecipeDefinition Recipe_Enchantment_ShortbowOfSharpshooting { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortbowOfSharpshooting");

        internal static RecipeDefinition Recipe_Enchantment_ShortswordLightbringer { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortswordLightbringer");

        internal static RecipeDefinition Recipe_Enchantment_ShortswordWhiteburn { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortswordWhiteburn");

        internal static RecipeDefinition Recipe_Enchantment_ShortwordOfSharpness { get; } =
            GetDefinition<RecipeDefinition>("Recipe_Enchantment_ShortwordOfSharpness");

        internal static RecipeDefinition RecipeBasic_Arrows { get; } =
            GetDefinition<RecipeDefinition>("RecipeBasic_Arrows");

        internal static RecipeDefinition RecipeBasic_Bolts { get; } =
            GetDefinition<RecipeDefinition>("RecipeBasic_Bolts");
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

    internal static class SlotTypeDefinitions
    {
        internal static SlotTypeDefinition ContainerSlot { get; } = GetDefinition<SlotTypeDefinition>("ContainerSlot");
        internal static SlotTypeDefinition GlovesSlot { get; } = GetDefinition<SlotTypeDefinition>("GlovesSlot");
        internal static SlotTypeDefinition HeadSlot { get; } = GetDefinition<SlotTypeDefinition>("HeadSlot");
        internal static SlotTypeDefinition MainHandSlot { get; } = GetDefinition<SlotTypeDefinition>("MainHandSlot");
        internal static SlotTypeDefinition OffHandSlot { get; } = GetDefinition<SlotTypeDefinition>("OffHandSlot");
        internal static SlotTypeDefinition TorsoSlot { get; } = GetDefinition<SlotTypeDefinition>("TorsoSlot");
    }

    internal static class SpellDefinitions
    {
        internal static SpellDefinition GlobeOfInvulnerability { get; } =
            GetDefinition<SpellDefinition>("GlobeOfInvulnerability");

        internal static SpellDefinition DelayedBlastFireball { get; } =
            GetDefinition<SpellDefinition>("DelayedBlastFireball");

        internal static SpellDefinition Knock { get; } = GetDefinition<SpellDefinition>("Knock");
        internal static SpellDefinition Earthquake { get; } = GetDefinition<SpellDefinition>("Earthquake");
        internal static SpellDefinition AcidArrow { get; } = GetDefinition<SpellDefinition>("AcidArrow");
        internal static SpellDefinition AcidSplash { get; } = GetDefinition<SpellDefinition>("AcidSplash");
        internal static SpellDefinition Aid { get; } = GetDefinition<SpellDefinition>("Aid");
        internal static SpellDefinition AnimalFriendship { get; } = GetDefinition<SpellDefinition>("AnimalFriendship");
        internal static SpellDefinition AnimalShapes { get; } = GetDefinition<SpellDefinition>("AnimalShapes");
        internal static SpellDefinition ArcaneSword { get; } = GetDefinition<SpellDefinition>("ArcaneSword");
        internal static SpellDefinition Bane { get; } = GetDefinition<SpellDefinition>("Bane");
        internal static SpellDefinition Banishment { get; } = GetDefinition<SpellDefinition>("Banishment");
        internal static SpellDefinition Barkskin { get; } = GetDefinition<SpellDefinition>("Barkskin");
        internal static SpellDefinition BeaconOfHope { get; } = GetDefinition<SpellDefinition>("BeaconOfHope");
        internal static SpellDefinition BestowCurse { get; } = GetDefinition<SpellDefinition>("BestowCurse");

        internal static SpellDefinition BestowCurseOnAttackRoll { get; } =
            GetDefinition<SpellDefinition>("BestowCurseOnAttackRoll");

        internal static SpellDefinition BlackTentacles { get; } = GetDefinition<SpellDefinition>("BlackTentacles");
        internal static SpellDefinition BladeBarrier { get; } = GetDefinition<SpellDefinition>("BladeBarrier");

        internal static SpellDefinition BladeBarrierWallLine { get; } =
            GetDefinition<SpellDefinition>("BladeBarrierWallLine");

        internal static SpellDefinition Bless { get; } = GetDefinition<SpellDefinition>("Bless");
        internal static SpellDefinition Blight { get; } = GetDefinition<SpellDefinition>("Blight");
        internal static SpellDefinition Blindness { get; } = GetDefinition<SpellDefinition>("Blindness");
        internal static SpellDefinition Blur { get; } = GetDefinition<SpellDefinition>("Blur");
        internal static SpellDefinition BrandingSmite { get; } = GetDefinition<SpellDefinition>("BrandingSmite");
        internal static SpellDefinition BurningHands { get; } = GetDefinition<SpellDefinition>("BurningHands");
        internal static SpellDefinition BurningHands_B { get; } = GetDefinition<SpellDefinition>("BurningHands_B");
        internal static SpellDefinition CallLightning { get; } = GetDefinition<SpellDefinition>("CallLightning");
        internal static SpellDefinition CalmEmotions { get; } = GetDefinition<SpellDefinition>("CalmEmotions");
        internal static SpellDefinition ChainLightning { get; } = GetDefinition<SpellDefinition>("ChainLightning");
        internal static SpellDefinition CharmPerson { get; } = GetDefinition<SpellDefinition>("CharmPerson");
        internal static SpellDefinition ChillTouch { get; } = GetDefinition<SpellDefinition>("ChillTouch");
        internal static SpellDefinition CircleOfDeath { get; } = GetDefinition<SpellDefinition>("CircleOfDeath");
        internal static SpellDefinition CloudKill { get; } = GetDefinition<SpellDefinition>("CloudKill");
        internal static SpellDefinition ColorSpray { get; } = GetDefinition<SpellDefinition>("ColorSpray");
        internal static SpellDefinition Command { get; } = GetDefinition<SpellDefinition>("Command");
        internal static SpellDefinition ConeOfCold { get; } = GetDefinition<SpellDefinition>("ConeOfCold");
        internal static SpellDefinition Confusion { get; } = GetDefinition<SpellDefinition>("Confusion");
        internal static SpellDefinition ConjureAnimals { get; } = GetDefinition<SpellDefinition>("ConjureAnimals");

        internal static SpellDefinition ConjureAnimalsOneBeast { get; } =
            GetDefinition<SpellDefinition>("ConjureAnimalsOneBeast");

        internal static SpellDefinition ConjureElemental { get; } = GetDefinition<SpellDefinition>("ConjureElemental");

        internal static SpellDefinition ConjureElementalAir { get; } =
            GetDefinition<SpellDefinition>("ConjureElementalAir");

        internal static SpellDefinition ConjureElementalFire { get; } =
            GetDefinition<SpellDefinition>("ConjureElementalFire");

        internal static SpellDefinition ConjureFey { get; } = GetDefinition<SpellDefinition>("ConjureFey");

        internal static SpellDefinition ConjureGoblinoids { get; } =
            GetDefinition<SpellDefinition>("ConjureGoblinoids");

        internal static SpellDefinition ConjureMinorElementals { get; } =
            GetDefinition<SpellDefinition>("ConjureMinorElementals");

        internal static SpellDefinition ConjureCelestialCouatl { get; } =
            GetDefinition<SpellDefinition>("ConjureCelestialCouatl");

        internal static SpellDefinition Contagion { get; } = GetDefinition<SpellDefinition>("Contagion");
        internal static SpellDefinition Counterspell { get; } = GetDefinition<SpellDefinition>("Counterspell");
        internal static SpellDefinition CreateFood { get; } = GetDefinition<SpellDefinition>("CreateFood");
        internal static SpellDefinition CureWounds { get; } = GetDefinition<SpellDefinition>("CureWounds");
        internal static SpellDefinition DancingLights { get; } = GetDefinition<SpellDefinition>("DancingLights");
        internal static SpellDefinition Darkness { get; } = GetDefinition<SpellDefinition>("Darkness");
        internal static SpellDefinition Darkvision { get; } = GetDefinition<SpellDefinition>("Darkvision");
        internal static SpellDefinition Daylight { get; } = GetDefinition<SpellDefinition>("Daylight");
        internal static SpellDefinition Dazzle { get; } = GetDefinition<SpellDefinition>("Dazzle");
        internal static SpellDefinition DeathWard { get; } = GetDefinition<SpellDefinition>("DeathWard");

        internal static SpellDefinition DetectEvilAndGood { get; } =
            GetDefinition<SpellDefinition>("DetectEvilAndGood");

        internal static SpellDefinition DetectMagic { get; } = GetDefinition<SpellDefinition>("DetectMagic");
        internal static SpellDefinition DimensionDoor { get; } = GetDefinition<SpellDefinition>("DimensionDoor");
        internal static SpellDefinition Disintegrate { get; } = GetDefinition<SpellDefinition>("Disintegrate");

        internal static SpellDefinition DispelEvilAndGood { get; } =
            GetDefinition<SpellDefinition>("DispelEvilAndGood");

        internal static SpellDefinition DispelMagic { get; } = GetDefinition<SpellDefinition>("DispelMagic");
        internal static SpellDefinition DivineBlade { get; } = GetDefinition<SpellDefinition>("DivineBlade");
        internal static SpellDefinition DivineFavor { get; } = GetDefinition<SpellDefinition>("DivineFavor");
        internal static SpellDefinition DivineWord { get; } = GetDefinition<SpellDefinition>("DivineWord");
        internal static SpellDefinition DominateBeast { get; } = GetDefinition<SpellDefinition>("DominateBeast");
        internal static SpellDefinition DominatePerson { get; } = GetDefinition<SpellDefinition>("DominatePerson");
        internal static SpellDefinition DreadfulOmen { get; } = GetDefinition<SpellDefinition>("DreadfulOmen");
        internal static SpellDefinition EldritchBlast { get; } = GetDefinition<SpellDefinition>("EldritchBlast");
        internal static SpellDefinition EnhanceAbility { get; } = GetDefinition<SpellDefinition>("EnhanceAbility");
        internal static SpellDefinition Entangle { get; } = GetDefinition<SpellDefinition>("Entangle");

        internal static SpellDefinition ExpeditiousRetreat { get; } =
            GetDefinition<SpellDefinition>("ExpeditiousRetreat");

        internal static SpellDefinition FaerieFire { get; } = GetDefinition<SpellDefinition>("FaerieFire");
        internal static SpellDefinition FalseLife { get; } = GetDefinition<SpellDefinition>("FalseLife");
        internal static SpellDefinition Fear { get; } = GetDefinition<SpellDefinition>("Fear");
        internal static SpellDefinition FeatherFall { get; } = GetDefinition<SpellDefinition>("FeatherFall");
        internal static SpellDefinition Feeblemind { get; } = GetDefinition<SpellDefinition>("Feeblemind");
        internal static SpellDefinition FingerOfDeath { get; } = GetDefinition<SpellDefinition>("FingerOfDeath");
        internal static SpellDefinition FindTraps { get; } = GetDefinition<SpellDefinition>("FindTraps");
        internal static SpellDefinition Fireball { get; } = GetDefinition<SpellDefinition>("Fireball");
        internal static SpellDefinition FireBolt { get; } = GetDefinition<SpellDefinition>("FireBolt");
        internal static SpellDefinition FireShield { get; } = GetDefinition<SpellDefinition>("FireShield");
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
        internal static SpellDefinition Haste { get; } = GetDefinition<SpellDefinition>("Haste");
        internal static SpellDefinition Heal { get; } = GetDefinition<SpellDefinition>("Heal");
        internal static SpellDefinition HealingWord { get; } = GetDefinition<SpellDefinition>("HealingWord");
        internal static SpellDefinition HeatMetal { get; } = GetDefinition<SpellDefinition>("HeatMetal");
        internal static SpellDefinition HellishRebuke { get; } = GetDefinition<SpellDefinition>("HellishRebuke");
        internal static SpellDefinition Heroism { get; } = GetDefinition<SpellDefinition>("Heroism");
        internal static SpellDefinition HideousLaughter { get; } = GetDefinition<SpellDefinition>("HideousLaughter");
        internal static SpellDefinition HoldMonster { get; } = GetDefinition<SpellDefinition>("HoldMonster");
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

        internal static SpellDefinition LesserRestoration { get; } =
            GetDefinition<SpellDefinition>("LesserRestoration");

        internal static SpellDefinition Levitate { get; } = GetDefinition<SpellDefinition>("Levitate");
        internal static SpellDefinition Light { get; } = GetDefinition<SpellDefinition>("Light");

        internal static SpellDefinition Light_Monk_NoFocus { get; } =
            GetDefinition<SpellDefinition>("Light_Monk_NoFocus");

        internal static SpellDefinition LightningBolt { get; } = GetDefinition<SpellDefinition>("LightningBolt");
        internal static SpellDefinition Longstrider { get; } = GetDefinition<SpellDefinition>("Longstrider");
        internal static SpellDefinition MageArmor { get; } = GetDefinition<SpellDefinition>("MageArmor");
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
        internal static SpellDefinition SpiderClimb { get; } = GetDefinition<SpellDefinition>("SpiderClimb");
        internal static SpellDefinition SpikeGrowth { get; } = GetDefinition<SpellDefinition>("SpikeGrowth");
        internal static SpellDefinition SpiritGuardians { get; } = GetDefinition<SpellDefinition>("SpiritGuardians");
        internal static SpellDefinition SpiritualWeapon { get; } = GetDefinition<SpellDefinition>("SpiritualWeapon");
        internal static SpellDefinition StinkingCloud { get; } = GetDefinition<SpellDefinition>("StinkingCloud");
        internal static SpellDefinition Stoneskin { get; } = GetDefinition<SpellDefinition>("Stoneskin");

        internal static SpellDefinition Sunbeam { get; } = GetDefinition<SpellDefinition>("Sunbeam");
        internal static SpellDefinition Sunburst { get; } = GetDefinition<SpellDefinition>("Sunburst");
        internal static SpellDefinition Tongues { get; } = GetDefinition<SpellDefinition>("Tongues");
        internal static SpellDefinition Thunderstorm { get; } = GetDefinition<SpellDefinition>("Thunderstorm");
        internal static SpellDefinition Thunderwave { get; } = GetDefinition<SpellDefinition>("Thunderwave");
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

        internal static SpellDefinition WallOfThornsWallLine { get; } =
            GetDefinition<SpellDefinition>("WallOfThornsWallLine");

        internal static SpellDefinition WallOfThornsWallRing { get; } =
            GetDefinition<SpellDefinition>("WallOfThornsWallRing");

        internal static SpellDefinition WardingBond { get; } = GetDefinition<SpellDefinition>("WardingBond");
        internal static SpellDefinition WindWall { get; } = GetDefinition<SpellDefinition>("WindWall");
    }

    internal static class SpellListDefinitions
    {
        internal static SpellListDefinition SpellListShockArcanist { get; } =
            GetDefinition<SpellListDefinition>("SpellListShockArcanist");

        internal static SpellListDefinition SpellListAllCantrips { get; } =
            GetDefinition<SpellListDefinition>("SpellListAllCantrips");

        internal static SpellListDefinition SpellListAllSpells { get; } =
            GetDefinition<SpellListDefinition>("SpellListAllSpells");

        internal static SpellListDefinition SpellListBard { get; } =
            GetDefinition<SpellListDefinition>("SpellListBard");

        internal static SpellListDefinition SpellListCleric { get; } =
            GetDefinition<SpellListDefinition>("SpellListCleric");

        internal static SpellListDefinition SpellListDruid { get; } =
            GetDefinition<SpellListDefinition>("SpellListDruid");

        internal static SpellListDefinition SpellListPaladin { get; } =
            GetDefinition<SpellListDefinition>("SpellListPaladin");

        internal static SpellListDefinition SpellListRanger { get; } =
            GetDefinition<SpellListDefinition>("SpellListRanger");

        internal static SpellListDefinition SpellListSorcerer { get; } =
            GetDefinition<SpellListDefinition>("SpellListSorcerer");

        internal static SpellListDefinition SpellListWarlock { get; } =
            GetDefinition<SpellListDefinition>("SpellListWarlock");

        internal static SpellListDefinition SpellListWizard { get; } =
            GetDefinition<SpellListDefinition>("SpellListWizard");
    }

    internal static class DecisionPackageDefinitions
    {
        internal static DecisionPackageDefinition DefaultMeleeWithBackupRangeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeWithBackupRangeDecisions");

        internal static DecisionPackageDefinition Fear { get; } = GetDefinition<DecisionPackageDefinition>("Fear");

        internal static DecisionPackageDefinition IdleGuard_Default { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition ClericCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition FighterCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition PaladinCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition RogueCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition CasterCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition OffensiveCasterCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition DefaultSupportCasterWithBackupAttacksDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        internal static DecisionPackageDefinition DefaultRangeWithBackupMeleeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");
    }

    internal static class ToolTypeDefinitions
    {
        internal static ToolTypeDefinition ArtisanToolSmithToolsType { get; } =
            GetDefinition<ToolTypeDefinition>("ArtisanToolSmithToolsType");

        internal static ToolTypeDefinition DisguiseKitType { get; } =
            GetDefinition<ToolTypeDefinition>("DisguiseKitType");

        internal static ToolTypeDefinition EnchantingToolType { get; } =
            GetDefinition<ToolTypeDefinition>("EnchantingToolType");

        internal static ToolTypeDefinition HerbalismKitType { get; } =
            GetDefinition<ToolTypeDefinition>("HerbalismKitType");

        internal static ToolTypeDefinition PoisonersKitType { get; } =
            GetDefinition<ToolTypeDefinition>("PoisonersKitType");

        internal static ToolTypeDefinition ScrollKitType { get; } = GetDefinition<ToolTypeDefinition>("ScrollKitType");

        internal static ToolTypeDefinition ThievesToolsType { get; } =
            GetDefinition<ToolTypeDefinition>("ThievesToolsType");
    }

    internal static class TreasureTableDefinitions
    {
        internal static TreasureTableDefinition DLC3_RandomTreasureTableJ_IngredientsEnchanted { get; } =
            GetDefinition<TreasureTableDefinition>("DLC3_RandomTreasureTableJ_IngredientsEnchanted");

        internal static TreasureTableDefinition RandomTreasureTableA_Gem { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableA_Gem");

        internal static TreasureTableDefinition RandomTreasureTableB_Consumables { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableB_Consumables");

        internal static TreasureTableDefinition RandomTreasureTableE_Ingredients { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableE_Ingredients");

        internal static TreasureTableDefinition RandomTreasureTableE2_Mundane_Ingredients { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableE2_Mundane_Ingredients");
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

        internal static WeaponTypeDefinition LightCrossbowType { get; } =
            GetDefinition<WeaponTypeDefinition>("LightCrossbowType");

        internal static WeaponTypeDefinition LongbowType { get; } = GetDefinition<WeaponTypeDefinition>("LongbowType");

        internal static WeaponTypeDefinition LongswordType { get; } =
            GetDefinition<WeaponTypeDefinition>("LongswordType");

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
