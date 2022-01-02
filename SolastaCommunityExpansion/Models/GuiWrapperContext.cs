using System.Collections.Generic;
using SolastaModApi.Infrastructure;

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

            guiWrapperService.GetField<IGuiWrapperService, Dictionary<string, GuiSpellDefinition>>("spellDefinitionsMap");

            var spellDefinitionsMap = guiWrapperService.GetField<IGuiWrapperService, Dictionary<string, GuiSpellDefinition>>("spellDefinitionsMap");

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

            var featDefinitionsMap = guiWrapperService.GetField<IGuiWrapperService, Dictionary<string, GuiFeatDefinition>>("featDefinitionsMap");

            featDefinitionsMap.Clear();

            foreach (FeatDefinition featDefinition in DatabaseRepository.GetDatabase<FeatDefinition>())
            {
                featDefinitionsMap.Add(featDefinition.Name, new GuiFeatDefinition(featDefinition));
            }
        }
    }
}
