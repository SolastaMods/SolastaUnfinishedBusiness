using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 04

    internal static SpellDefinition BuildStaggeringSmite()
    {
        const string NAME = "StaggeringSmite";

        var conditionStaggeringSmiteEnemy = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Enemy")
            .SetGuiPresentation($"AdditionalDamage{NAME}", Category.Feature, ConditionDazzled)
            .SetSpecialDuration(DurationType.Round, 1)
            .AddFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell)
                    .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{NAME}")
                    .SetGuiPresentation(NAME, Category.Spell)
                    .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .SetAllowedActionTypes(reaction: false)
                    .AddToDB())
            .AddToDB();

        var additionalDamageStaggeringSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NAME)
            .SetCustomSubFeatures(ValidatorsRestrictedContext.WeaponAttack)
            .SetDamageDice(DieType.D6, 4)
            .SetSpecificDamageType(DamageTypePsychic)
            .SetSavingThrowData(
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                AttributeDefinitions.Wisdom)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = false,
                    saveAffinity = EffectSavingThrowType.Negates,
                    conditionDefinition = conditionStaggeringSmiteEnemy,
                    operation = ConditionOperationDescription.ConditionOperation.Add
                })
            .AddToDB();

        var conditionStaggeringSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageStaggeringSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.StaggeringSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(conditionStaggeringSmite, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildBrainBulwark()
    {
        const string NAME = "BrainBulwark";

        var conditionBrainBulwark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionBlessed)
            .SetPossessive()
            .SetFeatures(
                DamageAffinityPsychicResistance,
                ConditionAffinityFrightenedImmunity,
                ConditionAffinityFrightenedFearImmunity,
                ConditionAffinityMindControlledImmunity,
                ConditionAffinityMindDominatedImmunity)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BrainBulwark, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(4)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Hour, 1)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel,
                        additionalTargetsPerIncrement: 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBrainBulwark, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildGravitySinkhole()
    {
        const string NAME = "GravitySinkhole";

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.GravitySinkhole, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 4)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Constitution,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetParticleEffectParameters(Shatter.EffectDescription.EffectParticleParameters)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 4)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 5, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(4)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetCustomSubFeatures(PushesOrDragFromEffectPoint.Marker)
            .AddToDB();

        return spell;
    }

    #endregion
}
