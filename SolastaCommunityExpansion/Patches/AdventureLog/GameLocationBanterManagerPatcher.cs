using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Patches.AdventureLog
{
    [HarmonyPatch(typeof(GameLocationBanterManager), "ForceBanterLine")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBanterManager_ForceBanterLine
    {
        internal static void Postfix(string line, GameLocationCharacter speaker)
        {
            if (Main.Settings.EnableAdventureLogBanterLines && speaker.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster)
            {
                AssetReferenceSprite assetReferenceSprite = null;

                if (rulesetCharacterMonster.HumanoidMonsterPresentationDefinition != null)
                {
                    assetReferenceSprite = rulesetCharacterMonster.HumanoidMonsterPresentationDefinition.GuiPresentation.SpriteReference;
                }
                else if (rulesetCharacterMonster.MonsterDefinition != null)
                {
                    assetReferenceSprite = rulesetCharacterMonster.MonsterDefinition.GuiPresentation.SpriteReference;
                }

                Models.AdventureLogContext.LogEntry(string.Empty, line, rulesetCharacterMonster.Name, assetReferenceSprite);
            }
        }
    }
}
