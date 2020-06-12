using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommandView
{
    public class GameMeta : MonoBehaviour
    {
        private Planet _planetHandler;
        private FaceHandler[] _faceHandlers;

        public int turnsElapsed;
        public int money;
        public int nukes;

        public int totalFaces;

        public List<TroopMeta> troops;

        //public MiniGameResult miniGameResult;

        // Start is called before the first frame update
        void Start()
        {
            troops = new List<TroopMeta>();
            SetupForDebug();

            turnsElapsed = 1;
            _planetHandler = GetComponent<Planet>();
            totalFaces = _planetHandler.faces.Length;

            nukes = 3;

            _faceHandlers = new FaceHandler[_planetHandler.faces.Length];
            for (int i = 0; i < _planetHandler.faces.Length; i++)
            {
                _faceHandlers[i] =
                    _planetHandler.faces[i].GetComponent<FaceHandler>(); // Save computation time from repeated 
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public int NumColonizedFaces()
        {
            int colonizedFaces = 0;
            foreach (FaceHandler face in _faceHandlers)
            {
                if (face.colonized)
                {
                    colonizedFaces++;
                }
            }

            return colonizedFaces;
        }

        public int NumDiscoveredFaces()
        {
            int discoveredFaces = 0;
            foreach (FaceHandler face in _faceHandlers)
            {
                if (face.discovered)
                {
                    discoveredFaces++;
                }
            }

            return discoveredFaces;
        }

        public void SetupForDebug()
        {
            String[] names = new[]
            {
                "James", "Michael", "Robert", "John", "David", "William", "Richard", "Thomas", "Mark", "Charles",
                "Mary",
                "Linda", "Patricia", "Susan", "Deborah", "Barbara", "Debra", "Karen", "Nancy", "Donna"
            };
            for (int i = 0; i < names.Length; i++)
            {
                troops.Add(new TroopMeta(TroopType.Marine, names[i]));
            }

            print("We have " + troops.Count + " Troops!");
        }

        public void UpdateGameStateWithResult()
        {
            MiniGameResult result = _planetHandler.result;

            foreach (TroopMeta t in result.inGameTroops)
            {
                troops.Add(t);
            }

            FaceHandler inBattleFaceHandler =
                _planetHandler.faces[_planetHandler.GetFaceInBattle()].GetComponent<FaceHandler>();

            if (!inBattleFaceHandler.noMoney)
            {
                money += result.moneyCollected;
            }

            _planetHandler.SetFaceInBattle(-1);

            if (result.didWin)
            {
                inBattleFaceHandler.SetColonized();
            }
        }
    }
}