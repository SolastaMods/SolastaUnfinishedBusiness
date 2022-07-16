using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.Models;

public static class PowerBundleContext
{
    internal const string UseCustomRestPowerFunctorName = "UseCustomRestPower";
    private static readonly Guid GuidNamespace = new("d99cec61-31b8-42a3-a5d6-082369fadaaf");

    private static readonly Dictionary<SpellDefinition, FeatureDefinitionPower> Spells2Powers = new();
    private static readonly Dictionary<FeatureDefinitionPower, SpellDefinition> Powers2Spells = new();
    private static readonly Dictionary<FeatureDefinitionPower, Bundle> Bundles = new();

    public static void RegisterPowerBundle([NotNull] FeatureDefinitionPower masterPower, bool terminateAll,
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

        var bundle = new Bundle();
        bundle.SubPowers.AddRange(subPowers);
        bundle.TerminateAll = terminateAll;

        Bundles.Add(masterPower, bundle);

        var masterSpell = RegisterPower(masterPower);
        var subSpells = bundle.SubPowers.Select(RegisterPower).ToList();

        masterSpell.SubspellsList.AddRange(subSpells);
    }


    [CanBeNull]
    public static Bundle GetBundle([CanBeNull] FeatureDefinitionPower master)
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

    [CanBeNull]
    public static List<FeatureDefinitionPower> GetBundleSubPowers(this FeatureDefinitionPower master)
    {
        return GetBundle(master)?.SubPowers;
    }

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

        var spell = SpellDefinitionBuilder.Create("Spell" + power.name, GuidNamespace)
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
        return Powers2Spells.Keys.FirstOrDefault(p => p.Name == name);
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
        if (activePower is not {OriginItem: null})
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

    public sealed class Bundle
    {
        /**
             * If set to true will terminate all powers in this bundle when 1 is terminated, so only one power
             * from this bundle can be in effect
             */
        public bool TerminateAll { get; internal set; }

        public List<FeatureDefinitionPower> SubPowers { get; } = new();
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
        var power = PowerBundleContext.GetPower(powerName);

        if (power == null && !DatabaseHelper.TryGetDefinition(powerName, null, out power))
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
                    rules.ApplyEffectForms(power.EffectDescription.EffectForms, formsParams);
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
