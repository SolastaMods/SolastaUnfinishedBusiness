using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;

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
        Definition.inventoryDefinition = DatabaseHelper.GetDefinition<InventoryDefinition>("HumanoidInventory");
        return this;
    }

    internal CharacterRaceDefinitionBuilder SetFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.Clear();
        AddFeaturesAtLevel(level, features);
        return this;
    }

    internal CharacterRaceDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
    {
        Definition.FeatureUnlocks.AddRange(features.Select(f => new FeatureUnlockByLevel(f, level)));
        return this;
    }

    #region Constructors

    protected CharacterRaceDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
