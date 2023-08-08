using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Builders;

internal class MonsterPresentationBuilder
{
    private readonly MonsterPresentation _presentation;

    private MonsterPresentationBuilder()
    {
        _presentation = new MonsterPresentation();
        InitDefaults();
    }

    private void InitDefaults()
    {
        _presentation.customShaderReference = new AssetReference();
        _presentation.attachedParticlesReference = new AssetReference();
        _presentation.bestiaryAttachedParticlesReference = new AssetReference();
        _presentation.hasPhantomDistortion = false;
        _presentation.hasPhantomFadingFeet = false;
    }

    internal static MonsterPresentationBuilder Create()
    {
        return new MonsterPresentationBuilder();
    }

    internal MonsterPresentation Build()
    {
        return _presentation;
    }

    internal MonsterPresentationBuilder SetHasMonsterPortraitBackground(bool value)
    {
        _presentation.hasMonsterPortraitBackground = value;
        return this;
    }

    internal MonsterPresentationBuilder SetCanGeneratePortrait(bool value)
    {
        _presentation.canGeneratePortrait = value;
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
        _presentation.femaleModelScale = scale;
        _presentation.maleModelScale = scale;
        return this;
    }

    internal MonsterPresentationBuilder SetAllPrefab(MonsterPresentation monsterPresentation)
    {
        _presentation.attachedParticlesReference = monsterPresentation.attachedParticlesReference;
        _presentation.customShaderReference = monsterPresentation.customShaderReference;
        _presentation.bestiaryAttachedParticlesReference = monsterPresentation.bestiaryAttachedParticlesReference;
        _presentation.femalePrefabReference = monsterPresentation.femalePrefabReference;
        _presentation.malePrefabReference = monsterPresentation.malePrefabReference;
        return this;
    }

    internal MonsterPresentationBuilder SetPhantom(bool distortion = true, bool fadingFeet = true)
    {
        _presentation.hasPhantomDistortion = distortion;
        _presentation.hasPhantomFadingFeet = fadingFeet;
        return this;
    }

    internal MonsterPresentationBuilder SetPrefab(AssetReference prefab)
    {
        _presentation.femalePrefabReference = prefab;
        _presentation.malePrefabReference = prefab;
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
