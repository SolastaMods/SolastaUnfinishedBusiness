using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ActiveCharacterPanelPatcher
{
    private static void OnCharacterPowerActivated(RulesetCharacter character, RulesetUsablePower power, int level)
    {
        character.RefreshAll();
    }

    [HarmonyPatch(typeof(ActiveCharacterPanel), nameof(ActiveCharacterPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ActiveCharacterPanel __instance)
        {
            if (__instance.GuiCharacter.RulesetCharacter is not { IsDeadOrDying: false })
            {
                return;
            }

            //PATCH: support for custom point pools and concentration powers on portrait
            IconsOnPortrait.CharacterPanelRefresh(__instance);

            //PATCH: support for button that shows info about non-Hero characters
            if (Main.Settings.ShowButtonWithControlledMonsterInfo &&
                __instance.GuiCharacter.RulesetCharacter is RulesetCharacterMonster)
            {
                CustomCharacterStatsPanel.Instance.Refresh();
            }
        }
    }

    [HarmonyPatch(typeof(ActiveCharacterPanel), nameof(ActiveCharacterPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ActiveCharacterPanel __instance)
        {
            //PATCH: properly update IconsOnPortrait
            var character = __instance.GuiCharacter.RulesetCharacter;

            if (character is { IsDeadOrDyingOrUnconscious: false } and not RulesetCharacterEffectProxy)
            {
                character.CharacterRefreshed += __instance.ConcentrationChanged;
                character.PowerActivated += OnCharacterPowerActivated;
            }

            //PATCH: support a better ratio with custom portraits
            if (Main.Settings.EnableCustomPortraits &&
                PortraitsContext.HasCustomPortrait(__instance.GuiCharacter.RulesetCharacter))
            {
                __instance.characterPortrait.rectTransform.sizeDelta = new Vector2(164, 247);
                __instance.characterPortrait.rectTransform.anchoredPosition = new Vector2(-48, 0);
            }
            else
            {
                __instance.characterPortrait.rectTransform.sizeDelta = new Vector2(212, 247);
                __instance.characterPortrait.rectTransform.anchoredPosition = new Vector2(0, 0);
            }

            //PATCH: support for button that shows info about non-Hero characters
            if (!Main.Settings.ShowButtonWithControlledMonsterInfo
                || __instance.GuiCharacter.RulesetCharacter is not RulesetCharacterMonster)
            {
                return;
            }

            CustomCharacterStatsPanel.Instance.Bind(__instance.GuiCharacter.RulesetCharacter);
        }
    }

    [HarmonyPatch(typeof(ActiveCharacterPanel), nameof(ActiveCharacterPanel.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ActiveCharacterPanel __instance)
        {
            //PATCH: properly update IconsOnPortrait
            var character = __instance.GuiCharacter.RulesetCharacter;

            // ReSharper disable once InvertIf
            if (character is { IsDeadOrDyingOrUnconscious: false } and not RulesetCharacterEffectProxy)
            {
                character.CharacterRefreshed -= __instance.ConcentrationChanged;
                character.PowerActivated -= OnCharacterPowerActivated;
            }
        }

        [UsedImplicitly]
        public static void Postfix()
        {
            //PATCH: support for button that shows info about non-Hero characters
            if (Main.Settings.ShowButtonWithControlledMonsterInfo)
            {
                CustomCharacterStatsPanel.Instance.Unbind();
            }
        }
    }
}
