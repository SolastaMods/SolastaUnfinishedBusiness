using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;
using static SolastaUnfinishedBusiness.Displays.BackgroundsAndRacesDisplay;
using static SolastaUnfinishedBusiness.Displays.BlueprintDisplay;
using static SolastaUnfinishedBusiness.Displays.CharacterDisplay;
using static SolastaUnfinishedBusiness.Displays.CreditsDisplay;
using static SolastaUnfinishedBusiness.Displays.DungeonMakerDisplay;
using static SolastaUnfinishedBusiness.Displays.EffectsDisplay;
using static SolastaUnfinishedBusiness.Displays.EncountersDisplay;
using static SolastaUnfinishedBusiness.Displays.GameServicesDisplay;
using static SolastaUnfinishedBusiness.Displays.GameUiDisplay;
using static SolastaUnfinishedBusiness.Displays.ItemsAndCraftingDisplay;
using static SolastaUnfinishedBusiness.Displays.ProficienciesDisplay;
using static SolastaUnfinishedBusiness.Displays.RulesDisplay;
using static SolastaUnfinishedBusiness.Displays.SpellsDisplay;
using static SolastaUnfinishedBusiness.Displays.SubclassesDisplay;
using static SolastaUnfinishedBusiness.Displays.ToolsDisplay;
using static SolastaUnfinishedBusiness.Displays.TranslationsDisplay;

namespace SolastaUnfinishedBusiness.Displays;

// ReSharper disable once ClassNeverInstantiated.Global
internal static class ModUi
{
    internal const int DontDisplayDescription = 4;
    internal const float PixelsPerColumn = 220;

