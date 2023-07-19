using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ModifyEffectDescriptionOnLevels : IModifyEffectDescription
{
    private readonly string className;
    private readonly (int level, EffectDescription description)[] effects;

    public ModifyEffectDescriptionOnLevels(string className, params (int, EffectDescription)[] effects)
    {
        this.className = className;
        this.effects = effects;
    }

    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        var level = GetLevel(character);
        return effects.Any(effect => level >= effect.level);
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect)
    {
        var level = GetLevel(character);

        foreach (var (from, upgrade) in effects)
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
        return string.IsNullOrEmpty(className)
            ? character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel)
            : character.GetClassLevel(className);
    }
}
