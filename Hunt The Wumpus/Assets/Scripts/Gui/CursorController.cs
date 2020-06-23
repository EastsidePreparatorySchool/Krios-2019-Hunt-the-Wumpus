using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    //private Image img;
    public Camera mc;
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        //img = GetComponent<Image>();
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = mc.ScreenToWorldPoint(Input.mousePosition);
        //cursorPos.z = -100;
        transform.position = cursorPos;
    }
}
