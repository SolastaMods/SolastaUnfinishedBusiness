using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix;

internal static class RulesetImplementationManagerPatcher
{
    // Call parts of the stuff `RulesetImplementationManagerLocation` does for `RulesetImplementationManagerCampaign`
    // This makes light and item effects correctly terminate when resting during world travel
    // The code is prettified decompiled code from `RulesetImplementationManagerLocation`
    [HarmonyPatch(typeof(RulesetImplementationManager), "TerminateEffect")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetImplementationManager_TerminateEffect
    {
        internal static void Postfix(RulesetImplementationManager __instance, RulesetEffect activeEffect,
            bool showGraphics = true)
        {
            if (__instance is not RulesetImplementationManagerCampaign)
            {
                return;
            }

            if (activeEffect is {TrackedLightSourceGuids.Count: > 0})
            {
                var service = ServiceRepository.GetService<IGameLocationVisibilityService>();
                foreach (var trackedLightSourceGuid in activeEffect.TrackedLightSourceGuids)
                {
                    var rulesetLightSource = (RulesetLightSource) null;
                    ref var local = ref rulesetLightSource;
                    if (RulesetEntity.TryGetEntity(trackedLightSourceGuid, out local) && rulesetLightSource != null)
                    {
                        rulesetLightSource.LightSourceExtinguished -= activeEffect.LightSourceExtinguished;
                        RulesetCharacter bearer;
                        if (rulesetLightSource.TargetItemGuid != 0UL &&
                            RulesetEntity.TryGetEntity(rulesetLightSource.TargetItemGuid, out RulesetItem rulesetItem))
                        {
                            if (RulesetEntity.TryGetEntity(rulesetItem.BearerGuid, out bearer) &&
                                bearer is {CharacterInventory: { }})
                            {
                                var itemAltered = bearer.CharacterInventory.ItemAltered;
                                if (itemAltered != null)
                                    itemAltered(bearer.CharacterInventory,
                                        bearer.CharacterInventory.FindSlotHoldingItem(rulesetItem), rulesetItem);
                            }

                            var fromActor = GameLocationCharacter.GetFromActor(bearer);
                            service?.RemoveCharacterLightSource(fromActor, rulesetItem.RulesetLightSource);
                            rulesetItem.RulesetLightSource?.Unregister();
                            rulesetItem.RulesetLightSource = null;
                        }
                        else if (rulesetLightSource.TargetGuid != 0UL &&
                                 RulesetEntity.TryGetEntity(rulesetLightSource.TargetGuid, out bearer))
                        {
                            var fromActor = GameLocationCharacter.GetFromActor(bearer);
                            service?.RemoveCharacterLightSource(fromActor, rulesetLightSource);
                            if (rulesetLightSource.UseSpecificLocationPosition)
                            {
                                if (bearer is RulesetCharacterEffectProxy proxy)
                                    proxy.RemoveAdditionalPersonalLightSource(rulesetLightSource);
                            }
                            else if (bearer != null)
                                bearer.PersonalLightSource = null;
                        }
                    }
                }

                activeEffect.TrackedLightSourceGuids.Clear();
            }

            if (activeEffect is {TrackedItemPropertyGuids.Count: > 0})
            {
                foreach (var itemPropertyGuid in activeEffect.TrackedItemPropertyGuids)
                {
                    var rulesetItemProperty = (RulesetItemProperty) null;
                    ref var local = ref rulesetItemProperty;
                    if (RulesetEntity.TryGetEntity(itemPropertyGuid, out local) && rulesetItemProperty != null)
                    {
                        if (RulesetEntity.TryGetEntity(rulesetItemProperty.TargetItemGuid,
                                out RulesetItem rulesetItem) &&
                            rulesetItem != null)
                        {
                            rulesetItem.ItemPropertyRemoved -= activeEffect.ItemPropertyRemoved;
                            rulesetItem.RemoveDynamicProperty(rulesetItemProperty);
                            if (RulesetEntity.TryGetEntity(rulesetItem.BearerGuid,
                                    out RulesetCharacter rulesetItemBearer) && rulesetItemBearer != null)
                            {
                                var characterInventory = rulesetItemBearer.CharacterInventory;
                                if (characterInventory != null)
                                {
                                    var itemAltered = characterInventory.ItemAltered;
                                    if (itemAltered != null)
                                        itemAltered(characterInventory,
                                            characterInventory.FindSlotHoldingItem(rulesetItem),
                                            rulesetItem);
                                }

                                rulesetItemBearer.RefreshAll();
                            }
                        }
                    }
                }
            }
        }
    }
}