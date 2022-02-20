using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;

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

            Subclass = CharacterSubclassDefinitionBuilder
                .Create(RogueSubclassThugName, RogueSubclassThugNameGuid)
                .SetGuiPresentation(Category.Subclass, MartialChampion.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(new RemoveGrantedFeatureBuilder(
                    featureName,
                    GuidHelper.Create(SubclassNamespace, featureName).ToString(),
                    AdditionalDamageRogueSneakAttack,
                    1,
                    DatabaseHelper.CharacterClassDefinitions.Rogue)
                    .SetGuiPresentationNoContent()
                    .AddToDB(), 3)
                .AddFeatureAtLevel(RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder.ExploitVulnerabilities, 3)
                .AddFeatureAtLevel(RogueSubclassThugProficienciesBuilder.ThugProficiencies, 3)
                .AddFeatureAtLevel(RogueSubclassThugBrutalMethodsBuilder.ThugBrutalMethods, 9)
                .AddFeatureAtLevel(RogueSubclassThugOvercomeCompetitionBuilder.ThugOvercomeCompetition, 13)
                .AddToDB();
        }

        private sealed class RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder : FeatureDefinitionAdditionalDamageBuilder
        {
            private const string ExploitVulnerabilitiesName = "KSRogueSubclassThugExploitVulnerabilities";
            private static readonly string ExploitVulnerabilitiesGuid = GuidHelper.Create(SubclassNamespace, ExploitVulnerabilitiesName).ToString();

            private RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder(string name, string guid) : base(AdditionalDamageRogueSneakAttack, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugExploitVulnerabilitiesSneakAttackTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugExploitVulnerabilitiesSneakAttackDescription";
                Definition.SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.None);
            }

            private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
            {
                return new RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionAdditionalDamage ExploitVulnerabilities =
                CreateAndAddToDB(ExploitVulnerabilitiesName, ExploitVulnerabilitiesGuid);
        }

        private sealed class RogueSubclassThugProficienciesBuilder : FeatureDefinitionProficiencyBuilder
        {
            private const string RogueSubclassThugProficienciesName = "KSRogueSubclassThugProficiencies";
            private static readonly string RogueSubclassThugProficienciesGuid = GuidHelper.Create(SubclassNamespace, RogueSubclassThugProficienciesName).ToString();

            private RogueSubclassThugProficienciesBuilder(string name, string guid) : base(name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugProficienciesTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugProficienciesDescription";
                Definition.SetProficiencyType(RuleDefinitions.ProficiencyType.Armor);
                Definition.Proficiencies.Add("MediumArmorCategory");
                Definition.Proficiencies.Add("ShieldCategory");
            }

            private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid)
            {
                return new RogueSubclassThugProficienciesBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionProficiency ThugProficiencies =
                CreateAndAddToDB(RogueSubclassThugProficienciesName, RogueSubclassThugProficienciesGuid);
        }

        private sealed class RogueSubclassThugBrutalMethodsBuilder : FeatureDefinitionActionAffinityBuilder
        {
            private const string RogueSubclassThugBrutalMethodsName = "KSRogueSubclassThugBrutalMethods";
            private static readonly string RogueSubclassThugBrutalMethodsGuid = GuidHelper.Create(SubclassNamespace, RogueSubclassThugBrutalMethodsName).ToString();

            private RogueSubclassThugBrutalMethodsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityThiefFastHands, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugBrutalMethodsTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugBrutalMethodsDescription";
                Definition.AuthorizedActions.Clear();
                Definition.AuthorizedActions.Add(RogueSubclassThugBrutalMethodsActionBuilder.ThugBrutalMethodsAction.Id);
            }

            private static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
            {
                return new RogueSubclassThugBrutalMethodsBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionActionAffinity ThugBrutalMethods =
                CreateAndAddToDB(RogueSubclassThugBrutalMethodsName, RogueSubclassThugBrutalMethodsGuid);
        }

        private sealed class RogueSubclassThugBrutalMethodsActionBuilder : ActionDefinitionBuilder
        {
            private const string RogueSubclassThugBrutalMethodsActionName = "KSRogueSubclassThugBrutalMethodsAction";
            private static readonly string RogueSubclassThugBrutalMethodsActionGuid = GuidHelper.Create(SubclassNamespace, RogueSubclassThugBrutalMethodsActionName).ToString();

            private RogueSubclassThugBrutalMethodsActionBuilder(string name, string guid) : base(DatabaseHelper.ActionDefinitions.ShoveBonus, name, guid)
            {
                Definition.SetId((ActionDefinitions.Id)THUG_BONUS_SHOVE_ACTION_ID);
            }

            private static ActionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new RogueSubclassThugBrutalMethodsActionBuilder(name, guid).AddToDB();
            }

            internal static readonly ActionDefinition ThugBrutalMethodsAction
                = CreateAndAddToDB(RogueSubclassThugBrutalMethodsActionName, RogueSubclassThugBrutalMethodsActionGuid);
        }

        private sealed class RogueSubclassThugOvercomeCompetitionBuilder : FeatureDefinitionAbilityCheckAffinityBuilder
        {
            private const string RogueSubclassThugOvercomeCompetitionName = "KSRogueSubclassThugOvercomeCompetition";
            private static readonly string RogueSubclassThugOvercomeCompetitionGuid = GuidHelper.Create(SubclassNamespace, RogueSubclassThugOvercomeCompetitionName).ToString();

            private RogueSubclassThugOvercomeCompetitionBuilder(string name, string guid) : base(name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugOvercomeCompetitionTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugOvercomeCompetitionDescription";
                Definition.AffinityGroups.Add(new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
                {
                    affinity = RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                    proficiencyName = DatabaseHelper.SkillDefinitions.Athletics.Name,
                    abilityScoreName = DatabaseHelper.SkillDefinitions.Athletics.AbilityScore
                });
            }

            private static FeatureDefinitionAbilityCheckAffinity CreateAndAddToDB(string name, string guid)
            {
                return new RogueSubclassThugOvercomeCompetitionBuilder(name, guid).AddToDB();
            }

            internal static readonly FeatureDefinitionAbilityCheckAffinity ThugOvercomeCompetition =
                CreateAndAddToDB(RogueSubclassThugOvercomeCompetitionName, RogueSubclassThugOvercomeCompetitionGuid);
        }
    }
}
