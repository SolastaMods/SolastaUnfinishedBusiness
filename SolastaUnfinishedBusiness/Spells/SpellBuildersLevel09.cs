#if false
// Spell/&ForesightDescription=You touch a willing creature and bestow a limited ability to see into the immediate future. For the duration, the target can't be surprised and has advantage on attack rolls, ability checks, and saving throws. Additionally, other creatures have disadvantage on attack rolls against the target for the duration.
// Spell/&ForesightTitle=Foresight
// Spell/&MassHealDescription=A flood of healing energy flows from you into injured creatures around you. You restore 120 hit points each to 6 creatures that you can see within range. Creatures healed by this spell are also cured of all diseases and any effect making them blinded or deafened. This spell has no effect on undead or constructs. 
// Spell/&MassHealTitle=Mass Heal
// Spell/&MeteorSwarmSingleTargetDescription=Blazing orbs of fire plummet to the ground at a single point you can see within range. Each creature in a 40-foot-radius sphere centered on the point you choose must make a Dexterity saving throw. The sphere spreads around corners. A creature takes 20d6 fire damage and 20d6 bludgeoning damage on a failed save, or half as much damage on a successful one. A creature in the area of more than one fiery burst is affected only once.
// Spell/&MeteorSwarmSingleTargetTitle=Meteor Swarm [Single Target]
// Spell/&PowerWordHealDescription=A wave of healing energy washes over the creature you touch. The target regains all its hit points. If the creature is charmed, frightened, paralyzed, or stunned, the condition ends. If the creature is prone, it can use its reaction to stand up. This spell has no effect on undead or constructs.
// Spell/&PowerWordHealTitle=Power Word Heal
// Spell/&PowerWordKillDescription=You utter a word of power that can compel one creature you can see within range to die instantly. If the creature you choose has 100 hit points or fewer, it dies. Otherwise, the spell has no effect.
// Spell/&PowerWordKillTitle=Power Word Kill
// Spell/&TimeStopDescription=You briefly stop the flow of time for everyone but yourself. No time passes for other creatures, while you take 1d4 + 1 turns in a row, during which you can use actions and move as normal.
// Spell/&TimeStopTitle=Time Stop
// Spell/&ShapechangeDescription=You assume the form of a different creature for the duration. The new form can be of any creature with a challenge rating equal to your level or lower.
// Spell/&ShapechangeTitle=Shapechange
// Spell/&WeirdDescription=Drawing on the deepest fears of a group of creatures, you create illusory creatures in their minds, visible only to them. Each creature in a 30-foot-radius sphere centered on a point of your choice within range must make a Wisdom saving throw. On a failed save, a creature becomes frightened for the duration. The illusion calls on the creature's deepest fears, manifesting its worst nightmares as an implacable threat. At the end of each of the frightened creature's turns, it must succeed on a Wisdom saving throw or take 4d10 psychic damage. On a successful save, the spell ends for that creature.
// Spell/&WeirdTitle=Weird
// Condition/&ConditionTimeStopDescription=Time is frozen for the affected creature.
// Condition/&ConditionTimeStopTitle=Time Stopped
// Condition/&ConditionWeirdDescription=Frightened. At the end of each turn, make a Wisdom saving throw. On a failure, take 4d10 psychic damage. On a success, this condition ends.
// Condition/&ConditionWeirdTitle=Weirded
// Condition/&ConditionForesightDescription=Advantage on all attack rolls, ability checks, and saving throws. Other creatures have disadvantage on attack rolls.
// Condition/&ConditionForesightTitle=Foresight
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static EffectForm;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 09

    internal static SpellDefinition BuildForesight()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(
                DurationType.Hour,
                8)
            .SetTargetingData(
                Side.Ally,
                RangeType.Touch,
                1,
                TargetType.Individuals)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionForesight")
                        .SetOrUpdateGuiPresentation(Category.Condition)
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
                    false)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("Foresight")
            .SetGuiPresentation(Category.Spell, TrueSeeing)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Minute1)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildMassHeal()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Individuals, 6)
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
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildMeteorSwarmSingleTarget()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.All,
                RangeType.Distance,
                200,
                TargetType.Sphere,
                8,
                8)
            // 20 dice number because hits dont stack even on single target
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeFire, 20, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeBludgeoning, 0, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                20)
            .SetParticleEffectParameters(FlameStrike)
            .Build();

        return SpellDefinitionBuilder
            .Create("MeteorSwarmSingleTarget")
            .SetGuiPresentation(Category.Spell, FlamingSphere)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildPowerWordHeal()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.Ally,
                RangeType.Distance,
                12,
                TargetType.Individuals)
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
                        ConditionCharmedByHypnoticPattern,
                        ConditionDefinitions.ConditionFrightened,
                        ConditionDefinitions.ConditionFrightenedFear,
                        ConditionFrightenedPhantasmalKiller,
                        ConditionDefinitions.ConditionParalyzed,
                        ConditionParalyzed_CrimsonSpiderVenom,
                        ConditionParalyzed_GhoulsCaress,
                        ConditionDefinitions.ConditionStunned,
                        ConditionStunned_MutantApeSlam,
                        ConditionStunnedConjuredDeath,
                        ConditionDefinitions.ConditionProne)
                    .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("PowerWordHeal")
            .SetGuiPresentation(Category.Spell, HealingWord)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildPowerWordKill()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(
                Side.Enemy,
                RangeType.Distance,
                12,
                TargetType.Individuals)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetKillForm(
                        KillCondition.UnderHitPoints,
                        0F,
                        100)
                    .SetLevelAdvancement(
                        LevelApplianceType.No,
                        LevelSourceType.ClassLevel)
                    .CreatedByCharacter()
                    .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("PowerWordKill")
            .SetGuiPresentation(Category.Spell, Disintegrate)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildShapechange()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetCreatedByCharacter()
            .SetParticleEffectParameters(PowerDruidWildShape)
            .SetDurationData(
                DurationType.Hour,
                1)
            .SetTargetingData(
                Side.Ally,
                RangeType.Distance,
                12,
                TargetType.Self)
            .SetEffectForms(
                new EffectForm
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
                            new() { requiredLevel = 1, substituteMonster = GoldDragon_AerElai },
                            new() { requiredLevel = 1, substituteMonster = Divine_Avatar },
                            new() { requiredLevel = 1, substituteMonster = Sorr_Akkath_Tshar_Boss },
                            new()
                            {
                                requiredLevel = 1, substituteMonster = GreenDragon_MasterOfConjuration
                            },
                            new() { requiredLevel = 1, substituteMonster = BlackDragon_MasterOfNecromancy },
                            new() { requiredLevel = 1, substituteMonster = Remorhaz },
                            new() { requiredLevel = 1, substituteMonster = Emperor_Laethar },
                            new() { requiredLevel = 1, substituteMonster = Giant_Ape },
                            new() { requiredLevel = 1, substituteMonster = Spider_Queen },
                            new() { requiredLevel = 1, substituteMonster = Sorr_Akkath_Shikkath }
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
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    internal static SpellDefinition BuildTimeStop()
    {
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
                        .SetSpecialInterruptions(
                            ConditionInterruption.Attacked,
                            ConditionInterruption.Damaged)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .Build())
            .ExcludeCaster()
            .Build();

        return SpellDefinitionBuilder
            .Create("TimeStop")
            .SetGuiPresentation(Category.Spell, PowerDomainLawWordOfLaw)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Divination)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    internal static SpellDefinition BuildWeird()
    {
        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Sphere, 6, 6)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Wisdom,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Constitution,
                20)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionFrightenedPhantasmalKiller, "ConditionWeird")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .HasSavingThrow(EffectSavingThrowType.Negates)
                .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("Weird")
            .SetGuiPresentation(Category.Spell, PhantasmalKiller)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(9)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion
}
#endif
