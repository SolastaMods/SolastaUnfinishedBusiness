using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{


    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class ForceArtilleryBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string ForceArtilleryName = "ForceArtillery";
        private const string ForceArtilleryGuid = "928af495-1ed7-4307-a194-81d7e7bbef12";

        protected ForceArtilleryBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalIceLance, name, guid)
        {

            Definition.GuiPresentation.Title = "Feature/&ForceArtilleryTitle";
            Definition.SetShortTitleOverride("Feature/&ForceArtilleryTitle");
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);

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

            DamageForm ForceArtillery = new DamageForm
            {
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 2,
                DamageType = RuleDefinitions.DamageTypeForce,
                BonusDamage = 0
            };

            EffectForm effect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm = (ForceArtillery)
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
            Definition.EffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower ForceArtillery = CreateAndAddToDB(ForceArtilleryName, ForceArtilleryGuid);


    }

    //*****************************************************************************************************************************************
    //***********************************		ForceArtillery_2Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class ForceArtillery_2Builder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string ForceArtillery_2Name = "ForceArtillery_2";
        private const string ForceArtillery_2Guid = "79e96b14-d276-4a33-8cab-7e884bcde120";

        protected ForceArtillery_2Builder(string name, string guid) : base(ForceArtilleryBuilder.ForceArtillery, name, guid)
        {

            Definition.GuiPresentation.Title = "Feat/&ForceArtillery_2Title";
            Definition.GuiPresentation.Description = "Feat/&ForceArtillery_2Description";

            Definition.EffectDescription.EffectForms[0].DamageForm.DiceNumber = 3;
            Definition.SetOverriddenPower(ForceArtilleryBuilder.ForceArtillery);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtillery_2Builder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower ForceArtillery_2 = CreateAndAddToDB(ForceArtillery_2Name, ForceArtillery_2Guid);


    }
    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class ForceArtilleryConstructBuilder : BaseDefinitionBuilder<MonsterDefinition>
    {
        private const string ForceArtilleryConstructName = "ForceArtilleryConstruct";
        private const string ForceArtilleryConstructGuid = "91cc706f-97b7-47d9-a46e-89b251fd5efe";

        protected ForceArtilleryConstructBuilder(string name, string guid) : base(DatabaseHelper.MonsterDefinitions.Magic_Mouth, name, guid)
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



            Definition.AttackIterations.Clear();


            MonsterAttackIteration monsterAttackIteration = new MonsterAttackIteration();

            Traverse.Create(monsterAttackIteration).Field("monsterAttackDefinition").SetValue(ForceArtilleryAttackBuilder.ForceArtilleryAttack);

            Traverse.Create(monsterAttackIteration).Field("number").SetValue(1);

            Definition.AttackIterations.AddRange(new List<MonsterAttackIteration> { monsterAttackIteration });


            Definition.CreatureTags.Add("ScalingTinkererArtilleryConstruct");



        }

        public static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryConstructBuilder(name, guid).AddToDB();
        }

        public static MonsterDefinition ForceArtilleryConstruct = CreateAndAddToDB(ForceArtilleryConstructName, ForceArtilleryConstructGuid);


    }


    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class ForceArtilleryConstruct_9Builder : BaseDefinitionBuilder<MonsterDefinition>
    {
        private const string ForceArtilleryConstruct_9Name = "ForceArtilleryConstruct_9";
        private const string ForceArtilleryConstruct_9Guid = "1a479ea4-0f72-4847-bd0b-54b2ded48057";

        protected ForceArtilleryConstruct_9Builder(string name, string guid) : base(ForceArtilleryConstructBuilder.ForceArtilleryConstruct, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ForceArtilleryConstructTitle_3";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryConstructDescription_3";

            // Definition.Features.Add(ForceArtillery_2Builder.ForceArtillery_2);
            Definition.Features.Add(ForceArtilleryAdditionalDamageBuilder.ForceArtilleryAdditionalDamage);

            Definition.Features.Add(SelfDestructBuilder.SelfDestruct);

        }

        public static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryConstruct_9Builder(name, guid).AddToDB();
        }

        public static MonsterDefinition ForceArtilleryConstruct_9 = CreateAndAddToDB(ForceArtilleryConstruct_9Name, ForceArtilleryConstruct_9Guid);


    }



    //*****************************************************************************************************************************************
    //***********************************		ForceArtilleryConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class ForceArtilleryConstruct_15Builder : BaseDefinitionBuilder<MonsterDefinition>
    {
        private const string ForceArtilleryConstruct_15Name = "ForceArtilleryConstruct_15";
        private const string ForceArtilleryConstruct_15Guid = "e7d49f53-cb44-4348-82d7-b8b561861448";

        protected ForceArtilleryConstruct_15Builder(string name, string guid) : base(ForceArtilleryConstruct_9Builder.ForceArtilleryConstruct_9, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ForceArtilleryConstructTitle_5";
            Definition.GuiPresentation.Description = "Feat/&ForceArtilleryConstructDescription_3";

            Definition.Features.Add(HalfCoverShieldBuilder.HalfCoverShield);

        }

        public static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryConstruct_15Builder(name, guid).AddToDB();
        }

        public static MonsterDefinition ForceArtilleryConstruct_15 = CreateAndAddToDB(ForceArtilleryConstruct_15Name, ForceArtilleryConstruct_15Guid);


    }


    //*****************************************************************************************************************************************
    //***********************************		SummonForceArtillerySpellConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SummonForceArtillerySpellConstructBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string SummonForceArtilleryConstructName = "SummonForceArtilleryConstruct";
        private const string SummonForceArtilleryConstructGuid = "c584b73c-9fa8-453f-90ad-944b8d1b5b05";

        protected SummonForceArtillerySpellConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.DancingLights, name, guid)
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

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonForceArtillerySpellConstructBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition SummonForceArtilleryConstruct = CreateAndAddToDB(SummonForceArtilleryConstructName, SummonForceArtilleryConstructGuid);

    }



    //*****************************************************************************************************************************************
    //***********************************		SummonForceArtillerySpellConstruct_9Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SummonForceArtillerySpellConstruct_9Builder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string SummonForceArtilleryConstruct_9Name = "SummonForceArtilleryConstruct_9";
        private const string SummonForceArtilleryConstruct_9Guid = "f1e8d7e1-44d9-4a82-ac23-5e0013b40650";

        protected SummonForceArtillerySpellConstruct_9Builder(string name, string guid) : base(SummonForceArtillerySpellConstructBuilder.SummonForceArtilleryConstruct, name, guid)
        {

            Definition.GuiPresentation.Title = "Feature/&ForceArtillery_09ModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ForceArtillery_09ModePowerDescription";

            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(ForceArtilleryConstruct_9Builder.ForceArtilleryConstruct_9.Name);
            //



        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonForceArtillerySpellConstruct_9Builder(name, guid).AddToDB();
        }

        public static SpellDefinition SummonForceArtilleryConstruct_9 = CreateAndAddToDB(SummonForceArtilleryConstruct_9Name, SummonForceArtilleryConstruct_9Guid);

    }



    //*****************************************************************************************************************************************
    //***********************************		SummonForceArtillerySpellConstruct_15Builder		*******************************************************************
    //*****************************************************************************************************************************************

    internal class SummonForceArtillerySpellConstruct_15Builder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string SummonForceArtilleryConstruct_15Name = "SummonForceArtilleryConstruct_15";
        private const string SummonForceArtilleryConstruct_15Guid = "b529386c-defa-4c39-a03c-09a08a104cc6";

        protected SummonForceArtillerySpellConstruct_15Builder(string name, string guid) : base(SummonForceArtillerySpellConstructBuilder.SummonForceArtilleryConstruct, name, guid)
        {

            Definition.GuiPresentation.Title = "Feature/&ForceArtillery_15ModePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ForceArtillery_15ModePowerDescription";
            Definition.SetUniqueInstance(false);

            Definition.EffectDescription.EffectForms[0].SummonForm.SetMonsterDefinitionName(ForceArtilleryConstruct_15Builder.ForceArtilleryConstruct_15.Name);





        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new SummonForceArtillerySpellConstruct_15Builder(name, guid).AddToDB();
        }

        public static SpellDefinition SummonForceArtilleryConstruct_15 = CreateAndAddToDB(SummonForceArtilleryConstruct_15Name, SummonForceArtilleryConstruct_15Guid);

    }



    internal class ForceArtilleryAttackBuilder : BaseDefinitionBuilder<MonsterAttackDefinition>
    {
        private const string ForceArtilleryAttackName = "ForceArtilleryAttack";
        private const string ForceArtilleryAttackGuid = "39c5b7ef-47f9-462c-9f24-accaee85d325";

        protected ForceArtilleryAttackBuilder(string name, string guid) : base(DatabaseHelper.MonsterAttackDefinitions.Attack_Orc_Grimblade_IceDagger, name, guid)
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
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games

            EffectForm effectmotion = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Motion
            };
            effectmotion.SetMotionForm(motionForm);
            effectmotion.SetCreatedByCharacter(true);
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            effectmotion.HasSavingThrow = false;
            effectmotion.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            effectmotion.SetLevelMultiplier(1);
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            effectmotion.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            effectmotion.SetApplyLevel(EffectForm.LevelApplianceType.No);

            DamageForm ForceArtilleryAttack = new DamageForm
            {
                DieType = RuleDefinitions.DieType.D8,
                DiceNumber = 2,
                DamageType = RuleDefinitions.DamageTypeForce,
                BonusDamage = 0
            };

            EffectForm effect = new EffectForm
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm = (ForceArtilleryAttack)
            };
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            effect.SetCreatedByCharacter(true);
            effect.HasSavingThrow = false;
            effect.AddBonusMode = RuleDefinitions.AddBonusMode.None;
            effect.SetLevelMultiplier(1);
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            effect.SetLevelType(RuleDefinitions.LevelSourceType.EffectLevel);
            effect.SetApplyLevel(EffectForm.LevelApplianceType.No);

            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(effect);
            Definition.EffectDescription.EffectForms.Add(effectmotion);
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.IndividualsUnique);
            Definition.EffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            Definition.EffectDescription.SetTargetParameter(1);
            Definition.EffectDescription.SetRangeParameter(24);
            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            Definition.EffectDescription.HasSavingThrow = false;
            Definition.EffectDescription.SetCreatedByCharacter(true);
            Definition.EffectDescription.SetCanBePlacedOnCharacter(true);

            // new comment to get past linter not liking the fact i'm trying to maintain old powers to avoid causing issues for peoples games
            Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);

            EffectParticleParameters effectParticleParameters = new EffectParticleParameters();
            effectParticleParameters.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);
            Definition.EffectDescription.SetEffectParticleParameters(effectParticleParameters);

        }

        public static MonsterAttackDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryAttackBuilder(name, guid).AddToDB();
        }

        public static MonsterAttackDefinition ForceArtilleryAttack = CreateAndAddToDB(ForceArtilleryAttackName, ForceArtilleryAttackGuid);


    }

    internal class ForceArtilleryAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        private const string ForceArtilleryAdditionalDamageName = "ForceArtilleryAdditionalDamage";
        private const string ForceArtilleryAdditionalDamageGuid = "2e726b2e-052f-482f-a869-721851fcb407";

        protected ForceArtilleryAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike, name, guid)
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

        public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryAdditionalDamageBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAdditionalDamage ForceArtilleryAdditionalDamage = CreateAndAddToDB(ForceArtilleryAdditionalDamageName, ForceArtilleryAdditionalDamageGuid);


    }




    internal class ForceArtilleryProjectileBuilder : BaseDefinitionBuilder<ItemDefinition>
    {
        private const string ForceArtilleryProjectileName = "ForceArtilleryProjectile";
        private const string ForceArtilleryProjectileGuid = "b30b1971-6ad6-4ea2-af5b-998043415f04";

        protected ForceArtilleryProjectileBuilder(string name, string guid) : base(DatabaseHelper.ItemDefinitions.OrcGrimblade_IceDagger, name, guid)
        {

            Definition.GuiPresentation.Title = "Item/&ForceArtilleryProjectileTitle";
            Definition.GuiPresentation.Description = "Item/&ForceArtilleryProjectileDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);

            EffectParticleParameters effectParticleParameters = new EffectParticleParameters();
            effectParticleParameters.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription.EffectParticleParameters);
            Definition.AmmunitionDescription.EffectDescription.SetEffectParticleParameters(effectParticleParameters);

        }

        public static ItemDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ForceArtilleryProjectileBuilder(name, guid).AddToDB();
        }

        public static ItemDefinition ForceArtilleryProjectile = CreateAndAddToDB(ForceArtilleryProjectileName, ForceArtilleryProjectileGuid);


    }
}

