using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Classes.Inventor;
using SolastaUnfinishedBusiness.Races;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class ValidatorsFeat
{
    //
    // validation routines for FeatDefinitionWithPrerequisites
    //

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsLevel4 =
        ValidateIsClass(string.Empty, 4);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsLevel16 =
        ValidateIsClass(string.Empty, 16, Fighter);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>
        IsBarbarianLevel4 = ValidateIsClass(Barbarian.FormatTitle(), 4, Barbarian);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsBardLevel4 =
        ValidateIsClass(Bard.FormatTitle(), 4, Bard);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsClericLevel4 =
        ValidateIsClass(Cleric.FormatTitle(), 4, Cleric);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsDruidLevel4 =
        ValidateIsClass(Druid.FormatTitle(), 4, Druid);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>
        IsFighterLevel4 = ValidateIsClass(Fighter.FormatTitle(), 4, Fighter);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsInventorLevel4 =
        ValidateIsClass(InventorClass.Class.FormatTitle(), 4, InventorClass.Class);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsPaladinLevel1 =
        ValidateIsClass(Paladin.FormatTitle(), 1, Paladin);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsPaladinLevel4 =
        ValidateIsClass(Paladin.FormatTitle(), 4, Paladin);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsRangerLevel1 =
        ValidateIsClass(Ranger.FormatTitle(), 1, Ranger);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsRangerLevel4 =
        ValidateIsClass(Ranger.FormatTitle(), 4, Ranger);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsRogueLevel4 =
        ValidateIsClass(Rogue.FormatTitle(), 4, Rogue);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>
        IsRangerOrRogueLevel4 = ValidateIsClass($"{Ranger.FormatTitle()} | {Rogue.FormatTitle()}", 4, Ranger, Rogue);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsSorcererLevel4 =
        ValidateIsClass(Sorcerer.FormatTitle(), 4, Sorcerer);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsWizardLevel4 =
        ValidateIsClass(Wizard.FormatTitle(), 4, Wizard);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsDragonborn =
        ValidateIsRace(Dragonborn.FormatTitle(), Dragonborn);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsElfOfHalfElf =
        ValidateIsRace(
            $"{Elf.FormatTitle()}, {HalfElf.FormatTitle()}",
            Elf, ElfHigh, ElfSylvan, HalfElf,
            DarkelfSubraceBuilder.SubraceDarkelf,
            RaceHalfElfVariantRaceBuilder.RaceHalfElfVariant,
            RaceHalfElfVariantRaceBuilder.RaceHalfElfDarkVariant,
            RaceHalfElfVariantRaceBuilder.RaceHalfElfHighVariant,
            RaceHalfElfVariantRaceBuilder.RaceHalfElfSylvanVariant);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsGnome =
        ValidateIsRace(GnomeRaceBuilder.RaceGnome.FormatTitle(), GnomeRaceBuilder.RaceGnome);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsSmallRace =
        ValidateIsRace(
            $"{Dwarf.FormatTitle()}, {GnomeRaceBuilder.RaceGnome.FormatTitle()}, {Halfling.FormatTitle()}",
            Dwarf, DwarfHill, DwarfSnow,
            GrayDwarfSubraceBuilder.SubraceGrayDwarf,
            GnomeRaceBuilder.RaceGnome,
            KoboldRaceBuilder.RaceKobold,
            Halfling, HalflingIsland, HalflingMarsh);

#if false
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> HasCantrips()
    {
        return (_, hero) =>
        {
            var hasCantrips = false;
            var selectedClass = LevelUpContext.GetSelectedClass(hero);
            var selectedSubClass = LevelUpContext.GetSelectedSubclass(hero);

            if (selectedClass != null)
            {
                hasCantrips = hero.SpellRepertoires.Exists(x =>
                    (x.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class &&
                     x.SpellCastingClass == selectedClass && x.SpellCastingFeature.SpellListDefinition.HasCantrips) ||
                    (x.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass &&
                     x.SpellCastingSubclass == selectedSubClass &&
                     x.SpellCastingFeature.SpellListDefinition.HasCantrips));
            }

            var guiLocalize = Gui.Localize("Tooltip/&PreReqHasCantrips");

            return hasCantrips ? (true, guiLocalize) : (false, Gui.Colorize(guiLocalize, Gui.ColorFailure));
        };
    }
#endif

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
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotFeature(
        [NotNull] FeatureDefinition featureDefinition)
    {
        return (_, hero) =>
        {
            var hasFeature = hero.HasAnyFeature(featureDefinition);
            var guiFormat = Gui.Format("Tooltip/&PreReqDoesNotKnow", featureDefinition.FormatTitle());

            return hasFeature ? (false, Gui.Colorize(guiFormat, Gui.ColorFailure)) : (true, guiFormat);
        };
    }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateHasFeature(
        [NotNull] FeatureDefinition featureDefinition)
    {
        return (_, hero) =>
        {
            var hasFeature = !hero.HasAnyFeature(featureDefinition);
            var guiFormat = Gui.Format("Tooltip/&PreReqMustKnow", featureDefinition.FormatTitle());

            return hasFeature ? (false, Gui.Colorize(guiFormat, Gui.ColorFailure)) : (true, guiFormat);
        };
    }

    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateNotClass(
        [NotNull] CharacterClassDefinition characterClassDefinition)
    {
        var className = characterClassDefinition.FormatTitle();

        return (_, hero) =>
        {
            var isNotClass = hero.ClassesHistory.Last() != characterClassDefinition;
            var guiFormat = Gui.Format("Tooltip/&PreReqIsNot", className);

            return isNotClass
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }

    [NotNull]
    private static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateIsClass(
        string description, int minLevels, params CharacterClassDefinition[] characterClassDefinition)
    {
        return (_, hero) =>
        {
            var guiFormat = Gui.Format("Tooltip/&PreReqIsWithLevel", description, minLevels.ToString());

            if (characterClassDefinition.Length > 0 && !hero.ClassesHistory.Intersect(characterClassDefinition).Any())
            {
                return (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
            }

            var levels = hero.ClassesHistory.Count;

            return Main.Settings.DisableClassPrerequisitesOnModFeats || levels >= minLevels
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }

    [NotNull]
    private static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateIsRace(
        string description, params CharacterRaceDefinition[] characterRaceDefinition)
    {
        return (_, hero) =>
        {
            if (Main.Settings.DisableRacePrerequisitesOnModFeats)
            {
                return (true, string.Empty);
            }

            var isRace = characterRaceDefinition.Contains(hero.RaceDefinition);
            var guiFormat = Gui.Format("Tooltip/&PreReqIs", description);

            return isRace
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }

#if false
    [NotNull]
    internal static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateMinCharLevel(
        int minCharLevel)
    {
        return (_, hero) =>
        {
            var isLevelValid = hero.ClassesHistory.Count >= minCharLevel;
            var guiFormat = Gui.Format("Tooltip/&PreReqLevelFormat", minCharLevel.ToString());

            return isLevelValid
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }
#endif
}
