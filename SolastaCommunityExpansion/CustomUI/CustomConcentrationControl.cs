using SolastaCommunityExpansion.Api.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.CustomUI;

public interface ICusomConcentrationProvider
{
    string Name { get; }
    string Tooltip { get; }
    AssetReferenceSprite Icon { get; }
    void Stop(RulesetCharacter character);
}

public class CustomConcentrationControl : MonoBehaviour
{
    public static CustomConcentrationControl Setup(ICusomConcentrationProvider provider, RulesetCharacter character,
        GameObject prefab, Transform parent)
    {
        CustomConcentrationControl control;

        var name = $"CustomConcentration({provider.Name})";
        var child = parent.Find(name);

        if (child != null)
        {
            control = child.GetComponent<CustomConcentrationControl>();
        }
        else
        {
            var obj = Instantiate(prefab, parent, false);
            obj.name = name;
            control = obj.AddComponent<CustomConcentrationControl>();

            control.Setup(provider, character);
        }

        control.UpdateState(provider, character);

        return control;
    }


    private void Setup(ICusomConcentrationProvider provider, RulesetCharacter character)
    {
        var image = transform.Find("ConcentrationImage").GetComponent<Image>();
        if (image != null)
        {
            image.sprite = null;
            image.SetupSprite(provider.Icon);
        }

        GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
    }

    private void UpdateState(ICusomConcentrationProvider provider, RulesetCharacter character)
    {
        gameObject.SetActive(true); //Do we need ability to set to inactive on update?

        var tooltip = GetComponent<GuiTooltip>();
        if (tooltip != null)
        {
            tooltip.Content = provider.Tooltip;
        }

        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => provider.Stop(character));
    }
}
