using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolastaCommunityExpansion.Models
{
    internal static class RemoveIdentificationContext
    {
        internal static void Load()
        {
            if (Main.Settings.NoIdentification)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresIdentification(false);
                }
            }
        }
    }
}
