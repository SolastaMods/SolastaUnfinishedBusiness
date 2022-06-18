// Decompiled with JetBrains decompiler
// Type: SolastaModApi.Main
// Assembly: OathOfRetribution_springupdate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0C31DE3-014A-4A7A-8428-F8DEFE001310
// Assembly location: C:\Users\paulo\Downloads\OathOfRetribution.dll

using System;
using System.Collections.Generic;
using UnityModManagerNet;

namespace SolastaOathOfRetribution
{
  public class Main
  {
    public static Guid ModGuidNamespace = new Guid("1e13fc7e-08ab-42d1-ba98-f7854b3f58ea");
    private static UnityModManager.ModEntry ModEntry;
    private static bool initialized = false;
    private static int lastcount = 0;

    public static void Log(string msg) => Main.ModEntry?.Logger.Log(msg);

    public static void Error(Exception ex) => Main.ModEntry?.Logger.Error(ex.ToString());

    public static void Error(string msg) => Main.ModEntry?.Logger.Error(msg);

    private static void Load(UnityModManager.ModEntry modEntry)
    {
      Main.ModEntry = modEntry;
      modEntry.OnUpdate = new System.Action<UnityModManager.ModEntry, float>(Main.OnUpdate);
    }

    private static void OnUpdate(UnityModManager.ModEntry modEntry, float time)
    {
      if (Main.initialized)
        return;
      if (DatabaseRepository.DatabasesCount() >= 20 && DatabaseRepository.DatabasesCount() == Main.lastcount)
      {
        Main.initialized = true;
        Main.ModifyDatabase();
      }
      if (DatabaseRepository.DatabasesCount() > Main.lastcount)
        Main.lastcount = DatabaseRepository.DatabasesCount();
    }

