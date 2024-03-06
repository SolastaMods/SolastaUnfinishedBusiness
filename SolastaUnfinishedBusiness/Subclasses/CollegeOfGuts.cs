using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static AttributeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfGuts : AbstractSubclass
{
    private const string Name = "CollegeOfGuts";

    public CollegeOfGuts()
    {
        var conditionArcaneDeflection = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArcaneDeflection")
            .SetGuiPresentation($"Power{Name}ArcaneDeflection", Category.Feature, ConditionShielded)
            .AddFeatures(
                MagicAffinityConditionShielded,
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}ArcaneDeflection")
                    .SetGuiPresentation($"Power{Name}ArcaneDeflection", Category.Feature)
                    .SetModifier(AttributeModifierOperation.Additive, ArmorClass, 3)
                    .AddToDB())
            .AddToDB();

        var powerArcaneDeflection = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneDeflection")
            .SetGuiPresentation(Category.Feature, ConditionShielded)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionArcaneDeflection))
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfGuts, 256))
            .AddFeaturesAtLevel(3, FeatureSetCasterFightingProficiency, MagicAffinityCasterFightingCombatMagicImproved)
            .AddFeaturesAtLevel(6, AttributeModifierCasterFightingExtraAttack, AttackReplaceWithCantripCasterFighting)
            .AddFeaturesAtLevel(14, powerArcaneDeflection)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => DatabaseHelper.CharacterClassDefinitions.Bard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
