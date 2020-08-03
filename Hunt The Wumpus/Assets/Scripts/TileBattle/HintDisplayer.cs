using CommandView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TileBattle
{
    public class HintDisplayer : MonoBehaviour
    {
        public GameObject wumpusHint;

        public GameObject pitHint;

        public GameObject batHint;

        public GameObject noHazards;

        private Planet _planet;

        // Start is called before the first frame update
        void Start()
        {
            _planet = GameObject.Find("Planet").GetComponent<Planet>();

            bool[] hints = _planet.GetHintsToGive();
            if (hints[0])
            {
                wumpusHint.SetActive(true);
            }

            if (hints[1])
            {
                pitHint.SetActive(true);
            }

            if (hints[2])
            {
                batHint.SetActive(true);
            }

            if (!hints[0] && !hints[1] && !hints[2])
            {
                noHazards.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnClick()
        {
            print("Returning to CommandView");
            _planet.SetHintsToGive(new[] {false, false, false});
            SceneManager.LoadScene(1);
        }
    }
}