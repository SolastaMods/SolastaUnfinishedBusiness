using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

/**
 * Adds tag to the weapon to be processed by IsValidContextForRestrictedContextProvider and RefreshAttackMode 
 */
public class AddTagToWeapon
{
    private readonly IsWeaponValidHandler isWeaponValid;
    private readonly string tag;
    private readonly TagsDefinitions.Criticity criticity;
    private readonly IsCharacterValidHandler[] validators;

    internal AddTagToWeapon(string tag, TagsDefinitions.Criticity criticity,
        IsWeaponValidHandler isWeaponValid, params IsCharacterValidHandler[] validators)
    {
        this.tag = tag;
        this.criticity = criticity;
        this.isWeaponValid = isWeaponValid;
        this.validators = validators;
    }

    private bool IsValid(RulesetCharacter character, RulesetItem weapon)
    {
        return character.IsValid(validators) && isWeaponValid(null, weapon, character);
    }

    internal static void TryAddTags(RulesetCharacter character, RulesetItem item,
        Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        var mods = character.GetSubFeaturesByType<AddTagToWeapon>();
        if (mods.Empty())
        {
            return;
        }

        foreach (var mod in mods.Where(mod => mod.IsValid(character, item)))
        {
            tags.TryAdd(mod.tag, mod.criticity);
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
        if (mods.Empty())
        {
            return description.WeaponTags;
        }

        var tags = new List<string>(description.WeaponTags);
        foreach (var mod in mods.Where(mod => mod.IsValid(character, weapon)))
        {
            tags.TryAdd(mod.tag);
        }

        return tags;
    }
}
