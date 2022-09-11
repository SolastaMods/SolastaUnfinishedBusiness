using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models;

internal static class MulticlassContext
{
    internal const int MaxClasses = 3;

    internal static void Load()
    {
        // ensure these are always referenced here for diagnostics dump
        _ = ArmorProficiencyMulticlassBuilder.ProficiencyBarbarianArmorMulticlass;
        _ = ArmorProficiencyMulticlassBuilder.ProficiencyFighterArmorMulticlass;
        _ = ArmorProficiencyMulticlassBuilder.ProficiencyPaladinArmorMulticlass;

        _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolBardSkillPointsMulticlass;
        _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolRangerSkillPointsMulticlass;
        _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolRogueSkillPointsMulticlass;

        // required to ensure level 20 and multiclass will work correctly on higher level heroes
        var spellListDefinitions = DatabaseRepository.GetDatabase<SpellListDefinition>();

        foreach (var spellListDefinition in spellListDefinitions)
        {
            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < Level20Context.MaxSpellLevel + (spellListDefinition.HasCantrips ? 1 : 0))
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = new List<SpellDefinition>()
                });
            }
        }

        // required to avoid some trace error messages that might affect multiplayer sessions and prevent level up from 19 to 20
        var castSpellDefinitions = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>();

        foreach (var castSpellDefinition in castSpellDefinitions)
        {
            while (castSpellDefinition.KnownCantrips.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.KnownCantrips.Add(0);
            }

            while (castSpellDefinition.KnownSpells.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.KnownSpells.Add(0);
            }

            while (castSpellDefinition.ReplacedSpells.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.ReplacedSpells.Add(0);
            }

            while (castSpellDefinition.ScribedSpells.Count < Level20Context.ModMaxLevel + 1)
            {
                castSpellDefinition.ScribedSpells.Add(0);
            }
        }
    }

    internal static void LateLoad()
    {
        MulticlassPatchingContext.Load(); // depends on IntegrationContext;
        SharedSpellsContext.Load(); // depends on IntegrationContext
    }
}

public sealed class ArmorProficiencyMulticlassBuilder : FeatureDefinitionProficiencyBuilder
{
    private const string ProficiencyBarbarianArmorMulticlassName = "ProficiencyBarbarianArmorMulticlass";
    private const string ProficiencyBarbarianArmorMulticlassGuid = "86558227b0cd4771b42978a60dc610db";

    private const string ProficiencyFighterArmorMulticlassName = "ProficiencyFighterArmorMulticlass";
    private const string ProficiencyFighterArmorMulticlassGuid = "5df5ec907a424fccbfec103344421b51";

    private const string ProficiencyPaladinArmorMulticlassName = "ProficiencyPaladinArmorMulticlass";
    private const string ProficiencyPaladinArmorMulticlassGuid = "69b18e44aabd4acca702c05f9d6c7fcb";

    public static readonly FeatureDefinitionProficiency ProficiencyBarbarianArmorMulticlass =
        CreateAndAddToDB(ProficiencyBarbarianArmorMulticlassName, ProficiencyBarbarianArmorMulticlassGuid,
            "Feature/&BarbarianArmorProficiencyTitle",
            EquipmentDefinitions.ShieldCategory
        );

    public static readonly FeatureDefinitionProficiency ProficiencyFighterArmorMulticlass =
        CreateAndAddToDB(ProficiencyFighterArmorMulticlassName, ProficiencyFighterArmorMulticlassGuid,
            "Feature/&FighterArmorProficiencyTitle",
            EquipmentDefinitions.LightArmorCategory,
            EquipmentDefinitions.MediumArmorCategory,
            EquipmentDefinitions.ShieldCategory
        );

    public static readonly FeatureDefinitionProficiency ProficiencyPaladinArmorMulticlass =
        CreateAndAddToDB(ProficiencyPaladinArmorMulticlassName, ProficiencyPaladinArmorMulticlassGuid,
            "Feature/&PaladinArmorProficiencyTitle",
            EquipmentDefinitions.LightArmorCategory,
            EquipmentDefinitions.MediumArmorCategory,
            EquipmentDefinitions.ShieldCategory
        );

    private ArmorProficiencyMulticlassBuilder(string name, string guid, string title,
        [NotNull] params string[] proficienciesToReplace) : base(ProficiencyFighterArmor, name, guid)
    {
        Definition.Proficiencies.SetRange(proficienciesToReplace);
        Definition.GuiPresentation.Title = title;
    }

    private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid, string title,
        [NotNull] params string[] proficienciesToReplace)
    {
        return new ArmorProficiencyMulticlassBuilder(name, guid, title, proficienciesToReplace).AddToDB();
    }
}

public static class SkillProficiencyPointPoolSkillsBuilder
{
    public static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolBardSkillPointsMulticlass", DefinitionBuilder.CENamespaceGuid)
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&BardSkillPointsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Arcana,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.History,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Medecine,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Religion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth,
                SkillDefinitions.Survival
            )
            .AddToDB();

    public static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRangerSkillPointsMulticlass", DefinitionBuilder.CENamespaceGuid)
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&RangerSkillsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Athletics,
                SkillDefinitions.Insight,
                SkillDefinitions.Investigation,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival,
                SkillDefinitions.Stealth
            )
            .AddToDB();

    public static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRogueSkillPointsMulticlass", DefinitionBuilder.CENamespaceGuid)
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&RogueSkillPointsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth
            )
            .AddToDB();
}
