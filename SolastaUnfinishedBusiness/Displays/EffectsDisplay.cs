using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class EffectsDisplay
{
    private static Vector2 EffectPosition { get; set; } = Vector2.zero;

    private static bool ToggleCaster { get; set; }
    private static bool ToggleCondition { get; set; }
    private static bool ToggleImpact { get; set; }
    private static bool ToggleZone { get; set; }

    internal static void DisplayEffects()
    {
        UI.Label();

        if (Gui.GameCampaign == null || Gui.GameCampaign.Party.CharactersList.Count < 2)
        {
            UI.Label("You must have a game open with at least 2 heroes in the party");

            return;
        }

        var toggle = ToggleCaster;

        if (UI.DisclosureToggle(Gui.Localize("Caster:"), ref toggle))
        {
            ToggleCaster = toggle;
        }

        if (ToggleCaster)
        {
            DisplayEffects(EffectHelpers.EffectType.Caster);
        }

        UI.Label();

        toggle = ToggleCondition;

        if (UI.DisclosureToggle(Gui.Localize("Condition:"), ref toggle))
        {
            ToggleCondition = toggle;
        }

        if (ToggleCondition)
        {
            DisplayEffects(EffectHelpers.EffectType.Condition);
        }

        UI.Label();

        toggle = ToggleImpact;

        if (UI.DisclosureToggle(Gui.Localize("Impact:"), ref toggle))
        {
            ToggleImpact = toggle;
        }

        if (ToggleImpact)
        {
            DisplayEffects(EffectHelpers.EffectType.Impact);
        }

        UI.Label();

        toggle = ToggleZone;

        if (UI.DisclosureToggle(Gui.Localize("Zone:"), ref toggle))
        {
            ToggleZone = toggle;
        }

        if (ToggleZone)
        {
            DisplayEffects(EffectHelpers.EffectType.Zone);
        }

        UI.Label();
    }

    private static void DisplayEffects(EffectHelpers.EffectType effectType)
    {
        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        if (gameLocationCharacterService.PartyCharacters.Count < 2)
        {
            return;
        }

        using var scrollView =
            new GUILayout.ScrollViewScope(EffectPosition, UI.AutoWidth(), UI.AutoHeight());

        EffectPosition = scrollView.scrollPosition;

        UI.Label();

        foreach (var effect in EffectsContext.Effects[effectType])
        {
            using (UI.HorizontalScope())
            {
                UI.ActionButton("+".Bold().Red(), () =>
                    {
                        var source = gameLocationCharacterService.PartyCharacters[0];
                        var target = gameLocationCharacterService.PartyCharacters[1];

                        EffectHelpers.StartVisualEffect(source, target, effect.Value.First().Item2, effectType);
                    },
                    UI.Width((float)30));

                var label = effect.Key + " - " + effect.Value.First().Item1;

                UI.Label(label, UI.AutoWidth());
            }
        }
    }
}
