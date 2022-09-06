// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using HarmonyLib;
// using SolastaCommunityExpansion.Models;
// using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
//
// namespace SolastaCommunityExpansion.Patches.CustomFeatures.SharedSpells;
//
// // handles Sorcerer wildshape scenarios / enforces sorcerer class level / correctly handle slots recovery scenarios
// [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySpellSlotsForm")]
// [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
// internal static class RulesetImplementationManager_ApplySpellSlotsForm
// {
//     internal static bool Prefix(EffectForm effectForm,
//         RulesetImplementationDefinitions.ApplyFormsParams formsParams)
//     {
//         var originalHero = formsParams.sourceCharacter as RulesetCharacterHero;
//         var substituteHero =
//             originalHero ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;
//
//         // this shouldn't happen so passing the problem back to original game code
//         if (substituteHero == null)
//         {
//             return true;
//         }
//
//         // patch is only required for Wildshape Heroes or Multiclassed ones
//         if (originalHero != null && !SharedSpellsContext.IsMulticaster(originalHero))
//         {
//             return true;
//         }
//
//         var spellSlotsForm = effectForm.SpellSlotsForm;
//
//         switch (spellSlotsForm.Type)
//         {
//             case SpellSlotsForm.EffectType.RecoverHalfLevelUp
//                 when SharedSpellsContext.RecoverySlots.TryGetValue(formsParams.activeEffect.Name,
//                     out var invokerClass) && invokerClass is CharacterClassDefinition characterClassDefinition:
//             {
//                 foreach (var spellRepertoire in substituteHero.SpellRepertoires)
//                 {
//                     var currentValue = 0;
//
//                     if (spellRepertoire.SpellCastingClass == characterClassDefinition)
//                     {
//                         currentValue = substituteHero.ClassesAndLevels[characterClassDefinition];
//                     }
//                     else if (spellRepertoire.SpellCastingSubclass != null)
//                     {
//                         var characterClass = substituteHero.ClassesAndSubclasses
//                             .FirstOrDefault(x => x.Value == spellRepertoire.SpellCastingSubclass).Key;
//
//                         if (characterClass == characterClassDefinition)
//                         {
//                             currentValue = substituteHero.ClassesAndLevels[characterClassDefinition];
//                         }
//                     }
//
//                     if (currentValue <= 0)
//                     {
//                         continue;
//                     }
//
//                     var slotsCapital = (currentValue % 2) + (currentValue / 2);
//
//                     Gui.GuiService.GetScreen<SlotRecoveryModal>()
//                         .ShowSlotRecovery(substituteHero, formsParams.activeEffect.SourceDefinition.Name,
//                             spellRepertoire, slotsCapital, spellSlotsForm.MaxSlotLevel);
//
//                     break;
//                 }
//
//                 break;
//             }
//             //
//             // handles Sorcerer and Wildshape scenarios slots / points scenarios
//             //
//             case SpellSlotsForm.EffectType.CreateSpellSlot or SpellSlotsForm.EffectType.CreateSorceryPoints:
//             {
//                 var spellRepertoire = substituteHero.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);
//
//                 Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(substituteHero, spellRepertoire,
//                     spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
//                 break;
//             }
//             case SpellSlotsForm.EffectType.GainSorceryPoints:
//                 formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
//                 break;
//             case SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp:
//             {
//                 var currentValue = substituteHero.ClassesAndLevels[Sorcerer];
//                 var sorceryPointsGain = (currentValue % 2) + (currentValue / 2);
//
//                 formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
//                 break;
//             }
//             case SpellSlotsForm.EffectType.RechargePower when formsParams.targetCharacter is RulesetCharacter:
//             {
//                 foreach (var usablePower in substituteHero.UsablePowers.Where(usablePower =>
//                              usablePower.PowerDefinition == spellSlotsForm.PowerDefinition))
//                 {
//                     usablePower.Recharge();
//                 }
//
//                 break;
//             }
//         }
//
//         return false;
//     }
// }
