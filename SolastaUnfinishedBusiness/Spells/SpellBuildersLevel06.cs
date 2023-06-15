#if false
//Spell/&GravityFissureDescription=Manifest a ravine of gravitational energy to draw enemies in and crush them.
//Spell/&GravityFissureTitle=Gravity Fissure

using SolastaUnfinishedBusiness.Builders;
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

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, LightningBolt.GuiPresentation.SpriteReference)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Line, 20)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement: 1)
                .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetParticleEffectParameters(LightningBolt.EffectDescription.EffectParticleParameters)
                .AddEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetMotionForm(MotionForm.MotionType.DragToOrigin, 3)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .AddEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetDamageForm(DamageTypeForce, 8, DieType.D8)
                        .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                        .Build())
                .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(6)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    #endregion
}
#endif
