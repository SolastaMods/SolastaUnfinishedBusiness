using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Models;

public static class HouseFeatureContext
{
    internal const int DefaultVisionRange = 16;
    internal const int MaxVisionRange = 120;

    public static void LateLoad()
    {
        FixDivineSmiteRestrictions();
        FixDivineSmiteDiceNumberWhenUsingHighLevelSlots();
        FixMountaineerBonusShoveRestrictions();
        FixRecklessAttackForReachWeapons();
    }

    /**
     * Makes Divine Smite trigger only from melee attacks.
     * This wasn't relevant until we changed how SpendSpellSlot trigger works.
     */
    private static void FixDivineSmiteRestrictions()
    {
        DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.attackModeOnly = true;
        DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.requiredProperty =
            RuleDefinitions.RestrictedContextRequiredProperty.MeleeWeapon;
    }

    /**
     * Makes Divine Smite use correct number of dice when spending slot level 5+.
     * Base game has config only up to level 4 slots, which leads to it using 1 die if level 5+ slot is spent.
     */
    private static void FixDivineSmiteDiceNumberWhenUsingHighLevelSlots()
    {
        DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite.diceByRankTable =
            DiceByRankMaker.MakeBySteps();
    }

    /**
     * Makes Mountaineer's `Shield Push` bonus shove work only with shield equipped.
     * This wasn't relevant until we removed forced shield check in the `GameLocationCharacter.GetActionStatus`.
     */
    private static void FixMountaineerBonusShoveRestrictions()
    {
        DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityMountaineerShieldCharge
            .SetCustomSubFeatures(new FeatureApplicationValidator(CharacterValidators.HasShield));
    }

    /**
     * Makes `Reckless` context check if main hand weapon is melee, instead of if character is next to target.
     * Required for it to work on reach weapons.
     */
    private static void FixRecklessAttackForReachWeapons()
    {
        DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityReckless
            .situationalContext = (RuleDefinitions.SituationalContext)ExtendedSituationalContext.MainWeaponIsMelee;
    }
}
