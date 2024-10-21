using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyWeaponAttackMode
{
    public void ModifyWeaponAttackMode(
        RulesetCharacter character,
        RulesetAttackMode attackMode,
        [CanBeNull] RulesetItem weapon,
        bool canAddAbilityDamageBonus);
}
