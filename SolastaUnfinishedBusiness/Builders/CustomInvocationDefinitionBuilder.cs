using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CustomInvocationDefinitionBuilder
    : InvocationDefinitionBuilder<InvocationDefinitionCustom, CustomInvocationDefinitionBuilder>
{
    internal CustomInvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(InvocationDefinitionCustom original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder SetPoolType(InvocationPoolTypeCustom poolType)
    {
        Definition.PoolType = poolType;
        return this;
    }

    internal CustomInvocationDefinitionBuilder SetActionId(
        ActionDefinitions.Id main = ActionDefinitions.Id.CastInvocation,
        ActionDefinitions.Id bonus = (ActionDefinitions.Id)ExtraActionId.CastInvocationBonus)
    {
        Definition.MainActionId = main;
        Definition.BonusActionId = bonus;
        return this;
    }

    internal CustomInvocationDefinitionBuilder SetActionId(
        ExtraActionId main = (ExtraActionId)ActionDefinitions.Id.CastInvocation,
        ExtraActionId bonus = ExtraActionId.CastInvocationBonus)
    {
        return SetActionId((ActionDefinitions.Id)main, (ActionDefinitions.Id)bonus);
    }
}
