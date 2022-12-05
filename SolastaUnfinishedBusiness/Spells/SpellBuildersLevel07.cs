using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 07

    internal static SpellDefinition BuildReverseGravity()
    {
        const string ReverseGravityName = "ReverseGravity";

        var effectDescription = EffectDescriptionBuilder.Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                true,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Dexterity,
                13)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        ConditionDefinitionBuilder
                            .Create(ConditionDefinitions.ConditionLevitate, "ConditionReverseGravity")
                            .SetOrUpdateGuiPresentation(Category.Condition)
                            .SetConditionType(ConditionType.Neutral)
                            .SetFeatures(
                                FeatureDefinitionMovementAffinitys.MovementAffinityConditionLevitate,
                                FeatureDefinitionMoveModes.MoveModeFly2
                            )
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        false,
                        false)
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
            .Build();

        return SpellDefinitionBuilder
            .Create(ReverseGravityName)
            .SetGuiPresentation(Category.Spell, Thunderwave)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(7)
            .SetCastingTime(ActivationTime.Action)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .SetRequiresConcentration(true)
            .AddToDB();
    }

    #endregion
}
