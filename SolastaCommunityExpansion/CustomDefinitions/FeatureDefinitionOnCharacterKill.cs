using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public sealed class FeatureDefinitionOnCharacterKill : FeatureDefinition, IOnCharacterKill
{
    private OnCharacterKillDelegate onCharacterKill;

    public void OnCharacterKill(
        GameLocationCharacter character,
        bool dropLoot,
        bool removeBody,
        bool forceRemove,
        bool considerDead,
        bool becomesDying)
    {
        onCharacterKill?.Invoke(character, dropLoot, removeBody, forceRemove, considerDead, becomesDying);
    }

    internal void SetOnCharacterKill(OnCharacterKillDelegate onCharacterKillSet)
    {
        onCharacterKill = onCharacterKillSet;
    }
}
