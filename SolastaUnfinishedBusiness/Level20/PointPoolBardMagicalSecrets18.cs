using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class PointPoolBardMagicalSecrets18Builder : FeatureDefinitionPointPoolBuilder
{
    private const string PointPoolBardMagicalSecrets18Name = "PointPoolBardMagicalSecrets18";
    private const string PointPoolBardMagicalSecrets18Guid = "8a2c90a9-a87a-4ad9-98d6-229668f23909";
    private static FeatureDefinitionPointPool _instance;

    private PointPoolBardMagicalSecrets18Builder() : base(PointPoolBardMagicalSecrets14,
        PointPoolBardMagicalSecrets18Name, PointPoolBardMagicalSecrets18Guid)
    {
        // Empty
    }

    internal static FeatureDefinitionPointPool PointPoolBardMagicalSecrets18 =>
        _instance ??= new PointPoolBardMagicalSecrets18Builder().AddToDB();
}
