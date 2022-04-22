using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
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

    //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect for GUI depending on caster properties
    [HarmonyPatch(typeof(GuiSpellDefinition), "EffectDescription", MethodType.Getter)]
    class GuiSpellDefinitionl_EffectDescription
    {
        static void Postfix(ref EffectDescription __result, GuiSpellDefinition __instance)
        {
            __result = CustomFeaturesContext.ModifySpellEffectGui(__result, __instance);
        }
    }
}
