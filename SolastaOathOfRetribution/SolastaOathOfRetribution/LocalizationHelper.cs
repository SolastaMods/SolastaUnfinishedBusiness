// Decompiled with JetBrains decompiler
// Type: SolastaModApi.LocalizationHelper
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using I2.Loc;

namespace SolastaOathOfRetribution
{
  internal class LocalizationHelper
  {
    public static string AddString(string key, string translation)
    {
      LanguageSourceData source = LocalizationManager.Sources[0];
      source.AddTerm(key).Languages[source.GetLanguageIndex("English")] = translation;
      return key;
    }
  }
}
