using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class DocumentationContext
{
    internal static void DumpDocumentation()
    {
        if (!Directory.Exists($"{Main.ModFolder}/Documentation"))
        {
            Directory.CreateDirectory($"{Main.ModFolder}/Documentation");
        }

        foreach (var characterFamilyDefinition in DatabaseRepository.GetDatabase<CharacterFamilyDefinition>()
                     .Where(x => x.Name != "Giant_Rugan" && x.Name != "Ooze"))
        {
            if (characterFamilyDefinition.ContentPack == CeContentPackContext.CeContentPack)
            {
                continue;
            }

            DumpMonsters($"SolastaMonsters{characterFamilyDefinition.Name}",
                x => x.CharacterFamily == characterFamilyDefinition.Name && x.DefaultFaction == "HostileMonsters");
        }

        DumpClasses("UnfinishedBusiness", x => x.ContentPack == CeContentPackContext.CeContentPack);
        DumpClasses("Solasta", x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpSubclasses("UnfinishedBusiness", GetModdedSubclasses());
        DumpSubclasses("Solasta", GetVanillaSubclasses());
        DumpRaces("UnfinishedBusiness", x => x.ContentPack == CeContentPackContext.CeContentPack);
        DumpRaces("Solasta", x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<FeatDefinition>("UnfinishedBusinessFeats", x => FeatsContext.Feats.Contains(x));
        DumpOthers<FeatDefinition>("SolastaFeats", x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<FightingStyleDefinition>("UnfinishedBusinessFightingStyles",
            x => FightingStyleContext.FightingStyles.Contains(x));
        DumpOthers<FightingStyleDefinition>("SolastaFightingStyles",
            x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<InvocationDefinition>("UnfinishedBusinessInvocations",
            x => InvocationsContext.Invocations.Contains(x));
        DumpOthers<InvocationDefinition>("SolastaInvocations",
            x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<SpellDefinition>("UnfinishedBusinessSpells",
            x => x.ContentPack == CeContentPackContext.CeContentPack && SpellsContext.Spells.Contains(x));
        DumpOthers<SpellDefinition>("SolastaSpells",
            x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<ItemDefinition>("UnfinishedBusinessItems",
            x => x.ContentPack == CeContentPackContext.CeContentPack &&
                 (x.IsArmor || x.IsWeapon));
        DumpOthers<ItemDefinition>("SolastaItems",
            x => x.ContentPack != CeContentPackContext.CeContentPack &&
                 (x.IsArmor || x.IsWeapon));
        DumpOthers<MetamagicOptionDefinition>("UnfinishedBusinessMetamagic",
            x => MetamagicContext.Metamagic.Contains(x));
        DumpOthers<MetamagicOptionDefinition>("SolastaMetamagic",
            x => x.ContentPack != CeContentPackContext.CeContentPack);
        DumpOthers<InvocationDefinition>("UnfinishedBusinessGambits",
            x => x is InvocationDefinitionCustom y &&
                 y.PoolType == InvocationPoolTypeCustom.Pools.Gambit);
        DumpOthers<InvocationDefinition>("UnfinishedBusinessArcaneShots",
            x => x is InvocationDefinitionCustom y &&
                 y.PoolType == InvocationPoolTypeCustom.Pools.ArcaneShotChoice);
        DumpOthers<InvocationDefinition>("UnfinishedBusinessInfusions",
            x => x is InvocationDefinitionCustom y &&
                 y.PoolType == InvocationPoolTypeCustom.Pools.Infusion);
        DumpOthers<InvocationDefinition>("UnfinishedBusinessVersatilities",
            x => x is InvocationDefinitionCustom y &&
                 y.PoolType == InvocationPoolTypeCustom.Pools.EldritchVersatilityPool);

#if DEBUG
        DumpSpellsCsv();
        DumpPowersCsv();
#endif
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

    private static void DumpClasses(string groupName, Func<BaseDefinition, bool> filter)
    {
        var outString = new StringBuilder();
        var counter = 1;

        foreach (var klass in DatabaseRepository.GetDatabase<CharacterClassDefinition>()
                     .Where(x => filter(x))
                     .OrderBy(x => x.FormatTitle()))
        {
            outString.AppendLine($"# {counter++}. - {klass.FormatTitle()}");
            outString.AppendLine();
            outString.AppendLine(LazyManStripXml(klass.FormatDescription()));
            outString.AppendLine();

            var level = 0;

            foreach (var featureUnlockByLevel in klass.FeatureUnlocks
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

        foreach (var (characterClassDefinition, subclass) in collection)
        {
            if (klass != characterClassDefinition)
            {
                counter = 1;
                klass = characterClassDefinition;

                outString.AppendLine($"# {klass.FormatTitle()}");
                outString.AppendLine();
            }

            outString.AppendLine($"## {counter++}. {subclass.FormatTitle()}");
            outString.AppendLine();
            outString.AppendLine(LazyManStripXml(subclass.FormatDescription()));
            outString.AppendLine();

            var level = 0;

            foreach (var featureUnlockByLevel in subclass.FeatureUnlocks
                         .Where(x => !x.FeatureDefinition.GuiPresentation.hidden)
                         .OrderBy(x => x.level))
            {
                if (level != featureUnlockByLevel.level)
                {
                    outString.AppendLine();
                    outString.AppendLine($"### Level {featureUnlockByLevel.level}");
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
            outString.AppendLine($"# {counter++}. - {race.FormatTitle()}");
            outString.AppendLine();
            outString.AppendLine(LazyManStripXml(race.FormatDescription()));
            outString.AppendLine();

            var level = 0;

            foreach (var featureUnlockByLevel in race.FeatureUnlocks
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

            outString.AppendLine();
            outString.AppendLine();
        }

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}Races.md");
        sw.WriteLine(outString.ToString());
    }

    private static void DumpOthers<T>(string groupName, Func<T, bool> filter) where T : BaseDefinition
    {
        var outString = new StringBuilder();
        var db = DatabaseRepository.GetDatabase<T>();
        var counter = 1;

        foreach (var featureDefinition in db
                     .Where(filter)
                     .OrderBy(x =>
                         x is SpellDefinition spellDefinition
                             ? spellDefinition.SpellLevel + x.FormatTitle()
                             : x.FormatTitle()))
        {
            var title = featureDefinition.FormatTitle();
            var description = LazyManStripXml(featureDefinition.FormatDescription());

            if (featureDefinition is SpellDefinition spellDefinition)
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
            }

            outString.AppendLine($"# {counter++}. - {title}");
            outString.AppendLine();
            outString.AppendLine(description);
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

        using var sw = new StreamWriter($"{Main.ModFolder}/Documentation/{groupName}.md");
        sw.WriteLine(outString.ToString());
    }

    private static string GetMonsterBlock([NotNull] MonsterDefinition monsterDefinition, ref int counter)
    {
        var outString = new StringBuilder();

        outString.AppendLine(
            $"# {counter++}. - {monsterDefinition.FormatTitle()}");
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

        if (featureDefinitionCastSpell != null)
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

#if DEBUG
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
