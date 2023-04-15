namespace SolastaUnfinishedBusiness.CustomValidators;

public class RestActivityValidationParams
{
    public readonly bool ConsiderHaving;
    public readonly bool ConsiderUses;

    public RestActivityValidationParams(bool considerUses, bool considerHaving)
    {
        ConsiderUses = considerUses;
        ConsiderHaving = considerHaving;
    }
}
