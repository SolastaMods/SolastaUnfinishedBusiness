namespace SolastaCommunityExpansion.Features;

interface IModifyAttackAttributeForWeapon
{
    void ModifyAttribute(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon);
}