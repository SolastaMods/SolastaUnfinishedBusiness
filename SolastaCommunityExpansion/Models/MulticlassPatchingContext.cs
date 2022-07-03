using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.IntegrationContext;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models;

internal static class MulticlassPatchingContext
{
    private const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

    // these features will be replaced to comply to SRD multiclass rules
    private static readonly Dictionary<FeatureDefinition, FeatureDefinition> FeaturesToReplace = new()
    {
        {ProficiencyBarbarianArmor, ArmorProficiencyMulticlassBuilder.BarbarianArmorProficiencyMulticlass},
        {ProficiencyFighterArmor, ArmorProficiencyMulticlassBuilder.FighterArmorProficiencyMulticlass},
        {ProficiencyPaladinArmor, ArmorProficiencyMulticlassBuilder.PaladinArmorProficiencyMulticlass},
        {PointPoolRangerSkillPoints, SkillProficiencyPointPoolSkillsBuilder.PointPoolRangerSkillPointsMulticlass},
        {PointPoolRogueSkillPoints, SkillProficiencyPointPoolSkillsBuilder.PointPoolRogueSkillPointsMulticlass}
    };

    // these features will be removed to comply with SRD multiclass rules
    private static readonly Dictionary<CharacterClassDefinition, List<FeatureDefinition>> FeaturesToExclude = new()
    {
        {Barbarian, new List<FeatureDefinition> {PointPoolBarbarianrSkillPoints, ProficiencyBarbarianSavingThrow}},
        {
            Cleric,
            new List<FeatureDefinition>
            {
                ProficiencyClericWeapon, PointPoolClericSkillPoints, ProficiencyClericSavingThrow
            }
        },
        {Druid, new List<FeatureDefinition> {PointPoolDruidSkillPoints, ProficiencyDruidSavingThrow}},
        {Fighter, new List<FeatureDefinition> {PointPoolFighterSkillPoints, ProficiencyFighterSavingThrow}},
        {Paladin, new List<FeatureDefinition> {PointPoolPaladinSkillPoints, ProficiencyPaladinSavingThrow}},
        {Ranger, new List<FeatureDefinition> {ProficiencyRangerSavingThrow}},
        {Rogue, new List<FeatureDefinition> {ProficiencyRogueWeapon, ProficiencyRogueSavingThrow}},
        {
            Sorcerer,
            new List<FeatureDefinition>
            {
                ProficiencySorcererWeapon, PointPoolSorcererSkillPoints, ProficiencySorcererSavingThrow
            }
        },
        {
            Wizard,
            new List<FeatureDefinition>
            {
                ProficiencyWizardWeapon, PointPoolWizardSkillPoints, ProficiencyWizardSavingThrow
            }
        }
    };

    private static KeyValuePair<MethodInfo, HeroContext> FeatureUnlocksContext { get; set; }

    internal static void Load()
    {
        PatchClassLevel();
        PatchEquipmentAssignment();
        PatchFeatureUnlocks();
        AddNonOfficialBlueprintsToFeaturesCollections();
    }

