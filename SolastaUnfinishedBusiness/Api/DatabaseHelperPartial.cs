using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using TA.AI;

namespace SolastaUnfinishedBusiness.Api;

public static partial class DatabaseHelper
{
    [NotNull]
    public static T GetDefinition<T>(string key) where T : BaseDefinition
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

    public static class DecisionPackageDefinitions
    {
        public static DecisionPackageDefinition DefaultFlyingBeastWithBackupRangeCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultFlyingBeastWithBackupRangeCombatDecisions");

        public static DecisionPackageDefinition DefaultMeleeBeastCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeBeastCombatDecisions");

        public static DecisionPackageDefinition DefaultMeleeBeastWithBackupRangeCombatDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeBeastWithBackupRangeCombatDecisions");

        public static DecisionPackageDefinition DefaultMeleeWithBackupRangeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultMeleeWithBackupRangeDecisions");

        public static DecisionPackageDefinition DefaultRangeWithBackupMeleeDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultRangeWithBackupMeleeDecisions");

        public static DecisionPackageDefinition DefaultSupportCasterWithBackupAttacksDecisions { get; } =
            GetDefinition<DecisionPackageDefinition>("DefaultSupportCasterWithBackupAttacksDecisions");

        public static DecisionPackageDefinition Idle { get; } =
            GetDefinition<DecisionPackageDefinition>("Idle");

        public static DecisionPackageDefinition IdleGuard_Default { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default");

        public static DecisionPackageDefinition IdleGuard_Default_CanAttackNPC { get; } =
            GetDefinition<DecisionPackageDefinition>("IdleGuard_Default_CanAttackNPC");
    }
}
