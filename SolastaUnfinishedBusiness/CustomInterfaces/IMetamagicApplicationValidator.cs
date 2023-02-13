using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMetamagicApplicationValidator
{
    // return true is available otherwise return a failure reason as well
    [UsedImplicitly]
    public bool IsMetamagicOptionValid(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref string failure);
}
