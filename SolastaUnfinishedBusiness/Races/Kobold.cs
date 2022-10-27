using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;

namespace SolastaUnfinishedBusiness.Races;

internal static class KoboldRaceBuilder
{
    internal static CharacterRaceDefinition RaceKobold { get; } = BuildKobold();

    [NotNull]
    private static CharacterRaceDefinition BuildKobold()
    {
        var proficiencyKoboldLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyKoboldLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceKobold")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                proficiencyKoboldLanguages)
            .AddToDB();

        raceKobold.SubRaces.SetRange(new List<CharacterRaceDefinition>
        {
            BuildDarkKobold(), BuildDraconicKobold()
        });
        
        raceKobold.GuiPresentation.sortOrder = Elf.GuiPresentation.sortOrder + 1;
        RacesContext.RaceScaleMap[raceKobold] = -0.04f / -0.06f;

        return raceKobold;
    }
    
    [NotNull]
    private static CharacterRaceDefinition BuildDarkKobold()
    {
        // var koboldSpriteReference = CustomIcons.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        var attributeModifierDarkKoboldDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDarkKoboldDexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 2)
            .AddToDB();

        var abilityCheckAffinityDarkKoboldLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarkKoboldLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityDarkKoboldLightSensitivity.AffinityGroups[0].lightingContext = LightingContext.BrightLight;

        var darkKoboldCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinitySensitiveToLight, "CombatAffinityDarkKoboldLightSensitivity")
            .SetOrUpdateGuiPresentation("LightAffinityDarkKoboldLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.None)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var conditionDarkKoboldLightSensitive = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLightSensitive, "ConditionDarkKoboldLightSensitive")
            .SetOrUpdateGuiPresentation("LightAffinityDarkKoboldLightSensitivity", Category.Feature)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityDarkKoboldLightSensitivity, darkKoboldCombatAffinityLightSensitivity)
            .AddToDB();

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(conditionDarkKoboldLightSensitive);

        var lightAffinityDarkKoboldLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityDarkKoboldLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = conditionDarkKoboldLightSensitive
                })
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create(TrueStrike.EffectDescription)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 2)
            .SetDurationData(DurationType.Round, 1)
            .Build();

        var conditionDarkKoboldGrovelCowerAndBeg = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionDarkKoboldGrovelCowerAndBeg")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = conditionDarkKoboldGrovelCowerAndBeg;

        var powerDarkKoboldGrovelCowerAndBeg = FeatureDefinitionPowerBuilder
            .Create("PowerDarkKoboldGrovelCowerAndBeg")
            .SetGuiPresentation(Category.Feature, Aid)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var darkKoboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        var raceDarkKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceDarkKobold")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetRacePresentation(darkKoboldRacePresentation)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                attributeModifierDarkKoboldDexterityAbilityScoreIncrease,
                powerDarkKoboldGrovelCowerAndBeg,
                CombatAffinityPackTactics,
                lightAffinityDarkKoboldLightSensitivity)
            .AddToDB();

        RacesContext.RaceScaleMap[raceDarkKobold] = -0.04f / -0.06f;;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceDarkKobold.name);

        return raceDarkKobold;
    }

    private static CharacterRaceDefinition BuildDraconicKobold()
    {
        var koboldSpriteReference = Dragonborn.GuiPresentation.SpriteReference;

        var combatAffinityKoboldDraconicCry = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinityParalyzedAdvantage, "CombatAffinityDraconicKoboldDraconicCry")
            .AddToDB();

        var conditionKoboldDraconicCry = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBaned, "ConditionDraconicKoboldDraconicCry")
            .SetFeatures(combatAffinityKoboldDraconicCry)
            .AddToDB();

        //var powerKoboldDraconicCry

        var spellListKoboldMagic = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListSorcerer, "SpellListDraconicKoboldMagic")
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells.ToArray())
            .FinalizeSpells()
            .AddToDB();

        var castSpellKoboldMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellDraconicKoboldMagic")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(spellListKoboldMagic)
            .AddToDB();

        var koboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        //koboldRacePresentation.defaultMusculature = 80;

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceDraconicKobold")
            .SetGuiPresentation(Category.Race, koboldSpriteReference)
            .SetRacePresentation(koboldRacePresentation)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionFeatureSets.FeatureSetHalfElfAbilityScoreIncrease,
                castSpellKoboldMagic)
            .AddToDB();

        RacesContext.RaceScaleMap[raceKobold] = -0.04f / -0.06f;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceKobold.name);

        return raceKobold;
    }
}
