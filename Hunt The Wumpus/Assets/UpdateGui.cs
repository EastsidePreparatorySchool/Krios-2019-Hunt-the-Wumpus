using System;
using System.Collections;
using CommandView;
using Gui;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UpdateGui : MonoBehaviour
{
    // 
    // This NEEDS to be in the top-level "Assests" folder to work; Kenneth can vouch
    //
    // Script reads off variables in a bunch of other scripts to update UI elements.
    //

    public GameObject planet;

    private GameObject _faceInfoBox; // Blueprint for the UI elements that will be spawned

    private TextMeshProUGUI _troopCounter;
    private TextMeshProUGUI _moneyCounter;
    private TextMeshProUGUI _nukeCounter;
    private TextMeshProUGUI _turnDisplay;

    private Button _endTurnBtn;
    private Graphic _endTurnBtnTargetGraphic;
    private TextMeshProUGUI _endTurnBtnText;
    
    
    private Button _openStoreBtn;
    private Graphic _openStoreBtnTargetGraphic;
    private TextMeshProUGUI _openStoreBtnText;

    private int[] _counterValues = new int[3];

    private Planet _planetHandler;
    private GameMeta _inGameMeta;
    private FaceHandler[] _faceHandlers;
    private TextMeshProUGUI[] _faceDataHolderText;
    private TextMeshProUGUI[] _compArr;

    private CameraOrbit _orbit;

    // private int _default = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // Fill Variables
        planet = GameObject.Find("Planet");
        _planetHandler = planet.GetComponent<Planet>();

        _openStoreBtn = GameObject.Find("OpenStoreBtn").GetComponent<Button>();
        _openStoreBtnTargetGraphic = _openStoreBtn.targetGraphic;
        _openStoreBtnText = _openStoreBtn.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();
        
        _endTurnBtn = GameObject.Find("EndTurnBtn").GetComponent<Button>();
        _endTurnBtnTargetGraphic = _endTurnBtn.targetGraphic;
        _endTurnBtnText = _endTurnBtn.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();

        _compArr = GetComponentsInChildren<TextMeshProUGUI>();
        _orbit = FindObjectOfType<CameraOrbit>();


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

        StartCoroutine(TurnDisplayAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Implement Troops, Money, & Score
        if (_inGameMeta == null)
        {
            _inGameMeta = planet.GetComponent<GameMeta>();
        }

        //print("Stats: " + _inGameMeta);
        // print("Troops: " + _inGameMeta.troops);
        int[] curCounterValue = {_inGameMeta.availableTroops.Count, _inGameMeta.money, _inGameMeta.nukes};
        if (!_counterValues.Equals(curCounterValue))
        {
            _troopCounter.text = "Available Troops: " + _inGameMeta.availableTroops.Count + "/" 
                                 + (_inGameMeta.exhaustedTroops.Count + _inGameMeta.availableTroops.Count);
            _moneyCounter.text = "Money: " + _inGameMeta.money;
            _nukeCounter.text = "Nukes: " + _inGameMeta.nukes;
        }
    }

    private IEnumerator WaitUntilGameBegins()
    {
        yield return new WaitUntil(() => Math.Abs(_orbit.beginningSpin) < 0.1f);

        Color openStoreBtnAlpha = _openStoreBtnTargetGraphic.color;
        Color endTurnBtnAlpha = _endTurnBtnTargetGraphic.color;
        while (_troopCounter.alpha < 1)
        {
            _troopCounter.alpha += 0.1f;
            _moneyCounter.alpha += 0.1f;
            _nukeCounter.alpha += 0.1f;

            _openStoreBtnText.alpha += 0.1f;
            openStoreBtnAlpha.a += 0.1f;
            _openStoreBtnTargetGraphic.color = openStoreBtnAlpha;
            
            _endTurnBtnText.alpha += 0.1f;
            endTurnBtnAlpha.a += 0.1f;
            _endTurnBtnTargetGraphic.color = openStoreBtnAlpha;

            yield return new WaitForSeconds(0.2F);
        }
    }

    private IEnumerator TurnDisplayAnimation()
    {
        yield return new WaitUntil(() => Math.Abs(_orbit.beginningSpin) < 0.1f);

        if (_inGameMeta.turnsElapsed == 1)
        {
            _turnDisplay.text = "Start!";
        }
        else
        {
            _turnDisplay.text = "Turn " + _inGameMeta.turnsElapsed;
        }

        _turnDisplay.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        while (_turnDisplay.alpha > 0)
        {
            _turnDisplay.alpha -= 0.2F;
            yield return new WaitForSeconds(0.2F);
        }

        _turnDisplay.gameObject.SetActive(false);
    }
}