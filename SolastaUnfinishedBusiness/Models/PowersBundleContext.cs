using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Models;

public static class PowersBundleContext
{
    internal const string UseCustomRestPowerFunctorName = "UseCustomRestPower";

    private static readonly Dictionary<SpellDefinition, FeatureDefinitionPower> Spells2Powers = new();
    private static readonly Dictionary<FeatureDefinitionPower, SpellDefinition> Powers2Spells = new();
    private static readonly Dictionary<FeatureDefinitionPower, Bundle> Bundles = new();

    public static void RegisterPowerBundle([NotNull] FeatureDefinitionPower masterPower, bool terminateAll,
        [NotNull] params FeatureDefinitionPower[] subPowers)
    {
        RegisterPowerBundle(masterPower, terminateAll, subPowers.ToList());
    }

    private static void RegisterPowerBundle([NotNull] FeatureDefinitionPower masterPower, bool terminateAll,
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
    public static Bundle GetBundle([CanBeNull] this FeatureDefinitionPower master)
    {
        if (master == null)
        {
            return null;
        }

        return Bundles.TryGetValue(master, out var result) ? result : null;
    }

    // [CanBeNull]
    // public static Bundle GetBundle([NotNull] SpellDefinition master)
    // {
    //     return GetBundle(GetPower(master));
    // }

    public static bool IsBundlePower([NotNull] this FeatureDefinitionPower power)
    {
        return Bundles.ContainsKey(power);
    }

    // [CanBeNull]
    // public static List<FeatureDefinitionPower> GetBundleSubPowers(this FeatureDefinitionPower master)
    // {
    //     return GetBundle(master)?.SubPowers;
    // }

    // [CanBeNull]
    // public static List<FeatureDefinitionPower> GetBundleSubPowers([NotNull] SpellDefinition master)
    // {
    //     return GetBundleSubPowers(GetPower(master));
    // }

    private static SpellDefinition RegisterPower([NotNull] FeatureDefinitionPower power)
    {
        if (Powers2Spells.ContainsKey(power))
        {
            return Powers2Spells[power];
        }

        var spell = SpellDefinitionBuilder.Create($"Spell{power.name}")
            .SetGuiPresentation(power.GuiPresentation)
            .AddToDB();
        Spells2Powers[spell] = power;
        Powers2Spells[power] = spell;
        return spell;
    }

    [CanBeNull]
    public static FeatureDefinitionPower GetPower([NotNull] SpellDefinition spell)
    {
        return Spells2Powers.TryGetValue(spell, out var result) ? result : null;
    }

    [CanBeNull]
    public static FeatureDefinitionPower GetPower(string name)
    {
        return Bundles.Keys.FirstOrDefault(p => p.Name == name);
    }

    [NotNull]
    public static List<FeatureDefinitionPower> GetMasterPowersBySubPower(FeatureDefinitionPower subPower)
    {
        return Bundles
            .Where(e => e.Value.SubPowers.Contains(subPower))
            .Select(e => e.Key)
            .ToList();
    }

    [CanBeNull]
    public static SpellDefinition GetSpell([NotNull] FeatureDefinitionPower power)
    {
        return Powers2Spells.TryGetValue(power, out var result) ? result : null;
    }

    // [CanBeNull]
    // public static List<SpellDefinition> GetSubSpells([CanBeNull] FeatureDefinitionPower masterPower)
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
    public static void SpendBundledPowerIfNeeded([NotNull] CharacterActionSpendPower action)
    {
        var activePower = action.ActionParams.RulesetEffect as RulesetEffectPower;
        if (activePower is not { OriginItem: null })
        {
            return;
        }

        var usablePower = activePower.UsablePower;

        if (usablePower.OriginClass != null
            || usablePower.OriginRace != null
            || usablePower.PowerDefinition.RechargeRate == RuleDefinitions.RechargeRate.AtWill)
        {
            return;
        }

        if (GetMasterPowersBySubPower(usablePower.PowerDefinition).Empty())
        {
            return;
        }

        action.ActingCharacter.RulesetCharacter.UsePower(usablePower);
    }

    public static void Load()
    {
        ServiceRepository.GetService<IFunctorService>()
            .RegisterFunctor(UseCustomRestPowerFunctorName, new FunctorUseCustomRestPower());
    }

    /**
     * Patch implementation
     * Replaces power with a spell that has sub-spells and then activates bundled power according to selected subspell.
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

        subpowerSelectionModal.Bind(bundle.SubPowers, box.activator, (power, _) =>
        {
            //Note: ideal solution would be to patch `Unbind` of `UsablePowerBox` to auto close selector, instead of this check
            if (box != null && box.powerEngaged != null)
            {
                box.powerEngaged(power);
            }
        }, box.RectTransform);
        subpowerSelectionModal.Show();

        return false;
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

        if (activity.Functor != UseCustomRestPowerFunctorName || activity.StringParameter == null)
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

    public sealed class Bundle
    {
        public Bundle(FeatureDefinitionPower masterPower, IEnumerable<FeatureDefinitionPower> subPowers,
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
        public bool TerminateAll { get; }

        public List<FeatureDefinitionPower> SubPowers { get; }

        // public FeatureDefinitionPower MasterPower { get; }

        //May be needed to hold powers for some native widgets
        // public FeatureDefinitionFeatureSet PowerSet { get; }

        private RulesetSpellRepertoire Repertoire { get; }
    }
}

internal sealed class FunctorUseCustomRestPower : Functor
{
    private bool powerUsed;

    public override IEnumerator Execute(
        [NotNull] FunctorParametersDescription functorParameters,
        FunctorExecutionContext context)
    {
        var functor = this;
        var powerName = functorParameters.StringParameter;
        var power = PowersBundleContext.GetPower(powerName);

        if (power == null && !DatabaseHelper.TryGetDefinition(powerName, out power))
        {
            yield break;
        }

        var ruleChar = functorParameters.RestingHero;
        var usablePower = UsablePowersProvider.Get(power, ruleChar);
        if (usablePower.PowerDefinition.EffectDescription.TargetType == RuleDefinitions.TargetType.Self)
        {
            var fromActor = GameLocationCharacter.GetFromActor(ruleChar);
            var rules = ServiceRepository.GetService<IRulesetImplementationService>();
            
            if (fromActor != null)
            {
                functor.powerUsed = false;
                ServiceRepository.GetService<IGameLocationActionService>();
                
                var actionParams = new CharacterActionParams(fromActor, ActionDefinitions.Id.PowerMain);
                
                actionParams.TargetCharacters.Add(fromActor);
                actionParams.ActionModifiers.Add(new ActionModifier());
                actionParams.RulesetEffect =
                    rules.InstantiateEffectPower(fromActor.RulesetCharacter, usablePower, true);
                actionParams.SkipAnimationsAndVFX = true;
                
                ServiceRepository.GetService<ICommandService>()
                    .ExecuteAction(actionParams, functor.ActionExecuted, false);
                
                while (!functor.powerUsed)
                {
                    yield return null;
                }
            }
            else
            {
                var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
                
                formsParams.FillSourceAndTarget(ruleChar, ruleChar);
                formsParams.FillFromActiveEffect(rules.InstantiateEffectPower(ruleChar, usablePower, false));
                formsParams.effectSourceType = RuleDefinitions.EffectSourceType.Power;

                if (power != null)
                {
                    //rules.ApplyEffectForms(power.EffectDescription.EffectForms, formsParams);
                    ruleChar.UpdateUsageForPowerPool(usablePower, power.CostPerUse);

                    GameConsoleHelper.LogCharacterUsedPower(ruleChar, power,
                        $"Feedback/&{power.Name}UsedWhileTravellingFormat");
                }
            }
        }

        Trace.LogWarning("Unable to assign targets to power");
    }

    private void ActionExecuted(CharacterAction action)
    {
        powerUsed = true;
    }
}
