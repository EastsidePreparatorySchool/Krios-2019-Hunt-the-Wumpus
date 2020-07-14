using UnityEngine;

namespace MiniGame.Terrain
{
    public class PathTile : ITile
    {
        private const bool Walkable = true;

        public GameObject TileGameObject;
    
        public bool CanWalkOn()
        {
            return Walkable;
        }

        public PathTile(GameObject tileGameObject)
        {
            TileGameObject = tileGameObject;
        }
    
    }
}
