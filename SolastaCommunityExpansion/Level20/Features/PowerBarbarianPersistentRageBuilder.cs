using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Level20.Features.ConditionPersistentRageBuilder;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class PowerBarbarianPersistentRageBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string PowerBarbarianPersistentRageStartName = "ZSPowerBarbarianPersistentRageStart";
        private const string PowerBarbarianPersistentRageStartGuid = "feea8426e38f49799628b3ceb0abe877";

        protected PowerBarbarianPersistentRageBuilder(string name, string guid) : base(PowerBarbarianRageStart, name, guid)
        {
            Definition.SetGuiPresentation(new GuiPresentationBuilder("Power/&ZSPowerBarbarianPersistentRageStartDescription", "Power/&ZSPowerBarbarianPersistentRageStartTitle").Build());
            Definition.SetOverriddenPower(DatabaseHelper.FeatureDefinitionPowers.PowerBarbarianRageStart);
            Definition.EffectDescription.EffectForms[0].ConditionForm.SetConditionDefinition(ConditionPersistentRage);

            //
            // NOTE: please review if ok to do this change here. need to ensure that the stop power also check for persistent rage condition
            //

            PowerBarbarianRageStop.EffectDescription.EffectForms.Add(new EffectForm()
            {
                ConditionForm = new ConditionForm()
                {
                    ConditionDefinition = ConditionPersistentRage
                }
            });
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new PowerBarbarianPersistentRageBuilder(name, guid).AddToDB();

        internal static readonly FeatureDefinitionPower PowerBarbarianPersistentRageStart =
            CreateAndAddToDB(PowerBarbarianPersistentRageStartName, PowerBarbarianPersistentRageStartGuid);
    }
}
