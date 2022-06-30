namespace SolastaCommunityExpansion.Models;

internal static class GuiWrapperContext
{
    internal static void Recache()
    {
        var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>();
        var runtimeService = ServiceRepository.GetService<IRuntimeService>();

        if (guiWrapperService is not GuiWrapperManager guiWrapperManager || runtimeService?.Runtime == null)
        {
            return;
        }

        guiWrapperManager.classDefinitionsMap.Clear();
        guiWrapperManager.featDefinitionsMap.Clear();
        guiWrapperManager.raceDefinitionsMap.Clear();
        guiWrapperManager.monsterDefinitionsMap.Clear();
        guiWrapperManager.merchantDefinitionsMap.Clear();
        guiWrapperManager.itemDefinitionsMap.Clear();
        guiWrapperManager.spellDefinitionsMap.Clear();
        guiWrapperManager.effectProxyDefinitionsMap.Clear();
        guiWrapperManager.powerDefinitionsMap.Clear();
        guiWrapperManager.toolTypeDefinitionsMap.Clear();
        guiWrapperManager.recipeDefinitionsMap.Clear();
        guiWrapperManager.factionDefinitionsMap.Clear();
        guiWrapperManager.environmentEffectDefinitionsMap.Clear();

        guiWrapperManager.RuntimeLoaded(runtimeService.Runtime);
    }

    internal static void RecacheFeats()
    {
        var guiWrapperService = ServiceRepository.GetService<IGuiWrapperService>() as GuiWrapperManager;

        if (guiWrapperService is not GuiWrapperManager guiWrapperManager)
        {
            return;
        }

        guiWrapperManager.featDefinitionsMap.Clear();
        guiWrapperManager.LoadFeatDefinitions();
    }
}
