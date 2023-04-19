using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomSpellEffectLevel
{
    public int GetEffectLevel(RulesetActor rulesetActor, [UsedImplicitly] RulesetEffectSpell rulesetEffectSpell);
}
