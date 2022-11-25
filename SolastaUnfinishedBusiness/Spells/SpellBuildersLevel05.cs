using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
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
        const string ConditionName = "ConditionFarStep";

        var power = FeatureDefinitionPowerBuilder
            .Create("PowerFarStep")
            .SetGuiPresentation(ConditionName, Category.Condition, Sprites.PowerFarStep)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Ally, RangeType.Self, 12, TargetType.Position)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                    .Build())
                .SetParticleEffectParameters(MistyStep)
                .Build())
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create(ConditionName)
            .SetGuiPresentation(Category.Condition, ConditionJump)
            .SetCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetFeatures(power)
            .AddToDB();

        return SpellDefinitionBuilder
            .Create("FarStep")
            .SetGuiPresentation(Category.Spell, Sprites.SpellFarStep)
            .SetSpellLevel(5)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetSomaticComponent(false)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 12, TargetType.Position)
                .SetEffectForms(EffectFormBuilder.Create()
                        .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
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
