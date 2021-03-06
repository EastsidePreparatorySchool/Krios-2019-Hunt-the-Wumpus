﻿using CommandView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gui
{
    public class TurnEnder : MonoBehaviour, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Planet _planet;
        private GameMeta _meta;
        public Button endTurnBtn;
        public GameObject confirmPanel;
        public GameObject tooltip;

        private bool _mouseIsOver;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            //Close the Window on Deselect only if a click occurred outside this panel
            if (!_mouseIsOver)
                CloseConfirm();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseIsOver = true;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseIsOver = false;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        void Start()
        {
            _planet = GameObject.Find("Planet").GetComponent<Planet>();
            _meta = GameObject.Find("Planet").GetComponent<GameMeta>();

            EventSystem.current.SetSelectedGameObject(confirmPanel);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseConfirm();
            }

            if (_planet.didSomething != endTurnBtn.interactable)
            {
                endTurnBtn.interactable = _planet.didSomething;
            }
        }

        public void EndTurnButton()
        {
            bool confirm = PlayerPrefs.GetInt("ConfirmTurn") == 1 ? true : false;
            if (!confirm)
                EndTurn();
            else
            {
                confirmPanel.SetActive(true);
            }
        }

        public void YesConfirm()
        {
            EndTurn();
            CloseConfirm();
        }

        public void NoConfirm()
        {
            CloseConfirm();
        }

        private void CloseConfirm()
        {
            confirmPanel.SetActive(false);
        }

        public void EndTurn()
        {
            _planet.didSomething = false;
            GameObject canvasGo = GameObject.Find("Canvas");
            TroopSelection troopSelector = canvasGo.GetComponent<TroopSelection>();
            if (troopSelector != null)
                troopSelector.ActivateTroopSelector(0, true);

            _meta.EndTurn();
        }
    }
}