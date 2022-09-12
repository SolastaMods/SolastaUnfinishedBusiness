using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Wizard;

internal sealed class WizardMasterManipulator : AbstractSubclass
{
    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal WizardMasterManipulator()
    {
        // Make Control Master subclass
        var arcaneControlAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMasterManipulatorControlHeightened", DefinitionBuilder.CENamespaceGuid)
            .SetWarList(1,
                CharmPerson, // enchantment
                Sleep, // enchantment
                ColorSpray, // illusion
                HoldPerson, // enchantment,
                Invisibility, // illusion
                Counterspell, // abjuration
                DispelMagic, // abjuration
                Banishment, // abjuration
                Confusion, // enchantment
                PhantasmalKiller, // illusion
                DominatePerson, // Enchantment
                HoldMonster) // Enchantment
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyMasterManipulatorMentalSavingThrows", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var powerDominate = FeatureDefinitionPowerBuilder
            .Create("PowerMasterManipulatorDominatePerson", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, DominatePerson.GuiPresentation.SpriteReference)
            .Configure(0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DominatePerson.EffectDescription, false /* unique instance */)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardMasterManipulator", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass,
                RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(arcaneControlAffinity, 2)
            .AddFeatureAtLevel(DcIncreaseAffinity, 6)
            .AddFeatureAtLevel(proficiency, 10)
            .AddFeatureAtLevel(powerDominate, 14).AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    [NotNull]
    private static GuiPresentationBuilder GetSpellDcPresentation()
    {
        return new GuiPresentationBuilder("Feature/&MagicAffinityMasterManipulatorDCTitle",
            Gui.Format("Feature/&MagicAffinityMasterManipulatorDCDescription",
                Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc.ToString()));
    }

    internal static void UpdateSpellDcBoost()
    {
        if (!DcIncreaseAffinity)
        {
            return;
        }

        DcIncreaseAffinity.saveDCModifier = Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc;
        DcIncreaseAffinity.guiPresentation = GetSpellDcPresentation().Build();
    }

    private static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(int attackModifier,
        RuleDefinitions.SpellParamsModifierType attackModifierType, int dcModifier,
        RuleDefinitions.SpellParamsModifierType dcModifierType, string name, GuiPresentation guiPresentation)
    {
        return FeatureDefinitionMagicAffinityBuilder
            .Create(name, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(guiPresentation)
            .SetCastingModifiers(attackModifier, attackModifierType, dcModifier, dcModifierType, false, false,
                false)
            .AddToDB();
    }

    #region DcIncreaseAffinity

    private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;

    private static FeatureDefinitionMagicAffinity DcIncreaseAffinity =>
        _dcIncreaseAffinity ??= BuildMagicAffinityModifiers(0, RuleDefinitions.SpellParamsModifierType.None,
            Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc,
            RuleDefinitions.SpellParamsModifierType.FlatValue, "MagicAffinityMasterManipulatorDC",
            GetSpellDcPresentation().Build());

    #endregion
}
