using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Models;

public static class EffectsContext
{
    internal static readonly
        Dictionary<EffectHelpers.EffectType, Dictionary<string, List<(string, EffectParticleParameters)>>> Effects =
            new();

    internal static void DumpEffects()
    {
        DumpCasterEffects();
        DumpConditionEffects();
        DumpImpactEffects();
        DumpZoneEffects();
    }

    private static void DumpCasterEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Caster,
            new Dictionary<string, List<(string, EffectParticleParameters)>>());

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

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
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Caster][effectReferenceGuid].Add((name, effectParticleParameters));
        }

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
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Caster][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }

    private static void DumpConditionEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Condition,
            new Dictionary<string, List<(string, EffectParticleParameters)>>());

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

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

            Effects[EffectHelpers.EffectType.Condition]
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Condition][effectReferenceGuid].Add((name, effectParticleParameters));
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

            Effects[EffectHelpers.EffectType.Condition]
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Condition][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }

    private static void DumpImpactEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Impact,
            new Dictionary<string, List<(string, EffectParticleParameters)>>());

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

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
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Impact][effectReferenceGuid].Add((name, effectParticleParameters));
        }

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
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Impact][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }

    private static void DumpZoneEffects()
    {
        Effects.Add(EffectHelpers.EffectType.Zone,
            new Dictionary<string, List<(string, EffectParticleParameters)>>());

        var powers = DatabaseRepository.GetDatabase<FeatureDefinitionPower>();
        var spells = DatabaseRepository.GetDatabase<SpellDefinition>();

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
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Zone][effectReferenceGuid].Add((name, effectParticleParameters));
        }

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
                .TryAdd(effectReferenceGuid, new List<(string, EffectParticleParameters)>());
            Effects[EffectHelpers.EffectType.Zone][effectReferenceGuid].Add((name, effectParticleParameters));
        }
    }
}
