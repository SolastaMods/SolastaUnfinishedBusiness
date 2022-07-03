using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using static GuiDropdown;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;
using static TMPro.TMP_Dropdown;
using Object = UnityEngine.Object;

namespace SolastaCommunityExpansion.Patches.Tools.SaveByLocation;

[HarmonyPatch(typeof(LoadPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class LoadPanel_OnBeginShow
{
    internal static GameObject Dropdown { get; private set; }

    public static bool Prefix(LoadPanel __instance, ScrollRect ___loadSaveLinesScrollview,
        [HarmonyArgument("instant")] bool _ = false)
    {
        if (!Main.Settings.EnableSaveByLocation)
        {
            if (Dropdown != null)
            {
                Dropdown.SetActive(false);
            }

            return true;
        }

        // From OnBeginShow
        __instance.StartAllModifiers(true);
        ___loadSaveLinesScrollview.normalizedPosition = new Vector2(0.0f, 1f);
        AccessTools
            .Method(typeof(LoadPanel), "Reset")
            .Invoke(__instance, Array.Empty<object>());

        // The Load Panel is being shown.
        // 1) create/activate a dropdown next to the load save button
        // 2) populate with list of campaign and location names
        // 3) select the currently loaded campaign/location in the dropdown, or select 'Main Campaign' if none

        var guiDropdown = CreateOrActivateDropdown();

        // get all user locations
        var userLocationPoolService = ServiceRepository.GetService<IUserLocationPoolService>();
        userLocationPoolService.EnumeratePool(out var _, new List<string>());
        var allLocations = userLocationPoolService.AllLocations;

        // get all user campaigns
        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();
        userCampaignPoolService.EnumeratePool(out var _, new List<string>());
        var allCampaigns = userCampaignPoolService.AllCampaigns;

        // populate the dropdown
        guiDropdown.ClearOptions();

        // add them together - each block sorted - can we have separators?
        var userContentList =
            allCampaigns
                .Select(l => new {LocationType = LocationType.CustomCampaign, l.Title})
                .OrderBy(l => l.Title)
                .Concat(allLocations
                    .Select(l => new {LocationType = LocationType.UserLocation, l.Title})
                    .OrderBy(l => l.Title)
                )
                .ToList();

        guiDropdown.AddOptions(
            Enumerable.Repeat(new {LocationType = LocationType.StandardCampaign, Title = "Standard campaigns"}, 1)
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
                    ShowInDropdown = opt.SaveFileCount > 0 || opt.LocationType == LocationType.StandardCampaign
                })
                .Where(opt => opt.ShowInDropdown) // Only show locations that have saves
                .Cast<OptionData>()
                .ToList());

        // Get the current campaign location and select it in the dropdown
        var selectedCampaign = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

        Main.Log($"LoadPanel: selected={selectedCampaign.CampaignOrLocationName}, {selectedCampaign.LocationType}");

        var option = guiDropdown.options
            .Cast<LocationOptionData>()
            .Select((o, i) => new {o.CampaignOrLocation, o.LocationType, Index = i})
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
                case LocationType.StandardCampaign:
                    return title;
                case LocationType.CustomCampaign:
                    return title.Yellow();
                case LocationType.UserLocation:
                    return title.Orange();
            }
        }

        void ValueChanged(GuiDropdown dropdown)
        {
            // update selected campaign
            var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

            var selected = dropdown.options.Skip(dropdown.value).FirstOrDefault() as LocationOptionData;

            Main.Log(
                $"ValueChanged: {dropdown.value}, selected={selected.LocationType}, {selected.text}, {selected.CampaignOrLocation}");

            switch (selected.LocationType)
            {
                default:
                    Main.Error($"Unknown LocationType: {selected.LocationType}");
                    break;
                case LocationType.StandardCampaign:
                    selectedCampaignService.SetStandardCampaignLocation();
                    break;
                case LocationType.UserLocation: // location (campaign=USER_CAMPAIGN + location)
                    selectedCampaignService.SetCampaignLocation(USER_CAMPAIGN, selected.CampaignOrLocation);
                    break;
                case LocationType.CustomCampaign: // campaign
                    selectedCampaignService.SetCampaignLocation(selected.CampaignOrLocation, string.Empty);
                    break;
            }

            dropdown.UpdateTooltip();

            // From OnBeginShow

            // reload the save file list
            var method = AccessTools.Method(typeof(LoadPanel), "EnumerateSaveLines");
            __instance.StartCoroutine((IEnumerator)method.Invoke(__instance, Array.Empty<object>()));
        }

        GuiDropdown CreateOrActivateDropdown()
        {
            GuiDropdown dd;

            // create a drop down singleton
            if (Dropdown == null)
            {
                var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");

                Dropdown = Object.Instantiate(dropdownPrefab);
                Dropdown.name = "LoadMenuDropDown";

                dd = Dropdown.GetComponent<GuiDropdown>();
                dd.onValueChanged.AddListener(delegate { ValueChanged(dd); });

                var buttonBar = __instance.gameObject
                    .GetComponentsInChildren<RectTransform>()
                    .SingleOrDefault(c => c.gameObject.name == "ButtonsBar")?.gameObject;

                if (buttonBar == null)
                {
                    return dd;
                }

                var horizontalLayoutGroup = buttonBar.GetComponent<HorizontalLayoutGroup>();

                if (horizontalLayoutGroup == null)
                {
                    return dd;
                }

                Dropdown.transform.SetParent(horizontalLayoutGroup.transform, false);

                horizontalLayoutGroup.childForceExpandWidth = true;
                horizontalLayoutGroup.childForceExpandHeight = true;
                horizontalLayoutGroup.childControlWidth = true;
                horizontalLayoutGroup.childControlHeight = true;
                horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;

                var dropDownLayout = dd.gameObject.AddComponent<LayoutElement>();
                // any large flexible width will do
                dropDownLayout.flexibleWidth = 3;
            }
            else
            {
                Dropdown.SetActive(true);
                dd = Dropdown.GetComponent<GuiDropdown>();
            }

            return dd;
        }
    }
}

internal class LocationOptionData : OptionDataAdvanced
{
    public string CampaignOrLocation { get; set; }
    public LocationType LocationType { get; set; }
    public bool ShowInDropdown { get; set; }
}
