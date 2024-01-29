using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

internal sealed class StopPowerConcentrationProvider : ICustomConcentrationProvider
{
    internal FeatureDefinitionPower StopPower;

    internal StopPowerConcentrationProvider(string name, string tooltip, AssetReferenceSprite icon)
    {
        Name = name;
        Tooltip = tooltip;
        Icon = icon;
    }

    public string Name { get; }
    public string Tooltip { get; }
    public AssetReferenceSprite Icon { get; }

    public void Stop(RulesetCharacter character)
    {
        if (StopPower == null)
        {
            return;
        }

        var locationCharacter = GameLocationCharacter.GetFromActor(character);

        if (locationCharacter == null)
        {
            return;
        }

        var implementationManagerService =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

        var usablePower = PowerProvider.Get(StopPower, character);
        var actionParams = new CharacterActionParams(locationCharacter, ActionDefinitions.Id.PowerNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            //CHECK: no need for AddAsActivePowerToSource
            RulesetEffect = implementationManagerService.MyInstantiateEffectPower(character, usablePower, true),
            UsablePower = usablePower,
            TargetCharacters = { locationCharacter },
            SkipAnimationsAndVFX = true
        };

        ServiceRepository.GetService<ICommandService>()
            .ExecuteAction(actionParams, _ => { }, false);
    }
}
