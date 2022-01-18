using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.GadgetBlueprints;

namespace SolastaCommunityExpansion.Models
{
    internal static class GameUiContext
    {
        internal static readonly GadgetBlueprint[] GadgetBlueprintsWithGizmos = new GadgetBlueprint[]
        {
            Exit,
            ExitMultiple,
            TeleporterIndividual,
            TeleporterParty,
        };

        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(Hotkeys.CTRL_C, (int)KeyCode.C, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Hotkeys.CTRL_L, (int)KeyCode.L, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Hotkeys.CTRL_M, (int)KeyCode.M, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Hotkeys.CTRL_P, (int)KeyCode.P, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Hotkeys.CTRL_H, (int)KeyCode.H, (int)KeyCode.LeftControl, -1, -1, -1, -1);
        }
    }
}
