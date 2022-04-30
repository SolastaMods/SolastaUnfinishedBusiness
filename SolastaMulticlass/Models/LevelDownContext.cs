using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaCommunityExpansion.Models.RespecContext;

namespace SolastaMulticlass.Models
{
    internal static class LevelDownContext
    {
        public class FunctorLevelDown : Functor
        {
            public override IEnumerator Execute(
                FunctorParametersDescription functorParameters,
                FunctorExecutionContext context)
            {
                if (SolastaCommunityExpansion.Models.Global.IsMultiplayer)
                {
                    yield break;
                }

                var state = -1;

                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Attention2,
                    "MainMenu/&ExportPdfTitle", "Message/&LevelDownConfirmationDescription",
                    "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                    new MessageModal.MessageValidatedHandler(() => state = 1),
                    new MessageModal.MessageCancelledHandler(() => state = 0));

                while (state < 0)
                {
                    yield return null;
                }

                if (state > 0)
                {
                    if (functorParameters.RestingHero.ClassesHistory.Count > 1)
                    {
                        LevelDown(functorParameters.RestingHero);
                    }
                    else
                    {
                        yield return new FunctorRespec().Execute(functorParameters, context);
                    }
                }
            }
        }

        internal static void Load()
        {
            ServiceRepository.GetService<IFunctorService>().RegisterFunctor("LevelDown", new FunctorLevelDown());
        }

        internal static void ConfirmAndExecute(string filename)
        {
            var service = ServiceRepository.GetService<ICharacterPoolService>();

            service.LoadCharacter(filename, out var rulesetCharacterHero, out _);

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Attention2,
                "MainMenu/&ExportPdfTitle", "Message/&LevelDownConfirmationDescription",
                "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                () => LevelDown(rulesetCharacterHero), null);
        }

        internal static void LevelDown(RulesetCharacterHero hero)
        {
            // collects relevant stats for indexLevel down and sets the context
            var heroLevel = hero.ClassesHistory.Count;
            var indexLevel = heroLevel - 1;
            var characterClassDefinition = hero.ClassesHistory.Last();
            var classLevel = hero.ClassesAndLevels[characterClassDefinition];
            var classTag = AttributeDefinitions.GetClassTag(characterClassDefinition, classLevel);
            var subclassTag = string.Empty;

            hero.ClassesAndSubclasses.TryGetValue(characterClassDefinition, out var characterSubclassDefinition);

            if (characterSubclassDefinition != null)
            {
                subclassTag = AttributeDefinitions.GetSubclassTag(characterClassDefinition, classLevel, characterSubclassDefinition);
            }

            // required as CacheSpellContext used here relies on LevelUpContext
            hero.GetOrCreateHeroBuildingData();
            LevelUpContext.RegisterHero(hero);
            LevelUpContext.SetSelectedClass(hero, characterClassDefinition);
            LevelUpContext.SetSelectedSubclass(hero, characterSubclassDefinition);

            if (subclassTag != string.Empty)
            {
                RemoveFeatures(hero, characterClassDefinition, subclassTag);
                hero.ActiveFeatures.Remove(subclassTag);
                hero.ClearFeatureModifiers(subclassTag);
            }

            RemoveFeatures(hero, characterClassDefinition, classTag);
            hero.ActiveFeatures.Remove(classTag);
            hero.ClearFeatureModifiers(classTag);

            UnlearnSpells(hero, indexLevel);

            hero.RemoveClassLevel();
            hero.RefreshActiveFightingStyles();
            hero.RefreshActiveItemFeatures();
            hero.RefreshArmorClass();
            hero.RefreshAttackModes();
            hero.RefreshAttributeModifiersFromConditions();
            hero.RefreshAttributeModifiersFromFeats();
            hero.RefreshAttributes();
            hero.RefreshClimbRules();
            hero.RefreshConditionFlags();
            hero.RefreshEncumberance();
            hero.RefreshJumpRules();
            hero.RefreshMoveModes();
            hero.RefreshPersonalityFlags();
            hero.RefreshPowers();
            hero.RefreshProficiencies();
            hero.RefreshSpellRepertoires();
            hero.RefreshTags();
            hero.RefreshUsableDeviceFunctions();
            hero.ComputeHitPoints(true);

            // remove building context
            hero.CleanHeroBuildingData();
            LevelUpContext.UnregisterHero(hero);

            // saves hero if not in game
            if (Gui.Game == null)
            {
                ServiceRepository.GetService<ICharacterPoolService>().SaveCharacter(hero, true);
            }
        }

