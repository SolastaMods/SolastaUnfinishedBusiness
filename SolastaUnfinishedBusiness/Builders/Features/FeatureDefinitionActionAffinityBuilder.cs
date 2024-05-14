using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionActionAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionActionAffinity, FeatureDefinitionActionAffinityBuilder>
{
    protected override void Initialise()
    {
        base.Initialise();
        SetDefaultAllowedActionTypes();
    }

    private void SetDefaultAllowedActionTypes()
    {
        const int MAX = (int)ActionDefinitions.ActionType.Max;
        Definition.AllowedActionTypes = new bool[MAX];
        for (var i = 0; i < MAX; i++)
        {
            Definition.AllowedActionTypes[i] = true;
        }
    }

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

    internal FeatureDefinitionActionAffinityBuilder SetRestrictedActions(params ActionDefinitions.Id[] actions)
    {
        Definition.RestrictedActions.SetRange(actions);
        Definition.RestrictedActions.Sort();
        return this;
    }

#if false
    internal FeatureDefinitionActionAffinityBuilder SetActionExecutionModifiers(
        params ActionDefinitions.ActionExecutionModifier[] modifiers)
    {
        Definition.ActionExecutionModifiers.SetRange(modifiers);
        return this;
    }
#endif

    internal FeatureDefinitionActionAffinityBuilder SetAllowedActionTypes(
        bool main = true,
        bool bonus = true,
        bool move = true,
        bool freeOnce = true,
        bool reaction = true,
        bool noCost = true)
    {
        var types = Definition.AllowedActionTypes;

        types[(int)ActionDefinitions.ActionType.Main] = main;
        types[(int)ActionDefinitions.ActionType.Bonus] = bonus;
        types[(int)ActionDefinitions.ActionType.Move] = move;
        types[(int)ActionDefinitions.ActionType.FreeOnce] = freeOnce;
        types[(int)ActionDefinitions.ActionType.Reaction] = reaction;
        types[(int)ActionDefinitions.ActionType.NoCost] = noCost;

        return this;
    }

#if false
    internal FeatureDefinitionActionAffinityBuilder SetMaxAttackNumber(int maxAttack)
    {
        Definition.maxAttacksNumber = maxAttack;
        return this;
    }
#endif

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
