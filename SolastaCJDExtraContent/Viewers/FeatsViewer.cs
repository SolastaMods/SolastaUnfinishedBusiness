using UnityModManagerNet;
using ModKit;
using System;
using System.Collections.Generic;
using SolastaCJDExtraContent.Models;

namespace SolastaCJDExtraContent.Viewers
{
    public class FeatsViewer : IMenuSelectablePage
    {
        public string Name => "Enable or Disable Feats";

        public int Priority => 0;



        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("A game restart is needed to see results of changing feat enabled state.");
            List<Action> actions = new List<Action>();
            foreach (string key in Models.FeatsContext.Feats.Keys)
            {
                AddUIForFeat(key, actions);
            }
            UI.HStack(null, 4, actions.ToArray());
        }

        private void AddUIForFeat(string key, List<Action> actions)
        {
            actions.Add(
                () =>
                {
                    bool toggle = !Main.Settings.FeatHidden.Contains(key);
                    if (UI.Toggle(key, ref toggle, 0, UI.AutoWidth()))
                    {
                        FeatsContext.Switch(key, toggle);
                    }
                }
            );
        }
    }
}

