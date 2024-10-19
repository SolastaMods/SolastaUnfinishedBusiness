using System.Linq;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class FeatHelpers
{
    internal sealed class ModifyWeaponAttackModeTypeFilter(
        FeatDefinition source,
        params WeaponTypeDefinition[] weaponTypeDefinition) : IModifyWeaponAttackMode
    {
        private readonly TrendInfo _trendInfo = new(1, FeatureSourceType.Feat, source.Name, source);

        public void ModifyWeaponAttackMode(
            RulesetCharacter character,
            RulesetAttackMode attackMode,
            RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (attackMode.SourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            attackMode.ToHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(_trendInfo);
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
