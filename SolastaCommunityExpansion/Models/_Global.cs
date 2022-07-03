using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;

namespace SolastaCommunityExpansion.Models;

// keep public for sidecars
public static class Global
{
    // is a user location
    // NOTE: don't use GameLocation?. or LocationDefinition?. which bypasses Unity object lifetime check
    public static bool IsUserLocation => Gui.GameLocation &&
                                         Gui.GameLocation.LocationDefinition &&
                                         Gui.GameLocation.LocationDefinition.IsUserLocation;

    // last level up hero name
    public static string LastLevelUpHeroName { get; set; }

    // level up hero
    public static RulesetCharacterHero ActiveLevelUpHero =>
        ServiceRepository.GetService<ICharacterBuildingService>()?.CurrentLocalHeroCharacter;

    // holds the active player character when in battle
    public static GameLocationCharacter ActivePlayerCharacter { get; set; }

    // inspected hero on both location and pool
    public static RulesetCharacterHero InspectedHero { get; set; }

    // holds the current action from any character on the map
    public static CharacterAction CurrentAction { get; set; }

    // holds the the casted spell
    public static SpellDefinition CastedSpell { get; set; }

    // holds the the casting repertoire
    public static RulesetSpellRepertoire CastedSpellRepertoire { get; set; }

    // last attack was a critical hit
    public static bool CriticalHit { get; set; }

    // holds a collection of conditions that should display on char panel even if set to silent
    public static HashSet<ConditionDefinition> CharacterLabelEnabledConditions { get; } = new();

    // true if in a multiplayer game
    public static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    // true if not in game
    public static bool IsOffGame => Gui.Game == null;

    public static bool ActiveLevelUpHeroHasCantrip(SpellDefinition spellDefinition)
    {
        var hero = ActiveLevelUpHero;

        if (hero == null)
        {
            return true;
        }

        return hero.SpellRepertoires.Any(x => x.KnownCantrips.Contains(spellDefinition))
               || hero.GetHeroBuildingData().AcquiredCantrips.Any(e => e.Value.Contains(spellDefinition));
    }

    public static bool ActiveLevelUpHeroHasSubclass(string subclass)
    {
        var hero = ActiveLevelUpHero;

        return hero == null || hero.ClassesAndSubclasses.Any(e => e.Value.Name == subclass);
    }

    public static bool ActiveLevelUpHeroHasFeature(FeatureDefinition feature, bool recursive = true)
    {
        var hero = ActiveLevelUpHero;

        if (feature is FeatureDefinitionFeatureSet set)
        {
            return hero != null && hero.HasAllFeatures(set.FeatureSet);
        }

        return hero == null || recursive
            ? hero.HasAnyFeature(feature)
            : hero.ActiveFeatures
                  .SelectMany(x => x.Value)
                  .Any(x => x == feature)
              || hero.GetHeroBuildingData().AllActiveFeatures.Contains(feature);
    }
}
