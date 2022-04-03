using System;
using System.Collections.Generic;
using System.Linq;
using ModKit.Utility;
using SolastaCommunityExpansion.DataViewer;
using UnityEngine;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class GameServicesDisplay
    {
#if DEBUG

        private static readonly Dictionary<string, Func<object>> TARGET_LIST = new()
        {
            { "None", null },
            { "IGameCampaignService", () => ServiceRepository.GetService<IGameCampaignService>() },
            { "IGameFactionService", () => ServiceRepository.GetService<IGameFactionService>() },
            { "IGameLocationActionService", () => ServiceRepository.GetService<IGameLocationActionService>() },
            { "IGameLocationAudioService", () => ServiceRepository.GetService<IGameLocationAudioService>() },
            { "IGameLocationBanterService", () => ServiceRepository.GetService<IGameLocationBanterService>() },
            { "IGameLocationBattleService", () => ServiceRepository.GetService<IGameLocationBattleService>() },
            { "IGameLocationCharacterService", () => ServiceRepository.GetService<IGameLocationCharacterService>() },
            { "IGameLocationEnvironmentService", () => ServiceRepository.GetService<IGameLocationEnvironmentService>() },
            { "IGameLocationGadgetService", () => ServiceRepository.GetService<IGameLocationGadgetService>() },
            { "IGameLocationItemService", () => ServiceRepository.GetService<IGameLocationItemService>() },
            { "IGameLocationMapService", () => ServiceRepository.GetService<IGameLocationMapService>() },
            { "IGameLocationPathfindingService", () => ServiceRepository.GetService<IGameLocationPathfindingService>() },
            { "IGameLocationPositioningService", () => ServiceRepository.GetService<IGameLocationPositioningService>() },
            { "IGameLocationSelectionService", () => ServiceRepository.GetService<IGameLocationSelectionService>() },
            { "IGameLocationService", () => ServiceRepository.GetService<IGameLocationService>() },
            { "IGameLocationTargetingService", () => ServiceRepository.GetService<IGameLocationTargetingService>() },
            { "IGameLocationTimelineService", () => ServiceRepository.GetService<IGameLocationTimelineService>() },
            { "IGameLocationVisibilityService", () => ServiceRepository.GetService<IGameLocationVisibilityService>() },
            { "IGameLoreService", () => ServiceRepository.GetService<IGameLoreService>() },
            { "IGameQuestService", () => ServiceRepository.GetService<IGameQuestService>() },
            { "IGameRestingService", () => ServiceRepository.GetService<IGameRestingService>() },
            { "IGameScavengerService", () => ServiceRepository.GetService<IGameScavengerService>() },
            { "IGameSerializationService", () => ServiceRepository.GetService<IGameSerializationService>() },
            { "IGameService", () => ServiceRepository.GetService<IGameService>() },
            { "IGameSettingsService", () => ServiceRepository.GetService<IGameSettingsService>() },
            { "IGameVariableService", () => ServiceRepository.GetService<IGameVariableService>() }
        };

        private static readonly string[] _targetNames = TARGET_LIST.Keys.ToArray();

        private static ReflectionTreeView _treeView;

        private static void ResetTree()
        {
            if (_treeView == null)
            {
                _treeView = new ReflectionTreeView();
            }

            Func<object> getTarget = TARGET_LIST[_targetNames[Main.Settings.SelectedRawDataType]];
            if (getTarget == null)
            {
                _treeView.Clear();
            }
            else
            {
                _treeView.SetRoot(getTarget());
            }
        }
        public static void DisplayGameServices()
        {
            try
            {
                if (_treeView == null)
                {
                    ResetTree();
                }

                // target selection
                GUIHelper.SelectionGrid(ref Main.Settings.SelectedRawDataType, _targetNames, 5, () => ResetTree());

                // tree view
                if (Main.Settings.SelectedRawDataType != 0)
                {
                    GUILayout.Space(10f);

                    _treeView.OnGUI();
                }
            }
            catch
            {
                Main.Settings.SelectedRawDataType = 0;
                _treeView.Clear();
            }
        }
#endif
    }
}
