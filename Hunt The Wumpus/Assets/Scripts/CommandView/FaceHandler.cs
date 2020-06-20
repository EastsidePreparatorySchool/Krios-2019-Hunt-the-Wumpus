using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gui;
//using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// using Random = System.Random;

//using Random = System.Random;

namespace CommandView
{
    public class MiniGameResult
    {
        public List<TroopMeta> inGameTroops;
        public int moneyCollected;
        public bool didWin;

        public MiniGameResult(List<TroopMeta> troopsToSend)
        {
            inGameTroops = troopsToSend;
        }
    }

    public enum BiomeType
    {
        None,
        Planes,
        Desert,
        Jungle,
    }

    // Data object and event handler for haves on CommandView planet
    public class FaceHandler : MonoBehaviour
    {
        //Public variables
        public static readonly Color UndiscoveredColor = new Color(.1f, .1f, .2f);

        public static readonly Color[] DiscoveredColor =
        {
            new Color(.25f, .25f, .25f), new Color(168f / 255f, 117f / 255f, 50f / 255f),
            new Color(66f / 255f, 92f / 255f, 49f / 255f)
        };

        public static readonly Color[] ColonizedColor =
        {
            Color.gray, new Color(227f / 255f, 156f / 255f, 64f / 255f), new Color(90f / 255f, 140f / 255f, 47f / 255f),
        };
        // new Color(173f/255f, 144f/255f, 106f/255f)

        //individual variables
        private Planet _planet; // store reference back to planet
        public bool[] state;

        // Face Properties
        public GameObject[] adjacentFaces;
        public FaceHandler[] adjacentFaceHandlers;
        public bool discovered;
        public bool colonized;
        public int lastSeenTurnsAgo;
        public BiomeType biomeType;

        public Vector3 lastRightClickPos;
        public bool noMoney;

        public List<MeshVertex> meshVertices = new List<MeshVertex>();

        // Game-relevant stats in this script are sent to a centralized location
        private GameMeta _ingameStat;
        public bool[] lastHintGiven;
        public int turnsSinceLastHint;
        public bool showHintOnTile;

        private HazardTypes _hazardObject = HazardTypes.None;
        private int _faceNumber;

        private bool _firstTimeRun;
        private bool playMiniGame = true; // you can turn this off if you just want to mess with the map

        private GameMeta meta;
        public List<TroopMeta> heldTroops = new List<TroopMeta>();

        public GameObject faceDataHolder; // assigned in this class's methods, used in the UpdateGui script
        public MeshFilter faceMeshFilter;
        public bool faceDataShowing;
        public Vector3 faceCenter;
        public Vector3 faceNormal;
        private List<GameObject> _hintsGo; // 0 = Wumpus, 1 = Pit, 2 = Bats
        private Vector3 _majorAxisA;
        private Vector3 _majorAxisB;

        private void Awake()
        {
            // Initialize variables
            _planet = transform.parent.gameObject.GetComponent<Planet>();
            _faceNumber = Convert.ToInt32(gameObject.name) - 1;
            _ingameStat = _planet.GetComponent<GameMeta>();
            lastHintGiven = _planet.GetHintsToGive();
            state = new[] {true, true, true, true};
            biomeType = BiomeType.None;
            meta = _planet.GetComponent<GameMeta>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            faceMeshFilter = GetComponent<MeshFilter>();
            CalculateFaceGeometry();

            adjacentFaceHandlers = new FaceHandler[adjacentFaces.Length];
            for (int i = 0; i < adjacentFaces.Length; i++)
            {
                adjacentFaceHandlers[i] = adjacentFaces[i].GetComponent<FaceHandler>();
            }

            RegenerateHintGOs();
        }

