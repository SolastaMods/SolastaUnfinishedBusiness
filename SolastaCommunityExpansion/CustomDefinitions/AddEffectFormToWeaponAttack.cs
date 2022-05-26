using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class AddEffectFormToWeaponAttack : IModifyAttackModeForWeapon
    {
        private readonly EffectForm effect;
        private readonly IsWeaponValidHandler isWeaponValid;
        private readonly CharacterValidator[] validators;

        public AddEffectFormToWeaponAttack(EffectForm effect, IsWeaponValidHandler isWeaponValid,
            params CharacterValidator[] validators)
        {
            this.effect = effect;
            this.isWeaponValid = isWeaponValid;
            this.validators = validators;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            if (!character.IsValid(validators))
            {
                return;
            }

            if (!isWeaponValid(attackMode, weapon))
            {
                return;
            }

            attackMode.EffectDescription.AddEffectForms(effect.Copy());
        }
    }
}
