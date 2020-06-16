using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CommandView;
using MiniGame.Creatures.DeathHandlers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        public readonly int row;
        public readonly int col;
        public int distanceFromStart;
        public int distanceFromPath;

        public Node previousNode;
        //public Direction previousDirection;

        public int connections;
        public bool connectedDown; //to node with larger row number
        public bool connectedRight; //to node with larger column number

        public int iTileRow;
        public int iTileCol;

        public Node(int row, int col)
        {
            this.row = row;
            this.col = col;
            this.distanceFromStart = -1;
            this.distanceFromPath = -1;
            this.connectedDown = false;
            this.connectedRight = false;
            this.connections = 0;
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        public const int nodeRows = 4;
        public const int nodeCols = 4;

        public const int startNodeRow = 0; //TODO randomize start and end location
        public const int startNodeCol = 0;
        public const int endNodeRow = 3;
        public const int endNodeCol = 3;

        public const int interiorTileSize = 1;
        public const int tilesPerInterior = 15;
        private const int interiorRows = (tilesPerInterior + 1) * nodeRows + 1;
        private const int interiorCols = (tilesPerInterior + 1) * nodeCols + 1;

        public NavMeshSurface surface;

        private const bool randomizeDoorLocations = true;
        private const int doorwaySize = 6;

        private static readonly Vector3 mazeLocationOffset = new Vector3(
            -(nodeRows / 2) * tilesPerInterior * interiorTileSize,
            1,
            -(nodeCols / 2) * tilesPerInterior * interiorTileSize);

        public GameObject pathTilePrefab;
        public GameObject wallTilePrefab;
        public GameObject coverTilePrefab;
        public GameObject nestPrefab;


        public GameObject[] biomeFloors;


        private GameObject[,] iTilePrefabs;
        private GameObject[,] iTiles;
        private List<GameObject> nests = new List<GameObject>();

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

        public void GenerateMaze()
        {
            randomMaze();
            GenerateInteriorTilePrefabs();
            GenerateInteriorTileObjects();
        }

        public void randomMaze()
        {
            this._nodes = new Node[nodeRows, nodeCols];
            for (int i = 0; i < nodeRows; i++)
            {
                for (int j = 0; j < nodeCols; j++)
                {
                    Node node = new Node(i, j);
                    _nodes[i, j] = node;
                }
            }


            //Random r = new Random();
            //TODO randomize start location
            Node current = _nodes[startNodeRow, startNodeCol];
            current.distanceFromStart = 0;
            // <generate maze>
            while (!(
                isCornered(_nodes, current, nodeRows, nodeCols)
                &&
                current.distanceFromStart == 0
                //continue until you are at the root and cornered
            ))
            {
                if (isCornered(_nodes, current, nodeRows, nodeCols) ||
                    forceDeadEnd(_nodes, current, nodeRows, nodeCols))
                {
                    // if nowhere to go or forced, backtrack
                    current = current.previousNode;
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
                            if (current.col == nodeCols - 1 ||
                                _nodes[current.row, current.col + 1].distanceFromStart != -1)
                            {
                                //if can't go right, try left
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.row, current.col + 1];
                            current.connectedRight = true;
                            break;
                        case (int) Direction.Left:
                            if (current.col == 0 || _nodes[current.row, current.col - 1].distanceFromStart != -1)
                            {
                                //if can't go left, try up
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.row, current.col - 1];
                            next.connectedRight = true;
                            break;
                        case (int) Direction.Up:
                            if (current.row == nodeRows - 1 ||
                                _nodes[current.row + 1, current.col].distanceFromStart != -1)
                            {
                                //if can't go up, try down
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.row + 1, current.col];
                            current.connectedDown = true;
                            break;
                        case (int) Direction.Down:
                            if (current.row == 0 || _nodes[current.row - 1, current.col].distanceFromStart != -1)
                            {
                                //if can't go down, try right
                                direction = (direction + 1) % 4; //technically not fair but idc;
                                break;
                            }

                            next = _nodes[current.row - 1, current.col];
                            next.connectedDown = true;
                            break;
                    }
                }

                //when you've selected next,
                next.distanceFromStart = current.distanceFromStart + 1;
                next.previousNode = current;
                next.connections++;
                current.connections++;

                current = next;

                //OpenPathBetweenNodes(current);
            }
            // </generate maze>


            //follow previousNode from end to find critical path Nodes
            current = _nodes[endNodeRow, endNodeCol];
            while (current != null)
            {
                current.distanceFromPath = 0;
                current = current.previousNode;
            }

            for (int i = 0; i < nodeRows; i++)
            {
                for (int j = 0; j < nodeCols; j++)
                {
                    setDistanceFromPath(_nodes[i, j]);
                }
            }

            string debug = "";
            for (int i = 0; i < nodeRows; i++)
            {
                for (int j = 0; j < nodeCols; j++)
                {
                    debug += _nodes[i, j].connections + " ";
                }

                debug += "\n";
            }

            // print(debug);
        }

        private bool isCornered(Node[,] maze, Node current, int rows, int cols)
        {
            return (
                (current.row == rows - 1 ||
                 maze[current.row + 1, current.col].distanceFromStart != -1
                ) //either at right edge or right neighbor is visited
                &&
                (current.row == 0 ||
                 maze[current.row - 1, current.col].distanceFromStart != -1
                ) //either at left edge or left neighbor is visited
                &&
                (current.col == cols - 1 ||
                 maze[current.row, current.col + 1].distanceFromStart != -1
                ) //either at top edge or top neighbor is visited
                &&
                (current.col == 0 ||
                 maze[current.row, current.col - 1].distanceFromStart != -1
                ) //either at bottom edge or bottom neighbor is visited
            );
        }

        public bool isStartNode(Node current)
        {
            return current.row == startNodeRow && current.col == startNodeCol;
        }

        private bool isEndNode(Node current)
        {
            return current.row == endNodeRow && current.col == endNodeCol;
        }

        private bool forceDeadEnd(Node[,] maze, Node current, int rows, int cols)
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

            if (current.distanceFromPath == -1)
            {
                setDistanceFromPath(current.previousNode);
                if (current.previousNode != null)
                {
                    current.distanceFromPath = current.previousNode.distanceFromPath + 1;
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
            iTilePrefabs = new GameObject[interiorRows, interiorCols];
            for (int i = 0; i < interiorRows; i++)
            {
                for (int j = 0; j < interiorCols; j++)
                {
                    iTilePrefabs[i, j] = wallTilePrefab; //fills the whole thing with walls
                }
            }

            for (int nodeI = 0; nodeI < nodeRows; nodeI++)
            {
                for (int nodeJ = 0; nodeJ < nodeCols; nodeJ++)
                {
                    //for each "room"
                    int doorwayOffsetRight;
                    int doorwayOffsetBottom;
                    doorwayOffsetRight = Random.Range(0, tilesPerInterior - doorwaySize + 1);
                    doorwayOffsetBottom = Random.Range(0, tilesPerInterior - doorwaySize + 1);

                    for (int tileI = 0; tileI < tilesPerInterior + 1; tileI++)
                    {
                        for (int tileJ = 0; tileJ < tilesPerInterior + 1; tileJ++)
                        {
                            int i = 1 + nodeI * (tilesPerInterior + 1) + tileI;
                            int j = 1 + nodeJ * (tilesPerInterior + 1) + tileJ;
                            GameObject prefab = pathTilePrefab;

                            if (tileI == tilesPerInterior) //the bottom wall
                            {
                                prefab = wallTilePrefab;
                                if (_nodes[nodeI, nodeJ].connectedDown &&
                                    tileJ >= doorwayOffsetBottom &&
                                    tileJ < doorwayOffsetBottom + doorwaySize) //if it's a doorway
                                {
                                    prefab = pathTilePrefab;
                                }
                            }

                            if (tileJ == tilesPerInterior) //the right wall
                            {
                                prefab = wallTilePrefab;
                                if (_nodes[nodeI, nodeJ].connectedRight &&
                                    tileI >= doorwayOffsetRight &&
                                    tileI < doorwayOffsetRight + doorwaySize) //if it's a doorway
                                {
                                    prefab = pathTilePrefab;
                                }
                            }

                            iTilePrefabs[i, j] = prefab;
                        }
                    }
                }
            }
        }

        private void GenerateInteriorTileObjects()
        {
            Planet planet = GameObject.Find("Planet").GetComponent<Planet>();

            biomeFloors[(int) planet.faces[planet.GetFaceInBattle()].GetComponent<FaceHandler>().biomeType - 1].SetActive(true);
            
            spawnPrefabs();
            spawnNests();

            surface.BuildNavMesh();
        }

        private void spawnPrefabs()
        {
            iTiles = new GameObject[interiorRows, interiorCols];
            for (int i = 0; i < interiorRows; i++)
            {
                for (int j = 0; j < interiorCols; j++)
                {
                    iTiles[i, j] = Instantiate(iTilePrefabs[i, j],
                        new Vector3(i * interiorTileSize, 0, j * interiorTileSize) + mazeLocationOffset,
                        new Quaternion());
                }
            }
        }

        private void spawnNests()
        {
            for (int nodeI = 0; nodeI < nodeRows; nodeI++)
            {
                for (int nodeJ = 0; nodeJ < nodeCols; nodeJ++)
                {
                    Node n = _nodes[nodeI, nodeJ];
                    if (n.connections == 1 && !isStartNode(n))
                    {
                        Vector3 centerOfTileOffset = new Vector3((float) 5.5, 0, (float) 5.5);
                        float i = nodeI * (tilesPerInterior + 1);
                        float j = nodeJ * (tilesPerInterior + 1);
                        GameObject nest = Instantiate(nestPrefab,
                            new Vector3(i * interiorTileSize,
                                0,
                                j * interiorTileSize) + centerOfTileOffset + mazeLocationOffset,
                            new Quaternion());
                        if (isEndNode(n))
                        {
                            nest.GetComponent<NestDeathHandler>().OnDeathEndMiniGame();
                        }

                        nests.Add(nest);
                    }
                }
            }
        }

        public Vector3 GetRandomPositionInNodeFromNode(Node node)
        {
            int row = Random.Range(node.row * interiorRows + 1, node.row * interiorRows + tilesPerInterior + 2);
            int col = Random.Range(node.col * interiorCols + 1, node.col * interiorCols + tilesPerInterior + 2);

            return iTiles[row, col].transform.position;
        }
    }
}