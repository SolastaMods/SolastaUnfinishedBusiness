using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ModifyEffectDescriptionOnLevels : IModifyEffectDescription
{
    private readonly CharacterClassDefinition _class;
    private readonly (int level, EffectDescription description)[] _effects;
    private readonly FeatureDefinitionPower _power;

    public ModifyEffectDescriptionOnLevels(
        CharacterClassDefinition klass,
        FeatureDefinitionPower power,
        params (int, EffectDescription)[] effects)
    {
        _class = klass;
        _power = power;
        _effects = effects;
    }

    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        var level = GetLevel(character);

        return definition == _power && _effects.Any(effect => level >= effect.level);
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
        return _class == null
            ? character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel)
            : character.GetClassLevel(_class);
    }
}
