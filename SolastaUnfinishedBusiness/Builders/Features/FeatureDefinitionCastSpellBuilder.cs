using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionCastSpellBuilder
    : DefinitionBuilder<FeatureDefinitionCastSpell, FeatureDefinitionCastSpellBuilder>
{
    internal static void EnumerateReplacedSpells(
        int startingLevel,
        int replaces,
        List<int> replacedSpells)
    {
        replacedSpells.Clear();

        var level = 1;

        for (; level < startingLevel; level++)
        {
            replacedSpells.Add(0);
        }

        for (; level < 21; level++)
        {
            replacedSpells.Add(replaces);
        }
    }

    internal static void EnumerateKnownSpells(
        int startingAmount,
        CasterProgression progression,
        List<int> knownSpells)
    {
        knownSpells.Clear();

        var level = 1;

        for (; level < (int)progression; level++)
        {
            knownSpells.Add(0);
        }

        switch (progression)
        {
            case CasterProgression.Full:
                for (; level < 21; level++)
                {
                    knownSpells.Add(startingAmount + BonusSpellsKnownByCasterLevel[level]);
                }

                break;
            case CasterProgression.Half:
            case CasterProgression.HalfRoundUp:
                for (; level < 21; level++)
                {
                    // +1 here because half casters effectively round up the spells known
                    knownSpells.Add(startingAmount + BonusSpellsKnownByCasterLevel[(level + 1) / 2]);
                }

                break;
            case CasterProgression.OneThird:
                for (; level < 21; level++)
                {
                    knownSpells.Add(startingAmount +
                                    // +2 here because third casters effectively "round up" for spells known
                                    BonusSpellsKnownByCasterLevel[(level + 2) / 3] +
                                    // Third casters also just learn spells faster
                                    BonusSpellsKnownThirdCaster[level]);
                }

                break;
            case CasterProgression.None:
                for (; level < 21; level++)
                {
                    knownSpells.Add(0);
                }

                break;
            case CasterProgression.Flat:
                for (; level < 21; level++)
                {
                    knownSpells.Add(startingAmount);
                }

                break;
            default:
                throw new SolastaUnfinishedBusinessException($"Unknown CasterProgression: {progression}");
        }
    }

    internal static void EnumerateSlotsPerLevel(
        CasterProgression progression,
        List<FeatureDefinitionCastSpell.SlotsByLevelDuplet> slotsPerLevels,
        bool forceOnes = false)
    {
        slotsPerLevels.Clear();

        var index = 0;
        var level = 1;
        var startingLevel = (int)progression;

        if (progression == CasterProgression.HalfRoundUp)
        {
            index = 1;
            startingLevel = 2;
        }

        for (; level < startingLevel; level++)
        {
            var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = SlotsByCasterLevel[index]
            };

            slotsPerLevels.Add(slotsForLevel);
        }

        switch (progression)
        {
            case CasterProgression.Full:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level,
                        Slots = SlotsByCasterLevel[level - startingLevel + 1]
                            .Select(x => x == 0 ? 0 : forceOnes ? 1 : x)
                            .ToList()
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            case CasterProgression.Half:
            case CasterProgression.HalfRoundUp:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level,
                        Slots = SlotsByCasterLevel[((level - startingLevel + 1) / 2) + 1]
                            .Select(x => x == 0 ? 0 : forceOnes ? 1 : x)
                            .ToList()
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            case CasterProgression.OneThird:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level,
                        Slots = SlotsByCasterLevel[((level - startingLevel + 2) / 3) + 1]
                            .Select(x => x == 0 ? 0 : forceOnes ? 1 : x)
                            .ToList()
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            case CasterProgression.None:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level, Slots = SlotsByCasterLevel[0]
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            case CasterProgression.Flat:
            default:
                throw new SolastaUnfinishedBusinessException($"Unknown CasterProgression: {progression}");
        }
    }

    private void InitializeFields()
    {
        SetKnownCantripsZero();
        SetKnownZero();
        SetScribedZero();
        SetSlotsZero();
    }

    internal FeatureDefinitionCastSpellBuilder SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin origin)
    {
        Definition.spellCastingOrigin = origin;
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetSpellCastingAbility(string attribute)
    {
        Definition.spellcastingAbility = attribute;
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetSpellList(SpellListDefinition spellList)
    {
        Definition.spellListDefinition = spellList;
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetSpellKnowledge(SpellKnowledge knowledge)
    {
        Definition.spellKnowledge = knowledge;
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetSpellReadyness(SpellReadyness readyness)
    {
        Definition.spellReadyness = readyness;
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetSlotsRecharge(RechargeRate slotRecharge)
    {
        Definition.slotsRecharge = slotRecharge;
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetUniqueLevelSlots(bool value)
    {
        Definition.uniqueLevelSlots = value;
        return this;
    }

#if false
    internal FeatureDefinitionCastSpellBuilder AddRestrictedSchools(params SchoolOfMagicDefinition[] schools)
    {
        foreach (var school in schools)
        {
            Definition.RestrictedSchools.Add(school.Name);
        }

        Definition.RestrictedSchools.Sort();
        return this;
    }
#endif

    internal FeatureDefinitionCastSpellBuilder SetFocusType(EquipmentDefinitions.FocusType focus)
    {
        Definition.focusType = focus;
        return this;
    }

#if false
    internal FeatureDefinitionCastSpellBuilder SetScribed(params int[] scribedCount)
    {
        Definition.ScribedSpells.SetRange(scribedCount);
        return this;
    }
#endif

    internal FeatureDefinitionCastSpellBuilder SetSpellPreparationCount(
        SpellPreparationCount prepCount)
    {
        Definition.spellPreparationCount = prepCount;
        return this;
    }

    private void SetKnownCantripsZero()
    {
        Definition.KnownCantrips.Clear();
        for (var level = 1; level < 21; level++)
        {
            Definition.KnownCantrips.Add(0);
        }
    }

#if false
    internal FeatureDefinitionCastSpellBuilder SetKnownCantrips(params int[] cantripsCount)
    {
        Definition.KnownCantrips.SetRange(cantripsCount);
        return this;
    }
#endif

    internal FeatureDefinitionCastSpellBuilder SetKnownCantrips(
        int startingAmount,
        int startingLevel,
        CasterProgression progression)
    {
        var level = 1;
        var numCantrips = 0;

        Definition.KnownCantrips.Clear();

        for (; level < startingLevel; level++)
        {
            Definition.KnownCantrips.Add(numCantrips);
        }

        numCantrips = startingAmount;

        switch (progression)
        {
            case CasterProgression.Full:
                for (; level < 4; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                numCantrips++;

                for (; level < 10; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                numCantrips++;

                for (; level < 21; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                break;
            case CasterProgression.Half:
            case CasterProgression.HalfRoundUp:
                for (; level < 10; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                numCantrips++;

                for (; level < 14; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                numCantrips++;

                for (; level < 21; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                break;
            case CasterProgression.OneThird:
                for (; level < 10; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                numCantrips++;

                for (; level < 21; level++)
                {
                    Definition.KnownCantrips.Add(numCantrips);
                }

                break;
            case CasterProgression.Flat:
                for (; level < 21; level++)
                {
                    Definition.KnownCantrips.Add(startingAmount);
                }

                break;
            case CasterProgression.None:
            default:
                throw new SolastaUnfinishedBusinessException($"Unknown CasterProgression: {progression}");
        }

        return this;
    }

    private void SetScribedZero()
    {
        Definition.ScribedSpells.Clear();

        for (var level = 1; level < 21; level++)
        {
            Definition.ScribedSpells.Add(0);
        }
    }

    private void SetKnownZero()
    {
        Definition.KnownSpells.Clear();
        for (var level = 1; level < 21; level++)
        {
            Definition.KnownSpells.Add(0);
        }
    }

    private void SetSlotsZero()
    {
        Definition.SlotsPerLevels.Clear();
        for (var level = 1; level < 21; level++)
        {
            Definition.SlotsPerLevels.Add(new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                level = level,
                Slots =
                [
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                ]
            });
        }
    }

#if false
    internal FeatureDefinitionCastSpellBuilder SetKnownSpells(params int[] spellsCount)
    {
        Definition.KnownSpells.SetRange(spellsCount);
        return this;
    }
#endif

    internal FeatureDefinitionCastSpellBuilder SetKnownSpells(int startingAmount, CasterProgression progression)
    {
        EnumerateKnownSpells(startingAmount, progression, Definition.KnownSpells);
        return this;
    }

#if false
    internal FeatureDefinitionCastSpellBuilder SetReplacedSpells(params int[] spellsCount)
    {
        Definition.ReplacedSpells.SetRange(spellsCount);
        return this;
    }
#endif

    internal FeatureDefinitionCastSpellBuilder SetReplacedSpells(int startingLevel, int replaces)
    {
        EnumerateReplacedSpells(startingLevel, replaces, Definition.ReplacedSpells);
        return this;
    }

#if false
    internal FeatureDefinitionCastSpellBuilder SetSlotsPerLevel(
        params FeatureDefinitionCastSpell.SlotsByLevelDuplet[] slotsPerLevels)
    {
        Definition.SlotsPerLevels.SetRange(slotsPerLevels);
        return this;
    }
#endif

    internal FeatureDefinitionCastSpellBuilder SetSlotsPerLevel(CasterProgression progression)
    {
        EnumerateSlotsPerLevel(progression, Definition.SlotsPerLevels);
        return this;
    }

    internal FeatureDefinitionCastSpellBuilder SetSlotsPerLevel(
        IEnumerable<FeatureDefinitionCastSpell.SlotsByLevelDuplet> slotsByLevelDuplets)
    {
        Definition.SlotsPerLevels.SetRange(slotsByLevelDuplets);
        return this;
    }

    internal enum CasterProgression
    {
        None = 0,
        Full = 1,
        Half = 2,
        OneThird = 3,
        HalfRoundUp = 4,
        Flat = 5
    }

    #region SpellSlots

    private static readonly int[] BonusSpellsKnownByCasterLevel =
    [
        0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 12, 13, 13, 13, 13
    ];

    private static readonly int[] BonusSpellsKnownThirdCaster =
    [
        0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4
    ];

    private static readonly List<int>[] SlotsByCasterLevel =
    [
        [
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            2,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            1,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            2,
            0,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            1,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            0,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            0,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            1,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            1,
            0,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            1,
            1,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            1,
            1,
            0,
            0
        ],
        [
            4,
            3,
            3,
            3,
            2,
            1,
            1,
            1,
            1,
            0
        ],
        [
            4,
            3,
            3,
            3,
            3,
            1,
            1,
            1,
            1,
            0
        ],
        [
            4,
            3,
            3,
            3,
            3,
            2,
            1,
            1,
            1,
            0
        ],
        [
            4,
            3,
            3,
            3,
            3,
            2,
            2,
            1,
            1,
            0
        ]
    ];

    #endregion

    #region Constructors

    protected FeatureDefinitionCastSpellBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
        InitializeFields();
    }

    protected FeatureDefinitionCastSpellBuilder(FeatureDefinitionCastSpell original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