        private void CalculateFaceGeometry()
        {
            Mesh faceMesh = faceMeshFilter.mesh;
            List<Vector3> transformedVerts = new List<Vector3>();
            faceCenter = new Vector3();

            // Calculate center
            foreach (Vector3 faceMeshVertex in faceMesh.vertices)
            {
                Vector3 transformedPoint = transform.TransformPoint(faceMeshVertex);
                transformedVerts.Add(transformedPoint);
                faceCenter += transformedPoint;
            }

            faceCenter /= faceMesh.vertices.Length;

            // Calculate face normal
            faceNormal = transform.TransformPoint(faceMesh.normals[0]);
            // Debug.DrawRay(faceCenter, faceNormal / 10f, Color.cyan);

            // Calculate major axis and standard direction
            float furthestDistance = 0f;

            for (int i = 0; i < transformedVerts.Count; i++)
            {
                for (int j = 0; j < transformedVerts.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Vector3 curVectorCheck = transformedVerts[i] - transformedVerts[j];
                    float distSqr = curVectorCheck.sqrMagnitude;

                    if (distSqr > furthestDistance)
                    {
                        furthestDistance = distSqr;
                        _majorAxisA = transformedVerts[j];
                        _majorAxisB = transformedVerts[i];
                    }
                }
            }

            // Debug.DrawLine(_majorAxisB, _majorAxisA, Color.red, Mathf.Infinity);
        }

        private void RegenerateHintGOs()
        {
            CalculateFaceGeometry();

            _hintsGo = new List<GameObject>();
            String[] hintGoResourcePathStrings = {"Wumpus", "Pit", "Bat"};
            foreach (string hazardName in hintGoResourcePathStrings)
            {
                GameObject hintObject =
                    Instantiate(Resources.Load<GameObject>("Objects/HintTiles/" + hazardName + "Hint"),
                        faceCenter, Quaternion.FromToRotation(Vector3.up, faceNormal));

                hintObject.transform.rotation =
                    Quaternion.LookRotation(_majorAxisA - hintObject.transform.position, faceNormal);

                hintObject.transform.parent = gameObject.transform;

                // Debug.DrawRay(hintObject.transform.position, hintObject.transform.right * 2, Color.yellow,
                // Mathf.Infinity);

                hintObject.SetActive(false);

                _hintsGo.Add(hintObject);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // CalculateFaceGeometry();
            if (!_firstTimeRun)
            {
                string temp = "Face " + _faceNumber + " has states: ";
                foreach (bool i in state)
                {
                    temp += i + ", ";
                }

                //print(temp);
                _firstTimeRun = true;
            }

            if (colonized && showHintOnTile)
            {
                if (!faceDataShowing || faceDataShowing && _hintsGo[0] == null)
                {
                    UpdateHintData();

                    DisplayHintsOnFace();
                }
            }
        }

        // When a face gets clicked
        private void OnMouseDown()
        {
            // TODO: Implement ignoring of most UI elements
            if (IsPointerOverUiElement()) return;
            ActionOnFace();
        }

        // ActionOnFace() will not be fired if there is a UI element over the face
        private static bool IsPointerOverUiElement()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var hit in results)
            {
                switch (hit.gameObject.name)
                {
                    case "TurnNumDisplay":
                        return false;
                }
            }

            return results.Count > 0;
        }


        // When face is clicked with non-left-click input
        // public void OnMouseOver()
        // {
        //     if (!Input.GetMouseButtonDown(1) || IsPointerOverUiElement()) return;
        //     StartCoroutine(WaitUntilRightMouseUp());
        // }
        //
        // private IEnumerator WaitUntilRightMouseUp()
        // {
        //     yield return new WaitUntil(() => Input.GetMouseButtonUp(1)); // Wait until right click is released 
        //     if (colonized)
        //     {
        //         displayFaceData = !displayFaceData;
        //         DisplayHintsOnFace();
        //     }
        //
        //     lastRightClickPos = Input.mousePosition;
        // }

        private void DisplayHintsOnFace()
        {
            print("Showing info");
            // lastHintGiven[0] = true;
            // lastHintGiven[1] = true;
            print("Hints: [" + lastHintGiven[0] + ", " + lastHintGiven[1] + ", " + lastHintGiven[2] + "]");
            List<GameObject> activeGOs = new List<GameObject>();

            RegenerateHintGOs();

            // Show relevant info
            for (int i = 0; i < lastHintGiven.Length; i++)
            {
                _hintsGo[i].SetActive(lastHintGiven[i]);
                if (lastHintGiven[i])
                {
                    activeGOs.Add(_hintsGo[i]);
                }
            }

            // hintObject.transform.position += 1.5f * hintObject.transform.forward;

            float distanceInterval = Vector3.Distance(_majorAxisA, _majorAxisB) / (activeGOs.Count + 1);
            float distanceStep = 1;

            foreach (GameObject activeGo in activeGOs)
            {
                activeGo.transform.position =
                    _majorAxisB + distanceInterval * distanceStep * activeGo.transform.forward;
                distanceStep++;
            }


            faceDataShowing = true;
        }

