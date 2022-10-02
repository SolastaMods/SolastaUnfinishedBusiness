using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterRaceDefinitionBuilder
    : DefinitionBuilder<CharacterRaceDefinition, CharacterRaceDefinitionBuilder>
{
    public CharacterRaceDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition characterSizeDefinition)
    {
        Definition.sizeDefinition = characterSizeDefinition;
        return this;
    }

    public CharacterRaceDefinitionBuilder SetMinimalAge(int minimalAge)
    {
        Definition.minimalAge = minimalAge;
        return this;
    }

    public CharacterRaceDefinitionBuilder SetMaximalAge(int maximalAge)
    {
        Definition.minimalAge = maximalAge;
        return this;
    }

    public CharacterRaceDefinitionBuilder SetBaseHeight(int baseHeight)
    {
        Definition.baseHeight = baseHeight;
        return this;
    }

    public CharacterRaceDefinitionBuilder SetBaseWeight(int baseWeight)
    {
        Definition.baseWeight = baseWeight;
        return this;
    }

    public CharacterRaceDefinitionBuilder SetRacePresentation(RacePresentation racePresentation)
    {
        Definition.racePresentation = racePresentation;
        return this;
    }

    public CharacterRaceDefinitionBuilder SetFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.Clear();

        AddFeaturesAtLevel(level, features);

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Definition.FeatureUnlocks.Sort(Sorting.Compare);
        }
        else
        {
            features.Do(x => x.GuiPresentation.sortOrder = level);
        }

        return this;
    }

    public CharacterRaceDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));

        if (Main.Settings.EnableSortingFutureFeatures)
        {
            Definition.FeatureUnlocks.Sort(Sorting.Compare);
        }
        else
        {
            features.Do(x => x.GuiPresentation.sortOrder = level);
        }

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