    internal static readonly HashSet<string> TabletopDefinitionNames =
    [
        "AirBlast",
        "AuraOfPerseverance",
        "AuraOfVitality",
        "BanishingSmite",
        "BindingIce",
        "BladeWard",
        "BlindFighting",
        "BlindingSmite",
        "BoomingBlade",
        "BoomingStep",
        "BurstOfRadiance",
        "ChromaticOrb",
        "CircleOfMagicalNegation",
        "CircleOfTheCosmos",
        "CircleOfTheNight",
        "CloudOfDaggers",
        "CollegeOfAudacity",
        "CollegeOfGuts",
        "CollegeOfLife",
        "CollegeOfValiance",
        "CrusadersMantle",
        "DivineWrath",
        "DomainSmith",
        "EarthTremor",
        "ElementalWeapon",
        "EnduringSting",
        "EnsnaringStrike",
        "FarStep",
        "FeatAlert",
        "FeatBladeMastery",
        "FeatBlindFighting",
        "FeatCharger",
        "FeatCleavingAttack",
        "FeatDeadeye",
        "FeatDefensiveDuelist",
        "FeatDragonWings",
        "FeatDualWeaponDefense",
        "FeatDungeonDelver",
        "FeatDurable",
        "FeatDwarvenFortitude",
        "FeatEldritchAdept",
        "FeatFellHanded",
        "FeatGiftOfTheChromaticDragon",
        "FeatGroupChef",
        "FeatGroupCrusher",
        "FeatGroupElementalAdept",
        "FeatGroupElvenAccuracy",
        "FeatGroupFadeAway",
        "FeatGroupFightingStyle",
        "FeatGroupFlamesOfPhlegethos",
        "FeatGroupMagicInitiate",
        "FeatGroupMediumArmor",
        "FeatGroupOrcishFury",
        "FeatGroupPiercer",
        "FeatGroupRevenantGreatSword",
        "FeatGroupSecondChance",
        "FeatGroupShadowTouched",
        "FeatGroupSkillExpert",
        "FeatGroupSlasher",
        "FeatGroupSpellSniper",
        "FeatGroupSquatNimbleness",
        "FeatGroupTelekinetic",
        "FeatGroupTeleportation",
        "FeatHealer",
        "FeatHeavyArmorMaster",
        "FeatInfernalConstitution",
        "FeatInspiringLeader",
        "FeatMediumArmorMaster",
        "FeatMetamagicAdept",
        "FeatMobile",
        "FeatPoisoner",
        "FeatPolearmExpert",
        "FeatRangedExpert",
        "FeatRemarkableTechnique",
        "FeatSavageAttack",
        "FeatSentinel",
        "FeatShieldTechniques",
        "FeatSkilled",
        "FeatSpearMastery",
        "FeatTacticianAdept",
        "FeatTough",
        "FeatWarCaster",
        "FindFamiliar",
        "FlameArrows",
        "Foresight",
        "ForestGuardian",
        "GravitySinkhole",
        "HeroicInfusion",
        "HungerOfTheVoid",
        "IceBlade",
        "Incineration",
        "InnovationArmor",
        "InnovationArtillerist",
        "InnovationWeapon",
        "Interception",
        "InvocationAbilitiesOfTheChainMaster",
        "InvocationAspectOfTheMoon",
        "InvocationBondOfTheTalisman",
        "InvocationEldritchMind",
        "InvocationEldritchSmite",
        "InvocationGiftOfTheEverLivingOnes",
        "InvocationGiftOfTheProtectors",
        "InvocationGraspingBlast",
        "InvocationHinderingBlast",
        "InvocationImprovedPactWeapon",
        "InvocationInexorableHex",
        "InvocationPerniciousCloak",
        "InvocationShroudOfShadow",
        "InvocationStasis",
        "InvocationSuperiorPactWeapon",
        "InvocationTombOfFrost",
        "InvocationTrickstersEscape",
        "InvocationUltimatePactWeapon",
        "InvocationUndyingServitude",
        "InvocationVexingHex",
        "LightningArrow",
        "MagnifyGravity",
        "MartialArcaneArcher",
        "MartialForceKnight",
        "MartialRoyalKnight",
        "MartialSpellShield",
        "MartialTactician",
        "MassHeal",
        "MeteorSwarmSingleTarget",
        "MindBlank",
        "MindSpike",
        "MirrorImage",
        "MysticalCloak",
        "OathOfAncients",
        "PathOfTheRavager",
        "PathOfTheSpirits",
        "PatronCelestial",
        "PatronSoulBlade",
        "PowerWordHeal",
        "PowerWordKill",
        "PsychicLance",
        "PsychicWhip",
        "PulseWave",
        "RaceBattleborn",
        "RaceBolgrif",
        "RaceFairy",
        "RaceKobold",
        "RaceMalakh",
        "RaceOligath",
        "RaceDarkelf",
        // "RaceHalfElfVariant",
        "RaceHalfElfDark",
        "RaceHalfElfHigh",
        "RaceHalfElfSylvan",
        // "RaceTiefling",
        "RaceTieflingDevilTongue",
        "RaceTieflingFeral",
        "RaceTieflingMephistopheles",
        "RaceTieflingZariel",
        "RangerGloomStalker",
        "RangerWildMaster",
        "RemarkableTechnique",
        "ResonatingStrike",
        "ReverseGravity",
        "RoguishSlayer",
        "Sanctuary",
        "SearingSmite",
        "ShadowBlade",
        "Shapechange",
        "SkinOfRetribution",
        "SorcerousDivineHeart",
        "SpellWeb",
        "SpikeBarrage",
        "SpiritShroud",
        "StaggeringSmite",
        "SteelWhirlwind",
        "StrikeWithTheWind",
        "SwordStorm",
        "Telekinesis",
        "ThornyVines",
        "ThunderousSmite",
        "ThunderStrike",
        "TimeStop",
        "TollTheDead",
        "VileBrew",
        "VitalityTransfer",
        "VoidGrasp",
        "WayOfSilhouette",
        "Weird",
        "WizardBladeDancer",
        "WizardDeadMaster",
        "WizardGraviturgist",
        "WrathfulSmite"
    ];

    internal static readonly HashSet<BaseDefinition> TabletopDefinitions = [];

    internal static void LoadTabletopDefinitions()
    {
        var raceDb = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        var subclassDb = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();
        var featDb = DatabaseRepository.GetDatabase<FeatDefinition>();
        var fightingStyleDb = DatabaseRepository.GetDatabase<FightingStyleDefinition>();
        var invocationDb = DatabaseRepository.GetDatabase<InvocationDefinition>();
        var metamagicOptionDb = DatabaseRepository.GetDatabase<MetamagicOptionDefinition>();
        var spellDb = DatabaseRepository.GetDatabase<SpellDefinition>();

        foreach (var definitionName in TabletopDefinitionNames)
        {
            if (raceDb.TryGetElement(definitionName, out var race))
            {
                TabletopDefinitions.Add(race);
            }
            else if (subclassDb.TryGetElement(definitionName, out var subclass))
            {
                TabletopDefinitions.Add(subclass);
            }
            else if (featDb.TryGetElement(definitionName, out var feat))
            {
                TabletopDefinitions.Add(feat);
            }
            else if (fightingStyleDb.TryGetElement(definitionName, out var fightingStyle))
            {
                TabletopDefinitions.Add(fightingStyle);
            }
            else if (invocationDb.TryGetElement(definitionName, out var invocation))
            {
                TabletopDefinitions.Add(invocation);
            }
            else if (metamagicOptionDb.TryGetElement(definitionName, out var metamagicOption))
            {
                TabletopDefinitions.Add(metamagicOption);
            }
            else if (spellDb.TryGetElement(definitionName, out var spell))
            {
                TabletopDefinitions.Add(spell);
            }
        }
    }

