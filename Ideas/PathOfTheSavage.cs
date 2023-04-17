#if false
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

// ReSharper disable once IdentifierTypo
internal sealed class PathOfTheSavage : AbstractSubclass
{
    private const string Name = "PathOfTheSavage";

    internal PathOfTheSavage()
    {
        // MAIN

        // LEVEL 03

        // Savage Flourish

        var attackModifierSavageFlourish = FeatureDefinitionAttackModifierBuilder
            .Create(FeatureDefinitionAttackModifiers.AttackModifierFeatAmbidextrous,
                $"AttackModifier{Name}SavageFlourish")
            .AddToDB();

        var blockRegularRageStart = FeatureDefinitionActionAffinityBuilder
            .Create("OnAfterActionRageStart")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes()
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.CombatRageStart)
            .SetForbiddenActions(ActionDefinitions.Id.RageStart)
            .AddToDB();
        
        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheReaver, 256))
            .AddFeaturesAtLevel(3, attackModifierSavageFlourish, BuildRageStartPower(), blockRegularRageStart)
            .AddFeaturesAtLevel(6)
            .AddFeaturesAtLevel(10)
            .AddFeaturesAtLevel(14)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPower BuildRageStartPower()
    {
        if (!TryGetDefinition<ActionDefinition>("RageStart", out var baseAction))
        {
            Main.Error("Couldn't fine RageStart action!");
            return null;
        }

        const string NAME = $"Power{Name}RageStartCombat";

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerBarbarianRageStart)
            .SetSharedPool(ActivationTime.NoCost, PowerBarbarianRageStart)
            .DelegatedToAction()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetParticleEffectParameters(PowerBarbarianRageStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionRagingNormal,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CombatRageStart")
            .SetGuiPresentation(NAME, Category.Feature, baseAction, baseAction.GuiPresentation.SortOrder)
            .OverrideClassName("RageStart")
            .SetActionId(ExtraActionId.CombatRageStart)
            .SetActionType(ActionDefinitions.ActionType.NoCost)
            .SetActivatedPower(power)
            .AddToDB();

        return power;
    }
}
#endif
