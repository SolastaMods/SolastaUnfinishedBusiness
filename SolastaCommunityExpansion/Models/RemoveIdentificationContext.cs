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
            if(Main.Settings.NoIdentification)
            {
                var itemDB = DatabaseRepository.GetDatabase<ItemDefinition>();
                foreach (ItemDefinition item in itemDB.GetAllElements())
                    item.SetRequiresIdentification(false);
            }
        }
    }
}
