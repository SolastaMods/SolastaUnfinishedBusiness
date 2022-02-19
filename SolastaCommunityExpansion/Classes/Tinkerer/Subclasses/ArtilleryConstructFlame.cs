using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    //*****************************************************************************************************************************************
    //***********************************		FlameArtilleryBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class FlameArtilleryBuilder : FeatureDefinitionPowerBuilder
    {
        private const string FlameArtilleryName = "FlameArtillery";
        private const string FlameArtilleryGuid = "3a93be16-4398-47cb-9c1c-4ec56903bd2f";

        private FlameArtilleryBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDragonBreath_Fire, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&FlameArtilleryTitle";
            Definition.GuiPresentation.Description = "Feat/&FlameArtilleryDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);

            DamageForm flameArtillery = new DamageForm
            {
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 2,
                DamageType = RuleDefinitions.DamageTypeFire,
                BonusDamage = 0
            };

            // AlterationForm alterationForm = new AlterationForm();
            //alterationForm.SetAlterationType (AlterationForm.Type.LightUp);

            EffectForm effect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm = flameArtillery
            };
            effect.SetCreatedByCharacter(true);
            effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            effect.HasSavingThrow = true;
            effect.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            effect.SetLevelMultiplier(1);
            effect.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            effect.SetApplyLevel(EffectForm.LevelApplianceType.No);

            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(effect);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Cone);
            Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.All);
            Definition.EffectDescription.SetTargetParameter(3);
            Definition.EffectDescription.SetRangeParameter(3);
            Definition.EffectDescription.HasSavingThrow = true;
            Definition.EffectDescription.SavingThrowAbility = DatabaseHelper.SmartAttributeDefinitions.Dexterity.Name;
            Definition.EffectDescription.FixedSavingThrowDifficultyClass = 15;
            Definition.EffectDescription.SetCreatedByCharacter(true);
            Definition.EffectDescription.SetCanBePlacedOnCharacter(true);

            Definition.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.BurningHands.EffectDescription.EffectParticleParameters);
            Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new FlameArtilleryBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower FlameArtillery = CreateAndAddToDB(FlameArtilleryName, FlameArtilleryGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		FlameArtillery_2Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class FlameArtillery2Builder : FeatureDefinitionPowerBuilder
    {
        private const string FlameArtillery_2Name = "FlameArtillery_2";
        private const string FlameArtillery_2Guid = "2ba003a5-718a-4eea-a0f8-33fa79884cb1";

        private FlameArtillery2Builder(string name, string guid) : base(FlameArtilleryBuilder.FlameArtillery, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&FlameArtillery_2Title";
            Definition.GuiPresentation.Description = "Feat/&FlameArtillery_2Description";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            Definition.EffectDescription.EffectForms[0].DamageForm.DiceNumber = 3;
            Definition.SetOverriddenPower(FlameArtilleryBuilder.FlameArtillery);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new FlameArtillery2Builder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower FlameArtillery2 = CreateAndAddToDB(FlameArtillery_2Name, FlameArtillery_2Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		FlameArtilleryConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class FlameArtilleryConstructBuilder : MonsterDefinitionBuilder
    {
        private const string FlameArtilleryConstructName = "FlameArtilleryConstruct";
        private const string FlameArtilleryConstructGuid = "26631741-1de8-4f4c-871e-0d71a2ed8c4b";

        private FlameArtilleryConstructBuilder(string name, string guid) : base(DatabaseHelper.MonsterDefinitions.Magic_Mouth, name, guid)
        {
            // can use set, need to copy individual parts of presentation
            //Definition.SetMonsterPresentation(DatabaseHelper.MonsterDefinitions.CubeOfLight.MonsterPresentation);

            Definition.GuiPresentation.Title = "Feat/&FlameArtilleryConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&FlameArtilleryConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.MonsterDefinitions.CubeOfLight.GuiPresentation.SpriteReference);

            Definition.MonsterPresentation.SetHasMonsterPortraitBackground(true);
            Definition.MonsterPresentation.SetCanGeneratePortrait(true);
            Definition.MonsterPresentation.SetAttachedParticlesReference(new AssetReference());

            Definition.SetArmorClass(18);
            Definition.SetNoExperienceGain(true);
            Definition.SetHitDice(3);
            Definition.SetHitDiceType(RuleDefinitions.DieType.D8);

            Definition.AbilityScores.Empty();
            Definition.AbilityScores.AddToArray(10);    // STR
            Definition.AbilityScores.AddToArray(10);    // DEX
            Definition.AbilityScores.AddToArray(10);    // CON
            Definition.AbilityScores.AddToArray(10);     // INT
            Definition.AbilityScores.AddToArray(10);    // WIS
            Definition.AbilityScores.AddToArray(10);     // CHA

            Definition.SetFullyControlledWhenAllied(true);
            Definition.SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None);
            Definition.SetStandardHitPoints(15);
            Definition.SetDefaultFaction("Party");
            Definition.SetCharacterFamily(TinkererConstructFamilyBuilder.TinkererConstructFamily.Name);
            //

            Definition.Features.Clear();
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove2);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityJump);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityBlindnessImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityDiseaseImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityGrappledImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityParalyzedmmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPetrifiedImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityUnconsciousImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPsychicImmunity);

            Definition.Features.Add(FlameArtilleryBuilder.FlameArtillery);

            Definition.CreatureTags.Add("ScalingTinkererArtilleryConstruct");
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new FlameArtilleryConstructBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition FlameArtilleryConstruct = CreateAndAddToDB(FlameArtilleryConstructName, FlameArtilleryConstructGuid);
    }

    internal sealed class FlameArtilleryConstruct9Builder : MonsterDefinitionBuilder
    {
        private const string FlameArtilleryConstruct_9Name = "FlameArtilleryConstruct_9";
        private const string FlameArtilleryConstruct_9Guid = "3445274f-9668-4606-8a91-4c6a420a7c30";

        private FlameArtilleryConstruct9Builder(string name, string guid) : base(FlameArtilleryConstructBuilder.FlameArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&FlameArtilleryConstructTitle_3";

            Definition.Features.Add(FlameArtillery2Builder.FlameArtillery2);
            Definition.Features.Add(SelfDestructBuilder.SelfDestruct);
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new FlameArtilleryConstruct9Builder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition FlameArtilleryConstruct9 = CreateAndAddToDB(FlameArtilleryConstruct_9Name, FlameArtilleryConstruct_9Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		FlameArtilleryConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class FlameArtilleryConstruct15Builder : MonsterDefinitionBuilder
    {
        private const string FlameArtilleryConstruct_15Name = "FlameArtilleryConstruct_15";
        private const string FlameArtilleryConstruct_15Guid = "8c4ff931-4a17-4de4-8571-6c94e8327e8e";

        private FlameArtilleryConstruct15Builder(string name, string guid) : base(FlameArtilleryConstruct9Builder.FlameArtilleryConstruct9, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&FlameArtilleryConstructTitle_5";

            Definition.Features.Add(HalfCoverShieldBuilder.HalfCoverShield);
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new FlameArtilleryConstruct15Builder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition FlameArtilleryConstruct15 = CreateAndAddToDB(FlameArtilleryConstruct_15Name, FlameArtilleryConstruct_15Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonFlameArtillerySpellConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonFlameArtillerySpellConstructBuilder : SpellDefinitionBuilder
    {
        private const string SummonFlameArtilleryConstructName = "SummonFlameArtilleryConstruct";
        private const string SummonFlameArtilleryConstructGuid = "785ca8dc-27a3-4805-88fd-6d013631bbbb";

        private SummonFlameArtillerySpellConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.DancingLights, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&FlameArtilleryModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&FlameArtilleryModePowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.BurningHands.GuiPresentation.SpriteReference);

            Definition.SetSpellLevel(1);
            Definition.SetRequiresConcentration(false);
            Definition.SetUniqueInstance(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Action);

            Definition.SetEffectDescription(ArtilleryConstructlevel03FeatureSetBuilder.FlameArtillery_03modepower.EffectDescription);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonFlameArtillerySpellConstructBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonFlameArtilleryConstruct = CreateAndAddToDB(SummonFlameArtilleryConstructName, SummonFlameArtilleryConstructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonFlameArtillerySpellConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonFlameArtillerySpellConstruct9Builder : SpellDefinitionBuilder
    {
        private const string SummonFlameArtilleryConstruct_9Name = "SummonFlameArtilleryConstruct_9";
        private const string SummonFlameArtilleryConstruct_9Guid = "4aaaf381-c54c-4285-9045-6a4d69aa37c9";

        private SummonFlameArtillerySpellConstruct9Builder(string name, string guid) : base(SummonFlameArtillerySpellConstructBuilder.SummonFlameArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&FlameArtillery_09ModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&FlameArtillery_09ModePowerDescription";
            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(FlameArtilleryConstruct9Builder.FlameArtilleryConstruct9.Name);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonFlameArtillerySpellConstruct9Builder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonFlameArtilleryConstruct9 = CreateAndAddToDB(SummonFlameArtilleryConstruct_9Name, SummonFlameArtilleryConstruct_9Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonFlameArtillerySpellConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonFlameArtillerySpellConstruct15Builder : SpellDefinitionBuilder
    {
        private const string SummonFlameArtilleryConstruct_15Name = "SummonFlameArtilleryConstruct_15";
        private const string SummonFlameArtilleryConstruct_15Guid = "68aba04a-07c5-4b83-bda7-db08cec2dec8";

        private SummonFlameArtillerySpellConstruct15Builder(string name, string guid) : base(SummonFlameArtillerySpellConstructBuilder.SummonFlameArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&FlameArtillery_15ModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&FlameArtillery_15ModePowerDescription";
            Definition.SetUniqueInstance(false);
            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(FlameArtilleryConstruct15Builder.FlameArtilleryConstruct15.Name);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonFlameArtillerySpellConstruct15Builder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonFlameArtilleryConstruct15 = CreateAndAddToDB(SummonFlameArtilleryConstruct_15Name, SummonFlameArtilleryConstruct_15Guid);
    }
}
