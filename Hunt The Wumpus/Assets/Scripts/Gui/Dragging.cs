using System.Collections;
using CommandView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gui
 {
     public class Dragging : EventTrigger
     {
         // Jankyness TBD
         // https://dev.to/matthewodle/simple-ui-element-dragging-script-in-unity-c-450p

         public FaceHandler face;
         private bool _dragging;

         // Start is called before the first frame update
         void Awake()
         {
 
         }

         // Update is called once per frame
         void Update()
         {
             if (_dragging)
             {
                 transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
             }
         }

         public override void OnPointerDown(PointerEventData eventData)
         {
             if (Input.GetMouseButtonDown(0))
             {
                 _dragging = true;
             } else if (Input.GetMouseButtonDown(1))
             {
                 StartCoroutine(WaitUntilRightMouseUp());
             }
         }

         public override void OnPointerUp(PointerEventData eventData)
         {
             _dragging = false;
         }

         private IEnumerator WaitUntilRightMouseUp()
         {
             yield return new WaitUntil(() => Input.GetMouseButtonUp(1)); // Wait until right click is released 
             face.displayFaceData = false;
         }

     }
 }