        private static void RemoveFeatureDefinitionPointPool(RulesetCharacterHero hero, RulesetSpellRepertoire heroRepertoire, FeatureDefinitionPointPool featureDefinitionPointPool)
        {
            var poolAmount = featureDefinitionPointPool.PoolAmount;

            switch (featureDefinitionPointPool.PoolType)
            {
                case HeroDefinitions.PointsPoolType.Cantrip:
                    heroRepertoire?.KnownCantrips.RemoveRange(heroRepertoire.KnownCantrips.Count - poolAmount, poolAmount);
                    break;

                case HeroDefinitions.PointsPoolType.Spell:
                    heroRepertoire?.KnownSpells.RemoveRange(heroRepertoire.KnownSpells.Count - poolAmount, poolAmount);
                    break;

                case HeroDefinitions.PointsPoolType.Expertise:
                    hero.TrainedExpertises.RemoveRange(hero.TrainedExpertises.Count - poolAmount, poolAmount);
                    break;

                case HeroDefinitions.PointsPoolType.Feat:
                    while (poolAmount-- > 0)
                    {
                        var featureDefinition = hero.TrainedFeats.Last();
                        //var featureDefinitionName = featureDefinition.Name;

                        hero.TrainedFeats.Remove(featureDefinition);

                        //// handles magic "inititate" (there is a typo indeed) feat special case
                        //if (featureDefinitionName.EndsWith("MagicInititateFeat") && heroRepertoire != null)
                        //{
                        //    var castSpellFeature = heroRepertoire.SpellCastingFeature;

                        //    foreach (var magicInititateFeature in featureDefinition.Features)
                        //    {
                        //        var cantripsToRemove = AccessTools.Field(magicInititateFeature.GetType(), "max_cantrips").GetValue(magicInititateFeature) as int?;
                        //        var spellsToRemove = AccessTools.Field(magicInititateFeature.GetType(), "max_spells").GetValue(magicInititateFeature) as int?;

                        //        while (cantripsToRemove-- > 0)
                        //        {
                        //            heroRepertoire.KnownCantrips.RemoveAt(heroRepertoire.KnownCantrips.Count - 1);
                        //        }

                        //        switch (castSpellFeature.SpellKnowledge)
                        //        {
                        //            case RuleDefinitions.SpellKnowledge.Spellbook:
                        //                var rulesetItemSpellbooks = new List<RulesetItemSpellbook>();

                        //                hero.CharacterInventory.BrowseAllCarriedItems<RulesetItemSpellbook>(rulesetItemSpellbooks);

                        //                if (rulesetItemSpellbooks.Count > 0)
                        //                {
                        //                    var rulesetItemSpellbook = rulesetItemSpellbooks[0];

                        //                    while (spellsToRemove-- > 0)
                        //                    {
                        //                        rulesetItemSpellbook.ScribedSpells.RemoveAt(rulesetItemSpellbook.ScribedSpells.Count - 1);
                        //                    }
                        //                }

                        //                break;

                        //            default:
                        //                while (spellsToRemove-- > 0)
                        //                {
                        //                    heroRepertoire.KnownSpells.RemoveAt(heroRepertoire.KnownSpells.Count - 1);
                        //                }

                        //                break;
                        //        }
                        //    }
                        //}
                    }

                    break;

                case HeroDefinitions.PointsPoolType.Language:
                    hero.TrainedLanguages.RemoveRange(hero.TrainedLanguages.Count - poolAmount, poolAmount);
                    break;

                case HeroDefinitions.PointsPoolType.Skill:
                    hero.TrainedSkills.RemoveRange(hero.TrainedSkills.Count - poolAmount, poolAmount);
                    break;

                case HeroDefinitions.PointsPoolType.Metamagic:
                    hero.TrainedMetamagicOptions.RemoveRange(hero.TrainedMetamagicOptions.Count - poolAmount, poolAmount);
                    break;

                case HeroDefinitions.PointsPoolType.Tool:
                    hero.TrainedToolTypes.RemoveRange(hero.TrainedToolTypes.Count - poolAmount, poolAmount);
                    break;
            }
        }

