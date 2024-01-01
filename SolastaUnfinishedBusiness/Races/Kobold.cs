using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceKoboldBuilder
{
    internal static CharacterRaceDefinition RaceKobold { get; } = BuildKobold();
    internal static CharacterRaceDefinition SubraceDarkKobold { get; private set; }

    [NotNull]
    private static CharacterRaceDefinition BuildKobold()
    {
        var koboldSpriteReference = Sprites.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        var proficiencyKoboldLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyKoboldLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceKobold")
            .SetGuiPresentation(Category.Race, koboldSpriteReference)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseHeight(30)
            .SetBaseWeight(30)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                proficiencyKoboldLanguages)
            .AddToDB();

        SubraceDarkKobold = BuildDarkKobold(raceKobold);

        raceKobold.subRaces =
            [SubraceDarkKobold, BuildDraconicKobold(raceKobold)];
        RacesContext.RaceScaleMap[raceKobold] = 6f / 9.4f;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceKobold.name);

        return raceKobold;
    }

    [NotNull]
    private static CharacterRaceDefinition BuildDarkKobold(CharacterRaceDefinition characterRaceDefinition)
    {
        var koboldSpriteReference = Sprites.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        var featureSetPactTactics = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetPackTactics")
            .SetGuiPresentation("CombatAffinityPackTactics", Category.Feature)
            .AddFeatureSet(CombatAffinityPackTactics)
            .AddToDB();

        var lightAffinityDarkKoboldLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityDarkKoboldLightSensitivity")
            .SetGuiPresentation(CustomConditionsContext.LightSensitivity.Name, Category.Condition)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = CustomConditionsContext.LightSensitivity
                })
            .AddToDB();

        var conditionDarkKoboldGrovelCowerAndBeg = ConditionDefinitionBuilder
            .Create(CustomConditionsContext.Distracted, "ConditionDarkKoboldGrovelCowerAndBeg")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        conditionDarkKoboldGrovelCowerAndBeg.SpecialInterruptions.Clear();

        var effectDescription = EffectDescriptionBuilder
            .Create(TrueStrike.EffectDescription)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .Build();

        effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = conditionDarkKoboldGrovelCowerAndBeg;

        var powerDarkKoboldGrovelCowerAndBeg = FeatureDefinitionPowerBuilder
            .Create("PowerDarkKoboldGrovelCowerAndBeg")
            .SetGuiPresentation(Category.Feature, Aid)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var raceDarkKobold = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceDarkKobold")
            .SetGuiPresentation(Category.Race, koboldSpriteReference)
            .SetFeaturesAtLevel(1,
                AttributeModifierElfAbilityScoreIncrease,
                featureSetPactTactics,
                lightAffinityDarkKoboldLightSensitivity,
                powerDarkKoboldGrovelCowerAndBeg)
            .AddToDB();

        return raceDarkKobold;
    }

    private static CharacterRaceDefinition BuildDraconicKobold(CharacterRaceDefinition characterRaceDefinition)
    {
        var koboldSpriteReference = Sprites.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        var powerDraconicKoboldDraconicCry = FeatureDefinitionPowerBuilder
            .Create("PowerDraconicKoboldDraconicCry")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDraconicCry", Resources.PowerDraconicCry, 256, 128))
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 5)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(CustomConditionsContext.Distracted, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var spellListDraconicKobold = SpellListDefinitionBuilder
            .Create("SpellListDraconicKobold")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use sorcerer spell list, so custom cantrips selected for sorcerer will show here 
        spellListDraconicKobold.SpellsByLevel[0].Spells =
            SpellListDefinitions.SpellListSorcerer.SpellsByLevel[0].Spells;

        var castSpellDraconicKoboldMagic = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellDraconicKoboldMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
            .SetReplacedSpells(1, 0)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(spellListDraconicKobold)
            .AddToDB();

        var raceDraconicKobold = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceDraconicKobold")
            .SetGuiPresentation(Category.Race, koboldSpriteReference)
            .SetFeaturesAtLevel(1,
                FlexibleRacesContext.AttributeChoiceThree.FeatureDefinition,
                powerDraconicKoboldDraconicCry,
                castSpellDraconicKoboldMagic)
            .AddToDB();

        return raceDraconicKobold;
    }
}
