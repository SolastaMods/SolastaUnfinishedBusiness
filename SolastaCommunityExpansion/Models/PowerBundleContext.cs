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

    /**
     * Patch implementation
     * Replaces power with a spell that has sub-spells and then activates bundled power according to selected subspell.
     * Returns true if nothing needs (or can) be done.
     */
    internal static bool PowerBoxActivated(UsablePowerBox box)
    {
        var masterPower = box.usablePower.PowerDefinition;

        if (GetBundle(masterPower) == null)
        {
            return true;
        }

        if (box.powerEngaged == null)
        {
            return true;
        }

        var masterSpell = GetSpell(masterPower);

        if (masterSpell == null)
        {
            return true;
        }

        var repertoire = new RulesetSpellRepertoire();
        repertoire.KnownSpells.AddRange(masterSpell.SubspellsList);

        //TODO: find a way to re-use same widget, but without resorting to fake spells creation
        var subspellSelectionModalScreen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
        subspellSelectionModalScreen.Bind(masterSpell, box.activator, repertoire, (_, spell, _) =>
        {
            var power = GetPower(spell);
            var engagedHandler = box.powerEngaged;
            var activator = box.activator;

            engagedHandler(UsablePowersProvider.Get(power, activator));
        }, 0, box.RectTransform);
        subspellSelectionModalScreen.Show();

        return false;
    }

    /**
     * Patch implementation
     * Replaces tooltip and name on the item with ones from actual sub-power that is being substituted
     */
    internal static bool ModifySubspellItemBind(
        SubspellItem __instance,
        RulesetCharacter caster,
        SpellDefinition spellDefinition,
        int index,
        SubspellItem.OnActivateHandler onActivate)
    {
        var power = GetPower(spellDefinition);

        if (power == null)
        {
            return true;
        }

        __instance.index = index;

        var guiPowerDefinition =
            ServiceRepository.GetService<IGuiWrapperService>().GetGuiPowerDefinition(power.Name);
        __instance.spellTitle.Text = guiPowerDefinition.Title;

        //add info about remaining spell slots if powers consume them
        // var usablePower = caster.GetPowerFromDefinition(power);
        // if (usablePower != null && power.rechargeRate == RuleDefinitions.RechargeRate.SpellSlot)
        // {
        //     var power_info = Helpers.Accessors.getNumberOfSpellsFromRepertoireOfSpecificSlotLevelAndFeature(power.costPerUse, caster, power.spellcastingFeature);
        //     instance.spellTitle.Text += $"   [{power_info.remains}/{power_info.total}]";
        // }

        __instance.tooltip.TooltipClass = guiPowerDefinition.TooltipClass;
        __instance.tooltip.Content = power.GuiPresentation.Description;
        __instance.tooltip.DataProvider = guiPowerDefinition;
        __instance.tooltip.Context = caster;

        __instance.onActivate = onActivate;

        return false;
    }

    /**
     * Patch implementation
     * Replaces how subspell activates for bundled powers
     */
    internal static bool CheckSubSpellActivated(SubspellSelectionModal selection, int index)
    {
        var masterPower = GetPower(selection.masterSpell);

        if (masterPower == null || selection.spellCastEngaged == null)
        {
            return true;
        }

        var repertoire = selection.spellRepertoire;
        selection.spellCastEngaged(repertoire, repertoire.KnownSpells[index], selection.slotLevel);

        selection.Hide();

        return false;
    }

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
        var masterSpell = masterPower ? GetSpell(masterPower) : null;

        if (!masterSpell)
        {
            return true;
        }

        var repertoire = new RulesetSpellRepertoire();
        var subspellSelectionModalScreen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
        var handler = new SpellsByLevelBox.SpellCastEngagedHandler((_, spell, _) =>
        {
            instance.button.interactable = false;

            var power = GetPower(spell)?.Name;
            if (power != null)
            {
                ServiceRepository.GetService<IGameRestingService>()
                    .ExecuteAsync(ExecuteAsync(instance, power), power);
            }
        });

        repertoire.KnownSpells.AddRange(masterSpell.SubspellsList);

        subspellSelectionModalScreen.Bind(masterSpell, instance.Hero, repertoire, handler, 0, instance.RectTransform);
        subspellSelectionModalScreen.Show();

        return false;
    }

    private static IEnumerator ExecuteAsync(AfterRestActionItem item, string powerName)
    {
        item.executing = true;

        var parameters = new FunctorParametersDescription {RestingHero = item.Hero, StringParameter = powerName};
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