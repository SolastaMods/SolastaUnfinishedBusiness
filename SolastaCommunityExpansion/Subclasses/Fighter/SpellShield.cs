using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;

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
            CharacterSubclassDefinitionBuilder spellShield = new CharacterSubclassDefinitionBuilder("FighterSpellShield", GuidHelper.Create(SubclassNamespace, "FighterSpellShield").ToString());
            GuiPresentationBuilder spellShieldPresentation = new GuiPresentationBuilder(
                "Subclass/&FighterSpellShieldTitle",
                "Subclass/&FighterSpellShieldDescription");
            spellShieldPresentation.SetSpriteReference(CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference);
            spellShield.SetGuiPresentation(spellShieldPresentation.Build());

            GuiPresentationBuilder combatCastingPresentation = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityFighterSpellShieldTitle",
                "Subclass/&MagicAffinityFighterSpellShieldDescription");
            FeatureDefinitionMagicAffinity magicAffinity = new FeatureDefinitionMagicAffinityBuilder("MagicAffinityFighterSpellShield",
                GuidHelper.Create(SubclassNamespace, "MagicAffinityFighterSpellShield").ToString(),
                combatCastingPresentation.Build()).SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0).SetHandsFullCastingModifiers(true, true, true)
                .SetCastingModifiers(0, 0, true, false, false).AddToDB();
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

            FeatureDefinitionSavingThrowAffinity spellShieldResistance = FeatureDefinitionSavingThrowAffinityBuilder.Create("SpellShieldSpellResistance", SubclassNamespace)
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
            ConditionDefinition deflectionCondition = new ConditionDefinitionBuilder("ConditionSpellShieldArcaneDeflection", GuidHelper.Create(SubclassNamespace, "ConditionSpellShieldArcaneDeflection").ToString(),
                new List<FeatureDefinition>() {
                    FeatureDefinitionAttributeModifierBuilder
                        .Create("AttributeSpellShieldArcaneDeflection", SubclassNamespace)
                        .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 3)
                        .SetGuiPresentation("ConditionSpellShieldArcaneDeflection", Category.Subclass, ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference)
                        .AddToDB(),
                },
                RuleDefinitions.DurationType.Round, 1, false)
                .SetGuiPresentation(Category.Subclass)
                .AddToDB();
            arcaneDeflection.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetConditionForm(deflectionCondition, ConditionForm.ConditionOperation.Add,
                true, true, new List<ConditionDefinition>()).Build());

            GuiPresentationBuilder arcaneDeflectionGuiPower = new GuiPresentationBuilder(
                "Subclass/&PowerSpellShieldArcaneDeflectionTitle",
                "Subclass/&PowerSpellShieldArcaneDeflectionDescription");
            arcaneDeflectionGuiPower.SetSpriteReference(ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference);
            FeatureDefinitionPower arcaneDeflectionPower = new FeatureDefinitionPowerBuilder("PowerSpellShieldArcaneDeflection", GuidHelper.Create(SubclassNamespace, "PowerSpellShieldArcaneDeflection").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.Reaction, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Intelligence, arcaneDeflection.Build(), arcaneDeflectionGuiPower.Build(), false /* unique instance */).AddToDB();
            spellShield.AddFeatureAtLevel(arcaneDeflectionPower, 15);

            GuiPresentationBuilder rangedDeflectionGuiPower = new GuiPresentationBuilder(
                "Subclass/&PowerSpellShieldRangedDeflectionTitle",
                "Subclass/&PowerSpellShieldRangedDeflectionDescription");
            spellShield.AddFeatureAtLevel(new SpellShieldRangedDeflection(FeatureDefinitionActionAffinitys.ActionAffinityTraditionGreenMageLeafScales,
                "ActionAffinitySpellShieldRangedDefense", GuidHelper.Create(SubclassNamespace, "ActionAffinitySpellShieldRangedDefense").ToString(), rangedDeflectionGuiPower.Build()).AddToDB(),
                18);

            Subclass = spellShield.AddToDB();
        }

        private sealed class SpellShieldRangedDeflection : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
        {
            public SpellShieldRangedDeflection(FeatureDefinitionActionAffinity original, string name, string guid, GuiPresentation guiPresentation) : base(original, name, guid)
            {
                Definition.SetGuiPresentation(guiPresentation);
            }
        }
    }
}
