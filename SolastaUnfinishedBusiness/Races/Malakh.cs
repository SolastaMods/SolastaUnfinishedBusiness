using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Races;
internal static class RaceMalakhBuilder
{
    private const string Name = "Malakh";

    internal static CharacterRaceDefinition RaceMalakh { get; } = BuildMalakh();

    internal static int ANGELIC_FORM_LEVEL = 3;

    [NotNull]
    private static CharacterRaceDefinition BuildMalakh()
    {
        var malakhSpriteReference = Sprites.GetSprite("Malakh", Resources.Malakh, 1024, 512);
        var featureSetMalakhAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}CharismaAbilityScoreIncrease")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Charisma, 2)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create($"PointPool{Name}AbilityScore")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.AbilityScore, 1)
                    .RestrictChoices(
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Constitution)
                    .AddToDB()
            )
            .AddToDB();

        var featureSetMalakhDivineResistance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DivineResistance")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance
                )
            .AddToDB();


        var featureSetMalakhLanguages = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Languages")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{Name}LanguageCommon")
                    .SetGuiPresentationNoContent(true)
                    .SetProficiencies(ProficiencyType.Language, "Language_Common")
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create($"PointPool{Name}LanguageAdditional")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Language, 1)
                    .AddToDB())
            .AddToDB();


        var spellListMalakh = SpellListDefinitionBuilder
            .Create($"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .FinalizeSpells()
            .AddToDB();

        // Use instead of bonus cantrip to add spell casting ability
        spellListMalakh.SpellsByLevel[0].Spells = new List<SpellDefinition>()
        {
            SpellDefinitions.Light
        };

        var castSpellMalakhMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, $"CastSpell{Name}Magic")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetFocusType(EquipmentDefinitions.FocusType.None)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.None)
            .SetSpellKnowledge(SpellKnowledge.FixedList)
            .SetSpellList(spellListMalakh)
            .AddToDB();

        var powerMalakhHealingTouch = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HealingTouch")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.CureWounds)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
                .SetDurationData(DurationType.Instantaneous)
                .AddEffectForms(EffectFormBuilder.Create()
                    .SetHealingForm(
                        HealingComputation.Dice,
                        0,
                        DieType.D1,
                        0,
                        false,
                        HealingCap.MaximumHitPoints)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                    .Build())
                .Build())
            .AddToDB();

        var additionalDamageMalakhAngelicForm = FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{Name}AngelicForm")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("AngelicForm")
                .SetAdditionalDamageType(AdditionalDamageType.Specific)
                .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
                .SetSpecificDamageType(DamageTypeRadiant)
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                .AddToDB();

        var powerMalakhAngelicFlight = BuildAngelicFlight(additionalDamageMalakhAngelicForm);
        var powerMalakhAngelicRadiance = BuildAngelicRadiance(additionalDamageMalakhAngelicForm);
        var powerMalakhAngelicVisage = BuildAngelicVisage(additionalDamageMalakhAngelicForm);

        RacePresentation racePresentation = Human.RacePresentation.DeepCopy();
        // disables the origin image from appearing
        racePresentation.originOptions.RemoveRange(1, racePresentation.originOptions.Count - 1);

        var raceMalakh = CharacterRaceDefinitionBuilder
            .Create(Human, "RaceMalakh")
            .SetGuiPresentation(Category.Race, malakhSpriteReference)
            .SetRacePresentation(racePresentation)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                featureSetMalakhAbilityScoreIncrease,
                featureSetMalakhDivineResistance,
                featureSetMalakhLanguages,
                castSpellMalakhMagic,
                powerMalakhHealingTouch
                )
            .AddToDB();

        // refactored to subrace because featureset doesn't seem to work in higher levels
        raceMalakh.subRaces =
            new List<CharacterRaceDefinition> {
                    BuildMalakhSubrace(raceMalakh, "Herald", powerMalakhAngelicFlight),
                    BuildMalakhSubrace(raceMalakh, "Guardian", powerMalakhAngelicRadiance),
                    BuildMalakhSubrace(raceMalakh, "Judgement", powerMalakhAngelicVisage),
            };


        return raceMalakh;
    }

    private static CharacterRaceDefinition BuildMalakhSubrace(
        CharacterRaceDefinition raceMalakh, 
        string suffix, FeatureDefinition powerAngelicForm)
    {
        var subraceName = $"Race{Name}{suffix}";
        return CharacterRaceDefinitionBuilder
            .Create(raceMalakh, subraceName)
            .SetOrUpdateGuiPresentation(subraceName, Category.Race)
            .SetFeaturesAtLevel(3,
                powerAngelicForm)
            .AddToDB();
    }

    private static FeatureDefinition BuildAngelicVisage(
        FeatureDefinitionAdditionalDamage additionalDamageMalakhAngelicForm)
    {
        var conditionAngelicVisage = ConditionDefinitionBuilder
            .Create($"Condition{Name}AngelicVisage")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionDivineFavor)
            .SetSpecialDuration(DurationType.Minute, 1)
            .SetConditionType(ConditionType.Beneficial)
            .CopyParticleReferences(ConditionDefinitions.ConditionFlyingAdaptive)
            .AddFeatures(additionalDamageMalakhAngelicForm)
            .AddToDB();

        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicVisage")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 2)
                .SetSavingThrowData(true,
                    AttributeDefinitions.Charisma, true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, 
                    AttributeDefinitions.Charisma)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(ConditionDefinitions.ConditionFrightenedFear, ConditionForm.ConditionOperation.Add)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(conditionAngelicVisage, ConditionForm.ConditionOperation.Add, true, true)
                        .Build()
                        )
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildAngelicFlight(
        FeatureDefinitionAdditionalDamage additionalDamageMalakhAngelicForm)
    {
        var conditionAngelicFlight = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{Name}AngelicFlight")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionDivineFavor)  
            .SetConditionType(ConditionType.Beneficial)
            .AddFeatures(additionalDamageMalakhAngelicForm)
            .AddToDB();

        return FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicFlight")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(conditionAngelicFlight, ConditionForm.ConditionOperation.Add, true)
                        .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildAngelicRadiance(FeatureDefinition additionalDamageMalakhAngelicForm)
    {
        var featureMalakhRadiantAura = FeatureDefinitionBuilder
            .Create($"Feature{Name}RadiantAura")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new MalakhRadiantDamageOnTurnEnd())
            .AddToDB();

        var conditionAngelicRadiance = ConditionDefinitionBuilder
            .Create($"Condition{Name}AngelicRadiance")
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionDivineFavor)
            .SetConditionType(ConditionType.Beneficial)
            .CopyParticleReferences(ConditionDefinitions.ConditionFlyingAdaptive)
            .AddFeatures(additionalDamageMalakhAngelicForm, featureMalakhRadiantAura)
            .AddToDB();

        var faerieFireLightSource =
            SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);
        var powerMalakhAngelicRadiance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AngelicRadiance")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(conditionAngelicRadiance, ConditionForm.ConditionOperation.Add, true)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetLightSourceForm(
                            LightSourceType.Sun, 2, 2, 
                            faerieFireLightSource.lightSourceForm.color, 
                            faerieFireLightSource.lightSourceForm.graphicsPrefabReference)
                            .Build())
                .Build())
            .AddToDB();

        return powerMalakhAngelicRadiance;
    }

    private class MalakhRadiantDamageOnTurnEnd : ICharacterTurnEndListener
    {
        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var battle = gameLocationBattleService.Battle;

            if (battle == null)
            {
                return;
            }
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            var classLevel = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            DieType dieType;

            switch (classLevel)
            {
                case < 5:
                    dieType = DieType.D4;
                    break;
                case < 9:
                    dieType = DieType.D6;
                    break;
                case < 13:
                    dieType = DieType.D8;
                    break;
                case < 17:
                    dieType = DieType.D10;
                    break;
                default:
                    dieType = DieType.D12;
                    break;
            }

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            var damageForm = new DamageForm
            {
                DamageType = DamageTypeRadiant,
                DieType = dieType,
                DiceNumber = 1,
                BonusDamage = 0,
                IgnoreCriticalDoubleDice = true
            };
            var rulesetActor = locationCharacter.RulesetActor;
            foreach (var enemy in gameLocationBattleService.Battle.EnemyContenders
                         .Where(enemy => enemy.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .Where(enemy => rulesetActor.DistanceTo(enemy.RulesetActor) <= 3)
                         .ToList()) // avoid changing enumerator
            {
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetCharacter,
                    targetCharacter = enemy.RulesetCharacter,
                    position = enemy.LocationPosition
                };
                implementationService.ApplyEffectForms(
                    new List<EffectForm> { new() { damageForm = damageForm } },
                    applyFormsParams,
                    new List<string> { DamageTypeRadiant },
                    out _,
                    out _);
            }
        }
    }
}
