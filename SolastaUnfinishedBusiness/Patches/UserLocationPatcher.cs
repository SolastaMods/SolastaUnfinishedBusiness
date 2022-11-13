using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserLocationPatcher
{
    [HarmonyPatch(typeof(UserLocation), "PostLoadJson")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class PostLoadJson_Patch
    {
        //PATCH: Performance enhancement for SaveByLocation feature
        public static bool Prefix()
        {
            return !SaveByLocationContext.UseLightEnumeration;
        }
    }
}
