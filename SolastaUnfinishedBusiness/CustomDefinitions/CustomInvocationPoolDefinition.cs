using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders.Features;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public class CustomInvocationPoolDefinition : FeatureDefinition
{
    private readonly Dictionary<int, List<CustomInvocationDefinition>> featuresByLevel = new();
    public List<CustomInvocationDefinition> AllFeatures { get; } = new();
    [NotNull] public List<int> AllLevels => featuresByLevel.Select(e => e.Key).ToList();
    public string PoolType { get; set; }

    public int Points { get; set; }
    public bool IsUnlearn { get; set; }

    /**Are level requirements in character levels or class levels?*/
    public bool RequireClassLevels { get; set; }


    public void Refresh()
    {
        AllFeatures.SetRange(DatabaseRepository.GetDatabase<InvocationDefinition>()
            .GetAllElements()
            .OfType<CustomInvocationDefinition>()
            .Where(d => d.PoolType == PoolType));

        featuresByLevel.Clear();
        AllFeatures.ForEach(f => AddLevelFeatures(f.requiredLevel, f));
    }

    private List<CustomInvocationDefinition> GetOrMakeLevelFeatures(int level)
    {
        List<CustomInvocationDefinition> levelFeatures;
        if (!featuresByLevel.ContainsKey(level))
        {
            levelFeatures = new List<CustomInvocationDefinition>();
            featuresByLevel.Add(level, levelFeatures);
        }
        else
        {
            levelFeatures = featuresByLevel[level];
        }

        return levelFeatures;
    }

    private void AddLevelFeatures(int level, [NotNull] CustomInvocationDefinition feature)
    {
        GetOrMakeLevelFeatures(level).Add(feature);
    }

    [NotNull]
    public List<CustomInvocationDefinition> GetLevelFeatures(int level)
    {
        //TODO: decide if we want to wrap this into new list, to be sure this one is immutable
        return (featuresByLevel.TryGetValue(level, out var result) ? result : null)
               ?? new List<CustomInvocationDefinition>();
    }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class CustomInvocationPoolDefinitionBuilder : FeatureDefinitionBuilder
    <CustomInvocationPoolDefinition, CustomInvocationPoolDefinitionBuilder>
{
    protected CustomInvocationPoolDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(CustomInvocationPoolDefinition original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(CustomInvocationPoolDefinition original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    public CustomInvocationPoolDefinitionBuilder Setup(string poolType, int points = 1, bool isUnlearn = false)
    {
        Definition.PoolType = poolType;
        Definition.Points = points;
        Definition.IsUnlearn = isUnlearn;
        return this;
    }
}

// public sealed class FeatureDefinitionRemover : FeatureDefinition, IFeatureDefinitionCustomCode
// {
//     public FeatureDefinition FeatureToRemove { get; set; }
//
//     public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
//     {
//         CustomFeaturesContext.ActuallyRemoveCharacterFeature(hero, FeatureToRemove);
//     }
//
//     public void RemoveFeature(RulesetCharacterHero hero, string tag)
//     {
//         ServiceRepository.GetService<ICharacterBuildingService>()
//             .GetLastAssignedClassAndLevel(hero, out var lastClass, out var classLevel);
//         // technically we return feature not where we took it from
//         // add 100 here to avoid this to collide with anything from 1 to 20
//         // i.e.: removing attributes on levels 4, 8, etc.
//         tag = AttributeDefinitions.GetClassTag(lastClass, 100 + classLevel);
//         ServiceRepository.GetService<ICharacterBuildingService>()
//             .GrantFeatures(hero, new List<FeatureDefinition> { FeatureToRemove }, tag, false);
//     }
// }

// public class FeatureDefinitionRemoverBuilder
//     : FeatureDefinitionBuilder<FeatureDefinitionRemover, FeatureDefinitionRemoverBuilder>
// {
//     [NotNull]
//     private static string WrapName(string name) { return $"{name}Remover"; }
//
//     private static FeatureDefinitionRemoverBuilder CreateFrom([NotNull] FeatureDefinition feature)
//     {
//         return Create(WrapName(feature.Name), CENamespaceGuid)
//             .SetGuiPresentation(
//                 feature.GuiPresentation.Title,
//                 feature.GuiPresentation.Description,
//                 feature.GuiPresentation.SpriteReference
//             )
//             .SetFeatureToRemove(feature);
//     }
//
//     public static FeatureDefinitionRemover CreateOrGetFrom([NotNull] FeatureDefinition feature)
//     {
//         var name = WrapName(feature.Name);
//         try
//         {
//             var result = DatabaseHelper.GetDefinition<FeatureDefinition>(name, null);
//             if (result is FeatureDefinitionRemover remover)
//             {
//                 return remover;
//             }
//         }
//         catch (SolastaCommunityExpansionException)
//         {
//         }
//
//         return CreateFrom(feature).AddToDB();
//     }
//
//     [NotNull]
//     private FeatureDefinitionRemoverBuilder SetFeatureToRemove(FeatureDefinition feature)
//     {
//         Definition.FeatureToRemove = feature;
//         return this;
//     }
//
//     #region Constructors
//
//     public FeatureDefinitionRemoverBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
//     {
//     }
//
//     public FeatureDefinitionRemoverBuilder(string name, string definitionGuid) : base(name, definitionGuid)
//     {
//     }
//
//     public FeatureDefinitionRemoverBuilder(FeatureDefinitionRemover original, string name, Guid namespaceGuid) :
//         base(
//             original, name, namespaceGuid)
//     {
//     }
//
//     public FeatureDefinitionRemoverBuilder(FeatureDefinitionRemover original, string name, string definitionGuid) :
//         base(original, name, definitionGuid)
//     {
//     }
//
//     #endregion
// }

// public sealed class FeatureDefinitionFeatureSetReplaceCustom : FeatureDefinitionFeatureSetCustom
// {
//     public FeatureDefinitionFeatureSetCustom ReplacedFeatureSet { get; private set; }
//
//     public void SetReplacedFeatureSet([NotNull] FeatureDefinitionFeatureSetCustom featureSet)
//     {
//         ReplacedFeatureSet = featureSet;
//         GuiPresentation.spriteReference = featureSet.GuiPresentation.SpriteReference;
//         foreach (var level in featureSet.AllLevels)
//         {
//             var features = featureSet.GetLevelFeatures(level);
//             var removers = features.Select(FeatureDefinitionRemoverBuilder.CreateOrGetFrom);
//             SetLevelFeatures(level, removers);
//         }
//     }
// }

// [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
// public class FeatureDefinitionFeatureSetReplaceCustomBuilder : FeatureDefinitionBuilder<
//     FeatureDefinitionFeatureSetReplaceCustom, FeatureDefinitionFeatureSetReplaceCustomBuilder>
// {
//     protected FeatureDefinitionFeatureSetReplaceCustomBuilder(string name, Guid namespaceGuid) : base(name,
//         namespaceGuid)
//     {
//     }
//
//     protected FeatureDefinitionFeatureSetReplaceCustomBuilder(string name, string definitionGuid) : base(name,
//         definitionGuid)
//     {
//     }
//
//     protected FeatureDefinitionFeatureSetReplaceCustomBuilder(FeatureDefinitionFeatureSetReplaceCustom original,
//         string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
//     {
//     }
//
//     protected FeatureDefinitionFeatureSetReplaceCustomBuilder(FeatureDefinitionFeatureSetReplaceCustom original,
//         string name, string definitionGuid) : base(original, name, definitionGuid)
//     {
//     }
//
//     [NotNull]
//     public FeatureDefinitionFeatureSetReplaceCustomBuilder SetReplacedFeatureSet(
//         [NotNull] FeatureDefinitionFeatureSetCustom featureSet)
//     {
//         Definition.SetReplacedFeatureSet(featureSet);
//         return this;
//     }
// }
