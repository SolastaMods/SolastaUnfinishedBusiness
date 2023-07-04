using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IUsePowerFinishedByMe
{
    [UsedImplicitly]
    public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power);
}
