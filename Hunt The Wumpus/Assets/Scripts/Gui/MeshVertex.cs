using System;
using System.Collections.Generic;
using CommandView;
using UnityEngine;
using Random = System.Random;

namespace Gui
{
    public class MeshVertex
    {
        private const double DistanceBetweenAdjacentVertices = 4.911235;
        private const double Tolerance = 0.01;
        public Vector3 Coords;
        public List<FaceHandler> ParentFaces = new List<FaceHandler>();
        public List<MeshVertex> VertexNeighbors = new List<MeshVertex>();
        public int Id;
        private static Random _random = new Random();
        
        public static List<MeshVertex> vertices = new List<MeshVertex>();

        public MeshVertex(FaceHandler parent, Vector3 vertexCoords)
        {
            ParentFaces.Add(parent);
            Coords = vertexCoords;
            Id = _random.Next(-900000, 900000);
        }

        public void FindNeighbors(FaceHandler parentFace)
        {
            foreach (var vertex in parentFace.meshVertices)
            {
                if (Math.Abs(DistanceBetweenAdjacentVertices - Vector3.Distance(Coords, vertex.Coords)) < Tolerance)
                {
                    VertexNeighbors.Add(vertex);
                }
                
                //Debug.Log(Coords);
            }

            foreach (GameObject face in parentFace.adjacentFaces)
            {
                foreach (var vertex in face.GetComponent<FaceHandler>().meshVertices)
                {
                    AddMeshVertexIfUnique(vertex);
                }
            }

            if (VertexNeighbors.Count == 4)
            {
                foreach (GameObject face in parentFace.adjacentFaces)
                {
                    foreach (GameObject onceRemovedNeighborFace in face.GetComponent<FaceHandler>().adjacentFaces)
                    {
                        foreach (var vertex in onceRemovedNeighborFace.GetComponent<FaceHandler>().meshVertices)
                        {
                            AddMeshVertexIfUnique(vertex);
                        }
                    }
                }
            }
            
            // Debug.Log(VertexNeighbors.Count);
            //  foreach (var VARIABLE in VertexNeighbors)
            //  {
            //      Debug.Log(VARIABLE.Coords);
            // }
        }

        private void AddMeshVertexIfUnique(MeshVertex vertex)
        {
            if (!(Math.Abs(DistanceBetweenAdjacentVertices - Vector3.Distance(Coords, vertex.Coords)) <
                  Tolerance)) return;
                    
            bool uniqueCoords = true;
            foreach(MeshVertex i in VertexNeighbors) {
                if (i.Coords == vertex.Coords)
                {
                    uniqueCoords = false;
                    break;
                }
            }

            if (uniqueCoords)
            {
                VertexNeighbors.Add(vertex);
            }
        }

        public static bool IsOnColonizedEdge(MeshVertex mv)
        {
            int countColonized = 0;
            foreach (FaceHandler face in mv.ParentFaces)
            {
                if (face.colonized)
                {
                    countColonized++;
                }
            } 
            //Debug.Log(ParentFaces.Count);
            if (countColonized == 0 || countColonized == mv.ParentFaces.Count)
            {
                return false;
            }
            
            return true;
        }

        public static bool IsOnDiscoveredEdge(MeshVertex mv)
        {
            bool isOnEdge = false;
            foreach (FaceHandler face in mv.ParentFaces)
            {
                if (face.colonized)
                {
                    return false;
                }
                if (face.discovered)
                {
                    isOnEdge = true;
                }
            }

            return isOnEdge;
        }
    }
}
