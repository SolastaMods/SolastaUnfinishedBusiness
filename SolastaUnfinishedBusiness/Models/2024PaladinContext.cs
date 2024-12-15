using System;
using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionAttributeModifier AttributeModifierPaladinChannelDivinity11 =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierPaladinChannelDivinity11")
            .SetGuiPresentationNoContent(true)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ChannelDivinityNumber)
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerPaladinAbjureFoes = FeatureDefinitionPowerBuilder
        .Create("PowerPaladinAbjureFoes")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite(FeatSteadyAim, Resources.PowerPaladinAbjureFoes, 256, 128))
        .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create(ConditionDefinitions.ConditionFrightened, "ConditionFrightenedByAbjureFoes")
                                .SetParentCondition(ConditionDefinitions.ConditionFrightened)
                                .SetSpecialInterruptions(ConditionInterruption.Damaged)
                                .SetFeatures()
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
        .AddCustomSubFeatures(new ModifyEffectDescriptionPowerPaladinAbjureFoes())
        .AddToDB();

    internal static void SwitchPaladinSpellCastingAtOne()
    {
        var level = Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellPaladin))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters2024
        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnablePaladinSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchPaladinLayOnHand()
    {
        PowerPaladinLayOnHands.activationTime = Main.Settings.EnablePaladinLayOnHands2024
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    internal static void SwitchPaladinAbjureFoes()
    {
        Paladin.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PowerPaladinAbjureFoes);

        if (Main.Settings.EnablePaladinAbjureFoes2024)
        {
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerPaladinAbjureFoes, 9));
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchPaladinChannelDivinity()
    {
        if (Main.Settings.EnablePaladinChannelDivinity2024)
        {
            AttributeModifierPaladinChannelDivinity.modifierValue = 2;
            AttributeModifierPaladinChannelDivinity.GuiPresentation.description =
                "Feature/&PaladinChannelDivinityDescription";
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(AttributeModifierPaladinChannelDivinity11, 11));
        }
        else
        {
            AttributeModifierPaladinChannelDivinity.modifierValue = 1;
            AttributeModifierPaladinChannelDivinity.GuiPresentation.description =
                "Feature/&ClericChannelDivinityDescription";
            Paladin.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == AttributeModifierPaladinChannelDivinity11);
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private sealed class ModifyEffectDescriptionPowerPaladinAbjureFoes : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == PowerPaladinAbjureFoes;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var charisma = character.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);
            var targets = Math.Max(1, charismaModifier);

            effectDescription.targetParameter = targets;

            return effectDescription;
        }
    }
}
