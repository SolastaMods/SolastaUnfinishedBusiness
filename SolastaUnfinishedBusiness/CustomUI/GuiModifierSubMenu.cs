using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolastaUnfinishedBusiness.CustomUI;

public class GuiModifierSubMenu : GuiModifier
{
    private readonly List<Vector3> itemPositions = new();
    private RectTransform header;
    private Vector3 headerPosition;
    private Vector2 headerSize;

    public void Init()
    {
        header = transform.GetChild(0).GetComponent<RectTransform>();
        headerSize = header.sizeDelta;
        headerPosition = header.position;
        for (var i = 1; i < transform.childCount; i++)
        {
            itemPositions.Add(transform.GetChild(i).GetComponent<RectTransform>().position);
        }
    }

    public override void InterpolateAndApply(float ratio)
    {
        var headerWidth = Mathf.Lerp(headerSize.x, headerSize.x * 1.2f, Math.Min(1.5f * ratio, 1));
        header.sizeDelta = new Vector2(headerWidth, headerSize.y);
        header.position = headerPosition + new Vector3((headerSize.x - headerWidth) / 2, 0, 0);

        var num = transform.childCount;
        for (var i = 1; i < transform.childCount; i++)
        {
            var r = Math.Min(ratio * num / i, 1);
            var rect = transform.GetChild(i).GetComponent<RectTransform>();
            var pos = itemPositions[i - 1];

            rect.position = new Vector3(pos.x, Mathf.Lerp(headerPosition.y, pos.y, r), 0);
        }
    }


    public void Clean()
    {
        itemPositions.Clear();
    }
}
