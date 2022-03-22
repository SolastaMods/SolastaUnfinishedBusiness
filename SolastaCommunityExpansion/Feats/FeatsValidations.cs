using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class FeatsValidations
    {
        //
        // validation routines for FeatDefinitionCustomBuilder
        //

        internal static bool ValidateMinCharacterLevel4(
            FeatDefinition _,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            var isLevelValid = hero.ClassesHistory.Count >= 4;

            if (prerequisiteOutput != string.Empty)
            {
                prerequisiteOutput += "\n";
            }

            var levelText = isLevelValid ? "4" : Gui.Colorize("4", "EA7171");

            prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", levelText);

            return isLevelValid;
        }

        internal static bool ValidateMinCharacterLevel8(
            FeatDefinition _,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            var isLevelValid = hero.ClassesHistory.Count >= 4;

            if (prerequisiteOutput != string.Empty)
            {
                prerequisiteOutput += "\n";
            }

            var levelText = isLevelValid ? "8" : Gui.Colorize("8", "EA7171");

            prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteLevelFormat", levelText);

            return isLevelValid;
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

        internal static bool ValidateNotBarbarian(
            FeatDefinition _,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            var isNotBarbarian = !hero.ClassesAndLevels.ContainsKey(Barbarian);

            if (prerequisiteOutput != string.Empty)
            {
                prerequisiteOutput += "\n";
            }

            if (isNotBarbarian)
            {
                prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteIsNotBarbarian");
            }
            else
            {
                prerequisiteOutput += Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteIsNotBarbarian"), "EA7171");
            }

            return isNotBarbarian;
        }

        internal static bool ValidateNotFighter(
            FeatDefinition _,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            var isNotFighter = !hero.ClassesAndLevels.ContainsKey(Fighter);

            if (prerequisiteOutput != string.Empty)
            {
                prerequisiteOutput += "\n";
            }

            if (isNotFighter)
            {
                prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteIsNotFighter");
            }
            else
            {
                prerequisiteOutput += Gui.Colorize(Gui.Format("Tooltip/&FeatPrerequisiteIsNotFighter"), "EA7171");
            }

            return isNotFighter;
        }

        internal static bool ValidateNotRogue(
            FeatDefinition _,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            var isNotRogue = !hero.ClassesAndLevels.ContainsKey(Rogue);

            if (prerequisiteOutput != string.Empty)
            {
                prerequisiteOutput += "\n";
            }

            if (isNotRogue)
            {
                prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteIsNotRogue");
            }
            else
            {
                Gui.Colorize(prerequisiteOutput += Gui.Format("Tooltip/&FeatPrerequisiteIsNotRogue"), "EA7171");
            }

            return isNotRogue;
        }
    }
}
