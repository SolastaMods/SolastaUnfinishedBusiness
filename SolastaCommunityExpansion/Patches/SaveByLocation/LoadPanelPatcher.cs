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

namespace SolastaCommunityExpansion
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

            var userContentList =
                allCampaigns
                        .Select(l => new { IsLocation = (bool?)false, l.Title })
                        .OrderBy(l => l.Title)
                .Concat(
                    allLocations
                    .Select(l => new { IsLocation = (bool?)true, l.Title })
                    .OrderBy(l => l.Title)
                )
                .ToList();

            guiDropdown.AddOptions(
                Enumerable.Repeat(new { IsLocation = (bool?)null, Title = "Main campaign" }, 1)
                .Union(userContentList)
                .Select(opt => new CEOptionData
                {
                    text = GetTitle(opt.IsLocation, opt.Title),
                    CampaignOrLocation = opt.Title,
                    image = GetImage(opt.IsLocation, opt.Title),
                    IsLocation = opt.IsLocation,
                    HasSaves = HasSaves(opt.IsLocation, opt.Title),
                    // TODO: get latest save date
                })
                .Where(opt => opt.HasSaves)
                .OfType<OptionData>()
                .ToList());

            // TODO: is this working?
            // Get the current campaign location and select it in the dropdown
            var currentLocation = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>().Location;
            var option = guiDropdown.options
                .Select((o, i) => new { o.text, Index = i })
                .FirstOrDefault(o => o.text == currentLocation);

            guiDropdown.value = option?.Index ?? 0;

            ValueChanged(guiDropdown);

            Sprite GetImage(bool? isLocation, string title)
            {
                // TODO: get suitable sprites - anyone?
                return null;
            }

            string GetTitle(bool? isLocation, string title)
            {
                switch (isLocation)
                {
                    case null:
                        return title;
                    case false:
                        return title.yellow();
                    default:
                        return title.orange();
                }
            }

            void ValueChanged(GuiDropdown dropdown)
            {
                // update selected campaign
                var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

                var selected = dropdown.options.Skip(dropdown.value).FirstOrDefault() as CEOptionData;

                switch (selected.IsLocation)
                {
                    case null: // default campaign
                        selectedCampaignService.Campaign = MAIN_CAMPAIGN;
                        selectedCampaignService.Location = "";
                        break;
                    case true: // location (campaign=USER_CAMPAIGN + location)
                        selectedCampaignService.Campaign = USER_CAMPAIGN;
                        selectedCampaignService.Location = selected.CampaignOrLocation;
                        break;
                    case false: // campaign
                        selectedCampaignService.Campaign = selected.CampaignOrLocation;
                        selectedCampaignService.Location = "";
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

    internal class CEOptionData : OptionData
    {
        public string CampaignOrLocation { get; set; }
        public bool? IsLocation { get; set; }
        public bool HasSaves { get; set; }
    }
}
