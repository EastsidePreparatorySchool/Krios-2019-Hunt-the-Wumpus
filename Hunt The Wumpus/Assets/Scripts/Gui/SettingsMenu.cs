using CommandView;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class SettingsMenu : MonoBehaviour
    {
        public GameObject volumePercent;
        public Planet planet;
        public GameObject aoVolume;
        public GameObject creditsCanvas;
        public GameObject cursor;

        public bool inCredits;

        private int[,] Resolutions = new int[3, 2];

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
            if (transform.InverseTransformPoint(creditsCanvas.transform.Find("Panel/TextHolder")
                .GetComponent<RectTransform>().position).y >= 2990f && inCredits)
            {
                creditsCanvas.transform.Find("Panel/TextHolder").GetComponent<RectTransform>().transform
                    .Translate(0, -400, 0);
                Credits();
            }
        }

        //Gameplay Tab
        public void SetCursor(int index)
        {
            cursor.GetComponent<CursorController>().SetCursor(index);
        }

        public void ConfirmTurn(bool confirmTurnBool)
        {
            planet.confirmTurn = confirmTurnBool;
        }

        public void Credits()
        {
            inCredits = !inCredits;
            creditsCanvas.SetActive(!creditsCanvas.activeSelf);
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

        public void SetAo(bool aoOn)
        {
            planet.ambientOcclusion = aoOn;
        }

        public void SetVolume(float volume)
        {
            planet.volume = volume;
            print(volume);
            AudioListener.volume = volume;
            volumePercent.GetComponent<Text>().text = "Volume: " + Mathf.Round(volume * 100) + "%";
        }
    }
}