using System.Collections;
using CommandView;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Gui
{
    public class SettingsMenu : MonoBehaviour
    {
        public GameObject volumePercent;
        private Planet _planet;
        public GameObject aoVolume;
        public GameObject creditsCanvas;
        public VideoPlayer creditsVideo;
        public CanvasGroup otherUi;
        private GameObject _cursor;


        public TMP_Dropdown cursorDrop;
        public TMP_Dropdown waypointDrop;
        public Toggle ConfirmTurnToggle;

        public Slider ResolutionSlider;
        public Toggle FullScreenToggle;
        public Slider QualitySlider;
        public Toggle BloomToggle;
        public Toggle AOToggle;
        public Slider VolumeSlider;


        public bool inCredits;

        private int[,] _resolutions = new int[3, 2];

        void Start()
        {
            _resolutions[0, 0] = 1280;
            _resolutions[0, 1] = 720;

            _resolutions[1, 0] = 1920;
            _resolutions[1, 1] = 1080;

            _resolutions[2, 0] = 2560;
            _resolutions[2, 1] = 1440;

            _planet = GameObject.Find("Planet").GetComponent<Planet>();
            _cursor = GameObject.Find("CursorImage");

            SetupProps();
        }

        void Update()
        {
        }

        public void SetupProps()
        {
            cursorDrop.value = PlayerPrefs.GetInt("Cursor", 0);
            waypointDrop.value = PlayerPrefs.GetInt("Waypoint", 4);
            ConfirmTurnToggle.isOn = PlayerPrefs.GetInt("ConfirmTurn", 0) == 1 ? true : false;

            ResolutionSlider.value = PlayerPrefs.GetInt("Resolution", 1);
            FullScreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1 ? true : false;
            QualitySlider.value = PlayerPrefs.GetInt("Quality", 1);
            BloomToggle.isOn = PlayerPrefs.GetInt("Bloom", 1) == 1 ? true : false;
            AOToggle.isOn = PlayerPrefs.GetInt("AmbientOcclusion", 1) == 1 ? true : false;
            VolumeSlider.value = PlayerPrefs.GetFloat("Volume", .5f);
        }

        //Gameplay Tab
        public void SetCursor(int index)
        {
            PlayerPrefs.SetInt("Cursor", index);
            _cursor.GetComponent<CursorController>().SetCursor(index);
        }

        public void SetWaypoint(int index)
        {
            PlayerPrefs.SetInt("Waypoint", index);
        }

        public void ConfirmTurn(bool confirmTurnBool)
        {
            PlayerPrefs.SetInt("ConfirmTurn", confirmTurnBool ? 1 : 0);
        }

        public void SetVideo(bool toggle)
        {
            ////////////////////////////fix this up
            if (toggle)
                PlayerPrefs.DeleteKey("needPlayIntroVid");
            else
                PlayerPrefs.SetInt("needPlayIntroVid", 0);
            PlayerPrefs.Save();
        }

        IEnumerator CreditsAfterDelay(float time)
        {
            yield return new WaitForSeconds(time);
            creditsCanvas.SetActive(!creditsCanvas.activeSelf);
        }

        public void Credits()
        {
            inCredits = !inCredits;
            creditsCanvas.SetActive(!creditsCanvas.activeSelf);
            if (inCredits)
            {
                StartCoroutine(PrepAndCloseCredits());
            }
            else
            {
                otherUi.alpha = 1;
            }
        }

        private IEnumerator PrepAndCloseCredits()
        {
            // creditsVideo.Prepare();
            yield return new WaitUntil(() => creditsVideo.isPrepared);
            // creditsVideo.Play();
            otherUi.alpha = 0;

            yield return new WaitUntil(() => !creditsVideo.isPlaying);

            if (inCredits)
            {
                Credits();
            }
        }


        //AV Tab
        public void SetResolution(float floatIndex)
        {
            int resolutionIndex = (int) floatIndex;
            PlayerPrefs.SetInt("Resolution", resolutionIndex);
            Screen.SetResolution(_resolutions[resolutionIndex, 0], _resolutions[resolutionIndex, 1], Screen.fullScreen);
            print(Screen.currentResolution);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            Screen.fullScreen = isFullscreen;
        }

        public void SetQuality(float floatInex)
        {
            int qualityInex = (int) floatInex;
            PlayerPrefs.SetInt("Quality", qualityInex);
            QualitySettings.SetQualityLevel(qualityInex);
        }

        public void SetBloom(bool bloomOn)
        {
            PlayerPrefs.SetInt("Bloom", bloomOn ? 1 : 0);
            _planet.bloom = bloomOn;
        }

        public void SetAo(bool aoOn)
        {
            PlayerPrefs.SetInt("AmbientOcclusion", aoOn ? 1 : 0);
            _planet.ambientOcclusion = aoOn;
        }

        public void SetVolume(float volume)
        {
            PlayerPrefs.SetFloat("Volume", volume);
            _planet.volume = volume;
            print(volume);
            AudioListener.volume = volume;
            volumePercent.GetComponent<Text>().text = "Volume: " + Mathf.Round(volume * 100) + "%";
        }
    }
}