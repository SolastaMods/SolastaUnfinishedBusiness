using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    internal static void SwitchOneDndPaladinLearnSpellCastingAtOne()
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

    internal static void SwitchOneDndPaladinLayOnHandAsBonusAction()
    {
        PowerPaladinLayOnHands.activationTime = Main.Settings.EnablePaladinLayOnHandsAsBonusAction2024
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }
}
