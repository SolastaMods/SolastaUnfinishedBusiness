using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using TA.AI;

namespace SolastaUnfinishedBusiness.Api;

public static partial class DatabaseHelper
{
    [NotNull]
    private static T GetDefinition<T>(string key) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException($"Database of type {typeof(T).Name} not found.");
        }

        var definition = db.TryGetElement(key, string.Empty);

        if (definition == null)
        {
            throw new SolastaUnfinishedBusinessException(
                $"Definition with name={key} not found in database {typeof(T).Name}.");
        }

        return definition;
    }

    public static bool TryGetDefinition<T>([CanBeNull] string key, [CanBeNull] out T definition)
        where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (key == null || db == null)
        {
            definition = null;

            return false;
        }

        definition = db.TryGetElement(key, string.Empty);

        return definition != null;
    }

    public static class CampaignDefinitions
    {
        public static CampaignDefinition UserCampaign { get; } =
            GetDefinition<CampaignDefinition>("UserCampaign");
    }

    public static class DecisionPackageDefinitions
    {
        public static DecisionPackageDefinition DefaultMeleeWithBackupRangeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeWithBackupRangeDecisions");

        public static DecisionPackageDefinition DefaultRangeWithBackupMeleeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultRangeWithBackupMeleeDecisions");

        public static DecisionPackageDefinition DefaultSupportCasterWithBackupAttacksDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultSupportCasterWithBackupAttacksDecisions");

        public static DecisionPackageDefinition IdleGuardDefault { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");
    }

    public static class FactionDefinitions
    {
        public static FactionDefinition HostileMonsters { get; } =
            GetDefinition<FactionDefinition>("HostileMonsters");
    }

    public static class FormationDefinitions
    {
        public static FormationDefinition Squad4 { get; } =
            GetDefinition<FormationDefinition>("Squad4");

        public static FormationDefinition SingleCreature { get; } =
            GetDefinition<FormationDefinition>("SingleCreature");
    }

    public static class GadgetBlueprints
    {
        public static GadgetBlueprint TeleporterIndividual { get; } =
            GetDefinition<GadgetBlueprint>("TeleporterIndividual");

        public static GadgetBlueprint TeleporterParty { get; } = GetDefinition<GadgetBlueprint>("TeleporterParty");

        public static GadgetBlueprint VirtualExit { get; } = GetDefinition<GadgetBlueprint>("VirtualExit");

        public static GadgetBlueprint VirtualExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExitMultiple");

        public static GadgetBlueprint Exit { get; } = GetDefinition<GadgetBlueprint>("Exit");

        public static GadgetBlueprint ExitMultiple { get; } = GetDefinition<GadgetBlueprint>("ExitMultiple");
    }

    public static class GadgetDefinitions
    {
        public static GadgetDefinition Activator { get; } =
            GetDefinition<GadgetDefinition>("Activator");
    }

    public static class LootPackDefinitions
    {
        public static LootPackDefinition PickpocketGenericLootLowMoney { get; } =
            GetDefinition<LootPackDefinition>("Pickpocket_generic_loot_LowMoney");

        public static LootPackDefinition PickpocketGenericLootMedMoney { get; } =
            GetDefinition<LootPackDefinition>("Pickpocket_generic_loot_MedMoney");
    }

    public static class MonsterAttackDefinitions
    {
        public static MonsterAttackDefinition AttackGenericGuardLongsword { get; } =
            GetDefinition<MonsterAttackDefinition>("Attack_Generic_Guard_Longsword");
    }

    public static class MorphotypeElementDefinitions
    {
        public static MorphotypeElementDefinition FaceShape_NPC_Aksha { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceShape_NPC_Aksha");

        public static MorphotypeElementDefinition FaceAndSkin_Neutral { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_Neutral");

        public static MorphotypeElementDefinition FaceAndSkin_Defiler { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_Defiler");

        public static MorphotypeElementDefinition BodyDecorationColor_Default_00 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_Default_00");

        public static MorphotypeElementDefinition FaceAndSkin_01 { get; } =
            GetDefinition<MorphotypeElementDefinition>("FaceAndSkin_01");

        public static MorphotypeElementDefinition HairColorSilver { get; } =
            GetDefinition<MorphotypeElementDefinition>("HairColorSilver");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_00 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_00");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_01 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_01");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_02 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_02");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_03 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_03");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_04 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_04");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_05 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_05");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_06 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_06");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_07 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_07");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_08 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_08");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_09 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_09");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_10 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_10");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_11 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_11");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_12 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_12");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_13 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_13");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_14 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_14");

        public static MorphotypeElementDefinition BodyDecorationColor_SorcererManaPainter_15 { get; } =
            GetDefinition<MorphotypeElementDefinition>("BodyDecorationColor_SorcererManaPainter_15");
    }

    public static class ReactionDefinitions
    {
        public static ReactionDefinition OpportunityAttack { get; } =
            GetDefinition<ReactionDefinition>("OpportunityAttack");
    }

    public static class TreasureTableDefinitions
    {
        public static TreasureTableDefinition RandomTreasureTableE_Ingredients { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableE_Ingredients");

        public static TreasureTableDefinition RandomTreasureTableA_Gem { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableA_Gem");

        public static TreasureTableDefinition RandomTreasureTableB_Consumables { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableB_Consumables");

        public static TreasureTableDefinition RandomTreasureTableE2_Mundane_Ingredients { get; } =
            GetDefinition<TreasureTableDefinition>("RandomTreasureTableE2_Mundane_Ingredients");
    }
}
