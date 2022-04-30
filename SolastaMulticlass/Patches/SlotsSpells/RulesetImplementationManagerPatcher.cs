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
                var originalHero = formsParams.sourceCharacter as RulesetCharacterHero;
                var substituteHero = originalHero ?? formsParams.sourceCharacter.OriginalFormCharacter as RulesetCharacterHero;

                // this shouldn't happen so passing the problem back to original game code
                if (substituteHero == null)
                {
                    return true;
                }

                // patch is only required for Wildshape Heroes or Multiclassed ones
                if (originalHero != null && !SharedSpellsContext.IsMulticaster(originalHero))
                {
                    return true;
                }

                var spellSlotsForm = effectForm.SpellSlotsForm;

                if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecoverHalfLevelUp
                    && SharedSpellsContext.RecoverySlots.TryGetValue(formsParams.activeEffect.Name, out var invokerClass)
                    && invokerClass is CharacterClassDefinition characterClassDefinition)
                {
                    foreach (var spellRepertoire in substituteHero.SpellRepertoires)
                    {
                        var currentValue = 0;

                        if (spellRepertoire.SpellCastingClass == characterClassDefinition)
                        {
                            currentValue = substituteHero.ClassesAndLevels[characterClassDefinition];
                        }
                        else if (spellRepertoire.SpellCastingSubclass != null)
                        {
                            var characterClass = substituteHero.ClassesAndSubclasses
                                .FirstOrDefault(x => x.Value == spellRepertoire.SpellCastingSubclass).Key;

                            if (characterClass == characterClassDefinition)
                            {
                                currentValue = substituteHero.ClassesAndLevels[characterClassDefinition];
                            }
                        }

                        if (currentValue > 0)
                        {
                            var slotsCapital = currentValue % 2 + currentValue / 2;

                            Gui.GuiService.GetScreen<SlotRecoveryModal>()
                                .ShowSlotRecovery(substituteHero, formsParams.activeEffect.SourceDefinition.Name, spellRepertoire, slotsCapital, spellSlotsForm.MaxSlotLevel);

                            break;
                        }
                    }
                }

                //
                // handles Sorcerer and Wildshape scenarios slots / points scenarios
                //

                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot || spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSorceryPoints)
                {
                    var spellRepertoire = substituteHero.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(substituteHero, spellRepertoire, spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.GainSorceryPoints)
                {
                    var spellRepertoire = substituteHero.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp)
                {
                    var spellRepertoire = substituteHero.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);
                    var currentValue = substituteHero.ClassesAndLevels[Sorcerer];
                    var sorceryPointsGain = currentValue % 2 + currentValue / 2;

                    formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RechargePower && formsParams.targetCharacter is RulesetCharacter)
                {
                    foreach (RulesetUsablePower usablePower in substituteHero.UsablePowers)
                    {
                        if (usablePower.PowerDefinition == spellSlotsForm.PowerDefinition)
                        {
                            usablePower.Recharge();
                        }
                    }
                }

                return false;
            }
        }
    }
}
