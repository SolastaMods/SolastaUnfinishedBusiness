using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    internal static void SwitchClericDomainLearningLevel()
    {
        var domains = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Where(x => x.Name.StartsWith("Domain"))
            .ToList();

        var fromLevel = 3;
        var toLevel = 1;

        if (Main.Settings.EnableClericToLearnDomainAtLevel3)
        {
            fromLevel = 1;
            toLevel = 3;
        }

        foreach (var featureUnlock in domains
                     .SelectMany(school => school.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        // handle level 2 grants
        var featuresGrantedAt2 = new[]
        {
            ("DomainBattle", "PowerDomainBattleDecisiveStrike"), //
            ("DomainDefiler", "FeatureSetDomainDefilerDefileLife"), // UB
            ("DomainElementalCold", "PowerDomainElementalIceLance"), //
            ("DomainElementalFire", "PowerDomainElementalFireBurst"), //
            ("DomainElementalLighting", "PowerDomainElementalLightningBlade"), //
            ("DomainInsight", "PowerDomainInsightForeknowledge"), //
            ("DomainLaw", "PowerDomainLawForceOfLaw"), //
            ("DomainLife", "PowerDomainLifePreserveLife"), //
            ("DomainMischief", "PowerDomainMischiefStrikeOfChaos"), //
            ("DomainNature", "FeatureSetDomainNatureCharmAnimalsAndPlants"), // UB
            ("DomainOblivion", "CampAffinityDomainOblivionPeacefulRest"), //
            ("DomainOblivion", "PowerDomainOblivionHeraldOfPain"), //
            ("DomainSmith", "FeatureSetDomainSmithDefileLife"), // UB
            ("DomainSun", "PowerDomainSunHeraldOfTheSun"), //
            ("DomainTempest", "FeatureSetDomainTempestDestructiveWrath") // UB
        };

        var level = Main.Settings.EnableClericToLearnDomainAtLevel3 ? 3 : 2;

        foreach (var (subClassName, featureName) in featuresGrantedAt2)
        {
            var subClass = DatabaseHelper.GetDefinition<CharacterSubclassDefinition>(subClassName);
            var feature = DatabaseHelper.GetDefinition<FeatureDefinition>(featureName);

            subClass.FeatureUnlocks.FirstOrDefault(x => x.FeatureDefinition == feature)!.level = level;
        }

        // change spell casting level on Cleric itself
        Cleric.FeatureUnlocks
            .FirstOrDefault(x =>
                x.FeatureDefinition == SubclassChoiceClericDivineDomains)!.level = toLevel;

        foreach (var domain in domains)
        {
            domain.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Cleric.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }
}
