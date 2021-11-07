using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class PowerClericTurnUndeadBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string PowerClericTurnUndead14Name = "ZSPowerClericTurnUndead14";
        const string PowerClericTurnUndead14Guid = "1258a27f594542e1b9df6f9d36a50fbe";

        const string PowerClericTurnUndead17Name = "ZSPowerClericTurnUndead17";
        const string PowerClericTurnUndead17Guid = "b0ef65ba1e784628b1c5b4af75d4f395";

        protected PowerClericTurnUndeadBuilder(string name, string guid, int challengeRating) : base(PowerClericTurnUndead8, name, guid)
        {
            Definition.EffectDescription.EffectForms[0].KillForm.SetChallengeRating(challengeRating);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid, int challengeRating)
            => new PowerClericTurnUndeadBuilder(name, guid, challengeRating).AddToDB();

        internal static readonly FeatureDefinitionPower PowerClericTurnUndead14 =
            CreateAndAddToDB(PowerClericTurnUndead14Name, PowerClericTurnUndead14Guid, 3);

        internal static readonly FeatureDefinitionPower PowerClericTurnUndead17 =
            CreateAndAddToDB(PowerClericTurnUndead17Name, PowerClericTurnUndead17Guid, 4);
    }
}