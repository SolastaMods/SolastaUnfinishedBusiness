using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Classes.Inventor;
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

    // supports custom MaxSpellLevelOfSpellCastLevel behaviors
    internal static bool UseMaxSpellLevelOfSpellCastingLevelDefaultBehavior { get; private set; }

    internal static Dictionary<string, BaseDefinition> RecoverySlots { get; } = new()
    {
        { PowerCircleLandNaturalRecovery.Name, Druid },
        { PowerWizardArcaneRecovery.Name, Wizard },
        { Level20Context.PowerWarlockEldritchMasterName, Warlock },
        { WizardSpellMaster.PowerSpellMasterBonusRecoveryName, Wizard }
    };

    private static Dictionary<string, CasterProgression> ClassCasterType { get; } = new()
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

    private static Dictionary<string, CasterProgression> SubclassCasterType { get; } = new()
    {
        { MartialSpellblade.Name, CasterProgression.OneThird },
        { RoguishShadowCaster.Name, CasterProgression.OneThird },
        { RoguishConArtist.Name, CasterProgression.OneThird },
        { MartialSpellShield.Name, CasterProgression.OneThird },
        { PathOfTheRageMage.Name, CasterProgression.OneThird }
    };

    internal static RulesetCharacterHero GetHero(string name)
    {
        // try to get hero from game campaign
        var gameCampaign = Gui.GameCampaign;

        if (gameCampaign == null)
        {
            // gets hero on inspection or falls back to level up
            return Global.InspectedHero ?? Global.LevelUpHero;
        }

        var gameCampaignCharacter =
            gameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == name);

        if (gameCampaignCharacter is { RulesetCharacter: RulesetCharacterHero rulesetCharacterHero })
        {
            return rulesetCharacterHero;
        }

        // gets hero on inspection or falls back to level up
        return Global.InspectedHero ?? Global.LevelUpHero;
    }

    // supports auto prepared spells scenarios on subs
    private static CasterProgression GetCasterTypeForClassOrSubclass(
        [CanBeNull] string characterClassDefinition,
        string characterSubclassDefinition)
    {
        if (characterClassDefinition != null && ClassCasterType.ContainsKey(characterClassDefinition))
        {
            return ClassCasterType[characterClassDefinition];
        }

        if (characterSubclassDefinition != null && SubclassCasterType.ContainsKey(characterSubclassDefinition))
        {
            return SubclassCasterType[characterSubclassDefinition];
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

    // need the null check for companions who don't have repertoires
    internal static bool IsSharedcaster([CanBeNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero != null
               && rulesetCharacterHero.SpellRepertoires
                   .Where(sr => sr.SpellCastingClass != Warlock)
                   .Count(sr => sr.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race) > 1;
    }

    // factor mystic arcanum level if Warlock repertoire
    internal static void FactorMysticArcanum(RulesetCharacterHero hero, RulesetSpellRepertoire repertoire,
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
    internal static int GetWarlockCasterLevel([CanBeNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (rulesetCharacterHero == null)
        {
            return 0;
        }

        var warlockLevel = 0;
        var warlock = rulesetCharacterHero.ClassesAndLevels.Keys.FirstOrDefault(x => x == Warlock);

        if (warlock != null)
        {
            warlockLevel = rulesetCharacterHero.ClassesAndLevels[warlock];
        }

        return warlockLevel;
    }

    internal static int GetWarlockSpellLevel(RulesetCharacterHero rulesetCharacterHero)
    {
        var warlockLevel = GetWarlockCasterLevel(rulesetCharacterHero);

        return warlockLevel > 0
            ? WarlockCastingSlots[warlockLevel - 1].Slots.IndexOf(0)
            : 0;
    }

    internal static int GetWarlockMaxSlots(RulesetCharacterHero rulesetCharacterHero)
    {
        var warlockLevel = GetWarlockCasterLevel(rulesetCharacterHero);

        return warlockLevel > 0 ? WarlockCastingSlots[warlockLevel - 1].Slots[0] : 0;
    }

    internal static int GetWarlockUsedSlots([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        var repertoire = GetWarlockSpellRepertoire(rulesetCharacterHero);

        if (repertoire == null)
        {
            return 0;
        }

        var slotLevel = IsMulticaster(rulesetCharacterHero)
            ? -1
            : GetWarlockSpellLevel(rulesetCharacterHero);

        repertoire.usedSpellsSlots.TryGetValue(slotLevel, out var warlockUsedSlots);

        return warlockUsedSlots;
    }

    [CanBeNull]
    internal static RulesetSpellRepertoire GetWarlockSpellRepertoire(
        [NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero.SpellRepertoires.FirstOrDefault(x => x.SpellCastingClass == Warlock);
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

            if (currentCharacterSubclassDefinition != null)
            {
                subclassName = currentCharacterSubclassDefinition.Name;
            }

            var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition.Name,
                subclassName);

            casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
        }

        return casterLevelContext.GetCasterLevel();
    }

    internal static int GetSharedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
    {
        if (!IsSharedcaster(rulesetCharacterHero))
        {
            var repertoire = rulesetCharacterHero.SpellRepertoires
                .Find(x => x.SpellCastingFeature.SpellCastingOrigin != CastingOrigin.Race &&
                           x.SpellCastingClass != Warlock);

            return repertoire == null ? 0 : MaxSpellLevelOfSpellCastingLevel(repertoire);
        }

        var sharedCasterLevel = GetSharedCasterLevel(rulesetCharacterHero);

        return sharedCasterLevel > 0 ? FullCastingSlots[sharedCasterLevel - 1].Slots.IndexOf(0) : 0;
    }

    internal static void LateLoad()
    {
        PatchMaxSpellLevelOfSpellCastingLevel();
        EnumerateSlotsPerLevel(CasterProgression.Full, FullCastingSlots);
    }

    private static void PatchMaxSpellLevelOfSpellCastingLevel()
    {
        const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler = new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>(SharedSpellsTranspiler)
            .Method;
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
                Main.Error("cannot fully patch Shared Slots");
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
            new CodeInstruction(OpCodes.Call, myMaxSpellLevelOfSpellCastLevelMethod));
    }

    #region Caster Level Context

    private sealed class CasterLevelContext
    {
        private readonly Dictionary<CasterProgression, int> levels;

        internal CasterLevelContext()
        {
            levels = new Dictionary<CasterProgression, int>
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
            levels[casterProgression] += increment;
        }

        internal int GetCasterLevel()
        {
            var casterLevel = 0;

            // Full Casters
            casterLevel += levels[CasterProgression.Full];

            // Artisan / ...
            if (levels[CasterProgression.HalfRoundUp] == 1)
            {
                casterLevel++;
            }
            // Half Casters
            else
            {
                casterLevel += (int)Math.Floor(levels[CasterProgression.HalfRoundUp] / 2.0);
            }

            casterLevel += (int)Math.Floor(levels[CasterProgression.Half] / 2.0);

            // Con Artist / ...
            casterLevel += (int)Math.Floor(levels[CasterProgression.OneThird] / 3.0);

            return casterLevel;
        }
    }

    #endregion

    #region Slots Definitions

    // game uses IndexOf(0) on these sub lists reason why the last 0 there
    internal static List<SlotsByLevelDuplet> WarlockCastingSlots { get; } = new()
    {
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                1,
                0,
                0,
                0,
                0,
                0
            },
            Level = 01
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                0,
                0,
                0,
                0,
                0
            },
            Level = 02
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                0,
                0,
                0,
                0
            },
            Level = 03
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                0,
                0,
                0,
                0
            },
            Level = 04
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                2,
                0,
                0,
                0
            },
            Level = 05
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                2,
                0,
                0,
                0
            },
            Level = 06
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                2,
                2,
                0,
                0
            },
            Level = 07
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                2,
                2,
                0,
                0
            },
            Level = 08
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                2,
                2,
                2,
                0
            },
            Level = 09
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
                2,
                2,
                2,
                0
            },
            Level = 10
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0
            },
            Level = 11
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0
            },
            Level = 12
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0
            },
            Level = 13
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0
            },
            Level = 14
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0
            },
            Level = 15
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0
            },
            Level = 16
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0
            },
            Level = 17
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0
            },
            Level = 18
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0
            },
            Level = 19
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0
            },
            Level = 20
        }
    };

    // game uses IndexOf(0) on these sub lists reason why the last 0 there
    internal static List<SlotsByLevelDuplet> FullCastingSlots { get; } = new();

    // additional spells supporting collections
    internal static IEnumerable<int> WarlockKnownSpells { get; } = new List<int>
    {
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
    };

    #endregion
}
