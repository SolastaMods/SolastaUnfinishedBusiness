using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle;

[HarmonyPatch(typeof(AfterRestActionItem), "OnExecuteCb")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class AfterRestActionItem_OnExecuteCb
{
    internal static bool Prefix(AfterRestActionItem __instance)
    {
        if (__instance.executing)
        {
            return true;
        }

        var activity = __instance.RestActivityDefinition;

        if (activity.Functor != PowerBundleContext.UseCustomRestPowerFunctorName || activity.StringParameter == null)
        {
            return true;
        }

        var masterPower = PowerBundleContext.GetPower(activity.StringParameter);

        if (!masterPower)
        {
            return true;
        }

        var masterSpell = PowerBundleContext.GetSpell(masterPower);
        var repertoire = new RulesetSpellRepertoire();
        var subspellSelectionModalScreen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
        var handler = new SpellsByLevelBox.SpellCastEngagedHandler(
            (spellRepertoire, spell, slotLevel) => PowerEngagedHandler(__instance, spell));

        repertoire.KnownSpells.AddRange(masterSpell.SubspellsList);

        subspellSelectionModalScreen.Bind(masterSpell, __instance.Hero, repertoire, handler, 0,
            __instance.RectTransform);
        subspellSelectionModalScreen.Show();

        return false;
    }

    private static void PowerEngagedHandler(AfterRestActionItem item, SpellDefinition spell)
    {
        item.button.interactable = false;

        var power = PowerBundleContext.GetPower(spell).Name;

        ServiceRepository.GetService<IGameRestingService>().ExecuteAsync(ExecuteAsync(item, power), power);
    }

    private static IEnumerator ExecuteAsync(AfterRestActionItem item, string powerName)
    {
        item.executing = true;

        var parameters = new FunctorParametersDescription {RestingHero = item.Hero, StringParameter = powerName};
        var gameRestingService = ServiceRepository.GetService<IGameRestingService>();

        yield return ServiceRepository.GetService<IFunctorService>()
            .ExecuteFunctorAsync(item.RestActivityDefinition.Functor, parameters, gameRestingService);

        yield return null;

        var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        if (gameLocationActionService != null && gameLocationCharacterService != null)
        {
            bool needsToWait;

            do
            {
                needsToWait = gameLocationCharacterService.PartyCharacters
                    .Any(partyCharacter => gameLocationActionService.IsCharacterActing(partyCharacter));

                if (needsToWait)
                {
                    yield return null;
                }
            } while (needsToWait);
        }

        item.AfterRestActionTaken?.Invoke();
        item.executing = false;

        var button = item.button;

        if (button != null)
        {
            button.interactable = true;
        }
    }
}
