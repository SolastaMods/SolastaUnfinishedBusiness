using UnityModManagerNet;
using ModKit;
using System;
using System.Collections.Generic;
using SolastaContentExpansion.Models;

namespace SolastaContentExpansion.Viewers
{
    public class FeatsViewer : IMenuSelectablePage
    {
        public string Name => "Feats";

        public int Priority => 2;

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

        public void DisplayFeatsSettings()
        {
            UI.Label("");
            UI.Label("Settings: [game restart required]".yellow());
            List<Action> actions = new List<Action>();
            foreach (string key in Models.FeatsContext.Feats.Keys)
            {
                AddUIForFeat(key, actions);
            }
            UI.HStack(null, 4, actions.ToArray());
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());
            UI.Div();

            DisplayFeatsSettings();
        }
    }
}

