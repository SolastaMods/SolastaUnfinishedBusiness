using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfGuts : AbstractSubclass
{
    internal CollegeOfGuts()
    {
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CollegeOfGuts")
            .SetGuiPresentation(Category.Subclass, DomainBattle)
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

    internal override DeityDefinition DeityDefinition { get; }
}
