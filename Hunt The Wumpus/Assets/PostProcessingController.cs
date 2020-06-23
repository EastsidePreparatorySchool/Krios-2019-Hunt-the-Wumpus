using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using CommandView;

public class PostProcessingController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;

    public Planet planet;

    Bloom bloomLayer = null;
    AmbientOcclusion ambientOcclusionLayer = null;

    private bool bloomWas;
    private bool aowas;

    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out bloomLayer);
        postProcessVolume.profile.TryGetSettings(out ambientOcclusionLayer);
        
        bloomWas = planet.bloom;
        aowas = planet.ambientOcclusion;
    }

    void Update()
    {
        if (planet.bloom != bloomWas)
        {
            bloomLayer.enabled.value = planet.bloom;
            bloomWas = planet.bloom;
        }
        if (planet.ambientOcclusion != aowas)
        {
            ambientOcclusionLayer.enabled.value = planet.ambientOcclusion;
            aowas = planet.ambientOcclusion;
        }
    }
}
