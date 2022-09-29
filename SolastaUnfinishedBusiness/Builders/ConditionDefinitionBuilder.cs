using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

[Flags]
public enum Silent
{
    None,
    WhenAdded = 1,
    WhenRemoved = 2,
    WhenAddedOrRemoved = WhenAdded | WhenRemoved
}

public abstract class ConditionDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : ConditionDefinition
    where TBuilder : ConditionDefinitionBuilder<TDefinition, TBuilder>
{
    private static ConditionDefinition SetEmptyParticleReferencesWhereNull(ConditionDefinition definition)
    {
        var assetReference = new AssetReference();

        definition.conditionStartParticleReference ??= assetReference;
        definition.conditionParticleReference ??= assetReference;
        definition.conditionEndParticleReference ??= assetReference;
        definition.characterShaderReference ??= assetReference;

        return definition;
    }

    protected override void Initialise()
    {
        base.Initialise();

        SetEmptyParticleReferencesWhereNull(Definition);
    }

    private void SetEmptyParticleReferencesWhereNull()
    {
        SetEmptyParticleReferencesWhereNull(Definition);
    }

    // Setters delegating to Definition
    public TBuilder SetAllowMultipleInstances(bool value)
    {
        Definition.allowMultipleInstances = value;
        return This();
    }

    public TBuilder SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
    {
        Definition.amountOrigin = value;
        return This();
    }

    public TBuilder SetConditionParticleReference(AssetReference value)
    {
        Definition.conditionParticleReference = value;
        return This();
    }

    public TBuilder SetCharacterShaderReference(AssetReference value)
    {
        Definition.characterShaderReference = value;
        return This();
    }

    public TBuilder SetConditionType(RuleDefinitions.ConditionType value)
    {
        Definition.conditionType = value;
        return This();
    }

    public TBuilder SetTurnOccurence(RuleDefinitions.TurnOccurenceType value)
    {
        Definition.turnOccurence = value;
        return This();
    }

    public TBuilder SetParentCondition(ConditionDefinition value)
    {
        Definition.parentCondition = value;
        return This();
    }

    public TBuilder ClearFeatures()
    {
        Definition.Features.Clear();
        return This();
    }

    public TBuilder AddFeatures(params FeatureDefinition[] value)
    {
        Definition.Features.AddRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return This();
    }

    public TBuilder SetFeatures(params FeatureDefinition[] value)
    {
        Definition.Features.SetRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return This();
    }

    public TBuilder SetAdditionalDamageData(RuleDefinitions.DieType dieType, int numberOfDie,
        ConditionDefinition.DamageQuantity damageQuantity, bool additionalDamageWhenHit)
    {
        Definition.additionalDamageWhenHit = additionalDamageWhenHit;
        Definition.additionalDamageDieType = dieType;
        Definition.additionalDamageDieNumber = numberOfDie;
        Definition.additionalDamageQuantity = damageQuantity;

        return This();
    }

    public TBuilder SetTerminateWhenRemoved(bool value)
    {
        Definition.terminateWhenRemoved = value;
        return This();
    }

    public TBuilder SetSilentWhenAdded(bool value)
    {
        Definition.silentWhenAdded = value;
        return This();
    }

    public TBuilder SetSilentWhenRemoved(bool value)
    {
        Definition.silentWhenRemoved = value;
        return This();
    }

    public TBuilder SetSilent(Silent silent)
    {
        SetSilentWhenRemoved(silent.HasFlag(Silent.WhenRemoved));
        SetSilentWhenAdded(silent.HasFlag(Silent.WhenAdded));
        return This();
    }

    public TBuilder SetSpecialDuration(bool value)
    {
        Definition.specialDuration = value;
        return This();
    }

    public TBuilder SetPossessive(bool value)
    {
        Definition.possessive = value;
        return This();
    }

    public TBuilder SetSpecialInterruptions(params RuleDefinitions.ConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.SetRange(value);
        return This();
    }

    public TBuilder SetSpecialInterruptions(params ExtraConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.SetRange(value.Select(v => (RuleDefinitions.ConditionInterruption)v));
        return This();
    }

    public TBuilder AddSpecialInterruptions(params RuleDefinitions.ConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.AddRange(value);
        return This();
    }

    public TBuilder AddSpecialInterruptions(params ExtraConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.AddRange(value.Select(v => (RuleDefinitions.ConditionInterruption)v));
        return This();
    }

    public TBuilder SetInterruptionDamageThreshold(int value)
    {
        Definition.interruptionDamageThreshold = value;
        return This();
    }

    public TBuilder SetDuration(RuleDefinitions.DurationType type, int duration = 0, bool validate = true)
    {
        if (validate)
        {
            Preconditions.IsValidDuration(type, duration);
        }

        Definition.durationParameter = duration;
        Definition.durationType = type;

        return This();
    }

    public TBuilder Configure(
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        bool silent,
        params FeatureDefinition[] conditionFeatures)
    {
        Definition.Features.AddRange(conditionFeatures);
        Definition.conditionType = RuleDefinitions.ConditionType.Beneficial;
        Definition.allowMultipleInstances = false;
        Definition.durationType = durationType;
        Definition.durationParameter = durationParameter;

        if (!silent)
        {
            return This();
        }

        Definition.silentWhenAdded = true;
        Definition.silentWhenRemoved = true;

        return This();
    }

    #region Constructors

    protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
        SetEmptyParticleReferencesWhereNull();
    }

    protected ConditionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
        SetEmptyParticleReferencesWhereNull();
    }

    protected ConditionDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected ConditionDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class ConditionDefinitionBuilder :
    ConditionDefinitionBuilder<ConditionDefinition, ConditionDefinitionBuilder>
{
    #region Constructors

    protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected ConditionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected ConditionDefinitionBuilder(ConditionDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
