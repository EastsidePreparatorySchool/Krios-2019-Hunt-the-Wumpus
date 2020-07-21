using Gui;
using TMPro;
using UnityEngine;

namespace CommandView
{
    public class Store : MonoBehaviour
    {
        public int marineCost;
        public int sniperCost;
        public int tankCost;
        public int defenderCost;

        public int troopDamageIncrease;
        public int troopDamageCost;
        public int nukeCost;
        public int sensorTowerCost;

        public TroopMeta checkedTroop;

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
            GameObject troopSelector = GameObject.Find("TroopSelectorUI");
            if (troopSelector != null)
                troopSelector.SetActive(false);

            // Activates all children of the StoreUI object
            GameObject store = GameObject.Find("StoreUI");
            for (int i = 0; i < store.transform.childCount; i++)
            {
                var child = store.transform.GetChild(i).gameObject;
                if (child != null)
                    child.SetActive(true);
            }

            store.transform.Find("StoreTroopSelect").gameObject.SetActive(false);

            // Closes "Not Enough Coins" Text if still up
            GameObject notEnoghText = GameObject.Find("StoreUI/BackShading/NotEnough");
            if (notEnoghText != null)
                notEnoghText.SetActive(false);
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
        public void ToggleUpgradePanel()
        {
            GameObject troopSelect = GameObject.Find("StoreUI/StoreTroopSelect");
            if (troopSelect != null)
                troopSelect.SetActive(false);
            else
            {
                GameObject store = GameObject.Find("StoreUI");
                for (int i = 0; i < store.transform.childCount; i++)
                {
                    var child = store.transform.GetChild(i).gameObject;
                    if (child != null)
                        child.SetActive(true);
                }

                GameObject.Find("Canvas").GetComponent<StoreTroopSelect>().needsRefresh = true;
            }
        }

        public void UpgradeTroop()
        {
            TroopMeta storeCheckedTroop = GameObject.Find("Canvas").GetComponent<StoreTroopSelect>().checkedTroop;
            if (storeCheckedTroop != null)
            {
                if (storeCheckedTroop.UpgradeLvl < _planetHandler.maxUpgrades)
                {
                    if (UseMoney(troopDamageCost))
                    {
                        storeCheckedTroop.Damage += troopDamageIncrease;
                        storeCheckedTroop.UpgradeLvl += 1;
                        print("upgraed by " + troopDamageIncrease + ", now up to " + storeCheckedTroop.Damage + ", lvl: " +
                              storeCheckedTroop.UpgradeLvl);
                        GetComponent<StoreTroopSelect>().needsRefresh = true;
                        GetComponent<TroopSelection>().needsRefresh = true;
                    }
                }
                else
                {
                    print("troop fully upgraded");
                    //make thi more apparent
                }
            }
        }

        // This is used for managing money whenever something is bought
        private bool UseMoney(int amount)
        {
            if (_gameMeta.money >= amount)
            {
                // Remove "NotEnoghCoins" text if it's up
                GameObject notEnoghText = GameObject.Find("StoreUI/BackShading/NotEnough");
                if (notEnoghText != null)
                    notEnoghText.SetActive(false);

                print("Deducting " + amount + " coins");
                _gameMeta.money -= amount;
                return true;
            }
            else
            {
                print("Not enough money");
                // Make "NotEnoghCoins" text visible
                GameObject notEnoghTextParent = GameObject.Find("StoreUI/BackShading");
                notEnoghTextParent.transform.GetChild(0).gameObject.SetActive(true);
                return false;
            }
        }

        //For buying troops (duh)
        public void BuyTroops()
        {
            GameObject buyPanel = GameObject.Find("BuyTroopsPanel");
            GameObject inpparent = buyPanel.transform.Find("InputField (TMP)").gameObject;
            GameObject dropparent = buyPanel.transform.Find("TroopTypeDropdown").gameObject;

            // Get TMPro components
            TMP_InputField troopNumberInput = inpparent.GetComponent<TMP_InputField>();
            TMP_Dropdown troopDropType = dropparent.GetComponent<TMP_Dropdown>();

            print("attempting to buy " + troopNumberInput.text + " troops of type " + troopDropType.value);
            // e.g.  attempting to buy 2 troops of type 0

            //temp until new troops
            int totalCost = marineCost * int.Parse(troopNumberInput.text);
            if (UseMoney(totalCost))
                for (int i = 0; i < int.Parse(troopNumberInput.text); i++)
                    _gameMeta.availableTroops.Add(new TroopMeta(TroopType.Marine,
                        _gameMeta.firstNames[Random.Range(0, _gameMeta.firstNames.Length)] + " " +
                        _gameMeta.lastNames[Random.Range(0, _gameMeta.lastNames.Length)]));
        }

        // Work in progress
        public void BuyTroopDmg()
        {
            if (_gameMeta.money >= troopDamageCost)
            {
                print("Buyng Damage Upgrade");
                //_gameMeta.money -= troopDamagCost;
                //_gameMeta.nukes++;
            }
            else
                print("Not enough money");
        }

        // Buy NUKES!!!
        public void BuyNuke()
        {
            if (UseMoney(nukeCost))
                _gameMeta.nukes++;
        }

        // Work in progress
        public void BuySensorTower()
        {
            if (UseMoney(sensorTowerCost))
                _gameMeta.sensorTowers++;
        }
    }
}