        // Private functions
        public void ActionOnFace()
        {
            // print("PlayMinigame: " + playMiniGame);
            // print("Picked face: " + _faceNumber + " which has " +
            //       adjacentFaceHandlers[0].GetTileNumber() + ", " +
            //       adjacentFaceHandlers[1].GetTileNumber() + ", " +
            //       adjacentFaceHandlers[2].GetTileNumber() + ", " +
            //       adjacentFaceHandlers[3].GetTileNumber() + " adjacent");
            // Check if actionable
            if (!discovered)
            {
                Debug.Log("This tile is not yet discovered");
                TroopSelection troopSelector = GameObject.Find("Canvas").GetComponent<TroopSelection>();
                if (troopSelector != null)
                {
                    troopSelector.ActivateTroopSelector(_faceNumber, true);
                }

                return;
            }

            if (colonized)
            {
                Debug.Log("This tile is already colonized");
                TroopSelection troopSelector = GameObject.Find("Canvas").GetComponent<TroopSelection>();
                troopSelector.ActivateTroopSelector(_faceNumber);
                troopSelector.ShowOnlyBuildSensorBtn();
                return;
            }

            //if discovered but not yet colonized, play game to try to colonize
            if (_planet.wumpus.location != this)
            {
                switch (_hazardObject)
                {
                    case HazardTypes.None:
                        print("Non-hazardous face, playing mini-game");
                        SetupMiniGame();
                        break;
                    case HazardTypes.Pit:
                        print("YOU'VE FALLEN INTO A WUMPUS NEST (PIT)");
                        SetupMiniGame();
                        break;
                    case HazardTypes.Bat:
                        //TODO: re-implement for split army sit.
                        print("YOU'VE ENCOUNTERED THE SUPER-BATS!");
                        SetupMiniGame();
                        // SetColonized();
                        // SetHazard(HazardTypes.None);
                        break;
                }
            }
            else
            {
                print("YOU'VE ENCOUNTERED THE WUMPUS");
                SetupMiniGame();
            }
        }

        private void SetupMiniGame() // Launch mini-game
        {
            _planet.SetFaceInBattle(_faceNumber);

            UpdateHintData();

            //print("before game");

            GameObject eventSystem = GameObject.Find("Canvas");
            eventSystem.GetComponent<TroopSelection>().ActivateTroopSelector(_faceNumber);
            //print("After game");
        }

        private void UpdateHintData()
        {
            bool[] hintsToGive = new bool[3];

            foreach (FaceHandler adjFace in GetOpenAdjacentFaces())
            {
                HazardTypes haz = adjFace.GetHazardObject();
                if (haz == HazardTypes.Pit)
                {
                    hintsToGive[1] = true;
                }
                else if (haz == HazardTypes.Bat)
                {
                    hintsToGive[2] = true;
                }
                else
                {
                    if (adjFace == _planet.wumpus.location)
                    {
                        hintsToGive[0] = true;
                    }
                }
            }

            _planet.SetHintsToGive(hintsToGive);
            UpdateLocalHintData(hintsToGive);
        }

