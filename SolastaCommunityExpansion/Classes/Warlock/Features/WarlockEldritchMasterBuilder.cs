using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal class WarlockEldritchMasterBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchMasterName = "ClassWarlockEldritchMaster";
        private static readonly string WarlockEldritchMasterGuid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchMasterName).ToString();

        [Obsolete]
        protected WarlockEldritchMasterBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery, name, guid)
        {
            Definition.GuiPresentation.Title =       "Feature/&ClassWarlockEldritchMasterTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchMasterDescription";
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Minute1);
        }

        [Obsolete]
        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchMasterBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        public static FeatureDefinitionPower WarlockEldritchMaster = CreateAndAddToDB(WarlockEldritchMasterName, WarlockEldritchMasterGuid);
    }

}
