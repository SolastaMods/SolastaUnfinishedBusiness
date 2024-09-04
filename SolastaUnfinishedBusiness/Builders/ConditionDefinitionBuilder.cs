using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using TA.AI;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[Flags]
internal enum Silent
{
    None,
    WhenAdded = 1,
    WhenRemoved = 2,
    WhenRefreshed = 4,
    WhenAddedOrRemoved = WhenAdded | WhenRemoved,
    WhenRefreshedOrRemoved = WhenRefreshed | WhenRemoved
}

[UsedImplicitly]
internal class ConditionDefinitionBuilder
    : DefinitionBuilder<ConditionDefinition, ConditionDefinitionBuilder>
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

    internal ConditionDefinitionBuilder AllowMultipleInstances()
    {
        Definition.allowMultipleInstances = true;
        return this;
    }

    internal ConditionDefinitionBuilder SetBrain(
        DecisionPackageDefinition battlePackage, bool forceBehavior, bool fearSource)
    {
        Definition.battlePackage = battlePackage;
        Definition.forceBehavior = forceBehavior;
        Definition.fearSource = fearSource;
        return this;
    }

    internal ConditionDefinitionBuilder SetCancellingConditions(params ConditionDefinition[] values)
    {
        Definition.cancellingConditions.SetRange(values);
        return this;
    }

    internal ConditionDefinitionBuilder SetAmountOrigin(ConditionDefinition.OriginOfAmount value)
    {
        Definition.amountOrigin = value;
        return this;
    }

    internal ConditionDefinitionBuilder SetAmountOrigin(ExtraOriginOfAmount value, string source = null)
    {
        //ExtraOriginOfAmount uses additionalDamageType as value for class or ability to get amount from
        if (source != null)
        {
            Definition.additionalDamageType = source;
        }

        return SetAmountOrigin((ConditionDefinition.OriginOfAmount)value);
    }

    internal ConditionDefinitionBuilder SetFixedAmount(int value)
    {
        Definition.amountOrigin = ConditionDefinition.OriginOfAmount.Fixed;
        Definition.baseAmount = value;
        return this;
    }

    internal ConditionDefinitionBuilder AdditionalDiceDamageWhenHit(
        int dieNumber = 0,
        DieType dieType = DieType.D1,
        AdditionalDamageType damageTypeDetermination = AdditionalDamageType.SameAsBaseDamage,
        string damageType = null)
    {
        if (damageTypeDetermination == AdditionalDamageType.Specific
            && string.IsNullOrEmpty(damageType))
        {
            throw new ArgumentException("Damage type must be set if damage type determination set to Specific");
        }

        Definition.additionalDamageWhenHit = true;
        Definition.additionalDamageQuantity = ConditionDefinition.DamageQuantity.Dice;
        Definition.additionalDamageDieNumber = dieNumber;
        Definition.additionalDamageDieType = dieType;
        Definition.additionalDamageTypeDetermination = damageTypeDetermination;
        Definition.additionalDamageType = damageType;

        return this;
    }

    internal ConditionDefinitionBuilder CopyParticleReferences(ConditionDefinition from)
    {
        Definition.conditionParticleReference = from.conditionParticleReference;
        Definition.conditionStartParticleReference = from.conditionStartParticleReference;
        Definition.conditionEndParticleReference = from.conditionEndParticleReference;
        Definition.recurrentEffectParticleReference = from.recurrentEffectParticleReference;
        return this;
    }

    internal ConditionDefinitionBuilder CopyParticleReferences(IMagicEffect magicEffect)
    {
        Definition.conditionParticleReference =
            magicEffect.EffectDescription.EffectParticleParameters.conditionParticleReference;
        Definition.conditionStartParticleReference =
            magicEffect.EffectDescription.EffectParticleParameters.conditionStartParticleReference;
        Definition.conditionEndParticleReference =
            magicEffect.EffectDescription.EffectParticleParameters.conditionEndParticleReference;
        Definition.recurrentEffectParticleReference = new AssetReference();
        return this;
    }

#if false
    internal ConditionDefinitionBuilder AddConditionTags(params string[] tags)
    {
        Definition.conditionTags.AddRange(tags);
        return this;
    }
