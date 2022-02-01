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
    [TargetType(typeof(RulesetCharacterMonster))]
    public static partial class RulesetCharacterMonsterExtensions
    {
        public static System.Collections.Generic.List<IAttackModificationProvider> GetAttackModifiers<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.List<IAttackModificationProvider>>("attackModifiers");
        }

        public static System.Collections.Generic.Dictionary<RulesetAttackMode, LegendaryActionDescription> GetLegendaryActionOfAttackMode<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.Dictionary<RulesetAttackMode, LegendaryActionDescription>>("legendaryActionOfAttackMode");
        }

        public static System.Collections.Generic.Dictionary<RulesetUsablePower, LegendaryActionDescription> GetLegendaryActionOfPower<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.Dictionary<RulesetUsablePower, LegendaryActionDescription>>("legendaryActionOfPower");
        }

        public static System.Collections.Generic.Dictionary<RulesetSpellRepertoire, LegendaryActionDescription> GetLegendaryActionOfSpellRepertoire<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.Dictionary<RulesetSpellRepertoire, LegendaryActionDescription>>("legendaryActionOfSpellRepertoire");
        }

        public static System.Collections.Generic.List<RulesetAttackMode> GetLegendaryAttackModes<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.List<RulesetAttackMode>>("legendaryAttackModes");
        }

        public static System.Collections.Generic.Dictionary<System.Int32, System.Int32> GetLegendaryMoveModes<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.Dictionary<System.Int32, System.Int32>>("legendaryMoveModes");
        }

        public static System.Collections.Generic.List<RulesetSpellRepertoire> GetLegendarySpellRepertoires<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.List<RulesetSpellRepertoire>>("legendarySpellRepertoires");
        }

        public static System.Collections.Generic.List<RulesetUsablePower> GetLegendaryUsablePowers<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.List<RulesetUsablePower>>("legendaryUsablePowers");
        }

        public static System.Collections.Generic.List<System.Int32> GetRemainingAttackUses<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.List<System.Int32>>("remainingAttackUses");
        }

        public static System.Collections.Generic.List<System.Boolean> GetUsedLegendaryActions<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            return entity.GetField<System.Collections.Generic.List<System.Boolean>>("usedLegendaryActions");
        }

        public static T SetBodyAssetPrefix<T>(this T entity, System.String value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("bodyAssetPrefix", value);
            return entity;
        }

        public static T SetConjuredByParty<T>(this T entity, System.Boolean value)
            where T : RulesetCharacterMonster
        {
            entity.ConjuredByParty = value;
            return entity;
        }

        public static T SetCurrentMonsterAttack<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterMonster
        {
            entity.CurrentMonsterAttack = value;
            return entity;
        }

        public static T SetDroppedLootPackOverride<T>(this T entity, LootPackDefinition value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("droppedLootPackOverride", value);
            return entity;
        }

        public static T SetFactionDamagingPenaltyOverride<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("factionDamagingPenaltyOverride", value);
            return entity;
        }

        public static T SetFactionKillingPenaltyOverride<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("factionKillingPenaltyOverride", value);
            return entity;
        }

        public static T SetFactionThievingPenaltyOverride<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("factionThievingPenaltyOverride", value);
            return entity;
        }

        public static T SetHumanoidMonsterPresentationDefinition<T>(this T entity, HumanoidMonsterPresentationDefinition value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("humanoidMonsterPresentationDefinition", value);
            return entity;
        }

        public static T SetIsActingLegendarily<T>(this T entity, System.Boolean value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("<IsActingLegendarily>k__BackingField", value);
            return entity;
        }

        public static T SetLegendaryActionUsed<T>(this T entity, RulesetCharacterMonster.LegendaryActionUsedHandler value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("<LegendaryActionUsed>k__BackingField", value);
            return entity;
        }

        public static T SetLegendaryResistanceUsed<T>(this T entity, RulesetCharacterMonster.LegendaryResistanceUsedHandler value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("<LegendaryResistanceUsed>k__BackingField", value);
            return entity;
        }

        public static T SetMonsterDefinition<T>(this T entity, MonsterDefinition value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("monsterDefinition", value);
            return entity;
        }

        public static T SetMonsterPresentationDefinition<T>(this T entity, MonsterPresentationDefinition value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("monsterPresentationDefinition", value);
            return entity;
        }

        public static T SetMorphotypeAssetPrefix<T>(this T entity, System.String value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("morphotypeAssetPrefix", value);
            return entity;
        }

        public static T SetOriginalFormCharacter<T>(this T entity, RulesetCharacter value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("originalFormCharacter", value);
            return entity;
        }

        public static T SetPrespawning<T>(this T entity, System.Boolean value)
            where T : RulesetCharacterMonster
        {
            entity.Prespawning = value;
            return entity;
        }

        public static T SetPrivateSeed<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("privateSeed", value);
            return entity;
        }

        public static T SetStealableLootPackOverride<T>(this T entity, LootPackDefinition value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("stealableLootPackOverride", value);
            return entity;
        }
    }
}