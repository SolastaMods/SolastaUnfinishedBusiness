using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfGuts : AbstractSubclass
{
    internal CollegeOfGuts()
    {
        var magicAffinityCollegeOfGutsCombatMagic = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityCollegeOfGutsCombatMagic")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
            .AddToDB();

        var powerCollegeOfGutsWarMagic = FeatureDefinitionPowerBuilder
            .Create("PowerCollegeOfGutsWarMagic")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSpellCast)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, validateDuration: false)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionCollegeOfGutsWarMagic")
                                    .SetGuiPresentationNoContent(true)
                                    .AddFeatures(FeatureDefinitionAttackModifiers.AttackModifierBerserkerFrenzy)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build()
                    )
                    .Build())
            .AddToDB();

        var replaceAttackWithCantripCollegeOfGuts = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripCollegeOfGuts")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfGuts")
            .SetGuiPresentation(Category.Subclass, DomainBattle)
            .AddFeaturesAtLevel(3,
                FeatureSetCasterFighting,
                magicAffinityCollegeOfGutsCombatMagic)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                replaceAttackWithCantripCollegeOfGuts)
            .AddFeaturesAtLevel(14,
                powerCollegeOfGutsWarMagic)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;
}
