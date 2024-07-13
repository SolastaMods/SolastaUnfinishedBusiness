using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionCastSpellBuilder;
using static FeatureDefinitionCastSpell;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpellPointsContext
{
    private static readonly List<int> SpellCostByLevel = [0, 2, 3, 5, 6, 7, 9, 10, 11, 13];
    private static readonly List<SlotsByLevelDuplet> FullCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> HalfCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> HalfRoundUpCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> OneThirdCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> SpellPointsFullCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> SpellPointsHalfCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> SpellPointsHalfRoundUpCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> SpellPointsOneThirdCastingSlots = [];

    private static readonly List<(string, List<SlotsByLevelDuplet>, List<SlotsByLevelDuplet>)>
        FeatureDefinitionCastSpellTab =
        [
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellBard.Name, FullCastingSlots,
                SpellPointsFullCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellCleric.Name, FullCastingSlots,
                SpellPointsFullCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellDruid.Name, FullCastingSlots,
                SpellPointsFullCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellPaladin.Name, HalfCastingSlots,
                SpellPointsHalfCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellRanger.Name, HalfCastingSlots,
                SpellPointsHalfCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellSorcerer.Name, FullCastingSlots,
                SpellPointsFullCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellWizard.Name, FullCastingSlots,
                SpellPointsFullCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.Name, OneThirdCastingSlots,
                SpellPointsOneThirdCastingSlots),
            (DatabaseHelper.FeatureDefinitionCastSpells.CastSpellShadowcaster.Name, OneThirdCastingSlots,
                SpellPointsOneThirdCastingSlots),
            (InventorClass.SpellCasting.Name, HalfRoundUpCastingSlots, SpellPointsHalfRoundUpCastingSlots),
            (RoguishArcaneScoundrel.CastSpellName, OneThirdCastingSlots, SpellPointsOneThirdCastingSlots),
            (MartialSpellShield.CastSpellName, OneThirdCastingSlots, SpellPointsOneThirdCastingSlots)
        ];

    internal static readonly FeatureDefinitionPower PowerSpellPoints = FeatureDefinitionPowerBuilder
        .Create("PowerSpellPoints")
        .SetGuiPresentationNoContent(true)
        .SetUsesFixed(RuleDefinitions.ActivationTime.NoCost, RuleDefinitions.RechargeRate.LongRest)
        .AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmountPowerSpellPoints())
        .AddToDB();

    internal static void LateLoad()
    {
        EnumerateSlotsPerLevel(CasterProgression.Full, FullCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.Half, HalfCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.HalfRoundUp, HalfRoundUpCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.OneThird, OneThirdCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.Full, SpellPointsFullCastingSlots, true);
        EnumerateSlotsPerLevel(CasterProgression.Half, SpellPointsHalfCastingSlots, true);
        EnumerateSlotsPerLevel(CasterProgression.HalfRoundUp, SpellPointsHalfRoundUpCastingSlots, true);
        EnumerateSlotsPerLevel(CasterProgression.OneThird, SpellPointsOneThirdCastingSlots, true);
        SwitchFeatureDefinitionCastSpellSlots();
    }

    internal static void ConsumeSpellPoints(RulesetCharacterHero hero, RulesetSpellRepertoire repertoire, int slotLevel)
    {
        var usablePower = PowerProvider.Get(PowerSpellPoints, hero);
        var cost = SpellCostByLevel[slotLevel];

        usablePower.remainingUses -= cost;

        if (slotLevel <= 5)
        {
            return;
        }

        var usedSpellsSlots = repertoire.usedSpellsSlots;

        usedSpellsSlots.TryAdd(slotLevel, 0);
        usedSpellsSlots[slotLevel] = 1;

        // no need to RepertoireRefreshed here as ConsumeSlots will end up doing it
    }

    internal static void ConsumeSlots(RulesetCharacterHero hero, RulesetSpellRepertoire repertoire)
    {
        var usablePower = PowerProvider.Get(PowerSpellPoints, hero);
        var level = repertoire.MaxSpellLevelOfSpellCastingLevel;

        for (var i = level; i > 0; i--)
        {
            if (usablePower.RemainingUses >= SpellCostByLevel[i])
            {
                continue;
            }

            var usedSpellsSlots = repertoire.usedSpellsSlots;

            usedSpellsSlots.TryAdd(i, 0);
            usedSpellsSlots[i] = 1;
        }

        repertoire.RepertoireRefreshed?.Invoke(repertoire);
    }

    internal static void RefreshSpellRepertoire(RulesetCharacterHero hero)
    {
        if (hero == null)
        {
            return;
        }

        var usablePower = PowerProvider.Get(PowerSpellPoints, hero);
        var activeConditions = hero.AllConditions.ToList();

        foreach (var activeCondition in activeConditions)
        {
            var removeCondition = false;

            foreach (var magicAffinity in activeCondition.ConditionDefinition.Features
                         .OfType<FeatureDefinitionMagicAffinity>()
                         .Where(x => x.AdditionalSlots.Count > 0))
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var additionalSlot in magicAffinity.AdditionalSlots)
                {
                    var slotCount = additionalSlot.SlotsNumber;
                    var slotLevel = additionalSlot.SlotLevel;
                    var totalPoints = slotCount * SpellCostByLevel[slotLevel];

                    usablePower.remainingUses += totalPoints;

                    removeCondition = true;
                }
            }

            if (removeCondition)
            {
                hero.RemoveCondition(activeCondition);
            }
        }
    }

    internal static void SwitchFeatureDefinitionCastSpellSlots()
    {
        var db = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>();

        foreach (var (name, slotsVanilla, slotsSpellPoints) in FeatureDefinitionCastSpellTab)
        {
            var featureCastSpell = db.GetElement(name);

            featureCastSpell.slotsPerLevels =
                Main.Settings.UseAlternateSpellPointsSystem ? slotsSpellPoints : slotsVanilla;
        }
    }

    private sealed class ModifyPowerPoolAmountPowerSpellPoints : IModifyPowerPoolAmount
    {
        private static readonly List<int> SpellPointsByLevel =
        [
            0, 4, 6, 14, 17,
            // 5
            27, 32, 38, 44, 57,
            // 10
            64, 73, 73, 83, 83,
            // 15
            94, 94, 107, 114, 123,
            // 20
            133
        ];

        public FeatureDefinitionPower PowerPool => PowerSpellPoints;

        public int PoolChangeAmount(RulesetCharacter character)
        {
            var hero = character.GetOriginalHero();
            var casterLevel = GetCasterLevel(hero);

            return SpellPointsByLevel[casterLevel];
        }

        private static int GetCasterLevel(RulesetCharacterHero hero)
        {
            if (SharedSpellsContext.IsMulticaster(hero))
            {
                return SharedSpellsContext.GetSharedCasterLevel(hero);
            }

            var spellRepertoire = hero.SpellRepertoires.FirstOrDefault(x =>
                x.SpellCastingFeature.SpellCastingOrigin
                    is CastingOrigin.Class
                    or CastingOrigin.Subclass);

            if (spellRepertoire == null)
            {
                return 0;
            }

            var characterLevel = hero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var casterType = SharedSpellsContext.GetCasterTypeForClassOrSubclass(
                spellRepertoire.SpellCastingClass?.Name, spellRepertoire.SpellCastingSubclass?.Name);

            return casterType switch
            {
                CasterProgression.Full => characterLevel,
                CasterProgression.Half when characterLevel <= 1 => 0,
                CasterProgression.Half => (characterLevel + 1) / 2,
                CasterProgression.HalfRoundUp => (characterLevel + 1) / 2,
                CasterProgression.OneThird => (characterLevel + 2) / 3,
                _ => 0
            };
        }
    }
}
