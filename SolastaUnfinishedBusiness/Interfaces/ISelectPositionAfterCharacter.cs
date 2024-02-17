using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.Interfaces;

internal static class SelectPositionAfterCharacter
{
    [UsedImplicitly] internal static readonly ConditionDefinition ConditionSelectedCharacter =
        ConditionDefinitionBuilder
            .Create("ConditionSelectedCharacter")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();
}

public interface ISelectPositionAfterCharacter
{
    public int PositionRange { get; }
}
