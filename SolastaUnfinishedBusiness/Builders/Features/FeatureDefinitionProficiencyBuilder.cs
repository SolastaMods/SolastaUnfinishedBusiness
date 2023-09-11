using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionProficiencyBuilder
    : DefinitionBuilder<FeatureDefinitionProficiency, FeatureDefinitionProficiencyBuilder>
{
    internal FeatureDefinitionProficiencyBuilder SetProficiencies(
        ProficiencyType type,
        params string[] proficiencies)
    {
        Definition.proficiencyType = type;
        Definition.Proficiencies.SetRange(proficiencies);
        Definition.Proficiencies.Sort();
        return this;
    }

    #region Constructors

    protected FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
