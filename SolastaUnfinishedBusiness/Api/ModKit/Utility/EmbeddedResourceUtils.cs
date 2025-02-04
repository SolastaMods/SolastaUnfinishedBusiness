﻿#if false
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SolastaUnfinishedBusiness.Api.ModKit;

public static class EmbeddedResourceUtils
{
    public static Stream StreamForResourceFile(string endingFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var manifestResourceNames = assembly.GetManifestResourceNames();

        foreach (var resourceName in manifestResourceNames)
        {
            var fileNameFromResourceName = _GetFileNameFromResourceName(resourceName);
            if (!fileNameFromResourceName.EndsWith(endingFileName))
            {
                continue;
            }

            using var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            if (manifestResourceStream == null)
            {
                continue;
            }

            return manifestResourceStream;
        }

        return null;
    }

    // https://stackoverflow.com/a/32176198/3764804
    private static string _GetFileNameFromResourceName(string resourceName)
    {
        var stringBuilder = new StringBuilder();
        var escapeDot = false;
        var haveExtension = false;

        for (var resourceNameIndex = resourceName.Length - 1;
             resourceNameIndex >= 0;
             resourceNameIndex--)
        {
            switch (resourceName[resourceNameIndex])
            {
                case '_':
                    escapeDot = true;
                    continue;
                case '.':
                {
                    if (!escapeDot)
                    {
                        if (haveExtension)
                        {
                            stringBuilder.Append('\\');
                            continue;
                        }

                        haveExtension = true;
                    }

                    break;
                }
                default:
                    escapeDot = false;
                    break;
            }

            stringBuilder.Append(resourceName[resourceNameIndex]);
        }

        var fileName = Path.GetDirectoryName(stringBuilder.ToString());
        return fileName == null ? null : new string(fileName.Reverse().ToArray());
    }
}
#endif
