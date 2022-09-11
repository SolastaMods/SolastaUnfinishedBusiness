using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Subclasses.Wizard;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetCharacterMonsterPatcher
{
    //PATCH: ensures that wildshape get all original character pools and current powers states
    [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FinalizeMonster_Patch
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

        // usable powers must be added after hand to overwrite default values from game
        internal static void Postfix(RulesetCharacterMonster __instance)
        {
            //TODO: Consider creating an interface for this if really necessary
            WizardDeadMaster.OnMonsterCreated(__instance);

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
