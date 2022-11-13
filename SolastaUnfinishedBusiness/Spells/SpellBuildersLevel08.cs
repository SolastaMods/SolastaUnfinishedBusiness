using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 08

    internal static SpellDefinition BuildMindBlank()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Hour, 24)
            .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionBearsEndurance, "ConditionMindBlank")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .SetFeatures(
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunityHypnoticPattern,
                            FeatureDefinitionConditionAffinitys.ConditionAffinityCalmEmotionCharmedImmunity,
                            FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create("MindBlank")
            .SetGuiPresentation(Category.Spell, MindTwist)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion
}
