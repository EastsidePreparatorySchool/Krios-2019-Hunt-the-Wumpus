using CommandView;
using MiniGame.Creatures;
using MiniGame.Creatures.DeathHandlers;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MiniGame.Terrain
{
    public enum Direction

    {
        Right = 0,
        Left = 1,
        Up = 2,
        Down = 3
    }

    public class Node
    {
        public readonly int Row;
        public readonly int Col;
        public int DistanceFromStart;
        public int DistanceFromPath;

        public Node PreviousNode;
        //public Direction previousDirection;

        public int Connections;
        public bool ConnectedDown; //to node with larger row number
        public bool ConnectedRight; //to node with larger column number

        // public int ITileRow;
        // public int ITileCol;

        public Node(int row, int col)
        {
            Row = row;
            Col = col;
            DistanceFromStart = -1;
            DistanceFromPath = -1;
            ConnectedDown = false;
            ConnectedRight = false;
            Connections = 0;
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        public const int NodeRows = 4;
        public const int NodeCols = 4;

        public const int StartNodeRow = 0; //TODO randomize start and end location
        public const int StartNodeCol = 0;
        public const int EndNodeRow = 3;
        public const int EndNodeCol = 3;

        public const int InteriorTileSize = 1;
        public const int TilesPerInterior = 15;
        private const int InteriorRows = (TilesPerInterior + 1) * NodeRows + 1;
        private const int InteriorCols = (TilesPerInterior + 1) * NodeCols + 1;

        public NavMeshSurface surface;

        private const bool RandomizeDoorLocations = true;
        private const int DoorwaySize = 6;

        private static readonly Vector3 MazeLocationOffset = new Vector3(
            -(NodeRows / 2) * TilesPerInterior * InteriorTileSize,
            1,
            -(NodeCols / 2) * TilesPerInterior * InteriorTileSize);

        public GameObject pathTilePrefab;
        public GameObject wallTilePrefab;
        public GameObject[] wallTilePrefabs;
        public GameObject[] wallTilePrefabSeconds;
        public GameObject coverTilePrefab;
        public GameObject nestPrefab;


        public GameObject[] biomeFloors;
        public HazardTypes hazardOnTile;
        public BiomeType biomeType;


        private GameObject[,] _iTilePrefabs;

        private GameObject[,] _iTiles;
        // private readonly List<GameObject> _nests = new List<GameObject>();

        private Node[,] _nodes;

        public Node[,] GetNodeMap()
        {
            return _nodes;
        }

        // Start is called before the first frame update
        void Start()
        {
            // print("started");

            //NodeInterior.PopulateTileMap(_maze, rows, cols);      
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void GenerateMaze(HazardTypes hazard, BiomeType type)
        {
            hazardOnTile = hazard;
            biomeType = type;
            RandomMaze();
            GenerateInteriorTilePrefabs();
            GenerateInteriorTileObjects();
        }

        public void RandomMaze()
        {
            this._nodes = new Node[NodeRows, NodeCols];
            for (int i = 0; i < NodeRows; i++)
            {
                for (int j = 0; j < NodeCols; j++)
                {
                    Node node = new Node(i, j);
                    _nodes[i, j] = node;
                }
            }


            //Random r = new Random();
            //TODO randomize start location
            Node current = _nodes[StartNodeRow, StartNodeCol];
            current.DistanceFromStart = 0;
            // <generate maze>
            while (!(
                isCornered(_nodes, current, NodeRows, NodeCols)
                &&
                current.DistanceFromStart == 0
                //continue until you are at the root and cornered
            ))
            {
                if (isCornered(_nodes, current, NodeRows, NodeCols) || ForceDeadEnd(current)
                    /*ForceDeadEnd(_nodes, current, NodeRows, NodeCols)*/)
                {
                    // if nowhere to go or forced, backtrack
                    current = current.PreviousNode;
                    continue;
                }

                int direction = Random.Range(0, 4); // randomly picks direction to explore
                Node next = null;
                while (next == null)
                {
                    //keep going until you pick a direction
                    switch (direction)
                    {
                        case (int) Direction.Right:
                            if (current.Col == NodeCols - 1 ||
                                _nodes[current.Row, current.Col + 1].DistanceFromStart != -1)
                            {
                                //if can't go right, try left
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.Row, current.Col + 1];
                            current.ConnectedRight = true;
                            break;
                        case (int) Direction.Left:
                            if (current.Col == 0 || _nodes[current.Row, current.Col - 1].DistanceFromStart != -1)
                            {
                                //if can't go left, try up
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.Row, current.Col - 1];
                            next.ConnectedRight = true;
                            break;
                        case (int) Direction.Up:
                            if (current.Row == NodeRows - 1 ||
                                _nodes[current.Row + 1, current.Col].DistanceFromStart != -1)
                            {
                                //if can't go up, try down
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.Row + 1, current.Col];
                            current.ConnectedDown = true;
                            break;
                        case (int) Direction.Down:
                            if (current.Row == 0 || _nodes[current.Row - 1, current.Col].DistanceFromStart != -1)
                            {
                                //if can't go down, try right
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.Row - 1, current.Col];
                            next.ConnectedDown = true;
                            break;
                    }
                }

                //when you've selected next,
                next.DistanceFromStart = current.DistanceFromStart + 1;
                next.PreviousNode = current;
                next.Connections++;
                current.Connections++;

                current = next;

                //OpenPathBetweenNodes(current);
            }
            // </generate maze>


            //follow previousNode from end to find critical path Nodes
            current = _nodes[EndNodeRow, EndNodeCol];
            while (current != null)
            {
                current.DistanceFromPath = 0;
                current = current.PreviousNode;
            }

            for (int i = 0; i < NodeRows; i++)
            {
                for (int j = 0; j < NodeCols; j++)
                {
                    setDistanceFromPath(_nodes[i, j]);
                }
            }

            // string debug = "";
            // for (int i = 0; i < NodeRows; i++)
            // {
            //     for (int j = 0; j < NodeCols; j++)
            //     {
            //         debug += _nodes[i, j].Connections + " ";
            //     }
            //
            //     debug += "\n";
            // }
            //
            // // print(debug);
        }

        private bool isCornered(Node[,] maze, Node current, int rows, int cols)
        {
            return (
                (current.Row == rows - 1 ||
                 maze[current.Row + 1, current.Col].DistanceFromStart != -1
                ) //either at right edge or right neighbor is visited
                &&
                (current.Row == 0 ||
                 maze[current.Row - 1, current.Col].DistanceFromStart != -1
                ) //either at left edge or left neighbor is visited
                &&
                (current.Col == cols - 1 ||
                 maze[current.Row, current.Col + 1].DistanceFromStart != -1
                ) //either at top edge or top neighbor is visited
                &&
                (current.Col == 0 ||
                 maze[current.Row, current.Col - 1].DistanceFromStart != -1
                ) //either at bottom edge or bottom neighbor is visited
            );
        }

        public static bool IsStartNode(Node current)
        {
            return current.Row == StartNodeRow && current.Col == StartNodeCol;
        }

        private bool isEndNode(Node current)
        {
            return current.Row == EndNodeRow && current.Col == EndNodeCol;
        }

        // private bool ForceDeadEnd(Node[,] maze, Node current, int rows, int cols)
        // {
        //     if (isEndNode(current)) //forces end if at end node TODO force dead ends elsewhere
        //     {
        //         return true;
        //     }
        //
        //     return false;
        // }
        private bool ForceDeadEnd(Node current)
        {
            if (isEndNode(current)) //forces end if at end node TODO force dead ends elsewhere
            {
                return true;
            }

            return false;
        }

        private void setDistanceFromPath(Node current)
        {
            if (current == null)
            {
                print("maze generation distanceFromPath is broken");
                return;
            }

            if (current.DistanceFromPath == -1)
            {
                setDistanceFromPath(current.PreviousNode);
                if (current.PreviousNode != null)
                {
                    current.DistanceFromPath = current.PreviousNode.DistanceFromPath + 1;
                }
            }
        }

        /*private void OpenPathBetweenNodes(Node node)
        {
            if (node.previousDirection.Equals(null)) return;
            
            int pathPos = Random.Range(0, interiorTileSize / 2);
            int[] path = new int[Node.PathSize];
            for (int i = 0; i < Random.Range(path.Length / 3, path.Length / 2); i++)
            {
                path[i] = pathPos + i;
            }

            Debug.Log(path);
            node.NodeOpenings[(int) node.previousDirection] = path;
            node.previousNode.NodeOpenings[(int) getOppositeDirection(node.previousDirection)] = path;
        }

        private Direction getOppositeDirection(Direction d)
        {
            switch ((int) d)
            {
                case 0: // Right
                    return Direction.Left;
                case 1: // Left
                    return Direction.Right;
                case 2: // Up
                    return Direction.Down;
            }
            // Down
            return Direction.Up;
        }*/

        private void GenerateInteriorTilePrefabs()
        {
            //print("trying to generate interior tiles");
            _iTilePrefabs = new GameObject[InteriorRows, InteriorCols];
            for (int i = 0; i < InteriorRows; i++)
            {
                for (int j = 0; j < InteriorCols; j++)
                {
                    ReassignWallTilePrefab();

                    _iTilePrefabs[i, j] = wallTilePrefab; //fills the whole thing with walls
                }
            }

            for (int nodeI = 0; nodeI < NodeRows; nodeI++)
            {
                for (int nodeJ = 0; nodeJ < NodeCols; nodeJ++)
                {
                    //for each "room"
                    int doorwayOffsetRight;
                    int doorwayOffsetBottom;
                    doorwayOffsetRight = Random.Range(0, TilesPerInterior - DoorwaySize + 1);
                    doorwayOffsetBottom = Random.Range(0, TilesPerInterior - DoorwaySize + 1);

                    for (int tileI = 0; tileI < TilesPerInterior + 1; tileI++)
                    {
                        for (int tileJ = 0; tileJ < TilesPerInterior + 1; tileJ++)
                        {
                            int i = 1 + nodeI * (TilesPerInterior + 1) + tileI;
                            int j = 1 + nodeJ * (TilesPerInterior + 1) + tileJ;
                            GameObject prefab = pathTilePrefab;

                            if (tileI == TilesPerInterior) //the bottom wall
                            {
                                ReassignWallTilePrefab();
                                prefab = wallTilePrefab;
                                if (_nodes[nodeI, nodeJ].ConnectedDown &&
                                    tileJ >= doorwayOffsetBottom &&
                                    tileJ < doorwayOffsetBottom + DoorwaySize) //if it's a doorway
                                {
                                    prefab = pathTilePrefab;
                                }
                            }

                            if (tileJ == TilesPerInterior) //the right wall
                            {
                                ReassignWallTilePrefab();
                                prefab = wallTilePrefab;
                                if (_nodes[nodeI, nodeJ].ConnectedRight &&
                                    tileI >= doorwayOffsetRight &&
                                    tileI < doorwayOffsetRight + DoorwaySize) //if it's a doorway
                                {
                                    prefab = pathTilePrefab;
                                }
                            }

                            _iTilePrefabs[i, j] = prefab;
                        }
                    }
                }
            }
        }

        private void ReassignWallTilePrefab()
        {
            bool altChance = Random.Range(0, 100) < 95;
            //print(altChance);

            if (hazardOnTile.Equals(HazardTypes.Pit))
            {
                wallTilePrefab = altChance ? wallTilePrefabs[0] : wallTilePrefabSeconds[0];
            }
            else
            {
                int biomeIndex = (int) biomeType;
                wallTilePrefab = altChance ? wallTilePrefabs[biomeIndex] : wallTilePrefabSeconds[biomeIndex];
            }
        }

        private void GenerateInteriorTileObjects()
        {
            Planet planet = GameObject.Find("Planet").GetComponent<Planet>();

            biomeFloors[(int) planet.faces[planet.GetFaceInBattle()].GetComponent<FaceHandler>().biomeType - 1]
                .SetActive(true);

            SpawnPrefabs();
            SpawnNests();

            surface.BuildNavMesh();
        }

        private void SpawnPrefabs()
        {
            _iTiles = new GameObject[InteriorRows, InteriorCols];
            for (int i = 0; i < InteriorRows; i++)
            {
                for (int j = 0; j < InteriorCols; j++)
                {
                    int rotation = 90 * Random.Range(0, 4);
                    Quaternion appliedRotation = Quaternion.Euler(0, rotation, 0);

                    _iTiles[i, j] = Instantiate(_iTilePrefabs[i, j],
                        new Vector3(i * InteriorTileSize, 0, j * InteriorTileSize) + MazeLocationOffset,
                        appliedRotation);
                }
            }
        }

        private void SpawnNests()
        {
            for (int nodeI = 0; nodeI < NodeRows; nodeI++)
            {
                for (int nodeJ = 0; nodeJ < NodeCols; nodeJ++)
                {
                    Node n = _nodes[nodeI, nodeJ];
                    if (n.Connections == 1 && !IsStartNode(n))
                    {
                        CreateNest(n, nodeI, nodeJ, 10);
                    }
                    else if (hazardOnTile == HazardTypes.Pit && !IsStartNode(n))
                    {
                        CreateNest(n, nodeI, nodeJ, 3);
                    }
                }
            }
        }

        private void CreateNest(Node n, int nodeI, int nodeJ, float spawnTime)
        {
            Vector3 centerOfTileOffset = new Vector3((float) 5.5, 0, (float) 5.5);
            float i = nodeI * (TilesPerInterior + 1);
            float j = nodeJ * (TilesPerInterior + 1);
            GameObject nest = Instantiate(nestPrefab,
                new Vector3(i * InteriorTileSize,
                    1,
                    j * InteriorTileSize) + centerOfTileOffset + MazeLocationOffset, Quaternion.Euler(180, 0, 0));
            nest.GetComponent<NestController>().timeBetweenSpawns = spawnTime;
            if (isEndNode(n))
            {
                nest.GetComponent<NestDeathHandler>().OnDeathEndMiniGame();
            }

            // _nests.Add(nest);
        }

        public Vector3 GetRandomPositionInNodeFromNode(Node node)
        {
            int row = Random.Range(node.Row * InteriorRows + 2, node.Row * InteriorRows + TilesPerInterior);
            int col = Random.Range(node.Col * InteriorCols + 2, node.Col * InteriorCols + TilesPerInterior);

            return _iTiles[row, col].transform.position;
        }
    }
}