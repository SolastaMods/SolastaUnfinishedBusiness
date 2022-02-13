using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
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
using  static  GadgetDefinitions ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  FeatureDefinitionAutoPreparedSpells ;
using  static  FeatureDefinitionCraftingAffinity ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  SoundbanksDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  FeatureDefinitionAbilityCheckAffinity ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ItemDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ItemDefinitionExtensions
    {
        public static T AddActiveTags<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            AddActiveTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddActiveTags<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.ActiveTags.AddRange(value);
            return entity;
        }

        public static T AddInactiveTags<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            AddInactiveTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddInactiveTags<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.InactiveTags.AddRange(value);
            return entity;
        }

        public static T AddItemTags<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            AddItemTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddItemTags<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.ItemTags.AddRange(value);
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : ItemDefinition
        {
            AddPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : ItemDefinition
        {
            entity.PersonalityFlagOccurences.AddRange(value);
            return entity;
        }

        public static T AddRequiredAttunementClasses<T>(this T entity,  params  CharacterClassDefinition [ ]  value)
            where T : ItemDefinition
        {
            AddRequiredAttunementClasses(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRequiredAttunementClasses<T>(this T entity, IEnumerable<CharacterClassDefinition> value)
            where T : ItemDefinition
        {
            entity.RequiredAttunementClasses.AddRange(value);
            return entity;
        }

        public static T AddSlotsWhereActive<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            AddSlotsWhereActive(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSlotsWhereActive<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.SlotsWhereActive.AddRange(value);
            return entity;
        }

        public static T AddSlotTypes<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            AddSlotTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSlotTypes<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.SlotTypes.AddRange(value);
            return entity;
        }

        public static T AddStaticProperties<T>(this T entity,  params  ItemPropertyDescription [ ]  value)
            where T : ItemDefinition
        {
            AddStaticProperties(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStaticProperties<T>(this T entity, IEnumerable<ItemPropertyDescription> value)
            where T : ItemDefinition
        {
            entity.StaticProperties.AddRange(value);
            return entity;
        }

        public static T ClearActiveTags<T>(this T entity)
            where T : ItemDefinition
        {
            entity.ActiveTags.Clear();
            return entity;
        }

        public static T ClearInactiveTags<T>(this T entity)
            where T : ItemDefinition
        {
            entity.InactiveTags.Clear();
            return entity;
        }

        public static T ClearItemTags<T>(this T entity)
            where T : ItemDefinition
        {
            entity.ItemTags.Clear();
            return entity;
        }

        public static T ClearPersonalityFlagOccurences<T>(this T entity)
            where T : ItemDefinition
        {
            entity.PersonalityFlagOccurences.Clear();
            return entity;
        }

        public static T ClearRequiredAttunementClasses<T>(this T entity)
            where T : ItemDefinition
        {
            entity.RequiredAttunementClasses.Clear();
            return entity;
        }

        public static T ClearSlotsWhereActive<T>(this T entity)
            where T : ItemDefinition
        {
            entity.SlotsWhereActive.Clear();
            return entity;
        }

        public static T ClearSlotTypes<T>(this T entity)
            where T : ItemDefinition
        {
            entity.SlotTypes.Clear();
            return entity;
        }

        public static T ClearStaticProperties<T>(this T entity)
            where T : ItemDefinition
        {
            entity.StaticProperties.Clear();
            return entity;
        }

        public static ItemDefinition Copy(this ItemDefinition entity)
        {
            var copy = new ItemDefinition();
            copy.Copy(entity);
            return entity;
        }

        public static T SetActiveOnGround<T>(this T entity, System.Boolean value)
            where T : ItemDefinition
        {
            entity.SetField("activeOnGround", value);
            return entity;
        }

        public static T SetActiveTags<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            SetActiveTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetActiveTags<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.ActiveTags.SetRange(value);
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

        public static T SetInactiveTags<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            SetInactiveTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetInactiveTags<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.InactiveTags.SetRange(value);
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

        public static T SetItemTags<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            SetItemTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetItemTags<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.ItemTags.SetRange(value);
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

        public static T SetPersonalityFlagOccurences<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : ItemDefinition
        {
            SetPersonalityFlagOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPersonalityFlagOccurences<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : ItemDefinition
        {
            entity.PersonalityFlagOccurences.SetRange(value);
            return entity;
        }

        public static T SetRequiredAttunementClasses<T>(this T entity,  params  CharacterClassDefinition [ ]  value)
            where T : ItemDefinition
        {
            SetRequiredAttunementClasses(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRequiredAttunementClasses<T>(this T entity, IEnumerable<CharacterClassDefinition> value)
            where T : ItemDefinition
        {
            entity.RequiredAttunementClasses.SetRange(value);
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

        public static T SetSlotsWhereActive<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            SetSlotsWhereActive(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSlotsWhereActive<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.SlotsWhereActive.SetRange(value);
            return entity;
        }

        public static T SetSlotTypes<T>(this T entity,  params  System . String [ ]  value)
            where T : ItemDefinition
        {
            SetSlotTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSlotTypes<T>(this T entity, IEnumerable<System.String> value)
            where T : ItemDefinition
        {
            entity.SlotTypes.SetRange(value);
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

        public static T SetStaticProperties<T>(this T entity,  params  ItemPropertyDescription [ ]  value)
            where T : ItemDefinition
        {
            SetStaticProperties(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStaticProperties<T>(this T entity, IEnumerable<ItemPropertyDescription> value)
            where T : ItemDefinition
        {
            entity.StaticProperties.SetRange(value);
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