using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaCommunityExpansion.Models.SpellsContext;

namespace SolastaCommunityExpansion.Spells;

public static class SrdSpells
{
    private static readonly Guid DhBaseGuid = new("05c1b1dbae144731b4505c1232fdc37e");

    internal static void Register()
    {
        // 7th level
        RegisterSpell(BuildReverseGravity(), 0, SpellListDruid, SpellListWizard, SpellListSorcerer);

        // 8th level
        RegisterSpell(BuildMindBlank(), 0, SpellListWarlock, SpellListWizard);

        // 9th level
        RegisterSpell(BuildForesight(), 0, SpellListWarlock, SpellListDruid, SpellListWizard);
        RegisterSpell(BuildMassHeal(), 0, SpellListCleric);
        RegisterSpell(BuildMeteorSwarmSingleTarget(), 0, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildPowerWordHeal(), 0, SpellListCleric);
        RegisterSpell(BuildPowerWordKill(), 0, SpellListWarlock, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildTimeStop(), 0, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildShapechange(), 0, SpellListDruid, SpellListWizard);
        RegisterSpell(BuildWeird(), 0, SpellListWarlock, SpellListWizard);
    }

    private static SpellDefinition BuildReverseGravity()
    {
        const string TEXT = "DHReverseGravitySpell";

        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Minute,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Cylinder,
                10,
                10)
            .SetSavingThrowData(
                true,
                false,
                SmartAttributeDefinitions.Dexterity.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                SmartAttributeDefinitions.Dexterity.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>())
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    ReverseGravityConditionBuilder.ReverseGravityCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .Build())
            .AddEffectForm(new EffectFormBuilder()
                .SetMotionForm(
                    MotionForm.MotionType.Levitate,
                    10)
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .Build())
            .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect);

        return SpellDefinitionBuilder
            .Create(TEXT, DhBaseGuid)
            .SetGuiPresentation(Category.Spell, Thunderwave.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    private static SpellDefinition BuildMindBlank()
    {
        var effectDescription = new EffectDescriptionBuilder();
        effectDescription.SetDurationData(
            RuleDefinitions.DurationType.Hour,
            24,
            RuleDefinitions.TurnOccurenceType.EndOfTurn
        );
        effectDescription.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Individuals
        );
        effectDescription.AddEffectForm(
            new EffectFormBuilder().SetConditionForm(
                    MindBlankConditionBuilder.MindBlankCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                )
                .Build()
        );

        return SpellDefinitionBuilder
            .Create("DHMindBlankSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, MindTwist.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildForesight()
    {
        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Hour,
                8,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Touch,
                1,
                RuleDefinitions.TargetType.Individuals)
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    ForesightConditionBuilder.ForesightCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .Build());

        return SpellDefinitionBuilder
            .Create("DHForesightSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, TrueSeeing.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Minute1)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildMassHeal()
    {
        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals,
                6)
            .AddEffectForm(new EffectFormBuilder()
                .SetHealingForm(
                    RuleDefinitions.HealingComputation.Dice,
                    120,
                    RuleDefinitions.DieType.D1,
                    0,
                    false,
                    RuleDefinitions.HealingCap.MaximumHitPoints)
                .Build());

        return SpellDefinitionBuilder
            .Create("DHMassHealSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, Heal.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Distance,
                200,
                RuleDefinitions.TargetType.Sphere,
                8,
                8)
            .AddEffectForm(
                new EffectFormBuilder().SetDamageForm(
                        false,
                        RuleDefinitions.DieType.D6,
                        RuleDefinitions.DamageTypeFire,
                        0,
                        RuleDefinitions.DieType.D6,
                        20, // 20 because hits dont stack even on single target
                        RuleDefinitions.HealFromInflictedDamage.Never,
                        new List<RuleDefinitions.TrendInfo>())
                    .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                    .Build())
            .AddEffectForm(new EffectFormBuilder()
                .SetDamageForm(
                    false,
                    RuleDefinitions.DieType.D6,
                    RuleDefinitions.DamageTypeBludgeoning,
                    0,
                    RuleDefinitions.DieType.D6,
                    20, // 20 because hits dont stack even on single target
                    RuleDefinitions.HealFromInflictedDamage.Never,
                    new List<RuleDefinitions.TrendInfo>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
                .Build())
            .SetSavingThrowData(
                true,
                false,
                SmartAttributeDefinitions.Dexterity.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                SmartAttributeDefinitions.Dexterity.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>())
            .SetParticleEffectParameters(FlameStrike.EffectDescription.EffectParticleParameters);

        return SpellDefinitionBuilder
            .Create("DHMeteorSwarmSingleTargetSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, FlamingSphere.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildPowerWordHeal()
    {
        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals)
            .AddEffectForm(new EffectFormBuilder().SetHealingForm(
                    RuleDefinitions.HealingComputation.Dice,
                    700,
                    RuleDefinitions.DieType.D1,
                    0,
                    false,
                    RuleDefinitions.HealingCap.MaximumHitPoints)
                .Build())
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    ConditionParalyzed,
                    ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                    false,
                    false,
                    new List<ConditionDefinition>
                    {
                        ConditionCharmed,
                        ConditionCharmedByHypnoticPattern,
                        ConditionFrightened,
                        ConditionFrightenedFear,
                        ConditionFrightenedPhantasmalKiller,
                        ConditionParalyzed,
                        ConditionParalyzed_CrimsonSpiderVenom,
                        ConditionParalyzed_GhoulsCaress,
                        ConditionStunned,
                        ConditionStunned_MutantApeSlam,
                        ConditionStunnedConjuredDeath,
                        ConditionProne
                    })
                .Build());

        return SpellDefinitionBuilder
            .Create("DHPowerWordHealSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, HealingWord.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildPowerWordKill()
    {
        var killForm = new KillForm {killCondition = RuleDefinitions.KillCondition.UnderHitPoints, hitPoints = 100};

        var effectForm = new EffectForm
        {
            applyLevel = EffectForm.LevelApplianceType.No,
            levelMultiplier = 1,
            levelType = RuleDefinitions.LevelSourceType.ClassLevel,
            createdByCharacter = true,
            formType = EffectForm.EffectFormType.Kill,
            killForm = killForm
        };

        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Instantaneous,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Individuals)
            .AddEffectForm(effectForm);

        return SpellDefinitionBuilder
            .Create("DHPowerWordKillSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, Disintegrate.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildShapechange()
    {
        var shapeChangeForm = new ShapeChangeForm
        {
            keepMentalAbilityScores = true,
            shapeChangeType = ShapeChangeForm.Type.FreeListSelection,
            specialSubstituteCondition = ConditionWildShapeSubstituteForm
        };

        shapeChangeForm.ShapeOptions.AddRange
        (
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(GoldDragon_AerElai),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Divine_Avatar),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Sorr_Akkath_Tshar_Boss),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(GreenDragon_MasterOfConjuration),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(BlackDragon_MasterOfNecromancy),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Remorhaz),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Emperor_Laethar),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Giant_Ape),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Spider_Queen),
            new ShapeOptionDescription().SetRequiredLevel(1).SetSubstituteMonster(Sorr_Akkath_Shikkath)
        );

        var effectForm = new EffectForm
        {
            addBonusMode = RuleDefinitions.AddBonusMode.None,
            applyLevel = EffectForm.LevelApplianceType.No,
            canSaveToCancel = false,
            createdByCharacter = true,
            formType = EffectForm.EffectFormType.ShapeChange,
            shapeChangeForm = shapeChangeForm
        };


        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Hour,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Self)
            .AddEffectForm(effectForm)
            .SetCreatedByCharacter()
            .SetParticleEffectParameters(PowerDruidWildShape.EffectDescription.EffectParticleParameters);

        return SpellDefinitionBuilder
            .Create("DHShapechangeSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, PowerDruidWildShape.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    private static SpellDefinition BuildTimeStop()
    {
        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Round,
                3,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.All,
                RuleDefinitions.RangeType.Self,
                0,
                RuleDefinitions.TargetType.Cylinder,
                20,
                10)
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    TimeStopConditionBuilder.TimeStopCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()).Build())
            .ExcludeCaster();

        return SpellDefinitionBuilder
            .Create("DHTimeStopSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, PowerDomainLawWordOfLaw.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    private static SpellDefinition BuildWeird()
    {
        var effectDescription = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Minute,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Distance,
                12,
                RuleDefinitions.TargetType.Sphere,
                6,
                6)
            .SetSavingThrowData(
                true,
                false,
                SmartAttributeDefinitions.Wisdom.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                SmartAttributeDefinitions.Constitution.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>())
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    WeirdConditionBuilder.WeirdCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .Build());

        return SpellDefinitionBuilder
            .Create("DHWeirdSpell", DhBaseGuid)
            .SetGuiPresentation(Category.Spell, PhantasmalKiller.GuiPresentation.SpriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(RuleDefinitions.ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetEffectDescription(effectDescription.Build())
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    private sealed class ReverseGravityConditionBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "DHReverseGravitySpellcondition";
        private const string Guid = "809f1cef-6bdc-4b5a-93bf-275af8ab0b36";

        internal static readonly ConditionDefinition ReverseGravityCondition = CreateAndAddToDB(Name, Guid);

        private ReverseGravityConditionBuilder(string name, string guid) : base(ConditionLevitate, name, guid)
        {
            Definition.conditionType = RuleDefinitions.ConditionType.Neutral;
            Definition.Features.SetRange
            (
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                FeatureDefinitionMoveModes.MoveModeFly2
            );
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ReverseGravityConditionBuilder(name, guid)
                .SetOrUpdateGuiPresentation("DHReverseGravitySpell", Category.Condition)
                .AddToDB();
        }
    }

    private sealed class MindBlankConditionBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "DHMindBlankSpellcondition";
        private const string Guid = "74f77a4c-b5cb-45d6-ac6d-d9fa2ebe3869";
        private const string TitleString = "Condition/&DHMindBlankSpellTitle";
        private const string DescriptionString = "Condition/&DHMindBlankSpellDescription";

        internal static readonly ConditionDefinition MindBlankCondition = CreateAndAddToDB(Name, Guid);

        private MindBlankConditionBuilder(string name, string guid) : base(ConditionBearsEndurance, name, guid)
        {
            Definition.GuiPresentation.Title = TitleString;
            Definition.GuiPresentation.Description = DescriptionString;
            Definition.Features.SetRange(
                FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity);
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new MindBlankConditionBuilder(name, guid).AddToDB();
        }
    }

    private sealed class ForesightConditionBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "DHForesightSpellcondition";
        private const string Guid = "4615c639-95f2-4c04-b904-e79f5b916b68";
        private const string TitleString = "Condition/&DHForesightSpellTitle";
        private const string DescriptionString = "Condition/&DHForesightSpellDescription";

        internal static readonly ConditionDefinition ForesightCondition = CreateAndAddToDB(Name, Guid);

        private ForesightConditionBuilder(string name, string guid) : base(ConditionBearsEndurance, name, guid)
        {
            Definition.GuiPresentation.Title = TitleString;
            Definition.GuiPresentation.Description = DescriptionString;
            Definition.Features.SetRange
            (
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionCatsGrace,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionEaglesSplendor,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionFoxsCunning,
                FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
                FeatureDefinitionCombatAffinitys.CombatAffinityStealthy,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze
            );
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForesightConditionBuilder(name, guid).AddToDB();
        }
    }

    private sealed class TimeStopConditionBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "DHTimeStopSpellCondition";
        private const string Guid = "f00e592f-61c3-4cbf-a800-97596e83028d";
        private const string TitleString = "Condition/&DHTimeStopSpellTitle";
        private const string DescriptionString = "Condition/&DHTimeStopSpellDescription";

        internal static readonly ConditionDefinition TimeStopCondition = CreateAndAddToDB(Name, Guid);

        private TimeStopConditionBuilder(string name, string guid) : base(ConditionIncapacitated, name, guid)
        {
            Definition.GuiPresentation.Title = TitleString;
            Definition.GuiPresentation.Description = DescriptionString;
            Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Attacked);
            Definition.HasSpecialInterruptionOfType(RuleDefinitions.ConditionInterruption.Damaged);
            Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Attacked);
            Definition.SpecialInterruptions.Add(RuleDefinitions.ConditionInterruption.Damaged);
            Definition.interruptionDamageThreshold = 1;
            Definition.interruptionRequiresSavingThrow = false;
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new TimeStopConditionBuilder(name, guid).AddToDB();
        }
    }

    private sealed class WeirdConditionBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "DHWeirdSpellCondition";
        private const string Guid = "0f76e7e1-4490-4ee8-a13f-a4a967ba1c08";
        private const string TitleString = "Condition/&DHWeirdSpellTitle";
        private const string DescriptionString = "Condition/&DHWeirdSpellDescription";

        internal static readonly ConditionDefinition WeirdCondition = CreateAndAddToDB(Name, Guid);

        private WeirdConditionBuilder(string name, string guid) : base(ConditionFrightenedPhantasmalKiller, name,
            guid)
        {
            Definition.GuiPresentation.Title = TitleString;
            Definition.GuiPresentation.Description = DescriptionString;

            // weird condition is the same as phantasma killer condition, just for more people
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new WeirdConditionBuilder(name, guid).AddToDB();
        }
    }
}
