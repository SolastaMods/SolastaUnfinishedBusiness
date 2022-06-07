using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;

namespace SolastaCommunityExpansion.Level20.Features;

internal sealed class PowerPaladinCleansingTouchBuilder : FeatureDefinitionPowerBuilder
{
    private const string PowerPaladinCleansingTouchName = "ZSPowerPaladinCleansingTouch";
    private const string PowerPaladinCleansingTouchGuid = "71861ca1-61ed-4344-bb26-ef21232adddd";

    internal static readonly FeatureDefinitionPower PowerPaladinCleansingTouch =
        CreateAndAddToDB(PowerPaladinCleansingTouchName, PowerPaladinCleansingTouchGuid);

    private PowerPaladinCleansingTouchBuilder(string name, string guid) : base(name, guid)
    {
        Definition.fixedUsesPerRecharge = 0;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed;
        Definition.usesAbilityScoreName = AttributeDefinitions.Charisma;
        Definition.activationTime = RuleDefinitions.ActivationTime.Action;
        Definition.costPerUse = 1;
        Definition.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
        var cleansingTouch = new EffectDescriptionBuilder();
        cleansingTouch.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals);
        cleansingTouch.SetDurationData(RuleDefinitions.DurationType.Instantaneous, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        cleansingTouch.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.DispelMagic.EffectDescription
            .EffectParticleParameters);

        cleansingTouch.AddEffectForm(new EffectFormBuilder().CreatedByCharacter()
            .SetCounterForm(CounterForm.CounterType.DissipateSpells, 9, 10, true, true).Build());
        cleansingTouch.AddEffectForm(new EffectFormBuilder().CreatedByCharacter()
            .SetAlterationForm(AlterationForm.Type.DissipateSpell).Build());
        Definition.effectDescription = cleansingTouch.Build();
        var cleansingGui = new GuiPresentationBuilder("Feature/&ZSPowerPaladinCleansingTouchTitle",
            "Feature/&ZSPowerPaladinCleansingTouchDescription");
        cleansingGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerPaladinLayOnHands
            .GuiPresentation.SpriteReference);
        Definition.guiPresentation = cleansingGui.Build();
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return new PowerPaladinCleansingTouchBuilder(name, guid).AddToDB();
    }
}
