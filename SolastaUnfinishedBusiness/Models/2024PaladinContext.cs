using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    internal static readonly FeatureDefinitionAttributeModifier AttributeModifierPaladinChannelDivinity11 =
        FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierPaladinChannelDivinity11")
            .SetGuiPresentationNoContent(true)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ChannelDivinityNumber)
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
}
