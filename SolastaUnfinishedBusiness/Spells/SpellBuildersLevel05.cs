using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 05

    internal static SpellDefinition BuildBanishingSmite()
    {
        const string NAME = "BanishingSmite";

        var conditionBanishingSmiteEnemy = ConditionDefinitionBuilder
            .Create(ConditionBanished, $"Condition{NAME}Enemy")
            .SetSpecialDuration(DurationType.Minute, 1)
            .AddToDB();

        var additionalDamageBanishingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NAME)
            .SetDamageDice(DieType.D10, 5)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 5, 1, 1, 5)
            .SetSpecificDamageType(DamageTypeForce)
            .SetIgnoreCriticalDoubleDice(true)
            .SetCustomSubFeatures(new OnAttackHitEffectBanishingSmite(conditionBanishingSmiteEnemy))
            .AddToDB();

        var conditionBanishingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageBanishingSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.ThunderousSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionBanishingSmite, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildFarStep()
    {
        var condition = ConditionDefinitionBuilder
            .Create("ConditionFarStep")
            .SetGuiPresentation(Category.Condition, ConditionJump)
            .SetCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .SetSilent(Silent.None)
            .SetPossessive()
            .SetFeatures(CustomActionIdContext.FarStep)
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
            .SetEffectDescription(EffectDescriptionBuilder.Create(CustomActionIdContext.FarStep)
                .SetDurationData(DurationType.Minute, 1)
                .AddEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(condition, ConditionForm.ConditionOperation.Add, true, true)
                    .Build())
                .Build())
            .AddToDB();
    }

    #endregion
}
