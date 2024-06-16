using System;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Races;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Validators;

internal static class ValidatorsFeat
{
    //
    // Levels
    //

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsLevel2 =
        ValidateIsClass(string.Empty, 2);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsLevel4 =
        ValidateIsClass(string.Empty, 4);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsLevel16 =
        ValidateIsClass(string.Empty, 16);

    //
    // Classes
    //

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

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsRogueLevel3 =
        ValidateIsClass(Rogue.FormatTitle(), 3, Rogue);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsRogueLevel5 =
        ValidateIsClass(Rogue.FormatTitle(), 5, Rogue);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsSorcererLevel4 =
        ValidateIsClass(Sorcerer.FormatTitle(), 4, Sorcerer);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsWizardLevel4 =
        ValidateIsClass(Wizard.FormatTitle(), 4, Wizard);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>
        IsClericOrPaladinLevel4 =
            ValidateIsClass($"{Cleric.FormatTitle()} | {Paladin.FormatTitle()}", 4, Cleric, Paladin);

    //
    // Races
    //

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>
        IsDarkElfOrHalfElfDark =
            ValidateIsRaceOrSubrace(
                $"{SubraceDarkelfBuilder.SubraceDarkelf.FormatTitle()}, {RaceHalfElfBuilder.RaceHalfElfDarkVariant.FormatTitle()}",
                SubraceDarkelfBuilder.SubraceDarkelf, RaceHalfElfBuilder.RaceHalfElfDarkVariant);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsDragonborn =
        ValidateIsRaceOrSubrace(Dragonborn.FormatTitle(), Dragonborn);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsDwarf =
        ValidateIsRaceOrSubrace(Dwarf.FormatTitle(), Dwarf);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsHalfling =
        ValidateIsRaceOrSubrace(Halfling.FormatTitle(), Halfling);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsElfOfHalfElf =
        ValidateIsRaceOrSubrace(
            $"{Elf.FormatTitle()}, {HalfElf.FormatTitle()}",
            Elf, HalfElf, RaceHalfElfBuilder.RaceHalfElfVariant);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsGnome =
        ValidateIsRaceOrSubrace(Gnome.FormatTitle(), Gnome);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsHalfOrc =
        ValidateIsRaceOrSubrace(HalfOrc.FormatTitle(), HalfOrc);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsTiefling =
        ValidateIsRaceOrSubrace(Tiefling.FormatTitle(), Tiefling, RaceTieflingBuilder.RaceTiefling);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsSylvanElf =
        ValidateIsRaceOrSubrace(
            $"{ElfSylvan.FormatTitle()}, {RaceHalfElfBuilder.RaceHalfElfSylvanVariant.FormatTitle()}",
            ElfSylvan, RaceHalfElfBuilder.RaceHalfElfSylvanVariant);

    internal static readonly Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> IsSmallRace =
        ValidateIsRaceOrSubrace(
            $"{Dwarf.FormatTitle()}, {Gnome.FormatTitle()}, {Halfling.FormatTitle()}, {RaceFairyBuilder.RaceFairy.FormatTitle()}, {RaceImpBuilder.RaceImp.FormatTitle()}, {RaceKoboldBuilder.RaceKobold.FormatTitle()}",
            Dwarf, Gnome, Halfling,
            RaceFairyBuilder.RaceFairy,
            RaceImpBuilder.RaceImp,
            RaceKoboldBuilder.RaceKobold);

    //
    // Validators
    //

#if false
    internal static (bool result, string output) ValidateHasExtraAttack(FeatDefinition _, RulesetCharacterHero hero)
    {
        var guiFormat = Gui.Localize("Tooltip/&PreReqMustHaveExtraAttacks");
        var hasExtraAttack = hero.GetFeaturesByType<FeatureDefinitionAttributeModifier>()
            .Any(x =>
                x.ModifiedAttribute == AttributeDefinitions.AttacksNumber &&
                x.ModifierOperation
                    is FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive
                    or FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter);

        return !hasExtraAttack ? (false, Gui.Colorize(guiFormat, Gui.ColorFailure)) : (true, guiFormat);
    }
#endif

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateIsClass(
        string description, int minLevels, params CharacterClassDefinition[] characterClassDefinitions)
    {
        return (_, hero) =>
        {
            var guiFormat = Gui.Format("Tooltip/&PreReqIsWithLevel", description, minLevels.ToString());

            if (characterClassDefinitions.Length > 0 && !hero.ClassesHistory.Intersect(characterClassDefinitions).Any())
            {
                return (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
            }

            var levels = hero.ClassesHistory.Count;

            if (characterClassDefinitions.Length > 0)
            {
                levels = characterClassDefinitions
                    .Select(x =>
                    {
                        hero.ClassesAndLevels.TryGetValue(x, out var val);

                        return val;
                    })
                    .Max();
            }

            return Main.Settings.DisableLevelPrerequisitesOnModFeats || levels >= minLevels
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }

    [NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)> ValidateIsRaceOrSubrace(
        string description, params CharacterRaceDefinition[] characterRaceDefinition)
    {
        return (_, hero) =>
        {
            if (Main.Settings.DisableRacePrerequisitesOnModFeats)
            {
                return (true, string.Empty);
            }

            var isRaceOrSubrace = characterRaceDefinition.Contains(hero.RaceDefinition) ||
                                  characterRaceDefinition.Contains(hero.SubRaceDefinition);
            var guiFormat = Gui.Format("Tooltip/&PreReqIs", description);

            return isRaceOrSubrace
                ? (true, guiFormat)
                : (false, Gui.Colorize(guiFormat, Gui.ColorFailure));
        };
    }
}
