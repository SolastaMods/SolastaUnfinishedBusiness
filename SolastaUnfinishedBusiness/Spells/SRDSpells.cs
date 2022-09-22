using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Models.SpellsContext;

namespace SolastaUnfinishedBusiness.Spells;

public static class SrdSpells
{
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
        const string TEXT = "ReverseGravity";

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
                    ConditionDefinitionBuilder
                        .Create(ConditionLevitate, "ConditionReverseGravity")
                        .SetGuiPresentation(Category.Condition)
                        .SetConditionType(RuleDefinitions.ConditionType.Neutral)
                        .SetFeatures(
                            FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                            FeatureDefinitionMoveModes.MoveModeFly2
                        )
                        .AddToDB(),
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
            .Create(TEXT)
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
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionMindBlank")
                        .SetGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                            FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity
                        )
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()
                )
                .Build()
        );

        return SpellDefinitionBuilder
            .Create("MindBlank")
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
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionForesight")
                        .SetGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionCatsGrace,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionEaglesSplendor,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionFoxsCunning,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
                            FeatureDefinitionCombatAffinitys.CombatAffinityStealthy,
                            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze
                        )
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .Build());

        return SpellDefinitionBuilder
            .Create("Foresight")
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
            .Create("MassHeal")
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
            .Create("MeteorSwarmSingleTarget")
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
            .Create("PowerWordHeal")
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
        var killForm = new KillForm { killCondition = RuleDefinitions.KillCondition.UnderHitPoints, hitPoints = 100 };

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
            .Create("PowerWordKill")
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
            .Create("Shapechange")
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
                    ConditionDefinitionBuilder
                        .Create(ConditionIncapacitated, "ConditionTimeStop")
                        .SetGuiPresentation(Category.Condition)
                        .SetInterruptionDamageThreshold(1)
                        .SetSpecialInterruptions(
                            RuleDefinitions.ConditionInterruption.Attacked,
                            RuleDefinitions.ConditionInterruption.Damaged)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>()).Build())
            .ExcludeCaster();

        return SpellDefinitionBuilder
            .Create("TimeStop")
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
                    ConditionDefinitionBuilder
                        .Create(ConditionFrightenedPhantasmalKiller, "ConditionWeird")
                        .SetGuiPresentation(Category.Condition)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .Build());

        return SpellDefinitionBuilder
            .Create("Weird")
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
}
