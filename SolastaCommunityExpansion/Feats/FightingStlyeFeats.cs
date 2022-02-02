using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.FightingStyles;
using SolastaCommunityExpansion.Models;
using SolastaModApi;

namespace SolastaCommunityExpansion.Feats
{
    internal static class FightingStlyeFeats
    {
        public static readonly Guid FightingStyleFeatsNamespace = new("db157827-0f8a-4fbb-bb87-6d54689a587a");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Two Weapon
            GuiPresentationBuilder twoWeaponPresentation = new GuiPresentationBuilder(
             "Feat/&FightingStyleTwoWeaponTitle",
             "Feat/&FightingStyleTwoWeaponDescription");

            FeatDefinitionBuilder twoWeapon = new FeatDefinitionBuilder("FeatFightingStyleTwoWeapon", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleTwoWeapon").ToString(),
                new List<FeatureDefinition>()
            {
                new FeatureDefinitionProficiencyBuilder("FeatFightingStyleTwoWeaponProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleTwoWeaponProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"TwoWeapon"}, twoWeaponPresentation.Build()).AddToDB(),
            }, twoWeaponPresentation.Build());
            feats.Add(twoWeapon.AddToDB());

            // Protection
            GuiPresentationBuilder protectionPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleProtectionTitle",
                "Feat/&FightingStyleProtectionDescription");

            FeatDefinitionBuilder protection = new FeatDefinitionBuilder("FeatFightingStyleProtection", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleProtection").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleProtectionProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleProtectionProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Protection"}, protectionPresentation.Build()).AddToDB(),
            }, protectionPresentation.Build());
            feats.Add(protection.AddToDB());

            // GreatWeapon
            GuiPresentationBuilder greatWeaponPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleGreatWeaponTitle",
                "Feat/&FightingStyleGreatWeaponDescription");

            FeatDefinitionBuilder greatWeapon = new FeatDefinitionBuilder("FeatFightingStyleGreatWeapon", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleGreatWeapon").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleGreatWeaponProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleGreatWeaponProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"GreatWeapon"}, greatWeaponPresentation.Build()).AddToDB(),
            }, greatWeaponPresentation.Build());
            feats.Add(greatWeapon.AddToDB());

            // Dueling
            GuiPresentationBuilder duelingPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleDuelingTitle",
                "Feat/&FightingStyleDuelingDescription");

            FeatDefinitionBuilder dueling = new FeatDefinitionBuilder("FeatFightingStyleDueling", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDueling").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleDuelingProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDuelingProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Dueling"}, duelingPresentation.Build()).AddToDB(),
            }, duelingPresentation.Build());
            feats.Add(dueling.AddToDB());

            // Defense
            GuiPresentationBuilder defensePresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleDefenseTitle",
                "Feat/&FightingStyleDefenseDescription");

            FeatDefinitionBuilder defense = new FeatDefinitionBuilder("FeatFightingStyleDefense", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDefense").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleDefenseProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDefenseProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Defense"}, defensePresentation.Build()).AddToDB(),
            }, defensePresentation.Build());
            feats.Add(defense.AddToDB());

            // Archery
            GuiPresentationBuilder archeryPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleArcheryTitle",
                "Feat/&FightingStyleArcheryDescription");

            FeatDefinitionBuilder archery = new FeatDefinitionBuilder("FeatFightingStyleArchery", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleArchery").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleArcheryProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleArcheryProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Archery"}, archeryPresentation.Build()).AddToDB(),
            }, archeryPresentation.Build());
            feats.Add(archery.AddToDB());

            foreach (AbstractFightingStyle fightingStyle in FightingStyleContext.Styles.Values)
            {
                feats.Add(BuildFightingStyleFeat(fightingStyle.GetStyle()));
            }
        }

        private static FeatDefinition BuildFightingStyleFeat(FightingStyleDefinition fightingStyle)
        {
            string name = "Feat" + fightingStyle.Name;

            FeatDefinitionBuilder feat = new FeatDefinitionBuilder(name, GuidHelper.Create(FightingStyleFeatsNamespace, name).ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder(name+"Proficiency", GuidHelper.Create(FightingStyleFeatsNamespace, name+"Proficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){fightingStyle.Name}, fightingStyle.GuiPresentation).AddToDB(),
            }, fightingStyle.GuiPresentation);

            return feat.AddToDB();
        }
    }
}
