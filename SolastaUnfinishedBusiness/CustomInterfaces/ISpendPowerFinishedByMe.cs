using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ISpendPowerFinishedByMe
{
    [UsedImplicitly]
    public IEnumerator OnSpendPowerFinishedByMe(CharacterActionSpendPower action, FeatureDefinitionPower power);
}
