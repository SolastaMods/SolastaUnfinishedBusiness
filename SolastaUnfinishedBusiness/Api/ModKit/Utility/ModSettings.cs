using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using static UnityModManagerNet.UnityModManager;

namespace SolastaUnfinishedBusiness.Api.ModKit.Utility;

public interface IUpdatableSettings
{
    void AddMissingKeys(IUpdatableSettings from);
}

internal static class ModSettings
{
    public static void SaveSettings<T>(this ModEntry modEntry, string fileName, T settings)
    {
        var userConfigFolder = modEntry.Path + "UserSettings";
        Main.EnsureFolderExists(userConfigFolder);
        var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";
        File.WriteAllText(userPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
    }

    public static void LoadSettings<T>(this ModEntry modEntry, string fileName, ref T settings)
        where T : IUpdatableSettings, new()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var userConfigFolder = modEntry.Path + "UserSettings";
        Main.EnsureFolderExists(userConfigFolder);
        var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";
        try
        {
            foreach (var res in assembly.GetManifestResourceNames())
            {
                //Logger.Log("found resource: " + res);
                if (!res.Contains(fileName))
                {
                    continue;
                }

                var stream = assembly.GetManifestResourceStream(res);

                if (stream == null)
                {
                    continue;
                }

                using StreamReader reader = new(stream);
                var text = reader.ReadToEnd();
                //Logger.Log($"read: {text}");
                settings = JsonConvert.DeserializeObject<T>(text);

                //Logger.Log($"read settings: {string.Join(Environment.NewLine, settings)}");
            }
        }
        catch (Exception e)
        {
            Main.Error($"{fileName} resource is not present or is malformed. exception: {e}");
            settings = new T();
        }

        if (File.Exists(userPath))
        {
            using var reader = File.OpenText(userPath);
            try
            {
                var userSettings = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                userSettings.AddMissingKeys(settings);
                settings = userSettings;
            }
            catch
            {
                Main.Error("Failed to load user settings. Settings will be rebuilt.");
                try { File.Copy(userPath, userConfigFolder + $"{Path.DirectorySeparatorChar}BROKEN_{fileName}", true); }
                catch { Main.Error("Failed to archive broken settings."); }
            }
        }

        File.WriteAllText(userPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
    }
}
