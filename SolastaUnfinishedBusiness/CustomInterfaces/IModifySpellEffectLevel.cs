using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifySpellEffectLevel
{
    public int GetEffectLevel(RulesetActor rulesetActor, [UsedImplicitly] RulesetEffectSpell rulesetEffectSpell);
}