        public void PlayMiniGame()
        {
            // TODO: put in Max's code to select troops
            List<TroopMeta> deployedTroops = new List<TroopMeta>();
            /*
            int numTroops = Random.Range(2, 5); //TODO make this not dumb

            for (int i = 0; i < deployedTroops.Count; i++)
            {
                print(meta.troops.Count);
                deployedTroops.Add(meta.troops[0]);
                meta.troops.RemoveAt(0);
            }*/

            foreach (var troop in meta.availableTroops)
            {
                if (troop.sendToBattle)
                {
                    deployedTroops.Add(troop);
                }
            }

            if (deployedTroops.Count != 0)
            {
                foreach (var troop in deployedTroops)
                {
                    meta.availableTroops.Remove(troop);
                    troop.sendToBattle = false;
                }

                if (_planet.wumpus.location.Equals(this))
                {
                    if (meta.availableTroops.Count == 0 && meta.exhaustedTroops.Count == 0 && meta.nukes == 0 &&
                        meta.money < 5 && _planet.didSomething)
                    {
                        _planet.curGameStatus = GameStatus.RanOutOfResources;
                    }
                    else
                    {
                        _planet.curGameStatus = GameStatus.LostSentTroopToWumpling;
                    }
                }
                else if (GetHazardObject().Equals(HazardTypes.Bat))
                {
                    SetHazard(HazardTypes.None);

                    //// Pick two faces out
                    // Get Face with border
                    /*List<FaceHandler> borderFaces = new List<FaceHandler>();
                    foreach (GameObject planetFace in _planet.faces)
                    {
                        FaceHandler curFaceHandler = planetFace.GetComponent<FaceHandler>();

                        if (!curFaceHandler.colonized)
                        {
                            continue;
                        }

                        bool useCurFace = true;
                        foreach (FaceHandler openAdjacentFace in curFaceHandler.GetOpenAdjacentFaces())
                        {
                            if (!openAdjacentFace.colonized)
                            {
                                borderFaces.Add(curFaceHandler);
                                useCurFace = false;
                                break;
                            }
                        }

                        if (useCurFace)
                        {
                            borderFaces.Add(curFaceHandler);
                        }
                    }

                    // pick one randomly
                    FaceHandler randomFace;
                    if (borderFaces.Count == 0)
                    {
                        randomFace = this;
                    }
                    else
                    {
                        randomFace = borderFaces[Random.Range(0, borderFaces.Count)];
                    }

                    // Move two faces out
                    for (int i = 0; i < 2; i++)
                    {
                        List<FaceHandler> validFaces = new List<FaceHandler>();

                        foreach (FaceHandler openAdjacentFace in randomFace.GetOpenAdjacentFaces())
                        {
                            if (borderFaces.Contains(openAdjacentFace) || validFaces.Contains(openAdjacentFace) ||
                                openAdjacentFace.Equals(randomFace) || openAdjacentFace.Equals(this))
                            {
                                continue;
                            }

                            if (!openAdjacentFace.colonized)
                            {
                                validFaces.Add(openAdjacentFace);
                            }
                        }

                        if (validFaces.Count == 0)
                        {
                            break;
                        }

                        randomFace = validFaces[Random.Range(0, validFaces.Count)];
                    }

                    // Set heldTroops to deployedTroops
                    randomFace.heldTroops = deployedTroops;
                    foreach (TroopMeta deployedTroop in deployedTroops)
                    {
                        meta.availableTroops.Remove(deployedTroop);
                        deployedTroop.sendToBattle = false;
                    }

                    randomFace.GetComponent<Renderer>().material.color = Color.yellow;
                    */
                    List<FaceHandler> undiscoveredFaces = new List<FaceHandler>();
                    List<FaceHandler> discoveredFaces = new List<FaceHandler>();
                    foreach (GameObject planetFace in _planet.faces)
                    {
                        FaceHandler curFaceHandler = planetFace.GetComponent<FaceHandler>();
                        if (!curFaceHandler.discovered)
                        {
                            undiscoveredFaces.Add(curFaceHandler);
                        }
                        else if (!curFaceHandler.colonized)
                        {
                            discoveredFaces.Add(curFaceHandler);
                        }
                    }

                    FaceHandler finalFace;
                    if (undiscoveredFaces.Count == 0)
                    {
                        finalFace = undiscoveredFaces[Random.Range(0, undiscoveredFaces.Count)];
                    }
                    else
                    {
                        finalFace = discoveredFaces[Random.Range(0, discoveredFaces.Count)];
                    }

                    finalFace.heldTroops = deployedTroops;
                    // TODO: fix with actual asset
                    finalFace.GetComponent<Renderer>().material.color = Color.yellow;

                    SetColonized();
                    GameObject canvasObject = GameObject.Find("Canvas");
                    canvasObject.GetComponent<TurnEnder>().EndTurn();
                    StartCoroutine(canvasObject.GetComponent<TroopSelection>().FlashBatsEncounterAlert());
                }
                else
                {
                    print("Sending " + deployedTroops.Count + " troops to battle");

                    _planet.result = new MiniGameResult(deployedTroops);

                    StartCoroutine(FadeOutAndSwitch());
                }
            }
            else
            {
                print("You didn't select any troops to deploy");
            }
        }

