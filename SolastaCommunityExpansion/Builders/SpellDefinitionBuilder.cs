using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public abstract class SpellDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : SpellDefinition
        where TBuilder : SpellDefinitionBuilder<TDefinition, TBuilder>
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

        protected SpellDefinitionBuilder(TDefinition original, string name, string guid) : base(original, name, guid)
        {
            InitializeFields();
        }

        protected SpellDefinitionBuilder(TDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
            InitializeFields();
        }
        #endregion

        private void InitializeFields()
        {
            //
            // Should fix lots of official spells getting modified by spells on this mod
            //
            Definition.SetEffectDescription(Definition.EffectDescription.DeepCopy());
            Definition.SetImplemented(true);
        }

        public TBuilder SetSpellLevel(int spellLevel)
        {
            Definition.SetSpellLevel(spellLevel);
            return This();
        }

        public TBuilder SetRequiresConcentration(bool value)
        {
            Definition.SetRequiresConcentration(value);
            return This();
        }

        public TBuilder SetSchoolOfMagic(SchoolOfMagicDefinition school)
        {
            Definition.SetSchoolOfMagic(school.Name);
            return This();
        }

        public TBuilder SetSubSpells(params TDefinition[] subspells)
        {
            return SetSubSpells(subspells.AsEnumerable());
        }

        public TBuilder SetSubSpells(IEnumerable<TDefinition> subspells)
        {
            Definition.SetSpellsBundle(true);
            Definition.SubspellsList.SetRange(subspells);
            return This();
        }

        public TBuilder SetCastingTime(RuleDefinitions.ActivationTime castingTime)
        {
            Definition.SetCastingTime(castingTime);
            return This();
        }

        public TBuilder SetRitualCasting(RuleDefinitions.ActivationTime ritualCastingTime)
        {
            Definition.SetRitual(true);
            Definition.SetRitualCastingTime(ritualCastingTime);
            return This();
        }

        public TBuilder SetUniqueInstance(bool unique = true)
        {
            Definition.SetUniqueInstance(unique);
            return This();
        }

        public TBuilder SetVerboseComponent(bool verboseComponent)
        {
            Definition.SetVerboseComponent(verboseComponent);
            return This();
        }

        public TBuilder SetSomaticComponent(bool somaticComponent)
        {
            Definition.SetSomaticComponent(somaticComponent);
            return This();
        }

        public TBuilder SetMaterialComponent(RuleDefinitions.MaterialComponentType materialComponentType)
        {
            Definition.SetMaterialComponentType(materialComponentType);
            return This();
        }

        public TBuilder SetSpecificMaterialComponent(string specificMaterialComponentTag,
            int specificMaterialComponentCostGp, bool specificMaterialComponentConsumed)
        {
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Specific);
            Definition.SetSpecificMaterialComponentTag(specificMaterialComponentTag);
            Definition.SetSpecificMaterialComponentCostGp(specificMaterialComponentCostGp);
            Definition.SetSpecificMaterialComponentConsumed(specificMaterialComponentConsumed);
            return This();
        }

        public TBuilder SetEffectDescription(EffectDescription effectDescription)
        {
            Definition.SetEffectDescription(effectDescription);
            return This();
        }

        public TBuilder SetAiParameters(SpellAIParameters aiParameters)
        {
            Definition.SetAiParameters(aiParameters);
            return This();
        }

        public TBuilder SetConcentrationAction(ActionDefinitions.ActionParameter concentrationAction)
        {
            Definition.SetConcentrationAction(concentrationAction);
            return This();
        }
    }

    public class SpellDefinitionBuilder : SpellDefinitionBuilder<SpellDefinition, SpellDefinitionBuilder>
    {
        #region Constructors

        public SpellDefinitionBuilder(string name, string guid) : base(name, guid)
        {
        }

        public SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }

        public SpellDefinitionBuilder(SpellDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public SpellDefinitionBuilder(SpellDefinition original, string name, Guid guidNamespace) : base(original, name,
            guidNamespace)
        {
        }

        #endregion Constructors
    }
}
