using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionSavingThrowAffinity;

namespace SolastaCommunityExpansion.Feats
{
    internal static class AcehighFeats
    {
        private static bool initialized;

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            feats.Add(PowerAttackFeatBuilder.PowerAttackFeat);
            feats.Add(RecklessFuryFeatBuilder.RecklessFuryFeat);

            initialized = true;
        }

        public static void UpdatePowerAttackModifier()
        {
            if (!initialized)
            {
                return;
            }

            PowerAttackOneHandedAttackModifierBuilder.PowerAttackAttackModifier.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
            PowerAttackOneHandedAttackModifierBuilder.PowerAttackAttackModifier.SetDamageRollModifier(Main.Settings.FeatPowerAttackModifier);
            PowerAttackTwoHandedAttackModifierBuilder.PowerAttackTwoHandedAttackModifier.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
            PowerAttackTwoHandedAttackModifierBuilder.PowerAttackTwoHandedAttackModifier.SetDamageRollModifier(2 * Main.Settings.FeatPowerAttackModifier);
        }

        internal sealed class PowerAttackPowerBuilder : FeatureDefinitionPowerBuilder
        {
            private const string PowerAttackPowerName = "PowerAttack";
            private const string PowerAttackPowerNameGuid = "0a3e6a7d-4628-4189-b91d-d7146d774bb6";

            private PowerAttackPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackPowerTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feature/&PowerAttackPowerDescription", Main.Settings.FeatPowerAttackModifier.ToString());

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
                Definition.SetShortTitleOverride("Feature/&PowerAttackPowerTitle");

                //Create the power attack effect
                EffectForm powerAttackEffect = new EffectForm
                {
                    ConditionForm = new ConditionForm(),
                    FormType = EffectForm.EffectFormType.Condition
                };
                powerAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                powerAttackEffect.ConditionForm.ConditionDefinition = PowerAttackConditionBuilder.PowerAttackCondition;

                //Add to our new effect
                EffectDescription newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(powerAttackEffect);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
                newEffectDescription.DurationParameter = 0;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackPowerBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionPower PowerAttackPower = CreateAndAddToDB(PowerAttackPowerName, PowerAttackPowerNameGuid);
        }

        internal sealed class PowerAttackTwoHandedPowerBuilder : FeatureDefinitionPowerBuilder
        {
            private const string PowerAttackTwoHandedPowerName = "PowerAttackTwoHanded";
            private const string PowerAttackTwoHandedPowerNameGuid = "b45b8467-7caa-428e-b4b5-ba3c4a153f07";

            private PowerAttackTwoHandedPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackTwoHandedPowerTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feature/&PowerAttackTwoHandedPowerDescription", Main.Settings.FeatPowerAttackModifier.ToString(), (Main.Settings.FeatPowerAttackModifier * 2).ToString());

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
                Definition.SetShortTitleOverride("Feature/&PowerAttackTwoHandedPowerTitle");

                //Create the power attack effect
                EffectForm powerAttackEffect = new EffectForm
                {
                    ConditionForm = new ConditionForm(),
                    FormType = EffectForm.EffectFormType.Condition
                };
                powerAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                powerAttackEffect.ConditionForm.ConditionDefinition = PowerAttackTwoHandedConditionBuilder.PowerAttackTwoHandedCondition;

                //Add to our new effect
                EffectDescription newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(powerAttackEffect);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Round;
                newEffectDescription.DurationParameter = 0;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackTwoHandedPowerBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionPower PowerAttackTwoHandedPower = CreateAndAddToDB(PowerAttackTwoHandedPowerName, PowerAttackTwoHandedPowerNameGuid);
        }

        internal sealed class PowerAttackOneHandedAttackModifierBuilder : FeatureDefinitionAttackModifierBuilder
        {
            private const string PowerAttackAttackModifierName = "PowerAttackAttackModifier";
            private const string PowerAttackAttackModifierNameGuid = "87286627-3e62-459d-8781-ceac1c3462e6";

            private PowerAttackOneHandedAttackModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackAttackModifierTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feature/&PowerAttackAttackModifierDescription", Main.Settings.FeatPowerAttackModifier.ToString());

                //Ideally this would be proficiency but there isn't a nice way to subtract proficiency.
                //To do this properly you could likely make multiple versions of this that get replaced at proficiency level ups but it's a bit of a pain, so going with -3 for now.
                //Originally I made an implemenation that used FeatureDefinitionAdditionalDamage and was going to restrict to melee weapons etc. but really power attack should be avaiable for any build as you choose.
                //The FeatureDefinitionAdditionalDamage was limited in the sense that you couldn't check for things like the 'TwoHanded' or 'Heavy' properties of a weapon so it wasn't worth using really.
                Definition.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
                Definition.SetDamageRollModifier(Main.Settings.FeatPowerAttackModifier);
            }

            private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackOneHandedAttackModifierBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionAttackModifier PowerAttackAttackModifier
                = CreateAndAddToDB(PowerAttackAttackModifierName, PowerAttackAttackModifierNameGuid);
        }

        internal sealed class PowerAttackTwoHandedAttackModifierBuilder : FeatureDefinitionAttackModifierBuilder
        {
            private const string PowerAttackTwoHandedAttackModifierName = "PowerAttackTwoHandedAttackModifier";
            private const string PowerAttackTwoHandedAttackModifierNameGuid = "b1b05940-7558-4f03-98d1-01f616b5ae25";

            private PowerAttackTwoHandedAttackModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackTwoHandedAttackModifierTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feature/&PowerAttackTwoHandedAttackModifierDescription", Main.Settings.FeatPowerAttackModifier.ToString(), (Main.Settings.FeatPowerAttackModifier * 2).ToString());

                //Ideally this would be proficiency but there isn't a nice way to subtract proficiency.
                //To do this properly you could likely make multiple versions of this that get replaced at proficiency level ups but it's a bit of a pain, so going with -3 for now.
                //Originally I made an implemenation that used FeatureDefinitionAdditionalDamage and was going to restrict to melee weapons etc. but really power attack should be avaiable for any build as you choose.
                //The FeatureDefinitionAdditionalDamage was limited in the sense that you couldn't check for things like the 'TwoHanded' or 'Heavy' properties of a weapon so it wasn't worth using really.
                Definition.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
                Definition.SetDamageRollModifier(2 * Main.Settings.FeatPowerAttackModifier);
            }

            private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackTwoHandedAttackModifierBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionAttackModifier PowerAttackTwoHandedAttackModifier = CreateAndAddToDB(PowerAttackTwoHandedAttackModifierName, PowerAttackTwoHandedAttackModifierNameGuid);
        }

        internal sealed class PowerAttackConditionBuilder : ConditionDefinitionBuilder
        {
            private const string PowerAttackConditionName = "PowerAttackCondition";
            private const string PowerAttackConditionNameGuid = "c125b7b9-e668-4c6f-a742-63c065ad2292";

            private PowerAttackConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackConditionTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feature/&PowerAttackConditionDescription", Main.Settings.FeatPowerAttackModifier.ToString());

                Definition.SetAllowMultipleInstances(false);
                Definition.Features.Clear();
                Definition.Features.Add(PowerAttackOneHandedAttackModifierBuilder.PowerAttackAttackModifier);

                Definition.SetDurationType(RuleDefinitions.DurationType.Round);
                Definition.SetDurationParameter(0);
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackConditionBuilder(name, guid).AddToDB();
            }

            public static readonly ConditionDefinition PowerAttackCondition = CreateAndAddToDB(PowerAttackConditionName, PowerAttackConditionNameGuid);
        }

        internal sealed class PowerAttackTwoHandedConditionBuilder : ConditionDefinitionBuilder
        {
            private const string PowerAttackTwoHandedConditionName = "PowerAttackTwoHandedCondition";
            private const string PowerAttackTwoHandedConditionNameGuid = "7d0eecbd-9ad8-4915-a3f7-cfa131001fe6";

            private PowerAttackTwoHandedConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackTwoHandedConditionTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feature/&PowerAttackTwoHandedConditionDescription", Main.Settings.FeatPowerAttackModifier.ToString(), (Main.Settings.FeatPowerAttackModifier * 2).ToString());

                Definition.SetAllowMultipleInstances(false);
                Definition.Features.Clear();
                Definition.Features.Add(PowerAttackTwoHandedAttackModifierBuilder.PowerAttackTwoHandedAttackModifier);

                Definition.SetDurationType(RuleDefinitions.DurationType.Round);
                Definition.SetDurationParameter(0);
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackTwoHandedConditionBuilder(name, guid).AddToDB();
            }

            public static readonly ConditionDefinition PowerAttackTwoHandedCondition = CreateAndAddToDB(PowerAttackTwoHandedConditionName, PowerAttackTwoHandedConditionNameGuid);
        }

        internal sealed class PowerAttackFeatBuilder : FeatDefinitionBuilder
        {
            private const string PowerAttackFeatName = "PowerAttackFeat";
            private const string PowerAttackFeatNameGuid = "88f1fb27-66af-49c6-b038-a38142b1083e";

            private PowerAttackFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
            {
                Definition.GuiPresentation.Title = "Feat/&PowerAttackFeatTitle";
                Definition.GuiPresentation.Description = Gui.Format("Feat/&PowerAttackFeatDescription", Main.Settings.FeatPowerAttackModifier.ToString(), (Main.Settings.FeatPowerAttackModifier * 2).ToString());

                Definition.Features.Clear();
                Definition.Features.Add(PowerAttackPowerBuilder.PowerAttackPower);
                Definition.Features.Add(PowerAttackTwoHandedPowerBuilder.PowerAttackTwoHandedPower);
                Definition.SetMinimalAbilityScorePrerequisite(false);
            }

            private static FeatDefinition CreateAndAddToDB(string name, string guid)
            {
                return new PowerAttackFeatBuilder(name, guid).AddToDB();
            }

            public static readonly FeatDefinition PowerAttackFeat = CreateAndAddToDB(PowerAttackFeatName, PowerAttackFeatNameGuid);
        }

        internal sealed class RecklessFuryFeatBuilder : FeatDefinitionBuilder
        {
            private const string RecklessFuryFeatName = "RecklessFuryFeat";
            private const string RecklessFuryFeatNameGuid = "78c5fd76-e25b-499d-896f-3eaf84c711d8";

            private RecklessFuryFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
            {
                Definition.GuiPresentation.Title = "Feat/&RecklessFuryFeatTitle";
                Definition.GuiPresentation.Description = "Feat/&RecklessFuryFeatDescription";

                Definition.Features.Clear();
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionPowers.PowerReckless);
                Definition.Features.Add(RagePowerBuilder.RagePower);
                Definition.SetMinimalAbilityScorePrerequisite(false);
            }

            private static FeatDefinition CreateAndAddToDB(string name, string guid)
            {
                return new RecklessFuryFeatBuilder(name, guid).AddToDB();
            }

            public static readonly FeatDefinition RecklessFuryFeat = CreateAndAddToDB(RecklessFuryFeatName, RecklessFuryFeatNameGuid);
        }

        internal sealed class RagePowerBuilder : FeatureDefinitionPowerBuilder
        {
            private const string RagePowerName = "AHRagePower";
            private const string RagePowerNameGuid = "a46c1722-7825-4a81-bca1-392b51cd7d97";

            private RagePowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RagePowerTitle";
                Definition.GuiPresentation.Description = "Feature/&RagePowerDescription";

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
                Definition.SetCostPerUse(1);
                Definition.SetFixedUsesPerRecharge(1);
                Definition.SetShortTitleOverride("Feature/&RagePowerTitle");

                //Create the power attack effect
                EffectForm rageEffect = new EffectForm
                {
                    ConditionForm = new ConditionForm
                    {
                        Operation = ConditionForm.ConditionOperation.Add,
                        ConditionDefinition = RageFeatConditionBuilder.RageFeatCondition
                    },
                    FormType = EffectForm.EffectFormType.Condition
                };

                //Add to our new effect
                EffectDescription newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(rageEffect);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Minute;
                newEffectDescription.DurationParameter = 1;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            {
                return new RagePowerBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionPower RagePower = CreateAndAddToDB(RagePowerName, RagePowerNameGuid);
        }

        internal sealed class RageFeatConditionBuilder : ConditionDefinitionBuilder
        {
            private const string RageFeatConditionName = "AHRageFeatCondition";
            private const string RageFeatConditionNameGuid = "2f34fb85-6a5d-4a4e-871b-026872bc24b8";

            private RageFeatConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RageFeatConditionTitle";
                Definition.GuiPresentation.Description = "Feature/&RageFeatConditionDescription";

                Definition.SetAllowMultipleInstances(false);
                Definition.Features.Clear();
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance);
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance);
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength);
                Definition.Features.Add(RageStrengthSavingThrowAffinityBuilder.RageStrengthSavingThrowAffinity);
                Definition.Features.Add(RageDamageBonusAttackModifierBuilder.RageDamageBonusAttackModifier);
                Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
                Definition.SetDurationParameter(1);
            }

            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new RageFeatConditionBuilder(name, guid).AddToDB();
            }

            public static readonly ConditionDefinition RageFeatCondition = CreateAndAddToDB(RageFeatConditionName, RageFeatConditionNameGuid);
        }

        internal sealed class RageStrengthSavingThrowAffinityBuilder : FeatureDefinitionSavingThrowAffinityBuilder
        {
            private const string RageStrengthSavingThrowAffinityName = "AHRageStrengthSavingThrowAffinity";
            private const string RageStrengthSavingThrowAffinityNameGuid = "17d26173-7353-4087-a295-96e1ec2e6cd4";

            private RageStrengthSavingThrowAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RageStrengthSavingThrowAffinityTitle";
                Definition.GuiPresentation.Description = "Feature/&RageStrengthSavingThrowAffinityDescription";

                Definition.AffinityGroups.Clear();

                Definition.AffinityGroups.Add(
                    new SavingThrowAffinityGroup
                    {
                        affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                        abilityScoreName = "Strength"
                    });
            }

            private static FeatureDefinitionSavingThrowAffinity CreateAndAddToDB(string name, string guid)
            {
                return new RageStrengthSavingThrowAffinityBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionSavingThrowAffinity RageStrengthSavingThrowAffinity = CreateAndAddToDB(RageStrengthSavingThrowAffinityName, RageStrengthSavingThrowAffinityNameGuid);
        }

        internal sealed class RageDamageBonusAttackModifierBuilder : FeatureDefinitionAttackModifierBuilder
        {
            private const string RageDamageBonusAttackModifierName = "AHRageDamageBonusAttackModifier";
            private const string RageDamageBonusAttackModifierNameGuid = "7bc1a47e-9519-4a37-a89a-10bcfa83e48a";

            private RageDamageBonusAttackModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RageDamageBonusAttackModifierTitle";
                Definition.GuiPresentation.Description = "Feature/&RageDamageBonusAttackModifierDescription";

                //Currently works with ranged weapons, in the end it's fine.
                Definition.SetAttackRollModifier(0);
                Definition.SetDamageRollModifier(2);//Could find a way to up this at level 9 to match barb but that seems like a lot of work right now :)
            }

            private static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
            {
                return new RageDamageBonusAttackModifierBuilder(name, guid).AddToDB();
            }

            public static readonly FeatureDefinitionAttackModifier RageDamageBonusAttackModifier = CreateAndAddToDB(RageDamageBonusAttackModifierName, RageDamageBonusAttackModifierNameGuid);
        }
    }
}
