using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace CommandView
{
    public class WinStats : MonoBehaviour
    {
        public VideoPlayer winVid;

        public TextMeshProUGUI turnsTxt;
        public TextMeshProUGUI troopsTxt;
        public TextMeshProUGUI sensorsTxt;
        public TextMeshProUGUI nukesTxt;

        public CanvasGroup statAlpha;

        private GameMeta _meta;

        // Start is called before the first frame update
        void Start()
        {
            _meta = GameObject.Find("Planet").GetComponent<Planet>().meta;

            turnsTxt.text = "In " + _meta.turnsElapsed + " Turns";
            turnsTxt.alpha = 0;
            
            troopsTxt.text = "With " + _meta.troopsUsed + " Troops"; // TODO: count this
            troopsTxt.alpha = 0;
            
            sensorsTxt.text = _meta.sensorTowersUsed + " Sensor Towers"; // TODO: count this
            sensorsTxt.alpha = 0;
            
            nukesTxt.text = "And " + _meta.nukesUsed + " Nukes"; // TODO: count this
            nukesTxt.alpha = 0;

            StartCoroutine(FadeInAnim());
        }

        private IEnumerator FadeInAnim()
        {
            float fadeDur = 0.25f;

            StartCoroutine(LerpAlpha(result => turnsTxt.alpha = result, 0, 1, fadeDur));
            
            yield return new WaitUntil(() => winVid.time >= 3.71666);
            StartCoroutine(LerpAlpha(result => troopsTxt.alpha = result, 0, 1, fadeDur));
            
            yield return new WaitUntil(() => winVid.time >= 7.25);
            StartCoroutine(LerpAlpha(result => sensorsTxt.alpha = result, 0, 1, fadeDur));
            
            yield return new WaitUntil(() => winVid.time >= 11.0);
            StartCoroutine(LerpAlpha(result => nukesTxt.alpha = result, 0, 1, fadeDur));
            
            yield return new WaitUntil(()=>winVid.time >= 14.3);
            statAlpha.alpha = 0;
        }

        private IEnumerator LerpAlpha(Action<float> target, float from, float to, float time)
        {
            float lerpStart = Time.time;
            while (true)
            {
                var progress = Time.time - lerpStart;
                target.Invoke(Mathf.Lerp(from, to, progress / time));
                if (time < progress)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}