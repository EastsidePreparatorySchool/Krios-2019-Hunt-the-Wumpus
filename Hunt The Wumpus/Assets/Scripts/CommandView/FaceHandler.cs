using Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public List<TroopMeta> InGameTroops;
        public int MoneyCollected;
        public bool DidWin;

        public MiniGameResult(List<TroopMeta> troopsToSend)
        {
            InGameTroops = troopsToSend;
        }
    }

    public enum BiomeType
    {
        None,
        Plains,
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
        public Material faceMaterial;
        private Color _targetColor;
        private Color _ogColor;
        private Color _aColor;
        private Color _bColor;
        private float _lastColorSwitch;
        private bool _pulsingColor;

        private GameObject _canvas;
        private GameObject _noNukeText;

        public Vector3 lastRightClickPos;
        public bool noMoney;

        public List<MeshVertex> meshVertices = new List<MeshVertex>();

        // Game-relevant stats in this script are sent to a centralized location
        // private GameMeta _inGameStat;
        public bool[] lastHintGiven;
        public int turnsSinceLastHint;
        public bool showHintOnTile;

        private HazardTypes _hazardObject = HazardTypes.None;
        private int _faceNumber;

        private bool _firstTimeRun;
        private bool playMiniGame = true; // you can turn this off if you just want to mess with the map

        private GameMeta _meta;
        public List<TroopMeta> heldTroops = new List<TroopMeta>();
        private GameObject _batDestinationIndicator;

        public GameObject faceDataHolder; // assigned in this class's methods, used in the UpdateGui script
        public MeshFilter faceMeshFilter;
        public bool faceDataShowing;
        public Vector3 faceCenter;
        public Vector3 faceNormal;
        private List<GameObject> _hintsGo; // 0 = Wumpus, 1 = Pit, 2 = Bats
        private GameObject _sensorTowerIcon;
        private Vector3 _majorAxisA;
        private Vector3 _majorAxisB;

        private void Awake()
        {
            // Initialize variables
            _planet = transform.parent.gameObject.GetComponent<Planet>();
            _faceNumber = Convert.ToInt32(gameObject.name) - 1;
            // _inGameStat = _planet.GetComponent<GameMeta>();
            lastHintGiven = _planet.GetHintsToGive();
            state = new[] {true, true, true, true};
            biomeType = BiomeType.None;
            _meta = _planet.GetComponent<GameMeta>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            faceMaterial = GetComponent<Renderer>().material;
            faceMeshFilter = GetComponent<MeshFilter>();
            CalculateFaceGeometry();
            _canvas = GameObject.Find("Canvas");
            _noNukeText = _canvas.transform.Find("OutOfNukesText").gameObject;

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
            // Debug.DrawRay(faceCenter, faceNormal / 10f, Color.cyan, Mathf.Infinity);

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


            if (_hintsGo != null)
            {
                foreach (GameObject o in _hintsGo)
                {
                    Destroy(o);
                }
            }

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

            if (_sensorTowerIcon == null)
            {
                _sensorTowerIcon = Instantiate(Resources.Load<GameObject>("Objects/HintTiles/SensorTower"),
                    faceCenter, Quaternion.FromToRotation(Vector3.up, faceNormal));

                _sensorTowerIcon.transform.rotation =
                    Quaternion.LookRotation(_majorAxisA - _sensorTowerIcon.transform.position, faceNormal);

                _sensorTowerIcon.transform.parent = gameObject.transform;

                // TODO: position the tower better
                _sensorTowerIcon.transform.position += 1.5f * _sensorTowerIcon.transform.right;

                _sensorTowerIcon.SetActive(false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // CalculateFaceGeometry();
            // if (!_firstTimeRun)
            // {
            //     string temp = "Face " + _faceNumber + " has states: ";
            //     foreach (bool i in state)
            //     {
            //         temp += i + ", ";
            //     }
            //
            //     //print(temp);
            //     _firstTimeRun = true;
            // }


            if (colonized && showHintOnTile)
            {
                UpdateHintData();
                DisplayHintsOnFace();
            }

            if (heldTroops.Any() && _batDestinationIndicator == null)
            {
                GenerateBatNet();
            }
            else if (!heldTroops.Any() && _batDestinationIndicator != null)
            {
                Destroy(_batDestinationIndicator);
            }

            if (_planet.selectedFace == _faceNumber)
            {
                if (!_pulsingColor)
                {
                    _ogColor = faceMaterial.color;
                    Color.RGBToHSV(_ogColor, out float ogH, out float ogS, out float ogV);
                    float delta = 0.5f;
                    if (1f - (ogV + delta) < 0)
                    {
                        delta = -delta;
                    }

                    _targetColor = Color.HSVToRGB(ogH, ogS, ogV + delta);

                    _aColor = _ogColor;
                    _bColor = _targetColor;
                    _lastColorSwitch = Time.time;

                    _pulsingColor = true;
                }

                float lerpAmount = (Time.time - _lastColorSwitch) / 0.75f;
                faceMaterial.color = Color.Lerp(_aColor, _bColor, lerpAmount);

                if (lerpAmount >= 1)
                {
                    _lastColorSwitch = Time.time;
                    Color temp = _aColor;
                    _aColor = _bColor;
                    _bColor = temp;
                }
            }
            else
            {
                if (_pulsingColor)
                {
                    faceMaterial.color = _ogColor;
                    _pulsingColor = false;
                }
            }

            if (Input.anyKeyDown && _noNukeText != null)
                _noNukeText.SetActive(false);
        }

        public void GenerateBatNet()
        {
            CalculateFaceGeometry();
            _batDestinationIndicator = Instantiate(Resources.Load<GameObject>("Objects/BatNet"), faceCenter,
                Quaternion.FromToRotation(Vector3.up, faceNormal));
            _batDestinationIndicator.transform.rotation =
                Quaternion.LookRotation(_majorAxisA - _batDestinationIndicator.transform.position, faceNormal);
            _batDestinationIndicator.transform.parent = gameObject.transform;
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
            PointerEventData eventData = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
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
            // print("Showing info");
            // lastHintGiven[0] = true;
            // lastHintGiven[1] = true;
            // print("Hints: [" + lastHintGiven[0] + ", " + lastHintGiven[1] + ", " + lastHintGiven[2] + "]");
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

            _sensorTowerIcon.SetActive(true);

            // hintObject.transform.position += 1.5f * hintObject.transform.forward;

            float distanceInterval = Vector3.Distance(_majorAxisA, _majorAxisB) / (activeGOs.Count + 1);
            float distanceStep = 1;

            foreach (GameObject activeGo in activeGOs)
            {
                activeGo.transform.position =
                    _majorAxisB + distanceInterval * distanceStep * activeGo.transform.forward;
                distanceStep++;
            }


            // faceDataShowing = true;
        }

        // Private functions
        private void ActionOnFace()
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
                CloseTroopSelector();

                _planet.selectedFace = -1;

                return;
            }

            _planet.selectedFace = _faceNumber;
            // StartCoroutine(PulseFace());

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

        private IEnumerator PulseFace()
        {
            Renderer faceRenderer = GetComponent<Renderer>();
            Material faceRendererMaterial = faceRenderer.material;
            float delta = 5f / 360f;
            int fadeDir = 1;
            Color ogColor = faceRendererMaterial.color;
            Color.RGBToHSV(ogColor, out float ogH, out float ogS, out float ogV);

            while (_planet.selectedFace == _faceNumber)
            {
                float change = delta * 0.1f * fadeDir;
                print(change);
                faceRendererMaterial.color = Color.HSVToRGB(ogH, ogS, ogV + change);

                Color.RGBToHSV(faceRendererMaterial.color, out _, out _, out float curV);
                print(curV + " -> " + ogV);
                if (curV > ogV + delta)
                {
                    fadeDir = -1;
                }
                else if (curV < ogV)
                {
                    fadeDir = 1;
                }

                yield return new WaitForSeconds(0.1f);
            }

            faceRendererMaterial.color = ogColor;
        }

        private void UpdateHintData()
        {
            bool[] hintsToGive = new bool[3];

            foreach (FaceHandler adjFace in adjacentFaceHandlers)
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

            // _planet.SetHintsToGive(hintsToGive);
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

            foreach (var troop in _meta.availableTroops)
            {
                if (troop.SendToBattle)
                {
                    deployedTroops.Add(troop);
                }
            }

            if (deployedTroops.Count != 0)
            {
                TroopSelection troopSelector = GameObject.Find("Canvas").GetComponent<TroopSelection>();
                troopSelector.ActivateTroopSelector(_faceNumber, true);

                foreach (var troop in deployedTroops)
                {
                    _meta.availableTroops.Remove(troop);
                    troop.SendToBattle = false;
                }

                if (_planet.wumpus.location.Equals(this))
                {
                    if (_meta.availableTroops.Count == 0 && _meta.exhaustedTroops.Count == 0 && _meta.nukes == 0 &&
                        _meta.money < 5 && !_planet.didSomething)
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

                    SetColonized();

                    List<FaceHandler> undiscoveredFaces = new List<FaceHandler>();
                    List<FaceHandler> discoveredFaces = new List<FaceHandler>();
                    foreach (GameObject planetFace in _planet.faces)
                    {
                        FaceHandler curFaceHandler = planetFace.GetComponent<FaceHandler>();
                        if (!curFaceHandler.discovered)
                        {
                            print("Added undiscovered");
                            undiscoveredFaces.Add(curFaceHandler);
                        }
                        else if (!curFaceHandler.colonized)
                        {
                            print("added uncolonized");
                            discoveredFaces.Add(curFaceHandler);
                        }
                    }

                    FaceHandler finalFace = undiscoveredFaces.Any()
                        ? undiscoveredFaces[Random.Range(0, undiscoveredFaces.Count)]
                        : discoveredFaces[Random.Range(0, discoveredFaces.Count)];

                    finalFace.heldTroops = deployedTroops;

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
            StartCoroutine(ShowLoadingCover());

            _planet.GetComponent<MusicController>().FadeOut();
            yield return new WaitUntil(() => Math.Abs(AudioListener.volume) < 0.001);

            // print("Stopping ambient");
            // CameraHandler cameraHandler = GameObject.Find("Main Camera").GetComponent<CameraHandler>();
            // cameraHandler.ambientMusic.Stop();
            // print(cameraHandler.ambientMusic.isPlaying);

            if (playMiniGame)
            {
                print("Going to Battle");
                CloseTroopSelector();
                SceneManager.LoadScene(1);
            }
            else
            {
                print("Going to TileBattle");
                SceneManager.LoadScene(2);
            }
        }

        private IEnumerator ShowLoadingCover()
        {
            CanvasGroup loadingCover = GameObject.Find("LoadingCover").GetComponent<CanvasGroup>();
            float lerpStart = Time.time;
            while (true)
            {
                var progress = Time.time - lerpStart;
                loadingCover.alpha = Mathf.Lerp(0, 1, progress / 0.2f);
                if (0.2f < progress)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public void NukeTile()
        {
            print("Nukes: " + _meta.nukes);
            if (_meta.nukes != 0)
            {
                StartCoroutine(NukePs());
            }
            else
            {
                _noNukeText.SetActive(true);
                print("Not enough nukes");
            }
        }

        private IEnumerator NukePs()
        {
            CalculateFaceGeometry();

            GameObject nukeGo = Instantiate(Resources.Load<GameObject>("Objects/Nuke"), faceCenter,
                Quaternion.FromToRotation(Vector3.up, faceNormal));
            nukeGo.transform.rotation =
                Quaternion.LookRotation(_majorAxisA - nukeGo.transform.position, faceNormal);
            nukeGo.transform.parent = gameObject.transform;

            nukeGo.transform.position += nukeGo.transform.up * 20f;

            while (Vector3.SqrMagnitude(nukeGo.transform.localPosition) > 0.0001f)
            {
                nukeGo.transform.position -= nukeGo.transform.up * (Time.deltaTime * 30f);
                yield return new WaitForEndOfFrame();
            }

            Destroy(nukeGo);
            NukeLogic();
        }

        private void NukeLogic()
        {
            Wumpus.Wumpus wumpus = _planet.wumpus;
            _meta.nukes--; // TODO: maybe change this to not directly call from GameMeta?
            SetColonized();
            heldTroops.Clear();
            //_planet.didSomething = true;
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

            _meta.EndTurn();
            CloseTroopSelector();
        }

        public void AddSensorOnTile()
        {
            if (_meta.sensorTowers > 0 && colonized && !showHintOnTile)
            {
                showHintOnTile = true;
                _meta.sensorTowers--;

                CloseTroopSelector();
            }
        }

        private void CloseTroopSelector()
        {
            TroopSelection troopSelector = GameObject.Find("Canvas").GetComponent<TroopSelection>();
            if (troopSelector != null)
            {
                print("Closing the selector");
                troopSelector.ActivateTroopSelector(_faceNumber, true);
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
                faceMaterial.color = ColonizedColor[(int) biomeType - 1];
            else if (discovered)
                faceMaterial.color = DiscoveredColor[(int) biomeType - 1];
            else
                faceMaterial.color = UndiscoveredColor;
            // if (heldTroops.Count > 0)
            //     faceMaterial.color = Color.yellow;

            _ogColor = faceMaterial.color;
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

            CalculateFaceGeometry();

            GameObject colonizedPsGo = Instantiate(Resources.Load<GameObject>("Objects/ColonizedPs"), faceCenter,
                Quaternion.FromToRotation(Vector3.up, faceNormal));
            colonizedPsGo.transform.rotation =
                Quaternion.LookRotation(_majorAxisA - colonizedPsGo.transform.position, faceNormal);
            colonizedPsGo.transform.parent = gameObject.transform;

            ParticleSystem colonizedPs = colonizedPsGo.GetComponentInChildren<ParticleSystem>();
            colonizedPs.Play();
            Destroy(colonizedPs.gameObject, 3.5f);
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