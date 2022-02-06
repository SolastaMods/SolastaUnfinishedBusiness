using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelDown
{
    public static class RulesetCharacterHeroPatcher
    {
        // use this patch to enable the Level Down after rest action
        [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
        internal static class RulesetCharacterHeroEnumerateAfterRestActions
        {
            internal static void Postfix(RulesetCharacterHero __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var characterLevel = __instance.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

                if (characterLevel > 1)
                {
                    __instance.AfterRestActions.Add(CustomDefinitions.RestActivityLevelDownBuilder.RestActivityLevelDown);
                }
            }
        }
    }
}
