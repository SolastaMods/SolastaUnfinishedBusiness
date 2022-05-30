using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models;

internal static class MulticlassContext
{
    internal const int MAX_CLASSES = 6;

    internal static void Load()
    {
        // ensure these are always referenced here for diagnostics dump
        _ = ArmorProficiencyMulticlassBuilder.BarbarianArmorProficiencyMulticlass;
        _ = ArmorProficiencyMulticlassBuilder.FighterArmorProficiencyMulticlass;
        _ = ArmorProficiencyMulticlassBuilder.PaladinArmorProficiencyMulticlass;
        _ = ArmorProficiencyMulticlassBuilder.WardenArmorProficiencyMulticlass;
        _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolBardSkillPointsMulticlass;
        _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolRangerSkillPointsMulticlass;
        _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolRogueSkillPointsMulticlass;

        // required to ensure level 20 and multiclass will work correctly on higher level heroes
        var spellListDefinitions = DatabaseRepository.GetDatabase<SpellListDefinition>();

        foreach (var spellListDefinition in spellListDefinitions)
        {
            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < Level20Context.MAX_SPELL_LEVEL + (spellListDefinition.HasCantrips ? 1 : 0))
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = new List<SpellDefinition>()
                });
            }
        }

        // required to avoid some trace error messages that might desync multiplayer sessions and prevent level up from 19 to 20
        var castSpellDefinitions = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>();

        foreach (var castSpellDefinition in castSpellDefinitions)
        {
            while (castSpellDefinition.KnownCantrips.Count < Level20Context.MOD_MAX_LEVEL + 1)
            {
                castSpellDefinition.KnownCantrips.Add(0);
            }

            while (castSpellDefinition.KnownSpells.Count < Level20Context.MOD_MAX_LEVEL + 1)
            {
                castSpellDefinition.KnownSpells.Add(0);
            }

            while (castSpellDefinition.ReplacedSpells.Count < Level20Context.MOD_MAX_LEVEL + 1)
            {
                castSpellDefinition.ReplacedSpells.Add(0);
            }

            while (castSpellDefinition.ScribedSpells.Count < Level20Context.MOD_MAX_LEVEL + 1)
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
    private const string BarbarianArmorProficiencyMulticlassName = "BarbarianArmorProficiencyMulticlass";
    private const string BarbarianArmorProficiencyMulticlassGuid = "86558227b0cd4771b42978a60dc610db";

    private const string FighterArmorProficiencyMulticlassName = "FighterArmorProficiencyMulticlass";
    private const string FighterArmorProficiencyMulticlassGuid = "5df5ec907a424fccbfec103344421b51";

    private const string PaladinArmorProficiencyMulticlassName = "PaladinArmorProficiencyMulticlass";
    private const string PaladinArmorProficiencyMulticlassGuid = "69b18e44aabd4acca702c05f9d6c7fcb";

    private const string WardenArmorProficiencyMulticlassName = "WardenArmorProficiencyMulticlass";
    private const string WardenArmorProficiencyMulticlassGuid = "19666e846975401b819d1ae72c5d27ac";

    public static readonly FeatureDefinitionProficiency BarbarianArmorProficiencyMulticlass =
        CreateAndAddToDB(BarbarianArmorProficiencyMulticlassName, BarbarianArmorProficiencyMulticlassGuid,
            "Feature/&BarbarianArmorProficiencyTitle",
            EquipmentDefinitions.ShieldCategory
        );

    public static readonly FeatureDefinitionProficiency FighterArmorProficiencyMulticlass =
        CreateAndAddToDB(FighterArmorProficiencyMulticlassName, FighterArmorProficiencyMulticlassGuid,
            "Feature/&FighterArmorProficiencyTitle",
            EquipmentDefinitions.LightArmorCategory,
            EquipmentDefinitions.MediumArmorCategory,
            EquipmentDefinitions.ShieldCategory
        );

    public static readonly FeatureDefinitionProficiency PaladinArmorProficiencyMulticlass =
        CreateAndAddToDB(PaladinArmorProficiencyMulticlassName, PaladinArmorProficiencyMulticlassGuid,
            "Feature/&PaladinArmorProficiencyTitle",
            EquipmentDefinitions.LightArmorCategory,
            EquipmentDefinitions.MediumArmorCategory,
            EquipmentDefinitions.ShieldCategory
        );

    public static readonly FeatureDefinitionProficiency WardenArmorProficiencyMulticlass =
        CreateAndAddToDB(WardenArmorProficiencyMulticlassName, WardenArmorProficiencyMulticlassGuid,
            "Feature/&WardenArmorProficiencyTitle",
            EquipmentDefinitions.LightArmorCategory,
            EquipmentDefinitions.MediumArmorCategory,
            EquipmentDefinitions.ShieldCategory
        );

    private ArmorProficiencyMulticlassBuilder(string name, string guid, string title,
        params string[] proficienciesToReplace) : base(ProficiencyFighterArmor, name, guid)
    {
        Definition.Proficiencies.SetRange(proficienciesToReplace);
        Definition.GuiPresentation.Title = title;
    }

    private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid, string title,
        params string[] proficienciesToReplace)
    {
        return new ArmorProficiencyMulticlassBuilder(name, guid, title, proficienciesToReplace).AddToDB();
    }
}

public static class SkillProficiencyPointPoolSkillsBuilder
{
    public static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolBardSkillPointsMulticlass", "a69b2527569b4893abe57ad1f80e97ed")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&BardSkillsTitle", "Feature/&SkillGainChoicesPluralDescription")
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
            .Create("PointPoolRangerSkillPointsMulticlass", "096e4e01b52b490e807cf8d458845aa5")
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
            .Create("PointPoolRogueSkillPointsMulticlass", "451259da8c5c41f4b1b363f00b01be4e")
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
