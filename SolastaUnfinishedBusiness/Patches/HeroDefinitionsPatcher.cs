using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Models.Level20Context;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class HeroDefinitionsPatcher
{
    [HarmonyPatch(typeof(HeroDefinitions), nameof(HeroDefinitions.MaxHeroExperience))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MaxHeroExperience_Patch
    {
        //PATCH: overrides the max experience allowed under Level 20 scenarios
        [UsedImplicitly]
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
