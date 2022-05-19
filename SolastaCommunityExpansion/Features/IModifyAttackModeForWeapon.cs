namespace SolastaCommunityExpansion.Features;

interface IModifyAttackModeForWeapon
{
    void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon);
}