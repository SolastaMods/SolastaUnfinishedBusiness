using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class FeatsValidations
    {
        //
        // validation routines for FeatDefinitionCustomBuilder
        //

        internal static IsFeatMatchingPrerequisites ValidateMinCharLevel(int minCharLevel)
        {
            return (FeatDefinition _, RulesetCharacterHero hero, ref string prerequisiteOutput) =>
            {
                var isLevelValid = hero.ClassesHistory.Count >= 4;

                if (prerequisiteOutput != string.Empty)
                {
                    prerequisiteOutput += "\n";
                }

                if (isLevelValid)
                {
                    prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", minCharLevel.ToString());
                }
                else
                {
                    prerequisiteOutput += Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", minCharLevel.ToString()), "EA7171");
                }

                return isLevelValid;
            };
        }

        internal static IsFeatMatchingPrerequisites ValidateNotClass(CharacterClassDefinition characterClassDefinition)
        {
            var className = characterClassDefinition.Name;

            return (FeatDefinition _, RulesetCharacterHero hero, ref string prerequisiteOutput) =>
            {
                var isNotClass = !hero.ClassesAndLevels.ContainsKey(characterClassDefinition);

                if (prerequisiteOutput != string.Empty)
                {
                    prerequisiteOutput += "\n";
                }

                if (isNotClass)
                {
                    prerequisiteOutput += Gui.Format($"Tooltip/&FeatPrerequisiteIsNot{className}");
                }
                else
                {
                    prerequisiteOutput += Gui.Colorize(Gui.Format($"Tooltip/&FeatPrerequisiteIsNot{className}"), "EA7171");
                }

                return isNotClass;
            };
        }

        internal static bool ValidateHasStealthAttack(
            FeatDefinition _,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            var features = new List<FeatureDefinition>();

            hero.EnumerateFeaturesToBrowse<FeatureDefinitionDamageAffinity>(features);

            var hasStealthAttack = hero.ClassesAndLevels.ContainsKey(Rogue) || features.Any(x => x.Name == "AdditionalDamageFeatShadySneakAttack");


            if (prerequisiteOutput != string.Empty)
            {
                prerequisiteOutput += "\n";
            }

            if (hasStealthAttack)
            {
                prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteHasStealthAttack");
            }
            else
            {
                prerequisiteOutput += Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteHasStealthAttack"), "EA7171");
            }

            return hasStealthAttack;
        }
    }
}
