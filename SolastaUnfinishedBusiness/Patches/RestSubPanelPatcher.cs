using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RestSubPanelPatcher
{
    [HarmonyPatch(typeof(RestSubPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] RestSubPanel __instance)
        {
            var y1 = __instance.characterPlatesTable.anchoredPosition.y;
            var y2 = __instance.restModulesTable.anchoredPosition.y;

            //PATCH: scales down the rest sub panel whenever the party size is bigger than 4 (PARTYSIZE)
            switch (Gui.GameCampaign.Party.CharactersList.Count)
            {
                case 5:
                    __instance.characterPlatesTable.anchoredPosition = new Vector2(-5f, y1);
                    __instance.restModulesTable.anchoredPosition = new Vector2(-5f, y2);
                    break;

                case 6:
                    __instance.characterPlatesTable.anchoredPosition = new Vector2(-125f, y1);
                    __instance.restModulesTable.anchoredPosition = new Vector2(-125f, y2);
                    break;

                default:
                    __instance.characterPlatesTable.anchoredPosition = new Vector2(0, y1);
                    __instance.restModulesTable.anchoredPosition = new Vector2(0, y2);
                    break;
            }

            //PATCH: Allow More Real State On Rest Panel
            if (!Main.Settings.AllowMoreRealStateOnRestPanel)
            {
                return;
            }

            // this is after rest actions which we hide in Rest Before Panel
            __instance.restModules[2].gameObject.SetActive(__instance is not RestBeforePanel);
            LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.RestModulesTable);
        }
    }
}
