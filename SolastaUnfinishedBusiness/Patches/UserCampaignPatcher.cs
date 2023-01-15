using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserCampaignPatcher
{
    [HarmonyPatch(typeof(UserCampaign), "PostLoadJson")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PostLoadJson_Patch
    {
        //PATCH: Performance enhancement for SaveByLocation feature
        [UsedImplicitly]
        public static bool Prefix()
        {
            return !SaveByLocationContext.UseLightEnumeration;
        }
    }
}
