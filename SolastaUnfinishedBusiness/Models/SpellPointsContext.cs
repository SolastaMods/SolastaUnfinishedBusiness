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
using TMPro;
using UnityEngine;
using static FeatureDefinitionCastSpell;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionCastSpellBuilder;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpellPointsContext
{
    private static readonly List<int> SpellCostByLevel = [0, 2, 3, 5, 6, 7, 9, 10, 11, 13];
    private static readonly List<SlotsByLevelDuplet> FullCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> HalfCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> HalfRoundUpCastingSlots = [];
    private static readonly List<SlotsByLevelDuplet> OneThirdCastingSlots = [];
    internal static readonly List<SlotsByLevelDuplet> SpellPointsFullCastingSlots = [];
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

    private static readonly FeatureDefinitionPower PowerSpellPoints = FeatureDefinitionPowerBuilder
        .Create("PowerSpellPoints")
        .SetGuiPresentationNoContent(true)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
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

    internal static void HideSpellSlots(RulesetCharacterHero hero, RectTransform table)
    {
        if (!Main.Settings.UseAlternateSpellPointsSystem ||
            SharedSpellsContext.GetWarlockSpellRepertoire(hero) != null)
        {
            return;
        }

        for (var index = 0; index < table.childCount; ++index)
        {
            var component = table.GetChild(index).GetComponent<SlotStatus>();

            component.Used.gameObject.SetActive(false);
            component.Available.gameObject.SetActive(false);
        }
    }

    internal static void SetupUseSlots(
        GuiCharacterAction guiCharacterAction,
        RectTransform useSlotsTable,
        GuiLabel highSlotNumber)
    {
        if (!Main.Settings.UseAlternateSpellPointsSystem ||
            (guiCharacterAction.ActionDefinition != DatabaseHelper.ActionDefinitions.CastMain &&
             guiCharacterAction.ActionDefinition != DatabaseHelper.ActionDefinitions.CastBonus))
        {
            return;
        }

        var rulesetCharacter = guiCharacterAction.ActingCharacter.RulesetCharacter;
        var remainingSpellPoints = GetRemainingSpellPoints(rulesetCharacter).ToString();

        highSlotNumber.gameObject.SetActive(true);
        useSlotsTable.gameObject.SetActive(false);
        highSlotNumber.Text = remainingSpellPoints;
        highSlotNumber.GuiTooltip.Content =
            Gui.Format("Screen/&SpellAlternatePointsTooltip", remainingSpellPoints);
    }

    private static int GetMaxSpellPoints(RulesetCharacter rulesetCharacter)
    {
        var usablePower = PowerProvider.Get(PowerSpellPoints, rulesetCharacter);
        var maxUsesOfPower = rulesetCharacter.GetMaxUsesOfPower(usablePower);

        return maxUsesOfPower;
    }

    private static int GetRemainingSpellPoints(RulesetCharacter rulesetCharacter)
    {
        var usablePower = PowerProvider.Get(PowerSpellPoints, rulesetCharacter);
        var remainingUsesOfPower = rulesetCharacter.GetRemainingUsesOfPower(usablePower);

        return remainingUsesOfPower;
    }

    internal static void SwitchRepertoireTitleOnInspectionScreen(
        CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
    {
        for (var i = 0; i < __instance.spellPanelsContainer.childCount; i++)
        {
            var child = __instance.spellPanelsContainer.GetChild(i);
            var repertoireTitle = child.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

            if (Main.Settings.UseAlternateSpellPointsSystem &&
                (SharedSpellsContext.IsMulticaster(heroCharacter) ||
                 SharedSpellsContext.GetWarlockSpellRepertoire(heroCharacter) == null))

            {
                var maxSpellPoints = GetMaxSpellPoints(heroCharacter).ToString();
                var postfix = Gui.Format("Screen/&SpellAlternatePointsCostTooltip", maxSpellPoints);

                repertoireTitle.text = Gui.Localize("Screen/&RepertoireSpellsTitle") + ": " + postfix;
            }
            else
            {
                repertoireTitle.text = Gui.Localize("Screen/&RepertoireSpellsTitle");
            }
        }
    }

    internal static void AddCostTextToSpellLevels(SlotStatusTable slotStatusTable, SlotStatus slotStatus, int slotLevel,
        int spellsAtLevel)
    {
        var cost = SpellCostByLevel[slotLevel].ToString();

        slotStatus.Used.gameObject.SetActive(false);
        slotStatus.Available.gameObject.SetActive(false);
        slotStatusTable.slotsText.gameObject.SetActive(true);
        slotStatusTable.slotsText.Text =
            spellsAtLevel < 2 ? cost : Gui.Format("Screen/&SpellAlternatePointsCostTooltip", cost);
    }

    internal static void GrantPowerSpellPoints(RulesetCharacterHero hero)
    {
        if (hero.HasAnyFeature(PowerSpellPoints))
        {
            return;
        }

        hero.ActiveFeatures[AttributeDefinitions.TagRace].Add(PowerSpellPoints);

        var usablePower = PowerProvider.Get(PowerSpellPoints, hero);
        var poolSize = hero.GetMaxUsesOfPower(usablePower);

        usablePower.remainingUses = poolSize;
        hero.UsablePowers.Add(usablePower);
    }

    internal static void ConsumeSlotsAtLevelsPointsCannotCastAnymore(
        RulesetCharacterHero hero, RulesetSpellRepertoire repertoire, int slotLevel)
    {
        // consume points
        var usablePower = PowerProvider.Get(PowerSpellPoints, hero);
        var cost = SpellCostByLevel[slotLevel];

        usablePower.remainingUses -= cost;

        // handle scenario where spells at level 6 and above can only be cast once per level
        if (slotLevel > 5)
        {
            var usedSpellsSlots = repertoire.usedSpellsSlots;

            usedSpellsSlots.TryAdd(slotLevel, 0);
            usedSpellsSlots[slotLevel] = 1;
        }

        // consume spell slots at levels points cannot cast anymore
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

    internal static void ConvertAdditionalSlotsIntoSpellPointsBeforeRefreshSpellRepertoire(RulesetCharacterHero hero)
    {
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

    internal static void RefreshActionPanelAfterFlexibleCastingItem()
    {
        if (!Main.Settings.UseAlternateSpellPointsSystem)
        {
            return;
        }

        var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();

        if (gameLocationScreenExploration.Visible)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var panel in gameLocationScreenExploration.CharacterControlPanel.ActionPanels)
            {
                foreach (var characterActionItem in
                         panel.actionItems.Where(x =>
                             x.CurrentItemForm.GuiCharacterAction.ActionDefinition ==
                             DatabaseHelper.ActionDefinitions.CastMain ||
                             x.CurrentItemForm.GuiCharacterAction.ActionDefinition ==
                             DatabaseHelper.ActionDefinitions.CastBonus))
                {
                    characterActionItem.CurrentItemForm.Refresh();
                }
            }

            return;
        }

        var gameLocationScreenBattle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();

        // ReSharper disable once InvertIf
        if (gameLocationScreenBattle.Visible)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var panel in gameLocationScreenBattle.CharacterControlPanel.ActionPanels)
            {
                foreach (var characterActionItem in
                         panel.actionItems.Where(x =>
                             x.CurrentItemForm.GuiCharacterAction.ActionDefinition ==
                             DatabaseHelper.ActionDefinitions.CastMain ||
                             x.CurrentItemForm.GuiCharacterAction.ActionDefinition ==
                             DatabaseHelper.ActionDefinitions.CastBonus))
                {
                    characterActionItem.CurrentItemForm.Refresh();
                }
            }
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
