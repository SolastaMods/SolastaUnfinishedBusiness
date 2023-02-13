using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IProvideMetamagicBehavior
{
    // provide the metamagic option name you're creating
    public string MetamagicOptionName();

    // return true is available otherwise return a failure reason as well
    [UsedImplicitly]
    public bool IsMetamagicOptionAvailable(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption,
        ref string failure);

    // change the rulesetEffectSpell here
    [UsedImplicitly]
    public void MetamagicSelected(
        RulesetCharacter caster,
        RulesetEffectSpell rulesetEffectSpell,
        MetamagicOptionDefinition metamagicOption);
}
