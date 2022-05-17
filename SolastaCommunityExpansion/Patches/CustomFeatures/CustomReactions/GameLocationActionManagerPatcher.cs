using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    internal static class GameLocationActionManagerPatcher
    {
        [HarmonyPatch(typeof(GameLocationActionManager), "ReactForOpportunityAttack")]
        internal static class GameLocationActionManager_ReactForOpportunityAttack
        {
            internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
            {
                var rulesetCharacter = reactionParams?.ActingCharacter?.RulesetCharacter;

                // should not trigger if a wildshape form
                if (rulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
                {
                    return true;
                }

                var affinities = rulesetCharacterHero.GetFeaturesByType<FeatureDefinitionMagicAffinity>();

                if (affinities != null && affinities.Any(a => a.Name == "MagicAffinityWarCasterFeat"))
                {
                    __instance.InvokeMethod("AddInterruptRequest", new ReactionRequestWarcaster(reactionParams));

                    return false;
                }

                return true;
            }
        }
    }
}
