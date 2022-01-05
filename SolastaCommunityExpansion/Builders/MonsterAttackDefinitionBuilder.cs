using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class MonsterAttackDefinitionBuilder : BaseDefinitionBuilder<MonsterAttackDefinition>
    {
        public MonsterAttackDefinitionBuilder(string name, string guid, MonsterAttackDefinition baseDefinition) :
            base(baseDefinition, name, guid)
        {
        }

        public MonsterAttackDefinitionBuilder SetToHitBonus(int value)
        {
            Definition.SetToHitBonus(value);
            return this;
        }

        public MonsterAttackDefinitionBuilder SetDamageBonus(int value)
        {
            Definition.EffectDescription.EffectForms[0].DamageForm.SetBonusDamage(value);
            return this;
        }
    }
}

