using MiniGame.Selection;
using UnityEngine;

namespace MiniGame.Creatures
{
    public class PointerSpawner : MonoBehaviour
    {
        public GameObject pointerPrefab;
        public Camera minigameCamera;
        
        private static readonly Color MoveColor = Color.green;
        private static readonly Color AttackColor = Color.red;
        
        private static readonly KeyCode WaypointKey = KeyCode.LeftShift;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (minigameCamera == null)
            {
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = minigameCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject pointerInstance = Instantiate(pointerPrefab, hit.point, Quaternion.identity);
                    PointerController pointerController = pointerInstance.GetComponent<PointerController>(); //the pointerController script of the new instance
                    
                    CheckAttackCommand(pointerController, pointerInstance);
                    bool makeWaypoint = Input.GetKey(WaypointKey);
                    
                    int followerCount = 0;
                    foreach (Selectable selectable in RTSSelection.Selectables)
                    {
                        if (selectable.IsSelected)
                        {
                            PlayerController playerController = selectable.gameObject.GetComponent<PlayerController>(); //playerController scrip of the soldier

                            if (!makeWaypoint)
                            {
                                playerController.RemovePointers();
                            }
                            playerController.AddPointer(pointerController);
                            playerController.atPointer = false;
                            followerCount++;
                        }
                    }

                    pointerController.followers = followerCount;
                }
            }
        }

        private static void CheckAttackCommand(PointerController pointerController, GameObject pointerInstance)
        {
            if (Input.GetAxisRaw("Attack Command") > 0)
            {
                pointerController.attackMove = true;
                pointerInstance.GetComponent<Renderer>().material.color = AttackColor;
            }
            else
            {
                pointerController.attackMove = false;
                pointerInstance.GetComponent<Renderer>().material.color = MoveColor;
            }
        }
    }
}
