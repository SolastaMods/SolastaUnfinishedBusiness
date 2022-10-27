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
        // var koboldSpriteReference = CustomIcons.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        var attributeModifierKoboldDexterityAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierKoboldDexterityAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 2)
            .AddToDB();

        var abilityCheckAffinityKoboldLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityKoboldLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityKoboldLightSensitivity.AffinityGroups[0].lightingContext = LightingContext.BrightLight;

        var koboldCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinitySensitiveToLight, "CombatAffinityKoboldLightSensitivity")
            .SetOrUpdateGuiPresentation("LightAffinityKoboldLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.None)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var conditionKoboldLightSensitive = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLightSensitive, "ConditionKoboldLightSensitive")
            .SetOrUpdateGuiPresentation("LightAffinityKoboldLightSensitivity", Category.Feature)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityKoboldLightSensitivity, koboldCombatAffinityLightSensitivity)
            .AddToDB();

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(conditionKoboldLightSensitive);

        var lightAffinityKoboldLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityKoboldLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = conditionKoboldLightSensitive
                })
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create(TrueStrike.EffectDescription)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 2)
            .SetDurationData(DurationType.Round, 1)
            .Build();

        var conditionKoboldGrovelCowerAndBeg = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTrueStrike, "ConditionKoboldGrovelCowerAndBeg")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = conditionKoboldGrovelCowerAndBeg;

        var powerKoboldGrovelCowerAndBeg = FeatureDefinitionPowerBuilder
            .Create("PowerKoboldGrovelCowerAndBeg")
            .SetGuiPresentation(Category.Feature, Aid)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var proficiencyKoboldLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyKoboldLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var koboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceKobold")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetRacePresentation(koboldRacePresentation)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                attributeModifierKoboldDexterityAbilityScoreIncrease,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                powerKoboldGrovelCowerAndBeg,
                CombatAffinityPackTactics,
                lightAffinityKoboldLightSensitivity,
                proficiencyKoboldLanguages)
            .AddToDB();

        raceKobold.GuiPresentation.sortOrder = Elf.GuiPresentation.sortOrder + 1;

        RacesContext.RaceScaleMap[raceKobold] = -0.04f / -0.06f;;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceKobold.name);

        return raceKobold;
    }
    
    //TODO: make one Kobold race with 2 subraces
    // @SilverGriffon WIP
    private static CharacterRaceDefinition BuildDraconicKobold()
    {
        var koboldSpriteReference = Dragonborn.GuiPresentation.SpriteReference;
        //CustomIcons.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        //var koboldSkills

        var combatAffinityKoboldDraconicCry = FeatureDefinitionCombatAffinityBuilder
            .Create(CombatAffinityParalyzedAdvantage, "CombatAffinityKoboldDraconicCry")
            .AddToDB();

        var conditionKoboldDraconicCry = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBaned, "ConditionKoboldDraconicCry")
            .SetFeatures(combatAffinityKoboldDraconicCry)
            .AddToDB();

        //var powerKoboldDraconicCry

        var spellListKoboldMagic = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListSorcerer, "SpellListKoboldMagic")
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells.ToArray())
            .FinalizeSpells()
            .AddToDB();

        var castSpellKoboldMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellKoboldMagic")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellList(spellListKoboldMagic)
            .AddToDB();

        var koboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        //koboldRacePresentation.defaultMusculature = 80;

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceKobold")
            .SetGuiPresentation(Category.Race, koboldSpriteReference)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetRacePresentation(koboldRacePresentation)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetBaseHeight(36)
            .SetBaseWeight(35)
            .SetFeaturesAtLevel(1,
                // FeatureDefinitionFeatureSets.FeatureSetHalfElfAbilityScoreIncrease,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionProficiencys.ProficiencyDragonbornLanguages,
                castSpellKoboldMagic)
            .AddToDB();

        raceKobold.GuiPresentation.sortOrder = Dragonborn.GuiPresentation.sortOrder - 1;

        RacesContext.RaceScaleMap[raceKobold] = -0.04f / -0.06f;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceKobold.name);

        return raceKobold;
    }
}
