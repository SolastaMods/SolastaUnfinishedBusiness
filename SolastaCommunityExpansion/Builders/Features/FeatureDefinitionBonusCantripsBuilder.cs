using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBonusCantripsBuilder : FeatureDefinitionBuilder<FeatureDefinitionBonusCantrips, FeatureDefinitionBonusCantripsBuilder>
    {
        #region Constructors
        protected FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original) : base(original)
        {
        }

        protected FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionBonusCantripsBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionBonusCantripsBuilder Create(FeatureDefinitionBonusCantrips original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionBonusCantripsBuilder(original, name, namespaceGuid);
        }

        public FeatureDefinitionBonusCantripsBuilder ClearBonusCantrips()
        {
            Definition.BonusCantrips.Clear();
            return this;
        }

        public FeatureDefinitionBonusCantripsBuilder AddBonusCantrip(SpellDefinition spellDefinition)
        {
            Definition.BonusCantrips.Add(spellDefinition);
            return this;
        }

        public FeatureDefinitionBonusCantripsBuilder SetBonusCantrips(params SpellDefinition[] spellDefinitions)
        {
            SetBonusCantrips(spellDefinitions.AsEnumerable());
            return this;
        }

        public FeatureDefinitionBonusCantripsBuilder SetBonusCantrips(IEnumerable<SpellDefinition> spellDefinitions)
        {
            Definition.BonusCantrips.SetRange(spellDefinitions);
            return this;
        }
    }
}
