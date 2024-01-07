using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ModifyEffectDescriptionOnLevels(
    CharacterClassDefinition klass,
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    FeatureDefinitionPower power,
    params (int, EffectDescription)[] effects)
    : IModifyEffectDescription
{
    private readonly (int level, EffectDescription description)[] _effects = effects;

    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        var level = GetLevel(character);

        return definition == power && _effects.Any(effect => level >= effect.level);
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect)
    {
        var level = GetLevel(character);

        foreach (var (from, upgrade) in _effects)
        {
            if (level >= from)
            {
                effectDescription = upgrade;
            }
        }

        return effectDescription;
    }

    private int GetLevel(RulesetCharacter character)
    {
        return klass == null
            ? character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel)
            : character.GetClassLevel(klass);
    }
}