    private static void AddNonOfficialBlueprintsToFeaturesCollections()
    {
        var dbFeatureDefinitionPointPool = DatabaseRepository.GetDatabase<FeatureDefinitionPointPool>();
        var dbFeatureDefinitionProficiency = DatabaseRepository.GetDatabase<FeatureDefinitionProficiency>();

        //
        // add these later as need to wait for these blueprints to be instantiated and not willing to publicise CE
        //

        //FeaturesToReplace.Add(
        //    dbFeatureDefinitionProficiency.GetElement("ProficiencyWardenArmor"), 
        //    ArmorProficiencyMulticlassBuilder.WardenArmorProficiencyMulticlass);

        FeaturesToExclude.Add(MonkClass,
            new List<FeatureDefinition>
            {
                dbFeatureDefinitionPointPool.GetElement("ClassMonkSkillProficiency"),
                dbFeatureDefinitionProficiency.GetElement("ClassMonkWeaponProficiency"),
                dbFeatureDefinitionProficiency.GetElement("ClassMonkSavingThrowProficiency")
            });

        FeaturesToExclude.Add(TinkererClass,
            new List<FeatureDefinition>
            {
                dbFeatureDefinitionPointPool.GetElement("PointPoolTinkererSkillPoints"),
                dbFeatureDefinitionProficiency.GetElement("ProficiencyWeaponTinkerer"),
                dbFeatureDefinitionProficiency.GetElement("ProficiencyTinkererSavingThrow")
            });

        // TODO: Uncomment this when class is ready
        //FeaturesToExclude.Add(WardenClass, new()
        //{
        //    dbFeatureDefinitionPointPool.GetElement("PointPoolWardenSkillPoints"),
        //    dbFeatureDefinitionPointPool.GetElement("ProficiencyWardenSavingthrow")
        //});

        FeaturesToExclude.Add(WitchClass,
            new List<FeatureDefinition>
            {
                dbFeatureDefinitionPointPool.GetElement("PointPoolWitchSkillPoints"),
                dbFeatureDefinitionProficiency.GetElement("ProficiencyWitchWeapon"),
                dbFeatureDefinitionProficiency.GetElement("ProficiencyWitchSavingthrow")
            });

        FeaturesToExclude.Add(WarlockClass,
            new List<FeatureDefinition>
            {
                dbFeatureDefinitionPointPool.GetElement("ClassWarlockSkillProficiency"),
                dbFeatureDefinitionProficiency.GetElement("ClassWarlockWeaponProficiency"),
                dbFeatureDefinitionProficiency.GetElement("ClassWarlockSavingThrowProficiency")
            });

        // TODO: Uncomment this when class is ready
        // FeaturesToExclude.Add(MagusClass,
        //     new List<FeatureDefinition>
        //     {
        //         dbFeatureDefinitionPointPool.GetElement("ClassMagusSkillProficiency"),
        //         dbFeatureDefinitionProficiency.GetElement("ClassMagusWeaponProficiency"),
        //         dbFeatureDefinitionProficiency.GetElement("ClassMagusSavingThrowProficiency")
        //     });
    }

    //
    // ClassLevel patching support
    //

    public static IEnumerable<CodeInstruction> ClassLevelTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var classesAndLevelsMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesAndLevels");
        var classesHistoryMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesHistory");
        var getClassLevelMethod = typeof(LevelUpContext).GetMethod("GetSelectedClassLevel");

        var instructionsToBypass = 0;

