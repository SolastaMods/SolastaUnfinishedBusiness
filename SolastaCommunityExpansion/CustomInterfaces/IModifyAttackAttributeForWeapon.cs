namespace SolastaCommunityExpansion.CustomInterfaces
{
    interface IModifyAttackAttributeForWeapon
    {
        void ModifyAttribute(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon);
    }
}

