using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 05

    internal static SpellDefinition BuildFarStep()
    {
        var power = FeatureDefinitionPowerBuilder
            .Create("PowerFarStep")
            .SetGuiPresentation("ConditionFarStep", Category.Condition,
                Sprites.GetSprite("PowerFarStep", Resources.PowerFarStep, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Ally, RangeType.Self, 12, TargetType.Position)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 0)
                    .Build())
                .SetParticleEffectParameters(MistyStep)
                .Build())
            .AddToDB();

        const string ConditionName = "ConditionFarStep";

        var condition = ConditionDefinitionBuilder
            .Create(ConditionName)
            .SetGuiPresentation(Category.Condition, ConditionJump)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFarStep")
                .SetGuiPresentation(ConditionName, Category.Condition)
                .SetCustomSubFeatures(new AddUsablePowerFromCondition(power))
                .AddToDB())
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("FarStep")
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite("SpellFarStep", Resources.SpellFarStep, 128))
            .SetSpellLevel(5)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 12, TargetType.Position)
                .SetEffectForms(EffectFormBuilder.Create()
                        .SetMotionForm(MotionForm.MotionType.TeleportToDestination, 0)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true)
                        .Build())
                .SetParticleEffectParameters(MistyStep)
                .Build())
            .AddToDB();
    }

    #endregion
}
