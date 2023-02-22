using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ItemMenuModalPatcher
{
    [HarmonyPatch(typeof(ItemMenuModal), nameof(ItemMenuModal.SetupFromItem))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetupFromItem_Patch
    {
#if false
        private static bool IsSpellDefinitionOnRepertoire(
            RulesetActor rulesetActor,
            RulesetSpellRepertoire spellRepertoire,
            SpellDefinition spellDefinition)
        {
            if (spellRepertoire.SpellCastingFeature.HasAccessToSpell(spellDefinition) ||
                spellRepertoire.AutoPreparedSpells.Contains(spellDefinition) ||
                spellRepertoire.IsSpellDefinitionInExtraSpells(spellDefinition))
            {
                return true;
            }

            // only exceptional case a Wizard in game can get additional spells
            return rulesetActor.HasAnyFeature(MagicAffinityGreenmageGreenMagicList) &&
                   MagicAffinityGreenmageGreenMagicList.ExtendedSpellList.ContainsSpell(spellDefinition);
        }
#endif

        //PATCH: allows mark deity to work with MC heroes (Multiclass)
        private static bool RequiresDeity(ItemMenuModal itemMenuModal)
        {
            return itemMenuModal.GuiCharacter.RulesetCharacterHero.ClassesHistory.Exists(x => x.RequiresDeity);
        }

        //PATCH: only allow to scribe spells the scriber class can do
        private static List<RulesetSpellRepertoire> SpellRepertoires(
            RulesetCharacterHero rulesetCharacterHero,
            GuiEquipmentItem guiEquipmentItem)
        {
            if (guiEquipmentItem.EquipementItem is not RulesetItemDevice rulesetItemDevice ||
                rulesetItemDevice.UsableFunctions[0] is not { })
            {
                return rulesetCharacterHero.SpellRepertoires;
            }

            return rulesetCharacterHero.SpellRepertoires
                .Where(x => x.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.Spellbook)
                .ToList();
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var requiresDeityMethod = typeof(CharacterClassDefinition).GetMethod("get_RequiresDeity");
            var myRequiresDeityMethod = new Func<ItemMenuModal, bool>(RequiresDeity).Method;

            var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
            var mySpellRepertoiresMethod =
                new Func<RulesetCharacterHero, GuiEquipmentItem, List<RulesetSpellRepertoire>>(SpellRepertoires).Method;

            return instructions
                .ReplaceCalls(requiresDeityMethod, "ItemMenuModal.SetupFromItem1",
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, myRequiresDeityMethod))
                .ReplaceCalls(spellRepertoiresMethod, "ItemMenuModal.SetupFromItem2",
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, mySpellRepertoiresMethod));
        }
    }
}
