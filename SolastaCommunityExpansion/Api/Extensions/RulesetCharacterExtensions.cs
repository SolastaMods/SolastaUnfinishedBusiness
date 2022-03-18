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
    [TargetType(typeof(RulesetCharacter)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetCharacterExtensions
    {
        public static T AddAllUsableDeviceFunctions<T>(this T entity,  params  RulesetDeviceFunction [ ]  value)
            where T : RulesetCharacter
        {
            AddAllUsableDeviceFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAllUsableDeviceFunctions<T>(this T entity, IEnumerable<RulesetDeviceFunction> value)
            where T : RulesetCharacter
        {
            entity.AllUsableDeviceFunctions.AddRange(value);
            return entity;
        }

        public static T AddAttackModes<T>(this T entity,  params  RulesetAttackMode [ ]  value)
            where T : RulesetCharacter
        {
            AddAttackModes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAttackModes<T>(this T entity, IEnumerable<RulesetAttackMode> value)
            where T : RulesetCharacter
        {
            entity.AttackModes.AddRange(value);
            return entity;
        }

        public static T AddControlledEffectProxies<T>(this T entity,  params  RulesetCharacterEffectProxy [ ]  value)
            where T : RulesetCharacter
        {
            AddControlledEffectProxies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddControlledEffectProxies<T>(this T entity, IEnumerable<RulesetCharacterEffectProxy> value)
            where T : RulesetCharacter
        {
            entity.ControlledEffectProxies.AddRange(value);
            return entity;
        }

        public static T AddItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacter
        {
            AddItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacter
        {
            entity.Items.AddRange(value);
            return entity;
        }

        public static T AddLastReceivedDamageTypes<T>(this T entity,  params  System . String [ ]  value)
            where T : RulesetCharacter
        {
            AddLastReceivedDamageTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLastReceivedDamageTypes<T>(this T entity, IEnumerable<System.String> value)
            where T : RulesetCharacter
        {
            entity.LastReceivedDamageTypes.AddRange(value);
            return entity;
        }

        public static T AddMagicEffectsCache<T>(this T entity,  params  IMagicEffect [ ]  value)
            where T : RulesetCharacter
        {
            AddMagicEffectsCache(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMagicEffectsCache<T>(this T entity, IEnumerable<IMagicEffect> value)
            where T : RulesetCharacter
        {
            entity.MagicEffectsCache.AddRange(value);
            return entity;
        }

        public static T AddPowersUsedByMe<T>(this T entity,  params  RulesetEffectPower [ ]  value)
            where T : RulesetCharacter
        {
            AddPowersUsedByMe(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPowersUsedByMe<T>(this T entity, IEnumerable<RulesetEffectPower> value)
            where T : RulesetCharacter
        {
            entity.PowersUsedByMe.AddRange(value);
            return entity;
        }

        public static T AddRecoveredFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : RulesetCharacter
        {
            AddRecoveredFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRecoveredFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : RulesetCharacter
        {
            entity.RecoveredFeatures.AddRange(value);
            return entity;
        }

        public static T AddRequiredSenseTypesToPerceive<T>(this T entity,  params  SenseMode . Type [ ]  value)
            where T : RulesetCharacter
        {
            AddRequiredSenseTypesToPerceive(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRequiredSenseTypesToPerceive<T>(this T entity, IEnumerable<SenseMode.Type> value)
            where T : RulesetCharacter
        {
            entity.RequiredSenseTypesToPerceive.AddRange(value);
            return entity;
        }

        public static T AddReviveOptionsCache<T>(this T entity,  params  RuleDefinitions . ReviveOption [ ]  value)
            where T : RulesetCharacter
        {
            AddReviveOptionsCache(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddReviveOptionsCache<T>(this T entity, IEnumerable<RuleDefinitions.ReviveOption> value)
            where T : RulesetCharacter
        {
            entity.ReviveOptionsCache.AddRange(value);
            return entity;
        }

        public static T AddSenseModes<T>(this T entity,  params  SenseMode [ ]  value)
            where T : RulesetCharacter
        {
            AddSenseModes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSenseModes<T>(this T entity, IEnumerable<SenseMode> value)
            where T : RulesetCharacter
        {
            entity.SenseModes.AddRange(value);
            return entity;
        }

        public static T AddSpellRepertoires<T>(this T entity,  params  RulesetSpellRepertoire [ ]  value)
            where T : RulesetCharacter
        {
            AddSpellRepertoires(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSpellRepertoires<T>(this T entity, IEnumerable<RulesetSpellRepertoire> value)
            where T : RulesetCharacter
        {
            entity.SpellRepertoires.AddRange(value);
            return entity;
        }

        public static T AddSpellsCastByMe<T>(this T entity,  params  RulesetEffectSpell [ ]  value)
            where T : RulesetCharacter
        {
            AddSpellsCastByMe(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSpellsCastByMe<T>(this T entity, IEnumerable<RulesetEffectSpell> value)
            where T : RulesetCharacter
        {
            entity.SpellsCastByMe.AddRange(value);
            return entity;
        }

        public static T AddStealableItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacter
        {
            AddStealableItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStealableItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacter
        {
            entity.StealableItems.AddRange(value);
            return entity;
        }

        public static T AddTags<T>(this T entity,  params  System . String [ ]  value)
            where T : RulesetCharacter
        {
            AddTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTags<T>(this T entity, IEnumerable<System.String> value)
            where T : RulesetCharacter
        {
            entity.Tags.AddRange(value);
            return entity;
        }

        public static T AddUsablePowers<T>(this T entity,  params  RulesetUsablePower [ ]  value)
            where T : RulesetCharacter
        {
            AddUsablePowers(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUsablePowers<T>(this T entity, IEnumerable<RulesetUsablePower> value)
            where T : RulesetCharacter
        {
            entity.UsablePowers.AddRange(value);
            return entity;
        }

        public static T AddUsableSpells<T>(this T entity,  params  SpellDefinition [ ]  value)
            where T : RulesetCharacter
        {
            AddUsableSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUsableSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : RulesetCharacter
        {
            entity.UsableSpells.AddRange(value);
            return entity;
        }

        public static T ClearAllUsableDeviceFunctions<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.AllUsableDeviceFunctions.Clear();
            return entity;
        }

        public static T ClearAttackModes<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.AttackModes.Clear();
            return entity;
        }

        public static T ClearControlledEffectProxies<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.ControlledEffectProxies.Clear();
            return entity;
        }

        public static T ClearItems<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.Items.Clear();
            return entity;
        }

        public static T ClearLastReceivedDamageTypes<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.LastReceivedDamageTypes.Clear();
            return entity;
        }

        public static T ClearMagicEffectsCache<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.MagicEffectsCache.Clear();
            return entity;
        }

        public static T ClearPowersUsedByMe<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.PowersUsedByMe.Clear();
            return entity;
        }

        public static T ClearRecoveredFeatures<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.RecoveredFeatures.Clear();
            return entity;
        }

        public static T ClearRequiredSenseTypesToPerceive<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.RequiredSenseTypesToPerceive.Clear();
            return entity;
        }

        public static T ClearReviveOptionsCache<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.ReviveOptionsCache.Clear();
            return entity;
        }

        public static T ClearSenseModes<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.SenseModes.Clear();
            return entity;
        }

        public static T ClearSpellRepertoires<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.SpellRepertoires.Clear();
            return entity;
        }

        public static T ClearSpellsCastByMe<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.SpellsCastByMe.Clear();
            return entity;
        }

        public static T ClearStealableItems<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.StealableItems.Clear();
            return entity;
        }

        public static T ClearTags<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.Tags.Clear();
            return entity;
        }

        public static T ClearUsablePowers<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.UsablePowers.Clear();
            return entity;
        }

        public static T ClearUsableSpells<T>(this T entity)
            where T : RulesetCharacter
        {
            entity.UsableSpells.Clear();
            return entity;
        }

        public static System.Collections.Generic.List<RulesetItem> GetConsumedItems<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<RulesetItem>>("consumedItems");
        }

        public static System.Collections.Generic.List<FeatureDefinition> GetDeathTemperingFeatures<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<FeatureDefinition>>("deathTemperingFeatures");
        }

        public static System.Collections.Generic.List<FeatureDefinition> GetHealingFeatures<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<FeatureDefinition>>("healingFeatures");
        }

        public static System.Collections.Generic.List<RuleDefinitions.TrendInfo> GetMagicAttackTrends<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<RuleDefinitions.TrendInfo>>("magicAttackTrends");
        }

        public static System.Collections.Generic.List<FeatureDefinition> GetMagicFeatures<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<FeatureDefinition>>("magicFeatures");
        }

        public static System.Collections.Generic.List<RulesetEffectPower> GetPowersToTerminate<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<RulesetEffectPower>>("powersToTerminate");
        }

        public static System.Collections.Generic.List<SpellDefinition> GetSortedSpellsToBrowseCache<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<SpellDefinition>>("sortedSpellsToBrowseCache");
        }

        public static System.Collections.Generic.List<RuleDefinitions.TrendInfo> GetSortedTrends<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<RuleDefinitions.TrendInfo>>("sortedTrends");
        }

        public static System.Collections.Generic.List<SpellDefinition> GetSpellsToBrowse<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<SpellDefinition>>("spellsToBrowse");
        }

        public static System.Collections.Generic.List<RulesetEffectSpell> GetSpellsToTerminate<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<RulesetEffectSpell>>("spellsToTerminate");
        }

        public static System.Collections.Generic.List<RulesetCondition> GetToRemove<T>(this T entity)
            where T : RulesetCharacter
        {
            return entity.GetField<System.Collections.Generic.List<RulesetCondition>>("toRemove");
        }

        public static T SetAbilityCheckRolled<T>(this T entity, RulesetCharacter.AbilityCheckRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<AbilityCheckRolled>k__BackingField", value);
            return entity;
        }

        public static T SetActivePowerAdded<T>(this T entity, RulesetCharacter.ActivePowerAddedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ActivePowerAdded>k__BackingField", value);
            return entity;
        }

        public static T SetActiveSpellAdded<T>(this T entity, RulesetCharacter.ActiveSpellAddedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ActiveSpellAdded>k__BackingField", value);
            return entity;
        }

        public static T SetAdditionalAbilityCheckDieRolled<T>(this T entity, RulesetCharacter.AdditionalAbilityCheckDieRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<AdditionalAbilityCheckDieRolled>k__BackingField", value);
            return entity;
        }

        public static T SetAdditionalAttackDieRolled<T>(this T entity, RulesetCharacter.AdditionalAttackDieRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<AdditionalAttackDieRolled>k__BackingField", value);
            return entity;
        }

        public static T SetAdditionalDamageGenerated<T>(this T entity, RulesetCharacter.AdditionalDamageGeneratedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<AdditionalDamageGenerated>k__BackingField", value);
            return entity;
        }

        public static T SetAllUsableDeviceFunctions<T>(this T entity,  params  RulesetDeviceFunction [ ]  value)
            where T : RulesetCharacter
        {
            SetAllUsableDeviceFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAllUsableDeviceFunctions<T>(this T entity, IEnumerable<RulesetDeviceFunction> value)
            where T : RulesetCharacter
        {
            entity.AllUsableDeviceFunctions.SetRange(value);
            return entity;
        }

        public static T SetAttackModes<T>(this T entity,  params  RulesetAttackMode [ ]  value)
            where T : RulesetCharacter
        {
            SetAttackModes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAttackModes<T>(this T entity, IEnumerable<RulesetAttackMode> value)
            where T : RulesetCharacter
        {
            entity.AttackModes.SetRange(value);
            return entity;
        }

        public static T SetAttackOfOpportunity<T>(this T entity, RulesetCharacter.AttackOfOpportunityHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<AttackOfOpportunity>k__BackingField", value);
            return entity;
        }

        public static T SetAutoActivatingPower<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("autoActivatingPower", value);
            return entity;
        }

        public static T SetBaseFaction<T>(this T entity, FactionDefinition value)
            where T : RulesetCharacter
        {
            entity.SetField("baseFaction", value);
            return entity;
        }

        public static T SetBorrowedLuck<T>(this T entity, RulesetCharacter.BorrowedLuckHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<BorrowedLuck>k__BackingField", value);
            return entity;
        }

        public static T SetBreakFreeExecuted<T>(this T entity, RulesetCharacter.BreakFreeExecutedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<BreakFreeExecuted>k__BackingField", value);
            return entity;
        }

        public static T SetCannotReceiveHealing<T>(this T entity, RulesetCharacter.CannotReceiveHealingHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CannotReceiveHealing>k__BackingField", value);
            return entity;
        }

        public static T SetCarryingSize<T>(this T entity, RuleDefinitions.CreatureSize value)
            where T : RulesetCharacter
        {
            entity.SetField("carryingSize", value);
            return entity;
        }

        public static T SetCharacterControlChanged<T>(this T entity, RulesetCharacter.CharacterControlChangedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CharacterControlChanged>k__BackingField", value);
            return entity;
        }

        public static T SetCharacterFactionChanged<T>(this T entity, RulesetCharacter.CharacterFactionChangedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CharacterFactionChanged>k__BackingField", value);
            return entity;
        }

        public static T SetCharacterInventory<T>(this T entity, RulesetInventory value)
            where T : RulesetCharacter
        {
            entity.CharacterInventory = value;
            return entity;
        }

        public static T SetCharacterRefreshed<T>(this T entity, RulesetCharacter.CharacterRefreshedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CharacterRefreshed>k__BackingField", value);
            return entity;
        }

        public static T SetCharacterRevived<T>(this T entity, RulesetCharacter.CharacterRevivedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CharacterRevived>k__BackingField", value);
            return entity;
        }

        public static T SetCharacterWasStolenItems<T>(this T entity, RulesetCharacter.CharacterWasStolenItemsHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CharacterWasStolenItems>k__BackingField", value);
            return entity;
        }

        public static T SetCharmedCharacterRemovedFromBattle<T>(this T entity, RulesetCharacter.CharmedCharacterRemovedFromBattleHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CharmedCharacterRemovedFromBattle>k__BackingField", value);
            return entity;
        }

        public static T SetCheatInfiniteActionResources<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("<CheatInfiniteActionResources>k__BackingField", value);
            return entity;
        }

        public static T SetCheatIsInvisible<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("<CheatIsInvisible>k__BackingField", value);
            return entity;
        }

        public static T SetConcentratedSpell<T>(this T entity, RulesetEffectSpell value)
            where T : RulesetCharacter
        {
            entity.SetField("concentratedSpell", value);
            return entity;
        }

        public static T SetConcentratedSpellIndex<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("concentratedSpellIndex", value);
            return entity;
        }

        public static T SetConcentrationChanged<T>(this T entity, RulesetCharacter.ConcentrationChangedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ConcentrationChanged>k__BackingField", value);
            return entity;
        }

        public static T SetConcentrationCheckRolled<T>(this T entity, RulesetCharacter.ConcentrationCheckRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ConcentrationCheckRolled>k__BackingField", value);
            return entity;
        }

        public static T SetConcentrationLost<T>(this T entity, RulesetCharacter.ConcentrationLostHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ConcentrationLost>k__BackingField", value);
            return entity;
        }

        public static T SetContestCheckRolled<T>(this T entity, RulesetCharacter.ContestCheckRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ContestCheckRolled>k__BackingField", value);
            return entity;
        }

        public static T SetControlledEffectProxies<T>(this T entity,  params  RulesetCharacterEffectProxy [ ]  value)
            where T : RulesetCharacter
        {
            SetControlledEffectProxies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetControlledEffectProxies<T>(this T entity, IEnumerable<RulesetCharacterEffectProxy> value)
            where T : RulesetCharacter
        {
            entity.ControlledEffectProxies.SetRange(value);
            return entity;
        }

        public static T SetCraftingAttempted<T>(this T entity, RulesetCharacter.CraftingAttemptedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CraftingAttempted>k__BackingField", value);
            return entity;
        }

        public static T SetCraftingUpdated<T>(this T entity, RulesetCharacter.CraftingUpdatedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<CraftingUpdated>k__BackingField", value);
            return entity;
        }

        public static T SetDamageAbsorbedByTemporaryHitPoints<T>(this T entity, RulesetCharacter.DamageAbsorbedByTemporaryHitPointsHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DamageAbsorbedByTemporaryHitPoints>k__BackingField", value);
            return entity;
        }

        public static T SetDamageRetaliated<T>(this T entity, RulesetCharacter.DamageRetaliatedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DamageRetaliated>k__BackingField", value);
            return entity;
        }

        public static T SetDamageSustained<T>(this T entity, RulesetCharacter.DamageSustainedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DamageSustained>k__BackingField", value);
            return entity;
        }

        public static T SetDeadlyFall<T>(this T entity, RulesetCharacter.DeadlyFallHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DeadlyFall>k__BackingField", value);
            return entity;
        }

        public static T SetDeathSaveFailures<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("deathSaveFailures", value);
            return entity;
        }

        public static T SetDeathSaveRegainLife<T>(this T entity, RulesetCharacter.DeathSaveRegainLifeHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DeathSaveRegainLife>k__BackingField", value);
            return entity;
        }

        public static T SetDeathSaveRolled<T>(this T entity, RulesetCharacter.DeathSaveRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DeathSaveRolled>k__BackingField", value);
            return entity;
        }

        public static T SetDeathSaveSuccesses<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("deathSaveSuccesses", value);
            return entity;
        }

        public static T SetDeathTimerTicked<T>(this T entity, RulesetCharacter.DeathTimerTickedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<DeathTimerTicked>k__BackingField", value);
            return entity;
        }

        public static T SetDeityDefinition<T>(this T entity, DeityDefinition value)
            where T : RulesetCharacter
        {
            entity.DeityDefinition = value;
            return entity;
        }

        public static T SetDummy<T>(this T entity, System.String value)
            where T : RulesetCharacter
        {
            entity.SetField("dummy", value);
            return entity;
        }

        public static T SetExecutedAttacks<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.ExecutedAttacks = value;
            return entity;
        }

        public static T SetFactionOverride<T>(this T entity, FactionDefinition value)
            where T : RulesetCharacter
        {
            entity.SetField("factionOverride", value);
            return entity;
        }

        public static T SetFakeExperienceGained<T>(this T entity, RulesetCharacter.FakeExperienceGainedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<FakeExperienceGained>k__BackingField", value);
            return entity;
        }

        public static T SetForceAutoBehavior<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.ForceAutoBehavior = value;
            return entity;
        }

        public static T SetForcedBeardShape<T>(this T entity, System.String value)
            where T : RulesetCharacter
        {
            entity.SetField("<ForcedBeardShape>k__BackingField", value);
            return entity;
        }

        public static T SetHasAccessToTreasury<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.HasAccessToTreasury = value;
            return entity;
        }

        public static T SetHasRegeneration<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("hasRegeneration", value);
            return entity;
        }

        public static T SetHealingFromInflictedDamageReceived<T>(this T entity, RulesetCharacter.HealingFromInflictedDamageReceivedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<HealingFromInflictedDamageReceived>k__BackingField", value);
            return entity;
        }

        public static T SetHealingReceived<T>(this T entity, RulesetCharacter.HealingReceivedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<HealingReceived>k__BackingField", value);
            return entity;
        }

        public static T SetHitDieRolled<T>(this T entity, RulesetCharacter.HitDieRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<HitDieRolled>k__BackingField", value);
            return entity;
        }

        public static T SetHitPointsRegenerated<T>(this T entity, RulesetCharacter.HitPointsRegeneratedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<HitPointsRegenerated>k__BackingField", value);
            return entity;
        }

        public static T SetImpairedSight<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("impairedSight", value);
            return entity;
        }

        public static T SetIndomitableResistanceUsed<T>(this T entity, RulesetCharacter.IndomitableResistanceUsedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<IndomitableResistanceUsed>k__BackingField", value);
            return entity;
        }

        public static T SetInitiativeRolled<T>(this T entity, RulesetCharacter.InitiativeRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<InitiativeRolled>k__BackingField", value);
            return entity;
        }

        public static T SetInitiativeSequenceComplete<T>(this T entity, RulesetCharacter.InitiativeSequenceCompleteHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<InitiativeSequenceComplete>k__BackingField", value);
            return entity;
        }

        public static T SetInstantDeathPrevented<T>(this T entity, RulesetCharacter.InstantDeathPreventedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<InstantDeathPrevented>k__BackingField", value);
            return entity;
        }

        public static T SetIsDead<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isDead", value);
            return entity;
        }

        public static T SetIsDeadOrDying<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isDeadOrDying", value);
            return entity;
        }

        public static T SetIsDeadOrDyingOrUnconscious<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isDeadOrDyingOrUnconscious", value);
            return entity;
        }

        public static T SetIsDeadOrUnconscious<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isDeadOrUnconscious", value);
            return entity;
        }

        public static T SetIsDying<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isDying", value);
            return entity;
        }

        public static T SetIsIncapacitated<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isIncapacitated", value);
            return entity;
        }

        public static T SetIsRemovedFromTheGame<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isRemovedFromTheGame", value);
            return entity;
        }

        public static T SetIsUnconscious<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("isUnconscious", value);
            return entity;
        }

        public static T SetItemConsumed<T>(this T entity, RulesetCharacter.ItemConsumedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ItemConsumed>k__BackingField", value);
            return entity;
        }

        public static T SetItemGained<T>(this T entity, RulesetCharacter.ItemGainedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ItemGained>k__BackingField", value);
            return entity;
        }

        public static T SetItemLost<T>(this T entity, RulesetCharacter.ItemGainedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ItemLost>k__BackingField", value);
            return entity;
        }

        public static T SetItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacter
        {
            SetItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacter
        {
            entity.Items.SetRange(value);
            return entity;
        }

        public static T SetItemUsed<T>(this T entity, RulesetCharacter.ItemUsedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ItemUsed>k__BackingField", value);
            return entity;
        }

        public static T SetKnockOutPrevented<T>(this T entity, RulesetCharacter.KnockOutPreventedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<KnockOutPrevented>k__BackingField", value);
            return entity;
        }

        public static T SetLastDeathSaveFailed<T>(this T entity, RulesetCharacter.LastDeathSaveFailedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<LastDeathSaveFailed>k__BackingField", value);
            return entity;
        }

        public static T SetLastInitiativeModifier<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("<LastInitiativeModifier>k__BackingField", value);
            return entity;
        }

        public static T SetLastInitiativeRoll<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("<LastInitiativeRoll>k__BackingField", value);
            return entity;
        }

        public static T SetLastReceivedDamageTypes<T>(this T entity,  params  System . String [ ]  value)
            where T : RulesetCharacter
        {
            SetLastReceivedDamageTypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLastReceivedDamageTypes<T>(this T entity, IEnumerable<System.String> value)
            where T : RulesetCharacter
        {
            entity.LastReceivedDamageTypes.SetRange(value);
            return entity;
        }

        public static T SetLineOfSightParametersModified<T>(this T entity, RulesetCharacter.LineOfSightParametersModifiedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<LineOfSightParametersModified>k__BackingField", value);
            return entity;
        }

        public static T SetMagicalHitPointsPoolRolled<T>(this T entity, RulesetCharacter.MagicalHitPointsPoolRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<MagicalHitPointsPoolRolled>k__BackingField", value);
            return entity;
        }

        public static T SetMagicEffectsCache<T>(this T entity,  params  IMagicEffect [ ]  value)
            where T : RulesetCharacter
        {
            SetMagicEffectsCache(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMagicEffectsCache<T>(this T entity, IEnumerable<IMagicEffect> value)
            where T : RulesetCharacter
        {
            entity.MagicEffectsCache.SetRange(value);
            return entity;
        }

        public static T SetMaxClimbRange<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("maxClimbRange", value);
            return entity;
        }

        public static T SetMaxJumpRange<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("maxJumpRange", value);
            return entity;
        }

        public static T SetMaxJumpType<T>(this T entity, RulesetCharacter.JumpRuleType value)
            where T : RulesetCharacter
        {
            entity.SetField("maxJumpType", value);
            return entity;
        }

        public static T SetMaxSenseRange<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("maxSenseRange", value);
            return entity;
        }

        public static T SetMetamagicActivated<T>(this T entity, RulesetCharacter.MetamagicActivatedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<MetamagicActivated>k__BackingField", value);
            return entity;
        }

        public static T SetMinSizeDifferenceToGoThroughEnemy<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("minSizeDifferenceToGoThroughEnemy", value);
            return entity;
        }

        public static T SetMonsterIdentificationRolled<T>(this T entity, RulesetCharacter.MonsterIdentificationRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<MonsterIdentificationRolled>k__BackingField", value);
            return entity;
        }

        public static T SetMotionRangeFlags<T>(this T entity, RulesetCharacter.MotionRange value)
            where T : RulesetCharacter
        {
            entity.SetField("motionRangeFlags", value);
            return entity;
        }

        public static T SetOutOfAttackUses<T>(this T entity, RulesetCharacter.OutOfAttackUsesHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<OutOfAttackUses>k__BackingField", value);
            return entity;
        }

        public static T SetOverConcentrationUsed<T>(this T entity, RulesetCharacter.OverConcentrationUsedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<OverConcentrationUsed>k__BackingField", value);
            return entity;
        }

        public static T SetPersonalLightSource<T>(this T entity, RulesetLightSource value)
            where T : RulesetCharacter
        {
            entity.PersonalLightSource = value;
            return entity;
        }

        public static T SetPersonalLightSourceAdded<T>(this T entity, RulesetCharacter.PersonalLightSourceAddedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PersonalLightSourceAdded>k__BackingField", value);
            return entity;
        }

        public static T SetPersonalLightSourceRemoved<T>(this T entity, RulesetCharacter.PersonalLightSourceRemovedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PersonalLightSourceRemoved>k__BackingField", value);
            return entity;
        }

        public static T SetPowerActivated<T>(this T entity, RulesetCharacter.PowerActivatedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PowerActivated>k__BackingField", value);
            return entity;
        }

        public static T SetPowerAppliesNoAdditionalCondition<T>(this T entity, RulesetCharacter.PowerAppliesNoAdditionalConditionHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PowerAppliesNoAdditionalCondition>k__BackingField", value);
            return entity;
        }

        public static T SetPowerFailureChecked<T>(this T entity, RulesetCharacter.PowerFailureCheckedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PowerFailureChecked>k__BackingField", value);
            return entity;
        }

        public static T SetPowerRecharged<T>(this T entity, RulesetCharacter.PowerRechargedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PowerRecharged>k__BackingField", value);
            return entity;
        }

        public static T SetPowerRefunded<T>(this T entity, RulesetCharacter.PowerRefundedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PowerRefunded>k__BackingField", value);
            return entity;
        }

        public static T SetPowersUsedByMe<T>(this T entity,  params  RulesetEffectPower [ ]  value)
            where T : RulesetCharacter
        {
            SetPowersUsedByMe(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPowersUsedByMe<T>(this T entity, IEnumerable<RulesetEffectPower> value)
            where T : RulesetCharacter
        {
            entity.PowersUsedByMe.SetRange(value);
            return entity;
        }

        public static T SetPreferredReadyCantrip<T>(this T entity, SpellDefinition value)
            where T : RulesetCharacter
        {
            entity.PreferredReadyCantrip = value;
            return entity;
        }

        public static T SetPreventedFromActingThisTurn<T>(this T entity, RulesetCharacter.PreventedToActThisTurnHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<PreventedFromActingThisTurn>k__BackingField", value);
            return entity;
        }

        public static T SetPronoun<T>(this T entity, Gui.LocalizationSpeakerGender value)
            where T : RulesetCharacter
        {
            entity.Pronoun = value;
            return entity;
        }

        public static T SetRandomBehaviourRolled<T>(this T entity, RulesetCharacter.RandomBehaviourRolledHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<RandomBehaviourRolled>k__BackingField", value);
            return entity;
        }

        public static T SetRecoveredFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : RulesetCharacter
        {
            SetRecoveredFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRecoveredFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : RulesetCharacter
        {
            entity.RecoveredFeatures.SetRange(value);
            return entity;
        }

        public static T SetRegenerationLapse<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("regenerationLapse", value);
            return entity;
        }

        public static T SetRequiredSenseTypesToPerceive<T>(this T entity,  params  SenseMode . Type [ ]  value)
            where T : RulesetCharacter
        {
            SetRequiredSenseTypesToPerceive(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRequiredSenseTypesToPerceive<T>(this T entity, IEnumerable<SenseMode.Type> value)
            where T : RulesetCharacter
        {
            entity.RequiredSenseTypesToPerceive.SetRange(value);
            return entity;
        }

        public static T SetReviveFailed<T>(this T entity, RulesetCharacter.ReviveFailedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<ReviveFailed>k__BackingField", value);
            return entity;
        }

        public static T SetReviveOptionsCache<T>(this T entity,  params  RuleDefinitions . ReviveOption [ ]  value)
            where T : RulesetCharacter
        {
            SetReviveOptionsCache(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetReviveOptionsCache<T>(this T entity, IEnumerable<RuleDefinitions.ReviveOption> value)
            where T : RulesetCharacter
        {
            entity.ReviveOptionsCache.SetRange(value);
            return entity;
        }

        public static T SetRitualCast<T>(this T entity, RulesetCharacter.RitualCastHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<RitualCast>k__BackingField", value);
            return entity;
        }

        public static T SetSenseModes<T>(this T entity,  params  SenseMode [ ]  value)
            where T : RulesetCharacter
        {
            SetSenseModes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSenseModes<T>(this T entity, IEnumerable<SenseMode> value)
            where T : RulesetCharacter
        {
            entity.SenseModes.SetRange(value);
            return entity;
        }

        public static T SetSex<T>(this T entity, RuleDefinitions.CreatureSex value)
            where T : RulesetCharacter
        {
            entity.Sex = value;
            return entity;
        }

        public static T SetSharedDamage<T>(this T entity, RulesetCharacter.SharedDamageHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SharedDamage>k__BackingField", value);
            return entity;
        }

        public static T SetShouldClearOverridenFactionFromCharmSpells<T>(this T entity, System.Boolean value)
            where T : RulesetCharacter
        {
            entity.SetField("shouldClearOverridenFactionFromCharmSpells", value);
            return entity;
        }

        public static T SetSorceryPointsAltered<T>(this T entity, RulesetCharacter.SorceryPointsAlteredHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SorceryPointsAltered>k__BackingField", value);
            return entity;
        }

        public static T SetSpellCast<T>(this T entity, RulesetCharacter.SpellCastHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellCast>k__BackingField", value);
            return entity;
        }

        public static T SetSpellcastingFailed<T>(this T entity, RulesetCharacter.SpellcastingFailedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellcastingFailed>k__BackingField", value);
            return entity;
        }

        public static T SetSpellComponentConsumed<T>(this T entity, RulesetCharacter.SpellComponentConsumedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellComponentConsumed>k__BackingField", value);
            return entity;
        }

        public static T SetSpellCounter<T>(this T entity, RulesetCharacter.SpellCounterHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellCounter>k__BackingField", value);
            return entity;
        }

        public static T SetSpellCounterAttack<T>(this T entity, RulesetCharacter.SpellCounterAttackHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellCounterAttack>k__BackingField", value);
            return entity;
        }

        public static T SetSpellIdentified<T>(this T entity, RulesetCharacter.SpellIdentifiedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellIdentified>k__BackingField", value);
            return entity;
        }

        public static T SetSpellMissingComponent<T>(this T entity, RulesetCharacter.SpellMissingComponentHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellMissingComponent>k__BackingField", value);
            return entity;
        }

        public static T SetSpellRepertoires<T>(this T entity,  params  RulesetSpellRepertoire [ ]  value)
            where T : RulesetCharacter
        {
            SetSpellRepertoires(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSpellRepertoires<T>(this T entity, IEnumerable<RulesetSpellRepertoire> value)
            where T : RulesetCharacter
        {
            entity.SpellRepertoires.SetRange(value);
            return entity;
        }

        public static T SetSpellRepertoireSlotsRecovered<T>(this T entity, RulesetCharacter.SpellRepertoireSlotsRecoveredHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellRepertoireSlotsRecovered>k__BackingField", value);
            return entity;
        }

        public static T SetSpellsCastByMe<T>(this T entity,  params  RulesetEffectSpell [ ]  value)
            where T : RulesetCharacter
        {
            SetSpellsCastByMe(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSpellsCastByMe<T>(this T entity, IEnumerable<RulesetEffectSpell> value)
            where T : RulesetCharacter
        {
            entity.SpellsCastByMe.SetRange(value);
            return entity;
        }

        public static T SetSpellScribed<T>(this T entity, RulesetCharacter.SpellScribedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellScribed>k__BackingField", value);
            return entity;
        }

        public static T SetSpellSlotPreserved<T>(this T entity, RulesetCharacter.SpellSlotPreservedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<SpellSlotPreserved>k__BackingField", value);
            return entity;
        }

        public static T SetStealableItems<T>(this T entity,  params  RulesetItem [ ]  value)
            where T : RulesetCharacter
        {
            SetStealableItems(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStealableItems<T>(this T entity, IEnumerable<RulesetItem> value)
            where T : RulesetCharacter
        {
            entity.StealableItems.SetRange(value);
            return entity;
        }

        public static T SetTags<T>(this T entity,  params  System . String [ ]  value)
            where T : RulesetCharacter
        {
            SetTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTags<T>(this T entity, IEnumerable<System.String> value)
            where T : RulesetCharacter
        {
            entity.Tags.SetRange(value);
            return entity;
        }

        public static T SetTemporaryHitPoints<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.TemporaryHitPoints = value;
            return entity;
        }

        public static T SetTemporaryHitPointsReceived<T>(this T entity, RulesetCharacter.TemporaryHitPointsReceivedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<TemporaryHitPointsReceived>k__BackingField", value);
            return entity;
        }

        public static T SetTimeOfDeath<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("timeOfDeath", value);
            return entity;
        }

        public static T SetTurnSkipped<T>(this T entity, RulesetCharacter.TurnSkippedHandler value)
            where T : RulesetCharacter
        {
            entity.SetField("<TurnSkipped>k__BackingField", value);
            return entity;
        }

        public static T SetUsablePowers<T>(this T entity,  params  RulesetUsablePower [ ]  value)
            where T : RulesetCharacter
        {
            SetUsablePowers(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUsablePowers<T>(this T entity, IEnumerable<RulesetUsablePower> value)
            where T : RulesetCharacter
        {
            entity.UsablePowers.SetRange(value);
            return entity;
        }

        public static T SetUsableSpells<T>(this T entity,  params  SpellDefinition [ ]  value)
            where T : RulesetCharacter
        {
            SetUsableSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUsableSpells<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : RulesetCharacter
        {
            entity.UsableSpells.SetRange(value);
            return entity;
        }

        public static T SetUsedChannelDivinity<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("usedChannelDivinity", value);
            return entity;
        }

        public static T SetUsedHealingPool<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("usedHealingPool", value);
            return entity;
        }

        public static T SetUsedIndomitableResistances<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("usedIndomitableResistances", value);
            return entity;
        }

        public static T SetUsedKnockOutImmunityPerLongRest<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.UsedKnockOutImmunityPerLongRest = value;
            return entity;
        }

        public static T SetUsedMainAttacks<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.UsedMainAttacks = value;
            return entity;
        }

        public static T SetUsedRagePoints<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("usedRagePoints", value);
            return entity;
        }

        public static T SetUsedSorceryPoints<T>(this T entity, System.Int32 value)
            where T : RulesetCharacter
        {
            entity.SetField("usedSorceryPoints", value);
            return entity;
        }

        public static T SetVisionHeight<T>(this T entity, System.Single value)
            where T : RulesetCharacter
        {
            entity.SetField("visionHeight", value);
            return entity;
        }

        public static T SetVisionHeightFactor<T>(this T entity, System.Single value)
            where T : RulesetCharacter
        {
            entity.SetField("visionHeightFactor", value);
            return entity;
        }

        public static T SetVoiceID<T>(this T entity, System.String value)
            where T : RulesetCharacter
        {
            entity.VoiceID = value;
            return entity;
        }

        public static T SetWieldingSize<T>(this T entity, RuleDefinitions.CreatureSize value)
            where T : RulesetCharacter
        {
            entity.SetField("wieldingSize", value);
            return entity;
        }
    }
}