using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
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
            .SetCustomSubFeatures(ValidatorsRestrictedContext.WeaponAttack)
            .SetDamageDice(DieType.D10, 5)
            .SetSpecificDamageType(DamageTypeForce)
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
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolAbjuration)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder.Create()
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
            .SetGuiPresentation(Category.Spell, Sprites.FarStep)
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

    internal static SpellDefinition BuildMantleOfThorns()
    {
        const string NAME = "MantleOfThorns";

        //Leaving this proxy in case someone already has spell in effect
        EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxySpikeGrowth, $"EffectProxy{NAME}")
            .SetCanMove()
            .SetCanMoveOnCharacters()
            .AddToDB();

        var effectDescription = EffectDescriptionBuilder.Create()
            .SetParticleEffectParameters(SpikeGrowth)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 3)
            .SetDurationData(DurationType.Minute, 1)
            .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnMove | RecurrentEffect.OnTurnStart)
            .AddEffectForms(
                EffectFormBuilder.DamageForm(DamageTypePiercing, 2, DieType.D8),
                EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, false),
                EffectFormBuilder.TopologyForm(TopologyForm.Type.DifficultThrough, false)
            )
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("MantleOfThorns", Resources.MantleOfThorns, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetVerboseComponent(true)
            .SetRequiresConcentration(true)
            .SetEffectDescription(effectDescription)
            .AddToDB();

        return spell;
    }

    #endregion
}
