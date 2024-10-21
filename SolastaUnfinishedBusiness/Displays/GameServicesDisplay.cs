using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.DataViewer;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class GameServicesDisplay
{
    private static int _selectedRawDataType;
    internal static int MaxRows = 20;
    internal static int MaxSearchDepth = 3;

    private static readonly Dictionary<string, Func<object>> TargetList = new()
    {
        { "None", null },
        {
            "Heroes", () => ServiceRepository.GetService<IGameLocationCharacterService>()?.PartyCharacters
                ?.Select(ch => ch.RulesetCharacter)
        },
        { "Action Service", ServiceRepository.GetService<IGameLocationActionService> },
        { "Audio Service", ServiceRepository.GetService<IGameLocationAudioService> },
        { "Banter Service", ServiceRepository.GetService<IGameLocationBanterService> },
        { "Battle Service", ServiceRepository.GetService<IGameLocationBattleService> },
        { "Building Command Service", ServiceRepository.GetService<IHeroBuildingCommandService> },
        { "Building Service", ServiceRepository.GetService<ICharacterBuildingService> },
        { "Camera Service", ServiceRepository.GetService<ICameraService> },
        { "Campaign Service", ServiceRepository.GetService<IGameCampaignService> },
        { "Character Service", ServiceRepository.GetService<IGameLocationCharacterService> },
        { "Environment Service", ServiceRepository.GetService<IGameLocationEnvironmentService> },
        { "Faction Service", ServiceRepository.GetService<IGameFactionService> },
        { "Gadget Service", ServiceRepository.GetService<IGameLocationGadgetService> },
        { "Game Service", ServiceRepository.GetService<IGameService> },
        { "Item Service", ServiceRepository.GetService<IGameLocationItemService> },
        { "Location Service", ServiceRepository.GetService<IGameLocationService> },
        { "Lore Service", ServiceRepository.GetService<IGameLoreService> },
        { "Map Service", ServiceRepository.GetService<IGameLocationMapService> },
        { "Networking Service", ServiceRepository.GetService<INetworkingService> },
        { "Pathfinding Service", ServiceRepository.GetService<IGameLocationPathfindingService> },
        { "Platform Service", ServiceRepository.GetService<IGamingPlatformService> },
        { "Pool Service", ServiceRepository.GetService<ICharacterPoolService> },
        { "Positioning Service", ServiceRepository.GetService<IGameLocationPositioningService> },
        { "Quest Service", ServiceRepository.GetService<IGameQuestService> },
        { "Resting Service", ServiceRepository.GetService<IGameRestingService> },
        { "Scavenger Service", ServiceRepository.GetService<IGameScavengerService> },
        { "Selection Service", ServiceRepository.GetService<IGameLocationSelectionService> },
        { "Serialization Service", ServiceRepository.GetService<IGameSerializationService> },
        { "Settings Service", ServiceRepository.GetService<IGameSettingsService> },
        { "Targeting Service", ServiceRepository.GetService<IGameLocationTargetingService> },
        { "Timeline Service", ServiceRepository.GetService<IGameLocationTimelineService> },
        { "Variable Service", ServiceRepository.GetService<IGameVariableService> },
        { "Visibility Service", ServiceRepository.GetService<IGameLocationVisibilityService> }
    };

    private static readonly string[] TargetNames = TargetList.Keys.ToArray();

    private static ReflectionTreeView TreeView { get; } = new();

    private static void ResetTree()
    {
        var getTarget = TargetList[TargetNames[_selectedRawDataType]];

        if (getTarget == null)
        {
            TreeView.Clear();
        }
        else
        {
            TreeView.SetRoot(getTarget());
        }
    }

    internal static void DisplayGameServices()
    {
        try
        {
            if (TreeView == null)
            {
                ResetTree();
            }

            UI.ActionSelectionGrid(ref _selectedRawDataType, TargetNames, 5, _ =>
            {
                ResetTree();
            });

            // tree view
            if (_selectedRawDataType == 0)
            {
                return;
            }

            GUILayout.Space(10f);

            TreeView?.OnGUI();
        }
        catch
        {
            _selectedRawDataType = 0;
            TreeView?.Clear();
        }
    }
}
