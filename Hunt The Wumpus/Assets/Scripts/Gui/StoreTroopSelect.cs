using System;
using System.Collections.Generic;
using CommandView;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;
using Text = UnityEngine.UI.Text;

namespace Gui
{
    public class StoreTroopSelection : MonoBehaviour
    {
        public GameObject troopSelector;
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

        public void ActivateTroopSelector(bool resetClose = false)
        {
            if (troopSelector.activeSelf && !resetClose)
            {
                // print("Meta: " + _gameMeta + "; Troops: " + _gameMeta.availableTroops);
                if (_toggles.Count == 0)
                {
                    foreach (var troop in _gameMeta.availableTroops)
                    {
                        _toggles.Add(CreateNewToggle(troop));
                    }
                }
            }
            else
            {
                foreach (var toggle in _toggles)
                {
                    Destroy(toggle);
                }

                _toggles.Clear();
            }
        }
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
            //print(("TOGGLE WORKS! isOn: " + toggle.isOn));
            troop.sendToBattle = toggle.isOn;
            //print(troop.sendToBattle + " should be the same as above");
        }
    }
}