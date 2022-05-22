using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class PowerUseValidity : IPowerUseValidity
    {
        private readonly CharacterValidator[] validators;

        public PowerUseValidity(params CharacterValidator[] validators)
        {
            this.validators = validators;
        }

        public bool CanUsePower(RulesetCharacter character)
        {
            return character.IsValid(validators);
        }
    }
}
