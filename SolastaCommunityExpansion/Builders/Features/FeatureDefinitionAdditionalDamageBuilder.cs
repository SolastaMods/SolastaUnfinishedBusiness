using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Builders.Features;

public abstract class
    FeatureDefinitionAdditionalDamageBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition,
        TBuilder>
    where TDefinition : FeatureDefinitionAdditionalDamage
    where TBuilder : FeatureDefinitionAdditionalDamageBuilder<TDefinition, TBuilder>
{
    public TBuilder SetSpecificDamageType(string damageType)
    {
        Definition.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific);
        Definition.SetSpecificDamageType(damageType);
        return This();
    }

    public TBuilder SetDamageDice(RuleDefinitions.DieType dieType, int diceNumber)
    {
        Definition.SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die);
        Definition.SetDamageDiceNumber(diceNumber);
        Definition.SetDamageDieType(dieType);
        return This();
    }

    public TBuilder SetNotificationTag(string tag)
    {
        Definition.SetNotificationTag(tag);
        return This();
    }

    public TBuilder SetNoAdvancement()
    {
        Definition.SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.None);
        return This();
    }

    public TBuilder SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement advancement,
        params (int rank, int dice)[] diceByRank)
    {
        return SetAdvancement(advancement,
            diceByRank.Select(d => DiceByRankBuilder.BuildDiceByRank(d.rank, d.dice)));
    }

    //public TBuilder SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement advancement, params DiceByRank[] diceByRanks)
    //{
    //    return SetAdvancement(advancement, diceByRanks.AsEnumerable());
    //}

    public TBuilder SetAdvancement(RuleDefinitions.AdditionalDamageAdvancement advancement,
        IEnumerable<DiceByRank> diceByRanks)
    {
        Definition.SetDamageAdvancement(advancement);
        Definition.DiceByRankTable.SetRange(diceByRanks);
        return This();
    }

    public TBuilder SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition trigger)
    {
        Definition.SetTriggerCondition(trigger);
        return This();
    }

    public TBuilder SetNoSave()
    {
        Definition.SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None);
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
        Definition.SetRequiredTargetCondition(requiredCondition);
        Definition.SetTriggerCondition(trigger);
        return This();
    }

    public TBuilder SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage limit)
    {
        Definition.SetLimitedUsage(limit);
        return This();
    }

    public TBuilder SetImpactParticleReference(AssetReference asset)
    {
        Definition.SetImpactParticleReference(asset);
        return This();
    }

    public TBuilder SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty property)
    {
        Definition.SetRequiredProperty(property);
        return This();
    }

    public TBuilder SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType damageType)
    {
        Definition.SetAdditionalDamageType(damageType);
        return This();
    }

    public TBuilder SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination determination)
    {
        Definition.SetDamageValueDetermination(determination);
        return This();
    }

    public TBuilder SetDiceByRank(params (int rank, int dice)[] diceByRank)
    {
        Definition.SetDiceByRankTable(diceByRank.Select(d => DiceByRankBuilder.BuildDiceByRank(d.rank, d.dice)));
        return This();
    }

    public TBuilder SetAddLightSource(bool addLightSource)
    {
        Definition.SetAddLightSource(addLightSource);
        return This();
    }

    public TBuilder SetLightSourceForm(LightSourceForm form)
    {
        Definition.SetLightSourceForm(form);
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
        RuleDefinitions.AdditionalDamageRequiredProperty requiredProperty,
        bool attackModeOnly, RuleDefinitions.DieType damageDieType, int damageDiceNumber,
        RuleDefinitions.AdditionalDamageType additionalDamageType,
        string specificDamageType, RuleDefinitions.AdditionalDamageAdvancement damageAdvancement,
        List<DiceByRank> diceByRankTable)
    {
        // this should be first to avoid issues with Damage Value determination
        SetDamageDice(damageDieType, damageDiceNumber);

        Definition.SetNotificationTag(notificationTag);
        Definition.SetLimitedUsage(limitedUsage);
        Definition.SetDamageValueDetermination(damageValueDetermination);
        Definition.SetTriggerCondition(triggerCondition);
        Definition.SetRequiredProperty(requiredProperty);
        Definition.SetAttackModeOnly(attackModeOnly);

        // Does this conflict with SetSpecificDamageType below?
        Definition.SetAdditionalDamageType(additionalDamageType);
        Definition.SetSpecificDamageType(specificDamageType);

        Definition.SetDamageAdvancement(damageAdvancement);
        Definition.DiceByRankTable.SetRange(diceByRankTable);
        Definition.SetDamageDieType(damageDieType);

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
