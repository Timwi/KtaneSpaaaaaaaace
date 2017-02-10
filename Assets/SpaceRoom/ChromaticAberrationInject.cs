using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ChromaticAberrationInject : MonoBehaviour
{
    public Material chromAberrationMaterial = null;
    public Material separableBlurMaterial = null;
    public Material vignetteMaterial = null;

    [HideInInspector]
    public VignetteAndChromaticAberration customAberration = null;

    private void Start()
    {
        customAberration = Camera.main.gameObject.AddComponent<VignetteAndChromaticAberration>();
        customAberration.chromAberrationShader = chromAberrationMaterial.shader;
        customAberration.separableBlurShader = separableBlurMaterial.shader;
        customAberration.vignetteShader = vignetteMaterial.shader;
    }

    private void OnEnable()
    {
        if (customAberration != null)
        {
            customAberration.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (customAberration != null)
        {
            customAberration.enabled = false;
        }
    }
}
