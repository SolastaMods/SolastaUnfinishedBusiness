using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class PowerSorcerousRestorationBuilder : FeatureDefinitionPowerBuilder
{
    private const string PowerSorcerousRestorationName = "PowerSorcerousRestoration";
    private const string PowerSorcerousRestorationGuid = "a524f8eb-8d30-4614-819d-a8f7df84f73e";

    internal static readonly FeatureDefinitionPower SorcerousRestoration =
        CreateAndAddToDB(PowerSorcerousRestorationName, PowerSorcerousRestorationGuid);

    private PowerSorcerousRestorationBuilder(string name, string guid) : base(name, guid)
    {
        Definition.fixedUsesPerRecharge = 1;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        Definition.usesAbilityScoreName = AttributeDefinitions.Charisma;
        Definition.activationTime = RuleDefinitions.ActivationTime.Rest;
        Definition.costPerUse = 1;
        Definition.rechargeRate = RuleDefinitions.RechargeRate.AtWill;

        var restoration = new EffectDescriptionBuilder();

        restoration.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Self,
            1,
            RuleDefinitions.TargetType.Self);
        restoration.SetParticleEffectParameters(
            DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription
                .EffectParticleParameters);

        var restoreForm = new EffectFormBuilder().CreatedByCharacter().SetSpellForm(9).Build();

        restoreForm.SpellSlotsForm.type = SpellSlotsForm.EffectType.GainSorceryPoints;
        restoreForm.SpellSlotsForm.sorceryPointsGain = 4;
        restoration.AddEffectForm(restoreForm);

        Definition.effectDescription = restoration.Build();

        _ = RestActivityBuilder.RestActivityRestoration;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return new PowerSorcerousRestorationBuilder(name, guid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private sealed class RestActivityBuilder : RestActivityDefinitionBuilder
    {
        private const string SorcerousRestorationName = "SorcerousRestoration";
        private const string SorcerousRestorationGuid = "5ee0315b-43b6-4dd9-8dd4-1eeded1cdb0e";

        // An alternative pattern for lazily creating definition.
        private static RestActivityDefinition _restActivityRestoration;

        private RestActivityBuilder(string name, string guid) : base(
            DatabaseHelper.RestActivityDefinitions.ArcaneRecovery, name, guid)
        {
            Definition.stringParameter = PowerSorcerousRestorationName;
        }

        // get only property
        public static RestActivityDefinition RestActivityRestoration =>
            _restActivityRestoration ??=
                new RestActivityBuilder(SorcerousRestorationName, SorcerousRestorationGuid)
                    .SetGuiPresentation("PowerSorcerousRestoration", Category.Feature)
                    .AddToDB();
    }
}
