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

            EffectDescription helpActionEffectDescription = new EffectDescription();
            helpActionEffectDescription.Copy(DatabaseHelper.SpellDefinitions.TrueStrike.EffectDescription);
            helpActionEffectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
            helpActionEffectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            helpActionEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            helpActionEffectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetDescription("Power/&FamiliarHelpActionDescription");
            helpActionEffectDescription.EffectForms[0].ConditionForm.ConditionDefinition.GuiPresentation.SetTitle("Power/&FamiliarHelpActionTitle");

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
                    helpActionEffectDescription,
                    new GuiPresentationBuilder(
                        "Power/&HelpActionDescription",
                        "Power/&HelpActionTitle").Build(),
                    true)
                    .AddToDB();

            return helpAction;

        }

    }
}


