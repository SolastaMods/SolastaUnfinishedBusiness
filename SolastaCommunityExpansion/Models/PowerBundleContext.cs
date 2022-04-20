using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    public class PowerBundleContext
    {
        private static readonly Guid GuidNamespace = new("d99cec61-31b8-42a3-a5d6-082369fadaaf");

        private static readonly Dictionary<SpellDefinition, FeatureDefinitionPower> Spells2Powers = new();
        private static readonly Dictionary<FeatureDefinitionPower, SpellDefinition> Powers2Spells = new();
        private static readonly Dictionary<FeatureDefinitionPower, List<FeatureDefinitionPower>> Bundles = new();

        public static void RegisterPowerBundle(FeatureDefinitionPower masterPower,
            IEnumerable<FeatureDefinitionPower> subPowers)
        {
            if (Bundles.ContainsKey(masterPower))
            {
                throw new Exception($"Bundle '{masterPower.name}' already registered!");
            }

            var subPowersList = subPowers.ToList();
            Bundles.Add(masterPower, subPowersList);

            var masterSpell = RegisterPower(masterPower);
            List<SpellDefinition> subSpells = new();

            foreach (var subPower in subPowersList)
            {
                subSpells.Add(RegisterPower(subPower));
            }

            masterSpell.AddSubspellsList(subSpells);
        }

        public static List<FeatureDefinitionPower> GetBundle(FeatureDefinitionPower master)
        {
            return master ? Bundles.GetValueOrDefault(master) : null;
        }

        public static List<FeatureDefinitionPower> GetBundle(SpellDefinition master)
        {
            return GetBundle(GetPower(master));
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
            return Spells2Powers.GetValueOrDefault(spell);
        }

        public static FeatureDefinitionPower GetPower(string name)
        {
            return Powers2Spells.Keys.FirstOrDefault(p => p.Name == name);
        }

        public static SpellDefinition GetSpell(FeatureDefinitionPower power)
        {
            return Powers2Spells.GetValueOrDefault(power);
        }

        public static List<SpellDefinition> GetSubSpells(FeatureDefinitionPower masterPower)
        {
            if (masterPower != null)
            {
                var subPowers = GetBundle(masterPower);
                if (subPowers != null)
                {
                    List<SpellDefinition> spells = new();
                    foreach (var power in subPowers)
                    {
                        spells.Add(GetSpell(power));
                    }

                    return spells;
                }
            }

            return null;
        }

        public static void Load()
        {
            ServiceRepository.GetService<IFunctorService>()
                .RegisterFunctor("UseCustomRestPower", new FunctorUseCustomRestPower());
        }
    }

    internal class FunctorUseCustomRestPower : Functor
    {
        private bool powerUsed;

        public override IEnumerator Execute(
            FunctorParametersDescription functorParameters,
            FunctorExecutionContext context)
        {
            FunctorUseCustomRestPower functorUsePower = this;
            var fromActor = GameLocationCharacter.GetFromActor(functorParameters.RestingHero);
            if (fromActor != null)
            {
                var power = PowerBundleContext.GetPower(functorParameters.StringParameter);
                if (power != null)
                {
                    var usablePower = UsablePowersProvider.Get(power, functorParameters.RestingHero);
                    if (usablePower.PowerDefinition.EffectDescription.TargetType == RuleDefinitions.TargetType.Self)
                    {
                        functorUsePower.powerUsed = false;
                        ServiceRepository.GetService<IGameLocationActionService>();
                        var actionParams = new CharacterActionParams(fromActor, ActionDefinitions.Id.PowerMain);
                        actionParams.TargetCharacters.Add(fromActor);
                        actionParams.ActionModifiers.Add(new ActionModifier());
                        var service = ServiceRepository.GetService<IRulesetImplementationService>();
                        actionParams.RulesetEffect =
                            service.InstantiateEffectPower(fromActor.RulesetCharacter, usablePower, true);
                        actionParams.SkipAnimationsAndVFX = true;
                        ServiceRepository.GetService<ICommandService>().ExecuteAction(actionParams,
                            functorUsePower.ActionExecuted, false);
                        while (!functorUsePower.powerUsed)
                            yield return null;
                    }
                }

                Trace.LogWarning("Unable to assign targets to power");
            }
        }

        private void ActionExecuted(CharacterAction action) => this.powerUsed = true;
    }
}
