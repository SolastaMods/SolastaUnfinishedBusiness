using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// allows to bypass spell concentration if original spell on this list
public interface IModifyConcentrationRequirement
{
    [UsedImplicitly]
    public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell);
}
