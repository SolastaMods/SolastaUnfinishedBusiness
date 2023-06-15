using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static EffectForm;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Foresight

    internal static SpellDefinition BuildForesight()
    {
        const string NAME = "Foresight";

        var spriteReference = Sprites.GetSprite(NAME, Resources.MindBlank, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Hour, 8)
            .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionBearsEndurance, "ConditionForesight")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBearsEndurance,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionCatsGrace,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionEaglesSplendor,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionFoxsCunning,
                            FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionOwlsWisdom,
                            FeatureDefinitionCombatAffinitys.CombatAffinityStealthy,
                            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityShelteringBreeze)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Minute1)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region Mass Heal

    internal static SpellDefinition BuildMassHeal()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetHealingForm(
                    HealingComputation.Dice,
                    120,
                    DieType.D1,
                    0,
                    false,
                    HealingCap.MaximumHitPoints)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("MassHeal")
            .SetGuiPresentation(Category.Spell, Heal)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region Meteor Swarm

    internal static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        const string NAME = "MeteorSwarmSingleTarget";

        var spriteReference = Sprites.GetSprite(NAME, Resources.MeteorSwarm, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Distance, 18, TargetType.Sphere, 8)
            // 20 dice number because hits dont stack even on single target
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 20, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 20, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Dexterity,
                13)
            .SetParticleEffectParameters(FlameStrike)
            .Build();

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region Power Word Heal

    internal static SpellDefinition BuildPowerWordHeal()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetHealingForm(
                        HealingComputation.Dice,
                        700,
                        DieType.D1,
                        0,
                        false,
                        HealingCap.MaximumHitPoints)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitions.ConditionParalyzed,
                        ConditionForm.ConditionOperation.RemoveDetrimentalAll,
                        false,
                        false,
                        ConditionDefinitions.ConditionCharmed,
                        ConditionDefinitions.ConditionCharmedByHypnoticPattern,
                        ConditionDefinitions.ConditionFrightened,
                        ConditionDefinitions.ConditionFrightenedFear,
                        ConditionDefinitions.ConditionFrightenedPhantasmalKiller,
                        ConditionDefinitions.ConditionParalyzed,
                        ConditionDefinitions.ConditionParalyzed_CrimsonSpiderVenom,
                        ConditionDefinitions.ConditionParalyzed_GhoulsCaress,
                        ConditionDefinitions.ConditionStunned,
                        ConditionDefinitions.ConditionStunned_MutantApeSlam,
                        ConditionDefinitions.ConditionStunnedConjuredDeath,
                        ConditionDefinitions.ConditionProne)
                    .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("PowerWordHeal")
            .SetGuiPresentation(Category.Spell, HealingWord)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region Power Word Kill

    internal static SpellDefinition BuildPowerWordKill()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetKillForm(KillCondition.UnderHitPoints, 0F, 100)
                    .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("PowerWordKill")
            .SetGuiPresentation(Category.Spell, Disintegrate)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region Shapechange

    internal static SpellDefinition BuildShapechange()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetParticleEffectParameters(PowerDruidWildShape)
            .SetDurationData(DurationType.Hour, 1)
            .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Self)
            .SetEffectForms(new EffectForm
            {
                addBonusMode = AddBonusMode.None,
                applyLevel = LevelApplianceType.No,
                canSaveToCancel = false,
                createdByCharacter = true,
                formType = EffectFormType.ShapeChange,
                shapeChangeForm = new ShapeChangeForm
                {
                    keepMentalAbilityScores = true,
                    shapeChangeType = ShapeChangeForm.Type.FreeListSelection,
                    specialSubstituteCondition = ConditionDefinitions.ConditionWildShapeSubstituteForm,
                    shapeOptions = new List<ShapeOptionDescription>
                    {
                        new() { requiredLevel = 1, substituteMonster = BlackDragon_MasterOfNecromancy },
                        new() { requiredLevel = 1, substituteMonster = Divine_Avatar },
                        new() { requiredLevel = 1, substituteMonster = Emperor_Laethar },
                        new() { requiredLevel = 1, substituteMonster = Giant_Ape },
                        new() { requiredLevel = 1, substituteMonster = GoldDragon_AerElai },
                        new() { requiredLevel = 1, substituteMonster = GreenDragon_MasterOfConjuration },
                        new() { requiredLevel = 1, substituteMonster = Remorhaz },
                        new() { requiredLevel = 1, substituteMonster = Spider_Queen },
                        new() { requiredLevel = 1, substituteMonster = Sorr_Akkath_Shikkath },
                        new() { requiredLevel = 1, substituteMonster = Sorr_Akkath_Tshar_Boss }
                    }
                }
            })
            .Build();

        return SpellDefinitionBuilder
            .Create("Shapechange")
            .SetGuiPresentation(Category.Spell, PowerDruidWildShape)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion

    #region Time Stop

    internal static SpellDefinition BuildTimeStop()
    {
        const string NAME = "TimeStop";

        var spriteReference = Sprites.GetSprite(NAME, Resources.TimeStop, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round, 3)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cylinder, 20, 10)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionIncapacitated, "ConditionTimeStop")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetInterruptionDamageThreshold(1)
                        .SetSpecialInterruptions(ConditionInterruption.Attacked, ConditionInterruption.Damaged)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add)
                .Build())
            .ExcludeCaster()
            .Build();

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion

    #region Weird

    internal static SpellDefinition BuildWeird()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Sphere, 6)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Wisdom,
                true,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Constitution,
                13)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionFrightenedPhantasmalKiller, "ConditionWeird")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add)
                .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("Weird")
            .SetGuiPresentation(Category.Spell, PhantasmalKiller)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolIllusion)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion
}
