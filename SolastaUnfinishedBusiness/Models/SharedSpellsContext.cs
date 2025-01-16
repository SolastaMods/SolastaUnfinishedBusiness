using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Subclasses;
using static FeatureDefinitionCastSpell;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionCastSpellBuilder;

namespace SolastaUnfinishedBusiness.Models;

internal static class SharedSpellsContext
{
    internal const int PactMagicSlotsTab = -1;

    internal static readonly Dictionary<string, BaseDefinition> RecoverySlots = new()
    {
        { PowerCircleLandNaturalRecovery.Name, Druid },
        { PowerWizardArcaneRecovery.Name, Wizard },
        { Level20Context.PowerWarlockEldritchMasterName, Warlock },
        { WizardSpellMaster.PowerSpellMasterBonusRecoveryName, Wizard }
    };

    internal static readonly Dictionary<string, CasterProgression> ClassCasterType = new()
    {
        { Bard.Name, CasterProgression.Full },
        { Cleric.Name, CasterProgression.Full },
        { Druid.Name, CasterProgression.Full },
        { Sorcerer.Name, CasterProgression.Full },
        { Wizard.Name, CasterProgression.Full },
        { Paladin.Name, CasterProgression.Half },
        { Ranger.Name, CasterProgression.Half },
        { InventorClass.ClassName, CasterProgression.HalfRoundUp }
    };

    internal static readonly Dictionary<string, CasterProgression> SubclassCasterType = new()
    {
        { MartialSpellblade.Name, CasterProgression.OneThird },
        { RoguishArcaneScoundrel.Name, CasterProgression.OneThird },
        { RoguishShadowCaster.Name, CasterProgression.OneThird },
        { MartialSpellShield.FullName, CasterProgression.OneThird }
    };

    // supports custom MaxSpellLevelOfSpellCastLevel behaviors
    internal static bool UseMaxSpellLevelOfSpellCastingLevelDefaultBehavior { get; private set; }

    // supports auto prepared spells scenarios on subs
    internal static CasterProgression GetCasterTypeForClassOrSubclass(
        [CanBeNull] string characterClassDefinition,
        string characterSubclassDefinition)
    {
        if (characterClassDefinition != null && ClassCasterType.TryGetValue(characterClassDefinition, out var value1))
        {
            return value1;
        }

        if (characterSubclassDefinition != null &&
            SubclassCasterType.TryGetValue(characterSubclassDefinition, out var value2))
        {
            return value2;
        }

        return CasterProgression.None;
    }

