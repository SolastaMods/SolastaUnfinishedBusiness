using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
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
            RulesetAttackMode attackMode,
            ActionModifier attackModifier,
            ref bool isThrown,
            ref bool leftHand)
        {
            if (!ShieldStrikeContext.IsShield(attackMode.SourceDefinition as ItemDefinition))
            {
                return;
            }

            var weaponTypeDefinition = ShieldStrikeContext.ShieldWeaponType;

            if (weaponTypeDefinition == null)
            {
                return;
            }

            leftHand = true;
            isThrown = false;
            __result = weaponTypeDefinition.AnimationTag;
        }
    }
}
