using TA;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMoveStepFinished
{
    public void MoveStepFinished(GameLocationCharacter mover, int3 previousPosition);
}
