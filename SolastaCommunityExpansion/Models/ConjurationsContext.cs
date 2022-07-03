using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models;

internal static class ConjurationsContext
{
    private const string InvisibleStalkerSubspellName = "ConjureElementalInvisibleStalker_CE_SubSpell_CR6";

    internal static readonly HashSet<MonsterDefinition> ConjuredMonsters = new()
    {
        // Conjure animals (3)
        ConjuredOneBeastTiger_Drake,
        ConjuredTwoBeast_Direwolf,
        ConjuredFourBeast_BadlandsSpider,
        ConjuredEightBeast_Wolf,

        // Conjure minor elementals (4)
        SkarnGhoul, // CR 2
        WindSnake, // CR 2
        Fire_Jester, // CR 1

        // Conjure woodland beings (4) - not implemented

        // Conjure elemental (5)
        Air_Elemental, // CR 5
        Fire_Elemental, // CR 5
        Earth_Elemental, // CR 5

        InvisibleStalker, // CR 6

        // Conjure fey (6)
        FeyGiantApe, // CR 6
        FeyGiant_Eagle, // CR 5
        FeyBear, // CR 4
        Green_Hag, // CR 3
        FeyWolf, // CR 2
        FeyDriad // CR 1
    };

    private static readonly Guid Namespace = new("de4539b8e0194684b1d0585100dd94e5");

    /// <summary>
    ///     Allow conjurations to fully controlled party members instead of AI controlled.
    /// </summary>
    internal static void Load()
    {
        // NOTE: assumes monsters have FullyControlledWhenAllied=false by default
        foreach (var conjuredMonster in ConjuredMonsters)
        {
            conjuredMonster.fullyControlledWhenAllied = Main.Settings.FullyControlConjurations;
        }

        if (Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            AddSummonsSubSpells();
        }
    }

    private static void AddSummonsSubSpells()
    {
        // Invisible Stalker
        if (!DatabaseRepository.GetDatabase<SpellDefinition>()
                .TryGetElement(InvisibleStalkerSubspellName, out var _))
        {
            var definition = SpellDefinitionBuilder
                .Create(ConjureElementalFire, InvisibleStalkerSubspellName, Namespace)
                .SetOrUpdateGuiPresentation("Spell/&IPConjureInvisibleStalkerTitle",
                    "Spell/&ConjureElementalDescription")
                .AddToDB();

            var summonForm = definition.EffectDescription
                .GetFirstFormOfType(EffectForm.EffectFormType.Summon)?.SummonForm;

            if (summonForm != null)
            {
                summonForm.monsterDefinitionName = InvisibleStalker.Name;

                ConjureElemental.SubspellsList.Add(definition);
            }
            else
            {
                Main.Error($"Unable to find summon form for {InvisibleStalker.Name}");
            }
        }

        // TODO: add higher level elemental
        // TODO: add higher level fey

        ConfigureAdvancement(ConjureFey);
        ConfigureAdvancement(ConjureElemental);
        ConfigureAdvancement(ConjureMinorElementals);

        // Set advancement at spell level, not sub-spell
        static void ConfigureAdvancement(SpellDefinition spell)
        {
            var advancement = spell.EffectDescription.EffectAdvancement;

            advancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;
            advancement.additionalSpellLevelPerIncrement = 1;
        }
    }
}
