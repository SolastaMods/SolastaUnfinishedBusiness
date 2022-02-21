using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
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
        private static readonly Guid SubclassNamespace = new("b6f6a95faee44d0b87fc2fa65a785e1b");

        private readonly CharacterSubclassDefinition Subclass;

        private const string RogueSubclassThugName = "KSRogueSubclassThug";

        private static readonly string RogueSubclassThugNameGuid = GuidHelper.Create(SubclassNamespace, RogueSubclassThugName).ToString();

        private const int THUG_BONUS_SHOVE_ACTION_ID = 44800;

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal Thug()
        {
            var featureName = AdditionalDamageRogueSneakAttack.Name + "Remove";

            var featureToRemove = FeatureDefinitionRemoveGrantedFeatureBuilder
                .Create(featureName, SubclassNamespace)
                .SetFeatureInfo(AdditionalDamageRogueSneakAttack, 1, DatabaseHelper.CharacterClassDefinitions.Rogue)
                .SetGuiPresentationNoContent(true)
                .AddToDB();

            Subclass = CharacterSubclassDefinitionBuilder
                .Create(RogueSubclassThugName, RogueSubclassThugNameGuid)
                .SetGuiPresentation(Category.Subclass, MartialChampion.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(featureToRemove, 3)
                .AddFeatureAtLevel(ExploitVulnerabilities, 3)
                .AddFeatureAtLevel(ThugProficiencies, 3)
                .AddFeatureAtLevel(ThugBrutalMethods, 9)
                .AddFeatureAtLevel(ThugOvercomeCompetition, 13)
                .AddToDB();
        }

        internal static FeatureDefinitionAdditionalDamage exploitVulnerabilities;
        internal static FeatureDefinitionAdditionalDamage ExploitVulnerabilities
        {
            get => exploitVulnerabilities ??= FeatureDefinitionAdditionalDamageBuilder
                .Create("KSRogueSubclassThugExploitVulnerabilities", SubclassNamespace)
                .SetGuiPresentation("KSRogueSubclassThugExploitVulnerabilitiesSneakAttack", Category.Feature)
                .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.None)
                .AddToDB();
        }

        internal static FeatureDefinitionProficiency thugProficiencies;
        internal static FeatureDefinitionProficiency ThugProficiencies
        {
            get => thugProficiencies ??= FeatureDefinitionProficiencyBuilder
                .Create("KSRogueSubclassThugProficiencies", SubclassNamespace)
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory)
                .AddToDB();
        }

        internal static FeatureDefinitionActionAffinity thugBrutalMethods;
        internal static FeatureDefinitionActionAffinity ThugBrutalMethods
        {
            get => thugBrutalMethods ??= FeatureDefinitionActionAffinityBuilder
                .Create(ActionAffinityThiefFastHands, "KSRogueSubclassThugBrutalMethods", SubclassNamespace)
                .SetGuiPresentation(Category.Feature)
                .SetAuthorizedActions(ThugBrutalMethodsAction.Id)
                .AddToDB();
        }

        internal static ActionDefinition thugBrutalMethodsAction;
        internal static ActionDefinition ThugBrutalMethodsAction
        {
            get => thugBrutalMethodsAction ??= ActionDefinitionBuilder
                .Create(ShoveBonus, "KSRogueSubclassThugBrutalMethodsAction", SubclassNamespace)
                // TODO: No GuiPresentation?  SetGuiPresentationNoContent()?
                .SetId((ActionDefinitions.Id)THUG_BONUS_SHOVE_ACTION_ID)
                .AddToDB();
        }

        internal static FeatureDefinitionAbilityCheckAffinity thugOvercomeCompetition;
        internal static FeatureDefinitionAbilityCheckAffinity ThugOvercomeCompetition
        {
            get => thugOvercomeCompetition ??= FeatureDefinitionAbilityCheckAffinityBuilder
              .Create("KSRogueSubclassThugOvercomeCompetition", SubclassNamespace)
              .SetGuiPresentation(Category.Feature)
              .SetAffinityGroups(new AbilityCheckAffinityGroup
              {
                  affinity = RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                  proficiencyName = Athletics.Name,
                  abilityScoreName = Athletics.AbilityScore
              })
              .AddToDB();
        }
    }
}