#endif

    internal ConditionDefinitionBuilder SetConditionType(ConditionType value)
    {
        Definition.conditionType = value;
        return this;
    }

    internal ConditionDefinitionBuilder Detrimental()
    {
        return SetConditionType(ConditionType.Detrimental);
    }

    internal ConditionDefinitionBuilder SetParentCondition(ConditionDefinition value)
    {
        Definition.parentCondition = value;
        return this;
    }

    internal ConditionDefinitionBuilder SetConditionParticleReference(AssetReference value)
    {
        Definition.conditionParticleReference = value;
        return this;
    }

    internal ConditionDefinitionBuilder SetConditionParticleReference(ConditionDefinition conditionDefinition)
    {
        Definition.conditionStartParticleReference = conditionDefinition.conditionStartParticleReference;
        Definition.conditionParticleReference = conditionDefinition.conditionParticleReference;
        Definition.conditionEndParticleReference = conditionDefinition.conditionEndParticleReference;
        return this;
    }

    internal ConditionDefinitionBuilder SetConditionParticleReference(BaseDefinition baseDefinition)
    {
        if (baseDefinition is ConditionDefinition conditionDefinition)
        {
            return SetConditionParticleReference(conditionDefinition);
        }

        if (baseDefinition is not IMagicEffect magicEffect)
        {
            return this;
        }

        Definition.conditionStartParticleReference =
            magicEffect.EffectDescription.EffectParticleParameters.conditionStartParticleReference;
        Definition.conditionParticleReference =
            magicEffect.EffectDescription.EffectParticleParameters.conditionParticleReference;
        Definition.conditionEndParticleReference =
            magicEffect.EffectDescription.EffectParticleParameters.conditionEndParticleReference;
        return this;
    }

    internal ConditionDefinitionBuilder SetCharacterShaderReference(AssetReference value)
    {
        Definition.characterShaderReference = value;
        return this;
    }

    internal ConditionDefinitionBuilder AddFeatures(params FeatureDefinition[] value)
    {
        Definition.Features.AddRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

    internal ConditionDefinitionBuilder SetFeatures(params FeatureDefinition[] value)
    {
        Definition.Features.SetRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

    internal ConditionDefinitionBuilder SetFeatures(IEnumerable<FeatureDefinition> value)
    {
        Definition.Features.SetRange(value);
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }

    internal ConditionDefinitionBuilder SetRecurrentEffectForms(params EffectForm[] forms)
    {
        Definition.RecurrentEffectForms.SetRange(forms);
        return this;
    }

    internal ConditionDefinitionBuilder SetSilent(Silent silent)
    {
        Definition.silentWhenAdded = silent.HasFlag(Silent.WhenAdded);
        Definition.silentWhenRemoved = silent.HasFlag(Silent.WhenRemoved);
        Definition.silentWhenRefreshed = silent.HasFlag(Silent.WhenRefreshed);
        return this;
    }

    internal ConditionDefinitionBuilder SetSpecialDuration(
        DurationType durationType = DurationType.Round,
        int duration = 0,
        TurnOccurenceType turnOccurence = TurnOccurenceType.EndOfTurn)
    {
        // ReSharper disable once InvocationIsSkipped
        // PreConditions.IsValidDuration(durationType, duration);

        if (duration != 0)
        {
            Definition.durationParameterDie = DieType.D1;
        }

        Definition.specialDuration = true;
        Definition.durationParameter = duration;
        Definition.durationType = durationType;
        Definition.turnOccurence = turnOccurence;
        return this;
    }

    internal ConditionDefinitionBuilder SetPossessive()
    {
        Definition.possessive = true;
        return this;
    }

    internal ConditionDefinitionBuilder SetSpecialInterruptions(params ConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.SetRange(value);
        return this;
    }

    internal ConditionDefinitionBuilder SetSpecialInterruptions(params ExtraConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.SetRange(value.Select(v => (ConditionInterruption)v));
        return this;
    }

    internal ConditionDefinitionBuilder AddSpecialInterruptions(params ConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.AddRange(value);
        return this;
    }

#if false
    internal ConditionDefinitionBuilder AddSpecialInterruptions(params ExtraConditionInterruption[] value)
    {
        Definition.SpecialInterruptions.AddRange(value.Select(v => (RuleDefinitions.ConditionInterruption)v));
        return this;
    }

    internal ConditionDefinitionBuilder ClearSpecialInterruptions()
    {
        Definition.SpecialInterruptions.Clear();
        return this;
    }

    internal ConditionDefinitionBuilder SetInterruptionDamageThreshold(int value)
    {
        Definition.interruptionDamageThreshold = value;
        return this;
    }
#endif

    #region Constructors

    protected ConditionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
        SetEmptyParticleReferencesWhereNull(Definition);
    }

    protected ConditionDefinitionBuilder(ConditionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
