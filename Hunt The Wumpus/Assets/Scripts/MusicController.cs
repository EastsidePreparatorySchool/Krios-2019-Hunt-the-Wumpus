using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public Planet planetHandler;
    public void FadeOut()
    {
        planetHandler.isFadingMusic = true;
        StartCoroutine(FaderOut());
    }
    
    private IEnumerator FaderOut()
    {
        print("Fader Out");
        float elapsedTime = 0.5f;
        float curVol = AudioListener.volume;
        float dur = 1f;

        while (elapsedTime < dur)
        {
            elapsedTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(curVol, 0, elapsedTime / dur);
            yield return null;
        }
        print("Fader done");
        planetHandler.isFadingMusic = false;
    }
    // Update is called once per frame
    void Update()
    {
    }
}
