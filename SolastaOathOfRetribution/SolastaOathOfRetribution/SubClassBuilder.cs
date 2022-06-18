// Decompiled with JetBrains decompiler
// Type: SolastaModApi.SubClassBuilder
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using HarmonyLib;
using UnityEngine;

namespace SolastaOathOfRetribution
{
  internal class SubClassBuilder
  {
    private CharacterSubclassDefinition MyClass;

    public SubClassBuilder() => this.MyClass = ScriptableObject.CreateInstance<CharacterSubclassDefinition>();

    public void SetName(string name)
    {
      Traverse.Create((object) this.MyClass).Field(nameof (name)).SetValue((object) name);
      this.MyClass.name = name;
      Traverse.Create((object) this.MyClass).Field("guid").SetValue((object) GuidHelper.Create(Main.ModGuidNamespace, name).ToString());
    }

    public void SetGuiPresentation(GuiPresentation gui) => Traverse.Create((object) this.MyClass).Field("guiPresentation").SetValue((object) gui);

    // public void AddPersonality(PersonalityFlagDefinition personalityType, int weight)
    // {
    //   PersonalityFlagOccurence root = new PersonalityFlagOccurence(DatabaseHelper.PersonalityFlagDefinitions.Normal);
    //   Traverse.Create((object) root).Field(nameof (weight)).SetValue((object) weight);
    //   Traverse.Create((object) root).Field("personalityFlag").SetValue((object) personalityType.Name);
    //   this.MyClass.PersonalityFlagOccurences.Add(root);
    // }

    public void AddFeatureAtLevel(FeatureDefinition feature, int level) => this.MyClass.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));

    public CharacterSubclassDefinition AddToDB()
    {
      DatabaseRepository.GetDatabase<CharacterSubclassDefinition>().Add(this.MyClass);
      return this.MyClass;
    }
  }
}
