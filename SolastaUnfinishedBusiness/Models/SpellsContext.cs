using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Displays;
using static SolastaUnfinishedBusiness.Spells.SpellBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class SpellsContext
{
    internal static readonly Dictionary<SpellDefinition, SpellDefinition> SpellsChildMaster = [];
    internal static readonly Dictionary<SpellListDefinition, SpellListContext> SpellListContextTab = [];

    internal static readonly SpellListDefinition EmptySpellList = SpellListDefinitionBuilder
        .Create("SpellListEmpty")
        .SetGuiPresentationNoContent(true)
        .ClearSpells()
        .FinalizeSpells(false)
        .AddToDB();

    // ReSharper disable once InconsistentNaming
    private static readonly SortedList<string, SpellListDefinition> spellLists = [];
    private static readonly Dictionary<SpellDefinition, List<SpellListDefinition>> SpellSpellListMap = [];

    internal static readonly SpellDefinition AirBlast = BuildAirBlast();
    internal static readonly SpellDefinition AuraOfLife = BuildAuraOfLife();
    internal static readonly SpellDefinition BanishingSmite = BuildBanishingSmite();
    internal static readonly SpellDefinition BindingIce = BuildBindingIce();
    internal static readonly SpellDefinition BlessingOfRime = BuildBlessingOfRime();
    internal static readonly SpellDefinition BlindingSmite = BuildBlindingSmite();
    internal static readonly SpellDefinition BurstOfRadiance = BuildBurstOfRadiance();
    internal static readonly SpellDefinition CorruptingBolt = BuildCorruptingBolt();
    internal static readonly SpellDefinition CausticZap = BuildCausticZap();
    internal static readonly SpellDefinition ColorBurst = BuildColorBurst();
    internal static readonly SpellDefinition DivineWrath = BuildDivineWrath();
    internal static readonly SpellDefinition ElementalWeapon = BuildElementalWeapon();
    internal static readonly SpellDefinition EarthTremor = BuildEarthTremor();
    internal static readonly SpellDefinition EnduringSting = BuildEnduringSting();
    internal static readonly SpellDefinition EnsnaringStrike = BuildEnsnaringStrike();
    internal static readonly SpellDefinition FarStep = BuildFarStep();
    internal static readonly SpellDefinition MaddeningDarkness = BuildMaddeningDarkness();
    internal static readonly SpellDefinition MantleOfThorns = BuildMantleOfThorns();
    internal static readonly SpellDefinition MirrorImage = BuildMirrorImage();
    internal static readonly SpellDefinition PetalStorm = BuildPetalStorm();
    internal static readonly SpellDefinition PsychicWhip = BuildPsychicWhip();
    internal static readonly SpellDefinition PulseWave = BuildPulseWave();
    internal static readonly SpellDefinition SearingSmite = BuildSearingSmite();
    internal static readonly SpellDefinition SonicBoom = BuildSonicBoom();
    internal static readonly SpellDefinition StaggeringSmite = BuildStaggeringSmite();
    internal static readonly SpellDefinition SteelWhirlwind = BuildSteelWhirlwind();
    internal static readonly SpellDefinition SunlightBlade = BuildSunlightBlade();
    internal static readonly SpellDefinition Telekinesis = BuildTelekinesis();
    internal static readonly SpellDefinition ThunderousSmite = BuildThunderousSmite();
    internal static readonly SpellDefinition Web = BuildWeb();
    internal static readonly SpellDefinition Wrack = BuildWrack();
    internal static readonly SpellDefinition WrathfulSmite = BuildWrathfulSmite();
    internal static HashSet<SpellDefinition> Spells { get; private set; } = [];

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
                        when featureDefinitionMagicAffinity.ExtendedSpellList &&
                             !spellLists.ContainsValue(featureDefinitionMagicAffinity.ExtendedSpellList):
                        spellLists.Add(title, featureDefinitionMagicAffinity.ExtendedSpellList);

                        foreach (var spell in featureDefinitionMagicAffinity.ExtendedSpellList.SpellsByLevel.SelectMany(
                                     x => x.Spells))
                        {
                            if (!SpellSpellListMap.TryGetValue(spell, out var value))
                            {
                                value = [];
                                SpellSpellListMap.Add(spell, value);
                            }

                            value.Add(featureDefinitionMagicAffinity.ExtendedSpellList);
                        }

                        break;
                    case FeatureDefinitionCastSpell featureDefinitionCastSpell
                        when featureDefinitionCastSpell.SpellListDefinition &&
                             !spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition):
                        spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);

                        foreach (var spell in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(
                                     x => x.Spells))
                        {
                            if (!SpellSpellListMap.TryGetValue(spell, out var value))
                            {
                                value = [];
                                SpellSpellListMap.Add(spell, value);
                            }

                            value.Add(featureDefinitionCastSpell.SpellListDefinition);
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

                // NOTE: don't use featureDefinitionCastSpell?. which bypasses Unity object lifetime check
                if (!featureDefinitionCastSpell
                    || !featureDefinitionCastSpell.SpellListDefinition
                    || spellLists.ContainsValue(featureDefinitionCastSpell.SpellListDefinition))
                {
                    continue;
                }

                spellLists.Add(title, featureDefinitionCastSpell.SpellListDefinition);

                foreach (var spell in featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(
                             x => x.Spells))
                {
                    if (!SpellSpellListMap.TryGetValue(spell, out var value))
                    {
                        value = [];
                        SpellSpellListMap.Add(spell, value);
                    }

                    value.Add(featureDefinitionCastSpell.SpellListDefinition);
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

    internal static bool IsTabletopSetSelected()
    {
        return SpellListContextTab.Values.All(spellListContext => spellListContext.IsTabletopSetSelected);
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

    internal static void SelectTabletopSet(bool toggle)
    {
        foreach (var spellListContext in SpellListContextTab.Values)
        {
            spellListContext.SelectTabletopSetInternal(toggle);
        }
    }

    internal static void RecalculateDisplayedSpells()
    {
        foreach (var spellListContext in SpellListContextTab.Values)
        {
            spellListContext.CalculateDisplayedSpellsInternal();
        }
    }

    internal static void LateLoad()
    {
        // init collections
        foreach (var spellList in SpellLists.Values)
        {
            var name = spellList.Name;

            SpellListContextTab.Add(spellList, new SpellListContext(spellList));

            Main.Settings.SpellListSpellEnabled.TryAdd(name, []);
            Main.Settings.DisplaySpellListsToggle.TryAdd(name, false);
            Main.Settings.SpellListSliderPosition.TryAdd(name, 4);
        }

        var spellListInventorClass = InventorClass.SpellList;

        // MUST COME BEFORE ANY MOD REGISTERED SPELL
        foreach (var kvp in SpellSpellListMap)
        {
            RegisterSpell(kvp.Key, kvp.Value.Count, kvp.Value.ToArray());
        }

        // cantrips
        RegisterSpell(BuildAcidClaw(), 0, SpellListDruid);
        RegisterSpell(AirBlast, 0, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildBladeWard(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildBoomingBlade(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BurstOfRadiance, 0, SpellListCleric);
        RegisterSpell(EnduringSting, 0, SpellListWizard);
        RegisterSpell(BuildIlluminatingSphere(), 0, SpellListBard, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildInfestation(), 0, SpellListDruid, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildLightningLure(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildMindSpike(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildMinorLifesteal(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildPrimalSavagery(), 0, SpellListDruid);
        RegisterSpell(BuildResonatingStrike(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(SunlightBlade, 0, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildSwordStorm(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildStarryWisp(), 0, SpellListBard, SpellListDruid);
        RegisterSpell(BuildTollTheDead(), 0, SpellListCleric, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildThornyVines(), 0, SpellListDruid, spellListInventorClass);
        RegisterSpell(BuildThunderStrike(), 0, SpellListBard, SpellListDruid, SpellListSorcerer, SpellListWarlock,
            SpellListWizard, spellListInventorClass);
        RegisterSpell(Wrack, 0, SpellListCleric);

        // 1st level
        RegisterSpell(CausticZap, 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildChaosBolt(), 0, SpellListSorcerer);
        RegisterSpell(BuildChromaticOrb(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(EarthTremor, 0, SpellListBard, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(EnsnaringStrike, 0, SpellListRanger);
        RegisterSpell(BuildElementalInfusion(), 0, SpellListDruid, SpellListRanger, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildFindFamiliar(), 0, SpellListWizard);
        RegisterSpell(BuildGiftOfAlacrity(), 0, SpellListWizard);
        RegisterSpell(BuildGoneWithTheWind(), 0, SpellListRanger);
        RegisterSpell(BuildIceBlade(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildMagnifyGravity(), 0, SpellListWizard);
        RegisterSpell(BuildMule(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildRadiantMotes(), 0, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildSanctuary(), 0, SpellListCleric, spellListInventorClass);
        RegisterSpell(SearingSmite, 0, SpellListPaladin, SpellListRanger);
        RegisterSpell(BuildSkinOfRetribution(), 0, SpellListWarlock);
        RegisterSpell(BuildSpikeBarrage(), 0, SpellListRanger);
        RegisterSpell(ThunderousSmite, 0, SpellListPaladin);
        RegisterSpell(BuildVileBrew(), 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildVoidGrasp(), 0, SpellListWarlock);
        RegisterSpell(BuildWitchBolt(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(WrathfulSmite, 0, SpellListPaladin);

        // 2nd level
        RegisterSpell(BuildAganazzarScorcher(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BindingIce, 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildBorrowedKnowledge(), 0, SpellListBard, SpellListCleric, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildCloudOfDaggers(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(ColorBurst, 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);
        DatabaseHelper.SpellDefinitions.ConjureGoblinoids.contentPack = CeContentPackContext.CeContentPack;
        RegisterSpell(DatabaseHelper.SpellDefinitions.ConjureGoblinoids, 0, SpellListDruid, SpellListRanger);
        RegisterSpell(BuildKineticJaunt(), 0, SpellListBard, SpellListSorcerer, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildNoxiousSpray(), 0, SpellListDruid, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(PetalStorm, 0, SpellListDruid);
        RegisterSpell(BuildProtectThreshold(), 0, SpellListCleric, SpellListDruid, SpellListPaladin);
        RegisterSpell(PsychicWhip, 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(MirrorImage, 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildShadowBlade(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildSnillocSnowballStorm(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(Web, 0, SpellListSorcerer, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildWitherAndBloom(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 3rd level
        RegisterSpell(BuildAdderFangs(), 0, SpellListDruid, SpellListRanger, SpellListSorcerer, SpellListWarlock);
        RegisterSpell(BuildAshardalonStride(), 0, SpellListRanger, SpellListSorcerer, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildAuraOfVitality(), 0, SpellListCleric, SpellListPaladin);
        RegisterSpell(BlindingSmite, 0, SpellListPaladin);
        RegisterSpell(BuildBoomingStep(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(CorruptingBolt, 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildCrusadersMantle(), 0, SpellListPaladin);
        RegisterSpell(ElementalWeapon, 0, SpellListDruid, SpellListPaladin, SpellListRanger, spellListInventorClass);
        RegisterSpell(BuildHungerOfTheVoid(), 0, SpellListWarlock);
        RegisterSpell(PulseWave, 0, SpellListWizard);
        RegisterSpell(BuildFlameArrows(), 0, SpellListDruid, SpellListRanger, SpellListSorcerer, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildLightningArrow(), 0, SpellListRanger);
        RegisterSpell(BuildIntellectFortress(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildSpiritShroud(), 0, SpellListCleric, SpellListPaladin, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildVitalityTransfer(), 0, SpellListCleric, SpellListWizard);
        RegisterSpell(BuildWinterBreath(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 4th level
        RegisterSpell(BuildAuraOfPerseverance(), 0, SpellListCleric, SpellListPaladin);
        RegisterSpell(AuraOfLife, 0, SpellListCleric, SpellListPaladin);
        RegisterSpell(BlessingOfRime, 0, SpellListBard, SpellListDruid, SpellListRanger);
        RegisterSpell(BuildBrainBulwark(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildElementalBane(), 0, SpellListDruid, SpellListWarlock, SpellListWizard,
            spellListInventorClass);
        RegisterSpell(BuildFaithfulHound(), 0, SpellListWizard, spellListInventorClass);
        RegisterSpell(BuildForestGuardian(), 0, SpellListDruid, SpellListRanger);
        RegisterSpell(BuildGravitySinkhole(), 0, SpellListWizard);
        RegisterSpell(BuildIrresistiblePerformance(), 0, SpellListBard);
        RegisterSpell(BuildPsychicLance(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildPsionicBlast(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(StaggeringSmite, 0, SpellListPaladin);

        //5th level
        RegisterSpell(BanishingSmite, 0, SpellListPaladin);
        RegisterSpell(BuildCircleOfMagicalNegation(), 0, SpellListPaladin);
        RegisterSpell(BuildDawn(), 0, SpellListCleric, SpellListWizard);
        RegisterSpell(DivineWrath, 0, SpellListPaladin);
        RegisterSpell(BuildEmpoweredKnowledge(), 0, SpellListBard, SpellListCleric, SpellListWarlock, SpellListWizard);
        RegisterSpell(FarStep, 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildIncineration(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(MantleOfThorns, 0, SpellListDruid);
        RegisterSpell(SteelWhirlwind, 0, SpellListRanger, SpellListWizard);
        RegisterSpell(SonicBoom, 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildSynapticStatic(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(Telekinesis, 0, SpellListSorcerer, SpellListWizard);

        // 6th level
        RegisterSpell(BuildHeroicInfusion(), 0, SpellListWizard);
        RegisterSpell(BuildFlashFreeze(), 0, SpellListDruid, SpellListSorcerer, SpellListWarlock);
        RegisterSpell(BuildMysticalCloak(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildPoisonWave(), 0, SpellListWizard);
        RegisterSpell(BuildFizbanPlatinumShield(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildRingOfBlades(), 0, SpellListWizard);
        RegisterSpell(BuildScatter(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildShelterFromEnergy(), 0, SpellListCleric, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 7th level
        RegisterSpell(BuildCrownOfStars(), 0, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildDraconicTransformation(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildReverseGravity(), 0, SpellListDruid, SpellListSorcerer, SpellListWizard);

        // 8th level
        RegisterSpell(BuildMindBlank(), 0, SpellListBard, SpellListWizard);
        RegisterSpell(MaddeningDarkness, 0, SpellListWarlock, SpellListWizard);

        // 9th level
        RegisterSpell(BuildForesight(), 0, SpellListBard, SpellListDruid, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildMassHeal(), 0, SpellListCleric);
        RegisterSpell(BuildMeteorSwarmSingleTarget(), 0, SpellListSorcerer, SpellListWizard);
        RegisterSpell(BuildPowerWordHeal(), 0, SpellListBard, SpellListCleric);
        RegisterSpell(BuildPowerWordKill(), 0, SpellListBard, SpellListSorcerer, SpellListWarlock, SpellListWizard);
        RegisterSpell(BuildTimeStop(), 0, SpellListWizard, SpellListSorcerer);
        RegisterSpell(BuildShapechange(), 0, SpellListDruid, SpellListWizard);
        RegisterSpell(BuildWeird(), 0, SpellListWarlock, SpellListWizard);

        Spells = [.. Spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.FormatTitle())];

        foreach (var kvp in SpellListContextTab)
        {
            // caches which spells are toggleable per spell list
            var spellListContext = kvp.Value;

            spellListContext.CalculateAllSpells();
            spellListContext.CalculateDisplayedSpellsInternal();

            // settings paring
            var spellListName = kvp.Key.Name;

            foreach (var name in Main.Settings.SpellListSpellEnabled[spellListName]
                         .Where(name => Spells.All(x => x.Name != name))
                         .ToList())
            {
                Main.Settings.SpellListSpellEnabled[spellListName].Remove(name);
            }
        }

        // cache spells with subs
        foreach (var parent in DatabaseRepository.GetDatabase<SpellDefinition>()
                     .Where(x => x.spellsBundle))
        {
            foreach (var child in parent.SubspellsList)
            {
                // tryAdd to avoid AtWill spells to mess up this collection
                SpellsChildMaster.TryAdd(child, parent);
            }
        }
    }

    private static void RegisterSpell(
        SpellDefinition spellDefinition,
        int suggestedStartsAt = 0,
        params SpellListDefinition[] registeredSpellLists)
    {
        if (!Spells.Add(spellDefinition))
        {
            return;
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

        var isActiveInAtLeastOneRepertoire = SpellLists.Values.Any(x => x.ContainsSpell(spellDefinition));

        if (!isActiveInAtLeastOneRepertoire || spellDefinition.contentPack != CeContentPackContext.CeContentPack)
        {
            return;
        }

        //Add cantrips to `All Cantrips` list, so that Warlock's `Pact of the Tome` and Loremaster's `Arcane Professor` would see them
        if (spellDefinition.SpellLevel == 0)
        {
            SpellListAllCantrips.AddSpell(spellDefinition);
        }

        //Add spells to `All Spells` list, so that Warlock's `Book of Ancient Secrets` and Bard's `Magic Secrets` would see them
        SpellListAllSpells.AddSpell(spellDefinition);

        //Add spells to Snipers lists
        var spellSniperClasses = new List<CharacterClassDefinition>
        {
            Cleric,
            Druid,
            Sorcerer,
            Warlock,
            Wizard
        };

        foreach (var spellSniperClass in spellSniperClasses)
        {
            if (spellDefinition.SpellLevel == 0 &&
                DatabaseHelper.TryGetDefinition<SpellListDefinition>(
                    $"SpellListFeatSpellSniper{spellSniperClass.Name}", out var spellListSniper))
            {
                spellListSniper.AddSpell(spellDefinition);
            }
        }
    }

    internal sealed class SpellListContext
    {
        internal SpellListContext(SpellListDefinition spellListDefinition)
        {
            SpellList = spellListDefinition;
            AllSpells = [];
            DisplayedSpells = [];
            DisplayedSuggestedSpells = [];
            DisplayedNonSuggestedSpells = [];
            DisplayedTabletopSpells = [];
            DisplayedNonTabletopSpells = [];
            MinimumSpells = [];
            SuggestedSpells = [];
            TabletopSpells = [];
        }

        private List<string> SelectedSpells => Main.Settings.SpellListSpellEnabled[SpellList.Name];
        private SpellListDefinition SpellList { get; }
        private HashSet<SpellDefinition> AllSpells { get; }
        internal HashSet<SpellDefinition> DisplayedSpells { get; }
        private HashSet<SpellDefinition> DisplayedSuggestedSpells { get; }
        private HashSet<SpellDefinition> DisplayedNonSuggestedSpells { get; }
        private HashSet<SpellDefinition> DisplayedTabletopSpells { get; }
        private HashSet<SpellDefinition> DisplayedNonTabletopSpells { get; }
        internal HashSet<SpellDefinition> MinimumSpells { get; }
        internal HashSet<SpellDefinition> SuggestedSpells { get; }
        private HashSet<SpellDefinition> TabletopSpells { get; }


        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsAllSetSelected =>
            DisplayedSpells.All(x => SelectedSpells.Contains(x.Name));

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsSuggestedSetSelected =>
            DisplayedSuggestedSpells.All(x => SelectedSpells.Contains(x.Name)) &&
            DisplayedNonSuggestedSpells.All(x => !SelectedSpells.Contains(x.Name));

        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal bool IsTabletopSetSelected =>
            DisplayedTabletopSpells.All(x => SelectedSpells.Contains(x.Name)) &&
            DisplayedNonTabletopSpells.All(x => !SelectedSpells.Contains(x.Name));

        internal void CalculateAllSpells()
        {
            var minSpellLevel = SpellList.HasCantrips ? 0 : 1;
            var maxSpellLevel = SpellList.MaxSpellLevel;

            foreach (var spell in Spells.Where(x =>
                         x.SpellLevel >= minSpellLevel &&
                         x.SpellLevel <= maxSpellLevel &&
                         !MinimumSpells.Contains(x)))
            {
                AllSpells.Add(spell);

                if (ModUi.TabletopDefinitionNames.Contains(spell.Name))
                {
                    TabletopSpells.Add(spell);
                }
            }
        }

        internal void CalculateDisplayedSpellsInternal()
        {
            DisplayedSpells.Clear();
            DisplayedSuggestedSpells.Clear();
            DisplayedNonSuggestedSpells.Clear();
            DisplayedTabletopSpells.Clear();
            DisplayedNonTabletopSpells.Clear();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var spell in AllSpells)
            {
                if (SpellsDisplay.SpellLevelFilter != -1 &&
                    spell.SpellLevel != SpellsDisplay.SpellLevelFilter)
                {
                    continue;
                }

                if (!Main.Settings.AllowDisplayingOfficialSpells &&
                    spell.ContentPack != CeContentPackContext.CeContentPack)
                {
                    continue;
                }

                if (!Main.Settings.AllowDisplayingNonSuggestedSpells &&
                    spell.ContentPack == CeContentPackContext.CeContentPack &&
                    !SuggestedSpells.Contains(spell))
                {
                    continue;
                }

                DisplayedSpells.Add(spell);

                if (SuggestedSpells.Contains(spell))
                {
                    DisplayedSuggestedSpells.Add(spell);
                }
                else
                {
                    DisplayedNonSuggestedSpells.Add(spell);
                }

                if (TabletopSpells.Contains(spell))
                {
                    DisplayedTabletopSpells.Add(spell);
                }
                else
                {
                    DisplayedNonTabletopSpells.Add(spell);
                }
            }
        }

        internal void SelectAllSetInternal(bool toggle)
        {
            foreach (var spell in DisplayedSpells)
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

            foreach (var spell in DisplayedSpells.Intersect(SuggestedSpells))
            {
                Switch(spell, toggle);
            }
        }

        internal void SelectTabletopSetInternal(bool toggle)
        {
            if (toggle)
            {
                SelectAllSetInternal(false);
            }

            foreach (var spell in DisplayedSpells.Intersect(TabletopSpells))
            {
                Switch(spell, toggle);
            }
        }

        internal void Switch([NotNull] SpellDefinition spellDefinition, bool active)
        {
            var spellListName = SpellList.Name;
            var spellName = spellDefinition.Name;

            if (!SpellList.HasCantrips && spellDefinition.SpellLevel == 0)
            {
                return;
            }

            InventorClass.SwitchSpellStoringItemSubPower(spellDefinition, active);

            if (!Main.Settings.AllowDisplayingOfficialSpells &&
                spellDefinition.ContentPack != CeContentPackContext.CeContentPack)
            {
                return;
            }

            if (active)
            {
                SpellList.AddSpell(spellDefinition);

                Main.Settings.SpellListSpellEnabled[spellListName].TryAdd(spellName);
            }
            else
            {
                foreach (var spellsByLevel in SpellList.SpellsByLevel)
                {
                    spellsByLevel.Spells.RemoveAll(x => x == spellDefinition);
                }

                Main.Settings.SpellListSpellEnabled[spellListName].Remove(spellName);
            }
        }
    }
}
