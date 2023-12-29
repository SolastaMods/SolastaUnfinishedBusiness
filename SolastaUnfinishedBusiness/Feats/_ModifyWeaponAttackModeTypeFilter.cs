using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FeatHelpers
{
    internal sealed class ModifyWeaponAttackModeTypeFilter : IModifyWeaponAttackMode
    {
        private readonly FeatDefinition _source;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = [];

        public ModifyWeaponAttackModeTypeFilter(
            FeatDefinition source,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _source = source;
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
            attackMode.ToHitBonusTrends.Add(new TrendInfo(1, FeatureSourceType.Feat, _source.Name, _source));
        }
    }
}
