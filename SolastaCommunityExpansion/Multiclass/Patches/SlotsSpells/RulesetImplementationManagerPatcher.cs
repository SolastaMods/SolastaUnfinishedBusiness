using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion;
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
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                //
                // handles slots recovery scenarios on short rest
                //

                var spellSlotsForm = effectForm.SpellSlotsForm;
                var invokerClass = formsParams.activeEffect.Name switch
                {
                    "TinkererSpellStoringItem" => IntegrationContext.TinkererClass,
                    "ArtificerInfusionSpellRefuelingRing" => IntegrationContext.TinkererClass,
                    "PowerAlchemistSpellBonusRecovery" => IntegrationContext.TinkererClass,
                    "PowerWizardArcaneRecovery" => Wizard,
                    "PowerCircleLandNaturalRecovery" => Druid,
                    "PowerSpellMasterBonusRecovery" => Wizard,
                    _ => null,
                };

                if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecoverHalfLevelUp && invokerClass != null)
                {
                    var sourceCharacter = formsParams.sourceCharacter as RulesetCharacterHero;

                    foreach (var spellRepertoire in sourceCharacter.SpellRepertoires)
                    {
                        var currentValue = 0;

                        if (spellRepertoire.SpellCastingClass == invokerClass)
                        {
                            currentValue = sourceCharacter.ClassesAndLevels[invokerClass];
                        }
                        else if (spellRepertoire.SpellCastingSubclass != null)
                        {
                            var characterClass = sourceCharacter.ClassesAndSubclasses.FirstOrDefault(x => x.Value == spellRepertoire.SpellCastingSubclass).Key;

                            if (characterClass == invokerClass)
                            {
                                currentValue = sourceCharacter.ClassesAndLevels[invokerClass];
                            }
                        }

                        if (currentValue > 0)
                        {
                            var slotsCapital = currentValue % 2 == 0 ? currentValue / 2 : (currentValue + 1) / 2;

                            Gui.GuiService.GetScreen<SlotRecoveryModal>().ShowSlotRecovery(sourceCharacter, formsParams.activeEffect.SourceDefinition.Name, spellRepertoire, slotsCapital, spellSlotsForm.MaxSlotLevel);
                            break;
                        }
                    }
                }

                //
                // handles Sorcerer and Wildshape scenarios slots / points scenarios
                //

                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot || spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSorceryPoints)
                {
                    var heroWithSpellRepertoire = WildshapeContext.GetHero(formsParams.sourceCharacter) as RulesetCharacterHero;
                    var spellRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(formsParams.sourceCharacter, spellRepertoire, spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.GainSorceryPoints)
                {
                    var heroWithSpellRepertoire = WildshapeContext.GetHero(formsParams.sourceCharacter) as RulesetCharacterHero;
                    var spellRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp)
                {
                    var heroWithSpellRepertoire = WildshapeContext.GetHero(formsParams.sourceCharacter) as RulesetCharacterHero;
                    var spellRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    var currentValue = heroWithSpellRepertoire.ClassesAndLevels[Sorcerer];
                    var sorceryPointsGain = currentValue % 2 == 0 ? currentValue / 2 : (currentValue + 1) / 2;

                    formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                }

                return false;
            }
        }
    }
}
