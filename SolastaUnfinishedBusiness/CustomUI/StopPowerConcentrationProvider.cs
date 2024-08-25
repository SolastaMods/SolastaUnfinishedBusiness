using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

internal sealed class StopPowerConcentrationProvider : CustomConcentrationControl.ICustomConcentrationProvider
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
        if (!StopPower)
        {
            return;
        }

        var locationCharacter = GameLocationCharacter.GetFromActor(character);
        var usablePower = PowerProvider.Get(StopPower, character);

#if false
        var implementationManager =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;


        var actionParams = new CharacterActionParams(locationCharacter, ActionDefinitions.Id.PowerNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = implementationManager.MyInstantiateEffectPower(character, usablePower, true),
            UsablePower = usablePower,
            TargetCharacters = { locationCharacter },
            SkipAnimationsAndVFX = true
        };

        ServiceRepository.GetService<ICommandService>()
            .ExecuteAction(actionParams, _ => { }, false);
#endif

        locationCharacter.MyExecuteActionSpendPower(usablePower, locationCharacter);
    }
}