    // need the null check for companions who don't have repertoires
    internal static bool IsMulticaster([CanBeNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero != null
               && rulesetCharacterHero.SpellRepertoires
                   .Count(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race) > 1;
    }

    // factor mystic arcanum level if Warlock repertoire
    internal static void FactorMysticArcanum(
        RulesetCharacterHero hero,
        RulesetSpellRepertoire repertoire,
        ref int level)
    {
        if (repertoire.spellCastingClass != Warlock)
        {
            return;
        }

        var warlockLevel = GetWarlockCasterLevel(hero);

        level = (warlockLevel + 1) / 2;
    }

    // need the null check for companions who don't have repertoires
    private static int GetWarlockCasterLevel([CanBeNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (rulesetCharacterHero == null)
        {
            return 0;
        }

        var warlockLevel = 0;
        var warlock = rulesetCharacterHero.ClassesAndLevels.Keys.FirstOrDefault(x => x == Warlock);

        if (warlock)
        {
            warlockLevel = rulesetCharacterHero.ClassesAndLevels[warlock];
        }

        return warlockLevel;
    }

    internal static int GetWarlockSpellLevel([CanBeNull] RulesetCharacterHero rulesetCharacterHero)
    {
        var warlockLevel = GetWarlockCasterLevel(rulesetCharacterHero);

        return warlockLevel > 0
            ? WarlockCastingSlots[warlockLevel - 1].Slots.IndexOf(0)
            : 0;
    }

    internal static int GetWarlockMaxSlots(RulesetCharacterHero rulesetCharacterHero)
    {
        var warlockLevel = GetWarlockCasterLevel(rulesetCharacterHero);
        var warlockAdditionalSlots = rulesetCharacterHero
            .GetFeaturesByType<FeatureDefinitionMagicAffinity>()
            .Where(x => x == DatabaseHelper.FeatureDefinitionMagicAffinitys
                .MagicAffinityChitinousBoonAdditionalSpellSlot)
            .SelectMany(x => x.AdditionalSlots)
            .Sum(x => x.SlotsNumber);
        var slots = warlockLevel > 0 ? WarlockCastingSlots[warlockLevel - 1].Slots[0] : 0;

        return slots + warlockAdditionalSlots;
    }

    internal static int GetWarlockUsedSlots([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        var repertoire = GetWarlockSpellRepertoire(rulesetCharacterHero);

        if (repertoire == null)
        {
            return 0;
        }

        var slotLevel = IsMulticaster(rulesetCharacterHero)
            ? PactMagicSlotsTab
            : GetWarlockSpellLevel(rulesetCharacterHero);

        repertoire.usedSpellsSlots.TryGetValue(slotLevel, out var warlockUsedSlots);

        return warlockUsedSlots;
    }

    [CanBeNull]
    internal static RulesetSpellRepertoire GetWarlockSpellRepertoire(
        [NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero.GetClassSpellRepertoire(Warlock);
    }

    internal static int GetSharedCasterLevel([CanBeNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (rulesetCharacterHero?.ClassesAndLevels == null)
        {
            return 0;
        }

        var casterLevelContext = new CasterLevelContext();

        foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
        {
            var currentCharacterClassDefinition = classAndLevel.Key;

            rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition,
                out var currentCharacterSubclassDefinition);

            string subclassName = null;

            if (currentCharacterSubclassDefinition)
            {
                subclassName = currentCharacterSubclassDefinition.Name;
            }

            var casterType = GetCasterTypeForClassOrSubclass(
                currentCharacterClassDefinition.Name, subclassName);

            casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
        }

        return casterLevelContext.GetCasterLevel();
    }

    internal static int GetSharedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
    {
        var sharedCasterLevel = GetSharedCasterLevel(rulesetCharacterHero);

        if (Main.Settings.UseAlternateSpellPointsSystem)
        {
            return sharedCasterLevel > 0
                ? SpellPointsContext.SpellPointsFullCastingSlots[sharedCasterLevel - 1].Slots.IndexOf(0)
                : 0;
        }

        return sharedCasterLevel > 0 ? FullCastingSlots[sharedCasterLevel - 1].Slots.IndexOf(0) : 0;
    }

    internal static void LateLoad()
    {
        PatchMaxSpellLevelOfSpellCastingLevel();
        EnumerateSlotsPerLevel(CasterProgression.Full, FullCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.Half, HalfCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.HalfRoundUp, HalfRoundUpCastingSlots);
        EnumerateSlotsPerLevel(CasterProgression.OneThird, OneThirdCastingSlots);
    }

    private static void PatchMaxSpellLevelOfSpellCastingLevel()
    {
        const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler =
            new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>(SharedSpellsTranspiler).Method;
        var methods = new[]
        {
            typeof(CharacterBuildingManager).GetMethod("ApplyFeatureCastSpell", PrivateBinding),
            typeof(GuiCharacter).GetMethod("DisplayUniqueLevelSpellSlots"),
            typeof(ItemMenuModal).GetMethod("SetupFromItem"),
            typeof(RulesetCharacter).GetMethod("EnumerateUsableSpells", PrivateBinding),
            typeof(RulesetCharacterHero).GetMethod("EnumerateUsableRitualSpells"),
            typeof(RulesetSpellRepertoire).GetMethod("HasKnowledgeOfSpell")
        };

        foreach (var method in methods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
            catch
            {
                Main.Error($"Failed to apply SharedSpellsTranspiler patch to {method.DeclaringType}.{method.Name}");
            }
        }
    }

    internal static int MaxSpellLevelOfSpellCastingLevel(RulesetSpellRepertoire rulesetSpellRepertoire)
    {
        UseMaxSpellLevelOfSpellCastingLevelDefaultBehavior = true;

        var result = rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel;

        UseMaxSpellLevelOfSpellCastingLevelDefaultBehavior = false;

        return result;
    }

    [NotNull]
    private static IEnumerable<CodeInstruction> SharedSpellsTranspiler(
        [NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var maxSpellLevelOfSpellCastLevelMethod =
            typeof(RulesetSpellRepertoire).GetMethod("get_MaxSpellLevelOfSpellCastingLevel");
        var myMaxSpellLevelOfSpellCastLevelMethod =
            new Func<RulesetSpellRepertoire, int>(MaxSpellLevelOfSpellCastingLevel).Method;

        return instructions.ReplaceCalls(maxSpellLevelOfSpellCastLevelMethod,
            "SharedSpellsContext.SharedSpellsTranspiler",
            new CodeInstruction(OpCodes.Call, myMaxSpellLevelOfSpellCastLevelMethod));
    }

    #region Caster Level Context

    private sealed class CasterLevelContext
    {
        private readonly Dictionary<CasterProgression, int> _levels;

        internal CasterLevelContext()
        {
            _levels = new Dictionary<CasterProgression, int>
            {
                { CasterProgression.None, 0 },
                { CasterProgression.Full, 0 },
                { CasterProgression.Half, 0 },
                { CasterProgression.HalfRoundUp, 0 },
                { CasterProgression.OneThird, 0 }
            };
        }

        internal void IncrementCasterLevel(CasterProgression casterProgression, int increment)
        {
            _levels[casterProgression] += increment;
        }

        internal int GetCasterLevel()
        {
            var totalKeysGreaterThanZero = 0;
            var fullLevels = 0f;
            var halfLevels = 0f;
            var oneThirdLevels = 0f;

            foreach (var level in _levels)
            {
                var casterType = level.Key;
                var levels = level.Value;

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (casterType)
                {
                    case CasterProgression.Full when levels > 0:
                        totalKeysGreaterThanZero++;
                        fullLevels += levels;
                        break;
                    case CasterProgression.Half or CasterProgression.HalfRoundUp when levels > 0:
                        totalKeysGreaterThanZero++;
                        halfLevels += levels / 2f;
                        break;
                    case CasterProgression.OneThird when levels > 0:
                        totalKeysGreaterThanZero++;
                        oneThirdLevels += levels / 3f;
                        break;
                }
            }

            // ReSharper disable once InvertIf
            if (totalKeysGreaterThanZero == 1)
            {
                if (halfLevels > 1 / 2f ||
                    _levels[CasterProgression.HalfRoundUp] > 0)
                {
                    halfLevels += 1 / 2f;
                }

                if (oneThirdLevels > 0.7f)
                {
                    oneThirdLevels += 2 / 3f;
                }
            }

            return (int)Math.Floor(fullLevels) + (int)Math.Floor(halfLevels) + (int)Math.Floor(oneThirdLevels);
        }
    }

    #endregion

    #region Slots Definitions

    internal static readonly List<SlotsByLevelDuplet> InitiateCastingSlots =
    [
        new() { Slots = [1], Level = 01 },
        new() { Slots = [1], Level = 02 },
        new() { Slots = [1], Level = 03 },
        new() { Slots = [1], Level = 04 },
        new() { Slots = [1], Level = 05 },
        new() { Slots = [1], Level = 06 },
        new() { Slots = [1], Level = 07 },
        new() { Slots = [1], Level = 08 },
        new() { Slots = [1], Level = 09 },
        new() { Slots = [1], Level = 10 },
        new() { Slots = [1], Level = 11 },
        new() { Slots = [1], Level = 12 },
        new() { Slots = [1], Level = 13 },
        new() { Slots = [1], Level = 14 },
        new() { Slots = [1], Level = 15 },
        new() { Slots = [1], Level = 16 },
        new() { Slots = [1], Level = 17 },
        new() { Slots = [1], Level = 18 },
        new() { Slots = [1], Level = 19 },
        new() { Slots = [1], Level = 20 }
    ];

    internal static readonly List<SlotsByLevelDuplet> RaceCastingSlots =
    [
        new() { Slots = [0, 0], Level = 01 },
        new() { Slots = [0, 0], Level = 02 },
        new() { Slots = [1, 0], Level = 03 },
        new() { Slots = [1, 0], Level = 04 },
        new() { Slots = [1, 1], Level = 05 },
        new() { Slots = [1, 1], Level = 06 },
        new() { Slots = [1, 1], Level = 07 },
        new() { Slots = [1, 1], Level = 08 },
        new() { Slots = [1, 1], Level = 09 },
        new() { Slots = [1, 1], Level = 10 },
        new() { Slots = [1, 1], Level = 11 },
        new() { Slots = [1, 1], Level = 12 },
        new() { Slots = [1, 1], Level = 13 },
        new() { Slots = [1, 1], Level = 14 },
        new() { Slots = [1, 1], Level = 15 },
        new() { Slots = [1, 1], Level = 16 },
        new() { Slots = [1, 1], Level = 17 },
        new() { Slots = [1, 1], Level = 18 },
        new() { Slots = [1, 1], Level = 19 },
        new() { Slots = [1, 1], Level = 20 }
    ];

    internal static readonly List<SlotsByLevelDuplet> RaceEmptyCastingSlots =
    [
        new() { Slots = [0], Level = 01 },
        new() { Slots = [0], Level = 02 },
        new() { Slots = [0], Level = 03 },
        new() { Slots = [0], Level = 04 },
        new() { Slots = [0], Level = 05 },
        new() { Slots = [0], Level = 06 },
        new() { Slots = [0], Level = 07 },
        new() { Slots = [0], Level = 08 },
        new() { Slots = [0], Level = 09 },
        new() { Slots = [0], Level = 10 },
        new() { Slots = [0], Level = 11 },
        new() { Slots = [0], Level = 12 },
        new() { Slots = [0], Level = 13 },
        new() { Slots = [0], Level = 14 },
        new() { Slots = [0], Level = 15 },
        new() { Slots = [0], Level = 16 },
        new() { Slots = [0], Level = 17 },
        new() { Slots = [0], Level = 18 },
        new() { Slots = [0], Level = 19 },
        new() { Slots = [0], Level = 20 }
    ];

    // game uses IndexOf(0) on these sub lists reason why the last 0 there
    private static readonly List<SlotsByLevelDuplet> WarlockCastingSlots =
    [
        new()
        {
            Slots =
            [
                1,
                0,
                0,
                0,
                0,
                0
            ],
            Level = 01
        },

        new()
        {
            Slots =
            [
                2,
                0,
                0,
                0,
                0,
                0
            ],
            Level = 02
        },

        new()
        {
            Slots =
            [
                2,
                2,
                0,
                0,
                0,
                0
            ],
            Level = 03
        },

        new()
        {
            Slots =
            [
                2,
                2,
                0,
                0,
                0,
                0
            ],
            Level = 04
        },

        new()
        {
            Slots =
            [
                2,
                2,
                2,
                0,
                0,
                0
            ],
            Level = 05
        },

        new()
        {
            Slots =
            [
                2,
                2,
                2,
                0,
                0,
                0
            ],
            Level = 06
        },

        new()
        {
            Slots =
            [
                2,
                2,
                2,
                2,
                0,
                0
            ],
            Level = 07
        },

        new()
        {
            Slots =
            [
                2,
                2,
                2,
                2,
                0,
                0
            ],
            Level = 08
        },

        new()
        {
            Slots =
            [
                2,
                2,
                2,
                2,
                2,
                0
            ],
            Level = 09
        },

        new()
        {
            Slots =
            [
                2,
                2,
                2,
                2,
                2,
                0
            ],
            Level = 10
        },

        new()
        {
            Slots =
            [
                3,
                3,
                3,
                3,
                3,
                0
            ],
            Level = 11
        },

        new()
        {
            Slots =
            [
                3,
                3,
                3,
                3,
                3,
                0
            ],
            Level = 12
        },

        new()
        {
            Slots =
            [
                3,
                3,
                3,
                3,
                3,
                0
            ],
            Level = 13
        },

        new()
        {
            Slots =
            [
                3,
                3,
                3,
                3,
                3,
                0
            ],
            Level = 14
        },

        new()
        {
            Slots =
            [
                3,
                3,
                3,
                3,
                3,
                0
            ],
            Level = 15
        },

        new()
        {
            Slots =
            [
                3,
                3,
                3,
                3,
                3,
                0
            ],
            Level = 16
        },

        new()
        {
            Slots =
            [
                4,
                4,
                4,
                4,
                4,
                0
            ],
            Level = 17
        },

        new()
        {
            Slots =
            [
                4,
                4,
                4,
                4,
                4,
                0
            ],
            Level = 18
        },

        new()
        {
            Slots =
            [
                4,
                4,
                4,
                4,
                4,
                0
            ],
            Level = 19
        },

        new()
        {
            Slots =
            [
                4,
                4,
                4,
                4,
                4,
                0
            ],
            Level = 20
        }
    ];

    internal static readonly List<SlotsByLevelDuplet> FullCastingSlots = [];
    internal static readonly List<SlotsByLevelDuplet> HalfCastingSlots = [];
    internal static readonly List<SlotsByLevelDuplet> HalfRoundUpCastingSlots = [];
    internal static readonly List<SlotsByLevelDuplet> OneThirdCastingSlots = [];

    // additional spells supporting collections
    internal static readonly List<int> WarlockKnownSpells =
    [
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        10,
        11,
        11,
        12,
        12,
        13,
        13,
        14,
        14,
        15,
        15
    ];

    #endregion
}
