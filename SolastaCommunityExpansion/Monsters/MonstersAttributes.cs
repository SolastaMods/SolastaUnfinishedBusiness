using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine.AddressableAssets;

//******************************************************************************************
// BY DEFINITION, REFACTORING REQUIRES CONFIRMING EXTERNAL BEHAVIOUR DOES NOT CHANGE
// "REFACTORING WITHOUT TESTS IS JUST CHANGING STUFF"
//******************************************************************************************

namespace SolastaMonsters.Monsters;

public class MonstersAttributes
{
    public static MonsterAttackIteration BlackDragonBiteAttackIteration = new();
    public static MonsterAttackIteration BlueDragonBiteAttackIteration = new();
    public static MonsterAttackIteration GreenDragonBiteAttackIteration = new();
    public static MonsterAttackIteration RedDragonBiteAttackIteration = new();
    public static MonsterAttackIteration WhiteDragonBiteAttackIteration = new();
    public static MonsterAttackIteration DragonClawAttackIteration = new();
    public static MonsterAttackIteration DragonClawAttackIteration_2 = new();
    public static LegendaryActionDescription DragonlegendaryActionDescription = new();
    public static LegendaryActionDescription DragonlegendaryActionDescription_2 = new();
    public static MonsterSkillProficiency DragonmonsterSkillProficiency_1 = new();
    public static MonsterSkillProficiency DragonmonsterSkillProficiency_2 = new();
    public static MonsterSkillProficiency GreenDragonmonsterSkillProficiency_3 = new();
    public static MonsterSkillProficiency GreenDragonmonsterSkillProficiency_4 = new();
    public static MonsterSkillProficiency GreenDragonmonsterSkillProficiency_5 = new();

    public static MonsterSkillProficiency ArchmagemonsterSkillProficiency_1 = new();
    public static MonsterSkillProficiency ArchmagemonsterSkillProficiency_2 = new();

    public static AssetReference BalorassetReference = new();
    public static MonsterAttackIteration BalorLongswordAttackIteration = new();
    public static MonsterAttackIteration BalorWhipAttackIteration = new();

    public static MonsterSkillProficiency DevamonsterSkillProficiency_1 = new();
    public static MonsterSkillProficiency DevamonsterSkillProficiency_2 = new();

    public static AssetReference DjinniassetReference = new();
    public static MonsterAttackIteration DjinniAttackIteration = new();
    public static MonsterAttackIteration DjinniAttackIteration_2 = new();

    public static AssetReference EfreetiassetReference = new();
    public static MonsterAttackIteration EfreetiAttackIteration = new();
    public static MonsterAttackIteration EfreetiAttackIteration_2 = new();

    public static MonsterAttackIteration ErinyesAttackIteration = new();
    public static MonsterAttackIteration ErinyesAttackIteration_2 = new();

    public static MonsterAttackIteration NagaAttackIteration = new();
    public static MonsterAttackIteration NagaAttackIteration_2 = new();

    public static MonsterAttackIteration HornedDevilForkAttackIteration = new();
    public static MonsterAttackIteration HornedDevilClawAttackIteration = new();
    public static MonsterAttackIteration HornedDevilTailAttackIteration = new();

    public static MonsterAttackIteration IceDevilBiteAttackIteration = new();
    public static MonsterAttackIteration IceDevilClawAttackIteration = new();
    public static MonsterAttackIteration IceDevilTailAttackIteration = new();

    public static MonsterAttackIteration LichAttackIteration = new();
    public static LegendaryActionDescription LichlegendaryActionDescription_0 = new();
    public static LegendaryActionDescription LichlegendaryActionDescription_4 = new();
    public static LegendaryActionDescription LichlegendaryActionDescription = new();
    public static LegendaryActionDescription LichlegendaryActionDescription_2 = new();
    public static LegendaryActionDescription LichlegendaryActionDescription_3 = new();
    public static MonsterSkillProficiency LichmonsterSkillProficiency_1 = new();
    public static MonsterSkillProficiency LichmonsterSkillProficiency_2 = new();
    public static MonsterSkillProficiency LichmonsterSkillProficiency_3 = new();
    public static MonsterSkillProficiency LichmonsterSkillProficiency_4 = new();
    public static AssetReference LichassetReference = new();

