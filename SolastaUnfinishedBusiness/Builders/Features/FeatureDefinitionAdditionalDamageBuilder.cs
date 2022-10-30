using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAdditionalDamageBuilder : DefinitionBuilder<
    FeatureDefinitionAdditionalDamage, FeatureDefinitionAdditionalDamageBuilder>
{
    internal FeatureDefinitionAdditionalDamageBuilder SetSpecificDamageType(string damageType)
    {
        Definition.additionalDamageType = RuleDefinitions.AdditionalDamageType.Specific;
        Definition.specificDamageType = damageType;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetDamageDice(RuleDefinitions.DieType dieType, int diceNumber)
    {
        Definition.damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.Die;
        Definition.damageDiceNumber = diceNumber;
        Definition.damageDieType = dieType;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetNotificationTag(string tag)
    {
        Definition.notificationTag = tag;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetIgnoreCriticalDoubleDice(bool value)
    {
        Definition.ignoreCriticalDoubleDice = value;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAdvancement(
        RuleDefinitions.AdditionalDamageAdvancement advancement,
        int start = 0,
        int increment = 1,
        int step = 1,
        int begin = 1)
    {
        Definition.damageAdvancement = advancement;
        Definition.DiceByRankTable.SetRange(DiceByRankBuilder.BuildDiceByRankTable(start, increment, step, begin));
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetTriggerCondition(
        RuleDefinitions.AdditionalDamageTriggerCondition trigger)
    {
        Definition.triggerCondition = trigger;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetTriggerCondition(
        ExtraAdditionalDamageTriggerCondition trigger)
    {
        return SetTriggerCondition((RuleDefinitions.AdditionalDamageTriggerCondition)trigger);
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetConditionOperations(
        params ConditionOperationDescription[] operations)
    {
        Definition.ConditionOperations.SetRange(operations);
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetTargetCondition(
        ConditionDefinition requiredCondition,
        RuleDefinitions.AdditionalDamageTriggerCondition trigger)
    {
        Definition.requiredTargetCondition = requiredCondition;
        Definition.triggerCondition = trigger;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage limit)
    {
        Definition.limitedUsage = limit;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAttackModeOnly()
    {
        Definition.attackModeOnly = true;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAttackOnly()
    {
        Definition.attackOnly = true;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetImpactParticleReference(AssetReference asset)
    {
        Definition.impactParticleReference = asset;
        return this;
    }


    internal FeatureDefinitionAdditionalDamageBuilder SetImpactParticleReference(IMagicEffect effect)
    {
        return SetImpactParticleReference(effect.EffectDescription.EffectParticleParameters.impactParticleReference);
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetRequiredProperty(
        RuleDefinitions.RestrictedContextRequiredProperty property)
    {
        Definition.requiredProperty = property;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAdditionalDamageType(
        RuleDefinitions.AdditionalDamageType damageType)
    {
        Definition.additionalDamageType = damageType;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetDamageValueDetermination(
        RuleDefinitions.AdditionalDamageValueDetermination determination)
    {
        Definition.damageValueDetermination = determination;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAddLightSource(bool addLightSource)
    {
        Definition.addLightSource = addLightSource;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetLightSourceForm(LightSourceForm form)
    {
        Definition.lightSourceForm = form;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetRequiredCharacterFamily(CharacterFamilyDefinition value)
    {
        Definition.requiredCharacterFamily = value;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
