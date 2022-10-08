using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class MonsterAttackDefinitionBuilder : DefinitionBuilder<MonsterAttackDefinition,
    MonsterAttackDefinitionBuilder>
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

    internal MonsterAttackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal MonsterAttackDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal MonsterAttackDefinitionBuilder(MonsterAttackDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    internal MonsterAttackDefinitionBuilder(MonsterAttackDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}
