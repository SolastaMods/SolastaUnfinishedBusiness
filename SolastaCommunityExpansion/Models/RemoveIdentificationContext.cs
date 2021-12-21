using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class RemoveIdentificationContext
    {
        internal static void Load()
        {
            if (Main.Settings.RemoveIdentifcationRequirements)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresIdentification(false);
                }
            }

            if (Main.Settings.RemoveAttunementRequirements)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresAttunement(false);
                }
            }
        }
    }
}
