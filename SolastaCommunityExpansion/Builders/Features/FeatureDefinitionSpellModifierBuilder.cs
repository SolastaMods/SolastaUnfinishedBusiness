using System;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class
        FeatureDefinitionSpellModifierBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : SpellModifyingFeatureDefinition
        where TBuilder : FeatureDefinitionSpellModifierBuilder<TDefinition, TBuilder>
    {
        public TBuilder SetEffectModifier(
            SpellModifyingFeatureDefinition.ModifySpellEffectDelegate modifier)
        {
            Definition.SpellModifier = modifier;
            return This();
        }

        #region Constructors

        protected FeatureDefinitionSpellModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSpellModifierBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        protected FeatureDefinitionSpellModifierBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
            original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSpellModifierBuilder(TDefinition original, string name, string definitionGuid) :
            base(original, name, definitionGuid)
        {
        }

        #endregion
    }

    public class FeatureDefinitionSpellModifierBuilder : FeatureDefinitionSpellModifierBuilder<
        SpellModifyingFeatureDefinition, FeatureDefinitionSpellModifierBuilder>
    {
        #region Constructors

        protected FeatureDefinitionSpellModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSpellModifierBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        protected FeatureDefinitionSpellModifierBuilder(SpellModifyingFeatureDefinition original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSpellModifierBuilder(SpellModifyingFeatureDefinition original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
