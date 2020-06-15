﻿using System;
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

        private GameObject _planet;
        private Planet _planetHandler;
        private GameMeta _gameMeta;

        private List<GameObject> _toggles = new List<GameObject>();

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
                _toggles.Add(CreateNewToggle(troop));
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
                foreach (GameObject t in _toggles)
                {
                    if (t.GetComponent<Toggle>().isOn)
                        t.GetComponent<Toggle>().isOn = false;
                }
                toggle.isOn = true;
            //troop.sendToBattle = toggle.isOn;
            }
        }
    }
}