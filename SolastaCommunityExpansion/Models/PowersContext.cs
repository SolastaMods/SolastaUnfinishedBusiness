using System;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models;

internal static class PowersContext
{
    private static readonly Guid BAZOU_POWERS_BASE_GUID = new("99cee84d-6187-4d7f-a36e-1bd96d3f2deb");

    private static FeatureDefinitionPower FeatureDefinitionPowerHelpAction { get; set; }

    internal static void Load()
    {
        LoadHelpPower();
    }

    internal static void LateLoad()
    {
        Switch();
    }

    internal static void Switch()
    {
        SwitchHelpPower();
    }

    internal static void SwitchHelpPower()
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

    private static void LoadHelpPower()
    {
        var effectDescription = TrueStrike.EffectDescription.Copy();

        effectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);
        effectDescription.SetDurationType(RuleDefinitions.DurationType.Round);
        effectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);

        var helpPowerCondition = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionTrueStrike, "ConditionHelpPower",
                DefinitionBuilder.CENamespaceGuid)
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

    public static void ApplyPowerEffectForms(FeatureDefinitionPower power, RulesetCharacter source,
        RulesetCharacter target, string logTag = null)
    {
        var ruleset = ServiceRepository.GetService<IRulesetImplementationService>();

        if (logTag != null)
        {
            target.AdditionalDamageGenerated(source, target, RuleDefinitions.DieType.D1, 0, 0, logTag);
        }

        var usablePower = UsablePowersProvider.Get(power, source);
        var effectPower = ruleset.InstantiateEffectPower(source, usablePower, false);
        var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
        formsParams.FillSourceAndTarget(source, target);
        formsParams.FillFromActiveEffect(effectPower);
        formsParams.FillSoloPowerSpecialParameters();
        formsParams.effectSourceType = RuleDefinitions.EffectSourceType.Power;
        ruleset.ApplyEffectForms(power.EffectDescription.EffectForms, formsParams);
        ruleset.ClearDamageFormsByIndex();
        ruleset.TerminateEffect(effectPower, false);
    }
}
