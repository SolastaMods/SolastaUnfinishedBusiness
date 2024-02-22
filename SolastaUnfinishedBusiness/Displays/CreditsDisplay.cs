using System.Collections.Generic;
using System.IO;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using UnityExplorer;
#if DEBUG
using static SolastaUnfinishedBusiness.Displays.PatchesDisplay;
#endif

namespace SolastaUnfinishedBusiness.Displays;

internal static class CreditsDisplay
{
#if DEBUG
    private static bool _displayPatches;
#endif

    // ReSharper disable once MemberCanBePrivate.Global
    internal static readonly List<(string, string)> CreditsTable =
    [
        ("Zappastuff",
            "maintenance, mod UI, infrastructure, gameplay, quality of life, rules, " +
            "backgrounds, feats, fighting styles, gambits, invocations, metamagic, spells, " +
            "Fairy, Half-elf, Tiefling, " +
            "Circle of the Ancient Forest, Circle of the Cosmos, Circle of the Eternal Grove, " +
            "College of Audacity, College of Guts, College of Life, College of Thespian, College of Valiance, " +
            "Innovation Artillerist, Innovation Vitriolist, Innovation Vivisectionist, " +
            "Martial Arcane Archer, Martial Force Knight, Martial Guardian, Martial Royal Knight, Martial Warlord, Martial Weapon Master, " +
            "Oath of Altruism, Oath of Dread, Oath of Thunder, " +
            "Path of the Elements, Path of the Reaver, Path of the Savagery, Path of the Yeoman, " +
            "Patron Celestial, Patron Moonlit, Patron Mountain, Patron Riftwalker, " +
            "Ranger Hellwalker, Ranger Lightbearer, Ranger Sky Warrior, Ranger Survivalist, Ranger Wildmaster, " +
            "Roguish Acrobat, Roguish Arcane Scoundrel, Roguish Blade Caller, Roguish Duelist, Roguish Slayer, Roguish Umbral Stalker, " +
            "Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Scion, Sorcerous Sorr-Akkath, Sorcerous Spellblade, " +
            "Way of the Discordance, Way of the Silhouette, Way of the Tempest, Way of Weal and Woe, Way of Zen Archery, " +
            "Wizard Bladedancer, Wizard Deadmaster, " +
            "Level 20, Lighting and Obscurement, Multiclass"),

        ("HiddenHax",
            "QA master, SFX, sprites, homebrew design [feats, gambits, invocations, spells, " +
            "Circle of the Eternal Grove, " +
            "College of Audacity, College of Thespian, College of Valiance, " +
            "Martial Arcane Archer, Martial Force Knight, Martial Guardian, Martial Warlord, Martial Weapon Master, " +
            "Oath of Ancients, Oath of Dread, " +
            "Path of the Elements, Path of the Reaver, Path of the Savagery, " +
            "Patron Moonlit, " +
            "Roguish Arcane Scoundrel, Roguish Blade Caller, Roguish Duelist, Roguish Slayer, Roguish Umbral Stalker, " +
            "Sorcerous Field Manipulator, Sorcerous Forceblade, Sorcerous Psion, Sorcerous Sorr-Akkath, Sorcerous Spellblade, " +
            "Way of the Discordance, Way of the Silhouette, Way of the Tempest, Way of Dragon, Way of Tempest, Way of Zen Archery]"),

        ("TPABOBAP",
            "custom behaviors, game UI, infrastructure, gameplay, quality of life, rules, " +
            "feats, fighting styles, gambits, invocations, metamagic, spells, " +
            "Innovation Armor, Innovation Grenadier, Innovation Weapon, " +
            "Patron Elementalist, Patron Soulblade, " +
            "Martial Tactician, Inventor, Multiclass"),

        ("ImpPhil", "api, builders, gameplay, rules, quality of life"),

        ("ChrisJohnDigital",
            "builders, gameplay, feats, fighting styles, " +
            "Wizard Arcane Fighter, Wizard Spellmaster, Martial Spell Shield"),

        ("Haxermn", "spells, Domain Defiler, Domain Smith, Oath of Ancients, Oath of Hatred, Way of Dragon"),

        ("Nd", "College of Wardancer, Roguish Opportunist, Roguish Raven"),
        ("tivie", "Circle of the Night, Path of the Spirits"),
        ("ElAntonius", "feats, fighting styles, Ranger Arcanist"),
        ("Kloranger", "feats, spells, Wizard Graviturgist"),
        ("magicskysword", "chinese translations, asian languages support, Oath of Demon Hunter"),
        ("DreadMaker", "Circle of the Forest Guardian"),
        ("RedOrca", "Path of the Light"),
        ("HIEROT", "Patron Eldritch Surge"),

        ("SilverGriffon",
            "gameplay, visuals, spells, " +
            "Dark Elf, Draconic Kobold, Grey Dwarf, Sorcerous Divine Heart"),
        ("Otearaisu", "Battleborn, Malakh, Oligath, Oni, Wendigo, Wildling, Wyrmkin"),
        ("Remunos", "Ironborn Dwarf, Obsidian Dwarf"),
        ("Holic75", "spells, Bolgrif"),

        ("Bazou", "fighting styles, rules, spells"),
        ("Andargor", "rules, quality of life"),
        ("TheRev", "quality of life"),
        ("Kiloku", "quality of life"),

        ("DemonicDuck",
            "QA, homebrew design [rules, feats, fighting styles, spells, Innovation Vivisectionist, Oath of the Thunder, Sorcerous Sorr-Akkath, Way of Weal and Woe]"),

        ("Vess", "QA, homebrew design [Innovation Vitriolist]"),

        ("Earandil",
            "homebrew design [Patron Mountain, Path of the Savagery, Path of the Yeoman, Ranger Sky Warrior, Ranger Survivalist]"),

        ("Taco", "sprites, homebrew design [feats, Roguish Acrobat, Domain Defiler, Oath of Altruism]"),
        ("DubhHerder", "quality of life, spells, homebrew design [Patron Elementalist, Patron Riftwalker]"),
        ("Stuffies12", "homebrew design [Ranger Hellwalker, Ranger Lightbearer]"),

        ("Narria", "modKit, mod UI improvements, Party Editor"),

        ("Artyoan", "pre-gen heroes and sample portraits"),
        ("Thaladar", "sample portraits"),

        ("team-waldo", "official korean font and translations"),
        ("akintos", "korean translations"),
        ("Dovel", "russian and non-official russian translations"),
        ("Ermite_Crabe", "french translations")
    ];

