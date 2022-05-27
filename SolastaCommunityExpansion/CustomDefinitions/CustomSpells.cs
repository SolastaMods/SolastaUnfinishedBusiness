using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class SpellDefinitionWithDependentEffects : SpellDefinition, ICustomMagicEffectBasedOnCaster
    {
        public List<(List<FeatureDefinition>, EffectDescription)> FeaturesEffectList { get; } = new();

        public EffectDescription GetCustomEffect(RulesetCharacter caster)
        {
            var casterFeatures = caster.GetFeaturesByType<FeatureDefinition>().ToHashSet();

            foreach (var (featureDefinitions, customEffect) in FeaturesEffectList)
            {
                if (featureDefinitions.All(f => casterFeatures.Contains(f)))
                {
                    return customEffect;
                }
            }

            return EffectDescription;
        }
    }

    public class SpellModifyingFeatureDefinition : FeatureDefinition, IModifySpellEffect
    {
        public delegate EffectDescription ModifySpellEffectDelegate(SpellDefinition spell, EffectDescription effect,
            RulesetCharacter caster);

        public ModifySpellEffectDelegate SpellModifier { get; set; }

        public EffectDescription ModifyEffect(SpellDefinition spell, EffectDescription effect, RulesetCharacter caster)
        {
            return SpellModifier != null ? SpellModifier(spell, effect, caster) : effect;
        }
    }

    internal class UpgradeEffectFromLevel : ICustomMagicEffectBasedOnCaster
    {
        private readonly int _level;
        private readonly EffectDescription _upgraded;

        public UpgradeEffectFromLevel(EffectDescription upgraded, int level)
        {
            _upgraded = upgraded;
            _level = level;
        }

        public EffectDescription GetCustomEffect(RulesetCharacter caster)
        {
            var casterLevel = caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            if (casterLevel < _level) { return null; }

            return _upgraded;
        }
    }
}
