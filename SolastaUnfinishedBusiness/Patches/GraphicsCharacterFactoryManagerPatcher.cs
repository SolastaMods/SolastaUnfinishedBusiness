using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GraphicsCharacterFactoryManagerPatcher
{
    [HarmonyPatch(typeof(GraphicsCharacterFactoryManager),
        nameof(GraphicsCharacterFactoryManager.InstantiateWieldedItemAsNeeded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InstantiateWieldedItemAsNeeded_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GraphicsCharacter graphicsCharacter,
            RulesetItem rulesetItem,
            string slotName)
        {
            //PATCH: Support for custom scaling of equipped items.
            //Used to scale reach weapons and hand crossbows
            var feature = rulesetItem.itemDefinition.GetFirstSubFeatureOfType<CustomScale>();

            if (feature == null)
            {
                return;
            }

            var flag = rulesetItem.ItemDefinition.IsArmor &&
                       rulesetItem.ItemDefinition.ArmorDescription.ArmorType == "ShieldType";

            AnimationDefinitions.BoneType boneType;

            if (rulesetItem.ItemDefinition.IsWeapon)
            {
                boneType = slotName != EquipmentDefinitions.SlotTypeOffHand
                    ? rulesetItem.ItemDefinition.WeaponDescription.WeaponTypeDefinition.IsAttachedToBone
                    : AnimationDefinitions.BoneType.Prop2;
            }
            else if (flag)
            {
                boneType = AnimationDefinitions.BoneType.Shield;
            }
            else if (slotName == EquipmentDefinitions.SlotTypeMainHand)
            {
                boneType = AnimationDefinitions.BoneType.Prop1;
            }
            else if (slotName == EquipmentDefinitions.SlotTypeOffHand)
            {
                boneType = AnimationDefinitions.BoneType.Prop2;
            }
            else
            {
                return;
            }


            var boneTransform = graphicsCharacter.GetBoneTransform(boneType);

            if (!boneTransform)
            {
                return;
            }

            var transform = boneTransform.Find(rulesetItem.Name);

            if (!transform)
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

#if false
    [HarmonyPatch(typeof(GraphicsCharacterFactoryManager),
        nameof(GraphicsCharacterFactoryManager.CollectBodyPartsToLoadWherePossible_Morphotypes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CollectBodyPartsToLoadWherePossible_Morphotypes_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GraphicsCharacterFactoryManager __instance)
        {
            //PATCH: support for horns on all races
            var searchTermFemale = "_Female_" + MorphotypeElementDefinition.ElementCategory.Horns;
            var searchTermMale = "_Male_" + MorphotypeElementDefinition.ElementCategory.Horns;

            for (var i = 0; i < __instance.shapePartsToLoad.Length; i++)
            {
                var pos = __instance.shapePartsToLoad[i].IndexOf(searchTermFemale, StringComparison.InvariantCulture);

                if (pos > 0)
                {
                    var raceName = __instance.shapePartsToLoad[i].Substring(0, pos);
                    var newPartName = __instance.shapePartsToLoad[i].Replace(raceName, "Dragonborn");

                    __instance.shapePartsToLoad[i] = newPartName;
                }

                pos = __instance.shapePartsToLoad[i].IndexOf(searchTermMale, StringComparison.InvariantCulture);

                // ReSharper disable once InvertIf
                if (pos > 0)
                {
                    var raceName = __instance.shapePartsToLoad[i].Substring(0, pos);
                    var newPartName = __instance.shapePartsToLoad[i].Replace(raceName, "Dragonborn");

                    __instance.shapePartsToLoad[i] = newPartName;
                }
            }
        }
    }
#endif
}
