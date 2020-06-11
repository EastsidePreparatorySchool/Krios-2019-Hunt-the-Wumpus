using System.Collections.Generic;
using CommandView;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

namespace Gui
{
    public class TroopSelection : MonoBehaviour
    {
        public GameObject planet;

        public GameObject toggleTroopSelect; // Button that brings up the menu

        public GameObject troopSelector; // The menu

        public GameObject troopToggleBlueprint;

        public GameObject scrollViewContent;

        public GameObject sendAllButton;

        public GameObject sendNoneButton;

        private GameMeta _gameMeta;

        private List<GameObject> _toggles = new List<GameObject>();

        // Start is called before the first frame update
        void Awake()
        {
            _gameMeta = planet.GetComponent<GameMeta>();
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
            Planet planetHandler = planet.GetComponent<Planet>();
            planetHandler.faces[planetHandler.GetFaceInBattle()].GetComponent<FaceHandler>().PlayMiniGame();
        }

        // Turn on toggle
        // Create new Toggle for each soldier in 

        private GameObject CreateNewToggle(TroopMeta troop)
        {
            GameObject obj = Instantiate(troopToggleBlueprint, scrollViewContent.transform);
            Toggle toggle = obj.gameObject.GetComponent<Toggle>();
            toggle.isOn = troop.sendToBattle;

            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle, troop); });

            return obj;
        }

        private void ToggleValueChanged(Toggle toggle, TroopMeta troop)
        {
            print(("TOGGLE WORKS! isOn: " + toggle.isOn));
            troop.sendToBattle = toggle.isOn;
            print(troop.sendToBattle + " should be the same as above");
        }
    }
}