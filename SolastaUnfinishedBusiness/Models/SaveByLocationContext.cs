using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Models;

internal static class SaveByLocationContext
{
    private const string LocationSaveFolder = @"CE\Location";
    private const string CampaignSaveFolder = @"CE\Campaign";
    private const string OfficialSaveFolder = @"CE\Official";

    internal static readonly string DefaultSaveGameDirectory =
        Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");

    private static readonly string LocationSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, LocationSaveFolder);

    private static readonly string CampaignSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, CampaignSaveFolder);

    private static readonly string OfficialSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, OfficialSaveFolder);

    private static List<CampaignDefinition> _allOfficialCampaigns;
    private static List<UserLocation> _allUserLocations;
    private static List<UserCampaign> _allUserCampaigns;

    internal static GameObject Dropdown { get; private set; }

    internal static bool UseLightEnumeration { get; private set; }

    private static IEnumerable<CampaignDefinition> AllOfficialCampaigns
    {
        get
        {
            if (_allOfficialCampaigns != null)
            {
                return _allOfficialCampaigns;
            }

            _allOfficialCampaigns = new List<CampaignDefinition>();

            var allElements = DatabaseRepository.GetDatabase<CampaignDefinition>().GetAllElements();

            foreach (var campaign in allElements)
            {
                if (campaign.GuiPresentation.Hidden || campaign.IsUserCampaign || campaign.EditorOnly)
                {
                    continue;
                }

                _allOfficialCampaigns.Add(campaign);
            }

            return _allOfficialCampaigns;
        }
    }

    private static IEnumerable<UserLocation> AllUserLocations
    {
        get
        {
            if (_allUserLocations != null)
            {
                return _allUserLocations;
            }

            var userLocationPoolService =
                (UserLocationPoolManager)ServiceRepository.GetService<IUserLocationPoolService>();

            if (!userLocationPoolService.Enumerated)
            {
                UseLightEnumeration = true;
                userLocationPoolService.EnumeratePool(out _, new List<string>());
                userLocationPoolService.enumerated = false;
                UseLightEnumeration = false;
            }

            _allUserLocations = userLocationPoolService.AllLocations;

            return _allUserLocations;
        }
    }

    private static IEnumerable<UserCampaign> AllUserCampaigns
    {
        get
        {
            if (_allUserCampaigns != null)
            {
                return _allUserCampaigns;
            }

            var userCampaignPoolService =
                (UserCampaignPoolManager)ServiceRepository.GetService<IUserCampaignPoolService>();

            if (!userCampaignPoolService.Enumerated)
            {
                UseLightEnumeration = true;
                userCampaignPoolService.EnumeratePool(out _, new List<string>());
                userCampaignPoolService.enumerated = false;
                UseLightEnumeration = false;
            }

            _allUserCampaigns = userCampaignPoolService.AllCampaigns;

            return _allUserCampaigns;
        }
    }

    internal static void LateLoad()
    {
        if (!Main.Settings.EnableSaveByLocation)
        {
            return;
        }

        // Ensure folders exist
        Directory.CreateDirectory(OfficialSaveGameDirectory);
        Directory.CreateDirectory(LocationSaveGameDirectory);
        Directory.CreateDirectory(CampaignSaveGameDirectory);

        // Find the most recently touched save file and select the correct location/campaign for that save
        var mostRecent = Directory.EnumerateDirectories(LocationSaveGameDirectory)
            .Select(d => new
            {
                Path = d,
                LastWriteTime =
                    Directory.EnumerateFiles(d, "*.sav").Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                LocationType = LocationType.UserLocation
            })
            .Concat(
                Directory.EnumerateDirectories(CampaignSaveGameDirectory)
                    .Select(d => new
                    {
                        Path = d,
                        LastWriteTime =
                            Directory.EnumerateFiles(d, "*.sav")
                                .Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                        LocationType = LocationType.CustomCampaign
                    })
            )
            .Concat(
                Directory.EnumerateDirectories(OfficialSaveGameDirectory)
                    .Select(d => new
                    {
                        Path = d,
                        LastWriteTime =
                            Directory.EnumerateFiles(d, "*.sav")
                                .Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                        LocationType = LocationType.StandardCampaign
                    })
            )
            .Concat(
                Enumerable.Repeat(
                    new
                    {
                        Path = DefaultSaveGameDirectory,
                        LastWriteTime =
                            Directory.EnumerateFiles(DefaultSaveGameDirectory, "*.sav")
                                .Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
                        LocationType = LocationType.Default
                    }
                    , 1)
            )
            .Where(d => d.LastWriteTime.HasValue)
            .OrderByDescending(d => d.LastWriteTime)
            .FirstOrDefault();

        var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

        if (mostRecent == null)
        {
            return;
        }

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"Most recent folder={mostRecent.Path}");

        selectedCampaignService.SetCampaignLocation(mostRecent.LocationType, Path.GetFileName(mostRecent.Path));
    }

    private static int SaveFileCount(LocationType locationType, string folder)
    {
        switch (locationType)
        {
            case LocationType.UserLocation:
            {
                var saveFolder = Path.Combine(LocationSaveGameDirectory, folder);

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            case LocationType.CustomCampaign:
            {
                var saveFolder = Path.Combine(CampaignSaveGameDirectory, folder);

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            case LocationType.StandardCampaign:
            {
                var saveFolder = Path.Combine(OfficialSaveGameDirectory, folder);

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            case LocationType.Default:
            {
                var saveFolder = DefaultSaveGameDirectory;

                return Directory.Exists(saveFolder) ? Directory.EnumerateFiles(saveFolder, "*.sav").Count() : 0;
            }
            default:
                Main.Error($"Unknown LocationType: {locationType}");
                break;
        }

        return 0;
    }

    internal static bool LoadPanelOnBeginShowSaveByLocationBehavior(LoadPanel __instance)
    {
        // From OnBeginShow
        __instance.StartAllModifiers(true);
        __instance.loadSaveLinesScrollview.normalizedPosition = new Vector2(0.0f, 1f);
        __instance.Reset();

        // The Load Panel is being shown.
        // 1) create/activate a dropdown next to the load save button
        // 2) populate with list of campaign and location names
        // 3) select the currently loaded campaign/location in the dropdown, or select 'Main Campaign' if none

        var guiDropdown = CreateOrActivateDropdown();

        // populate the dropdown
        guiDropdown.ClearOptions();

        var officialCampaigns = AllOfficialCampaigns
            .Select(c => new
            {
                LocationType = LocationType.StandardCampaign, Title = Gui.Localize(c.GuiPresentation.Title)
            })
            .OrderBy(c => c.Title)
            .ToList();

        // add them together - each block sorted - can we have separators?
        var userContentList =
            AllUserCampaigns
                .Select(l => new { LocationType = LocationType.CustomCampaign, l.Title })
                .OrderBy(l => l.Title)
                .Concat(AllUserLocations
                    .Select(l => new { LocationType = LocationType.UserLocation, l.Title })
                    .OrderBy(l => l.Title)
                )
                .ToList();

        guiDropdown.AddOptions(
            Enumerable.Repeat(new { LocationType = LocationType.Default, Title = "Default" }, 1)
                .Union(officialCampaigns)
                .Union(userContentList)
                .Select(opt => new
                {
                    opt.LocationType, opt.Title, SaveFileCount = SaveFileCount(opt.LocationType, opt.Title)
                })
                .Select(opt => new LocationOptionData
                {
                    LocationType = opt.LocationType,
                    text = GetTitle(opt.LocationType, opt.Title),
                    CampaignOrLocation = opt.Title,
                    TooltipContent = $"{opt.SaveFileCount} save{(opt.SaveFileCount == 1 ? "" : "s")}",
                    ShowInDropdown = opt.SaveFileCount > 0 || opt.LocationType is LocationType.Default
                })
                .Where(opt => opt.ShowInDropdown) // Only show locations that have saves
                .Cast<TMP_Dropdown.OptionData>()
                .ToList());

        // Get the current campaign location and select it in the dropdown
        var selectedCampaign = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"LoadPanel: selected={selectedCampaign.CampaignOrLocationName}, {selectedCampaign.LocationType}");

        var option = guiDropdown.options
            .Cast<LocationOptionData>()
            .Select((o, i) => new { o.CampaignOrLocation, o.LocationType, Index = i })
            .Where(opt => opt.LocationType == selectedCampaign.LocationType)
            .FirstOrDefault(o => o.CampaignOrLocation == selectedCampaign.CampaignOrLocationName);

        var newValue = option?.Index ?? 0;

        if (guiDropdown.value == newValue)
        {
            if (newValue == 0)
            {
                // I think we only want to do this on first open
                // or we refresh the list when we don't need to.
                // May need to change slightly.
                ValueChanged(guiDropdown);
            }
        }
        else
        {
            // This will trigger a ValueChanged
            guiDropdown.value = newValue;
        }

        return false;

        string GetTitle(LocationType locationType, string title)
        {
            switch (locationType)
            {
                default:
                    Main.Error($"Unknown LocationType: {locationType}");
                    return title.Red();
                case LocationType.Default:
                    return title;
                case LocationType.StandardCampaign:
                    return title;
                case LocationType.CustomCampaign:
                    return title.Khaki();
                case LocationType.UserLocation:
                    return title.Orange();
            }
        }

        void ValueChanged([NotNull] GuiDropdown dropdown)
        {
            // update selected campaign
            var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

            if (dropdown.options.Skip(dropdown.value).FirstOrDefault() is not LocationOptionData selected)
            {
                return;
            }

            // ReSharper disable once InvocationIsSkipped
            Main.Log(
                $"ValueChanged: {dropdown.value}, selected={selected.LocationType}, {selected.text}, {selected.CampaignOrLocation}");

            selectedCampaignService.SetCampaignLocation(selected);

            dropdown.UpdateTooltip();

            // From OnBeginShow

            // reload the save file list
            __instance.StartCoroutine(__instance.EnumerateSaveLines());
        }

        GuiDropdown CreateOrActivateDropdown()
        {
            if (Dropdown == null)
            {
                // ReSharper disable once Unity.UnknownResource
                var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");

                Dropdown = Object.Instantiate(dropdownPrefab, __instance.loadButton.transform.parent.parent);
                Dropdown.name = "LoadMenuDropDown";

                var dropdown = Dropdown.GetComponent<GuiDropdown>();
                var dropDownRect = Dropdown.GetComponent<RectTransform>();

                dropdown.onValueChanged.AddListener(delegate { ValueChanged(dropdown); });
                dropDownRect.anchoredPosition = new Vector2(72f, 230f);
                dropDownRect.sizeDelta = new Vector2(320f, 30f);
            }
            else
            {
                Dropdown.SetActive(true);
            }

            return Dropdown.GetComponent<GuiDropdown>();
        }
    }

    internal sealed class LocationOptionData : GuiDropdown.OptionDataAdvanced
    {
        internal string CampaignOrLocation { get; set; }
        internal LocationType LocationType { get; set; }
        internal bool ShowInDropdown { get; set; }
    }

    internal static class ServiceRepositoryEx
    {
        [NotNull]
        internal static T GetOrCreateService<T>() where T : class, IService, new()
        {
            var repo = ServiceRepository.GetService<T>();

            if (repo != null)
            {
                return repo;
            }

            repo = new T();
            ServiceRepository.AddService(repo);

            return repo;
        }
    }

    private interface ISelectedCampaignService : IService
    {
        // string CampaignOrLocationName { get; }
        // LocationType LocationType { get; }
        // string SaveGameDirectory { get; }
        // void SetCampaignLocation(string campaign, string location);
    }

    internal enum LocationType
    {
        Default,
        StandardCampaign,
        UserLocation,
        CustomCampaign
    }

    internal sealed class SelectedCampaignService : ISelectedCampaignService
    {
        internal string CampaignOrLocationName { get; private set; }
        internal string SaveGameDirectory { get; private set; }
        internal LocationType LocationType { get; private set; }

        internal void SetCampaignLocation([NotNull] LocationOptionData selected)
        {
            SetCampaignLocation(selected.LocationType, selected.CampaignOrLocation);
        }

        internal void SetCampaignLocation(LocationType type, string name)
        {
            var baseFolder = type switch
            {
                LocationType.Default => DefaultSaveGameDirectory,
                LocationType.StandardCampaign => OfficialSaveGameDirectory,
                LocationType.UserLocation => LocationSaveGameDirectory,
                LocationType.CustomCampaign => CampaignSaveGameDirectory,
                _ => DefaultSaveGameDirectory
            };

            LocationType = type;
            SaveGameDirectory = type == LocationType.Default
                ? baseFolder
                : Path.Combine(baseFolder, name.Trim());
            CampaignOrLocationName = name;

            // ReSharper disable once InvocationIsSkipped
            Main.Log(
                $"SelectedCampaignService: Type='{LocationType}', Name='{CampaignOrLocationName}', Folder='{SaveGameDirectory}'");
        }
    }
}
