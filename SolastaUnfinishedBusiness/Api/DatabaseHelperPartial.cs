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
}
