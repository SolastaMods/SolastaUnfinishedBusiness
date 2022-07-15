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
        public string Name => Gui.Localize("ModUi/&Character");

        public int Priority => 100;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref characterSelectedPane,
                new NamedAction(Gui.Localize("ModUi/&GeneralMenu"), DisplayCharacter),
                new NamedAction(
                    Gui.Localize("ModUi/&RacesClassesSubclasses"),
                    DisplayClassesAndSubclasses),
                new NamedAction(Gui.Localize("ModUi/&FeatsFightingStyles"),
                    DisplayFeatsAndFightingStyles),
                new NamedAction(Gui.Localize("ModUi/&SpellsMenu"), DisplaySpells));
        }
    }

    public class GameplayViewer : IMenuSelectablePage
    {
        private int gamePlaySelectedPane;
        public string Name => Gui.Localize("ModUi/&Gameplay");

        public int Priority => 200;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref gamePlaySelectedPane,
                new NamedAction(Gui.Localize("ModUi/&Rules"), DisplayRules),
                new NamedAction(
                    Gui.Localize("ModUi/&ItemsCraftingMerchants"),
                    DisplayItemsAndCrafting),
                new NamedAction(Gui.Localize("ModUi/&Tools"), DisplayTools));
        }
    }

    public class InterfaceViewer : IMenuSelectablePage
    {
        private int interfaceSelectedPane;
        public string Name => Gui.Localize("ModUi/&Interface");

        public int Priority => 300;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref interfaceSelectedPane,
                new NamedAction(Gui.Localize("ModUi/&DungeonMakerMenu"),
                    DisplayDungeonMaker),
                new NamedAction(Gui.Localize("ModUi/&GameUi"), DisplayGameUi),
                new NamedAction(Gui.Localize("ModUi/&Input"),
                    DisplayKeyboardAndMouse),
                new NamedAction(Gui.Localize("ModUi/&Translations"),
                    DisplayTranslations));
        }
    }

    public class EncountersViewer : IMenuSelectablePage
    {
        private int encountersSelectedPane;
        public string Name => Gui.Localize("ModUi/&Encounters");

        public int Priority => 400;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            DisplaySubMenu(ref encountersSelectedPane,
                new NamedAction(Gui.Localize("ModUi/&GeneralMenu"),
                    DisplayEncountersGeneral),
                new NamedAction(Gui.Localize("ModUi/&Bestiary"), DisplayBestiary),
                new NamedAction(Gui.Localize("ModUi/&CharactersPool"), DisplayNpcs));
        }
    }

    public class CreditsAndDiagnosticsViewer : IMenuSelectablePage
    {
        private int creditsSelectedPane;
        public string Name => Gui.Localize("ModUi/&CreditsAndDiagnostics");

        public int Priority => 999;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref creditsSelectedPane,
            new NamedAction(Gui.Localize("ModUi/&Credits"), DisplayCredits),
#if DEBUG
            new NamedAction("Diagnostics", DisplayDiagnostics),
#endif
            new NamedAction(Gui.Localize("ModUi/&Blueprints"), DisplayBlueprints),
            new NamedAction(Gui.Localize("ModUi/&Services"), DisplayGameServices)
        );
    }
}
