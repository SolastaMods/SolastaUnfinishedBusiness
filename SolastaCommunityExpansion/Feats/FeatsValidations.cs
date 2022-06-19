using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Feats;

internal static class FeatsValidations
{
    //
    // validation routines for FeatDefinitionWithPrerequisites
    //

    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateMinCharLevel(
        int minCharLevel)
    {
        return (_, hero) =>
        {
            var isLevelValid = hero.ClassesHistory.Count >= minCharLevel;

            if (isLevelValid)
            {
                return (true, Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", minCharLevel.ToString()));
            }

            return (false,
                Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", minCharLevel.ToString()),
                    "EA7171"));
        };
    }

    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotClass(
        CharacterClassDefinition characterClassDefinition)
    {
        var className = characterClassDefinition.Name;

        return (_, hero) =>
        {
            var isNotClass = !hero.ClassesAndLevels.ContainsKey(characterClassDefinition);

            if (isNotClass)
            {
                return (true, Gui.Localize($"Tooltip/&FeatPrerequisiteIsNot{className}"));
            }

            return (false, Gui.Colorize(Gui.Localize($"Tooltip/&FeatPrerequisiteIsNot{className}"), "EA7171"));
        };
    }

    internal static (bool, string) ValidateHasStealthAttack(FeatDefinition _, RulesetCharacterHero hero)
    {
        var features = new List<FeatureDefinition>();

        hero.EnumerateFeaturesToBrowse<FeatureDefinitionAdditionalDamage>(features);

        var hasStealthAttack = features.Any(x => x.Name.Contains(TagsDefinitions.AdditionalDamageSneakAttackTag));

        if (hasStealthAttack)
        {
            return (true, Gui.Localize("Tooltip/&FeatPrerequisiteHasStealthAttack"));
        }

        return (false, Gui.Colorize(Gui.Localize("Tooltip/&FeatPrerequisiteHasStealthAttack"), "EA7171"));
    }
}
