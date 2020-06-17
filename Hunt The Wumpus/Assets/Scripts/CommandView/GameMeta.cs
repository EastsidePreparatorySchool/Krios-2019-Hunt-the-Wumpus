using System;
using System.Collections;
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
        public int sensorTowers;

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

        public List<TroopMeta> availableTroops;
        public List<TroopMeta> exhaustedTroops;

        //public MiniGameResult miniGameResult;

        // Start is called before the first frame update
        void Start()
        {
            availableTroops = new List<TroopMeta>();
            exhaustedTroops = new List<TroopMeta>();
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
            for (int i = 0; i < firstNames.Length; i++)
            {
                availableTroops.Add(new TroopMeta(TroopType.Marine, firstNames[i] + " " + lastNames[i]));
            }

            print("We have " + availableTroops.Count + " Troops!");
        }

        public void UpdateGameStateWithResult()
        {
            MiniGameResult result = _planetHandler.result;
            
            foreach (TroopMeta t in result.inGameTroops)
            {
                exhaustedTroops.Add(t);
            }
            
            FaceHandler inBattleFaceHandler =
                _planetHandler.faceHandlers[_planetHandler.GetFaceInBattle()];

            foreach (TroopMeta heldTroop in inBattleFaceHandler.heldTroops)
            {
                exhaustedTroops.Add(heldTroop);
            }
            inBattleFaceHandler.heldTroops = new List<TroopMeta>();

            money += result.moneyCollected;

            _planetHandler.SetFaceInBattle(-1);

            if (result.didWin)
            {
                inBattleFaceHandler.SetColonized(false);
            }
        }

        public void EndTurn()
         {
             print("end turn");
             turnsElapsed++;
             foreach (TroopMeta troop in exhaustedTroops)
             {
                 availableTroops.Add(troop);
             }
             exhaustedTroops.Clear();
 
             foreach (FaceHandler face in _faceHandlers)
             {
                 if (face.colonized && !face.noMoney)
                 {
                     money++;
                 }
             }
         }
     }
 }