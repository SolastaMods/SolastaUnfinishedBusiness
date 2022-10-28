using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionSpendSpellSlotToReduceDamageBuilder
    : DefinitionBuilder<FeatureDefinitionSpendSpellSlotToReduceDamage,
        FeatureDefinitionSpendSpellSlotToReduceDamageBuilder>
{
    [NotNull]
    internal FeatureDefinitionSpendSpellSlotToReduceDamageBuilder SetNotificationTag(string notificationTag)
    {
        Definition.NotificationTag = notificationTag;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionSpendSpellSlotToReduceDamageBuilder SetReducedDamage(int reducedDamage)
    {
        Definition.ReducedDamage = reducedDamage;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionSpendSpellSlotToReduceDamageBuilder SetSourceName(string sourceName)
    {
        Definition.SourceName = sourceName;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionSpendSpellSlotToReduceDamageBuilder SetSourceType(
        RuleDefinitions.FeatureSourceType sourceType)
    {
        Definition.SourceType = sourceType;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionSpendSpellSlotToReduceDamageBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionSpendSpellSlotToReduceDamageBuilder(
        FeatureDefinitionSpendSpellSlotToReduceDamage original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
