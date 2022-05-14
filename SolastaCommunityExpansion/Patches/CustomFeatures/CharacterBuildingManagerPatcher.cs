using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    // register the hero getting created
    [HarmonyPatch(typeof(CharacterBuildingManager), "CreateNewCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_CreateCharacter
    {
        internal static void Postfix(CharacterBuildingManager __instance)
        {
            LevelUpContext.RegisterHero(__instance.CurrentLocalHeroCharacter, null, null);
        }
    }

    // register the hero leveling up
    [HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_LevelUpCharacter
    {
        internal static void Prefix(RulesetCharacterHero hero)
        {
            var lastClass = hero.ClassesHistory.Last();

            hero.ClassesAndSubclasses.TryGetValue(lastClass, out var lastSubclass);

            LevelUpContext.RegisterHero(hero, lastClass, lastSubclass, levelingUp: true);
        }
    }

    // unregister the hero leveling up and add all known spells to wholelist casters
    [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_FinalizeCharacter
    {
        internal static void Postfix(RulesetCharacterHero hero)
        {
            hero.SpellRepertoires.Sort((a, b) =>
            {
                if (a.SpellCastingRace != null)
                {
                    return -1;
                }

                if (b.SpellCastingRace != null)
                {
                    return 1;
                }

                var title1 = a.SpellCastingClass != null
                    ? a.SpellCastingClass.FormatTitle()
                    : a.SpellCastingSubclass.FormatTitle();

                var title2 = b.SpellCastingClass != null
                    ? b.SpellCastingClass.FormatTitle()
                    : b.SpellCastingSubclass.FormatTitle();

                return title1.CompareTo(title2);
            });

            var selectedClassRepertoire = LevelUpContext.GetSelectedClassOrSubclassRepertoire(hero);

            if (selectedClassRepertoire == null
                || selectedClassRepertoire.SpellCastingFeature.SpellKnowledge != RuleDefinitions.SpellKnowledge.WholeList)
            {
                LevelUpContext.UnregisterHero(hero);

                return;
            }

            var spellCastingClass = selectedClassRepertoire.SpellCastingClass;

            if (spellCastingClass == null)
            {
                LevelUpContext.UnregisterHero(hero);

                return;
            }

            var castingLevel = SharedSpellsContext.GetClassSpellLevel(selectedClassRepertoire);
            var knownSpells = LevelUpContext.GetAllowedSpells(hero);

            if (knownSpells == null)
            {
                LevelUpContext.RecacheSpells(hero);

                knownSpells = LevelUpContext.GetAllowedSpells(hero);
            }

            selectedClassRepertoire.KnownSpells.AddRange(knownSpells
                .Where(x => x.SpellLevel == castingLevel));

            LevelUpContext.UnregisterHero(hero);
        }
    }

    //
    // this is now handled in MC as we cannot have both a bool prefix and a void prefix on same method
    //
#if false
    // captures the desired class
    [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_AssignClassLevel
    {
        internal static void Prefix(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
        {
            LevelUpContext.SetSelectedClass(hero, classDefinition);
        }
    }
#endif

    // captures the desired sub class
    [HarmonyPatch(typeof(CharacterBuildingManager), "AssignSubclass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_AssignSubclass
    {
        internal static void Prefix(RulesetCharacterHero hero, CharacterSubclassDefinition subclassDefinition)
        {
            LevelUpContext.SetSelectedSubclass(hero, subclassDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        internal static void Postfix(RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures, string tag)
        {
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            // this is a MC scenario
            if (isLevelingUp && isClassSelectionStage)
            {
                return;
            }

            CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, tag, grantedFeatures);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "ClearPrevious")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_ClearPrevious
    {
        private static readonly List<FeatureDefinition> ToRemove = new();
        internal static void Prefix(RulesetCharacterHero hero, string tag)
        {
            ToRemove.Clear();
            if (string.IsNullOrEmpty(tag) || !hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }
            ToRemove.AddRange(hero.ActiveFeatures[tag]);
        }

        internal static void Postfix(RulesetCharacterHero hero, string tag)
        {
            if (ToRemove.Empty())
            {
                return;
            }
            //TODO: check if other places where this is called require same prefx/postfix treatment
            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, ToRemove);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnacquireBonusCantrips")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnacquireBonusCantrips
    {
        internal static void Prefix(CharacterHeroBuildingData heroBuildingData, ref string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            tag = CustomFeaturesContext.UnCustomizeTag(tag);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignRace")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignRace
    {
        internal static void Prefix(CharacterHeroBuildingData heroBuildingData)
        {
            var hero = heroBuildingData.HeroCharacter;
            var tag = AttributeDefinitions.TagRace;

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
        }
    }

    //
    // this is now handled in MC as we cannot have both a bool prefix and a void prefix on same method
    //
#if false
    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastClassLevel
    {
        internal static void Prefix(RulesetCharacterHero hero)
        {
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            // this is a MC scenario
            if (isLevelingUp && isClassSelectionStage)
            {
                return;
            }

            var heroBuildingData = hero.GetOrCreateHeroBuildingData();
            var classDefinition = hero.ClassesHistory[heroBuildingData.HeroCharacter.ClassesHistory.Count - 1];
            var classesAndLevel = hero.ClassesAndLevels[classDefinition];
            var tag = AttributeDefinitions.GetClassTag(classDefinition, classesAndLevel);

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
        }
    }
#endif

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignBackground")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignBackground
    {
        internal static void Prefix(CharacterHeroBuildingData heroBuildingData)
        {
            var hero = heroBuildingData.HeroCharacter;
            var tag = AttributeDefinitions.TagBackground;

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastSubclass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastSubclass
    {
        internal static void Prefix(RulesetCharacterHero hero, bool onlyIfCurrentLevel = false)
        {
            var classDefinition = hero.ClassesHistory[hero.ClassesHistory.Count - 1];
            var classLevel = hero.ClassesAndLevels[classDefinition];
            var level = 0;

            if (onlyIfCurrentLevel)
            {
                classDefinition.TryGetSubclassFeature(out _, out level);
            }

            if (!hero.ClassesAndSubclasses.ContainsKey(classDefinition) || onlyIfCurrentLevel && classLevel > level)
            {
                return;
            }

            var classesAndSubclass = hero.ClassesAndSubclasses[classDefinition];
            var tag = AttributeDefinitions.GetSubclassTag(classDefinition, classLevel, classesAndSubclass);

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
        }
    }
}
