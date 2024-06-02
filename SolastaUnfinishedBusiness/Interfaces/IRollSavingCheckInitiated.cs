using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IRollSavingCheckInitiated
{
    [UsedImplicitly]
    public void OnRollSavingCheckInitiated(
        RulesetCharacter defender,
        int saveDC,
        string damageType,
        ref ActionModifier actionModifier,
        ref int modifier);
}
