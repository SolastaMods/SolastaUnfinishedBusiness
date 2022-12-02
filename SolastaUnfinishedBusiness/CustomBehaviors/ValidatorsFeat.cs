using System;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class ValidatorsFeat
{
    //
    // validation routines for FeatDefinitionWithPrerequisites
    //

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotMetamagic(
        [NotNull] BaseDefinition baseDefinition)
    {
        return (_, hero) =>
        {
            var hasMetamagic = hero.TrainedMetamagicOptions.Any(x => x.Name == baseDefinition.Name);
            var guiFormat = Gui.Format("Tooltip/&PreReqDoesNotKnow", baseDefinition.FormatTitle());

            return hasMetamagic ? (false, Gui.Colorize(guiFormat, Gui.ColorFailure)) : (true, guiFormat);
        };
    }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotFightingStyle(
        [NotNull] BaseDefinition baseDefinition)
    {
        return (_, hero) =>
        {
            var hasFightingStyle = hero.TrainedFightingStyles.Any(x => x.Name == baseDefinition.Name);
            var guiFormat = Gui.Format("Tooltip/&PreReqDoesNotKnow", baseDefinition.FormatTitle());

            return hasFightingStyle ? (false, Gui.Colorize(guiFormat, Gui.ColorFailure)) : (true, guiFormat);
        };
    }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotClass(
        [NotNull] CharacterClassDefinition characterClassDefinition)
    {
        var className = characterClassDefinition.FormatTitle();

        return (_, hero) =>
        {
            var isNotClass = !hero.ClassesAndLevels.ContainsKey(characterClassDefinition);
            var guiFormat = Gui.Format("Tooltip/&PreReqIsNot", className);

            return isNotClass
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }

#if false
    // Tooltip/&FeatPreReqIs=Is {0}
    internal static (bool, string) IsElfOrHalfElf(
        FeatDefinitionWithPrerequisites _,
        [NotNull] RulesetCharacterHero hero)
    {
        var isElf = hero.RaceDefinition.Name.Contains(DatabaseHelper.CharacterRaceDefinitions.Elf.Name);
        var elfTitle = DatabaseHelper.CharacterRaceDefinitions.Elf.FormatTitle();
        var halfElfTitle = DatabaseHelper.CharacterRaceDefinitions.HalfElf.FormatTitle();
        var param = $"{elfTitle}, {halfElfTitle}";
        var guiFormat = Gui.Format("Tooltip/&FeatPreReqIs", param);

        return isElf
            ? (true, guiFormat)
            : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
    }

    [NotNull]
    // Tooltip/&FeatPreReqLevelFormat=Min Character Level {0}
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateMinCharLevel(
        int minCharLevel)
    {
        return (_, hero) =>
        {
            var isLevelValid = hero.ClassesHistory.Count >= minCharLevel;
            var guiFormat = Gui.Format("Tooltip/&FeatPreReqLevelFormat", minCharLevel.ToString());

            return isLevelValid
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }
#endif
}
