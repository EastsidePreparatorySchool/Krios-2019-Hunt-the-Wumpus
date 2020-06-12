using UnityEngine;

namespace MiniGame
{
    public class PointerController : MonoBehaviour
    {
        public int followers;
        public bool attackMove;
        public PointerController next;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (followers <= 0)
            {
                Object.Destroy(this.gameObject);
            }
        }
        
        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}