    public static MonsterAttackIteration NalfeshneeBiteAttackIteration = new();
    public static MonsterAttackIteration NalfeshneeClawAttackIteration = new();

    public static MonsterAttackIteration PitFiendBiteAttackIteration = new();
    public static MonsterAttackIteration PitFiendClawAttackIteration = new();
    public static MonsterAttackIteration PitFiendTailAttackIteration = new();
    public static MonsterAttackIteration PitFiendWeaponAttackIteration = new();


    public static MonsterAttackIteration PlanetarLongswordAttackIteration = new();
    public static MonsterSkillProficiency PlanetarmonsterSkillProficiency_1 = new();

    public static MonsterAttackIteration RocBiteAttackIteration = new();
    public static MonsterAttackIteration RocClawAttackIteration = new();
    public static MonsterSkillProficiency RocmonsterSkillProficiency_1 = new();

    public static MonsterAttackIteration SolarLongswordAttackIteration = new();
    public static MonsterAttackIteration SolarLongbowAttackIteration = new();
    public static LegendaryActionDescription SolarlegendaryActionDescription = new();
    public static LegendaryActionDescription SolarlegendaryActionDescription_2 = new();
    public static LegendaryActionDescription SolarlegendaryActionDescription_3 = new();
    public static MonsterSkillProficiency SolarmonsterSkillProficiency_1 = new();

    public static MonsterSkillProficiency StormGiantmonsterSkillProficiency_1 = new();
    public static MonsterSkillProficiency StormGiantmonsterSkillProficiency_2 = new();
    public static MonsterSkillProficiency StormGiantmonsterSkillProficiency_3 = new();
    public static MonsterSkillProficiency StormGiantmonsterSkillProficiency_4 = new();


    public static LegendaryActionDescription VampirelegendaryActionDescription = new();
    public static LegendaryActionDescription VampirelegendaryActionDescription_2 = new();

    public static LegendaryActionDescription VampirelegendaryActionDescription_3 = new();

    //public static LegendaryActionDescription VampirelegendaryActionDescription_4 = new LegendaryActionDescription();
    public static MonsterSkillProficiency VampiremonsterSkillProficiency_1 = new();
    public static MonsterSkillProficiency VampiremonsterSkillProficiency_2 = new();

    public static AssetReference EmptyassetReference = new();


    public static MonsterAttackIteration TarrasqueBiteAttackIteration = new();
    public static MonsterAttackIteration TarrasqueClawAttackIteration = new();
    public static MonsterAttackIteration TarrasqueHornAttackIteration = new();
    public static MonsterAttackIteration TarrasqueTailAttackIteration = new();


    public static LegendaryActionDescription TarrasquelegendaryActionDescription = new();
    public static LegendaryActionDescription TarrasquelegendaryActionDescription_2 = new();
    public static LegendaryActionDescription TarrasquelegendaryActionDescription_3 = new();
    public static LegendaryActionDescription TarrasquelegendaryActionDescription_4 = new();


