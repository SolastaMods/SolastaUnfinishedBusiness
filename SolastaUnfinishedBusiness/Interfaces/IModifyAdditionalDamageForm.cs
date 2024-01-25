using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyAdditionalDamageForm
{
    [UsedImplicitly]
    public DamageForm AdditionalDamageForm(
        [UsedImplicitly] GameLocationCharacter attacker,
        [UsedImplicitly] GameLocationCharacter defender,
        IAdditionalDamageProvider provider,
        DamageForm damageForm);
}
