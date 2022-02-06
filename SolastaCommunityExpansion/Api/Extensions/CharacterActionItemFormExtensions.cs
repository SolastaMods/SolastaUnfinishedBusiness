using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
    [TargetType(typeof(CharacterActionItemForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterActionItemFormExtensions
    {
        public static T SetActionActivated<T>(this T entity, CharacterActionItemForm.ActionActivatedHandler value)
            where T : CharacterActionItemForm
        {
            entity.SetField("<ActionActivated>k__BackingField", value);
            return entity;
        }

        public static T SetAdditionalImages<T>(this T entity, UnityEngine.UI.Image[] value)
            where T : CharacterActionItemForm
        {
            entity.SetField("additionalImages", value);
            return entity;
        }

        public static T SetAdditionalImagesGrid<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("additionalImagesGrid", value);
            return entity;
        }

        public static T SetAttacksNumberGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("attacksNumberGroup", value);
            return entity;
        }

        public static T SetAttacksNumberValue<T>(this T entity, GuiLabel value)
            where T : CharacterActionItemForm
        {
            entity.SetField("attacksNumberValue", value);
            return entity;
        }

        public static T SetBackground<T>(this T entity, UnityEngine.UI.Image value)
            where T : CharacterActionItemForm
        {
            entity.SetField("background", value);
            return entity;
        }

        public static T SetButton<T>(this T entity, UnityEngine.UI.Button value)
            where T : CharacterActionItemForm
        {
            entity.SetField("button", value);
            return entity;
        }

        public static T SetCanvasGroup<T>(this T entity, UnityEngine.CanvasGroup value)
            where T : CharacterActionItemForm
        {
            entity.SetField("canvasGroup", value);
            return entity;
        }

        public static T SetCaptionLabel<T>(this T entity, GuiLabel value)
            where T : CharacterActionItemForm
        {
            entity.SetField("captionLabel", value);
            return entity;
        }

        public static T SetContentIcon<T>(this T entity, UnityEngine.UI.Image value)
            where T : CharacterActionItemForm
        {
            entity.SetField("contentIcon", value);
            return entity;
        }

        public static T SetContentLargeImage<T>(this T entity, UnityEngine.UI.Image value)
            where T : CharacterActionItemForm
        {
            entity.SetField("contentLargeImage", value);
            return entity;
        }

        public static T SetDarkColor<T>(this T entity, UnityEngine.Color value)
            where T : CharacterActionItemForm
        {
            entity.SetField("darkColor", value);
            return entity;
        }

        public static T SetDarkDisabledColor<T>(this T entity, UnityEngine.Color value)
            where T : CharacterActionItemForm
        {
            entity.SetField("darkDisabledColor", value);
            return entity;
        }

        public static T SetDynamicItemPropertiesEnumerator<T>(this T entity, DynamicItemPropertiesEnumerator value)
            where T : CharacterActionItemForm
        {
            entity.SetField("dynamicItemPropertiesEnumerator", value);
            return entity;
        }

        public static T SetFrame<T>(this T entity, UnityEngine.UI.Image value)
            where T : CharacterActionItemForm
        {
            entity.SetField("frame", value);
            return entity;
        }

        public static T SetGuiCharacterAction<T>(this T entity, GuiCharacterAction value)
            where T : CharacterActionItemForm
        {
            entity.SetField("guiCharacterAction", value);
            return entity;
        }

        public static T SetHighlightFrame<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("highlightFrame", value);
            return entity;
        }

        public static T SetHourGlass<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("hourGlass", value);
            return entity;
        }

        public static T SetHourGlassModifier<T>(this T entity, GuiModifierRotation value)
            where T : CharacterActionItemForm
        {
            entity.SetField("hourGlassModifier", value);
            return entity;
        }

        public static T SetHover<T>(this T entity, UnityEngine.UI.Image value)
            where T : CharacterActionItemForm
        {
            entity.SetField("hover", value);
            return entity;
        }

        public static T SetLimitedUseAmountLabel<T>(this T entity, GuiLabel value)
            where T : CharacterActionItemForm
        {
            entity.SetField("limitedUseAmountLabel", value);
            return entity;
        }

        public static T SetLimitedUseGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("limitedUseGroup", value);
            return entity;
        }

        public static T SetLimitedUseImage<T>(this T entity, UnityEngine.UI.Image value)
            where T : CharacterActionItemForm
        {
            entity.SetField("limitedUseImage", value);
            return entity;
        }

        public static T SetPointerEnter<T>(this T entity, CharacterActionItemForm.PointerEnterHandler value)
            where T : CharacterActionItemForm
        {
            entity.SetField("<PointerEnter>k__BackingField", value);
            return entity;
        }

        public static T SetPointerExit<T>(this T entity, CharacterActionItemForm.PointerExitHandler value)
            where T : CharacterActionItemForm
        {
            entity.SetField("<PointerExit>k__BackingField", value);
            return entity;
        }

        public static T SetSlotStatusPrefab<T>(this T entity, UnityEngine.GameObject value)
            where T : CharacterActionItemForm
        {
            entity.SetField("slotStatusPrefab", value);
            return entity;
        }

        public static T SetStackCountGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("stackCountGroup", value);
            return entity;
        }

        public static T SetStackCountLabel<T>(this T entity, GuiLabel value)
            where T : CharacterActionItemForm
        {
            entity.SetField("stackCountLabel", value);
            return entity;
        }

        public static T SetTooltip<T>(this T entity, GuiTooltip value)
            where T : CharacterActionItemForm
        {
            entity.SetField("tooltip", value);
            return entity;
        }

        public static T SetUseSlotsTable<T>(this T entity, UnityEngine.RectTransform value)
            where T : CharacterActionItemForm
        {
            entity.SetField("useSlotsTable", value);
            return entity;
        }
    }
}