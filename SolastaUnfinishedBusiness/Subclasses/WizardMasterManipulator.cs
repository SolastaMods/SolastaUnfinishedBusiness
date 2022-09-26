using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardMasterManipulator : AbstractSubclass
{
    private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal WizardMasterManipulator()
    {
        // Make Control Master subclass
        var arcaneControlAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMasterManipulatorControlHeightened")
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
            .Create("ProficiencyMasterManipulatorMentalSavingThrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var powerDominate = FeatureDefinitionPowerBuilder
            .Create("PowerMasterManipulatorDominatePerson")
            .SetGuiPresentation(Category.Feature, DominatePerson.GuiPresentation.SpriteReference)
            .Configure(0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Intelligence,
                DominatePerson.EffectDescription)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardMasterManipulator")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(2, arcaneControlAffinity)
            .AddFeaturesAtLevel(6, DcIncreaseAffinity)
            .AddFeaturesAtLevel(10, proficiency)
            .AddFeaturesAtLevel(14, powerDominate)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

#if false
    [NotNull]
    private static GuiPresentationBuilder GetSpellDcPresentation()
    {
        return new GuiPresentationBuilder(
            "Feature/&MagicAffinityMasterManipulatorDCTitle",
            Gui.Format(
                "Feature/&MagicAffinityMasterManipulatorDCDescription",
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
#endif

    private static FeatureDefinitionMagicAffinity DcIncreaseAffinity =>
        _dcIncreaseAffinity ??= FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityMasterManipulatorDC")
            .SetGuiPresentation(Category.Feature)
            .SetCastingModifiers(
                0,
                RuleDefinitions.SpellParamsModifierType.None,
                2, // Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc,
                RuleDefinitions.SpellParamsModifierType.FlatValue,
                false,
                false,
                false)
            .AddToDB();
}
