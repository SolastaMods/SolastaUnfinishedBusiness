using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityModManagerNet;
using HarmonyLib;
using I2.Loc;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using SolastaModApi.Diagnostics;

namespace SolastaCommunityExpansion.Subclasses.Fighter
{
    class Tactician : AbstractSubclass
    {
        private CharacterSubclassDefinition Subclass;
        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            if (Subclass == null)
            {
                Subclass = TacticianFighterSubclassBuilder.BuildAndAddSubclass();
            }
            return Subclass;
        }
    }

    internal class KnockDownPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string KnockDownPowerName = "KnockDownPower";
        const string KnockDownPowerNameGuid = "90dd5e81-40d7-4824-89b4-45bcf4c05218";

        protected KnockDownPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterActionSurge, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&KnockDownPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&KnockDownPowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ChannelDivinity);
            Definition.SetCostPerUse(1);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.OnAttackHit);

            //Create the damage form - TODO make it do the same damage as the wielded weapon?  This doesn't seem possible
            EffectForm damageEffect = new EffectForm();
            damageEffect.DamageForm = new DamageForm();
            damageEffect.DamageForm.DiceNumber = 1;
            damageEffect.DamageForm.DieType = RuleDefinitions.DieType.D6;
            damageEffect.DamageForm.BonusDamage = 2;
            damageEffect.DamageForm.DamageType = "DamageBludgeoning";
            damageEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            //Create the prone effect - Weirdly enough the motion form seems to also automatically apply the prone condition
            EffectForm proneMotionEffect = new EffectForm();
            proneMotionEffect.FormType = EffectForm.EffectFormType.Motion;
            var proneMotion = new MotionForm();
            proneMotion.SetType(MotionForm.MotionType.FallProne);
            proneMotion.SetDistance(1);
            proneMotionEffect.SetMotionForm(proneMotion);
            proneMotionEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);
            newEffectDescription.EffectForms.Add(proneMotionEffect);
            newEffectDescription.SetSavingThrowDifficultyAbility("Strength");
            newEffectDescription.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            newEffectDescription.SavingThrowAbility = "Strength";
            newEffectDescription.HasSavingThrow = true;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Turn;

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new KnockDownPowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower KnockDownPower = CreateAndAddToDB(KnockDownPowerName, KnockDownPowerNameGuid);
    }


    internal class InspirePowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string InspirePowerName = "InspirePower";
        const string InspirePowerNameGuid = "163c28de-48e5-4f75-bdd0-d42374a75ef8";

        protected InspirePowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&InspirePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&InspirePowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ChannelDivinity);
            Definition.SetCostPerUse(1);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetShortTitleOverride("Feature/&InspirePowerTitle");

            //Create the temp hp form
            EffectForm healingEffect = new EffectForm();
            healingEffect.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
            var tempHPForm = new TemporaryHitPointsForm();
            tempHPForm.DiceNumber = 1;
            tempHPForm.DieType = RuleDefinitions.DieType.D6;
            tempHPForm.BonusHitPoints = 2;
            healingEffect.SetTemporaryHitPointsForm(tempHPForm);

            //Create the bless effect - A fun test, unfortunately the two effects can't have varying durations AFAIK so a bless or similar effect might be overpowered (was thinking a bless for 1 round).  Alternatively both could last 1 minute instead and be intended for in battle.
            //EffectForm blessEffect = new EffectForm();
            //blessEffect.ConditionForm = new ConditionForm();
            //blessEffect.FormType = EffectForm.EffectFormType.Condition;
            //blessEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            //blessEffect.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionBlessed;

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(healingEffect);
            //newEffectDescription.EffectForms.Add(blessEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.DurationType = RuleDefinitions.DurationType.Day;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            newEffectDescription.SetTargetProximityDistance(12);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new InspirePowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower InspirePower = CreateAndAddToDB(InspirePowerName, InspirePowerNameGuid);
    }


    internal class CounterStrikePowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string CounterStrikePowerName = "CounterStrikePower";
        const string CounterStrikePowerNameGuid = "88c294ce-14fa-4f7e-8b81-ea4d289e3d8b";

        protected CounterStrikePowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&CounterStrikePowerTitle";
            Definition.GuiPresentation.Description = "Feature/&CounterStrikePowerDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ChannelDivinity);
            Definition.SetCostPerUse(1);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Reaction);
            Definition.SetReactionContext(RuleDefinitions.ReactionTriggerContext.HitByMelee);

            //Create the damage form - TODO make it do the same damage as the wielded weapon (seems impossible with current tools, would need to use the AdditionalDamage feature but I'm not sure how to combine that with this to make it a reaction ability).
            EffectForm damageEffect = new EffectForm();
            damageEffect.DamageForm = new DamageForm();
            damageEffect.DamageForm.DiceNumber = 1;
            damageEffect.DamageForm.DieType = RuleDefinitions.DieType.D6;
            damageEffect.DamageForm.BonusDamage = 2;
            damageEffect.DamageForm.DamageType = "DamageBludgeoning";
            damageEffect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            => new CounterStrikePowerBuilder(name, guid).AddToDB();

        public static FeatureDefinitionPower CounterStrikePower = CreateAndAddToDB(CounterStrikePowerName, CounterStrikePowerNameGuid);
    }


    internal class GambitResourcePoolBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        const string GambitResourcePoolName = "GambitResourcePool";
        const string GambitResourcePoolNameGuid = "00da2b27-139a-4ca0-a285-aaa70d108bc8";

        protected GambitResourcePoolBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierClericChannelDivinity, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&GambitResourcePoolTitle";
            Definition.GuiPresentation.Description = "Feature/&GambitResourcePoolDescription";

            //Re-uses Channel divinity pool - For some reason when I set the number to 4 it didn't take so I made the GambitResourcePoolAddBuilder
        }

        public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            => new GambitResourcePoolBuilder(name, guid).AddToDB();

        public static FeatureDefinitionAttributeModifier GambitResourcePool = CreateAndAddToDB(GambitResourcePoolName, GambitResourcePoolNameGuid);
    }

    internal class GambitResourcePoolAddBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        const string GambitResourcePoolAddName = "GambitResourcePoolAdd";
        const string GambitResourcePoolAddNameGuid = "056d786a-2611-4981-a652-704fa5056375";

        protected GambitResourcePoolAddBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierClericChannelDivinityAdd, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&GambitResourcePoolAddTitle";
            Definition.GuiPresentation.Description = "Feature/&GambitResourcePoolAddDescription";
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
        }

        public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            => new GambitResourcePoolAddBuilder(name, guid).AddToDB();

        public static FeatureDefinitionAttributeModifier GambitResourcePoolAdd = CreateAndAddToDB(GambitResourcePoolAddName, GambitResourcePoolAddNameGuid);
    }

    internal class GambitResourcePoolAdd3Builder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        const string GambitResourcePoolAddName = "GambitResourcePoolAdd3";
        const string GambitResourcePoolAddNameGuid = "b56ff27c-cd19-4ccf-a307-9e86a7001930";

        protected GambitResourcePoolAdd3Builder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierClericChannelDivinityAdd, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&GambitResourcePoolAdd3Title";
            Definition.GuiPresentation.Description = "Feature/&GambitResourcePoolAdd3Description";
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
            Definition.SetModifierValue(3);
        }

        public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
            => new GambitResourcePoolAdd3Builder(name, guid).AddToDB();

        public static FeatureDefinitionAttributeModifier GambitResourcePoolAdd3 = CreateAndAddToDB(GambitResourcePoolAddName, GambitResourcePoolAddNameGuid);
    }


    public static class TacticianFighterSubclassBuilder
    {
        //Unfortunate copy paste error.  Going to leave it incorrect here to not give errors for people that already have a previously release.
        //It doesn't run into issues with the  actual GambitResourcePool since it's in a different DB.
        const string TacticianFighterSubclassName = "GambitResourcePool";
        const string TacticianFighterSubclassNameGuid = "00da2b27-139a-4ca0-a285-aaa70d108bc8";

        public static CharacterSubclassDefinition BuildAndAddSubclass()
        {
            SetupPowers();
            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&TactitionFighterSubclassDescription",
                    "Subclass/&TactitionFighterSubclassTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(TacticianFighterSubclassName, TacticianFighterSubclassNameGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(GambitResourcePool, 3)
                    .AddFeatureAtLevel(GambitResourcePoolAdd3, 3)
                    .AddFeatureAtLevel(KnockDownPower, 3)
                    .AddFeatureAtLevel(InspirePower, 3)
                    .AddFeatureAtLevel(CounterStrikePower, 3)
                    .AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetChampionRemarkableAthlete, 7) //Wasn't sure what to do for level mostly a ribbon feature
                    .AddFeatureAtLevel(GambitResourcePoolAdd, 10)
                    .AddToDB();

            return definition;
        }

        public static void SetupPowers()
        {
            KnockDownPower = KnockDownPowerBuilder.KnockDownPower;
            InspirePower = InspirePowerBuilder.InspirePower;
            CounterStrikePower = CounterStrikePowerBuilder.CounterStrikePower;
            GambitResourcePool = GambitResourcePoolBuilder.GambitResourcePool;
            GambitResourcePoolAdd = GambitResourcePoolAddBuilder.GambitResourcePoolAdd;
            GambitResourcePoolAdd3 = GambitResourcePoolAdd3Builder.GambitResourcePoolAdd3;
        }

        public static FeatureDefinitionPower KnockDownPower;
        public static FeatureDefinitionPower InspirePower;
        public static FeatureDefinitionPower CounterStrikePower;
        public static FeatureDefinition GambitResourcePool;
        public static FeatureDefinition GambitResourcePoolAdd;
        public static FeatureDefinition GambitResourcePoolAdd3;
    }
}
