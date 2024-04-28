using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Mind Blank

    internal static SpellDefinition BuildMindBlank()
    {
        const string NAME = "MindBlank";

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.MindBlank, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSomaticComponent(true)
            .SetVerboseComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Healing)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 24)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create(ConditionBearsEndurance, "ConditionMindBlank")
                                .SetOrUpdateGuiPresentation(Category.Condition)
                                .SetFeatures(
                                    ConditionAffinityCharmImmunity,
                                    ConditionAffinityCharmImmunityHypnoticPattern,
                                    ConditionAffinityCalmEmotionCharmedImmunity,
                                    DamageAffinityPsychicImmunity)
                                .AddToDB()))
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();
    }

    #endregion

    #region Maddening Darkness

    internal static SpellDefinition BuildMaddeningDarkness()
    {
        const string NAME = "MaddeningDarkness";

        return SpellDefinitionBuilder
            .Create(Darkness, NAME)
            .SetOrUpdateGuiPresentation(Category.Spell)
            .SetSpellLevel(8)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Darkness)
                    .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Sphere, 12)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypePsychic, 8, DieType.D8)
                            .Build())
                    .SetParticleEffectParameters(DispelMagic)
                    .Build())
            .AddToDB();
    }

    #endregion
}
