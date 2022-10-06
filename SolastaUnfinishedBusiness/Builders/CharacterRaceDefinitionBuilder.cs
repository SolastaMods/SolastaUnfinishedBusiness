using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterRaceDefinitionBuilder
    : DefinitionBuilder<CharacterRaceDefinition, CharacterRaceDefinitionBuilder>
{
    internal CharacterRaceDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition characterSizeDefinition)
    {
        Definition.sizeDefinition = characterSizeDefinition;
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetMinimalAge(int minimalAge)
    {
        Definition.minimalAge = minimalAge;
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetMaximalAge(int maximalAge)
    {
        Definition.minimalAge = maximalAge;
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetBaseHeight(int baseHeight)
    {
        Definition.baseHeight = baseHeight;
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetBaseWeight(int baseWeight)
    {
        Definition.baseWeight = baseWeight;
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetRacePresentation(RacePresentation racePresentation)
    {
        Definition.racePresentation = racePresentation;
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.Clear();

        AddFeaturesAtLevel(level, features);

        // if (Main.Settings.EnableSortingFutureFeatures)
        // {
        //     Definition.FeatureUnlocks.Sort(Sorting.Compare);
        // }
        // else
        // {
        //     features.Do(x => x.GuiPresentation.sortOrder = level);
        // }

        return this;
    }

    internal CharacterRaceDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));

        // if (Main.Settings.EnableSortingFutureFeatures)
        // {
        //     Definition.FeatureUnlocks.Sort(Sorting.Compare);
        // }
        // else
        // {
        //     features.Do(x => x.GuiPresentation.sortOrder = level);
        // }

        return this;
    }

    #region Constructors

    protected CharacterRaceDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterRaceDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}
