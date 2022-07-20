using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Diagnostics;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions;

public class FeatureDefinitionFeatureSetCustom : FeatureDefinition
{
    private readonly List<FeatureDefinition> _allFeatureSet = new();
    private readonly Dictionary<int, List<FeatureDefinition>> featuresByLevel = new();
    private bool _fullSetIsDirty;

    /**Are level requirements in character levels or class levels?*/
    public bool RequireClassLevels { get; set; }

    [NotNull] public List<int> AllLevels => featuresByLevel.Select(e => e.Key).ToList();

    public List<FeatureDefinition> AllFeatures
    {
        get
        {
            if (!_fullSetIsDirty)
            {
                return _allFeatureSet;
            }

            _allFeatureSet.SetRange(featuresByLevel.SelectMany(e => e.Value));
            _fullSetIsDirty = false;

            return _allFeatureSet;
        }
    }

    private List<FeatureDefinition> GetOrMakeLevelFeatures(int level)
    {
        List<FeatureDefinition> levelFeatures;
        if (!featuresByLevel.ContainsKey(level))
        {
            levelFeatures = new List<FeatureDefinition>();
            featuresByLevel.Add(level, levelFeatures);
        }
        else
        {
            levelFeatures = featuresByLevel[level];
        }

        return levelFeatures;
    }

    public void AddLevelFeatures(int level, [NotNull] params FeatureDefinition[] features)
    {
        GetOrMakeLevelFeatures(level).AddRange(features);
        _fullSetIsDirty = true;
    }

    public void AddLevelFeatures(int level, [NotNull] IEnumerable<FeatureDefinition> features)
    {
        GetOrMakeLevelFeatures(level).AddRange(features);
        _fullSetIsDirty = true;
    }

    public void SetLevelFeatures(int level, [NotNull] params FeatureDefinition[] features)
    {
        GetOrMakeLevelFeatures(level).SetRange(features);
        _fullSetIsDirty = true;
    }

    public void SetLevelFeatures(int level, [NotNull] IEnumerable<FeatureDefinition> features)
    {
        GetOrMakeLevelFeatures(level).SetRange(features);
        _fullSetIsDirty = true;
    }

    [CanBeNull]
    public List<FeatureDefinition> GetLevelFeatures(int level)
    {
        //TODO: decide if we want to wrap this into new list, to be sure this one is immutable
        return featuresByLevel.TryGetValue(level, out var result) ? result : null;
    }
}

