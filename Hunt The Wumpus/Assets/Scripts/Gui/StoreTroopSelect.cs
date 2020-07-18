using System.Collections.Generic;
using CommandView;
using UnityEngine;
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
        public TroopMeta checkedTroop;

        private GameObject _planet;
        private Planet _planetHandler;
        private GameMeta _gameMeta;

        public List<GameObject> toggles = new List<GameObject>();
        public List<GameObject> tmpToggles = new List<GameObject>();

        public bool needsRefresh;

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
            if (needsRefresh)
            {
                foreach (GameObject t in toggles)
                    Destroy(t);

                oldValue = null;

                foreach (TroopMeta troop in _gameMeta.availableTroops)
                {
                    if (troop == checkedTroop)
                        tmpToggles.Add(CreateNewToggle(troop, true));
                    else
                        tmpToggles.Add(CreateNewToggle(troop, false));
                }

                toggles = tmpToggles;
                needsRefresh = false;
            }
        }


        // Turn on toggle
        // Create new Toggle for each soldier in 

        private GameObject CreateNewToggle(TroopMeta troop, bool check)
        {
            GameObject newTroopToggle =
                Instantiate(troopToggleBlueprint,
                    scrollViewContent.transform); //Resources.Load<GameObject>("Objects/TroopToggle");
            // newTroopToggle.transform.parent = scrollViewContent.transform;

            Toggle toggle = newTroopToggle.gameObject.GetComponent<Toggle>();
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle, troop); });

            // Code for naming troops
            GameObject labelGameObject = toggle.transform.Find("Label").gameObject;
            Text label = labelGameObject.GetComponent<Text>();
            label.text = troop.Type + ": " + troop.Name;

            GameObject upgradeBar = newTroopToggle.gameObject.transform.Find("UpgradeBar/UpgradeLevel").gameObject;
            upgradeBar.GetComponent<RectTransform>().offsetMax =
                new Vector2(270 / _planetHandler.maxUpgrades * troop.UpgradeLvl - 270, 0);

            toggle.isOn = check;
            //roop.sendToBattle = check;

            return newTroopToggle;
        }

        private void ToggleValueChanged(Toggle toggle, TroopMeta troop)
        {
            if (toggle.isOn)
            {
                if (oldValue == null)
                {
                    checkedTroop = troop;
                    oldValue = toggle;
                }
                else
                {
                    oldValue.isOn = false;
                    checkedTroop = troop;
                    oldValue = toggle;
                }
            }
        }
    }
}