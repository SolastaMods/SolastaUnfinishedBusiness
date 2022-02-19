using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class MonsterAttackDefinitionBuilder : DefinitionBuilder<MonsterAttackDefinition, MonsterAttackDefinitionBuilder>
    {
        public MonsterAttackDefinitionBuilder(string name, string guid) :
            base(name, guid)
        {
        }

        public MonsterAttackDefinitionBuilder(string name, Guid namespaceGuid) :
            base(name, namespaceGuid)
        {
        }

        public MonsterAttackDefinitionBuilder(MonsterAttackDefinition original, string name, string guid) :
            base(original, name, guid)
        {
        }

        public MonsterAttackDefinitionBuilder(MonsterAttackDefinition original, string name, Guid namespaceGuid) :
            base(original, name, namespaceGuid)
        {
        }

        public MonsterAttackDefinitionBuilder SetToHitBonus(int value)
        {
            Definition.SetToHitBonus(value);
            return this;
        }

        public MonsterAttackDefinitionBuilder SetDamageBonusOfFirstDamageForm(int value)
        {
            var form = Definition.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Damage);
            form?.DamageForm.SetBonusDamage(value);
            return this;
        }
    }
}