    public static void EnableInDungeonMaker()
    {
        BlackDragonBiteAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.DictionaryOfAncientDragonBites["Ancient Black Dragon"];
        BlackDragonBiteAttackIteration.number = 1;

        BlueDragonBiteAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.DictionaryOfAncientDragonBites["Ancient Blue Dragon"];
        BlueDragonBiteAttackIteration.number = 1;

        GreenDragonBiteAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.DictionaryOfAncientDragonBites["Ancient Green Dragon"];
        GreenDragonBiteAttackIteration.number = 1;

        RedDragonBiteAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.DictionaryOfAncientDragonBites["Ancient Red Dragon"];
        RedDragonBiteAttackIteration.number = 1;

        WhiteDragonBiteAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.DictionaryOfAncientDragonBites["Ancient White Dragon"];
        WhiteDragonBiteAttackIteration.number = 1;

        DragonClawAttackIteration.monsterAttackDefinition = NewMonsterAttacks.AncientDragon_Claw_Attack;
        DragonClawAttackIteration.number = 1;

        DragonClawAttackIteration_2.monsterAttackDefinition =
            NewMonsterAttacks.AncientDragon_Claw_Attack;
        DragonClawAttackIteration_2.number = 1;

        DragonlegendaryActionDescription.cost = 2;
        DragonlegendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.Power;
        DragonlegendaryActionDescription.canMove = true;
        DragonlegendaryActionDescription.moveMode = DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly6;
        DragonlegendaryActionDescription.featureDefinitionPower = NewMonsterPowers.AncientDragon_Wing_Power;
        DragonlegendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDragonWingAttack;

        DragonlegendaryActionDescription_2.cost = 1;
        DragonlegendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
        DragonlegendaryActionDescription_2.monsterAttackDefinition = NewMonsterAttacks.AncientDragon_Tail_Attack;
        DragonlegendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDragonAttack;

        DragonmonsterSkillProficiency_1.skillName = SkillDefinitions.Perception;
        DragonmonsterSkillProficiency_1.bonus = 16;
        DragonmonsterSkillProficiency_2.skillName = SkillDefinitions.Stealth;
        DragonmonsterSkillProficiency_2.bonus = 8;

        GreenDragonmonsterSkillProficiency_3.skillName = SkillDefinitions.Persuasion;
        GreenDragonmonsterSkillProficiency_3.bonus = 11;
        GreenDragonmonsterSkillProficiency_4.skillName = SkillDefinitions.Deception;
        GreenDragonmonsterSkillProficiency_4.bonus = 11;
        GreenDragonmonsterSkillProficiency_5.skillName = SkillDefinitions.Insight;
        GreenDragonmonsterSkillProficiency_5.bonus = 10;

        ArchmagemonsterSkillProficiency_1.skillName = SkillDefinitions.Arcana;
        ArchmagemonsterSkillProficiency_1.bonus = 13;

        ArchmagemonsterSkillProficiency_2.skillName = SkillDefinitions.History;
        ArchmagemonsterSkillProficiency_2.bonus = 13;

        BalorassetReference.SetField("m_AssetGUID", "5d249b514baa99040874880ba8d35295");

        BalorLongswordAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Balor_Longsword_Attack;
        BalorLongswordAttackIteration.number = 1;

        BalorWhipAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Balor_Whip_Attack;
        BalorWhipAttackIteration.number = 1;

        DevamonsterSkillProficiency_1.skillName = SkillDefinitions.Insight;
        DevamonsterSkillProficiency_1.bonus = 9;

        DevamonsterSkillProficiency_2.skillName = SkillDefinitions.Perception;
        DevamonsterSkillProficiency_2.bonus = 9;

        DjinniassetReference.SetField("m_AssetGUID", "2a2913c5eec57a24da4af020cf0e0f0f");

        DjinniAttackIteration.monsterAttackDefinition = NewMonsterAttacks.LightningScimatar_Attack;
        DjinniAttackIteration.number = 3;

        DjinniAttackIteration_2.monsterAttackDefinition = NewMonsterAttacks.AirBlast_Attack;
        DjinniAttackIteration_2.number = 3;

        EfreetiassetReference.SetField("m_AssetGUID", "1a7c8ed60c954dd42bf2beb0fcf894c4");

        EfreetiAttackIteration.monsterAttackDefinition = NewMonsterAttacks.FireScimatar_Attack;
        EfreetiAttackIteration.number = 2;

        EfreetiAttackIteration_2.monsterAttackDefinition = NewMonsterAttacks.HurlFlame_Attack;
        EfreetiAttackIteration_2.number = 2;

        ErinyesAttackIteration.monsterAttackDefinition = NewMonsterAttacks.PoisonLongsword_Attack;
        ErinyesAttackIteration.number = 3;

        ErinyesAttackIteration_2.monsterAttackDefinition = NewMonsterAttacks.PoisonLongbow_Attack;
        ErinyesAttackIteration_2.number = 3;

        NagaAttackIteration.monsterAttackDefinition = NewMonsterAttacks.NagaSpit_Attack;
        NagaAttackIteration.number = 1;

        NagaAttackIteration_2.monsterAttackDefinition = NewMonsterAttacks.NagaBite_Attack;
        NagaAttackIteration_2.number = 1;

        HornedDevilForkAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Fork_Attack;
        HornedDevilForkAttackIteration.number = 1;

        HornedDevilClawAttackIteration.monsterAttackDefinition = NewMonsterAttacks.HurlFlame_Attack;
        HornedDevilClawAttackIteration.number = 1;

        HornedDevilTailAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.HornedDevilTail_Attack;
        HornedDevilTailAttackIteration.number = 1;

        IceDevilBiteAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Ice_Bite_Attack;
        IceDevilBiteAttackIteration.number = 1;

        IceDevilClawAttackIteration.monsterAttackDefinition =
            DatabaseHelper.MonsterAttackDefinitions.Attack_Green_Dragon_Claw;
        IceDevilClawAttackIteration.number = 1;

        IceDevilTailAttackIteration.monsterAttackDefinition =
            DatabaseHelper.MonsterAttackDefinitions.Attack_Green_Dragon_Tail;
        IceDevilTailAttackIteration.number = 1;

        LichAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Lich_ParalyzingTouch_Attack;
        LichAttackIteration.number = 1;

        LichassetReference.SetField("m_AssetGUID", "5bbe5d35725c2cc4492672476f4fc783");

        LichlegendaryActionDescription_0.cost = 1;
        LichlegendaryActionDescription_0.subaction = LegendaryActionDescription.SubactionType.Spell;
        LichlegendaryActionDescription_0.spellDefinition = DatabaseHelper.SpellDefinitions.RayOfFrost;
        LichlegendaryActionDescription_0.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryLaetharCast_Debuff;

        LichlegendaryActionDescription_4.cost = 1;
        LichlegendaryActionDescription_4.subaction = LegendaryActionDescription.SubactionType.Spell;
        LichlegendaryActionDescription_4.spellDefinition = DatabaseHelper.SpellDefinitions.ChillTouch;
        LichlegendaryActionDescription_4.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryLaetharCast_Debuff;

        LichlegendaryActionDescription.cost = 2;
        LichlegendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.Power;
        LichlegendaryActionDescription.featureDefinitionPower = DatabaseHelper.FeatureDefinitionPowers
            .PowerLaetharParalyzingGaze;
        LichlegendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryLaetharCast_Debuff;

        LichlegendaryActionDescription_2.cost = 2;
        LichlegendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
        LichlegendaryActionDescription_2.monsterAttackDefinition = NewMonsterAttacks.Lich_ParalyzingTouch_Attack;
        LichlegendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDragonAttack;

        LichlegendaryActionDescription_3.cost = 3;
        LichlegendaryActionDescription_3.subaction = LegendaryActionDescription.SubactionType.Power;
        LichlegendaryActionDescription_3.featureDefinitionPower = NewMonsterPowers.Lich_DisruptLife_Power;
        LichlegendaryActionDescription_3.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryAoE_DpS;

        LichmonsterSkillProficiency_1.skillName = SkillDefinitions.Arcana;
        LichmonsterSkillProficiency_1.bonus = 19;

        LichmonsterSkillProficiency_2.skillName = SkillDefinitions.History;
        LichmonsterSkillProficiency_2.bonus = 12;

        LichmonsterSkillProficiency_3.skillName = SkillDefinitions.Insight;
        LichmonsterSkillProficiency_3.bonus = 9;

        LichmonsterSkillProficiency_4.skillName = SkillDefinitions.Perception;
        LichmonsterSkillProficiency_4.bonus = 9;

        NalfeshneeBiteAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.Generic_Stronger_Bite_Attack;
        NalfeshneeBiteAttackIteration.number = 1;

        NalfeshneeClawAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.AncientDragon_Claw_Attack;
        NalfeshneeClawAttackIteration.number = 1;

        PitFiendBiteAttackIteration.monsterAttackDefinition = NewMonsterAttacks.PitFiend_Bite_Attack;
        PitFiendBiteAttackIteration.number = 1;

        PitFiendClawAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.AncientDragon_Claw_Attack;
        PitFiendClawAttackIteration.number = 1;

        PitFiendTailAttackIteration.monsterAttackDefinition =
            DatabaseHelper.MonsterAttackDefinitions.Attack_Green_Dragon_Tail;
        PitFiendTailAttackIteration.number = 1;

        PitFiendWeaponAttackIteration.monsterAttackDefinition = NewMonsterAttacks.PitFiend_Mace_Attack;
        PitFiendWeaponAttackIteration.number = 1;

        PlanetarLongswordAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.RadiantLongsword_Attack;
        PlanetarLongswordAttackIteration.number = 2;

        PlanetarmonsterSkillProficiency_1.skillName = SkillDefinitions.Perception;
        PlanetarmonsterSkillProficiency_1.bonus = 11;

        RocBiteAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Roc_Beak_Attack;
        RocBiteAttackIteration.number = 1;

        RocClawAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Roc_Talons_Attack;
        RocClawAttackIteration.number = 1;

        RocmonsterSkillProficiency_1.skillName = SkillDefinitions.Perception;
        RocmonsterSkillProficiency_1.bonus = 4;

        SolarLongswordAttackIteration.monsterAttackDefinition =
            NewMonsterAttacks.RadiantLongsword_Attack;
        SolarLongswordAttackIteration.number = 2;

        SolarLongbowAttackIteration.monsterAttackDefinition = NewMonsterAttacks.RadiantLongbow_Attack;
        SolarLongbowAttackIteration.number = 2;

        SolarlegendaryActionDescription.cost = 1;
        SolarlegendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.Power;
        SolarlegendaryActionDescription.featureDefinitionPower = DatabaseHelper.FeatureDefinitionPowers
            .PowerLaetharMistyFormEscape;
        SolarlegendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryLaetharCast_Teleport;

        SolarlegendaryActionDescription_2.cost = 2;
        SolarlegendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.Power;
        SolarlegendaryActionDescription_2.featureDefinitionPower = NewMonsterPowers.SearingBurst_Power;
        SolarlegendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryAoE_DpS;

        SolarlegendaryActionDescription_3.cost = 3;
        SolarlegendaryActionDescription_3.subaction = LegendaryActionDescription.SubactionType.Power;
        SolarlegendaryActionDescription_3.featureDefinitionPower = NewMonsterPowers.BlindingGaze_Power;
        SolarlegendaryActionDescription_3.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryLaetharCast_Debuff;

        SolarmonsterSkillProficiency_1.skillName = SkillDefinitions.Perception;
        SolarmonsterSkillProficiency_1.bonus = 14;
        StormGiantmonsterSkillProficiency_1.skillName = SkillDefinitions.Arcana;
        StormGiantmonsterSkillProficiency_1.bonus = 8;

        StormGiantmonsterSkillProficiency_2.skillName = SkillDefinitions.Athletics;
        StormGiantmonsterSkillProficiency_2.bonus = 14;

        StormGiantmonsterSkillProficiency_3.skillName = SkillDefinitions.History;
        StormGiantmonsterSkillProficiency_3.bonus = 8;

        StormGiantmonsterSkillProficiency_3.skillName = SkillDefinitions.Perception;
        StormGiantmonsterSkillProficiency_3.bonus = 9;

        VampirelegendaryActionDescription.cost = 1;
        VampirelegendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
        VampirelegendaryActionDescription.monsterAttackDefinition = DatabaseHelper.MonsterAttackDefinitions
            .Attack_Defiler_Bite_Razan;
        VampirelegendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDefilerAttack;
        VampirelegendaryActionDescription.canMove = true;
        VampirelegendaryActionDescription.moveMode = DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly6;
        VampirelegendaryActionDescription.noOpportunityAttack = true;

        VampirelegendaryActionDescription_2.cost = 1;
        VampirelegendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.Power;
        VampirelegendaryActionDescription_2.featureDefinitionPower = DatabaseHelper.FeatureDefinitionPowers
            .PowerDefilerDarkness;
        VampirelegendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDefilerDarkness;


        VampirelegendaryActionDescription_3.cost = 1;
        VampirelegendaryActionDescription_3.subaction = LegendaryActionDescription.SubactionType.Power;
        VampirelegendaryActionDescription_3.featureDefinitionPower = NewMonsterPowers.VampireCharmPower;
        VampirelegendaryActionDescription_3.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryLaetharCast_Debuff;

        //VampirelegendaryActionDescription_4.SetCost(3);
        //VampirelegendaryActionDescription_4.SetSubaction(LegendaryActionDescription.SubactionType.Power);
        //VampirelegendaryActionDescription_4.SetFeatureDefinitionPower(DatabaseHelper.FeatureDefinitionPowers.PowerLaetharParalyzingGaze);
        //VampirelegendaryActionDescription_4.SetDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.LegendaryLaetharCast_Debuff);

        VampiremonsterSkillProficiency_1.skillName = SkillDefinitions.Perception;
        VampiremonsterSkillProficiency_1.bonus = 7;

        VampiremonsterSkillProficiency_2.skillName = SkillDefinitions.Stealth;
        VampiremonsterSkillProficiency_2.bonus = 9;


        EmptyassetReference.SetField("m_AssetGUID", "");
        EmptyassetReference.SetField("m_SubObjectName", "");
        EmptyassetReference.SetField("m_SubObjectType", "");

        TarrasqueBiteAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Bite_Attack;
        TarrasqueBiteAttackIteration.number = 1;

        TarrasqueClawAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Claw_Attack;
        TarrasqueClawAttackIteration.number = 1;

        TarrasqueHornAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Horn_Attack;
        TarrasqueHornAttackIteration.number = 1;

        TarrasqueTailAttackIteration.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Tail_Attack;
        TarrasqueTailAttackIteration.number = 1;

        TarrasquelegendaryActionDescription.cost = 2;
        TarrasquelegendaryActionDescription.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
        TarrasquelegendaryActionDescription.canMove = true;
        TarrasquelegendaryActionDescription.moveMode = DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly6;
        TarrasquelegendaryActionDescription.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Bite_Attack;
        TarrasquelegendaryActionDescription.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDragonAttack;

        TarrasquelegendaryActionDescription_2.cost = 1;
        TarrasquelegendaryActionDescription_2.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
        TarrasquelegendaryActionDescription_2.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Tail_Attack;
        TarrasquelegendaryActionDescription_2.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDragonAttack;

        TarrasquelegendaryActionDescription_3.cost = 1;
        TarrasquelegendaryActionDescription_3.subaction = LegendaryActionDescription.SubactionType.MonsterAttack;
        TarrasquelegendaryActionDescription_3.monsterAttackDefinition = NewMonsterAttacks.Tarrasque_Claw_Attack;
        TarrasquelegendaryActionDescription_3.decisionPackage = DatabaseHelper.DecisionPackageDefinitions
            .LegendaryDragonAttack;

        //   TarrasquelegendaryActionDescription_4.SetCost(2);
        //   TarrasquelegendaryActionDescription_4.SetSubaction(LegendaryActionDescription.SubactionType.Power);
        //   TarrasquelegendaryActionDescription_4.SetFeatureDefinitionPower(NewMonsterPowers.TarrasqueSwallowPower);
        //   TarrasquelegendaryActionDescription_4.SetDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.LegendaryLaetharCast_Debuff);
    }
}
