using System.Collections.Generic;
using System.Linq;
using Gui;
using SaveLoad;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum HazardTypes
{
    None,
    Pit,
    Bat
}

public enum GameStatus
{
    InPlay,
    Win,
    RanOutOfResources,
    LostSentTroopToWumpus,
    LostToWumpus
}

// public struct Mountain
// {
//     public GameObject mountain;
//     public MeshVertex MeshVertex1;
//     public MeshVertex MeshVertex2;
//     public bool Discovered;
//     public bool Colonized;
//
//     public Mountain(MeshVertex mv1, MeshVertex mv2)
//     {
//         MeshVertex1 = mv1;
//         MeshVertex2 = mv2;
//     }
// }

namespace CommandView
{
    // Planet will be the Global (constant) data holder
    public class Planet : MonoBehaviour
    {
        // User Prefs
        //public bool confirmTurn;

        // General
        public bool backFromMiniGame;
        private bool _startGame;
        public bool isFadingMusic;
        public bool bloom = true;
        public bool ambientOcclusion = true;

        public float volume = 0.5f;

        public bool readyToPause;
        public bool readyToPlay = true;

        public int maxUpgrades;

        // Hold an instance of the Planet
        public static Planet Instance;

        // Wumpus
        public Wumpus.Wumpus wumpus;

        // Hold ref to all of the faces
        public GameObject[] faces;
        public FaceHandler[] faceHandlers;

        public EventSystem lineEventSystem;

        // Planet properties and class-wide variables
        //private readonly Random _random = new Random();
        private bool _isHidden;

        public GameStatus curGameStatus = GameStatus.InPlay;

        // UI global variables
        private List<MeshVertex> _vertices = new List<MeshVertex>();
        private GameObject _colonizedLine;
        private Material _altLineMaterial;
        public const float TerritoryLineWidth = 0.15f;
        private List<GameObject> _lines = new List<GameObject>();

        public bool borderAroundTerritory;

        // public List<Mountain> mountains = new List<Mountain>();
        public int lastDisplayedTurn;

        // Mini-game global variables
        private int _faceInBattle = -1; // which face is being played on (-1=none)
        private bool[] _hintsToGive = new bool[3]; //index 0=Wumpus, 1=pit, 2=bat
        public int[] wumplingWaves;
        public int soldiers;

        //public int cursorIndex;
        //public int waypointIndex = 4;

        // Some of the save/load variables
        private bool[,] States = new bool[4, 30];
        private int[] BiomeNum = new int[30];
        private bool[] IsColonized = new bool[30];
        private int[] HazardType = new int[30];
        private bool[] ShowHint = new bool[30];
        private bool[] NoMoney = new bool[30];
        private int[] _troopType;
        private string[] _troopName;
        private bool[] _isExausted;
        private bool[] _isHeld;
        private int[] _heldLoc;
        private int _totalTroops;

        public GameMeta meta;
        public MiniGameResult result;

        public int selectedFace = -1;

        public bool didSomething;

        // TODO: remove if unnecessary
        public Planet()
        {
            readyToPause = false;
        }

