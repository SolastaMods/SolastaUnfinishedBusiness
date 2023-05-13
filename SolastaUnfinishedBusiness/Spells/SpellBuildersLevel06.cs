using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 06

    internal static SpellDefinition BuildGravityFissure()
    {
        const string NAME = "GravityFissure";

        var spriteReference = Sprites.GetSprite(NAME, Resources.GravityFissure, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                EffectDifficultyClassComputation.SpellCastingFeature)
            .SetParticleEffectParameters(LightningBolt.EffectDescription.EffectParticleParameters)
            //.SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cube, 20, 3)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Line, 20, 3)
            .AddEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeForce, dieType: DieType.D8, diceNumber: 8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
            ).Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
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
