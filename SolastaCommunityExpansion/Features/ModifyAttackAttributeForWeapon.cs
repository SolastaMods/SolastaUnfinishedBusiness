namespace SolastaCommunityExpansion.Features;

public class CanUseAttributeForWeapon : IModifyAttackModeForWeapon
{
    private readonly CharacterValidator[] _validators;
    private readonly string attribute;
    private readonly GetWeaponValidityHandler isWeaponValid;

    public CanUseAttributeForWeapon(string attribute, GetWeaponValidityHandler isWeaponValid,
        params CharacterValidator[] validators)
    {
        this.attribute = attribute;
        this.isWeaponValid = isWeaponValid;
        _validators = validators;
    }

    public delegate bool GetWeaponValidityHandler(RulesetAttackMode attackMode, RulesetItem weapon);

    public void Apply(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
    {
        if (!character.IsValid(_validators))
        {
            return;
        }

        if (!isWeaponValid(attackMode, weapon))
        {
            return;
        }

        if (character.GetAttribute(attribute).CurrentValue >
            character.GetAttribute(attackMode.AbilityScore).CurrentValue)
        {
            attackMode.AbilityScore = attribute;
        }
    }
}