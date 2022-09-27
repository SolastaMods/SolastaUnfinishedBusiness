namespace SolastaUnfinishedBusiness.CustomBehaviors;

public abstract class CustomOverchargeProvider
{
    public abstract (int, int)[] OverchargeSteps(RulesetCharacter character);

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
