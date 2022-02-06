using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Utils
{
    // Loosely based on https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
    internal static class CustomIcons
    {
        const string CUSTOM_ICON_PREFIX = "CUSTOM_ICON_PREFIX_";

        internal static readonly Dictionary<string, Sprite> LoadedIcons = new();

        internal static Sprite ImageToSprite(string filePath, int sizeX, int sizeY)
        {
            var bytes = File.ReadAllBytes(filePath);
            var texture = new Texture2D(sizeX, sizeY, TextureFormat.DXT5, false);

            texture.LoadImage(bytes);

            return Sprite.Create(texture, new Rect(0, 0, sizeX, sizeY), new Vector2(0, 0));
        }

        internal static Texture2D TextureFromSprite(Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                var newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                var newColors = sprite.texture.GetPixels(
                    (int)sprite.textureRect.x,
                    (int)sprite.textureRect.y,
                    (int)sprite.textureRect.width,
                    (int)sprite.textureRect.height);

                newText.SetPixels(newColors);
                newText.Apply();

                return newText;
            }
            else
            {
                return sprite.texture;
            }
        }

        internal static Texture2D DuplicateTexture(Texture2D source)
        {
            var renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            return readableText;
        }

        internal static void SaveTextureAsPNG(Texture2D texture, string path)
        {
            var bytes = DuplicateTexture(texture).EncodeToPNG();

            File.WriteAllBytes(path, bytes);
        }

        internal static void SaveSpriteFromAssetReferenceAsPNG(AssetReferenceSprite sprite_reference, string path)
        {
            try
            {
                var sprite = Gui.LoadAssetSync<Sprite>(sprite_reference);
                var texture = TextureFromSprite(sprite);

                SaveTextureAsPNG(texture, path);
            }
            catch (Exception e)
            {
                Main.Logger.Log(e.ToString());
            }
        }

        //stacks horizontaly images from files from left to right, resizes resulting image to final scale and stores it to a file
        internal static void CombineImages(string[] files, (int, int) finalScale, string finalImageFilename)
        {
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;

                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image, new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
                finalImage = new System.Drawing.Bitmap(finalImage, new System.Drawing.Size(finalScale.Item1, finalScale.Item2));
                finalImage.Save(finalImageFilename);
            }
            catch (Exception)
            {
                finalImage?.Dispose();

                throw;
            }
            finally
            {
                images.ForEach(x => x.Dispose());
            }
        }

        //puts scaled inner image into specified postion of base image and stores it to a file
        internal static void Merge2Images(
            string baseImageFile,
            string innerImageFile,
            (int, int) innerImageScale,
            (int, int) innerImagePosition,
            string finalImageFilename)
        {
            System.Drawing.Bitmap base_image = null;
            System.Drawing.Bitmap inner_image = null;

            try
            {
                base_image = new System.Drawing.Bitmap(baseImageFile);
                inner_image = new System.Drawing.Bitmap(new System.Drawing.Bitmap(innerImageFile), new System.Drawing.Size(innerImageScale.Item1, innerImageScale.Item2));

                var src_region = new System.Drawing.Rectangle(0, 0, innerImageScale.Item1, innerImageScale.Item2);
                var dst_region = new System.Drawing.Rectangle(innerImagePosition.Item1, innerImagePosition.Item2, innerImageScale.Item1, innerImageScale.Item2);
                using (System.Drawing.Graphics grD = System.Drawing.Graphics.FromImage(base_image))
                {
                    grD.DrawImage(inner_image, dst_region, src_region, System.Drawing.GraphicsUnit.Pixel);
                }

                base_image.Save(finalImageFilename);
            }
            finally
            {
                base_image?.Dispose();
                inner_image?.Dispose();
            }
        }

        internal static AssetReferenceSprite StoreCustomIcon(string name, string filePath, int sizeX, int sizeY)
        {
            var sprite = ImageToSprite(filePath, sizeX, sizeY);

            LoadedIcons.Add(CUSTOM_ICON_PREFIX + name, sprite);

            return new AssetReferenceSprite(CUSTOM_ICON_PREFIX + name);
        }

        internal static Sprite LoadStoredCustomIcon(string guid)
        {
            if (!LoadedIcons.ContainsKey(guid))
            {
                return null;
            }

            return LoadedIcons[guid];
        }

        internal static bool IsCustomIcon(Sprite sprite)
        {
            if (sprite == null)
            {
                return false;
            }

            return LoadedIcons.ContainsValue(sprite);
        }
    }
}
