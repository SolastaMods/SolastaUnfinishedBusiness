using SolastaContentExpansion.Features;
using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolastaContentExpansion.Feats
{
    class FightingStlyeFeats
    {
        public static Guid FightingStyleFeatsNamespace = new Guid("db157827-0f8a-4fbb-bb87-6d54689a587a");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Two Weapon
            GuiPresentationBuilder twoWeaponPresentation = new GuiPresentationBuilder(
             "Feat/&FightingStyleTwoWeaponDescription",
             "Feat/&FightingStyleTwoWeaponTitle");

            FeatDefinitionBuilder twoWeapon = new FeatDefinitionBuilder("FeatFightingStyleTwoWeapon", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleTwoWeapon").ToString(),
                new List<FeatureDefinition>()
            {
                new FeatureDefinitionProficiencyBuilder("FeatFightingStyleTwoWeaponProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleTwoWeaponProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"TwoWeapon"}, twoWeaponPresentation.Build()).AddToDB(),
            }, twoWeaponPresentation.Build());
            feats.Add(twoWeapon.AddToDB());

            // Protection
            GuiPresentationBuilder protectionPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleProtectionDescription",
                "Feat/&FightingStyleProtectionTitle");

            FeatDefinitionBuilder protection = new FeatDefinitionBuilder("FeatFightingStyleProtection", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleProtection").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleProtectionProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleProtectionProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Protection"}, twoWeaponPresentation.Build()).AddToDB(),
            }, protectionPresentation.Build());
            feats.Add(protection.AddToDB());

            // GreatWeapon
            GuiPresentationBuilder greatWeaponPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleGreatWeaponDescription",
                "Feat/&FightingStyleGreatWeaponTitle");

            FeatDefinitionBuilder greatWeapon = new FeatDefinitionBuilder("FeatFightingStyleGreatWeapon", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleGreatWeapon").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleGreatWeaponProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleGreatWeaponProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"GreatWeapon"}, twoWeaponPresentation.Build()).AddToDB(),
            }, greatWeaponPresentation.Build());
            feats.Add(greatWeapon.AddToDB());

            // Dueling
            GuiPresentationBuilder duelingPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleDuelingDescription",
                "Feat/&FightingStyleDuelingTitle");

            FeatDefinitionBuilder dueling = new FeatDefinitionBuilder("FeatFightingStyleDueling", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDueling").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleDuelingProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDuelingProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Dueling"}, twoWeaponPresentation.Build()).AddToDB(),
            }, duelingPresentation.Build());
            feats.Add(dueling.AddToDB());

            // Defense
            GuiPresentationBuilder defensePresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleDefenseDescription",
                "Feat/&FightingStyleDefenseTitle");

            FeatDefinitionBuilder defense = new FeatDefinitionBuilder("FeatFightingStyleDefense", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDefense").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleDefenseProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleDefenseProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Defense"}, twoWeaponPresentation.Build()).AddToDB(),
            }, defensePresentation.Build());
            feats.Add(defense.AddToDB());

            // Archery
            GuiPresentationBuilder archeryPresentation = new GuiPresentationBuilder(
                "Feat/&FightingStyleArcheryDescription",
                "Feat/&FightingStyleArcheryTitle");

            FeatDefinitionBuilder archery = new FeatDefinitionBuilder("FeatFightingStyleArchery", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleArchery").ToString(),
                new List<FeatureDefinition>()
            {
                    new FeatureDefinitionProficiencyBuilder("FeatFightingStyleArcheryProficiency", GuidHelper.Create(FightingStyleFeatsNamespace, "FeatFightingStyleArcheryProficiency").ToString(),
                    RuleDefinitions.ProficiencyType.FightingStyle, new List<string>(){"Archery"}, twoWeaponPresentation.Build()).AddToDB(),
            }, archeryPresentation.Build());
            feats.Add(archery.AddToDB());
        }
    }
}
