using UnityEngine;

namespace MiniGame.Creatures.DeathHandlers
{
    public class WumplingDeathHandler : MonoBehaviour, IDeathHandler
    {
        // private ResultHandler _resultHandler;
        // private GameObject _camera;
        // Start is called before the first frame update
        void Start()
        {
            // _camera = GameObject.FindWithTag("ResultHandler");
            // _resultHandler = _camera.GetComponent<ResultHandler>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Die()
        {
            print("Wumpling death");
        }
    }
}