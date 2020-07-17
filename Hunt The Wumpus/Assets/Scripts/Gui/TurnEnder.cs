using CommandView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gui
{
    public class TurnEnder : MonoBehaviour, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Planet _planet;
        private GameMeta _meta;
        public GameObject confirmPanel;
        private GameObject _cantEndPanel;

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
            _cantEndPanel = confirmPanel.transform.Find("CantEndPanel").gameObject;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseConfirm();
            }
        }

        public void EndTurnButton()
        {
            if (!_planet.confirmTurn)
                EndTurn();
            else
            {
                confirmPanel.SetActive(true);
                if (!_planet.didSomething)
                    CantEndTurn();
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
            if (_cantEndPanel != null)
                _cantEndPanel.SetActive(false);
        }

        public void EndTurn()
        {
            if (_planet.didSomething)
            {
                _planet.didSomething = false;
                GameObject canvasGo = GameObject.Find("Canvas");
                TroopSelection troopSelector = canvasGo.GetComponent<TroopSelection>();
                if (troopSelector != null)
                    troopSelector.ActivateTroopSelector(0, true);

                _meta.EndTurn();
            }
            else
                CantEndTurn();
        }

        private void CantEndTurn()
        {
            _cantEndPanel.SetActive(true);
        }
    }
}