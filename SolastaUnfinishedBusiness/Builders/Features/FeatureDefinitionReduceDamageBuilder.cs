using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

// ReSharper disable once ClassNeverInstantiated.Global
public class FeatureDefinitionReduceDamageBuilder : FeatureDefinitionBuilder<FeatureDefinitionReduceDamage,
    FeatureDefinitionReduceDamageBuilder>
{
    [NotNull]
    public FeatureDefinitionReduceDamageBuilder SetNotificationTag(string notificationTag)
    {
        Definition.NotificationTag = notificationTag;
        return this;
    }

    [NotNull]
    public FeatureDefinitionReduceDamageBuilder SetReducedDamage(int reducedDamage)
    {
        Definition.ReducedDamage = reducedDamage;
        return this;
    }

    [NotNull]
    public FeatureDefinitionReduceDamageBuilder SetSourceName(string sourceName)
    {
        Definition.SourceName = sourceName;
        return this;
    }

    [NotNull]
    public FeatureDefinitionReduceDamageBuilder SetSourceType(RuleDefinitions.FeatureSourceType sourceType)
    {
        Definition.SourceType = sourceType;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionReduceDamageBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionReduceDamageBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionReduceDamageBuilder(FeatureDefinitionReduceDamage original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionReduceDamageBuilder(FeatureDefinitionReduceDamage original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
