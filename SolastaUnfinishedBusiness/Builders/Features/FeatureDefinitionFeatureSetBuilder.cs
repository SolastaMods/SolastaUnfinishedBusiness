using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Builders.Features;

public abstract class
    FeatureDefinitionFeatureSetBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionFeatureSet
    where TBuilder : FeatureDefinitionFeatureSetBuilder<TDefinition, TBuilder>
{
    public TBuilder ClearFeatureSet()
    {
        Definition.FeatureSet.Clear();
        return (TBuilder)this;
    }

    public TBuilder SetFeatureSet(params FeatureDefinition[] featureDefinitions)
    {
        return SetFeatureSet(featureDefinitions.AsEnumerable());
    }

    public TBuilder SetFeatureSet(IEnumerable<FeatureDefinition> featureDefinitions)
    {
        Definition.FeatureSet.SetRange(featureDefinitions);
        Definition.FeatureSet.Sort(Sorting.CompareTitle);
        return (TBuilder)this;
    }

    public TBuilder AddFeatureSet(params FeatureDefinition[] featureDefinitions)
    {
        return AddFeatureSet(featureDefinitions.AsEnumerable());
    }

    public TBuilder AddFeatureSet(IEnumerable<FeatureDefinition> featureDefinitions)
    {
        Definition.FeatureSet.AddRange(featureDefinitions);
        Definition.FeatureSet.Sort(Sorting.CompareTitle);
        return (TBuilder)this;
    }

    public TBuilder SetEnumerateInDescription(bool value)
    {
        Definition.enumerateInDescription = value;
        return (TBuilder)this;
    }

    public TBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
    {
        Definition.mode = mode;
        return (TBuilder)this;
    }

    public TBuilder SetUniqueChoices(bool uniqueChoice)
    {
        Definition.uniqueChoices = uniqueChoice;
        return (TBuilder)this;
    }

    #region Constructors

    protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}

public class FeatureDefinitionFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder<FeatureDefinitionFeatureSet,
    FeatureDefinitionFeatureSetBuilder>
{
    #region Constructors

    protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}

public class FeatureDefinitionFeatureSetWithPreRequisitesBuilder : FeatureDefinitionFeatureSetBuilder<
    FeatureDefinitionFeatureSetWithPreRequisites, FeatureDefinitionFeatureSetWithPreRequisitesBuilder>
{
    public FeatureDefinitionFeatureSetWithPreRequisitesBuilder SetValidators(
        params IFeatureDefinitionWithPrerequisites.Validate[] validators)
    {
        Definition.Validators.AddRange(validators);

        return this;
    }

    #region Constructors

    protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(
        FeatureDefinitionFeatureSetWithPreRequisites original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetWithPreRequisitesBuilder(
        FeatureDefinitionFeatureSetWithPreRequisites original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}
