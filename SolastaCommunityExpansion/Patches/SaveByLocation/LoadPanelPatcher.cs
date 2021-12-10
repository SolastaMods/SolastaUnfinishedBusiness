using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion
{
    [HarmonyPatch(typeof(LoadPanel), "OnBeginShow")]
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

            GuiDropdown guiDropdown;

            // create a drop down singleton
            if (Dropdown == null)
            {
                var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");

                Dropdown = Object.Instantiate(dropdownPrefab);
                Dropdown.name = "LoadMenuDropDown";

                guiDropdown = Dropdown.GetComponent<GuiDropdown>();
                guiDropdown.onValueChanged.AddListener(delegate { ValueChanged(guiDropdown); });

                var buttonBar = canvas.GetComponentsInChildren<RectTransform>().SingleOrDefault(c => c.gameObject.name == "ButtonsBar")?.gameObject;

                if (buttonBar != null)
                {
                    var horizontalLayoutGroup = buttonBar.GetComponent<HorizontalLayoutGroup>();

                    if(horizontalLayoutGroup != null)
                    {
                        Dropdown.transform.SetParent(horizontalLayoutGroup.transform, false);
                    }
                }
            }
            else
            {
                Dropdown.SetActive(true);
                guiDropdown = Dropdown.GetComponent<GuiDropdown>();
            }

            var removedContent = new List<string>();

            // get all user locations
            var userLocationPoolService = ServiceRepository.GetService<IUserLocationPoolService>();
            userLocationPoolService.EnumeratePool(out var _, removedContent);
            var allLocations = userLocationPoolService.AllLocations;

            // get all user campaigns
            var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();
            userCampaignPoolService.EnumeratePool(out var _, removedContent);
            var allCampaigns = userCampaignPoolService.AllCampaigns;

            // populate the dropdown
            guiDropdown.ClearOptions();

            var userContentList = allLocations.Select(l => l.Title).Union(allCampaigns.Select(l => l.Title)).ToList();

            userContentList.Sort();
            guiDropdown.AddOptions(Enumerable.Repeat("Main campaign", 1).Union(userContentList).Select(l => new TMPro.TMP_Dropdown.OptionData { text = l }).ToList());

            // Get the current campaign location and select it in the dropdown
            var currentLocation = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>().Location;
            var option = guiDropdown.options
                .Select((o, i) => new { o.text, Index = i })
                .FirstOrDefault(o => o.text == currentLocation);

            guiDropdown.value = option?.Index ?? 0;

            ValueChanged(guiDropdown);

            void ValueChanged(GuiDropdown dropdown)
            {
                // update selected campaign
                var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();
                var title = dropdown.value == 0 ? string.Empty : dropdown.options.Skip(dropdown.value).FirstOrDefault()?.text;

                selectedCampaignService.Campaign = dropdown.value == 0 ? MAIN_CAMPAIGN : USER_CAMPAIGN;
                selectedCampaignService.Location = title;

                // reload the save list
                var method = AccessTools.Method(typeof(LoadPanel), "EnumerateSaveLines");
                __instance.StartCoroutine((IEnumerator)method.Invoke(__instance, null));

                // reset the load button
                method = AccessTools.Method(typeof(LoadPanel), "Reset");
                method.Invoke(__instance, null);
            }
        }
    }
}
