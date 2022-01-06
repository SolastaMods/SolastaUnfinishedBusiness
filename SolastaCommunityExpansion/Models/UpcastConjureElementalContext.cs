using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static EffectForm;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class UpcastConjureElementalContext
    {
        private static readonly Guid Namespace = new Guid("de4539b8e0194684b1d0585100dd94e5");

        private const string FireElementalCR6Name = "FireElementalCE_CR6";
        private const string AirElementalCR6Name = "AirElementalCE_CR6";
        private const string EarthElementalCR6Name = "EarthElementalCE_CR6";

        public static void Load()
        {
            if (!Main.Settings.EnableUpcastConjureElemental)
            {
                ResetAdvancement(ConjureElemental);
                return;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Conjure Elemental
            ConfigureAdvancement(ConjureElemental);

            CreateHigherLevelSummons();

            AddUpcastSummons(ConjureElementalAir, AirElementalCR6Name);
            AddUpcastSummons(ConjureElementalEarth, EarthElementalCR6Name);
            AddUpcastSummons(ConjureElementalFire, FireElementalCR6Name);

            // Set advancement at spell level,not sub-spell
            void ConfigureAdvancement(SpellDefinition definition)
            {
                var advancement = definition.EffectDescription.EffectAdvancement;
                advancement.SetEffectIncrementMethod(EffectIncrementMethod.PerAdditionalSlotLevel);
                advancement.SetAdditionalSpellLevelPerIncrement(1);
            }

            void ResetAdvancement(SpellDefinition definition)
            {
                // NOTE: this assumes that default spell doesn't support advancement
                var advancement = definition.EffectDescription.EffectAdvancement;
                advancement.SetEffectIncrementMethod(EffectIncrementMethod.None);
                advancement.SetAdditionalSpellLevelPerIncrement(0);
            }

            void AddUpcastSummons(SpellDefinition definition, params string[] upcastMonsterDefinitionNames)
            {
                var description = definition.EffectDescription;

                if (description.EffectForms.Count != 1)
                {
                    throw new SolastaModApiException($"The supplied spellDefinition does not have exactly one effect form.");
                }

                if (description.EffectForms[0].FormType != EffectFormType.Summon)
                {
                    throw new SolastaModApiException($"The supplied spellDefinition effect form is not EffectFormType.Summon.");
                }

                var originalEffectForm = definition.EffectDescription.EffectForms[0];

                description.EffectForms[0] = new UpcastConjureElementalEffectForm(
                    definition.SpellLevel, originalEffectForm.SummonForm, upcastMonsterDefinitionNames);
            }
        }

        internal static void CreateHigherLevelSummons()
        {
            // Not in DM, not in bestiary.  Purely for summons purposes.

            // Fire
            if (!DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(FireElementalCR6Name, out var _))
            {
                var builder = GetMonsterBuilder(FireElementalCR6Name,
                    "Fire Elemental (CR6)", MonsterDefinitions.Fire_Elemental);

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
                    "Air Elemental (CR6)", MonsterDefinitions.Air_Elemental);

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
                    "Earth Elemental (CR6)", MonsterDefinitions.Earth_Elemental);

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

            // Helpers

            MonsterBuilder GetMonsterBuilder(string name, string title, MonsterDefinition baseMonster)
            {
                return new MonsterBuilder(name, CreateGuid(name), title, baseMonster.GuiPresentation.Description, baseMonster);
            }

            string CreateGuid(string name) => GuidHelper.Create(Namespace, name).ToString("N");

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
        }

        private sealed class UpcastConjureElementalEffectForm : CustomEffectForm
        {
            public int OriginalSpellLevel { get; }
            public List<string> MonsterDefinitionNames { get; }

            internal UpcastConjureElementalEffectForm(int originalSpellLevel, SummonForm summonForm, IEnumerable<string> upcastMonsterDefinitionNames)
            {
                OriginalSpellLevel = originalSpellLevel;

                FormType = (EffectFormType)(-1);

                this.SetSummonForm(SummonForm.GetCopy(summonForm));

                MonsterDefinitionNames = Enumerable.Repeat(summonForm.MonsterDefinitionName, 1).Concat(upcastMonsterDefinitionNames).ToList();
            }

            public override void ApplyForm(RulesetImplementationDefinitions.ApplyFormsParams formsParams, bool retargeting, bool proxyOnly, bool forceSelfConditionOnly)
            {
                var service = ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManagerLocation;

                if(service == null)
                {
                    return;
                }

                try
                {
                    FormType = EffectFormType.Summon;

                    var monsterName = MonsterDefinitionNames[0];
                    var effectLevel = formsParams.effectLevel;

                    if (effectLevel > OriginalSpellLevel)
                    {
                        if (effectLevel - OriginalSpellLevel >= MonsterDefinitionNames.Count)
                        {
                            Main.Log($"ApplyForm: {formsParams.effectLevel} no suitable monster - using highest available.");
                            monsterName = MonsterDefinitionNames.Last();
                        }
                        else
                        {
                            monsterName = MonsterDefinitionNames[effectLevel - OriginalSpellLevel];
                        }
                    }

                    SummonForm.SetMonsterDefinitionName(monsterName);

                    service.ApplySummonForm(this, formsParams);
                }
                finally
                {
                    FormType = (EffectFormType)(-1);
                }
            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
            {
                // empty
            }
        }
    }
}
