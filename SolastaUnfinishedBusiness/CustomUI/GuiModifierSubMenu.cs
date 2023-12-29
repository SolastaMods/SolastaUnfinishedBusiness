using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class GuiModifierSubMenu : GuiModifier
{
    private readonly List<Vector3> _itemPositions = [];
    private Image _background;
    private RectTransform _featTable;
    private RectTransform _header;
    private Vector3 _headerPosition;
    private Vector2 _headerSize;
    private float _maxHeaderWidth;

    internal void Init(Image bkg, RectTransform table, float headerWidth)
    {
        _featTable = table;
        _background = bkg;

        var num = table.childCount;

        _header = table.GetChild(num - 1).GetComponent<RectTransform>();
        _headerSize = _header.sizeDelta;
        _headerPosition = _header.position;
        _maxHeaderWidth = headerWidth;

        for (var i = 0; i < table.childCount - 1; i++)
        {
            _itemPositions.Add(table.GetChild(i).GetComponent<RectTransform>().position);
        }
    }

    public override void InterpolateAndApply(float ratio)
    {
        var headerWidth = Mathf.Lerp(_headerSize.x, _maxHeaderWidth, (float)Math.Sqrt(ratio));

        _header.sizeDelta = new Vector2(headerWidth, _headerSize.y);
        _header.position = _headerPosition + new Vector3((_headerSize.x - headerWidth) / 2, 0, 0);

        var num = _featTable.childCount;

        for (var i = 0; i < _featTable.childCount - 1; i++)
        {
            var r = Math.Min(ratio * num / (num - i), 1);
            var rect = _featTable.GetChild(i).GetComponent<RectTransform>();
            var pos = _itemPositions[i];

            rect.position = new Vector3(pos.x, Mathf.Lerp(_headerPosition.y, pos.y, r), 0);
        }

        _background.color = new Color(0, 0, 0, 0.65f * ratio);
    }


    internal void Clean()
    {
        _itemPositions.Clear();
    }
}
