using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region Mind Blank

    internal static SpellDefinition BuildMindBlank()
    {
        const string NAME = "MindBlank";

        var spriteReference = Sprites.GetSprite(NAME, Resources.MindBlank, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Hour, 24)
            .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
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
                    ConditionForm.ConditionOperation.Add)
                .Build())
            .Build();

        return SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(8)
            .SetCastingTime(ActivationTime.Action)
            .SetSomaticComponent(false)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetEffectDescription(effectDescription)
            .SetAiParameters(new SpellAIParameters())
            .AddToDB();
    }

    #endregion
}
