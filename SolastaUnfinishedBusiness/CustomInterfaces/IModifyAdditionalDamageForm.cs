namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyAdditionalDamageForm
{
    public DamageForm AdditionalDamageForm(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        IAdditionalDamageProvider provider,
        DamageForm damageForm);
}
