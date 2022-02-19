using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Classes.Tinkerer.Subclasses
{
    ////*****************************************************************************************************************************************
    ////***********************************		TinkererConstructFamilyBuilder		*************************************************************
    ////*****************************************************************************************************************************************

    internal sealed class TinkererConstructFamilyBuilder : CharacterFamilyDefinitionBuilder
    {
        private const string TinkererConstructFamilyName = "TinkererConstruct";
        private const string TinkererConstructFamilyGuid = "ab9d8ea6-3cc2-4c36-939a-b9a43bad023e";

        private TinkererConstructFamilyBuilder(string name, string guid) : base(DatabaseHelper.CharacterFamilyDefinitions.Construct, name, guid)
        {
            Definition.SetExtraplanar(true);
        }

        private static CharacterFamilyDefinition CreateAndAddToDB(string name, string guid)
        {
            return new TinkererConstructFamilyBuilder(name, guid).AddToDB();
        }

        public static readonly CharacterFamilyDefinition TinkererConstructFamily = CreateAndAddToDB(TinkererConstructFamilyName, TinkererConstructFamilyGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		AddConstructCantripsBuilder		*******************************************************
    //*****************************************************************************************************************************************

    internal sealed class AddConstructCantripsBuilder : FeatureDefinitionBonusCantripsBuilder
    {
        private const string Name = "AddConstructCantrips";
        private const string Guid = "942183c0-e581-464a-afe4-cc00a9cd9c26";

        private AddConstructCantripsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainOblivion, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&AddConstructCantripsTitle";
            Definition.GuiPresentation.Description = "Feat/&AddConstructCantripsDescription";

            Definition.BonusCantrips.Clear();
            Definition.BonusCantrips.Add(MendingConstructBuilder.MendingConstruct);
            Definition.BonusCantrips.Add(DismissConstructBuilder.DismissConstruct);
        }

        private static FeatureDefinitionBonusCantrips CreateAndAddToDB(string name, string guid)
        {
            return new AddConstructCantripsBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionBonusCantrips AddConstructCantrips = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		MendingConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class MendingConstructBuilder : SpellDefinitionBuilder
    {
        private const string Name = "MendingConstruct";
        private const string Guid = "92de3dbf-4b57-46d3-aebf-0f7819b0ac2d";

        private MendingConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.PrayerOfHealing, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&MendingConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&MendingConstructDescription";

            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.IndividualsUnique);
            Definition.EffectDescription.SetTargetParameter(1);
            Definition.EffectDescription.SetRangeType(RuleDefinitions.RangeType.Touch);

            HealingForm mendingconstruct = new HealingForm
            {
                BonusHealing = 0,
                DieType = RuleDefinitions.DieType.D6,
                DiceNumber = 2
            };

            EffectForm effect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Healing
            };
            effect.SetHealingForm(mendingconstruct);
            effect.SetCreatedByCharacter(true);

            Definition.SetRitual(false);
            Definition.SetSpellLevel(0);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Minute1);
            Definition.EffectDescription.EffectAdvancement.Clear();
            Definition.EffectDescription.EffectForms.Clear();
            Definition.EffectDescription.EffectForms.Add(effect);
            Definition.EffectDescription.RestrictedCreatureFamilies.Add(TinkererConstructFamilyBuilder.TinkererConstructFamily.Name);
            Definition.EffectDescription.ImmuneCreatureFamilies.Clear();
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new MendingConstructBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition MendingConstruct = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		DismissConstructBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class DismissConstructBuilder : SpellDefinitionBuilder
    {
        private const string Name = "DismissConstruct";
        private const string Guid = "8003917a-9c90-4748-bb2f-f32b7edf8844";

        private DismissConstructBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.Banishment, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&DismissConstructTitle";
            Definition.GuiPresentation.Description = "Feat/&DismissConstructDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.AnimalFriendship.GuiPresentation.SpriteReference);

            Definition.SetSpellLevel(0);
            Definition.SetRequiresConcentration(false);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetSomaticComponent(false);
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.None);
            Definition.SetVerboseComponent(false);

            CounterForm dismissConstruct = new CounterForm();
            dismissConstruct.SetType(CounterForm.CounterType.DismissCreature);

            EffectForm effect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Counter
            };
            effect.SetCounterForm(dismissConstruct);
            effect.HasSavingThrow = false;
            effect.SetCreatedByCharacter(true);

            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(Definition.EffectDescription);
            effectDescription.EffectForms.Clear();
            effectDescription.HasSavingThrow = false;
            effectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            effectDescription.DurationParameter = 1;
            effectDescription.DurationType = RuleDefinitions.DurationType.Permanent;
            effectDescription.EffectForms.Add(effect);
            effectDescription.EffectAdvancement.Clear();

            effectDescription.RestrictedCreatureFamilies.Add(TinkererConstructFamilyBuilder.TinkererConstructFamily.Name);

            effectDescription.ImmuneCreatureFamilies.AddRange(
                DatabaseHelper.CharacterFamilyDefinitions.Aberration.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Beast.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Celestial.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Construct.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Dragon.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Elemental.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Fey.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Giant.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Humanoid.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Monstrosity.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Ooze.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Plant.Name,
                DatabaseHelper.CharacterFamilyDefinitions.Undead.Name
            );

            Definition.SetEffectDescription(effectDescription);
        }

        private static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DismissConstructBuilder(name, guid).AddToDB();
        }

        public static readonly SpellDefinition DismissConstruct = CreateAndAddToDB(Name, Guid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ArtificialServantBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class ArtificialServantBuilder : MonsterDefinitionBuilder
    {
        private const string ArtificialServantName = "ArtificialServant";
        private const string ArtificialServantGuid = "fce9181c-f62c-4b33-b0df-fff4fe3ceab2";

        private ArtificialServantBuilder(string name, string guid) : base(DatabaseHelper.MonsterDefinitions.Fire_Jester, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ArtificialServantTitle";
            Definition.GuiPresentation.Description = "Feat/&ArtificialServantDescription";
            Definition.MonsterPresentation.SetUniqueNameTitle("Feat/&ArtificialServantTitle");
            Definition.MonsterPresentation.SetHasMonsterPortraitBackground(true);
            Definition.MonsterPresentation.SetCanGeneratePortrait(true);
            Definition.SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None);

            Definition.SetArmorClass(13);
            Definition.SetNoExperienceGain(true);
            Definition.SetHitDice(3);
            Definition.SetHitDiceType(RuleDefinitions.DieType.D4);

            Definition.AbilityScores.Empty();
            Definition.AbilityScores.AddToArray(4);    // STR
            Definition.AbilityScores.AddToArray(15);    // DEX
            Definition.AbilityScores.AddToArray(12);    // CON
            Definition.AbilityScores.AddToArray(10);     // INT
            Definition.AbilityScores.AddToArray(10);    // WIS
            Definition.AbilityScores.AddToArray(7);     // CHA

            //assume PB=4

            Definition.SavingThrowScores.SetRange(
                new MonsterSavingThrowProficiency(AttributeDefinitions.Dexterity, 2));

            Definition.SkillScores.SetRange(
                new MonsterSkillProficiency(SkillDefinitions.Stealth, 2),
                new MonsterSkillProficiency(SkillDefinitions.Perception, 4));

            Definition.SetFullyControlledWhenAllied(true);
            Definition.SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None);
            Definition.SetStandardHitPoints(10);
            Definition.SetDefaultFaction("Party");
            Definition.SetCharacterFamily(TinkererConstructFamilyBuilder.TinkererConstructFamily.Name);

            // a tag should be added if scaling is applied to the servant
            //Definition.CreatureTags.Add();

            Definition.SetDefaultBattleDecisionPackage(DatabaseHelper.DecisionPackageDefinitions.DefaultRangeWithBackupMeleeDecisions);
            Definition.Features.Clear();
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision12);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionBootsWinged);
            Definition.Features.Add(CancelFlyingConditionBuilder.CancelFlyingCondition);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityExhaustionImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity);
            Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity);
            // Definition.Features.Add(DatabaseHelper.);

            Definition.AttackIterations.SetRange(new MonsterAttackIteration(ArtificialServantAttackBuilder.ArtificialServantAttack, 1));

            Definition.MonsterPresentation.SetMalePrefabReference(new UnityEngine.AddressableAssets.AssetReference("ab0501343e8629149ae0aa4dace755f5"));
            Definition.MonsterPresentation.SetFemalePrefabReference(new UnityEngine.AddressableAssets.AssetReference("ab0501343e8629149ae0aa4dace755f5"));
            Definition.MonsterPresentation.SetMaleModelScale(0.2f);
            Definition.MonsterPresentation.SetFemaleModelScale(0.2f);
        }

        private static MonsterDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ArtificialServantBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterDefinition ArtificialServant = CreateAndAddToDB(ArtificialServantName, ArtificialServantGuid);
    }

    //*****************************************************************************************************************************************
    //***********************************		ArtificialServantAttacksListBuilder		*******************************************************************
    //*****************************************************************************************************************************************

    internal sealed class ArtificialServantAttackBuilder : MonsterAttackDefinitionBuilder
    {
        private const string ArtificialServantAttacksListName = "ArtificialServantAttacksList";
        private const string ArtificialServantAttacksListGuid = "86840282-4d84-44b7-a4fd-6bf6b598f776";

        private ArtificialServantAttackBuilder(string name, string guid) : base(DatabaseHelper.MonsterAttackDefinitions.Attack_Goblin_PebbleThrow, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ArtificialServantAttackTitle";
            Definition.GuiPresentation.Description = "Feat/&ArtificialServantAttackDescription";

            EffectForm damageEffect = new EffectForm
            {
                AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus,
                DamageForm = new DamageForm
                {
                    DiceNumber = 1,
                    DieType = RuleDefinitions.DieType.D4
                }
            };

            const int assumedIntModifier = 3;
            const int assumedProficiencyBonus = 2;
            damageEffect.DamageForm.BonusDamage = assumedProficiencyBonus;
            damageEffect.DamageForm.DamageType = "DamageForce";

            //Add to our new effect
            EffectDescription newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(damageEffect);
            newEffectDescription.HasSavingThrow = false;
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            newEffectDescription.SetTargetProximityDistance(6);
            newEffectDescription.SetCanBePlacedOnCharacter(true);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);
            newEffectDescription.SetRangeParameter(6);

            // newEffectDescription.SetEffectParticleParameters(DatabaseHelper.SpellDefinitions.ShadowDagger.EffectDescription.EffectParticleParameters);
            newEffectDescription.SetEffectParticleParameters(DatabaseHelper.MonsterAttackDefinitions.Attack_Goblin_PebbleThrow.EffectDescription.EffectParticleParameters);

            Definition.SetEffectDescription(newEffectDescription);

            Definition.SetToHitBonus(assumedIntModifier + assumedProficiencyBonus);
        }

        private static MonsterAttackDefinition CreateAndAddToDB(string name, string guid)
        {
            return new ArtificialServantAttackBuilder(name, guid).AddToDB();
        }

        public static readonly MonsterAttackDefinition ArtificialServantAttack = CreateAndAddToDB(ArtificialServantAttacksListName, ArtificialServantAttacksListGuid);
    }

    internal sealed class CancelFlyingConditionBuilder : FeatureDefinitionPowerBuilder
    {
        private const string Name = "CancelFlyingConditionArtificialServant";
        private const string Guid = "15bff3c5-632e-451f-8c46-1511ed4cf805";

        private CancelFlyingConditionBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionBootsWinged, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&CancelFlyingConditionTitle";
            Definition.GuiPresentation.Description = "Feat/&CancelFlyingConditionDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.ExpeditiousRetreat.GuiPresentation.SpriteReference);

            ConditionForm Condition = new ConditionForm();
            Condition.SetApplyToSelf(false);
            Condition.SetForceOnSelf(false);
            Condition.Operation = ConditionForm.ConditionOperation.Remove;
            Condition.SetConditionDefinitionName(DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged.Name);
            Condition.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionFlyingBootsWinged;

            EffectForm effect = new EffectForm();
            effect.SetApplyLevel(EffectForm.LevelApplianceType.No);
            effect.SetLevelMultiplier(1);
            effect.SetLevelType(RuleDefinitions.LevelSourceType.None);
            effect.SetCreatedByCharacter(true);
            effect.FormType = EffectForm.EffectFormType.Condition;
            effect.ConditionForm = Condition;
            effect.SetCanSaveToCancel(false);

            EffectDescription effectDescription = new EffectDescription();
            effectDescription.Copy(Definition.EffectDescription);
            effectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
            effectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
            effectDescription.DurationType = RuleDefinitions.DurationType.Permanent;
            effectDescription.DurationParameter = 1;
            effectDescription.HasSavingThrow = false;
            effectDescription.EffectAdvancement.Clear();
            effectDescription.EffectForms.Clear();
            effectDescription.EffectForms.Add(effect);

            Definition.SetEffectDescription(effectDescription);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new CancelFlyingConditionBuilder(name, guid).AddToDB();
        }

        public static readonly FeatureDefinitionPower CancelFlyingCondition = CreateAndAddToDB(Name, Guid);
    }
}
