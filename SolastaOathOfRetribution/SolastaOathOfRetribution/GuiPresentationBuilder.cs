// Decompiled with JetBrains decompiler
// Type: SolastaModApi.GuiPresentationBuilder
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaOathOfRetribution
{
  internal class GuiPresentationBuilder
  {
    private GuiPresentation guiPresentation;

    public GuiPresentationBuilder(string description, string title)
    {
      this.guiPresentation = new GuiPresentation();
      this.guiPresentation.Description = description;
      this.guiPresentation.Title = title;
    }

    public void SetColor(Color color) => Traverse.Create((object) this.guiPresentation).Field(nameof (color)).SetValue((object) new Color(1f, 1f, 1f, 1f));

    public void SetSortOrder(int sortOrder) => Traverse.Create((object) this.guiPresentation).Field(nameof (sortOrder)).SetValue((object) 1);

    public void SetSpriteReference(AssetReferenceSprite sprite) => Traverse.Create((object) this.guiPresentation).Field("spriteReference").SetValue((object) sprite);

    public GuiPresentation Build() => this.guiPresentation;
  }
}
