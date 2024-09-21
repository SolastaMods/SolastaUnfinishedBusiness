using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Displays;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class DocumentationContext
{
    private static void EnsureFolderExists()
    {
        Main.EnsureFolderExists($"{Main.ModFolder}/Documentation");
        Main.EnsureFolderExists($"{Main.ModFolder}/Documentation/Monsters");
    }

    internal static void DumpDocumentation()
    {
        EnsureFolderExists();
        foreach (var characterFamilyDefinition in DatabaseRepository.GetDatabase<CharacterFamilyDefinition>()
                     .Where(x =>
                         x.Name is not ("Giant_Rugan" or "Ooze") &&
                         x.ContentPack != CeContentPackContext.CeContentPack))
        {
            DumpMonsters($"SolastaMonsters{characterFamilyDefinition.Name}",
                x => x.CharacterFamily == characterFamilyDefinition.Name && x.DefaultFaction == "HostileMonsters");
        }

        var vanillaRaces = DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .Where(x => x.ContentPack != CeContentPackContext.CeContentPack && x.SubRaces.Count != 0);

        DumpRaces("Races", x => vanillaRaces.Contains(x) || RacesContext.Races.Contains(x));
        DumpRaces("Subraces", x => !vanillaRaces.Contains(x) && !RacesContext.Races.Contains(x));

        DumpClasses(string.Empty, _ => true);
        DumpSubclasses(string.Empty, GetModdedSubclasses().Union(GetVanillaSubclasses()));

        DumpOthers<SpellDefinition>("Spells",
            x =>
                (x.ContentPack == CeContentPackContext.CeContentPack &&
                 SpellsContext.Spells.Contains(x)) ||
                (x.ContentPack != CeContentPackContext.CeContentPack &&
                 !SpellsContext.SpellsChildMaster.ContainsKey(x) &&
                 x.implemented &&
                 !x.Name.Contains("Invocation") &&
                 !x.Name.EndsWith("NoFocus") &&
                 !x.Name.EndsWith("_B")));

        DumpOthers<CharacterBackgroundDefinition>("Backgrounds",
            x => x.ContentPack == CeContentPackContext.CeContentPack || !x.GuiPresentation.Hidden);
        DumpOthers<FeatDefinition>("Feats",
            x => FeatsContext.Feats.Contains(x) ||
                 x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<FightingStyleDefinition>("FightingStyles",
            x =>
                FightingStyleContext.FightingStyles.Contains(x) ||
                x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<InvocationDefinition>("Invocations",
            x =>
                InvocationsContext.Invocations.Contains(x) ||
                x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<ItemDefinition>("Items",
            x => x.IsArmor || x.IsWeapon);
        DumpOthers<MetamagicOptionDefinition>("Metamagic",
            x =>
                MetamagicContext.Metamagic.Contains(x) ||
                x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<InvocationDefinition>("Maneuvers",
            x =>
                x is InvocationDefinitionCustom y &&
                y.PoolType == InvocationPoolTypeCustom.Pools.Gambit);
        DumpOthers<InvocationDefinition>("ArcaneShots",
            x =>
                x is InvocationDefinitionCustom y &&
                y.PoolType == InvocationPoolTypeCustom.Pools.ArcaneShotChoice);
        DumpOthers<InvocationDefinition>("Infusions",
            x =>
                x is InvocationDefinitionCustom y &&
                y.PoolType == InvocationPoolTypeCustom.Pools.Infusion);
        DumpOthers<InvocationDefinition>("Versatilities",
            x =>
                x is InvocationDefinitionCustom y &&
                y.PoolType == InvocationPoolTypeCustom.Pools.EldritchVersatilityPool);

#if false
        DumpSpellsCsv();
        DumpPowersCsv();
#endif
    }

    private static string GetTag(BaseDefinition definition)
    {
        return definition.ContentPack == CeContentPackContext.CeContentPack ? "[UB]" : "[SOL]";
    }

    private static string LazyManStripXml(string input)
    {
        return input
            .Replace("<color=#add8e6ff>", string.Empty)
            .Replace("<#57BCF4>", "\r\n\t")
            .Replace("<#B5D3DE>", string.Empty)
            .Replace("</color>", string.Empty)
            .Replace("<b>", string.Empty)
            .Replace("<i>", string.Empty)
            .Replace("</b>", string.Empty)
            .Replace("</i>", string.Empty);
    }

    private static void DumpFeatureUnlockByLevel(StringBuilder outString, List<FeatureUnlockByLevel> featureUnlocks)
    {
        var level = 0;

        foreach (var featureUnlockByLevel in featureUnlocks
                     .Where(x => !x.FeatureDefinition.GuiPresentation.hidden)
                     .OrderBy(x => x.level))
        {
            if (level != featureUnlockByLevel.level)
            {
                outString.AppendLine();
                outString.AppendLine($"## Level {featureUnlockByLevel.level}");
                outString.AppendLine();
                level = featureUnlockByLevel.level;
            }

            var featureDefinition = featureUnlockByLevel.FeatureDefinition;
            var description = LazyManStripXml(featureDefinition.FormatDescription());

            outString.AppendLine($"* {featureDefinition.FormatTitle()}");
            outString.AppendLine();
            outString.AppendLine(description);
            outString.AppendLine();
        }
    }

    private static void DumpClasses(string groupName, Func<BaseDefinition, bool> filter)
    {
        var outString = new StringBuilder();
        var counter = 1;

        foreach (var klass in DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                     .Where(x => filter(x))
                     .OrderBy(x => x.FormatTitle()))
        {
            outString.AppendLine($"# {counter++}. - {klass.FormatTitle()} {GetTag(klass)}");
            outString.AppendLine();
            outString.AppendLine(LazyManStripXml(klass.FormatDescription()));
            outString.AppendLine();

            DumpFeatureUnlockByLevel(outString, klass.FeatureUnlocks);

            outString.AppendLine();
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}Classes.md");
        sw.WriteLine(outString.ToString());
    }

    private static IEnumerable<(CharacterClassDefinition, CharacterSubclassDefinition)> GetModdedSubclasses()
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var kvp in SubclassesContext.KlassListContextTab
                     .OrderBy(x => x.Key.FormatTitle()))
        {
            var klass = kvp.Key;

            foreach (var subClass in kvp.Value.AllSubClasses
                         .OrderBy(x => x.FormatTitle()))
            {
                yield return (klass, subClass);
            }
        }
    }

    private static IEnumerable<(CharacterClassDefinition, CharacterSubclassDefinition)> GetVanillaSubclasses()
    {
        foreach (var klass in DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                     .OrderBy(x => x.FormatTitle()))
        {
            foreach (var featureDefinitionSubclassChoice in klass.FeatureUnlocks.Select(x => x.FeatureDefinition)
                         .OfType<FeatureDefinitionSubclassChoice>())
            {
                if (featureDefinitionSubclassChoice.FilterByDeity)
                {
                    foreach (var subclass in DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                                 .Where(x => x.Name.StartsWith(featureDefinitionSubclassChoice.SubclassSuffix) &&
                                             x.ContentPack != CeContentPackContext.CeContentPack))
                    {
                        yield return (klass, subclass);
                    }
                }
                else
                {
                    foreach (var subclass in featureDefinitionSubclassChoice.Subclasses
                                 .Select(x => DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
                                     .GetElement(x)))
                    {
                        yield return (klass, subclass);
                    }
                }
            }
        }
    }

    private static void DumpSubclasses(
        string groupName,
        IEnumerable<(CharacterClassDefinition, CharacterSubclassDefinition)> collection)
    {
        var outString = new StringBuilder();
        var counter = 0;

        CharacterClassDefinition klass = null;

        foreach (var (characterClassDefinition, subclass) in collection
                     .OrderBy(x => x.Item1.FormatTitle() + x.Item2.FormatTitle()))
        {
            if (klass != characterClassDefinition)
            {
                counter = 1;
                klass = characterClassDefinition;

                outString.AppendLine($"# {klass.FormatTitle()}");
                outString.AppendLine();
            }

            var title = subclass.FormatTitle();

            if (ModUi.TabletopDefinitions.Contains(subclass))
            {
                title = $"*{title}* \u00a9"; // copyright symbol
            }

            outString.AppendLine($"## {counter++}. {title} {GetTag(subclass)}");
            outString.AppendLine();
            outString.AppendLine(LazyManStripXml(subclass.FormatDescription()));
            outString.AppendLine();

            DumpFeatureUnlockByLevel(outString, subclass.FeatureUnlocks);

            outString.AppendLine();
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}Subclasses.md");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpRaces(string groupName, Func<BaseDefinition, bool> filter)
    {
        var outString = new StringBuilder();
        var counter = 1;

        foreach (var race in DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
                     .Where(x => filter(x))
                     .OrderBy(x => x.FormatTitle()))
        {
            var title = race.FormatTitle();

            if (ModUi.TabletopDefinitions.Contains(race))
            {
                title = $"*{title}* \u00a9";
            }

            outString.AppendLine($"# {counter++}. - {title} {GetTag(race)}");
            outString.AppendLine();
            outString.AppendLine(LazyManStripXml(race.FormatDescription()));
            outString.AppendLine();

            DumpFeatureUnlockByLevel(outString, race.FeatureUnlocks);

            outString.AppendLine();
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}.md");
        sw.WriteLine(outString.ToString());
    }

    private static readonly Dictionary<SpellListDefinition, CharacterClassDefinition> SpellListClassMap =
        new()
        {
            { InventorClass.SpellList, InventorClass.Class },
            { SpellListBard, Bard },
            { SpellListCleric, Cleric },
            { SpellListDruid, Druid },
            { SpellListPaladin, Paladin },
            { SpellListRanger, Ranger },
            { SpellListSorcerer, Sorcerer },
            { SpellListWarlock, Warlock },
            { SpellListWizard, Wizard }
        };

    private static string GetClassesWhichCanCastSpell(SpellDefinition spell)
    {
        var result = SpellListClassMap
            .OrderBy(kvp => kvp.Value.FormatTitle())
            .Where(kvp =>
                kvp.Key.SpellsByLevel
                    .SelectMany(x => x.Spells)
                    .Contains(spell) ||
                SpellsContext.SpellListContextTab[kvp.Key].SuggestedSpells.Contains(spell))
            .Aggregate(string.Empty, (current, kvp) => current + kvp.Value.FormatTitle() + ", ");

        return result == string.Empty
            ? string.Empty
            : "**[" + result.Substring(0, result.Length - 2) + "]**" + Environment.NewLine;
    }

    private static void DumpOthers<T>(string groupName, Func<T, bool> filter) where T : BaseDefinition
    {
        var outString = new StringBuilder();
        var db = DatabaseRepository.GetDatabase<T>();
        var counter = 1;

        foreach (var definition in db
                     .Where(filter)
                     .OrderBy(x =>
                         x is SpellDefinition spellDefinition
                             ? spellDefinition.SpellLevel + x.FormatTitle()
                             : x.FormatTitle()))
        {
            var title = definition.FormatTitle();

            if (ModUi.TabletopDefinitionNames.Contains(definition.Name))
            {
                title = $"*{title}* \u00a9";
            }

            var description = LazyManStripXml(definition.FormatDescription());

            //TODO: refactor this out with a proper optional change description action
            if (definition is SpellDefinition spellDefinition)
            {
                var components = " (";

                components += spellDefinition.MaterialComponentType == RuleDefinitions.MaterialComponentType.Specific
                    ? "M,"
                    : string.Empty;

                components += spellDefinition.VerboseComponent
                    ? "V,"
                    : string.Empty;

                components += spellDefinition.SomaticComponent
                    ? "S,"
                    : string.Empty;

                components = components.Substring(0, components.Length - 1) + ")";

                var school = DatabaseRepository.GetDatabase<SchoolOfMagicDefinition>()
                    .GetElement(spellDefinition.SchoolOfMagic).FormatTitle();

                title += $"{components} level {spellDefinition.SpellLevel} {school}";

                if (spellDefinition.RequiresConcentration)
                {
                    title += " [" + Gui.Format("Tooltip/&TagConcentrationTitle") + "]";
                }

                description = GetClassesWhichCanCastSpell(spellDefinition) + Environment.NewLine + description;
            }

            outString.AppendLine($"# {counter++}. - {title} {GetTag(definition)}");
            outString.AppendLine();
            outString.AppendLine(description);
            outString.AppendLine();

            //TODO: refactor this out with a proper optional footer action
            if (definition is not CharacterBackgroundDefinition backgroundDefinition)
            {
                continue;
            }

            var details = new StringBuilder();

            foreach (var featureDefinition in backgroundDefinition.Features
                         .Where(featureDefinition => featureDefinition is not FeatureDefinitionAttackModifier))
            {
                details.AppendLine($"- {featureDefinition.FormatDescription()}");
            }

            details.Append("- ");

            foreach (var equipment in backgroundDefinition.EquipmentRows
                         .SelectMany(x => x.EquipmentColumns)
                         .SelectMany(x => x.EquipmentOptions))
            {
                var quantity = equipment.Number;
                var itemTitle = equipment.ItemDefinition.FormatTitle();

                details.Append($"{quantity} x {itemTitle}");
                details.Append(", ");
            }

            var finalDetails = details.ToString();

            finalDetails = finalDetails.Substring(0, finalDetails.Length - 2);

            outString.AppendLine(LazyManStripXml(finalDetails));
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}.md");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpMonsters(string groupName, Func<MonsterDefinition, bool> filter)
    {
        var outString = new StringBuilder();
        var counter = 1;

        foreach (var monsterDefinition in DatabaseRepository.GetDatabase<MonsterDefinition>()
                     .Where(filter)
                     .OrderBy(x => x.FormatTitle()))
        {
            outString.Append(GetMonsterBlock(monsterDefinition, ref counter));
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/Monsters/{groupName}.md");
        sw.WriteLine(outString.ToString());
    }

    private static string GetMonsterBlock([NotNull] MonsterDefinition monsterDefinition, ref int counter)
    {
        var outString = new StringBuilder();

        outString.AppendLine($"# {counter++}. - {monsterDefinition.FormatTitle()}");
        outString.AppendLine();

        var description = LazyManStripXml(monsterDefinition.FormatDescription());

        if (!string.IsNullOrEmpty(description))
        {
            outString.AppendLine(monsterDefinition.FormatDescription());
        }

        outString.AppendLine();
        outString.AppendLine($"Alignment: *{monsterDefinition.Alignment.SplitCamelCase()}* ");

        var inDungeonMaker = monsterDefinition.DungeonMakerPresence == MonsterDefinition.DungeonMaker.None
            ? "NO"
            : "YES";

        outString.AppendLine();
        outString.AppendLine($"Dungeon Maker: *{inDungeonMaker}* ");

        outString.AppendLine();
        outString.AppendLine($"Size: *{monsterDefinition.SizeDefinition.FormatTitle()}* ");

        outString.AppendLine("|  AC  |   HD   |  CR  |");
        outString.AppendLine("| ---- | ------ | ---- |");

        outString.Append($"|  {monsterDefinition.ArmorClass:0#}  ");
        outString.Append($"| {monsterDefinition.HitDice:0#} {monsterDefinition.HitDiceType} ");
        outString.Append($"| {monsterDefinition.ChallengeRating:0#.0} ");
        outString.Append('|');
        outString.AppendLine();

        outString.AppendLine();
        outString.AppendLine("| Str | Dex | Con | Int | Wis | Cha |");
        outString.AppendLine("| --- | --- | --- | --- | --- | --- |");

        outString.Append($"|  {monsterDefinition.AbilityScores[0]:0#} ");
        outString.Append($"|  {monsterDefinition.AbilityScores[1]:0#} ");
        outString.Append($"|  {monsterDefinition.AbilityScores[2]:0#} ");
        outString.Append($"|  {monsterDefinition.AbilityScores[3]:0#} ");
        outString.Append($"|  {monsterDefinition.AbilityScores[4]:0#} ");
        outString.Append($"|  {monsterDefinition.AbilityScores[5]:0#} ");
        outString.Append('|');
        outString.AppendLine();

        outString.AppendLine();
        outString.AppendLine("*Features:*");

        FeatureDefinitionCastSpell featureDefinitionCastSpell = null;

        foreach (var featureDefinition in monsterDefinition.Features
                     .Where(x => x != null)
                     .OrderBy(x => x.Name))
        {
            switch (featureDefinition)
            {
                case FeatureDefinitionCastSpell definitionCastSpell:
                    featureDefinitionCastSpell = definitionCastSpell;
                    break;
                default:
                    outString.Append(GetMonsterFeatureBlock(featureDefinition));
                    break;
            }
        }

        if (featureDefinitionCastSpell)
        {
            outString.AppendLine();
            outString.AppendLine("*Spells:*");
            outString.AppendLine("| Level | Spell | Description |");
            outString.AppendLine("| ----- | ----- | ----------- |");

            foreach (var spellsByLevelDuplet in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel)
            {
                foreach (var spell in spellsByLevelDuplet.Spells)
                {
                    outString.AppendLine(
                        $"| {spellsByLevelDuplet.level} | {spell.FormatTitle()} | {spell.FormatDescription()} |");
                }
            }

            outString.AppendLine();
        }

        outString.AppendLine();
        outString.AppendLine("*Attacks:*");
        outString.AppendLine("| Type | Reach | Hit Bonus | Max Uses |");
        outString.AppendLine("| ---- | ----- | --------- | -------- |");

        foreach (var attackIteration in monsterDefinition.AttackIterations
                     .OrderBy(x => x.MonsterAttackDefinition.Name))
        {
            var title = attackIteration.MonsterAttackDefinition.FormatTitle();

            if (title == "None")
            {
                title = attackIteration.MonsterAttackDefinition.name.SplitCamelCase();
            }

            outString.Append($"| {title} ");
            outString.Append($"| {attackIteration.MonsterAttackDefinition.ReachRange} ");
            outString.Append($"| {attackIteration.MonsterAttackDefinition.ToHitBonus} ");
            outString.Append(attackIteration.MonsterAttackDefinition.MaxUses < 0
                ? "| 1 "
                : $"| {attackIteration.MonsterAttackDefinition.MaxUses} ");
            outString.Append('|');
            outString.AppendLine();
        }

        return outString.ToString();
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static string GetMonsterFeatureBlock(FeatureDefinition featureDefinition)
    {
        var outString = new StringBuilder();

        switch (featureDefinition)
        {
            case FeatureDefinitionFeatureSet featureDefinitionFeatureSet:
            {
                foreach (var featureDefinitionFromSet in featureDefinitionFeatureSet.FeatureSet)
                {
                    outString.Append(GetMonsterFeatureBlock(featureDefinitionFromSet));
                }

                break;
            }
            case FeatureDefinitionMoveMode featureDefinitionMoveMode:
                outString.Append("* ");
                outString.Append(featureDefinitionMoveMode.MoveMode);
                outString.Append(' ');
                outString.Append(featureDefinitionMoveMode.Speed);
                outString.AppendLine();

                break;
            case FeatureDefinitionLightAffinity featureDefinitionLightAffinity:
                foreach (var lightingEffectAndCondition in
                         featureDefinitionLightAffinity.LightingEffectAndConditionList)
                {
                    outString.AppendLine(
                        $"* {lightingEffectAndCondition.condition.FormatTitle()} - {lightingEffectAndCondition.lightingState}");
                }

                break;
            default:
                var title = featureDefinition.FormatTitle();

                if (title == "None")
                {
                    title = featureDefinition.Name.SplitCamelCase();
                }

                outString.Append("* ");
                outString.AppendLine(title);

                break;
        }

        return outString.ToString();
    }

#if false
    private static void DumpSpellsCsv()
    {
        var outString = new StringBuilder();

        outString.Append("Name");
        outString.Append('\t');
        outString.Append("Spell Level");
        outString.Append('\t');
        outString.Append("Spell");
        outString.Append('\t');
        outString.Append("Description");
        outString.Append('\t');
        outString.Append("Range Type");
        outString.Append('\t');
        outString.Append("Range Parameter");
        outString.Append('\t');
        outString.Append("Target Side");
        outString.Append('\t');
        outString.Append("Target Type");
        outString.Append('\t');
        outString.Append("Target Parameter");
        outString.Append('\t');
        outString.Append("Duration Type");
        outString.Append('\t');
        outString.Append("Duration Parameter");
        outString.Append('\t');
        outString.Append("End of Effect");
        outString.Append('\t');
        outString.Append("Saving");
        outString.Append('\t');
        outString.Append("Ignore Cover");
        outString.Append('\t');
        outString.Append("Source");
        outString.AppendLine();

        foreach (var spell in DatabaseRepository.GetDatabase<SpellDefinition>()
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack ||
                                 SpellsContext.Spells.Contains(x)))
        {
            outString.Append(spell.Name);
            outString.Append('\t');
            outString.Append(spell.SpellLevel);
            outString.Append('\t');
            outString.Append(spell.FormatTitle());
            outString.Append('\t');
            outString.Append(spell.FormatDescription().Replace("\n", " "));
            outString.Append('\t');
            outString.Append(spell.EffectDescription.RangeType);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.RangeParameter);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.TargetSide);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.TargetType);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.TargetParameter);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.DurationType);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.DurationParameter);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.EndOfEffect);
            outString.Append('\t');
            outString.Append(spell.EffectDescription.HasSavingThrow
                ? spell.EffectDescription.SavingThrowAbility
                : "NONE");
            outString.Append('\t');
            outString.Append(spell.EffectDescription.IgnoreCover);
            outString.Append('\t');
            outString.Append(spell.ContentPack == CeContentPackContext.CeContentPack ? "MOD" : "VANILLA");
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/spells.txt");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpPowersCsv()
    {
        var outString = new StringBuilder();

        outString.Append("Name");
        outString.Append('\t');
        outString.Append("Power");
        outString.Append('\t');
        outString.Append("Description");
        outString.Append('\t');
        outString.Append("Range Type");
        outString.Append('\t');
        outString.Append("Range Parameter");
        outString.Append('\t');
        outString.Append("Target Side");
        outString.Append('\t');
        outString.Append("Target Type");
        outString.Append('\t');
        outString.Append("Target Parameter");
        outString.Append('\t');
        outString.Append("Duration Type");
        outString.Append('\t');
        outString.Append("Duration Parameter");
        outString.Append('\t');
        outString.Append("End of Effect");
        outString.Append('\t');
        outString.Append("Saving");
        outString.Append('\t');
        outString.Append("Ignore Cover");
        outString.AppendLine();
        outString.Append("Source");
        outString.AppendLine();

        var classPowers = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
            .SelectMany(y => y.FeatureUnlocks
                .Select(z => z.FeatureDefinition))
            .OfType<FeatureDefinitionPower>()
            .ToArray();

        var racePowers = DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .SelectMany(y => y.FeatureUnlocks
                .Select(z => z.FeatureDefinition))
            .OfType<FeatureDefinitionPower>().ToArray();

        var subClassPowers = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .SelectMany(y => y.FeatureUnlocks
                .Select(z => z.FeatureDefinition))
            .OfType<FeatureDefinitionPower>().ToArray();

        foreach (var power in DatabaseRepository.GetDatabase<FeatureDefinition>()
                     .OfType<FeatureDefinitionPower>()
                     .Where(x => !x.GuiPresentation.Hidden &&
                                 (classPowers.Contains(x) || racePowers.Contains(x) || subClassPowers.Contains(x))))
        {
            outString.Append(power.Name);
            outString.Append('\t');
            outString.Append(power.FormatTitle());
            outString.Append('\t');
            outString.Append(power.FormatDescription().Replace("\n", " "));
            outString.Append('\t');
            outString.Append(power.EffectDescription.RangeType);
            outString.Append('\t');
            outString.Append(power.EffectDescription.RangeParameter);
            outString.Append('\t');
            outString.Append(power.EffectDescription.TargetSide);
            outString.Append('\t');
            outString.Append(power.EffectDescription.TargetType);
            outString.Append('\t');
            outString.Append(power.EffectDescription.TargetParameter);
            outString.Append('\t');
            outString.Append(power.EffectDescription.DurationType);
            outString.Append('\t');
            outString.Append(power.EffectDescription.DurationParameter);
            outString.Append('\t');
            outString.Append(power.EffectDescription.EndOfEffect);
            outString.Append('\t');
            outString.Append(power.EffectDescription.HasSavingThrow
                ? power.EffectDescription.SavingThrowAbility
                : "NONE");
            outString.Append('\t');
            outString.Append(power.EffectDescription.IgnoreCover);
            outString.Append('\t');
            outString.Append(power.ContentPack == CeContentPackContext.CeContentPack ? "MOD" : "VANILLA");
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/powers.txt");
        sw.WriteLine(outString.ToString());
    }
#endif
}
