using System.Collections;
using System.Collections.Generic;
using CommandView;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;
using Text = UnityEngine.UI.Text;
using System.Linq;

namespace Gui
{
    public class TroopSelection : MonoBehaviour
    {
        public GameObject troopSelector; // The menu

        public GameObject troopToggleBlueprint;
        public GameObject troopLabelBlueprint;

        public GameObject scrollViewContent;

        public GameObject sendAllButton;
        private bool _sendAllButtonState = true; //true for send all, false for send none

        //public GameObject sendNoneButton; the sendAllButton is a toggle now

        public GameObject troopSelectScrollObject;
        public GameObject sendToBattleBtn;
        public GameObject nukeBtn;
        public GameObject buildSensorBtn;

        public GameObject batEncounterAlertText;

        private GameObject _planet;
        private Planet _planetHandler;
        private GameMeta _gameMeta;

        private List<GameObject> _toggles = new List<GameObject>();

        private int _prevFaceNum;

        private float _alpha;

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
                // for (int i = 0; i < scrollViewContent.transform.childCount; i++)
                // {
                //     var child = scrollViewContent.transform.GetChild(i).gameObject;
                //     if (child != null)
                //         Destroy(child);
                // }
                // //foreach (GameObject toggle in _toggles)
                // //    Destroy(toggle);
                //
                // foreach (var troop in _gameMeta.availableTroops)
                //     _toggles.Add(CreateNewToggle(troop));
                ActivateTroopSelector(0, true);
                needsRefresh = false;
            }
        }

        public void ActivateTroopSelector(int faceNum, bool resetClose = false)
        {
            if (!resetClose)
            {
                bool activate = true;
                if (_prevFaceNum == faceNum)
                {
                    activate = !troopSelector.activeSelf;
                }
                else
                {
                    _prevFaceNum = faceNum;
                }

                troopSelector.SetActive(activate);
                ShowOnlyBuildSensorBtn(!activate);
            }
            else
            {
                troopSelector.SetActive(false);
            }


            if (troopSelector.activeSelf && !resetClose)
            {
                // print("Meta: " + _gameMeta + "; Troops: " + _gameMeta.availableTroops);
                if (_toggles.Count == 0)
                {
                    foreach (var troop in _gameMeta.availableTroops)
                        _toggles.Add(CreateNewToggle(troop));
                }

                if (!_gameMeta.availableTroops.Any())
                {
                    if (_gameMeta.exhaustedTroops.Any())
                        CreateLabel(_gameMeta.exhaustedTroops.Count + " Exhausted Troops");
                    else
                        CreateLabel("All your troops have died");
                }
            }
            else
            {
                ShowOnlyBuildSensorBtn(false);
                foreach (var toggle in _toggles)
                {
                    Destroy(toggle);
                }

                _toggles.Clear();
                _planetHandler.selectedFace = -1;
            }
        }

        //Is actually the Select/Deselect button
        public void SelectAll()
        {
            foreach (var toggle in _toggles)
            {
                Toggle toggle1 = toggle.gameObject.GetComponent<Toggle>();
                toggle1.isOn = _sendAllButtonState;
            }

            //I couldn't figure out how to set the name of the button as I was having issues with TextMeshPro kinda not existing.
            switch (_sendAllButtonState)
            {
                case true:
                    _sendAllButtonState = false;
                    break;
                case false:
                    _sendAllButtonState = true;
                    break;
            }
        }

        public void SendTroopsToBattle()
        {
            print("Sending selected troops to battle!");
            _planetHandler.faces[_planetHandler.GetFaceInBattle()].GetComponent<FaceHandler>().PlayMiniGame();
        }

        IEnumerator WaitAndNuke()
        {
            yield return new WaitForSeconds(0.4f);
            print("Nuking Tile");
            StartCoroutine(
                _planetHandler.faces[_planetHandler.GetFaceInBattle()].GetComponent<FaceHandler>().NukeTile());
        }

        public void NukeTile()
        {
            print("Waiting To Nuke Tile");
            StartCoroutine(WaitAndNuke());
        }

        public void AddSensorOnTile()
        {
            _planetHandler.faces[_prevFaceNum].GetComponent<FaceHandler>().AddSensorOnTile();
        }

        public void ShowOnlyBuildSensorBtn(bool show = true)
        {
            sendAllButton.SetActive(!show);
            //sendNoneButton.SetActive(!show);
            troopSelectScrollObject.SetActive(!show);
            sendToBattleBtn.SetActive(!show);
            nukeBtn.SetActive(!show);
            buildSensorBtn.SetActive(show);
        }

        public IEnumerator FlashBatsEncounterAlert()
        {
            batEncounterAlertText.SetActive(true);
            float nextTime = Time.time + 10f;
            yield return new WaitUntil(() => Time.time > nextTime);
            batEncounterAlertText.SetActive(false);
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
            toggle.isOn = troop.SendToBattle;

            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle, troop); });

            // Code for naming troops
            GameObject labelGameObject = toggle.transform.Find("Label").gameObject;
            Text label = labelGameObject.GetComponent<Text>();
            label.text = troop.Type + ": " + troop.Name;

            GameObject upgradeBar = newTroopToggle.gameObject.transform.Find("UpgradeBar/UpgradeLevel").gameObject;
            upgradeBar.GetComponent<RectTransform>().offsetMax =
                new Vector2(270 / _planetHandler.maxUpgrades * troop.UpgradeLvl - 270, 0);

            return newTroopToggle;
        }

        private void CreateLabel(string str)
        {
            GameObject labelGo =
                Instantiate(troopLabelBlueprint,
                    scrollViewContent.transform);

            Text label = labelGo.GetComponent<Text>();
            RectTransform rectTransform = labelGo.GetComponent<RectTransform>();
            label.text = str;
            rectTransform.Translate(-10000, 0, 0);
        }

        private void ToggleValueChanged(Toggle toggle, TroopMeta troop)
        {
            //print(("TOGGLE WORKS! isOn: " + toggle.isOn));
            troop.SendToBattle = toggle.isOn;
            //print(troop.sendToBattle + " should be the same as above");
        }
    }
}