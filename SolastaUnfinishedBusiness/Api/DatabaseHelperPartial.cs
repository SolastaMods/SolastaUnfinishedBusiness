using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using TA.AI;

namespace SolastaUnfinishedBusiness.Api;

internal static partial class DatabaseHelper
{
    [NotNull]
    internal static T GetDefinition<T>(string key) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException(
                $"{typeof(T).Name} not found.");
        }

        if (!db.TryGetElement(key, out var definition))
        {
            throw new SolastaUnfinishedBusinessException(
                $"{key} not found in database {typeof(T).Name}.");
        }

        return definition;
    }

    internal static bool TryGetDefinition<T>(string key, out T definition) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (key != null && db != null)
        {
            return db.TryGetElement(key, out definition);
        }

        definition = null;

        return false;
    }

    internal static class DecisionPackageDefinitions
    {
        internal static DecisionPackageDefinition DefaultMeleeWithBackupRangeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeWithBackupRangeDecisions");

        internal static DecisionPackageDefinition DefaultSupportCasterWithBackupAttacksDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultSupportCasterWithBackupAttacksDecisions");

        internal static DecisionPackageDefinition IdleGuard_Default { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");
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

        internal static GadgetBlueprint TeleporterParty { get; } =
            GetDefinition<GadgetBlueprint>("TeleporterParty");

        internal static GadgetBlueprint VirtualExit { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExit");

        internal static GadgetBlueprint VirtualExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExitMultiple");

        internal static GadgetBlueprint Exit { get; } =
            GetDefinition<GadgetBlueprint>("Exit");

        internal static GadgetBlueprint ExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("ExitMultiple");
    }

    internal static class GadgetDefinitions
    {
        internal static GadgetDefinition Activator { get; } =
            GetDefinition<GadgetDefinition>("Activator");
    }

    internal static class LootPackDefinitions
    {
        internal static LootPackDefinition Pickpocket_generic_loot_LowMoney { get; } =
            GetDefinition<LootPackDefinition>("Pickpocket_generic_loot_LowMoney");

        internal static LootPackDefinition Pickpocket_generic_loot_MedMoney { get; } =
            GetDefinition<LootPackDefinition>("Pickpocket_generic_loot_MedMoney");
    }

    internal static class MonsterAttackDefinitions
    {
        internal static MonsterAttackDefinition Attack_Generic_Guard_Longsword { get; } =
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
}
