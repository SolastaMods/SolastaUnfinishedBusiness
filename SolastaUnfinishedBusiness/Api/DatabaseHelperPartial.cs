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
            throw new SolastaUnfinishedBusinessException(
                $"Database of type {typeof(T).Name} not found.");
        }

        if (!db.TryGetElement(key, out var definition))
        {
            throw new SolastaUnfinishedBusinessException(
                $"Definition with name={key} not found in database {typeof(T).Name}.");
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

    internal static class CampaignDefinitions
    {
        internal static CampaignDefinition UserCampaign { get; } = GetDefinition<CampaignDefinition>("UserCampaign");
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

    internal static class FactionDefinitions
    {
        internal static FactionDefinition HostileMonsters { get; } =
            GetDefinition<FactionDefinition>("HostileMonsters");
    }

    internal static class FormationDefinitions
    {
        internal static FormationDefinition Squad4 { get; } = GetDefinition<FormationDefinition>("Squad4");

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
        internal static GadgetDefinition Activator { get; } = GetDefinition<GadgetDefinition>("Activator");
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

    internal static class ReactionDefinitions
    {
        internal static ReactionDefinition OpportunityAttack { get; } =
            GetDefinition<ReactionDefinition>("OpportunityAttack");
    }
}
