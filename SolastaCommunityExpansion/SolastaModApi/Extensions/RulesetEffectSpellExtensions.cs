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
    [TargetType(typeof(RulesetEffectSpell))]
    public static partial class RulesetEffectSpellExtensions
    {
        public static T SetCaster<T>(this T entity, RulesetCharacter value)
            where T : RulesetEffectSpell
        {
            entity.SetField("caster", value);
            return entity;
        }

        public static T SetCasterId<T>(this T entity, System.UInt64 value)
            where T : RulesetEffectSpell
        {
            entity.SetField("casterId", value);
            return entity;
        }

        public static T SetCounterAffinity<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : RulesetEffectSpell
        {
            entity.CounterAffinity = value;
            return entity;
        }

        public static T SetCounterAffinityOrigin<T>(this T entity, System.String value)
            where T : RulesetEffectSpell
        {
            entity.CounterAffinityOrigin = value;
            return entity;
        }

        public static T SetIdentifiedBy<T>(this T entity, System.String value)
            where T : RulesetEffectSpell
        {
            entity.SetField("<IdentifiedBy>k__BackingField", value);
            return entity;
        }

        public static T SetMetamagicOption<T>(this T entity, MetamagicOptionDefinition value)
            where T : RulesetEffectSpell
        {
            entity.MetamagicOption = value;
            return entity;
        }

        public static T SetName<T>(this T entity, System.String value)
            where T : RulesetEffectSpell
        {
            entity.Name = value;
            return entity;
        }

        public static T SetOriginItem<T>(this T entity, RulesetItemDevice value)
            where T : RulesetEffectSpell
        {
            entity.SetField("originItem", value);
            return entity;
        }

        public static T SetSlotLevel<T>(this T entity, System.Int32 value)
            where T : RulesetEffectSpell
        {
            entity.SlotLevel = value;
            return entity;
        }

        public static T SetSpellDefinition<T>(this T entity, SpellDefinition value)
            where T : RulesetEffectSpell
        {
            entity.SetField("spellDefinition", value);
            return entity;
        }

        public static T SetSpellRepertoire<T>(this T entity, RulesetSpellRepertoire value)
            where T : RulesetEffectSpell
        {
            entity.SetField("spellRepertoire", value);
            return entity;
        }
    }
}