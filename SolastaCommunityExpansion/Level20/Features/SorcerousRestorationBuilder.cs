using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class SorcerousRestorationBuilder : FeatureDefinitionPowerBuilder
    {
        private const string SorcerousRestorationName = "ZSSorcerousRestoration";
        private const string SorcerousRestorationGuid = "a524f8eb-8d30-4614-819d-a8f7df84f73e";

        private SorcerousRestorationBuilder(string name, string guid) : base(name, guid)
        {
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            Definition.SetUsesAbilityScoreName(AttributeDefinitions.Charisma);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Rest);
            Definition.SetCostPerUse(1);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            EffectDescriptionBuilder restoration = new EffectDescriptionBuilder();
            restoration.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            restoration.SetParticleEffectParameters(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.EffectDescription.EffectParticleParameters);

            EffectForm restoreForm = new EffectFormBuilder().CreatedByCharacter().SetSpellForm(9).Build();
            restoreForm.SpellSlotsForm.SetType(SpellSlotsForm.EffectType.GainSorceryPoints);
            restoreForm.SpellSlotsForm.SetSorceryPointsGain(4);
            restoration.AddEffectForm(restoreForm);
            Definition.SetEffectDescription(restoration.Build());

            GuiPresentationBuilder gui = new GuiPresentationBuilder(
               "Sorceror/&ZSSorcerousRestorationTitle",
               "Sorceror/&ZSSorcerousRestorationDescription");
            gui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation.SpriteReference);
            Definition.SetGuiPresentation(gui.Build());

            _ = RestActivityBuilder.RestActivityRestoration;
        }

        private sealed class RestActivityBuilder : RestActivityDefinitionBuilder
        {
            private const string SorcerousRestorationRestName = "ZSSorcerousRestorationRest";
            private const string SorcerousRestorationRestGuid = "5ee0315b-43b6-4dd9-8dd4-1eeded1cdb0e";

            private RestActivityBuilder(string name, string guid) : base(DatabaseHelper.RestActivityDefinitions.ArcaneRecovery, name, guid)
            {
                Definition.GuiPresentation.Title = "RestActivity/&ZSSorcerousRestorationTitle";
                Definition.GuiPresentation.Description = "RestActivity/&ZSSorcerousRestorationDescription";
                Definition.SetStringParameter(SorcerousRestorationName);
            }

            // An alternative pattern for lazily creating definition.
            private static RestActivityDefinition _restActivityRestoration;

            // get only property
            public static RestActivityDefinition RestActivityRestoration =>
                _restActivityRestoration ??= new RestActivityBuilder(SorcerousRestorationRestName, SorcerousRestorationRestGuid).AddToDB();
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new SorcerousRestorationBuilder(name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionPower SorcerousRestoration =
            CreateAndAddToDB(SorcerousRestorationName, SorcerousRestorationGuid);
    }
}
