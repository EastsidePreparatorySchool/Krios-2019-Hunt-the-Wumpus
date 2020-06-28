using System.Collections;
using System.Collections.Generic;
using CommandView;
using Gui;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnEnder : MonoBehaviour, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Planet _planet;
    private GameMeta _meta;
    public GameObject ConfirmPanel;

    private bool mouseIsOver = false;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Close the Window on Deselect only if a click occurred outside this panel
        if (!mouseIsOver)
            CloseConfirm();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsOver = false;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void Start()
    {
        _planet = GameObject.Find("Planet").GetComponent<Planet>();
        _meta = GameObject.Find("Planet").GetComponent<GameMeta>();

        EventSystem.current.SetSelectedGameObject(ConfirmPanel);
    }

    void Update()
    {
        
    }

    public void EndTurnButton()
    {
        if (!_planet.ConfirmTurn)
            EndTurn();
        else
        {
            if (_planet.didSomething)
                ConfirmPanel.SetActive(true);
            else
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
        ConfirmPanel.SetActive(false);
    }

    public void EndTurn()
    {
        if (_planet.didSomething)
        {
            _planet.didSomething = false;
            GameObject CanvasGO = GameObject.Find("Canvas");
            TroopSelection troopSelector = CanvasGO.GetComponent<TroopSelection>();
            if (troopSelector != null)
                troopSelector.ActivateTroopSelector(0, true);

            _meta.EndTurn();
        }
        else
            CantEndTurn();
    }

    private void CantEndTurn()
    {

    }
}