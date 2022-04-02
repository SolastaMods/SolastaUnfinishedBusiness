using System.Collections.Generic;
using System.Text;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Models
{
    internal static class AdventureLogContext
    {
        private static readonly List<int> captionHashes = new();

        internal static void LogEntry(ItemDefinition itemDefinition, AssetReferenceSprite assetReferenceSprite)
        {
            var isUserText = itemDefinition.Name.StartsWith("Custom") && itemDefinition.IsDocument;

            if (isUserText)
            {
                var builder = new StringBuilder();
                var fragments = itemDefinition.DocumentDescription.ContentFragments.ConvertAll(x => x.Text);

                fragments.ForEach(x => builder.Append(x));
                LogEntry(itemDefinition.FormatTitle(), builder.ToString(), string.Empty, assetReferenceSprite);
            }
        }

        internal static void LogEntry(string title, string text, string speakerName = "", AssetReferenceSprite assetReferenceSprite = null)
        {
            var gameCampaign = Gui.GameCampaign;

            if (gameCampaign != null && Gui.GameLocation.UserLocation != null)
            {
                var adventureLog = gameCampaign.AdventureLog;
                var hashCode = text.GetHashCode();

                if (adventureLog != null && !captionHashes.Contains(hashCode))
                {
                    var adventureLogDefinition = adventureLog.GetField<GameAdventureLog, AdventureLogDefinition>("adventureLogDefinition");
                    var loreEntry = new GameAdventureEntryDungeonMaker(adventureLogDefinition, title, text, speakerName, assetReferenceSprite);

                    captionHashes.Add(hashCode);
                    adventureLog.AddAdventureEntry(loreEntry);
                }
            }
        }

        internal class GameAdventureEntryDungeonMaker : GameAdventureEntry
        {
            private string assetGuid;
            private AssetReferenceSprite assetReferenceSprite;
            private List<GameAdventureConversationInfo> conversationInfos = new();
            private string title;

            public GameAdventureEntryDungeonMaker()
            {
            }

            public GameAdventureEntryDungeonMaker(AdventureLogDefinition adventureLogDefinition, string header, string text, string actorName, AssetReferenceSprite sprite) : base(adventureLogDefinition)
            {
                assetGuid = assetReferenceSprite == null ? string.Empty : assetReferenceSprite.AssetGUID;
                assetReferenceSprite = sprite;
                conversationInfos.Add(new GameAdventureConversationInfo(actorName, text, actorName != ""));
                TextBreakers.Add(new TextBreaker());
                title = header;
            }

            public override bool HasIllustration
            {
                get
                {
                    var illustrationReference = IllustrationReference;

                    return illustrationReference != null && illustrationReference.RuntimeKeyIsValid();
                }
            }

            public override AssetReference IllustrationReference => assetReferenceSprite;

            public List<TextBreaker> TextBreakers { get; } = new List<TextBreaker>();

            public string Title => title;

            public override void ComputeHeight(float areaWidth, ITextComputer textCompute)
            {
                base.ComputeHeight(areaWidth, textCompute);
                Height = AdventureLogDefinition.ConversationHeaderHeight;

                for (var i = 0; i < TextBreakers.Count; ++i)
                {
                    var textBreaker = TextBreakers[i];

                    if (conversationInfos[i].ActorName != "")
                    {
                        Parameters.Clear();
                        AddParameter(AdventureStyleDuplet.ParameterType.NpcName, conversationInfos[i].ActorName + ":");
                        BreakdownFragments(textBreaker, "{0}" + Gui.Localize(conversationInfos[i].ActorLine), AdventureLogDefinition.BaseStyle);
                    }
                    else
                    {
                        BreakdownFragments(textBreaker, Gui.Localize(conversationInfos[i].ActorLine), AdventureLogDefinition.BaseStyle);
                    }

                    textBreaker.ComputeFragmentExtents(textCompute, AdventureLogDefinition.ConversationLineHeight);
                    Height += textBreaker.DispatchFragments(areaWidth, AdventureLogDefinition.ConversationIndentWidth, AdventureLogDefinition.ConversationLineHeight, AdventureLogDefinition.ConversationLineHeight, AdventureLogDefinition.ConversationWordSpacing, true, Height - AdventureLogDefinition.ConversationHeaderHeight);
                    Height += AdventureLogDefinition.ConversationParagraphSpacing;
                    Height += AdventureLogDefinition.ConversationTrailingHeight;
                }
            }

            public override void SerializeAttributes(IAttributesSerializer serializer, IVersionProvider versionProvider)
            {
                base.SerializeAttributes(serializer, versionProvider);
                assetGuid = serializer.SerializeAttribute("AssetGuid", assetGuid);
                title = serializer.SerializeAttribute("SectionTitle", title);

                if (assetGuid != string.Empty)
                {
                    assetReferenceSprite = new AssetReferenceSprite(assetGuid);
                }
            }

            public override void SerializeElements(IElementsSerializer serializer, IVersionProvider versionProvider)
            {
                base.SerializeElements(serializer, versionProvider);
                conversationInfos = serializer.SerializeElement("ConversationInfos", conversationInfos);

                for (int i = 0; i < conversationInfos.Count; ++i)
                {
                    var conversationInfo = conversationInfos[i];

                    if (conversationInfo.LineType == GameAdventureConversationInfo.Type.SpeechLine)
                    {
                        var hashCode = conversationInfo.ActorLine.GetHashCode();

                        captionHashes.Add(hashCode);
                    }

                    TextBreakers.Add(new TextBreaker());
                }
            }
        }
    }
}
