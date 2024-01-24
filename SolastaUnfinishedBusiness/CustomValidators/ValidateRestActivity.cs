namespace SolastaUnfinishedBusiness.CustomValidators;

public class ValidateRestActivity(bool considerUses, bool considerHaving)
{
    public readonly bool ConsiderHaving = considerHaving;
    public readonly bool ConsiderUses = considerUses;
}
