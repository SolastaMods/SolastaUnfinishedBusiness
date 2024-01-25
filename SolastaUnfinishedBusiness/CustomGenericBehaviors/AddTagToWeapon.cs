using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomValidators;

namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

/**
 * Adds tag to the weapon to be processed by IsValidContextForRestrictedContextProvider and RefreshAttackMode
 */
public class AddTagToWeapon
{
    private readonly TagsDefinitions.Criticity _criticity;
    private readonly IsWeaponValidHandler _isWeaponValid;
    private readonly string _tag;
    private readonly IsCharacterValidHandler[] _validators;

    internal AddTagToWeapon(
        string tag,
        TagsDefinitions.Criticity criticity,
        IsWeaponValidHandler isWeaponValid,
        params IsCharacterValidHandler[] validators)
    {
        _tag = tag;
        _criticity = criticity;
        _isWeaponValid = isWeaponValid;
        _validators = validators;
    }

    private bool IsValid(RulesetCharacter character, RulesetItem weapon)
    {
        return character.IsValid(_validators) && _isWeaponValid(null, weapon, character);
    }

    internal static void TryAddTags(
        RulesetCharacter character,
        RulesetItem item,
        Dictionary<string,
            TagsDefinitions.Criticity> tags)
    {
        var mods = character.GetSubFeaturesByType<AddTagToWeapon>();

        if (mods.Count == 0)
        {
            return;
        }

        foreach (var mod in mods.Where(mod => mod.IsValid(character, item)))
        {
            tags.TryAdd(mod._tag, mod._criticity);
        }
    }

    internal static List<string> GetCustomWeaponTags(
        WeaponDescription description,
        RulesetCharacter character,
        RulesetAttackMode mode)
    {
        return GetCustomWeaponTags(description, character, mode?.SourceObject as RulesetItem);
    }

    internal static List<string> GetCustomWeaponTags(
        WeaponDescription description,
        RulesetCharacter character,
        RulesetItem weapon)
    {
        if (weapon == null)
        {
            return description.WeaponTags;
        }

        var mods = character.GetSubFeaturesByType<AddTagToWeapon>();

        if (mods.Count == 0)
        {
            return description.WeaponTags;
        }

        var tags = new List<string>(description.WeaponTags);

        foreach (var mod in mods.Where(mod => mod.IsValid(character, weapon)))
        {
            tags.TryAdd(mod._tag);
        }

        return tags;
    }
}
