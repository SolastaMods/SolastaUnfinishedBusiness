using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAdditionalDamageBuilder
    : DefinitionBuilder<FeatureDefinitionAdditionalDamage, FeatureDefinitionAdditionalDamageBuilder>
{
    internal FeatureDefinitionAdditionalDamageBuilder SetSpecificDamageType(string damageType)
    {
        Definition.additionalDamageType = AdditionalDamageType.Specific;
        Definition.specificDamageType = damageType;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetFlatDamageBonus(int bonus)
    {
        Definition.flatBonus = bonus;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetDamageDice(DieType dieType, int diceNumber)
    {
        Definition.damageValueDetermination = AdditionalDamageValueDetermination.Die;
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

    [UsedImplicitly]
    internal FeatureDefinitionAdditionalDamageBuilder SetAdvancement(
        AdditionalDamageAdvancement advancement,
        IEnumerable<DiceByRank> ranks)
    {
        Definition.damageAdvancement = advancement;
        Definition.DiceByRankTable.SetRange(ranks);
        return this;
    }

    [UsedImplicitly]
    internal FeatureDefinitionAdditionalDamageBuilder SetAdvancement(
        ExtraAdditionalDamageAdvancement advancement,
        IEnumerable<DiceByRank> ranks)
    {
        return SetAdvancement((AdditionalDamageAdvancement)advancement, ranks);
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAdvancement(
        AdditionalDamageAdvancement advancement,
        int start = 1,
        int increment = 1,
        int step = 1,
        int begin = 1)
    {
        return SetAdvancement(advancement, DiceByRankBuilder.BuildDiceByRankTable(start, increment, step, begin));
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetTriggerCondition(
        AdditionalDamageTriggerCondition trigger)
    {
        Definition.triggerCondition = trigger;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetTriggerCondition(
        ExtraAdditionalDamageTriggerCondition trigger)
    {
        return SetTriggerCondition((AdditionalDamageTriggerCondition)trigger);
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetRequiredSpecificSpell(SpellDefinition spell)
    {
        Definition.requiredSpecificSpell = spell;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetConditionOperations(
        params ConditionOperationDescription[] operations)
    {
        Definition.ConditionOperations.SetRange(operations);
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder AddConditionOperation(ConditionOperationDescription operation)
    {
        Definition.ConditionOperations.Add(operation);
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder AddConditionOperation(
        ConditionOperationDescription.ConditionOperation operation, ConditionDefinition condition)
    {
        Definition.ConditionOperations.Add(
            new ConditionOperationDescription { operation = operation, conditionDefinition = condition });
        return this;
    }

#if false
    internal FeatureDefinitionAdditionalDamageBuilder AddCondition(ConditionDefinition condition)
    {
        return AddConditionOperation(new ConditionOperationDescription
        {
            operation = ConditionOperationDescription.ConditionOperation.Add, conditionDefinition = condition
        });
    }
#endif

    internal FeatureDefinitionAdditionalDamageBuilder SetTargetCondition(
        ConditionDefinition requiredCondition,
        AdditionalDamageTriggerCondition trigger)
    {
        Definition.requiredTargetCondition = requiredCondition;
        Definition.triggerCondition = trigger;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetFrequencyLimit(FeatureLimitedUsage limit)
    {
        Definition.limitedUsage = limit;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAttackOnly()
    {
        Definition.attackOnly = true;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetAttackModeOnly()
    {
        Definition.attackModeOnly = true;
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
        RestrictedContextRequiredProperty property)
    {
        Definition.requiredProperty = property;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetDamageValueDetermination(
        AdditionalDamageValueDetermination determination)
    {
        Definition.damageValueDetermination = determination;
        return this;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetDamageValueDetermination(
        ExtraAdditionalDamageValueDetermination determination)
    {
        return SetDamageValueDetermination((AdditionalDamageValueDetermination)determination);
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

#if false
    internal FeatureDefinitionAdditionalDamageBuilder AddLightSourceForm(LightSourceForm form)
    {
        Definition.addLightSource = true;
        Definition.lightSourceForm = form;
        return this;
    }
#endif

    internal FeatureDefinitionAdditionalDamageBuilder SetRequiredCharacterFamily(CharacterFamilyDefinition value)
    {
        Definition.requiredCharacterFamily = value;
        return this;
    }

    protected override void Initialise()
    {
        base.Initialise();
        Definition.damageAdvancement = AdditionalDamageAdvancement.None;
        Definition.damageDiceNumber = 0;
        Definition.flatBonus = 0;
        Definition.notificationTag = string.Empty;
    }

    internal FeatureDefinitionAdditionalDamageBuilder SetSavingThrowData(
        EffectDifficultyClassComputation dcComputation =
            EffectDifficultyClassComputation.SpellCastingFeature,
        EffectSavingThrowType damageSaveAffinity = EffectSavingThrowType.Negates,
        string savingThrowAbility = AttributeDefinitions.Constitution,
        string savingThrowDcAbilityModifier = AttributeDefinitions.Wisdom,
        int savingThrowDc = 12)
    {
        Definition.hasSavingThrow = true;
        Definition.dcComputation = dcComputation;
        Definition.damageSaveAffinity = damageSaveAffinity;
        Definition.savingThrowAbility = savingThrowAbility;
        Definition.savingThrowDCAbilityModifier = savingThrowDcAbilityModifier;
        Definition.savingThrowDC = savingThrowDc;

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
