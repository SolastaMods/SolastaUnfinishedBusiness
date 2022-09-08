using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20;

internal sealed class PowerClericTurnUndeadBuilder : FeatureDefinitionPowerBuilder
{
    private const string PowerClericTurnUndead17Name = "ZSPowerClericTurnUndead17";
    private const string PowerClericTurnUndead17Guid = "b0ef65ba1e784628b1c5b4af75d4f395";

    internal static readonly FeatureDefinitionPower PowerClericTurnUndead17 =
        CreateAndAddToDB(PowerClericTurnUndead17Name, PowerClericTurnUndead17Guid, 4);

    private PowerClericTurnUndeadBuilder(string name, string guid, int challengeRating) : base(
        PowerClericTurnUndead8, name, guid)
    {
        Definition.EffectDescription.EffectForms[0].KillForm.challengeRating = challengeRating;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid, int challengeRating)
    {
        return new PowerClericTurnUndeadBuilder(name, guid, challengeRating).AddToDB();
    }
}
