using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject volumePercent;
    public Planet planet;
    public GameObject AOVolume;
    public GameObject CreditsCanvas;

    public bool inCredits = false;

    private int[,] Resolutions = new int[3,2];

    void Start()
    {
        Resolutions[0, 0] = 1280;
        Resolutions[0, 1] = 720;

        Resolutions[1, 0] = 1920;
        Resolutions[1, 1] = 1080;

        Resolutions[2, 0] = 2560;
        Resolutions[2, 1] = 1440;
    }

    void Update()
    {
        if (transform.InverseTransformPoint(CreditsCanvas.transform.Find("Panel/TextHolder").GetComponent<RectTransform>().position).y > 3400f)
        {
            CreditsCanvas.transform.Find("Panel/TextHolder").GetComponent<RectTransform>().transform.Translate(0, -400, 0);
            Credits();
        }
    }
    //Gameplay Tab
    public void ConfirmTurn(bool ConfirmTurnBool)
    {
        planet.ConfirmTurn = ConfirmTurnBool;
    }

    public void Credits()
    {
        inCredits = !inCredits;
        CreditsCanvas.SetActive(!CreditsCanvas.activeSelf);
    }


    //AV Tab
    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(Resolutions[resolutionIndex, 0], Resolutions[resolutionIndex, 1], Screen.fullScreen);
        print(Screen.currentResolution);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetQuality(int qualityInex)
    {
        QualitySettings.SetQualityLevel(qualityInex);
    }

    public void SetBloom(bool bloomOn)
    {
        planet.bloom = bloomOn;
    }

    public void SetAO(bool AOOn)
    {
        planet.ambientOcclusion = AOOn;
    }

    public void SetVolume (float volume)
    {
        planet.volume = volume;
        print(volume);
        AudioListener.volume = volume;
        volumePercent.GetComponent<Text>().text = "Volume: " + Mathf.Round(volume * 100).ToString() + "%";
    }
}
