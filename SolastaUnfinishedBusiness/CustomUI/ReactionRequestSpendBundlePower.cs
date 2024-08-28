using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomUI;

internal sealed class ReactionRequestSpendBundlePower : ReactionRequest, IReactionRequestWithResource
{
    internal const string Name = "ReactionSpendPowerBundle";
    private readonly GuiCharacter _guiCharacter;
    private readonly FeatureDefinitionPower _masterPower;
    private readonly ActionModifier _modifier;
    private readonly GameLocationCharacter _target;

    internal ReactionRequestSpendBundlePower([NotNull] CharacterActionParams reactionParams)
        : base(Name, reactionParams)
    {
        _target = reactionParams.TargetCharacters[0];
        _modifier = reactionParams.ActionModifiers.ElementAtOrDefault(0) ?? new ActionModifier();
        _guiCharacter = new GuiCharacter(reactionParams.ActingCharacter);
        _masterPower = ((RulesetEffectPower)reactionParams.RulesetEffect).PowerDefinition;
        BuildSuboptions();
    }

    public override int SelectedSubOption
    {
        get
        {
            var power = (ReactionParams.RulesetEffect as RulesetEffectPower)?.PowerDefinition;

            if (!power)
            {
                return -1;
            }

            var subPowers = _masterPower.GetBundle()?.SubPowers;

            return subPowers?.FindIndex(p => p == power) ?? -1;
        }
    }


    [NotNull] public override string SuboptionTag => "PowerBundle";

    public override bool IsStillValid =>
        ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters.Contains(_target) &&
        _target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false };

    public ICustomReactionResource Resource { get; set; }

    private void BuildSuboptions()
    {
        SubOptionsAvailability.Clear();

        var actingCharacter = ReactionParams.ActingCharacter;
        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var bundle = _masterPower.GetBundle();
        var selected = false;

        if (bundle == null)
        {
            return;
        }

        ReactionParams.SpellRepertoire = new RulesetSpellRepertoire();

        var i = 0;

        foreach (var power in bundle.SubPowers
                     .Intersect(rulesetCharacter.UsablePowers
                         .Select(x => x.PowerDefinition))
                     .Where(x => CanUsePower(rulesetCharacter, x)))
        {
            reactionParams.SpellRepertoire.KnownSpells.Add(PowerBundle.GetSpell(power));
            SubOptionsAvailability.Add(i, true);

            if (!selected)
            {
                SelectSubOption(i);
                selected = true;
            }

            i++;
        }
    }

    private static bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
    {
        var powerValidators = power.GetAllSubFeaturesOfType<IValidatePowerUse>();

        if (powerValidators.Any(v => !v.CanUsePower(character, power)))
        {
            return false;
        }

        // must use GetRemainingPowerUses as power could be a Shared Pool
        return character.GetRemainingPowerUses(power) > 0;
    }

    public override void SelectSubOption(int option)
    {
        ReactionParams.RulesetEffect?.Terminate(false);

        var targetCharacters = ReactionParams.TargetCharacters;
        var modifiers = ReactionParams.ActionModifiers;

        targetCharacters.Clear();
        modifiers.Clear();

        if (option < 0)
        {
            return;
        }

        var actingCharacter = ReactionParams.ActingCharacter;

        ReactionParams.ActionDefinition = ServiceRepository.GetService<IGameLocationActionService>()
            .AllActionDefinitions[ActionDefinitions.Id.SpendPower];

        var spell = ReactionParams.SpellRepertoire.KnownSpells[option];
        var power = PowerBundle.GetPower(spell);
        var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var usablePower = PowerProvider.Get(power, rulesetCharacter);
        var powerEffect = implementationService.InstantiateEffectPower(rulesetCharacter, usablePower, false);

        ReactionParams.RulesetEffect = powerEffect;

        if (!power)
        {
            return;
        }

        var effectDescription = power.EffectDescription;

        // We let targets collection be empty with RangeType MeleeHit on sub power so it can be handled manually in code
        // i.e.: Cunning / Devious Strike powers that need to be applied on attack finished not before hit
        if (effectDescription.RangeType == RangeType.MeleeHit)
        {
            return;
        }

        if (effectDescription.RangeType == RangeType.Self
            || effectDescription.TargetType == TargetType.Self)
        {
            targetCharacters.Add(actingCharacter);
            modifiers.Add(_modifier);
        }
        else
        {
            targetCharacters.Add(_target);
            modifiers.Add(_modifier);

            var targets = powerEffect.ComputeTargetParameter();

            if (!effectDescription.IsSingleTarget || targets <= 1)
            {
                return;
            }

            while (_target != null && _modifier != null && targetCharacters.Count < targets)
            {
                targetCharacters.Add(_target);
                modifiers.Add(_modifier);
            }
        }
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&ReactionSpendPowerBundle{ReactionParams.StringParameter}Title");
    }

    public override string FormatDescription()
    {
        var format = $"Reaction/&ReactionSpendPowerBundle{ReactionParams.StringParameter}Description";

        return Gui.Format(format, _guiCharacter.Name);
    }

    public override string FormatReactTitle()
    {
        var format = $"Reaction/&ReactionSpendPowerBundle{ReactionParams.StringParameter}ReactTitle";

        return Gui.Format(format, _guiCharacter.Name);
    }

    public override string FormatReactDescription()
    {
        var format = $"Reaction/&ReactionSpendPowerBundle{ReactionParams.StringParameter}ReactDescription";

        return Gui.Format(format, _guiCharacter.Name);
    }

    public override void OnSetInvalid()
    {
        base.OnSetInvalid();
        ReactionParams.RulesetEffect?.Terminate(false);
    }
}
