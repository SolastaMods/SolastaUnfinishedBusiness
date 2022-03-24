using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells
{
    internal static class RulesetImplementationManagerPatcher
    {
        internal static RulesetCharacterHero HeroWithSpellRepertoire { get; set; }
        internal static RulesetSpellRepertoire SpellRepertoire { get; set; }

        // gets hero and repertoire context to be used later on FlexibleCastingItem / ensures Wizard / Sorcerer are using caster level
        [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySpellSlotsForm")]
        internal static class RulesetImplementationManagerApplySpellSlotsForm
        {
            internal static bool Prefix(EffectForm effectForm, RulesetImplementationDefinitions.ApplyFormsParams formsParams)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var spellSlotsForm = effectForm.SpellSlotsForm;
                var invokerClass = formsParams.activeEffect.Name switch
                {
                    "TinkererSpellStoringItem" => Models.IntegrationContext.TinkererClass,
                    "ArtificerInfusionSpellRefuelingRing" => Models.IntegrationContext.TinkererClass,
                    "PowerAlchemistSpellBonusRecovery" => Models.IntegrationContext.TinkererClass,
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
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot || spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSorceryPoints)
                {
                    HeroWithSpellRepertoire = WildshapeContext.GetHero(formsParams.sourceCharacter) as RulesetCharacterHero;
                    SpellRepertoire = HeroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(formsParams.sourceCharacter, SpellRepertoire, spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.GainSorceryPoints)
                {
                    HeroWithSpellRepertoire = WildshapeContext.GetHero(formsParams.sourceCharacter) as RulesetCharacterHero;
                    SpellRepertoire = HeroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    formsParams.sourceCharacter.GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp)
                {
                    HeroWithSpellRepertoire = WildshapeContext.GetHero(formsParams.sourceCharacter) as RulesetCharacterHero;
                    SpellRepertoire = HeroWithSpellRepertoire.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);

                    var currentValue = HeroWithSpellRepertoire.ClassesAndLevels[Sorcerer];
                    var sorceryPointsGain = currentValue % 2 == 0 ? currentValue / 2 : (currentValue + 1) / 2;

                    formsParams.sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                }

                return false;
            }
        }
    }
}
