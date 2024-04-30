using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Reverse Gravity

    internal static SpellDefinition BuildReverseGravity()
    {
        const string NAME = "ReverseGravity";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ReverseGravity, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionDefinitions.ConditionLevitate, "ConditionReverseGravity")
                                    .SetOrUpdateGuiPresentation(Category.Condition)
                                    .SetConditionType(ConditionType.Neutral)
                                    .SetParentCondition(ConditionDefinitions.ConditionFlying)
                                    .SetFeatures(FeatureDefinitionMoveModes.MoveModeFly2)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(
                                MotionForm.MotionType.Levitate,
                                10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Draconic Transformation

    internal static SpellDefinition BuildDraconicTransformation()
    {
        const string NAME = "DraconicTransformation";

        var sprite = Sprites.GetSprite(NAME, Resources.DraconicTransformation, 128);

        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 12)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 6, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetCasterEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(EldritchBlast)
                    .Build())
            .AddToDB();

        power.disableIfConditionIsOwned = conditionMark;

        var condition = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                power,
                FeatureDefinitionMoveModes.MoveModeFly12,
                FeatureDefinitionSenses.SenseBlindSight6)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        condition.GuiPresentation.description = Gui.NoLocalization;

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetMaterialComponent(MaterialComponentType.Specific)
            .SetSpecificMaterialComponent(TagsDefinitions.ItemTagDiamond, 500, false)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 12)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionMark, ConditionForm.ConditionOperation.Add, true,
                            true),
                        EffectFormBuilder.ConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 6, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetCasterEffectParameters(GravitySlam)
                    .SetImpactEffectParameters(EldritchBlast)
                    .Build())
            .AddToDB();
    }

    #endregion
}
