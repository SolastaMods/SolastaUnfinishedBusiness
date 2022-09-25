using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders.Features;

public abstract class
    FeatureDefinitionAdditionalDamageBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition,
        TBuilder>
    where TDefinition : FeatureDefinitionAdditionalDamage
    where TBuilder : FeatureDefinitionAdditionalDamageBuilder<TDefinition, TBuilder>
{
    public TBuilder SetSpecificDamageType(string damageType)
    {
        Definition.additionalDamageType = RuleDefinitions.AdditionalDamageType.Specific;
        Definition.specificDamageType = damageType;
        return This();
    }

    public TBuilder SetDamageDice(RuleDefinitions.DieType dieType, int diceNumber)
    {
        Definition.damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.Die;
        Definition.damageDiceNumber = diceNumber;
        Definition.damageDieType = dieType;
        return This();
    }

    public TBuilder SetNotificationTag(string tag)
    {
        Definition.notificationTag = tag;
        return This();
    }

    public TBuilder SetNoAdvancement()
    {
        Definition.damageAdvancement = RuleDefinitions.AdditionalDamageAdvancement.None;
        return This();
    }

    public TBuilder SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement advancement,
        params (int rank, int dice)[] diceByRank)
    {
        return SetAdvancement(advancement,
            diceByRank.Select(d => DiceByRankBuilder.BuildDiceByRank(d.rank, d.dice)));
    }

    public TBuilder SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement advancement,
        params DiceByRank[] diceByRanks)
    {
        return SetAdvancement(advancement, diceByRanks.AsEnumerable());
    }

    public TBuilder SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement advancement,
        IEnumerable<DiceByRank> diceByRanks)
    {
        Definition.damageAdvancement = advancement;
        Definition.DiceByRankTable.SetRange(diceByRanks);
        return This();
    }

    public TBuilder SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition trigger)
    {
        Definition.triggerCondition = trigger;
        return This();
    }

    public TBuilder SetNoSave()
    {
        Definition.damageSaveAffinity = RuleDefinitions.EffectSavingThrowType.None;
        return This();
    }

    public TBuilder SetConditionOperations(params ConditionOperationDescription[] operations)
    {
        return SetConditionOperations(operations.AsEnumerable());
    }

    public TBuilder SetConditionOperations(IEnumerable<ConditionOperationDescription> operations)
    {
        Definition.ConditionOperations.SetRange(operations);
        return This();
    }

    public TBuilder SetTargetCondition(ConditionDefinition requiredCondition,
        RuleDefinitions.AdditionalDamageTriggerCondition trigger)
    {
        Definition.requiredTargetCondition = requiredCondition;
        Definition.triggerCondition = trigger;
        return This();
    }

    public TBuilder SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage limit)
    {
        Definition.limitedUsage = limit;
        return This();
    }

    public TBuilder SetImpactParticleReference(AssetReference asset)
    {
        Definition.impactParticleReference = asset;
        return This();
    }

    public TBuilder SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty property)
    {
        Definition.requiredProperty = property;
        return This();
    }

    public TBuilder SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType damageType)
    {
        Definition.additionalDamageType = damageType;
        return This();
    }

    public TBuilder SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination determination)
    {
        Definition.damageValueDetermination = determination;
        return This();
    }

    public TBuilder SetAddLightSource(bool addLightSource)
    {
        Definition.addLightSource = addLightSource;
        return This();
    }

    public TBuilder SetLightSourceForm(LightSourceForm form)
    {
        Definition.lightSourceForm = form;
        return This();
    }

    #region Constructors

    protected FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(TDefinition original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(TDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}

public class FeatureDefinitionAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder<
    FeatureDefinitionAdditionalDamage, FeatureDefinitionAdditionalDamageBuilder>
{
    // Can we give this a specific name? ConfigureXXX.
    // Or move into ArcaneFighter.
    public FeatureDefinitionAdditionalDamageBuilder Configure(
        string notificationTag, RuleDefinitions.FeatureLimitedUsage limitedUsage,
        RuleDefinitions.AdditionalDamageValueDetermination damageValueDetermination,
        RuleDefinitions.AdditionalDamageTriggerCondition triggerCondition,
        RuleDefinitions.RestrictedContextRequiredProperty requiredProperty,
        bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber,
        RuleDefinitions.AdditionalDamageType additionalDamageType,
        string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement,
        [NotNull] IEnumerable<DiceByRank> diceByRankTable,
        bool ignoreCriticalDoubleDice = false)
    {
        // this should be first to avoid issues with Damage Value determination
        SetDamageDice(damageDieType, damageDiceNumber);

        Definition.notificationTag = notificationTag;
        Definition.limitedUsage = limitedUsage;
        Definition.damageValueDetermination = damageValueDetermination;
        Definition.triggerCondition = triggerCondition;
        Definition.requiredProperty = requiredProperty;
        Definition.attackModeOnly = attackModeOnly;

        // Does this conflict with SetSpecificDamageType below?
        Definition.additionalDamageType = additionalDamageType;
        Definition.specificDamageType = specificDamageType;

        Definition.damageAdvancement = damageAdvancement;
        Definition.DiceByRankTable.SetRange(diceByRankTable);
        Definition.damageDieType = damageDieType;

        Definition.ignoreCriticalDoubleDice = ignoreCriticalDoubleDice;
        return This();
    }

    #region Constructors

    protected FeatureDefinitionAdditionalDamageBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalDamageBuilder(FeatureDefinitionAdditionalDamage original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
