using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Definitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionDieRollModifierDamageTypeDependentBuilder
    : DefinitionBuilder<FeatureDefinitionDieRollModifierDamageTypeDependent,
        FeatureDefinitionDieRollModifierDamageTypeDependentBuilder>
{
    internal FeatureDefinitionDieRollModifierDamageTypeDependentBuilder SetModifiers(
        RollContext context,
        int rerollCount,
        int minRollValue,
        int minReRollValue,
        string consoleLocalizationKey,
        params string[] damageTypes)
    {
        Definition.validityContext = context;
        Definition.rerollLocalizationKey = consoleLocalizationKey;
        Definition.rerollCount = rerollCount;
        Definition.minRollValue = minRollValue;
        Definition.minRerollValue = minReRollValue;
        Definition.damageTypes.SetRange(damageTypes);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionDieRollModifierDamageTypeDependentBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionDieRollModifierDamageTypeDependentBuilder(
        FeatureDefinitionDieRollModifierDamageTypeDependent original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
