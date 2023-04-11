using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

internal class MonsterPresentationBuilder
{
    private readonly MonsterPresentation presentation;

    private MonsterPresentationBuilder()
    {
        presentation = new MonsterPresentation();
        InitDefaults();
    }

    private void InitDefaults()
    {
        presentation.customShaderReference = new AssetReference();
        presentation.attachedParticlesReference = new AssetReference();
        presentation.bestiaryAttachedParticlesReference = new AssetReference();
        presentation.hasPhantomDistortion = false;
        presentation.hasPhantomFadingFeet = false;
    }

    internal static MonsterPresentationBuilder Create()
    {
        return new MonsterPresentationBuilder();
    }

    internal MonsterPresentation Build()
    {
        return presentation;
    }

    internal MonsterPresentationBuilder SetHasMonsterPortraitBackground(bool value)
    {
        presentation.hasMonsterPortraitBackground = value;
        return this;
    }

    internal MonsterPresentationBuilder SetCanGeneratePortrait(bool value)
    {
        presentation.canGeneratePortrait = value;
        return this;
    }

#if false
    internal MonsterPresentationBuilder SetCustomShaderReference(AssetReference shaderReference)
    {
        presentation.customShaderReference = shaderReference;
        return this;
    }
#endif

    internal MonsterPresentationBuilder SetModelScale(float scale)
    {
        presentation.femaleModelScale = scale;
        presentation.maleModelScale = scale;
        return this;
    }

    internal MonsterPresentationBuilder SetAllPrefab(MonsterPresentation monsterPresentation)
    {
        presentation.attachedParticlesReference = monsterPresentation.attachedParticlesReference;
        presentation.customShaderReference = monsterPresentation.customShaderReference;
        presentation.bestiaryAttachedParticlesReference = monsterPresentation.bestiaryAttachedParticlesReference;
        presentation.femalePrefabReference = monsterPresentation.femalePrefabReference;
        presentation.malePrefabReference = monsterPresentation.malePrefabReference;
        return this;
    }

    internal MonsterPresentationBuilder SetPhantom(bool distortion = true, bool fadingFeet = true)
    {
        presentation.hasPhantomDistortion = distortion;
        presentation.hasPhantomFadingFeet = fadingFeet;
        return this;
    }

    internal MonsterPresentationBuilder SetPrefab(AssetReference prefab)
    {
        presentation.femalePrefabReference = prefab;
        presentation.malePrefabReference = prefab;
        return this;
    }

#if false
    internal MonsterPresentationBuilder SetPortraitFOV(float fov)
    {
        presentation.portraitCameraFOV = fov;
        return this;
    }

    internal MonsterPresentationBuilder SetPortraitCameraFollowOffset(float x = 0f, float y = -0.5f, float z = -5f)
    {
        presentation.portraitCameraFollowOffset = new Vector3(x, y, z);
        return this;
    }
#endif
}
