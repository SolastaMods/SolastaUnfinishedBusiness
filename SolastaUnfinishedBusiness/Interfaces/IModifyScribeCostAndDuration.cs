using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyScribeCostAndDuration
{
    public void ModifyScribeCostMultiplier(
        [UsedImplicitly] RulesetCharacter character, SpellDefinition spellDefinition, ref float costMultiplier);

    public void ModifyScribeDurationMultiplier(
        [UsedImplicitly] RulesetCharacter character, SpellDefinition spellDefinition, ref float durationMultiplier);
}
