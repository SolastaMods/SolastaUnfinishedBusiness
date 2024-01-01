using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Models;

public static class EffectsContext
{
    internal static readonly
        Dictionary<EffectHelpers.EffectType, Dictionary<string, List<(string, EffectParticleParameters)>>> Effects =
            new();

    internal static readonly Dictionary<string, List<(string, BaseDefinition)>> ConditionEffects = new();

    internal static bool Dirty { get; private set; } = true;

    internal static void DumpEffects()
    {
        DumpCasterEffects();
        DumpConditionEffects();
        DumpEffectEffects();
        DumpImpactEffects();
        DumpZoneEffects();

        Dirty = false;
    }

    private static void DumpCasterEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Caster,
            new Dictionary<string, List<(string, EffectParticleParameters)>>());

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

        foreach (var spell in spells
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = spell.Name;
            var effectParticleParameters = spell.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.casterParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Caster]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Caster][effectReferenceGuid].Add((name, effectParticleParameters));
        }

        foreach (var power in powers
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = power.Name;
            var effectParticleParameters = power.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.casterParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Caster]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Caster][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }

    private static void DumpConditionEffects()
    {
        var conditions = DatabaseRepository.GetDatabase<ConditionDefinition>();
        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

        foreach (var condition in conditions
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = condition.Name;
            var effectReferenceGuid = condition.conditionParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            ConditionEffects
                .TryAdd(effectReferenceGuid, []);
            ConditionEffects[effectReferenceGuid].Add((name, condition));
        }

        foreach (var spell in spells
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = spell.Name;
            var effectParticleParameters = spell.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.conditionParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            ConditionEffects
                .TryAdd(effectReferenceGuid, []);
            ConditionEffects[effectReferenceGuid].Add((name, spell));
        }

        foreach (var power in powers
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = power.Name;
            var effectParticleParameters = power.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.conditionParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            ConditionEffects
                .TryAdd(effectReferenceGuid, []);
            ConditionEffects[effectReferenceGuid].Add((name, power));
        }
    }

    private static void DumpEffectEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Effect,
            new Dictionary<string, List<(string, EffectParticleParameters)>>());

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

        foreach (var spell in spells
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = spell.Name;
            var effectParticleParameters = spell.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.effectParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Effect]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Effect][effectReferenceGuid].Add((name, effectParticleParameters));
        }

        foreach (var power in powers
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = power.Name;
            var effectParticleParameters = power.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.effectParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Effect]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Effect][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }

    private static void DumpImpactEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Impact, []);

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

        foreach (var spell in spells
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = spell.Name;
            var effectParticleParameters = spell.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.impactParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Impact]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Impact][effectReferenceGuid].Add((name, effectParticleParameters));
        }

        foreach (var power in powers
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = power.Name;
            var effectParticleParameters = power.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.impactParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Impact]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Impact][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }

    private static void DumpZoneEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Zone, []);

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

        foreach (var spell in spells
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = spell.Name;
            var effectParticleParameters = spell.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.zoneParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Zone]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Zone][effectReferenceGuid].Add((name, effectParticleParameters));
        }

        foreach (var power in powers
                     .Where(x => x.ContentPack != CeContentPackContext.CeContentPack))
        {
            var name = power.Name;
            var effectParticleParameters = power.EffectDescription.EffectParticleParameters;
            var effectReferenceGuid = effectParticleParameters.zoneParticleReference?.AssetGUID;

            if (string.IsNullOrEmpty(effectReferenceGuid))
            {
                continue;
            }

            Effects[EffectHelpers.EffectType.Zone]
                .TryAdd(effectReferenceGuid, []);
            Effects[EffectHelpers.EffectType.Zone][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }
}
