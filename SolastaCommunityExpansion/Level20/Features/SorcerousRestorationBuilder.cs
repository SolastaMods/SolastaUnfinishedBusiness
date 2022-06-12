using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Level20.Features;

internal sealed class SorcerousRestorationBuilder : FeatureDefinitionPowerBuilder
{
    private const string SorcerousRestorationName = "ZSSorcerousRestoration";
    private const string SorcerousRestorationGuid = "a524f8eb-8d30-4614-819d-a8f7df84f73e";

    internal static readonly FeatureDefinitionPower SorcerousRestoration =
        CreateAndAddToDB(SorcerousRestorationName, SorcerousRestorationGuid);

    private SorcerousRestorationBuilder(string name, string guid) : base(name, guid)
    {
        Definition.fixedUsesPerRecharge = 1;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        Definition.usesAbilityScoreName = AttributeDefinitions.Charisma;
        Definition.activationTime = RuleDefinitions.ActivationTime.Rest;
        Definition.costPerUse = 1;
        Definition.rechargeRate = RuleDefinitions.RechargeRate.AtWill;
        var restoration = new EffectDescriptionBuilder();
        restoration.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
            RuleDefinitions.TargetType.Self);
        restoration.SetParticleEffectParameters(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery
            .EffectDescription.EffectParticleParameters);

        var restoreForm = new EffectFormBuilder().CreatedByCharacter().SetSpellForm(9).Build();
        restoreForm.SpellSlotsForm.type = SpellSlotsForm.EffectType.GainSorceryPoints;
        restoreForm.SpellSlotsForm.sorceryPointsGain = 4;
        restoration.AddEffectForm(restoreForm);
        Definition.effectDescription = restoration.Build();

        var gui = new GuiPresentationBuilder(
            "Sorceror/&ZSSorcerousRestorationTitle",
            "Sorceror/&ZSSorcerousRestorationDescription");
        gui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation
            .SpriteReference);
        Definition.guiPresentation = gui.Build();

        _ = RestActivityBuilder.RestActivityRestoration;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return new SorcerousRestorationBuilder(name, guid).AddToDB();
    }

    private sealed class RestActivityBuilder : RestActivityDefinitionBuilder
    {
        private const string SorcerousRestorationRestName = "ZSSorcerousRestorationRest";
        private const string SorcerousRestorationRestGuid = "5ee0315b-43b6-4dd9-8dd4-1eeded1cdb0e";

        // An alternative pattern for lazily creating definition.
        private static RestActivityDefinition _restActivityRestoration;

        private RestActivityBuilder(string name, string guid) : base(
            DatabaseHelper.RestActivityDefinitions.ArcaneRecovery, name, guid)
        {
            Definition.GuiPresentation.Title = "RestActivity/&ZSSorcerousRestorationTitle";
            Definition.GuiPresentation.Description = "RestActivity/&ZSSorcerousRestorationDescription";
            Definition.stringParameter = SorcerousRestorationName;
        }

        // get only property
        public static RestActivityDefinition RestActivityRestoration =>
            _restActivityRestoration ??=
                new RestActivityBuilder(SorcerousRestorationRestName, SorcerousRestorationRestGuid).AddToDB();
    }
}
