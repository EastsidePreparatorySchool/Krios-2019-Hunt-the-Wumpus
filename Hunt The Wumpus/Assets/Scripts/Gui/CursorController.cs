using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public Image img;
    //public Camera mc;
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        //img = GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = -12;
        transform.position = cursorPos;*/

        Vector3 mousePos = Input.mousePosition;
        transform.position = mousePos;
        mousePos.z = 12;
        Vector3 newpos = Camera.main.ScreenToWorldPoint(mousePos);
        //newpos.x *= 3;
       //newpos.x += 19;
        //newpos.y *= 3;
        //ewpos.y += 2;
        Debug.Log(newpos);

        transform.position = new Vector3(newpos.x, newpos.y, transform.position.z - 14);

    }
}
