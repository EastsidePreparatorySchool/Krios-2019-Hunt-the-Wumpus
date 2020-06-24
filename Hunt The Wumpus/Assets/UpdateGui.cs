﻿using System;
using System.Collections;
using System.Runtime.InteropServices;
using CommandView;
using Gui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class UpdateGui : MonoBehaviour
{
    // 
    // This NEEDS to be in the top-level "Assests" folder to work; Kenneth can vouch
    //
    // Script reads off variables in a bunch of other scripts to update UI elements.
    //


    private GameObject _planet;
    private Planet _planetScript;

    private GameObject _faceInfoBox; // Blueprint for the UI elements that will be spawned

    private TextMeshProUGUI _troopCounter;
    private TextMeshProUGUI _moneyCounter;
    private TextMeshProUGUI _nukeCounter;
    private TextMeshProUGUI _sensorCounter;
    private TextMeshProUGUI _turnDisplay;

    private Button _endTurnBtn;
    private Graphic _endTurnBtnTargetGraphic;
    private TextMeshProUGUI _endTurnBtnText;


    private Button _openStoreBtn;
    private Graphic _openStoreBtnTargetGraphic;
    private TextMeshProUGUI _openStoreBtnText;

    private int[] _counterValues = new int[4];

    private Planet _planetHandler;
    private GameMeta _inGameMeta;
    private FaceHandler[] _faceHandlers;
    private TextMeshProUGUI[] _faceDataHolderText;
    private TextMeshProUGUI[] _compArr;

    private PlanetSpin _orbit;

    private MainMenuVars _menu;

    private bool _ispaused;

    // private int _default = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Fill Variables
        _planet = GameObject.Find("Planet");
        _planetScript = _planet.GetComponent<Planet>();
        _planetHandler = _planetScript.GetComponent<Planet>();

        _openStoreBtn = GameObject.Find("OpenStoreBtn").GetComponent<Button>();
        _openStoreBtnTargetGraphic = _openStoreBtn.targetGraphic;
        _openStoreBtnText = _openStoreBtn.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();

        _endTurnBtn = GameObject.Find("EndTurnBtn").GetComponent<Button>();
        _endTurnBtnTargetGraphic = _endTurnBtn.targetGraphic;
        _endTurnBtnText = _endTurnBtn.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();

        _compArr = GetComponentsInChildren<TextMeshProUGUI>();
        _orbit = FindObjectOfType<PlanetSpin>();

        _menu = GameObject.Find("Main Camera").GetComponent<MainMenuVars>();


        // Sync UI appearance with camera entry spin
        foreach (TextMeshProUGUI i in _compArr)
        {
            switch (i.name)
            {
                case "TroopCounter":
                    _troopCounter = i;
                    _troopCounter.alpha = 0f;
                    break;
                case "MoneyCounter":
                    _moneyCounter = i;
                    _moneyCounter.alpha = 0f;
                    break;
                case "NukeCounter":
                    _nukeCounter = i;
                    _nukeCounter.alpha = 0f;
                    break;
                case "SensorCounter":
                    _sensorCounter = i;
                    _sensorCounter.alpha = 0f;
                    break;
                case "TurnNumDisplay":
                    _turnDisplay = i;
                    _turnDisplay.alpha = 1f;
                    _turnDisplay.gameObject.SetActive(false);
                    break;
            }
        }

        Color storeBtnAlpha = _openStoreBtnTargetGraphic.color;
        _openStoreBtnText.alpha = 0f;
        storeBtnAlpha.a = 0f;
        _openStoreBtnTargetGraphic.color = storeBtnAlpha;

        Color endTurnBtnAlpha = _endTurnBtnTargetGraphic.color;
        _endTurnBtnText.alpha = 0f;
        endTurnBtnAlpha.a = 0f;
        _endTurnBtnTargetGraphic.color = endTurnBtnAlpha;

        //

        StartCoroutine(WaitUntilGameBegins());


        // StartCoroutine(TurnDisplayAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Implement Troops, Money, & Score
        if (_inGameMeta == null)
        {
            _inGameMeta = _planetScript.GetComponent<GameMeta>();
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

        if (_inGameMeta.turnsElapsed != _planetScript.lastDisplayedTurn)
        {
            StartCoroutine(TurnDisplayAnimation(_inGameMeta.turnsElapsed));
            _planetScript.lastDisplayedTurn = _inGameMeta.turnsElapsed;
        }

        if (_menu.isPause == true)
        {
            if (_ispaused == false)
            {
                _ispaused = _menu.isPause;
                StartCoroutine(FadeOut());
            }
        }

        if (_menu.isPause == false)
        {
            if (_ispaused == true)
            {
                _ispaused = _menu.isPause;
                StartCoroutine(WaitUntilGameBegins());
            }
        }
    }

    private IEnumerator WaitUntilGameBegins()
    {
        yield return new WaitUntil(() => _menu.isPause == false);
        yield return new WaitForSeconds(2F);
        Color openStoreBtnAlpha = _openStoreBtnTargetGraphic.color;
        Color endTurnBtnAlpha = _endTurnBtnTargetGraphic.color;
        
        /*while (openStoreBtnAlpha.a < 1)
        {
            coverColor.a += 0.1f;
            _textCover.color = coverColor;

            _troopCounter.alpha += 0.1f;
            _moneyCounter.alpha += 0.1f;
            _nukeCounter.alpha += 0.1f;
            _sensorCounter.alpha += 0.1f;

            _openStoreBtnText.alpha += 0.1f;
            openStoreBtnAlpha.a += 0.1f;
            _openStoreBtnTargetGraphic.color = openStoreBtnAlpha;

            _endTurnBtnText.alpha += 0.1f;
            endTurnBtnAlpha.a += 0.1f;
            _endTurnBtnTargetGraphic.color = openStoreBtnAlpha;

            yield return new WaitForSeconds(0.025F);
        }*/
        
        float fade = 0.25f;
        if (_troopCounter.alpha < 1)
        {
            StartCoroutine(LerpAlpha(result => _troopCounter.alpha = result, 0, 1, fade));
            StartCoroutine(LerpAlpha(result => _moneyCounter.alpha = result, 0, 1, fade));
            StartCoroutine(LerpAlpha(result => _nukeCounter.alpha = result, 0, 1, fade));
            StartCoroutine(LerpAlpha(result => _sensorCounter.alpha = result, 0, 1, fade));
            
            StartCoroutine(LerpAlpha(result => _openStoreBtnText.alpha = result, 0, 1, fade));
            StartCoroutine(LerpAlpha(result =>
            {
                openStoreBtnAlpha.a = result;
                _openStoreBtnTargetGraphic.color = openStoreBtnAlpha;
            }, 0, 1, fade));

            StartCoroutine(LerpAlpha(result => _endTurnBtnText.alpha = result, 0, 1, fade));
            StartCoroutine(LerpAlpha(result =>
            {
                endTurnBtnAlpha.a = result;
                _endTurnBtnTargetGraphic.color = endTurnBtnAlpha;
            }, 0, 1, fade));
        }
        _planetHandler.readyToPause = true;
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitUntil(() => _menu.isPause == true);
        Color openStoreBtnAlpha = _openStoreBtnTargetGraphic.color;
        Color endTurnBtnAlpha = _endTurnBtnTargetGraphic.color;
        
        /*while (_troopCounter.alpha > 0)
        {
            coverColor.a -= 0.1f;
            _textCover.color = coverColor;

            _troopCounter.alpha -= 0.1f;
            _moneyCounter.alpha -= 0.1f;
            _nukeCounter.alpha -= 0.1f;
            _sensorCounter.alpha -= 0.1f;

            _openStoreBtnText.alpha -= 0.1f;
            openStoreBtnAlpha.a -= 0.1f;
            _openStoreBtnTargetGraphic.color = openStoreBtnAlpha;

            _endTurnBtnText.alpha -= 0.1f;
            endTurnBtnAlpha.a -= 0.1f;
            _endTurnBtnTargetGraphic.color = openStoreBtnAlpha;

            yield return new WaitForSeconds(0.025F);
        }*/

        float fade = 0.25f;
        if (_troopCounter.alpha > 0)
        {
            StartCoroutine(LerpAlpha(result => _troopCounter.alpha = result, 1, 0, fade));
            StartCoroutine(LerpAlpha(result => _moneyCounter.alpha = result, 1, 0, fade));
            StartCoroutine(LerpAlpha(result => _nukeCounter.alpha = result, 1, 0, fade));
            StartCoroutine(LerpAlpha(result => _sensorCounter.alpha = result, 1, 0, fade));
            
            StartCoroutine(LerpAlpha(result => _openStoreBtnText.alpha = result, 1, 0, fade));
            StartCoroutine(LerpAlpha(result =>
            {
                openStoreBtnAlpha.a = result;
                _openStoreBtnTargetGraphic.color = openStoreBtnAlpha;
            }, 1, 0, fade));

            StartCoroutine(LerpAlpha(result => _endTurnBtnText.alpha = result, 1, 0, fade));
            StartCoroutine(LerpAlpha(result =>
            {
                endTurnBtnAlpha.a = result;
                _endTurnBtnTargetGraphic.color = endTurnBtnAlpha;
            }, 1, 0, fade));
        }
        _planetHandler.readyToPlay = true;
    }

    private IEnumerator TurnDisplayAnimation(int turnToDisplay)
    {
        yield return new WaitUntil(() => _menu.isPause == false);
        
        _turnDisplay.text = "Turn " + turnToDisplay;

        _turnDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        /*while (_turnDisplay.alpha > 0)
        {
            _turnDisplay.alpha -= 0.2F;
            yield return new WaitForSeconds(0.2F);
        }*/

        float fade = 1.5f;
        StartCoroutine(LerpAlpha(result => _turnDisplay.alpha = result, 1, 0, fade));
        yield return new WaitForSeconds(fade);

        _turnDisplay.gameObject.SetActive(false);
    }

    /*private IEnumerator LerpAlpha(TextMeshProUGUI target, float from, float to, float time)
    {
        float lerpStart = Time.time;
        while (true)
        {
            var progress = Time.time - lerpStart;
            target.alpha = Mathf.Lerp(from, to, progress / time);
            if (time < progress)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }*/
    
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