using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // overrides the max experience returned
    [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HeroDefinitions_MaxHeroExperience
    {
        internal static void Prefix(ref int levelCap)
        {
            if (levelCap == 20)
            {
                levelCap = 19;
            }
        }
    }

    [HarmonyPatch(typeof(GraphicsResourceManager), "LateUpdate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GraphicsResourceManager_LateUpdate
    {
        internal static bool Prefix(List<(UnityEngine.Object, byte)> ___syncAddressablesToRelease)
        {
            for (int index = ___syncAddressablesToRelease.Count - 1; index >= 0; --index)
            {
                var tuple = ___syncAddressablesToRelease[index];

                if (tuple.Item2 == (byte)0)
                {
                    Main.Logger.Log(tuple.Item1.name);
                    Addressables.Release(tuple.Item1);
                    ___syncAddressablesToRelease.RemoveAt(index);
                }
                else
                {
                    --tuple.Item2;
                    ___syncAddressablesToRelease[index] = tuple;
                }
            }

            return false;
        }
    }
}
