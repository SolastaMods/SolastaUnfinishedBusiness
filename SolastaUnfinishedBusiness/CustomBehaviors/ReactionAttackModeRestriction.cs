// using System.Linq;
// using JetBrains.Annotations;
// using SolastaUnfinishedBusiness.CustomInterfaces;
//
// namespace SolastaUnfinishedBusiness.CustomBehaviors;
//
// public sealed class ReactionAttackModeRestriction : IReactionAttackModeRestriction
// {
//     public static readonly ValidReactionModeHandler MeleeOnly = (mode, ranged, _, _) =>
//     {
//         if (ranged)
//         {
//             return false;
//         }
//
//         var item = mode.SourceDefinition as ItemDefinition;
//         if (item == null)
//         {
//             return false;
//         }
//
//         var weapon = item.WeaponDescription;
//         if (weapon == null)
//         {
//             return false;
//         }
//
//         return weapon.WeaponTypeDefinition.WeaponProximity == RuleDefinitions.AttackProximity.Melee;
//     };
//
//     private readonly ValidReactionModeHandler[] validators;
//
//     public ReactionAttackModeRestriction(params ValidReactionModeHandler[] validators)
//     {
//         this.validators = validators;
//     }
//
//     public bool ValidReactionMode(RulesetAttackMode attackMode, bool rangedAttack,
//         GameLocationCharacter character, GameLocationCharacter target)
//     {
//         return validators.All(v => v(attackMode, rangedAttack, character, target));
//     }
//
//     [NotNull]
//     public static ValidReactionModeHandler TargetHasNoCondition(ConditionDefinition condition)
//     {
//         return (_, _, _, target) =>
//         {
//             var rulesetCharacter = target.RulesetCharacter;
//             return rulesetCharacter != null && !rulesetCharacter.HasConditionOfType(condition.Name);
//         };
//     }
// }


