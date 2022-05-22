using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.Multiclass.Wildshape
{
    // ensures that wildshape get all original character pools and current powers states
    [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterMonster_FinalizeMonster
    {
        // remaining pools must be added beforehand to avoid a null pointer exception
        internal static void Prefix(RulesetCharacterMonster __instance)
        {
            if (__instance.OriginalFormCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            foreach (var attribute in hero.Attributes.Where(x => !__instance.Attributes.ContainsKey(x.Key)))
            {
                __instance.Attributes.Add(attribute.Key, attribute.Value);
            }
        }

        // usable powers must be added afterhand to overwrite default values from game
        internal static void Postfix(RulesetCharacterMonster __instance)
        {
            if (__instance.OriginalFormCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            __instance.UsablePowers.SetRange(hero.UsablePowers);

            // sync rage points
            var count = hero.UsedRagePoints;

            while (count-- > 0)
            {
                __instance.SpendRagePoint();
            }
        }
    }
}
