using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    internal static class ReadyActionSelectionPanelPatcher
    {
        internal static class GameLocationActionManagerPatcher
        {
            [HarmonyPatch(typeof(ReadyActionSelectionPanel), "Bind")]
            internal static class ReadyActionSelectionPanel_Bind
            {
                internal static void Prefix(ReadyActionSelectionPanel __instance)
                {
                    SetupForcePreferredToggle(__instance.GetField<RectTransform>("preferredCantripSelectionGroup"));
                }
            }

            private static void SetupForcePreferredToggle(RectTransform parent)
            {
                PersonalityFlagToggle toggle;
                if (parent.childCount < 3)
                {
                    var prefab = Resources.Load<GameObject>("Gui/Prefabs/CharacterEdition/PersonalityFlagToggle");
                    var asset = Object.Instantiate(prefab, parent, false);
                    asset.name = "ForcePreferredToggle";

                    var transform = asset.GetComponent<RectTransform>();
                    transform.SetParent(parent, false);
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    transform.anchoredPosition = new Vector2(0f, 1);
                    transform.localPosition = new Vector3(0, -30);
                    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
                    transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);

                    var title = parent.GetChild(0);
                    title.localPosition = new Vector3(-100, 55);

                    var group = parent.GetChild(1).GetComponent<RectTransform>();
                    group.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 25);
                    group.localPosition = new Vector3(-100, 5);

                    toggle = asset.GetComponent<PersonalityFlagToggle>();

                    var guiLabel = toggle.GetField<GuiLabel>("titleLabel");
                    guiLabel.Text = "UI/&ForcePreferredCantripTitle";

                    var tooltip = toggle.GetField<GuiTooltip>("tooltip");
                    tooltip.Content = "UI/&ForcePreferredCantripDescription";

                    toggle.PersonalityFlagDefinition = DatabaseHelper.PersonalityFlagDefinitions.Authority;
                    toggle.PersonalityFlagSelected = (_, state) =>
                    {
                        CustomReactionsContext.ForcePreferredCantrip = state;
                        tooltip.Content = "UI/&ForcePreferredCantripDescription";
                    };

                }
                else
                {
                    toggle = parent.FindChildRecursive("ForcePreferredToggle").GetComponent<PersonalityFlagToggle>();
                }

                toggle.Refresh(CustomReactionsContext.ForcePreferredCantrip, true);
                toggle.GetField<GuiTooltip>("tooltip").Content = "UI/&ForcePreferredCantripDescription";
            }
        }
    }
}
