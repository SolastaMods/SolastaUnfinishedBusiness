using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly Dictionary<int, List<FeatureDefinition>> FeaturesByLevel = new();
    private bool _fullSetIsDirty;

    /**Are level requirements in character levels or class levels?*/
    public bool RequireClassLevels { get; set; }

    public List<int> AllLevels => FeaturesByLevel.Select(e => e.Key).ToList();

    public List<FeatureDefinition> AllFeatures
    {
        get
        {
            if (_fullSetIsDirty)
            {
                _allFeatureSet.SetRange(FeaturesByLevel.SelectMany(e => e.Value));
                _fullSetIsDirty = false;
            }

            return _allFeatureSet;
        }
    }

    private List<FeatureDefinition> GetOrMakeLevelFeatures(int level)
    {
        List<FeatureDefinition> levelFeatures;
        if (!FeaturesByLevel.ContainsKey(level))
        {
            levelFeatures = new List<FeatureDefinition>();
            FeaturesByLevel.Add(level, levelFeatures);
        }
        else
        {
            levelFeatures = FeaturesByLevel[level];
        }

        return levelFeatures;
    }

    public void AddLevelFeatures(int level, params FeatureDefinition[] features)
    {
        GetOrMakeLevelFeatures(level).AddRange(features);
        _fullSetIsDirty = true;
    }

    public void AddLevelFeatures(int level, IEnumerable<FeatureDefinition> features)
    {
        GetOrMakeLevelFeatures(level).AddRange(features);
        _fullSetIsDirty = true;
    }

    public void SetLevelFeatures(int level, params FeatureDefinition[] features)
    {
        GetOrMakeLevelFeatures(level).SetRange(features);
        _fullSetIsDirty = true;
    }

    public void SetLevelFeatures(int level, IEnumerable<FeatureDefinition> features)
    {
        GetOrMakeLevelFeatures(level).SetRange(features);
        _fullSetIsDirty = true;
    }

    public List<FeatureDefinition> GetLevelFeatures(int level)
    {
        //TODO: decide if we want to wrap this into new list, to be sure this one is immutable
        return FeaturesByLevel.TryGetValue(level, out var result) ? result : null;
    }
}

public class FeatureDefinitionFeatureSetCustomBuilder : FeatureDefinitionBuilder<FeatureDefinitionFeatureSetCustom,
    FeatureDefinitionFeatureSetCustomBuilder>
{
    public FeatureDefinitionFeatureSetCustomBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public FeatureDefinitionFeatureSetCustomBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public FeatureDefinitionFeatureSetCustomBuilder(FeatureDefinitionFeatureSetCustom original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionFeatureSetCustomBuilder(FeatureDefinitionFeatureSetCustom original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    public FeatureDefinitionFeatureSetCustomBuilder AddLevelFeatures(int level, params FeatureDefinition[] features)
    {
        Definition.AddLevelFeatures(level, features);
        return this;
    }

    public FeatureDefinitionFeatureSetCustomBuilder AddLevelFeatures(int level, List<FeatureDefinition> features)
    {
        Definition.AddLevelFeatures(level, features);
        return this;
    }

    public FeatureDefinitionFeatureSetCustomBuilder SetLevelFeatures(int level, params FeatureDefinition[] features)
    {
        Definition.SetLevelFeatures(level, features);
        return this;
    }

    public FeatureDefinitionFeatureSetCustomBuilder SetLevelFeatures(int level, List<FeatureDefinition> features)
    {
        Definition.SetLevelFeatures(level, features);
        return this;
    }

    public FeatureDefinitionFeatureSetCustomBuilder SetRequireClassLevels(bool value)
    {
        Definition.RequireClassLevels = value;
        return this;
    }
}

public class FeatureDefinitionRemover : FeatureDefinition, IFeatureDefinitionCustomCode
{
    public FeatureDefinition FeatureToRemove { get; set; }

    public void ApplyFeature(RulesetCharacterHero hero, string tag)
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
    private static string WrapName(string name) { return $"{$"{name}Remover"}"; }

    public static FeatureDefinitionRemoverBuilder CreateFrom(FeatureDefinition feature)
    {
        return Create(WrapName(feature.Name), CENamespaceGuid)
            .SetGuiPresentation(
                feature.GuiPresentation.Title,
                feature.GuiPresentation.Description,
                feature.GuiPresentation.SpriteReference
            )
            .SetFeatureToRemove(feature);
    }

    public static FeatureDefinitionRemover CreateOrGetFrom(FeatureDefinition feature)
    {
        var name = WrapName(feature.Name);
        try
        {
            var result = DatabaseHelper.GetDefinition<FeatureDefinition>(name, null);
            if (result != null && result is FeatureDefinitionRemover remover)
            {
                return remover;
            }
        }
        catch (SolastaCommunityExpansionException)
        {
        }

        return CreateFrom(feature).AddToDB();
    }

    public FeatureDefinitionRemoverBuilder SetFeatureToRemove(FeatureDefinition feature)
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

public class FeatureDefinitionFeatureSetReplaceCustom : FeatureDefinitionFeatureSetCustom
{
    public FeatureDefinitionFeatureSetCustom ReplacedFeatureSet { get; private set; }

    public void SetReplacedFeatureSet(FeatureDefinitionFeatureSetCustom featureSet)
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

public class FeatureDefinitionFeatureSetReplaceCustomBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionFeatureSetReplaceCustom, FeatureDefinitionFeatureSetReplaceCustomBuilder>
{
    public FeatureDefinitionFeatureSetReplaceCustomBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    public FeatureDefinitionFeatureSetReplaceCustomBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    public FeatureDefinitionFeatureSetReplaceCustomBuilder(FeatureDefinitionFeatureSetReplaceCustom original,
        string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionFeatureSetReplaceCustomBuilder(FeatureDefinitionFeatureSetReplaceCustom original,
        string name, string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    public FeatureDefinitionFeatureSetReplaceCustomBuilder SetReplacedFeatureSet(
        FeatureDefinitionFeatureSetCustom featureSet)
    {
        Definition.SetReplacedFeatureSet(featureSet);
        return this;
    }
}
