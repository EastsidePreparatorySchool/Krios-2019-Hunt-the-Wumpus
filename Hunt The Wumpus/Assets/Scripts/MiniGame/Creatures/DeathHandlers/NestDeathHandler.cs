using System;
using UnityEngine;

namespace MiniGame.Creatures.DeathHandlers
{
    
    public class NestDeathHandler : MonoBehaviour, DeathHandler
    {
        private ResultHandler _resultHandler;

        private GameObject _camera;

        public bool endMinigame;
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
            _resultHandler.EarnMoney(1);
            if (endMinigame)
            {
                _resultHandler.EndMiniGame();
            }
        }

        public void OnDeathEndMiniGame()
        {
            // print("unity suxxx");
            endMinigame = true;
        }
    }
}
