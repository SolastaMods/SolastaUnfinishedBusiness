using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Subclasses.Fighter
{
    internal class SpellShield : AbstractSubclass
    {
        private static Guid SubclassNamespace = new Guid("d4732dc2-c4f9-4a35-a12a-ae2d7858ff74");
        private readonly CharacterSubclassDefinition Subclass;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
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
                "Subclass/&FighterSpellShieldDescription",
                "Subclass/&FighterSpellShieldTitle");
            spellShieldPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference);
            spellShield.SetGuiPresentation(spellShieldPresentation.Build());

            GuiPresentationBuilder combatCastingPresentation = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityFighterSpellShieldDescription",
                "Subclass/&MagicAffinityFighterSpellShieldTitle");
            FeatureDefinitionMagicAffinity magicAffinity = new FeatureDefinitionMagicAffinityBuilder("MagicAffinityFighterSpellShield",
                GuidHelper.Create(SubclassNamespace, "MagicAffinityFighterSpellShield").ToString(),
                combatCastingPresentation.Build()).SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0).SetHandsFullCastingModifiers(true, true, true)
                .SetCastingModifiers(0, 0, true, false, false).AddToDB();
            spellShield.AddFeatureAtLevel(magicAffinity, 3);

            CastSpellBuilder spellCasting = new CastSpellBuilder("CastSpellSpellShield", GuidHelper.Create(SubclassNamespace, "CastSpellSpellShield").ToString());
            spellCasting.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass);
            spellCasting.SetSpellCastingAbility(AttributeDefinitions.Intelligence);
            spellCasting.SetSpellList(DatabaseHelper.SpellListDefinitions.SpellListWizard);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolAbjuration);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolTransmutation);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolNecromancy);
            spellCasting.AddRestrictedSchool(DatabaseHelper.SchoolOfMagicDefinitions.SchoolIllusion);
            spellCasting.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection);
            spellCasting.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);
            spellCasting.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            spellCasting.SetKnownCantrips(3, 3, CastSpellBuilder.CasterProgression.THIRD_CASTER);
            spellCasting.SetKnownSpells(4, 3, CastSpellBuilder.CasterProgression.THIRD_CASTER);
            spellCasting.SetSlotsPerLevel(3, CastSpellBuilder.CasterProgression.THIRD_CASTER);
            GuiPresentationBuilder spellcastGui = new GuiPresentationBuilder(
                "Subclass/&FighterSpellShieldSpellcastingDescription",
                "Subclass/&FighterSpellShieldSpellcastingTitle");
            spellCasting.SetGuiPresentation(spellcastGui.Build());
            spellShield.AddFeatureAtLevel(spellCasting.AddToDB(), 3);

            GuiPresentationBuilder spellResistance = new GuiPresentationBuilder(
                "Subclass/&FighterSpellShieldSpellResistanceDescription",
                "Subclass/&FighterSpellShieldSpellResistanceTitle");
            // add a saving throw affinity against spells or something?
            FeatureDefinitionSavingThrowAffinity spellShieldResistance = BuildSavingThrowAffinity(new List<string>()
            {
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Charisma,
            }, RuleDefinitions.CharacterSavingThrowAffinity.Advantage, true, "SpellShieldSpellResistance", spellResistance.Build());
            spellShield.AddFeatureAtLevel(spellShieldResistance, 7);
            // or maybe some boost to the spell shield spells?

            GuiPresentationBuilder bonusSpellGui = new GuiPresentationBuilder(
                "Subclass/&SpellShieldAdditionalActionDescription",
                "Subclass/&SpellShieldAdditionalActionTitle");
            FeatureDefinitionAdditionalAction bonusSpell = new FeatureDefinitionAdditionalActionBuilder("SpellShieldAdditionalAction",
                GuidHelper.Create(SubclassNamespace, "SpellShieldAdditionalAction").ToString(), ActionDefinitions.ActionType.Main, new List<ActionDefinitions.Id>(),
                new List<ActionDefinitions.Id>(), new List<ActionDefinitions.Id>()
                {
                    ActionDefinitions.Id.CastMain,
                }, -1, RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy,
                bonusSpellGui.Build()).AddToDB();
            spellShield.AddFeatureAtLevel(bonusSpell, 10);

            GuiPresentationBuilder arcaneDeflectionGuiCondition = new GuiPresentationBuilder(
                "Subclass/&ConditionSpellShieldArcaneDeflectionDescription",
                "Subclass/&ConditionSpellShieldArcaneDeflectionTitle");
            arcaneDeflectionGuiCondition.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference);
            EffectDescriptionBuilder arcaneDeflection = new EffectDescriptionBuilder();
            arcaneDeflection.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 0, ActionDefinitions.ItemSelectionType.None);
            ConditionDefinition deflectionCondition = new ConditionDefinitionBuilder("ConditionSpellShieldArcaneDeflection", GuidHelper.Create(SubclassNamespace, "ConditionSpellShieldArcaneDeflection").ToString(),
                new List<FeatureDefinition>() {
                    new FeatureDefinitionAttributeModifierBuilder("AttributeSpellShieldArcaneDeflection", GuidHelper.Create(SubclassNamespace, "AttributeSpellShieldArcaneDeflection").ToString(),
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 3, arcaneDeflectionGuiCondition.Build()).AddToDB(),
                }, RuleDefinitions.DurationType.Round, 1, false, arcaneDeflectionGuiCondition.Build()).AddToDB();
            arcaneDeflection.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetConditionForm(deflectionCondition, ConditionForm.ConditionOperation.Add,
                true, true, new List<ConditionDefinition>()).Build());

            GuiPresentationBuilder arcaneDeflectionGuiPower = new GuiPresentationBuilder(
                "Subclass/&PowerSpellShieldArcaneDeflectionDescription",
                "Subclass/&PowerSpellShieldArcaneDeflectionTitle");
            arcaneDeflectionGuiCondition.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionShielded.GuiPresentation.SpriteReference);
            FeatureDefinitionPower arcaneDeflectionPower = new FeatureDefinitionPowerBuilder("PowerSpellShieldArcaneDeflection", GuidHelper.Create(SubclassNamespace, "PowerSpellShieldArcaneDeflection").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.Reaction, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Intelligence, arcaneDeflection.Build(), arcaneDeflectionGuiPower.Build(), false /* unique instance */).AddToDB();
            spellShield.AddFeatureAtLevel(arcaneDeflectionPower, 15);

            GuiPresentationBuilder rangedDeflectionGuiPower = new GuiPresentationBuilder(
                "Subclass/&PowerSpellShieldRangedDeflectionDescription",
                "Subclass/&PowerSpellShieldRangedDeflectionTitle");
            spellShield.AddFeatureAtLevel(new SpellShieldRangedDeflection(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityTraditionGreenMageLeafScales,
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

        public static FeatureDefinitionSavingThrowAffinity BuildSavingThrowAffinity(List<string> abilityScores,
            RuleDefinitions.CharacterSavingThrowAffinity affinityType, bool againstMagic, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionSavingThrowAffinityBuilder builder = new FeatureDefinitionSavingThrowAffinityBuilder(name, GuidHelper.Create(SubclassNamespace, name).ToString(),
                abilityScores, affinityType, againstMagic, guiPresentation);
            return builder.AddToDB();
        }
    }
}
