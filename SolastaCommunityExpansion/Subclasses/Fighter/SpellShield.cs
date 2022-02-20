using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Fighter
{
    internal class SpellShield : AbstractSubclass
    {
        private static Guid SubclassNamespace = new("d4732dc2-c4f9-4a35-a12a-ae2d7858ff74");
        private readonly CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal SpellShield()
        {
            // Make Spell Shield subclass
            CharacterSubclassDefinitionBuilder spellShield = CharacterSubclassDefinitionBuilder
                .Create("FighterSpellShield", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass, DomainBattle.GuiPresentation.SpriteReference);

            FeatureDefinitionMagicAffinity magicAffinity = FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinityFighterSpellShield", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
                .SetHandsFullCastingModifiers(true, true, true)
                .SetCastingModifiers(0, 0, true, false, false)
                .AddToDB();
            spellShield.AddFeatureAtLevel(magicAffinity, 3);

            FeatureDefinitionCastSpellBuilder spellCasting = FeatureDefinitionCastSpellBuilder
                .Create("CastSpellSpellShield", SubclassNamespace)
                .SetGuiPresentation("FighterSpellShieldSpellcasting", Category.Subclass)
                .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
                .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
                .SetSpellList(SpellListDefinitions.SpellListWizard)
                .AddRestrictedSchool(SchoolOfMagicDefinitions.SchoolAbjuration)
                .AddRestrictedSchool(SchoolOfMagicDefinitions.SchoolTransmutation)
                .AddRestrictedSchool(SchoolOfMagicDefinitions.SchoolNecromancy)
                .AddRestrictedSchool(SchoolOfMagicDefinitions.SchoolIllusion)
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
                .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
                .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
                .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
                .SetKnownSpells(4, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
                .SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

            spellShield.AddFeatureAtLevel(spellCasting.AddToDB(), 3);

            FeatureDefinitionSavingThrowAffinity spellShieldResistance = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("SpellShieldSpellResistance", SubclassNamespace)
                .SetGuiPresentation("FighterSpellShieldSpellResistance", Category.Subclass)
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Charisma)
                .AddToDB();

            spellShield.AddFeatureAtLevel(spellShieldResistance, 7);
            // or maybe some boost to the spell shield spells?

            FeatureDefinitionAdditionalAction bonusSpell = FeatureDefinitionAdditionalActionBuilder
                .Create("SpellShieldAdditionalAction", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetActionType(ActionDefinitions.ActionType.Main)
                .SetRestrictedActions(ActionDefinitions.Id.CastMain)
                .SetMaxAttacksNumber(-1)
                .SetTriggerCondition(RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy)
                .AddToDB();
            spellShield.AddFeatureAtLevel(bonusSpell, 10);

            EffectDescriptionBuilder arcaneDeflection = new EffectDescriptionBuilder();
            arcaneDeflection.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 0, ActionDefinitions.ItemSelectionType.None);
            ConditionDefinition deflectionCondition = ConditionDefinitionBuilder
                .Create("ConditionSpellShieldArcaneDeflection", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .Configure(definition =>
                {
                    var attributeModifier = FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeSpellShieldArcaneDeflection", SubclassNamespace)
                        .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 3)
                        .SetGuiPresentation("ConditionSpellShieldArcaneDeflection", Category.Subclass, ConditionShielded.GuiPresentation.SpriteReference)
                        .AddToDB();

                    definition.Features.Add(attributeModifier);

                    definition
                        .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                        .SetAllowMultipleInstances(false)
                        .SetDurationType(RuleDefinitions.DurationType.Round)
                        .SetDurationParameter(1);
                })
                .AddToDB();

            arcaneDeflection.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetConditionForm(deflectionCondition, ConditionForm.ConditionOperation.Add,
                true, true, new List<ConditionDefinition>()).Build());

            FeatureDefinitionPower arcaneDeflectionPower = FeatureDefinitionPowerBuilder
                .Create("PowerSpellShieldArcaneDeflection", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass, ConditionShielded.GuiPresentation.SpriteReference)
                .Configure(
                    0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.Reaction, 0, RuleDefinitions.RechargeRate.AtWill,
                    false, false, AttributeDefinitions.Intelligence, arcaneDeflection.Build(), false /* unique instance */)
                .AddToDB();

            spellShield.AddFeatureAtLevel(arcaneDeflectionPower, 15);

            spellShield.AddFeatureAtLevel(
                FeatureDefinitionActionAffinityBuilder
                    .Create(FeatureDefinitionActionAffinitys.ActionAffinityTraditionGreenMageLeafScales, "ActionAffinitySpellShieldRangedDefense", SubclassNamespace)
                    .SetGuiPresentation("PowerSpellShieldRangedDeflection", Category.Subclass)
                    .AddToDB(),
                18);

            Subclass = spellShield.AddToDB();
        }
    }
}
