using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.DataViewer
{
    public class BlueprintLoader : MonoBehaviour
    {
        public delegate void LoadBlueprintsCallback(IEnumerable<BaseDefinition> blueprints);

        private LoadBlueprintsCallback callback;

        private static BlueprintLoader _shared;

        public static BlueprintLoader Shared
        {
            get
            {
                if (_shared == null)
                {
                    _shared = new GameObject().AddComponent<BlueprintLoader>();
                    DontDestroyOnLoad(_shared.gameObject);
                }
                return _shared;
            }
        }

        public float Progress { get; set; }

        private IEnumerator coroutine;

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
            var databases = (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null);
            int loaded = 0;
            int total = databases.Count;

            // iterate over all DBs / BPs and collect them
            var blueprints = new List<BaseDefinition>();
            foreach (IEnumerable<BaseDefinition> db in databases.Values.OrderBy(db => db.GetType().GetGenericArguments()[0].Name))
            {
                yield return null;
                loaded++;
                var items = 0;
                foreach (BaseDefinition baseDefinition in db.OrderBy(def => def.Name))
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
            Main.Log($"loaded {blueprints.Count} blueprints");
            callback(blueprints);
            yield return null;
            StopCoroutine(coroutine);
            coroutine = null;
        }

        public void Load(LoadBlueprintsCallback callback)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            this.callback = callback;
            coroutine = LoadBlueprints();
            StartCoroutine(coroutine);
        }

        public bool LoadInProgress()
        {
            return coroutine != null;
        }
    }
}
