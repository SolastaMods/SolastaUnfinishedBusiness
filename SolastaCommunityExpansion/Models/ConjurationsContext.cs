using System;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.MonsterDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class ConjurationsContext
    {
        /// <summary>
        /// Allow conjurations to fully controlled party members instead of AI controlled.
        /// </summary>
        internal static void Load()
        {
            // NOTE: assumes monsters have FullyControlledWhenAllied=false by default

            var controlled = Main.Settings.FullyControlConjurations;

            // Conjure animals (3)
            ConjuredOneBeastTiger_Drake.SetFullyControlledWhenAllied(controlled);
            ConjuredTwoBeast_Direwolf.SetFullyControlledWhenAllied(controlled);
            ConjuredFourBeast_BadlandsSpider.SetFullyControlledWhenAllied(controlled);
            ConjuredEightBeast_Wolf.SetFullyControlledWhenAllied(controlled);

            // Conjure minor elementals (4)
            SkarnGhoul.SetFullyControlledWhenAllied(controlled); // CR 2
            WindSnake.SetFullyControlledWhenAllied(controlled); // CR 2
            Fire_Jester.SetFullyControlledWhenAllied(controlled); // CR 1

            // Conjure woodland beings (4) - not implemented

            // Conjure elemental (5)
            Air_Elemental.SetFullyControlledWhenAllied(controlled); // CR 5
            Fire_Elemental.SetFullyControlledWhenAllied(controlled); // CR 5
            Earth_Elemental.SetFullyControlledWhenAllied(controlled); // CR 5

            InvisibleStalker.SetFullyControlledWhenAllied(controlled); // CR 6

            // Conjure fey (6)
            FeyGiantApe.SetFullyControlledWhenAllied(controlled); // CR 6
            FeyGiant_Eagle.SetFullyControlledWhenAllied(controlled); // CR 5
            FeyBear.SetFullyControlledWhenAllied(controlled); // CR 4
            Green_Hag.SetFullyControlledWhenAllied(controlled); // CR 3
            FeyWolf.SetFullyControlledWhenAllied(controlled); // CR 2
            FeyDriad.SetFullyControlledWhenAllied(controlled); // CR 1

            if (Main.Settings.DismissControlledConjurationsWhenDeliberatelyDropConcentration)
            {
                // Change conjured Fey to persist and become hostile when concentration accidentally dropped
                SetPersist(ConjureFey_Ape);
                SetPersist(ConjureFey_Bear);
                SetPersist(ConjureFey_Dryad);
                SetPersist(ConjureFey_Eagle);
                SetPersist(ConjureFey_GreenHag);
                SetPersist(ConjureFey_Wolf);

                static void SetPersist(SpellDefinition spell)
                {
                    spell.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Summon).SummonForm.SetPersistOnConcentrationLoss(true);
                }
            }

            if (Main.Settings.EnableUpcastConjureElementalAndFey)
            {
                CreateAdditionalSummons();
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
                    .SetGuiPresentation("Spell/&IPConjureInvisibleStalkerTitle", "Spell/&ConjureElementalDescription")
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

        internal static void CreateAdditionalSummons()
        {
            /*
            // Fire
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(FireElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(FireElementalCR6Name,
                    "Fire Elemental (CR6)", Fire_Elemental);

                var definition = builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(42)
                    .SetStandardHitPoints(77 + 42)
                    .SetAbilityScores(12, 17, 16, 6, 10, 7)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();

                definition.AttackIterations.SetRange(CreateAttackIteration(definition.AttackIterations[0], "CE_CR6"));
            }

            // Air
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(AirElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(AirElementalCR6Name,
                    "Air Elemental (CR6)", Air_Elemental);

                var definition = builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(28)
                    .SetStandardHitPoints(77 + 28)
                    .SetAbilityScores(16, 20, 14, 6, 10, 6)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();

                definition.AttackIterations.SetRange(CreateAttackIteration(definition.AttackIterations[0], "CE_CR6"));
            }

            // Earth
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(EarthElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(EarthElementalCR6Name,
                    "Earth Elemental (CR6)", Earth_Elemental);

                var definition = builder
                    .SetHitDiceNumber(14)
                    .SetHitPointsBonus(60)
                    .SetStandardHitPoints(77 + 60)
                    .SetAbilityScores(22, 8, 20, 5, 10, 5)
                    .SetModelScale(0.75f)
                    .SetChallengeRating(6)
                    .SetInDungeonEditor(false)
                    .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
                    .AddToDB();

                definition.AttackIterations.SetRange(CreateAttackIteration(definition.AttackIterations[0], "CE_CR6"));
            }
            */

            // Helpers

            /*            MonsterBuilder GetMonsterBuilder(string name, string title, MonsterDefinition baseMonster)
                        {
                            return new MonsterBuilder(name, CreateGuid(name), title, baseMonster.GuiPresentation.Description, baseMonster);
                        }

                        MonsterAttackIteration CreateAttackIteration(MonsterAttackIteration attackIteration, string namePrefix, int attacks = 2)
                        {
                            // copy existing attack iteration and bump up ToHitBonus and DamageBonus by 1
                            var attackDefinition = CreateAttackDefinition(attackIteration.MonsterAttackDefinition, namePrefix);

                            return new MonsterAttackIteration(attackDefinition, attacks);
                        }

                        MonsterAttackDefinition CreateAttackDefinition(MonsterAttackDefinition attackDefinition, string namePrefix)
                        {
                            var name = $"{namePrefix}_{attackDefinition.Name}";

                            var builder = new MonsterAttackDefinitionBuilder(name, CreateGuid(name), attackDefinition);

                            builder.SetDamageBonusOfFirstDamageForm(4);
                            builder.SetToHitBonus(7);

                            return builder.AddToDB();
                        }
            */
        }
    }
}