        private void Awake()
        {
            volume = .5f;

            // make sure there is only one instance of the Planet and make it persistent
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            foreach (var line in _lines)
            {
                line.SetActive(true);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //meta.setupForDebug();
            // Generate planet map
            MapMaker();

            // Populate UI variables
            _colonizedLine = GameObject.Find("ColonizedLine");
            _colonizedLine.SetActive(false);
            _altLineMaterial = Resources.Load<Material>("Materials/DiscoveredLineEmission");
            CreateMeshVertices();

            faceHandlers = new FaceHandler[faces.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                faceHandlers[i] = faces[i].GetComponent<FaceHandler>();
            }

            // Populate biomes
            foreach (FaceHandler faceHandler in faceHandlers)
            {
                if (faceHandler.biomeType != BiomeType.None)
                {
                    continue;
                }

                int biomeNum = Random.Range(0, 3);
                faceHandler.biomeType = biomeNum == 0 ? BiomeType.Plains :
                    biomeNum == 1 ? BiomeType.Desert : BiomeType.Jungle;

                //print(biomeNum);

                foreach (FaceHandler adjacentHandler in faceHandler.GetOpenAdjacentFaces())
                {
                    if (adjacentHandler.biomeType == BiomeType.None)
                    {
                        adjacentHandler.biomeType = faceHandler.biomeType;
                    }
                }
            }

            // Reset planet to undiscovered
            foreach (FaceHandler faceHandler in faceHandlers)
            {
                faceHandler.UpdateFaceColors();
            }

            // Set player start location to a random face, make it colonized (safe)
            // _playerLoc = Mathf.RoundToInt(_random.Next(0, 29));
            int splashSpot = Random.Range(0, 30);
            faceHandlers[splashSpot].SetColonized();
            /*transform.Rotate(Vector3.up, 
                Vector3.Angle(Vector3.forward, 
                    new Vector3(faceHandlers[splashSpot].faceNormal.x, 0, faceHandlers[splashSpot].faceNormal.z)), 
                Space.World);*/
            print("Player starting at " + splashSpot);

            // TODO: either use the code below or something else to init wumpus with correct location
            // Initialize Wumpus location as to not conflict with player initial location
            // int wumpLoc;
            //
            // List<int> usedLocations = new List<int> {splashSpot};
            // usedLocations.AddRange(faces[splashSpot].GetComponent<FaceHandler>().GetOpenAdjacentFaces()
            //     .Select(openAdjacentFace => openAdjacentFace.GetComponent<FaceHandler>().GetTileNumber()));
            //
            // do
            // {
            //     wumpLoc = Mathf.RoundToInt(_random.Next(0, 29));
            // } while (usedLocations.Contains(wumpLoc));
            //
            // wumpus.location = wumpLoc;


            //print("Planet Started");

            // Initialize hazards
            MakeHazardObjects(splashSpot);

            // CreateMountains();
        }

        // Update is called once per frame
        void Update()
        {
            // Hide/show the planet depending on if you're playing the mini-game
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.buildIndex != 0 ^ _isHidden) // apologies for such bullshit, TODO: Remove comment🤡
            {
                ShowPlanet(_isHidden);
            }
        }

        private void MakeHazardObjects(int safeSpot)
        {
            // Keep track of faces that have already been assigned a Hazard
            List<int> usedFaces = new List<int> {safeSpot};

            // Making sure no hazards get added to the initial face and it's adjacent faces
            foreach (FaceHandler face in faceHandlers[safeSpot].GetOpenAdjacentFaces())
            {
                int adjFace = face.GetTileNumber();
                usedFaces.Add(adjFace);
            }

            // Creating the 4 other Hazard faces
            for (int i = 0; i < 4; i++)
            {
                // Pick a random face and make sure it hasn't been used yet
                int randFace;
                do
                {
                    randFace = Random.Range(0, 30);
                } while (usedFaces.Contains(randFace));

                usedFaces.Add(randFace);

                // Assign type Bat or Pit to the face
                HazardTypes type = i % 2 == 0 ? HazardTypes.Pit : HazardTypes.Bat;

                faces[randFace].GetComponent<FaceHandler>().SetHazard(type);

                print("Face: " + randFace + " has Hazard of type: " + type);
            }
        }

        // Private Functions
        private void ShowPlanet(bool show = true)
        {
            _isHidden = !show;
            foreach (GameObject face in faces)
            {
                face.SetActive(show);
            }

            foreach (Transform lines in lineEventSystem.transform)
            {
                lines.gameObject.SetActive(show);
            }
        }

