using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IForceLightingState
{
    [UsedImplicitly]
    LocationDefinitions.LightingState GetLightingState(
        GameLocationCharacter gameLocationCharacter,
        LocationDefinitions.LightingState lightingState);
}
