using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class PowerFighterActionSurge2Builder : FeatureDefinitionPowerBuilder
{
    private const string PowerFighterActionSurgeName = "PowerFighterActionSurge2";
    private const string PowerFighterActionSurgeGuid = "a20a3955a66142e5ba9d2580a71b6c36";

    internal static readonly FeatureDefinitionPower PowerFighterActionSurge2 =
        CreateAndAddToDB(PowerFighterActionSurgeName, PowerFighterActionSurgeGuid);

    private PowerFighterActionSurge2Builder(string name, string guid) : base(PowerFighterActionSurge, name, guid)
    {
        Definition.fixedUsesPerRecharge = 2;
        Definition.overriddenPower = PowerFighterActionSurge;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
    {
        return new PowerFighterActionSurge2Builder(name, guid).AddToDB();
    }
}
