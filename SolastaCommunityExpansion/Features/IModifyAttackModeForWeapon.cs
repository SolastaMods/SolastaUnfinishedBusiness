namespace SolastaCommunityExpansion.Features;

interface IModifyAttackModeForWeapon
{
    void Apply(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon);
}