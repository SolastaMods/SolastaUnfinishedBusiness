using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterReactionItemPatcher
{
    [HarmonyPatch(typeof(CharacterReactionItem), nameof(CharacterReactionItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: replaces calls to the Bind of `CharacterReactionSubitem` with custom method
            var bind = typeof(CharacterReactionSubitem).GetMethod("Bind");
            var customBindMethod =
                new Action<CharacterReactionSubitem, RulesetSpellRepertoire, int, string, bool,
                    CharacterReactionSubitem.SubitemSelectedHandler, ReactionRequest>(CustomBind).Method;
            var spellRepertoires = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
            var customSpellRepertoiresMethod =
                new Func<RulesetCharacter, List<RulesetSpellRepertoire>>(SpellRepertoiresNoRace).Method;

            return instructions
                .ReplaceCalls(bind, "CharacterReactionItem.Bind",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, customBindMethod))
                .ReplaceCalls(spellRepertoires, "CharacterReactionItem.SpellRepertoires",
                    new CodeInstruction(OpCodes.Call, customSpellRepertoiresMethod))
                //PATCH: removes Trace.Assert() that checks if character has any spell repertoires
                //this assert was added in 1.4.5 and triggers if non-spell caster does AoO
                //happens because we replaced default AoO reaction with warcaster one, so they would merge properly when several are triggered at once
                .RemoveBoolAsserts();
        }

        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterReactionItem __instance)
        {
            var request = __instance.ReactionRequest;
            var size = request is ReactionRequestWarcaster or ReactionRequestSpendBundlePower
                ? 400
                : 290;

            __instance.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);

            if (request is ReactionRequestSpendBundlePower)
            {
                __instance.powerReactionGroup.gameObject.SetActive(false);
            }

            //PATCH: support for displaying custom resources on reaction popup
            if (__instance.ReactionRequest is IReactionRequestWithResource attack)
            {
                SetupResource(__instance, attack.Resource);
            }

            //BUGFIX: vanilla is collecting KI Points instead of Sorcery Points
            var reactionParams = __instance.reactionRequest.ReactionParams;
            var usablePower = (reactionParams?.RulesetEffect is RulesetEffectPower rulesetEffectPower
                                  ? rulesetEffectPower.UsablePower
                                  : null)
                              ?? reactionParams?.UsablePower;
            var powerDefinition = usablePower?.PowerDefinition;

            if (!powerDefinition)
            {
                return;
            }

            if (powerDefinition.CostPerUse <= 0 ||
                powerDefinition.RechargeRate != RuleDefinitions.RechargeRate.SorceryPoints)
            {
                return;
            }

            __instance.remainingResourceValue.Text =
                __instance.guiCharacter.RulesetCharacter.RemainingSorceryPoints.ToString();
        }

        //BUGFIX: game currently gets the first spell repertoire to present slots on screen
        private static List<RulesetSpellRepertoire> SpellRepertoiresNoRace(RulesetCharacter rulesetCharacter)
        {
            return rulesetCharacter.SpellRepertoires
                .Where(x => !x.SpellCastingRace)
                .ToList();
        }

        private static void SetupResource(CharacterReactionItem item, ICustomReactionResource resource)
        {
            if (resource == null)
            {
                return;
            }

            Gui.ReleaseAddressableAsset(item.resourceCostSprite);
            item.resourceCostSprite = Gui.LoadAssetSync<Sprite>(resource.Icon);
            item.remainingResourceGroup.gameObject.SetActive(true);
            item.remainingResourceImage.sprite = item.resourceCostSprite;
            item.remainingResourceValue.Text = resource.GetUses(item.guiCharacter.rulesetCharacter);
            item.resourceCostGroup.gameObject.SetActive(true);
            item.resourceCostImage.sprite = item.resourceCostSprite;
            item.resourceCostValue.Text = (resource as ICustomReactionCustomResourceUse)?.GetRequestPoints(item) ?? "1";
        }

        //patch implementation
        //calls custom warcaster and power bundle binds when appropriate
        private static void CustomBind(
            [NotNull] CharacterReactionSubitem instance,
            RulesetSpellRepertoire spellRepertoire,
            int slotLevel,
            string text,
            bool interactable,
            CharacterReactionSubitem.SubitemSelectedHandler subitemSelected,
            ReactionRequest reactionRequest)
        {
            switch (reactionRequest)
            {
                case ReactionRequestWarcaster warcasterRequest:
                    instance.BindWarcaster(warcasterRequest, slotLevel, interactable, subitemSelected);
                    break;
                case ReactionRequestSpendBundlePower bundlePowerRequest:
                    instance.BindPowerBundle(bundlePowerRequest, slotLevel, interactable, subitemSelected);
                    break;
                default:
                    instance.Bind(spellRepertoire, slotLevel, text, interactable, subitemSelected);
                    break;
            }
        }
    }

    //TODO: check if still relevant - while this method wasn't touched, maybe sub-items are now disposed properly?
    [HarmonyPatch(typeof(CharacterReactionItem), nameof(CharacterReactionItem.GetSelectedSubItem))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetSelectedSubItem_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterReactionItem __instance, out int __result)
        {
            //BUGFIX: replaces `GetSelectedSubItem` to fix reaction selection crashes
            // Default one selects last item that is Selected, regardless if it is active or not, leading to wrong spell slots for smites being selected
            // This implementation returns first item that is both Selected and active
            __result = 0;

            var itemsTable = __instance.subItemsTable;

            for (var index = 0; index < itemsTable.childCount; ++index)
            {
                var item = itemsTable.GetChild(index).GetComponent<CharacterReactionSubitem>();

                if (!item.gameObject.activeSelf || !item.Selected)
                {
                    continue;
                }

                __result = index;
                break;
            }

            return false;
        }
    }
}
