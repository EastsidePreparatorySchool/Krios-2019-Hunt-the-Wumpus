using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject volumePercent;
    public GameObject planet;

    public void SetVolume (float volume)
    {
        planet.GetComponent<Planet>().volume = volume;
        print(volume);
        AudioListener.volume = volume;
        volumePercent.GetComponent<Text>().text = "Volume: " + Mathf.Round(volume * 100).ToString() + "%";
    }

}
