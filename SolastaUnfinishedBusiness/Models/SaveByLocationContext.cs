using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Models;

internal static class SaveByLocationContext
{
    private const string LocationSaveFolder = @"CE\Location";
    private const string CampaignSaveFolder = @"CE\Campaign";
    private const string OfficialSaveFolder = @"CE\Official";
    private const string DefaultName = "Default";

    internal static readonly string DefaultSaveGameDirectory =
        Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");

    private static readonly string LocationSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, LocationSaveFolder);

    private static readonly string CampaignSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, CampaignSaveFolder);

    private static readonly string OfficialSaveGameDirectory =
        Path.Combine(DefaultSaveGameDirectory, OfficialSaveFolder);

    internal static CustomDropDown Dropdown { get; private set; }

    //TODO: is this still used?
    internal static bool UseLightEnumeration { get; private set; }

    private static List<SavePlace> GetAllSavePlaces()
    {
        // Find the most recently touched save file and select the correct location/campaign for that save
        return EnumerateDirectories(LocationSaveGameDirectory, LocationType.UserLocation)
            .Concat(EnumerateDirectories(CampaignSaveGameDirectory, LocationType.CustomCampaign))
            .Concat(EnumerateDirectories(OfficialSaveGameDirectory, LocationType.StandardCampaign))
            .Append(MostRecentFile(DefaultSaveGameDirectory, LocationType.Default))
            .Where(d => d.Available)
            .ToList();
    }

    internal static SavePlace GetMostRecentPlace()
    {
        return GetAllSavePlaces()
                   .Where(d => d.Date.HasValue)
                   .OrderByDescending(d => d.Date.Value)
                   .FirstOrDefault()
               ?? SavePlace.Default();
    }

    private static IEnumerable<SavePlace> EnumerateDirectories(string where, LocationType type)
    {
        return Directory.EnumerateDirectories(where)
            .Select(dir => MostRecentFile(dir, type));
    }

    private static SavePlace MostRecentFile(string dir, LocationType type)
    {
        var files = Directory.EnumerateFiles(dir, "*.sav").ToList();
        var place = new SavePlace
        {
            Name = type == LocationType.Default ? DefaultName : Path.GetFileName(dir),
            Path = dir,
            Count = files.Count,
            Date = files.Max(f => (DateTime?)File.GetLastWriteTimeUtc(f)),
            Type = type
        };
        Main.Log2($"[{place.Name}] type:{place.Type} count:{place.Count}, date:{place.Date}, path:{place.Path}");
        return place;
    }

    internal static void LateLoad()
    {
        if (!Main.Settings.EnableSaveByLocation)
        {
            return;
        }

        // Ensure folders exist
        Main.EnsureFolderExists(OfficialSaveGameDirectory);
        Main.EnsureFolderExists(LocationSaveGameDirectory);
        Main.EnsureFolderExists(CampaignSaveGameDirectory);

        // Find the most recently touched save file and select the correct location/campaign for that save
        var place = GetMostRecentPlace();
        Main.Log2($"MOST RECENT '{place.Path}' type:{place.Type}");

        ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>()
            .SetCampaignLocation(place.Type, place.Name);
    }

    internal static void LoadPanelOnBeginShowSaveByLocationBehavior(LoadPanel panel)
    {
        // The Load Panel is being shown.
        // 1) create/activate a dropdown next to the load save button
        // 2) populate with list of campaign and location names
        // 3) select the currently loaded campaign/location in the dropdown, or select 'Main Campaign' if none

        var guiDropdown = CreateOrActivateDropdown();

        // populate the dropdown
        guiDropdown.ClearOptions();
        guiDropdown.AddOptions(
            GetAllSavePlaces()
                .Where(p => p.Available)
                .OrderBy(p => p)
                .Select(LocationOptionData.Create)
                .Cast<TMP_Dropdown.OptionData>()
                .ToList());

        // Get the current campaign location and select it in the dropdown
        var selectedCampaign = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

        // ReSharper disable once InvocationIsSkipped
        Main.Log($"LoadPanel: selected={selectedCampaign.CampaignOrLocationName}, {selectedCampaign.LocationType}");

        var option = guiDropdown.Options
            .Cast<LocationOptionData>()
            .Select((o, i) => new { CampaignOrLocation = o.Title, o.LocationType, Index = i })
            .Where(opt => opt.LocationType == selectedCampaign.LocationType)
            .FirstOrDefault(o => o.CampaignOrLocation == selectedCampaign.CampaignOrLocationName);

        var newValue = option?.Index ?? 0;

        if (guiDropdown.Selected == newValue)
        {
            if (newValue == 0)
            {
                // I think we only want to do this on first open
                // or we refresh the list when we don't need to.
                // May need to change slightly.
                ValueChanged(guiDropdown.Options[0]);
            }
        }
        else
        {
            // This will trigger a ValueChanged
            guiDropdown.SetSelected(newValue);
        }

        return;

        void ValueChanged([NotNull] TMP_Dropdown.OptionData selected)
        {
            // update selected campaign
            var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

            if (selected is not LocationOptionData locationData)
            {
                return;
            }

            // ReSharper disable once InvocationIsSkipped
            Main.Log(
                $"ValueChanged: selected={locationData.LocationType}, {locationData.text}, {locationData.Title}");

            selectedCampaignService.SetCampaignLocation(locationData);

            // reload the save file list
            panel.StartCoroutine(panel.EnumerateSaveLines());
        }

        CustomDropDown CreateOrActivateDropdown()
        {
            if (Dropdown == null)
            {
                var root = panel.loadButton.transform.parent.parent;

                Dropdown = new CustomDropDown(CustomDropDown.MakeDropdown("LoadMenuDropDown", root),
                    CustomDropDown.MakeSelector("LoadMenuSelector", root));
                Dropdown.OnValueChangedHandler += ValueChanged;

                var rect = Dropdown.DropList.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(72f, 230f);
                rect.sizeDelta = new Vector2(320f, 30f);

                Dropdown.Selector.MarkGlobal();
                Dropdown.Selector.Unbind();

                rect = Dropdown.Selector.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(175f, 250f);
                rect.sizeDelta = new Vector2(350, 50f);

                var hLayout = Dropdown.Selector.GetComponent<HorizontalLayoutGroup>();
                hLayout.childControlWidth = false;

                var selectorTransform = Dropdown.Selector.transform;

                rect = selectorTransform.Find("Box").GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(245, 50f);

                selectorTransform.Find("CountLabel").GetComponent<RectTransform>().sizeDelta = new Vector2(55, 25);

                var obj = selectorTransform.Find("Binding");
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                obj.GetComponent<Image>().preserveAspect = true;
            }

            Dropdown.SetActive(true);

            return Dropdown;
        }
    }

    internal sealed class LocationOptionData : GuiDropdown.OptionDataAdvanced
    {
        internal string Title { get; private set; }
        internal LocationType LocationType { get; private set; }

        internal static LocationOptionData Create(SavePlace place)
        {
            return new LocationOptionData
            {
                LocationType = place.Type,
                text = GetTitle(place.Type, place.Name),
                Title = place.Name,
                TooltipContent = $"{place.Count} save{(place.Count == 1 ? "" : "s")}",
            };
        }

        private static string GetTitle(LocationType locationType, string title)
        {
            switch (locationType)
            {
                case LocationType.Default:
                    return title;
                case LocationType.StandardCampaign:
                    return title;
                case LocationType.CustomCampaign:
                    return title.Khaki();
                case LocationType.UserLocation:
                    return title.Orange();
                default:
                    Main.Error($"Unknown LocationType: {locationType}");
                    return title.Red();
            }
        }
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

    internal enum LocationType
    {
        Default,
        StandardCampaign,
        CustomCampaign,
        UserLocation
    }

    internal sealed class SelectedCampaignService : IService
    {
        internal string CampaignOrLocationName { get; private set; }
        internal string SaveGameDirectory { get; private set; }
        internal LocationType LocationType { get; private set; }

        internal void SetCampaignLocation([NotNull] LocationOptionData selected)
        {
            SetCampaignLocation(selected.LocationType, selected.Title);
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

    internal class SavePlace : IComparable<SavePlace>
    {
        public string Name;
        public string Path;
        public int Count;
        public DateTime? Date;
        public LocationType Type;

        public bool Available => Count > 0 || Type is LocationType.Default;

        public static SavePlace Default()
        {
            return new SavePlace
            {
                Path = DefaultSaveGameDirectory, Count = 0, Date = null, Type = LocationType.Default
            };
        }

        public int CompareTo(SavePlace other)
        {
            if (other == null) { return -1; }

            var type = Type.CompareTo(other.Type);
            return type != 0
                ? type
                : String.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}
