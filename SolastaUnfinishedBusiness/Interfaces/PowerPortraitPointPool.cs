using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Interfaces;

[PublicAPI]
public class PowerPortraitPointPool(FeatureDefinitionPower power, AssetReferenceSprite icon)
    : ICustomPortraitPointPoolProvider
{
    public string Name { get; set; } = power.Name;
    public string TooltipFormat { get; set; } = $"PortraitPool{power.Name}PointsFormat";
    public AssetReferenceSprite Icon => icon;
    [CanBeNull] public IsCharacterValidHandler IsActiveHandler { get; set; }


    public bool IsActive(RulesetCharacter character) => IsActiveHandler == null || IsActiveHandler(character);

    public string Tooltip(RulesetCharacter character)
    {
        var max = character.GetMaxUsesForPool(power);
        var charges = character.GetRemainingPowerCharges(power);
        return TooltipFormat.Formatted(Category.Tooltip, charges, max);
    }

    public string GetPoints(RulesetCharacter character)
    {
        return $"{character.GetRemainingPowerCharges(power)}";
    }
}
