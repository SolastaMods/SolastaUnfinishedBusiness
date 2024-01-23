namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RestActivityValidationParams(bool considerUses, bool considerHaving)
{
    public readonly bool ConsiderHaving = considerHaving;
    public readonly bool ConsiderUses = considerUses;
}
