using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyAdditionalDamageForm
{
    [UsedImplicitly]
    public DamageForm AdditionalDamageForm(
        [UsedImplicitly] GameLocationCharacter attacker,
        [UsedImplicitly] GameLocationCharacter defender,
        FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
        DamageForm damageForm);
}
