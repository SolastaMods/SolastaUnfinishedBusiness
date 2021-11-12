using System.Collections.Generic;

namespace SolastaCommunityExpansion.Feats
{
    class ElAntoniousFeats
    {
        public static void CreateFeats(List<FeatDefinition> feats)
        {
            feats.Add(DualFlurryFeatBuilder.DualFlurryFeat);
            //feats.Add(TorchBearerFeatBuilder.TorchbearerFeat));
        }
    }
}
