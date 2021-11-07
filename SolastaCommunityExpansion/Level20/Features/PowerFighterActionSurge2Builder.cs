//
// Chris - please review this feature
//


//using SolastaModApi;
//using SolastaModApi.Extensions;
//using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

//namespace SolastaCommunityExpansion.Level20.Features
//{
//    internal class PowerFighterActionSurge2Builder : BaseDefinitionBuilder<FeatureDefinitionPower>
//    {
//        const string PowerFighterActionSurgeName = "ZSPowerFighterActionSurge2";
//        const string PowerFighterActionSurgeGuid = "a20a3955a66142e5ba9d2580a71b6c36";

//        protected PowerFighterActionSurge2Builder(string name, string guid, int challengeRating) : base(PowerFighterActionSurge, name, guid)
//        {
//            //Definition.SetFixedUsesPerRecharge(2);
//        }

//        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid, int challengeRating)
//            => new PowerFighterActionSurge2Builder(name, guid, challengeRating).AddToDB();

//        internal static readonly FeatureDefinitionPower PowerFighterActionSurge2 =
//            CreateAndAddToDB(PowerFighterActionSurgeName, PowerFighterActionSurgeGuid, 3);
//    }
//}