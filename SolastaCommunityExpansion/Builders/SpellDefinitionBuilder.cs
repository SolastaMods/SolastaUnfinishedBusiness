using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class SpellDefinitionBuilder : DefinitionBuilder<SpellDefinition, SpellDefinitionBuilder>
    {
        #region Constructors
        protected SpellDefinitionBuilder(string name, string guid) : base(name, guid)
        {
            InitializeFields();
        }

        protected SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
            InitializeFields();
        }

        protected SpellDefinitionBuilder(SpellDefinition original, string name, string guid) : base(original, name, guid)
        {
            InitializeFields();
        }

        protected SpellDefinitionBuilder(SpellDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
            InitializeFields();
        }
        #endregion

        #region Factory methods
        public static SpellDefinitionBuilder Create(string name, string guid)
        {
            return new SpellDefinitionBuilder(name, guid);
        }

        public static SpellDefinitionBuilder Create(string name, Guid guidNamespace)
        {
            return new SpellDefinitionBuilder(name, guidNamespace);
        }

        public static SpellDefinitionBuilder Create(SpellDefinition original, string name, string guid)
        {
            return new SpellDefinitionBuilder(original, name, guid);
        }

        public static SpellDefinitionBuilder Create(SpellDefinition original, string name, Guid guidNamespace)
        {
            return new SpellDefinitionBuilder(original, name, guidNamespace);
        }
        #endregion

        private void InitializeFields()
        {
            Definition.SetImplemented(true);
        }

        public SpellDefinitionBuilder SetSpellLevel(int spellLevel)
        {
            Definition.SetSpellLevel(spellLevel);
            return this;
        }

        public SpellDefinitionBuilder SetRequiresConcentration(bool value)
        {
            Definition.SetRequiresConcentration(value);
            return this;
        }

        public SpellDefinitionBuilder SetSchoolOfMagic(SchoolOfMagicDefinition school)
        {
            Definition.SetSchoolOfMagic(school.Name);
            return this;
        }

        public SpellDefinitionBuilder SetSubSpells(params SpellDefinition[] subspells)
        {
            return SetSubSpells(subspells.AsEnumerable());
        }

        public SpellDefinitionBuilder SetSubSpells(IEnumerable<SpellDefinition> subspells)
        {
            Definition.SetSpellsBundle(true);
            Definition.SubspellsList.SetRange(subspells);
            return this;
        }

        public SpellDefinitionBuilder SetCastingTime(RuleDefinitions.ActivationTime castingTime)
        {
            Definition.SetCastingTime(castingTime);
            return this;
        }

        public SpellDefinitionBuilder SetRitualCasting(RuleDefinitions.ActivationTime ritualCastingTime)
        {
            Definition.SetRitual(true);
            Definition.SetRitualCastingTime(ritualCastingTime);
            return this;
        }

        public SpellDefinitionBuilder SetUniqueInstance(bool unique = true)
        {
            Definition.SetUniqueInstance(unique);
            return this;
        }

        public SpellDefinitionBuilder SetVerboseComponent(bool verboseComponent)
        {
            Definition.SetVerboseComponent(verboseComponent);
            return this;
        }

        public SpellDefinitionBuilder SetSomaticComponent(bool somaticComponent)
        {
            Definition.SetSomaticComponent(somaticComponent);
            return this;
        }

        public SpellDefinitionBuilder SetMaterialComponent(RuleDefinitions.MaterialComponentType materialComponentType)
        {
            Definition.SetMaterialComponentType(materialComponentType);
            return this;
        }

        public SpellDefinitionBuilder SetSpecificMaterialComponent(string specificMaterialComponentTag,
            int specificMaterialComponentCostGp, bool specificMaterialComponentConsumed)
        {
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Specific);
            Definition.SetSpecificMaterialComponentTag(specificMaterialComponentTag);
            Definition.SetSpecificMaterialComponentCostGp(specificMaterialComponentCostGp);
            Definition.SetSpecificMaterialComponentConsumed(specificMaterialComponentConsumed);
            return this;
        }

        public SpellDefinitionBuilder SetEffectDescription(EffectDescription effectDescription)
        {
            Definition.SetEffectDescription(effectDescription);
            return this;
        }

        public SpellDefinitionBuilder SetAiParameters(SpellAIParameters aiParameters)
        {
            Definition.SetAiParameters(aiParameters);
            return this;
        }

        public SpellDefinitionBuilder SetConcentrationAction(ActionDefinitions.ActionParameter concentrationAction)
        {
            Definition.SetConcentrationAction(concentrationAction);
            return this;
        }
    }
}
