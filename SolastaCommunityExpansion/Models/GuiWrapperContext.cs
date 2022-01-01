using System.Collections.Generic;
using HarmonyLib;

namespace SolastaCommunityExpansion.Models
{
    internal static class GuiWrapperContext
    {
        internal static void RecacheSpells() 
        {
            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();

            if (guiWrapperService == null)
            {
                return;
            }

            var spellDefinitionsMap = AccessTools.Field(guiWrapperService.GetType(), "spellDefinitionsMap").GetValue(guiWrapperService) as Dictionary<string, GuiSpellDefinition>;

            spellDefinitionsMap.Clear();

            foreach (var spellDefinition in DatabaseRepository.GetDatabase<SpellDefinition>()) 
            {
                spellDefinitionsMap.Add(spellDefinition.Name, new GuiSpellDefinition(spellDefinition));
            }     
        }

        internal static void RecacheFeats()
        {
            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();

            if (guiWrapperService == null)
            {
                return;
            }

            var featDefinitionsMap = AccessTools.Field(guiWrapperService.GetType(), "featDefinitionsMap").GetValue(guiWrapperService) as Dictionary<string, GuiFeatDefinition>;

            featDefinitionsMap.Clear();

            foreach (FeatDefinition featDefinition in DatabaseRepository.GetDatabase<FeatDefinition>())
            {
                featDefinitionsMap.Add(featDefinition.Name, new GuiFeatDefinition(featDefinition));
            }
        }
    }
}
