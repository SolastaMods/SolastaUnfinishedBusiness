using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class AddMarkerToCondition
{
    private readonly string marker;

    public AddMarkerToCondition(string marker)
    {
        this.marker = marker;
    }

    private void Mark(RulesetCondition condition)
    {
        condition.attributes.TryAdd(marker, null);
    }
    
    internal static bool RemoveMarker(RulesetCondition condition, string marker)
    {
        return condition.attributes.Remove(marker);
    }

    internal static bool HasMarker(RulesetCondition condition, string marker)
    {
        return condition.HasAttribute(marker);
    }

    internal static void AddMarkers(RulesetCondition condition)
    {
        var markers = condition.ConditionDefinition.GetAllSubFeaturesOfType<AddMarkerToCondition>();
        foreach (var marker in markers)
        {
            marker.Mark(condition);
        }
    }
}

public class MarkAsRemoveAfterAttacked : AddMarkerToCondition
{
    public const string MARK = "AfterWasAttacked";
    internal static AddMarkerToCondition Instance { get; } = new MarkAsRemoveAfterAttacked();
    
    private MarkAsRemoveAfterAttacked() : base(MARK)
    {
    }

}
