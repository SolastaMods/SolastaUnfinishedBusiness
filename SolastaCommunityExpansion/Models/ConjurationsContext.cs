using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class ConjurationsContext
    {
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
            FeyDriad, // CR 1
        };

        /// <summary>
        /// Allow conjurations to fully controlled party members instead of AI controlled.
        /// </summary>
        internal static void Load()
        {
            // NOTE: assumes monsters have FullyControlledWhenAllied=false by default

            var controlled = Main.Settings.FullyControlConjurations;

            foreach (var conjuredMonster in ConjuredMonsters)
            {
                conjuredMonster.SetFullyControlledWhenAllied(controlled);
            }

            if (Main.Settings.EnableUpcastConjureElementalAndFey)
            {
                AddSummonsSubSpells();
            }
        }

        private static readonly Guid Namespace = new("de4539b8e0194684b1d0585100dd94e5");

        private const string InvisibleStalkerSubspellName = "ConjureElementalInvisibleStalker_CE_SubSpell_CR6";

        internal static void AddSummonsSubSpells()
        {
            // Invisible Stalker
            if (!DatabaseRepository.GetDatabase<SpellDefinition>().TryGetElement(InvisibleStalkerSubspellName, out var _))
            {
                var definition = SpellDefinitionBuilder
                    .Create(ConjureElementalFire, InvisibleStalkerSubspellName, Namespace)
                    .SetOrUpdateGuiPresentation("Spell/&IPConjureInvisibleStalkerTitle", "Spell/&ConjureElementalDescription")
                    .AddToDB();

                var summonForm = definition.EffectDescription
                    .GetFirstFormOfType(EffectForm.EffectFormType.Summon)?.SummonForm;

                if (summonForm != null)
                {
                    summonForm.SetMonsterDefinitionName(InvisibleStalker.Name);

                    ConjureElemental.SubspellsList.Add(definition);
                }
                else
                {
                    Main.Error($"Unable to find summon form for {InvisibleStalker.Name}");
                }
            }

            // TODO: add higher and lower level elementals
            // TODO: add higher level fey

            ConfigureAdvancement(ConjureElemental);
            ConfigureAdvancement(ConjureFey);

            // Set advancement at spell level,not sub-spell
            static void ConfigureAdvancement(SpellDefinition spell)
            {
                var advancement = spell.EffectDescription.EffectAdvancement;
                advancement.SetEffectIncrementMethod(EffectIncrementMethod.PerAdditionalSlotLevel);
                advancement.SetAdditionalSpellLevelPerIncrement(1);
            }
        }
    }
}
