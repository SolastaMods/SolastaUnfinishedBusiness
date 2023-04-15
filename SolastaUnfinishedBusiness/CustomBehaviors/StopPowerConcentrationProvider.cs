using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomValidators;

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

        var rules = ServiceRepository.GetService<IRulesetImplementationService>();
        var usable = UsablePowersProvider.Get(StopPower, character);
        var locationCharacter = GameLocationCharacter.GetFromActor(character);
        var actionParams = new CharacterActionParams(locationCharacter,
            ActionDefinitions.Id.PowerNoCost)
        {
            SkipAnimationsAndVFX = true,
            TargetCharacters = { locationCharacter },
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = rules.InstantiateEffectPower(character, usable, true)
        };

        ServiceRepository.GetService<ICommandService>()
            .ExecuteAction(actionParams, _ => { }, false);
    }
}
