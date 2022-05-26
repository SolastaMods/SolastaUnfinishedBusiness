using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions
{

    public class CanUseAttributeForWeapon : IModifyAttackAttributeForWeapon
    {
        private readonly CharacterValidator[] _validators;
        private readonly string attribute;
        private readonly IsWeaponValidHandler isWeaponValid;

        public CanUseAttributeForWeapon(string attribute, IsWeaponValidHandler isWeaponValid,
            params CharacterValidator[] validators)
        {
            this.attribute = attribute;
            this.isWeaponValid = isWeaponValid;
            _validators = validators;
        }

        public void ModifyAttribute(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            if (attackMode == null)
            {
                return;
            }

            if (!character.IsValid(_validators))
            {
                return;
            }

            if (!isWeaponValid(attackMode, weapon))
            {
                return;
            }

            if (character.GetAttribute(attribute).CurrentValue >
                character.GetAttribute(attackMode.AbilityScore).CurrentValue)
            {
                attackMode.AbilityScore = attribute;
            }
        }
    }

    public class UpgradeWeaponDice : IModifyAttackModeForWeapon
    {
        private readonly CharacterValidator[] _validators;
        private readonly IsWeaponValidHandler isWeaponValid;
        private readonly GetWeaponDiceHandler getWeaponDice;

        public delegate (RuleDefinitions.DieType, int) GetWeaponDiceHandler(RulesetCharacter character, RulesetItem weapon);

        public UpgradeWeaponDice(GetWeaponDiceHandler getWeaponDice, IsWeaponValidHandler isWeaponValid, params CharacterValidator[] validators)
        {
            this.isWeaponValid = isWeaponValid;
            this.getWeaponDice = getWeaponDice;
            _validators = validators;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            if (attackMode == null)
            {
                return;
            }

            if (!character.IsValid(_validators))
            {
                return;
            }

            if (!isWeaponValid(attackMode, weapon))
            {
                return;
            }

            var effectDescription = attackMode?.EffectDescription;
            var damage = effectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var (newDie, newNumber) = getWeaponDice(character, weapon);
            var newDamage = RuleDefinitions.DieAverage(newDie) * newNumber;

            var oldDamage = RuleDefinitions.DieAverage(damage.DieType) * damage.DiceNumber;
            var oldDamageVersatile = RuleDefinitions.DieAverage(damage.VersatileDieType) * damage.DiceNumber;


            if (newDamage > oldDamage)
            {
                damage.DieType = newDie;
                damage.DiceNumber = newNumber;
            }

            if (newDamage > oldDamageVersatile)
            {
                damage.VersatileDieType = newDie;
            }
        }
    }

    public class AddTagToWeaponAttack : IModifyAttackModeForWeapon
    {
        private readonly CharacterValidator[] _validators;
        private readonly IsWeaponValidHandler isWeaponValid;
        private readonly string tag;

        public AddTagToWeaponAttack(string tag, IsWeaponValidHandler isWeaponValid, params CharacterValidator[] validators)
        {
            this.isWeaponValid = isWeaponValid;
            this.tag = tag;
            _validators = validators;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            if (attackMode == null)
            {
                return;
            }

            if (!character.IsValid(_validators))
            {
                return;
            }

            if (!isWeaponValid(attackMode, weapon))
            {
                return;
            }

            attackMode.AddAttackTagAsNeeded(tag);
        }
    }
}


