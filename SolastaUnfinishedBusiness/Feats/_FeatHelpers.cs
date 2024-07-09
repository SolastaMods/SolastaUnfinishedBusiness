using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FeatHelpers
{
    internal sealed class ModifyWeaponAttackModeTypeFilter(
        FeatDefinition source,
        params WeaponTypeDefinition[] weaponTypeDefinition) : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (attackMode?.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            attackMode.ToHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(1, FeatureSourceType.Feat, source.Name, source));
        }
    }

    internal sealed class SpellTag
    {
        internal SpellTag(string spellTag, bool forceFixedList = false)
        {
            Name = spellTag;
            ForceFixedList = forceFixedList;
        }

        internal string Name { get; }
        internal bool ForceFixedList { get; }
    }
}
