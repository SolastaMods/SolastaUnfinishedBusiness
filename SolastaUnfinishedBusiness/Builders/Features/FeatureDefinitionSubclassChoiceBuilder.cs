using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionSubclassChoiceBuilder
    : DefinitionBuilder<FeatureDefinitionSubclassChoice, FeatureDefinitionSubclassChoiceBuilder>
{
    internal FeatureDefinitionSubclassChoiceBuilder SetFilterByDeity(bool requireDeity)
    {
        Definition.filterByDeity = requireDeity;
        return this;
    }

    internal FeatureDefinitionSubclassChoiceBuilder SetSubclassSuffix(string subclassSuffix)
    {
        Definition.subclassSuffix = subclassSuffix;
        return this;
    }

    internal FeatureDefinitionSubclassChoiceBuilder SetSubclasses(params CharacterSubclassDefinition[] subclasses)
    {
        Definition.Subclasses.SetRange(subclasses.Select(sc => sc.Name));
        Definition.Subclasses.Sort();
        return this;
    }

    #region Constructors

    protected FeatureDefinitionSubclassChoiceBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
