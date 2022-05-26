using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Models
{
    internal static class GuiWrapperContext
    {
        internal static void Recache()
        {
            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();
            var runtimeService = ServiceRepository.GetService<IRuntimeService>();

            if (guiWrapperService == null || runtimeService?.Runtime == null)
            {
                return;
            }

            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiCharacterClassDefinition>>("classDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiCharacterRaceDefinition>>("raceDefinitionsMap")
                .Clear();
            guiWrapperService.GetField<IGuiWrapperService, Dictionary<string, GuiFeatDefinition>>("featDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiMonsterDefinition>>("monsterDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiMerchantDefinition>>("merchantDefinitionsMap")
                .Clear();
            guiWrapperService.GetField<IGuiWrapperService, Dictionary<string, GuiItemDefinition>>("itemDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiSpellDefinition>>("spellDefinitionsMap").Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiEffectProxyDefinition>>("effectProxyDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiPowerDefinition>>("powerDefinitionsMap").Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiToolTypeDefinition>>("toolTypeDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiRecipeDefinition>>("recipeDefinitionsMap").Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiFactionDefinition>>("factionDefinitionsMap")
                .Clear();
            guiWrapperService
                .GetField<IGuiWrapperService, Dictionary<string, GuiEnvironmentEffectDefinition>>(
                    "environmentEffectDefinitionsMap").Clear();

            var methodRuntimeLoaded = AccessTools.Method(guiWrapperService.GetType(), "RuntimeLoaded");

            methodRuntimeLoaded.Invoke(guiWrapperService, new object[] {runtimeService.Runtime});
        }

        internal static void RecacheFeats()
        {
            var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();

            if (guiWrapperService == null)
            {
                return;
            }

            var methodRuntimeLoaded = AccessTools.Method(guiWrapperService.GetType(), "LoadFeatDefinitions");

            guiWrapperService.GetField<IGuiWrapperService, Dictionary<string, GuiFeatDefinition>>("featDefinitionsMap")
                .Clear();
            methodRuntimeLoaded.Invoke(guiWrapperService, System.Array.Empty<object>());
        }
    }
}
