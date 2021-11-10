namespace SolastaCommunityExpansion.Models
{
    internal static class SetFactionRelationsContext
    {
        internal static void SetFactionRelation(string name, int value)
        {
            var service = ServiceRepository.GetService<IGameFactionService>();
            if (service != null)
            {
                service.ModifyRelation(name, FactionDefinition.RelationOperation.SetValue, value, "" /* this string doesn't matter if we're using "SetValue" */);
            }
        }
    }
}
