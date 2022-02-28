using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionCombatAffinityBuilder<TDefinition, TBuilder> : FeatureDefinitionAffinityBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionCombatAffinity
        where TBuilder : FeatureDefinitionCombatAffinityBuilder<TDefinition, TBuilder>
    {
        #region Constructors

        protected FeatureDefinitionCombatAffinityBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // Methods specific to FeatureDefinitionCombatAffinity

        public TBuilder SetMyAttackModifierDieType(RuleDefinitions.DieType dieType)
        {
            Definition.SetMyAttackModifierDieType(dieType);
            return This();
        }
    }

    public class FeatureDefinitionCombatAffinityBuilder
        : FeatureDefinitionCombatAffinityBuilder<FeatureDefinitionCombatAffinity, FeatureDefinitionCombatAffinityBuilder>
    {
        #region Constructors

        protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        #region Factory methods (create builder)
        public static FeatureDefinitionCombatAffinityBuilder Create(string name, string guid)
        {
            return new FeatureDefinitionCombatAffinityBuilder(name, guid);
        }

        public static FeatureDefinitionCombatAffinityBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionCombatAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionCombatAffinityBuilder Create(FeatureDefinitionCombatAffinity original, string name, string guid)
        {
            return new FeatureDefinitionCombatAffinityBuilder(original, name, guid);
        }

        public static FeatureDefinitionCombatAffinityBuilder Create(FeatureDefinitionCombatAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionCombatAffinityBuilder(original, name, namespaceGuid);
        }
        #endregion
    }
}
