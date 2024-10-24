namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyScribeCostAndDuration
{
    public void ModifyScribeCostMultiplier(
        RulesetCharacter character, SpellDefinition spellDefinition, ref float costMultiplier);

    public void ModifyScribeDurationMultiplier(
        RulesetCharacter character, SpellDefinition spellDefinition, ref float durationMultiplier);
}