    internal static void DisplaySubMenu(ref int selectedPane, string title = null, params NamedAction[] actions)
    {
        if (!Main.Enabled)
        {
            return;
        }

        if (title != null)
        {
            UI.Div();
            UI.Label(title);
            UI.Space(7f);
        }

        UI.SubMenu(ref selectedPane, title != null, null, actions);
    }

    internal static bool DisplayDefinitions<T>(
        string label,
        Action<T, bool> switchAction,
        [NotNull] HashSet<T> registeredDefinitions,
        [NotNull] List<string> selectedDefinitions,
        ref bool displayToggle,
        ref int sliderPosition,
        bool useAlternateDescription = false,
        [CanBeNull] Action headerRendering = null,
        [CanBeNull] Action additionalRendering = null,
        bool displaySelectTabletop = true) where T : BaseDefinition
    {
        if (registeredDefinitions.Count == 0)
        {
            return false;
        }

        var selectAll = selectedDefinitions.Count == registeredDefinitions.Count;
        var selectTabletop =
            selectedDefinitions.Count == TabletopDefinitions.Intersect(registeredDefinitions).Count() &&
            selectedDefinitions.All(TabletopDefinitionNames.Contains);

        UI.Label();

        var toggle = displayToggle;

        if (UI.DisclosureToggle($"{label}:", ref toggle, 200))
        {
            displayToggle = toggle;
        }

        if (!displayToggle)
        {
            return selectTabletop;
        }

        UI.Label();

        headerRendering?.Invoke();

        using (UI.HorizontalScope())
        {
            toggle = sliderPosition == 1;

            if (UI.Toggle(Gui.Localize("ModUi/&ShowDescriptions"), ref toggle, UI.Width(PixelsPerColumn)))
            {
                sliderPosition = toggle ? 1 : 4;
            }

            if (additionalRendering != null)
            {
                additionalRendering.Invoke();
            }
            else
            {
                if (UI.Toggle(Gui.Localize("ModUi/&SelectAll"), ref selectAll, UI.Width(PixelsPerColumn)))
                {
                    foreach (var registeredDefinition in registeredDefinitions)
                    {
                        switchAction.Invoke(registeredDefinition, selectAll);
                    }
                }

                if (displaySelectTabletop)
                {
                    if (UI.Toggle(Gui.Localize("ModUi/&SelectTabletop"), ref selectTabletop,
                            UI.Width(PixelsPerColumn)))
                    {
                        foreach (var registeredDefinition in registeredDefinitions)
                        {
                            switchAction.Invoke(
                                registeredDefinition,
                                selectTabletop && TabletopDefinitions.Contains(registeredDefinition));
                        }
                    }
                }
            }
        }

        // UI.Slider("slide left for description / right to collapse".white().bold().italic(), ref sliderPosition, 1, maxColumns, 1, "");

        UI.Label();

        var flip = false;
        var current = 0;
        var count = registeredDefinitions.Count;

        using (UI.VerticalScope())
        {
            while (current < count)
            {
                var columns = sliderPosition;

                using (UI.HorizontalScope())
                {
                    while (current < count && columns-- > 0)
                    {
                        var definition = registeredDefinitions.ElementAt(current);
                        var title = definition.FormatTitle();
                        var isTabletop = TabletopDefinitions.Contains(definition);
                        var isVanilla = definition.ContentPack != CeContentPackContext.CeContentPack;

                        if (flip)
                        {
                            title = title.Khaki();
                        }
                        else if (sliderPosition == 1)
                        {
                            title = title.White();
                        }
                        else if (isTabletop)
                        {
                            title = title.Color("#D89555").Bold() + " \u00a9".Grey(); // copyright symbol
                        }
                        else if (isVanilla)
                        {
                            title = title.Orange() + " \u263c".Grey(); // sun symbol
                        }

                        toggle = selectedDefinitions.Contains(definition.Name);

                        if (UI.Toggle(title, ref toggle, UI.Width(PixelsPerColumn)))
                        {
                            switchAction.Invoke(definition, toggle);
                        }

                        if (sliderPosition == 1)
                        {
                            var description = useAlternateDescription
                                ? Gui.Localize($"ModUi/&{definition.Name}Description")
                                : definition.FormatDescription();

                            description = flip ? description.Khaki() : description.White();

                            UI.Label(description, UI.Width(PixelsPerColumn * 3));

                            flip = !flip;
                        }

                        current++;
                    }
                }
            }
        }

        return selectTabletop;
    }
}

