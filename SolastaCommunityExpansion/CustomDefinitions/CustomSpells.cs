using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ICustomMagicEffectBasedOnCaster
    {
        EffectDescription GetCustomEffect(RulesetCharacter caster);
    }

    public interface IModifySpellEffect
    {
        EffectDescription ModifyEffect(RulesetEffectSpell spell, EffectDescription effect);
    }

    public class SpellWithCasterFeatureDependentEffects : SpellDefinition, ICustomMagicEffectBasedOnCaster
    {
        private readonly List<(List<FeatureDefinition>, EffectDescription)> _featuresEffectList = new();

        public List<(List<FeatureDefinition>, EffectDescription)> FeaturesEffectList => _featuresEffectList;

        public EffectDescription GetCustomEffect(RulesetCharacter caster)
        {
            var casterFeatures = CustomFeaturesContext.FeaturesByType<FeatureDefinition>(caster).ToHashSet();

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
}
