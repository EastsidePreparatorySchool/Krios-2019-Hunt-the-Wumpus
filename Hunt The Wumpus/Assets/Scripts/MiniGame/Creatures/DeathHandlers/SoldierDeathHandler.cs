using CommandView;
using UnityEngine;

namespace MiniGame.Creatures.DeathHandlers
{
    public class SoldierDeathHandler :  MonoBehaviour, DeathHandler
    {
        private ResultHandler _resultHandler;
        private GameObject _camera;

        public TroopMeta troopMeta;
        
        // Start is called before the first frame update
        void Start()
        {
            _camera = GameObject.FindWithTag("ResultHandler");
            _resultHandler = _camera.GetComponent<ResultHandler>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        public void Die()
        {
            print("Troop Death");
            PointerController pointer = gameObject.GetComponent<PlayerController>().pointer;
            while (pointer != null)
            {
                pointer.followers--;
                pointer = pointer.next;
            }
            _resultHandler.RemoveTroop(troopMeta);
            if (_resultHandler.NumTroopsLeft() == 0)
            {
                _resultHandler.EndMiniGame(false);
            }
        }
    }
}
