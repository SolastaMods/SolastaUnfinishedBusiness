namespace SolastaCommunityExpansion.Features;

interface ICanUseAttributeForWeapon
{
    string GetAttribute(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon);
}

public class CanUseAttributeForWeapon : ICanUseAttributeForWeapon
{
    private readonly ICharacterValidator[] _validators;
    private readonly string attribute;
    private readonly GetWeaponValidityHandler isWeaponValid;

    public CanUseAttributeForWeapon(string attribute, GetWeaponValidityHandler isWeaponValid,
        params ICharacterValidator[] validators)
    {
        this.attribute = attribute;
        this.isWeaponValid = isWeaponValid;
        _validators = validators;
    }

    public delegate bool GetWeaponValidityHandler(RulesetAttackMode attackMode, RulesetItem weapon);

    public string GetAttribute(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
    {
        if (!character.IsValid(_validators))
        {
            return null;
        }

        if (isWeaponValid(attackMode, weapon))
        {
            return attribute;
        }

        return null;
    }
}