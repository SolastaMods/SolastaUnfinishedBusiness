using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes.Inventor;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class InvocationPoolTypeCustom
{
    private static readonly List<InvocationPoolTypeCustom> PrivatePools = new();

    private readonly Dictionary<int, List<InvocationDefinitionCustom>> privateFeaturesByLevel = new();

    private InvocationPoolTypeCustom()
    {
    }

    internal string Name { get; private set; }

    /**Are level requirements in character levels or class levels?*/
    internal string RequireClassLevels { get; private set; }

    internal AssetReferenceSprite Sprite { get; private set; }

    public bool Hidden { get; private set; }

    internal Id MainActionId { get; private set; } = Id.CastInvocation;
    internal Id BonusActionId { get; private set; } = (Id)ExtraActionId.CastInvocationBonus;
    internal Id NoCostActionId { get; private set; } = (Id)ExtraActionId.CastInvocationNoCost;

    internal List<int> AllLevels { get; } = new();
    private List<InvocationDefinitionCustom> AllFeatures { get; } = new();

    internal string PanelTitle => $"Screen/&InvocationPool{Name}Header";

    private static InvocationPoolTypeCustom Register(
        string name,
        AssetReferenceSprite sprite = null,
        string requireClassLevel = null,
        bool hidden = false,
        Id main = Id.CastInvocation,
        Id bonus = (Id)ExtraActionId.CastInvocationBonus,
        Id noCost = (Id)ExtraActionId.CastInvocationNoCost)
    {
        var pool = new InvocationPoolTypeCustom
        {
            Name = name,
            Sprite = sprite,
            RequireClassLevels = requireClassLevel,
            Hidden = hidden,
            MainActionId = main,
            BonusActionId = bonus,
            NoCostActionId = noCost
        };
        PrivatePools.Add(pool);

        return pool;
    }

    internal static void RefreshAll()
    {
        var invocations = DatabaseRepository.GetDatabase<InvocationDefinition>()
            .OfType<InvocationDefinitionCustom>()
            .ToList();

        foreach (var pool in PrivatePools)
        {
            pool.Refresh(invocations);
        }
    }

    private string GuiName(bool unlearn)
    {
        return $"InvocationPool{Name}{(unlearn ? "Unlearn" : "Learn")}";
    }

    internal string FormatDescription(bool unlearn)
    {
        return Gui.Localize(GuiPresentationBuilder.CreateDescriptionKey(GuiName(unlearn), Category.Feature));
    }

    internal string FormatTitle(bool unlearn)
    {
        return Gui.Localize(GuiPresentationBuilder.CreateTitleKey(GuiName(unlearn), Category.Feature));
    }

    internal List<InvocationDefinitionCustom> GetLevelFeatures(int level)
    {
        //TODO: decide if we want to wrap this into new list, to be sure this one is immutable
        return (privateFeaturesByLevel.TryGetValue(level, out var result) ? result : null)
               ?? new List<InvocationDefinitionCustom>();
    }

    private void Refresh(IEnumerable<InvocationDefinitionCustom> invocations)
    {
        privateFeaturesByLevel.Clear();
        AllFeatures.SetRange(invocations.Where(d => d.PoolType == this));
        AllFeatures.ForEach(f => GetOrMakeLevelFeatures(f.requiredLevel).Add(f));
        AllLevels.SetRange(privateFeaturesByLevel.Select(e => e.Key));
        AllLevels.Sort();
    }

    private List<InvocationDefinitionCustom> GetOrMakeLevelFeatures(int level)
    {
        List<InvocationDefinitionCustom> levelFeatures;
        if (!privateFeaturesByLevel.ContainsKey(level))
        {
            levelFeatures = new List<InvocationDefinitionCustom>();
            privateFeaturesByLevel.Add(level, levelFeatures);
        }
        else
        {
            levelFeatures = privateFeaturesByLevel[level];
        }

        return levelFeatures;
    }

    internal static class Pools
    {
        internal static readonly InvocationPoolTypeCustom ArmamentAdroitness =
            Register("ArmamentAdroitness", CharacterClassDefinitions.Fighter.ClassPictogramReference, hidden: true);

        internal static readonly InvocationPoolTypeCustom Infusion =
            Register("Infusion", InventorClass.Pictogram, InventorClass.ClassName,
                main: (Id)ExtraActionId.InventorInfusion);

        internal static readonly InvocationPoolTypeCustom Gambit =
            //TODO: add proper sprite
            Register("Gambit", InventorClass.Pictogram, CharacterClassDefinitions.Fighter.Name,
                main: (Id)ExtraActionId.TacticianGambitMain,
                bonus: (Id)ExtraActionId.TacticianGambitBonus,
                noCost: (Id)ExtraActionId.TacticianGambitNoCost);

        internal static readonly InvocationPoolTypeCustom MartialWeaponMaster =
            Register("MartialWeaponMaster", CharacterClassDefinitions.Fighter.ClassPictogramReference, hidden: true);

        internal static readonly InvocationPoolTypeCustom PlaneMagic =
            Register("PlaneMagic",
                hidden: true,
                main: (Id)ExtraActionId.CastPlaneMagicMain,
                bonus: (Id)ExtraActionId.CastPlaneMagicBonus);

        internal static IEnumerable<InvocationPoolTypeCustom> All => PrivatePools;
    }
}
