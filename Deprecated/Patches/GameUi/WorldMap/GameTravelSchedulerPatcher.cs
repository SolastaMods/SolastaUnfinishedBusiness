// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using HarmonyLib;
// using static SolastaCommunityExpansion.Api.DatabaseHelper;
// using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
//
// namespace SolastaCommunityExpansion.Patches.GameUi.WorldMap;
//
// [HarmonyPatch(typeof(GameTravelScheduler), "ExecuteLongRestComplete")]
// [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
// internal static class GameTravelScheduler_ExecuteLongRestComplete
// {
//     internal static void Prefix(GameTravelScheduler __instance, int hour)
//     {
//         var campaignCharacterList = new List<GameCampaignCharacter>();
//
//         campaignCharacterList.AddRange(__instance.partyCharacters);
//         campaignCharacterList.AddRange(__instance.guestCharacters);
//
//         foreach (GameCampaignCharacter campaignCharacter2 in campaignCharacterList)
//         {
//             if (campaignCharacter2.RulesetCharacter.CanCastSpell(MageArmor, true, out var spellRepertoire)
//                 && campaignCharacter2.RulesetCharacter.AreSpellComponentsValid(MageArmor))
//             {
//                 var entry = new GameTravelJournalEntry(
//                     __instance.travelJournal.TravelJournalDefinition,
//                     hour,
//                     TravelEventDefinitions.TravelEventCastFoodSpell,
//                     __instance.RollRandomLine(new List<string>
//                     {
//                         "{0} casts the {1} spell."
//                     }),
//                     GameTravelJournalEntry.EventOutcome.Positive);
//
//                 entry.AddParameter(TravelStyleDuplet.ParameterType.Hero, campaignCharacter2.Name);
//                 entry.AddParameter(TravelStyleDuplet.ParameterType.SpellPowerItem, Gui.Localize(MageArmor.GuiPresentation.Title));
//                 __instance.travelJournal.AddEntry(entry, -1);
//
//                 var rulesetEffectSpell = new RulesetEffectSpell(campaignCharacter2.RulesetCharacter, spellRepertoire, MageArmor, 1);
//                 var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
//                 var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
//
//                 applyFormsParams.FillSourceAndTarget(campaignCharacter2.RulesetCharacter,
//                     campaignCharacter2.RulesetCharacter);
//                 applyFormsParams.FillFromActiveEffect(rulesetEffectSpell);
//                 campaignCharacter2.CastSpell(MageArmor, spellRepertoire);
//                 rulesetImplementationService.ApplyEffectForms(MageArmor.EffectDescription.EffectForms,
//                     applyFormsParams);
//             }
//         }
//     }
// }
//
//



