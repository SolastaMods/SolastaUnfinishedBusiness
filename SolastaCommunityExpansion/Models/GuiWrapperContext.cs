using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Models
{
    internal static class GuiWrapperContext
    {
        internal static void Recache()
        {
            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>() as GuiWrapperManager;
            var runtimeService = ServiceRepository.GetService<IRuntimeService>();

            if (guiWrapperService == null || runtimeService?.Runtime == null)
            {
                return;
            }

            guiWrapperService.classDefinitionsMap.Clear();
            guiWrapperService.raceDefinitionsMap.Clear();
            guiWrapperService.monsterDefinitionsMap.Clear();
            guiWrapperService.merchantDefinitionsMap.Clear();
            guiWrapperService.itemDefinitionsMap.Clear();
            guiWrapperService.spellDefinitionsMap.Clear();
            guiWrapperService.effectProxyDefinitionsMap.Clear();
            guiWrapperService.powerDefinitionsMap.Clear();
            guiWrapperService.toolTypeDefinitionsMap.Clear();
            guiWrapperService.recipeDefinitionsMap.Clear();
            guiWrapperService.factionDefinitionsMap.Clear();
            guiWrapperService.environmentEffectDefinitionsMap.Clear();

            guiWrapperService.RuntimeLoaded(runtimeService.Runtime);
        }

        internal static void RecacheFeats()
        {
            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>() as GuiWrapperManager;

            if (guiWrapperService == null)
            {
                return;
            }

            guiWrapperService.featDefinitionsMap.Clear();
            guiWrapperService.LoadFeatDefinitions();
        }
    }
}
