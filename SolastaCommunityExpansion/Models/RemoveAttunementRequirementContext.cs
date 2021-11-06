using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class RemoveAttunementRequirementContext
    {
        internal static void Load()
        {
            if (Main.Settings.NoAttunementRequired)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresAttunement(false);
                }
            }
        }
    }
}
