// using System;
// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using JetBrains.Annotations;
// using SolastaUnfinishedBusiness.Builders.Features;
//
// namespace SolastaUnfinishedBusiness.CustomDefinitions;
//
// /// <summary>
// ///     Grants you immunity to opportunity attacks when the attacker has the specified condition (inflicted by you).
// /// </summary>
// public sealed class FeatureDefinitionOpportunityAttackImmunityIfAttackerHasCondition : FeatureDefinition,
//     ICombatAffinityProvider
// {
//     public string ConditionName { get; set; }
//
//     public void ComputeDefenseModifier(RulesetCharacter myself, RulesetCharacter attacker, int sustainedAttacks,
//         bool defenderAlreadyAttackedByAttackerThisTurn, ActionModifier attackModifier,
//         RuleDefinitions.FeatureOrigin featureOrigin,
//         float distance)
//     {
//         throw new NotImplementedException();
//     }
//
//     public RuleDefinitions.SituationalContext SituationalContext => RuleDefinitions.SituationalContext.None;
//     public bool CanRageToOvercomeSurprise => false;
//     public bool AutoCritical => false;
//     public bool CriticalHitImmunity => false;
//     [CanBeNull] public ConditionDefinition RequiredCondition => null;
//     public bool IgnoreCover => false;
//     public RuleDefinitions.CoverType PermanentCover { get; }
//     public RuleDefinitions.AdvantageType ReadyAttackAdvantage { get; }
//     public bool ShoveOnReadyAttackHit { get; }
//
//     public void ComputeAttackModifier(RulesetCharacter myself, RulesetCharacter defender, RulesetAttackMode attackMode,
//         ActionModifier attackModifier, RuleDefinitions.FeatureOrigin featureOrigin, int bardicDieRoll, float distance)
//     {
//         // Intentionally empty?
//     }
//
//     public RuleDefinitions.AdvantageType GetAdvantageOnOpportunityAttackOnMe(RulesetCharacter myself,
//         RulesetCharacter attacker, float distance)
//     {
//         return RuleDefinitions.AdvantageType.None;
//     }
//
//     public bool IsImmuneToOpportunityAttack(RulesetCharacter myself, [NotNull] RulesetCharacter attacker,
//         float distance)
//     {
//         return attacker.ConditionsByCategory.SelectMany(keyValuePair => keyValuePair.Value).Any(rulesetCondition =>
//             rulesetCondition.SourceGuid == myself.Guid &&
//             rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionName));
//     }
//
//     public void ComputeDefenseModifier(RulesetCharacter myself, RulesetCharacter attacker, int sustainedAttacks,
//         bool defenderAlreadyAttackedByAttackerThisTurn, ActionModifier attackModifier,
//         RuleDefinitions.FeatureOrigin featureOrigin)
//     {
//         // Intentionally empty?
//     }
// }
//
// [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
// internal class FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder
//     : FeatureDefinitionBuilder<FeatureDefinitionOpportunityAttackImmunityIfAttackerHasCondition,
//         FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder>
// {
//     protected FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder(string name,
//         Guid namespaceGuid) : base(name, namespaceGuid)
//     {
//     }
//
//     [NotNull]
//     public FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder SetConditionName(
//         string conditionName)
//     {
//         Definition.ConditionName = conditionName;
//         return this;
//     }
// }



