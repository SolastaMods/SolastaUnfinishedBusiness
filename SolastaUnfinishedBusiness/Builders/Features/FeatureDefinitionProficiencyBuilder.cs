using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

public class FeatureDefinitionProficiencyBuilder : FeatureDefinitionBuilder<FeatureDefinitionProficiency,
    FeatureDefinitionProficiencyBuilder>
{
    public FeatureDefinitionProficiencyBuilder SetProficiencies(RuleDefinitions.ProficiencyType type,
        params string[] proficiencies)
    {
        return SetProficiencies(type, proficiencies.AsEnumerable());
    }

    public FeatureDefinitionProficiencyBuilder SetProficiencies(RuleDefinitions.ProficiencyType type,
        IEnumerable<string> proficiencies)
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

    protected FeatureDefinitionProficiencyBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
