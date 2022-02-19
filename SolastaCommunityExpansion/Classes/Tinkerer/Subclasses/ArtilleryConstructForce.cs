using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class ForceArtilleryConstructBuilder : MonsterDefinitionBuilder
    {
        private const string ForceArtilleryConstructName = "ForceArtilleryConstruct";
        private const string ForceArtilleryConstructGuid = "91cc706f-97b7-47d9-a46e-89b251fd5efe";

        private ForceArtilleryConstructBuilder(string name, string guid) : base(DatabaseHelper.MonsterDefinitions.Magic_Mouth, name, guid)
        {
            // can use set, need to copy individual parts of presentation
            //Definition.SetMonsterPresentation(DatabaseHelper.MonsterDefinitions.CubeOfLight.MonsterPresentation);

            Definition.GuiPresentation.Title = "Feat/&ForceArtilleryConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.MonsterDefinitions.CubeOfLight.GuiPresentation.SpriteReference);

            Definition.MonsterPresentation.SetHasMonsterPortraitBackground(true);
            Definition.MonsterPresentation.SetCanGeneratePortrait(true);
            Definition.MonsterPresentation.SetAttachedParticlesReference(new AssetReference());
            Definition.MonsterPresentation.SetCustomShaderReference(DatabaseHelper.MonsterDefinitions.FeyGiantApe.MonsterPresentation.CustomShaderReference);
            Definition.MonsterPresentation.SetOverrideCharacterShaderColors(true);
            Definition.MonsterPresentation.SetFirstCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyGiantApe.MonsterPresentation.FirstCharacterShaderColor);
            Definition.MonsterPresentation.SetSecondCharacterShaderColor(DatabaseHelper.MonsterDefinitions.FeyGiantApe.MonsterPresentation.SecondCharacterShaderColor);

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

            // Definition.Features.Add(ForceArtilleryBuilder.ForceArtillery);

            Definition.AttackIterations.SetRange(new MonsterAttackIteration(ForceArtilleryAttackBuilder.ForceArtilleryAttack, 1));

            Definition.CreatureTags.Add("ScalingTinkererArtilleryConstruct");
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryConstructBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition ForceArtilleryConstruct = CreateAndAddToDB(ForceArtilleryConstructName, ForceArtilleryConstructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class ForceArtilleryConstruct9Builder : MonsterDefinitionBuilder
    {
        private const string ForceArtilleryConstruct_9Name = "ForceArtilleryConstruct_9";
        private const string ForceArtilleryConstruct_9Guid = "1a479ea4-0f72-4847-bd0b-54b2ded48057";

        private ForceArtilleryConstruct9Builder(string name, string guid) : base(ForceArtilleryConstructBuilder.ForceArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ForceArtilleryConstructTitle_3";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryConstructDescription_3";

            // Definition.Features.Add(ForceArtillery_2Builder.ForceArtillery_2);
            Definition.Features.Add(ForceArtilleryAdditionalDamageBuilder.ForceArtilleryAdditionalDamage);

            Definition.Features.Add(SelfDestructBuilder.SelfDestruct);
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryConstruct9Builder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition ForceArtilleryConstruct9 = CreateAndAddToDB(ForceArtilleryConstruct_9Name, ForceArtilleryConstruct_9Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class ForceArtilleryConstruct15Builder : MonsterDefinitionBuilder
    {
        private const string ForceArtilleryConstruct_15Name = "ForceArtilleryConstruct_15";
        private const string ForceArtilleryConstruct_15Guid = "e7d49f53-cb44-4348-82d7-b8b561861448";

        private ForceArtilleryConstruct15Builder(string name, string guid) : base(ForceArtilleryConstruct9Builder.ForceArtilleryConstruct9, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ForceArtilleryConstructTitle_5";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryConstructDescription_3";

            Definition.Features.Add(HalfCoverShieldBuilder.HalfCoverShield);
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryConstruct15Builder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition ForceArtilleryConstruct15 = CreateAndAddToDB(ForceArtilleryConstruct_15Name, ForceArtilleryConstruct_15Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonForceArtillerySpellConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonForceArtillerySpellConstructBuilder : SpellDefinitionBuilder
    {
        private const string SummonForceArtilleryConstructName = "SummonForceArtilleryConstruct";
        private const string SummonForceArtilleryConstructGuid = "c584b73c-9fa8-453f-90ad-944b8d1b5b05";

        private SummonForceArtillerySpellConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.DancingLights, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ForceArtilleryModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ForceArtilleryModePowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            Definition.SetSpellLevel(1);
            Definition.SetRequiresConcentration(false);
            Definition.SetUniqueInstance(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Action);

            Definition.SetEffectDescription(ArtilleryConstructlevel03FeatureSetBuilder.ForceArtillery_03modepower.EffectDescription);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonForceArtillerySpellConstructBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonForceArtilleryConstruct = CreateAndAddToDB(SummonForceArtilleryConstructName, SummonForceArtilleryConstructGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonForceArtillerySpellConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonForceArtillerySpellConstruct9Builder : SpellDefinitionBuilder
    {
        private const string SummonForceArtilleryConstruct_9Name = "SummonForceArtilleryConstruct_9";
        private const string SummonForceArtilleryConstruct_9Guid = "f1e8d7e1-44d9-4a82-ac23-5e0013b40650";

        private SummonForceArtillerySpellConstruct9Builder(string name, string guid) : base(SummonForceArtillerySpellConstructBuilder.SummonForceArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ForceArtillery_09ModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ForceArtillery_09ModePowerDescription";

            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(ForceArtilleryConstruct9Builder.ForceArtilleryConstruct9.Name);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonForceArtillerySpellConstruct9Builder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonForceArtilleryConstruct_9 = CreateAndAddToDB(SummonForceArtilleryConstruct_9Name, SummonForceArtilleryConstruct_9Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		SummonForceArtillerySpellConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class SummonForceArtillerySpellConstruct15Builder : SpellDefinitionBuilder
    {
        private const string SummonForceArtilleryConstruct_15Name = "SummonForceArtilleryConstruct_15";
        private const string SummonForceArtilleryConstruct_15Guid = "b529386c-defa-4c39-a03c-09a08a104cc6";

        private SummonForceArtillerySpellConstruct15Builder(string name, string guid) : base(SummonForceArtillerySpellConstructBuilder.SummonForceArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ForceArtillery_15ModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ForceArtillery_15ModePowerDescription";
            Definition.SetUniqueInstance(false);

            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(ForceArtilleryConstruct15Builder.ForceArtilleryConstruct15.Name);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonForceArtillerySpellConstruct15Builder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition SummonForceArtilleryConstruct15 = CreateAndAddToDB(SummonForceArtilleryConstruct_15Name, SummonForceArtilleryConstruct_15Guid);
    }

    internal sealed class ForceArtilleryAttackBuilder : MonsterAttackDefinitionBuilder
    {
        private const string ForceArtilleryAttackName = "ForceArtilleryAttack";
        private const string ForceArtilleryAttackGuid = "39c5b7ef-47f9-462c-9f24-accaee85d325";

        private ForceArtilleryAttackBuilder(string name, string guid) : base(DatabaseHelper.MonsterAttackDefinitions.Attack_Orc_Grimblade_IceDagger, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ForceArtilleryTitle";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            Definition.SetToHitBonus(0);
            Definition.SetActionType(ActionDefinitions.ActionType.Main);
            Definition.SetCloseRange(24);
            Definition.SetMaxRange(24);
            Definition.SetReachRange(24);

            /*
             *Arrow
Bolt
Bolt_Poisoned_BasicPoison
Dart
Dryad_Spine
FireArrow
Giant_Rock
Javelin
OrcGrimblade_IceDagger
SkeletonMarksman_Necro_Arrow
Sorak_PoisonedSpine
Spear
SpiderCrimson_AcidSpit
Spit
Web
*/
            // Definition.SetProjectile("OrcGrimblade_IceDagger");
            Definition.SetProjectile(ForceArtilleryProjectileBuilder.ForceArtilleryProjectile.name);
            Definition.SetProjectileBone(AnimationDefinitions.BoneType.Chest);

            MotionForm motionForm = new MotionForm();
            motionForm.SetType(MotionForm.MotionType.PushFromOrigin);
            motionForm.SetDistance(1);

            EffectForm effectmotion = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Motion
            };
            effectmotion.SetMotionForm(motionForm);
            effectmotion.SetCreatedByCharacter(true);
            effectmotion.HasSavingThrow = false;
            effectmotion.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            effectmotion.SetLevelMultiplier(1);
            effectmotion.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            effectmotion.SetApplyLevel(EffectForm.LevelApplianceType.No);

            DamageForm forceArtilleryAttack = new DamageForm
            {
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 2,
                DamageType = RuleDefinitions.DamageTypeForce,
                BonusDamage = 0
            };

            EffectForm effect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm = forceArtilleryAttack
            };
            effect.SetCreatedByCharacter(true);
            effect.HasSavingThrow = false;
            effect.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            effect.SetLevelMultiplier(1);
            effect.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            effect.SetApplyLevel(EffectForm.LevelApplianceType.No);

            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(effect);
            Definition.EffectDescription.EffectForms.Add(effectmotion);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.IndividualsUnique);
            Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            Definition.EffectDescription.SetTargetParameter(1);
            Definition.EffectDescription.SetRangeParameter(24);
            Definition.EffectDescription.HasSavingThrow = false;
            Definition.EffectDescription.SetCreatedByCharacter(true);
            Definition.EffectDescription.SetCanBePlacedOnCharacter(true);

            Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);

            EffectParticleParameters effectParticleParameters = new EffectParticleParameters();
            effectParticleParameters.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);
            Definition.EffectDescription.SetEffectParticleParameters(effectParticleParameters);
        }

        public static MonsterAttackDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryAttackBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterAttackDefinition ForceArtilleryAttack = CreateAndAddToDB(ForceArtilleryAttackName, ForceArtilleryAttackGuid);
    }

    internal sealed class ForceArtilleryAdditionalDamageBuilder : FeatureDefinitionAdditionalDamageBuilder
    {
        private const string ForceArtilleryAdditionalDamageName = "ForceArtilleryAdditionalDamage";
        private const string ForceArtilleryAdditionalDamageGuid = "2e726b2e-052f-482f-a869-721851fcb407";

        private ForceArtilleryAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ForceArtilleryAdditionalDamageTitle";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryAdditionalDamageDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            Definition.SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.SameAsBaseDamage);
            Definition.SetAttackModeOnly(true);
            Definition.SetDamageDiceNumber(1);
            Definition.SetDamageDieType(RuleDefinitions.DieType.D8);
            Definition.SetNotificationTag("UpgradedConstruct");
            Definition.SetLimitedUsage(RuleDefinitions.FeatureLimitedUsage.None);
        }

        private static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryAdditionalDamageBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionAdditionalDamage ForceArtilleryAdditionalDamage = CreateAndAddToDB(ForceArtilleryAdditionalDamageName, ForceArtilleryAdditionalDamageGuid);
    }

    internal sealed class ForceArtilleryProjectileBuilder : ItemDefinitionBuilder
    {
        private const string ForceArtilleryProjectileName = "ForceArtilleryProjectile";
        private const string ForceArtilleryProjectileGuid = "b30b1971-6ad6-4ea2-af5b-998043415f04";

        private ForceArtilleryProjectileBuilder(string name, string guid) : base(DatabaseHelper.ItemDefinitions.OrcGrimblade_IceDagger, name, guid)
        {
            Definition.GuiPresentation.Title = "Item/&ForceArtilleryProjectileTitle";
            Definition.GuiPresentation.Description = "Item/&ForceArtilleryProjectileDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectParticleParameters effectParticleParameters = new EffectParticleParameters();
            effectParticleParameters.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);
            Definition.AmmunitionDescription.EffectDescription.SetEffectParticleParameters(effectParticleParameters);
        }

        private static ItemDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryProjectileBuilder(name, guid).AddToDB();
        }

        public static readonly ItemDefinition ForceArtilleryProjectile = CreateAndAddToDB(ForceArtilleryProjectileName, ForceArtilleryProjectileGuid);
    }
}
