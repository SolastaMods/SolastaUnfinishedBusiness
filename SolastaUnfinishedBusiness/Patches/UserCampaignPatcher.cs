using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserCampaignPatcher
{
    [HarmonyPatch(typeof(UserCampaign), "PostLoadJson")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class PostLoadJson_Patch
    {
        //PATCH: Ensures game doesn't remove `invalid` monsters created with Dungeon Maker Pro (DMP)
        public static bool Prefix()
        {
            return !SaveByLocationContext.UseLightEnumeration;
        }
    }
}
