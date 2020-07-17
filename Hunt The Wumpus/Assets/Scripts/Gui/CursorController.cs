using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class CursorController : MonoBehaviour
    {
        //public Image img;
        
        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            //img = GetComponent<Image>();

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}
