using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class PowerBundle
{
    private static readonly Dictionary<SpellDefinition, FeatureDefinitionPower> Spells2Powers = new();
    private static readonly Dictionary<FeatureDefinitionPower, SpellDefinition> Powers2Spells = new();
    private static readonly Dictionary<FeatureDefinitionPower, Bundle> Bundles = new();

    private static readonly Dictionary<ulong, Dictionary<string, EffectDescription>> SpellEffectCache = new();

    private static Transform _parent;

    internal static void RechargeLinkedPowers(
        [NotNull] RulesetCharacter character,
        RestType restType)
    {
        var pointPoolPowerDefinitions = new List<FeatureDefinitionPower>();

        foreach (var usablePower in character.UsablePowers)
        {
            FeatureDefinitionPower rechargedPower;

            if (usablePower.PowerDefinition is IPowerSharedPool pool)
            {
                rechargedPower = pool.GetUsagePoolPower();
            }
            else if (usablePower.PowerDefinition.HasSubFeatureOfType<HasModifiedUses>())
            {
                rechargedPower = usablePower.PowerDefinition;
            }
            else
            {
                continue;
            }

            // Only add to recharge here if it (recharges on a short rest and this is a short or long rest) or
            // it recharges on a long rest and this is a long rest
            if (!pointPoolPowerDefinitions.Contains(rechargedPower)
                && (rechargedPower.RechargeRate == RechargeRate.ShortRest
                    || (rechargedPower.RechargeRate == RechargeRate.LongRest
                        && restType == RestType.LongRest)))
            {
                pointPoolPowerDefinitions.Add(rechargedPower);
            }
        }

        // Find the UsablePower of the point pool powers.
        foreach (var poolPower in character.UsablePowers)
        {
            if (!pointPoolPowerDefinitions.Contains(poolPower.PowerDefinition))
            {
                continue;
            }

            var poolSize = character.GetMaxUsesOfPower(poolPower);

            poolPower.remainingUses = poolSize;

            AssignUsesToSharedPowersForPool(character, poolPower, poolSize, poolSize);
        }
    }

    private static void AssignUsesToSharedPowersForPool(
        [NotNull] RulesetCharacter character,
        RulesetUsablePower poolPower,
        int remainingUses,
        int totalUses)
    {
        // Find powers that rely on this pool
        foreach (var usablePower in character.UsablePowers)
        {
            var power = usablePower.PowerDefinition;

            if (power is not IPowerSharedPool pool)
            {
                continue;
            }

            var pointPoolPower = pool.GetUsagePoolPower();

            if (pointPoolPower != poolPower.PowerDefinition)
            {
                continue;
            }

            if (power.CostPerUse == 0)
            {
                //Shared pool powers should have some cost, otherwise why make them shared?
                Main.Error($"Shared pool power '{power.Name}' has zero use cost!");
                usablePower.maxUses = totalUses;
                usablePower.remainingUses = remainingUses;
            }
            else
            {
                usablePower.maxUses = totalUses / power.CostPerUse;
                usablePower.remainingUses = remainingUses / power.CostPerUse;
            }
        }
    }

    [CanBeNull]
    internal static RulesetUsablePower GetPoolPower([NotNull] RulesetUsablePower power, RulesetCharacter character)
    {
        if (power.PowerDefinition is not IPowerSharedPool pool)
        {
            return null;
        }

        var poolPower = pool.GetUsagePoolPower();

        return UsablePowersProvider.Get(poolPower, character);
    }

    internal static int GetMaxUsesForPool(this RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power)
    {
        if (power is IPowerSharedPool poolPower)
        {
            power = poolPower.GetUsagePoolPower();
        }

        var usablePower = UsablePowersProvider.Get(power, character);

        return character.GetMaxUsesOfPower(usablePower);
    }

    internal static void UpdateUsageForPower(this RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power,
        int poolUsage)
    {
        if (power is IPowerSharedPool poolPower)
        {
            power = poolPower.GetUsagePoolPower();
        }

        var usablePower = UsablePowersProvider.Get(power, character);

        UpdateUsageForPowerPool(character, poolUsage, usablePower);
    }

    internal static void UpdateUsageForPower(this RulesetCharacter character,
        [NotNull] RulesetUsablePower modifiedPower,
        int poolUsage)
    {
        RulesetUsablePower usablePower = null;

        if (modifiedPower.PowerDefinition is IPowerSharedPool sharedPoolPower)
        {
            var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

            usablePower = UsablePowersProvider.Get(pointPoolPower, character);
        }
        else if (modifiedPower.PowerDefinition.HasSubFeatureOfType<IsPowerPool>())
        {
            usablePower = modifiedPower;
        }


        if (usablePower != null)
        {
            UpdateUsageForPowerPool(character, poolUsage, usablePower);
        }
    }

    internal static void UpdateUsageForPowerPool(
        this RulesetCharacter character,
        int poolUsage,
        RulesetUsablePower usablePower)
    {
        var maxUses = character.GetMaxUsesOfPower(usablePower);
        var remainingUses = Mathf.Clamp(usablePower.RemainingUses - poolUsage, 0, maxUses);

        usablePower.remainingUses = remainingUses;
        AssignUsesToSharedPowersForPool(character, usablePower, remainingUses, maxUses);

        // refresh character control panel after power pool usage is updated
        // needed for custom point pools on portrait to update properly in some cases
        GameUiContext.GameHud.RefreshCharacterControlPanel();
    }

    internal static int GetRemainingPowerUses(this RulesetCharacter character, [NotNull] FeatureDefinitionPower power)
    {
        if (power.CostPerUse == 0 || power.RechargeRate == RechargeRate.AtWill)
        {
            return int.MaxValue;
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (power.RechargeRate)
        {
            case RechargeRate.BardicInspiration:
                return (character.TryGetAttributeValue(AttributeDefinitions.BardicInspirationNumber) -
                        character.UsedBardicInspiration) /
                       power.CostPerUse;
            case RechargeRate.ChannelDivinity:
                return (character.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber) -
                        character.UsedChannelDivinity) /
                       power.CostPerUse;
            case RechargeRate.HealingPool:
                return (character.TryGetAttributeValue(AttributeDefinitions.HealingPool) - character.UsedHealingPool) /
                       power.CostPerUse;
            case RechargeRate.MaxHitPoints:
                return character.TryGetAttributeValue(AttributeDefinitions.HitPoints) / power.CostPerUse;
            case RechargeRate.RagePoints:
                return (character.TryGetAttributeValue(AttributeDefinitions.RagePoints) - character.UsedRagePoints) /
                       power.CostPerUse;
            case RechargeRate.SorceryPoints:
                return (character.TryGetAttributeValue(AttributeDefinitions.SorceryPoints) -
                        character.UsedSorceryPoints) /
                       power.CostPerUse;
            case RechargeRate.KiPoints:
                return (character.TryGetAttributeValue(AttributeDefinitions.KiPoints) - character.UsedKiPoints) /
                       power.CostPerUse;
#if false
            case RechargeRate.AtWill:
                break;
            case RechargeRate.OneMinute:
                break;
            case RechargeRate.ShortRest:
                break;
            case RechargeRate.LongRest:
                break;
            case RechargeRate.Dawn:
                break;
            case RechargeRate.D6_6:
                break;
            case RechargeRate.SpellSlot:
                break;
            case RechargeRate.None:
                break;
            case RechargeRate.D6_56:
                break;
            case RechargeRate.BindChain:
                break;
            case RechargeRate.TurnStart:
                break;
#endif
        }

        if (power is IPowerSharedPool poolPower)
        {
            return GetRemainingPowerPoolUses(character, poolPower) / power.CostPerUse;
        }

        var usablePower = UsablePowersProvider.Get(power, character);

        return usablePower.RemainingUses / power.CostPerUse;
    }

    internal static int GetRemainingPowerCharges(this RulesetCharacter character,
        [NotNull] FeatureDefinitionPower power)
    {
        if (power is IPowerSharedPool poolPower)
        {
            return GetRemainingPowerPoolUses(character, poolPower);
        }

        var usablePower = UsablePowersProvider.Get(power, character);

        return usablePower.RemainingUses;
    }

    private static int GetRemainingPowerPoolUses(
        this RulesetCharacter character,
        [NotNull] IPowerSharedPool sharedPoolPower)
    {
        var pointPoolPower = sharedPoolPower.GetUsagePoolPower();

        return UsablePowersProvider.Get(pointPoolPower, character).RemainingUses;
    }

    internal static EffectDescription ModifySpellEffect(EffectDescription original, [NotNull] RulesetEffectSpell spell)
    {
        return ModifyMagicEffect(original, spell.SpellDefinition, spell.Caster, spell);
    }

    internal static EffectDescription ModifySpellEffect([NotNull] SpellDefinition spell, RulesetCharacter caster)
    {
        return ModifyMagicEffect(spell.EffectDescription, spell, caster, null);
    }

    internal static EffectDescription ModifyPowerEffect(EffectDescription original, [NotNull] RulesetEffectPower power)
    {
        return ModifyMagicEffect(original, power.PowerDefinition, power.User, power);
    }

    private static string Key(BaseDefinition definition, BaseDefinition metamagic)
    {
        var key = $"{definition.GetType()}:{definition.Name}:";

        if (metamagic != null)
        {
            key += metamagic.Name;
        }

        return key;
    }

    private static EffectDescription GetCachedEffect(
        RulesetEntity caster,
        BaseDefinition definition,
        BaseDefinition metamagic)
    {
        if (!SpellEffectCache.TryGetValue(caster.Guid, out var effects))
        {
            return null;
        }

        return !effects.TryGetValue(Key(definition, metamagic), out var effect) ? null : effect;
    }

    private static void CacheEffect(
        RulesetEntity caster,
        BaseDefinition definition,
        BaseDefinition metamagic,
        EffectDescription effect)
    {
        if (!SpellEffectCache.TryGetValue(caster.Guid, out var value))
        {
            value = new Dictionary<string, EffectDescription>();
            SpellEffectCache.Add(caster.Guid, value);
        }

        value.AddOrReplace(Key(definition, metamagic), effect);
    }

    internal static void ClearSpellEffectCache(RulesetCharacter caster)
    {
        SpellEffectCache.Remove(caster.Guid);
    }

    private static EffectDescription ModifyMagicEffect(
        EffectDescription original,
        BaseDefinition definition,
        [CanBeNull] RulesetCharacter caster,
        [CanBeNull] RulesetEffect effect)
    {
        if (caster == null)
        {
            return original;
        }

        var metamagic = effect is RulesetEffectSpell spell ? spell.MetamagicOption : null;
        var cached = GetCachedEffect(caster, definition, metamagic);

        if (cached != null)
        {
            return cached;
        }

        //collect all valid modifiers from caster
        var result = original;
        var modifiers = new List<IModifyEffectDescription>();

        modifiers.AddRange(definition.GetAllSubFeaturesOfType<IModifyEffectDescription>()
            .Where(x => x.IsValid(definition, caster, result)));

        foreach (var modifier in caster.GetSubFeaturesByType<IModifyEffectDescription>()
                     .Where(x => x.IsValid(definition, caster, result)))
        {
            modifiers.TryAdd(modifier);
        }

        if (metamagic != null)
        {
            // all metamagic from metamagic feature are valid so no need to filter
            modifiers.AddRange(metamagic.GetAllSubFeaturesOfType<IModifyEffectDescription>());
        }

        if (modifiers.Count > 0)
        {
            result = modifiers.Aggregate(
                // ensure we get a copy as directly modifying blueprints can cause nasty effects
                EffectDescriptionBuilder
                    .Create(result)
                    .Build(),
                (current, f) => f.GetEffectDescription(definition, current, caster, effect));
        }

        CacheEffect(caster, definition, metamagic, result);

        return result;
    }

    /**Modifies spell/power description for GUI purposes.*/
    internal static EffectDescription ModifyMagicEffectGui(
        EffectDescription original,
        [NotNull] BaseDefinition definition)
    {
        return ModifyMagicEffect(original, definition, Global.CurrentCharacter, null);
    }

    internal static bool ValidatePrerequisites(
        [NotNull] RulesetCharacter character,
        [NotNull] BaseDefinition feature,
        [NotNull] IEnumerable<IValidateDefinitionPreRequisites.Validate> validators,
        [NotNull] out List<string> prerequisites)
    {
        var result = true;

        prerequisites = new List<string>();

        foreach (var validator in validators)
        {
            if (!validator(character, feature, out var line))
            {
                result = false;
            }

            if (line != null)
            {
                prerequisites.Add(line);
            }
        }

        return result;
    }

    internal static void RegisterPowerBundle([NotNull] FeatureDefinitionPower masterPower, bool terminateAll,
        [NotNull] params FeatureDefinitionPower[] subPowers)
    {
        RegisterPowerBundle(masterPower, terminateAll, subPowers.ToList());
    }

    public static void RegisterPowerBundle([NotNull] FeatureDefinitionPower masterPower, bool terminateAll,
        [NotNull] IEnumerable<FeatureDefinitionPower> subPowers)
    {
        if (Bundles.ContainsKey(masterPower))
        {
            throw new Exception($"Bundle '{masterPower.name}' already registered!");
        }

        var bundle = new Bundle(masterPower, subPowers, terminateAll);
        Bundles.Add(masterPower, bundle);
    }


    [CanBeNull]
    internal static Bundle GetBundle([CanBeNull] this FeatureDefinitionPower master)
    {
        if (master == null)
        {
            return null;
        }

        return Bundles.TryGetValue(master, out var result) ? result : null;
    }

    // [CanBeNull]
    // internal static Bundle GetBundle([NotNull] SpellDefinition master)
    // {
    //     return GetBundle(GetPower(master));
    // }

    internal static bool IsBundlePower([NotNull] this FeatureDefinitionPower power)
    {
        return Bundles.ContainsKey(power);
    }

    // [CanBeNull]
    // internal static List<FeatureDefinitionPower> GetBundleSubPowers(this FeatureDefinitionPower master)
    // {
    //     return GetBundle(master)?.SubPowers;
    // }

    // [CanBeNull]
    // internal static List<FeatureDefinitionPower> GetBundleSubPowers([NotNull] SpellDefinition master)
    // {
    //     return GetBundleSubPowers(GetPower(master));
    // }

    private static SpellDefinition RegisterPower([NotNull] FeatureDefinitionPower power)
    {
        if (Powers2Spells.TryGetValue(power, out var value))
        {
            return value;
        }

        var spell = SpellDefinitionBuilder.Create($"Spell{power.name}")
            .SetGuiPresentation(power.GuiPresentation)
            .AddToDB();
        Spells2Powers[spell] = power;
        Powers2Spells[power] = spell;
        return spell;
    }

    [CanBeNull]
    internal static FeatureDefinitionPower GetPower([NotNull] SpellDefinition spell)
    {
        return Spells2Powers.TryGetValue(spell, out var result) ? result : null;
    }

    [CanBeNull]
    internal static FeatureDefinitionPower GetPower(string name)
    {
        return Bundles.Keys.FirstOrDefault(p => p.Name == name);
    }

    [NotNull]
    internal static List<FeatureDefinitionPower> GetMasterPowersBySubPower(FeatureDefinitionPower subPower)
    {
        return Bundles
            .Where(e => e.Value.SubPowers.Contains(subPower))
            .Select(e => e.Key)
            .ToList();
    }

    [CanBeNull]
    internal static SpellDefinition GetSpell([NotNull] FeatureDefinitionPower power)
    {
        return Powers2Spells.TryGetValue(power, out var result) ? result : null;
    }

    // [CanBeNull]
    // internal static List<SpellDefinition> GetSubSpells([CanBeNull] FeatureDefinitionPower masterPower)
    // {
    //     if (masterPower == null)
    //     {
    //         return null;
    //     }
    //
    //     var subPowers = GetBundleSubPowers(masterPower);
    //
    //     return subPowers?.Select(GetSpell).ToList();
    // }

    // Bundled sub-powers usually are not added to the character, so their UsablePower lacks class or race origin
    // This means that CharacterActionSpendPower will not call `UsePower` on them
    // This method fixes that
    internal static void SpendBundledPowerIfNeeded([NotNull] CharacterActionSpendPower action)
    {
        if (action.ActionParams.RulesetEffect is not RulesetEffectPower { OriginItem: null } activePower)
        {
            return;
        }

        var usablePower = activePower.UsablePower;

        if (usablePower.OriginClass != null
            || usablePower.OriginRace != null
            || usablePower.PowerDefinition.RechargeRate == RechargeRate.AtWill)
        {
            return;
        }

        if (GetMasterPowersBySubPower(usablePower.PowerDefinition).Empty())
        {
            return;
        }

        action.ActingCharacter.RulesetCharacter.UsePower(usablePower);
    }

    /**
     * Patch implementation
     * Shows sub-power selection modal for power bundles, then activates selected sub power
     * Returns true if nothing needs (or can) be done.
     */
    internal static bool PowerBoxActivated(UsablePowerBox box)
    {
        var masterPower = box.usablePower.PowerDefinition;
        var bundle = GetBundle(masterPower);

        if (bundle == null)
        {
            return true;
        }

        if (box.powerEngaged == null)
        {
            return true;
        }

        var subpowerSelectionModal = Gui.GuiService.GetScreen<SubpowerSelectionModal>();
        var transform = subpowerSelectionModal.transform;

        _parent = transform.parent;
        transform.parent = box.transform.parent.parent;

        subpowerSelectionModal.Bind(bundle.SubPowers, box.activator, (power, _) =>
        {
            if (box != null && box.powerEngaged != null)
            {
                box.powerEngaged(power);
            }
            else
            {
                Main.Error("Can't activate sub-power: box or handler is null");
            }
        }, box.RectTransform);
        subpowerSelectionModal.Show();

        return false;
    }

    /**
     * Patch implementation
     * Replaces invocation activation with sub-power selection modal, after sub-power is selected activates invocation selected handler with proper sub-power index
     * Returns true if nothing needs (or can) be done.
     */
    internal static bool InvocationPowerActivated(InvocationActivationBox box,
        InvocationSelectionPanel.InvocationSelectedHandler selected)
    {
        var invocation = box.Invocation;
        var masterPower = invocation.InvocationDefinition.GetPower();

        if (masterPower == null)
        {
            return true;
        }

        var bundle = GetBundle(masterPower);

        if (bundle == null)
        {
            return true;
        }

        if (selected == null)
        {
            return true;
        }

        var subpowerSelectionModal = Gui.GuiService.GetScreen<SubpowerSelectionModal>();
        var transform = subpowerSelectionModal.transform;

        _parent = transform.parent;
        transform.parent = box.transform.parent.parent;

        subpowerSelectionModal.Bind(bundle.SubPowers, box.activator, (_, i) =>
        {
            if (box != null)
            {
                selected(invocation, i);
            }
            else
            {
                Main.Error("Can't activate invocation sub-power: box is null");
            }
        }, box.RectTransform);
        subpowerSelectionModal.Show();

        return false;
    }

    /**
     * Patch implementation
     * Closes sub-power selection modal
     */
    internal static void CloseSubPowerSelectionModal(bool instant)
    {
        var subpowerSelectionModal = Gui.GuiService.GetScreen<SubpowerSelectionModal>();

        if (subpowerSelectionModal == null)
        {
            return;
        }

        // required to support the after rest action menu that doesn't keep state
        if (_parent != null)
        {
            subpowerSelectionModal.transform.parent = _parent;
        }

        subpowerSelectionModal.Hide(instant);
    }

    //TODO: decide if we need this, or can re-use native method of rest bundle powers
    /**
     * Patch implementation
     * Makes after rest action activation show sub-power selection for bundled powers
     */
    internal static bool ExecuteAfterRestCb(AfterRestActionItem instance)
    {
        if (instance.executing)
        {
            return true;
        }

        var activity = instance.RestActivityDefinition;

        if (activity.Functor != PowerBundleContext.UseCustomRestPowerFunctorName || activity.StringParameter == null)
        {
            return true;
        }

        var masterPower = GetPower(activity.StringParameter);
        var bundle = masterPower ? masterPower.GetBundle() : null;

        if (bundle == null)
        {
            return true;
        }

        var subpowerSelectionModal = Gui.GuiService.GetScreen<SubpowerSelectionModal>();

        subpowerSelectionModal.Bind(bundle.SubPowers, instance.Hero, (rulesetPower, _) =>
        {
            instance.button.interactable = false;

            var power = rulesetPower.powerDefinition.Name;

            ServiceRepository.GetService<IGameRestingService>().ExecuteAsync(ExecuteAsync(instance, power), power);
        }, instance.RectTransform);

        subpowerSelectionModal.Show();

        return false;
    }

    private static IEnumerator ExecuteAsync(AfterRestActionItem item, string powerName)
    {
        item.executing = true;

        var parameters = new FunctorParametersDescription { RestingHero = item.Hero, StringParameter = powerName };
        var gameRestingService = ServiceRepository.GetService<IGameRestingService>();

        yield return ServiceRepository.GetService<IFunctorService>()
            .ExecuteFunctorAsync(item.RestActivityDefinition.Functor, parameters, gameRestingService);

        yield return null;

        var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        if (gameLocationActionService != null && gameLocationCharacterService != null)
        {
            bool needsToWait;

            do
            {
                needsToWait = gameLocationCharacterService.PartyCharacters
                    .Any(partyCharacter => gameLocationActionService.IsCharacterActing(partyCharacter));

                if (needsToWait)
                {
                    yield return null;
                }
            } while (needsToWait);
        }

        item.AfterRestActionTaken?.Invoke();
        item.executing = false;

        var button = item.button;

        if (button != null)
        {
            button.interactable = true;
        }
    }

    internal sealed class Bundle
    {
        internal Bundle(FeatureDefinitionPower masterPower, IEnumerable<FeatureDefinitionPower> subPowers,
            bool terminateAll)
        {
            // MasterPower = masterPower;
            SubPowers = new List<FeatureDefinitionPower>(subPowers);
            TerminateAll = terminateAll;

            var subSpells = SubPowers.Select(RegisterPower).ToList();

            Repertoire = new RulesetSpellRepertoire();
            Repertoire.KnownSpells.AddRange(subSpells);

            var masterSpell = RegisterPower(masterPower);

            masterSpell.SubspellsList.AddRange(subSpells);
        }

        /**
         * If set to true will terminate all powers in this bundle when 1 is terminated, so only one power
         * from this bundle can be in effect
         */
        internal bool TerminateAll { get; }

        internal List<FeatureDefinitionPower> SubPowers { get; }

        // internal FeatureDefinitionPower MasterPower { get; }

        //May be needed to hold powers for some native widgets
        // internal FeatureDefinitionFeatureSet PowerSet { get; }

        private RulesetSpellRepertoire Repertoire { get; }
    }
}

internal class IsPowerPool : PowerVisibilityModifier
{
    private IsPowerPool() : base((_, _, _) => false)
    {
    }

    public static IsPowerPool Marker { get; } = new();
}
