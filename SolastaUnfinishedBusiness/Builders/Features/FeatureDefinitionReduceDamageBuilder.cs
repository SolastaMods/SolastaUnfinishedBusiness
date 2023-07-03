using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionReduceDamageBuilder
    : DefinitionBuilder<FeatureDefinitionReduceDamage, FeatureDefinitionReduceDamageBuilder>
{
    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetNotificationTag(string notificationTag)
    {
        Definition.NotificationTag = notificationTag;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetFixedReducedDamage(
        Func<GameLocationCharacter, GameLocationCharacter, int> reducedDamage,
        params string[] damageTypes)
    {
        Definition.DamageTypes.SetRange(damageTypes);
        Definition.TriggerCondition = RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive;
        Definition.ReducedDamage = reducedDamage;
        return this;
    }

    [NotNull]
    internal FeatureDefinitionReduceDamageBuilder SetConsumeSpellSlotsReducedDamage(
        CharacterClassDefinition spellCastingClass,
        Func<GameLocationCharacter, GameLocationCharacter, int> reducedDamage,
        params string[] damageTypes)
    {
        Definition.SpellCastingClass = spellCastingClass;
        Definition.TriggerCondition = RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot;
        Definition.ReducedDamage = reducedDamage;
        Definition.DamageTypes.SetRange(damageTypes);
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
