using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionAdditionalDamage))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionAdditionalDamageExtensions
{
    public static T AddConditionOperations<T>(this T entity, params ConditionOperationDescription[] value)
        where T : FeatureDefinitionAdditionalDamage
    {
        AddConditionOperations(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddConditionOperations<T>(this T entity, IEnumerable<ConditionOperationDescription> value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.ConditionOperations.AddRange(value);
        return entity;
    }

    public static T AddDiceByRankTable<T>(this T entity, params DiceByRank[] value)
        where T : FeatureDefinitionAdditionalDamage
    {
        AddDiceByRankTable(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddDiceByRankTable<T>(this T entity, IEnumerable<DiceByRank> value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.DiceByRankTable.AddRange(value);
        return entity;
    }

    public static T AddFamiliesWithAdditionalDice<T>(this T entity, params String[] value)
        where T : FeatureDefinitionAdditionalDamage
    {
        AddFamiliesWithAdditionalDice(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddFamiliesWithAdditionalDice<T>(this T entity, IEnumerable<String> value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.FamiliesWithAdditionalDice.AddRange(value);
        return entity;
    }

    public static T ClearConditionOperations<T>(this T entity)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.ConditionOperations.Clear();
        return entity;
    }

    public static T ClearDiceByRankTable<T>(this T entity)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.DiceByRankTable.Clear();
        return entity;
    }

    public static T ClearFamiliesWithAdditionalDice<T>(this T entity)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.FamiliesWithAdditionalDice.Clear();
        return entity;
    }

    public static T SetAcidImpactParticleReference<T>(this T entity,
        AssetReference value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("acidImpactParticleReference", value);
        return entity;
    }

    public static T SetAdditionalDamageType<T>(this T entity, AdditionalDamageType value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("additionalDamageType", value);
        return entity;
    }

    public static T SetAddLightSource<T>(this T entity, Boolean value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("addLightSource", value);
        return entity;
    }

    public static T SetAttackModeOnly<T>(this T entity, Boolean value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("attackModeOnly", value);
        return entity;
    }

    public static T SetColdImpactParticleReference<T>(this T entity,
        AssetReference value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("coldImpactParticleReference", value);
        return entity;
    }

    public static T SetComputeDescription<T>(this T entity, Boolean value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("computeDescription", value);
        return entity;
    }

    public static T SetConditionOperations<T>(this T entity, params ConditionOperationDescription[] value)
        where T : FeatureDefinitionAdditionalDamage
    {
        SetConditionOperations(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetConditionOperations<T>(this T entity, IEnumerable<ConditionOperationDescription> value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.ConditionOperations.SetRange(value);
        return entity;
    }

    public static T SetDamageAdvancement<T>(this T entity, AdditionalDamageAdvancement value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("damageAdvancement", value);
        return entity;
    }

    public static T SetDamageDiceNumber<T>(this T entity, Int32 value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("damageDiceNumber", value);
        return entity;
    }

    public static T SetDamageDieType<T>(this T entity, DieType value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("damageDieType", value);
        return entity;
    }

    public static T SetDamageSaveAffinity<T>(this T entity, EffectSavingThrowType value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("damageSaveAffinity", value);
        return entity;
    }

    public static T SetDamageValueDetermination<T>(this T entity, AdditionalDamageValueDetermination value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("damageValueDetermination", value);
        return entity;
    }

    public static T SetDcComputation<T>(this T entity, EffectDifficultyClassComputation value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("dcComputation", value);
        return entity;
    }

    public static T SetDiceByRankTable<T>(this T entity, params DiceByRank[] value)
        where T : FeatureDefinitionAdditionalDamage
    {
        SetDiceByRankTable(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetDiceByRankTable<T>(this T entity, IEnumerable<DiceByRank> value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.DiceByRankTable.SetRange(value);
        return entity;
    }

    public static T SetFamiliesDiceNumber<T>(this T entity, Int32 value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("familiesDiceNumber", value);
        return entity;
    }

    public static T SetFamiliesWithAdditionalDice<T>(this T entity, params String[] value)
        where T : FeatureDefinitionAdditionalDamage
    {
        SetFamiliesWithAdditionalDice(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetFamiliesWithAdditionalDice<T>(this T entity, IEnumerable<String> value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.FamiliesWithAdditionalDice.SetRange(value);
        return entity;
    }

    public static T SetFireImpactParticleReference<T>(this T entity,
        AssetReference value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("fireImpactParticleReference", value);
        return entity;
    }

    public static T SetHasSavingThrow<T>(this T entity, Boolean value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("hasSavingThrow", value);
        return entity;
    }

    public static T SetIgnoreCriticalDoubleDice<T>(this T entity, Boolean value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("ignoreCriticalDoubleDice", value);
        return entity;
    }

    public static T SetImpactParticleReference<T>(this T entity, AssetReference value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("impactParticleReference", value);
        return entity;
    }

    public static T SetLightningImpactParticleReference<T>(this T entity,
        AssetReference value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("lightningImpactParticleReference", value);
        return entity;
    }

    public static T SetLightSourceForm<T>(this T entity, LightSourceForm value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("lightSourceForm", value);
        return entity;
    }

    public static T SetLimitedUsage<T>(this T entity, FeatureLimitedUsage value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("limitedUsage", value);
        return entity;
    }

    public static T SetNotificationTag<T>(this T entity, String value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("notificationTag", value);
        return entity;
    }

    public static T SetParticlesBasedOnAncestryDamageType<T>(this T entity, Boolean value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("particlesBasedOnAncestryDamageType", value);
        return entity;
    }

    public static T SetPoisonImpactParticleReference<T>(this T entity,
        AssetReference value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("poisonImpactParticleReference", value);
        return entity;
    }

    public static T SetRequiredCharacterFamily<T>(this T entity, CharacterFamilyDefinition value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("requiredCharacterFamily", value);
        return entity;
    }

    public static T SetRequiredProperty<T>(this T entity, AdditionalDamageRequiredProperty value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("requiredProperty", value);
        return entity;
    }

    public static T SetRequiredTargetCondition<T>(this T entity, ConditionDefinition value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("requiredTargetCondition", value);
        return entity;
    }

    public static T SetRequiredTargetCreatureTag<T>(this T entity, String value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("requiredTargetCreatureTag", value);
        return entity;
    }

    public static T SetRequiredTargetSenseType<T>(this T entity, SenseMode.Type value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("requiredTargetSenseType", value);
        return entity;
    }

    public static T SetSavingThrowAbility<T>(this T entity, String value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("savingThrowAbility", value);
        return entity;
    }

    public static T SetSavingThrowDC<T>(this T entity, Int32 value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("savingThrowDC", value);
        return entity;
    }

    public static T SetSpecificDamageType<T>(this T entity, String value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("specificDamageType", value);
        return entity;
    }

    public static T SetTriggerCondition<T>(this T entity, AdditionalDamageTriggerCondition value)
        where T : FeatureDefinitionAdditionalDamage
    {
        entity.SetField("triggerCondition", value);
        return entity;
    }
}
