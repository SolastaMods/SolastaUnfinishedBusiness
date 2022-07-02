using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static void RegisterPowerBundle(FeatureDefinitionPower masterPower, bool terminateAll,
        params FeatureDefinitionPower[] subPowers)
    {
        RegisterPowerBundle(masterPower, terminateAll, subPowers.ToList());
    }

    public static void RegisterPowerBundle(FeatureDefinitionPower masterPower, bool terminateAll,
        IEnumerable<FeatureDefinitionPower> subPowers)
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
        List<SpellDefinition> subSpells = new();

        foreach (var subPower in bundle.SubPowers)
        {
            subSpells.Add(RegisterPower(subPower));
        }

        masterSpell.SubspellsList.AddRange(subSpells);
    }


    public static Bundle GetBundle(FeatureDefinitionPower master)
    {
        if (master == null)
        {
            return null;
        }

        return Bundles.TryGetValue(master, out var result) ? result : null;
    }

    public static Bundle GetBundle(SpellDefinition master)
    {
        return GetBundle(GetPower(master));
    }

    public static bool IsBundlePower(this FeatureDefinitionPower power)
    {
        return Bundles.ContainsKey(power);
    }

    public static List<FeatureDefinitionPower> GetBundleSubPowers(this FeatureDefinitionPower master)
    {
        return GetBundle(master)?.SubPowers;
    }

    public static List<FeatureDefinitionPower> GetBundleSubPowers(SpellDefinition master)
    {
        return GetBundleSubPowers(GetPower(master));
    }

    private static SpellDefinition RegisterPower(FeatureDefinitionPower power)
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

    public static FeatureDefinitionPower GetPower(SpellDefinition spell)
    {
        return Spells2Powers.TryGetValue(spell, out var result) ? result : null;
    }

    public static FeatureDefinitionPower GetPower(string name)
    {
        return Powers2Spells.Keys.FirstOrDefault(p => p.Name == name);
    }

    public static List<FeatureDefinitionPower> GetMasterPowersBySubPower(FeatureDefinitionPower subPower)
    {
        return Bundles
            .Where(e => e.Value.SubPowers.Contains(subPower))
            .Select(e => e.Key)
            .ToList();
    }

    public static SpellDefinition GetSpell(FeatureDefinitionPower power)
    {
        return Powers2Spells.TryGetValue(power, out var result) ? result : null;
    }

    public static List<SpellDefinition> GetSubSpells(FeatureDefinitionPower masterPower)
    {
        if (masterPower == null)
        {
            return null;
        }

        var subPowers = GetBundleSubPowers(masterPower);

        return subPowers?.Select(GetSpell).ToList();
    }

    // Bundled sub-powers usually are not added to the character, so their UsblePower lacks class or race oringin
    // This means that CharacterActionSpendPower will not call `UsePower` on them
    // This method fixes that
    public static void SpendBundledPowerIfNeeded(CharacterActionSpendPower action)
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
        FunctorParametersDescription functorParameters,
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

                rules.ApplyEffectForms(power.EffectDescription.EffectForms, formsParams);
                ruleChar.UpdateUsageForPowerPool(usablePower, power.CostPerUse);

                GameConsoleHelper.LogCharacterUsedPower(ruleChar, power,
                    $"Feedback/&{power.Name}UsedWhileTravellingFormat");
            }
        }

        Trace.LogWarning("Unable to assign targets to power");
    }

    private void ActionExecuted(CharacterAction action)
    {
        powerUsed = true;
    }
}
