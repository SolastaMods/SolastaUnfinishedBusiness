using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ICustomMagicEffectBasedOnCaster
    {
        EffectDescription GetCustomEffect(RulesetEffectSpell spell);
    }

    public interface IModifySpellEffect
    {
        EffectDescription ModifyEffect(RulesetEffectSpell spell, EffectDescription effect);
    }

    public class SpellWithCasterFeatureDependentEffects : SpellDefinition, ICustomMagicEffectBasedOnCaster
    {
        public List<(List<FeatureDefinition>, EffectDescription)> featuresEffectList = new();

        public EffectDescription GetCustomEffect(RulesetEffectSpell spell)
        {
            var casterFeatures = CustomFeaturesContext.FeaturesByType<FeatureDefinition>(spell.Caster).ToHashSet();

            foreach (var (featureDefinitions, customEffect) in featuresEffectList)
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

    //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect depending on some caster properties
    //and IModifySpellEffect which modifies existing effect (changing elemental damage type for example)
    [HarmonyPatch(typeof(RulesetEffectSpell), "EffectDescription", MethodType.Getter)]
    class RulesetEffectSpell_EffectDescription
    {
        static void Postfix(ref EffectDescription __result, RulesetEffectSpell __instance)
        {
            __result = CustomFeaturesContext.ModifySpellEffect(__result, __instance);
        }
    }
}
