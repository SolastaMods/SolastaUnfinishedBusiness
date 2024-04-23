using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ISelectPositionAfterCharacter
{
    public int PositionRange { get; }

    public bool EnforcePositionSelection([UsedImplicitly] CursorLocationSelectPosition cursorLocationSelectPosition);
}
