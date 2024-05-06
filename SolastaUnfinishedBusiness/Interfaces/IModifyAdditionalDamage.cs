using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyAdditionalDamage
{
    [UsedImplicitly]
    public void ModifyAdditionalDamage(
        [UsedImplicitly] GameLocationCharacter attacker,
        [UsedImplicitly] GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
        List<EffectForm> actualEffectForms,
        ref DamageForm damageForm);
}
