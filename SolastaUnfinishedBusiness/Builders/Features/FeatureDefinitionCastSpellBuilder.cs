using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionCastSpellBuilder : FeatureDefinitionBuilder<FeatureDefinitionCastSpell,
    FeatureDefinitionCastSpellBuilder>
{
    private static readonly int[] BonusSpellsKnownByCasterLevel =
    {
        0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 12, 13, 13, 13, 13
    };

    private static readonly int[] BonusSpellsKnownThirdCaster =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4
    };

    private static readonly List<int>[] SlotsByCasterLevel =
    {
        new()
        {
            0,
            0,
            0,
            0,
            0,
            0
        },
        new()
        {
            2,
            0,
            0,
            0,
            0,
            0
        },
        new()
        {
            3,
            0,
            0,
            0,
            0,
            0
        },
        new()
        {
            4,
            2,
            0,
            0,
            0,
            0
        },
        new()
        {
            4,
            3,
            0,
            0,
            0,
            0
        },
        new()
        {
            4,
            3,
            2,
            0,
            0,
            0
        },
        new()
        {
            4,
            3,
            3,
            0,
            0,
            0
        },
        new()
        {
            4,
            3,
            3,
            1,
            0,
            0
        },
        new()
        {
            4,
            3,
            3,
            2,
            0,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            1,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            2,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            3,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            3,
            0
        },
        new()
        {
            4,
            3,
            3,
            3,
            3,
            0
        }
    };

    internal static void EnumerateKnownSpells(
        int startingAmount,
        int startingLevel,
        CasterProgression progression,
        List<int> knownSpells
    )
    {
        knownSpells.Clear();

        var level = 1;

        for (; level < startingLevel; level++)
        {
            knownSpells.Add(0);
        }

        switch (progression)
        {
            case CasterProgression.FullCaster:
                for (; level < 21; level++)
                {
                    knownSpells.Add(startingAmount + BonusSpellsKnownByCasterLevel[level]);
                }

                break;
            case CasterProgression.HalfCaster:
                for (; level < 21; level++)
                {
                    // +1 here because half casters effectively round up the spells known
                    knownSpells.Add(startingAmount + BonusSpellsKnownByCasterLevel[(level + 1) / 2]);
                }

                break;
            case CasterProgression.ThirdCaster:
                for (; level < 21; level++)
                {
                    knownSpells.Add(startingAmount +
                                    // +2 here because third casters effectively "round up" for spells known
                                    BonusSpellsKnownByCasterLevel[(level + 2) / 3] +
                                    // Third casters also just learn spells faster
                                    BonusSpellsKnownThirdCaster[level]);
                }

                break;
            default:
                throw new SolastaUnfinishedBusinessException($"Unknown CasterProgression: {progression}");
        }
    }

    internal static void EnumerateSlotsPerLevel(
        int startingLevel,
        CasterProgression progression,
        List<FeatureDefinitionCastSpell.SlotsByLevelDuplet> slotsPerLevels)
    {
        var level = 1;

        for (; level < startingLevel; level++)
        {
            var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = SlotsByCasterLevel[0]
            };

            slotsPerLevels.Add(slotsForLevel);
        }

        switch (progression)
        {
            case CasterProgression.FullCaster:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level, Slots = SlotsByCasterLevel[level - startingLevel + 1]
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            case CasterProgression.HalfCaster:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level, Slots = SlotsByCasterLevel[((level - startingLevel) / 2) + 1]
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            case CasterProgression.ThirdCaster:
                for (; level < 21; level++)
                {
                    var slotsForLevel = new FeatureDefinitionCastSpell.SlotsByLevelDuplet
                    {
                        Level = level, Slots = SlotsByCasterLevel[((level - startingLevel + 2) / 3) + 1]
                    };

                    slotsPerLevels.Add(slotsForLevel);
                }

                break;
            default:
                throw new SolastaUnfinishedBusinessException($"Unknown CasterProgression: {progression}");
        }
    }

    private void InitializeFields()
    {
        SetKnownCantripsZero();
        SetKnownZero();
        SetScribedZero();
    }

    public FeatureDefinitionCastSpellBuilder SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin origin)
    {
        Definition.spellCastingOrigin = origin;
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetSpellCastingAbility(string attribute)
    {
        Definition.spellcastingAbility = attribute;
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetSpellList(SpellListDefinition spellList)
    {
        Definition.spellListDefinition = spellList;
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetSpellKnowledge(RuleDefinitions.SpellKnowledge knowledge)
    {
        Definition.spellKnowledge = knowledge;
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetSpellReadyness(RuleDefinitions.SpellReadyness readyness)
    {
        Definition.spellReadyness = readyness;
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetSlotsRecharge(RuleDefinitions.RechargeRate slotRecharge)
    {
        Definition.slotsRecharge = slotRecharge;
        return this;
    }

    public FeatureDefinitionCastSpellBuilder AddRestrictedSchool(SchoolOfMagicDefinition school)
    {
        Definition.RestrictedSchools.Add(school.Name);
        Definition.RestrictedSchools.Sort();
        return this;
    }

    public FeatureDefinitionCastSpellBuilder AddRestrictedSchools(params SchoolOfMagicDefinition[] schools)
    {
        foreach (var school in schools)
        {
            AddRestrictedSchool(school);
        }

        Definition.RestrictedSchools.Sort();
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetScribed(params int[] scribedCount)
    {
        return SetScribed(scribedCount.AsEnumerable());
    }

    public FeatureDefinitionCastSpellBuilder SetScribed(IEnumerable<int> scribedCount)
    {
        Definition.ScribedSpells.SetRange(scribedCount);
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

    public FeatureDefinitionCastSpellBuilder SetKnownCantrips(params int[] cantripsCount)
    {
        return SetKnownCantrips(cantripsCount.AsEnumerable());
    }

    public FeatureDefinitionCastSpellBuilder SetKnownCantrips(IEnumerable<int> cantripsCount)
    {
        Definition.KnownCantrips.SetRange(cantripsCount);
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetKnownCantrips(
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
            case CasterProgression.FullCaster:
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
            case CasterProgression.HalfCaster:
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
            case CasterProgression.ThirdCaster:
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

    public FeatureDefinitionCastSpellBuilder SetKnownSpells(params int[] spellsCount)
    {
        return SetKnownSpells(spellsCount.AsEnumerable());
    }

    public FeatureDefinitionCastSpellBuilder SetKnownSpells(IEnumerable<int> spellsCount)
    {
        Definition.KnownSpells.SetRange(spellsCount);
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetKnownSpells(int startingAmount, int startingLevel,
        CasterProgression progression)
    {
        EnumerateKnownSpells(startingAmount, startingLevel, progression, Definition.KnownSpells);
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetReplacedSpells(params int[] spellsCount)
    {
        return SetReplacedSpells(spellsCount.AsEnumerable());
    }

    public FeatureDefinitionCastSpellBuilder SetReplacedSpells(IEnumerable<int> spellsCount)
    {
        Definition.ReplacedSpells.SetRange(spellsCount);
        return this;
    }

    public FeatureDefinitionCastSpellBuilder SetSlotsPerLevel(
        params FeatureDefinitionCastSpell.SlotsByLevelDuplet[] slotsPerLevels)
    {
        return SetSlotsPerLevel(slotsPerLevels.AsEnumerable());
    }

    public FeatureDefinitionCastSpellBuilder SetSlotsPerLevel(
        IEnumerable<FeatureDefinitionCastSpell.SlotsByLevelDuplet> slotsPerLevels)
    {
        Definition.SlotsPerLevels.SetRange(slotsPerLevels);
        return this;
    }


    public FeatureDefinitionCastSpellBuilder SetSlotsPerLevel(int startingLevel, CasterProgression progression)
    {
        EnumerateSlotsPerLevel(startingLevel, progression, Definition.SlotsPerLevels);
        return this;
    }

    internal enum CasterProgression
    {
        FullCaster,
        HalfCaster,
        ThirdCaster
    }

    #region Constructors

    protected FeatureDefinitionCastSpellBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
        InitializeFields();
    }

    protected FeatureDefinitionCastSpellBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
        InitializeFields();
    }

    protected FeatureDefinitionCastSpellBuilder(FeatureDefinitionCastSpell original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionCastSpellBuilder(FeatureDefinitionCastSpell original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
