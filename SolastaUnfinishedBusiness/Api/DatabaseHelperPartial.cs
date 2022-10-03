using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using TA.AI;

namespace SolastaUnfinishedBusiness.Api;

internal static partial class DatabaseHelper
{
    [NotNull]
    private static T GetDefinition<T>(string key) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException($"Database of type {typeof(T).Name} not found.");
        }

        if (!db.TryGetElement(key, out var definition))
        {
            throw new SolastaUnfinishedBusinessException(
                $"Definition with name={key} not found in database {typeof(T).Name}.");
        }

        return definition;
    }

    internal static bool TryGetDefinition<T>(string key, out T definition)
        where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (key != null && db != null)
        {
            return db.TryGetElement(key, out definition);
        }

        definition = null;

        return false;
    }

    internal static class CampaignDefinitions
    {
        internal static CampaignDefinition UserCampaign { get; } =
            GetDefinition<CampaignDefinition>("UserCampaign");
    }

    internal static class DecisionPackageDefinitions
    {
        internal static DecisionPackageDefinition DefaultMeleeWithBackupRangeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeWithBackupRangeDecisions");

        internal static DecisionPackageDefinition DefaultRangeWithBackupMeleeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultRangeWithBackupMeleeDecisions");

        internal static DecisionPackageDefinition DefaultSupportCasterWithBackupAttacksDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultSupportCasterWithBackupAttacksDecisions");

        internal static DecisionPackageDefinition IdleGuardDefault { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");
    }

    internal static class FactionDefinitions
    {
        internal static FactionDefinition HostileMonsters { get; } =
            GetDefinition<FactionDefinition>("HostileMonsters");
    }

    internal static class FormationDefinitions
    {
        internal static FormationDefinition Squad4 { get; } =
            GetDefinition<FormationDefinition>("Squad4");

        internal static FormationDefinition SingleCreature { get; } =
            GetDefinition<FormationDefinition>("SingleCreature");
    }

    internal static class GadgetBlueprints
    {
        internal static GadgetBlueprint TeleporterIndividual { get; } =
            GetDefinition<GadgetBlueprint>("TeleporterIndividual");

        internal static GadgetBlueprint TeleporterParty { get; } = GetDefinition<GadgetBlueprint>("TeleporterParty");

        internal static GadgetBlueprint VirtualExit { get; } = GetDefinition<GadgetBlueprint>("VirtualExit");

        internal static GadgetBlueprint VirtualExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExitMultiple");

        internal static GadgetBlueprint Exit { get; } = GetDefinition<GadgetBlueprint>("Exit");

        internal static GadgetBlueprint ExitMultiple { get; } = GetDefinition<GadgetBlueprint>("ExitMultiple");
    }

    internal static class GadgetDefinitions
    {
        internal static GadgetDefinition Activator { get; } =
            GetDefinition<GadgetDefinition>("Activator");
    }

    internal static class LootPackDefinitions
    {
        internal static LootPackDefinition PickpocketGenericLootLowMoney { get; } =
            GetDefinition<LootPackDefinition>("Pickpocket_generic_loot_LowMoney");

        internal static LootPackDefinition PickpocketGenericLootMedMoney { get; } =
            GetDefinition<LootPackDefinition>("Pickpocket_generic_loot_MedMoney");
    }

    internal static class MonsterAttackDefinitions
    {
        internal static MonsterAttackDefinition AttackGenericGuardLongsword { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Generic_Guard_Longsword");
    }

    internal static class MorphotypeElementDefinitions
    {
        internal static MorphotypeElementDefinition FaceShape_NPC_Aksha { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceShape_NPC_Aksha");

        internal static MorphotypeElementDefinition FaceAndSkin_Neutral { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_Neutral");

        internal static MorphotypeElementDefinition FaceAndSkin_Defiler { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_Defiler");

        internal static MorphotypeElementDefinition BodyDecorationColor_Default_00 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_Default_00");

        internal static MorphotypeElementDefinition FaceAndSkin_01 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_01");

        internal static MorphotypeElementDefinition HairColorSilver { get; } =
            GetDefinition<MorphotypeElementDefinition>("HairColorSilver");

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
    }

    internal static class ReactionDefinitions
    {
        internal static ReactionDefinition OpportunityAttack { get; } =
            GetDefinition<ReactionDefinition>("OpportunityAttack");
    }

    internal static class TreasureTableDefinitions
    {
        internal static TreasureTableDefinition RandomTreasureTableE_Ingredients { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableE_Ingredients");

        internal static TreasureTableDefinition RandomTreasureTableA_Gem { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableA_Gem");

        internal static TreasureTableDefinition RandomTreasureTableB_Consumables { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableB_Consumables");

        internal static TreasureTableDefinition RandomTreasureTableE2_Mundane_Ingredients { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableE2_Mundane_Ingredients");
    }

    internal static class MonsterDefinitions
    {
        internal static MonsterDefinition Air_Elemental { get; } = GetDefinition<MonsterDefinition>("Air_Elemental");

        internal static MonsterDefinition Ape_MonsterDefinition { get; } =
            GetDefinition<MonsterDefinition>("Ape_MonsterDefinition");

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
        internal static MonsterDefinition Ghoul { get; } = GetDefinition<MonsterDefinition>("Ghoul");
        internal static MonsterDefinition Giant_Ape { get; } = GetDefinition<MonsterDefinition>("Giant_Ape");

        internal static MonsterDefinition GoldDragon_AerElai { get; } =
            GetDefinition<MonsterDefinition>("GoldDragon_AerElai");

        internal static MonsterDefinition Green_Hag { get; } = GetDefinition<MonsterDefinition>("Green_Hag");

        internal static MonsterDefinition GreenDragon_MasterOfConjuration { get; } =
            GetDefinition<MonsterDefinition>("GreenDragon_MasterOfConjuration");

        internal static MonsterDefinition InvisibleStalker { get; } =
            GetDefinition<MonsterDefinition>("InvisibleStalker");

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

        internal static MonsterDefinition Skeleton_Sorcerer { get; } =
            GetDefinition<MonsterDefinition>("Skeleton_Sorcerer");

        internal static MonsterDefinition Sorr_Akkath_Shikkath { get; } =
            GetDefinition<MonsterDefinition>("Sorr-Akkath_Shikkath");

        internal static MonsterDefinition Sorr_Akkath_Tshar_Boss { get; } =
            GetDefinition<MonsterDefinition>("Sorr-Akkath_Tshar_Boss");

        internal static MonsterDefinition Spider_Queen { get; } = GetDefinition<MonsterDefinition>("Spider_Queen");

        internal static MonsterDefinition SuperEgo_Servant_Hostile { get; } =
            GetDefinition<MonsterDefinition>("SuperEgo_Servant_Hostile");

        internal static MonsterDefinition WindSnake { get; } = GetDefinition<MonsterDefinition>("WindSnake");
        internal static MonsterDefinition Wolf { get; } = GetDefinition<MonsterDefinition>("Wolf");
        internal static MonsterDefinition Zombie { get; } = GetDefinition<MonsterDefinition>("Zombie");
    }

    internal static class ConditionDefinitions
    {
        internal static ConditionDefinition ConditionBlinded { get; } =
            GetDefinition<ConditionDefinition>("ConditionBlinded");

        internal static ConditionDefinition ConditionFrightened { get; } =
            GetDefinition<ConditionDefinition>("ConditionFrightened");

        internal static ConditionDefinition ConditionStunned { get; } =
            GetDefinition<ConditionDefinition>("ConditionStunned");

        internal static ConditionDefinition ConditionBlessed { get; } =
            GetDefinition<ConditionDefinition>("ConditionBlessed");

        internal static ConditionDefinition ConditionShielded { get; } =
            GetDefinition<ConditionDefinition>("ConditionShielded");

        internal static ConditionDefinition ConditionTrueStrike { get; } =
            GetDefinition<ConditionDefinition>("ConditionTrueStrike");

        internal static ConditionDefinition ConditionHeroism { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeroism");

        internal static ConditionDefinition ConditionDazzled { get; } =
            GetDefinition<ConditionDefinition>("ConditionDazzled");

        internal static ConditionDefinition ConditionEncumbered { get; } =
            GetDefinition<ConditionDefinition>("ConditionEncumbered");

        internal static ConditionDefinition ConditionDarkvision { get; } =
            GetDefinition<ConditionDefinition>("ConditionDarkvision");

        internal static ConditionDefinition ConditionOnFire1D4 { get; } =
            GetDefinition<ConditionDefinition>("ConditionOnFire1D4");

        internal static ConditionDefinition ConditionDead { get; } =
            GetDefinition<ConditionDefinition>("ConditionDead");

        internal static ConditionDefinition ConditionRestrained { get; } =
            GetDefinition<ConditionDefinition>("ConditionRestrained");

        internal static ConditionDefinition ConditionDarkness { get; } =
            GetDefinition<ConditionDefinition>("ConditionDarkness");

        internal static ConditionDefinition ConditionLevitate { get; } =
            GetDefinition<ConditionDefinition>("ConditionLevitate");

        internal static ConditionDefinition ConditionCharmed { get; } =
            GetDefinition<ConditionDefinition>("ConditionCharmed");

        internal static ConditionDefinition ConditionParalyzed { get; } =
            GetDefinition<ConditionDefinition>("ConditionParalyzed");

        internal static ConditionDefinition ConditionBaned { get; } =
            GetDefinition<ConditionDefinition>("ConditionBaned");

        internal static ConditionDefinition ConditionBleeding { get; } =
            GetDefinition<ConditionDefinition>("ConditionBleeding");

        internal static ConditionDefinition ConditionBarkskin { get; } =
            GetDefinition<ConditionDefinition>("ConditionBarkskin");

        internal static ConditionDefinition ConditionLightSensitive { get; } =
            GetDefinition<ConditionDefinition>("ConditionLightSensitive");

        internal static ConditionDefinition ConditionInvisible { get; } =
            GetDefinition<ConditionDefinition>("ConditionInvisible");

        internal static ConditionDefinition ConditionKindredSpiritBondAC { get; } =
            GetDefinition<ConditionDefinition>("ConditionKindredSpiritBondAC");

        internal static ConditionDefinition ConditionKindredSpiritBondHP { get; } =
            GetDefinition<ConditionDefinition>("ConditionKindredSpiritBondHP");

        internal static ConditionDefinition ConditionKindredSpiritBondMeleeAttack { get; } =
            GetDefinition<ConditionDefinition>("ConditionKindredSpiritBondMeleeAttack");

        internal static ConditionDefinition ConditionKindredSpiritBondMeleeDamage { get; } =
            GetDefinition<ConditionDefinition>("ConditionKindredSpiritBondMeleeDamage");

        internal static ConditionDefinition ConditionKindredSpiritBondSavingThrows { get; } =
            GetDefinition<ConditionDefinition>("ConditionKindredSpiritBondSavingThrows");

        internal static ConditionDefinition ConditionMarkedByBrandingSmite { get; } =
            GetDefinition<ConditionDefinition>("ConditionMarkedByBrandingSmite");

        internal static ConditionDefinition ConditionFlying12 { get; } =
            GetDefinition<ConditionDefinition>("ConditionFlying12");

        internal static ConditionDefinition ConditionBearsEndurance { get; } =
            GetDefinition<ConditionDefinition>("ConditionBearsEndurance");

        internal static ConditionDefinition ConditionProne { get; } =
            GetDefinition<ConditionDefinition>("ConditionProne");

        internal static ConditionDefinition ConditionParalyzed_CrimsonSpiderVenom { get; } =
            GetDefinition<ConditionDefinition>("ConditionParalyzed_CrimsonSpiderVenom");

        internal static ConditionDefinition ConditionParalyzed_GhoulsCaress { get; } =
            GetDefinition<ConditionDefinition>("ConditionParalyzed_GhoulsCaress");

        internal static ConditionDefinition ConditionBranded { get; } =
            GetDefinition<ConditionDefinition>("ConditionBranded");

        internal static ConditionDefinition ConditionSeeInvisibility { get; } =
            GetDefinition<ConditionDefinition>("ConditionSeeInvisibility");

        internal static ConditionDefinition ConditionStunned_MutantApeSlam { get; } =
            GetDefinition<ConditionDefinition>("ConditionStunned_MutantApeSlam");

        internal static ConditionDefinition ConditionStunnedConjuredDeath { get; } =
            GetDefinition<ConditionDefinition>("ConditionStunnedConjuredDeath");

        internal static ConditionDefinition ConditionCalmedByCalmEmotionsAlly { get; } =
            GetDefinition<ConditionDefinition>("ConditionCalmedByCalmEmotionsAlly");

        internal static ConditionDefinition ConditionProtectedFromPoison { get; } =
            GetDefinition<ConditionDefinition>("ConditionProtectedFromPoison");

        internal static ConditionDefinition ConditionBullsStrength { get; } =
            GetDefinition<ConditionDefinition>("ConditionBullsStrength");

        internal static ConditionDefinition ConditionRaging { get; } =
            GetDefinition<ConditionDefinition>("ConditionRaging");

        internal static ConditionDefinition ConditionAcidSpit { get; } =
            GetDefinition<ConditionDefinition>("ConditionAcidSpit");

        internal static ConditionDefinition ConditionHighlighted { get; } =
            GetDefinition<ConditionDefinition>("ConditionHighlighted");

        internal static ConditionDefinition ConditionIncapacitated { get; } =
            GetDefinition<ConditionDefinition>("ConditionIncapacitated");

        internal static ConditionDefinition ConditionWildShapeSubstituteForm { get; } =
            GetDefinition<ConditionDefinition>("ConditionWildShapeSubstituteForm");

        internal static ConditionDefinition ConditionHeraldOfBattle { get; } =
            GetDefinition<ConditionDefinition>("ConditionHeraldOfBattle");

        internal static ConditionDefinition ConditionConjuredCreature { get; } =
            GetDefinition<ConditionDefinition>("ConditionConjuredCreature");

        internal static ConditionDefinition ConditionFrightenedPhantasmalKiller { get; } =
            GetDefinition<ConditionDefinition>("ConditionFrightenedPhantasmalKiller");

        internal static ConditionDefinition ConditionFrightenedFear { get; } =
            GetDefinition<ConditionDefinition>("ConditionFrightenedFear");

        internal static ConditionDefinition ConditionStoneResilience { get; } =
            GetDefinition<ConditionDefinition>("ConditionStoneResilience");

        internal static ConditionDefinition ConditionHindered_By_Frost { get; } =
            GetDefinition<ConditionDefinition>("ConditionHindered_By_Frost");

        internal static ConditionDefinition ConditionInvisibleBase { get; } =
            GetDefinition<ConditionDefinition>("ConditionInvisibleBase");

        internal static ConditionDefinition ConditionInvisibleGreater { get; } =
            GetDefinition<ConditionDefinition>("ConditionInvisibleGreater");

        internal static ConditionDefinition ConditionJump { get; } =
            GetDefinition<ConditionDefinition>("ConditionJump");

        internal static ConditionDefinition ConditionCharmedByHypnoticPattern { get; } =
            GetDefinition<ConditionDefinition>("ConditionCharmedByHypnoticPattern");

        internal static ConditionDefinition ConditionDummy { get; } =
            GetDefinition<ConditionDefinition>("ConditionDummy");

        internal static ConditionDefinition ConditionRevealedByDetectGoodOrEvil { get; } =
            GetDefinition<ConditionDefinition>("ConditionRevealedByDetectGoodOrEvil");

        internal static ConditionDefinition ConditionSorcererChildRiftDeflection { get; } =
            GetDefinition<ConditionDefinition>("ConditionSorcererChildRiftDeflection");
    }
}
