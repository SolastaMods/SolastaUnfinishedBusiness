using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock.AHSpells
{
    internal sealed class PactMarkSpellBuilder : SpellDefinitionBuilder
    {
        private PactMarkSpellBuilder() : base(SpellDefinitions.HuntersMark, "AHPactMarkSpell", CENamespaceGuid)
        {
            var markedByPactCondition = ConditionDefinitionBuilder
                .Create(ConditionMarkedByHunter, "AHPactMarkMarkedByPactCondition", CENamespaceGuid)
                .SetGuiPresentation(Category.Spell)
                .AddToDB();

            var pactMarkAdditionalDamage = FeatureDefinitionAdditionalDamageBuilder
                .Create(FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, "AHPactMarkAdditionalDamage", CENamespaceGuid)
                .SetGuiPresentation(Category.Spell)
                .SetAttackModeOnly(false)
                .SetRequiredTargetCondition(markedByPactCondition)
                .SetNotificationTag("PactMarked")
                .AddToDB();

            var pactMarkPackMarkCondition = ConditionDefinitionBuilder
                .Create(ConditionHuntersMark, "AHPactMarkPactMarkCondition", CENamespaceGuid)
                .SetGuiPresentation(Category.Spell)
                .SetFeatures(pactMarkAdditionalDamage)
                .AddToDB();

            var markedByPactEffectForm = EffectFormBuilder
                .Create(markedByPactCondition)
                .SetCreatedByCharacter(true)
                .Build();

            var pactMarkEffectForm = EffectFormBuilder
                .Create(pactMarkPackMarkCondition, ConditionForm.ConditionOperation.Add, true)
                .SetCreatedByCharacter(true)
                .Build();

            // TODO: this inherits everything from HuntersMark - is that ok?
            Definition.EffectDescription
                .SetRangeType(RuleDefinitions.RangeType.Distance)
                .SetRangeParameter(24)
                .SetTargetParameter(1)
                .SetEffectForms(markedByPactEffectForm, pactMarkEffectForm);
        }

        public static SpellDefinition CreateAndAddToDB()
        {
            return new PactMarkSpellBuilder()
                .SetSpellLevel(1)
                .SetSomaticComponent(true)
                .SetVerboseComponent(true)
                .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolEnchantment)
                .SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Mundane)
                .SetCastingTime(RuleDefinitions.ActivationTime.BonusAction)
                .SetGuiPresentation(Category.Spell)
                .AddToDB();
        }

        public static readonly SpellDefinition PactMarkSpell = CreateAndAddToDB();
    }
}
