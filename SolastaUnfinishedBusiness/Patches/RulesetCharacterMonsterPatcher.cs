using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Subclasses.Wizard;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetCharacterMonsterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FinalizeMonster_Patch
    {
        private static void OnMulticlassWildshapeCreated(RulesetCharacterMonster __instance)
        {
            if (__instance.OriginalFormCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            // rebase all attributes from hero but first delete some pools we want to copy over again
            __instance.Attributes.Remove(AttributeDefinitions.HealingPool);
            __instance.Attributes.Remove(AttributeDefinitions.IndomitableResistances);
            __instance.Attributes.Remove(AttributeDefinitions.KiPoints);
            __instance.Attributes.Remove(AttributeDefinitions.RageDamage);
            __instance.Attributes.Remove(AttributeDefinitions.RagePoints);
            __instance.Attributes.Remove(AttributeDefinitions.SorceryPoints);

            foreach (var attribute in hero.Attributes.Where(x => !__instance.Attributes.ContainsKey(x.Key)))
            {
                __instance.Attributes.Add(attribute.Key, attribute.Value);
            }

            // rebase all powers from hero
            __instance.UsablePowers.SetRange(hero.UsablePowers);

            // sync rage points
            var count = hero.UsedRagePoints;

            while (count-- > 0)
            {
                __instance.SpendRagePoint();
            }
        }

        internal static void Postfix(RulesetCharacterMonster __instance)
        {
            //TODO: Consider creating an interface for these

            //PATCH: allows us to change monsters created by Dead Master
            WizardDeadMaster.OnMonsterCreated(__instance);

            //PATCH: ensures wildshape get all original character pools, states and attributes
            OnMulticlassWildshapeCreated(__instance);
        }
    }
}
