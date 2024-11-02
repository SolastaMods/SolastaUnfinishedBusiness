using HarmonyLib;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;

namespace SolastaUnfinishedBusiness.Models;

internal static class ClassesContext
{
    internal static void Load()
    {
        InventorClass.Build();

        DatabaseRepository.GetDatabase<CharacterClassDefinition>()
            .Do(x => x.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock));
    }
}
