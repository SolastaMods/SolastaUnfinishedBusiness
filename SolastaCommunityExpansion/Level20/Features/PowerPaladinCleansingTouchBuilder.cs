using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class PowerPaladinCleansingTouchBuilder : FeatureDefinitionPowerBuilder
    {
        private const string PowerPaladinCleansingTouchName = "ZSPowerPaladinCleansingTouch";
        private const string PowerPaladinCleansingTouchGuid = "71861ca1-61ed-4344-bb26-ef21232adddd";

        private PowerPaladinCleansingTouchBuilder(string name, string guid) : base(name, guid)
        {
            Definition.SetFixedUsesPerRecharge(0);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed);
            Definition.SetUsesAbilityScoreName(AttributeDefinitions.Charisma);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetCostPerUse(1);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            EffectDescriptionBuilder cleansingTouch = new EffectDescriptionBuilder();
            cleansingTouch.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Touch, 1, RuleDefinitions.TargetType.Individuals, 1, 1, ActionDefinitions.ItemSelectionType.None);
            cleansingTouch.SetDurationData(RuleDefinitions.DurationType.Instantaneous, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            cleansingTouch.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.DispelMagic.EffectDescription.EffectParticleParameters);

            cleansingTouch.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetCounterForm(CounterForm.CounterType.DissipateSpells, 9, 10, true, true).Build());
            cleansingTouch.AddEffectForm(new EffectFormBuilder().CreatedByCharacter().SetAlterationForm(AlterationForm.Type.DissipateSpell).Build());
            Definition.SetEffectDescription(cleansingTouch.Build());
            GuiPresentationBuilder cleansingGui = new GuiPresentationBuilder("Feature/&ZSPowerPaladinCleansingTouchTitle", "Feature/&ZSPowerPaladinCleansingTouchDescription");
            cleansingGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerPaladinLayOnHands.GuiPresentation.SpriteReference);
            Definition.SetGuiPresentation(cleansingGui.Build());
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new PowerPaladinCleansingTouchBuilder(name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionPower PowerPaladinCleansingTouch =
            CreateAndAddToDB(PowerPaladinCleansingTouchName, PowerPaladinCleansingTouchGuid);
    }
}