    private static void ModifyDatabase()
    {
      SubClassBuilder subClassBuilder = new SubClassBuilder();
      subClassBuilder.SetName("OathOfRetribution");
      GuiPresentationBuilder presentationBuilder1 = new GuiPresentationBuilder(LocalizationHelper.AddString("Subclass/&OathOfRetributionDescription", "Paladins who swear the Oath of Retribution fully commit themselves to terminate the unholiness and injustice with extreme prejudice. For these paladins, the duty of avenging sins and corruptions weighs greater than their own values. "), LocalizationHelper.AddString("Subclass/&OathOfRetributionTitle", "Oath of Retribution"));
      presentationBuilder1.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference);
      subClassBuilder.SetGuiPresentation(presentationBuilder1.Build());
      FeatureDefinitionAutoPreparedSpells feature1 = FeatureBuilder.BuildPaladinAutoPreparedSpellGroup(new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>()
      {
        FeatureBuilder.BuildAutoPreparedSpellGroup(3, new List<SpellDefinition>()
        {
          DatabaseHelper.SpellDefinitions.Bane,
          DatabaseHelper.SpellDefinitions.HuntersMark
        }),
        FeatureBuilder.BuildAutoPreparedSpellGroup(5, new List<SpellDefinition>()
        {
          DatabaseHelper.SpellDefinitions.HoldPerson,
          DatabaseHelper.SpellDefinitions.MistyStep
        }),
        FeatureBuilder.BuildAutoPreparedSpellGroup(6, new List<SpellDefinition>()
        {
          DatabaseHelper.SpellDefinitions.Haste,
          DatabaseHelper.SpellDefinitions.ProtectionFromEnergy
        })
      }, DatabaseHelper.CharacterClassDefinitions.Paladin, "AutoPreparedSpellsOathOfRetribution", new GuiPresentationBuilder(LocalizationHelper.AddString("Feature/&DomainSpellsDescription", "In your list and always prepared: \nLevel 3: Bane, Hunter's Mark \nLevel 5: Hold Person, Misty Step \nLevel 6: Haste, Protection from Energy"), LocalizationHelper.AddString("Feature/&AutoPreparedSpellsOathOfRetributionTitle", "Oath of Retribution Spells")).Build());
      subClassBuilder.AddFeatureAtLevel((FeatureDefinition) feature1, 3);
      GuiPresentationBuilder presentationBuilder2 = new GuiPresentationBuilder(LocalizationHelper.AddString("Rules/&ConditionFrightenedZealousAccusationDescription", "Absolutely overwhelmed by the power of holiness. \nCannot move and frighted for 1 minute or until take any damage. "), LocalizationHelper.AddString("Rules/&ConditionFrightenedZealousAccusationTitle", "Terrified"));
      presentationBuilder2.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionFrightened.GuiPresentation.SpriteReference);
      ConditionDefinition condition1_1 = FeatureBuilder.BuildCondition(new List<FeatureDefinition>()
      {
        (FeatureDefinition) DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
        (FeatureDefinition) DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained
      }, RuleDefinitions.DurationType.Minute, 1, new List<RuleDefinitions.ConditionInterruption>()
      {
        RuleDefinitions.ConditionInterruption.Damaged,
        RuleDefinitions.ConditionInterruption.DamagedByFriendly
      }, "ConditionFrightenedZealousAccusation", presentationBuilder2.Build());
      GuiPresentationBuilder presentationBuilder3 = new GuiPresentationBuilder(LocalizationHelper.AddString("Feature/&PowerOathOfRetributionZealousAccusationDescription", "Channel Divinity to designate a creature within 12 tiles and force the target a Wisdom saving throw. Fiends and undead have a disadvantage on this saving throw. \nOn a failed save, the target cannot move and being frightened for one minute or until it takes any damage. "), LocalizationHelper.AddString("Feature/&PowerOathOfRetributionZealousAccusationTitle", "Channel Divinity: Zealous Accusation"));
      presentationBuilder3.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation.SpriteReference);
      FeatureDefinitionPower feature2 = FeatureBuilder.BuildActionConditionPowerPaladinCD1(1, RuleDefinitions.UsesDetermination.Fixed, "Wisdom", RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ChannelDivinity, RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Individuals, global::ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn, "Wisdom", "Wisdom", 15, new List<SaveAffinityBySenseDescription>()
      {
        FeatureBuilder.BuildSaveAffinityBySense(SenseMode.Type.Darkvision, RuleDefinitions.AdvantageType.Disadvantage),
        FeatureBuilder.BuildSaveAffinityBySense(SenseMode.Type.SuperiorDarkvision, RuleDefinitions.AdvantageType.Disadvantage)
      }, condition1_1, "PowerOathOfRetributionZealousAccusation", presentationBuilder3.Build());
      subClassBuilder.AddFeatureAtLevel((FeatureDefinition) feature2, 3);
      GuiPresentationBuilder presentationBuilder4 = new GuiPresentationBuilder(LocalizationHelper.AddString("Rules/&ConditionTSZealousCondemnationDescription", "Marked by glorious determination. \nAttacks to this target will receive an advantage for 1 minute or until the target is dead or unconscious. "), LocalizationHelper.AddString("Rules/&ConditionTSZealousCondemnationTitle", "Stigmatized"));
      presentationBuilder4.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionGuided.GuiPresentation.SpriteReference);
      ConditionDefinition condition1_2 = FeatureBuilder.BuildCondition(new List<FeatureDefinition>()
      {
        (FeatureDefinition) DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityTrueStrike
      }, RuleDefinitions.DurationType.Minute, 1, new List<RuleDefinitions.ConditionInterruption>()
      {
        RuleDefinitions.ConditionInterruption.None
      }, "ConditionTrueStrikeZealousCondemnation", presentationBuilder4.Build());
      GuiPresentationBuilder presentationBuilder5 = new GuiPresentationBuilder(LocalizationHelper.AddString("Feature/&PowerOathOfRetributionZealousCondemnationDescription", "Channel Divinity to gain an advantage on your attack against the designated target within 2 tiles as a bonus action. The power lasts 1 minute or until the target is dead or unconscious. "), LocalizationHelper.AddString("Feature/&PowerOathOfRetributionZealousCondemnationTitle", "Channel Divinity: Zealous Condemnation"));
      presentationBuilder5.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarSmiteTheHidden.GuiPresentation.SpriteReference);
      FeatureDefinitionPower feature3 = FeatureBuilder.BuildActionConditionPowerPaladinCD2(1, RuleDefinitions.UsesDetermination.Fixed, "Wisdom", RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.ChannelDivinity, RuleDefinitions.RangeType.Distance, 2, RuleDefinitions.TargetType.Individuals, global::ActionDefinitions.ItemSelectionType.None, RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn, condition1_2, "PowerOathOfRetributionZealousCondemnation", presentationBuilder5.Build());
      subClassBuilder.AddFeatureAtLevel((FeatureDefinition) feature3, 3);
      GuiPresentationBuilder presentationBuilder6 = new GuiPresentationBuilder(LocalizationHelper.AddString("Rules/&ConditionBonusRushTenaciousPursuitDescription", "The blessing grants you a power to chase and avenge the enemy. \nBonus dash is enabled for a single turn. This movement doesn't provoke opportunity attacks. "), LocalizationHelper.AddString("Rules/&ConditionBonusRushTenaciousPursuitTitle", "Chasing"));
      presentationBuilder6.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionHasted.GuiPresentation.SpriteReference);
      ConditionDefinition condition = FeatureBuilder.BuildBuffCondition(new List<FeatureDefinition>()
      {
        (FeatureDefinition) DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging,
        (FeatureDefinition) DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityExpeditiousRetreat,
        (FeatureDefinition) DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionExpeditiousRetreat
      }, RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn, new List<RuleDefinitions.ConditionInterruption>()
      {
        RuleDefinitions.ConditionInterruption.None
      }, "ConditionBonusRushTenaciousPursuitGui", presentationBuilder6.Build());
      GuiPresentationBuilder presentationBuilder7 = new GuiPresentationBuilder(LocalizationHelper.AddString("Feature/&PowerOathOfRetributionTenaciousPursuitDescription", "When you successfully hit the melee attack, this power will grants you a dash move as a bonus action for a single turn, 5 times per long rest. This movement doesn't provoke opportunity attacks. "), LocalizationHelper.AddString("Feature/&PowerOathOfRetributionTenaciousPursuitTitle", "Tenacious Pursuit"));
      presentationBuilder7.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.GuiPresentation.SpriteReference);
      LocalizationHelper.AddString("Reaction/&SpendPowerPowerOathOfRetributionTenaciousPursuitDescription", "{0} can earn bonus action dash move by striking this enemy using this power. ");
      LocalizationHelper.AddString("Reaction/&SpendPowerPowerOathOfRetributionTenaciousPursuitReactDescription", "Click to activate bonus action dash. ");
      LocalizationHelper.AddString("Reaction/&SpendPowerPowerOathOfRetributionTenaciousPursuitReactTitle", "Pursue");
      LocalizationHelper.AddString("Reaction/&SpendPowerPowerOathOfRetributionTenaciousPursuitTitle", "Tenacious Pursuit");
      LocalizationHelper.AddString("Feature/&PowerOathOfRetributionTenaciousPursuitShortTitle", "Tenacious Pursuit");
      FeatureDefinitionPower feature4 = FeatureBuilder.BuildBonusMoveAfterHitPower(RuleDefinitions.ActivationTime.OnAttackHit, RuleDefinitions.RechargeRate.LongRest, 1, "Wisdom", 5, "Charisma", "Feature/&PowerOathOfRetributionTenaciousPursuitShortTitle", RuleDefinitions.TargetType.Self, global::ActionDefinitions.ItemSelectionType.Equiped, RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn, condition, "PowerOathOfRetributionTenaciousPursuit", presentationBuilder7.Build());
      subClassBuilder.AddFeatureAtLevel((FeatureDefinition) feature4, 7);
      DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoicePaladinSacredOaths.Subclasses.Add(subClassBuilder.AddToDB().Name);
    }
  }
}
