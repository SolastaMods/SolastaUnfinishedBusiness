using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class MonsterAttackDefinitionBuilder
    : DefinitionBuilder<MonsterAttackDefinition, MonsterAttackDefinitionBuilder>
{
    internal MonsterAttackDefinitionBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.EffectDescription = effect;
        return this;
    }

    public MonsterAttackDefinitionBuilder SetToHitBonus(int bonus)
    {
        Definition.toHitBonus = bonus;
        return this;
    }

    #region Constructors

    protected MonsterAttackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MonsterAttackDefinitionBuilder(MonsterAttackDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
