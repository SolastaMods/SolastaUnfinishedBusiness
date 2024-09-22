 using TA;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMoveStepStarted
{
    public void MoveStepStarted(GameLocationCharacter mover, int3 source, int3 destination);
}
