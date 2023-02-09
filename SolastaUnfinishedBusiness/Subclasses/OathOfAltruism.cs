using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using System.Collections;
using System.Collections.Generic;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Models.SpellsContext;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Subclasses;
internal sealed class OathOfAltruism : AbstractSubclass
{
    internal const string NAME = "OathOfAltruism";
    internal const string NAME2 = $"Feature{NAME}DefensiveStrike";
    internal const string NAME3 = $"Feature{NAME}DefensiveStrikeAlly";
    
    internal OathOfAltruism()
    {

        var autoPreparedSpellsAltruism = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("DomainSpells", Category.Feature)
            .SetPreparedSpellGroups(
                BuildSpellGroup(3, Sanctuary, ShieldOfFaith),
                BuildSpellGroup(5, CalmEmotions, HoldPerson),
                BuildSpellGroup(9, Counterspell, HypnoticPattern),
                BuildSpellGroup(13, DominateBeast, GuardianOfFaith),
                BuildSpellGroup(17, HoldMonster, WallOfForce)
                )
            .SetSpellcastingClass(CharacterClassDefinitions.Paladin)
            .AddToDB();

        var featureSpiritualShielding = FeatureDefinitionBuilder
            .Create($"Feature{NAME}SpiritualShielding")
            .SetGuiPresentation(Category.Feature, ShieldOfFaith)
            .SetCustomSubFeatures(BlockAttacks.SpiritualShieldingMarker)
            .AddToDB();
        
        var featureDefensiveStrike = FeatureDefinitionBuilder
            .Create(NAME2)
            .SetGuiPresentation(Category.Feature, PowerDomainBattleDecisiveStrike)
            .SetCustomSubFeatures(DefensiveStrikeAttack.DefensiveStrikeMarker)
            .AddToDB();

        var featureAuraOfTheGuardian = FeatureDefinitionBuilder
            .Create("FeatureAuraOfTheGaurdian")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianConditionMarker)
            .AddToDB();

        var conditionAuraOfTheGuardian = ConditionDefinitionBuilder
            .Create($"Condition{NAME}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Condition, ConditionShielded)
            .SetFeatures(featureAuraOfTheGuardian)
            .AddToDB();
 
        var powerAuraOfTheGuardian = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection,$"Power{NAME}AuraOfTheGuardian")
            .SetGuiPresentation(Category.Feature, GuardianOfFaith)
            .SetCustomSubFeatures(GuardianAuraHpSwap.AuraGuardianUserMarker)
            .AddToDB();

        powerAuraOfTheGuardian.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionAuraOfTheGuardian, ConditionForm.ConditionOperation.Add)
            .Build();


        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
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

}
