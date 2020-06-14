using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Gui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum HazardTypes
{
    None,
    Pit,
    Bat
}

namespace CommandView
{
    // Planet will be the Global (constant) data holder
    public class Planet : MonoBehaviour
    {
        // Hold an instance of the Planet
        public static Planet Instance;

        // Wumpus
        public Wumpus.Wumpus wumpus;

        // Hold ref to all of the faces
        public GameObject[] faces;
        public FaceHandler[] faceHandlers;

        public EventSystem lineEventSystem;

        // Planet properties and class-wide variables
        private readonly Random _random = new Random();
        private bool _isHidden;
        public bool displayHints;
        private bool _lastPressed;

        // UI global variables
        private List<MeshVertex> _vertices = new List<MeshVertex>();
        private GameObject _colonizedLine;
        private List<GameObject> _lines = new List<GameObject>();
        public bool borderAroundTerritory;

        // Mini-game global variables
        private int _faceInBattle = -1; // which face is being played on (-1=none)
        private bool[] _hintsToGive = new bool[3]; //index 0=Wumpus, 1=pit, 2=bat
        public int[] wumplingWaves;
        public int soldiers;

        public GameMeta meta;
        public MiniGameResult result;

        private void Awake()
        {
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
                faceHandler.biomeType = biomeNum == 0 ? BiomeType.Planes :
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


            print("Planet Started");

            // Initialize hazards
            MakeHazardObjects(splashSpot);



            // print("Open faces for face 3: " + OutputList(FindOpenAdjacentFaces(2)));
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

            if (Input.GetButtonDown("ShowAllHints"))
            {
                displayHints = !displayHints;
            }

            // else
            // {
            //     if (Input.GetButtonUp("ShowAllHints"))
            //     {
            //         _lastPressed = false;
            //     }
            // }
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

        private /*static*/ void Shuffle(List<int> list, Random rnd)
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
            Random random = new Random();

            List<int> order = new List<int>(); //a list of the rooms in order we're going to look at
            for (int i = 1; i < 31; i++)
            {
                order.Add(i);
            }

            //print(OutputList(order));
            Shuffle(order, random);
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

        public void ColonizedLineUpdate()
        {
            if (!borderAroundTerritory)
            {
                return;
            }

            DestroyVertexLines();

            List<MeshVertex> edgeVertices = new List<MeshVertex>();
            List<int> edgePairs = new List<int>();
            foreach (MeshVertex vertex in _vertices)
            {
                if (MeshVertex.IsOnColonizedEdge(vertex))
                {
                    edgeVertices.Add(vertex);
                }
            }
            // Debug.Log("Vertices on edge: " + edgeVertices.Count);

            // O(n^3)
            foreach (MeshVertex vertex in edgeVertices)
            {
                foreach (MeshVertex neighbor in vertex.VertexNeighbors)
                {
                    int colonizedSharedFaces = 0;
                    foreach (var faceHandler1 in neighbor.ParentFaces)
                    {
                        foreach (var faceHandler2 in vertex.ParentFaces)
                        {
                            if (!faceHandler2.Equals(faceHandler1))
                            {
                                continue;
                            }

                            if (!(faceHandler1.colonized && faceHandler2.colonized))
                            {
                                continue;
                            }

                            colonizedSharedFaces++;
                        }
                    }

                    if (colonizedSharedFaces == 2 || colonizedSharedFaces == 0)
                    {
                        continue;
                    }

                    if (edgeVertices.Contains(neighbor))
                    {
                        if (!edgePairs.Contains(vertex.Id * neighbor.Id)
                        ) // Just going to hope that the products between vertex IDs are unique
                        {
                            edgePairs.Add(vertex.Id * neighbor.Id);
                            DrawVertexLine(vertex, neighbor);
                            // There might be more to add here
                        }
                    }
                }
            }
        }

        private void DrawVertexLine(MeshVertex fromVertex, MeshVertex toVertex)
        {
            // Debug.Log("Making Line...");
            GameObject line =
                Instantiate(_colonizedLine, GameObject.Find("EventSystem").transform);

            line.SetActive(true);
            line.transform.position = Vector3.zero;
            line.transform.localScale = Vector3.one;
            _lines.Add(line);
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.SetPositions(new[] {fromVertex.Coords, toVertex.Coords});
        }

        private void DestroyVertexLines()
        {
            foreach (var line in _lines)
            {
                Destroy(line);
            }

            _lines = new List<GameObject>();
        }

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
            DoSaving.DoTheSaving(this);
        }

        public void Loadfunc()
        {
            /*Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;
            if (sceneName == "MVPMiniGame")
            {
                GameObject canvas = GameObject.Find("PauseCanvas");
                PauseMenu menu = canvas.GetComponent<PauseMenu>();
                menu.Resume();
                SceneManager.LoadScene("CommandView");
            }*/
            
            SaveData data = DoSaving.LoadTheData();

            meta.turnsElapsed = data.turnsElapsed;
            meta.money = data.money;
            meta.nukes = data.nukes;

            wumpus.location = data.wumpusLocation;

            int i = 0;
            foreach (GameObject face in faces)
            {
                FaceHandler faceHandler = face.GetComponent<FaceHandler>();
                faceHandler.biomeType = (BiomeType)data.biomeNum[i];
                faceHandler.colonized = data.isColonized[i];
                faceHandler.SetHazard((HazardTypes)data.hazardType[i]);
            }

            meta.availableTroops.Clear();
            meta.exhaustedTroops.Clear();

            for (i = 0; i < data.troopType.Count(); i++)
            {
                if (data.isEhausted[i])
                {
                    meta.exhaustedTroops.Add(new TroopMeta((TroopType)data.troopType[i],data.troopName[i]));
                }
                else
                {
                    meta.availableTroops.Add(new TroopMeta((TroopType)data.troopType[i], data.troopName[i]));
                }
            }
        }

    }
}