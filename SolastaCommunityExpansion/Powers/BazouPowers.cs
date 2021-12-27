using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Features;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Powers
{
    internal static class BazouPowers
    {
        public static readonly Guid BAZOU_POWERS_BASE_GUID = new Guid("99cee84d-6187-4d7f-a36e-1bd96d3f2deb");

        public static void CreatePowers(List<FeatureDefinitionPower> powers)
        {

            powers.Add(BuildHelpAction());

        }

        private static FeatureDefinitionPower BuildHelpAction()
        {

            var effectDescription = new EffectDescription();
            effectDescription.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription);
            effectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
            effectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            effectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            effectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetDescription("Condition/&HelpActionDescription");
            effectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetTitle("Condition/&HelpActionTitle");

            var helpAction = new FeatureDefinitionPowerBuilder(
                    "HelpAction",
                    GuidHelper.Create(BAZOU_POWERS_BASE_GUID, "HelpAction").ToString(),
                    1,
                    RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Charisma,
                    RuleDefinitions.ActivationTime.Action,
                    0,
                    RuleDefinitions.RechargeRate.AtWill,
                    false,
                    false,
                    AttributeDefinitions.Charisma,
                    effectDescription,
                    new GuiPresentationBuilder(
                        "Power/&HelpActionDescription",
                        "Power/&HelpActionTitle").Build()
                        .SetSpriteReference(DatabaseHelper.SpellDefinitions.Aid.GuiPresentation.SpriteReference),
                    true)
                    .AddToDB();

            return helpAction;

        }

    }
}


