using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class ConjurationsContext
    {
        internal static void Load()
        {
            // NOTE: assumes monsters have FullyControlledWhenAllied=false by default

            var controlled = Main.Settings.FullyControlAlliedConjurations;

            // Conjure animals (3)
            ConjuredOneBeastTiger_Drake.SetFullyControlledWhenAllied(controlled);
            ConjuredTwoBeast_Direwolf.SetFullyControlledWhenAllied(controlled);
            ConjuredFourBeast_BadlandsSpider.SetFullyControlledWhenAllied(controlled);
            ConjuredEightBeast_Wolf.SetFullyControlledWhenAllied(controlled);

            // Conjure minor elementals (4)
            SkarnGhoul.SetFullyControlledWhenAllied(controlled); // CR 2
            WindSnake.SetFullyControlledWhenAllied(controlled); // CR 2
            Fire_Jester.SetFullyControlledWhenAllied(controlled); // CR 1

            // Conjure woodland beings (4) - not implemented

            // Conjure elemental (5)
            Air_Elemental.SetFullyControlledWhenAllied(controlled); // CR 5
            Fire_Elemental.SetFullyControlledWhenAllied(controlled); // CR 5
            Earth_Elemental.SetFullyControlledWhenAllied(controlled); // CR 5

            // Conjure fey (6) - should all be CR 6 - not implemented in base game yet
            //FeyBear.SetFullyControlledWhenAllied(controlled); // CR 4
            //FeyGiantApe.SetFullyControlledWhenAllied(controlled); // CR 6
            //FeyGiant_Eagle.SetFullyControlledWhenAllied(controlled); // CR 5
            //FeyWolf.SetFullyControlledWhenAllied(controlled); // CR 2
            //Dryad.SetFullyControlledWhenAllied(controlled); // CR 1
            //Green_Hag.SetFullyControlledWhenAllied(controlled); // CR 3

            // Conjure celestial (7)
        }
    }
}
