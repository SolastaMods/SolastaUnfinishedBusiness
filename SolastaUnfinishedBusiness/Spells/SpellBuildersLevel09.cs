using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Foresight

    internal static SpellDefinition BuildForesight()
    {
        const string NAME = "Foresight";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MindBlank, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Minute1)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 8)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create(ConditionDefinitions.ConditionBearsEndurance, "ConditionForesight")
                                .SetOrUpdateGuiPresentation(Category.Condition)
                                .SetFeatures(
                                    AbilityCheckAffinityConditionBearsEndurance,
                                    AbilityCheckAffinityConditionBullsStrength,
                                    AbilityCheckAffinityConditionCatsGrace,
                                    AbilityCheckAffinityConditionEaglesSplendor,
                                    AbilityCheckAffinityConditionFoxsCunning,
                                    AbilityCheckAffinityConditionOwlsWisdom,
                                    CombatAffinityStealthy,
                                    SavingThrowAffinityShelteringBreeze)
                                .AddToDB()))
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Mass Heal

    internal static SpellDefinition BuildMassHeal()
    {
        return SpellDefinitionBuilder
            .Create("MassHeal")
            .SetGuiPresentation(Category.Spell, Heal)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(9)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.IndividualsUnique, 6)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(
                                HealingComputation.Dice,
                                120,
                                DieType.D1,
                                0,
                                false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(Heal)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Meteor Swarm

    internal static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        const string NAME = "MeteorSwarmSingleTarget";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MeteorSwarm, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
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
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Power Word Heal

    internal static SpellDefinition BuildPowerWordHeal()
    {
        return SpellDefinitionBuilder
            .Create("PowerWordHeal")
            .SetGuiPresentation(Category.Spell, HealingWord)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
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
                                ConditionDefinitions.ConditionFrightened,
                                ConditionDefinitions.ConditionParalyzed,
                                ConditionDefinitions.ConditionProne)
                            .Build())
                    .SetParticleEffectParameters(Regenerate)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Power Word Kill

    internal static SpellDefinition BuildPowerWordKill()
    {
        return SpellDefinitionBuilder
            .Create("PowerWordKill")
            .SetGuiPresentation(Category.Spell, Disintegrate)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetKillForm(KillCondition.UnderHitPoints, 0F, 100)
                            .Build())
                    .SetParticleEffectParameters(FingerOfDeath)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Shapechange

    internal static SpellDefinition BuildShapechange()
    {
        return SpellDefinitionBuilder
            .Create("Shapechange")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("Shapechange", Resources.ShapeChange, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent("Diamond", 1500, false)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerDruidWildShape)
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetShapeChangeForm(
                                ShapeChangeForm.Type.FreeListSelection,
                                true,
                                ConditionDefinitions.ConditionWildShapeSubstituteForm,
                                [
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = BlackDragon_MasterOfNecromancy
                                    },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Divine_Avatar },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = Emperor_Laethar
                                    },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Giant_Ape },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = GoldDragon_AerElai
                                    },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = GreenDragon_MasterOfConjuration
                                    },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Remorhaz },
                                    new ShapeOptionDescription { requiredLevel = 1, substituteMonster = Spider_Queen },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = Sorr_Akkath_Shikkath
                                    },
                                    new ShapeOptionDescription
                                    {
                                        requiredLevel = 1, substituteMonster = Sorr_Akkath_Tshar_Boss
                                    }
                                ])
                            .Build())
                    .Build())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion

    #region Time Stop

    internal static SpellDefinition BuildTimeStop()
    {
        const string NAME = "TimeStop";

        var conditionTimeStop = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionIncapacitated, "ConditionTimeStop")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetInterruptionDamageThreshold(1)
            .SetSpecialInterruptions(ConditionInterruption.Attacked, ConditionInterruption.Damaged)
            .AddToDB();

        conditionTimeStop.conditionStartParticleReference = ConditionDefinitions.ConditionPatronTimekeeperCurseOfTime
            .conditionStartParticleReference;
        conditionTimeStop.conditionParticleReference = ConditionDefinitions.ConditionPatronTimekeeperCurseOfTime
            .conditionParticleReference;
        conditionTimeStop.conditionEndParticleReference = ConditionDefinitions.ConditionPatronTimekeeperCurseOfTime
            .conditionStartParticleReference;
        conditionTimeStop.recurrentEffectParticleReference = ConditionDefinitions.ConditionPatronTimekeeperCurseOfTime
            .recurrentEffectParticleReference;

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.TimeStop, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(false)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 3)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cylinder, 20, 10)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionTimeStop))
                    .ExcludeCaster()
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Weird

    internal static SpellDefinition BuildWeird()
    {
        return SpellDefinitionBuilder
            .Create("Weird")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("Weird", Resources.Weird, 128, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolIllusion)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(
                EffectDescriptionBuilder
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
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionFrightenedPhantasmalKiller, "ConditionWeird")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .SetParticleEffectParameters(PhantasmalKiller)
                    .Build())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion
}
