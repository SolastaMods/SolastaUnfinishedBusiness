using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class SpellWithCasterFeatureDependentEffects : SpellDefinition, ICustomMagicEffectBasedOnCaster
    {
        private readonly List<(List<FeatureDefinition>, EffectDescription)> _featuresEffectList = new();

        public List<(List<FeatureDefinition>, EffectDescription)> FeaturesEffectList => _featuresEffectList;

        public EffectDescription GetCustomEffect(RulesetCharacter caster)
        {
            var casterFeatures = caster.GetFeaturesByType<FeatureDefinition>().ToHashSet();

            foreach (var (featureDefinitions, customEffect) in _featuresEffectList)
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
        public delegate EffectDescription ModifySpellEffectDelegate(RulesetEffectSpell spell, EffectDescription effect);

        private ModifySpellEffectDelegate _spellModifier;

        public ModifySpellEffectDelegate SpellModifier { get => _spellModifier; set => _spellModifier = value; }

        public EffectDescription ModifyEffect(RulesetEffectSpell spell, EffectDescription effect)
        {
            return _spellModifier != null ? _spellModifier(spell, effect) : effect;
        }
    }
    internal class UpgradeEffectFromLevel : ICustomMagicEffectBasedOnCaster
    {
        private readonly EffectDescription _upgraded;
        private readonly int _level;

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
