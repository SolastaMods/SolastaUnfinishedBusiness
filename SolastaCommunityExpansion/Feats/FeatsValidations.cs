using System;
using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class FeatsValidations
    {
        //
        // validation routines for FeatDefinitionWithPrerequisites
        //

        internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateMinCharLevel(int minCharLevel)
        {
            return (FeatDefinition _, RulesetCharacterHero hero) =>
            {
                var isLevelValid = hero.ClassesHistory.Count >= 4;

                if (isLevelValid)
                {
                    return (true, Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", minCharLevel.ToString()));
                }
                else
                {
                    return (false, Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", minCharLevel.ToString()), "EA7171"));
                }
            };
        }

        internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotClass(CharacterClassDefinition characterClassDefinition)
        {
            var className = characterClassDefinition.Name;

            return (FeatDefinition _, RulesetCharacterHero hero) =>
            {
                var isNotClass = !hero.ClassesAndLevels.ContainsKey(characterClassDefinition);

                if (isNotClass)
                {
                    return (true, Gui.Format($"Tooltip/&FeatPrerequisiteIsNot{className}"));
                }
                else
                {
                    return (false, Gui.Colorize(Gui.Format($"Tooltip/&FeatPrerequisiteIsNot{className}"), "EA7171"));
                }
            };
        }

        internal static (bool, string) ValidateHasStealthAttack(FeatDefinition _, RulesetCharacterHero hero)
        {
            var features = new List<FeatureDefinition>();

            hero.EnumerateFeaturesToBrowse<FeatureDefinitionDamageAffinity>(features);

            var hasStealthAttack = hero.ClassesAndLevels.ContainsKey(Rogue) || features.Any(x => x.Name == "AdditionalDamageFeatShadySneakAttack");

            if (hasStealthAttack)
            {
                return (true, Gui.Format("Tooltip/&FeatPrerequisiteHasStealthAttack"));
            }
            else
            {
                return (false, Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteHasStealthAttack"), "EA7171"));
            }
        }
    }
}
