using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class FeatureDefinitionGrantCustomInvocations : FeatureDefinition
{
    internal List<CustomInvocationDefinition> Invocations { get; } = new();

    internal static void GrantInvocations(
        RulesetCharacterHero hero,
        string tag,
        List<FeatureDefinition> grantedFeatures)
    {
        var features = grantedFeatures
            .OfType<FeatureDefinitionGrantCustomInvocations>()
            .ToList();

        if (features.Empty())
        {
            return;
        }

        var command = ServiceRepository.GetService<IHeroBuildingCommandService>();

        foreach (var invocation in features.SelectMany(f => f.Invocations))
        {
            command.TrainCharacterFeature(hero, tag, invocation.Name, HeroDefinitions.PointsPoolType.Invocation);
        }
    }

    internal static void RemoveInvocations(
        RulesetCharacterHero hero,
        string tag,
        List<FeatureDefinition> removedFeatures)
    {
        var features = removedFeatures
            .OfType<FeatureDefinitionGrantCustomInvocations>()
            .ToList();

        if (features.Empty())
        {
            return;
        }

        var command = ServiceRepository.GetService<IHeroBuildingCommandService>();

        foreach (var invocation in features.SelectMany(f => f.Invocations))
        {
            command.UntrainCharacterFeature(hero, tag, invocation.Name, HeroDefinitions.PointsPoolType.Invocation);
        }
    }
}

[UsedImplicitly]
internal class FeatureDefinitionGrantCustomInvocationsBuilder : DefinitionBuilder<
    FeatureDefinitionGrantCustomInvocations,
    FeatureDefinitionGrantCustomInvocationsBuilder>
{
    internal FeatureDefinitionGrantCustomInvocationsBuilder SetInvocations(
        params CustomInvocationDefinition[] invocations)
    {
        Definition.Invocations.SetRange(invocations);
        return this;
    }

    #region Constructors

    internal FeatureDefinitionGrantCustomInvocationsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionGrantCustomInvocationsBuilder(FeatureDefinitionGrantCustomInvocations original,
        string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
