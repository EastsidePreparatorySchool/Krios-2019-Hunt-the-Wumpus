using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class CursorController : MonoBehaviour
    {
        public Image img;
        //public Camera mc;

        private Transform _myTransform;
        
        // Start is called before the first frame update
        void Start()
        {
            //Cursor.visible = false;
            //img = GetComponent<Image>();
            _myTransform = transform;

        }

        // Update is called once per frame
        void Update()
        {
            /*Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = -12;
        transform.position = cursorPos;*/

            Vector3 mousePos = Input.mousePosition;
            // _myTransform.position = mousePos;
            mousePos.z = 12;
            if (Camera.main != null)
            {
                Vector3 newpos = Camera.main.ScreenToWorldPoint(mousePos);
                //newpos.x *= 3;
                //newpos.x += 19;
                //newpos.y *= 3;
                //ewpos.y += 2;
                Debug.Log(newpos);

                _myTransform.position = new Vector3(newpos.x, newpos.y, transform.position.z - 14);
            }
        }
    }
}
