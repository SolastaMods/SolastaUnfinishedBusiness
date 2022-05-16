using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;

namespace SolastaCommunityExpansion.Features;

public interface IConditionalMovementModifier
{
    void AddModifiers(RulesetCharacter character, List<FeatureDefinition> modifiers);
}

public class ConditionalMovementModifier : IConditionalMovementModifier
{
    private readonly FeatureDefinition modifier;
    private readonly CharacterValidator[] validators;

    public ConditionalMovementModifier(FeatureDefinition modifier, params CharacterValidator[] validators)
    {
        this.modifier = modifier;
        this.validators = validators;
    }

    public void AddModifiers(RulesetCharacter character, List<FeatureDefinition> modifiers)
    {
        if (character.IsValid(validators))
        {
            modifiers.Add(modifier);
        }
    }
}