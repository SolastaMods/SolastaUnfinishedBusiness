using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeSpellEffectLevel
{
    public int GetEffectLevel(RulesetActor rulesetActor, [UsedImplicitly] RulesetEffectSpell rulesetEffectSpell);
}
