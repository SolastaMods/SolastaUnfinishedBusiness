using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionDamageAffinityBuilder<TDefinition, TBuilder> : FeatureDefinitionAffinityBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionDamageAffinity
        where TBuilder : FeatureDefinitionDamageAffinityBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionDamageAffinityBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder SetDamageType(string value)
        {
            Definition.SetDamageType(value);
            return This();
        }
        public TBuilder SetDamageAffinityType(RuleDefinitions.DamageAffinityType value)
        {
            Definition.SetDamageAffinityType(value);
            return This();
        }
        public TBuilder SetRetaliate(FeatureDefinitionPower power, int range)
        {
            Definition.SetRetaliateWhenHit(true);
            Definition.SetRetaliateRangeCells(range);
            Definition.SetRetaliatePower(power);
            return This();
        }

        public TBuilder SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType value)
        {
            Definition.SetSavingThrowAdvantageType(value);
            return This();
        }
    }

    public class FeatureDefinitionDamageAffinityBuilder : FeatureDefinitionDamageAffinityBuilder<FeatureDefinitionDamageAffinity, FeatureDefinitionDamageAffinityBuilder>
    {
        #region Constructors
        protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original) : base(original)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
