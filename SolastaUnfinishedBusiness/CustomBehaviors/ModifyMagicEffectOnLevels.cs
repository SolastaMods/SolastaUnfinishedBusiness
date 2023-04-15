using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

public class ModifyMagicEffectOnLevels : IModifyMagicEffect
{
    private readonly string className;
    private readonly (int, EffectDescription)[] effects;

    public ModifyMagicEffectOnLevels(string className, params (int, EffectDescription)[] effects)
    {
        this.className = className;
        this.effects = effects;
    }

    // public ModifyMagicEffectOnLevels(params (int, EffectDescription)[] effects) : this(null, effects)
    // {
    // }

    public EffectDescription ModifyEffect(BaseDefinition definition, EffectDescription effect,
        RulesetCharacter character)
    {
        var level = string.IsNullOrEmpty(className)
            ? character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel)
            : character.GetClassLevel(className);

        foreach (var (from, upgrade) in effects)
        {
            if (level >= from)
            {
                effect = upgrade;
            }
        }

        return effect;
    }
}