public abstract class FeatureDefinitionFeatureSetCustomBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionFeatureSetCustom,
    FeatureDefinitionFeatureSetCustomBuilder>
{
    protected FeatureDefinitionFeatureSetCustomBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetCustomBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionFeatureSetCustomBuilder(FeatureDefinitionFeatureSetCustom original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetCustomBuilder(FeatureDefinitionFeatureSetCustom original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    [NotNull]
    public FeatureDefinitionFeatureSetCustomBuilder AddLevelFeatures(int level,
        [NotNull] params FeatureDefinition[] features)
    {
        Definition.AddLevelFeatures(level, features);
        return this;
    }

    [NotNull]
    public FeatureDefinitionFeatureSetCustomBuilder AddLevelFeatures(int level,
        [NotNull] IEnumerable<FeatureDefinition> features)
    {
        Definition.AddLevelFeatures(level, features);
        return this;
    }

    [NotNull]
    public FeatureDefinitionFeatureSetCustomBuilder SetLevelFeatures(int level,
        [NotNull] params FeatureDefinition[] features)
    {
        Definition.SetLevelFeatures(level, features);
        return this;
    }

    [NotNull]
    public FeatureDefinitionFeatureSetCustomBuilder SetLevelFeatures(int level,
        [NotNull] IEnumerable<FeatureDefinition> features)
    {
        Definition.SetLevelFeatures(level, features);
        return this;
    }

    [NotNull]
    public FeatureDefinitionFeatureSetCustomBuilder SetRequireClassLevels(bool value)
    {
        Definition.RequireClassLevels = value;
        return this;
    }
}

public sealed class FeatureDefinitionRemover : FeatureDefinition, IFeatureDefinitionCustomCode
{
    public FeatureDefinition FeatureToRemove { get; set; }

    public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
    {
        CustomFeaturesContext.ActuallyRemoveCharacterFeature(hero, FeatureToRemove);
    }

    public void RemoveFeature(RulesetCharacterHero hero, string tag)
    {
        ServiceRepository.GetService<ICharacterBuildingService>()
            .GetLastAssignedClassAndLevel(hero, out var lastClass, out var classLevel);
        // technically we return feature not where we took it from
        // add 100 here to avoid this to collide with anything from 1 to 20
        // i.e.: removing attributes on levels 4, 8, etc.
        tag = AttributeDefinitions.GetClassTag(lastClass, 100 + classLevel);
        ServiceRepository.GetService<ICharacterBuildingService>()
            .GrantFeatures(hero, new List<FeatureDefinition> {FeatureToRemove}, tag, false);
    }
}

public class FeatureDefinitionRemoverBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionRemover, FeatureDefinitionRemoverBuilder>
{
    [NotNull]
    private static string WrapName(string name) { return $"{name}Remover"; }

    private static FeatureDefinitionRemoverBuilder CreateFrom([NotNull] FeatureDefinition feature)
    {
        return Create(WrapName(feature.Name), CENamespaceGuid)
            .SetGuiPresentation(
                feature.GuiPresentation.Title,
                feature.GuiPresentation.Description,
                feature.GuiPresentation.SpriteReference
            )
            .SetFeatureToRemove(feature);
    }

    public static FeatureDefinitionRemover CreateOrGetFrom([NotNull] FeatureDefinition feature)
    {
        var name = WrapName(feature.Name);
        try
        {
            var result = DatabaseHelper.GetDefinition<FeatureDefinition>(name, null);
            if (result is FeatureDefinitionRemover remover)
            {
                return remover;
            }
        }
        catch (SolastaCommunityExpansionException)
        {
        }

        return CreateFrom(feature).AddToDB();
    }

    [NotNull]
    private FeatureDefinitionRemoverBuilder SetFeatureToRemove(FeatureDefinition feature)
    {
        Definition.FeatureToRemove = feature;
        return this;
    }

    #region Constructors

    public FeatureDefinitionRemoverBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public FeatureDefinitionRemoverBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public FeatureDefinitionRemoverBuilder(FeatureDefinitionRemover original, string name, Guid namespaceGuid) :
        base(
            original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionRemoverBuilder(FeatureDefinitionRemover original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}

public sealed class FeatureDefinitionFeatureSetReplaceCustom : FeatureDefinitionFeatureSetCustom
{
    public FeatureDefinitionFeatureSetCustom ReplacedFeatureSet { get; private set; }

    public void SetReplacedFeatureSet([NotNull] FeatureDefinitionFeatureSetCustom featureSet)
    {
        ReplacedFeatureSet = featureSet;
        GuiPresentation.spriteReference = featureSet.GuiPresentation.SpriteReference;
        foreach (var level in featureSet.AllLevels)
        {
            var features = featureSet.GetLevelFeatures(level);
            var removers = features.Select(FeatureDefinitionRemoverBuilder.CreateOrGetFrom);
            SetLevelFeatures(level, removers);
        }
    }
}

public abstract class FeatureDefinitionFeatureSetReplaceCustomBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionFeatureSetReplaceCustom, FeatureDefinitionFeatureSetReplaceCustomBuilder>
{
    protected FeatureDefinitionFeatureSetReplaceCustomBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetReplaceCustomBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionFeatureSetReplaceCustomBuilder(FeatureDefinitionFeatureSetReplaceCustom original,
        string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetReplaceCustomBuilder(FeatureDefinitionFeatureSetReplaceCustom original,
        string name, string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    [NotNull]
    public FeatureDefinitionFeatureSetReplaceCustomBuilder SetReplacedFeatureSet(
        [NotNull] FeatureDefinitionFeatureSetCustom featureSet)
    {
        Definition.SetReplacedFeatureSet(featureSet);
        return this;
    }
}
