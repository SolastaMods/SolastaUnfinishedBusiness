using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomFightingStyle;

internal static class GraphicsCharacterHeroPatcher
{
    [HarmonyPatch(typeof(GraphicsCharacterHero), "GetAttackAnimationData")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GraphicsCharacterHero_GetAttackAnimationData
    {
        internal static void Postfix(
            ref string __result,
            [NotNull] RulesetAttackMode attackMode,
            ActionModifier attackModifier,
            ref bool isThrown,
            ref bool leftHand)
        {
            if (!ShieldStrikeContext.IsShield(attackMode.SourceDefinition as ItemDefinition))
            {
                return;
            }

            var weaponTypeDefinition = ShieldStrikeContext.ShieldWeaponType;

            leftHand = true;
            isThrown = false;
            __result = weaponTypeDefinition.AnimationTag;
        }
    }
}
