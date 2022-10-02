namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICanIgnoreAoOImmunity
{
    bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker, float distance);
}
