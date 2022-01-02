using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix
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
            if (!Main.Settings.BugFixBestiarySorting)
            {
                return;
            }

            if (__result && ___entriesByMonster.TryGetValue(monsterDefinition, out var gameBestiaryEntry))
            {
                gameBestiaryEntry.LastUpdateTimeCode = gameTime.GetTimeCode();
            }
        }
    }


    /// <summary>
    /// Fix issue: 
    /// sorting should put all monsters with knowledge level = 0 at the end of the list since they have no useful info
    /// sorting should sort by category and then by name to make it less of a jumble
    /// </summary>
    [HarmonyPatch(typeof(GameBestiary), "Compare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameBestiary_Compare
    {
        public static bool Prefix(GameRecordEntry left, GameRecordEntry right,
            int ___sortSign, BestiaryDefinitions.SortCategory ___sortCategory, ref int __result)
        {
            if (!Main.Settings.BugFixBestiarySorting)
            {
                return true;
            }

            if (left == null)
            {
                __result = 1;
                return false;
            }

            if (right == null)
            {
                __result = -1;
                return false;
            }

            GameBestiaryEntry gameBestiaryEntry1 = left as GameBestiaryEntry;
            GameBestiaryEntry gameBestiaryEntry2 = right as GameBestiaryEntry;
            MonsterDefinition monsterDefinition1 = gameBestiaryEntry1.MonsterDefinition;
            MonsterDefinition monsterDefinition2 = gameBestiaryEntry2.MonsterDefinition;

            if (monsterDefinition1 == null)
            {
                __result = 1;
                return false;
            }

            if (monsterDefinition2 == null)
            {
                __result = -1;
                return false;
            }

            var level1 = gameBestiaryEntry1.KnowledgeLevelDefinition.Level;
            var level2 = gameBestiaryEntry2.KnowledgeLevelDefinition.Level;

            __result = SplitByUnknownVsKnown();

            switch (___sortCategory)
            {
                case BestiaryDefinitions.SortCategory.Alphabetical:
                    __result = __result == 0 ? SortByName(___sortSign) : __result;
                    break;
                case BestiaryDefinitions.SortCategory.UpdateDate:
                    __result = __result == 0 ? SortByUpdateDate() : __result;
                    __result = __result == 0 ? SortByName() : __result;
                    break;
                case BestiaryDefinitions.SortCategory.KnowledgeLevel:
                    __result = __result == 0 ? SortByKnowledgeLevel() : __result;
                    __result = __result == 0 ? SortByName() : __result;
                    break;
                case BestiaryDefinitions.SortCategory.Size:
                    __result = __result == 0 ? SortBySize() : __result;
                    __result = __result == 0 ? SortByName() : __result;
                    break;
                case BestiaryDefinitions.SortCategory.ChallengeRating:
                    __result = __result == 0 ? -___sortSign * monsterDefinition1.ChallengeRating.CompareTo(monsterDefinition2.ChallengeRating) : __result;
                    __result = __result == 0 ? SortByName() : __result;
                    break;
            }

            return false;

            int SplitByUnknownVsKnown()
            {
                if (level1 == 0) { return 1; }
                if (level2 == 0) { return -1; }
                return 0;
            }

            int SortBySize()
            {
                return -___sortSign * monsterDefinition1.SizeDefinition.GuiPresentation.SortOrder.CompareTo(monsterDefinition2.SizeDefinition.GuiPresentation.SortOrder);
            }

            int SortByUpdateDate()
            {
                return -___sortSign * gameBestiaryEntry1.LastUpdateTimeCode.CompareTo(gameBestiaryEntry2.LastUpdateTimeCode);
            }

            int SortByKnowledgeLevel()
            {
                return -___sortSign * level1.CompareTo(level2);
            }

            int SortByName(int sortSign = 1)
            {
                if (string.IsNullOrEmpty(monsterDefinition1.GuiPresentation.Title))
                {
                    return 1;
                }

                if (string.IsNullOrEmpty(monsterDefinition2.GuiPresentation.Title))
                {
                    return -1;
                }

                var str = Gui.Localize(monsterDefinition1.GuiPresentation.Title);
                var strB = Gui.Localize(monsterDefinition2.GuiPresentation.Title);
                if (string.IsNullOrEmpty(str))
                {
                    return 1;
                }

                if (string.IsNullOrEmpty(strB))
                {
                    return -1;
                }

                return sortSign * str.CompareTo(strB);
            }
        }
    }
}