[UsedImplicitly]
internal sealed class GameplayViewer : IMenuSelectablePage
{
    private int _gamePlaySelectedPane;
    public string Name => Gui.Localize("ModUi/&Gameplay");

    public int Priority => 100;

    public void OnGUI(UnityModManager.ModEntry modEntry)
    {
        ModUi.DisplaySubMenu(ref _gamePlaySelectedPane, Name,
            new NamedAction(Gui.Localize("ModUi/&Tools"), DisplayTools),
            new NamedAction(Gui.Localize("ModUi/&GeneralMenu"), DisplayCharacter),
            new NamedAction(Gui.Localize("ModUi/&Rules"), DisplayRules),
            new NamedAction(Gui.Localize("ModUi/&ItemsCraftingMerchants"), DisplayItemsAndCrafting));
    }
}

[UsedImplicitly]
internal sealed class CharacterViewer : IMenuSelectablePage
{
    private int _characterSelectedPane;
    public string Name => Gui.Localize("ModUi/&Character");

    public int Priority => 200;

    public void OnGUI(UnityModManager.ModEntry modEntry)
    {
        ModUi.DisplaySubMenu(ref _characterSelectedPane, Name,
            new NamedAction(Gui.Localize("ModUi/&BackgroundsAndRaces"),
                DisplayBackgroundsAndDeities),
            new NamedAction(Gui.Localize("Screen/&FeatureListingProficienciesTitle"),
                DisplayProficiencies),
            new NamedAction(Gui.Localize("ModUi/&SpellsMenu"),
                DisplaySpells),
            new NamedAction(Gui.Localize("ModUi/&Subclasses"),
                DisplaySubclasses));
    }
}

[UsedImplicitly]
internal sealed class InterfaceViewer : IMenuSelectablePage
{
    private int _interfaceSelectedPane;
    public string Name => Gui.Localize("ModUi/&Interface");

    public int Priority => 300;

    public void OnGUI(UnityModManager.ModEntry modEntry)
    {
        ModUi.DisplaySubMenu(ref _interfaceSelectedPane, Name,
            new NamedAction(Gui.Localize("ModUi/&GameUi"), DisplayGameUi),
            new NamedAction(Gui.Localize("ModUi/&DungeonMakerMenu"), DisplayDungeonMaker),
            new NamedAction(Gui.Localize("ModUi/&Translations"), DisplayTranslations));
    }
}

[UsedImplicitly]
internal sealed class PartyEditorViewer : IMenuSelectablePage
{
    public string Name => "PartyEditor".Localized();

    public int Priority => 400;

    public void OnGUI(UnityModManager.ModEntry modEntry)
    {
        PartyEditor.OnGUI();
    }
}

[UsedImplicitly]
internal sealed class EncountersViewer : IMenuSelectablePage
{
    private int _encountersSelectedPane;
    public string Name => Gui.Localize("ModUi/&Encounters");

    public int Priority => 500;

    public void OnGUI(UnityModManager.ModEntry modEntry)
    {
        ModUi.DisplaySubMenu(ref _encountersSelectedPane, Name,
            new NamedAction(Gui.Localize("ModUi/&GeneralMenu"), DisplayEncountersGeneral),
            new NamedAction(Gui.Localize("ModUi/&Bestiary"), DisplayBestiary),
            new NamedAction(Gui.Localize("ModUi/&CharactersPool"), DisplayNpcs));
    }
}

[UsedImplicitly]
internal sealed class CreditsAndDiagnosticsViewer : IMenuSelectablePage
{
    private int _creditsSelectedPane;
    public string Name => Gui.Localize("ModUi/&CreditsAndDiagnostics");

    public int Priority => 999;

    public void OnGUI(UnityModManager.ModEntry modEntry)
    {
        ModUi.DisplaySubMenu(ref _creditsSelectedPane, null,
            new NamedAction(Gui.Localize("ModUi/&Credits"), DisplayCredits),
            new NamedAction("Effects", DisplayEffects),
            new NamedAction(Gui.Localize("ModUi/&Blueprints"), DisplayBlueprints),
            new NamedAction(Gui.Localize("ModUi/&Services"), DisplayGameServices));
    }
}
