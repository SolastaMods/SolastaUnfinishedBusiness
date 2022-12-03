namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyAttackAttributeForWeapon
{
    void ModifyAttribute(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon,
        bool canAddAbilityDamageBonus);
}
