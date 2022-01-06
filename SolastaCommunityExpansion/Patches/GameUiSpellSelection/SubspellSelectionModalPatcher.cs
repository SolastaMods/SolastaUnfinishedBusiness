using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi;

namespace SolastaCommunityExpansion.Patches.GameUiSpellSelection
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_Bind
    {
        public static int? FilterBySlotLevel { get; private set; }

        public static void Prefix(SpellDefinition masterSpell, int slotLevel)
        {
            FilterBySlotLevel = 
                masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureElemental.Name
                || masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureFey.Name
                ? slotLevel
                : (int?)null;
        }

        public static void Postfix()
        {

            FilterBySlotLevel = null;
        }
    }

    [HarmonyPatch(typeof(CharacterEditionScreen), "SubspellsList", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellDefinition_SubspellsList
    {
        public static void Postfix(ref List<SpellDefinition> __result)
        {
            if (!SubspellSelectionModal_Bind.FilterBySlotLevel.HasValue)
            {
                return;
            }

            __result = __result
                .Where(s =>
                {
                    var monsterName = s.EffectDescription
                        .GetFirstFormOfType(EffectForm.EffectFormType.Summon)
                        .SummonForm
                        .MonsterDefinitionName;

                    return DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(monsterName, out var monsterDefinition)
                        && monsterDefinition.ChallengeRating <= SubspellSelectionModal_Bind.FilterBySlotLevel.Value;
                })
                .ToList();
        }
    }
}
