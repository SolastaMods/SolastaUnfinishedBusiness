using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static EffectForm;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Models.SpellsContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static class HolicSpells
{
    internal static void Register()
    {
        RegisterSpell(BuildAcidClaw(), 0, SpellListDruid);
        RegisterSpell(BuildAirBlast(), 0, SpellListWizard, SpellListSorcerer, SpellListDruid);
        RegisterSpell(BuildBurstOfRadiance(), 0, SpellListCleric);
        RegisterSpell(BuildThunderStrike(), 0, SpellListWizard, SpellListSorcerer, SpellListDruid);
        RegisterSpell(BuildWinterBreath(), 0, SpellListWizardGreenmage, SpellListWizard, SpellListSorcerer,
            SpellListDruid);
        RegisterSpell(BuildEarthTremor(), 0, SpellListWizardGreenmage, SpellListDruid);
    }

    private static SpellDefinition BuildAcidClaw()
    {
        const string NAME = "AcidClaws";
        const string CONDITION = "ConditionAcidClaws";
        const string MODIFIER = "AttributeModifierAcidClawsACDebuff";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.AcidClaws, 128, 128);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 5, additionalDicePerIncrement: 1)
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetDamageForm(dieType: DieType.D8, diceNumber: 1, damageType: DamageTypeNecrotic)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .AddEffectForm(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create(CONDITION)
                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAcidSpit.GuiPresentation.SpriteReference)
                        .SetDuration(DurationType.Round, 1)
                        .SetSpecialDuration(true)
                        .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                            .Create(MODIFIER)
                            .SetGuiPresentation(Category.Feature)
                            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                                AttributeDefinitions.ArmorClass, -1)
                            .AddToDB())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .Build())
                .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(false)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildAirBlast()
    {
        const string NAME = "AirBlast";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.AirBlast, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(true, false, AttributeDefinitions.Strength, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals, 1, 2)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeBludgeoning, 0, DieType.D6, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build()
            ).Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildBurstOfRadiance()
    {
        const string NAME = "BurstOfRadiance";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.BurstOfRadiance, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Constitution, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 13)
            .SetDurationData(DurationType.Instantaneous)
            .SetParticleEffectParameters(BurningHands.EffectDescription.EffectParticleParameters)
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 1, 2)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeRadiant, 0, DieType.D6, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build()
            ).Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(false)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildThunderStrike()
    {
        const string NAME = "ThunderStrike";

        var spriteReference = Shield.GuiPresentation.SpriteReference;

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(
                EffectIncrementMethod.CasterLevelTable, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Constitution, false,
                EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15)
            .SetDurationData(DurationType.Instantaneous)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeThunder, 0, DieType.D6, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build()
            ).Build();

        effectDescription.SetTargetExcludeCaster(true);

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(0)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildEarthTremor()
    {
        const string NAME = "EarthTremor";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.EarthTremor, 128, 128);

        //var rubbleProxy = EffectProxyDefinitionBuilder
        //    .Create(EffectProxyDefinitions.ProxyGrease, "RubbleProxy", "")
        //    .SetGuiPresentation(Category.EffectProxy, spriteReference)
        //    .AddToDB();

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(
                EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Dexterity, false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 12)
            .SetDurationData(DurationType.Minute, 10)
            .SetParticleEffectParameters(Grease.EffectDescription.EffectParticleParameters)
            .SetTargetingData(Side.All, RangeType.Distance, 24, TargetType.Cylinder)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .CreatedByCharacter()
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(false, DieType.D1, DamageTypeBludgeoning, 0, DieType.D12, 3)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
            ).Build();

        effectDescription.EffectForms.AddRange(
            Grease.EffectDescription.EffectForms.Find(e =>
                e.formType == EffectFormType.Topology));

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .AddToDB();

        return spell;
    }

    private static SpellDefinition BuildWinterBreath()
    {
        const string NAME = "WinterBreath";

        var spriteReference =
            CustomIcons.CreateAssetReferenceSprite(NAME, Resources.WinterBreath, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(true, true, AttributeDefinitions.Dexterity, false,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Wisdom, 12)
            .SetDurationData(DurationType.Minute, 1)
            .SetParticleEffectParameters(ConeOfCold.EffectDescription.EffectParticleParameters)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3, 2)
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates).Build())
            .AddEffectForm(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(damageType: DamageTypeCold, dieType: DieType.D8, diceNumber: 4)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage).Build()
            ).Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetRequiresConcentration(false)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetMaterialComponent(MaterialComponentType.Mundane)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .AddToDB();

        return spell;
    }
}