        foreach (var instruction in instructions)
        {
            if (instructionsToBypass > 0)
            {
                instructionsToBypass--;
            }
            else if (instruction.Calls(classesAndLevelsMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                instructionsToBypass = 2; // bypasses the [] and the classDefinition index
            }
            else if (instruction.Calls(classesHistoryMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                instructionsToBypass = 1; // bypasses the count
            }
            else
            {
                yield return instruction;
            }
        }
    }

    private static void PatchClassLevel()
    {
        var methods = new[]
        {
            // these use ClassesAndLevels[classDefinition]
            typeof(CharacterStageClassSelectionPanel).GetMethod("FillClassFeatures", PrivateBinding),
            typeof(CharacterStageClassSelectionPanel).GetMethod("RefreshCharacter", PrivateBinding),
            // these use ClassesHistory.Count
            typeof(CharacterStageSpellSelectionPanel).GetMethod("Refresh", PrivateBinding),
            typeof(CharacterBuildingManager).GetMethod("AutoAcquireSpells")
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var transpiler = typeof(MulticlassPatchingContext).GetMethod("ClassLevelTranspiler");

        foreach (var method in methods)
        {
            harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
        }
    }

    //
    // Equipment patching support
    //

    public static bool ShouldEquipmentBeAssigned(CharacterHeroBuildingData heroBuildingData)
    {
        var hero = heroBuildingData.HeroCharacter;
        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);

        return !isLevelingUp;
    }

    private static void PatchEquipmentAssignment()
    {
        var methods = new[]
        {
            typeof(CharacterBuildingManager).GetMethod("BuildWieldedConfigurations"),
            typeof(CharacterBuildingManager).GetMethod("ClearWieldedConfigurations"),
            typeof(CharacterBuildingManager).GetMethod("GrantBaseEquipment"),
            typeof(CharacterBuildingManager).GetMethod("RemoveBaseEquipment"),
            typeof(CharacterBuildingManager).GetMethod("UnassignEquipment")
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var prefix = typeof(MulticlassPatchingContext).GetMethod("ShouldEquipmentBeAssigned");

        foreach (var method in methods)
        {
            harmony.Patch(method, new HarmonyMethod(prefix));
        }
    }

    private static void PatchFeatureUnlocks()
    {
        var patches = new Dictionary<MethodInfo, HeroContext>
        {
            {typeof(CharacterStageClassSelectionPanel).GetMethod("OnHigherLevelCb"), HeroContext.StagePanel},
            {
                typeof(CharacterStageClassSelectionPanel).GetMethod("EnumerateActiveFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {
                typeof(CharacterStageClassSelectionPanel).GetMethod("FillClassFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {typeof(CharacterStageDeitySelectionPanel).GetMethod("OnHigherLevelCb"), HeroContext.StagePanel},
            {
                typeof(CharacterStageDeitySelectionPanel).GetMethod("EnumerateActiveFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {
                typeof(CharacterStageDeitySelectionPanel).GetMethod("FillSubclassFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {typeof(CharacterStageDeitySelectionPanel).GetMethod("EnterStage"), HeroContext.StagePanel},
            {typeof(CharacterStageLevelGainsPanel).GetMethod("OnHigherLevelClassCb"), HeroContext.StagePanel},
            {
                typeof(CharacterStageLevelGainsPanel).GetMethod("EnumerateActiveClassFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {
                typeof(CharacterStageLevelGainsPanel).GetMethod("FillUnlockedClassFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {typeof(CharacterStageLevelGainsPanel).GetMethod("Refresh", PrivateBinding), HeroContext.StagePanel},
            {typeof(CharacterStageSubclassSelectionPanel).GetMethod("OnHigherLevelCb"), HeroContext.StagePanel},
            {
                typeof(CharacterStageSubclassSelectionPanel).GetMethod("EnumerateActiveFeatures",
                    PrivateBinding),
                HeroContext.StagePanel
            },
            {
                typeof(CharacterStageSubclassSelectionPanel).GetMethod("FillSubclassFeatures", PrivateBinding),
                HeroContext.StagePanel
            },
            {typeof(CharacterStageSubclassSelectionPanel).GetMethod("Refresh", PrivateBinding), HeroContext.StagePanel},
            {typeof(CharacterBuildingManager).GetMethod("FinalizeCharacter"), HeroContext.BuildingManager},
            {
                typeof(CharacterInformationPanel).GetMethod("TryFindChoiceFeature", PrivateBinding),
                HeroContext.InformationPanel
            },
            {typeof(RulesetCharacterHero).GetMethod("FindClassHoldingFeature"), HeroContext.CharacterHero},
            {typeof(RulesetCharacterHero).GetMethod("LookForFeatureOrigin", PrivateBinding), HeroContext.CharacterHero}
        };

        var harmony = new Harmony("SolastaCommunityExpansion");
        var transpiler = typeof(MulticlassPatchingContext).GetMethod("FeatureUnlocksTranspiler");

        foreach (var patch in patches)
        {
            FeatureUnlocksContext = patch;

            harmony.Patch(patch.Key, transpiler: new HarmonyMethod(transpiler));
        }
    }

    private static IEnumerable<CodeInstruction> YieldHero()
    {
        switch (FeatureUnlocksContext.Value)
        {
            case HeroContext.StagePanel:
                var classType = FeatureUnlocksContext.Key.DeclaringType;
                var currentHeroField =
                    classType.GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                break;

            case HeroContext.BuildingManager:
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                break;

            case HeroContext.CharacterHero:
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                break;

            case HeroContext.InformationPanel:
                var inspectedCharacterMethod =
                    typeof(CharacterInformationPanel).GetMethod("get_InspectedCharacter");
                var rulesetCharacterHeroMethod = typeof(GuiCharacter).GetMethod("get_RulesetCharacterHero");

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call, inspectedCharacterMethod);
                yield return new CodeInstruction(OpCodes.Call, rulesetCharacterHeroMethod);
                break;
        }
    }

    public static IEnumerable<CodeInstruction> FeatureUnlocksTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
        var classFilteredFeatureUnlocksMethod =
            typeof(MulticlassPatchingContext).GetMethod("ClassFilteredFeatureUnlocks");

        var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
        var subclassFilteredFeatureUnlocksMethod =
            typeof(MulticlassPatchingContext).GetMethod("SubclassFilteredFeatureUnlocks");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(classFeatureUnlocksMethod))
            {
                foreach (var inst in YieldHero())
                {
                    yield return inst;
                }

                yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
            }
            else if (instruction.Calls(subclassFeatureUnlocksMethod))
            {
                foreach (var inst in YieldHero())
                {
                    yield return inst;
                }

                yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    // support class filtered feature unlocks
    public static List<FeatureUnlockByLevel> ClassFilteredFeatureUnlocks(
        CharacterClassDefinition characterClassDefinition, RulesetCharacterHero rulesetCharacterHero)
    {
        var firstClass = rulesetCharacterHero.ClassesHistory[0];
        var selectedClass = LevelUpContext.GetSelectedClass(rulesetCharacterHero) ?? characterClassDefinition;
        var selectedSubClass = LevelUpContext.GetSelectedSubclass(rulesetCharacterHero);
        var filteredFeatureUnlockByLevels = selectedClass.FeatureUnlocks.ToList();

        //
        // supports a better MC UI offering
        //
        if (LevelUpContext.IsLevelingUp(rulesetCharacterHero)
            && LevelUpContext.IsClassSelectionStage(rulesetCharacterHero)
            && selectedSubClass != null)
        {
            filteredFeatureUnlockByLevels.AddRange(SubclassFilteredFeatureUnlocks(selectedSubClass,
                rulesetCharacterHero));
        }

        if (!LevelUpContext.IsMulticlass(rulesetCharacterHero) || firstClass == selectedClass)
        {
            return characterClassDefinition.FeatureUnlocks;
        }

        // remove any extra attacks except on classes that get them at 11 (fighter and swiftblade)
        var attacksNumber = rulesetCharacterHero.GetAttribute(AttributeDefinitions.AttacksNumber).CurrentValue;

        if (attacksNumber > 1 && LevelUpContext.GetSelectedClassLevel(rulesetCharacterHero) < 11)
        {
            filteredFeatureUnlockByLevels.RemoveAll(x =>
                x.FeatureDefinition is FeatureDefinitionAttributeModifier featureDefinitionAttributeModifier
                && featureDefinitionAttributeModifier.ModifiedAttribute == AttributeDefinitions.AttacksNumber);
        }

        foreach (var featureNameToReplace in FeaturesToReplace)
        {
            var count = filteredFeatureUnlockByLevels.RemoveAll(
                x => x.FeatureDefinition == featureNameToReplace.Key);

            if (count > 0)
            {
                filteredFeatureUnlockByLevels.Add(new FeatureUnlockByLevel(featureNameToReplace.Value, 1));
            }
        }

        // exclude features per mc rules
        FeaturesToExclude.TryGetValue(selectedClass, out var featureNamesToExclude);

        if (featureNamesToExclude != null)
        {
            filteredFeatureUnlockByLevels.RemoveAll(x => featureNamesToExclude.Contains(x.FeatureDefinition));
        }

        // sort back results
        filteredFeatureUnlockByLevels.Sort((a, b) =>
        {
            var result = a.Level.CompareTo(b.Level);

            if (result == 0)
            {
                result = a.FeatureDefinition.FormatTitle().CompareTo(b.FeatureDefinition.FormatTitle());
            }

            return result;
        });

        return filteredFeatureUnlockByLevels;
    }

    // support subclass filtered feature unlocks
    public static List<FeatureUnlockByLevel> SubclassFilteredFeatureUnlocks(
        CharacterSubclassDefinition characterSublassDefinition, RulesetCharacterHero rulesetCharacterHero)
    {
        if (!LevelUpContext.IsMulticlass(rulesetCharacterHero))
        {
            return characterSublassDefinition.FeatureUnlocks;
        }

        var filteredFeatureUnlockByLevels = characterSublassDefinition.FeatureUnlocks.ToList();

        // remove any extra attacks except on classes that get them at 11 (fighter and swiftblade)
        var attacksNumber = rulesetCharacterHero.GetAttribute(AttributeDefinitions.AttacksNumber).CurrentValue;

        if (attacksNumber > 1 && LevelUpContext.GetSelectedClassLevel(rulesetCharacterHero) < 11)
        {
            filteredFeatureUnlockByLevels.RemoveAll(x =>
                x.FeatureDefinition is FeatureDefinitionAttributeModifier featureDefinitionAttributeModifier
                && featureDefinitionAttributeModifier.ModifiedAttribute == AttributeDefinitions.AttacksNumber);
        }

        return filteredFeatureUnlockByLevels;
    }

    //
    // FeatureUnlocks patching support
    //

    private enum HeroContext
    {
        BuildingManager,
        CharacterHero,
        StagePanel,
        InformationPanel
    }
}
