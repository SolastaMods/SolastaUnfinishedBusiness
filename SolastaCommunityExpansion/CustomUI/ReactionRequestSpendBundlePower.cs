using System.Linq;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomUI
{
    public class ReactionRequestSpendBundlePower : ReactionRequest
    {
        private const string Name = "SpendPowerBundle";

        private readonly GameLocationCharacter target;
        private readonly ActionModifier modifier;
        private readonly GuiCharacter guiCharacter;
        private readonly FeatureDefinitionPower masterPower;

        public ReactionRequestSpendBundlePower(CharacterActionParams reactionParams)
            : base(Name, reactionParams)
        {
            target = reactionParams.TargetCharacters[0];
            modifier = reactionParams.ActionModifiers.ElementAtOrDefault(0) ?? new ActionModifier();
            guiCharacter = new GuiCharacter(reactionParams.ActingCharacter);
            masterPower = ((RulesetEffectPower)reactionParams.RulesetEffect).PowerDefinition;
            BuildSuboptions();
        }

        private void BuildSuboptions()
        {
            SubOptionsAvailability.Clear();

            var reactionParams = ReactionParams;
            var actingCharacter = reactionParams.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;


            var subPowers = masterPower.GetBundleSubPowers();
            var selected = false;

            reactionParams.SpellRepertoire = new RulesetSpellRepertoire();
            var i = 0;
            foreach (var p in subPowers)
            {
                reactionParams.SpellRepertoire.KnownSpells.Add(PowerBundleContext.GetSpell(p));
                var canUsePower = CanUsePower(rulesetCharacter, p);
                SubOptionsAvailability.Add(i, canUsePower);

                if (canUsePower && !selected)
                {
                    SelectSubOption(i);
                    selected = true;
                }

                i++;
            }
        }

        private static bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            var powerValidators = power.GetAllSubFeaturesOfType<IPowerUseValidity>();
            if (powerValidators.Any(v => !v.CanUsePower(character)))
            {
                return false;
            }

            return character.GetRemainingPowerUses(power) > 0;
        }

        public override int SelectedSubOption
        {
            get
            {
                var power = (ReactionParams.RulesetEffect as RulesetEffectPower)?.PowerDefinition;
                if (power == null)
                {
                    return -1;
                }

                var subPowers = PowerBundleContext.GetBundle(masterPower)?.SubPowers;
                return subPowers?.FindIndex(p => p == power) ?? -1;
            }
        }

        public override void SelectSubOption(int option)
        {
            ReactionParams.RulesetEffect?.Terminate(false);
            var reactionParams = ReactionParams;

            var targetCharacters = reactionParams.TargetCharacters;
            var modifiers = reactionParams.ActionModifiers;

            targetCharacters.Clear();
            modifiers.Clear();

            if (option < 0)
            {
                return;
            }

            var actingCharacter = reactionParams.ActingCharacter;
            reactionParams.ActionDefinition = ServiceRepository.GetService<IGameLocationActionService>()
                .AllActionDefinitions[ActionDefinitions.Id.SpendPower];

            var spell = reactionParams.SpellRepertoire.KnownSpells[option];
            var power = PowerBundleContext.GetPower(spell);

            var rulesService = ServiceRepository.GetService<IRulesetImplementationService>();
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(power, rulesetCharacter);
            var powerEffect = rulesService.InstantiateEffectPower(rulesetCharacter, usablePower, false);

            ReactionParams.RulesetEffect = powerEffect;

            var effectDescription = power.EffectDescription;
            if (effectDescription.RangeType == RuleDefinitions.RangeType.Self
                || effectDescription.TargetType == RuleDefinitions.TargetType.Self)
            {
                targetCharacters.Add(actingCharacter);
                modifiers.Add(modifier);
            }
            else
            {
                targetCharacters.Add(target);
                modifiers.Add(modifier);

                var targets = powerEffect.ComputeTargetParameter();
                if (effectDescription.IsSingleTarget && targets > 1)
                {
                    while (target != null && modifier != null && targetCharacters.Count < targets)
                    {
                        targetCharacters.Add(target);
                        modifiers.Add(modifier);
                    }
                }
            }
        }


        public override string SuboptionTag => "PowerBundle";

        public override bool IsStillValid =>
            ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters
                .Contains(target) && !target.RulesetCharacter.IsDeadOrDyingOrUnconscious;

        public override string FormatTitle() =>
            Gui.Localize($"Reaction/&SpendPowerBundle{ReactionParams.StringParameter}Title");

        public override string FormatDescription()
        {
            var format = $"Reaction/&SpendPowerBundle{ReactionParams.StringParameter}Description";
            return Gui.Format(format, guiCharacter.Name);
        }

        public override string FormatReactTitle()
        {
            var format = $"Reaction/&SpendPowerBundle{ReactionParams.StringParameter}ReactTitle";
            return Gui.Format(format, guiCharacter.Name);
        }

        public override string FormatReactDescription()
        {
            var format = $"Reaction/&SpendPowerBundle{ReactionParams.StringParameter}ReactDescription";
            return Gui.Format(format, guiCharacter.Name);
        }

        public override void OnSetInvalid()
        {
            base.OnSetInvalid();
            ReactionParams.RulesetEffect?.Terminate(false);
        }
    }
}