using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard;

internal sealed class MasterManipulator : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("af7255d2-8ce2-4398-8999-f1ef536001f6");
    private readonly CharacterSubclassDefinition Subclass;

    internal MasterManipulator()
    {
        // Make Control Master subclass
        var arcaneControlAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityControlHeightened", SubclassNamespace)
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
            .SetGuiPresentation("MagicAffinityMasterManipulatorList", Category.Subclass)
            .AddToDB();

        var proficiency = FeatureDefinitionProficiencyBuilder
            .Create("ManipulatorMentalSavingThrows", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass)
            .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var powerDominate = FeatureDefinitionPowerBuilder
            .Create("PowerManipulatorDominatePerson", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass, DominatePerson.GuiPresentation.SpriteReference)
            .Configure(0,
                RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                DominatePerson.EffectDescription, false /* unique instance */)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MasterManipulator", SubclassNamespace)
            .SetGuiPresentation("TraditionMasterManipulator", Category.Subclass,
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
        return new GuiPresentationBuilder("Subclass/&MagicAffinityMasterManipulatorDCTitle",
            "Subclass/&MagicAffinityMasterManipulatorDC" +
            Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc + "Description");
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
            .Create(name, SubclassNamespace)
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
