using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class HitDiceRestItemPatcher
{
    //PATCH: correctly mark the consumed die type (MULTICLASS)
    [HarmonyPatch(typeof(HitDiceRestItem), nameof(HitDiceRestItem.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(HitDiceRestItem __instance)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return true;
            }

            var maxHitDiceCount = __instance.character.MaxHitDiceCount();
            var remainingHitDiceCount = __instance.character.RemainingHitDiceCount();
            var restHealingBonusDiceCount = 0;

            if (remainingHitDiceCount > 0)
            {
                restHealingBonusDiceCount += __instance.restHealingBonuses
                    .Count(restHealingBonus => restHealingBonus.Die != DieType.D1);
            }

            var key = maxHitDiceCount + restHealingBonusDiceCount <= 16
                ? maxHitDiceCount + restHealingBonusDiceCount <= 9
                    ? maxHitDiceCount + restHealingBonusDiceCount <= 4
                        ? HitDiceRestItem.DiceSize.Large
                        : HitDiceRestItem.DiceSize.Medium
                    : HitDiceRestItem.DiceSize.Small
                : HitDiceRestItem.DiceSize.VerySmall;

            var cellSize = __instance.cellSizes[key];
            var gridLayoutGroup = __instance.diceTable.GetComponent<GridLayoutGroup>();

            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

            while (__instance.diceTable.childCount < maxHitDiceCount + restHealingBonusDiceCount)
            {
                Gui.GetPrefabFromPool(__instance.dieItemPrefab, __instance.diceTable);
            }

            var canUseHitDice = false;
            var missingHitPoints = __instance.character.MissingHitPoints;

            if (__instance.canRollAtThisStage &&
                !__instance.character.IsDeadOrDying &&
                missingHitPoints > 0 &&
                remainingHitDiceCount > 0)
            {
                canUseHitDice = true;
            }

            var restHealingBonusList = __instance.restHealingBonuses
                .Where(restHealingBonus => restHealingBonus.Die != 0).ToList();

            var classHitDiceIndex = 0;
            var bonusHitDiceIndex = 0;

            // BEGIN CHANGE
            var spentHitDice = __instance.character.spentHitDice.ToDictionary(x => x.Key, x => x.Value);
            // END CHANGE

            for (var hitDiceIndex = 0; hitDiceIndex < __instance.diceTable.childCount; ++hitDiceIndex)
            {
                var child = __instance.diceTable.GetChild(hitDiceIndex);

                child.gameObject.SetActive(hitDiceIndex < maxHitDiceCount + restHealingBonusDiceCount);

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                var dieItem = child.GetComponent<DieItem>();
                var hitDieStatus = DieGroup.HitDieStatus.Spent;
                var isBonus = hitDiceIndex < restHealingBonusDiceCount;
                var dieType = isBonus
                    ? restHealingBonusList[bonusHitDiceIndex].Die
                    : __instance.character.ClassesHistory[classHitDiceIndex].HitDice;

                switch (isBonus)
                {
                    case true when !__instance.restHealingBonusSpent:
                        hitDieStatus = __instance.canRollAtThisStage
                            ? __instance.character.IsDeadOrDying || missingHitPoints <= 0
                                ? DieGroup.HitDieStatus.Disabled
                                : DieGroup.HitDieStatus.Normal
                            : DieGroup.HitDieStatus.Available;
                        break;
                    // BEGIN CHANGE
                    case false:
                    {
                        var isConsumed = false;

                        if (spentHitDice.TryGetValue(dieType, out var value) && value > 0)
                        {
                            spentHitDice[dieType]--;
                            isConsumed = true;
                        }

                        hitDieStatus = __instance.canRollAtThisStage
                            ? __instance.character.IsDeadOrDying || missingHitPoints <= 0 || isConsumed
                                ? DieGroup.HitDieStatus.Disabled
                                : DieGroup.HitDieStatus.Normal
                            : DieGroup.HitDieStatus.Available;
                        break;
                    }
                    // END CHANGE
                }

                dieItem.Setup(dieType, __instance.fontSizes[key], __instance.OnRollCb);
                dieItem.Refresh(hitDieStatus, isBonus, __instance.OnRollCb);

                if (isBonus)
                {
                    ++bonusHitDiceIndex;
                }
                else
                {
                    ++classHitDiceIndex;
                }
            }

            __instance.rollButton.gameObject.SetActive(canUseHitDice);
            __instance.statusLabel.gameObject.SetActive(!canUseHitDice);
            __instance.RefreshRollButton();

            if (!canUseHitDice)
            {
                return false;
            }

            var str = string.Empty;

            if (__instance.character.IsDead)
            {
                str = "Screen/&HitDiceItemStatusDeadTitle";
            }
            else if (missingHitPoints == 0)
            {
                str = "Screen/&HitDiceItemStatusFullHPTitle";
            }
            else if (remainingHitDiceCount == 0)
            {
                str = "Screen/&HitDiceItemStatusOutofHDTitle";
            }

            __instance.statusLabel.Text = str;

            return false;
        }
    }
}
