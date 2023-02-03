using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal delegate void TooltipModifier<in T>(GuiTooltip tooltip, Image img, Transform obj, T definition, object context)
    where T : BaseDefinition;
