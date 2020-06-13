using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommandView;
using TMPro;

namespace Gui
{
    public class Store : MonoBehaviour
    {
        public int marineCost;
        public int sniperCost;
        public int tankCost;
        public int defenderCost;

        public int troopDamagIncrease;
        public int troopDamagCost;
        public int NukeCost;
        public int SensorTowerCost;

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

        // For opening store UI
        public void Open()
        {
            // Closes TroopSelector if open
            GameObject TroopSelector = GameObject.Find("TroopSelectorUI");
            if(TroopSelector != null)
            {
                TroopSelector.active = false;
            }

            // Activates all children of the StoreUI object
            GameObject store = GameObject.Find("StoreUI");
            for (int i = 0; i < store.transform.childCount; i++)
            {
                var child = store.transform.GetChild(i).gameObject;
                if (child != null)
                    child.SetActive(true);
            }
        }


        // For closing store UI
        public void Close()
        {
            // De-activates all children of StoreUI object
            GameObject store = GameObject.Find("StoreUI");
            for (int i = 0; i < store.transform.childCount; i++)
            {
                var child = store.transform.GetChild(i).gameObject;
                if (child != null)
                    child.SetActive(false);
            }
        }


        // work in progress
        public void OpenUpgradePanel()
        {

        }


        // For buying troops (duh)
        public void BuyTroops()
        {
            GameObject BuyPanel = GameObject.Find("BuyTroopsPanel");
            GameObject inpparent = BuyPanel.transform.Find("InputField (TMP)").gameObject;
            GameObject dropparent = BuyPanel.transform.Find("TroopTypeDropdown").gameObject;

            // Get TMPro components
            TMP_InputField troopNumberInput = inpparent.GetComponent<TMP_InputField>();
            TMP_Dropdown troopDropType = dropparent.GetComponent<TMP_Dropdown>();

            print("attempting to buy " + troopNumberInput.text + " troops of type " + troopDropType.value);
            // e.g.  attempting to buy 2 troops of type 0

            //temp until new troops
            if (_gameMeta.money >= marineCost)
            {
                for (int i = 0; i < int.Parse(troopNumberInput.text); i++)
                {
                    _gameMeta.money -= marineCost;
                    _gameMeta.troops.Add(new TroopMeta(TroopType.Marine, _gameMeta.names[Random.Range(0, _gameMeta.names.Length)]));
                }
            }
        }

        // Work in progress
        public void BuyTroopDmg()
        {
            if (_gameMeta.money >= troopDamagCost)
            {
                print("Buyng Damage Upgrade");
                _gameMeta.money -= troopDamagCost;
                //_gameMeta.nukes++;
            }
            else
                print("Not enough money");
        }

        // Buy NUKES!!!
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

        // Work in progress
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