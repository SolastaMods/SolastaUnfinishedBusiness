using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionFeatureSet FeatureSetRangerDeftExplorer =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetRangerDeftExplorer")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolRangerDeftExplorerLanguages")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Language, 2)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolRangerDeftExplorerSkills")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                    .AddCustomSubFeatures(new TryAlterOutcomeAttributeCheckPrimalKnowledge())
                    .AddToDB())
            .AddToDB();

    private static readonly FeatureDefinitionMovementAffinity MovementAffinityRangerRoving =
        FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityRangerRoving")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedAdditiveModifier(2)
            .SetClimbing(true)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolRangerExpertise = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolRangerExpertise")
        .SetGuiPresentation(Category.Feature)
        .SetPool(HeroDefinitions.PointsPoolType.Expertise, 2)
        .AddToDB();

    private static readonly FeatureDefinitionCombatAffinity CombatAffinityRangerPreciseHunter =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityRangerPreciseHunter")
            .SetGuiPresentation(Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.Advantage)
            .SetSituationalContext(SituationalContext.TargetHasCondition, ConditionDefinitions.ConditionMarkedByHunter)
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerRangerTireless = FeatureDefinitionPowerBuilder
        .Create("PowerRangerTireless")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    internal static void SwitchOneDndRangerLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellRanger))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters2024
        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnableRangerSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchRangerDeftExplorer()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureSetRangerDeftExplorer);

        if (Main.Settings.EnableRangerDeftExplorer2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetRangerDeftExplorer, 2));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerExpertise()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PointPoolRangerExpertise);

        if (Main.Settings.EnableRangerExpertise2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolRangerExpertise, 9));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerFeralSenses()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == Level20Context.SenseRangerFeralSenses ||
                x.FeatureDefinition == Level20Context.SenseRangerFeralSenses2024);

        Ranger.FeatureUnlocks.Add(Main.Settings.EnableRangerFeralSenses2024
            ? new FeatureUnlockByLevel(Level20Context.SenseRangerFeralSenses, 18)
            : new FeatureUnlockByLevel(Level20Context.SenseRangerFeralSenses2024, 18));

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerFoeSlayers()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == Level20Context.FeatureFoeSlayer ||
                x.FeatureDefinition == Level20Context.FeatureFoeSlayer2024);

        Ranger.FeatureUnlocks.Add(Main.Settings.EnableRangerFoeSlayers2024
            ? new FeatureUnlockByLevel(Level20Context.FeatureFoeSlayer, 20)
            : new FeatureUnlockByLevel(Level20Context.FeatureFoeSlayer2024, 20));

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerNatureShroud()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureDefinitionPowerNatureShroud);

        if (Main.Settings.EnableRangerNatureShroud2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureDefinitionPowerNatureShroud, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerPreciseHunter()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == CombatAffinityRangerPreciseHunter);

        if (Main.Settings.EnableRangerPreciseHunter2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(CombatAffinityRangerPreciseHunter, 17));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerRoving()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == MovementAffinityRangerRoving);

        if (Main.Settings.EnableRangerRoving2024)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(MovementAffinityRangerRoving, 6));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerTireless()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == PowerRangerHideInPlainSight ||
                x.FeatureDefinition == PowerRangerTireless);

        Ranger.FeatureUnlocks.Add(Main.Settings.EnableRangerTireless
            ? new FeatureUnlockByLevel(PowerRangerTireless, 10)
            : new FeatureUnlockByLevel(PowerRangerHideInPlainSight, 10));

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }
}
