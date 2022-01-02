using System.Collections.Generic;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Spells
{
    internal class SrdSpells
    {
        public static void Load()
        {
            SpellsContext.RegisterSpell(BuildDivineWord(), "SpellListCleric");
        }

        private static SpellDefinition BuildDivineWord()
        {
            SpellBuilder spellBuilder = new SpellBuilder(
                    "CJDivineWord",
                    "18ecba41-a8ac-4048-979e-2139e66934a7");

            spellBuilder.SetSchoolOfMagic(DatabaseHelper.SchoolOfMagicDefinitions.SchoolEvocation);
            spellBuilder.SetMaterialComponent(RuleDefinitions.MaterialComponentType.None);
            spellBuilder.SetCastingTime(RuleDefinitions.ActivationTime.BonusAction);
            spellBuilder.SetSomaticComponent(false);
            spellBuilder.SetVerboseComponent(true);
            spellBuilder.SetSpellLevel(7);
            spellBuilder.SetGuiPresentation(
                    new GuiPresentationBuilder(
                            "Spell/&CJDivineWordDescription",
                            "Spell/&CJDivineWordTitle").Build()
                            .SetSpriteReference(DatabaseHelper.SpellDefinitions.DivineFavor.GuiPresentation.SpriteReference));


            spellBuilder.SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.IndividualsUnique, 20, 1, ActionDefinitions.ItemSelectionType.None)
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MassHealingWord.EffectDescription.EffectParticleParameters)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .SetSavingThrowData(true, false, AttributeDefinitions.Charisma, true, RuleDefinitions.EffectDifficultyClassComputation.SpellCastingFeature, AttributeDefinitions.Wisdom, 15, false, new List<SaveAffinityBySenseDescription>())
                .AddEffectForm(new DivineWordEffectForm())
                .Build());
            SpellDefinition spell = spellBuilder.AddToDB();
            return spell;
        }

        private sealed class DivineWordEffectForm : CustomEffectForm
        {
            private readonly List<string> monsterFamilyPlaneshiftList = new List<string>()
            {
                "Celestial",
                "Elemental",
                "Fey",
                "Fiend",
            };

            public override void ApplyForm(RulesetImplementationDefinitions.ApplyFormsParams formsParams, bool retargeting, bool proxyOnly, bool forceSelfConditionOnly)
            {
                if (formsParams.saveOutcome == RuleDefinitions.RollOutcome.CriticalSuccess || formsParams.saveOutcome == RuleDefinitions.RollOutcome.Success)
                {
                    return;
                }

                // If the target is in one of the special families, banish it.
                if (formsParams.targetCharacter is RulesetCharacterMonster monster && monsterFamilyPlaneshiftList.Contains(monster.CharacterFamily))
                {
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionBanished, RuleDefinitions.DurationType.Day, 1);
                    return;
                }

                int curHP = formsParams.targetCharacter.CurrentHitPoints;
                if (curHP <= 20)
                {
                    if (formsParams.targetCharacter.IsDead)
                    {
                        return;
                    }
                    ServiceRepository.GetService<IGameLocationActionService>().InstantKillCharacter(formsParams.targetCharacter as RulesetCharacter);
                }
                else if (curHP <= 30)
                {
                    // blind, deafened, stunned 1 hour
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionDeafened, RuleDefinitions.DurationType.Hour, 1);
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionBlinded, RuleDefinitions.DurationType.Hour, 1);
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionStunned, RuleDefinitions.DurationType.Hour, 1);
                }
                else if (curHP <= 40)
                {
                    // deafened, blinded 10 minutes
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionDeafened, RuleDefinitions.DurationType.Minute, 10);
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionBlinded, RuleDefinitions.DurationType.Minute, 10);
                }
                else if (curHP <= 50)
                {
                    // deafened 1 minute
                    ApplyCondition(formsParams, DatabaseHelper.ConditionDefinitions.ConditionDeafened, RuleDefinitions.DurationType.Minute, 1);
                }
            }

            public override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
            {
                DatabaseHelper.ConditionDefinitions.ConditionDeafened.FillTags(tagsMap);
                DatabaseHelper.ConditionDefinitions.ConditionBlinded.FillTags(tagsMap);
                DatabaseHelper.ConditionDefinitions.ConditionStunned.FillTags(tagsMap);
                DatabaseHelper.ConditionDefinitions.ConditionBanished.FillTags(tagsMap);
            }

            private static void ApplyCondition(RulesetImplementationDefinitions.ApplyFormsParams formsParams, ConditionDefinition condition, RuleDefinitions.DurationType durationType, int durationParam)
            {
                // Prepare params for inflicting conditions
                ulong sourceGuid = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.Guid : 0L;
                string sourceFaction = formsParams.sourceCharacter != null ? formsParams.sourceCharacter.CurrentFaction.Name : string.Empty;
                string effectDefinitionName = string.Empty;
                if (formsParams.attackMode != null)
                {
                    effectDefinitionName = formsParams.attackMode.SourceDefinition.Name;
                }
                else if (formsParams.activeEffect != null)
                {
                    effectDefinitionName = formsParams.activeEffect.SourceDefinition.Name;
                }
                int sourceAbilityBonus = formsParams.activeEffect != null ? formsParams.activeEffect.ComputeSourceAbilityBonus(formsParams.sourceCharacter) : 0;

                formsParams.targetCharacter.InflictCondition(condition.Name, durationType, durationParam, RuleDefinitions.TurnOccurenceType.EndOfTurn, "11Effect", sourceGuid, sourceFaction, formsParams.effectLevel, effectDefinitionName, 0, sourceAbilityBonus);
            }
        }
    }
}
