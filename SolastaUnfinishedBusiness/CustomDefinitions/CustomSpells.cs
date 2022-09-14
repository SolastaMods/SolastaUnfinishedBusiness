using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

// public class SpellDefinitionWithDependentEffects : SpellDefinition, ICustomMagicEffectBasedOnCaster
// {
//     private List<(List<FeatureDefinition>, EffectDescription)> FeaturesEffectList { get; } = new();
//
//     public EffectDescription GetCustomEffect(RulesetCharacter caster)
//     {
//         var casterFeatures = caster.GetFeaturesByType<FeatureDefinition>().ToHashSet();
//
//         foreach (var (featureDefinitions, customEffect) in FeaturesEffectList)
//         {
//             if (featureDefinitions.All(f => casterFeatures.Contains(f)))
//             {
//                 return customEffect;
//             }
//         }
//
//         return EffectDescription;
//     }
// }

// public class SpellModifyingFeatureDefinition : FeatureDefinition, IModifySpellEffect
// {
//     public delegate EffectDescription ModifySpellEffectDelegate(SpellDefinition spell, EffectDescription effect,
//         RulesetCharacter caster);
//
//     public ModifySpellEffectDelegate SpellModifier { get; set; }
//
//     public EffectDescription ModifyEffect(SpellDefinition spell, EffectDescription effect, RulesetCharacter caster)
//     {
//         return SpellModifier != null ? SpellModifier(spell, effect, caster) : effect;
//     }
// }

internal sealed class UpgradeEffectFromLevel : ICustomMagicEffectBasedOnCaster
{
    private readonly int _level;
    private readonly EffectDescription _upgraded;

    public UpgradeEffectFromLevel(EffectDescription upgraded, int level)
    {
        _upgraded = upgraded;
        _level = level;
    }

    [CanBeNull]
    public EffectDescription GetCustomEffect([NotNull] RulesetCharacter caster)
    {
        var casterLevel = caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
        return casterLevel < _level ? null : _upgraded;
    }
}
