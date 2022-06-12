using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Api.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(EffectParticleParameters))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class EffectParticleParametersExtensions
{
    public static EffectParticleParameters Copy(this EffectParticleParameters entity)
    {
        var copy = new EffectParticleParameters();
        copy.Copy(entity);
        return copy;
    }
}
