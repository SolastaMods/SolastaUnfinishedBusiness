using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class AdventureLogContext
    {
        internal static void LogEntry(ItemDefinition itemDefinition)
        {
            var isUserText = itemDefinition.Name.StartsWith("Custom") && itemDefinition.IsDocument;

            if (isUserText)
            {
                var fragments = itemDefinition.DocumentDescription.ContentFragments.Select(x => x.Text).ToList();

                LogEntry(fragments);
            }
        }

        internal static void LogEntry(List<string> captions, GameLocationCharacter speaker = null)
        {
            var gameCampaign = Gui.GameCampaign;

            if (gameCampaign != null && gameCampaign.CampaignDefinitionName == "UserCampaign")
            {
                var speakerName = speaker != null ? speaker.Name : string.Empty;
                var adventureLog = gameCampaign.AdventureLog;
                var adventureLogDefinition = AccessTools.Field(adventureLog.GetType(), "adventureLogDefinition").GetValue(adventureLog) as AdventureLogDefinition;
                var loreEntry = new GameAdventureEntryDungeonMaker(adventureLogDefinition, captions, speakerName);

                adventureLog.AddEntry(loreEntry);
            }
        }

        internal class GameAdventureEntryDungeonMaker : GameAdventureEntry
        {
            private List<GameAdventureConversationInfo> conversationInfos = new List<GameAdventureConversationInfo>();
            private readonly List<TextBreaker> textBreakers = new List<TextBreaker>();

            public GameAdventureEntryDungeonMaker(AdventureLogDefinition adventureLogDefinition, List<string> captions, string actorName = "", string itemDefinitionGuid = "") : base(adventureLogDefinition)
            {
                foreach (var caption in captions)
                {
                    this.conversationInfos.Add(new GameAdventureConversationInfo(actorName, caption, actorName != ""));
                    this.textBreakers.Add(new TextBreaker());
                }
            }

            public List<TextBreaker> TextBreakers => this.textBreakers;

            public override void ComputeHeight(float areaWidth, ITextComputer textCompute)
            {
                base.ComputeHeight(areaWidth, textCompute);
                this.Height = this.AdventureLogDefinition.ConversationHeaderHeight;

                for (var i = 0; i < textBreakers.Count; ++i)
                {
                    var textBreaker = textBreakers[i];

                    if (this.conversationInfos[i].ActorName != "")
                    {
                        this.Parameters.Clear();
                        this.AddParameter(AdventureStyleDuplet.ParameterType.NpcName, this.conversationInfos[i].ActorName + ":");
                        this.BreakdownFragments(textBreaker, "{0}" + Gui.Localize(this.conversationInfos[i].ActorLine), this.AdventureLogDefinition.BaseStyle);
                    }
                    else
                    {
                        this.BreakdownFragments(textBreaker, Gui.Localize(this.conversationInfos[i].ActorLine), this.AdventureLogDefinition.BaseStyle);
                    }

                    textBreaker.ComputeFragmentExtents(textCompute, this.AdventureLogDefinition.ConversationLineHeight);
                    this.Height += textBreaker.DispatchFragments(areaWidth, this.AdventureLogDefinition.ConversationIndentWidth, this.AdventureLogDefinition.ConversationLineHeight, this.AdventureLogDefinition.ConversationLineHeight, this.AdventureLogDefinition.ConversationWordSpacing, true, this.Height - this.AdventureLogDefinition.ConversationHeaderHeight);
                    this.Height += this.AdventureLogDefinition.ConversationParagraphSpacing;
                    this.Height += this.AdventureLogDefinition.ConversationTrailingHeight;
                }
            }

            public override void SerializeElements(IElementsSerializer serializer, IVersionProvider versionProvider)
            {
                base.SerializeElements(serializer, versionProvider);
                this.conversationInfos = serializer.SerializeElement<GameAdventureConversationInfo>("ConversationInfos", this.conversationInfos);

                for (int i = 0; i < this.conversationInfos.Count; ++i)
                {
                    this.textBreakers.Add(new TextBreaker());
                }
            }
        }
    }
}
