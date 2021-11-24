using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;

namespace SolastaCommunityExpansion.Models
{
    internal static class HeroNameContext
    {
        internal static readonly HashSet<char> InvalidFilenameChars = new HashSet<char>(Path.GetInvalidFileNameChars());

        internal static bool Invoke(TMP_InputField textField)
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
