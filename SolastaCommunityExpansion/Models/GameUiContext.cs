using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
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

        public const InputCommands.Id CTRL_C = (InputCommands.Id)44440000;
        public const InputCommands.Id CTRL_L = (InputCommands.Id)44440001;
        public const InputCommands.Id CTRL_M = (InputCommands.Id)44440002;
        public const InputCommands.Id CTRL_P = (InputCommands.Id)44440003;

        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(CTRL_C, (int)KeyCode.C, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(CTRL_L, (int)KeyCode.L, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(CTRL_M, (int)KeyCode.M, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(CTRL_P, (int)KeyCode.P, (int)KeyCode.LeftControl, -1, -1, -1, -1);
        }

        internal static class RemoveInvalidFilenameChars
        {
            private static readonly HashSet<char> InvalidFilenameChars = new HashSet<char>(Path.GetInvalidFileNameChars());

            public static bool Invoke(TMP_InputField textField)
            {
                if (textField != null)
                {
                    // Solasta original code disallows invalid filename chars and an additional list of illegal chars.
                    // We're disallowing invalid filename chars only.
                    // We're trimming whitespace from start only as per original method.
                    // This allows the users to create a name with spaces inside, but also allows trailing space.
                    textField.text = new string(
                        textField.text
                            .Where(n => !InvalidFilenameChars.Contains(n))
                            .ToArray()).TrimStart();

                    return false;
                }

                return true;
            }
        }
    }
}
