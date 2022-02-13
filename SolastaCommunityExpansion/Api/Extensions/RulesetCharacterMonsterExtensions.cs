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
    [TargetType(typeof(RulesetCharacterMonster)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetCharacterMonsterExtensions
    {
        public static T AddActiveFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddActiveFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddActiveFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : RulesetCharacterMonster
        {
            entity.ActiveFeatures.AddRange(value);
            return entity;
        }

        public static T AddAdditionalDroppedItemDefinitions<T>(this T entity,  params  ItemDefinition [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddAdditionalDroppedItemDefinitions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAdditionalDroppedItemDefinitions<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : RulesetCharacterMonster
        {
            entity.AdditionalDroppedItemDefinitions.AddRange(value);
            return entity;
        }

        public static T AddAmmunitionToRecoverList<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddAmmunitionToRecoverList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAmmunitionToRecoverList<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacterMonster
        {
            entity.AmmunitionToRecoverList.AddRange(value);
            return entity;
        }

        public static T AddAttackModes<T>(this T entity,  params  RulesetAttackMode [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddAttackModes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAttackModes<T>(this T entity, IEnumerable<RulesetAttackMode> value)
            where T : RulesetCharacterMonster
        {
            entity.AttackModes.AddRange(value);
            return entity;
        }

        public static T AddDroppedItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddDroppedItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDroppedItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacterMonster
        {
            entity.DroppedItems.AddRange(value);
            return entity;
        }

        public static T AddSpellRepertoires<T>(this T entity,  params  RulesetSpellRepertoire [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddSpellRepertoires(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSpellRepertoires<T>(this T entity, IEnumerable<RulesetSpellRepertoire> value)
            where T : RulesetCharacterMonster
        {
            entity.SpellRepertoires.AddRange(value);
            return entity;
        }

        public static T AddStealableItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddStealableItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStealableItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacterMonster
        {
            entity.StealableItems.AddRange(value);
            return entity;
        }

        public static T AddUsablePowers<T>(this T entity,  params  RulesetUsablePower [ ]  value)
            where T : RulesetCharacterMonster
        {
            AddUsablePowers(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUsablePowers<T>(this T entity, IEnumerable<RulesetUsablePower> value)
            where T : RulesetCharacterMonster
        {
            entity.UsablePowers.AddRange(value);
            return entity;
        }

        public static T ClearActiveFeatures<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.ActiveFeatures.Clear();
            return entity;
        }

        public static T ClearAdditionalDroppedItemDefinitions<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.AdditionalDroppedItemDefinitions.Clear();
            return entity;
        }

        public static T ClearAmmunitionToRecoverList<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.AmmunitionToRecoverList.Clear();
            return entity;
        }

        public static T ClearAttackModes<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.AttackModes.Clear();
            return entity;
        }

        public static T ClearDroppedItems<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.DroppedItems.Clear();
            return entity;
        }

        public static T ClearSpellRepertoires<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.SpellRepertoires.Clear();
            return entity;
        }

        public static T ClearStealableItems<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.StealableItems.Clear();
            return entity;
        }

        public static T ClearUsablePowers<T>(this T entity)
            where T : RulesetCharacterMonster
        {
            entity.UsablePowers.Clear();
            return entity;
        }

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

        public static T SetActiveFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetActiveFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetActiveFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : RulesetCharacterMonster
        {
            entity.ActiveFeatures.SetRange(value);
            return entity;
        }

        public static T SetAdditionalDroppedItemDefinitions<T>(this T entity,  params  ItemDefinition [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetAdditionalDroppedItemDefinitions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAdditionalDroppedItemDefinitions<T>(this T entity, IEnumerable<ItemDefinition> value)
            where T : RulesetCharacterMonster
        {
            entity.AdditionalDroppedItemDefinitions.SetRange(value);
            return entity;
        }

        public static T SetAmmunitionToRecoverList<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetAmmunitionToRecoverList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAmmunitionToRecoverList<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacterMonster
        {
            entity.AmmunitionToRecoverList.SetRange(value);
            return entity;
        }

        public static T SetAttackModes<T>(this T entity,  params  RulesetAttackMode [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetAttackModes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAttackModes<T>(this T entity, IEnumerable<RulesetAttackMode> value)
            where T : RulesetCharacterMonster
        {
            entity.AttackModes.SetRange(value);
            return entity;
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

        public static T SetDroppedItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetDroppedItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDroppedItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacterMonster
        {
            entity.DroppedItems.SetRange(value);
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

        public static T SetSpellRepertoires<T>(this T entity,  params  RulesetSpellRepertoire [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetSpellRepertoires(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSpellRepertoires<T>(this T entity, IEnumerable<RulesetSpellRepertoire> value)
            where T : RulesetCharacterMonster
        {
            entity.SpellRepertoires.SetRange(value);
            return entity;
        }

        public static T SetStealableItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetStealableItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStealableItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacterMonster
        {
            entity.StealableItems.SetRange(value);
            return entity;
        }

        public static T SetStealableLootPackOverride<T>(this T entity, LootPackDefinition value)
            where T : RulesetCharacterMonster
        {
            entity.SetField("stealableLootPackOverride", value);
            return entity;
        }

        public static T SetUsablePowers<T>(this T entity,  params  RulesetUsablePower [ ]  value)
            where T : RulesetCharacterMonster
        {
            SetUsablePowers(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUsablePowers<T>(this T entity, IEnumerable<RulesetUsablePower> value)
            where T : RulesetCharacterMonster
        {
            entity.UsablePowers.SetRange(value);
            return entity;
        }
    }
}