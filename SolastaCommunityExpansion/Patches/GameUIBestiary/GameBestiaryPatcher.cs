using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUIBestiary
{
    /// <summary>
    /// Fix issue: GameBestiaryEntry.LastUpdateTimeCode is never set. 
    /// Should be set when increasing knowledge level.
    /// </summary>
    [HarmonyPatch(typeof(GameBestiary), "ApplyKnowledgeLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameBestiary_ApplyKnowledgeLevel
    {
        public static void Postfix(MonsterDefinition monsterDefinition, GameTime gameTime,
            Dictionary<MonsterDefinition, GameBestiaryEntry> ___entriesByMonster, bool __result)
        {
            if (__result && ___entriesByMonster.TryGetValue(monsterDefinition, out var gameBestiaryEntry))
            {
                gameBestiaryEntry.LastUpdateTimeCode = gameTime.GetTimeCode();
            }
        }
    }


    /// <summary>
    /// Fix issue: 
    /// sorting should put all monsters with knowledge level = 0 at the end of the list
    /// sorting should sort by category and then by name
    /// </summary>
    [HarmonyPatch(typeof(GameBestiary), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameBestiary_Compare
    {
        public static void Postfix(GameRecordEntry left, GameRecordEntry right,
            int ___sortSign, BestiaryDefinitions.SortCategory ___sortCategory, ref int __result)
        {
            if (left == null)
                return;
            if (right == null)
                return;
            var gameBestiaryEntry1 = left as GameBestiaryEntry;
            var gameBestiaryEntry2 = right as GameBestiaryEntry;
            var monsterDefinition1 = gameBestiaryEntry1.MonsterDefinition;
            var monsterDefinition2 = gameBestiaryEntry2.MonsterDefinition;
            if (monsterDefinition1 == null)
                return;
            if (monsterDefinition2 == null)
                return;

            Main.Log($"{gameBestiaryEntry1.LastUpdateTimeCode}");

            switch (___sortCategory)
            {
                case BestiaryDefinitions.SortCategory.Alphabetical:
                    break;
                case BestiaryDefinitions.SortCategory.UpdateDate:
                case BestiaryDefinitions.SortCategory.KnowledgeLevel:
                case BestiaryDefinitions.SortCategory.Size:
                case BestiaryDefinitions.SortCategory.ChallengeRating:
                    __result = __result == 0 ? -___sortSign * SortByName() : __result;
                    break;
            }

            int SortByKnowledgeLevel()
            {
                // TODO: make kl=0 always last regardless of sort order
                return -___sortSign * gameBestiaryEntry1.KnowledgeLevelDefinition.Level.CompareTo(gameBestiaryEntry2.KnowledgeLevelDefinition.Level);
            }

            int SortByName()
            {
                if (string.IsNullOrEmpty(monsterDefinition1.GuiPresentation.Title))
                    return 1;
                if (string.IsNullOrEmpty(monsterDefinition2.GuiPresentation.Title))
                    return -1;
                var str = Gui.Localize(monsterDefinition1.GuiPresentation.Title);
                var strB = Gui.Localize(monsterDefinition2.GuiPresentation.Title);
                if (string.IsNullOrEmpty(str))
                    return 1;
                if (string.IsNullOrEmpty(strB))
                    return -1;
                return ___sortSign * str.CompareTo(strB);
            }
        }
    }

}
