using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

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
    [CanBeNull]
    public static RulesetCharacterHero ActiveLevelUpHero =>
        ServiceRepository.GetService<ICharacterBuildingService>()?.CurrentLocalHeroCharacter;

    // inspected hero on both location and pool
    [CanBeNull] public static RulesetCharacterHero InspectedHero { get; set; }

    // holds the active player character
    public static GameLocationCharacter ActivePlayerCharacter { get; private set; }

    // holds the current action from any character on the map
    public static CharacterAction CurrentAction { get; private set; }

    // holds the the casted spell
    public static SpellDefinition CastedSpell { get; private set; }

    // last attack was a critical hit
    public static bool CriticalHit { get; set; }

    // holds a collection of conditions that should display on char panel even if set to silent
    public static HashSet<ConditionDefinition> CharacterLabelEnabledConditions { get; } = new();

    // true if in a multiplayer game
    public static bool IsSettingUpMultiplayer { get; set; }

    public static bool IsMultiplayer => IsSettingUpMultiplayer
                                        || ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    // true if not in game
    public static bool IsOffGame => Gui.Game == null;

    /* Deprecating Magus-related flags
    public static bool IsSpellStrike { get; set; }

    public static RuleDefinitions.RollOutcome SpellStrikeRollOutcome { get; set; } =
        RuleDefinitions.RollOutcome.Neutral;

    public static int SpellStrikeDieRoll { get; set; } = 10;
    */

    internal static void ActionStarted([NotNull] CharacterAction characterAction)
    {
        CurrentAction = characterAction;
        ActivePlayerCharacter = characterAction.ActingCharacter;
        CastedSpell = null;
        /* Deprecating Magus-related flags
        IsSpellStrike = false;
        SpellStrikeRollOutcome = RuleDefinitions.RollOutcome.Neutral;
        SpellStrikeDieRoll = -1;
        */

        switch (characterAction)
        {
            case CharacterActionCastSpell actionCastSpell:
                CastedSpell = actionCastSpell.ActiveSpell.SpellDefinition;
                // Hold the state of the SHIFT key on BOOL PARAM 5
                // Used to determine which slot to use on MC Warlock
                characterAction.actionParams.BoolParameter5 =
                    Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                break;
            case CharacterActionReady actionReady:
                CustomReactionsContext.ReadReadyActionPreferredCantrip(actionReady.actionParams);

                break;
            case CharacterActionSpendPower spendPower:
                PowersBundleContext.SpendBundledPowerIfNeeded(spendPower);

                break;
        }
    }

    // public static bool ActiveLevelUpHeroHasCantrip(SpellDefinition spellDefinition)
    // {
    //     var hero = ActiveLevelUpHero;
    //
    //     if (hero == null)
    //     {
    //         return true;
    //     }
    //
    //     return hero.SpellRepertoires.Any(x => x.KnownCantrips.Contains(spellDefinition))
    //            || hero.GetHeroBuildingData().AcquiredCantrips.Any(e => e.Value.Contains(spellDefinition));
    // }
    //
    // public static bool ActiveLevelUpHeroHasSubclass(string subclass)
    // {
    //     var hero = ActiveLevelUpHero;
    //
    //     return hero == null || hero.ClassesAndSubclasses.Any(e => e.Value.Name == subclass);
    // }
    //
    // public static bool ActiveLevelUpHeroHasFeature(FeatureDefinition feature, bool recursive = true)
    // {
    //     var hero = ActiveLevelUpHero;
    //
    //     if (feature is FeatureDefinitionFeatureSet set)
    //     {
    //         return hero != null && hero.HasAllFeatures(set.FeatureSet);
    //     }
    //
    //     return hero == null || recursive
    //         ? hero.HasAnyFeature(feature)
    //         : hero.ActiveFeatures
    //               .SelectMany(x => x.Value)
    //               .Any(x => x == feature)
    //           || hero.GetHeroBuildingData().AllActiveFeatures.Contains(feature);
    // }
}
