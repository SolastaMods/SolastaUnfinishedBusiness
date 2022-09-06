using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.PatchCode.CustomFeatures;

internal static class ShieldAttack
{
    public static void UseOffhandForShieldAttackAnimation(RulesetAttackMode attackMode, ref string animation,
        ref bool isThrown,
        ref bool leftHand)
    {
        if (!ShieldStrikeContext.IsShield(attackMode.SourceDefinition as ItemDefinition))
        {
            return;
        }

        leftHand = true;
        isThrown = false;
        animation = ShieldStrikeContext.ShieldWeaponType.AnimationTag;
    }
}