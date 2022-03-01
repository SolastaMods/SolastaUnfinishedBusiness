using System;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class WarlockEldritchMasterBuilder : FeatureDefinitionPowerBuilder
    {
        private const string WarlockEldritchMasterName = "ClassWarlockEldritchMaster";
        private static readonly string WarlockEldritchMasterGuid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchMasterName).ToString();

        protected WarlockEldritchMasterBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery, name, guid)
        {
            Definition.GuiPresentation.Title =       "Feature/&ClassWarlockEldritchMasterTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchMasterDescription";
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Minute1);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchMasterBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchMaster = CreateAndAddToDB(WarlockEldritchMasterName, WarlockEldritchMasterGuid);
    }

}
