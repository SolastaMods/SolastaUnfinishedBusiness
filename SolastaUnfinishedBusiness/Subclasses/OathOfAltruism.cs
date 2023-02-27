using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class OathOfAltruism : AbstractSubclass
{
    private const string Name = "OathOfAltruism";
    internal const string DefensiveStrike = $"Feature{Name}DefensiveStrike";

    internal OathOfAltruism()
    {
        var autoPreparedSpellsAltruism = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Oath")
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, HealingWord, ShieldOfFaith),
                BuildSpellGroup(5, CalmEmotions, HoldPerson),
                BuildSpellGroup(9, Counterspell, HypnoticPattern),
                BuildSpellGroup(13, DominateBeast, GuardianOfFaith),
                BuildSpellGroup(17, HoldMonster, WallOfForce)
            )
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var featureSpiritualShielding = FeatureDefinitionBuilder
            .Create($"Feature{Name}SpiritualShielding")
            .SetGuiPresentation(Category.Feature, ShieldOfFaith)
            .SetCustomSubFeatures(BlockAttacks.SpiritualShieldingMarker)
            .AddToDB();

        var featureDefensiveStrike = FeatureDefinitionBuilder
            .Create(DefensiveStrike)
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDecisiveStrike)
            .SetCustomSubFeatures(DefensiveStrikeMarker.Mark)
            .AddToDB();

        var featureAuraOfTheGuardian = FeatureDefinitionBuilder
            .Create("FeatureAuraOfTheGuardian")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianConditionMarker)
            .AddToDB();

        var conditionAuraOfTheGuardian = ConditionDefinitionBuilder
            .Create($"Condition{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Condition, ConditionShielded)
            .SetFeatures(featureAuraOfTheGuardian)
            .AddToDB();

        var powerAuraOfTheGuardian = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Feature, GuardianOfFaith)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianUserMarker)
            .AddToDB();

        powerAuraOfTheGuardian.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraOfTheGuardian, ConditionForm.ConditionOperation.Add)
            .Build();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, DomainLife)
            .AddFeaturesAtLevel(3,
                autoPreparedSpellsAltruism,
                featureDefensiveStrike,
                featureSpiritualShielding)
            .AddFeaturesAtLevel(7, powerAuraOfTheGuardian)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => FeatureDefinitionSubclassChoices
        .SubclassChoicePaladinSacredOaths;

    internal override DeityDefinition DeityDefinition { get; }
}
