using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.ModKit;
using SolastaCommunityExpansion.DataViewer;
using UnityEngine;

namespace SolastaCommunityExpansion.Displays;

internal static class GameServicesDisplay
{
    private static readonly Dictionary<string, Func<object>> TARGET_LIST = new()
    {
        {"None", null},
        {"IGamingPlatformService", () => ServiceRepository.GetService<IGamingPlatformService>()},
        {"ICharacterBuildingService", () => ServiceRepository.GetService<ICharacterBuildingService>()},
        {"ICharacterPoolService", () => ServiceRepository.GetService<ICharacterPoolService>()},
        {"IGameCampaignService", () => ServiceRepository.GetService<IGameCampaignService>()},
        {"IGameFactionService", () => ServiceRepository.GetService<IGameFactionService>()},
        {"IHeroBuildingCommandService", () => ServiceRepository.GetService<IHeroBuildingCommandService>()},
        {"INetworkingService", () => ServiceRepository.GetService<INetworkingService>()},
        {"IGameLocationActionService", () => ServiceRepository.GetService<IGameLocationActionService>()},
        {"IGameLocationAudioService", () => ServiceRepository.GetService<IGameLocationAudioService>()},
        {"IGameLocationBanterService", () => ServiceRepository.GetService<IGameLocationBanterService>()},
        {"IGameLocationBattleService", () => ServiceRepository.GetService<IGameLocationBattleService>()},
        {"IGameLocationCharacterService", () => ServiceRepository.GetService<IGameLocationCharacterService>()},
        {"IGameLocationEnvironmentService", () => ServiceRepository.GetService<IGameLocationEnvironmentService>()},
        {"IGameLocationGadgetService", () => ServiceRepository.GetService<IGameLocationGadgetService>()},
        {"IGameLocationItemService", () => ServiceRepository.GetService<IGameLocationItemService>()},
        {"IGameLocationMapService", () => ServiceRepository.GetService<IGameLocationMapService>()},
        {"IGameLocationPathfindingService", () => ServiceRepository.GetService<IGameLocationPathfindingService>()},
        {"IGameLocationPositioningService", () => ServiceRepository.GetService<IGameLocationPositioningService>()},
        {"IGameLocationSelectionService", () => ServiceRepository.GetService<IGameLocationSelectionService>()},
        {"IGameLocationService", () => ServiceRepository.GetService<IGameLocationService>()},
        {"IGameLocationTargetingService", () => ServiceRepository.GetService<IGameLocationTargetingService>()},
        {"IGameLocationTimelineService", () => ServiceRepository.GetService<IGameLocationTimelineService>()},
        {"IGameLocationVisibilityService", () => ServiceRepository.GetService<IGameLocationVisibilityService>()},
        {"IGameLoreService", () => ServiceRepository.GetService<IGameLoreService>()},
        {"IGameQuestService", () => ServiceRepository.GetService<IGameQuestService>()},
        {"IGameRestingService", () => ServiceRepository.GetService<IGameRestingService>()},
        {"IGameScavengerService", () => ServiceRepository.GetService<IGameScavengerService>()},
        {"IGameSerializationService", () => ServiceRepository.GetService<IGameSerializationService>()},
        {"IGameService", () => ServiceRepository.GetService<IGameService>()},
        {"IGameSettingsService", () => ServiceRepository.GetService<IGameSettingsService>()},
        {"IGameVariableService", () => ServiceRepository.GetService<IGameVariableService>()}
    };

    private static readonly string[] targetNames = TARGET_LIST.Keys.ToArray();

    private static ReflectionTreeView TreeView { get; } = new();

    private static void ResetTree()
    {
        var getTarget = TARGET_LIST[targetNames[Main.Settings.SelectedRawDataType]];

        if (getTarget == null)
        {
            TreeView.Clear();
        }
        else
        {
            TreeView.SetRoot(getTarget());
        }
    }

    public static void DisplayGameServices()
    {
        try
        {
            if (TreeView == null)
            {
                ResetTree();
            }

            // target selection
            GUIHelper.SelectionGrid(ref Main.Settings.SelectedRawDataType, targetNames, 8, () => ResetTree());

            // tree view
            if (Main.Settings.SelectedRawDataType != 0)
            {
                GUILayout.Space(10f);

                TreeView.OnGUI();
            }
        }
        catch
        {
            Main.Settings.SelectedRawDataType = 0;
            TreeView.Clear();
        }
    }
}
