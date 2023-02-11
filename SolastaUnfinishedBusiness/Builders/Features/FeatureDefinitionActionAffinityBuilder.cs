using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionActionAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionActionAffinity, FeatureDefinitionActionAffinityBuilder>
{
    internal FeatureDefinitionActionAffinityBuilder SetAuthorizedActions(params ActionDefinitions.Id[] actions)
    {
        Definition.AuthorizedActions.SetRange(actions);
        Definition.AuthorizedActions.Sort();
        return this;
    }

    internal FeatureDefinitionActionAffinityBuilder SetForbiddenActions(params ActionDefinitions.Id[] actions)
    {
        Definition.ForbiddenActions.SetRange(actions);
        Definition.ForbiddenActions.Sort();
        return this;
    }

#if false
    internal FeatureDefinitionActionAffinityBuilder SetRestrictedActions(params ActionDefinitions.Id[] actions)
    {
        Definition.RestrictedActions.SetRange(actions);
        Definition.RestrictedActions.Sort();
        return this;
    }
#endif

    internal FeatureDefinitionActionAffinityBuilder SetActionExecutionModifiers(
        params ActionDefinitions.ActionExecutionModifier[] modifiers)
    {
        Definition.ActionExecutionModifiers.SetRange(modifiers);
        return this;
    }

    internal FeatureDefinitionActionAffinityBuilder SetDefaultAllowedActionTypes()
    {
        Definition.AllowedActionTypes = new[] { true, true, true, true, true, true };
        return this;
    }

    internal FeatureDefinitionActionAffinityBuilder SetAllowedActionTypes(
        bool main = true,
        bool bonus = true,
        bool move = true,
        bool freeOnce = true,
        bool reaction = true,
        bool noCost = true)
    {
        Definition.AllowedActionTypes = new[] {main, bonus, move, freeOnce, reaction, noCost};
        return this;
    }

    internal FeatureDefinitionActionAffinityBuilder SetMaxAttackNumber(int maxAttack)
    {
        Definition.maxAttacksNumber = maxAttack;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
