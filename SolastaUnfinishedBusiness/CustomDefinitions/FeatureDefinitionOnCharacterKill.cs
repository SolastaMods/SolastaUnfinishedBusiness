using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionOnCharacterKill : FeatureDefinition, IOnCharacterKill
{
    private OnCharacterKillDelegate onCharacterKill;

    public void OnCharacterKill(GameLocationCharacter character)
    {
        onCharacterKill?.Invoke(character);
    }

    internal void SetOnCharacterKill(OnCharacterKillDelegate onCharacterKillSet)
    {
        onCharacterKill = onCharacterKillSet;
    }
}
