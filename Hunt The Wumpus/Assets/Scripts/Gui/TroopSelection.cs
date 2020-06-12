using System;
using System.Collections.Generic;
using CommandView;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

namespace Gui
{
    public class TroopSelection : MonoBehaviour
    {
        public GameObject troopSelector; // The menu

        public GameObject troopToggleBlueprint;

        public GameObject scrollViewContent;

        public GameObject sendAllButton;

        public GameObject sendNoneButton;

        private GameObject _planet;
        private Planet _planetHandler;
        private GameMeta _gameMeta;

        private List<GameObject> _toggles = new List<GameObject>();

        private String _troopSelectorName;
        private String _scrollViewContentName;

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

        public void ActivateTroopSelector()
        {
            troopSelector.SetActive(!troopSelector.activeSelf);

            if (troopSelector.activeSelf)
            {
                print("Meta: "+_gameMeta+"; Troops: "+_gameMeta.troops);
                foreach (var troop in _gameMeta.troops)
                {
                    _toggles.Add(CreateNewToggle(troop));
                }
            }
            else
            {
                foreach (var toggle in _toggles)
                {
                    Destroy(toggle);
                }

                _toggles = new List<GameObject>();
            }
        }

        public void SendTroopsToBattle()
        {
            print("Sending selected troops to battle!");
            _planetHandler.faces[_planetHandler.GetFaceInBattle()].GetComponent<FaceHandler>().PlayMiniGame();
        }

        public void NukeTile()
        {
            print("Nuking Tile!");
            _planetHandler.faces[_planetHandler.GetFaceInBattle()].GetComponent<FaceHandler>().NukeTile();
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

            return newTroopToggle;
        }

        private void ToggleValueChanged(Toggle toggle, TroopMeta troop)
        {
            print(("TOGGLE WORKS! isOn: " + toggle.isOn));
            troop.sendToBattle = toggle.isOn;
            print(troop.sendToBattle + " should be the same as above");
        }
    }
}