namespace SolastaUnfinishedBusiness.CustomInterfaces;

// allows to bypass spell concentration if original spell on this list
public interface IModifyConcentrationRequirement
{
    public bool RequiresConcentration(RulesetCharacter rulesetCharacter, RulesetEffectSpell rulesetEffectSpell);
}
