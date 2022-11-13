using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionReduceDamageBuilder
    : DefinitionBuilder<FeatureDefinitionReduceDamage,
        FeatureDefinitionReduceDamageBuilder>
{
    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetNotificationTag(string notificationTag)
    {
        Definition.NotificationTag = notificationTag;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetReducedDamage(int reducedDamage)
    {
        Definition.ReducedDamage = reducedDamage;
        return this;
    }

#if false
    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetSourceName(string sourceName)
    {
        Definition.SourceName = sourceName;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetSourceType(
        RuleDefinitions.FeatureSourceType sourceType)
    {
        Definition.SourceType = sourceType;
        return this;
    }
#endif

    #region Constructors

    protected FeatureDefinitionReduceDamageBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionReduceDamageBuilder(
        FeatureDefinitionReduceDamage original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
