namespace SolastaCommunityExpansion.Models
{
    internal static class SetFactionRelationsContext
    {
        internal static void Load()
        {
            if (Main.Settings.SetMaxFactionRelations)
            {
                var service = ServiceRepository.GetService<IGameFactionService>();
                if (service != null)
                {
                    service.ModifyRelation("Antiquarians", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                    service.ModifyRelation("Arcaneum", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                    service.ModifyRelation("ChurchOfEinar", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                    service.ModifyRelation("CircleOfDanantar", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                    service.ModifyRelation("Principality", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                    service.ModifyRelation("Scavengers", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                    service.ModifyRelation("TowerOfKnowledge", FactionDefinition.RelationOperation.SetValue, 100, "LivingLegend");
                }
            }
        }
    }
}
