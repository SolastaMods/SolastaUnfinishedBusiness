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
                var affinitys = reactionParams?.ActingCharacter?.RulesetCharacter
                    .GetFeaturesByType<FeatureDefinitionMagicAffinity>();
                if (affinitys != null && affinitys.Any(a => a.Name == "MagicAffinityWarCasterFeat"))
                {
                    __instance.InvokeMethod("AddInterruptRequest", new ReactionRequestWarcaster(reactionParams));
                    return false;
                }

                return true;
            }
        }
    }
}
