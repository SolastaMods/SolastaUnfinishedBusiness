using SolastaCommunityExpansion.Api.ModKit;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Displays.BlueprintDisplay;
using static SolastaCommunityExpansion.Displays.CharacterDisplay;
using static SolastaCommunityExpansion.Displays.CreditsDisplay;
using static SolastaCommunityExpansion.Displays.DungeonMakerDisplay;
using static SolastaCommunityExpansion.Displays.EncountersDisplay;
using static SolastaCommunityExpansion.Displays.FeatsAndFightingStylesDisplay;
using static SolastaCommunityExpansion.Displays.GameServicesDisplay;
using static SolastaCommunityExpansion.Displays.GameUiDisplay;
using static SolastaCommunityExpansion.Displays.ItemsAndCraftingDisplay;
using static SolastaCommunityExpansion.Displays.KeyboardAndMouseDisplay;
using static SolastaCommunityExpansion.Displays.RacesClassesAndSubclassesDisplay;
using static SolastaCommunityExpansion.Displays.RulesDisplay;
using static SolastaCommunityExpansion.Displays.Shared;
using static SolastaCommunityExpansion.Displays.SpellsDisplay;
using static SolastaCommunityExpansion.Displays.ToolsDisplay;
using static SolastaCommunityExpansion.Displays.TranslationsDisplay;
#if DEBUG
using static SolastaCommunityExpansion.Displays.DiagnosticsDisplay;
#endif

namespace SolastaCommunityExpansion.Displays
{
    public class ModUi : IMenuSelectablePage
    {
        private int characterSelectedPane;
        public string Name => Main.Enabled ? Gui.Localize("ModUi/&Character") : "Character";

        public int Priority => 100;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref characterSelectedPane,
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&GeneralMenu") : "General", DisplayCharacter),
                new NamedAction(
                    Main.Enabled ? Gui.Localize("ModUi/&RacesClassesSubclasses") : "Races, Classes & Subclasses",
                    DisplayClassesAndSubclasses),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&FeatsFightingStyles") : "Feats & Fighting Styles",
                    DisplayFeatsAndFightingStyles),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&SpellsMenu") : "Spells", DisplaySpells));
        }
    }

    public class GameplayViewer : IMenuSelectablePage
    {
        private int gamePlaySelectedPane;
        public string Name => Main.Enabled ? Gui.Localize("ModUi/&Gameplay") : "Gameplay";

        public int Priority => 200;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref gamePlaySelectedPane,
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Rules") : "Rules", DisplayRules),
                new NamedAction(
                    Main.Enabled ? Gui.Localize("ModUi/&ItemsCraftingMerchants") : "Items, Crafting & Merchants",
                    DisplayItemsAndCrafting),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Tools") : "Tools", DisplayTools));
        }
    }

    public class InterfaceViewer : IMenuSelectablePage
    {
        private int interfaceSelectedPane;
        public string Name => Main.Enabled ? Gui.Localize("ModUi/&Interface") : "Interface";

        public int Priority => 300;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref interfaceSelectedPane,
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&DungeonMakerMenu") : "Dungeon Maker",
                    DisplayDungeonMaker),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&GameUi") : "Game UI", DisplayGameUi),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&KeyboardMouse") : "Keyboard & Mouse",
                    DisplayKeyboardAndMouse),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Translations") : "Translations",
                    DisplayTranslations));
        }
    }

    public class EncountersViewer : IMenuSelectablePage
    {
        private int encountersSelectedPane;
        public string Name => Main.Enabled ? Gui.Localize("ModUi/&Encounters") : "Encounters";

        public int Priority => 400;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref encountersSelectedPane,
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&GeneralMenu") : "General",
                    DisplayEncountersGeneral),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Bestiary") : "Bestiary", DisplayBestiary),
                new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&CharactersPool") : "Characters Pool", DisplayNPCs));
        }
    }

    public class CreditsAndDiagnosticsViewer : IMenuSelectablePage
    {
        private int creditsSelectedPane;
        public string Name => Main.Enabled ? Gui.Localize("ModUi/&CreditsAndDiagnostics") : "Credits & Diagnostics";

        public int Priority => 999;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref creditsSelectedPane,
            new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Credits") : "Credits", DisplayCredits),
#if DEBUG
            new NamedAction("Diagnostics", DisplayDiagnostics),
#endif
            new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Blueprints") : "Blueprints", DisplayBlueprints),
            new NamedAction(Main.Enabled ? Gui.Localize("ModUi/&Services") : "Services", DisplayGameServices)
        );
    }
}
