using System;
using HarmonyLib;
using SolastaCommunityExpansion;
using static SolastaCommunityExpansion.Models.MulticlassContext;

namespace SolastaMulticlass.Patches.LevelDown
{
    // use this patch to enable the level down after rest action
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
    internal static class RulesetCharacterHeroEnumerateAfterRestActions
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            if (Main.Settings.EnableRespec)
            {
                var characterLevel = __instance.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

                if (characterLevel > 1)
                {
                    __instance.AfterRestActions.Add(RestActivityLevelDown);
                }
            }
        }
    }
}
