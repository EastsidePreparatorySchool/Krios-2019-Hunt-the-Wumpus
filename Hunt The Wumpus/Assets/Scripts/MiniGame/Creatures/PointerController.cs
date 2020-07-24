using UnityEngine;

namespace MiniGame.Creatures
{
    public class PointerController : MonoBehaviour
    {
        public int followers;
        public GameObject AttackKnob;
        public bool attackMove;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (followers <= 0)
            {
                Destroy(gameObject);
            }
        }

        public Vector3 GetPosition()
        {
            Vector3 position = transform.position;
            return new Vector3(position.x, 0f, position.z);
        }
    }
}