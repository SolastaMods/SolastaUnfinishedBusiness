using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Builders;

internal class
    CancelConditionPowerBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, CancelConditionPowerBuilder>
{
    // TODO: replace with methods or remove and make helper method
    public CancelConditionPowerBuilder(string name, string guid, GuiPresentation presentation,
        ConditionDefinition condition) : base(name, guid)
    {
        var effectDescriptionBuilder = new EffectDescriptionBuilder();

        effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
            RuleDefinitions.TargetType.Self);
        effectDescriptionBuilder.AddEffectForm(new EffectFormBuilder()
            .SetConditionForm(condition, ConditionForm.ConditionOperation.Remove, false, false,
                new List<ConditionDefinition>()).Build());

        Definition.effectDescription = effectDescriptionBuilder.Build();
        Definition.GuiPresentation = presentation;
        Definition.fixedUsesPerRecharge = 1;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        Definition.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        Definition.activationTime = RuleDefinitions.ActivationTime.NoCost;
    }

    #region Constructors

    protected CancelConditionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CancelConditionPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected CancelConditionPowerBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected CancelConditionPowerBuilder(FeatureDefinitionPower original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}
