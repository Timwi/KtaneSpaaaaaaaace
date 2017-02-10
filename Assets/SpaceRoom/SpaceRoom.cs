using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

public class SpaceRoom : MonoBehaviour
{
    public Light[] lights = null;

    public Material skyboxOverride = null;

    private KMBombInfo _bombInfo = null;
    private BloomInject _bloomInject = null;
    private ChromaticAberrationInject _aberrationInject = null;

    private Material _oldSkybox = null;
    private bool _lightsOn = true;
    private Color _oldAmbientColor = Color.black;
    private AmbientMode _oldAmbientMode = AmbientMode.Skybox;

    private void Start()
    {
        _bloomInject = GetComponent<BloomInject>();
        _aberrationInject = GetComponent<ChromaticAberrationInject>();

        KMGameplayRoom gameplayRoom = GetComponent<KMGameplayRoom>();
        gameplayRoom.OnLightChange += OnLightChange;

        _bombInfo = GetComponent<KMBombInfo>();

        if (skyboxOverride != null)
        {
            _oldSkybox = RenderSettings.skybox;
            RenderSettings.skybox = skyboxOverride;
        }

        _oldAmbientColor = RenderSettings.ambientLight;
        _oldAmbientMode = RenderSettings.ambientMode;

        OnLightChange(true);
    }

    private void Update()
    {
        UpdateIntensity();
    }

    private void OnDestroy()
    {
        if (_oldSkybox != null)
        {
            RenderSettings.skybox = _oldSkybox;
        }

        OnLightChange(true);
    }

    private void OnLightChange(bool on)
    {
        foreach (Light light in lights)
        {
            light.enabled = on;
        }

        _lightsOn = on;

        UpdateIntensity();
    }

    private void UpdateIntensity()
    {
        float intensity = 0.0f;
        if (_bombInfo.IsBombPresent())
        {
            float remainingTime = _bombInfo.GetTime();
            intensity = Mathf.Clamp01((60.0f - remainingTime) / 60.0f);
        }

        foreach (Light light in lights)
        {
            light.enabled = _lightsOn;
            if (_lightsOn)
            {
                light.intensity = Mathf.Lerp(0.5f, 1.3f, intensity);
            }
        }

        RenderSettings.ambientMode = _lightsOn ? _oldAmbientMode : AmbientMode.Flat;
        RenderSettings.ambientLight = _lightsOn ? _oldAmbientColor : new Color(0.05f, 0.05f, 0.05f);

        Bloom bloom = _bloomInject.customBloom;
        if (bloom != null)
        {
            bloom.bloomBlurIterations = 3;
            bloom.bloomIntensity = Mathf.Lerp(0.0f, 0.9f, intensity);
            bloom.bloomThreshold = Mathf.Lerp(0.5f, 0.2f, intensity);
            bloom.sepBlurSpread = Mathf.Lerp(0.0f, 1.5f, intensity);
        }

        VignetteAndChromaticAberration aberration = _aberrationInject.customAberration;
        if (aberration != null)
        {
            aberration.intensity = Mathf.Lerp(0.0f, 0.35f, intensity);
            aberration.blur = Mathf.Lerp(0.0f, 0.75f, intensity);
            aberration.blurSpread = Mathf.Lerp(0.0f, 0.7f, intensity);
            aberration.chromaticAberration = Mathf.Lerp(0.0f, 10.0f, intensity);
        }

        if (_lightsOn)
        {
            skyboxOverride.SetFloat("_Saturation", Mathf.Lerp(86.9f, 0.0f, intensity));
            skyboxOverride.SetFloat("_Brightness", Mathf.Lerp(1.5f, 2.7f, intensity));
            skyboxOverride.SetFloat("_Distfading", Mathf.Lerp(50.0f, 80.0f, intensity));
        }
        else
        {
            skyboxOverride.SetFloat("_Saturation", 86.9f);
            skyboxOverride.SetFloat("_Brightness", 1.5f);
            skyboxOverride.SetFloat("_Distfading", 20.0f);
        }
    }
}
