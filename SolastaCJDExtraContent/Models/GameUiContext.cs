using UnityEngine;

namespace SolastaCJDExtraContent.Models
{
    internal static class GameUiContext
    {
        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_C, (int)KeyCode.C, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_L, (int)KeyCode.L, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_M, (int)KeyCode.M, (int)KeyCode.LeftControl, -1, -1, -1, -1);
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_P, (int)KeyCode.P, (int)KeyCode.LeftControl, -1, -1, -1, -1);
        }
    }
}
