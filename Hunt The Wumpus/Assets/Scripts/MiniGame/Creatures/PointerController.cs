using CommandView;
using UnityEngine;

namespace MiniGame.Creatures
{
    public class PointerController : MonoBehaviour
    {
        private Planet _planet;
        public int followers;
        public GameObject indicator;
        public GameObject attackKnob;
        public bool attackMove;
        public Sprite[] sprites;

        // Start is called before the first frame update
        void Start()
        {
            _planet = GameObject.Find("Planet").GetComponent<Planet>();
            indicator.GetComponent<SpriteRenderer>().sprite = sprites[PlayerPrefs.GetInt("Waypoint")];
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