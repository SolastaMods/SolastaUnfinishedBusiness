using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChainActionAfterMagicEffect
{
    [UsedImplicitly]
    public CharacterAction GetNextAction(CharacterActionMagicEffect baseEffect);
}
