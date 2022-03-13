using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Graphics = System.Drawing.Graphics;
using UnityGraphics = UnityEngine.Graphics;

namespace SolastaCommunityExpansion.Utils
{
    // Loosely based on https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
    internal static class CustomIcons
    {
        internal static readonly Dictionary<string, Sprite> SpriteCache = new();

        #region Helpers
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

            UnityGraphics.Blit(source, renderTex);
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
        internal static void CombineImages(IEnumerable<string> files, (int, int) finalScale, string finalImageFilename)
        {
            List<Bitmap> images = new List<Bitmap>();

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    Bitmap bitmap = new Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                using var finalImage = new Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;

                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                using var finalImage2 = new Bitmap(finalImage, new Size(finalScale.Item1, finalScale.Item2));
                finalImage2.Save(finalImageFilename);
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
            using var baseImage = new Bitmap(baseImageFile);
            using var originalImage = new Bitmap(innerImageFile);
            using var innerImage = new Bitmap(originalImage, new Size(innerImageScale.Item1, innerImageScale.Item2));

            var sourceRegion = new Rectangle(0, 0, innerImageScale.Item1, innerImageScale.Item2);
            var destinationRegion = new Rectangle(innerImagePosition.Item1, innerImagePosition.Item2, innerImageScale.Item1, innerImageScale.Item2);

            using (Graphics g = Graphics.FromImage(baseImage))
            {
                g.DrawImage(innerImage, destinationRegion, sourceRegion, GraphicsUnit.Pixel);
            }

            baseImage.Save(finalImageFilename);
        }
        #endregion

        /// <summary>
        /// Convert a bitmap stored as an embedded resource to a Sprite.
        /// </summary>
        internal static Sprite GetOrCreateSprite(string name, Bitmap bitmap, int size, bool throwIfAlreadyExists = false)
        {
            return GetOrCreateSprite(name, bitmap, size, size, throwIfAlreadyExists);
        }

        internal static Sprite GetOrCreateSprite(string name, Bitmap bitmap, int sizex, int sizey, bool throwIfAlreadyExists = false)
        {
            var ids = GetSpriteId(name, sizex, sizey);
            var id = ids.id;

            if (SpriteCache.TryGetValue(id, out var sprite))
            {
                if (throwIfAlreadyExists)
                {
                    throw new SolastaModApiException($"A sprite with name {name} and size [{sizex},{sizey}] already exists.");
                }
                else
                {
                    return sprite;
                }
            }

            var texture = new Texture2D(sizex, sizey, TextureFormat.DXT5, false);
            texture.LoadImage((byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])));
            sprite = Sprite.Create(texture, new Rect(0, 0, sizex, sizey), new Vector2(0, 0));
            SpriteCache[id] = sprite;

            sprite.name = id;

            Main.Log($"Created sprite, id={id}, guid={ids.guid}.");

            return sprite;
        }

        internal static Sprite GetSpriteById(string id)
        {
            return SpriteCache.TryGetValue(id, out var sprite) ? sprite : null;
        }

        /// <summary>
        /// Create a unique Id to serve as id of a sprite in our internal cache and as the guid for AssetReference
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static (string id, string guid) GetSpriteId(string name, int x, int y)
        {
            var id = $"_CE_{name}[{x},{y}]";

            return (id, GuidHelper.Create(DefinitionBuilder.CENamespaceGuid, id).ToString("n"));
        }

        internal static AssetReferenceSprite CreateAssetReferenceSprite(string name, Bitmap bitmap, int size)
        {
            var sprite = GetOrCreateSprite(name, bitmap, size);
            return new CEAssetReferenceSprite(sprite);
        }

        class CEAssetReferenceSprite : AssetReferenceSprite
        {
            public CEAssetReferenceSprite(Sprite sprite) : base(sprite.name)
            {
            }

            // Need to make runtime key a guid in order to eliminate CEAssetReferenceSprite
            public override bool RuntimeKeyIsValid()
            {
                return true;
            }
        }
    }
}
