using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Insertion;

public class GraphicsCharacterFactoryManagerPatcher
{
    [HarmonyPatch(typeof(GraphicsCharacterFactoryManager), "InstantiateWieldedItemAsNeeded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackOn
    {
        internal static void Postfix(GraphicsCharacterFactoryManager __instance,
            GraphicsCharacter graphicsCharacter,
            GameObject prefab,
            RulesetItem rulesetItem,
            string slotName)
        {
            var feature = rulesetItem.itemDefinition.GetFirstSubFeatureOfType<CustomScale>();
            if (feature == null)
            {
                return;
            }

            var flag = rulesetItem.ItemDefinition.IsArmor &&
                       rulesetItem.ItemDefinition.ArmorDescription.ArmorType == "ShieldType";
            AnimationDefinitions.BoneType boneType;
            if (rulesetItem.ItemDefinition.IsWeapon)
                boneType = slotName != EquipmentDefinitions.SlotTypeOffHand
                    ? rulesetItem.ItemDefinition.WeaponDescription.WeaponTypeDefinition.IsAttachedToBone
                    : AnimationDefinitions.BoneType.Prop2;
            else if (flag)
                boneType = AnimationDefinitions.BoneType.Shield;
            else if (slotName == EquipmentDefinitions.SlotTypeMainHand)
                boneType = AnimationDefinitions.BoneType.Prop1;
            else if (slotName == EquipmentDefinitions.SlotTypeOffHand)
            {
                boneType = AnimationDefinitions.BoneType.Prop2;
            }
            else
            {
                return;
            }


            var boneTransform = graphicsCharacter.GetBoneTransform(boneType);
            if (boneTransform == null)
            {
                return;
            }

            var transform = boneTransform.Find(rulesetItem.Name);
            if (transform == null)
            {
                return;
            }

            var scale = transform.localScale;
            scale.x *= feature.X;
            scale.y *= feature.Y;
            scale.z *= feature.Z;
            transform.localScale = scale;
        }
    }
}
