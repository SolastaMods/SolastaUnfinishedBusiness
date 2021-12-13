using HarmonyLib;
using ModKit;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;
using static TMPro.TMP_Dropdown;

namespace SolastaCommunityExpansion.Patches.SaveByLocation
{
    [HarmonyPatch(typeof(LoadPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LoadPanel_OnBeginShow
    {
        internal static GameObject Dropdown { get; private set; }

        public static void Postfix(LoadPanel __instance, [HarmonyArgument("instant")] bool _ = false)
        {
            var canvas = __instance.gameObject;

            if (!Main.Settings.EnableSaveByLocation)
            {
                if (Dropdown != null)
                {
                    Dropdown.SetActive(false);
                }

                return;
            }

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
                    .Select(l => new { LocationType = LocationType.CustomCampaign, l.Title })
                    .OrderBy(l => l.Title)
                .Concat(allLocations
                    .Select(l => new { LocationType = LocationType.UserLocation, l.Title })
                    .OrderBy(l => l.Title)
                )
                .ToList();

            guiDropdown.AddOptions(
                Enumerable.Repeat(new { LocationType = LocationType.MainCampaign, Title = "Main campaign" }, 1)
                .Union(userContentList)
                .Select(opt => new LocationOptionData
                {
                    text = GetTitle(opt.LocationType, opt.Title),
                    CampaignOrLocation = opt.Title,
                    image = GetSprite(opt.LocationType, opt.Title), // TODO
                    LocationType = opt.LocationType,
                    HasSaves = HasSaves(opt.LocationType, opt.Title)
                })
                .Where(opt => opt.HasSaves) // Only show locations that have saves
                .Cast<OptionData>()
                .ToList());

            // Get the current campaign location and select it in the dropdown
            var selectedCampaign = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

            Main.Log($"LoadPanel: selected={selectedCampaign.CampaignOrLocationName}, {selectedCampaign.LocationType}");

            var option = guiDropdown.options
                .Cast<LocationOptionData>()
                .Select((o, i) => new { o.CampaignOrLocation, o.LocationType, Index = i })
                .Where(opt => opt.LocationType == selectedCampaign.LocationType)
                .FirstOrDefault(o => o.CampaignOrLocation == selectedCampaign.CampaignOrLocationName);

            foreach (var o in guiDropdown.options.Cast<LocationOptionData>().Select((od, i) => new {od, i}))
            {
                Main.Log($"{o.od.LocationType}, {o.od.CampaignOrLocation}, {o.i}");
            }

            guiDropdown.value = option?.Index ?? 0;

            ValueChanged(guiDropdown);

#pragma warning disable S1172 // Unused method parameters should be removed
            Sprite GetSprite(LocationType locationType, string title)
            {
                // TODO: get suitable sprites - anyone?
                return null;
            }
#pragma warning restore S1172 // Unused method parameters should be removed

            string GetTitle(LocationType locationType, string title)
            {
                switch (locationType)
                {
                    default:
                    case LocationType.MainCampaign:
                        return title;
                    case LocationType.CustomCampaign:
                        return title.yellow();
                    case LocationType.UserLocation:
                        return title.orange();
                }
            }

            void ValueChanged(GuiDropdown dropdown)
            {
                // update selected campaign
                var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

                var selected = dropdown.options.Skip(dropdown.value).FirstOrDefault() as LocationOptionData;

                switch (selected.LocationType)
                {
                    case LocationType.MainCampaign:
                        selectedCampaignService.SetCampaignLocation(MAIN_CAMPAIGN, string.Empty);
                        break;
                    case LocationType.UserLocation: // location (campaign=USER_CAMPAIGN + location)
                        selectedCampaignService.SetCampaignLocation(USER_CAMPAIGN, selected.CampaignOrLocation);
                        break;
                    case LocationType.CustomCampaign: // campaign
                        selectedCampaignService.SetCampaignLocation(selected.CampaignOrLocation, string.Empty);
                        break;
                }

                // reload the save list
                var method = AccessTools.Method(typeof(LoadPanel), "EnumerateSaveLines");
                __instance.StartCoroutine((IEnumerator)method.Invoke(__instance, null));

                // reset the load button
                method = AccessTools.Method(typeof(LoadPanel), "Reset");
                method.Invoke(__instance, null);
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

                    var buttonBar = canvas.GetComponentsInChildren<RectTransform>().SingleOrDefault(c => c.gameObject.name == "ButtonsBar")?.gameObject;

                    if (buttonBar != null)
                    {
                        var horizontalLayoutGroup = buttonBar.GetComponent<HorizontalLayoutGroup>();

                        if (horizontalLayoutGroup != null)
                        {
                            Dropdown.transform.SetParent(horizontalLayoutGroup.transform, false);
                        }
                    }
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

    internal class LocationOptionData : OptionData
    {
        public string CampaignOrLocation { get; set; }
        public LocationType LocationType { get; set; }
        public bool HasSaves { get; set; }
    }
}
