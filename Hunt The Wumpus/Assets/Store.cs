using UnityEngine;
using CommandView;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements;
using Gui;

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
        GameObject TroopSelector = GameObject.Find("TroopSelectorUI");
        if(TroopSelector != null)
            TroopSelector.SetActive(false);

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
            GameObject.Find("Canvas").GetComponent<StoreTroopSelect>().ActivateStoreTroopSelector();
        }
    }

    public void UpgradeTroop()
    {
        TroopMeta checkedTroop = GameObject.Find("Canvas").GetComponent<StoreTroopSelect>().checkedTroop;
        if (checkedTroop != null)
        {
            if (useMoney(troopDamagCost))
            {
                checkedTroop.damage += troopDamagIncrease;
                print("upgraed by " + troopDamagIncrease + ", now up to " + checkedTroop.damage);
            }
        }
    }

    // This is used for managing money whenever something is bought
    private bool useMoney(int amount)
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
        GameObject BuyPanel = GameObject.Find("BuyTroopsPanel");
        GameObject inpparent = BuyPanel.transform.Find("InputField (TMP)").gameObject;
        GameObject dropparent = BuyPanel.transform.Find("TroopTypeDropdown").gameObject;

        // Get TMPro components
        TMP_InputField troopNumberInput = inpparent.GetComponent<TMP_InputField>();
        TMP_Dropdown troopDropType = dropparent.GetComponent<TMP_Dropdown>();

        print("attempting to buy " + troopNumberInput.text + " troops of type " + troopDropType.value);
        // e.g.  attempting to buy 2 troops of type 0

        //temp until new troops
        int totalCost = marineCost * int.Parse(troopNumberInput.text);
        if (useMoney(totalCost))
            for (int i = 0; i < int.Parse(troopNumberInput.text); i++)
                _gameMeta.availableTroops.Add(new TroopMeta(TroopType.Marine, _gameMeta.firstNames[Random.Range(0, _gameMeta.firstNames.Length)]));
    }

    // Work in progress
    public void BuyTroopDmg()
    {
        if (_gameMeta.money >= troopDamagCost)
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
        if (useMoney(NukeCost))
            _gameMeta.nukes++;
    }

    // Work in progress
    public void BuySensorTower()
    {
        if (useMoney(SensorTowerCost))
            _gameMeta.sensorTowers++;
    }
}
