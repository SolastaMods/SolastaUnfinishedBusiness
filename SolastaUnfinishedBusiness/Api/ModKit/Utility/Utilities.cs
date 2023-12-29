// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.ModKit.Utility;

public static class Utilities
{
    [UsedImplicitly]
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
        TValue defaultValue = default)
    {
        if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); } // using C# 6

        if (key == null) { throw new ArgumentNullException(nameof(key)); } //  using C# 6

        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    [UsedImplicitly]
    private static object GetPropValue(this object obj, string name)
    {
        foreach (var part in name.Split('.'))
        {
            if (obj == null) { return null; }

            var type = obj.GetType();
            var info = type.GetProperty(part);
            if (info == null) { return null; }

            obj = info.GetValue(obj, null);
        }

        return obj;
    }

    [UsedImplicitly]
    public static T GetPropValue<T>(this object obj, string name)
    {
        var retValue = GetPropValue(obj, name);
        if (retValue == null) { return default; }

        // throws InvalidCastException if types are incompatible
        return (T)retValue;
    }

    [UsedImplicitly]
    public static object SetPropValue(this object obj, string name, object value)
    {
        var parts = name.Split('.');
        var final = parts.Last();

        foreach (var part in parts)
        {
            var type = obj.GetType();
            var info = type.GetProperty(part);
            if (info == null) { return null; }

            if (part == final)
            {
                info.SetValue(obj, value);
                return value;
            }

            obj = info.GetValue(obj, null);
        }

        return null;
    }

    // ReSharper disable once InconsistentNaming
    [UsedImplicitly]
    public static string StripHTML(this string s)
    {
        return Regex.Replace(s, "<.*?>", string.Empty);
    }

    [UsedImplicitly]
    public static string UnityRichTextToHtml(string s)
    {
        s = s.Replace("<color=", "<font color=");
        s = s.Replace("</color>", "</font>");
        s = s.Replace("<size=", "<size size=");
        s = s.Replace("</size>", "</font>");
        s += "<br/>";

        return s;
    }

    [UsedImplicitly]
    public static string MergeSpaces(this string str, bool trim = false)
    {
        if (str == null)
        {
            return null;
        }

        var stringBuilder = new StringBuilder(str.Length);

        var i = 0;
        foreach (var c in str)
        {
            if (c != ' ' || i == 0 || str[i - 1] != ' ')
            {
                stringBuilder.Append(c);
            }

            i++;
        }

        return trim ? stringBuilder.ToString().Trim() : stringBuilder.ToString();
    }

    [UsedImplicitly]
    public static string ReplaceLastOccurrence(this string source, string find, string replace)
    {
        var place = source.LastIndexOf(find, StringComparison.CurrentCulture);

        if (place == -1)
        {
            return source;
        }

        var result = source.Remove(place, find.Length).Insert(place, replace);
        return result;
    }

    [UsedImplicitly]
    public static string[] GetObjectInfo(object o)
    {
        var fields = Traverse.Create(o).Fields().Aggregate("", (current, field) => current + field + ", ");

        var methods = Traverse.Create(o).Methods().Aggregate("", (current, method) => current + method + ", ");

        var properties = Traverse.Create(o).Properties()
            .Aggregate("", (current, property) => current + property + ", ");

        return [fields, methods, properties];
    }

    [UsedImplicitly]
    public static string SubstringBetweenCharacters(this string input, char charFrom, char charTo)
    {
        var posFrom = input.IndexOf(charFrom);
        if (posFrom == -1) //if found char
        {
            return string.Empty;
        }

        var posTo = input.IndexOf(charTo, posFrom + 1);
        return posTo != -1
            ? //if found char
            input.Substring(posFrom + 1, posTo - posFrom - 1)
            : string.Empty;
    }

    [UsedImplicitly]
    public static string[] TrimCommonPrefix(this string[] values)
    {
        var prefix = string.Empty;
        int? resultLength = null;

        if (values == null)
        {
            return null;
        }

        switch (values.Length)
        {
            case > 1:
            {
                var min = values.Min(value => value.Length);
                for (var charIndex = 0; charIndex < min; charIndex++)
                {
                    for (var valueIndex = 1; valueIndex < values.Length; valueIndex++)
                    {
                        if (values[0][charIndex] == values[valueIndex][charIndex])
                        {
                            continue;
                        }

                        resultLength = charIndex;
                        break;
                    }

                    if (resultLength.HasValue)
                    {
                        break;
                    }
                }

                if (resultLength is > 0)
                {
                    prefix = values[0].Substring(0, resultLength.Value);
                }

                break;
            }
            case > 0:
                prefix = values[0];
                break;
        }

        return prefix.Length > 0 ? values.Select(s => s.Replace(prefix, "")).ToArray() : values;
    }

    [UsedImplicitly]
    public static Dictionary<string, TEnum> NameToValueDictionary<TEnum>(this TEnum enumValue) where TEnum : struct
    {
        var enumType = enumValue.GetType();
        return Enum.GetValues(enumType)
            .Cast<TEnum>()
            .ToDictionary(e => Enum.GetName(enumType, e), e => e);
    }

    [UsedImplicitly]
    public static Dictionary<TEnum, string> ValueToNameDictionary<TEnum>(this TEnum enumValue) where TEnum : struct
    {
        var enumType = enumValue.GetType();
        return Enum.GetValues(enumType)
            .Cast<TEnum>()
            .ToDictionary(e => e, e => Enum.GetName(enumType, e));
    }

    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static Dictionary<TK, TV> Filter<TK, TV>(this Dictionary<TK, TV> dict, Predicate<KeyValuePair<TK, TV>> pred)
    {
        return dict.Where(it => pred(it)).ToDictionary(it => it.Key, it => it.Value);
    }
}
