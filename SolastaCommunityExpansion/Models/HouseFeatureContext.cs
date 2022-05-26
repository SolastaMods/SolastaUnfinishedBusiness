using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    public static class HouseFeatureContext
    {
        public static void LateLoad()
        {
            FixDivineSmiteRestrictions();
            FixMountaineerBonusShoveRestrictions();
        }

        /**
         * Makes Divine Smite trigger only from melee attacks.
         * This wasn't relevant until we changed how SpendSpellSlot trigger works.
         */
        private static void FixDivineSmiteRestrictions()
        {
            DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamagePaladinDivineSmite
                .SetAttackModeOnly(true)
                .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon);
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
    }
}
