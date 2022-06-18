// Decompiled with JetBrains decompiler
// Type: SolastaModApi.GuidHelper
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace SolastaOathOfRetribution
{
  public static class GuidHelper
  {
    public static Guid Create(Guid namespaceId, string name) => GuidHelper.Create(namespaceId, name, 5);

    public static Guid Create(Guid namespaceId, string name, int version)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (version != 3 && version != 5)
        throw new ArgumentOutOfRangeException(nameof (version), "version must be either 3 or 5.");
      byte[] bytes = Encoding.UTF8.GetBytes(name);
      byte[] byteArray = namespaceId.ToByteArray();
      GuidHelper.SwapByteOrder(byteArray);
      byte[] hash;
      using (HashAlgorithm hashAlgorithm = version == 3 ? (HashAlgorithm) MD5.Create() : (HashAlgorithm) SHA1.Create())
      {
        hashAlgorithm.TransformBlock(byteArray, 0, byteArray.Length, (byte[]) null, 0);
        hashAlgorithm.TransformFinalBlock(bytes, 0, bytes.Length);
        hash = hashAlgorithm.Hash;
      }
      byte[] numArray = new byte[16];
      Array.Copy((Array) hash, 0, (Array) numArray, 0, 16);
      numArray[6] = (byte) ((int) numArray[6] & 15 | version << 4);
      numArray[8] = (byte) ((int) numArray[8] & 63 | 128);
      GuidHelper.SwapByteOrder(numArray);
      return new Guid(numArray);
    }

    internal static void SwapByteOrder(byte[] guid)
    {
      GuidHelper.SwapBytes(guid, 0, 3);
      GuidHelper.SwapBytes(guid, 1, 2);
      GuidHelper.SwapBytes(guid, 4, 5);
      GuidHelper.SwapBytes(guid, 6, 7);
    }

    private static void SwapBytes(byte[] guid, int left, int right)
    {
      byte num = guid[left];
      guid[left] = guid[right];
      guid[right] = num;
    }
  }
}
