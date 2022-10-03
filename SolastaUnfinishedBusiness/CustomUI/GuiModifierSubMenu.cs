using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class GuiModifierSubMenu : GuiModifier
{
    private readonly List<Vector3> itemPositions = new();
    private Image background;
    private RectTransform featTable;
    private RectTransform header;
    private Vector3 headerPosition;
    private Vector2 headerSize;

    internal void Init(Image bkg, RectTransform table)
    {
        featTable = table;
        background = bkg;

        var num = table.childCount;

        header = table.GetChild(num - 1).GetComponent<RectTransform>();
        headerSize = header.sizeDelta;
        headerPosition = header.position;

        for (var i = 0; i < table.childCount - 1; i++)
        {
            itemPositions.Add(table.GetChild(i).GetComponent<RectTransform>().position);
        }
    }

    public override void InterpolateAndApply(float ratio)
    {
        var headerWidth = Mathf.Lerp(headerSize.x, headerSize.x * 1.2f, Math.Min(1.5f * ratio, 1));

        header.sizeDelta = new Vector2(headerWidth, headerSize.y);
        header.position = headerPosition + new Vector3((headerSize.x - headerWidth) / 2, 0, 0);

        var num = featTable.childCount;

        for (var i = 0; i < featTable.childCount - 1; i++)
        {
            var r = Math.Min(ratio * num / (num - i), 1);
            var rect = featTable.GetChild(i).GetComponent<RectTransform>();
            var pos = itemPositions[i];

            rect.position = new Vector3(pos.x, Mathf.Lerp(headerPosition.y, pos.y, r), 0);
        }

        background.color = new Color(0, 0, 0, 0.65f * ratio);
    }


    internal void Clean()
    {
        itemPositions.Clear();
    }
}
