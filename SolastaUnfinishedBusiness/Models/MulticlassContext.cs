using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;

namespace SolastaUnfinishedBusiness.Models;

internal static class MulticlassContext
{
    internal const int MaxClasses = 3;

    private const string ArmorTrainingDescription = "Feature/&ArmorTrainingShortDescription";

    private const string SkillGainChoicesDescription = "Feature/&SkillGainChoicesPluralDescription";

    internal static readonly FeatureDefinitionProficiency ProficiencyBarbarianArmorMulticlass =
        FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBarbarianArmorMulticlass")
            .SetGuiPresentation("Feature/&BarbarianArmorProficiencyTitle", ArmorTrainingDescription)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Armor,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

    internal static readonly FeatureDefinitionProficiency ProficiencyFighterArmorMulticlass =
        FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFighterArmorMulticlass")
            .SetGuiPresentation("Feature/&FighterArmorProficiencyTitle", ArmorTrainingDescription)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Armor,
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

    internal static readonly FeatureDefinitionProficiency ProficiencyPaladinArmorMulticlass =
        FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyPaladinArmorMulticlass")
            .SetGuiPresentation("Feature/&PaladinArmorProficiencyTitle", ArmorTrainingDescription)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Armor,
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

    internal static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolBardSkillPointsMulticlass")
            .SetGuiPresentation("Feature/&BardSkillPointsTitle", SkillGainChoicesDescription)
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

    internal static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRangerSkillPointsMulticlass")
            .SetGuiPresentation("Feature/&RangerSkillsTitle", SkillGainChoicesDescription)
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

    internal static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass =
        FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRogueSkillPointsMulticlass")
            .SetGuiPresentation("Feature/&RogueSkillPointsTitle", SkillGainChoicesDescription)
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

    internal static void Load()
    {
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
