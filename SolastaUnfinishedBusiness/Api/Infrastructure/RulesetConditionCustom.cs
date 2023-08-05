using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;


/// <summary>
/// Do not instantiate any of this class, except for Marker
/// You should inherit GetFromPoolAndCopyOriginalRulesetCondition and use it in the interface
/// pool size is 10, since you can compress all things in this condition, you should not apply it everywhere. (But it automatically expand if needed)
/// typical example:
/// class MyRulesetConditionCustom: RulesetConditionCustom<MyRulesetConditionCustom>, IBindToRulesetConditionCustom
///     Initialize Marker, BindingDefinition, Category in static constructor
/// set BindingDefinition with custom subfeatures: Marker, ICustomConditionFeature
///     The type conversion is handled in IBindToRulesetConditionCustom when the condition definition marked by Marker is added to character
///     then you can handle condition added or removed in ICustomConditionFeature
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class RulesetConditionCustom<T> : RulesetCondition, IForceConditionCategory where T : RulesetConditionCustom<T>, IBindToRulesetConditionCustom
{
    // A static object pool to prevent memory allocation (following vanilla practice)
    public static ObjectPool<T> myObjectPool = new ObjectPool<T>(10);
    // Please be careful with these static fields initialization time
    public static ConditionDefinition BindingDefinition;
    public static T Marker;
    public static string Category;

    public override void Unregister()
    {
        myObjectPool.Return(this as T);
        IRulesetEntityService service = ServiceRepository.GetService<IRulesetEntityService>();
        if (service != null)
        {
            if (!service.RulesetEntities.ContainsKey(this.guid))
            {
                Trace.LogWarning("Trying to unregister unknown entity {0} of Id {1}", new object[]
                {
                    this.SystemName,
                    this.guid.ToString()
                });
            }
            service.UnregisterEntity(this);
        }
    }

    protected abstract void ClearCustomStates();

    public static T GetFromPoolAndCopyOriginalRulesetCondition(
        RulesetCondition rulesetCondition)
    {
        var customCondition = myObjectPool.Get();
        customCondition.ResetGuid();
        customCondition.Clear();
        customCondition.ClearCustomStates();
        if (rulesetCondition is T)
        {
            Main.Error($"Please do not instantiate {nameof(T)} and add to character!");
            return null;
        }
        if (BindingDefinition is null ||
            string.IsNullOrEmpty(Category) ||
            Marker is null)
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
        supportCondition = rulesetCharacter.TryGetConditionOfCategoryAndType(Category, BindingDefinition.Name, out var rulesetCondition) ?
            rulesetCondition as T :
            null;
        if (supportCondition is null)
        {
            return false;
        }
        return true;
    }

    public virtual string GetForcedCategory(RulesetActor actor, RulesetCondition newCondition, string category)
    {
        return Category;
    }
}
