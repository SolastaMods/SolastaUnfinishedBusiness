using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class PowerFighterActionSurge2Builder : FeatureDefinitionPowerBuilder
    {
        private const string PowerFighterActionSurgeName = "ZSPowerFighterActionSurge2";
        private const string PowerFighterActionSurgeGuid = "a20a3955a66142e5ba9d2580a71b6c36";

        private PowerFighterActionSurge2Builder(string name, string guid) : base(PowerFighterActionSurge, name, guid)
        {
            Definition.SetFixedUsesPerRecharge(2);
            Definition.SetOverriddenPower(PowerFighterActionSurge);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new PowerFighterActionSurge2Builder(name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionPower PowerFighterActionSurge2 =
            CreateAndAddToDB(PowerFighterActionSurgeName, PowerFighterActionSurgeGuid);
    }
}
