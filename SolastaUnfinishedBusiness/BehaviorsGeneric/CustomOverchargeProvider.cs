using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

internal interface ICustomOverchargeProvider
{
    public (int, int)[] OverchargeSteps(RulesetCharacter character);
}

internal delegate (int, int)[] OverchargeStepsHandler(RulesetCharacter character);

[UsedImplicitly]
internal class CustomOverchargeProvider : ICustomOverchargeProvider
{
    private readonly OverchargeStepsHandler _handler;

    internal CustomOverchargeProvider(OverchargeStepsHandler handler)
    {
        _handler = handler;
    }

    public (int, int)[] OverchargeSteps(RulesetCharacter character)
    {
        return _handler(character);
    }

    internal static int GetAdvancementFromOvercharge(int overcharge, (int, int)[] steps)
    {
        if (steps == null || steps.Length == 0)
        {
            return 0;
        }

        foreach (var (item, i) in steps)
        {
            if (item == overcharge)
            {
                return i;
            }
        }

        return 0;
    }
}
