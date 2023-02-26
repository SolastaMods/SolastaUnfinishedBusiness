using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    #region LEVEL 03

    internal static SpellDefinition BuildBlindingSmite()
    {
        const string NAME = "BlindingSmite";

        var additionalDamageBlindingSmite = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NAME)
            .SetDamageDice(DieType.D8, 3)
            .SetSpecificDamageType(DamageTypeRadiant)
            .SetSavingThrowData( //explicitly stating all relevant properties (even default ones) for readability
                EffectDifficultyClassComputation.SpellCastingFeature,
                EffectSavingThrowType.None,
                // ReSharper disable once RedundantArgumentDefaultValue
                AttributeDefinitions.Constitution)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = true,
                    canSaveToCancel = true,
                    saveAffinity = EffectSavingThrowType.Negates,
                    saveOccurence = TurnOccurenceType.StartOfTurn,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create(ConditionDefinitions.ConditionBlinded, $"Condition{NAME}Enemy")
                        .SetSpecialDuration(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                        .AddToDB(),
                    operation = ConditionOperationDescription.ConditionOperation.Add
                })
            .AddToDB();

        var conditionBlindingSmite = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Spell, ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(additionalDamageBlindingSmite)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(BrandingSmite, NAME)
            .SetGuiPresentation(Category.Spell, Sprites.GetSprite(NAME, Resources.BlindingSmite, 128))
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .SetSpellLevel(3)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetVerboseComponent(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(conditionBlindingSmite, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildWinterBreath()
    {
        const string NAME = "WinterBreath";

        var spriteReference = Sprites.GetSprite(NAME, Resources.WinterBreath, 128, 128);

        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 1, 0, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Dexterity,
                false,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Wisdom,
                12)
            .SetDurationData(DurationType.Instantaneous)
            .SetParticleEffectParameters(ConeOfCold)
            .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne, 1)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeCold, 4, DieType.D8)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .Build())
            .Build();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetEffectDescription(effectDescription)
            .SetCastingTime(ActivationTime.Action)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Attack)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolConjuration)
            .AddToDB();

        return spell;
    }

    internal static SpellDefinition BuildCrusadersMantle()
    {
        const string NAME = "CrusadersMantle";

        var conditionCrusadersMantle = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Spell,
                Sprites.GetSprite("CrusadersMantle", Resources.CrusadersMantle, 128))
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetParticleEffectParameters(DivineFavor)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Sphere, 6)
                .SetDurationData(DurationType.Minute, 1)
                .SetRecurrentEffect(RecurrentEffect.OnActivation |
                                    RecurrentEffect.OnTurnStart |
                                    RecurrentEffect.OnEnter)
                .AddEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionCrusadersMantle, ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .SetCastingTime(ActivationTime.Action)
            .SetRequiresConcentration(true)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Buff)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEvocation)
            .AddToDB();

        return spell;
    }

    #region Spirit Shroud

    internal static SpellDefinition BuildSpiritShroud()
    {
        var hinder = ConditionDefinitionBuilder
            .Create(ConditionHindered_By_Frost, "ConditionSpiritShroudHinder")
            .SetSilent(Silent.None)
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionSpiritGuardians)
            .AddToDB();

        var noHeal = ConditionDefinitionBuilder
            .Create("ConditionSpiritShroudNoHeal")
            .SetGuiPresentation(Category.Condition, ConditionChilledByTouch.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(FeatureDefinitionHealingModifiers.HealingModifierChilledByTouch)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var sprite = Sprites.GetSprite("SpiritShroud", Resources.SpiritShroud, 128);

        return SpellDefinitionBuilder
            .Create("SpiritShroud")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetRequiresConcentration(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
                .Build())
            .SetSubSpells(
                BuildSpiritShroudSubSpell(DamageTypeRadiant, hinder, noHeal, sprite),
                BuildSpiritShroudSubSpell(DamageTypeNecrotic, hinder, noHeal, sprite),
                BuildSpiritShroudSubSpell(DamageTypeCold, hinder, noHeal, sprite))
            .AddToDB();
    }

    private static SpellDefinition BuildSpiritShroudSubSpell(
        string damage,
        ConditionDefinition hinder,
        ConditionDefinition noHeal,
        AssetReferenceSprite sprite)
    {
        return SpellDefinitionBuilder
            .Create($"SpiritShroud{damage}")
            .SetGuiPresentation(Category.Spell, sprite)
            .SetSpellLevel(3)
            .SetVocalSpellSameType(VocalSpellSemeType.Defense)
            .SetCastingTime(ActivationTime.BonusAction)
            .SetRequiresConcentration(true)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.Cube, 5)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, 2, additionalDicePerIncrement: 1)
                //RAW it should only trigger if target starts turn in the area, but game doesn't trigger on turn start for some reason without other flags
                .SetRecurrentEffect(RecurrentEffect.OnActivation
                                    | RecurrentEffect.OnTurnStart
                                    | RecurrentEffect.OnEnter)
                .SetParticleEffectParameters(SpiritGuardians)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(hinder, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create($"ConditionSpiritShroud{damage}")
                            // .SetGuiPresentation(Category.Condition, ConditionSpiritGuardiansSelf)
                            .SetGuiPresentationNoContent()
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .CopyParticleReferences(ConditionSpiritGuardiansSelf)
                            .SetFeatures(FeatureDefinitionAdditionalDamageBuilder
                                .Create($"AdditionalDamageSpiritShroud{damage}")
                                .SetGuiPresentationNoContent(true)
                                .SetNotificationTag($"SpiritShroud{damage}")
                                .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetWithin10Ft)
                                .SetAttackOnly()
                                .SetConditionOperations(new ConditionOperationDescription
                                {
                                    operation = ConditionOperationDescription.ConditionOperation.Add,
                                    conditionDefinition = noHeal,
                                    hasSavingThrow = false
                                })
                                .SetDamageDice(DieType.D8, 1)
                                .SetSpecificDamageType(damage)
                                .SetAdvancement(AdditionalDamageAdvancement.SlotLevel, 0, 1, 2)
                                .AddToDB())
                            .AddToDB(), ConditionForm.ConditionOperation.Add, true, true)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetTopologyForm(TopologyForm.Type.DangerousZone, true)
                        .Build())
                .Build())
            .AddToDB();
    }

    #endregion

    #endregion
}