    private static readonly bool IsUnityExplorerInstalled =
        File.Exists(Path.Combine(Main.ModFolder, "UnityExplorer.STANDALONE.Mono.dll")) &&
        File.Exists(Path.Combine(Main.ModFolder, "UniverseLib.Mono.dll"));

    private static bool IsUnityExplorerEnabled { get; set; }

    private static void EnableUnityExplorerUi()
    {
        IsUnityExplorerEnabled = true;

        try
        {
            ExplorerStandalone.CreateInstance();
        }
        catch
        {
            // ignored
        }
    }

    internal static void DisplayCredits()
    {
        UI.Label();

        if (IsUnityExplorerInstalled && !IsUnityExplorerEnabled)
        {
            UI.ActionButton("Unity Explorer UI".Bold().Khaki(), EnableUnityExplorerUi, UI.Width(150f));
            UI.Label();
        }

#if DEBUG
        DiagnosticsDisplay.DisplayDiagnostics();

        UI.DisclosureToggle(Gui.Localize("ModUi/&Patches"), ref _displayPatches, 200);
        UI.Label();

        if (_displayPatches)
        {
            DisplayPatches();
        }
        else
#endif
        {
            UI.Label(
                "<b><color=#D89555>SPECIAL THANKS:</color></b> Tactical Adventures / JetBrains <i><color=#F0DAA0>[development licenses]</color></i> / DemonicDuck, Gwizzz, Vess <i><color=#F0DAA0>[hardware acquisition]</color></i> / Balmz <i><color=#F0DAA0>[coffee eh!]</color></i>");

            UI.Label();

            // credits
            foreach (var (author, content) in CreditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(author.Orange(), UI.Width(150f));
                    UI.Label(content, UI.Width(740f));
                }
            }
        }

        UI.Label();
    }
}
