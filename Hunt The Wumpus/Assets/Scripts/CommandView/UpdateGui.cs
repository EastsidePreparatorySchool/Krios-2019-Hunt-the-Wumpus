using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CommandView
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UpdateGui : MonoBehaviour
    {
        private GameObject _planet;
        private Planet _planetHandler;

        // private GameObject _faceInfoBox; // Blueprint for the UI elements that will be spawned

        public CanvasGroup canvasGroup;

        public TextMeshProUGUI troopCounter;
        public TextMeshProUGUI moneyCounter;
        public TextMeshProUGUI nukeCounter;
        public TextMeshProUGUI sensorCounter;
        public TextMeshProUGUI turnDisplay;

        private readonly int[] _counterValues = new int[4];

        private GameMeta _inGameMeta;

        private FaceHandler[] _faceHandlers;
        // private TextMeshProUGUI[] _faceDataHolderText;

        public MainMenuVars menu;

        private bool _isPaused;

        // private int _default = 0;

        // Start is called before the first frame update
        void Start()
        {
            // get Planet refs
            _planet = GameObject.Find("Planet");
            _planetHandler = _planet.GetComponent<Planet>();
            _inGameMeta = _planet.GetComponent<GameMeta>();

            // Sync UI appearance with camera entry spin
            canvasGroup.alpha = 0f;
            turnDisplay.alpha = 1f;
            turnDisplay.gameObject.SetActive(false);

            StartCoroutine(FadeAnim(false));
        }

        // Update is called once per frame
        void Update()
        {
            int[] curCounterValue =
                {_inGameMeta.availableTroops.Count, _inGameMeta.money, _inGameMeta.nukes, _inGameMeta.sensorTowers};
            if (!_counterValues.Equals(curCounterValue))
            {
                troopCounter.text = "Troops: " + _inGameMeta.availableTroops.Count + "/" +
                                    (_inGameMeta.exhaustedTroops.Count + _inGameMeta.availableTroops.Count);
                moneyCounter.text = "Money: " + _inGameMeta.money;
                nukeCounter.text = "Nukes: " + _inGameMeta.nukes;
                sensorCounter.text = "Sensor Towers: " + _inGameMeta.sensorTowers;
            }

            if (_inGameMeta.turnsElapsed != _planetHandler.lastDisplayedTurn)
            {
                StartCoroutine(TurnDisplayAnimation(_inGameMeta.turnsElapsed));
                _planetHandler.lastDisplayedTurn = _inGameMeta.turnsElapsed;
            }

            if (menu.isPause)
            {
                if (_isPaused == false)
                {
                    _isPaused = menu.isPause;
                    StartCoroutine(FadeAnim());
                }
            }

            if (menu.isPause == false)
            {
                if (_isPaused)
                {
                    _isPaused = menu.isPause;
                    StartCoroutine(FadeAnim(false));
                }
            }
        }

        private IEnumerator FadeAnim(bool fadeOut = true)
        {
            yield return new WaitUntil(() => menu.isPause == fadeOut);
            if (!_inGameMeta.gameInPlay && !fadeOut)
            {
                yield return new WaitForSeconds(2f);
                _inGameMeta.gameInPlay = true;
            }

            float fadeDuration = 0.25f;

            if (fadeOut && canvasGroup.alpha > 0)
            {
                StartCoroutine(LerpAlpha(result => canvasGroup.alpha = result, 1, 0,
                    fadeDuration));
            }
            else if (!fadeOut && canvasGroup.alpha < 1)
            {
                StartCoroutine(LerpAlpha(result => canvasGroup.alpha = result, 0, 1,
                    fadeDuration));
            }

            if (fadeOut)
            {
                _planetHandler.readyToPlay = true;
            }
            else
            {
                _planetHandler.readyToPause = true;
            }
        }

        private IEnumerator TurnDisplayAnimation(int turnToDisplay)
        {
            yield return new WaitUntil(() => menu.isPause == false);

            turnDisplay.text = "Turn " + turnToDisplay;

            turnDisplay.alpha = 1;
            turnDisplay.gameObject.SetActive(true);

            yield return new WaitForSeconds(2);

            float fade = 1.5f;
            StartCoroutine(LerpAlpha(result => turnDisplay.alpha = result, 1, 0, fade));
            yield return new WaitForSeconds(fade);

            turnDisplay.gameObject.SetActive(false);
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