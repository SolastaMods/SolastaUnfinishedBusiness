namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RestActivityValidationParams
{
    public readonly bool ConsiderUses;
    public readonly bool ConsiderHaving;

    public RestActivityValidationParams(bool considerUses, bool considerHaving)
    {
        ConsiderUses = considerUses;
        ConsiderHaving = considerHaving;
    }
}
