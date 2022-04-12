namespace SolastaCommunityExpansion.Models
{
    internal static class SetFactionRelationsContext
    {
        internal static void SetFactionRelation(string name, int value)
        {
            var service = ServiceRepository.GetService<IGameFactionService>();
            if (service != null)
            {
                service.ExecuteFactionOperation(name, FactionDefinition.FactionOperation.Increase, value - service.FactionRelations[name], "", null /* this string and monster doesn't matter if we're using "SetValue" */);
            }
        }
    }
}
