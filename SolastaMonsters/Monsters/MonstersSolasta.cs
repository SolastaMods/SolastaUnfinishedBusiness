using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************
namespace SolastaMonsters.Monsters
{
    internal static class MonstersSolasta
    {
        public static void EnableInDungeonMaker()
        {
            List<MonsterDefinition> listofExistingMonsters = new List<MonsterDefinition>
            {
                DatabaseHelper.MonsterDefinitions.Emperor_Laethar,					// CR 10
				DatabaseHelper.MonsterDefinitions.BlackDragon_MasterOfNecromancy,	// CR 14
				DatabaseHelper.MonsterDefinitions.SilverDragon_Princess,			// CR 15
				DatabaseHelper.MonsterDefinitions.SpectralDragon_Magister,			// CR 15
				DatabaseHelper.MonsterDefinitions.Divine_Avatar,					// CR 16
				DatabaseHelper.MonsterDefinitions.Divine_Avatar_Cleric,				// CR 16
				DatabaseHelper.MonsterDefinitions.Divine_Avatar_Wizard,				// CR 16
				DatabaseHelper.MonsterDefinitions.GoldDragon_AerElai,				// CR 17

				// unfinished monsters, wait until DLC 1
				//DatabaseHelper.MonsterDefinitions.MummyLord,						// CR 15
				//DatabaseHelper.MonsterDefinitions.Golem_Stone,					// CR 10
				//DatabaseHelper.MonsterDefinitions.Golem_Iron						// CR 16
			};

            if (SolastaCommunityExpansion.Main.Settings.EnableExtraHighLevelMonsters)
            {

                foreach (MonsterDefinition monster in listofExistingMonsters)
                {
                    if (monster.DungeonMakerPresence == MonsterDefinition.DungeonMaker.None)
                    {
                        monster.SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.Monster);
                    }
                    if (monster == DatabaseHelper.MonsterDefinitions.SilverDragon_Princess)
                    {
                        // silver dragon is half finished so it needs to reuse other dragon attributes,
                        // TA uses green dragon attacks for silver dragon so the trend is continued here
                        monster.SetGroupAttacks(true);
                        monster.SetLegendaryCreature(true);
                        monster.AttackIterations.Clear();
                        monster.AttackIterations.AddRange(DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration.AttackIterations);
                        monster.Features.Clear();
                        monster.Features.AddRange(DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration.Features);
                        monster.LegendaryActionOptions.AddRange(DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration.LegendaryActionOptions);
                        monster.SetDefaultBattleDecisionPackage(DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration.DefaultBattleDecisionPackage);
                        monster.SetThreatEvaluatorDefinition(DatabaseHelper.MonsterDefinitions.GreenDragon_MasterOfConjuration.ThreatEvaluatorDefinition);
                        // guipresentation title is mislabeled as a green dragon
                        monster.GuiPresentation.SetTitle(monster.Name);
                    }
                };
            }
        }
    }
}
