using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(RulesetActor))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static partial class RulesetActorExtensions
{
    public static T AddAllConditions<T>(this T entity, params RulesetCondition[] value)
        where T : RulesetActor
    {
        AddAllConditions(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddAllConditions<T>(this T entity, IEnumerable<RulesetCondition> value)
        where T : RulesetActor
    {
        entity.AllConditions.AddRange(value);
        return entity;
    }

    public static T AddFeaturesToBrowse<T>(this T entity, params FeatureDefinition[] value)
        where T : RulesetActor
    {
        AddFeaturesToBrowse(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddFeaturesToBrowse<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : RulesetActor
    {
        entity.FeaturesToBrowse.AddRange(value);
        return entity;
    }

    public static T ClearAllConditions<T>(this T entity)
        where T : RulesetActor
    {
        entity.AllConditions.Clear();
        return entity;
    }

    public static T ClearFeaturesToBrowse<T>(this T entity)
        where T : RulesetActor
    {
        entity.FeaturesToBrowse.Clear();
        return entity;
    }

    public static List<ISavingThrowAffinityProvider> GetAccountedProviders<T>(this T entity)
        where T : RulesetActor
    {
        return entity.accountedProviders;
    }

    public static List<RulesetCondition> GetAllConditionsForEnumeration<T>(this T entity)
        where T : RulesetActor
    {
        return entity.allConditionsForEnumeration;
    }

    public static List<RulesetCondition> GetConditionsToExecute<T>(this T entity)
        where T : RulesetActor
    {
        return entity.conditionsToExecute;
    }

    public static List<RulesetCondition> GetConditionsToTerminate<T>(this T entity)
        where T : RulesetActor
    {
        return entity.conditionsToTerminate;
    }

    public static List<EffectForm> GetDummyEffectForms<T>(this T entity)
        where T : RulesetActor
    {
        return entity.dummyEffectForms;
    }

    public static List<RulesetCondition> GetMatchingCancellingConditions<T>(this T entity)
        where T : RulesetActor
    {
        return entity.matchingCancellingConditions;
    }

    public static List<RulesetCondition> GetMatchingInterruptionConditions<T>(this T entity)
        where T : RulesetActor
    {
        return entity.matchingInterruptionConditions;
    }

    public static List<RulesetCondition> GetMatchingOccurenceConditions<T>(this T entity)
        where T : RulesetActor
    {
        return entity.matchingOccurenceConditions;
    }

    public static List<RulesetCondition> GetMatchingRealTimeLaspeConditions<T>(this T entity)
        where T : RulesetActor
    {
        return entity.matchingRealTimeLaspeConditions;
    }

    public static List<RulesetCondition> GetMatchingRestConditions<T>(this T entity)
        where T : RulesetActor
    {
        return entity.matchingRestConditions;
    }

    public static T SetAbilityScoreIncreased<T>(this T entity, RulesetActor.AbilityScoreIncreasedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AbilityScoreIncreased>k__BackingField", value);
        return entity;
    }

    public static T SetActionModifier<T>(this T entity, ActionModifier value)
        where T : RulesetActor
    {
        entity.SetField("actionModifier", value);
        return entity;
    }

    public static T SetActorReplaced<T>(this T entity, RulesetActor.ActorReplacedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ActorReplaced>k__BackingField", value);
        return entity;
    }

    public static T SetAdditionalSaveDieRolled<T>(this T entity, RulesetActor.AdditionalSaveDieRolledHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AdditionalSaveDieRolled>k__BackingField", value);
        return entity;
    }

    public static T SetAllConditions<T>(this T entity, params RulesetCondition[] value)
        where T : RulesetActor
    {
        SetAllConditions(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetAllConditions<T>(this T entity, IEnumerable<RulesetCondition> value)
        where T : RulesetActor
    {
        entity.AllConditions.SetRange(value);
        return entity;
    }

    public static T SetAlterationInflicted<T>(this T entity, RulesetActor.AlterationInflictedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AlterationInflicted>k__BackingField", value);
        return entity;
    }

    public static T SetAttackAutomaticCritical<T>(this T entity, RulesetActor.AttackAutomaticCriticalHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AttackAutomaticCritical>k__BackingField", value);
        return entity;
    }

    public static T SetAttackAutomaticHit<T>(this T entity, RulesetActor.AttackAutomaticHitHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AttackAutomaticHit>k__BackingField", value);
        return entity;
    }

    public static T SetAttackInitiated<T>(this T entity, RulesetActor.AttackInitiatedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AttackInitiated>k__BackingField", value);
        return entity;
    }

    public static T SetAttackRolled<T>(this T entity, RulesetActor.AttackRolledHandler value)
        where T : RulesetActor
    {
        entity.SetField("<AttackRolled>k__BackingField", value);
        return entity;
    }

    public static T SetConditionAdded<T>(this T entity, RulesetActor.ConditionAddedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ConditionAdded>k__BackingField", value);
        return entity;
    }

    public static T SetConditionOccurenceReached<T>(this T entity,
        RulesetActor.ConditionOccurenceReachedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ConditionOccurenceReached>k__BackingField", value);
        return entity;
    }

    public static T SetConditionRemoved<T>(this T entity, RulesetActor.ConditionRemovedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ConditionRemoved>k__BackingField", value);
        return entity;
    }

    public static T SetConditionRemovedForVisual<T>(this T entity,
        RulesetActor.ConditionRemovedForVisualHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ConditionRemovedForVisual>k__BackingField", value);
        return entity;
    }

    public static T SetConditionSaveRerollRequested<T>(this T entity,
        RulesetActor.ConditionSaveRerollRequestedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ConditionSaveRerollRequested>k__BackingField", value);
        return entity;
    }

    public static T SetCurrentHitPoints<T>(this T entity, Int32 value)
        where T : RulesetActor
    {
        entity.SetProperty("CurrentHitPoints", value);
        return entity;
    }

    public static T SetDamageAltered<T>(this T entity, RulesetActor.DamageAlteredHandler value)
        where T : RulesetActor
    {
        entity.SetField("<DamageAltered>k__BackingField", value);
        return entity;
    }

    public static T SetDamageFormsTriggered<T>(this T entity, RulesetActor.DamageFormsTriggeredHandler value)
        where T : RulesetActor
    {
        entity.SetField("<DamageFormsTriggered>k__BackingField", value);
        return entity;
    }

    public static T SetDamageHalved<T>(this T entity, RulesetActor.DamageHalvedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<DamageHalved>k__BackingField", value);
        return entity;
    }

    public static T SetDamageReceived<T>(this T entity, RulesetActor.DamageReceivedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<DamageReceived>k__BackingField", value);
        return entity;
    }

    public static T SetDamageReduced<T>(this T entity, RulesetActor.DamageReducedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<DamageReduced>k__BackingField", value);
        return entity;
    }

    public static T SetDieRerolled<T>(this T entity, RulesetActor.DieRerolledHandler value)
        where T : RulesetActor
    {
        entity.SetField("<DieRerolled>k__BackingField", value);
        return entity;
    }

    public static T SetFeaturesToBrowse<T>(this T entity, params FeatureDefinition[] value)
        where T : RulesetActor
    {
        SetFeaturesToBrowse(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetFeaturesToBrowse<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : RulesetActor
    {
        entity.FeaturesToBrowse.SetRange(value);
        return entity;
    }

    public static T SetForcedName<T>(this T entity, String value)
        where T : RulesetActor
    {
        entity.SetField("<ForcedName>k__BackingField", value);
        return entity;
    }

    public static T SetHealingFormsTriggered<T>(this T entity, RulesetActor.HealingFormsTriggeredHandler value)
        where T : RulesetActor
    {
        entity.SetField("<HealingFormsTriggered>k__BackingField", value);
        return entity;
    }

    public static T SetImmuneToCondition<T>(this T entity, RulesetActor.ImmuneToConditionHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ImmuneToCondition>k__BackingField", value);
        return entity;
    }

    public static T SetImmuneToDamage<T>(this T entity, RulesetActor.ImmuneToDamageHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ImmuneToDamage>k__BackingField", value);
        return entity;
    }

    public static T SetImmuneToIncomingDamageNotified<T>(this T entity,
        RulesetActor.IncomingDamageNotifiedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ImmuneToIncomingDamageNotified>k__BackingField", value);
        return entity;
    }

    public static T SetImmuneToSpell<T>(this T entity, RulesetActor.ImmuneToSpellHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ImmuneToSpell>k__BackingField", value);
        return entity;
    }

    public static T SetImmuneToSpellLevel<T>(this T entity, RulesetActor.ImmuneToSpellLevelHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ImmuneToSpellLevel>k__BackingField", value);
        return entity;
    }

    public static T SetIncomingAttackRolled<T>(this T entity, RulesetActor.IncomingAttackRolledHandler value)
        where T : RulesetActor
    {
        entity.SetField("<IncomingAttackRolled>k__BackingField", value);
        return entity;
    }

    public static T SetIncomingDamageNotified<T>(this T entity, RulesetActor.IncomingDamageNotifiedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<IncomingDamageNotified>k__BackingField", value);
        return entity;
    }

    public static T SetMatchingCancellingCondition<T>(this T entity, Boolean value)
        where T : RulesetActor
    {
        entity.SetField("matchingCancellingCondition", value);
        return entity;
    }

    public static T SetMatchingInterruption<T>(this T entity, Boolean value)
        where T : RulesetActor
    {
        entity.SetField("matchingInterruption", value);
        return entity;
    }

    public static T SetMaxExtentY<T>(this T entity, Int32 value)
        where T : RulesetActor
    {
        entity.SetProperty("MaxExtentY", value);
        return entity;
    }

    public static T SetName<T>(this T entity, String value)
        where T : RulesetActor
    {
        entity.Name = value;
        return entity;
    }

    public static T SetPostLoaded<T>(this T entity, Boolean value)
        where T : RulesetActor
    {
        entity.SetField("<PostLoaded>k__BackingField", value);
        return entity;
    }

    public static T SetReplacedAbilityScoreForSave<T>(this T entity,
        RulesetActor.ReplacedAbilityScoreForSaveHandler value)
        where T : RulesetActor
    {
        entity.SetField("<ReplacedAbilityScoreForSave>k__BackingField", value);
        return entity;
    }

    public static T SetSaveRolled<T>(this T entity, RulesetActor.SaveRolledHandler value)
        where T : RulesetActor
    {
        entity.SetField("<SaveRolled>k__BackingField", value);
        return entity;
    }

    public static T SetSide<T>(this T entity, Side value)
        where T : RulesetActor
    {
        entity.SetField("side", value);
        return entity;
    }

    public static T SetSizeParams<T>(this T entity, RulesetActor.SizeParameters value)
        where T : RulesetActor
    {
        entity.SizeParams = value;
        return entity;
    }

    public static T SetSortIndex<T>(this T entity, Int32 value)
        where T : RulesetActor
    {
        entity.SortIndex = value;
        return entity;
    }

    public static T SetSpellDissipated<T>(this T entity, RulesetActor.SpellDissipatedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<SpellDissipated>k__BackingField", value);
        return entity;
    }

    public static T SetTagRevealed<T>(this T entity, RulesetActor.TagRevealedHandler value)
        where T : RulesetActor
    {
        entity.SetField("<TagRevealed>k__BackingField", value);
        return entity;
    }
}
