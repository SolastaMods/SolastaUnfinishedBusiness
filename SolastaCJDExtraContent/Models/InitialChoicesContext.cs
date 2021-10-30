using SolastaModApi;

namespace SolastaCJDExtraContent.Models
{
    internal static class InitialChoicesContext
    {
        internal static int previousAllRacesInitialFeats = -1;
        internal static bool previousAlternateHuman = false;

        internal static void Load()
        {
            // keep this outside loop for backward compatibility
            _ = new Models.Features.PointPoolBonusFeatsBuilder("PointPool2BonusFeats", "dbec86c7-468f-4569-917b-2d96d21f9ddf", HeroDefinitions.PointsPoolType.Feat, 2,
                    new GuiPresentationBuilder("Race/&PointPoolSelect2FeatsDescription", "Race/&PointPoolSelect2FeatsTitle").Build()).AddToDB(true);

            // 11 here as need to count the Alternate Human Feat
            for (var i = 3; i <= 11; i++)
            {
                var name = $"PointPool{i}BonusFeats";
                var guid = GuidHelper.Create(new System.Guid(Settings.GUID), name).ToString();

                _ = new Models.Features.PointPoolBonusFeatsBuilder(name, guid, HeroDefinitions.PointsPoolType.Feat, 2,
                        new GuiPresentationBuilder($"Race/&PointPoolSelect{i}FeatsDescription", $"Race/&PointPoolSelect{i}FeatsTitle").Build()).AddToDB(true);
            }

            Models.InitialChoicesContext.RefreshAllRacesInitialFeats();
        }

        internal static void RefreshAllRacesInitialFeats()
        {
            if (previousAllRacesInitialFeats > 0)
            {
                UnloadRacesLevel1Feats(Main.Settings.AllRacesInitialFeats, Main.Settings.AlternateHuman);
            }
            previousAllRacesInitialFeats = Main.Settings.AllRacesInitialFeats;
            previousAlternateHuman = Main.Settings.AlternateHuman;
            LoadRacesLevel1Feats(previousAllRacesInitialFeats, previousAlternateHuman);
        }

        internal static void BuildFeatureUnlocks(int initialFeats, bool alternateHuman, out FeatureUnlockByLevel featureUnlockByLevelNonHuman, out FeatureUnlockByLevel featureUnlockByLevelHuman)
        {
            var featureDefinitionPointPoolDb = DatabaseRepository.GetDatabase<FeatureDefinitionPointPool>();
            string name;

            featureUnlockByLevelNonHuman = null;
            featureUnlockByLevelHuman = null;

            if (initialFeats == 1)
            {
                featureUnlockByLevelNonHuman = new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionPointPools.PointPoolBonusFeat, 1);

                name = "PointPool2BonusFeats";
                if (alternateHuman && featureDefinitionPointPoolDb.TryGetElement(name, out FeatureDefinitionPointPool pointPool2BonusFeats))
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(pointPool2BonusFeats, 1);
                }
            }
            else if (initialFeats > 1)
            {
                name = $"PointPool{initialFeats}BonusFeats";
                if (featureDefinitionPointPoolDb.TryGetElement(name, out FeatureDefinitionPointPool featureDefinitionPointPool))
                {
                    featureUnlockByLevelNonHuman = new FeatureUnlockByLevel(featureDefinitionPointPool, 1);
                }

                name = $"PointPool{initialFeats + 1}BonusFeats";
                if (alternateHuman && featureDefinitionPointPoolDb.TryGetElement(name, out FeatureDefinitionPointPool pointPoolXBonusFeats))
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(pointPoolXBonusFeats, 1);
                }
            }
        }

        internal static void LoadRacesLevel1Feats(int initialFeats, bool alternateHuman)
        {
            var human = DatabaseHelper.CharacterRaceDefinitions.Human;

            BuildFeatureUnlocks(initialFeats, alternateHuman, out FeatureUnlockByLevel featureUnlockByLevelNonHuman, out FeatureUnlockByLevel featureUnlockByLevelHuman);

            if (featureUnlockByLevelNonHuman != null)
            {
                foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements())
                {
                    if (characterRaceDefinition != human)
                    {
                        characterRaceDefinition.FeatureUnlocks.Add(featureUnlockByLevelNonHuman);
                    }
                }
            }

            if (featureUnlockByLevelHuman != null)
            {
                human.FeatureUnlocks.Add(featureUnlockByLevelHuman);

                FeatureUnlockByLevel pointPoolAbilityScoreImprovement = new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionPointPools.PointPoolAbilityScoreImprovement, 1);
                human.FeatureUnlocks.Add(pointPoolAbilityScoreImprovement);

                FeatureUnlockByLevel pointPoolHumanSkillPool = new FeatureUnlockByLevel((FeatureDefinition)DatabaseHelper.FeatureDefinitionPointPools.PointPoolHumanSkillPool, 1);
                human.FeatureUnlocks.Add(pointPoolHumanSkillPool);
            }
        }

        internal static void UnloadRacesLevel1Feats(int initialFeats, bool alternateHuman)
        {
            void Remove(CharacterRaceDefinition characterRaceDefinition, FeatureUnlockByLevel featureUnlockByLevel)
            {
                var ndx = -1;

                for (var i = 0; i < characterRaceDefinition.FeatureUnlocks.Count; i++)
                {
                    if (characterRaceDefinition.FeatureUnlocks[i].Level == 1)
                    {
                        var featureDefinition = characterRaceDefinition.FeatureUnlocks[i].FeatureDefinition;

                        if (featureDefinition == featureUnlockByLevel.FeatureDefinition || 
                            featureDefinition == DatabaseHelper.FeatureDefinitionPointPools.PointPoolHumanSkillPool ||
                            featureDefinition == DatabaseHelper.FeatureDefinitionPointPools.PointPoolAbilityScoreImprovement)
                        {
                            characterRaceDefinition.FeatureUnlocks.RemoveAt(ndx);
                        }
                    }
                }

                if (ndx > 0)
                {
                    characterRaceDefinition.FeatureUnlocks.RemoveAt(ndx);
                }
            }

            var human = DatabaseHelper.CharacterRaceDefinitions.Human;

            BuildFeatureUnlocks(initialFeats, alternateHuman, out FeatureUnlockByLevel featureUnlockByLevelNonHuman, out FeatureUnlockByLevel featureUnlockByLevelHuman);

            if (featureUnlockByLevelNonHuman != null)
            {
                foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>().GetAllElements())
                {
                    if (characterRaceDefinition != human)
                    {
                        Remove(characterRaceDefinition, featureUnlockByLevelNonHuman);
                    }
                }

                if (featureUnlockByLevelHuman != null)
                {
                    Remove(human, featureUnlockByLevelHuman);

                    FeatureUnlockByLevel pointPoolAbilityScoreImprovement = new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionPointPools.PointPoolAbilityScoreImprovement, 1);
                    human.FeatureUnlocks.Add(pointPoolAbilityScoreImprovement);

                    FeatureUnlockByLevel pointPoolHumanSkillPool = new FeatureUnlockByLevel((FeatureDefinition)DatabaseHelper.FeatureDefinitionPointPools.PointPoolHumanSkillPool, 1);
                    human.FeatureUnlocks.Add(pointPoolHumanSkillPool);
                }
            }
        }
    }
}
