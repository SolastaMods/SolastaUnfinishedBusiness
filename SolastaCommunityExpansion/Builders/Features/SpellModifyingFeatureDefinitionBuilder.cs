using System;
using SolastaCommunityExpansion;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class
        SpellModifyingFeatureDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : SpellModifyingFeatureDefinition
        where TBuilder : SpellModifyingFeatureDefinitionBuilder<TDefinition, TBuilder>
    {
        #region Constructors

        protected SpellModifyingFeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected SpellModifyingFeatureDefinitionBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        protected SpellModifyingFeatureDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
            original, name, namespaceGuid)
        {
        }

        protected SpellModifyingFeatureDefinitionBuilder(TDefinition original, string name, string definitionGuid) :
            base(original, name, definitionGuid)
        {
        }

        #endregion

        public TBuilder SetEffectModifier(
            SpellModifyingFeatureDefinition.ModifySpellEffectDelegate modifier)
        {
            Definition.SpellModifier = modifier;
            return This();
        }
    }

    public class SpellModifyingFeatureDefinitionBuilder : SpellModifyingFeatureDefinitionBuilder<
        SpellModifyingFeatureDefinition, SpellModifyingFeatureDefinitionBuilder>
    {
        #region Constructors

        protected SpellModifyingFeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected SpellModifyingFeatureDefinitionBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        protected SpellModifyingFeatureDefinitionBuilder(SpellModifyingFeatureDefinition original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected SpellModifyingFeatureDefinitionBuilder(SpellModifyingFeatureDefinition original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
