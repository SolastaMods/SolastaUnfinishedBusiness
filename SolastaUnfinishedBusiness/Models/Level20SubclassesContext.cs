using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20SubclassesContext
{
    internal static void Load()
    {
        ClericLoad();
        FighterLoad();
        MonkLoad();
        PaladinLoad();
        RogueLoad();
        SorcererLoad();
    }

    private static void ClericLoad()
    {
        // Divine Intervention

        var powerClericDivineInterventionImprovementCleric = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionCleric, "PowerClericDivineInterventionImprovementCleric")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionCleric)
            .AddToDB();

        var powerClericDivineInterventionImprovementPaladin = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionPaladin, "PowerClericDivineInterventionImprovementPaladin")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionPaladin)
            .AddToDB();

        var powerClericDivineInterventionImprovementWizard = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionWizard, "PowerClericDivineInterventionImprovementWizard")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionWizard)
            .AddToDB();

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainMischief.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainOblivion.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
    }

    private static void FighterLoad()
    {
    }

    private static void MonkLoad()
    {
    }

    private static void PaladinLoad()
    {
        AutoPreparedSpellsOathOfDevotion.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, GuardianOfFaith, FreedomOfMovement));

        AutoPreparedSpellsOathOfJugement.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, Banishment, Blight));

        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, WallOfFire, FireShield));

        AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, DreadfulOmen, PhantasmalKiller));
    }

    private static void RogueLoad()
    {
    }

    private static void SorcererLoad()
    {
    }
}
