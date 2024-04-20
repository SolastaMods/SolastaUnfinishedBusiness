using SolastaUnfinishedBusiness.Api.GameExtensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class CustomConcentrationControl : MonoBehaviour
{
    internal static void Setup(
        ICustomConcentrationProvider provider,
        RulesetCharacter character,
        GameObject prefab, Transform parent)
    {
        CustomConcentrationControl control;

        var name = $"CustomConcentration({provider.Name})";
        var child = parent.Find(name);

        if (child)
        {
            control = child.GetComponent<CustomConcentrationControl>();
        }
        else
        {
            var obj = Instantiate(prefab, parent, false);

            obj.name = name;
            control = obj.AddComponent<CustomConcentrationControl>();
            control.Setup(provider);
        }

        control.UpdateState(provider, character);
    }


    private void Setup(ICustomConcentrationProvider provider)
    {
        var image = transform.Find("ConcentrationImage").GetComponent<Image>();

        if (image)
        {
            image.sprite = null;
            image.SetupSprite(provider.Icon);
        }

        GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
    }

    private void UpdateState(ICustomConcentrationProvider provider, RulesetCharacter character)
    {
        gameObject.SetActive(true); //Do we need ability to set to inactive on update?

        var tooltip = GetComponent<GuiTooltip>();

        if (tooltip)
        {
            tooltip.Content = provider.Tooltip;
        }

        var button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => provider.Stop(character));
    }

    internal interface ICustomConcentrationProvider
    {
        public string Name { get; }
        public string Tooltip { get; }
        public AssetReferenceSprite Icon { get; }
        public void Stop(RulesetCharacter character);
    }
}
