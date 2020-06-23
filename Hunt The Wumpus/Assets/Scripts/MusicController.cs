using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public void FadeOut()
    {
        StartCoroutine(FaderOut());
    }
    
    private IEnumerator FaderOut()
    {
        float elapsedTime = 0.5f;
        float curVol = AudioListener.volume;
        float dur = 1f;

        while (elapsedTime < dur)
        {
            elapsedTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(curVol, 0, elapsedTime / dur);
            yield return null;
        }       
    }
    // Update is called once per frame
    void Update()
    {
    }
}
