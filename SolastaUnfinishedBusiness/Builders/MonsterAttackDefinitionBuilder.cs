using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

internal abstract class
    MonsterAttackDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : MonsterAttackDefinition
    where TBuilder : MonsterAttackDefinitionBuilder<TDefinition, TBuilder>

{
    internal TBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.EffectDescription = effect;
        return This();
    }

    #region Constructors

    protected MonsterAttackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MonsterAttackDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected MonsterAttackDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected MonsterAttackDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class MonsterAttackDefinitionBuilder : MonsterAttackDefinitionBuilder<MonsterAttackDefinition,
    MonsterAttackDefinitionBuilder>
{
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
