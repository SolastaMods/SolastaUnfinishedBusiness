using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using TA.AI;

namespace SolastaUnfinishedBusiness.Api;

public static partial class DatabaseHelper
{
    [NotNull]
    public static T GetDefinition<T>(string key, string guid) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException($"Database of type {typeof(T).Name} not found.");
        }

        var definition = db.TryGetElement(key, guid);

        if (definition == null)
        {
            throw new SolastaUnfinishedBusinessException(
                $"Definition with name={key} or guid={guid} not found in database {typeof(T).Name}.");
        }

        return definition;
    }

    public static bool TryGetDefinition<T>([CanBeNull] string key, string guid, [CanBeNull] out T definition)
        where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (key == null || db == null)
        {
            definition = null;
            return false;
        }

        definition = db.TryGetElement(key, guid);

        return definition != null;
    }

    public static class DecisionPackageDefinitions
    {
        public static DecisionPackageDefinition DefaultFlyingBeastWithBackupRangeCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultFlyingBeastWithBackupRangeCombatDecisions",
                "2de7c4b469d4b984b80c0dbb2f82acab");

        public static DecisionPackageDefinition DefaultMeleeBeastCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeBeastCombatDecisions",
                "fae777d0212f18d4189c57c8ba45a0e7");

        public static DecisionPackageDefinition DefaultMeleeBeastWithBackupRangeCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeBeastWithBackupRangeCombatDecisions",
                "9de82cc9c02a76d4999813828cd55c64");

        public static DecisionPackageDefinition DefaultMeleeWithBackupRangeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeWithBackupRangeDecisions",
                "36bb3688d84582249bf0f1c85064ad10");

        public static DecisionPackageDefinition DefaultRangeWithBackupMeleeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultRangeWithBackupMeleeDecisions",
                "a938084b17b2aad46bffadc07456119a");

        public static DecisionPackageDefinition DefaultSupportCasterWithBackupAttacksDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultSupportCasterWithBackupAttacksDecisions",
                "f99448e8f4c2d9d478837c543e3e205f");

        public static DecisionPackageDefinition Idle { get; } =
            GetDefinition<DecisionPackageDefinition>("Idle", "47dfd6fd3d22b214cb8c446f2438f3c6");
        
        public static DecisionPackageDefinition IdleGuard_Default { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default", "232428b431ff2414eb8254fb1e9e4cf1");

        public static DecisionPackageDefinition IdleGuard_Default_CanAttackNPC { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default_CanAttackNPC",
                "359aff696ee49104e8224b564ee00372");
    }
}
