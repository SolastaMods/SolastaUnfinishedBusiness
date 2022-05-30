using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions;

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

        if (!isWeaponValid(attackMode, weapon, character))
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

public abstract class ModifyAttackModeForWeaponBase : IModifyAttackModeForWeapon
{
    private readonly IsWeaponValidHandler isWeaponValid;
    private readonly CharacterValidator[] validators;

    protected ModifyAttackModeForWeaponBase(IsWeaponValidHandler isWeaponValid,
        params CharacterValidator[] validators)
    {
        this.isWeaponValid = isWeaponValid;
        this.validators = validators;
    }

    public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon)
    {
        if (attackMode == null)
        {
            return;
        }

        if (!character.IsValid(validators))
        {
            return;
        }

        if (!isWeaponValid(attackMode, weapon, character))
        {
            return;
        }

        TryModifyAttackMode(character, attackMode, weapon);
    }

    protected abstract void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode,
        RulesetItem weapon);
}

public class UpgradeWeaponDice : ModifyAttackModeForWeaponBase
{
    public delegate (RuleDefinitions.DieType, int) GetWeaponDiceHandler(RulesetCharacter character,
        RulesetItem weapon);

    private readonly GetWeaponDiceHandler getWeaponDice;

    public UpgradeWeaponDice(GetWeaponDiceHandler getWeaponDice, IsWeaponValidHandler isWeaponValid,
        params CharacterValidator[] validators) : base(isWeaponValid, validators)
    {
        this.getWeaponDice = getWeaponDice;
    }

    protected override void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode,
        RulesetItem weapon)
    {
        var effectDescription = attackMode.EffectDescription;
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

public class AddTagToWeaponAttack : ModifyAttackModeForWeaponBase
{
    private readonly string tag;

    public AddTagToWeaponAttack(string tag, IsWeaponValidHandler isWeaponValid,
        params CharacterValidator[] validators) : base(isWeaponValid, validators)
    {
        this.tag = tag;
    }

    protected override void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode,
        RulesetItem weapon)
    {
        attackMode.AddAttackTagAsNeeded(tag);
    }
}

public class AddEffectToWeaponAttack : ModifyAttackModeForWeaponBase
{
    private readonly EffectForm effect;

    public AddEffectToWeaponAttack(EffectForm effect, IsWeaponValidHandler isWeaponValid,
        params CharacterValidator[] validators) : base(isWeaponValid, validators)
    {
        this.effect = effect;
    }

    protected override void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode,
        RulesetItem weapon)
    {
        attackMode.EffectDescription.AddEffectForms(effect);
    }
}
