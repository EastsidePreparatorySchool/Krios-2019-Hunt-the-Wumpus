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

    public GameObject Planet;

    private GameObject _faceInfoBox; // Blueprint for the UI elements that will be spawned

    private TextMeshProUGUI _troopCounter;
    private TextMeshProUGUI _moneyCounter;
    private TextMeshProUGUI _turnDisplay;

    private Planet _planetHandler;
    private GameMeta _inGameMeta;
    private FaceHandler[] _faceHandlers;
    private TextMeshProUGUI[] _faceDataHolderText;
    private TextMeshProUGUI[] _compArr;

    private CameraOrbit _orbit;

    private int _default = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // Fill Variables
        Planet = GameObject.Find("Planet");
        _planetHandler = Planet.GetComponent<Planet>();
        _faceInfoBox = GameObject.Find("FaceInfoBox");
        _faceInfoBox.SetActive(false);
        _compArr = GetComponentsInChildren<TextMeshProUGUI>();
        _orbit = FindObjectOfType<CameraOrbit>();

        _faceHandlers = new FaceHandler[_planetHandler.faces.Length];
        _faceDataHolderText = new TextMeshProUGUI[_planetHandler.faces.Length];
        for (int i = 0; i < _planetHandler.faces.Length; i++)
        {
            _faceHandlers[i] =
                _planetHandler.faces[i].GetComponent<FaceHandler>(); // Save computation time from repeated 
            FaceHandler faceHandler = _faceHandlers[i];
            faceHandler.faceDataHolder =
                Instantiate(_faceInfoBox, GameObject.Find("Canvas").transform); // Could be source of bug later on
            faceHandler.faceDataHolder.SetActive(false);
            faceHandler.faceDataHolder.GetComponentInChildren<Button>().onClick
                .AddListener(() => CloseFaceDataHolder(faceHandler));
            faceHandler.faceDataHolder.GetComponent<Dragging>().face = faceHandler;
            _faceDataHolderText[i] =
                faceHandler.faceDataHolder
                    .GetComponent<TextMeshProUGUI>(); // Reduce invocations of GetComponent() later in script
        }
        //

        // Sync UI appearance with camera entry spin
        foreach (TextMeshProUGUI i in _compArr)
        {
            switch (i.name)
            {
                case "TroopCounter":
                    _troopCounter = i;
                    _troopCounter.alpha = 0;
                    break;
                case "MoneyCounter":
                    _moneyCounter = i;
                    _moneyCounter.alpha = 0;
                    break;
                case "TurnNumDisplay":
                    _turnDisplay = i;
                    _turnDisplay.alpha = 1;
                    _turnDisplay.gameObject.SetActive(false);
                    break;
            }
        }
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
            _inGameMeta = Planet.GetComponent<GameMeta>();
        }

        //print("Stats: " + _inGameMeta);
       // print("Troops: " + _inGameMeta.troops);
        _troopCounter.text = "Troops: " + _inGameMeta.troops.Count;
        _moneyCounter.text = "Money: " + _inGameMeta.money;

        for (int i = 0; i < _faceHandlers.Length; i++)
        {
            FaceHandler face = _faceHandlers[i];
            if (face.displayFaceData)
            {
                // Update faceDataHolder GUI positioning
                if (!face.faceDataHolder.activeSelf)
                {
                    if (face.lastRightClickPos.x < Screen.width / 2)
                    {
                        face.faceDataHolder.transform.position =
                            new Vector2((float) (Screen.width * 0.25), face.lastRightClickPos.y);
                    }
                    else
                    {
                        face.faceDataHolder.transform.position =
                            new Vector2((float) (Screen.width * 0.75), face.lastRightClickPos.y);
                    }
                }

                face.faceDataHolder.SetActive(true);

                // Update faceDataHolder text
                TextMeshProUGUI text = _faceDataHolderText[i];
                bool[] hints = face.lastHintGiven;

                text.text = "<align=\"left\"><b>Tile " + face.transform.name + "</b></align>\n";
                if (hints[0])
                {
                    text.text += "Wumpus nearby\n";
                }

                if (hints[1])
                {
                    text.text += "Pit nearby\n";
                }

                if (hints[2])
                {
                    text.text += "Bat nearby\n";
                }

                if (!hints[0] && !hints[1] && !hints[2] && face.discovered == false)
                {
                    text.text += "Tile not yet seen.\n";
                }
                else if (!hints[0] && !hints[1] && !hints[2] && face.colonized == false)
                {
                    text.text += "Tile not yet visited.\n";
                }
                else if (!hints[0] && !hints[1] && !hints[2])
                {
                    text.text += "No hazards nearby!\n(last hint: " + face.turnsSinceLastHint + " turns ago)\n";
                }

                // text.text += "(last hint: " + face.turnsSinceLastHint + " turns ago)\n";

                if (face.colonized || face.discovered)
                {
                    text.text += "<align=\"right\">" + _inGameMeta.NumColonizedFaces() + "/" + _inGameMeta.totalFaces +
                                 " colonized</align>";
                }
                else
                {
                    text.text += "<align=\"right\">" + _inGameMeta.NumDiscoveredFaces() + "/" + _inGameMeta.totalFaces +
                                 " discovered</align>";
                }
            }
            else if (face.faceDataHolder)
            {
                face.faceDataHolder.SetActive(false);
            }
        }
    }

    private void CloseFaceDataHolder(FaceHandler faceHandler)
    {
        faceHandler.displayFaceData = false;
    }

    private IEnumerator WaitUntilGameBegins()
    {
        yield return new WaitUntil(() => Math.Abs(_orbit.beginningSpin) < 0.1f);

        while (_troopCounter.alpha < 1)
        {
            _troopCounter.alpha += 0.1F;
            _moneyCounter.alpha += 0.1F;

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