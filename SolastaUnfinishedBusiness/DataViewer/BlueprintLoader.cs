using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.DataViewer;

internal sealed class BlueprintLoader : MonoBehaviour
{
    private static BlueprintLoader _shared;

    private LoadBlueprintsCallback callback;

    private IEnumerator coroutine;

    [NotNull]
    internal static BlueprintLoader Shared
    {
        get
        {
            if (_shared != null)
            {
                return _shared;
            }

            _shared = new GameObject().AddComponent<BlueprintLoader>();
            DontDestroyOnLoad(_shared.gameObject);

            return _shared;
        }
    }

    internal float Progress { get; private set; }

    private void UpdateProgress(int loaded, int total)
    {
        if (total <= 0)
        {
            Progress = 0.0f;
            return;
        }

        Progress = loaded / (float)total;
    }

    private IEnumerator LoadBlueprints()
    {
        // safe access to the database
        var databases = DatabaseRepository.databases;
        var loaded = 0;
        var total = databases.Count;

        // iterate over all DBs / BPs and collect them
        var blueprints = new List<BaseDefinition>();
        foreach (IEnumerable<BaseDefinition> db in databases.Values.OrderBy(db =>
                     db.GetType().GetGenericArguments()[0].Name))
        {
            yield return null;

            loaded++;

            var items = 0;

            foreach (var baseDefinition in db.OrderBy(def => def.Name))
            {
                blueprints.Add(baseDefinition);
                UpdateProgress(loaded, total);
                items++;
                if (items % 100 == 0)
                {
                    yield return null;
                }
            }
        }

        // clean up
        // ReSharper disable once InvocationIsSkipped
        Main.Log($"loaded {blueprints.Count} blueprints");
        callback(blueprints);

        yield return null;

        StopCoroutine(coroutine);

        coroutine = null;
    }

    internal void Load(LoadBlueprintsCallback myCallback)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        callback = myCallback;
        coroutine = LoadBlueprints();
        StartCoroutine(coroutine);
    }

    internal bool LoadInProgress()
    {
        return coroutine != null;
    }

    internal delegate void LoadBlueprintsCallback(IEnumerable<BaseDefinition> blueprints);
}