        private static void Swap(List<int> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private /*static*/ void Shuffle(List<int> list)
        {
            for (var i = list.Count; i > 0; i--)
                Swap(list, 0, Random.Range(0, i));
        }


        private void Disconnect(int o, int t)
        {
            //uses array element #, not room ID number (i.e. room 0, not 1)
            int tl = faces[o].GetComponent<FaceHandler>().adjacentFaces.ToList().IndexOf(faces[t]);
            if (tl > -1)
            {
                faces[o].GetComponent<FaceHandler>().state[tl] = false;
            }
            else
            {
                print("failure to find room " + t + " from room " + o);
            }

            // while (faces[o].GetComponent<FaceHandler>().adjacentFaces[tl].GetComponent<FaceHandler>().GetTileNumber() !=
            //        (t + 1))
            // {
            //     tl++;
            //     if (tl > 3)
            //     {
            //         
            //         break;
            //     }
            // }


            //repeat for opposite side of the connection
            int ol = faces[t].GetComponent<FaceHandler>().adjacentFaces.ToList().IndexOf(faces[o]);
            // while (faces[t].GetComponent<FaceHandler>().adjacentFaces[ol].GetComponent<FaceHandler>().GetTileNumber() !=
            //        (o + 1))
            // {
            //     ol++;
            //     if (ol > 3)
            //     {
            //         
            //         break;
            //     }
            // }

            if (ol > -1)
            {
                faces[t].GetComponent<FaceHandler>().state[ol] = false;
            }
            else
            {
                print("failure to find room " + t + " from room " + o);
            }
        }

        private void Connect(int o, int t)
        {
            //uses array element #, not room ID number (i.e. room 0, not 1)
            int tl = faces[o].GetComponent<FaceHandler>().adjacentFaces.ToList().IndexOf(faces[t]);
            // while (faces[o].GetComponent<FaceHandler>().adjacentFaces[tl].GetComponent<FaceHandler>().GetTileNumber() !=
            //        (t + 1))
            // {
            //     tl++;
            //     if (tl > 3)
            //     {
            //         
            //         break;
            //     }
            // }

            if (tl > -1)
            {
                faces[o].GetComponent<FaceHandler>().state[tl] = true;
            }
            else
            {
                print("failure to find room " + t + " from room " + o);
            }

            //repeat for opposite side of the connection
            int ol = faces[t].GetComponent<FaceHandler>().adjacentFaces.ToList().IndexOf(faces[o]);
            // while (faces[t].GetComponent<FaceHandler>().adjacentFaces[ol].GetComponent<FaceHandler>().GetTileNumber() !=
            //        (o + 1))
            // {
            //     ol++;
            //     if (ol > 3)
            //     {
            //         
            //         break;
            //     }
            // }

            if (ol > -1)
            {
                faces[t].GetComponent<FaceHandler>().state[ol] = true;
            }
            else
            {
                print("failure to find room " + t + " from room " + o);
            }
        }

        private bool CanFind()
        {
            int[] found = new int[30]; //syntax?
            int countOf0 = 0; //number of 0s in array--use this later.
            found = Explore(1, found);
            int i = 0;
            int j;
            while (true)
            {
                // print("hi!");
                j = i;
                while (found[i] != 1)
                {
                    i = (i + 1) % 30;
                    if (i == j)
                    {
                        //went through whole array, no new connections
                        //check for all 0s? if all 0s return false
                        for (int n = 0; n < 30; n++)
                        {
                            if (found[n] == 0)
                            {
                                countOf0++;
                            }
                        }

                        if (countOf0 > 0)
                        {
                            return false; // we explored all connections to o but, but still didn't reach some rooms
                        }

                        return true;
                    }
                } //i found a 1 at [i]

                found = Explore(i + 1, found); //update found array
            }
        }

        private int[] Explore(int t, int[] ar)
        {
            //this is part of the canFind() situation
            if (ar[t - 1] == 0)
            {
                ar[t - 1] = 2;
            }
            else
            {
                ar[t - 1]++;
            }

            for (int i = 0; i < 4; i++)
            {
                //for every [open] connection room has
                //if(r[t-1].state[i] == true){
                if (faces[t - 1].GetComponent<FaceHandler>().state[i])
                {
                    if (ar[
                        faces[t - 1].GetComponent<FaceHandler>().adjacentFaces[i].GetComponent<FaceHandler>()
                            .GetTileNumber()] == 0)
                    {
                        //this is the first connection we've seen to that room!
                        ar[
                            faces[t - 1].GetComponent<FaceHandler>().adjacentFaces[i].GetComponent<FaceHandler>()
                                .GetTileNumber()] = 1;
                        //ar[r[t-1].c[i].ID-1] = 1;  //add a 1 so we know to explore later
                    }
                }
            }

            return ar;
        }

        private void MapMaker()
        {
            List<int> order = new List<int>(); //a list of the rooms in order we're going to look at
            for (int i = 1; i < 31; i++)
            {
                order.Add(i);
            }

            //print(OutputList(order));
            Shuffle(order);
            //print(OutputList(order));

            for (int i = 0; i < 30; i++)
            {
                int r = order[i]; //r is ID of room
                int m = Random.Range(0, 4);

                if (faces[r - 1].GetComponent<FaceHandler>().CountCs() > 3)
                {
                    //r has 4 connections
                    int n = 0;
                    while (n < 4 && faces[r - 1].GetComponent<FaceHandler>().CountCs() > 3)
                    {
                        //if(caves.countCs(caves.r[r-1].c[m].ID) > 1){ //maybe >2?
                        if (faces[r - 1].GetComponent<FaceHandler>().adjacentFaces[m].GetComponent<FaceHandler>()
                            .CountCs() > 1)
                        {
                            Disconnect(r - 1,
                                faces[r - 1].GetComponent<FaceHandler>().adjacentFaces[m].GetComponent<FaceHandler>()
                                    .GetTileNumber());
                        }

                        if (CanFind() == false)
                        {
                            //we broke the connectivity, must re-connect
                            Connect(r - 1,
                                faces[r - 1].GetComponent<FaceHandler>().adjacentFaces[m].GetComponent<FaceHandler>()
                                    .GetTileNumber());
                        }

                        m = (m + 1) % 4;
                        n++;
                    }
                }

                if (faces[r - 1].GetComponent<FaceHandler>().CountCs() < 3)
                {
                    //r has 1 or 2 connections
                    int n = 0;
                    while (n < 4 && faces[r - 1].GetComponent<FaceHandler>().CountCs() < 3)
                    {
                        if (faces[r - 1].GetComponent<FaceHandler>().adjacentFaces[m].GetComponent<FaceHandler>()
                            .CountCs() < 3)
                        {
                            Connect((r - 1),
                                faces[r - 1].GetComponent<FaceHandler>().adjacentFaces[m].GetComponent<FaceHandler>()
                                    .GetTileNumber());
                        }

                        n++;
                        m = (m + 1) % 4;
                    }
                }
            }
        }

        private void CreateMeshVertices()
        {
            // Accumulate list of all vertices
            List<Vector3> allVertices = new List<Vector3>();
            foreach (var face in faces)
            {
                foreach (Vector3 vertex in face.GetComponent<MeshFilter>().mesh.vertices)
                {
                    Vector3 correctCoords = face.transform.TransformPoint(vertex);
                    FaceHandler faceHandler = face.GetComponent<FaceHandler>();
                    if (!allVertices.Contains(correctCoords))
                    {
                        allVertices.Add(correctCoords);
                        MeshVertex mv = new MeshVertex(faceHandler, correctCoords);
                        _vertices.Add(mv);
                        faceHandler.meshVertices.Add(mv);
                    }
                    else
                    {
                        foreach (var meshVertex in _vertices)
                        {
                            if (meshVertex.Coords == correctCoords)
                            {
                                meshVertex.ParentFaces.Add(faceHandler);
                                faceHandler.meshVertices.Add(meshVertex);
                            }
                        }
                    }
                }
            }

            UpdateVertexNeighbors();
        }

        public void ColonizedLineUpdate(bool setActive = true)
        {
            if (!borderAroundTerritory)
            {
                return;
            }

            DestroyVertexLines();

            List<MeshVertex> edgeVertices = new List<MeshVertex>();
            List<MeshVertex> discoveredEdgeVertices = new List<MeshVertex>();
            List<int> edgePairs = new List<int>();
            foreach (MeshVertex vertex in _vertices)
            {
                if (MeshVertex.IsOnColonizedEdge(vertex))
                {
                    edgeVertices.Add(vertex);
                }

                if (MeshVertex.IsOnDiscoveredEdge(vertex))
                {
                    discoveredEdgeVertices.Add(vertex);
                }
            }
            // Debug.Log("Vertices on edge: " + edgeVertices.Count);

            // O(n^3)
            foreach (MeshVertex vertex in edgeVertices)
            {
                foreach (MeshVertex neighbor in vertex.VertexNeighbors)
                {
                    int colonizedSharedFaces = 0;
                    int discoveredSharedFaces = 0;
                    int undiscoveredSharedFaces = 0;
                    foreach (var faceHandler1 in neighbor.ParentFaces)
                    {
                        foreach (var faceHandler2 in vertex.ParentFaces)
                        {
                            if (faceHandler2.Equals(faceHandler1))
                            {
                                if (faceHandler1.colonized)
                                {
                                    colonizedSharedFaces++;
                                }
                                else if (faceHandler1.discovered && !faceHandler1.colonized)
                                {
                                    discoveredSharedFaces++;
                                }
                                else if (!faceHandler1.discovered)
                                {
                                    undiscoveredSharedFaces++;
                                }
                            }
                        }
                    }

                    if (colonizedSharedFaces == 2 || colonizedSharedFaces == 0)
                    {
                        if (discoveredSharedFaces != 1 || undiscoveredSharedFaces != 1)
                        {
                            continue;
                        }
                    }

                    if (edgeVertices.Contains(neighbor))
                    {
                        if (!edgePairs.Contains(vertex.Id * neighbor.Id)
                        ) // Just going to hope that the products between vertex IDs are unique
                        {
                            edgePairs.Add(vertex.Id * neighbor.Id);
                            if (discoveredSharedFaces == 1 && undiscoveredSharedFaces == 1)
                            {
                                _lines.Add(DrawVertexLine(vertex, neighbor, setActive, TerritoryLineWidth / 2f));
                                continue;
                            }

                            _lines.Add(DrawVertexLine(vertex, neighbor, setActive));
                            // There might be more to add here
                        }
                    }
                }
            }

            foreach (var discoveredEdgeVertex in discoveredEdgeVertices)
            {
                foreach (MeshVertex neighbor in discoveredEdgeVertex.VertexNeighbors)
                {
                    int sharedFaces = 0;
                    foreach (var faceHandler1 in neighbor.ParentFaces)
                    {
                        foreach (var faceHandler2 in discoveredEdgeVertex.ParentFaces)
                        {
                            if (faceHandler2.Equals(faceHandler1) && faceHandler1.discovered)
                            {
                                sharedFaces++;
                            }
                        }
                    }

                    if (sharedFaces == 2 || sharedFaces == 0)
                    {
                        continue;
                    }

                    if (edgePairs.Contains(discoveredEdgeVertex.Id * neighbor.Id)) continue;
                    edgePairs.Add(discoveredEdgeVertex.Id * neighbor.Id);
                    _lines.Add(DrawVertexLine(discoveredEdgeVertex, neighbor, setActive, TerritoryLineWidth / 2f));
                    // There might be more to add here
                }
            }
        }

        private GameObject DrawVertexLine(MeshVertex fromVertex, MeshVertex toVertex, bool setActive,
            float width = TerritoryLineWidth)
        {
            // Debug.Log("Making Line...");
            GameObject line =
                Instantiate(_colonizedLine, GameObject.Find("EventSystem").transform);

            line.transform.position = Vector3.zero;
            line.transform.localScale = Vector3.one;
            line.SetActive(setActive);

            LineRenderer lr = line.GetComponent<LineRenderer>();
            if (!lr) return line; // Return immediately if not a Line GameObject

            lr.startWidth = width;
            lr.endWidth = width;
            if (width < TerritoryLineWidth)
            {
                lr.material = _altLineMaterial;
            }

            lr.SetPositions(new[] {fromVertex.Coords, toVertex.Coords});
            return line;
        }

        private void DestroyVertexLines()
        {
            foreach (var line in _lines)
            {
                Destroy(line);
            }

            _lines = new List<GameObject>();
        }

        // public void CreateMountains()
        // {
        //     // go through each face
        //     // if face is not discovered yet, skip
        //     // if face had a false adjacent face
        //     // get adjacent face
        //     // find the meshes that both of the faces share
        //     // create new Mountain and attach the mv's to it
        //     // draw line between the mountain ftm
        //
        //     mountains.Clear();
        //     
        //     foreach (var faceHandler in faceHandlers)
        //     {
        //         if (!faceHandler.discovered)
        //         {
        //             continue;
        //         }
        //         for (int i = 0; i < faceHandler.state.Length; i++)
        //         {
        //             if (!faceHandler.state[i])
        //             {
        //                 MeshVertex mv1 = null;
        //                 MeshVertex mv2 = null;
        //                 foreach (var vertex1 in faceHandler.adjacentFaceHandlers[i].meshVertices)
        //                 {
        //                     foreach (var vertex2 in faceHandler.meshVertices.Where(vertex2 => vertex1 == vertex2))
        //                     {
        //                         if (mv1 == null)
        //                         {
        //                             mv1 = vertex2;
        //                         }else if(mv2 == null)
        //                         {
        //                             mv2 = vertex2;
        //                         }
        //                         else
        //                         {
        //                             break;
        //                         }
        //                     }
        //                 }
        //
        //                 bool skip = mountains.Any(mt => mt.meshVertex1 == mv1 || mt.meshVertex1 == mv2 || mt.meshVertex2 == mv1 || mt.meshVertex2 == mv2);
        //                 if (skip)
        //                 {
        //                     continue;
        //                 }
        //                 
        //                 Mountain mountain = new Mountain(mv1, mv2);
        //                 // DrawVertexLine(mountain.meshVertex1, mountain.meshVertex2, true, 0.3f);
        //                 Vector3 mvAverage = (mountain.meshVertex1.Coords + mountain.meshVertex2.Coords) / 2;
        //                 mountain.mountain = Instantiate(Resources.Load<GameObject>("Objects/BorderLine"), GameObject.Find("EventSystem").transform);
        //                 // mountain.mountain = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //                 // mountain.mountain.transform.parent = GameObject.Find("EventSystem").transform;
        //                 mountain.mountain.transform.position = mvAverage;
        //                 mountain.mountain.transform.rotation = Quaternion.FromToRotation(mountain.mountain.transform.up, mountain.meshVertex1.Coords - mountain.mountain.transform.position);
        //                 // mountain.mountain.transform.rotation = Quaternion.FromToRotation(mountain.mountain.transform.right, mountain.meshVertex1.Coords - mountain.mountain.transform.position);
        //                 //mountain.mountain.transform.rotation = Quaternion.LookRotation(mountain.mountain.transform.position, Vector3.Cross(mountain.meshVertex1.Coords, mvAverage));
        //                 //mountain.mountain.transform.rotation = Quaternion.LookRotation(mountain.mountain.transform.position, mountain.meshVertex1.Coords);
        //                 mountains.Add(mountain);
        //             }
        //         }
        //     }
        //     
        // }

        private string OutputList(List<int> l)
        {
            string temp = "";
            foreach (int i in l)
            {
                temp += i + ", ";
            }

            return temp;
        }

        //Public Get functions

        public bool[] GetHintsToGive()
        {
            return _hintsToGive;
        }

        public int GetFaceInBattle()
        {
            return _faceInBattle;
        }

        // Public Set functions

        public void SetHintsToGive(bool[] hints)
        {
            _hintsToGive = hints;
        }

        public void SetFaceInBattle(int face)
        {
            _faceInBattle = face;
        }

        // Public Void functions
        public void
            ColonizeFace(
                int faceIndex) // There is also a colonize function on FaceHandler, so this one is hardly ever called
        {
            faces[faceIndex].GetComponent<FaceHandler>().SetColonized();
            ColonizedLineUpdate();
        }

        public List<int> FindAdjacentFaces(int inputLocation)
        {
            //print("called");
            // int[] adjFaces = new int[4];

            // for(int i = 0; i < 4; i++) {
            //     int[i] = faces[inputLocation].GetComponent<FaceHandler>().adjacentFaces[i].GetTileNumber();
            // }
            // return adjFaces;

            List<int> adjFaces = new List<int>();

            foreach (FaceHandler face in faceHandlers[inputLocation].adjacentFaceHandlers)
            {
                int adjFace = face.GetTileNumber();
                adjFaces.Add(adjFace);
            }

            return adjFaces;
        }

        private void UpdateVertexNeighbors()
        {
            foreach (var face in faces)
            {
                FaceHandler handler = face.GetComponent<FaceHandler>();
                foreach (MeshVertex vertex in handler.meshVertices)
                {
                    vertex.FindNeighbors(handler);
                }
            }
        }


        //not sure if this is helpful but this returns adjacent faces that are open to travel
        public List<int> FindOpenAdjacentFaces(int inputLocation)
        {
            List<int> openFaces = new List<int>();

            // print("Adjacent faces of #" + (inputLocation+1) + ": " + OutputList(FindAdjacentFaces(inputLocation)));

            for (int i = 0; i < 4; i++)
            {
                if (faces[inputLocation].GetComponent<FaceHandler>().state[i])
                {
                    //open connection!
                    int openFace = faces[inputLocation].GetComponent<FaceHandler>().adjacentFaces[i]
                        .GetComponent<FaceHandler>().GetTileNumber(); //syntax?
                    openFaces.Add(openFace);
                }
            }

            return openFaces;
        }

        public int[] GetPlayerWaveArray()
        {
            return new[] {soldiers};
        }

        public int[] GetWumplingWaveArray()
        {
            return wumplingWaves;
        }

        public void Savefunc()
        {
            int i = 0;
            foreach (GameObject face in faces)
            {
                FaceHandler faceHandler = face.GetComponent<FaceHandler>();
                for (int j = 0; j < 4; j++)
                    States[j, i] = faceHandler.state[j];
                BiomeNum[i] = (int) faceHandler.biomeType;
                IsColonized[i] = faceHandler.colonized;
                HazardType[i] = (int) faceHandler.GetHazardObject();
                ShowHint[i] = faceHandler.showHintOnTile;
                NoMoney[i] = faceHandler.noMoney;
                i++;
            }

            _totalTroops = GetComponent<GameMeta>().availableTroops.Count() +
                           GetComponent<GameMeta>().exhaustedTroops.Count();

            foreach (FaceHandler face in faceHandlers)
                _totalTroops += face.heldTroops.Count();

            _troopType = new int[_totalTroops];
            _troopName = new string[_totalTroops];
            _isExausted = new bool[_totalTroops];
            _isHeld = new bool[_totalTroops];
            _heldLoc = new int[_totalTroops];

            i = 0;
            foreach (TroopMeta troop in GetComponent<GameMeta>().availableTroops)
            {
                _troopType[i] = (int) troop.Type;
                _troopName[i] = troop.Name;
                _isExausted[i] = false;
                _isHeld[i] = false;
                i++;
            }

            foreach (TroopMeta troop in GetComponent<GameMeta>().exhaustedTroops)
            {
                _troopType[i] = (int) troop.Type;
                _troopName[i] = troop.Name;
                _isExausted[i] = true;
                _isHeld[i] = false;
                i++;
            }

            for (int j = 0; j < faceHandlers.Count(); j++)
            {
                FaceHandler face = faceHandlers[j];
                print(face.heldTroops.Count());
                if (face.heldTroops.Count() > 0)
                {
                    foreach (TroopMeta troop in face.heldTroops)
                    {
                        _troopType[i] = (int) troop.Type;
                        _troopName[i] = troop.Name;
                        _isExausted[i] = true;
                        _isHeld[i] = true;
                        _heldLoc[i] = j;
                        i++;
                    }
                }
            }


            DoSaving.DoTheSaving(this, States, BiomeNum, IsColonized, HazardType, ShowHint, NoMoney, _troopType, _troopName,
                _isExausted, _isHeld, _heldLoc, _totalTroops);
        }

        public void Loadfunc()
        {
            SaveData data = DoSaving.LoadTheData();

            meta.turnsElapsed = data.turnsElapsed;
            didSomething = data.didSomething;
            meta.money = data.money;
            meta.nukes = data.nukes;
            meta.sensorTowers = data.sensors;

            wumpus.location = faces[data.wumpusLocation].GetComponent<FaceHandler>();

            foreach (GameObject face in faces)
            {
                face.GetComponent<FaceHandler>().colonized = false;
                face.GetComponent<FaceHandler>().discovered = false;
            }

            int i = 0;
            foreach (GameObject face in faces)
            {
                FaceHandler faceHandler = face.GetComponent<FaceHandler>();
                for (int j = 0; j < 4; j++)
                    faceHandler.state[j] = data.state[j, i];
                faceHandler.biomeType = (BiomeType) data.biomeNum[i];
                faceHandler.SetHazard((HazardTypes) data.hazardType[i]);
                faceHandler.showHintOnTile = data.showHint[i];
                faceHandler.noMoney = data.noMoney[i];
                if (data.isColonized[i])
                {
                    print("is colonized");
                    faceHandler.SetColonized();
                }
                else
                    print("not colonized");

                ColonizedLineUpdate();
                faceHandler.UpdateFaceColors();
                i++;
            }


            meta.availableTroops.Clear();
            meta.exhaustedTroops.Clear();

            for (i = 0; i < data.troopType.Count(); i++)
            {
                if (data.isHeld[i])
                {
                    faceHandlers[data.heldLoc[i]].heldTroops
                        .Add(new TroopMeta((TroopType) data.troopType[i], data.troopName[i]));
                    faceHandlers[data.heldLoc[i]].UpdateFaceColors();
                }
                else if (data.isExausted[i])
                    meta.exhaustedTroops.Add(new TroopMeta((TroopType) data.troopType[i], data.troopName[i]));

                if (!data.isExausted[i] && !data.isHeld[i])
                    meta.availableTroops.Add(new TroopMeta((TroopType) data.troopType[i], data.troopName[i]));
            }
        }

        public void SetStartGame(bool to = true)
        {
            _startGame = to;
        }

        public bool GetStartGame()
        {
            return _startGame;
        }
    }
}