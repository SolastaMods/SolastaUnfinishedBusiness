using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaUnfinishedBusiness.Models.Level20Context;

namespace SolastaUnfinishedBusiness.Patches;

public static class HeroDefinitionsPatcher
{
    [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class MaxHeroExperience_Patch
    {
        //PATCH: overrides the max experience allowed under Level 20 scenarios
        public static bool Prefix(ref int __result)
        {
            if (!Main.Settings.EnableLevel20)
            {
                return true;
            }

            __result = ModMaxExperience;

            return false;
        }
    }
}
