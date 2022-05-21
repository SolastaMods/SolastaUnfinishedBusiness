using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IConditionalMovementModifier
{
    void AddModifiers(RulesetCharacter character, List<FeatureDefinition> modifiers);
}
