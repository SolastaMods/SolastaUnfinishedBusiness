using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes.Inventor;
using static SolastaUnfinishedBusiness.Spells.SpellBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpellsContext
{
    internal static readonly Dictionary<SpellListDefinition, SpellListContext> SpellListContextTab = new();

    internal static readonly SpellListDefinition EmptySpellList = SpellListDefinitionBuilder
        .Create("SpellListEmpty")
        .SetGuiPresentationNoContent(true)
        .ClearSpells()
        .FinalizeSpells(false)
        .AddToDB();

    // ReSharper disable once InconsistentNaming
    private static readonly SortedList<string, SpellListDefinition> spellLists = new();
    private static readonly Dictionary<SpellDefinition, List<SpellListDefinition>> SpellSpellListMap = new();
    internal static readonly SpellDefinition FarStep = BuildFarStep();
    internal static readonly SpellDefinition SearingSmite = BuildSearingSmite();
    internal static readonly SpellDefinition SunlightBlade = BuildSunlightBlade();
    internal static HashSet<SpellDefinition> Spells { get; private set; } = new();

    [NotNull]
    internal static SortedList<string, SpellListDefinition> SpellLists
    {
        get
        {
            if (spellLists.Count != 0)
            {
                return spellLists;
            }

            // only this sub matters for spell selection. this might change if we add additional subs to mod
            var characterSubclass = DatabaseHelper.CharacterSubclassDefinitions.TraditionLight;

            var title = characterSubclass.FormatTitle();

            var featureDefinitions = characterSubclass.FeatureUnlocks
                .Select(x => x.FeatureDefinition)
                .Where(x => x is FeatureDefinitionCastSpell or FeatureDefinitionMagicAffinity);

            foreach (var featureDefinition in featureDefinitions)
            {
                switch (featureDefinition)
                {
                    case FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity
                        when featureDefinitionMagicAffinity.ExtendedSpellList != null &&
                             !spellLists.ContainsValue(featureDefinitionMagicAffinity.ExtendedSpellList):
                        spellLists.Add(title, featureDefinitionMagicAffinity.ExtendedSpellList);

                        foreach (var spell in featureDefinitionMagicAffinity.ExtendedSpellList.SpellsByLevel.SelectMany(
                                     x => x.Spells))
                        {
                            if (!SpellSpellListMap.ContainsKey(spell))
                            {
                                SpellSpellListMap.Add(spell, new List<SpellListDefinition>());
                            }

                            SpellSpellListMap[spell].Add(featureDefinitionMagicAffinity.ExtendedSpellList);
                        }

                        break;
                    case FeatureDefinitionCastSpell featureDefinitionCastSpell
                        when featureDefinitionCastSpell.SpellListDefinition != null &&
                             !spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition):
                        spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);

                        foreach (var spell in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(
                                     x => x.Spells))
                        {
                            if (!SpellSpellListMap.ContainsKey(spell))
                            {
                                SpellSpellListMap.Add(spell, new List<SpellListDefinition>());
                            }

                            SpellSpellListMap[spell].Add(featureDefinitionCastSpell.SpellListDefinition);
                        }

                        break;
                }
            }

            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            foreach (var characterClass in dbCharacterClassDefinition)
            {
                title = characterClass.FormatTitle();

                var featureDefinitionCastSpell = characterClass.FeatureUnlocks
                    .Select(x => x.FeatureDefinition)
                    .OfType<FeatureDefinitionCastSpell>()
                    .FirstOrDefault();

                // this is an exception to comport Warlock Variant and force the original game one
                if (characterClass == DatabaseHelper.CharacterClassDefinitions.Warlock)
                {
                    featureDefinitionCastSpell = DatabaseHelper.FeatureDefinitionCastSpells.CastSpellWarlock;
                }

                // NOTE: don't use featureDefinitionCastSpell?. which bypasses Unity object lifetime check
                if (!featureDefinitionCastSpell
                    || !featureDefinitionCastSpell.SpellListDefinition
                    || spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition))
                {
                    continue;
                }

                {
                    spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);

                    foreach (var spell in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(
                                 x => x.Spells))
                    {
                        if (!SpellSpellListMap.ContainsKey(spell))
                        {
                            SpellSpellListMap.Add(spell, new List<SpellListDefinition>());
                        }

                        SpellSpellListMap[spell].Add(featureDefinitionCastSpell.SpellListDefinition);
                    }
                }
            }

            return spellLists;
        }
    }

    internal static bool IsAllSetSelected()
    {
        return SpellListContextTab.Values.All(spellListContext => spellListContext.IsAllSetSelected);
    }

    internal static bool IsSuggestedSetSelected()
    {
        return SpellListContextTab.Values.All(spellListContext => spellListContext.IsSuggestedSetSelected);
    }

    internal static void SelectAllSet(bool toggle)
    {
        foreach (var spellListContext in SpellListContextTab.Values)
        {
            spellListContext.SelectAllSetInternal(toggle);
        }
    }

    internal static void SelectSuggestedSet(bool toggle)
    {
        foreach (var spellListContext in SpellListContextTab.Values)
        {
            spellListContext.SelectSuggestedSetInternal(toggle);
        }
    }

    internal static void LateLoad()
    {
        // init collections
        foreach (var spellList in SpellLists.Values)
        {
            var name = spellList.Name;

            SpellListContextTab.Add(spellList, new SpellListContext(spellList));

            Main.Settings.SpellListSpellEnabled.TryAdd(name, new List<string>());
            Main.Settings.DisplaySpellListsToggle.TryAdd(name, true);
            Main.Settings.SpellListSliderPosition.TryAdd(name, 4);
        }

        var spellListInventorClass = InventorClass.SpellList;

        // MUST COME BEFORE ANY MOD REGISTERED SPELL
        AllowAssigningOfficialSpells();
        
        // Dead Master Spells
        Subclasses.WizardDeadMaster.DeadMasterSpells.Do(x => RegisterSpell(x));

        // cantrips
        RegisterSpell(BuildAcidClaw(), 0, SpellListDruid);
        RegisterSpell(BuildAirBlast(), 0, SpellListBard, SpellListCleric, SpellListDruid, SpellListSorcerer,
            SpellListWizard);
        RegisterSpell(BuildBladeWard(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildBurstOfRadiance(), 0, SpellListCleric);
        RegisterSpell(BuildEnduringSting(), 0, SpellListWizard);
        RegisterSpell(BuildIlluminatingSphere(), 0, SpellListBard, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildMinorLifesteal(), 0, SpellListBard, SpellListSorcerer, SpellListWizard, SpellListWarlock);
        RegisterSpell(BuildResonatingStrike(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(SunlightBlade, 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildThornyVines(), 0, SpellListDruid, spellListInventorClass);
        RegisterSpell(BuildThunderStrike(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard,
            spellListInventorClass);

        // 1st level
        RegisterSpell(BuildChromaticOrb(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildMule(), 0, SpellListWizard);
        RegisterSpell(BuildRadiantMotes(), 0, SpellListWizard, spellListInventorClass);
        RegisterSpell(SearingSmite, 0, SpellListPaladin, SpellListRanger);
        RegisterSpell(BuildThunderousSmite(), 0, SpellListPaladin);
        RegisterSpell(BuildWrathfulSmite(), 0, SpellListPaladin);

        // 2nd level
        RegisterSpell(BuildPetalStorm(), 0, SpellListDruid);
        RegisterSpell(BuildProtectThreshold(), 0, SpellListCleric, SpellListDruid, SpellListPaladin);
        RegisterSpell(BuildMirrorImage(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildShadowBlade(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);

        // 3rd level
        RegisterSpell(BuildBlindingSmite(), 0, SpellListPaladin);
        RegisterSpell(BuildEarthTremor(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildWinterBreath(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildSpiritShroud(), 0, SpellListCleric, SpellListPaladin, SpellListWarlock, SpellListWizard);

        // 3rd level
        RegisterSpell(BuildStaggeringSmite(), 0, SpellListPaladin);
        
        //5th level
        RegisterSpell(FarStep, 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);

        // 7th level
        RegisterSpell(BuildReverseGravity(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 8th level
        RegisterSpell(BuildMindBlank(), 0, SpellListBard, SpellListWizard);

        // 9th level
        RegisterSpell(BuildForesight(), 0, SpellListBard, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildMassHeal(), 0, SpellListCleric);
        RegisterSpell(BuildMeteorSwarmSingleTarget(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildPowerWordHeal(), 0, SpellListBard, SpellListCleric);
        RegisterSpell(BuildPowerWordKill(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildTimeStop(), 0, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildShapechange(), 0, SpellListDruid, SpellListWizard);
        RegisterSpell(BuildWeird(), 0, SpellListWizard);

        Spells = Spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.FormatTitle()).ToHashSet();

        foreach (var kvp in SpellListContextTab)
        {
            // caches which spells are toggleable per spell list
            var spellListContext = kvp.Value;

            spellListContext.CalculateAllSpells();

            // settings paring
            var spellListName = kvp.Key.Name;

            foreach (var name in Main.Settings.SpellListSpellEnabled[spellListName]
                         .Where(name => Spells.All(x => x.Name != name))
                         .ToList())
            {
                Main.Settings.SpellListSpellEnabled[spellListName].Remove(name);
            }
        }
    }

    private static void AllowAssigningOfficialSpells()
    {
        if (!Main.Settings.AllowAssigningOfficialSpells)
        {
            return;
        }

        foreach (var kvp in SpellSpellListMap)
        {
            RegisterSpell(kvp.Key, kvp.Value.Count, kvp.Value.ToArray());
        }
    }

    internal static void RegisterSpell(
        SpellDefinition spellDefinition,
        int suggestedStartsAt = 0,
        params SpellListDefinition[] registeredSpellLists)
    {
        if (Spells.Contains(spellDefinition))
        {
            return;
        }

        Spells.Add(spellDefinition);

        //Add spells to `All Spells` list, so that Warlock's `Book of Ancient Secrets` and Bard's `Magic Secrets` would see them
        if (spellDefinition.contentPack == CeContentPackContext.CeContentPack)
        {
            SpellListAllSpells.AddSpell(spellDefinition);

            //Add cantrips to `All Cantrips` list, so that Warlock's `Pact of the Tome` and Loremaster's `Arcane Professor` would see them
            if (spellDefinition.SpellLevel == 0)
            {
                SpellListAllCantrips.AddSpell(spellDefinition);
            }
        }

        for (var i = 0; i < registeredSpellLists.Length; i++)
        {
            var spellList = registeredSpellLists[i];

            if (i < suggestedStartsAt)
            {
                SpellListContextTab[spellList].MinimumSpells.Add(spellDefinition);
            }
            else
            {
                SpellListContextTab[spellList].SuggestedSpells.Add(spellDefinition);
            }
        }

        foreach (var spellList in SpellLists.Values)
        {
            if (!Main.Settings.SpellListSpellEnabled.ContainsKey(spellList.Name))
            {
                continue;
            }

            if (SpellListContextTab[spellList].MinimumSpells.Contains(spellDefinition))
            {
                continue;
            }

            var enable = Main.Settings.SpellListSpellEnabled[spellList.Name].Contains(spellDefinition.Name);

            SpellListContextTab[spellList].Switch(spellDefinition, enable);
        }
    }

    internal sealed class SpellListContext
    {
        internal SpellListContext(SpellListDefinition spellListDefinition)
        {
            SpellList = spellListDefinition;
            AllSpells = new HashSet<SpellDefinition>();
            MinimumSpells = new HashSet<SpellDefinition>();
            SuggestedSpells = new HashSet<SpellDefinition>();
        }

        private List<string> SelectedSpells => Main.Settings.SpellListSpellEnabled[SpellList.Name];
        private SpellListDefinition SpellList { get; }
        internal HashSet<SpellDefinition> AllSpells { get; }
        internal HashSet<SpellDefinition> MinimumSpells { get; }
        internal HashSet<SpellDefinition> SuggestedSpells { get; }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsAllSetSelected => SelectedSpells.Count == AllSpells.Count;

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsSuggestedSetSelected => SelectedSpells.Count == SuggestedSpells.Count
                                                && SuggestedSpells.All(x => SelectedSpells.Contains(x.Name));

        internal void CalculateAllSpells()
        {
            var minSpellLevel = SpellList.HasCantrips ? 0 : 1;
            var maxSpellLevel = SpellList.MaxSpellLevel;

            AllSpells.Clear();

            foreach (var spell in Spells
                         .Where(x => x.SpellLevel >= minSpellLevel && x.SpellLevel <= maxSpellLevel &&
                                     !MinimumSpells.Contains(x)))
            {
                AllSpells.Add(spell);
            }
        }

        internal void SelectAllSetInternal(bool toggle)
        {
            foreach (var spell in AllSpells)
            {
                Switch(spell, toggle);
            }
        }

        internal void SelectSuggestedSetInternal(bool toggle)
        {
            if (toggle)
            {
                SelectAllSetInternal(false);
            }

            foreach (var spell in SuggestedSpells)
            {
                Switch(spell, toggle);
            }
        }

        internal void Switch([NotNull] SpellDefinition spellDefinition, bool active)
        {
            var spellListName = SpellList.Name;
            var spellName = spellDefinition.Name;
            var spells = SpellList.SpellsByLevel.Find(x => x.Level == spellDefinition.SpellLevel)?.Spells;

            // this happens on spell lists without cantrips
            if (spells == null)
            {
                return;
            }

            if (active)
            {
                spells.TryAdd(spellDefinition);
                Main.Settings.SpellListSpellEnabled[spellListName].TryAdd(spellName);
            }
            else
            {
                spells.Remove(spellDefinition);
                Main.Settings.SpellListSpellEnabled[spellListName].Remove(spellName);
            }
        }
    }
}
