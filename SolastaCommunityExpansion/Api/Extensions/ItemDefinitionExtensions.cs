using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ItemDefinition))]
    public static partial class ItemDefinitionExtensions
    {
        public static T SetActiveOnGround<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("activeOnGround", value);
            return entity;
        }

        public static T SetAmmunitionDescription<T>(this T entity, AmmunitionDescription value)
            where T : ItemDefinition
        {
            entity.SetField("ammunitionDefinition", value);
            return entity;
        }

        public static T SetArmorDescription<T>(this T entity, ArmorDescription value)
            where T : ItemDefinition
        {
            entity.SetField("armorDefinition", value);
            return entity;
        }

        public static T SetCanBeStacked<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("canBeStacked", value);
            return entity;
        }

        public static T SetContainerItemDescription<T>(this T entity, ContainerItemDescription value)
            where T : ItemDefinition
        {
            entity.SetField("containerItemDefinition", value);
            return entity;
        }

        public static T SetCosts<T>(this T entity, System.Int32[] value)
            where T : ItemDefinition
        {
            entity.SetField("costs", value);
            return entity;
        }

        public static T SetDefaultStackCount<T>(this T entity, System.Int32 value)
            where T : ItemDefinition
        {
            entity.SetField("defaultStackCount", value);
            return entity;
        }

        public static T SetDestroyedWhenUnequiped<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("destroyedWhenUnequiped", value);
            return entity;
        }

        public static T SetDocumentDescription<T>(this T entity, DocumentDescription value)
            where T : ItemDefinition
        {
            entity.SetField("documentDescription", value);
            return entity;
        }

        public static T SetFactionRelicDescription<T>(this T entity, FactionRelicDescription value)
            where T : ItemDefinition
        {
            entity.SetField("factionRelicDescription", value);
            return entity;
        }

        public static T SetFocusItemDescription<T>(this T entity, FocusItemDescription value)
            where T : ItemDefinition
        {
            entity.SetField("focusItemDefinition", value);
            return entity;
        }

        public static T SetFoodDescription<T>(this T entity, FoodDescription value)
            where T : ItemDefinition
        {
            entity.SetField("foodDescription", value);
            return entity;
        }

        public static T SetForceEquip<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("forceEquip", value);
            return entity;
        }

        public static T SetForceEquipSlot<T>(this T entity, System.String value)
            where T : ItemDefinition
        {
            entity.SetField("forceEquipSlot", value);
            return entity;
        }

        public static T SetInDungeonEditor<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("inDungeonEditor", value);
            return entity;
        }

        public static T SetIsAmmunition<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsAmmunition = value;
            return entity;
        }

        public static T SetIsArmor<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsArmor = value;
            return entity;
        }

        public static T SetIsContainerItem<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsContainerItem = value;
            return entity;
        }

        public static T SetIsDocument<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsDocument = value;
            return entity;
        }

        public static T SetIsFactionRelic<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsFactionRelic = value;
            return entity;
        }

        public static T SetIsFocusItem<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsFocusItem = value;
            return entity;
        }

        public static T SetIsFood<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsFood = value;
            return entity;
        }

        public static T SetIsLightSourceItem<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsLightSourceItem = value;
            return entity;
        }

        public static T SetIsSpellbook<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsSpellbook = value;
            return entity;
        }

        public static T SetIsStarterPack<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsStarterPack = value;
            return entity;
        }

        public static T SetIsTool<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsTool = value;
            return entity;
        }

        public static T SetIsUsableDevice<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsUsableDevice = value;
            return entity;
        }

        public static T SetIsWealthPile<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsWealthPile = value;
            return entity;
        }

        public static T SetIsWeapon<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.IsWeapon = value;
            return entity;
        }

        public static T SetItemPresentation<T>(this T entity, ItemPresentation value)
            where T : ItemDefinition
        {
            entity.SetField("itemPresentation", value);
            return entity;
        }

        public static T SetItemRarity<T>(this T entity, RuleDefinitions.ItemRarity value)
            where T : ItemDefinition
        {
            entity.SetField("itemRarity", value);
            return entity;
        }

        public static T SetLightSourceItemDescription<T>(this T entity, LightSourceItemDescription value)
            where T : ItemDefinition
        {
            entity.SetField("lightSourceItemDefinition", value);
            return entity;
        }

        public static T SetMagical<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("magical", value);
            return entity;
        }

        public static T SetMerchantCategory<T>(this T entity, System.String value)
            where T : ItemDefinition
        {
            entity.SetField("merchantCategory", value);
            return entity;
        }

        public static T SetRequiresAttunement<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("requiresAttunement", value);
            return entity;
        }

        public static T SetRequiresIdentification<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("requiresIdentification", value);
            return entity;
        }

        public static T SetSoundEffectDescription<T>(this T entity, SoundEffectDescription value)
            where T : ItemDefinition
        {
            entity.SetField("soundEffectDescriptionOverride", value);
            return entity;
        }

        public static T SetSoundEffectOnHitDescription<T>(this T entity, SoundEffectOnHitDescription value)
            where T : ItemDefinition
        {
            entity.SetField("soundEffectOnHitDescriptionOverride", value);
            return entity;
        }

        public static T SetSpellbookDescription<T>(this T entity, SpellbookDescription value)
            where T : ItemDefinition
        {
            entity.SetField("spellbookDefinition", value);
            return entity;
        }

        public static T SetStackSize<T>(this T entity, System.Int32 value)
            where T : ItemDefinition
        {
            entity.SetField("stackSize", value);
            return entity;
        }

        public static T SetStarterPackDescription<T>(this T entity, StarterPackDescription value)
            where T : ItemDefinition
        {
            entity.SetField("starterPackDefinition", value);
            return entity;
        }

        public static T SetToolDescription<T>(this T entity, ToolDescription value)
            where T : ItemDefinition
        {
            entity.SetField("toolDefinition", value);
            return entity;
        }

        public static T SetUsableDeviceDescription<T>(this T entity, UsableDeviceDescription value)
            where T : ItemDefinition
        {
            entity.SetField("usableDeviceDescription", value);
            return entity;
        }

        public static T SetUserItem<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("<UserItem>k__BackingField", value);
            return entity;
        }

        public static T SetWealthPileDescription<T>(this T entity, WealthPileDescription value)
            where T : ItemDefinition
        {
            entity.SetField("wealthPileDefinition", value);
            return entity;
        }

        public static T SetWeaponDescription<T>(this T entity, WeaponDescription value)
            where T : ItemDefinition
        {
            entity.SetField("weaponDefinition", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, System.Single value)
            where T : ItemDefinition
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}