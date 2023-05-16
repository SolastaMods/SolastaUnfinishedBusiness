using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FeatHelpers
{
    internal sealed class ModifyWeaponAttackModeTypeFilter : IModifyWeaponAttackMode
    {
        private readonly string _sourceName;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public ModifyWeaponAttackModeTypeFilter(string sourceName,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _sourceName = sourceName;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (attackMode.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            attackMode.ToHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(1,
                RuleDefinitions.FeatureSourceType.CharacterFeature, _sourceName, null));
        }
    }
}
