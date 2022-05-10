namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IChainMagicEffect
    {
        public CharacterActionMagicEffect GetNextMagicEffect(CharacterActionMagicEffect baseEffect, CharacterActionAttack triggeredAttack);
    }
}
