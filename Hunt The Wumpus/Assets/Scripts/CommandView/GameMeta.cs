using Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int nukesUsed;
        public int sensorTowers;
        public int sensorTowersUsed;

        public String[] firstNames = new[]
        {
            "James", "Michael", "Robert", "John", "David", "William", "Richard", "Thomas", "Mark", "Charles",
            "Mary", "Linda", "Patricia", "Susan", "Deborah", "Barbara", "Debra", "Karen", "Nancy", "Donna"
        };

        public String[] lastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzales", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin"
        };

        public int totalFaces;

        public List<TroopMeta> AvailableTroops;
        public List<TroopMeta> ExhaustedTroops;
        public List<TroopMeta> TroopsUsed = new List<TroopMeta>();

        public bool gameInPlay;

        public bool needToPlayIntro = true;
        //public MiniGameResult miniGameResult;

        // Start is called before the first frame update
        void Start()
        {
            AvailableTroops = new List<TroopMeta>();
            ExhaustedTroops = new List<TroopMeta>();
            SetupForDebug();

            turnsElapsed = 1;
            _planetHandler = GetComponent<Planet>();
            totalFaces = _planetHandler.faces.Length;

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
            for (int i = 0; i < 10; i++)
            {
                AvailableTroops.Add(new TroopMeta(TroopType.Marine, firstNames[i] + " " + lastNames[i]));
            }
        }

        public void UpdateGameStateWithResult()
        {
            MiniGameResult result = _planetHandler.result;

            foreach (TroopMeta t in result.InGameTroops)
            {
                ExhaustedTroops.Add(t);
            }

            FaceHandler inBattleFaceHandler =
                _planetHandler.faceHandlers[_planetHandler.GetFaceInBattle()];

            if (inBattleFaceHandler.heldTroops.Any() && result.DidWin)
            {
                foreach (TroopMeta heldTroop in inBattleFaceHandler.heldTroops)
                {
                    ExhaustedTroops.Add(heldTroop);
                }

                inBattleFaceHandler.heldTroops = new List<TroopMeta>();
            }

            money += result.MoneyCollected;

            _planetHandler.SetFaceInBattle(-1);

            if (result.DidWin)
            {
                StartCoroutine(SetColonizedDelayed(inBattleFaceHandler));
            }
        }

        private IEnumerator SetColonizedDelayed(FaceHandler faceHandler)
        {
            yield return new WaitForEndOfFrame();
            faceHandler.SetColonized(false, true);
        }

        public void EndTurn()
        {
            print("end turn");
            turnsElapsed++;
            foreach (TroopMeta troop in ExhaustedTroops)
            {
                AvailableTroops.Add(troop);
            }

            ExhaustedTroops.Clear();

            foreach (FaceHandler face in _faceHandlers)
            {
                if (face.colonized && !face.noMoney)
                {
                    money++;
                }
            }

            GameObject.Find("Canvas").GetComponent<TroopSelection>().ActivateTroopSelector(0, true);
            
            if (AvailableTroops.Count + ExhaustedTroops.Count + nukes == 0 &&
                money < 5 && !_planetHandler.didSomething)
            {
                _planetHandler.curGameStatus = GameStatus.RanOutOfResources;
            }
        }
    }
}