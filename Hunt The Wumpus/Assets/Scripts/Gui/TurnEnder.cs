using System.Collections;
using System.Collections.Generic;
using CommandView;
using Gui;
using UnityEngine;

public class TurnEnder : MonoBehaviour
{
    // Start is called before the first frame update
    private GameMeta _meta;
    void Start()
    {
        _meta = GameObject.Find("Planet").GetComponent<GameMeta>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void EndTurn()
    {
        GameObject CanvasGO = GameObject.Find("Canvas");
        TroopSelection troopSelector = CanvasGO.GetComponent<TroopSelection>();
        if (troopSelector != null)
        {
            troopSelector.ActivateTroopSelector(0, true);
        }
        _meta.EndTurn();
    }
}
