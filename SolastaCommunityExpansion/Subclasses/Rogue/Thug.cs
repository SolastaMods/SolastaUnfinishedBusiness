using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.ActionDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.SkillDefinitions;
using static FeatureDefinitionAbilityCheckAffinity;

namespace SolastaCommunityExpansion.Subclasses.Rogue
{
    internal class Thug : AbstractSubclass
    {
        private const int THUG_BONUS_SHOVE_ACTION_ID = 44800;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        private static readonly CharacterSubclassDefinition Subclass = CreateThug();

        private static CharacterSubclassDefinition CreateThug()
        {
            var featureName = AdditionalDamageRogueSneakAttack.Name + "Remove";
            var subclassNamespace = new Guid("b6f6a95faee44d0b87fc2fa65a785e1b");

            var featureToRemove = FeatureDefinitionRemoveGrantedFeatureBuilder
                .Create(featureName, subclassNamespace)
                .SetFeatureInfo(AdditionalDamageRogueSneakAttack, 1, DatabaseHelper.CharacterClassDefinitions.Rogue)
                .SetGuiPresentationNoContent(true)
                .AddToDB();

            var exploitVulnerabilities = FeatureDefinitionAdditionalDamageBuilder
                .Create(AdditionalDamageRogueSneakAttack, "KSRogueSubclassThugExploitVulnerabilities", subclassNamespace)
                .SetGuiPresentation("KSRogueSubclassThugExploitVulnerabilitiesSneakAttack", Category.Feature)
                .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.None)
                .AddToDB();

            var thugProficiencies = FeatureDefinitionProficiencyBuilder
                .Create("KSRogueSubclassThugProficiencies", subclassNamespace)
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory)
                .AddToDB();

            var thugBrutalMethodsAction = ActionDefinitionBuilder
                .Create(ShoveBonus, "KSRogueSubclassThugBrutalMethodsAction", subclassNamespace)
                .SetId((ActionDefinitions.Id)THUG_BONUS_SHOVE_ACTION_ID)
                .AddToDB();

            var thugBrutalMethods = FeatureDefinitionActionAffinityBuilder
                .Create(ActionAffinityThiefFastHands, "KSRogueSubclassThugBrutalMethods", subclassNamespace)
                .SetGuiPresentation(Category.Feature)
                .SetAuthorizedActions(thugBrutalMethodsAction.Id)
                .AddToDB();

            var thugOvercomeCompetition = FeatureDefinitionAbilityCheckAffinityBuilder
                .Create("KSRogueSubclassThugOvercomeCompetition", subclassNamespace)
                .SetGuiPresentation(Category.Feature)
                .SetAffinityGroups(new AbilityCheckAffinityGroup
                {
                    affinity = RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                    proficiencyName = Athletics.Name,
                    abilityScoreName = Athletics.AbilityScore
                })
                .AddToDB();

            return CharacterSubclassDefinitionBuilder
                .Create("KSRogueSubclassThug", subclassNamespace)
                .SetGuiPresentation(Category.Subclass, MartialChampion.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(featureToRemove, 3)
                .AddFeatureAtLevel(exploitVulnerabilities, 3)
                .AddFeatureAtLevel(thugProficiencies, 3)
                .AddFeatureAtLevel(thugBrutalMethods, 9)
                .AddFeatureAtLevel(thugOvercomeCompetition, 13)
                .AddToDB();
        }
    }
}
