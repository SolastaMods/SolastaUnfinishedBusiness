using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyActionParams
{
    [UsedImplicitly]
    IEnumerator Modify(CharacterAction characterAction);
}
