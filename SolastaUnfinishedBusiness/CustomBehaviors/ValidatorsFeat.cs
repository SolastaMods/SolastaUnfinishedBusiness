using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class ValidatorsFeat
{
    //
    // validation routines for FeatDefinitionWithPrerequisites
    //

    internal static (bool, string) IsElfOrHalfElf(FeatDefinitionWithPrerequisites _,
        [NotNull] RulesetCharacterHero hero)
    {
        var isElf = hero.RaceDefinition.Name.Contains(DatabaseHelper.CharacterRaceDefinitions.Elf.Name);
        var elfTitle = DatabaseHelper.CharacterRaceDefinitions.Elf.FormatTitle();
        var halfElfTitle = DatabaseHelper.CharacterRaceDefinitions.HalfElf.FormatTitle();
        var param = $"{elfTitle}, {halfElfTitle}";

        return isElf
            ? (true, Gui.Format("Tooltip/&FeatPreReqIs", param))
            : (false, Gui.Colorize(Gui.Format("Tooltip/&FeatPreReqIs", param), Gui.ColorFailure));
    }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateMinCharLevel(
        int minCharLevel)
    {
        return (_, hero) =>
        {
            var isLevelValid = hero.ClassesHistory.Count >= minCharLevel;

            if (isLevelValid)
            {
                return (true, Gui.Format("Tooltip/&FeatPreReqLevelFormat", minCharLevel.ToString()));
            }

            return (false,
                Gui.Colorize(Gui.Format("Tooltip/&FeatPreReqLevelFormat", minCharLevel.ToString()),
                    Gui.ColorFailure));
        };
    }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateHasFeat(
        FeatDefinition featDefinition)
    {
        return (_, hero) =>
        {
            var isValid = hero.TrainedFeats.Contains(featDefinition);

            if (isValid)
            {
                return (true, Gui.Format("Tooltip/&FeatPreReqFeatFormat", featDefinition.FormatTitle()));
            }

            return (false,
                Gui.Colorize(Gui.Format("Tooltip/&FeatPreReqFeatFormat", featDefinition.FormatTitle()),
                    Gui.ColorFailure));
        };
    }

    // [NotNull]
    // internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateRace(
    //     CharacterRaceDefinition characterRaceDefinition)
    // {
    //     var raceName = characterRaceDefinition.FormatTitle();
    //
    //     return (_, hero) =>
    //     {
    //         var isRace = characterRaceDefinition == hero.RaceDefinition;
    //
    //         return isRace
    //             ? (true, Gui.Format("Tooltip/&FeatPreReqIs", raceName))
    //             : (false, Gui.Colorize(Gui.Format("Tooltip/&FeatPreReqIs", raceName), Gui.ColorFailure));
    //     };
    // }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotClass(
        [NotNull] CharacterClassDefinition characterClassDefinition)
    {
        var className = characterClassDefinition.FormatTitle();

        return (_, hero) =>
        {
            var isNotClass = !hero.ClassesAndLevels.ContainsKey(characterClassDefinition);

            return isNotClass
                ? (true, Gui.Format("Tooltip/&FeatPreReqIsNot", className))
                : (false, Gui.Colorize(Gui.Format("Tooltip/&FeatPreReqIsNot", className), Gui.ColorFailure));
        };
    }
}
