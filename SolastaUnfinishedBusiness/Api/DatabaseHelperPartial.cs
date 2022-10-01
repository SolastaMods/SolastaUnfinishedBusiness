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

    public static class ReactionDefinitions
    {
        public static ReactionDefinition OpportunityAttack { get; } =
            GetDefinition<ReactionDefinition>("OpportunityAttack");
    }
}
