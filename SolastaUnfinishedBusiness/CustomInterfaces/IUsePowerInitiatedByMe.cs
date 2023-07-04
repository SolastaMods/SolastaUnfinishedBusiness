using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IUsePowerInitiatedByMe
{
    [UsedImplicitly]
    IEnumerator OnUsePowerInitiatedByMe(CharacterAction action, FeatureDefinitionPower power);
}
