using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SolastaCommunityExpansion.Helpers;
using HarmonyLib;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.ExtraSummoningModifiers
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "ApplySummonForm")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class RulesetImplementationManagerLocation_ApplySummonForm
    {
        internal static Dictionary<string, int> ConditionToAmount { get; private set; } = new Dictionary<string, int>();

        internal static void Prefix(
            EffectForm effectForm,
            RulesetImplementationDefinitions.ApplyFormsParams formsParams)
        {
            SummonForm summonForm = effectForm.SummonForm;
            if (summonForm.SummonType == SummonForm.Type.Creature && !string.IsNullOrEmpty(summonForm.MonsterDefinitionName) && DatabaseRepository.GetDatabase<MonsterDefinition>().HasElement(summonForm.MonsterDefinitionName))
            {
                MonsterDefinition element = DatabaseRepository.GetDatabase<MonsterDefinition>().GetElement(summonForm.MonsterDefinitionName);
                for (int index = 0; index < summonForm.Number; ++index)
                {
                    formsParams.sourceCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionSummoningAffinity>(formsParams.sourceCharacter.FeaturesToBrowse);
                    foreach (FeatureDefinitionSummoningAffinity summoningAffinity in formsParams.sourceCharacter.FeaturesToBrowse.OfType<FeatureDefinitionSummoningAffinity>())
                    {
                        if (string.IsNullOrEmpty(summoningAffinity.RequiredMonsterTag) || element.CreatureTags.Contains(summoningAffinity.RequiredMonsterTag))
                        {
                            foreach (ConditionDefinition addedCondition in summoningAffinity.AddedConditions)
                            {
                                int sourceAmount = 0;
                                switch (addedCondition.AmountOrigin)
                                {
                                    case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus:
                                        sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                                        ConditionToAmount.AddOrReplace(addedCondition.Name, sourceAmount);
                                        break;
                                    case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceCharacterLevel:
                                        sourceAmount = formsParams.sourceCharacter.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
                                        ConditionToAmount.AddOrReplace(addedCondition.Name, sourceAmount);
                                        break;
                                    case (ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel:
                                        var sourceCharacter = (RulesetCharacterHero)formsParams.sourceCharacter;
                                        // Find a better place to put this in?
                                        string classType = addedCondition.AdditionalDamageType;
                                        if (DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement(classType, out CharacterClassDefinition classDef))
                                        {
                                            if (sourceCharacter.ClassesAndLevels != null 
                                                && sourceCharacter.ClassesAndLevels.TryGetValue(classDef, out int classLevel))
                                            {
                                                sourceAmount = classLevel;
                                            }

                                            ConditionToAmount.AddOrReplace(addedCondition.Name, sourceAmount);
                                        }
                                        break;

                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void Postfix()
        {
            ConditionToAmount.Clear();
        }
    }

    [HarmonyPatch(typeof(RulesetActor), "InflictCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class RulesetActor_InflictCondition
    {

        internal static void Prefix(string conditionDefinitionName,
            ref int sourceAmount)
        {
            if (RulesetImplementationManagerLocation_ApplySummonForm.ConditionToAmount.ContainsKey(conditionDefinitionName))
            {
                sourceAmount = RulesetImplementationManagerLocation_ApplySummonForm.ConditionToAmount[conditionDefinitionName];
            }
        }
    }
}
