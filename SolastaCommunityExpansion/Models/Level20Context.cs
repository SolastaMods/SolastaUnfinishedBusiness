using SolastaCommunityExpansion.Level20.Classes;
using SolastaCommunityExpansion.Level20.SubClasses;

namespace SolastaCommunityExpansion.Models;

internal static class Level20Context
{
    public const int MaxSpellLevel = 9;

    public const int ModMaxLevel = 20;
    public const int GameMaxLevel = 12;

    public const int ModMaxExperience = 355000;
    public const int GameMaxExperience = 100000;

    internal static void Load()
    {
        //
        // should not be protected to avoid issues on MP or loading heroes
        //

        BarbarianBuilder.Load();
        ClericBuilder.Load();
        DruidBuilder.Load();
        FighterBuilder.Load();
        PaladinBuilder.Load();
        RangerBuilder.Load();
        RogueBuilder.Load();
        SorcererBuilder.Load();
        WizardBuilder.Load();

        MartialSpellBladeBuilder.Load();
        ShadowcasterBuilder.Load();
    }

    internal static void LateLoad()
    {
        Level20PatchingContext.Load();
    }
}
