using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

/// <summary>
///     Do not instantiate any of this class, except for Marker
///     You should inherit GetFromPoolAndCopyOriginalRulesetCondition and use it in the interface
///     pool size is 10, since you can compress all things in this condition, you should not apply it everywhere. (But it
///     automatically expand if needed)
///     typical example:
///     class MyRulesetConditionCustom: RulesetConditionCustom-MyRulesetConditionCustom, IBindToRulesetConditionCustom
///     Initialize Marker, BindingDefinition, Category in static constructor
///     set BindingDefinition with custom sub features: Marker, OnConditionAddedOrRemoved
///     The type conversion is handled in IBindToRulesetConditionCustom when the condition definition marked by Marker is
///     added to character
///     then you can handle condition added or removed in OnConditionAddedOrRemoved
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class RulesetConditionCustom<T> : RulesetCondition, IForceConditionCategory
    where T : RulesetConditionCustom<T>, IBindToRulesetConditionCustom
{
    // A static object pool to prevent memory allocation (following vanilla practice)
    private static readonly ObjectPool<T> MyObjectPool = new(10);

    // Please be careful with these static fields initialization time
    // ReSharper disable once StaticMemberInGenericType
    public static ConditionDefinition BindingDefinition;
    protected static T Marker;

    // ReSharper disable once StaticMemberInGenericType
    protected static string Category;

    public virtual string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category)
    {
        return Category;
    }

    public override void Unregister()
    {
        MyObjectPool.Return(this as T);

        var service = ServiceRepository.GetService<IRulesetEntityService>();

        if (service == null)
        {
            return;
        }

        if (!service.RulesetEntities.ContainsKey(guid))
        {
            Trace.LogWarning("Trying to unregister unknown entity {0} of Id {1}", SystemName, guid.ToString());
        }

        service.UnregisterEntity(this);
    }

    protected abstract void ClearCustomStates();

    protected static T GetFromPoolAndCopyOriginalRulesetCondition(RulesetCondition rulesetCondition)
    {
        var customCondition = MyObjectPool.Get();

        customCondition.ResetGuid();
        customCondition.Clear();
        customCondition.ClearCustomStates();

        if (rulesetCondition is T)
        {
            Main.Error($"Please do not instantiate {nameof(T)} and add to character!");
            return null;
        }

        if (BindingDefinition is null || string.IsNullOrEmpty(Category) || Marker is null)
        {
            Main.Error($"Custom RulesetCondition {nameof(T)} compulsory fields unset!");
            return null;
        }

        customCondition.targetGuid = rulesetCondition.targetGuid;
        customCondition.conditionDefinition = rulesetCondition.conditionDefinition;
        customCondition.durationType = rulesetCondition.DurationType;
        customCondition.durationParameter = rulesetCondition.DurationParameter;
        customCondition.remainingRounds = rulesetCondition.RemainingRounds;
        customCondition.endOccurence = rulesetCondition.EndOccurence;
        customCondition.sourceGuid = rulesetCondition.SourceGuid;
        customCondition.sourceFactionName = rulesetCondition.SourceFactionName;
        customCondition.effectLevel = rulesetCondition.EffectLevel;
        customCondition.effectDefinitionName = rulesetCondition.EffectDefinitionName;
        customCondition.amount = rulesetCondition.Amount;
        customCondition.sourceAbilityBonus = rulesetCondition.SourceAbilityBonus;
        customCondition.sourceProficiencyBonus = rulesetCondition.SourceProficiencyBonus;
        customCondition.doNotTerminateWhenRemoved = rulesetCondition.DoNotTerminateWhenRemoved;

        return customCondition;
    }

    public static bool GetCustomConditionFromCharacter(
        RulesetCharacter rulesetCharacter,
        out T supportCondition)
    {
        // Main.Info($"Category is {Category}, Definition is {BindingDefinition.Name}");
        supportCondition =
            rulesetCharacter.TryGetConditionOfCategoryAndType(Category, BindingDefinition.Name,
                out var rulesetCondition)
                ? rulesetCondition as T
                : null;

        return supportCondition is not null;
    }
}
