using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyAdditionalDamageForm
{
    [UsedImplicitly]
    public DamageForm AdditionalDamageForm(
        [UsedImplicitly] GameLocationCharacter attacker,
        [UsedImplicitly] GameLocationCharacter defender,
        IAdditionalDamageProvider provider,
        DamageForm damageForm);
}
