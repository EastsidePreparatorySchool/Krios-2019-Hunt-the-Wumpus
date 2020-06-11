using UnityEngine;

namespace MiniGame
{
    public class PointerSpawner : MonoBehaviour
    {
        public GameObject pointerPrefab;

        public Camera minigameCamera;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (minigameCamera != null)
                {
                    Ray ray = minigameCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        GameObject pointerInstance = Instantiate(pointerPrefab, hit.point, Quaternion.identity);
                        PointerController pointerController = pointerInstance.GetComponent<PointerController>(); //the pointerController script of the new instance
                        int followerCount = 0;
                        foreach (Selectable selectable in RTSSelection.Selectables)
                        {
                            if (selectable.IsSelected)
                            {
                                PlayerController playerController = selectable.gameObject.GetComponent<PlayerController>(); //playerController scrip of the soldier
                                if (playerController.pointer)
                                {
                                    playerController.pointer.followers--;
                                }

                                playerController.pointer = pointerController;
                                playerController.atPointer = false;
                                followerCount++;
                            }
                        }

                        pointerController.followers = followerCount;
                    }
                }
            }
        }
    }
}
