using UnityEngine;

namespace MiniGame.Selection
{
    public class MovementTooltips : MonoBehaviour
    {
        public GameObject tooltip;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                tooltip.SetActive(!tooltip.activeSelf);
                if (tooltip.activeSelf)
                {
                    Time.timeScale = 0.2f;
                }
                else
                {
                    Time.timeScale = 1;
                }
            }
        }
    }
}