        private static void UnlearnSpells(RulesetCharacterHero hero, int indexLevel)
        {
            var heroRepertoire = hero.SpellRepertoires.FirstOrDefault(x => LevelUpContext.IsRepertoireFromSelectedClassSubclass(hero, x));

            if (heroRepertoire == null)
            {
                return;
            }

            var cantripsToRemove = heroRepertoire.SpellCastingFeature.KnownCantrips[indexLevel] - heroRepertoire.SpellCastingFeature.KnownCantrips[indexLevel - 1];

            heroRepertoire.PreparedSpells.Clear();

            while (cantripsToRemove-- > 0)
            {
                heroRepertoire.KnownCantrips.RemoveAt(heroRepertoire.KnownCantrips.Count - 1);
            }

            switch (heroRepertoire.SpellCastingFeature.SpellKnowledge)
            {
                case RuleDefinitions.SpellKnowledge.Spellbook:
                    var scribbedSpellsToRemove = heroRepertoire.SpellCastingFeature.ScribedSpells[indexLevel];
                    var rulesetItemSpellbooks = new List<RulesetItemSpellbook>();

                    hero.CharacterInventory.BrowseAllCarriedItems(rulesetItemSpellbooks);

                    if (rulesetItemSpellbooks.Count > 0)
                    {
                        var rulesetItemSpellbook = rulesetItemSpellbooks[0];

                        while (scribbedSpellsToRemove-- > 0)
                        {
                            rulesetItemSpellbook.ScribedSpells.RemoveAt(rulesetItemSpellbook.ScribedSpells.Count - 1);
                        }
                    }

                    break;

                case RuleDefinitions.SpellKnowledge.Selection:
                    var spellsToRemove = heroRepertoire.SpellCastingFeature.KnownSpells[indexLevel] - heroRepertoire.SpellCastingFeature.KnownSpells[indexLevel - 1];

                    while (spellsToRemove-- > 0)
                    {
                        heroRepertoire.KnownSpells.RemoveAt(heroRepertoire.KnownSpells.Count - 1);
                    }

                    break;
            }
        }

        private static void RemoveFeatures(RulesetCharacterHero hero, CharacterClassDefinition characterClassDefinition, string tag)
        {
            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            var classLevel = hero.ClassesAndLevels[characterClassDefinition];
            var heroRepertoire = hero.SpellRepertoires.FirstOrDefault(x => LevelUpContext.IsRepertoireFromSelectedClassSubclass(hero, x));

            foreach (var featureDefinition in hero.ActiveFeatures[tag])
            {
                var featureDefinitionTypeName = featureDefinition.GetType().Name;

                if (featureDefinition is FeatureDefinitionCastSpell && heroRepertoire != null)
                {
                    hero.SpellRepertoires.Remove(heroRepertoire);
                }
                else if (featureDefinition is FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells && heroRepertoire != null)
                {
                    var spellsToRemove = featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups.FirstOrDefault(x => x.ClassLevel == classLevel)?.SpellsList.Count ?? 0;

                    while (spellsToRemove-- > 0)
                    {
                        heroRepertoire.AutoPreparedSpells.RemoveAt(heroRepertoire.AutoPreparedSpells.Count - 1);
                    }
                }
                else if (featureDefinition is FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips && heroRepertoire != null)
                {
                    var spellsToRemove = featureDefinitionBonusCantrips.BonusCantrips.Count;

                    while (spellsToRemove-- > 0)
                    {
                        heroRepertoire.KnownCantrips.RemoveAt(heroRepertoire.KnownCantrips.Count - 1);
                    }
                }
                else if (featureDefinition is FeatureDefinitionFightingStyleChoice)
                {
                    hero.TrainedFightingStyles.RemoveAt(hero.TrainedFightingStyles.Count - 1);
                }
                else if (featureDefinition is FeatureDefinitionSubclassChoice)
                {
                    hero.ClassesAndSubclasses.Remove(characterClassDefinition);
                }
                else if (featureDefinition is FeatureDefinitionPointPool featureDefinitionPointPool)
                {
                    RemoveFeatureDefinitionPointPool(hero, heroRepertoire, featureDefinitionPointPool);
                }
                else if (featureDefinition is FeatureDefinitionFeatureSet featureDefinitionFeatureSet && featureDefinitionFeatureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                {
                    foreach (var featureDefinitionInSet in featureDefinitionFeatureSet.FeatureSet)
                    {
                        if (featureDefinitionInSet is FeatureDefinitionPointPool featureDefinitionPointPool1)
                        {
                            RemoveFeatureDefinitionPointPool(hero, heroRepertoire, featureDefinitionPointPool1);
                        }
                    }
                }
                else if (featureDefinition is IFeatureDefinitionCustomCode featureDefinitionCustomCode)
                {
                    featureDefinitionCustomCode.RemoveFeature(hero, tag);
                }
            }
        }
    }
}
