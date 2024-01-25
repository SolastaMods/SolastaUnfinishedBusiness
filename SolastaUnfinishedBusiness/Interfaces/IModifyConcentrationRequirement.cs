using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// allows to bypass spell concentration if original spell on this list
public interface IModifyConcentrationRequirement
{
    [UsedImplicitly]
    public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell);
}