        private IEnumerator FadeOutAndSwitch()
        {
            _planet.GetComponent<MusicController>().FadeOut();

            yield return new WaitUntil(() => Math.Abs(AudioListener.volume) < 0.001);

            if (playMiniGame)
            {
                print("Going to Battle");
                SceneManager.LoadScene(2);
            }
            else
            {
                print("Going to TileBattle");
                SceneManager.LoadScene(1);
            }
        }

        public void NukeTile()
        {
            print("Nukes: " + meta.nukes);
            if (meta.nukes != 0)
            {
                Wumpus.Wumpus wumpus = _planet.wumpus;
                meta.nukes--; // TODO: maybe change this to not directly call from GameMeta?
                SetColonized();
                _planet.didSomething = true;
                if (wumpus.location.Equals(this))
                {
                    print("Hit the Wumpus! You win!");
                    _planet.curGameStatus = GameStatus.Win;
                }
                else
                {
                    print("You didn't hit the Wumpus, but the tile is cleared. Adjacent faces don't make money");

                    bool wumpusAdjacent = false;

                    foreach (FaceHandler adjacentFaceHandler in GetOpenAdjacentFaces())
                    {
                        adjacentFaceHandler.noMoney = true;

                        if (wumpus.location == adjacentFaceHandler)
                        {
                            wumpusAdjacent = true;
                        }
                    }

                    if (wumpusAdjacent)
                    {
                        wumpus.Move(30);
                    }
                }

                meta.EndTurn();
            }
            else
            {
                print("Not enough nukes");
            }

            GameObject troopSelector = GameObject.Find("TroopSelectorUI");
            if (troopSelector != null)
                troopSelector.SetActive(false);
        }

        public void AddSensorOnTile()
        {
            if (meta.sensorTowers > 0 && colonized && !showHintOnTile)
            {
                showHintOnTile = true;
                meta.sensorTowers--;
            }
        }


        // Function to update hint data that is stored exclusively on this face
        private void UpdateLocalHintData(bool[] hints)
        {
            lastHintGiven = hints;
            turnsSinceLastHint = 0;
        }

        // Public Set functions
        public void UpdateFaceColors()
        {
            if (colonized)
            {
                GetComponent<Renderer>().material.color = ColonizedColor[(int) biomeType - 1];
            }
            else if (discovered)
            {
                GetComponent<Renderer>().material.color = DiscoveredColor[(int) biomeType - 1];
            }
            else
            {
                GetComponent<Renderer>().material.color = UndiscoveredColor;
            }
        }

        public void SetHazard(HazardTypes haz)
        {
            _hazardObject = haz;
        }

        public void SetColonized(bool setTerritoryLinesActive = true)
        {
            discovered = true;
            colonized = true;

            SetHazard(HazardTypes.None);

            UpdateFaceColors();

            // Identify adjacent faces as discovered
            foreach (FaceHandler faceHandler in GetOpenAdjacentFaces())
            {
                if (faceHandler.colonized)
                {
                    continue;
                }

                faceHandler.SetDiscovered();
            }

            // print("Colonized Here");
            _planet.ColonizedLineUpdate(setTerritoryLinesActive);
            // _planet.CreateMountains();
        }

        public void SetDiscovered()
        {
            discovered = true;
            UpdateFaceColors();
        }

        // Public Get functions
        public int GetTileNumber()
        {
            return _faceNumber;
        }

        public List<FaceHandler> GetOpenAdjacentFaces()
        {
            List<FaceHandler> openAdjacent = new List<FaceHandler>();
            for (int i = 0; i < adjacentFaceHandlers.Length; i++)
            {
                if (state[i])
                {
                    openAdjacent.Add(adjacentFaceHandlers[i]);
                }
            }

            return openAdjacent;
        }

        public HazardTypes GetHazardObject()
        {
            return _hazardObject;
        }

        public int CountCs()
        {
            int n = 4;
            for (int i = 0; i < 4; i++)
            {
                if (!state[i])
                {
                    n--;
                }
            }

            return n;
        }

        public bool HadWumpus()
        {
            return _planet.wumpus.location == this;
        }
    }
}