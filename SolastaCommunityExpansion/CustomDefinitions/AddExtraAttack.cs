using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.Classes.Monk;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class AddExtraUnarmedAttack : IAddExtraAttack
    {
        private readonly ActionDefinitions.ActionType actionType;
        private readonly int attacksNumber;
        private readonly bool clearSameType;
        private readonly CharacterValidator[] validators;
        private readonly List<string> additionalTags = new();

        public AddExtraUnarmedAttack(ActionDefinitions.ActionType actionType, int attacksNumber, bool clearSameType,
            params CharacterValidator[] validators)
        {
            this.actionType = actionType;
            this.attacksNumber = attacksNumber;
            this.clearSameType = clearSameType;
            this.validators = validators;
        }

        public AddExtraUnarmedAttack(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) :
            this(actionType, 1, false, validators)
        {
        }

        public AddExtraUnarmedAttack SetTags(params string[] tags)
        {
            additionalTags.AddRange(tags);
            return this;
        }

        public void TryAddExtraAttack(RulesetCharacterHero hero)
        {
            if (!hero.IsValid(validators))
            {
                return;
            }

            var strikeDefinition = hero.UnarmedStrikeDefinition;

            var attackModifiers = hero.GetField<List<IAttackModificationProvider>>("attackModifiers");

            var attackModes = hero.AttackModes;
            if (clearSameType)
            {
                for (var i = attackModes.Count - 1; i > 0; i--)
                {
                    var mode = attackModes[i];
                    if (mode.ActionType == actionType)
                    {
                        RulesetAttackMode.AttackModesPool.Return(mode);
                        attackModes.RemoveAt(i);
                    }
                }
            }

            var attackMode = hero.RefreshAttackModePublic(
                actionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                false,
                true,
                EquipmentDefinitions.SlotTypeOffHand,
                attackModifiers,
                hero.FeaturesOrigin,
                null
            );
            attackMode.AttacksNumber = attacksNumber;
            attackMode.AttackTags.AddRange(additionalTags);

            attackModes.Add(attackMode);
        }
    }
}