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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

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

        var koboldRacePresentation = Dragonborn.RacePresentation.DeepCopy();

        var raceKobold = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceKobold")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetRacePresentation(koboldRacePresentation)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetBaseWeight(30)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                attributeModifierKoboldDexterityAbilityScoreIncrease,
                SenseDarkvision12,
                powerKoboldGrovelCowerAndBeg,
                CombatAffinityPackTactics,
                lightAffinityKoboldLightSensitivity,
                ProficiencyDragonbornLanguages)
            .AddToDB();

        raceKobold.GuiPresentation.sortOrder = Elf.GuiPresentation.sortOrder + 1;

        RacesContext.RaceScaleMap[raceKobold] = 3.0f / 6.4f;

        return raceKobold;
    }
}
