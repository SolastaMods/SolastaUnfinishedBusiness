using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions;

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
