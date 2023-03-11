using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfGuts : AbstractSubclass
{
    internal CollegeOfGuts()
    {
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfGuts")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("CollegeOfGuts", Resources.CollegeOfGuts, 256))
            .AddFeaturesAtLevel(3,
                FeatureSetCasterFightingProficiency,
                MagicAffinityCasterFightingCombatMagic)
            .AddFeaturesAtLevel(6,
                AttributeModifierCasterFightingExtraAttack,
                ReplaceAttackWithCantripCasterFighting)
            .AddFeaturesAtLevel(14,
                PowerCasterFightingWarMagic)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
