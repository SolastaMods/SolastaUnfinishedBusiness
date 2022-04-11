using UnityModManagerNet;
using ModKit;
using static SolastaCommunityExpansion.Displays.BlueprintDisplay;
using static SolastaCommunityExpansion.Displays.CampaignsAndLocationsDisplay;
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
#if DEBUG
using static SolastaCommunityExpansion.Displays.DiagnosticsDisplay;
#endif

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SolastaCommunityExpansion
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class ModUi : IMenuSelectablePage
    {
        public string Name => "Character";

        public int Priority => 100;

        private int characterSelectedPane;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref characterSelectedPane,
            new NamedAction("General", DisplayCharacter),
            new NamedAction("Races, Classes & Subclasses", DisplayClassesAndSubclasses),
            new NamedAction("Feats & Fighting Styles", DisplayFeatsAndFightingStyles),
            new NamedAction("Spells", DisplaySpells));
    }

    public class GameplayViewer : IMenuSelectablePage
    {
        public string Name => "Gameplay";

        public int Priority => 200;

        private int gamePlaySelectedPane;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref gamePlaySelectedPane,
            new NamedAction("Rules", DisplayRules),
            new NamedAction("Campaigns & Locations", DisplayCampaignsAndLocations),
            new NamedAction("Items, Crafting & Merchants", DisplayItemsAndCrafting),
            new NamedAction("Tools", DisplayTools));
    }

    public class InterfaceViewer : IMenuSelectablePage
    {
        public string Name => "Interface";

        public int Priority => 300;

        private int interfaceSelectedPane;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref interfaceSelectedPane,
            new NamedAction("Dungeon Maker", DisplayDungeonMaker),
            new NamedAction("Game UI", DisplayGameUi),
            new NamedAction("Keyboard & Mouse", DisplayKeyboardAndMouse));
    }

    public class EncountersViewer : IMenuSelectablePage
    {
        public string Name => "Encounters";

        public int Priority => 400;

        private int encountersSelectedPane;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref encountersSelectedPane,
            new NamedAction("General", DisplayEncountersGeneral),
            new NamedAction("Bestiary", DisplayBestiary),
            new NamedAction("Characters Pool", DisplayNPCs));
    }

    public class CreditsAndDiagnosticsViewer : IMenuSelectablePage
    {
        public string Name => "Credits & Diagnostics";

        public int Priority => 999;

        private int creditsSelectedPane;

        public void OnGUI(UnityModManager.ModEntry modEntry) => DisplaySubMenu(ref creditsSelectedPane,
            new NamedAction("Credits", DisplayCredits),
#if DEBUG
            new NamedAction("Diagnostics", DisplayDiagnostics),
#endif
            new NamedAction("Blueprints", DisplayBlueprints),
            new NamedAction("Services", DisplayGameServices)
            );
    }
}
