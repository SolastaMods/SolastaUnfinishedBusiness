using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

[Flags]
internal enum Silent
{
    None,
    WhenAdded = 1,
    WhenRemoved = 2,
    WhenAddedOrRemoved = WhenAdded | WhenRemoved
}

[UsedImplicitly]
internal class ConditionDefinitionBuilder :
    ConditionDefinitionBuilder<ConditionDefinition, ConditionDefinitionBuilder>
{
    #region Constructors

    protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}

internal abstract class ConditionDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : ConditionDefinition
    where TBuilder : ConditionDefinitionBuilder<TDefinition, TBuilder>
{
    private static void SetEmptyParticleReferencesWhereNull(ConditionDefinition definition)
    {
        var assetReference = new AssetReference();

        definition.conditionStartParticleReference ??= assetReference;
        definition.conditionParticleReference ??= assetReference;
        definition.conditionEndParticleReference ??= assetReference;
        definition.characterShaderReference ??= assetReference;
    }

    protected override void Initialise()
    {
        base.Initialise();
        SetEmptyParticleReferencesWhereNull(Definition);
    }

    internal TBuilder SetAllowMultipleInstances(bool value)
    {
        Definition.allowMultipleInstances = value;
        return (TBuilder)this;
    }

    internal TBuilder SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
    {
        Definition.amountOrigin = value;
        return (TBuilder)this;
    }

    internal TBuilder SetConditionParticleReference(AssetReference value)
    {
        Definition.conditionParticleReference = value;
        return (TBuilder)this;
    }

    internal TBuilder CopyParticleReferences(ConditionDefinition from)
    {
        Definition.conditionParticleReference = from.conditionParticleReference;
        Definition.conditionStartParticleReference = from.conditionStartParticleReference;
        Definition.conditionEndParticleReference = from.conditionEndParticleReference;
        Definition.recurrentEffectParticleReference = from.recurrentEffectParticleReference;
        return (TBuilder)this;
    }

    internal TBuilder SetAdditionalDamageType(string value)
    {
        Definition.additionalDamageType = value;
        return (TBuilder)this;
    }

    internal TBuilder AddConditionTags(params string[] tags)
    {
        Definition.conditionTags.AddRange(tags);
        return (TBuilder)this;
    }

    internal TBuilder SetCharacterShaderReference(AssetReference value)
    {
        Definition.characterShaderReference = value;
        return (TBuilder)this;
    }

    internal TBuilder SetConditionType(RuleDefinitions.ConditionType value)
    {
        Definition.conditionType = value;
        return (TBuilder)this;
    }

    internal TBuilder SetTurnOccurence(RuleDefinitions.TurnOccurenceType value)
    {
        Definition.turnOccurence = value;
        return (TBuilder)this;
    }

    internal TBuilder SetParentCondition(ConditionDefinition value)
    {
        Definition.parentCondition = value;
        return (TBuilder)this;
    }

    internal TBuilder ClearFeatures()
    {
        Definition.Features.Clear();
        return (TBuilder)this;
    }

    internal TBuilder AddFeatures(params FeatureDefinition[] value)
    {
        Definition.Features.AddRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return (TBuilder)this;
    }

    internal TBuilder SetFeatures(params FeatureDefinition[] value)
    {
        Definition.Features.SetRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return (TBuilder)this;
    }

    internal TBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
    {
        Definition.Features.SetRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return (TBuilder)this;
    }

    internal TBuilder SetRecurrentEffectForms(params EffectForm[] forms)
    {
        Definition.RecurrentEffectForms.SetRange(forms);
        return (TBuilder)this;
    }

    internal TBuilder SetAdditionalDamageWhenHit(
        ConditionDefinition.DamageQuantity damageQuantity = ConditionDefinition.DamageQuantity.Dice,
        RuleDefinitions.DieType dieType = RuleDefinitions.DieType.D1,
        int numberOfDie = 0,
        string damageType = "",
        bool active = true)
    {
        Definition.additionalDamageWhenHit = active;
        Definition.additionalDamageDieType = dieType;
        Definition.additionalDamageDieNumber = numberOfDie;
        Definition.additionalDamageQuantity = damageQuantity;
        Definition.additionalDamageType = damageType;
        return (TBuilder)this;
    }

    internal TBuilder SetTerminateWhenRemoved(bool value)
    {
        Definition.terminateWhenRemoved = value;
        return (TBuilder)this;
    }

    internal TBuilder SetSilent(Silent silent)
    {
        Definition.silentWhenAdded = silent.HasFlag(Silent.WhenAdded);
        Definition.silentWhenRemoved = silent.HasFlag(Silent.WhenRemoved);
        return (TBuilder)this;
    }

    internal TBuilder SetSpecialDuration(bool value)
    {
        Definition.specialDuration = value;
        return (TBuilder)this;
    }

    internal TBuilder SetPossessive(bool value = true)
    {
        Definition.possessive = value;
        return (TBuilder)this;
    }

    internal TBuilder ClearSpecialInterruptions()
    {
        Definition.SpecialInterruptions.Clear();
        return (TBuilder)this;
    }

    internal TBuilder SetSpecialInterruptions(params RuleDefinitions.ConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.SetRange(value);
        return (TBuilder)this;
    }

    internal TBuilder SetSpecialInterruptions(params ExtraConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.SetRange(value.Select(v => (RuleDefinitions.ConditionInterruption)v));
        return (TBuilder)this;
    }

    internal TBuilder AddSpecialInterruptions(params RuleDefinitions.ConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.AddRange(value);
        return (TBuilder)this;
    }

#if false
    internal TBuilder AddSpecialInterruptions(params ExtraConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.AddRange(value.Select(v => (RuleDefinitions.ConditionInterruption)v));
        return (TBuilder)this;
    }
#endif

    internal TBuilder SetInterruptionDamageThreshold(int value)
    {
        Definition.interruptionDamageThreshold = value;
        return (TBuilder)this;
    }

    internal TBuilder SetDuration(RuleDefinitions.DurationType type, int duration = 0, bool validate = true)
    {
        if (validate)
        {
            PreConditions.IsValidDuration(type, duration);
        }

        Definition.durationParameter = duration;
        Definition.durationType = type;
        return (TBuilder)this;
    }

    #region Constructors

    protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
        SetEmptyParticleReferencesWhereNull(Definition);
    }

    protected ConditionDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
