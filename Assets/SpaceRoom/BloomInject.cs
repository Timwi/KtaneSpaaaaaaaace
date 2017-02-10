using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class BloomInject : MonoBehaviour
{
    public Material blurAndFlaresMaterial = null;
    public Material brightPassFilterMaterial = null;
    public Material lensFlareMaterial = null;
    public Material screenBlendMaterial = null;

    [HideInInspector]
    public Bloom customBloom = null;

    private void Start()
    {
        customBloom = Camera.main.gameObject.AddComponent<Bloom>();

        customBloom.blurAndFlaresShader = blurAndFlaresMaterial.shader;
        customBloom.brightPassFilterShader = brightPassFilterMaterial.shader;
        customBloom.lensFlareShader = lensFlareMaterial.shader;
        customBloom.screenBlendShader = screenBlendMaterial.shader;
    }

    private void OnEnable()
    {
        if (customBloom != null)
        {
            customBloom.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (customBloom != null)
        {
            customBloom.enabled = false;
        }
    }
}
