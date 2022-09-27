namespace SolastaUnfinishedBusiness.CustomBehaviors;

public interface ICustomOverchargeProvider
{
    public (int, int)[] OverchargeSteps(RulesetCharacter character);
}

public delegate (int, int)[] OverchargeStepsHandler(RulesetCharacter character);

public class CustomOverchargeProvider: ICustomOverchargeProvider
{
    private readonly OverchargeStepsHandler _handler;

    public CustomOverchargeProvider(OverchargeStepsHandler handler)
    {
        this._handler = handler;
    }
    
    public (int, int)[] OverchargeSteps(RulesetCharacter character)
    {
        return _handler(character);
    }

    public static int GetAdvancementFromOvercharge(int overcharge, (int, int)[] steps)
    {
        if (steps == null || steps.Length == 0)
        {
            return 0;
        }
        
        foreach (var step in steps)
        {
            if (step.Item1 == overcharge)
            {
                return step.Item2;
            }
        }

        return 0;
    }
}
