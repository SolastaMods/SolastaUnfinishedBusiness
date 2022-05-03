using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.CustomUI
{
    public class CustomTooltipProvider : GuiBaseDefinitionWrapper, ISubTitleProvider, IPrerequisitesProvider, IImageProvider
    {
        private readonly GuiPresentation _guiPresentation;
        private string _prerequisites;
        private string _subtitle;

        public string Subtitle => _subtitle;

        public CustomTooltipProvider(BaseDefinition baseDefinition, GuiPresentation guiPresentation) : base(baseDefinition)
        {
            _guiPresentation = guiPresentation;
            _subtitle = GetDefaultSubtitle();
        }

        private string GetDefaultSubtitle()
        {
            return BaseDefinition switch
            {
                FeatureDefinitionPower => "UI/&CustomFeatureSelectionTooltipTypePower",
                FeatureDefinitionBonusCantrips => "UI/&CustomFeatureSelectionTooltipTypeCantrip",
                FeatureDefinitionProficiency => "UI/&CustomFeatureSelectionTooltipTypeProficiency",
                _ => "UI/&CustomFeatureSelectionTooltipTypeFeature",
            };
        }

        public override void SetupSprite(Image image, object context = null)
        {
            if (image.sprite != null)
            {
                ReleaseSprite(image);
                image.sprite = null;
            }
            if (_guiPresentation != null && _guiPresentation.SpriteReference != null && _guiPresentation.SpriteReference.RuntimeKeyIsValid())
            {
                image.gameObject.SetActive(true);
                image.sprite = Gui.LoadAssetSync<Sprite>(_guiPresentation.SpriteReference);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }

        public void SetPrerequisites(params string[] missingRequirements)
        {
            SetPrerequisites(missingRequirements.ToList());
        }

        public CustomTooltipProvider SetPrerequisites(List<string> missingRequirements)
        {
            _prerequisites = missingRequirements == null || missingRequirements.Empty() 
                ? null 
                : Gui.Colorize(String.Join("\n", missingRequirements.Select(e => Gui.Localize(e))), Gui.ColorNegative);

            return this;
        }

        public CustomTooltipProvider SetSubtitle(string subtitle)
        {
            _subtitle = string.IsNullOrEmpty(subtitle)
                ? GetDefaultSubtitle()
                : subtitle;

            return this;
        }

        public string EnumeratePrerequisites(RulesetCharacterHero hero)
        {
            return _prerequisites;
        }
    }
}
