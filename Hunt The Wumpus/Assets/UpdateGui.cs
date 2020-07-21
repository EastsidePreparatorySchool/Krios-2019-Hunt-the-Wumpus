using System;
using System.Collections;
using CommandView;
using UnityEngine;
using TMPro;


public class UpdateGui : MonoBehaviour
{
    // 
    // This NEEDS to be in the top-level "Assets" folder to work; Kenneth can vouch
    //
    // Script reads off variables in a bunch of other scripts to update UI elements.
    //


    private GameObject _planet;
    private Planet _planetHandler;

    private GameObject _faceInfoBox; // Blueprint for the UI elements that will be spawned

    private CanvasGroup _canvasGroup;

    private TextMeshProUGUI _troopCounter;
    private TextMeshProUGUI _moneyCounter;
    private TextMeshProUGUI _nukeCounter;
    private TextMeshProUGUI _sensorCounter;
    private TextMeshProUGUI _turnDisplay;

    private readonly int[] _counterValues = new int[4];

    private GameMeta _inGameMeta;
    private FaceHandler[] _faceHandlers;
    private TextMeshProUGUI[] _faceDataHolderText;
    private TextMeshProUGUI[] _compArr;

    // private PlanetSpin _orbit;

    private MainMenuVars _menu;

    private bool _isPaused;

    // private int _default = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Fill Variables
        _planet = GameObject.Find("Planet");
        _planetHandler = _planet.GetComponent<Planet>();

        _canvasGroup = GameObject.Find("MainUICvsGroup").GetComponent<CanvasGroup>();

        _compArr = GetComponentsInChildren<TextMeshProUGUI>();
        // _orbit = FindObjectOfType<PlanetSpin>();

        _menu = GameObject.Find("Main Camera").GetComponent<MainMenuVars>();


        // Sync UI appearance with camera entry spin
        _canvasGroup.alpha = 0f;
        foreach (TextMeshProUGUI i in _compArr)
        {
            switch (i.name)
            {
                case "TroopCounter":
                    _troopCounter = i;
                    break;
                case "MoneyCounter":
                    _moneyCounter = i;
                    break;
                case "NukeCounter":
                    _nukeCounter = i;
                    break;
                case "SensorCounter":
                    _sensorCounter = i;
                    break;
                case "TurnNumDisplay":
                    _turnDisplay = i;
                    _turnDisplay.alpha = 1f;
                    _turnDisplay.gameObject.SetActive(false);
                    break;
            }
        }

        StartCoroutine(FadeAnim(false));
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Implement Troops, Money, & Score
        if (_inGameMeta == null)
        {
            _inGameMeta = _planetHandler.GetComponent<GameMeta>();
        }

        //print("Stats: " + _inGameMeta);
        // print("Troops: " + _inGameMeta.troops);
        int[] curCounterValue =
            {_inGameMeta.availableTroops.Count, _inGameMeta.money, _inGameMeta.nukes, _inGameMeta.sensorTowers};
        if (!_counterValues.Equals(curCounterValue))
        {
            _troopCounter.text = "Troops: " + _inGameMeta.availableTroops.Count + "/" +
                                 (_inGameMeta.exhaustedTroops.Count + _inGameMeta.availableTroops.Count);
            _moneyCounter.text = "Money: " + _inGameMeta.money;
            _nukeCounter.text = "A.R.R.O.Ws: " + _inGameMeta.nukes;
            _sensorCounter.text = "Sensor Towers: " + _inGameMeta.sensorTowers;
        }

        if (_inGameMeta.turnsElapsed != _planetHandler.lastDisplayedTurn)
        {
            StartCoroutine(TurnDisplayAnimation(_inGameMeta.turnsElapsed));
            _planetHandler.lastDisplayedTurn = _inGameMeta.turnsElapsed;
        }

        if (_menu.isPause)
        {
            if (_isPaused == false)
            {
                _isPaused = _menu.isPause;
                StartCoroutine(FadeAnim());
            }
        }

        if (_menu.isPause == false)
        {
            if (_isPaused)
            {
                _isPaused = _menu.isPause;
                StartCoroutine(FadeAnim(false));
            }
        }
    }

    private IEnumerator FadeAnim(bool fadeOut = true)
    {
        yield return new WaitUntil(() => _menu.isPause == fadeOut);
        if (!_inGameMeta.gameInPlay && !fadeOut)
        {
            yield return new WaitForSeconds(2f);
            _inGameMeta.gameInPlay = true;
        }

        float fadeDuration = 0.25f;

        if (fadeOut && _canvasGroup.alpha > 0)
        {
            StartCoroutine(LerpAlpha(result => _canvasGroup.alpha = result, 1, 0,
                fadeDuration));
        } else if (!fadeOut && _canvasGroup.alpha < 1)
        {
            StartCoroutine(LerpAlpha(result => _canvasGroup.alpha = result, 0, 1,
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
        yield return new WaitUntil(() => _menu.isPause == false);

        _turnDisplay.text = "Turn " + turnToDisplay;

        _turnDisplay.alpha = 1;
        _turnDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        float fade = 1.5f;
        StartCoroutine(LerpAlpha(result => _turnDisplay.alpha = result, 1, 0, fade));
        yield return new WaitForSeconds(fade);

        _turnDisplay.gameObject.SetActive(false);
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