using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

#if false
internal class CustomPortraitPoolPower : ICustomPortraitPointPoolProvider
{
    private readonly FeatureDefinitionPower power;

    internal CustomPortraitPoolPower(FeatureDefinitionPower power, string name = null, string tooltip = null,
        AssetReferenceSprite icon = null)
    {
        this.power = power;
        Name = name ?? power.Name;
        Tooltip = tooltip ?? $"Tooltip/&CustomPortraitPool{Name}";
        Icon = icon ?? power.GuiPresentation.SpriteReference;
    }

    public string Name { get; }
    public string Tooltip { get; }
    public AssetReferenceSprite Icon { get; }

    public int GetPoints(RulesetCharacter character)
    {
        return character.GetRemainingPowerUses(power);
    }
}
#endif

internal class CustomPortraitPointPool : MonoBehaviour
{
    internal static void Setup(ICustomPortraitPointPoolProvider provider, RulesetCharacter character,
        GameObject prefab, Transform parent)
    {
        CustomPortraitPointPool pool;

        var name = $"CustomPool({provider.Name})";
        var child = parent.Find(name);

        if (child != null)
        {
            pool = child.GetComponent<CustomPortraitPointPool>();
        }
        else
        {
            var obj = Instantiate(prefab, parent, false);

            obj.name = name;
            pool = obj.AddComponent<CustomPortraitPointPool>();
            pool.Setup(provider);
        }

        pool.UpdateState(provider, character);
    }

    private void Setup(ICustomPortraitPointPoolProvider provider)
    {
        var image = transform.Find("SorceryPointsImage").GetComponent<Image>();

        if (image == null)
        {
            return;
        }

        image.sprite = null;
        image.SetupSprite(provider.Icon);
    }

    private void UpdateState(ICustomPortraitPointPoolProvider provider, RulesetCharacter character)
    {
        gameObject.SetActive(true); //Do we need ability to set to inactive on update?

        //yes, this label name has a typo
        // ReSharper disable once StringLiteralTypo
        var label = transform.Find("SorceyPointsLabel")?.GetComponent<GuiLabel>();

        if (label != null)
        {
            label.Text = provider.GetPoints(character);
        }

        var tooltip = GetComponent<GuiTooltip>();

        if (tooltip != null)
        {
            tooltip.Content = provider.Tooltip(character);
        }
    }
}
