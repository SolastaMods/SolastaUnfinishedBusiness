using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Models
{
    internal static class AdventureLogContext
    {
        private static readonly List<int> captionHashes = new List<int>();

        internal static void LogEntry(ItemDefinition itemDefinition, AssetReferenceSprite assetReferenceSprite)
        {
            var isUserText = itemDefinition.Name.StartsWith("Custom") && itemDefinition.IsDocument;

            if (isUserText)
            {
                var fragments = itemDefinition.DocumentDescription.ContentFragments.Select(x => x.Text).ToList();

                LogEntry(itemDefinition.FormatTitle(), fragments, string.Empty, assetReferenceSprite);
            }
        }

        internal static void LogEntry(string title, List<string> captions, string speakerName = "", AssetReferenceSprite assetReferenceSprite = null)
        {
            var gameCampaign = Gui.GameCampaign;

            if (gameCampaign != null && gameCampaign.CampaignDefinitionName == "UserCampaign")
            {
                var adventureLog = gameCampaign.AdventureLog;
                var gameDailyLog = adventureLog.DailyLog as GameDailyLog;
                var adventureLogDefinition = AccessTools.Field(adventureLog.GetType(), "adventureLogDefinition").GetValue(adventureLog) as AdventureLogDefinition;
                var loreEntry = new GameAdventureEntryDungeonMaker(adventureLogDefinition, title, captions, speakerName, assetReferenceSprite);
                var found = false;

                //
                // TODO: still debugging this
                //
                foreach (GameDailyIndex gameDailyIndex in gameDailyLog.Entries)
                {
                    foreach (var gameAdventureEntry in gameDailyIndex.DailyEntries)
                    {
                        if (gameAdventureEntry is GameAdventureEntryDungeonMaker gameAdventureEntryDungeonMaker && gameAdventureEntryDungeonMaker.CaptionsHash == loreEntry.CaptionsHash)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found && !captionHashes.Contains(loreEntry.CaptionsHash))
                {
                    captionHashes.Add(loreEntry.CaptionsHash);
                    adventureLog.AddAdventureEntry(loreEntry);
                }
            }
        }

        internal class GameAdventureEntryDungeonMaker : GameAdventureEntry
        {
            private int captionsHash;
            private string assetGuid;
            private AssetReferenceSprite assetReferenceSprite;
            private List<GameAdventureConversationInfo> conversationInfos = new List<GameAdventureConversationInfo>();
            private readonly List<TextBreaker> textBreakers = new List<TextBreaker>();
            private string title;

            public GameAdventureEntryDungeonMaker()
            {

            }

            public GameAdventureEntryDungeonMaker(AdventureLogDefinition adventureLogDefinition, string title, List<string> captions, string actorName, AssetReferenceSprite assetReferenceSprite) : base(adventureLogDefinition)
            {
                var builder = new StringBuilder();

                foreach (var caption in captions)
                {
                    builder.Append(caption);
                    this.conversationInfos.Add(new GameAdventureConversationInfo(actorName, caption, actorName != ""));
                    this.textBreakers.Add(new TextBreaker());
                }

                this.assetGuid = assetReferenceSprite == null ? string.Empty : assetReferenceSprite.AssetGUID;
                this.assetReferenceSprite = assetReferenceSprite;
                this.captionsHash = builder.ToString().GetHashCode();
                this.title = title;
            }

            public int CaptionsHash => this.captionsHash;

            public override bool HasIllustration
            {
                get
                {
                    //
                    // TODO: revisit addressables
                    //
                    return false;

                    var illustrationReference = this.IllustrationReference;

                    return illustrationReference != null && illustrationReference.RuntimeKeyIsValid();
                }
            }

            public override AssetReference IllustrationReference => assetReferenceSprite;

            public List<TextBreaker> TextBreakers => this.textBreakers;

            public string Title => this.title;

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

            public override void SerializeAttributes(IAttributesSerializer serializer, IVersionProvider versionProvider)
            {
                base.SerializeAttributes(serializer, versionProvider);
                this.assetGuid = serializer.SerializeAttribute<string>("AssetGuid", this.assetGuid);
                this.captionsHash = serializer.SerializeAttribute<int>("CaptionsHash", this.captionsHash);
                this.title = serializer.SerializeAttribute<string>("SectionTitle", this.title);

                if (this.assetGuid != string.Empty)
                {
                    this.assetReferenceSprite = new AssetReferenceSprite(this.assetGuid);
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
