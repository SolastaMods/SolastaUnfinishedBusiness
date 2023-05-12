using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
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
    #region LEVEL 06
    private static SpellDefinition BuildGravityFissure()
    {
        const string NAME = "GravityFissure";

        //var spriteReference =
        //    CustomIcons.CreateAssetReferenceSprite(NAME, Resources.GravityFissure, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(false, AttributeDefinitions.Constitution, true, EffectDifficultyClassComputation.SpellCastingFeature)
            //.SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(LightningBolt.EffectDescription.EffectParticleParameters)
            .SetTargetingData(Side.All,RangeType.Self,0,TargetType.Cube,20,3)
            //.SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Line, 20, 3)
            .AddEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(damageType: DamageTypeForce, dieType: DieType.D8, diceNumber: 8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
            ).Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, LightningBolt.GuiPresentation.SpriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(6)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }
    #endregion
}
