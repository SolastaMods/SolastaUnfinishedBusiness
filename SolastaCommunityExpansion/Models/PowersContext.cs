using System;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class PowersContext
    {
        private static readonly Guid BAZOU_POWERS_BASE_GUID = new("99cee84d-6187-4d7f-a36e-1bd96d3f2deb");

        private static FeatureDefinitionPower FeatureDefinitionPowerHelpAction { get; set; }

        internal static void AddToDB()
        {
            LoadHelpPower();
        }

        internal static void Switch()
        {
            SwitchHelpPower();
        }

        internal static void SwitchHelpPower()
        {
            var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();

            if (Main.Settings.AddHelpActionToAllRaces)
            {
                foreach (var characterRaceDefinition in dbCharacterRaceDefinition
                    .Where(a => !a.FeatureUnlocks.Exists(x => x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
                {
                    characterRaceDefinition.AddFeatureUnlock(FeatureDefinitionPowerHelpAction, 1);
                }
            }
            else
            {
                foreach (var characterRaceDefinition in dbCharacterRaceDefinition
                    .Where(a => a.FeatureUnlocks.Exists(x => x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
                {
                    characterRaceDefinition.FeatureUnlocks.RemoveAll(x => x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction);
                }
            }
        }

        private static void LoadHelpPower()
        {
            var effectDescription = TrueStrike.EffectDescription.Copy();

            effectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
            effectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
            effectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

            var helpPowerCondition = ConditionDefinitionBuilder
                .Create(DatabaseHelper.ConditionDefinitions.ConditionTrueStrike, "ConditionHelpPower", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation("HelpAction", Category.Condition)
                .AddToDB();

            effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = helpPowerCondition;

            FeatureDefinitionPowerHelpAction = FeatureDefinitionPowerBuilder
                .Create("HelpAction", BAZOU_POWERS_BASE_GUID)
                .SetGuiPresentation(Category.Power, Aid.GuiPresentation.SpriteReference)
                .Configure(
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
                    true)
                .AddToDB();
        }
    }
}
