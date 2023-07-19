using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class ModifyEffectDescriptionOnLevels : IModifyEffectDescription
{
    private readonly string className;
    private readonly (int, EffectDescription)[] effects;

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
        return true;
    }

    // public ModifyMagicEffectOnLevels(params (int, EffectDescription)[] effects) : this(null, effects)
    // {
    // }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect)
    {
        var level = string.IsNullOrEmpty(className)
            ? character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel)
            : character.GetClassLevel(className);

        foreach (var (from, upgrade) in effects)
        {
            if (level >= from)
            {
                effectDescription = upgrade;
            }
        }

        return effectDescription;
    }
}
