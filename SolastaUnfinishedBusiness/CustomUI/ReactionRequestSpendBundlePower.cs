using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomUI;

internal sealed class ReactionRequestSpendBundlePower : ReactionRequest, IReactionRequestWithResource
{
    internal const string Name = "ReactionSpendPowerBundle";
    private readonly GuiCharacter guiCharacter;
    private readonly FeatureDefinitionPower masterPower;
    private readonly ActionModifier modifier;
    private readonly GameLocationCharacter target;

    internal ReactionRequestSpendBundlePower([NotNull] CharacterActionParams reactionParams)
        : base(Name, reactionParams)
    {
        target = reactionParams.TargetCharacters[0];
        modifier = reactionParams.ActionModifiers.ElementAtOrDefault(0) ?? new ActionModifier();
        guiCharacter = new GuiCharacter(reactionParams.ActingCharacter);
        masterPower = ((RulesetEffectPower)reactionParams.RulesetEffect).PowerDefinition;
        BuildSuboptions();
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

            var subPowers = masterPower.GetBundle()?.SubPowers;

            return subPowers?.FindIndex(p => p == power) ?? -1;
        }
    }


    [NotNull] public override string SuboptionTag => "PowerBundle";

    public override bool IsStillValid =>
        ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters.Contains(target) &&
        target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false };

    public ICustomReactionResource Resource { get; set; }

    private void BuildSuboptions()
    {
        SubOptionsAvailability.Clear();

        var actingCharacter = ReactionParams.ActingCharacter;
        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var bundle = masterPower.GetBundle();
        var selected = false;

        if (bundle == null)
        {
            return;
        }

        ReactionParams.SpellRepertoire = new RulesetSpellRepertoire();

        var i = 0;

        foreach (var power in bundle.SubPowers
                     .Where(x => CanUsePower(rulesetCharacter, x) &&
                                 rulesetCharacter.GetFeaturesByType<FeatureDefinitionPower>().Contains(x)))
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
        var powerValidators = power.GetAllSubFeaturesOfType<IPowerUseValidity>();

        if (powerValidators.Any(v => !v.CanUsePower(character, power)))
        {
            return false;
        }

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

        var rulesService = ServiceRepository.GetService<IRulesetImplementationService>();
        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var usablePower = UsablePowersProvider.Get(power, rulesetCharacter);
        var powerEffect = rulesService
            .InstantiateEffectPower(rulesetCharacter, usablePower, false)
            .AddAsActivePowerToSource();

        ReactionParams.RulesetEffect = powerEffect;

        if (power == null)
        {
            return;
        }

        var effectDescription = power.EffectDescription;

        // We let targets collection be empty with RangeType MeleeHit on sub power so it can be handled manually in code
        // i.e.: Cunning / Devious Strike powers that need to be applied on attack finished not before hit
        if (effectDescription.RangeType == RuleDefinitions.RangeType.MeleeHit)
        {
            return;
        }

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

            if (!effectDescription.IsSingleTarget || targets <= 1)
            {
                return;
            }

            while (target != null && modifier != null && targetCharacters.Count < targets)
            {
                targetCharacters.Add(target);
                modifiers.Add(modifier);
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

        return Gui.Format(format, guiCharacter.Name);
    }

    public override string FormatReactTitle()
    {
        var format = $"Reaction/&ReactionSpendPowerBundle{ReactionParams.StringParameter}ReactTitle";

        return Gui.Format(format, guiCharacter.Name);
    }

    public override string FormatReactDescription()
    {
        var format = $"Reaction/&ReactionSpendPowerBundle{ReactionParams.StringParameter}ReactDescription";

        return Gui.Format(format, guiCharacter.Name);
    }

    public override void OnSetInvalid()
    {
        base.OnSetInvalid();
        ReactionParams.RulesetEffect?.Terminate(false);
    }
}
