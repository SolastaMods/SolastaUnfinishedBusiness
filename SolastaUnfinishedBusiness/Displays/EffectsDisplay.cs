using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class EffectsDisplay
{
    private static readonly ConditionDefinition DummyCondition = ConditionDefinitionBuilder
        .Create("DummyCondition")
        .SetGuiPresentation("Dummy", "Dummy")
        .SetSpecialDuration(RuleDefinitions.DurationType.Round, 1)
        .AddToDB();

    private static Vector2 EffectPosition { get; set; } = Vector2.zero;

    private static bool ToggleCaster { get; set; }
    private static bool ToggleCondition { get; set; }
    private static bool ToggleEffect { get; set; }
    private static bool ToggleImpact { get; set; }
    private static bool ToggleZone { get; set; }

    internal static void DisplayEffects()
    {
        if (EffectsContext.Dirty)
        {
            EffectsContext.DumpEffects();
        }

        UI.Label();

        if (Gui.GameCampaign == null || Gui.GameCampaign.Party.CharactersList.Count < 2)
        {
            UI.Label("You must have a game open with at least 2 heroes in the party...".Red().Bold());
            UI.Label();

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
            DisplayConditionEffects();
        }

        UI.Label();

        toggle = ToggleEffect;

        if (UI.DisclosureToggle(Gui.Localize("Effect:"), ref toggle))
        {
            ToggleEffect = toggle;
        }

        if (ToggleEffect)
        {
            DisplayEffects(EffectHelpers.EffectType.Effect);
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

        UI.Label();

        using var scrollView =
            new GUILayout.ScrollViewScope(EffectPosition, UI.AutoWidth(), UI.AutoHeight());

        EffectPosition = scrollView.scrollPosition;

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

    private static void DisplayConditionEffects()
    {
        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        UI.Label();

        using var scrollView =
            new GUILayout.ScrollViewScope(EffectPosition, UI.AutoWidth(), UI.AutoHeight());

        EffectPosition = scrollView.scrollPosition;

        foreach (var conditionEffect in EffectsContext.ConditionEffects)
        {
            using (UI.HorizontalScope())
            {
                UI.ActionButton("+".Bold().Red(), () =>
                    {
                        var source = gameLocationCharacterService.PartyCharacters[0];
                        var rulesetCharacter = source.RulesetCharacter;
                        var baseDefinition = conditionEffect.Value.First().Item2;

                        switch (baseDefinition)
                        {
                            case ConditionDefinition conditionDefinition:
                            {
                                DummyCondition.conditionStartParticleReference =
                                    conditionDefinition.conditionStartParticleReference;
                                DummyCondition.conditionParticleReference =
                                    conditionDefinition.conditionParticleReference;
                                DummyCondition.conditionEndParticleReference =
                                    conditionDefinition.conditionEndParticleReference;
                                break;
                            }
                            case SpellDefinition spellDefinition:
                            {
                                var effectParticleParameters =
                                    spellDefinition.EffectDescription.EffectParticleParameters;

                                DummyCondition.conditionStartParticleReference =
                                    effectParticleParameters.conditionStartParticleReference;
                                DummyCondition.conditionParticleReference =
                                    effectParticleParameters.conditionParticleReference;
                                DummyCondition.conditionEndParticleReference =
                                    effectParticleParameters.conditionEndParticleReference;
                                break;
                            }
                            case FeatureDefinitionPower featureDefinitionPower:
                            {
                                var effectParticleParameters =
                                    featureDefinitionPower.EffectDescription.EffectParticleParameters;

                                DummyCondition.conditionStartParticleReference =
                                    effectParticleParameters.conditionStartParticleReference;
                                DummyCondition.conditionParticleReference =
                                    effectParticleParameters.conditionParticleReference;
                                DummyCondition.conditionEndParticleReference =
                                    effectParticleParameters.conditionEndParticleReference;
                                break;
                            }
                        }

                        rulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                            AttributeDefinitions.TagEffect, DummyCondition.Name);
                        rulesetCharacter.InflictCondition(
                            DummyCondition.Name,
                            DummyCondition.DurationType,
                            DummyCondition.DurationParameter,
                            DummyCondition.TurnOccurence,
                            AttributeDefinitions.TagEffect,
                            rulesetCharacter.guid,
                            rulesetCharacter.CurrentFaction.Name,
                            1,
                            DummyCondition.Name,
                            0,
                            0,
                            0);
                    },
                    UI.Width((float)30));

                var label = conditionEffect.Key + " - " + conditionEffect.Value.First().Item1;

                UI.Label(label, UI.AutoWidth());
            }
        }
    }
}
