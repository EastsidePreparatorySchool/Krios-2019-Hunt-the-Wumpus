using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Gui;
using UnityEditorInternal.Profiling;
//using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using Random = System.Random;
using Random = UnityEngine.Random;

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

    // Data object and event handler for haves on CommandView planet
    public class FaceHandler : MonoBehaviour
    {
        //Public variables
        public static readonly Color UndiscoveredColor = Color.gray;
        public static readonly Color DiscoveredColor = Color.white;
        public static readonly Color ColonizedColor = Color.yellow;

        //individual variables
        private Planet _planet; // store reference back to planet
        public bool[] state;

        // Face Properties
        public GameObject[] adjacentFaces;
        public bool discovered;
        public bool colonized;
        public int lastSeenTurnsAgo;

        public Vector3 lastRightClickPos;
        public bool noMoney;

        public List<MeshVertex> meshVertices = new List<MeshVertex>();

        // Game-relevant stats in this script are sent to a centralized location
        private GameMeta _ingameStat;
        public bool[] lastHintGiven;
        public int turnsSinceLastHint;

        private HazardTypes _hazardObject = HazardTypes.None;
        private int _faceNumber;

        private bool _firstTimeRun;
        private bool playMiniGame = true; // you can turn this off if you just want to mess with the map

        private GameMeta meta;

        public GameObject faceDataHolder; // assigned in this class's methods, used in the UpdateGui script
        public bool displayFaceData; // is UI screen of this face's data displayed
        public bool faceDataShowing;
        public bool lastPressed;
        public Vector3 faceCenter;
        public Vector3 faceNormal;
        private List<GameObject> _hintsGo; // 0 = Wumpus, 1 = Pit, 2 = Bats
        private Vector3 _majorAxisA = new Vector3();
        private Vector3 _majorAxisB = new Vector3();

        private void Awake()
        {
            // Initialize variables
            _planet = transform.parent.gameObject.GetComponent<Planet>();
            _faceNumber = Convert.ToInt32(gameObject.name) - 1;
            _ingameStat = _planet.GetComponent<GameMeta>();
            lastHintGiven = _planet.GetHintsToGive();
            state = new[] {true, true, true, true};
            meta = _planet.GetComponent<GameMeta>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            MeshFilter faceMeshFilter = GetComponent<MeshFilter>();
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
            Debug.DrawRay(faceCenter, faceNormal / 10f, Color.cyan);

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

            Debug.DrawLine(_majorAxisB, _majorAxisA, Color.red, Mathf.Infinity);


            _hintsGo = new List<GameObject>();
            String[] hintGoResourcePathStrings = {"Wumpus", "Pit", "Bat"};
            for (int i = 0; i < hintGoResourcePathStrings.Length; i++)
            {
                GameObject hintObject =
                    Instantiate(Resources.Load<GameObject>("Objects/" + hintGoResourcePathStrings[i] + "Hint"),
                        faceCenter, Quaternion.FromToRotation(Vector3.up, faceNormal));

                hintObject.transform.rotation =
                    Quaternion.LookRotation(_majorAxisA - hintObject.transform.position, faceNormal);

                Debug.DrawRay(hintObject.transform.position, hintObject.transform.right * 2, Color.yellow,
                    Mathf.Infinity);

                hintObject.SetActive(false);

                _hintsGo.Add(hintObject);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_firstTimeRun)
            {
                string temp = "Face " + _faceNumber + " has states: ";
                foreach (bool i in state)
                {
                    temp += i + ", ";
                }

                print(temp);
                _firstTimeRun = true;
            }

            if (Input.GetButton("ShowAllHints") && !lastPressed)
            {
                lastPressed = true;
                bool show = false;
                foreach (GameObject adjacentFace in adjacentFaces)
                {
                    if (!adjacentFace.GetComponent<FaceHandler>().colonized)
                    {
                        show = true;
                        break;
                    }
                }

                if (show)
                {
                    displayFaceData = !displayFaceData;
                    DisplayHintsOnFace();
                }
            }
            else
            {
                if (!Input.GetButton("ShowAllHints"))
                {
                    lastPressed = false;
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
            return results.Count > 0;
        }


        // When face is clicked with non-left-click input
        public void OnMouseOver()
        {
            if (!Input.GetMouseButtonDown(1) || IsPointerOverUiElement()) return;
            StartCoroutine(WaitUntilRightMouseUp());
        }

        private IEnumerator WaitUntilRightMouseUp()
        {
            yield return new WaitUntil(() => Input.GetMouseButtonUp(1)); // Wait until right click is released 
            if (colonized)
            {
                displayFaceData = !displayFaceData;
                DisplayHintsOnFace();
            }

            lastRightClickPos = Input.mousePosition;
        }

        private void DisplayHintsOnFace()
        {
            if (displayFaceData && !faceDataShowing)
            {
                print("Showing info");
                // lastHintGiven[0] = true;
                // lastHintGiven[1] = true;
                print("Hints: [" + lastHintGiven[0] + ", " + lastHintGiven[1] + ", " + lastHintGiven[2] + "]");
                List<GameObject> activeGOs = new List<GameObject>();
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
            else
            {
                print("Hiding info");
                foreach (GameObject o in _hintsGo)
                {
                    o.SetActive(false);
                }

                faceDataShowing = false;
            }
        }

        // Private functions
        public void ActionOnFace(bool arrivedViaBat = false)
        {
            print("PlayMinigame: " + playMiniGame);
            print("Picked face: " + _faceNumber + " which has " +
                  adjacentFaces[0].GetComponent<FaceHandler>().GetTileNumber() + ", " +
                  adjacentFaces[1].GetComponent<FaceHandler>().GetTileNumber() + ", " +
                  adjacentFaces[2].GetComponent<FaceHandler>().GetTileNumber() + ", " +
                  adjacentFaces[3].GetComponent<FaceHandler>().GetTileNumber() + " adjacent");
            // Check if actionable
            if (!discovered)
            {
                Debug.Log("This tile is not yet discovered");
                return;
            }

            if (colonized)
            {
                Debug.Log("This tile is already colonized");
                return;
            }

            if (!arrivedViaBat)
            {
                _ingameStat.turnsElapsed++;
                foreach (GameObject face in _planet.faces)
                {
                    face.GetComponent<FaceHandler>()
                        .turnsSinceLastHint++; // TODO: Consider optimizing by storing FaceHandler[]
                }
            }

            //if discovered but not yet colonized, play game to try to colonize
            if (_planet.GetComponent<Wumpus.Wumpus>().location != _faceNumber)
            {
                switch (_hazardObject)
                {
                    case HazardTypes.None:
                        print("Non-hazardous face, playing mini-game");
                        SetupMiniGame();
                        break;
                    case HazardTypes.Pit:
                        print("YOU'VE FALLEN INTO A WUMPUS NEST (PIT)");
                        // TODO: Run pit mini game
                        break;
                    case HazardTypes.Bat:
                        //TODO: re-implement for split army sit.
                        print("YOU'VE ENCOUNTERED THE SUPER-BATS!");
                        Random random = new Random();

                        // Generate new location for player
                        int newPlayerLoc;
                        do
                        {
                            newPlayerLoc = Random.Range(0, 29);
                        } while (newPlayerLoc == _faceNumber);

                        // Generate new location for bat
                        int newBatLoc;
                        do
                        {
                            newBatLoc = Random.Range(0, 29);
                        } while (newBatLoc == newPlayerLoc);

                        _planet.faces[newBatLoc].GetComponent<FaceHandler>().SetHazard(HazardTypes.Bat);

                        print("Player will now be located at " + newPlayerLoc);

                        // Do action on the face (like colonize)
                        FaceHandler newPlayerFaceHandler = _planet.faces[newPlayerLoc].GetComponent<FaceHandler>();
                        if (!newPlayerFaceHandler.colonized)
                        {
                            newPlayerFaceHandler.SetDiscovered();
                            newPlayerFaceHandler.ActionOnFace(true);
                        }

                        // Clean up current face
                        SetHazard(HazardTypes.None);
                        SetColonized();
                        print("Super bats from " + _faceNumber + " are now at " + newBatLoc);
                        break;
                }
            }
            else
            {
                print("YOU'VE ENCOUNTERED THE WUMPUS");
                // TODO: handle wumpus encounter
            }
        }

        private void SetupMiniGame() // Launch mini-game
        {
            _planet.SetFaceInBattle(_faceNumber);

            bool[] hintsToGive = new bool[3];

            foreach (GameObject face in GetOpenAdjacentFaces())
            {
                FaceHandler adjFace = face.GetComponent<FaceHandler>();
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
                    if (adjFace.GetTileNumber() == _planet.GetComponent<Wumpus.Wumpus>().location)
                    {
                        hintsToGive[0] = true;
                    }
                }
            }


            _planet.SetHintsToGive(hintsToGive);
            UpdateLocalHintData(hintsToGive);

            //print("before game");

            GameObject eventSystem = GameObject.Find("Canvas");
            eventSystem.GetComponent<TroopSelection>().ActivateTroopSelector(_faceNumber);
            //print("After game");
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

            foreach (var troop in meta.troops)
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
                    meta.troops.Remove(troop);
                    troop.sendToBattle = false;
                }

                print("Sending " + deployedTroops.Count + " troops to battle");

                _planet.result = new MiniGameResult(deployedTroops);

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
            else
            {
                print("You didn't select any troops to deploy");
            }
        }

        public void NukeTile()
        {
            print("Nukes: " + meta.nukes);
            if (meta.nukes != 0)
            {
                Wumpus.Wumpus wumpus = _planet.GetComponent<Wumpus.Wumpus>();
                meta.nukes--; // TODO: maybe change this to not directly call from GameMeta?
                SetColonized();
                if (wumpus.location == _faceNumber)
                {
                    print("Hit the Wumpus! You win!");
                    // TODO: end the game
                }
                else
                {
                    print("You didn't hit the Wumpus, but the tile is cleared. Adjacent faces don't make money");

                    bool wumpusAdjacent = false;

                    foreach (GameObject adjacentFace in adjacentFaces)
                    {
                        FaceHandler adjacentFaceHandler = adjacentFace.GetComponent<FaceHandler>();

                        adjacentFaceHandler.noMoney = true;

                        if (wumpus.location == adjacentFaceHandler._faceNumber)
                        {
                            wumpusAdjacent = true;
                        }
                    }

                    if (wumpusAdjacent)
                    {
                        wumpus.Move();
                    }
                }
            }
            else
            {
                print("Not enough nukes");
            }
        }


        // Function to update hint data that is stored exclusively on this face
        private void UpdateLocalHintData(bool[] hints)
        {
            lastHintGiven = hints;
            turnsSinceLastHint = 0;
        }

        // Public Set functions
        public void SetHazard(HazardTypes haz)
        {
            _hazardObject = haz;
        }

        public void SetColonized()
        {
            discovered = true;
            colonized = true;

            GetComponent<Renderer>().material.color = ColonizedColor;

            // Identify adjacent faces as discovered
            foreach (GameObject face in GetOpenAdjacentFaces())
            {
                FaceHandler faceHandler = face.GetComponent<FaceHandler>();
                if (faceHandler.colonized)
                {
                    continue;
                }

                faceHandler.SetDiscovered();
            }

            print("Colonized Here");
            _planet.ColonizedLineUpdate();
        }

        public void SetDiscovered()
        {
            discovered = true;
            GetComponent<Renderer>().material.color = DiscoveredColor;
        }

        // Public Get functions
        public int GetTileNumber()
        {
            return _faceNumber;
        }

        public List<GameObject> GetOpenAdjacentFaces()
        {
            List<GameObject> openAdjacent = new List<GameObject>();
            for (int i = 0; i < adjacentFaces.Length; i++)
            {
                if (state[i])
                {
                    openAdjacent.Add(adjacentFaces[i]);
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
    }
}