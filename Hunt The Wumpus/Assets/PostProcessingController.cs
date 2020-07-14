using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using CommandView;

public class PostProcessingController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;

    public Planet planet;

    private Bloom _bloomLayer;
    private AmbientOcclusion _ambientOcclusionLayer;

    private bool _bloomWas;
    private bool _aowas;

    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out _bloomLayer);
        postProcessVolume.profile.TryGetSettings(out _ambientOcclusionLayer);
        
        _bloomWas = planet.bloom;
        _aowas = planet.ambientOcclusion;
    }

    void Update()
    {
        if (planet.bloom != _bloomWas)
        {
            _bloomLayer.enabled.value = planet.bloom;
            _bloomWas = planet.bloom;
        }
        if (planet.ambientOcclusion != _aowas)
        {
            _ambientOcclusionLayer.enabled.value = planet.ambientOcclusion;
            _aowas = planet.ambientOcclusion;
        }
    }
}
