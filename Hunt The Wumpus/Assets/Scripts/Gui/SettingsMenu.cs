using System.Collections;
using CommandView;
using TMPro;
using UnityEngine;
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

            SetupDrops();
        }

        void Update()
        {
        }

        public void SetupDrops()
        {
            cursorDrop.value = _planet.CursorIndex;
            waypointDrop.value = _planet.WaypointIndex;
        }

        //Gameplay Tab
        public void SetCursor(int index)
        {
            _planet.CursorIndex = index;
            _cursor.GetComponent<CursorController>().SetCursor(index);
        }

        public void SetWaypoint(int index)
        {
            _planet.WaypointIndex = index;
        }

        public void ConfirmTurn(bool confirmTurnBool)
        {
            _planet.confirmTurn = confirmTurnBool;
        }

        public void SetVideo(bool toggle)
        {
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
            Screen.SetResolution(_resolutions[resolutionIndex, 0], _resolutions[resolutionIndex, 1], Screen.fullScreen);
            print(Screen.currentResolution);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetQuality(float floatInex)
        {
            int qualityInex = (int)floatInex;
            QualitySettings.SetQualityLevel(qualityInex);
        }

        public void SetBloom(bool bloomOn)
        {
            _planet.bloom = bloomOn;
        }

        public void SetAo(bool aoOn)
        {
            _planet.ambientOcclusion = aoOn;
        }

        public void SetVolume(float volume)
        {
            _planet.volume = volume;
            print(volume);
            AudioListener.volume = volume;
            volumePercent.GetComponent<Text>().text = "Volume: " + Mathf.Round(volume * 100) + "%";
        }
    }
}