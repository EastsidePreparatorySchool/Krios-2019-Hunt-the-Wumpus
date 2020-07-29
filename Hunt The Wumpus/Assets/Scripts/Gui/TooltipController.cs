using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{
    public bool Enabled;

    public Button button;
    public GameObject tooltip;

    public Vector2 offset;

    private Vector2 _mousePos;

    void Update()
    {
        if (Enabled)
        {
            if (!button.interactable)
            {
                _mousePos = Input.mousePosition;
                tooltip.transform.position = new Vector2(_mousePos.x + offset.x, _mousePos.y + offset.y);
            }
        }
    }
    
    public void MouseEnter()
    {
        if (!button.interactable)
            tooltip.SetActive(true);
    }

    public void MouseExit()
    {
        tooltip.SetActive(false);
    }
}
