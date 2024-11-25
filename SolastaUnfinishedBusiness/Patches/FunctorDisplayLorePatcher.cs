using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FunctorDisplayLorePatcher
{
    [HarmonyPatch(typeof(FunctorDisplayLore), nameof(FunctorDisplayLore.Execute))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectCharacters_Patch
    {
        private static IEnumerator Execute(
            FunctorDisplayLore __instance, FunctorParametersDescription functorParameters)
        {
            var gamePartyStatusScreen = Gui.GuiService.GetScreen<GamePartyStatusScreen>();
            var stringParameter = functorParameters.StringParameter;
            var boolParameter = functorParameters.BoolParameter;
            var floatParameter = functorParameters.FloatParameter;
            var intParameter = (GadgetBlueprintDefinitions.LoreFormat)functorParameters.IntParameter;

            switch (intParameter)
            {
                case GadgetBlueprintDefinitions.LoreFormat.MainCharacter:
                case GadgetBlueprintDefinitions.LoreFormat.RandomCharacter:
                    __instance.validCharacters.Clear();
                    foreach (var partyCharacter in ServiceRepository.GetService<IGameLocationCharacterService>()
                                 .PartyCharacters
                                 .Where(partyCharacter => !partyCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious))
                    {
                        __instance.validCharacters.Add(partyCharacter);
                    }

                    if (__instance.validCharacters.Empty())
                    {
                        break;
                    }

                    var speaker = intParameter == GadgetBlueprintDefinitions.LoreFormat.MainCharacter
                        ? __instance.validCharacters[0]
                        : __instance.validCharacters[DeterministicRandom.Range(0, __instance.validCharacters.Count)];
                    ServiceRepository.GetService<IGameLocationBanterService>()
                        .ForceBanterLine(stringParameter, speaker);
                    __instance.validCharacters.Clear();
                    break;
                case GadgetBlueprintDefinitions.LoreFormat.GadgetFeedback:
                    Gui.Game.GameCampaign.AdventureLog.RecordLoreTextEntry(stringParameter);

                    Gui.GuiService.ShowTextFeedbackVector3(functorParameters.SourceGadget.TextFeedbackPosition,
                        stringParameter, "DEDCDC", boolParameter ? floatParameter : 0.0f);

                    Gui.Game.GameConsole.LogSimpleLine(stringParameter);
                    break;
                case GadgetBlueprintDefinitions.LoreFormat.TopHeader:
                    if (!gamePartyStatusScreen.Visible)
                    {
                        break;
                    }

                    // before to allow time to create the speech as this could be timed
                    SpeechContext.Speak(stringParameter);
                    gamePartyStatusScreen.ShowHeaderPopup(stringParameter, boolParameter ? floatParameter : 0.0f);
                    break;
                case GadgetBlueprintDefinitions.LoreFormat.TimedPanel:
                    Gui.Game.GameCampaign.AdventureLog.RecordLoreTextEntry(stringParameter);

                    if (!gamePartyStatusScreen.Visible)
                    {
                        break;
                    }

                    // before to allow time to create the speech as this could be timed
                    SpeechContext.Speak(stringParameter);
                    gamePartyStatusScreen.ShowBottomPopup(stringParameter, boolParameter ? floatParameter : 0.0f);
                    break;
                case GadgetBlueprintDefinitions.LoreFormat.ModalPanel:
                    Gui.Game.GameCampaign.AdventureLog.RecordLoreTextEntry(stringParameter);

                    var posterScreen = Gui.GuiService.GetScreen<PosterScreen>();

                    GuiDefinitions.ExtractCaptionsParagraphs(Gui.Localize(stringParameter), __instance.textParagraphs);

                    posterScreen.Show(__instance.textParagraphs);
                    SpeechContext.Speak(stringParameter);

                    __instance.textParagraphs.Clear();
                    break;
                default:
                    yield break;
            }
        }

        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            FunctorDisplayLore __instance,
            FunctorParametersDescription functorParameters)
        {
            __result = Execute(__instance, functorParameters);

            return false;
        }
    }
}
