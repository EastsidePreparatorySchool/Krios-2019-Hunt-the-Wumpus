using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommandView;

namespace Gui
{
    public class Store : MonoBehaviour
    {
        private GameObject _planet;
        private Planet _planetHandler;
        private GameMeta _gameMeta;

        // Start is called before the first frame update
        void Start()
        {
            _planet = GameObject.Find("Planet");
            _planetHandler = _planet.GetComponent<Planet>();
            _gameMeta = _planet.GetComponent<GameMeta>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        int troopDmgIncrease = 1;
        int troopDmgCost = 1;
        int NukeCost = 1;
        int SensorTowerCost = 1;

        public void BuyTroops()
        {
            print("buying troops");
        }

        public void BuyTroopDmg()
        {
            if (_gameMeta.money >= troopDmgCost)
            {
                print("Buyng Damage Upgrade");
                _gameMeta.money -= troopDmgCost;
                //_gameMeta.nukes++;
            }
            else
                print("Not enough money");
        }

        public void BuyNuke()
        {
            if (_gameMeta.money >= NukeCost)
            {
                print("Buyng Nuke");
                _gameMeta.money -= NukeCost;
                _gameMeta.nukes++;
            }
            else
                print("Not enough money");
        }

        public void BuySensorTower()
        {
            if (_gameMeta.money >= SensorTowerCost)
            {
                print("Buyng Sensor Tower");
                _gameMeta.money -= SensorTowerCost;
                //_gameMeta.sensor....++;
            }
            else
                print("Not enough money");
        }
    }
}