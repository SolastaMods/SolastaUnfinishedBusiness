using System.Linq;
using HarmonyLib;
using SolastaMulticlass.Models;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Patches.SlotsSpells
{
    internal static class RulesetImplementationManagerPatcher
    {
        // handles Sorcerer wildshape scenarios / enforces sorcerer class level / correctly handle slots recovery scenarios
        [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySpellSlotsForm")]
        internal static class RulesetImplementationManagerApplySpellSlotsForm
        {
            internal static bool Prefix(EffectForm effectForm, RulesetImplementationDefinitions.ApplyFormsParams formsParams)
            {    
                var spellSlotsForm = effectForm.SpellSlotsForm;

                if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecoverHalfLevelUp
                    && SharedSpellsContext.RecoverySlots.TryGetValue(formsParams.activeEffect.Name, out var invokerClass)
                    && invokerClass is CharacterClassDefinition characterClassDefinition)
                {
                    var heroWithSpellRepertoire = formsParams.sourceCharacter as RulesetCharacterHero
                        ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;
                      
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                    {
                        var currentValue = 0;

                        if (spellRepertoire.SpellCastingClass == characterClassDefinition)
                        {
                            currentValue = heroWithSpellRepertoire.ClassesAndLevels[characterClassDefinition];
                        }
                        else if (spellRepertoire.SpellCastingSubclass != null)
                        {
                            var characterClass = heroWithSpellRepertoire.ClassesAndSubclasses
                                .FirstOrDefault(x => x.Value == spellRepertoire.SpellCastingSubclass).Key;

                            if (characterClass == characterClassDefinition)
                            {
                                currentValue = heroWithSpellRepertoire.ClassesAndLevels[characterClassDefinition];
                            }
                        }

                        if (currentValue > 0)
                        {
                            var slotsCapital = currentValue % 2 + currentValue / 2;

                            Gui.GuiService.GetScreen<SlotRecoveryModal>()
                                .ShowSlotRecovery(heroWithSpellRepertoire, formsParams.activeEffect.SourceDefinition.Name, spellRepertoire, slotsCapital, spellSlotsForm.MaxSlotLevel);

                            break;
                        }
                    }
                }

                //
                // handles Sorcerer and Wildshape scenarios slots / points scenarios
                //

                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot || spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSorceryPoints)
                {
                    var heroWithSpellRepertoire = formsParams.sourceCharacter as RulesetCharacterHero
                        ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;
                    var spellRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(heroWithSpellRepertoire, spellRepertoire, spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.GainSorceryPoints)
                {
                    var heroWithSpellRepertoire = formsParams.sourceCharacter as RulesetCharacterHero
                        ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;
                    var spellRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp)
                {
                    var heroWithSpellRepertoire = formsParams.sourceCharacter as RulesetCharacterHero
                        ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;
                    var spellRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    var currentValue = heroWithSpellRepertoire.ClassesAndLevels[Sorcerer];
                    var sorceryPointsGain = currentValue % 2 + currentValue / 2;

                    formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                }

                return false;
            }
        }
    }
}
