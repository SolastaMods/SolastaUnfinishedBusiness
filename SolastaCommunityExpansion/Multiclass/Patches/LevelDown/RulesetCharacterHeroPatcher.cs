using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.CustomDefinitions;

using System;

namespace SolastaMulticlass.Patches.LevelDown
{
    // use this patch to enable the level down after rest action
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
    internal static class RulesetCharacterHeroEnumerateAfterRestActions
    {
        [Obsolete]
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            if (Main.Settings.EnableLevelDown)
            {
                var characterLevel = __instance.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

                if (characterLevel > 1)
                {
                    __instance.AfterRestActions.Add(RestActivityLevelDownBuilder.RestActivityLevelDown);
                }
            }
        }
    }
}
