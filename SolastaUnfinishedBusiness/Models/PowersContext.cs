using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class PowersContext
{
    internal static readonly HashSet<FeatureDefinitionPower> PowersThatIgnoreInterruptions = new();

    private static readonly Guid BazouPowersBaseGuid = new("99cee84d-6187-4d7f-a36e-1bd96d3f2deb");

    private static FeatureDefinitionPower FeatureDefinitionPowerHelpAction { get; set; }

    internal static void LateLoad()
    {
        Switch();
    }

    internal static void Switch()
    {
        var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();
        var subRaces = dbCharacterRaceDefinition
            .SelectMany(x => x.SubRaces);
        var races = dbCharacterRaceDefinition
            .Where(x => !subRaces.Contains(x));

        if (Main.Settings.AddHelpActionToAllRaces)
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => !a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
            {
                characterRaceDefinition.FeatureUnlocks.Add(
                    new FeatureUnlockByLevel(FeatureDefinitionPowerHelpAction, 1));
            }
        }
        else
        {
            foreach (var characterRaceDefinition in races
                         .Where(a => a.FeatureUnlocks.Exists(x =>
                             x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction)))
            {
                characterRaceDefinition.FeatureUnlocks.RemoveAll(x =>
                    x.Level == 1 && x.FeatureDefinition == FeatureDefinitionPowerHelpAction);
            }
        }
    }

    internal static void Load()
    {
        var effectDescription = TrueStrike.EffectDescription.Copy();

        effectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
        effectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
        effectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

        var helpPowerCondition = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionTrueStrike, "ConditionDistractedByAlly",
                DefinitionBuilder.CENamespaceGuid)
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = helpPowerCondition;

        FeatureDefinitionPowerHelpAction = FeatureDefinitionPowerBuilder
            .Create("PowerHelp", BazouPowersBaseGuid)
            .SetGuiPresentation(Category.Feature, Aid.GuiPresentation.SpriteReference)
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
