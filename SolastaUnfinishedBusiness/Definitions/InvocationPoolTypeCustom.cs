using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

// ReSharper disable once CheckNamespace
internal class InvocationPoolTypeCustom
{
    private static readonly List<InvocationPoolTypeCustom> PrivatePools = [];

    private readonly Dictionary<int, List<InvocationDefinitionCustom>> _privateFeaturesByLevel = new();

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

    internal List<int> AllLevels { get; } = [];
    private List<InvocationDefinitionCustom> AllFeatures { get; } = [];

    internal string PanelTitle { get; private set; }

    internal static int GetClassOrSubclassLevel(RulesetCharacterHero hero, string classOrSubclassName)
    {
        if (TryGetDefinition<CharacterClassDefinition>(classOrSubclassName, out var classDefinition) &&
            classDefinition)
        {
            return hero.GetClassLevel(classDefinition);
        }

        if (!TryGetDefinition<CharacterSubclassDefinition>(classOrSubclassName, out var subclassDefinition) ||
            !subclassDefinition)
        {
            return 0;
        }

        var classDefinitionFromSubclass = hero.ClassesAndSubclasses
            .FirstOrDefault(x => x.Value == subclassDefinition);

        return classDefinitionFromSubclass.Key
            // ReSharper disable once TailRecursiveCall
            ? GetClassOrSubclassLevel(hero, classDefinitionFromSubclass.Key.Name)
            : 0;
    }

    internal static string GetClassOrSubclassTitle(string classOrSubclassName)
    {
        if (TryGetDefinition<CharacterClassDefinition>(classOrSubclassName, out var classDefinition) &&
            classDefinition)
        {
            return classDefinition.FormatTitle();
        }

        if (TryGetDefinition<CharacterSubclassDefinition>(classOrSubclassName, out var subclassDefinition) &&
            subclassDefinition)
        {
            return subclassDefinition.FormatTitle();
        }

        return string.Empty;
    }

    private static InvocationPoolTypeCustom Register(
        string name,
        AssetReferenceSprite sprite = null,
        string requireClassLevel = null,
        bool hidden = false,
        string panelTitle = null,
        Id main = Id.CastInvocation,
        Id bonus = (Id)ExtraActionId.CastInvocationBonus,
        Id noCost = (Id)ExtraActionId.CastInvocationNoCost)
    {
        var pool = new InvocationPoolTypeCustom
        {
            Name = name,
            PanelTitle = panelTitle ?? $"Screen/&InvocationPool{name}Header",
            Sprite = sprite,
            RequireClassLevels = requireClassLevel,
            Hidden = hidden && string.IsNullOrEmpty(panelTitle),
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
            .ToArray();

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
        return (_privateFeaturesByLevel.TryGetValue(level, out var result) ? result : null)
               ?? [];
    }

    private void Refresh(IEnumerable<InvocationDefinitionCustom> invocations)
    {
        _privateFeaturesByLevel.Clear();
        AllFeatures.SetRange(invocations.Where(d => d.PoolType == this));
        AllFeatures.ForEach(f => GetOrMakeLevelFeatures(f.requiredLevel).Add(f));
        AllLevels.SetRange(_privateFeaturesByLevel.Select(e => e.Key));
        AllLevels.Sort();
    }

    private List<InvocationDefinitionCustom> GetOrMakeLevelFeatures(int level)
    {
        if (_privateFeaturesByLevel.TryGetValue(level, out var value))
        {
            return value;
        }

        value = [];
        _privateFeaturesByLevel.Add(level, value);

        return value;
    }

    internal static class Pools
    {
        internal static readonly InvocationPoolTypeCustom ArcaneShotChoice =
            Register("ArcaneShotChoice");

        internal static readonly InvocationPoolTypeCustom AngelicFormChoice =
            Register("AngelicFormChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom PathClawDraconicChoice =
            Register("PathClawDraconicChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom SorcererDraconicChoice =
            Register("SorcererDraconicChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom PathOfTheElementsElementalFuryChoiceChoice =
            Register("PathOfTheElementsElementalFuryChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom KindredSpiritChoice =
            Register("KindredSpiritChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom GrudgeBearerChoice =
            Register("FeatGrudgeBearerChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom WeaponMasterChoice =
            Register("FeatWeaponMasterChoice", hidden: true);

        internal static readonly InvocationPoolTypeCustom MartialWeaponMasterWeaponSpecialization =
            Register("MartialWeaponMaster", hidden: true);

        internal static readonly InvocationPoolTypeCustom MonkWeaponSpecialization =
            Register("MonkWeaponSpecialization", hidden: true);

        internal static readonly InvocationPoolTypeCustom WeaponMasterySpecialization =
            Register("WeaponMastery", hidden: true);

        internal static readonly InvocationPoolTypeCustom RangerTerrainTypeAffinity =
            Register("RangerTerrainTypeAffinity", panelTitle: "Feature/&RangerNaturalExplorerTitle", hidden: true);

        internal static readonly InvocationPoolTypeCustom RangerPreferredEnemy =
            Register("RangerPreferredEnemy", panelTitle: "Feature/&RangerFavoredEnemyTitle", hidden: true);

        internal static readonly InvocationPoolTypeCustom Infusion =
            Register("Infusion", requireClassLevel: InventorClass.ClassName,
                main: (Id)ExtraActionId.InventorInfusion);

        internal static readonly InvocationPoolTypeCustom Gambit =
            Register("Gambit", requireClassLevel: MartialTactician.Name,
                main: (Id)ExtraActionId.TacticianGambitMain,
                bonus: (Id)ExtraActionId.TacticianGambitBonus,
                noCost: (Id)ExtraActionId.TacticianGambitNoCost);

        internal static readonly InvocationPoolTypeCustom PlaneMagic =
            Register("PlaneMagic",
                main: (Id)ExtraActionId.CastPlaneMagicMain,
                bonus: (Id)ExtraActionId.CastPlaneMagicBonus, hidden: true);

        internal static readonly InvocationPoolTypeCustom EldritchVersatilityPool =
            Register("EldritchVersatilityPool",
                main: (Id)ExtraActionId.EldritchVersatilityMain,
                bonus: (Id)ExtraActionId.EldritchVersatilityBonus,
                noCost: (Id)ExtraActionId.EldritchVersatilityNoCost);

        internal static IEnumerable<InvocationPoolTypeCustom> All => PrivatePools;
    }
}
