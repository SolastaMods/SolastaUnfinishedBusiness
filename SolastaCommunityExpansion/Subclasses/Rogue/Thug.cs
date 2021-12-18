using SolastaModApi;
using SolastaModApi.Extensions;
using System;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaCommunityExpansion.Subclasses.Rogue
{
    internal class Thug : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new Guid("b6f6a95faee44d0b87fc2fa65a785e1b");

        private readonly CharacterSubclassDefinition Subclass;

        private const string RogueSubclassThugName = "KSRogueSubclassThug";

        private static readonly string RogueSubclassThugNameGuid = GuidHelper.Create(SubclassNamespace, RogueSubclassThugName).ToString();

        internal static int THUG_BONUS_SHOVE_ACTION_ID = 44800;

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
            var guiPresentation = new GuiPresentationBuilder(
                "Subclass/&KSRogueSubclassThugDescription", 
                "Subclass/&KSRogueSubclassThugTitle")
                .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.GuiPresentation.SpriteReference)
                .Build();

            Subclass = new CharacterSubclassDefinitionBuilder(RogueSubclassThugName, RogueSubclassThugNameGuid)
                .SetGuiPresentation(guiPresentation)
                .AddFeatureAtLevel(NegativeFeatureBuilder.AdditionalDamageRogueSneakAttackRemove, 3)
                .AddFeatureAtLevel(RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder.ExploitVulnerabilities, 3)
                .AddFeatureAtLevel(RogueSubclassThugProficienciesBuilder.ThugProficiencies, 3)
                .AddFeatureAtLevel(RogueSubclassThugBrutalMethodsBuilder.ThugBrutalMethods, 9)
                .AddFeatureAtLevel(RogueSubclassThugOvercomeCompetitionBuilder.ThugOvercomeCompetition, 13)
                .AddToDB();
        }

        internal class NegativeFeatureDefinition : FeatureDefinition
        {
            public FeatureDefinition FeatureToRemove;
        }

        private class NegativeFeatureBuilder : BaseDefinitionBuilder<NegativeFeatureDefinition>
        {
            protected NegativeFeatureBuilder(FeatureDefinition featureToRemove) 
                : base(featureToRemove.Name + "Remove", GuidHelper.Create(Thug.SubclassNamespace, featureToRemove.Name + "Remove").ToString())
            {
                Definition.FeatureToRemove = featureToRemove;
                Definition.GuiPresentation.SetHidden(true);
            }

            private static NegativeFeatureDefinition CreateAndAddToDB(FeatureDefinition featureToRemove)
                => new NegativeFeatureBuilder(featureToRemove).AddToDB();

            internal static readonly NegativeFeatureDefinition AdditionalDamageRogueSneakAttackRemove =
                CreateAndAddToDB(AdditionalDamageRogueSneakAttack);
        }

        private class RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
        {
            private const string ExploitVulnerabilitiesName = "KSRogueSubclassThugExploitVulnerabilities";
            private static readonly string ExploitVulnerabilitiesGuid = GuidHelper.Create(Thug.SubclassNamespace, ExploitVulnerabilitiesName).ToString();

            protected RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder(string name, string guid) : base(AdditionalDamageRogueSneakAttack, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugExploitVulnerabilitiesSneakAttackTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugExploitVulnerabilitiesSneakAttackDescription";
                FeatureDefinitionAdditionalDamageExtensions.SetRequiredProperty(Definition, RuleDefinitions.AdditionalDamageRequiredProperty.None);
            }

            private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
                => new RogueSubclassThugExploitVulnerabilitiesSneakAttackBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionAdditionalDamage ExploitVulnerabilities = 
                CreateAndAddToDB(ExploitVulnerabilitiesName, ExploitVulnerabilitiesGuid);
        }

        private class RogueSubclassThugProficienciesBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
        {
            private const string RogueSubclassThugProficienciesName = "KSRogueSubclassThugProficiencies";
            private static readonly string RogueSubclassThugProficienciesGuid = GuidHelper.Create(Thug.SubclassNamespace, RogueSubclassThugProficienciesName).ToString();

            protected RogueSubclassThugProficienciesBuilder(string name, string guid) : base(name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugProficienciesTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugProficienciesDescription";
                FeatureDefinitionProficiencyExtensions.SetProficiencyType<FeatureDefinitionProficiency>(Definition, RuleDefinitions.ProficiencyType.Armor);
                Definition.Proficiencies.Add("MediumArmorCategory");
                Definition.Proficiencies.Add("ShieldCategory");
            }

            private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid)
                => new RogueSubclassThugProficienciesBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionProficiency ThugProficiencies = 
                CreateAndAddToDB(RogueSubclassThugProficienciesName, RogueSubclassThugProficienciesGuid);
        }

        private class RogueSubclassThugBrutalMethodsBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
        {
            private const string RogueSubclassThugBrutalMethodsName = "KSRogueSubclassThugBrutalMethods";
            private static readonly string RogueSubclassThugBrutalMethodsGuid = GuidHelper.Create(Thug.SubclassNamespace, RogueSubclassThugBrutalMethodsName).ToString();

            protected RogueSubclassThugBrutalMethodsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityThiefFastHands, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&KSRogueSubclassThugBrutalMethodsTitle";
                Definition.GuiPresentation.Description = "Feature/&KSRogueSubclassThugBrutalMethodsDescription";
                Definition.AuthorizedActions.Clear();
                Definition.AuthorizedActions.Add(RogueSubclassThugBrutalMethodsActionBuilder.ThugBrutalMethodsAction.Id);
            }

            private static FeatureDefinitionActionAffinity CreateAndAddToDB(string name, string guid)
                => new RogueSubclassThugBrutalMethodsBuilder(name, guid).AddToDB();

            internal static readonly FeatureDefinitionActionAffinity ThugBrutalMethods = 
                CreateAndAddToDB(RogueSubclassThugBrutalMethodsName, RogueSubclassThugBrutalMethodsGuid);
        }

        private class RogueSubclassThugBrutalMethodsActionBuilder : BaseDefinitionBuilder<ActionDefinition>
        {
            private const string RogueSubclassThugBrutalMethodsActionName = "KSRogueSubclassThugBrutalMethodsAction";
            private static readonly string RogueSubclassThugBrutalMethodsActionGuid = GuidHelper.Create(Thug.SubclassNamespace, RogueSubclassThugBrutalMethodsActionName).ToString();

            protected RogueSubclassThugBrutalMethodsActionBuilder(string name, string guid) : base(DatabaseHelper.ActionDefinitions.ShoveBonus, name, guid)
            {
                ActionDefinitionExtensions.SetId(Definition, (ActionDefinitions.Id)THUG_BONUS_SHOVE_ACTION_ID);
            }

            private static ActionDefinition CreateAndAddToDB(string name, string guid) 
                => new RogueSubclassThugBrutalMethodsActionBuilder(name, guid).AddToDB();

            public static ActionDefinition ThugBrutalMethodsAction 
                = CreateAndAddToDB(RogueSubclassThugBrutalMethodsActionName, RogueSubclassThugBrutalMethodsActionGuid);
        }

        private class RogueSubclassThugOvercomeCompetitionBuilder : BaseDefinitionBuilder<FeatureDefinitionAbilityCheckAffinity>
        {
            private const string RogueSubclassThugOvercomeCompetitionName = "KSRogueSubclassThugOvercomeCompetition";
            private static readonly string RogueSubclassThugOvercomeCompetitionGuid = GuidHelper.Create(Thug.SubclassNamespace, RogueSubclassThugOvercomeCompetitionName).ToString();

            protected RogueSubclassThugOvercomeCompetitionBuilder(string name, string guid) : base(name, guid)
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
                => new RogueSubclassThugOvercomeCompetitionBuilder(name, guid).AddToDB();

            internal static FeatureDefinitionAbilityCheckAffinity ThugOvercomeCompetition = 
                CreateAndAddToDB(RogueSubclassThugOvercomeCompetitionName, RogueSubclassThugOvercomeCompetitionGuid);
        }
    }
}
