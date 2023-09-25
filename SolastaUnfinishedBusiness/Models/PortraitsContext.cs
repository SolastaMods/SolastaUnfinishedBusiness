using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Models;

public static class PortraitsContext
{
    #region Custom Portraits Helpers

    private static readonly Dictionary<string, Texture2D> CustomHeroPortraits = new();
    private static readonly Dictionary<string, Texture2D> CustomMonsterPortraits = new();

    internal static readonly string PortraitsFolder = $"{Main.ModFolder}/Portraits";
    private static readonly string PreGenFolder = $"{PortraitsFolder}/PreGen";
    private static readonly string PersonalFolder = $"{PortraitsFolder}/Personal";
    private static readonly string MonstersFolder = $"{PortraitsFolder}/Monsters";

    internal static void Load()
    {
        if (!Directory.Exists(PortraitsFolder))
        {
            Directory.CreateDirectory(PortraitsFolder);
        }

        if (!Directory.Exists(PreGenFolder))
        {
            Directory.CreateDirectory(PreGenFolder);
        }

        if (!Directory.Exists(PersonalFolder))
        {
            Directory.CreateDirectory(PersonalFolder);
        }

        if (!Directory.Exists(MonstersFolder))
        {
            Directory.CreateDirectory(MonstersFolder);
        }
    }

    internal static bool HasCustomPortrait(RulesetCharacter rulesetCharacter)
    {
        var name = rulesetCharacter.Name;

        return (rulesetCharacter is RulesetCharacterHero && CustomHeroPortraits.ContainsKey(name))
               || (rulesetCharacter is RulesetCharacterMonster && CustomMonsterPortraits.ContainsKey(name));
    }

    internal static void ChangePortrait(GuiCharacter __instance, RawImage rawImage)
    {
        if (!Main.Settings.EnableCustomPortraits || ToolsContext.FunctorRespec.IsRespecing)
        {
            return;
        }

        if (__instance.RulesetCharacterMonster != null)
        {
            if (TryGetMonsterPortrait(__instance.Name, rawImage, out var texture))
            {
                rawImage.texture = texture;
            }
        }
        else if (__instance.BuiltIn)
        {
            if (TryGetPreGenHeroPortrait(__instance.Name, rawImage, out var texture))
            {
                rawImage.texture = texture;
            }
        }
        else
        {
            if (TryGetHeroPortrait(__instance.Name, rawImage, out var texture))
            {
                rawImage.texture = texture;
            }
        }
    }

    private static bool TryGetHeroPortrait(string name, RawImage original, out Texture2D texture)
    {
        var filename = $"{PersonalFolder}/{name}.png";

        return TryGetPortrait(CustomHeroPortraits, name, filename, original, out texture);
    }

    private static bool TryGetPreGenHeroPortrait(string name, RawImage original, out Texture2D texture)
    {
        var filename = $"{PreGenFolder}/{name}.png";

        return TryGetPortrait(CustomHeroPortraits, name, filename, original, out texture);
    }

    private static bool TryGetMonsterPortrait(string name, RawImage original, out Texture2D texture)
    {
        var filename = $"{MonstersFolder}/{name}.png";

        return TryGetPortrait(CustomMonsterPortraits, name, filename, original, out texture);
    }

    private static bool TryGetPortrait(
        IDictionary<string, Texture2D> dict, string name, string filename, RawImage original, out Texture2D texture)
    {
        if (dict.TryGetValue(name, out texture))
        {
            return true;
        }

        if (!File.Exists(filename))
        {
            return false;
        }

        var fileData = File.ReadAllBytes(filename);

        texture = new Texture2D(original.texture.width, original.texture.height, TextureFormat.ARGB32, false)
        {
            wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear
        };
        texture.LoadImage(fileData);
        dict.Add(name, texture);

        return true;
    }

    #endregion
}
