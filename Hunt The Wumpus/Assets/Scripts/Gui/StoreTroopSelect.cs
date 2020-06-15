using System;
using System.Collections;
using System.Collections.Generic;
using CommandView;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;
using Text = UnityEngine.UI.Text;

namespace Gui
{
    public class StoreTroopSelect : MonoBehaviour
    {
        public GameObject troopSelector; // The menu

        public GameObject troopToggleBlueprint;

        public GameObject scrollViewContent;

        public Toggle oldValue;

        private GameObject _planet;
        private Planet _planetHandler;
        private GameMeta _gameMeta;

        public List<GameObject> toggles = new List<GameObject>();

        // Start is called before the first frame update
        private void Start()
        {
            _planet = GameObject.Find("Planet");
            _planetHandler = _planet.GetComponent<Planet>();
            _gameMeta = _planet.GetComponent<GameMeta>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ActivateStoreTroopSelector()
        {
            foreach (var troop in _gameMeta.availableTroops)
                toggles.Add(CreateNewToggle(troop));
        }

        
        // Turn on toggle
        // Create new Toggle for each soldier in 

        private GameObject CreateNewToggle(TroopMeta troop)
        {
            GameObject newTroopToggle =
                Instantiate(troopToggleBlueprint,
                    scrollViewContent.transform); //Resources.Load<GameObject>("Objects/TroopToggle");
            // newTroopToggle.transform.parent = scrollViewContent.transform;

            Toggle toggle = newTroopToggle.gameObject.GetComponent<Toggle>();
            toggle.isOn = troop.sendToBattle;

            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle, troop); });

            // Code for naming troops
            GameObject labelGameObject = toggle.transform.Find("Label").gameObject;
            Text label = labelGameObject.GetComponent<Text>();
            label.text = troop.type + ": " + troop.name;

            return newTroopToggle;
        }

        private void ToggleValueChanged(Toggle toggle, TroopMeta troop)
        {
            if (toggle.isOn)
            {
                if (oldValue == null)
                {
                    oldValue = toggle;
                    return;
                }
                else
                {
                    oldValue.isOn = false;
                    oldValue.troop.
                    oldValue = toggle;
                }
                troop
            }

        }

        public void UpgradeTroop()
        {
            foreach (GameObject toggleGO in toggles)
            {
                Toggle toggle = toggleGO.GetComponent<Toggle>();
                if (toggle.isOn)
                {
                    GameObject.Find("Canvas").GetComponent<Store>.UpgradeTroop(toggle);
                }
            }
        }
    }
}