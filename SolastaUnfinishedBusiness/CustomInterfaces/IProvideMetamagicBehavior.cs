using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IProvideMetamagicBehavior
{
    // return false here if not the metamagicOption you need to handle
    [UsedImplicitly]
    public bool IsMetamagicOptionAvailable(
        RulesetEffectSpell rulesetEffectSpell,
        RulesetCharacter caster,
        MetamagicOptionDefinition metamagicOption,
        ref string failure,
        ref bool result);

    [UsedImplicitly]
    public void MetamagicSelected(
        GameLocationCharacter caster,
        RulesetEffectSpell spellEffect,
        MetamagicOptionDefinition metamagicOption);
}
