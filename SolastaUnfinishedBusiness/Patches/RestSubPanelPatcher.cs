using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class RestSubPanelPatcher
{
    [HarmonyPatch(typeof(RestSubPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] RestSubPanel __instance)
        {
            //PATCH: scales down the rest sub panel whenever the party size is bigger than 4 (PARTYSIZE)
            switch (Gui.GameCampaign.Party.CharactersList.Count)
            {
                case 5:
                    __instance.characterPlatesTable.anchoredPosition = new Vector2(-5f, -25f);
                    __instance.restModulesTable.anchoredPosition = new Vector2(-5f, 0);
                    break;
                
                case 6:
                    __instance.characterPlatesTable.anchoredPosition = new Vector2(-125f, -25f);
                    __instance.restModulesTable.anchoredPosition = new Vector2(-125f, 0);
                    break;
                
                default:
                    __instance.characterPlatesTable.anchoredPosition = new Vector2(0, -25f);
                    __instance.restModulesTable.anchoredPosition = new Vector2(0, 0);
                    break;    
            }
        }
    }
}
