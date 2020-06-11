using UnityEngine;

namespace MiniGame.Creatures.DeathHandlers
{
    public class WumplingDeathHandler : MonoBehaviour, DeathHandler
    {
        private ResultHandler _resultHandler;
        private GameObject camera;
        // Start is called before the first frame update
        void Start()
        {
            camera = GameObject.FindWithTag("ResultHandler");
            _resultHandler = camera.GetComponent<ResultHandler>();
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
